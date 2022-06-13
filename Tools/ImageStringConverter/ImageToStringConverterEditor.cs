#if UNITY_IMAGECONVERSION_ENABLED
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
	public class ImageToStringConverterEditor : EditorWindow
	{
		[MenuItem("Tools/MyBox/Image To String Converter", false, 50)]
		private static void CreateWindow()
		{
			var window = GetWindow<ImageToStringConverterEditor>(false, "Image To String Converter");
			window.minSize = new Vector2(400, 300);
			window.maxSize = new Vector2(400, 800);
			window.Show();
		}

		private string _representation;
		private int _width = 64;
		private int _height = 64;

		private Texture2D _texture;

		private void OnGUI()
		{
			EditorGUILayout.Space();
			EditorGUILayout.HelpBox("1: Drag any image to \"Drag Texture\" field below \n" +
			                        "	(from any place, not just an assets folder)\n" +
			                        "* it's better to optimize it before, try tinypng.com\n" +
			                        "* image size is very important too, big pics will cost a lot!\n" +
			                        "2: Store the result as a const string in your sources\n" +
			                        "3: Convert the string into image during runtime with method \n" +
			                        "	\"ImageStringConverter.ImageFromString\"", MessageType.Info);
			EditorGUILayout.Space();
			
			var selected = MyGUI.DropAreaPaths("Drag Texture", 60);
			GUI.enabled = false;
			EditorGUILayout.TextArea(_representation);
			GUI.enabled = true;

			EditorGUILayout.Space();
			using (new EditorGUILayout.HorizontalScope())
			{
				EditorGUILayout.LabelField("Preview Width: ", GUILayout.Width(120));
				_width = EditorGUILayout.IntField(GUIContent.none, _width, GUILayout.Width(60));
				EditorGUILayout.LabelField("Preview Height: ", GUILayout.Width(120));
				_height = EditorGUILayout.IntField(GUIContent.none, _height, GUILayout.Width(60));
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
#endif