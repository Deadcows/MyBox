#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEditor;
using Object = UnityEngine.Object;

namespace MyBox.EditorTools
{
	public static class MyGUI
	{
		#region Colors

		public static class Colors
		{
			public static readonly Color Red = new Color(.8f, .6f, .6f);

			public static readonly Color Green = new Color(.4f, .6f, .4f);

			public static readonly Color Blue = new Color(.6f, .6f, .8f);

			public static readonly Color Gray = new Color(.3f, .3f, .3f);

			public static readonly Color Yellow = new Color(.8f, .8f, .2f, .6f);

			public static readonly Color Brown = new Color(.7f, .5f, .2f, .6f);
		}

		#endregion


		#region Characters

		public static class Characters
		{
			public const string ArrowUp = "▲";

			public const string ArrowDown = "▼";

			public const string ArrowLeft = "◀";

			public const string ArrowRight = "▶";

			public const string ArrowLeftLight = "←";

			public const string ArrowRightLight = "→";

			public const string ArrowTopRightLight = "↘";

			public const string Check = "✓";

			public const string Cross = "×";
		}

		#endregion


		#region Editor Icons

		public static class EditorIcons
		{
			public static GUIContent Plus => EditorGUIUtility.IconContent("Toolbar Plus");
			public static GUIContent Minus => EditorGUIUtility.IconContent("Toolbar Minus");
			public static GUIContent Refresh => EditorGUIUtility.IconContent("Refresh");

			public static GUIContent ConsoleInfo => EditorGUIUtility.IconContent("console.infoicon.sml");
			public static GUIContent ConsoleWarning => EditorGUIUtility.IconContent("console.warnicon.sml");
			public static GUIContent ConsoleError => EditorGUIUtility.IconContent("console.erroricon.sml");

			public static GUIContent Check => EditorGUIUtility.IconContent("FilterSelectedOnly");
			public static GUIContent Cross => EditorGUIUtility.IconContent("d_winbtn_win_close");

			public static GUIContent Dropdown => EditorGUIUtility.IconContent("icon dropdown");

			public static GUIContent EyeOn => EditorGUIUtility.IconContent("d_VisibilityOn");
			public static GUIContent EyeOff => EditorGUIUtility.IconContent("d_VisibilityOff");
			public static GUIContent Zoom => EditorGUIUtility.IconContent("d_ViewToolZoom");

			public static GUIContent Help => EditorGUIUtility.IconContent("_Help");
			public static GUIContent Favourite => EditorGUIUtility.IconContent("Favorite");
			public static GUIContent Label => EditorGUIUtility.IconContent("FilterByLabel");

			public static GUIContent Settings => EditorGUIUtility.IconContent("d_Settings");
			public static GUIContent SettingsPopup => EditorGUIUtility.IconContent("_Popup");
			public static GUIContent SettingsMixer => EditorGUIUtility.IconContent("Audio Mixer");

			public static GUIContent Circle => EditorGUIUtility.IconContent("TestNormal");
			public static GUIContent CircleYellow => EditorGUIUtility.IconContent("TestInconclusive");
			public static GUIContent CircleDotted => EditorGUIUtility.IconContent("TestIgnored");
			public static GUIContent CircleRed => EditorGUIUtility.IconContent("TestFailed");
		}

		#endregion


		#region Editor Styles

		/// <summary>
		/// HelpBox with centered text alignment
		/// </summary>
		public static GUIStyle HelpBoxStyle
		{
			get
			{
				if (_helpBoxStyle != null) return _helpBoxStyle;
				_helpBoxStyle = new GUIStyle(GUI.skin.GetStyle("HelpBox"));
				_helpBoxStyle.alignment = TextAnchor.MiddleCenter;
				return _helpBoxStyle;
			}
		}

		private static GUIStyle _helpBoxStyle;


		/// <summary>
		/// ToolbarButtonStyle is not resizable by default
		/// </summary>
		public static GUIStyle ResizableToolbarButtonStyle
		{
			get
			{
				if (_resizableToolbarButtonStyle != null) return _resizableToolbarButtonStyle;
				_resizableToolbarButtonStyle = new GUIStyle(EditorStyles.toolbarButton);
				_resizableToolbarButtonStyle.fixedHeight = 0;
				return _resizableToolbarButtonStyle;
			}
		}

		private static GUIStyle _resizableToolbarButtonStyle;


		/// <summary>
		/// ToolbarButton with Border, Margin and Padding set to 0
		/// </summary>
		public static GUIStyle BorderlessToolbarButtonStyle
		{
			get
			{
				if (_borderlessToolbarButton != null) return _borderlessToolbarButton;
				_borderlessToolbarButton = new GUIStyle(ResizableToolbarButtonStyle);
				var emptyOffset = new RectOffset();
				_borderlessToolbarButton.border = emptyOffset;
				_borderlessToolbarButton.margin = emptyOffset;
				_borderlessToolbarButton.padding = emptyOffset;
				return _borderlessToolbarButton;
			}
		}

		private static GUIStyle _borderlessToolbarButton;


		/// <summary>
		/// Style for a toggle button
		/// </summary>
		public static GUIStyle ButtonToggledStyle(bool toggled)
		{
			if (!toggled) return EditorStyles.miniButton;

			if (_buttonToggledStyle != null) return _buttonToggledStyle;
			_buttonToggledStyle = new GUIStyle(EditorStyles.miniButton);
			_buttonToggledStyle.normal.background = _buttonToggledStyle.active.background;
			return _buttonToggledStyle;
		}

		private static GUIStyle _buttonToggledStyle;


		/// <summary>
		/// MiniButtonLeft/Middle/Right style based on array index
		/// </summary>
		/// <param name="index"></param>
		/// <param name="collection"></param>
		/// <returns></returns>
		public static GUIStyle MiniButtonStyle(int index, Array collection)
		{
			if (collection.Length == 1) return EditorStyles.miniButton;
			if (index == 0) return EditorStyles.miniButtonLeft;
			if (index == collection.Length - 1) return EditorStyles.miniButtonRight;
			return EditorStyles.miniButtonMid;
		}

		#endregion


		#region Draw Coloured lines and boxes

		/// <summary>
		/// Draw Separator within GuiLayout
		/// </summary>
		public static void Separator()
		{
			var color = GUI.color;

			EditorGUILayout.Space();
			var spaceRect = EditorGUILayout.GetControlRect();
			var separatorRectPosition = new Vector2(spaceRect.position.x, spaceRect.position.y + spaceRect.height / 2);
			var separatorRect = new Rect(separatorRectPosition, new Vector2(spaceRect.width, 1));

			GUI.color = Color.white;
			GUI.Box(separatorRect, GUIContent.none);
			GUI.color = color;
		}

		/// <summary>
		/// Draw Line within GUILayout
		/// </summary>
		public static void DrawLine(Color color, bool withSpace = false)
		{
			if (withSpace) EditorGUILayout.Space();

			var defaultBackgroundColor = GUI.backgroundColor;
			GUI.backgroundColor = color;
			GUILayout.Box("", GUILayout.ExpandWidth(true), GUILayout.Height(1));
			GUI.backgroundColor = defaultBackgroundColor;

			if (withSpace) EditorGUILayout.Space();
		}

		/// <summary>
		/// Draw line within Rect and get Rect back with offset
		/// </summary>
		public static Rect DrawLine(Color color, Rect rect)
		{
			var h = rect.height;
			var defaultBackgroundColor = GUI.backgroundColor;
			GUI.backgroundColor = color;
			rect.y += 5;
			rect.height = 1;
			GUI.Box(rect, "");
			rect.y += 5;
			GUI.backgroundColor = defaultBackgroundColor;
			rect.height = h;
			return rect;
		}

		/// <summary>
		/// Draw Rect filled with Color
		/// </summary>
		public static void DrawColouredRect(Rect rect, Color color)
		{
			var defaultBackgroundColor = GUI.backgroundColor;
			GUI.backgroundColor = color;
			GUI.Box(rect, "");
			GUI.backgroundColor = defaultBackgroundColor;
		}

		/// <summary>
		/// Draw background Line within GUILayout
		/// </summary>
		public static void DrawBackgroundLine(Color color, int yOffset = 0)
		{
			var defColor = GUI.color;
			GUI.color = color;
			var rect = GUILayoutUtility.GetLastRect();
			rect.center = new Vector2(rect.center.x, rect.center.y + 6 + yOffset);
			rect.height = 17;
			GUI.DrawTexture(rect, EditorGUIUtility.whiteTexture);
			GUI.color = defColor;
		}

		/// <summary>
		/// Draw background Line of height
		/// </summary>
		public static void DrawBackgroundBox(Color color, int height, int yOffset = 0)
		{
			var defColor = GUI.color;
			GUI.color = color;
			var rect = GUILayoutUtility.GetLastRect();
			rect.center = new Vector2(rect.center.x, rect.center.y + 6 + yOffset);
			rect.height = height;
			GUI.DrawTexture(rect, EditorGUIUtility.whiteTexture);
			GUI.color = defColor;
		}

		#endregion


		#region Property Field

		/// <summary>
		/// Make a field for SerializedProperty and check if changed
		/// </summary>
		public static bool PropertyField(SerializedProperty property, params GUILayoutOption[] options)
		{
			EditorGUI.BeginChangeCheck();
			EditorGUILayout.PropertyField(property, options);
			return EditorGUI.EndChangeCheck();
		}

		/// <summary>
		/// Make a field for SerializedProperty and check if changed
		/// </summary>
		public static bool PropertyField(SerializedProperty property, GUIContent label, params GUILayoutOption[] options)
		{
			EditorGUI.BeginChangeCheck();
			EditorGUILayout.PropertyField(property, label, options);
			return EditorGUI.EndChangeCheck();
		}

		#endregion


		#region SerializedProperty Manipulation Buttons

		/// <summary>
		/// Move array element at index on index-1 position
		/// </summary>
		public static void MoveArrayElementUpButton(this SerializedProperty property, int elementAt)
		{
			var guiState = GUI.enabled;
			if (elementAt <= 0) GUI.enabled = false;

			if (UpButton())
			{
				EditorApplication.delayCall += () =>
				{
					property.MoveArrayElement(elementAt, elementAt - 1);
					property.serializedObject.ApplyModifiedProperties();
				};
			}

			GUI.enabled = guiState;
		}


		/// <summary>
		/// Move array element at index on index+1 position
		/// </summary>
		public static void MoveArrayElementDownButton(this SerializedProperty property, int elementAt)
		{
			var guiState = GUI.enabled;
			if (elementAt >= property.arraySize - 1) GUI.enabled = false;

			if (DownButton())
			{
				EditorApplication.delayCall += () =>
				{
					property.MoveArrayElement(elementAt, elementAt + 1);
					property.serializedObject.ApplyModifiedProperties();
				};
			}

			GUI.enabled = guiState;
		}

		/// <summary>
		/// Add new array element to property and get it as SerializedProperty
		/// </summary>
		public static SerializedProperty NewArrayElementButton(this SerializedProperty property)
		{
			if (PlusButton())
			{
				return property.NewElement();
			}

			return null;
		}

		/// <summary>
		/// Remove array element at index
		/// </summary>
		public static void RemoveElementButton(this SerializedProperty property, int elementAt)
		{
			var guiState = GUI.enabled;
			if (elementAt < 0 || elementAt >= property.arraySize - 1) GUI.enabled = false;

			if (CrossButton())
			{
				EditorApplication.delayCall += () =>
				{
					property.DeleteArrayElementAtIndex(elementAt);
					property.serializedObject.ApplyModifiedProperties();
				};
			}

			GUI.enabled = guiState;
		}

		#endregion


		#region Drop Area

		/// <summary>
		/// Drag-and-Drop Area to catch objects of specific type
		/// </summary>
		/// <typeparam name="T">Asset type to catch</typeparam>
		/// <param name="areaText">Label to display</param>
		/// <param name="height">Height of the Drop Area</param>
		/// <param name="allowExternal">Allow to drag external files and import as unity assets</param>
		/// <param name="externalImportFolder">Path relative to Assets folder</param>
		/// <returns>Received objects. Null if none received</returns>
		public static T[] DropArea<T>(string areaText, float height, bool allowExternal = false,
			string externalImportFolder = null) where T : Object
		{
			Event currentEvent = Event.current;
			Rect dropArea = GUILayoutUtility.GetRect(0.0f, height, GUILayout.ExpandWidth(true));
			var style = new GUIStyle(GUI.skin.box);
			style.alignment = TextAnchor.MiddleCenter;
			GUI.Box(dropArea, areaText, style);

			bool dragEvent = currentEvent.type == EventType.DragUpdated || currentEvent.type == EventType.DragPerform;
			if (!dragEvent) return null;
			bool overDropArea = dropArea.Contains(currentEvent.mousePosition);
			if (!overDropArea) return null;

			DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
			if (currentEvent.type != EventType.DragPerform) return null;

			DragAndDrop.AcceptDrag();
			Event.current.Use();

			List<T> result = new List<T>();

			bool anyExternal = DragAndDrop.paths.Length > 0 &&
			                   DragAndDrop.paths.Length > DragAndDrop.objectReferences.Length;
			if (allowExternal && anyExternal)
			{
				var folderToLoad = "/";
				if (!string.IsNullOrEmpty(externalImportFolder))
				{
					folderToLoad = "/" + externalImportFolder.Replace("Assets/", "").Trim('/', '\\') + "/";
				}

				List<string> importedFiles = new List<string>();

				foreach (string externalPath in DragAndDrop.paths)
				{
					if (externalPath.Length == 0) continue;
					try
					{
						var filename = Path.GetFileName(externalPath);
						var relativePath = folderToLoad + filename;
						Directory.CreateDirectory(Application.dataPath + folderToLoad);
						FileUtil.CopyFileOrDirectory(externalPath, Application.dataPath + relativePath);
						importedFiles.Add("Assets" + relativePath);
					}
					catch (Exception ex)
					{
						Debug.LogException(ex);
					}
				}

				AssetDatabase.Refresh();

				foreach (var importedFile in importedFiles)
				{
					var asset = AssetDatabase.LoadAssetAtPath<T>(importedFile);
					if (asset != null)
					{
						result.Add(asset);
						Debug.Log("Asset imported at path: " + importedFile);
					}
					else AssetDatabase.DeleteAsset(importedFile);
				}
			}
			else
			{
				foreach (Object dragged in DragAndDrop.objectReferences)
				{
					var validObject = dragged as T ?? AssetDatabase.LoadAssetAtPath<T>(AssetDatabase.GetAssetPath(dragged));

					if (validObject != null) result.Add(validObject);
				}
			}

			return result.Count > 0 ? result.OrderBy(o => o.name).ToArray() : null;
		}


		/// <summary>
		/// Drag-and-Drop Area to get paths of received objects
		/// </summary>
		/// <param name="areaText">Label to display</param>
		/// <param name="height">Height of the Drop Area</param>
		/// <returns>Received paths</returns>
		public static string[] DropAreaPaths(string areaText, float height)
		{
			Event currentEvent = Event.current;
			Rect dropArea = GUILayoutUtility.GetRect(0.0f, height, GUILayout.ExpandWidth(true));
			var style = new GUIStyle(GUI.skin.box);
			style.alignment = TextAnchor.MiddleCenter;
			GUI.Box(dropArea, areaText, style);

			bool dragEvent = currentEvent.type == EventType.DragUpdated || currentEvent.type == EventType.DragPerform;
			if (!dragEvent) return null;
			bool overDropArea = dropArea.Contains(currentEvent.mousePosition);
			if (!overDropArea) return null;

			DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
			if (currentEvent.type != EventType.DragPerform) return null;

			DragAndDrop.AcceptDrag();
			Event.current.Use();

			return DragAndDrop.paths;
		}

		#endregion


		#region Browse Buttons

		/// <summary>
		/// Creates a filepath textfield with a browse button. Opens the open file panel.
		/// </summary>
		public static string BrowsFileLabel(string name, float labelWidth, string path, string extension)
		{
			EditorGUILayout.BeginHorizontal();
			GUILayout.Label(name, GUILayout.MaxWidth(labelWidth));
			string filepath = EditorGUILayout.TextField(path);
			if (GUILayout.Button("Browse"))
			{
				filepath = EditorUtility.OpenFilePanel(name, path, extension);
			}

			EditorGUILayout.EndHorizontal();
			return filepath;
		}

		/// <summary>
		/// Creates a folder path textfield with a browse button. Opens the save folder panel.
		/// </summary>
		public static string BrowseFolderLabel(string name, float labelWidth, string path)
		{
			EditorGUILayout.BeginHorizontal();
			string filepath = EditorGUILayout.TextField(name, path, GUILayout.MaxWidth(labelWidth));
			if (GUILayout.Button("Browse", GUILayout.MaxWidth(60)))
			{
				filepath = EditorUtility.SaveFolderPanel(name, path, "Folder");
			}

			EditorGUILayout.EndHorizontal();
			return filepath;
		}

		#endregion


		#region Predefined Buttons

		/// <summary>
		/// Display Button with ArrowUI
		/// </summary>
		public static bool UpButton()
		{
			return GUILayout.Button(Characters.ArrowUp, EditorStyles.toolbarButton, GUILayout.Width(18));
		}

		public static bool DownButton()
		{
			return GUILayout.Button(Characters.ArrowDown, EditorStyles.toolbarButton, GUILayout.Width(18));
		}

		public static bool PlusButton()
		{
			return GUILayout.Button("+", EditorStyles.toolbarButton, GUILayout.Width(18));
		}


		public static bool CrossButton()
		{
			return GUILayout.Button(Characters.Cross, EditorStyles.toolbarButton, GUILayout.Width(18));
		}

		#endregion


		#region EnumToolbar - Toolbar buttons out of Enum

		/// <summary>
		/// Creates a toolbar that is filled in from an Enum. Useful for setting tool modes.
		/// </summary>
		public static Enum EnumToolbar(Enum selected)
		{
			string[] toolbar = Enum.GetNames(selected.GetType());
			Array values = Enum.GetValues(selected.GetType());

			for (int i = 0; i < toolbar.Length; i++)
			{
				string toolName = toolbar[i];
				toolName = toolName.Replace("_", " ");
				toolbar[i] = toolName;
			}

			int selectedIndex = 0;
			while (selectedIndex < values.Length)
			{
				if (selected.ToString() == values.GetValue(selectedIndex).ToString())
				{
					break;
				}

				selectedIndex++;
			}

			selectedIndex = GUILayout.Toolbar(selectedIndex, toolbar);
			return (Enum)values.GetValue(selectedIndex);
		}

		#endregion


		#region SearchablePopup

		/// <summary>
		/// A popup window that displays a list of options and may use a search string to filter the displayed content
		/// </summary>
		/// <param name="activatorRect">Rectangle of the button that triggered the popup</param>
		/// <param name="options">List of strings to choose from</param>
		/// <param name="current">Index of the currently selected string</param>
		/// <param name="onSelectionMade">Callback to trigger when a choice is made</param>
		public static void SearchablePopup(Rect activatorRect, string[] options, int current, Action<int> onSelectionMade)
		{
			Internal.SearchablePopup.Show(activatorRect, options, current, onSelectionMade);
		}
		
		/// <summary>
		/// A popup window that displays a list of options and may use a search string to filter the displayed content
		/// </summary>
		/// <param name="options">List of strings to choose from</param>
		/// <param name="current">Index of the currently selected string</param>
		/// <param name="onSelectionMade">Callback to trigger when a choice is made</param>
		public static void SearchablePopup(string[] options, int current, Action<int> onSelectionMade)
		{
			var position = new Rect(Event.current.mousePosition, Vector2.zero);
			Internal.SearchablePopup.Show(position, options, current, onSelectionMade);
		}

		#endregion


		/// <summary>
		/// Creates an array foldout like in inspectors
		/// </summary>
		public static string[] ArrayFoldout(string label, string[] array, ref bool foldout)
		{
			EditorGUILayout.BeginVertical();

			EditorGUIUtility.labelWidth = 0;
			EditorGUIUtility.fieldWidth = 0;
			foldout = EditorGUILayout.Foldout(foldout, label);
			string[] newArray = array;
			if (foldout)
			{
				EditorGUILayout.BeginHorizontal();
				EditorGUILayout.Space();
				EditorGUILayout.BeginVertical();
				int arraySize = EditorGUILayout.IntField("Size", array.Length);
				if (arraySize != array.Length)
					newArray = new string[arraySize];
				for (int i = 0; i < arraySize; i++)
				{
					string entry = "";
					if (i < array.Length)
						entry = array[i];
					newArray[i] = EditorGUILayout.TextField("Element " + i, entry);
				}

				EditorGUILayout.EndVertical();
				EditorGUILayout.EndHorizontal();
			}

			EditorGUILayout.EndVertical();
			return newArray;
		}


#if UNITY_IMAGECONVERSION_ENABLED
		/// <summary>
		/// Display Eye/Crossed Eye button
		/// </summary>
		public static bool EyeButton(bool on)
		{
			if (_eyeOpenContent == null) _eyeOpenContent = new GUIContent(MyIconPresets.IconEye16);
			if (_eyeClosedContent == null) _eyeClosedContent = new GUIContent(MyIconPresets.IconEyeCrossed16);

			return GUILayout.Button(on ? _eyeOpenContent : _eyeClosedContent, EditorStyles.toolbarButton, GUILayout.Width(20));
		}

		private static GUIContent _eyeOpenContent;
		private static GUIContent _eyeClosedContent;
#endif
	}
}
#endif