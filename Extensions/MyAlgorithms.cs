using System;
using UnityEngine.Events;

namespace MyBox
{
	public static class MyAlgorithms
	{
		/// <summary>
		/// Convert to a different type.
		/// </summary>
		public static R Cast<R>(this IConvertible source) =>
			(R)Convert.ChangeType(source, typeof(R));

		/// <summary>
		/// Check if this is a particular type.
		/// </summary>
		public static bool Is<R>(this object source) => source is R;

		/// <summary>
		/// Cast to a different type, exception-safe.
		/// </summary>
		public static R As<R>(this object source) where R : class
		{
			return source as R;
		}

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
		public static R Pipe<T, R>(this T argument, Func<T, R> function) =>
			function(argument);

		/// <summary>
		/// Take an object, pass it as an argument to a function, return the object.
		/// </summary>
		public static T PipeKeep<T, R>(this T argument, Func<T, R> function)
		{
			function(argument);
			return argument;
		}

		/// <summary>
		/// Adds a listener that executes only once to the UnityEvent.
		/// </summary>
		public static UnityEvent Once(this UnityEvent source, UnityAction action)
		{
			UnityAction wrapperAction = null;
			wrapperAction = () =>
			{
				source.RemoveListener(wrapperAction);
				action();
			};
			source.AddListener(wrapperAction);
			return source;
		}

		/// <summary>
		/// Adds a listener that executes only once to the UnityEvent.
		/// </summary>
		public static UnityEvent<T> Once<T>(this UnityEvent<T> source,
			UnityAction<T> action)
		{
			UnityAction<T> wrapperAction = null;
			wrapperAction = p =>
			{
				source.RemoveListener(wrapperAction);
				action(p);
			};
			source.AddListener(wrapperAction);
			return source;
		}

		/// <summary>
		/// Adds a listener that executes only once to the UnityEvent.
		/// </summary>
		public static UnityEvent<T0, T1> Once<T0, T1>(
			this UnityEvent<T0, T1> source,
			UnityAction<T0, T1> action)
		{
			UnityAction<T0, T1> wrapperAction = null;
			wrapperAction = (p0, p1) =>
			{
				source.RemoveListener(wrapperAction);
				action(p0, p1);
			};
			source.AddListener(wrapperAction);
			return source;
		}

		/// <summary>
		/// Adds a listener that executes only once to the UnityEvent.
		/// </summary>
		public static UnityEvent<T0, T1, T2> Once<T0, T1, T2>(
			this UnityEvent<T0, T1, T2> source,
			UnityAction<T0, T1, T2> action)
		{
			UnityAction<T0, T1, T2> wrapperAction = null;
			wrapperAction = (p0, p1, p2) =>
			{
				source.RemoveListener(wrapperAction);
				action(p0, p1, p2);
			};
			source.AddListener(wrapperAction);
			return source;
		}

		/// <summary>
		/// Adds a listener that executes only once to the UnityEvent.
		/// </summary>
		public static UnityEvent<T0, T1, T2, T3> Once<T0, T1, T2, T3>(
			this UnityEvent<T0, T1, T2, T3> source,
			UnityAction<T0, T1, T2, T3> action)
		{
			UnityAction<T0, T1, T2, T3> wrapperAction = null;
			wrapperAction = (p0, p1, p2, p3) =>
			{
				source.RemoveListener(wrapperAction);
				action(p0, p1, p2, p3);
			};
			source.AddListener(wrapperAction);
			return source;
		}
	}
}
