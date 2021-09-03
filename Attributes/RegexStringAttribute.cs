using System.Text.RegularExpressions;
using UnityEngine;

namespace MyBox
{
	/// <summary>
	/// Validate a string field by regex expression
	/// RegexMode.Match will keep only matching content
	/// RegexMode.Replace will keep all except regex match
	/// </summary>
	public class RegexStringAttribute : PropertyAttribute
	{
		public readonly Regex Regex;
		public readonly RegexStringMode AttributeMode;

		public RegexStringAttribute(string regex, RegexStringMode mode = RegexStringMode.Match, RegexOptions options = RegexOptions.None)
		{
			Regex = new Regex(regex, options);
			AttributeMode = mode;
		}
	}

	public enum RegexStringMode
	{
		/// <summary>
		/// Keep only parts of the string that Match the Expression
		/// </summary>
		Match,
		/// <summary>
		/// Remove from the string parts that not match the Expression
		/// </summary>
		Replace,
		/// <summary>
		/// Highlight the field if any of the parts of the string matching the Expression
		/// </summary>
		WarningIfMatch,
		/// <summary>
		/// Highlight the field if some parts of the string not matching the Expression
		/// </summary>
		WarningIfNotMatch
	}
}

#if UNITY_EDITOR
namespace MyBox.Internal
{
	using UnityEditor;
	using EditorTools;

	[CustomPropertyDrawer(typeof(RegexStringAttribute))]
	public class RegexStringAttributeDrawer : PropertyDrawer
	{
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			if (property.propertyType != SerializedPropertyType.String)
			{
				MyGUI.DrawColouredRect(position, MyGUI.Colors.Red);
				EditorGUI.LabelField(position, new GUIContent("", "[RegexStringAttribute] used with non-string property"));
			}
			else
			{
				var regex = (RegexStringAttribute)attribute;
				var mode = regex.AttributeMode;
				bool ifMatch = mode == RegexStringMode.WarningIfMatch;
				bool ifNotMatch = mode == RegexStringMode.WarningIfNotMatch;
				bool anyMatching = regex.Regex.IsMatch(property.stringValue);
				bool warn = (ifMatch && anyMatching) || (ifNotMatch && !anyMatching);
				var originalPosition = position;

				DrawWarning();
				position.width -= 20;
				EditorGUI.PropertyField(position, property, label, true);
				DrawTooltip();

				if (GUI.changed)
				{
					if (mode == RegexStringMode.Replace) OnReplace();
					if (mode == RegexStringMode.Match) OnKeepMatching();
				}

				if (warn)
				{
					GUILayoutUtility.GetRect(GUIContent.none, EditorStyles.objectField);
					position = originalPosition;
					position.y += EditorGUIUtility.singleLineHeight;
					DrawWarning();
					position.x += EditorGUIUtility.labelWidth;
					var warningContent = new GUIContent("Regex rule violated!");
					EditorGUI.LabelField(position, warningContent, EditorStyles.miniBoldLabel);
				}

				property.serializedObject.ApplyModifiedProperties();


				void OnReplace() => property.stringValue = regex.Regex.Replace(property.stringValue, "");
				void OnKeepMatching() => property.stringValue = regex.Regex.KeepMatching(property.stringValue);

				void DrawWarning()
				{
					if (!ifMatch && !ifNotMatch) return;
					MyGUI.DrawColouredRect(position, warn ? MyGUI.Colors.Yellow : Color.clear);
				}

				void DrawTooltip()
				{
					string tooltip = "Regex field: ";
					if (mode == RegexStringMode.Match || mode == RegexStringMode.WarningIfNotMatch) tooltip += "match expression";
					else tooltip += "remove expression";
					tooltip += $"\n[{regex.Regex}]";

					position.x += position.width + 2;
					position.width = 18;
					var tooltipContent = new GUIContent(MyGUI.EditorIcons.Help);
					tooltipContent.tooltip = tooltip;
					EditorGUI.LabelField(position, tooltipContent, EditorStyles.label);
				}
			}
		}
	}
}
#endif