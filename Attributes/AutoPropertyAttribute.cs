using System;
using UnityEngine;

namespace MyBox
{
	/// <summary>
	/// Automatically assign components from this GO to this Property.
	/// </summary>
	[AttributeUsage(AttributeTargets.Field)]
	public class AutoPropertyAttribute : PropertyAttribute
	{
	}
}

#if UNITY_EDITOR
namespace MyBox.Internal
{
	using UnityEditor;
	using EditorTools;
	using Object = UnityEngine.Object;

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
		static AutoPropertyHandler()
		{
			MyEditorEvents.OnSave += CheckComponents;
		}

		private static void CheckComponents()
		{
			var autoProperties = MyEditor.GetFieldsWithAttribute<AutoPropertyAttribute>();
			for (var i = 0; i < autoProperties.Length; i++)
			{
				FillProperty(autoProperties[i]);
			}
		}
		
		private static void FillProperty(MyEditor.ComponentField property)
		{
			var propertyType = property.Field.FieldType;

			if (property.Field.FieldType.IsArray)
			{
				var underlyingType = propertyType.GetElementType();
				Object[] components = property.Component.GetComponentsInChildren(underlyingType, true);
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
				var component = property.Component.GetComponentInChildren(propertyType, true);
				if (component != null)
				{
					var serializedObject = new SerializedObject(property.Component);
					var serializedProperty = serializedObject.FindProperty(property.Field.Name);
					serializedProperty.objectReferenceValue = component;
					serializedObject.ApplyModifiedProperties();
					return;
				}
			}

			Debug.LogError(string.Format("{0} caused: {1} is failed to Auto Assign property. No match",
					property.Component.name, property.Field.Name),
				property.Component.gameObject);
		}
	}
}
#endif