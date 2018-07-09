using Unity.Entities;
using UnityEngine;


public struct UniqueComponent<T> where T : Component
{
	public ComponentArray<T> Components;
	public EntityArray Entities;

	public T Instance
	{
		get
		{
			Debug.Assert(Components.Length == 1, "UniqueComponent: There is more/less tnan one instance of component");
			return Components[0];
		}
	}

	public Entity EntityInstance => Entities[0];
}

public struct UniqueTransformComponent<T> where T : Component
{
	public ComponentArray<T> Components;
	public ComponentArray<Transform> Transform;
	public EntityArray Entities;

	public T Instance
	{
		get
		{
			Debug.Assert(Components.Length == 1, "UniqueComponent: There is more/less tnan one instance of component");
			return Components[0];
		}
	}

	public Transform TransformInstance
	{
		get
		{
			Debug.Assert(Components.Length == 1, "UniqueComponent: There is more/less tnan one instance of component");
			return Transform[0];
		}
	}

	public Entity EntityInstance => Entities[0];
}


public struct UniqueComponentData<T> where T : struct, IComponentData
{
	public ComponentDataArray<T> Components;
	public EntityArray Entities;

	public T Instance
	{
		get
		{
			Debug.Assert(Components.Length == 1, "UniqueComponent: There is more/less tnan one instance of component");
			return Components[0];
		}
	}

	public Entity EntityInstance => Entities[0];
}