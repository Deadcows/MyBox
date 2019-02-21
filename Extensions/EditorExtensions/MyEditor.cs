#if UNITY_EDITOR
using System.IO;
using System.Reflection;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;

namespace MyBox.EditorTools
{
	public static class MyEditor
	{
#pragma warning disable 618
		#region Hierarchy Management

		/// <summary>
		/// Fold/Unfold GameObject hierarchy
		/// </summary>
		public static void FoldInHierarchy(GameObject go, bool expand)
		{
			if (go == null) return;
			var type = typeof(EditorWindow).Assembly.GetType("UnityEditor.SceneHierarchyWindow");
			var methodInfo = type.GetMethod("SetExpandedRecursive");
			if (methodInfo == null) return;

			EditorApplication.ExecuteMenuItem("Window/General/Hierarchy");
			var window = EditorWindow.focusedWindow;

			methodInfo.Invoke(window, new object[] {go.GetInstanceID(), expand});
		}

		/// <summary>
		/// Fold objects hierarchy for all opened scenes
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


		#region GameObject Rename Mode

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
				var renameMethod = type.GetMethod("RenameGO", BindingFlags.Instance | BindingFlags.NonPublic);
				if (renameMethod == null)
				{
					Debug.LogError("RenameGO method is obsolete?");
					return;
				}
				renameMethod.Invoke(hierarchyWindow, null);
			}
		}

		private static double _renameTimestamp;

		#endregion


		#region Prefab Management

		/// <summary>
		/// Apply changes on GameObject to prefab
		/// </summary>
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

		/// <summary>
		/// Get Prefab path in Asset Database
		/// </summary>
		/// <returns>Null if not a prefab</returns>
		public static string GetPrefabPath(GameObject gameObject, bool withAssetName = true)
		{
			if (gameObject == null) return null;

			Object prefabParent = PrefabUtility.GetCorrespondingObjectFromSource(gameObject);
			if (prefabParent == null) return null;
			var assetPath = AssetDatabase.GetAssetPath(prefabParent);

			return !withAssetName ? Path.GetDirectoryName(assetPath) : assetPath;
		}

		public static bool IsPrefabInstance(this GameObject go)
		{
			return PrefabUtility.GetPrefabType(go) == PrefabType.Prefab;
		}

		#endregion


		#region Set Editor Icon

		/// <summary>
		/// Set Editor Icon (the one that appear in SceneView)
		/// </summary>
		public static void SetEditorIcon(this GameObject gameObject, bool textIcon, int iconIndex)
		{
			GUIContent[] icons = textIcon ? GetTextures("sv_label_", string.Empty, 0, 8) : GetTextures("sv_icon_dot", "_pix16_gizmo", 0, 16);

			var egu = typeof(EditorGUIUtility);
			var flags = BindingFlags.InvokeMethod | BindingFlags.Static | BindingFlags.NonPublic;
			var args = new object[] {gameObject, icons[iconIndex].image};
			var setIconMethod = egu.GetMethod("SetIconForObject", flags, null, new[] {typeof(Object), typeof(Texture2D)}, null);
			if (setIconMethod != null) setIconMethod.Invoke(null, args);
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
#pragma warning restore 618
	}
}
#endif