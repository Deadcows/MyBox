using UnityEngine;

/// <summary>
/// Create Popup wint predefined values for string, int or float property
/// </summary>
public class DefinedValuesAttribute : PropertyAttribute 
{
	public object[] ValuesArray;
	public DefinedValuesAttribute(params object[] definedValues)
	{
		ValuesArray = definedValues;
	}
}
