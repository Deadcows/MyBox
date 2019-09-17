using System;

namespace MyBox
{
	[Serializable]
	public class AssetFolderPath
	{
		public string Path;
	}
}

#if UNITY_EDITOR
namespace MyBox.Internal
{
	using UnityEditor;
	using UnityEngine;

	[CustomPropertyDrawer(typeof(AssetFolderPath))]
	public class AssetFolderPathDrawer : PropertyDrawer
	{
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			var pathProperty = property.FindPropertyRelative("Path");
			
			string name = label.text;
			string path = pathProperty.stringValue;

			var buttonWidth = 60;

			Rect textfieldBox = position;
			textfieldBox.width -= buttonWidth + 16;
			
			Rect buttonBox = position;
			buttonBox.width = buttonWidth;
			buttonBox.x = textfieldBox.width + 16;
			
			if (!path.Contains(Application.dataPath))
			{
				path = Application.dataPath;
			}

			string truncatedPath = path.Substring(Application.dataPath.Length);
			string filepath = Application.dataPath + EditorGUI.TextField(textfieldBox, name, truncatedPath);
			if (GUI.Button(buttonBox, "Browse"))
			{
				var newPath = EditorUtility.SaveFolderPanel(name, path, "Select Folder");
				if (newPath.Length > 0) filepath = newPath;
				
				if (!filepath.Contains(Application.dataPath))
				{
					Debug.LogError("Please choose a folder that is in this Assets Folder");

					pathProperty.stringValue = path;
					return;
				}
			}

			pathProperty.stringValue = filepath;
		}
	}
}
#endif