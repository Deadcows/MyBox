using System;
using System.IO;
using UnityEngine;


public class MyLogger
{

	public static bool Disabled;

	public static int SessionMark { get; private set; }

	public static string Version
	{
		get { return string.IsNullOrEmpty(_version) ? "Version not initiated" : _version; }
		private set { _version = value; }
	}
	private static string _version;


	private const int MaxMessageLength = 4000;

	static MyLogger()
	{
		SessionMark = UnityEngine.Random.Range(1000000000, int.MaxValue);

		AppDomain.CurrentDomain.UnhandledException += (sender, args) => LogIt(args.ExceptionObject as Exception);

		Application.logMessageReceived += (condition, trace, type) =>
		{
			LogIt(string.Format("Console Log ({0}): {1}{2}{3}",
				type, condition, Environment.NewLine, trace));
		};
	}

	public static void SetVersion(string version)
	{
		Version = version;
		LogIt("Initiated");
	}

	public static void LogIt(string text)
	{
		if (Application.isEditor) return;
		if (Disabled) return;

		string path = Path.Combine(Application.dataPath, "customLog.txt");

		if (text.Length > MaxMessageLength)
			text = text.Substring(0, MaxMessageLength) + "...<trimmed>";

		try
		{
			if (!File.Exists(path))
			{
				using (StreamWriter sw = File.CreateText(path))
				{
					sw.WriteLine(DateTime.Now.ToUniversalTime() + " || created" + Environment.NewLine);
					sw.WriteLine(DateTime.Now.ToString("MM-dd_HH-mm-ss") + ": " + text);
				}
			}
			else
			{
				using (StreamWriter sw = File.AppendText(path))
				{
					sw.WriteLine(DateTime.Now.ToString("MM-dd_HH-mm-ss") + ": " + text);
				}
			}
		}
		catch (Exception ex)
		{
			Debug.LogException(ex);
		}
	}

	public static void LogIt(Exception ex)
	{
		LogIt("Exception:" + Environment.NewLine + ex.Message + Environment.NewLine + ex.StackTrace);
	}

}