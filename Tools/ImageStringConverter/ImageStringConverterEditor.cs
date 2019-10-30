#if UNITY_EDITOR
using System;
using System.IO;
using MyBox.EditorTools;
using UnityEditor;
using UnityEngine;

namespace MyBox.Internal
{
	/// <summary>
	/// This tool allows to save image as a string and load image back from that string.
	/// It allows to store simple textures as const strings right in script files.
	/// I suggest to use tinypng.com before conversion! 
	/// </summary>
	public class ImageStringConverterEditor : EditorWindow
	{
		[MenuItem("Tools/MyBox/String Image Converter", false, 50)]
		private static void CreateWindow()
		{
			GetWindow<ImageStringConverterEditor>(false, "String Image Converter").Show();
		}

		private string _representation;
		private int _width = 64;
		private int _height = 64;

		private Texture2D _texture;

		private void OnGUI()
		{
			var selected = MyGUI.DropAreaPaths("Drag Texture", 20);
			using (new EditorGUILayout.HorizontalScope())
			{
				_width = EditorGUILayout.IntField("Width", _width);
				_height = EditorGUILayout.IntField("Width", _height);
			}

			if (_texture != null) EditorGUILayout.LabelField(new GUIContent(_texture), GUILayout.Width(_width), GUILayout.Height(_height));

			if (selected == null || selected.Length == 0) return;

			string content = Convert.ToBase64String(File.ReadAllBytes(selected[0]));
			_representation = content;
			
			MyEditor.CopyToClipboard(_representation);
			ShowNotification(new GUIContent(selected[0] + "\nCopied to Clipboard as string"));

			_texture = ImageStringConverter.ImageFromString(_representation, _width, _height);
		}
	}
}
#endif