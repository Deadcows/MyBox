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


	public static T CreateAsset<T>(string name, string folder = "Assets") where T : ScriptableObject
	{
#if UNITY_EDITOR
		if (string.IsNullOrEmpty(name))
		{
			Debug.LogError("ScriptableObjectUtility caused: Create Asset failed because Name is empty");
			return null;
		}
		string path = folder + "/" + name.Trim() + ".asset";

		var instance = ScriptableObject.CreateInstance<T>();

		AssetDatabase.CreateAsset(instance, AssetDatabase.GenerateUniqueAssetPath(path));

		AssetDatabase.SaveAssets();

		Debug.Log("Scriptable object asset created at " + path);

		return instance;
#else

		return null;

#endif
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

}
