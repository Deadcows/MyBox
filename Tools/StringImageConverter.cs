using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace MyBox
{
	
	/// <summary>
	/// This tool allows to save image as a string and load image back from that string.
	/// It allows to store simple textures as const strings right in script files.
	/// I suggest to use tinypng.com before conversion! 
	/// </summary>
	public class StringImageConverter : EditorWindow
	{

		/// <summary>
		/// Use "Tools/MyBox/String Image Converter" to get string image representation
		/// </summary>
		public static Texture2D ConvertFromString(string source, int width, int height)
		{
			var bytes = Convert.FromBase64String(source);
			var texture = new Texture2D(width, height);
			texture.LoadImage(bytes);
			return texture;
		}

		
		[MenuItem("Tools/MyBox/String Image Converter", false, 50)]
		private static void CreateWindow()
		{
			GetWindow<StringImageConverter>(false, "String Image Converter").Show();
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
			
			CopyToClipboard(_representation);
			ShowNotification(new GUIContent(selected[0] + "\nCopied to Clipboard as string"));

			_texture = ConvertFromString(_representation, _width, _height);
		}
		void CopyToClipboard(string text)
		{
			TextEditor te = new TextEditor();
			te.text = text;
			te.SelectAll();
			te.Copy();
		}
	}
}