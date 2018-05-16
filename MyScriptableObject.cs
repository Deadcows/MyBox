using System.IO;
using System.Linq;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public static class MyScriptableObject
{

	public static T[] LoadAssetsFromResources<T>() where T : ScriptableObject
	{
		return Resources.LoadAll("", typeof(T)).Cast<T>().ToArray();
	}


#if UNITY_EDITOR

	public static T[] LoadAssets<T>() where T : ScriptableObject
	{
		string[] guids = AssetDatabase.FindAssets("t:" + typeof(T).Name);
		T[] a = new T[guids.Length];
		for (int i = 0; i < guids.Length; i++)
		{
			string path = AssetDatabase.GUIDToAssetPath(guids[i]);
			a[i] = AssetDatabase.LoadAssetAtPath<T>(path);
		}

		return a;
	}

	public static string GetScriptAssetsPath(ScriptableObject so)
	{
		MonoScript ms = MonoScript.FromScriptableObject(so);
		return AssetDatabase.GetAssetPath(ms);
	}

	public static string GetScriptFullDirectory(ScriptableObject so)
	{
		var assetsPath = GetScriptAssetsPath(so);
		return new FileInfo(assetsPath).DirectoryName;
	}

	public static T CreateAsset<T>(string name, string folder = "Assets") where T : ScriptableObject
	{
		if (string.IsNullOrEmpty(name))
		{
			Debug.LogError("ScriptableObjectUtility caused: Create Asset failed because Name is empty");
			return null;
		}
		string path = folder + "/" + name.Trim() + ".asset";

		var instance = ScriptableObject.CreateInstance<T>();

		var fullPath = Path.GetFullPath(path);
		var directory = Path.GetDirectoryName(fullPath);
		if (directory != null) Directory.CreateDirectory(directory);

		AssetDatabase.CreateAsset(instance, AssetDatabase.GenerateUniqueAssetPath(path));

		AssetDatabase.SaveAssets();

		Debug.Log("Scriptable object asset created at " + path);

		return instance;
	}


#endif

}
