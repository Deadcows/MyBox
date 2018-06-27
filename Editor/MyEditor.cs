using System.Reflection;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class MyEditor
{

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

		methodInfo.Invoke(window, new object[] { go.GetInstanceID(), expand });
	}

	public static void FoldSceneHierarchy()
	{
		var roots = GetSceneRoots();
		for (var i = 0; i < roots.Length; i++)
		{
			FoldInHierarchy(roots[i], false);
		}
	}

	public static void PushUpInInspector(this Component component)
	{
		UnityEditorInternal.ComponentUtility.MoveComponentUp(component);
		UnityEditorInternal.ComponentUtility.MoveComponentUp(component);
		UnityEditorInternal.ComponentUtility.MoveComponentUp(component);
		UnityEditorInternal.ComponentUtility.MoveComponentUp(component);
		UnityEditorInternal.ComponentUtility.MoveComponentUp(component);
	}

	/// <summary>
	/// Get all root objects on scene
	/// </summary>
	public static GameObject[] GetSceneRoots()
	{
		return SceneManager.GetActiveScene().GetRootGameObjects();
	}

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
	private static double _renameTimestamp;
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


	public static void ApplyPrefab(GameObject instance)
	{  
		var instanceRoot = PrefabUtility.FindRootGameObjectWithSameParentPrefab(instance);
		var targetPrefab = PrefabUtility.GetPrefabParent(instanceRoot);

		if (instanceRoot == null || targetPrefab == null)
		{
			Debug.LogError("ApplyPrefab failed. Target object " + instance.name + " is not a prefab");
			return;
		}

		PrefabUtility.ReplacePrefab(
				instanceRoot,
				targetPrefab,
				ReplacePrefabOptions.ConnectToPrefab
				);
	}

	#region Animation Asset Creation

	/// <summary>
	/// Create .controller asset at path and assign with targetObject.Animator
	/// </summary>
	/// <param name="targetObject">Object to add Animation Controller</param>
	/// <param name="path">Path to save controller</param>
	/// <param name="clips">Create .anim assets with given names and assign to .controller.
	/// Names with + on end will generate as looping clips</param>
	public static void CreateAnimationControllerAsset(GameObject targetObject, string path, params string[] clips)
	{
		var animator = targetObject.GetComponent<Animator>();
		if (animator == null) animator = targetObject.AddComponent<Animator>();
		if (animator.runtimeAnimatorController != null)
		{
			Debug.LogWarning("Target already contains Animator with Controller");
			return;
		}

		var controllerPath = AssetDatabase.GenerateUniqueAssetPath(path + "Animation.controller");
		var controller = AnimatorController.CreateAnimatorControllerAtPath(controllerPath);


		if (clips != null)
		{
			for (var i = 0; i < clips.Length; i++)
			{
				var clipName = clips[i];
				var clip = new AnimationClip();

				if (clipName.EndsWith("+"))
				{
					var clipSO = new SerializedObject(clip);
					var clipSettingsProp = clipSO.FindProperty("m_AnimationClipSettings");
					var loopProp = clipSettingsProp.FindPropertyRelative("m_LoopTime");

					loopProp.boolValue = true;
					clipSO.ApplyModifiedProperties();

					clipName = clipName.TrimEnd('+');
				}
				var clipPath = AssetDatabase.GenerateUniqueAssetPath(path + clipName + ".anim");
				AssetDatabase.CreateAsset(clip, clipPath);

				var motion = controller.AddMotion(clip);
				motion.name = clipName;

				EditorUtility.SetDirty(clip);
			}
		}

		animator.runtimeAnimatorController = controller;

		EditorUtility.SetDirty(controller);
		EditorUtility.SetDirty(targetObject);
	}

	#endregion

}
