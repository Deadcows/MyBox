using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(InspectorTypeEnforcerAttribute))]
public class InspectorTypeEnforcerPropertyDrawer : PropertyDrawer
{
	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
	{
		var tempColor = GUI.color;

		var enforcedType = ((InspectorTypeEnforcerAttribute)attribute).EnforcedType;
		if (property.objectReferenceValue != null && !enforcedType.IsInstanceOfType(property.objectReferenceValue))
		{
			GUI.color = Color.red;
			label.text = "!! " + label.text;
			if (string.IsNullOrEmpty(label.tooltip))
			{
				label.tooltip = "Error: Object does not implement/inherit from Type: " + enforcedType.Name;
			}
		}
		else if (!string.IsNullOrEmpty(label.tooltip))
		{
			label.tooltip = string.Empty;
		}
		
		EditorGUI.PropertyField(position, property, label);
		
		GUI.color = tempColor;
	}
}