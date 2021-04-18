using UnityEngine;

namespace MyBox
{
	public class OverrideLabelAttribute : PropertyAttribute
	{
		public string NewName;

		public OverrideLabelAttribute(string newName) => NewName = newName;
	}

#if UNITY_EDITOR
	namespace Internal
	{
		using UnityEditor;

		[CustomPropertyDrawer(typeof(OverrideLabelAttribute))]
		public class OverrideLabelDrawer : PropertyDrawer
		{
			public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
			{
				label.text = (attribute as OverrideLabelAttribute).NewName;
				EditorGUI.PropertyField(position, property, label);
			}
		}
	}
#endif
}
