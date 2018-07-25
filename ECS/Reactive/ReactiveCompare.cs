using Unity.Entities;

namespace BovineLabs.Toolkit.Reactive
{
    public struct ReactiveCompare<T> : IComponentData, IReactiveCompare<T>
        where T : struct, IComponentData
    {
        public T PreviousState;

        public bool Equals(T t)
        {
            return t.Equals(PreviousState);
        }

        public void Set(T t)
        {
            PreviousState = t;
        }
    }

    public interface IReactiveCompare<in T>
    {
        bool Equals(T t);
        void Set(T t);
    }
}