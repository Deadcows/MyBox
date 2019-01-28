using System;

public static class MyString
{

	public static string ToCamelCase(this string camelCaseString)
	{
		return System.Text.RegularExpressions.Regex.Replace(camelCaseString, "([a-z](?=[A-Z])|[A-Z](?=[A-Z][a-z]))", "$1 ").Trim();
	}
	

	/// <summary>
	/// Surround string with "color" tag
	/// </summary>
	public static string Colored(this string message, Colors color)
	{
		return string.Format("<color={0}>{1}</color>", color, message);
	}

	/// <summary>
	/// Surround string with "color" tag
	/// </summary>
	public static string Colored(this string message, string colorCode)
	{
		return string.Format("<color={0}>{1}</color>", colorCode, message);
	}

	/// <summary>
	/// Surround string with "size" tag
	/// </summary>
	public static string Sized(this string message, int size)
	{
		return string.Format("<size={0}>{1}</size>", size, message);
	}

	/// <summary>
	/// Surround string with "b" tag
	/// </summary>
	public static string Bold(this string message)
	{
		return string.Format("<b>{0}</b>", message);
	}

	/// <summary>
	/// Surround string with "i" tag
	/// </summary>
	public static string Italics(this string message)
	{
		return string.Format("<i>{0}</i>", message);
	}
}

public enum Colors
{
	aqua,
	black,
	blue,
	brown,
	cyan,
	darkblue,
	fuchsia,
	green,
	grey,
	lightblue,
	lime,
	magenta,
	maroon,
	navy,
	olive,
	purple,
	red,
	silver,
	teal,
	white,
	yellow
}