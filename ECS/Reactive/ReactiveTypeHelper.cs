using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using UnityEngine.Assertions;

namespace BovineLabs.Toolkit.Reactive
{
    public interface IReactiveAddRemoveGroup
    {
        ComponentType[] AddComponents { get; }
        ComponentType[] RemoveComponents { get; }
        JobHandle CreateAddJob(JobHandle inputDeps, EntityArray entities, EntityCommandBuffer commandBuffer);
        JobHandle CreateRemoveJob(JobHandle inputDeps, EntityArray entities, EntityCommandBuffer commandBuffer);
    }
    
    public interface IReactiveUpdateGroup
    {
        ComponentType[] Components { get; }
        JobHandle CreateAddJob(JobHandle inputDeps, EntityArray entities, EntityCommandBuffer commandBuffer);
        JobHandle CreateRemoveJob(JobHandle inputDeps, EntityArray entities, EntityCommandBuffer commandBuffer);
        void CreateWorldSystemsIfRequired(World world);
    }
    
    public class ReactiveBarrierSystem : BarrierSystem
    {
            
    }
    
    public static class ReactiveTypeHelper
    {
        private static readonly List<ComponentType> ComponentTypes = new List<ComponentType>();
        private static ModuleBuilder _moduleBuilder;
        private static readonly Dictionary<Tuple<ComponentType[], ComponentType[]>, IReactiveAddRemoveGroup>
            _reactiveGroups = new Dictionary<Tuple<ComponentType[], ComponentType[]>, IReactiveAddRemoveGroup>(new KeyCompare());

        private static readonly Dictionary<Tuple<ComponentType[], ComponentType[]>, IReactiveUpdateGroup>
            _reactiveUpdates = new Dictionary<Tuple<ComponentType[], ComponentType[]>, IReactiveUpdateGroup>();

        private static ModuleBuilder ModuleBuilder
        {
            get
            {
                if (_moduleBuilder == null)
                {
                    var assemblyName = new AssemblyName("BovineLabsReactive");
                    var assemblyBuilder = AppDomain.CurrentDomain
                        .DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.RunAndSave);
                    _moduleBuilder =
                        assemblyBuilder.DefineDynamicModule(assemblyName.Name, assemblyName.Name + ".dll");
                }

                return _moduleBuilder;
            }
        }
        
        public static IReactiveAddRemoveGroup GetReactiveAddRemoveGroup(ComponentType[] componentTypes,
            ComponentType[] conditionTypes)
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
            {
                reactiveGroup = _reactiveGroups[key] = CreateReactiveAddRemoveGroup(componentTypes, conditionTypes);
            }

            return reactiveGroup;
        }


        public static IReactiveUpdateGroup GetReactiveUpdateGroup(World world, IReactiveAddRemoveGroup addRemoveGroup)
        {
            Assert.AreEqual(2, addRemoveGroup.AddComponents.Length);

            var componentType = addRemoveGroup.AddComponents[0];
            Assert.AreNotEqual(ComponentType.AccessMode.Subtractive, componentType.AccessModeType);
            
            var componentTypes = new[] {componentType};
            var conditionTypes = new ComponentType[0];

            var key = Tuple.Create(componentTypes, conditionTypes);

            if (!_reactiveUpdates.TryGetValue(key, out var reactiveGroup))
                reactiveGroup = _reactiveUpdates[key] = CreateReactiveUpdateGroup(componentType);

            reactiveGroup.CreateWorldSystemsIfRequired(world);
            return reactiveGroup;
        }


        private static IReactiveAddRemoveGroup CreateReactiveAddRemoveGroup(
            IReadOnlyList<ComponentType> componentTypes,
            ComponentType[] conditionTypes)
        {
            var reactiveComponentStateType = CreateReactiveTypeState(componentTypes);

            ComponentTypes.AddRange(componentTypes);
            ComponentTypes.AddRange(conditionTypes);
            ComponentTypes.Add(ComponentType.Subtractive(reactiveComponentStateType));

            var addComponents = ComponentTypes.ToArray();

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

            var removeComponents = ComponentTypes.ToArray();
            
            ComponentTypes.Clear();
            
            var reactiveComponentState = Activator.CreateInstance(reactiveComponentStateType);
            var makeme = typeof(ReactiveGroup<>).MakeGenericType(reactiveComponentStateType);
            return (IReactiveAddRemoveGroup) Activator.CreateInstance(makeme, reactiveComponentState, addComponents,
                removeComponents);
        }

        private static IReactiveUpdateGroup CreateReactiveUpdateGroup(ComponentType componentType)
        {
            var reactiveCompareType = typeof(ReactiveCompare<>).MakeGenericType(componentType.GetManagedType());
            var reactiveDirtyType = CreateReactiveUpdateTypeState(new[] {componentType});

            ComponentType[] components = {componentType, ComponentType.ReadOnly(reactiveDirtyType)};

            var makeme = typeof(ReactiveUpdateGroup<,,>).MakeGenericType(componentType.GetManagedType(),
                reactiveCompareType, reactiveDirtyType);
            return (IReactiveUpdateGroup) Activator.CreateInstance(makeme, components);
        }

        private static Type CreateReactiveTypeState(IEnumerable<ComponentType> componentTypes)
        {
            var name = string.Join("_", componentTypes);
            var typeName = $"{typeof(ReactiveTypeHelper).Name}_{name}";
            var typeBuilder = ModuleBuilder.DefineType(typeName, TypeAttributes.Public, typeof(ValueType));
            typeBuilder.AddInterfaceImplementation(typeof(ISystemStateComponentData));
            return typeBuilder.CreateType();
        }

        private static Type CreateReactiveUpdateTypeState(IEnumerable<ComponentType> componentTypes)
        {
            var name = string.Join("_", componentTypes);
            var typeName = $"{typeof(ReactiveTypeHelper)}_{name}__Dirty";
            var typeBuilder = ModuleBuilder.DefineType(typeName, TypeAttributes.Public, typeof(ValueType));
            typeBuilder.AddInterfaceImplementation(typeof(IComponentData));
            return typeBuilder.CreateType();
        }

        private class ReactiveGroup<T> : IReactiveAddRemoveGroup where T : struct, ISystemStateComponentData
        {
            private readonly T _stateComponent;
            
            public ReactiveGroup(T stateComponent, ComponentType[] addComponents, ComponentType[] removeComponents)
            {
                _stateComponent = stateComponent;
                AddComponents = addComponents;
                RemoveComponents = removeComponents;
            }

            public ComponentType[] AddComponents { get; }
            public ComponentType[] RemoveComponents { get; }

            public JobHandle CreateAddJob(JobHandle inputDeps, EntityArray entities, EntityCommandBuffer commandBuffer)
            {
                return new AddJob
                {
                    CommandBuffer = commandBuffer,
                    ComponentData = _stateComponent,
                    Entities = entities
                }.Schedule(entities.Length, 64, inputDeps);
            }

            public JobHandle CreateRemoveJob(JobHandle inputDeps, EntityArray entities, EntityCommandBuffer commandBuffer)
            {
                return new RemoveJob
                {
                    CommandBuffer = commandBuffer,
                    Entities = entities
                }.Schedule(entities.Length, 64, inputDeps);
            }

            private struct AddJob : IJobParallelFor
            {
                [ReadOnly] public EntityArray Entities;
                [ReadOnly] public T ComponentData;
                public EntityCommandBuffer.Concurrent CommandBuffer;

                public void Execute(int index)
                {
                    CommandBuffer.AddComponent(Entities[index], ComponentData);
                }
            }


            private struct RemoveJob : IJobParallelFor
            {
                [ReadOnly] public EntityArray Entities;
                public EntityCommandBuffer.Concurrent CommandBuffer;

                public void Execute(int index)
                {
                    CommandBuffer.RemoveComponent<T>(Entities[index]);
                }
            }
        }



        private class ReactiveUpdateGroup<T, TC, TD> : IReactiveUpdateGroup
            where T : struct, IComponentData
            where TC : struct, IReactiveCompare<T>, IComponentData
            where TD : struct, IComponentData
        {
            private readonly HashSet<World> _worlds = new HashSet<World>();

            public ReactiveUpdateGroup(ComponentType[] components)
            {
                Components = components;
            }

            public ComponentType[] Components { get; }

            public JobHandle CreateAddJob(JobHandle inputDeps,
                EntityArray entities, EntityCommandBuffer commandBuffer)
            {
                return new AddComponentJob
                {
                    CommandBuffer = commandBuffer,
                    ComponentData = new TC(),
                    Entities = entities
                }.Schedule(entities.Length, 64, inputDeps);
            }

            public JobHandle CreateRemoveJob(JobHandle inputDeps,
                EntityArray entities, EntityCommandBuffer commandBuffer)
            {
                return new RemoveComponentJob
                {
                    CommandBuffer = commandBuffer,
                    Entities = entities
                }.Schedule(entities.Length, 64, inputDeps);
            }

            private struct AddComponentJob : IJobParallelFor
            {
                [ReadOnly] public EntityArray Entities;
                [ReadOnly] public TC ComponentData;
                public EntityCommandBuffer.Concurrent CommandBuffer;

                public void Execute(int index)
                {
                    CommandBuffer.AddComponent(Entities[index], ComponentData);
                }
            }


            private struct RemoveComponentJob : IJobParallelFor
            {
                [ReadOnly] public EntityArray Entities;
                public EntityCommandBuffer.Concurrent CommandBuffer;

                public void Execute(int index)
                {
                    CommandBuffer.RemoveComponent<TC>(Entities[index]);
                }
            }

            public void CreateWorldSystemsIfRequired(World world)
            {
                if (_worlds.Contains(world))
                    return;

                _worlds.Add(world);

                
                var reactiveCompareSystem =
                    typeof(ReactiveCompareSystem<,,>).MakeGenericType(typeof(T), typeof(TC), typeof(TD));
                world.CreateManager(reactiveCompareSystem);

                var removeReactiveSystem = typeof(RemoveReactiveSystem<>).MakeGenericType(typeof(TD));
                world.CreateManager(removeReactiveSystem);
            }
        }

        public class KeyCompare : IEqualityComparer<Tuple<ComponentType[], ComponentType[]>>
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