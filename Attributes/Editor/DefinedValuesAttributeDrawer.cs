using System;
using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(DefinedValuesAttribute))]
public class DefinedValuesAttributeDrawer : PropertyDrawer
{
	private Action<int> _setValue;
	private Func<int, int> _validateValue;
	private string[] _list;

	private string[] List
	{
		get
		{
			if (_list == null)
			{
				_list = new string[DefinedValuesAttribute.ValuesArray.Length];
				for (int i = 0; i < DefinedValuesAttribute.ValuesArray.Length; i++)
				{
					_list[i] = DefinedValuesAttribute.ValuesArray[i].ToString();
				}
			}
			return _list;
		}
	}

	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
	{
		if (_validateValue == null && _setValue == null) SetUp(property);
		
		if (_validateValue == null && _setValue == null)
		{
			base.OnGUI(position, property, label);
			return;
		}

		if (_validateValue == null) return;

		int selectedIndex = 0;
		for (int i = 0; i < List.Length; i++)
		{
			selectedIndex = _validateValue(i);
			if (selectedIndex != 0)
				break;
		}

		EditorGUI.BeginChangeCheck();
		selectedIndex = EditorGUI.Popup(position, label.text, selectedIndex, List);
		if (EditorGUI.EndChangeCheck())
		{
			_setValue(selectedIndex);
		}
	}

	void SetUp(SerializedProperty property)
	{
		if (VariableType == typeof(string))
		{
			_validateValue = (index) => property.stringValue == List[index] ? index : 0;

			_setValue = (index) =>
			{
				property.stringValue = List[index];
			};
		}
		else if (VariableType == typeof(int))
		{
			_validateValue = (index) => property.intValue == Convert.ToInt32(List[index]) ? index : 0;

			_setValue = (index) =>
			{
				property.intValue = Convert.ToInt32(List[index]);
			};
		}
		else if (VariableType == typeof(float))
		{
			_validateValue = (index) => Mathf.Approximately(property.floatValue, Convert.ToSingle(List[index])) ? index : 0;
			_setValue = (index) =>
			{
				property.floatValue = Convert.ToSingle(List[index]);
			};
		}

	}

	private DefinedValuesAttribute DefinedValuesAttribute => (DefinedValuesAttribute)attribute;

	private Type VariableType => DefinedValuesAttribute.ValuesArray[0].GetType();
}