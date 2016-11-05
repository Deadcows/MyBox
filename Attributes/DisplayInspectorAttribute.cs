using UnityEngine;

/// <summary>
/// Use to display inspector of property object
/// </summary>
public class DisplayInspectorAttribute : PropertyAttribute
{

	public bool DisplayScript;

	public DisplayInspectorAttribute(bool displayScriptField = true)
	{
		DisplayScript = displayScriptField;
	}

}
