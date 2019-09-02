#if UNITY_EDITOR
using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Plugins.MyBox.Tools
{
	[InitializeOnLoad, Serializable]
	public class OnPlaymodeLogger : ScriptableObject
	{
		public static void Log(string log)
		{
			var instance = GetOrCreateInstance();
			instance.Logs = instance.Logs.Append(log).ToArray();
			EditorUtility.SetDirty(instance);
			Debug.Log(instance.Logs.Length);
		}
		
		[SerializeField]
		public string[] Logs = new string[0];
		
		
		static OnPlaymodeLogger()
		{
			//MyEditorEvents.OnFirstFrame += LogAll;
		}

		private static void LogAll()
		{
			var logger = GetOrCreateInstance();
			
			Debug.LogWarning(logger.Logs.Length + " /");
			
			foreach (var loggerLog in logger.Logs)
			{
				Debug.LogWarning("LOGGED!!! " + loggerLog);
			}
		}

		private static OnPlaymodeLogger  GetOrCreateInstance()
		{
			if (_instance != null) return _instance;
			_instance = Resources.FindObjectsOfTypeAll<OnPlaymodeLogger>().FirstOrDefault();
			if (_instance == null) return CreateInstance<OnPlaymodeLogger>();
			return _instance;
		}
		private static OnPlaymodeLogger _instance;
	}
}
#endif