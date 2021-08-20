// ---------------------------------------------------------------------------- 
// Author: Anton
// https://github.com/antontidev
// ----------------------------------------------------------------------------

using System;
using System.Linq;
using UnityEngine;

namespace MyBox
{
	/// <summary>
	/// Used to pick scene from inspector.
	/// Consider to use <see cref="SceneReference"/> type instead as it is more flexible
	/// </summary>
	[AttributeUsage(AttributeTargets.Field)]
	public class SceneAttribute : PropertyAttribute
	{
	}
}

#if UNITY_EDITOR
namespace MyBox.Internal
{
	using UnityEditor;

	[CustomPropertyDrawer(typeof(SceneAttribute))]
	public class SceneDrawer : PropertyDrawer
	{
		private SceneAttribute _attribute;
		private string[] _scenesInBuild;
		private int _index;

		private void Initialize(string initialValue)
		{
			if (_attribute != null) return;

			_attribute = (SceneAttribute)attribute;
			
			_scenesInBuild = new string[EditorBuildSettings.scenes.Length + 1];

			_index = 0;
			for (var i = 0; i < EditorBuildSettings.scenes.Length; i++)
			{
				var formatted = EditorBuildSettings.scenes[i].path.Split('/').Last().Replace(".unity", string.Empty);
				if (initialValue == formatted) _index = i + 1;
				formatted += $" [{i}]";
				_scenesInBuild[i + 1] = formatted;
			}

			var defaultValue = "NULL";
			if (initialValue.NotNullOrEmpty() && _index == 0) defaultValue = "NOT FOUND: " + initialValue;
			_scenesInBuild[0] = defaultValue;
		}
		
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			if (property.propertyType != SerializedPropertyType.String)
			{
				EditorGUI.LabelField(position, label.text, "Use [Scene] with strings.");
				return;
			}
			
			Initialize(property.stringValue);
			
			var newIndex = EditorGUI.Popup(position, label.text, _index, _scenesInBuild);
			if (newIndex != _index)
			{
				_index = newIndex;
				var value = _scenesInBuild[_index];
				property.stringValue = newIndex == 0 ? string.Empty : value.Substring(0, value.IndexOf('[') - 1);
				property.serializedObject.ApplyModifiedProperties();
			}
		}
	}
}
#endif