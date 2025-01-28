using System.Text.RegularExpressions;
using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

namespace MyBox
{
	public static class MyString
	{
		public static bool IsNullOrEmpty(this string str) => string.IsNullOrEmpty(str);
		public static bool NotNullOrEmpty(this string str) => !string.IsNullOrEmpty(str);

		public static string RemoveStart(this string str, string remove)
		{
			int index = str.IndexOf(remove, StringComparison.Ordinal);
			return index < 0 ? str : str.Remove(index, remove.Length);
		}
		
		public static string RemoveEnd(this string str, string remove)
		{
			if (!str.EndsWith(remove)) return str;
			return str.Remove(str.LastIndexOf(remove, StringComparison.Ordinal));
		}
		
		/// <summary>
		/// "Camel case string" => "CamelCaseString" 
		/// </summary>
		public static string ToCamelCase(this string message) {
			message = message.Replace("-", " ").Replace("_", " ");
			message = CultureInfo.InvariantCulture.TextInfo.ToTitleCase(message);
			message = message.Replace(" ", "");
			return message;
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
		/// Convert a string value to an Enum value.
		/// </summary>
		public static T AsEnum<T>(this string source, bool ignoreCase = true) where T : Enum => (T) Enum.Parse(typeof(T), source, ignoreCase);


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
		/// Get the "message" string with the "surround" string at the both sides 
		/// </summary>
		public static string SurroundedWith(this string message, string surround) => surround + message + surround;
		
		/// <summary>
		/// Get the "message" string with the "start" at the beginning and "end" at the end of the string
		/// </summary>
		public static string SurroundedWith(this string message, string start, string end) => start + message + end;

		/// <summary>
		/// Surround string with "color" tag
		/// </summary>
		public static string Colored(this string message, Colors color) => $"<color={ColorHash[color]}>{message}</color>";

		/// <summary>
		/// Surround string with "color" tag
		/// </summary>
		public static string Colored(this string message, Color color) => $"<color={color.ToHex()}>{message}</color>";

		/// <summary>
		/// Surround string with "color" tag
		/// </summary>
		public static string Colored(this string message, string colorCode) => $"<color={colorCode}>{message}</color>";

		/// <summary>
		/// Surround string with "size" tag
		/// </summary>
		public static string Sized(this string message, int size) => $"<size={size}>{message}</size>";
		
		/// <summary>
		/// Surround string with "u" tag
		/// </summary>
		public static string Underlined(this string message) => $"<u>{message}</u>";

		/// <summary>
		/// Surround string with "b" tag
		/// </summary>
		public static string Bold(this string message) => $"<b>{message}</b>";

		/// <summary>
		/// Surround string with "i" tag
		/// </summary>
		public static string Italics(this string message) => $"<i>{message}</i>";
		
		
		public static Dictionary<Colors, string> ColorHash = new Dictionary<Colors, string>
		{
			{Colors.aqua, "#00ffffff"},
			{Colors.black, "#000000ff"},
			{Colors.blue, "#0000ffff"},
			{Colors.brown, "#a52a2aff"},
			{Colors.cyan, "#00ffffff"},
			{Colors.darkblue, "#0000a0ff"},
			{Colors.fuchsia, "#ff00ffff"},
			{Colors.green, "#008000ff"},
			{Colors.grey, "#808080ff"},
			{Colors.lightblue, "#add8e6ff"},
			{Colors.lime, "#00ff00ff"},
			{Colors.magenta, "#ff00ffff"},
			{Colors.maroon, "#800000ff"},
			{Colors.navy, "#000080ff"},
			{Colors.olive, "#808000ff"},
			{Colors.purple, "#800080ff"},
			{Colors.red, "#ff0000ff"},
			{Colors.silver, "#c0c0c0ff"},
			{Colors.teal, "#008080ff"},
			{Colors.white, "#ffffffff"},
			{Colors.yellow, "#ffff00ff"}
		};
	}

	
	/// <summary>
	/// Represents list of supported by Unity Console color names
	/// </summary>
	public enum Colors
	{
		// ReSharper disable InconsistentNaming
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
		// ReSharper restore InconsistentNaming
	}
}