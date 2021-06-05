using System;
using UnityEngine;

namespace MyBox
{
	/// <summary>
	/// Automatically assign components to this Property.
	/// It searches for components from this GO or its children by default.
	/// Pass in an <c>AutoPropertyMode</c> to override this behaviour.
	/// </summary>
	[AttributeUsage(AttributeTargets.Field)]
	public class AutoPropertyAttribute : PropertyAttribute
	{
		public readonly AutoPropertyMode Mode;

		public AutoPropertyAttribute(AutoPropertyMode mode = AutoPropertyMode.Children) => Mode = mode;
	}

	public enum AutoPropertyMode
	{
		/// <summary>
		/// Search for Components from this GO or its children.
		/// </summary>
		Children = 0,
		/// <summary>
		/// Search for Components from this GO or its parents.
		/// </summary>
		Parent = 1,
		/// <summary>
		/// Search for Components from this GO's current scene.
		/// </summary>
		Scene = 2,
		/// <summary>
		/// Search for Objects from this project's asset folder.
		/// </summary>
		Asset = 3,
		/// <summary>
		/// Search for Objects from anywhere in the project.
		/// Combines the results of Scene and Asset modes.
		/// </summary>
		Any = 4
	}
}

#if UNITY_EDITOR
namespace MyBox.Internal
{
	using UnityEditor;
	using EditorTools;
	using UnityEditor.Experimental.SceneManagement;
	using Object = UnityEngine.Object;
	using System.Collections.Generic;
	using System.Linq;

	[CustomPropertyDrawer(typeof(AutoPropertyAttribute))]
	public class AutoPropertyDrawer : PropertyDrawer
	{
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			GUI.enabled = false;
			EditorGUI.PropertyField(position, property, label);
			GUI.enabled = true;
		}
	}


	[InitializeOnLoad]
	public static class AutoPropertyHandler
	{
		private static readonly Dictionary<AutoPropertyMode, Func<MyEditor.ComponentField, Object[]>> MultipleObjectsGetters
			= new Dictionary<AutoPropertyMode, Func<MyEditor.ComponentField, Object[]>>
			{
				[AutoPropertyMode.Children] = property => property.Component
					.GetComponentsInChildren(property.Field.FieldType.GetElementType(), true),
				[AutoPropertyMode.Parent] = property => property.Component
					.GetComponentsInParent(property.Field.FieldType.GetElementType(), true),
				[AutoPropertyMode.Scene] = property => Object
					.FindObjectsOfType(property.Field.FieldType.GetElementType()),
				[AutoPropertyMode.Asset] = property =>
				{
					MyEditor.LoadAllAssetsOfType(property.Field.FieldType.GetElementType());
					MyEditor.LoadAllAssetsOfType("Prefab");
					return Resources.FindObjectsOfTypeAll(property.Field.FieldType.GetElementType()).Where(AssetDatabase.Contains).ToArray();
				},
				[AutoPropertyMode.Any] = property =>
				{
					MyEditor.LoadAllAssetsOfType(property.Field.FieldType.GetElementType());
					MyEditor.LoadAllAssetsOfType("Prefab");
					return Resources.FindObjectsOfTypeAll(property.Field.FieldType.GetElementType());
				}
			};

		private static readonly Dictionary<AutoPropertyMode, Func<MyEditor.ComponentField, Object>> SingularObjectGetters
			= new Dictionary<AutoPropertyMode, Func<MyEditor.ComponentField, Object>>
			{
				[AutoPropertyMode.Children] = property => property.Component
					.GetComponentInChildren(property.Field.FieldType, true),
				[AutoPropertyMode.Parent] = property => property.Component
					.GetComponentsInParent(property.Field.FieldType, true)
					.FirstOrDefault(),
				[AutoPropertyMode.Scene] = property => Object
					.FindObjectOfType(property.Field.FieldType),
				[AutoPropertyMode.Asset] = property =>
				{
					MyEditor.LoadAllAssetsOfType(property.Field.FieldType);
					MyEditor.LoadAllAssetsOfType("Prefab");
					return Resources.FindObjectsOfTypeAll(property.Field.FieldType).FirstOrDefault(AssetDatabase.Contains);
				},
				[AutoPropertyMode.Any] = property =>
				{
					MyEditor.LoadAllAssetsOfType(property.Field.FieldType);
					MyEditor.LoadAllAssetsOfType("Prefab");
					return Resources.FindObjectsOfTypeAll(property.Field.FieldType)
						.FirstOrDefault();
				}
			};

		static AutoPropertyHandler()
		{
			// this event is for GameObjects in the scene.
			MyEditorEvents.OnSave += CheckComponentsInScene;
			// this event is for prefabs saved in edit mode.
			PrefabStage.prefabSaved += CheckComponentsInPrefab;
			PrefabStage.prefabStageOpened += stage => CheckComponentsInPrefab(stage.prefabContentsRoot);
		}

		private static void CheckComponentsInScene()
		{
			var autoProperties = MyEditor.GetFieldsWithAttribute<AutoPropertyAttribute>();
			for (var i = 0; i < autoProperties.Length; i++)
			{
				FillProperty(autoProperties[i]);
			}
		}

		private static void CheckComponentsInPrefab(GameObject prefab)
		{
			var autoProperties = MyEditor.GetFieldsWithAttribute<AutoPropertyAttribute>(prefab);
			for (var i = 0; i < autoProperties.Length; i++)
			{
				FillProperty(autoProperties[i]);
			}
		}

		private static void FillProperty(MyEditor.ComponentField property)
		{
			var apAttribute = property.Field
				.GetCustomAttributes(typeof(AutoPropertyAttribute), true)
				.FirstOrDefault() as AutoPropertyAttribute;
			if (apAttribute == null) return;

			if (property.Field.FieldType.IsArray)
			{
				Object[] components = MultipleObjectsGetters[apAttribute.Mode].Invoke(property);
				if (components != null && components.Length > 0)
				{
					var serializedObject = new SerializedObject(property.Component);
					var serializedProperty = serializedObject.FindProperty(property.Field.Name);
					serializedProperty.ReplaceArray(components);
					serializedObject.ApplyModifiedProperties();
					return;
				}
			}
			else
			{
				var component = SingularObjectGetters[apAttribute.Mode].Invoke(property);
				if (component != null)
				{
					var serializedObject = new SerializedObject(property.Component);
					var serializedProperty = serializedObject.FindProperty(property.Field.Name);
					serializedProperty.objectReferenceValue = component;
					serializedObject.ApplyModifiedProperties();
					return;
				}
			}

			Debug.LogError($"{property.Component.name} caused: {property.Field.Name} is failed to Auto Assign property. No match",
				property.Component.gameObject);
		}
	}
}
#endif