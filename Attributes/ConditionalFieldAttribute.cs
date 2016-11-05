using System;
using UnityEngine;

/// <summary>
/// Use to display inspector property conditionally based on some other property value
/// </summary>
[AttributeUsage(AttributeTargets.Field)]
public class ConditionalFieldAttribute : PropertyAttribute
{
	public string PropertyToCheck;

	public object CompareValue;
	 
	public ConditionalFieldAttribute(string propertyToCheck, object compareValue = null)
	{
		PropertyToCheck = propertyToCheck;
		CompareValue = compareValue;
	}
}