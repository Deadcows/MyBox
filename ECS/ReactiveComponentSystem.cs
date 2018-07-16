using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using Unity.Entities;

public abstract class ReactiveComponentSystem : ComponentSystem
{
	private static readonly List<ComponentType> _componentTypes = new List<ComponentType>();
	private static ModuleBuilder _moduleBuilder;

	private readonly Dictionary<ComponentType[], IReactiveGroup> _reactiveGroups =
		new Dictionary<ComponentType[], IReactiveGroup>(new ArrayCompare());

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
		if (componentTypes == null)
			throw new ArgumentNullException(nameof(componentTypes));

		if (componentTypes.Length == 0)
			throw new ArgumentException("Need to have at least 1 component", nameof(componentTypes));

		if (!_reactiveGroups.TryGetValue(componentTypes, out var reactiveGroup))
			reactiveGroup = _reactiveGroups[componentTypes] = CreateReactiveGroup(componentTypes);

		return reactiveGroup.AddGroup;
	}

	protected ComponentGroup GetReactiveRemoveGroup(params ComponentType[] componentType)
	{
		if (componentType == null)
			throw new ArgumentNullException(nameof(componentType));

		if (componentType.Length == 0)
			throw new ArgumentException("Need to have at least 1 component", nameof(componentType));

		if (!_reactiveGroups.TryGetValue(componentType, out var reactiveGroup))
			reactiveGroup = _reactiveGroups[componentType] = CreateReactiveGroup(componentType);

		return reactiveGroup.RemoveGroup;
	}

	private IReactiveGroup CreateReactiveGroup(ComponentType[] componentTypes)
	{
		var reactiveComponentStateType = CreateReactiveTypeState(componentTypes);

		_componentTypes.AddRange(componentTypes);
		_componentTypes.Add(ComponentType.Subtractive(reactiveComponentStateType));

		var addGroup = GetComponentGroup(_componentTypes.ToArray());

		_componentTypes.Clear();

		// Invert our component access for the remove group
		for (var index = 0; index < componentTypes.Length; index++)
		{
			var c = componentTypes[index];
			c.AccessModeType = c.AccessModeType == ComponentType.AccessMode.Subtractive
				? ComponentType.AccessMode.ReadWrite
				: ComponentType.AccessMode.Subtractive;
			_componentTypes.Add(c);
		}

		_componentTypes.Add(ComponentType.ReadOnly(reactiveComponentStateType));

		var removeGroup = GetComponentGroup(_componentTypes.ToArray());

		_componentTypes.Clear();

		var reactiveComponentState = Activator.CreateInstance(reactiveComponentStateType);
		var makeme = typeof(ReactiveGroup<>).MakeGenericType(reactiveComponentStateType);
		return (IReactiveGroup) Activator.CreateInstance(makeme, reactiveComponentState, addGroup, removeGroup);
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

			var addGroupEntities = group.AddGroup.GetEntityArray();
			for (var index = 0; index < addGroupEntities.Length; index++)
				group.AddComponent(PostUpdateCommands, addGroupEntities[index]);

			var removeGroupEntities = group.RemoveGroup.GetEntityArray();
			for (var index = 0; index < removeGroupEntities.Length; index++)
				group.RemoveComponent(PostUpdateCommands, removeGroupEntities[index]);
		}
	}

	protected abstract void OnReactiveUpdate();

	private interface IReactiveGroup
	{
		ComponentGroup AddGroup { get; }
		ComponentGroup RemoveGroup { get; }
		void AddComponent(EntityCommandBuffer entityCommandBuffer, Entity entity);
		void RemoveComponent(EntityCommandBuffer entityCommandBuffer, Entity entity);
	}

	private class ReactiveGroup<T> : IReactiveGroup where T : struct, ISystemStateComponentData
	{
		private readonly T _stateComponent;

		public ReactiveGroup(T stateComponent, ComponentGroup addGroup, ComponentGroup removeGroup)
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

	private class ArrayCompare : IEqualityComparer<ComponentType[]>
	{
		public bool Equals(ComponentType[] a, ComponentType[] b)
		{
			if (a == null && b == null)
				return true;
			if (a == null || b == null)
				return false;

			return a.SequenceEqual(b);
		}

		public int GetHashCode(ComponentType[] a)
		{
			return a.Aggregate(0, (acc, i) => unchecked(acc * 457 + i.GetHashCode() * 389));
		}
	}
}