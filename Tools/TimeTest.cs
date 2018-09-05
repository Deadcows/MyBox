using System;
using System.Text;
using UnityEngine;

namespace MyBox
{
	public class TimeTest : IDisposable
	{
		private readonly string _testTitle;
		private readonly bool _precise;
		private readonly DateTime _startTime;

		public TimeTest(string title, bool useMilliseconds = false)
		{
			_testTitle = title;
			_precise = useMilliseconds;
			_startTime = DateTime.Now;
		}

		public void Dispose()
		{
			LogEnd(_testTitle, _startTime, _precise);
		}


		private static string _staticTestTitle;
		private static bool _staticPrecise;
		private static DateTime _staticTestTime;

		public static void Start(string title, bool useMilliseconds = false)
		{
			_staticTestTitle = title;
			_staticPrecise = useMilliseconds;
			_staticTestTime = DateTime.Now;
		}

		public static void End()
		{
			LogEnd(_staticTestTitle, _staticTestTime, _staticPrecise);
		}

		private static void LogEnd(string testName, DateTime startTime, bool precise)
		{
			var elapsed = DateTime.Now - startTime;
			var elapsedVal = precise ? elapsed.TotalMilliseconds : elapsed.TotalSeconds;
			var valMark = precise ? "ms" : "s";

			StringBuilder message = new StringBuilder();
			message.Append("Time Test <color=brown>").Append(testName).Append("</color>: ")
				.Append(elapsedVal).Append(valMark);

			Debug.LogWarning(message);
		}
	}
}