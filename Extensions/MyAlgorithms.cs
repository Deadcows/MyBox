using System;
using UnityEngine.Events;
using UnityEngine;

namespace MyBox
{
	public static class MyAlgorithms
	{
		/// <summary>
		/// Convert to a different type.
		/// </summary>
		public static T Cast<T>(this IConvertible source) => (T)Convert.ChangeType(source, typeof(T));

		/// <summary>
		/// Check if this is a particular type.
		/// </summary>
		public static bool Is<T>(this object source) => source is T;

		/// <summary>
		/// Cast to a different type, exception-safe.
		/// </summary>
		public static T As<T>(this object source) where T : class => source as T;

		/// <summary>
		/// Take an object and pass it as an argument to a void function.
		/// </summary>
		public static T Pipe<T>(this T argument, Action<T> action)
		{
			action(argument);
			return argument;
		}

		/// <summary>
		/// Take an object, pass it as an argument to a function, return the result.
		/// </summary>
		public static TResult Pipe<T, TResult>(this T argument, Func<T, TResult> function) => function(argument);

		/// <summary>
		/// Take an object, pass it as an argument to a function, return the object.
		/// </summary>
		public static T PipeKeep<T, TResult>(this T argument, Func<T, TResult> function)
		{
			function(argument);
			return argument;
		}

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
