using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace MyBox
{
	/// <summary>
	/// Use to display inspector of property object
	/// </summary>
	public class DisplayInspectorAttribute : PropertyAttribute
	{
		public readonly bool DisplayScript;

		public DisplayInspectorAttribute(bool displayScriptField = true)
		{
			DisplayScript = displayScriptField;
		}
	}
}

#if UNITY_EDITOR
namespace MyBox.Internal
{
	[CustomPropertyDrawer(typeof(DisplayInspectorAttribute))]
	public class DisplayInspectorAttributeDrawer : PropertyDrawer
	{
		private DisplayInspectorAttribute Instance
		{
			get { return _instance ?? (_instance = attribute as DisplayInspectorAttribute); }
		}

		private DisplayInspectorAttribute _instance;

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			if (Instance.DisplayScript || property.objectReferenceValue == null)
			{
				position.height = EditorGUI.GetPropertyHeight(property);
				EditorGUI.PropertyField(position, property);
				position.y += EditorGUI.GetPropertyHeight(property) + 4;
			}

			if (property.objectReferenceValue != null)
			{
				var startY = position.y;
				float startX = position.x;

				var propertyObject = new SerializedObject(property.objectReferenceValue).GetIterator();
				propertyObject.Next(true);
				propertyObject.NextVisible(false);

				var xPos = position.x + 10;
				var width = position.width - 10;

				while (propertyObject.NextVisible(propertyObject.isExpanded))
				{
					position.x = xPos + 10 * propertyObject.depth;
					position.width = width - 10 * propertyObject.depth;

					position.height = propertyObject.isExpanded ? 16 : EditorGUI.GetPropertyHeight(propertyObject);
					EditorGUI.PropertyField(position, propertyObject);
					position.y += propertyObject.isExpanded ? 20 : EditorGUI.GetPropertyHeight(propertyObject) + 4;
				}

				var bgRect = new Rect(position);
				bgRect.y = startY - 5;
				bgRect.x = startX - 10;
				bgRect.height = position.y - startY;
				bgRect.width = 10;
				DrawColouredRect(bgRect, new Color(.6f, .6f, .8f, .5f));

				if (GUI.changed) propertyObject.serializedObject.ApplyModifiedProperties();
			}

			if (GUI.changed) property.serializedObject.ApplyModifiedProperties();
		}

		public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
			if (property.objectReferenceValue == null) return base.GetPropertyHeight(property, label);

			float height = Instance.DisplayScript ? EditorGUI.GetPropertyHeight(property) + 4 : 0;

			var propertyObject = new SerializedObject(property.objectReferenceValue).GetIterator();
			propertyObject.Next(true);
			propertyObject.NextVisible(true);

			while (propertyObject.NextVisible(propertyObject.isExpanded))
			{
				height += propertyObject.isExpanded ? 20 : EditorGUI.GetPropertyHeight(propertyObject) + 4;
			}

			return height;
		}

		private void DrawColouredRect(Rect rect, Color color)
		{
			var defaultBackgroundColor = GUI.backgroundColor;
			GUI.backgroundColor = color;
			GUI.Box(rect, "");
			GUI.backgroundColor = defaultBackgroundColor;
		}
	}
}
#endif