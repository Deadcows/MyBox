using System.Text.RegularExpressions;
using System;

namespace MyBox
{
	public static class MyString
	{
		/// <summary>
		/// "Camel case string" => "CamelCaseString" 
		/// </summary>
		public static string ToCamelCase(this string camelCaseString)
		{
			return Regex.Replace(camelCaseString, "([a-z](?=[A-Z])|[A-Z](?=[A-Z][a-z]))", "$1 ").Trim();
		}

		/// <summary>
		/// "CamelCaseString" => "Camel Case String"
		/// </summary>
		public static string SplitCamelCase(this string camelCaseString)
		{
			if (string.IsNullOrEmpty(camelCaseString)) return camelCaseString;

			string camelCase = Regex.Replace(Regex.Replace(camelCaseString, @"(\P{Ll})(\P{Ll}\p{Ll})", "$1 $2"), @"(\p{Ll})(\P{Ll})", "$1 $2");
			string firstLetter = camelCase.Substring(0, 1).ToUpper();

			if (camelCaseString.Length > 1)
			{
				string rest = camelCase.Substring(1);

				return firstLetter + rest;
			}

			return firstLetter;
		}
		

		/// <summary>
		/// Number presented in Roman numerals
		/// </summary>
		public static string ToRoman(this int i)
		{
			if (i > 999) return "M" + ToRoman(i - 1000);
			if (i > 899) return "CM" + ToRoman(i - 900);
			if (i > 499) return "D" + ToRoman(i - 500);
			if (i > 399) return "CD" + ToRoman(i - 400);
			if (i > 99) return "C" + ToRoman(i - 100);
			if (i > 89) return "XC" + ToRoman(i - 90);
			if (i > 49) return "L" + ToRoman(i - 50);
			if (i > 39) return "XL" + ToRoman(i - 40);
			if (i > 9) return "X" + ToRoman(i - 10);
			if (i > 8) return "IX" + ToRoman(i - 9);
			if (i > 4) return "V" + ToRoman(i - 5);
			if (i > 3) return "IV" + ToRoman(i - 4);
			if (i > 0) return "I" + ToRoman(i - 1);
			return "";
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

		/// <summary>
		/// Convert a string value to an Enum value.
		/// </summary>
		public static T AsEnum<T>(this string source, bool ignoreCase = true)
			where T : System.Enum =>
			(T) Enum.Parse(typeof(T), source, ignoreCase);
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
}