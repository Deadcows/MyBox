using JetBrains.Annotations;
using UnityEngine.Events;

namespace MyBox
{
	[PublicAPI]
	public static class MyUnityEventExtensions
	{
		/// <summary>
		/// Adds a listener that executes only once to the UnityEvent.
		/// </summary>
		public static UnityEvent Once(this UnityEvent source, UnityAction action)
		{
			source.AddListener(WrapperAction);
			return source;

			void WrapperAction()
			{
				source.RemoveListener(WrapperAction);
				action();
			}
		}

		/// <summary>
		/// Adds a listener that executes only once to the UnityEvent.
		/// </summary>
		public static UnityEvent<T> Once<T>(this UnityEvent<T> source, UnityAction<T> action)
		{
			source.AddListener(WrapperAction);
			return source;
			
			void WrapperAction(T p)
			{
				source.RemoveListener(WrapperAction);
				action(p);
			}
		}

		/// <summary>
		/// Adds a listener that executes only once to the UnityEvent.
		/// </summary>
		public static UnityEvent<T0, T1> Once<T0, T1>(this UnityEvent<T0, T1> source, UnityAction<T0, T1> action)
		{
			source.AddListener(WrapperAction);
			return source;
			
			void WrapperAction(T0 p0, T1 p1)
			{
				source.RemoveListener(WrapperAction);
				action(p0, p1);
			}
		}

		/// <summary>
		/// Adds a listener that executes only once to the UnityEvent.
		/// </summary>
		public static UnityEvent<T0, T1, T2> Once<T0, T1, T2>(this UnityEvent<T0, T1, T2> source, UnityAction<T0, T1, T2> action)
		{
			source.AddListener(WrapperAction);
			return source;
			
			void WrapperAction(T0 p0, T1 p1, T2 p2)
			{
				source.RemoveListener(WrapperAction);
				action(p0, p1, p2);
			}
		}

		/// <summary>
		/// Adds a listener that executes only once to the UnityEvent.
		/// </summary>
		public static UnityEvent<T0, T1, T2, T3> Once<T0, T1, T2, T3>(this UnityEvent<T0, T1, T2, T3> source, UnityAction<T0, T1, T2, T3> action)
		{
			source.AddListener(WrapperAction);
			return source;
			
			void WrapperAction(T0 p0, T1 p1, T2 p2, T3 p3)
			{
				source.RemoveListener(WrapperAction);
				action(p0, p1, p2, p3);
			}
		}
	}
}