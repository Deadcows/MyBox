using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(ConditionalFieldAttribute))]
public class ConditionalFieldAttributeDrawer : PropertyDrawer
{

	private ConditionalFieldAttribute Attribute => _attribute ?? (_attribute = attribute as ConditionalFieldAttribute);

	private string PropertyToCheck => Attribute != null ? _attribute.PropertyToCheck : null;

	private object CompareValue => Attribute != null ? _attribute.CompareValue : null;


	private ConditionalFieldAttribute _attribute;
	private bool _toShow = true;

	public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
	{
		return _toShow ? EditorGUI.GetPropertyHeight(property) : 0;
	}

	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
	{
		if (!PropertyToCheck.IsNullOrEmpty())
		{
			var conditionProperty = property.serializedObject.FindProperty(PropertyToCheck);
			if (conditionProperty != null)
			{
				bool isBoolMatch = conditionProperty.propertyType == SerializedPropertyType.Boolean && conditionProperty.boolValue;
				string compareStringValue = CompareValue?.ToString().ToUpper() ?? "NULL";
				if (isBoolMatch && compareStringValue == "FALSE") isBoolMatch = false;

				string conditionPropertyStringValue = conditionProperty.AsStringValue().ToUpper();
				bool objectMatch = compareStringValue == conditionPropertyStringValue;

				if (!isBoolMatch && !objectMatch)
				{
					_toShow = false;
					return;
				}
			}
		}

		_toShow = true;
		EditorGUI.PropertyField(position, property, label);
	}

}