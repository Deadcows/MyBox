using System;
using JetBrains.Annotations;

namespace MyBox
{
	/// <see href="https://github.com/Deadcows/MyBox/wiki/Extensions#myalgorithms" />
	[PublicAPI]
	public static class MyAlgorithms
	{
		/// <summary>
		/// Convert to a different type
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
	}
}
