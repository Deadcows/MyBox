using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class TagAttribute : PropertyAttribute
{
}

#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(TagAttribute))]
public class TagAttributeDrawer : PropertyDrawer
{
	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
	{
		property.stringValue = EditorGUI.TagField(position, label, property.stringValue);
	}
}
#endif