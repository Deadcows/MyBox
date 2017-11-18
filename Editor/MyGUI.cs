using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEditor;
using Object = UnityEngine.Object;

/// <summary>
/// Editor GUI Helpers and Extensions
/// </summary>
public static class MyGUI
{
	
	// Color Presets

	public static Color Red => new Color(.8f, .6f, .6f);

	public static Color Green => new Color(.4f, .6f, .4f);

	public static Color Blue => new Color(.6f, .6f, .8f);

	public static Color Gray => new Color(.3f, .3f, .3f);

	public static Color Yellow => new Color(.8f, .8f, .2f, .6f);

	public static Color Brown => new Color(.7f, .5f, .2f, .6f);

	
	// Characters Presets

	public static string ArrowUp => "▲";

	public static string ArrowDown => "▼";

	public static string ArrowLeft => "◀";

	public static string ArrowRight => "▶";

	public static string ArrowLeftLight => "←";

	public static string ArrowRightLight => "→";

	public static string Cross => "×";
	


	#region Editor Styles

	public static GUIStyle HelpBoxStyle
	{
		get
		{
			var style = new GUIStyle(GUI.skin.GetStyle("HelpBox"));
			style.alignment = TextAnchor.MiddleCenter;
			return style;
		}
	}

	public static GUIStyle ResizableToolbarButtonStyle
	{
		get
		{
			var style = new GUIStyle(EditorStyles.toolbarButton);
			style.fixedHeight = 0;
			return style;
		}
	}

	#endregion


	#region Draw Coloured lines and boxes

	public static void Separator()
	{
		var color = GUI.color;

		EditorGUILayout.Space();
		var spaceRect = EditorGUILayout.GetControlRect();
		var separatorRect = new Rect(spaceRect.position.OffsetY(spaceRect.height / 2), new Vector2(spaceRect.width, 1));

		GUI.color = Color.white;
		GUI.Box(separatorRect, GUIContent.none);
		GUI.color = color;
	}

	public static void DrawLine(Color color, bool withSpace = false)
	{
		if (withSpace) EditorGUILayout.Space();

		var defaultBackgroundColor = GUI.backgroundColor;
		GUI.backgroundColor = color;
		GUILayout.Box("", GUILayout.ExpandWidth(true), GUILayout.Height(1));
		GUI.backgroundColor = defaultBackgroundColor;

		if (withSpace) EditorGUILayout.Space();
	}

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

	public static void DrawColouredRect(Rect rect, Color color)
	{
		var defaultBackgroundColor = GUI.backgroundColor;
		GUI.backgroundColor = color;
		GUI.Box(rect, "");
		GUI.backgroundColor = defaultBackgroundColor;
	}

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


	#region SerializedProperty Array Extensions

	/// <summary>
	/// Get array of property childs, if parent property is array
	/// </summary>
	public static SerializedProperty[] AsArray(this SerializedProperty property)
	{
		List<SerializedProperty> items = new List<SerializedProperty>();
		for (int i = 0; i < property.arraySize; i++)
			items.Add(property.GetArrayElementAtIndex(i));
		return items.ToArray();
	}


	public static T[] AsArray<T>(this SerializedProperty property)
	{
		var propertiesArray = property.AsArray();
		return propertiesArray.Select(s => s.objectReferenceValue).OfType<T>().ToArray();
	}

	/// <summary>
	/// Get array of property childs, if parent property is array
	/// </summary>
	public static IEnumerable<SerializedProperty> AsIEnumerable(this SerializedProperty property)
	{
		for (int i = 0; i < property.arraySize; i++)
			yield return property.GetArrayElementAtIndex(i);
	}

	public static void ReplaceArray(this SerializedProperty property, Object[] newElements)
	{
		property.arraySize = 0;
		property.serializedObject.ApplyModifiedProperties();
		property.arraySize = newElements.Length;
		for (var i = 0; i < newElements.Length; i++)
		{
			property.GetArrayElementAtIndex(i).objectReferenceValue = newElements[i];
		}
		property.serializedObject.ApplyModifiedProperties();
	}

	/// <summary>
	/// If property is array, insert new element at the end and get it as a property
	/// </summary>
	public static SerializedProperty NewElement(this SerializedProperty property)
	{
		int newElementIndex = property.arraySize;
		property.InsertArrayElementAtIndex(newElementIndex);
		return property.GetArrayElementAtIndex(newElementIndex);
	}

	/// <summary>
	/// If property is array, insert new element at the end and get it as a property
	/// </summary>
	public static void MoveArrayElementUpButton(this SerializedProperty property, int elementAt)
	{
		var guiState = GUI.enabled;
		if (elementAt <= 0) GUI.enabled = false;

		if (GUILayout.Button(ArrowUp, EditorStyles.toolbarButton, GUILayout.Width(18)))
			property.MoveArrayElement(elementAt, elementAt - 1);

		GUI.enabled = guiState;
	}

	/// <summary>
	/// If property is array, insert new element at the end and get it as a property
	/// </summary>
	public static void MoveArrayElementDownButton(this SerializedProperty property, int elementAt)
	{
		var guiState = GUI.enabled;
		if (elementAt >= property.arraySize - 1) GUI.enabled = false;

		if (GUILayout.Button(ArrowDown, EditorStyles.toolbarButton, GUILayout.Width(18)))
			property.MoveArrayElement(elementAt, elementAt + 1);

		GUI.enabled = guiState;
	}

	/// <summary>
	/// If property is array, insert new element at the end and get it as a property
	/// </summary>
	public static SerializedProperty NewArrayElementButton(this SerializedProperty property)
	{
		if (GUILayout.Button("+", EditorStyles.toolbarButton, GUILayout.Width(18)))
		{
			return property.NewElement();
		}
		return null;
	}



	public static bool RemoveElementButton()
	{
		return GUILayout.Button(Cross, EditorStyles.toolbarButton, GUILayout.Width(18));
	}

	#endregion


	#region Drop Area

	/// <summary>
	/// Drag'n'Drop Area to catch objects of specific type
	/// </summary>
	/// <typeparam name="T">Asset type to catch</typeparam>
	/// <param name="areaText">Lable to display</param>
	/// <param name="height">Height of the Drop Area</param>
	/// <param name="allowExternal">Allow to drag external files and import as unity assets</param>
	/// <param name="externalImportFolder">Path relative to Assets folder</param>
	/// <returns>Received objects. Null if none catched</returns>
	public static T[] DropArea<T>(string areaText, float height, bool allowExternal = false,
		string externalImportFolder = null) where T : Object
	{
		Event currentEvent = Event.current;
		Rect drop_area = GUILayoutUtility.GetRect(0.0f, height, GUILayout.ExpandWidth(true));
		var style = new GUIStyle(GUI.skin.box);
		style.alignment = TextAnchor.MiddleCenter;
		GUI.Box(drop_area, areaText, style);

		switch (currentEvent.type)
		{
			case EventType.DragUpdated:
			case EventType.DragPerform:
				if (!drop_area.Contains(currentEvent.mousePosition))
					return null;

				DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
				if (currentEvent.type == EventType.DragPerform)
				{
					DragAndDrop.AcceptDrag();
					Event.current.Use();

					List<T> result = new List<T>();

					if (allowExternal && DragAndDrop.paths.Length > 0 && DragAndDrop.paths.Length > DragAndDrop.objectReferences.Length)
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
							var validObject = dragged as T;
							if (validObject == null) validObject = AssetDatabase.LoadAssetAtPath<T>(AssetDatabase.GetAssetPath(dragged));

							if (validObject != null) result.Add(validObject);
						}
					}

					if (result.Count > 0)
						return result.OrderBy(o => o.name).ToArray();
					return null;
				}
				break;
		}
		return null;
	}


	#endregion


	/// <summary>
	/// Creates a filepath textfield with a browse button. Opens the open file panel.
	/// </summary>
	public static string FileLabel(string name, float labelWidth, string path, string extension)
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
	public static string FolderLabel(string name, float labelWidth, string path)
	{
		EditorGUILayout.BeginHorizontal();
		string filepath = EditorGUILayout.TextField(name, path);
		if (GUILayout.Button("Browse", GUILayout.MaxWidth(60)))
		{
			filepath = EditorUtility.SaveFolderPanel(name, path, "Folder");
		}
		EditorGUILayout.EndHorizontal();
		return filepath;
	}


	/// <summary>
	/// Creates a toolbar that is filled in from an Enum. Useful for setting tool modes.
	/// </summary>
	public static Enum EnumToolbar(Enum selected)
	{
		string[] toolbar = Enum.GetNames(selected.GetType());
		Array values = Enum.GetValues(selected.GetType());

		for (int i = 0; i < toolbar.Length; i++)
		{
			string toolname = toolbar[i];
			toolname = toolname.Replace("_", " ");
			toolbar[i] = toolname;
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


	/// <summary>
	/// Creates an array foldout like in inspectors. Hand editable ftw!
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
	
}
