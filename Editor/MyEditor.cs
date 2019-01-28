using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class MyEditor
{

	#region Hierarchy Management

	/// <summary>
	/// Fold/Unfold GameObject with all childs in hierarchy
	/// </summary>
	public static void FoldInHierarchy(GameObject go, bool expand)
	{
		if (go == null) return;
		var type = typeof(EditorWindow).Assembly.GetType("UnityEditor.SceneHierarchyWindow");
		var methodInfo = type.GetMethod("SetExpandedRecursive");

		EditorApplication.ExecuteMenuItem("Window/General/Hierarchy");
		var window = EditorWindow.focusedWindow;

		methodInfo.Invoke(window, new object[] {go.GetInstanceID(), expand});
	}

	/// <summary>
	/// Fold objects in hierarchy for all opened scenes
	/// </summary>
	public static void FoldSceneHierarchy()
	{
		for (var i = 0; i < SceneManager.sceneCount; i++)
		{
			var scene = SceneManager.GetSceneAt(i);
			if (!scene.isLoaded) continue;
			var roots = SceneManager.GetSceneAt(i).GetRootGameObjects();
			for (var o = 0; o < roots.Length; o++)
			{
				FoldInHierarchy(roots[o], false);
			}
		}
	}
	
	#endregion

	
	#region GameObject Editor tools

	/// <summary>
	/// Set currently selected object to Rename Mode
	/// </summary>
	public static void InitiateObjectRename(GameObject objectToRename)
	{
		EditorApplication.update += ObjectRename;
		_renameTimestamp = EditorApplication.timeSinceStartup + 0.4d;
		EditorApplication.ExecuteMenuItem("Window/Hierarchy");
		Selection.activeGameObject = objectToRename;

		
	}
	private static void ObjectRename()
	{
		if (EditorApplication.timeSinceStartup >= _renameTimestamp)
		{
			EditorApplication.update -= ObjectRename;
			var type = typeof(EditorWindow).Assembly.GetType("UnityEditor.SceneHierarchyWindow");
			var hierarchyWindow = EditorWindow.GetWindow(type);
			var rename = type.GetMethod("RenameGO", BindingFlags.Instance | BindingFlags.NonPublic);
			rename.Invoke(hierarchyWindow, null);
		}
	}
	private static double _renameTimestamp;

	/// <summary>
	/// Apply changes on GO to prefab
	/// </summary>
	/// <param name="instance"></param>
	public static void ApplyPrefab(GameObject instance)
	{
		var instanceRoot = PrefabUtility.FindRootGameObjectWithSameParentPrefab(instance);
		var targetPrefab = PrefabUtility.GetCorrespondingObjectFromSource(instanceRoot);

		if (instanceRoot == null || targetPrefab == null)
		{
			Debug.LogError("ApplyPrefab failed. Target object " + instance.name + " is not a prefab");
			return;
		}

		PrefabUtility.ReplacePrefab(instanceRoot, targetPrefab, ReplacePrefabOptions.ConnectToPrefab);
	}

	#endregion

	
	#region Animation Asset Creation

	

	#endregion
}