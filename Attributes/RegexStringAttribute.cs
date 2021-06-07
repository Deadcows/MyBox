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
		public readonly Mode AttributeMode;

		public RegexStringAttribute(string regex, Mode mode = Mode.Match, RegexOptions options = RegexOptions.None)
		{
			Regex = new Regex(regex, options);
			AttributeMode = mode;
		}

		public enum Mode
		{
			Match,
			Replace,
			WarningIfMatch,
			WarningIfNotMatch
		}

		public static class Expression
		{
			public const string WholeNumbers = @"^\d+$";
			public const string WholeAndDecimalNumbers = @"^\d*(\.\d+)?$";

			public const string AlphanumericWithoutSpace = @"^[a-zA-Z0-9]*$";
			public const string AlphanumericWihSpace = @"^[a-zA-Z0-9 ]*$";

			public const string Email = @"^([a-zA-Z0-9._%-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,6})*$";
			public const string URL = @"(https?:\/\/)?(www\.)?[-a-zA-Z0-9@:%._\+~#=]{2,256}\.[a-z]{2,6}\b([-a-zA-Z0-9@:%_\+.~#?&//=]*)";
		}
	}
}

#if UNITY_EDITOR
namespace MyBox.Internal
{
	using UnityEditor;
	using EditorTools;
	using System.Linq;

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

				DrawWarning();
				position.width -= 20;
				property.stringValue = EditorGUI.DelayedTextField(position, label, property.stringValue);
				//EditorGUI.PropertyField(position, property, label, true);
				DrawTooltip();

				if (regex.AttributeMode == RegexStringAttribute.Mode.Replace) OnReplace();
				if (regex.AttributeMode == RegexStringAttribute.Mode.Match) OnKeepMatching();

				property.serializedObject.ApplyModifiedProperties();


				void OnReplace() => property.stringValue = regex.Regex.Replace(property.stringValue, "");
				void OnKeepMatching() => property.stringValue = regex.Regex.Matches(property.stringValue).Cast<Match>()
					.Aggregate(string.Empty, (a, m) => a + m.Value);

				void DrawWarning()
				{
					bool ifMatch = regex.AttributeMode == RegexStringAttribute.Mode.WarningIfMatch;
					bool ifNotMatch = regex.AttributeMode == RegexStringAttribute.Mode.WarningIfNotMatch;
					if (!ifMatch && !ifNotMatch) return;

					bool anyMatching = regex.Regex.IsMatch(property.stringValue);
					bool warn = (ifMatch && anyMatching) || (ifNotMatch && !anyMatching);
					if (warn) MyGUI.DrawColouredRect(position, MyGUI.Colors.Yellow);
				}
				
				void DrawTooltip()
				{
					string tooltip = "Regex field: ";
					if (regex.AttributeMode == RegexStringAttribute.Mode.Match) tooltip += "match expression";
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