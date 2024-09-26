#if UNITY_EDITOR
#pragma warning disable 618
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;
using Object = UnityEngine.Object;
using System.Linq;
using JetBrains.Annotations;

namespace MyBox.EditorTools
{
	[PublicAPI]
	public static class MyEditor
	{
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

			methodInfo.Invoke(window, new object[] { go.GetInstanceID(), expand });
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

		public enum EditorIconColor
		{
			Gray = 0,
			Blue = 1,
			Teal = 2,
			Green = 3,
			Yellow = 4,
			Orange = 5,
			Red = 6,
			Purple = 7
		}
		
		/// <summary>
		/// Set Editor Icon (the one that appear in SceneView)
		/// </summary>
		public static void SetEditorIcon(this GameObject gameObject, bool textIcon, int iconIndex) 
			=> SetEditorIcon(gameObject, textIcon, (EditorIconColor)iconIndex);
		
		/// <summary>
		/// Set Editor Icon (the one that appear in SceneView)
		/// </summary>
		public static void SetEditorIcon(this GameObject gameObject, bool textIcon, EditorIconColor color)
		{
			GUIContent[] icons = textIcon ? GetTextures("sv_label_", string.Empty, 0, 8) : GetTextures("sv_icon_dot", "_pix16_gizmo", 0, 16);
			var iconIndex = (int)color;
#if UNITY_2021_3_OR_NEWER
			EditorGUIUtility.SetIconForObject(gameObject, (Texture2D)icons[iconIndex].image);
#else
			var egu = typeof(EditorGUIUtility);
			var flags = BindingFlags.InvokeMethod | BindingFlags.Static | BindingFlags.NonPublic;
			var args = new object[] { gameObject, icons[iconIndex].image };
			var setIconMethod = egu.GetMethod("SetIconForObject", flags, null, new[] { typeof(Object), typeof(Texture2D) }, null);
			if (setIconMethod != null) setIconMethod.Invoke(null, args);
#endif
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


		#region Get Fields With Attribute

		/// <summary>
		/// Get all fields with specified attribute on all Unity Objects
		/// </summary>
		public static List<ObjectField> GetFieldsWithAttributeFromScenes<T>() where T : Attribute
		{
			var allObjects = GetAllBehavioursInScenes();

			// ReSharper disable once CoVariantArrayConversion
			return GetFieldsWithAttribute<T>(allObjects);
		}
		
		/// <summary>
		/// Get all fields with specified attribute on all Unity Objects
		/// </summary>
		public static List<ObjectField> GetFieldsWithAttributeFromAll<T>() where T : Attribute
		{
			var allObjects = GetAllUnityObjects();

			return GetFieldsWithAttribute<T>(allObjects);
		}
		
		/// <summary>
		/// Get all fields with specified attribute from Prefab Root GO
		/// </summary>
		public static List<ObjectField> GetFieldsWithAttribute<T>(GameObject root) where T : Attribute
		{
			var allObjects = root.GetComponentsInChildren<MonoBehaviour>();

			// ReSharper disable once CoVariantArrayConversion
			return GetFieldsWithAttribute<T>(allObjects);
		}

		/// <summary>
		/// Get all fields with specified attribute from set of Unity Objects
		/// </summary>
		public static List<ObjectField> GetFieldsWithAttribute<T>(Object[] objects) where T : Attribute
		{
			var desiredAttribute = typeof(T);
			var result = new List<ObjectField>();
			foreach (var o in objects)
			{
				if (o == null) continue;
				var objectType = o.GetType();
				
				while (objectType != null)
				{
					var fields = objectType.GetFields(
						BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance
					);
					
					foreach (var field in fields)
					{
						if (!field.IsDefined(desiredAttribute, false)) continue;

						result.Add(new ObjectField(field, o));
					}

					objectType = objectType.BaseType;
				}
			}

			return result;
		}

		/// <summary>
		/// Get all Components in the same scene as a specified GameObject,
		/// including inactive components.
		/// </summary>
		public static IEnumerable<Component> GetAllComponentsInSceneOf(Object obj,
			Type type)
		{
			GameObject contextGO;
			if (obj is Component comp) contextGO = comp.gameObject;
			else if (obj is GameObject go) contextGO = go;
			else return Array.Empty<Component>();
			if (contextGO.scene.isLoaded) return contextGO.scene.GetRootGameObjects()
				.SelectMany(rgo => rgo.GetComponentsInChildren(type, true));
			return Array.Empty<Component>();
		}

		public struct ObjectField
		{
			public readonly FieldInfo Field;
			public readonly Object Context;

			public ObjectField(FieldInfo field, Object context)
			{
				Field = field;
				Context = context;
			}
		}

		/// <summary>
		/// Get every assets possible, including lazily-loaded assets.
		/// </summary>
		public static Object[] GetAllUnityObjects()
		{
			LoadAllAssetsOfType(typeof(ScriptableObject));
			LoadAllAssetsOfType("Prefab");
			return Resources.FindObjectsOfTypeAll(typeof(Object));
		}
		
		/// <summary>
		/// It's like FindObjectsOfType, but allows to get disabled objects
		/// </summary>
		public static MonoBehaviour[] GetAllBehavioursInScenes()
		{
			var components = new List<MonoBehaviour>();

			for (var i = 0; i < SceneManager.sceneCount; i++)
			{
				var scene = SceneManager.GetSceneAt(i);
				if (!scene.isLoaded) continue;
				
				var root = scene.GetRootGameObjects();
				foreach (var gameObject in root)
				{
					var behaviours = gameObject.GetComponentsInChildren<MonoBehaviour>(true);
					foreach (var behaviour in behaviours) components.Add(behaviour);
				}
			}

			return components.ToArray();
		}

		#endregion


		#region Get Script Asseet Path

		/// <summary>
		/// Get relative to Assets folder path to script file location
		/// </summary>
		public static string GetRelativeScriptAssetsPath(ScriptableObject so)
		{
			MonoScript ms = MonoScript.FromScriptableObject(so);
			return AssetDatabase.GetAssetPath(ms);
		}

		/// <summary>
		/// Get full path to script file location
		/// </summary>
		public static string GetScriptAssetPath(ScriptableObject so)
		{
			var assetsPath = GetRelativeScriptAssetsPath(so);
			return new FileInfo(assetsPath).DirectoryName;
		}

		/// <summary>
		/// Get relative to Assets folder path to script file location
		/// </summary>
		public static string GetRelativeScriptAssetsPath(MonoBehaviour mb)
		{
			MonoScript ms = MonoScript.FromMonoBehaviour(mb);
			return AssetDatabase.GetAssetPath(ms);
		}

		/// <summary>
		/// Get full path to script file location
		/// </summary>
		public static string GetScriptAssetPath(MonoBehaviour mb)
		{
			var assetsPath = GetRelativeScriptAssetsPath(mb);
			return new FileInfo(assetsPath).DirectoryName;
		}

		#endregion

		/// <summary>
		/// Force Unity Editor to load lazily-loaded types such as ScriptableObject.
		/// </summary>
		public static void LoadAllAssetsOfType(Type type) => AssetDatabase
			.FindAssets($"t:{type.FullName}")
			.ForEach(p => AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(p), type));

		/// <summary>
		/// Force Unity Editor to load lazily-loaded types such as ScriptableObject.
		/// </summary>
		public static void LoadAllAssetsOfType(string typeName) => AssetDatabase
			.FindAssets($"t:{typeName}")
			.ForEach(p => AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(p), typeof(UnityEngine.Object)));

		/// <summary>
		/// Copy the specified text, just like with Ctrl+C
		/// </summary>
		public static void CopyToClipboard(string text)
		{
			TextEditor te = new TextEditor();
			te.text = text;
			te.SelectAll();
			te.Copy();
		}
	}
}
#pragma warning restore 618
#endif