using System;
using UnityEngine;

public class TimeTest : IDisposable
{
	private readonly string _testTitle;
	private readonly bool _percise;
	private readonly DateTime _startTime;

	public TimeTest(string title, bool useMiliseconds = false)
	{
		_testTitle = title;
		_percise = useMiliseconds;
		_startTime = DateTime.Now;
	}

	public void Dispose()
	{
		var elapsed = DateTime.Now - _startTime;
		string message = "Time Test <color=brown>" + _testTitle + "</color>: " +
						 (_percise ? (elapsed.TotalMilliseconds + "ms") : (elapsed.TotalSeconds + "s"));
		Debug.LogWarning(message);
	}

	private static string _staticTestTitle;
	private static bool _staticPercise;
	private static DateTime _staticTestTime;

	public static void Start(string title, bool useMiliseconds = false)
	{
		_staticTestTitle = title;
		_staticPercise = useMiliseconds;
		_staticTestTime = DateTime.Now;
	}

	public static void End()
	{
		var elapsed = DateTime.Now - _staticTestTime;
		string message = "Time Test <color=brown>" + _staticTestTitle + "</color>: " +
						 (_staticPercise ? (elapsed.TotalMilliseconds + "ms") : (elapsed.TotalSeconds + "s"));
		Debug.LogWarning(message);
	}
}