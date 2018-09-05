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
		return $"<color={color}>{message}</color>";
	}

	/// <summary>
	/// Surround string with "color" tag
	/// </summary>
	public static string Colored(this string message, string colorCode)
	{
		return $"<color={colorCode}>{message}</color>";
	}

	/// <summary>
	/// Surround string with "size" tag
	/// </summary>
	public static string Sized(this string message, int size)
	{
		return $"<size={size}>{message}</size>";
	}

	/// <summary>
	/// Surround string with "b" tag
	/// </summary>
	public static string Bold(this string message)
	{
		return $"<b>{message}</b>";
	}

	/// <summary>
	/// Surround string with "i" tag
	/// </summary>
	public static string Italics(this string message)
	{
		return $"<i>{message}</i>";
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