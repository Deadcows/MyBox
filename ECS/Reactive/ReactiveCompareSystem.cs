using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using UnityEngine.Experimental.PlayerLoop;

namespace BovineLabs.Toolkit.Reactive
{
    [UpdateBefore(typeof(EarlyUpdate))]
    public class ReactiveUpdateBarrier : BarrierSystem
    {

    }

    [DisableAutoCreation]
    [UpdateBefore(typeof(EarlyUpdate))]
    public class ReactiveCompareSystem<T, TC, TN> : JobComponentSystem
        where T : struct, IComponentData
        where TC : struct, IReactiveCompare<T>, IComponentData
        where TN : struct, IComponentData
    {
        [Inject] private ReactiveUpdateBarrier _barrier;

        private ComponentGroup _group;
        private ComponentGroup _remove;

        // [BurstCompile] Burst does not support EntityCommandBuffer yet
        private struct CompareJob : IJobParallelFor
        {
            public EntityCommandBuffer.Concurrent CommandBuffer;

            [ReadOnly] public EntityArray Entities;
            [ReadOnly] public ComponentDataArray<T> Components;
            [ReadOnly] public ComponentDataArray<TC> Previous;

            public void Execute(int index)
            {
                // Hasn't changed
                if (Previous[index].Equals(Components[index]))
                    return;

                var previous = Previous[index];
                previous.Set(Components[index]);
                CommandBuffer.SetComponent(Entities[index], previous);
                CommandBuffer.AddComponent(Entities[index], new TN());
            }
        }

        protected override void OnCreateManager(int capacity)
        {
            _group = GetComponentGroup(ComponentType.ReadOnly<T>(), ComponentType.ReadOnly<TC>());
            _remove = GetComponentGroup(ComponentType.ReadOnly<TN>());
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            var compareJob = new CompareJob
                {
                    Entities = _group.GetEntityArray(),
                    Components = _group.GetComponentDataArray<T>(),
                    Previous = _group.GetComponentDataArray<TC>(),
                    CommandBuffer = _barrier.CreateCommandBuffer()
                }
                .Schedule(_group.CalculateLength(), 64, inputDeps);

            return compareJob;
        }
    }
}