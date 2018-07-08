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
			Debug.Assert(Components.Length == 1, "UniqueComponent: There is more tnan one instance of component");
			return Components[0];
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
			Debug.Assert(Components.Length == 1, "UniqueComponent: There is more tnan one instance of component");
			return Components[0];
		}
	}

	public Entity EntityInstance => Entities[0];
}