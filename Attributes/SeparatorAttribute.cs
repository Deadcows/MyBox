using UnityEngine;

public class SeparatorAttribute : PropertyAttribute
{
	public readonly string Title;
	public readonly bool WithOffset;


	public SeparatorAttribute()
	{
		Title = "";
	}

	public SeparatorAttribute(string title, bool withOffset = false)
	{
		Title = title;
		WithOffset = withOffset;
	}
}