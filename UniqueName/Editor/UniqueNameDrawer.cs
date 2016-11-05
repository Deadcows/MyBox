using System;
using UnityEditor;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Object = UnityEngine.Object;
using Debug = UnityEngine.Debug;


[CustomEditor(typeof(UniqueName))]
public class UniqueNameDrawer : Editor
{
	public override void OnInspectorGUI()
	{
	}
}


public class UniqueNameGenerator : UnityEditor.AssetModificationProcessor
{

	public static void GenerateNames()
	{
		Names.Clear();

		var uniqueNames = Object.FindObjectsOfType<UniqueName>();

		foreach (var uniqueName in uniqueNames)
		{
			var trimmedName = uniqueName.name.TrimEnd();
			if (!Names.Contains(trimmedName))
			{
				Names.Add(trimmedName);
				continue;
			}

			string baseName = trimmedName;
			if (Regex.IsMatch(baseName, @".+ \(\d+\)$"))
				baseName = baseName.Substring(0, baseName.LastIndexOf("(", StringComparison.Ordinal)).TrimEnd();

			string newName = baseName;
			int index = 1;
			while (Names.Contains(newName))
			{
				newName = baseName + " (" + index + ")";
				index++;
			}

			Debug.LogWarning("Object name changed from " + uniqueName.name + " to " + newName, uniqueName);
			uniqueName.name = newName;
			Names.Add(newName);
		}
	}

	private static readonly List<string> Names = new List<string>();

	public static string[] OnWillSaveAssets(string[] paths)
	{
		GenerateNames();

		return paths;
	}

}
