/*
using Unity.Entities;


public struct Event<T> where T : struct, IComponentData
{
	[Unity.Collections.ReadOnly] public ComponentDataArray<T> Data;
	[Unity.Collections.ReadOnly] public ComponentDataArray<EventsSystem.EventActiveComponent> EventActive;
	public EntityArray Entity;
	
	public bool Fired => Data.Length > 0;
}

public struct SharedDataEvent<T> where T : struct, ISharedComponentData
{
	[Unity.Collections.ReadOnly] public SharedComponentDataArray<T> Data;
	[Unity.Collections.ReadOnly] public ComponentDataArray<EventsSystem.EventActiveComponent> EventActive;
	public EntityArray Entity;

	public bool Fired => Data.Length > 0;
}


[AlwaysUpdateSystem]
public class EventsSystem : ComponentSystem
{
	private ComponentGroup _firedEvents;
	private ComponentGroup _firedActiveEvents;

	protected override void OnCreateManager()
	{
		_firedEvents = GetComponentGroup(ComponentType.Create<EventComponent>(), ComponentType.Subtractive<EventActiveComponent>());
		_firedActiveEvents = GetComponentGroup(ComponentType.Create<EventComponent>(), ComponentType.Create<EventActiveComponent>());
	}

	protected override void OnUpdate()
	{
		DestroyActiveEvents();
		ActivateInactiveEvents();

		void DestroyActiveEvents()
		{
			var activeEvents = _firedActiveEvents.GetEntityArray();

			for (var i = 0; i < activeEvents.Length; i++)
			{
				var entity = activeEvents[i];
				PostUpdateCommands.DestroyEntity(entity);
			}
		}

		void ActivateInactiveEvents()
		{
			var inactiveEvents = _firedEvents.GetEntityArray();
			for (var i = 0; i < inactiveEvents.Length; i++)
			{
				var entity = inactiveEvents[i];
				PostUpdateCommands.AddComponent(entity, new EventActiveComponent());
			}
		}
	}

	public struct EventComponent : IComponentData
	{
	}

	public struct EventActiveComponent : IComponentData
	{
	}
}

public static class EventsSystemExtension
{
	public static void AddEvent<T>(this EntityCommandBuffer buffer, T eventComponent) where T : struct, IComponentData
	{
		buffer.CreateEntity();
		buffer.AddComponent(eventComponent);
		buffer.AddComponent(new EventsSystem.EventComponent());
	}

	public static Entity AddEvent<T>(this EntityManager manager, T eventComponent) where T : struct, IComponentData
	{
		var eventEntity = manager.CreateEntity();
		manager.AddComponentData(eventEntity, eventComponent);
		manager.AddComponent(eventEntity, ComponentType.Create<EventsSystem.EventComponent>());
		return eventEntity;
	}

	public static void AddSharedEvent<T>(this EntityCommandBuffer buffer, T eventComponent) where T : struct, ISharedComponentData
	{
		buffer.CreateEntity();
		buffer.AddSharedComponent(eventComponent);
		buffer.AddComponent(new EventsSystem.EventComponent());
	}

	public static Entity AddSharedEvent<T>(this EntityManager manager, T eventComponent) where T : struct, ISharedComponentData
	{
		var eventEntity = manager.CreateEntity();
		manager.AddSharedComponentData(eventEntity, eventComponent);
		manager.AddComponent(eventEntity, ComponentType.Create<EventsSystem.EventComponent>());
		return eventEntity;
	}
}
*/