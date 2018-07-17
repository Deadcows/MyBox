using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using Unity.Entities;

namespace BovineLabs.Toolkit.Reactive
{
    public abstract class ReactiveComponentSystem : ComponentSystem
    {
        private static readonly List<ComponentType> ComponentTypes = new List<ComponentType>();
        private static ModuleBuilder _moduleBuilder;

        private readonly Dictionary<Tuple<ComponentType[], ComponentType[]>, IReactiveAddRemoveGroup> _reactiveGroups =
            new Dictionary<Tuple<ComponentType[], ComponentType[]>, IReactiveAddRemoveGroup>(new KeyCompare());

        private readonly Dictionary<Tuple<ComponentType[], ComponentType[]>, IReactiveUpdateGroup> _reactiveUpdates =
            new Dictionary<Tuple<ComponentType[], ComponentType[]>, IReactiveUpdateGroup>();

        private static ModuleBuilder ModuleBuilder
        {
            get
            {
                if (_moduleBuilder == null)
                {
                    var assemblyName = new AssemblyName("BovineLabsReactive");
                    var assemblyBuilder = AppDomain.CurrentDomain
                        .DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.RunAndSave);
                    _moduleBuilder = assemblyBuilder.DefineDynamicModule(assemblyName.Name, assemblyName.Name + ".dll");
                }

                return _moduleBuilder;
            }
        }

        protected ComponentGroup GetReactiveAddGroup(params ComponentType[] componentTypes)
        {
            return GetReactiveAddGroup(componentTypes, new ComponentType[0]);
        }

        protected ComponentGroup GetReactiveRemoveGroup(params ComponentType[] componentTypes)
        {
            return GetReactiveRemoveGroup(componentTypes, new ComponentType[0]);
        }

        protected ComponentGroup GetReactiveAddGroup(ComponentType[] componentTypes, ComponentType[] conditionTypes)
        {
            return GetReactiveAddRemoveGroup(componentTypes, conditionTypes).AddGroup;
        }

        protected ComponentGroup GetReactiveRemoveGroup(ComponentType[] componentTypes, ComponentType[] conditionTypes)
        {
            return GetReactiveAddRemoveGroup(componentTypes, conditionTypes).RemoveGroup;
        }

        protected ComponentGroup GetReactiveUpdateGroup(ComponentType componentType)
        {
            if (componentType == null)
                throw new ArgumentNullException(nameof(componentType));

            var componentTypes = new[] {componentType};
            var conditionTypes = new ComponentType[0];
            
            var key = Tuple.Create(componentTypes, conditionTypes);

            if (!_reactiveUpdates.TryGetValue(key, out var reactiveGroup))
            {
                GetReactiveAddRemoveGroup(componentTypes, conditionTypes); // we depend on having an add/remove group
                reactiveGroup = _reactiveUpdates[key] = CreateReactiveUpdateGroup(componentType);
            }

            return reactiveGroup.Group;
        }

        private IReactiveAddRemoveGroup GetReactiveAddRemoveGroup(ComponentType[] componentTypes, ComponentType[] conditionTypes)
        {
            if (componentTypes == null)
                throw new ArgumentNullException(nameof(componentTypes));

            if (conditionTypes == null)
                throw new ArgumentNullException(nameof(conditionTypes));

            if (componentTypes.Length == 0)
                throw new ArgumentException("Need to have at least 1 component", nameof(componentTypes));

            for (var i = 0; i < componentTypes.Length; i++)
                if (componentTypes[i].AccessModeType == ComponentType.AccessMode.Subtractive)
                    throw new ArgumentException("Can not use subtractive components in reactive system");

            var key = Tuple.Create(componentTypes, conditionTypes);

            if (!_reactiveGroups.TryGetValue(key, out var reactiveGroup))
                reactiveGroup = _reactiveGroups[key] = CreateReactiveAddRemoveGroup(componentTypes, conditionTypes);

            return reactiveGroup;
        }

        private IReactiveAddRemoveGroup CreateReactiveAddRemoveGroup(IReadOnlyList<ComponentType> componentTypes,
            ComponentType[] conditionTypes)
        {
            var reactiveComponentStateType = CreateReactiveTypeState(componentTypes);

            ComponentTypes.AddRange(componentTypes);
            ComponentTypes.AddRange(conditionTypes);
            ComponentTypes.Add(ComponentType.Subtractive(reactiveComponentStateType));

            var addGroup = GetComponentGroup(ComponentTypes.ToArray());

            ComponentTypes.Clear();

            // Invert our component access for the remove group
            for (var index = 0; index < componentTypes.Count; index++)
            {
                var c = componentTypes[index];
                // Component mode can only be read or readwrite so inverting always makes it subtractive
                c.AccessModeType = c.AccessModeType = ComponentType.AccessMode.Subtractive;
                ComponentTypes.Add(c);
            }

            ComponentTypes.Add(ComponentType.ReadOnly(reactiveComponentStateType));

            var removeGroup = GetComponentGroup(ComponentTypes.ToArray());

            ComponentTypes.Clear();

            var reactiveComponentState = Activator.CreateInstance(reactiveComponentStateType);
            var makeme = typeof(ReactiveAddRemoveGroup<>).MakeGenericType(reactiveComponentStateType);
            return (IReactiveAddRemoveGroup) Activator.CreateInstance(makeme, reactiveComponentState, addGroup, removeGroup);
        }

        private IReactiveUpdateGroup CreateReactiveUpdateGroup(ComponentType componentType)
        {
            var group = GetComponentGroup(componentType, ComponentType.ReadOnly<ReactiveChanged>());
            
            var reactiveCompareType = typeof(ReactiveCompare<>).MakeGenericType(componentType.GetManagedType());
            
            var reactiveCompareSystem = typeof(ReactiveCompareSystem<,>).MakeGenericType(componentType.GetManagedType(), reactiveCompareType);
            World.CreateManager(reactiveCompareSystem);
            
            var makeme = typeof(ReactiveAddRemoveGroup<,>).MakeGenericType(componentType.GetManagedType(), reactiveCompareType);
            return (IReactiveUpdateGroup) Activator.CreateInstance(makeme, group);
        }

        private Type CreateReactiveTypeState(IEnumerable<ComponentType> componentTypes)
        {
            var name = string.Join("_", componentTypes);
            var typeName = $"{GetType().Name}_{name}";
            var typeBuilder = ModuleBuilder.DefineType(typeName, TypeAttributes.Public, typeof(ValueType));
            typeBuilder.AddInterfaceImplementation(typeof(ISystemStateComponentData));
            return typeBuilder.CreateType();
        }

        protected sealed override void OnUpdate()
        {
            OnReactiveUpdate();

            foreach (var kvp in _reactiveGroups)
            {
                var group = kvp.Value;

                var isUpdateGroup = _reactiveUpdates.TryGetValue(kvp.Key, out var updateGroup);
                
                var addGroupEntities = group.AddGroup.GetEntityArray();
                for (var index = 0; index < addGroupEntities.Length; index++)
                {
                    group.AddComponent(PostUpdateCommands, addGroupEntities[index]);
                    if (isUpdateGroup)
                        updateGroup.AddComponent(PostUpdateCommands, addGroupEntities[index]);
                }

                var removeGroupEntities = group.RemoveGroup.GetEntityArray();
                for (var index = 0; index < removeGroupEntities.Length; index++)
                {
                    group.RemoveComponent(PostUpdateCommands, removeGroupEntities[index]);
                    if (isUpdateGroup)
                        updateGroup.RemoveComponent(PostUpdateCommands, addGroupEntities[index]); // will these cause issues on deleted entities?
                }
            }

            foreach (var kvp in _reactiveUpdates)
            {
                var entities = kvp.Value.Group.GetEntityArray();
                for (var index = 0; index < entities.Length; index++)
                {
                    PostUpdateCommands.RemoveComponent<ReactiveChanged>(entities[index]);
                }
            }
        }

        protected abstract void OnReactiveUpdate();

        private interface IReactiveAddRemoveGroup
        {
            ComponentGroup AddGroup { get; }
            ComponentGroup RemoveGroup { get; }
            void AddComponent(EntityCommandBuffer entityCommandBuffer, Entity entity);
            void RemoveComponent(EntityCommandBuffer entityCommandBuffer, Entity entity);
        }

        private class ReactiveAddRemoveGroup<T> : IReactiveAddRemoveGroup where T : struct, ISystemStateComponentData
        {
            private readonly T _stateComponent;

            public ReactiveAddRemoveGroup(T stateComponent, ComponentGroup addGroup, ComponentGroup removeGroup)
            {
                _stateComponent = stateComponent;
                AddGroup = addGroup;
                RemoveGroup = removeGroup;
            }

            public ComponentGroup AddGroup { get; }
            public ComponentGroup RemoveGroup { get; }

            public void AddComponent(EntityCommandBuffer entityCommandBuffer, Entity entity)
            {
                entityCommandBuffer.AddComponent(entity, _stateComponent);
            }

            public void RemoveComponent(EntityCommandBuffer entityCommandBuffer, Entity entity)
            {
                entityCommandBuffer.RemoveComponent<T>(entity);
            }
        }

        private interface IReactiveUpdateGroup
        {
            ComponentGroup Group { get; }
            void AddComponent(EntityCommandBuffer entityCommandBuffer, Entity entity);
            void RemoveComponent(EntityCommandBuffer entityCommandBuffer, Entity entity);
        }

        private class ReactiveAddRemoveGroup<T, TC> : IReactiveUpdateGroup
            where T : struct, IComponentData
            where TC : struct, IReactiveCompare<T>, IComponentData
        {
            public ReactiveAddRemoveGroup(ComponentGroup group)
            {
                Group = group;
            }

            public ComponentGroup Group { get; }

            public void AddComponent(EntityCommandBuffer entityCommandBuffer, Entity entity)
            {
                entityCommandBuffer.AddComponent(entity, new TC());
            }

            public void RemoveComponent(EntityCommandBuffer entityCommandBuffer, Entity entity)
            {
                entityCommandBuffer.RemoveComponent<TC>(entity);
            }
        }

        private class KeyCompare : IEqualityComparer<Tuple<ComponentType[], ComponentType[]>>
        {
            private readonly ComponentComparer _componentComparer = new ComponentComparer();

            public bool Equals(Tuple<ComponentType[], ComponentType[]> x, Tuple<ComponentType[], ComponentType[]> y)
            {
                return x.Item1.SequenceEqual(y.Item1, _componentComparer) &&
                       x.Item2.SequenceEqual(y.Item2, _componentComparer);
            }

            public int GetHashCode(Tuple<ComponentType[], ComponentType[]> obj)
            {
                var x = obj.Item1.Aggregate(0, (acc, i) => unchecked(acc * 457 + i.GetHashCode() * 389));
                var y = obj.Item2.Aggregate(0, (acc, i) => unchecked(acc * 457 + i.GetHashCode() * 389));
                return x * 257 + y;
            }

            private class ComponentComparer : IEqualityComparer<ComponentType>
            {
                public bool Equals(ComponentType lhs, ComponentType rhs)
                {
                    return lhs.TypeIndex == rhs.TypeIndex && lhs.FixedArrayLength == rhs.FixedArrayLength &&
                           lhs.AccessModeType == rhs.AccessModeType;
                }

                public int GetHashCode(ComponentType obj)
                {
                    return (obj.TypeIndex * 5813) ^ obj.FixedArrayLength;
                }
            }
        }
    }
}