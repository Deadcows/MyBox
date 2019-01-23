using System;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[AttributeUsage(AttributeTargets.Field)]
public class ConditionalFieldAttribute : PropertyAttribute
{
    private readonly string _propertyToCheck;
    private readonly object _compareValue;

    public ConditionalFieldAttribute(string propertyToCheck, object compareValue = null)
    {
        _propertyToCheck = propertyToCheck;
        _compareValue = compareValue;
    }

#if UNITY_EDITOR
    
    // TODO: Skip array fields
    public bool CheckBehaviourPropertyVisible(MonoBehaviour behaviour, string propertyName)
    {
        if (_propertyToCheck.IsNullOrEmpty()) return true;

        var so = new SerializedObject(behaviour);
        var property = so.FindProperty(propertyName);

        return CheckPropertyVisible(property);

    }
    
    public bool CheckPropertyVisible(SerializedProperty property)
    {
        var conditionProperty = FindRelativeProperty(_propertyToCheck);
        if (conditionProperty == null) return true;


        bool isBoolMatch = conditionProperty.propertyType == SerializedPropertyType.Boolean &&
                           conditionProperty.boolValue;
        string compareStringValue = _compareValue?.ToString().ToUpper() ?? "NULL";
        if (isBoolMatch && compareStringValue == "FALSE") isBoolMatch = false;

        string conditionPropertyStringValue = AsStringValue(conditionProperty).ToUpper();
        bool objectMatch = compareStringValue == conditionPropertyStringValue;

        bool notVisible = !isBoolMatch && !objectMatch;
        return !notVisible;


        SerializedProperty FindRelativeProperty(string toGet)
        {
            if (property.depth == 0) return property.serializedObject.FindProperty(toGet);

            var path = property.propertyPath.Replace(".Array.data[", "[");
            var elements = path.Split('.');
            SerializedProperty parent = null;


            for (int i = 0; i < elements.Length - 1; i++)
            {
                var element = elements[i];
                int index = -1;
                if (element.Contains("["))
                {
                    index = Convert.ToInt32(element.Substring(element.IndexOf("[", StringComparison.Ordinal))
                        .Replace("[", "").Replace("]", ""));
                    element = element.Substring(0, element.IndexOf("[", StringComparison.Ordinal));
                }

                parent = i == 0
                    ? property.serializedObject.FindProperty(element)
                    : parent.FindPropertyRelative(element);

                if (index >= 0) parent = parent.GetArrayElementAtIndex(index);
            }

            return parent.FindPropertyRelative(toGet);
        }

        string AsStringValue(SerializedProperty prop)
        {
            switch (prop.propertyType)
            {
                case SerializedPropertyType.String:
                    return prop.stringValue;

                case SerializedPropertyType.Character:
                case SerializedPropertyType.Integer:
                    if (prop.type == "char") return Convert.ToChar(prop.intValue).ToString();
                    return prop.intValue.ToString();

                case SerializedPropertyType.ObjectReference:
                    return prop.objectReferenceValue != null ? prop.objectReferenceValue.ToString() : "null";

                case SerializedPropertyType.Boolean:
                    return prop.boolValue.ToString();

                case SerializedPropertyType.Enum:
                    return prop.enumNames[prop.enumValueIndex];

                default:
                    return string.Empty;
            }
        }
    }
#endif
}