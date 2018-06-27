using Unity.Entities;

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

}

/// <summary>
/// Blitable boolean representation
/// </summary>
public struct Bool
{
	public bool Value
	{
		get => BlitableValue == 1;
		set => BlitableValue = (byte)(value ? 1 : 0);
	}

	public byte BlitableValue;
}

