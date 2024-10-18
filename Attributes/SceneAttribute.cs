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
		private string[] _scenesInBuildFormatted;

		private void Initialize(string initialValue)
		{
			if (_attribute != null) return;

			_attribute = (SceneAttribute)attribute;
			
			_scenesInBuild = new string[EditorBuildSettings.scenes.Length + 1];
			_scenesInBuildFormatted = new string[_scenesInBuild.Length];
			
			for (var i = 0; i < EditorBuildSettings.scenes.Length; i++)
			{
				var scene = EditorBuildSettings.scenes[i].path.Split('/').Last().Replace(".unity", string.Empty);
				var formattedScene = scene + $" [{i}]";
				_scenesInBuild[i + 1] = scene;
				_scenesInBuildFormatted[i + 1] = formattedScene;
			}

			var defaultValue = "NULL";
			if (initialValue.NotNullOrEmpty() && CurrentIndex(initialValue) == 0) 
				defaultValue = "NOT FOUND: " + initialValue;
			_scenesInBuildFormatted[0] = defaultValue;
		}

		private int CurrentIndex(string currentValue)
		{
			if (_scenesInBuild.Length <= 1 || currentValue.IsNullOrEmpty()) return 0;
			
			for (var i = 0; i < _scenesInBuild.Length; i++)
			{
				if (_scenesInBuild[i] == currentValue) return i;
			}

			return 0;
		}
		
		
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			if (property.propertyType != SerializedPropertyType.String)
			{
				EditorGUI.LabelField(position, label.text, "Use [Scene] with strings.");
				return;
			}
			Initialize(property.stringValue);

			
			var index = CurrentIndex(property.stringValue);
			var newIndex = EditorGUI.Popup(position, label.text, index, _scenesInBuildFormatted);
			if (newIndex != index)
			{
				property.stringValue = newIndex == 0 ? string.Empty : _scenesInBuild[newIndex];
				property.serializedObject.ApplyModifiedProperties();
			}
		}
	}
}
#endif