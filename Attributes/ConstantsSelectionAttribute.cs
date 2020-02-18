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
	//TODO: DO NOT OVERRIDE current value if it is not found in current constants list. Highlight it instead
	//TODO: Warning if used on field of wrong type
	//TODO: NON FOR STRINGS ONLY! Get type of the property, get constants of the type
	using UnityEditor;

	[CustomPropertyDrawer(typeof(ConstantsSelectionAttribute))]
	public class ConstantsSelectionAttributeDrawer : PropertyDrawer
	{
		private ConstantsSelectionAttribute _attribute;
		private string[] _names;
		private string[] _values;
		private int _selectedValueIndex = -1;


		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			if (_attribute == null) Initialize(property);
			if (_values.IsNullOrEmpty() || _selectedValueIndex < 0)
			{
				EditorGUI.PropertyField(position, property, label);
				return;
			}

			EditorGUI.BeginChangeCheck();
			_selectedValueIndex = EditorGUI.Popup(position, label.text, _selectedValueIndex, _names);
			if (EditorGUI.EndChangeCheck())
			{
				property.stringValue = _values[_selectedValueIndex];
			}
		}

		private void Initialize(SerializedProperty property)
		{
			_attribute = (ConstantsSelectionAttribute) attribute;

			List<FieldInfo> constants = new List<FieldInfo>();
			FieldInfo[] allPublicStatics = _attribute.SelectFromType.GetFields(
				BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy);
			
			// IsLiteral determines if its value is written at compile time and not changeable
			// IsInitOnly determines if the field can be set in the body of the constructor
			// for C# a field which is readonly keyword would have both true but a const field would have only IsLiteral equal to true
			foreach (FieldInfo fi in allPublicStatics)
			{
				if (fi.IsLiteral && !fi.IsInitOnly && fi.FieldType == typeof(string))
					constants.Add(fi);
			}
			
			if (constants.IsNullOrEmpty()) return;
			_names = new string[constants.Count];
			_values = new string[constants.Count];
			for (var i = 0; i < constants.Count; i++)
			{
				_names[i] = constants[i].Name;
				_values[i] = constants[i].GetValue(null) as string;
			}
			
			_selectedValueIndex = GetSelectedIndex(property);
		}

		
		private int GetSelectedIndex(SerializedProperty property)
		{
			for (var i = 0; i < _values.Length; i++)
			{
				if (property.stringValue == _values[i]) return i;
			}
			
			return 0;
		}
	}
}
#endif