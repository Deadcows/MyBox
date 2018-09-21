using System;
using UnityEngine;

/// <summary>
/// Example of use : [ButtonAttribute ("MethodName", "ButtonNameInInspector", "TooltipInInspector", true, true, "param1", 10, "param3")]
/// </summary>
[AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
public sealed class ButtonAttribute : PropertyAttribute
{
	public readonly string Function;
	public readonly string ButtonName;
	public readonly object[] Parameters;

	public readonly bool OnlyInPlayMode;
	public readonly string Tooltip;

	public readonly bool DisplayVariable;

	/// <summary>
	/// WARNING : not working with overloaded functions
	/// </summary>
	public ButtonAttribute(string function, string name, string tooltip, bool onlyInPlayMode, bool displayVariable, params object[] parameters)
	{
		Function = function;
		ButtonName = name;

		OnlyInPlayMode = onlyInPlayMode;
		Tooltip = tooltip;

		DisplayVariable = displayVariable;

		Parameters = parameters;
	}
}