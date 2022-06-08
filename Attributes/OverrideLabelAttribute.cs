using UnityEngine;

namespace MyBox
{
	public class OverrideLabelAttribute : PropertyAttribute
	{
		public readonly string NewLabel;

		public OverrideLabelAttribute(string newLabel) => NewLabel = newLabel;
	}
}

#if UNITY_EDITOR
namespace MyBox.Internal
{
	using UnityEditor;

	[CustomPropertyDrawer(typeof(OverrideLabelAttribute))]
	public class OverrideLabelDrawer : PropertyDrawer
	{
		public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
			var customDrawer = CustomDrawerUtility.GetPropertyDrawerForProperty(property, fieldInfo, attribute);
			if (customDrawer != null) return customDrawer.GetPropertyHeight(property, label);
			
			return EditorGUI.GetPropertyHeight(property, label);
		}

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			if (property.isArray) WarningsPool.LogCollectionsNotSupportedWarning(property, nameof(OverrideLabelAttribute));
			
			label.text = ((OverrideLabelAttribute)attribute).NewLabel;

			var customDrawer = CustomDrawerUtility.GetPropertyDrawerForProperty(property, fieldInfo, attribute);
			if (customDrawer != null) customDrawer.OnGUI(position, property, label);
			else EditorGUI.PropertyField(position, property, label, true);
		}
	}
}
#endif