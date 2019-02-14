using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class LayerAttribute : PropertyAttribute
{
}

#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(LayerAttribute))]
public class LayerAttributeDrawer : PropertyDrawer
{
	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
	{
		property.intValue = EditorGUI.LayerField(position, label, property.intValue);
	}
}
#endif