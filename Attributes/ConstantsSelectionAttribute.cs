using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace MyBox
{
	public class ConstantsSelectionAttribute : PropertyAttribute
	{
		public readonly Type SelectFromType;

		public ConstantsSelectionAttribute(Type type)
		{
			SelectFromType = type;
		}
	}
}

#if UNITY_EDITOR
namespace MyBox.Internal
{
	using UnityEditor;
	using EditorTools;

	//TODO: Allow to set custom value
	[CustomPropertyDrawer(typeof(ConstantsSelectionAttribute))]
	public class ConstantsSelectionAttributeDrawer : PropertyDrawer
	{
		private ConstantsSelectionAttribute _attribute;
		private readonly List<FieldInfo> _constants = new List<FieldInfo>();
		private string[] _names;
		private object[] _values;
		private Type _targetType;
		private int _selectedValueIndex;
		private bool _valueFound;

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			if (_attribute == null) Initialize(property);
			if (_values.IsNullOrEmpty() || _selectedValueIndex < 0)
			{
				EditorGUI.PropertyField(position, property, label);
				return;
			}

			if (!_valueFound && _selectedValueIndex == 0) MyGUI.DrawColouredRect(position, MyGUI.Yellow);

			EditorGUI.BeginChangeCheck();
			_selectedValueIndex = EditorGUI.Popup(position, label.text, _selectedValueIndex, _names);
			if (EditorGUI.EndChangeCheck())
			{
				fieldInfo.SetValue(property.serializedObject.targetObject, _values[_selectedValueIndex]);
				property.serializedObject.ApplyModifiedProperties();
				EditorUtility.SetDirty(property.serializedObject.targetObject);
			}
		}

		private object GetValue(SerializedProperty property)
		{
			return fieldInfo.GetValue(property.serializedObject.targetObject);
		}
		
		private void Initialize(SerializedProperty property)
		{
			_attribute = (ConstantsSelectionAttribute) attribute;
			_targetType = fieldInfo.FieldType;
			
			
			FieldInfo[] allPublicStatics = _attribute.SelectFromType.GetFields(
				BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy);

			// IsLiteral determines if its value is written at compile time and not changeable
			// IsInitOnly determines if the field can be set in the body of the constructor
			// for C# a field which is readonly keyword would have both true but a const field would have only IsLiteral equal to true
			foreach (FieldInfo publicField in allPublicStatics)
			{
				if ((publicField.IsInitOnly || publicField.IsLiteral) && publicField.FieldType == _targetType)
					_constants.Add(publicField);
			}

			if (_constants.IsNullOrEmpty()) return;
			_names = new string[_constants.Count];
			_values = new object[_constants.Count];
			for (var i = 0; i < _constants.Count; i++)
			{
				_names[i] = _constants[i].Name;
				_values[i] = _constants[i].GetValue(null);
			}

			var currentValue = GetValue(property);
			for (var i = 0; i < _values.Length; i++)
			{
				if (currentValue.Equals(_values[i]))
				{
					_valueFound = true;
					_selectedValueIndex = i;
				}
			}

			if (!_valueFound)
			{
				_names = _names.InsertAt(0);
				_values = _values.InsertAt(0);
				var actualValue = GetValue(property);
				_names[0] = "NOT FOUND: " + actualValue;
				_values[0] = actualValue;
			}
		}
	}
}
#endif
