using System;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

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
	[CustomPropertyDrawer(typeof(DefinedValuesAttribute))]
	public class DefinedValuesAttributeDrawer : PropertyDrawer
	{
		private DefinedValuesAttribute _attribute;
		private Type _variableType;
		private string[] _values;
		private int _selectedValueIndex = -1;


		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			if (_attribute == null) Initialize(property);
			if (_values == null || _values.Length == 0 || _selectedValueIndex < 0)
			{
				EditorGUI.PropertyField(position, property, label);
				return;
			}

			EditorGUI.BeginChangeCheck();
			_selectedValueIndex = EditorGUI.Popup(position, label.text, _selectedValueIndex, _values);
			if (EditorGUI.EndChangeCheck()) ApplyNewValue(property);
		}

		private void Initialize(SerializedProperty property)
		{
			_attribute = (DefinedValuesAttribute) attribute;
			if (_attribute.ValuesArray == null || _attribute.ValuesArray.Length == 0) return;
			_variableType = _attribute.ValuesArray[0].GetType();
			if (TypeMismatch(property)) return;


			_values = new string[_attribute.ValuesArray.Length];
			for (int i = 0; i < _attribute.ValuesArray.Length; i++)
			{
				_values[i] = _attribute.ValuesArray[i].ToString();
			}

			_selectedValueIndex = GetSelectedIndex(property);
		}

		private int GetSelectedIndex(SerializedProperty property)
		{
			for (var i = 0; i < _values.Length; i++)
			{
				if (IsString && property.stringValue == _values[i]) return i;
				if (IsInt && property.intValue == Convert.ToInt32(_values[i])) return i;
				if (IsFloat && Mathf.Approximately(property.floatValue, Convert.ToSingle(_values[i]))) return i;
			}

			return 0;
		}

		private bool TypeMismatch(SerializedProperty property)
		{
			if (IsString && property.propertyType != SerializedPropertyType.String) return true;
			if (IsInt && property.propertyType != SerializedPropertyType.Integer) return true;
			if (IsFloat && property.propertyType != SerializedPropertyType.Float) return true;

			return false;
		}

		private void ApplyNewValue(SerializedProperty property)
		{
			if (IsString) property.stringValue = _values[_selectedValueIndex];
			else if (IsInt) property.intValue = Convert.ToInt32(_values[_selectedValueIndex]);
			else if (IsFloat) property.floatValue = Convert.ToSingle(_values[_selectedValueIndex]);

			property.serializedObject.ApplyModifiedProperties();
		}

		private bool IsString
		{
			get { return _variableType == typeof(string); }
		}

		private bool IsInt
		{
			get { return _variableType == typeof(int); }
		}

		private bool IsFloat
		{
			get { return _variableType == typeof(float); }
		}
	}
}
#endif