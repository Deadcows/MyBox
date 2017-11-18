using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;


public static class JsonDataManager
{

	//TODO: Log configs usage? Collect used configs in SettingsFolder and log unused on application exit?
	//TODO: How to track type/fields rename? (well.. version control system is here for us)
	//TODO: Заменить менеджер на базовый тип (вместо SO)?
	//TODO: Добавить отдельный файл для самого DataManager, в котором отслеживать фолдауты или типа того?

	public const string SettingsFolder = "Misc/Configs";
	private static readonly Dictionary<string, ScriptableObject> Cache = new Dictionary<string, ScriptableObject>();

	private static string GetPath(string type)
	{
		return Path.Combine(Application.dataPath, SettingsFolder, type + ".json");
	}

	public static T FromJson<T>(bool directlyFromFile = false) where T : ScriptableObject, new()
	{
		string typeString = typeof(T).ToString();
		if (!directlyFromFile && Cache.ContainsKey(typeString)) return Cache[typeString] as T;

		string path = GetPath(typeString);
		var data = ScriptableObject.CreateInstance<T>();

		if (File.Exists(path))
		{
			string json = File.ReadAllText(path);
			JsonUtility.FromJsonOverwrite(json, data);
		}
		else SaveJson(data);

		Cache[typeString] = data;
		return data;
	}


	public static void SaveJson<T>(T so) where T : ScriptableObject
	{
		string typeString = typeof(T).ToString();
		var jsonString = JsonUtility.ToJson(so, true);
		var path = GetPath(typeString);

		File.WriteAllText(path, jsonString);

		AssetDatabase.Refresh();
	}
}
