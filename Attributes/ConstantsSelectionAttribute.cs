using System;
using System.Collections.Generic;
using System.Reflection;
using MyBox.EditorTools;
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
	//TODO: DO NOT OVERRIDE current value if it is not found in current constants list. Highlight it instead
	//TODO: Warning if used on field of wrong type
	//TODO: NON FOR STRINGS ONLY! Get type of the property, get constants of the type
	using UnityEditor;

	[CustomPropertyDrawer(typeof(ConstantsSelectionAttribute))]
	public class ConstantsSelectionAttributeDrawer : PropertyDrawer
	{
		private ConstantsSelectionAttribute _attribute;
		private readonly List<FieldInfo> _constants = new List<FieldInfo>(); 
		private string[] _names;
		private string[] _values;
		private int _selectedValueIndex;
		private bool _valueFound;


		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			if (property.propertyType != SerializedPropertyType.String) return;
			
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
				property.stringValue = _values[_selectedValueIndex];
				property.serializedObject.ApplyModifiedProperties();
			}
		}

		private void Initialize(SerializedProperty property)
		{
			_attribute = (ConstantsSelectionAttribute) attribute;

			FieldInfo[] allPublicStatics = _attribute.SelectFromType.GetFields(
				BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy);
			
			// IsLiteral determines if its value is written at compile time and not changeable
			// IsInitOnly determines if the field can be set in the body of the constructor
			// for C# a field which is readonly keyword would have both true but a const field would have only IsLiteral equal to true
			foreach (FieldInfo fi in allPublicStatics)
			{
				if (fi.IsLiteral && !fi.IsInitOnly && fi.FieldType == typeof(string))
					_constants.Add(fi);
			}
			
			if (_constants.IsNullOrEmpty()) return;
			_names = new string[_constants.Count];
			_values = new string[_constants.Count];
			for (var i = 0; i < _constants.Count; i++)
			{
				_names[i] = _constants[i].Name;
				_values[i] = _constants[i].GetValue(null) as string;
			}

			for (var i = 0; i < _values.Length; i++)
			{
				if (property.stringValue == _values[i])
				{
					_valueFound = true;
					_selectedValueIndex = i;
				}
			}
			

			if (!_valueFound)
			{
				_names = _names.InsertAt(0);
				_values = _values.InsertAt(0);
				_names[0] = "NOT FOUND: " + property.stringValue;
				_values[0] = property.stringValue;
			}
		}
	}
}
#endif