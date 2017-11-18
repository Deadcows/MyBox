using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using Object = UnityEngine.Object;

public class ReferenceFilter : EditorWindow
{
	//[MenuItem("Assets/What objects use this?", false, 20)]
	//private static void OnSearchForReferences()
	//{
	//	string final = "";
	//	List<Object> matches = new List<Object>();
	//	int iid = Selection.activeInstanceID;
	//	if (AssetDatabase.IsMainAsset(iid))
	//	{
	//		// only main assets have unique paths
	//		string path = AssetDatabase.GetAssetPath(iid);
	//		// strip down the name
	//		final = System.IO.Path.GetFileNameWithoutExtension(path);
	//	}
	//	else
	//	{
	//		Debug.Log("Error Asset not found");
	//		return;
	//	}
	//	// get everything
	//	Object[] _Objects = Resources.FindObjectsOfTypeAll<Object>();
	//	//loop through everything
	//	foreach (Object go in _Objects)
	//	{
	//		// needs to be an array
	//		Object[] g = new Object[1];
	//		g[0] = go;
	//		// All objects
	//		Object[] depndencies = EditorUtility.CollectDependencies(g);
	//		foreach (Object o in depndencies)
	//			if (string.CompareOrdinal(o.name, final) == 0)
	//			{
	//				Debug.Log("Referenced by: " + AssetDatabase.GetAssetPath(go));
	//				matches.Add(go);// add it to our list to highlight
	//			}
	//	}
	//	Selection.objects = matches.ToArray();
	//	matches.Clear(); // clear the list 
	//}
}