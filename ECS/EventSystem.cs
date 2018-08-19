using Unity.Entities;
using UnityEngine.Experimental.PlayerLoop;

[UpdateInGroup(typeof(PreUpdate))]
public class EventComponentSystem : ComponentSystem
{
	public struct EventComponent : IComponentData
	{
	}

	private struct Group
	{
		public EntityArray Entity;
		public ComponentDataArray<EventComponent> Event;
		public readonly int Length;
	}

	[Inject] private Group _group;

	protected override void OnUpdate()
	{
		for (var i = 0; i < _group.Length; i++)
		{
			var entity = _group.Entity[i];
			EntityManager.DestroyEntity(entity);
		}
	}
}