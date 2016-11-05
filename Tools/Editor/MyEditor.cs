using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

public static class MyEditor
{

	/// <summary>
	/// Fold/Unfold GameObject with all childs in hierarchy
	/// </summary>
	public static void FoldInHierarchy(GameObject go, bool expand)
	{
		var type = typeof(EditorWindow).Assembly.GetType("UnityEditor.SceneHierarchyWindow");
		var methodInfo = type.GetMethod("SetExpandedRecursive");

		EditorApplication.ExecuteMenuItem("Window/Hierarchy");
		var window = EditorWindow.focusedWindow;

		methodInfo.Invoke(window, new object[] { go.GetInstanceID(), expand });
	}

	/// <summary>
	/// Get all root objects on scene
	/// </summary>
	public static IEnumerable<GameObject> GetSceneRoots()
	{
		var prop = new HierarchyProperty(HierarchyType.GameObjects);
		var expanded = new int[0];
		while (prop.Next(expanded))
		{
			yield return prop.pptrValue as GameObject;
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


	#region Extensions
	
	/// <summary>
	/// Get Prefab path in Asset Database
	/// </summary>
	/// <returns>Null if not a prefab</returns>
	public static string PrefabPath(this GameObject gameObject, bool withAssetName = true)
	{
		if (gameObject == null) return null;
		Object currentBackgroundPrefab = PrefabUtility.GetPrefabParent(gameObject);
		return currentBackgroundPrefab != null ?
			!withAssetName ? Path.GetDirectoryName(AssetDatabase.GetAssetPath(currentBackgroundPrefab)) : AssetDatabase.GetAssetPath(currentBackgroundPrefab)
			: null;
	}

	public static string AsStringValue(this SerializedProperty property)
	{
		switch (property.propertyType)
		{
			case SerializedPropertyType.String:
				return property.stringValue;

			case SerializedPropertyType.Character:
			case SerializedPropertyType.Integer:
				if (property.type == "char")
				{
					return System.Convert.ToChar(property.intValue).ToString();
				}
				return property.intValue.ToString();

			case SerializedPropertyType.ObjectReference:
				return property.objectReferenceValue != null ? property.objectReferenceValue.ToString() : "null";

			case SerializedPropertyType.Boolean:
				return property.boolValue.ToString();

			case SerializedPropertyType.Enum:
				return property.enumNames[property.enumValueIndex];

			default:
				return string.Empty;
		}
	}

	public static void SetEditorIcon(this GameObject gameObject, bool textIcon, int iconIndex)
	{
		GUIContent[] icons = textIcon ? GetTextures("sv_label_", string.Empty, 0, 8) :
			GetTextures("sv_icon_dot", "_pix16_gizmo", 0, 16);

		var egu = typeof(EditorGUIUtility);
		var flags = BindingFlags.InvokeMethod | BindingFlags.Static | BindingFlags.NonPublic;
		var args = new object[] { gameObject, icons[iconIndex].image };
		var setIcon = egu.GetMethod("SetIconForObject", flags, null, new[] { typeof(Object), typeof(Texture2D) }, null);
		setIcon.Invoke(null, args);
	}

	private static GUIContent[] GetTextures(string baseName, string postFix, int startIndex, int count)
	{
		GUIContent[] array = new GUIContent[count];
		for (int i = 0; i < count; i++)
		{
			array[i] = EditorGUIUtility.IconContent(baseName + (startIndex + i) + postFix);
		}
		return array;
	}

	#endregion

}
