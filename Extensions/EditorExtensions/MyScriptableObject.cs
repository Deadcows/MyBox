#if UNITY_EDITOR
using System.IO;
using UnityEngine;
using UnityEditor;

namespace MyBox.EditorTools
{
	public static class MyScriptableObject
	{
		/// <summary>
		/// Load all ScriptableObjects of type
		/// </summary>
		public static T[] LoadAssetsFromResources<T>() where T : ScriptableObject
		{
			return Resources.FindObjectsOfTypeAll<T>();
		}

		/// <summary>
		/// Load all SO of type from Assets
		/// </summary>
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

		
		/// <summary>
		/// Create ScriptableObject asset of name in folder
		/// </summary>
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

		public static T CreateAssetWithFolderDialog<T>(string filename) where T : ScriptableObject
		{
			var path = EditorUtility.SaveFolderPanel("Where to save", "Assets/", "");
			if (path.Length <= 0) return null;
			var relativePath = "Assets" + path.Substring(Application.dataPath.Length);
			
			return CreateAsset<T>(filename, relativePath);
		}
	}
}
#endif