// ---------------------------------------------------------------------------- 
// Author: Ryan Hipple
// Date:   05/01/2018
// Source: https://github.com/roboryantron/UnityEditorJunkie
// ----------------------------------------------------------------------------

using UnityEngine;


namespace MyBox
{
	/// <summary>
	/// Put this attribute on a public (or SerializeField) enum in a
	/// MonoBehaviour or ScriptableObject to get an improved enum selector
	/// popup. The enum list is scrollable and can be filtered by typing.
	/// </summary>
	public class SearchableEnumAttribute : PropertyAttribute
	{
	}
}

#if UNITY_EDITOR
namespace MyBox.EditorTools
{
	using UnityEditor;
	
	/// <summary>
	/// Base class to easily create searchable enum types
	/// </summary>
	public class SearchableEnumDrawer : PropertyDrawer
	{
		private Internal.SearchableEnumAttributeDrawer _drawer;
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			if (_drawer == null) _drawer = new Internal.SearchableEnumAttributeDrawer();
			GUIContent content = new GUIContent(property.displayName);
			Rect drawerRect = EditorGUILayout.GetControlRect(true, _drawer.GetPropertyHeight(property, content));
			_drawer.OnGUI(drawerRect, property, content);
		}
	}
}

namespace MyBox.Internal
{
	using UnityEditor;
	using System;

	/// <summary>
	/// Draws the custom enum selector popup for enum fields using the
	/// SearchableEnumAttribute.
	/// </summary>
	[CustomPropertyDrawer(typeof(SearchableEnumAttribute))]
	public class SearchableEnumAttributeDrawer : PropertyDrawer
	{
		private const string TYPE_ERROR = "SearchableEnum can only be used on enum fields.";

		/// <summary>
		/// Cache of the hash to use to resolve the ID for the drawer.
		/// </summary>
		private int idHash;

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			// If this is not used on an enum, show an error
			if (property.type != "Enum")
			{
				GUIStyle errorStyle = "CN EntryErrorIconSmall";
				Rect r = new Rect(position);
				r.width = errorStyle.fixedWidth;
				position.xMin = r.xMax;
				GUI.Label(r, "", errorStyle);
				GUI.Label(position, TYPE_ERROR);
				return;
			}

			// By manually creating the control ID, we can keep the ID for the
			// label and button the same. This lets them be selected together
			// with the keyboard in the inspector, much like a normal popup.
			if (idHash == 0) idHash = "SearchableEnumAttributeDrawer".GetHashCode();
			int id = GUIUtility.GetControlID(idHash, FocusType.Keyboard, position);

			label = EditorGUI.BeginProperty(position, label, property);
			position = EditorGUI.PrefixLabel(position, id, label);

			GUIContent buttonText = new GUIContent(property.enumDisplayNames[property.enumValueIndex]);
			if (DropdownButton(id, position, buttonText))
			{
				SearchablePopup.Show(position, property.enumDisplayNames, property.enumValueIndex, OnSelect);
				
				void OnSelect(int i)
				{
					property.enumValueIndex = i;
					property.serializedObject.ApplyModifiedProperties();
				}
			}

			EditorGUI.EndProperty();
		}

		/// <summary>
		/// A custom button drawer that allows for a controlID so that we can
		/// sync the button ID and the label ID to allow for keyboard
		/// navigation like the built-in enum drawers.
		/// </summary>
		private static bool DropdownButton(int id, Rect position, GUIContent content)
		{
			Event current = Event.current;
			switch (current.type)
			{
				case EventType.MouseDown:
					if (position.Contains(current.mousePosition) && current.button == 0)
					{
						Event.current.Use();
						return true;
					}

					break;
				case EventType.KeyDown:
					if (GUIUtility.keyboardControl == id && current.character == '\n')
					{
						Event.current.Use();
						return true;
					}

					break;
				case EventType.Repaint:
					EditorStyles.popup.Draw(position, content, id, false);
					break;
			}

			return false;
		}
	}
}
#endif