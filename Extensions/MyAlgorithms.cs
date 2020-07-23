using System;

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
		public static bool Is<R>(this System.Object source) => source is R;

		/// <summary>
		/// Cast to a different type, exception-safe.
		/// </summary>
		public static R As<R>(this System.Object source) where R : class
		{
			if (source is R) return source as R;
			return null;
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
	}
}
