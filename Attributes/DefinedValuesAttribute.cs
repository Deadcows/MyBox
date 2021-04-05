using System;
using System.Linq;
using UnityEngine;

namespace MyBox
{
	/// <summary>
	/// Create Popup with predefined values for string, int or float property
	/// </summary>
	public class DefinedValuesAttribute : PropertyAttribute
	{
		public readonly object[] ValuesArray;

		public DefinedValuesAttribute(params object[] definedValues)
		{
			ValuesArray = definedValues;
		}
	}
}

#if UNITY_EDITOR
namespace MyBox.Internal
{
	using UnityEditor;

	[CustomPropertyDrawer(typeof(DefinedValuesAttribute))]
	public class DefinedValuesAttributeDrawer : PropertyDrawer
	{
		private string[] _values;

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			var values = ((DefinedValuesAttribute) attribute).ValuesArray;

			if (values.IsNullOrEmpty() || !TypeMatch(values[0].GetType(), property))
			{
				EditorGUI.PropertyField(position, property, label);
				return;
			}

			if (_values.IsNullOrEmpty()) _values = values.Select(v => v.ToString()).ToArray();

			var valType = values[0].GetType();
			bool isString = valType == typeof(string);
			bool isInt = valType == typeof(int);
			bool isFloat = valType == typeof(float);

			EditorGUI.BeginChangeCheck();
			var newIndex = EditorGUI.Popup(position, label.text, GetSelectedIndex(), _values);
			if (EditorGUI.EndChangeCheck()) ApplyNewValue(_values[newIndex]);


			int GetSelectedIndex()
			{
				for (var i = 0; i < _values.Length; i++)
				{
					if (isString && property.stringValue == _values[i]) return i;
					if (isInt && property.intValue == Convert.ToInt32(_values[i])) return i;
					if (isFloat && Mathf.Approximately(property.floatValue, Convert.ToSingle(_values[i]))) return i;
				}

				return 0;
			}

			void ApplyNewValue(string newValue)
			{
				if (isString) property.stringValue = newValue;
				if (isInt) property.intValue = Convert.ToInt32(newValue);
				if (isFloat) property.floatValue = Convert.ToSingle(newValue);

				property.serializedObject.ApplyModifiedProperties();
			}
		}

		private bool TypeMatch(Type valType, SerializedProperty property)
		{
			if (valType == typeof(string) && property.propertyType == SerializedPropertyType.String) return true;
			if (valType == typeof(int) && property.propertyType == SerializedPropertyType.Integer) return true;
			if (valType == typeof(float) && property.propertyType == SerializedPropertyType.Float) return true;

			return false;
		}
	}
}
#endif