using System;
using UnityEngine;

namespace MyBox
{
	public class DisplayPreviewAttribute : PropertyAttribute
	{
		public readonly int Width;
		public readonly int Height;
		
		public DisplayPreviewAttribute(int width = 70, int height = 70)
		{
			Width = width;
			Height = height;
		}
	}
}

#if UNITY_EDITOR
namespace MyBox.Internal
{
	using UnityEditor;

	[CustomPropertyDrawer(typeof(DisplayPreviewAttribute))]
	public class DrawSpriteDrawer : PropertyDrawer
	{
		public override float GetPropertyHeight(SerializedProperty property, GUIContent label) =>
			((DisplayPreviewAttribute)attribute).Height;

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			var width = ((DisplayPreviewAttribute)attribute).Width;
			var height = ((DisplayPreviewAttribute)attribute).Height;
			var fieldType = fieldInfo.FieldType.GetElementType() ?? fieldInfo.FieldType;
			
			
			EditorGUI.BeginProperty(new Rect(position.x, position.y, EditorGUIUtility.currentViewWidth, height), label, property);

			EditorGUI.LabelField(position, label);
			var previewRect = new Rect(position.x + EditorGUIUtility.labelWidth, position.y, width, height);
			
			if (IsImageType(fieldType) || property.objectReferenceValue == null) 
				EditorGUI.ObjectField(previewRect, property, fieldType, GUIContent.none);
			else
			{
				var preview = AssetPreview.GetAssetPreview(property.objectReferenceValue);
				if (preview != null) previewRect.width += 18;
				EditorGUI.ObjectField(previewRect, property, fieldType, GUIContent.none);

				if (preview != null)
				{
					previewRect.width -= 20;
					previewRect.height -= 2;
					previewRect.x += 1;
					previewRect.y += 1;
					GUI.DrawTexture(previewRect, preview, ScaleMode.ScaleToFit);
				}
			}
			
			EditorGUI.EndProperty();
			bool IsImageType(Type t) => t == typeof(Sprite) || t == typeof(Texture2D) || t == typeof(Texture);
		}
	}
}
#endif