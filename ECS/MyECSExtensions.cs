using Unity.Entities;
using UnityEngine;

public static class MyECSExtensions
{
	private static EntityManager Manager => _manager ?? (_manager = World.Active.GetOrCreateManager<EntityManager>());
	private static EntityManager _manager;

	/// <summary>
	/// EntityManager.HasComponent(entity);
	/// </summary>
	public static bool HasComponent<T>(this Entity entity)
	{
		return Manager.HasComponent<T>(entity);
	}
	
	/// <summary>
	/// EntityManager.GetComponentData(entity);
	/// </summary>
	public static T GetComponent<T>(this Entity entity) where T : struct, IComponentData
	{
		return Manager.GetComponentData<T>(entity);
	}
	
	/// <summary>
	/// EntityManager.GetComponentData(entity);
	/// </summary>
	public static T GetComponentObject<T>(this Entity entity) where T : Component
	{
		return Manager.GetComponentObject<T>(entity);
	}
	
	
	
	/// <summary>
	/// EntityManager.SetComponentData(entity, componentData);
	/// </summary>
	public static void SetComponent<T>(this Entity entity, T componentData) where T : struct, IComponentData
	{
		Manager.SetComponentData(entity, componentData);
	}

	/// <summary>
	/// EntityManager.AddComponentData(entity, componentData);
	/// </summary>
	public static void AddComponentData<T>(this Entity entity, T componentData) where T : struct, IComponentData
	{
		Manager.AddComponentData(entity, componentData);
	}


	public static void ReplaceComponentData<T>(this EntityCommandBuffer commandBuffer, Entity entity, T component)
		where T : struct, IComponentData
	{
		var entityManager = World.Active.GetExistingManager<EntityManager>();
		if (entityManager.HasComponent<T>(entity))
			commandBuffer.SetComponent(entity, component);
		else
			commandBuffer.AddComponent(entity, component);
	}

	public static void ReplaceComponentData<T>(this EntityCommandBuffer commandBuffer, Entity entity)
		where T : struct, IComponentData
	{
		var entityManager = World.Active.GetExistingManager<EntityManager>();
		if (entityManager.HasComponent<T>(entity))
			commandBuffer.RemoveComponent<T>(entity);
	}
}