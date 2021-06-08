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
		Match,
		Replace,
		WarningIfMatch,
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
				var regex = (RegexStringAttribute) attribute;
				var mode = regex.AttributeMode;

				DrawWarning();
				position.width -= 20;
				GUI.SetNextControlName("FilteredField");
				EditorGUI.PropertyField(position, property, label, true);
				DrawTooltip();

				if (GUI.changed)
				{
					if (mode == RegexStringMode.Replace) OnReplace();
					if (mode == RegexStringMode.Match) OnKeepMatching();
				}

				property.serializedObject.ApplyModifiedProperties();


				void OnReplace() => property.stringValue = regex.Regex.Replace(property.stringValue, "");
				void OnKeepMatching() => property.stringValue = regex.Regex.KeepMatching(property.stringValue);

				void DrawWarning()
				{
					var focused = GUI.GetNameOfFocusedControl() == "FilteredField";
					bool ifMatch = mode == RegexStringMode.WarningIfMatch;
					bool ifNotMatch = mode == RegexStringMode.WarningIfNotMatch;
					if (!ifMatch && !ifNotMatch) return;

					bool anyMatching = regex.Regex.IsMatch(property.stringValue);
					bool warn = (ifMatch && anyMatching) || (ifNotMatch && !anyMatching);
					if (warn) MyGUI.DrawColouredRect(position, MyGUI.Colors.Yellow);
					
					if (focused) GUI.FocusControl("FilteredField");
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