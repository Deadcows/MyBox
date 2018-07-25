using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using UnityEngine.Experimental.PlayerLoop;

namespace BovineLabs.Toolkit.Reactive
{
    [UpdateAfter(typeof(PostLateUpdate))]
    public class RemoveReactiveBarrier : BarrierSystem
    {

    }

    [UpdateAfter(typeof(PostLateUpdate))]
    public class RemoveReactiveSystem<T> : JobComponentSystem
        where T : struct, IComponentData
    {
        [Inject] private RemoveReactiveBarrier _barrier;
        private ComponentGroup _group;

        private struct RemoveReactiveChangedJob : IJobParallelFor
        {
            [ReadOnly] public EntityArray Entities;
            public EntityCommandBuffer.Concurrent CommandBuffer;

            public void Execute(int index)
            {
                CommandBuffer.RemoveComponent<T>(Entities[index]);
            }
        }

        protected override void OnCreateManager(int capacity)
        {
            _group = GetComponentGroup(ComponentType.ReadOnly<T>());
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            var entities = _group.GetEntityArray();

            return new RemoveReactiveChangedJob
            {
                CommandBuffer = _barrier.CreateCommandBuffer(),
                Entities = entities
            }.Schedule(entities.Length, 64, inputDeps);
        }
    }
}