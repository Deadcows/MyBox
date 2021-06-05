using UnityEngine;

namespace MyBox
{
	/// <summary>
	/// Validate a string field to only allow or disallow a set of pre-defined
	/// characters on typing.
	/// </summary>
	public class CharacterRangeAttribute : PropertyAttribute
	{
		public readonly string Characters;
		public readonly bool AllowMode;

		public CharacterRangeAttribute(string characters, bool allowMode = true)
		{ Characters = characters; AllowMode = allowMode; }
	}
}

#if UNITY_EDITOR
namespace MyBox.Internal
{
	using UnityEditor;
	using EditorTools;
	using System.Linq;

	[CustomPropertyDrawer(typeof(CharacterRangeAttribute))]
	public class CharacterRangeAttributeDrawer : PropertyDrawer
	{
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			if (property.propertyType != SerializedPropertyType.String)
			{
				MyGUI.DrawColouredRect(position, MyGUI.Colors.Red);
				EditorGUI.LabelField(position, new GUIContent("", "[CharacterRangeAttribute] used with non-string property"));
			}
			else
			{
				var crAttribute = attribute as CharacterRangeAttribute;
				var disallowedCharacters = property.stringValue.Distinct()
					.Where(c => crAttribute.Characters.Contains(c)
						^ crAttribute.AllowMode);
				property.stringValue = disallowedCharacters.Aggregate(
					property.stringValue,
					(p, c) => p.Replace(c.ToString(), ""));
				property.serializedObject.ApplyModifiedProperties();
			}

			EditorGUI.PropertyField(position, property, label, true);
		}
	}
}
#endif