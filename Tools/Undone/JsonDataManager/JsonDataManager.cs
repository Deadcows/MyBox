//using System.Collections.Generic;
//using System.IO;
//using System.Text;
//#if UNITY_EDITOR
//using UnityEditor;
//#endif
//using UnityEngine;
//
//namespace MyBox.Internal
//{
//	public static class JsonDataManager
//	{
//		//TODO: Log configs usage? Collect used configs in SettingsFolder and log unused on application exit?
//		//TODO: How to track type/fields rename? (well.. version control system is here for us)
//		//TODO: Change this manager to some BaseType (instead of SO)?
//		//TODO: Add some separate file to track settings foldouts? 
//
//		public const string EditorSettingsFolder = "Misc/Configs";
//		public const string BuildSettingsFolder = "Configs";
//		private static readonly Dictionary<string, ScriptableObject> Cache = new Dictionary<string, ScriptableObject>();
//
//		private static string GetPath(string type)
//		{
//			var settingsFolder = Application.isEditor ? EditorSettingsFolder : BuildSettingsFolder;
//			var sb = new StringBuilder();
//			sb.Append(Application.dataPath);
//			sb.Append(Path.PathSeparator);
//			sb.Append(settingsFolder);
//			sb.Append(Path.PathSeparator);
//			sb.Append(type);
//			sb.Append(".json");
//			return sb.ToString();
//		}
//
//		public static T FromJson<T>(bool directlyFromFile = false) where T : ScriptableObject, new()
//		{
//			string typeString = typeof(T).ToString();
//			if (!directlyFromFile && Cache.ContainsKey(typeString)) return Cache[typeString] as T;
//
//			string path = GetPath(typeString);
//			var data = ScriptableObject.CreateInstance<T>();
//
//			if (File.Exists(path))
//			{
//				string json = File.ReadAllText(path);
//				JsonUtility.FromJsonOverwrite(json, data);
//			}
//			else SaveJson(data);
//
//			Cache[typeString] = data;
//			return data;
//		}
//
//		public static void SaveJson<T>(T so) where T : ScriptableObject
//		{
//#if UNITY_EDITOR
//			string typeString = typeof(T).ToString();
//			var jsonString = JsonUtility.ToJson(so, true);
//			var path = GetPath(typeString);
//
//			File.WriteAllText(path, jsonString);
//
//			AssetDatabase.Refresh();
//#endif
//		}
//	}
//}