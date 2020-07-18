using System;

namespace MyBox
{
	public static class MyAlgorithms
	{
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
