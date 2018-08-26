using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
using UnityEngine.Experimental.PlayerLoop;


public struct Event<T> where T : struct, IComponentData
{
	[Unity.Collections.ReadOnly] public ComponentDataArray<T> _event;
		
	public bool Fired => _event.Length > 0;
	public T Data => _event[0];
}
	
public struct SharedDataEvent<T> where T : struct, ISharedComponentData
{
	[Unity.Collections.ReadOnly] public SharedComponentDataArray<T> _event;

	public bool Fired => _event.Length > 0;
	public T Data => _event[0];
}

[UpdateInGroup(typeof(PreUpdate))]
public class EventsSystem : ComponentSystem
{

	private readonly Dictionary<ComponentType, (ComponentGroup Group, EntityArchetype Archetype)> _registeredEvents =
		new Dictionary<ComponentType, (ComponentGroup Group, EntityArchetype Archetype)>();

	private readonly Dictionary<ComponentSystem, ComponentType> _firedEvents = new Dictionary<ComponentSystem, ComponentType>();

	
	public void RegisterEvent(ComponentType componentType)
	{
		if (_registeredEvents.ContainsKey(componentType)) return;

		var eventGroup = GetComponentGroup(componentType);
		var eventArchetype = EntityManager.CreateArchetype(componentType);
		_registeredEvents.Add(componentType, (eventGroup, eventArchetype));
	}


	public void ShootEvent<T>(ComponentSystem system) where T : struct, IComponentData
	{
		ShootEvent(system, new T());
	}

	public void ShootEvent<T>(ComponentSystem system, T eventComponent) where T : struct, IComponentData
	{
		ComponentType eventType = ComponentType.Create<T>();
		
		if (!_registeredEvents.ContainsKey(eventType)) RegisterEvent(eventType);
		
		system.PostUpdateCommands.CreateEntity(_registeredEvents[eventType].Archetype);
		system.PostUpdateCommands.SetComponent(eventComponent);

		_firedEvents.Add(system, eventType);
		Debug.Log("Event shooted!");
	}
	
	
	public void ShootSharedDataEvent<T>(ComponentSystem system, T eventComponent) where T : struct, ISharedComponentData
	{
		ComponentType eventType = ComponentType.Create<T>();
		
		if (!_registeredEvents.ContainsKey(eventType)) RegisterEvent(eventType);
		
		system.PostUpdateCommands.CreateEntity(_registeredEvents[eventType].Archetype);
		system.PostUpdateCommands.SetSharedComponent(eventComponent);

		_firedEvents.Add(system, eventType);
		Debug.Log("Shared event shooted!");
	}


	public void ResetEvents(ComponentSystem system) 
	{
		if (!_firedEvents.ContainsKey(system)) return;
		var eventType = _firedEvents[system];
		var group = _registeredEvents[eventType].Group;

		var eventEntities = group.GetEntityArray();
		for (var i = 0; i < eventEntities.Length; i++)
		{
			system.PostUpdateCommands.DestroyEntity(eventEntities[i]);
			Debug.Log("Reseted");
		}
		
		_firedEvents.Remove(system);
	}
	
	

	protected override void OnCreateManager(int capacity)
	{
		Enabled = false;
	}

	protected override void OnUpdate()
	{
	}
}