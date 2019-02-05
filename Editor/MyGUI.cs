using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using MyBox;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using Object = UnityEngine.Object;

/// <summary>
/// Editor GUI Helpers and Extensions
/// </summary>
public static class MyGUI
{
    // Color Presets

    public static readonly Color Red = new Color(.8f, .6f, .6f);

    public static readonly Color Green = new Color(.4f, .6f, .4f);

    public static readonly Color Blue = new Color(.6f, .6f, .8f);

    public static readonly Color Gray = new Color(.3f, .3f, .3f);

    public static readonly Color Yellow = new Color(.8f, .8f, .2f, .6f);

    public static readonly Color Brown = new Color(.7f, .5f, .2f, .6f);


    // Characters Presets

    public const string ArrowUp = "▲";

    public const string ArrowDown = "▼";

    public const string ArrowLeft = "◀";

    public const string ArrowRight = "▶";

    public const string ArrowLeftLight = "←";

    public const string ArrowRightLight = "→";

    public const string ArrowTopRightLight = "↘";

    public const string Cross = "×";


    #region Editor Styles

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


    public static GUIStyle ImageBasedButtonStyle
    {
        get
        {
            if (_imageBasedButton != null) return _imageBasedButton;
            _imageBasedButton = new GUIStyle(ResizableToolbarButtonStyle);
            _imageBasedButton.border = new RectOffset();
            _imageBasedButton.margin = new RectOffset();
            _imageBasedButton.padding = new RectOffset();
            return _imageBasedButton;
        }
    }
    private static GUIStyle _imageBasedButton;
    

    public static GUIStyle MiniButton(int index, Array collection)
    {
        if (collection.Length == 1) return EditorStyles.miniButton;
        if (index == 0) return EditorStyles.miniButtonLeft;
        if (index == collection.Length - 1) return EditorStyles.miniButtonRight;
        return EditorStyles.miniButtonMid;
    }

    #endregion


    #region Draw Coloured lines and boxes

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

    #region SerializedProperty GetParent

    // Found here http://answers.unity.com/answers/425602/view.html
    public static object GetParent(this SerializedProperty prop)
    {
        var path = prop.propertyPath.Replace(".Array.data[", "[");
        object obj = prop.serializedObject.targetObject;
        var elements = path.Split('.');
        foreach (var element in elements.Take(elements.Length - 1))
        {
            if (element.Contains("["))
            {
                var elementName = element.Substring(0, element.IndexOf("["));
                var index = Convert.ToInt32(element.Substring(element.IndexOf("[")).Replace("[", "").Replace("]", ""));
                obj = GetValueAt(obj, elementName, index);
            }
            else
            {
                obj = GetValue(obj, element);
            }
        }

        return obj;
    }

    private static object GetValue(object source, string name)
    {
        if (source == null)
            return null;
        var type = source.GetType();
        var f = type.GetField(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
        if (f == null)
        {
            var p = type.GetProperty(name,
                BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
            if (p == null)
                return null;
            return p.GetValue(source, null);
        }

        return f.GetValue(source);
    }

    private static object GetValueAt(object source, string name, int index)
    {
        var enumerable = GetValue(source, name) as IEnumerable;
        if (enumerable == null) return null;

        var enm = enumerable.GetEnumerator();
        while (index-- >= 0)
            enm.MoveNext();
        return enm.Current;
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
        /*
        List<string> paths = new List<string>();

        bool anyExternal = DragAndDrop.paths.Length > 0 && DragAndDrop.paths.Length > DragAndDrop.objectReferences.Length;
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
                    paths.Add(asset);
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

                if (validObject != null) paths.Add(validObject);
            }
        }

        return paths.Count > 0 ? paths.OrderBy(o => o.name).ToArray() : null;
        */
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
        return (Enum) values.GetValue(selectedIndex);
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


    public static FieldInfo GetFieldInfo(this SerializedProperty property)
    {
        var targetObject = property.serializedObject.targetObject;
        var targetType = targetObject.GetType();
        return targetType.GetField(property.propertyPath);
    }

    public static bool IsAttributeDefined<T>(this SerializedProperty property) where T : Attribute
    {
        var fieldInfo = property.GetFieldInfo();
        if (fieldInfo == null) return false;
        return Attribute.IsDefined(fieldInfo, typeof(T));
    }


    #region Reordable Collection

    public class ReorderableCollection
    {
        public bool IsExpanded
        {
            get { return _property.isExpanded; }
            set { _property.isExpanded = value; }
        }

        public void Draw()
        {
            if (!_property.isExpanded) DrawHeader();
            else _list.DoLayoutList();
        }

        public Action<SerializedProperty, Rect, int> CustomDrawer;
        public Func<int, bool> CustomRemove;

        private ReorderableList _list;
        private SerializedProperty _property;

        public ReorderableCollection(SerializedProperty property, bool withAddButton = true,
            bool withRemoveButton = true)
        {
            _property = property;
            CreateList(property, withAddButton, withRemoveButton);
        }

        ~ReorderableCollection()
        {
            _property = null;
            _list = null;
        }

        private void DrawHeader()
        {
            EditorGUILayout.BeginHorizontal();
            _property.isExpanded = EditorGUILayout.ToggleLeft(string.Format("{0}[]", _property.displayName),
                _property.isExpanded,
                EditorStyles.boldLabel);
            EditorGUILayout.LabelField(string.Format("size: {0}", _property.arraySize));
            EditorGUILayout.EndHorizontal();
        }

        private void CreateList(SerializedProperty property, bool withAddButton, bool withRemoveButton)
        {
            _list = new ReorderableList(property.serializedObject, property, true, true, withAddButton,
                withRemoveButton);
            _list.onChangedCallback += list => Apply();
            _list.onRemoveCallback += RemoveElement;
            _list.drawHeaderCallback += DrawElementHeader;
            _list.onCanRemoveCallback += (list) => _list.count > 0;
            _list.drawElementCallback += DrawElement;
            _list.elementHeightCallback += GetElementHeight;
        }

        private void DrawElementHeader(Rect rect)
        {
            _property.isExpanded =
                EditorGUI.ToggleLeft(rect, _property.displayName, _property.isExpanded, EditorStyles.boldLabel);
        }

        private void RemoveElement(ReorderableList list)
        {
            if (CustomRemove == null || !CustomRemove(list.index))
                ReorderableList.defaultBehaviours.DoRemoveButton(list);
        }

        private void DrawElement(Rect rect, int index, bool active, bool focused)
        {
            var element = _property.GetArrayElementAtIndex(index);
            EditorGUI.BeginChangeCheck();
            var newRect = rect;
            newRect.x += 20;

            if (element.propertyType == SerializedPropertyType.Generic)
                EditorGUI.LabelField(newRect, element.displayName);

            rect.height = GetElementHeight(index);
            rect.y += 1;

            var property = _property.GetArrayElementAtIndex(index);

            if (CustomDrawer != null) CustomDrawer(property, rect, index);
            else EditorGUI.PropertyField(rect, property, GUIContent.none, true);

            _list.elementHeight = rect.height + 4.0f;
            if (EditorGUI.EndChangeCheck()) Apply();
        }

        private float GetElementHeight(int index)
        {
            var element = _property.GetArrayElementAtIndex(index);
            var height = EditorGUI.GetPropertyHeight(element, GUIContent.none, true);
            return Mathf.Max(EditorGUIUtility.singleLineHeight, height + 4.0f);
        }

        private void Apply()
        {
            _property.serializedObject.ApplyModifiedProperties();
        }
    }

    #endregion

    public static bool EyeButton(bool on)
    {
        if (_eyeOpenContent == null) _eyeOpenContent = new GUIContent(IconEye16);
        if (_eyeClosedContent == null) _eyeClosedContent = new GUIContent(IconEyeCrossed16);

        return GUILayout.Button(on ? _eyeOpenContent : _eyeClosedContent, EditorStyles.toolbarButton, GUILayout.Width(20));
    }

    private static GUIContent _eyeOpenContent;
    private static GUIContent _eyeClosedContent;
    

    public static Texture2D IconReload16
    {
        get { return StringImageConverter.ConvertFromString(IconReload, 16, 16); }
    }

    public static Texture2D IconReload32
    {
        get { return StringImageConverter.ConvertFromString(IconReload, 32, 32); }
    }

    public static Texture2D IconReload64
    {
        get { return StringImageConverter.ConvertFromString(IconReload, 64, 64); }
    }

    public static Texture2D IconEye16
    {
        get { return StringImageConverter.ConvertFromString(IconEye, 16, 16); }
    }


    public static Texture2D IconEye32
    {
        get { return StringImageConverter.ConvertFromString(IconEye, 32, 32); }
    }

    public static Texture2D IconEyeCrossed16
    {
        get { return StringImageConverter.ConvertFromString(IconEyeCrossed, 16, 16); }
    }

    public static Texture2D IconEyeCrossed32
    {
        get { return StringImageConverter.ConvertFromString(IconEyeCrossed, 32, 32); }
    }

    private const string IconReload =
        "iVBORw0KGgoAAAANSUhEUgAAAIAAAACACAMAAAD04JH5AAAArlBMVEUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAABeyFOlAAAAOXRSTlMA5ODTwwz7gEAF6PHbvRYQ9ZdGJAnOnhz318mqo3ZsWVEhGOy3cTUuE4t7XlQrrmZjSzuykqiGMigneNQWAAAEbUlEQVR42uzX2XaiQBSF4QMyKzggiKgMiopGNMZM+/1frG+6e7ksBLEq5sbvBfjhVFFAT09PT09Pj2ZGJIaafhlxNNN1fbs8JG+FQzfpTkD80uPSt+Ueziy80/vrF9VZK7CIkxtNOii3UN53KlVILEAmHvkhk1BF6/QNk66YBQAkul+xkVEvUA6j0vFPAa4Aw9dwI2nm0qXvEFwBbz6akJcXgzhK4AlQP3toSDnSmZkGngCjheZ6fn62+/+ThN1+vdbu7945gSfgW8HdNkMiKkLwBLxY4JAVtLPAEdDdg491AjgCRgq48QSkHn41YOQBvxkwauEHSBzXf3CAjzq9Tmhnme1J2k8EvFZfe+xHAzdX1eFQ7TrrRM8sTWzAKsB1rY2hMhOLPyyBAeoYV2WxSaXWfVlYwBTXKDFd5+ptMQExrrCWat1nW1tAQCqhlDZ1qc7QFhAwQanFjmoVSsAfMECpzopqHSUBa2DooYw3olpb1JLuXYGhc8uPn4gAs4US45TqOBmEBOxQom1QnSKEmAAbJV7rB2dBTMBbAJZCdT6FnYY+WNoXVVMnEBXgdMDSqdoqhLCAGCx5TpVeFhAX0AdrSVXcPpqQmk9gnFOVSJIFBhwbPwDK89R42ZzGmpAAHQypSzcYzr8TfT8ONL4Acw/GB93MVItk++FZvbsDHBmMHTWVDpZ9Re7dE7AGY5HTXVLj8J612g0DYjBOxMFZJdtpuLg9YAvGlnh13cHMV/5VWE1fQwkJoc6NqG/LbQRUhd0E2poE6q4inaoo7JqZ0yO1cCnM6ZHYk8BW6ZFkXNqb9Eh/2rXTJTWBKArAB1AWQVTcF1xw1DKacRmXnPd/sfxMKt0INEhVpvgegOpLF7e4p1scK6YOXvl2b8ATw4A6csn/FbRQpprsf7BMX/zXqIsyDSgIUCaLghBlOlAwRZm2FGh1lGgojrimjdLII64OyiOfC3yUKKDIgrK9ZfeQyVCnwOjlKMf1PkO7hfRmFJ2QhZj2jLzBaesjnRNFhgMlyxH/GA/m1yGSRRqLmg1ahlDJ3fqltAftHRRYanFbMKKoCZFS3Nb3kchQWbjI71NxM8+UcLfI6E6JxgrJejoltC4ymZNUbesHyujCCrI/w40AKJ8XjLd56+cD6dgmZdwNRPE3hvL09A7lbj5SeNYoFyCt1Zpy/QCJrDblZg5SW5iM0UnopcsmY3gtZDBjHO02RKxgajLOAlm0PMbSH7u6NLNdNtuM1UE21zbjmc3QjvC3Xnf5MPjCpIeMznxtPN2fLvb2+bwGx83to8GXtG7eMU3OdNdum8ncK7Kr11iYI1REfRYkhJquwULcICj1Jtce6pw982ockcvcZC7GDjkFBnOYRsgtmlGVG6IQc51KajYK0p0yu3WIAh08ZjT4iUL5mwbTM2sXFG74w2A6bm3p4B2cw+eaicYdG++zDb9cvqDPDhHey9md72PKtJtWsEIZHH+3sGbNieHpmqbp3rg/+ehsLqs6KpVKpVL5D/0GR6Q1W4O7GC4AAAAASUVORK5CYII=";

    private const string IconEye =
        "iVBORw0KGgoAAAANSUhEUgAAACAAAAAgCAMAAABEpIrGAAAAllBMVEUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA6C80qAAAAMXRSTlMA/AQdmFkx8dyRhYBwSCEW6cG7s653X0A2LPTf2NTHmpRMGRMG0c3Jw39osKWgiTwpnSU/9gAAAQRJREFUOMvlkVdyhDAQRDVIZDaQc/Cy2bHvfzljSYUpihvs+1GrZrrno9nLYRSBiMMwFkFhbIyt/oiZo7DW4wgAtR9eGvifFwIQl4vx+YuAW3ZgmjHrAPLmQ6cbcM2l3JvZXor8CnTasa9BqUoSmOiVMyU4XPpr2MrOBCRC/XIbzpRhdLAtHQUFcfUvbNwN5oF2TGFCYzLFjuCzx/9Ctl54A1JmhKiK7RPfFaLpOTiwdUYPyaD9FZrxT3AH5BuyDUEADVKfPaA5qdXDHWhVCDdNruwNEI5zjx4Bl4AzDQ9agB7LTssYE07i+r6b1Jh4/1n1WQ7Lut2SbWA93SSKEvdpsZfjF8DaIuCDeOKkAAAAAElFTkSuQmCC";

    private const string IconEyeCrossed =
        "iVBORw0KGgoAAAANSUhEUgAAACAAAAAgCAMAAABEpIrGAAAAilBMVEUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAATAggvAAAALXRSTlMABPkXHfHWyeremYBvMvTPd1lIQAy0r6SRhV5MIffBuytoOjYnEOPDtpSKRBNc009oAAABPklEQVQ4y7WS13LDIBBFF9R7r46KS1xz///3IgEztpUMbz5PK+2eiwTQ5/jemdp+eR/8b02fpRxXzQDrY7gPTcDBRdyzP9ox9KtxrPxw54Cn237ku3ihKjftCQC3my4Ng8YVE/Nr+J0DTm/Ih3DAOQF4x0hhOEBSiPIr60MLbkFFsigGyZceeCplHyvDfnVTDs8UvgdL6iT7SGR2YcEzFsuBFakoSLg6haOFC6MOPCdJBkVGkpwjoOA50G8HdkBKbER8/H+JQ4xJfaTMiCwIbsqPYf+shemBB2wJSKR/Y+KXO8A21EZdljovG6CZs8yUug2MwpfTHHABRy1uhjZE6pO5wsK5boOgrT0s1Cd642HhBbed6Z2TjSGM9m09TXW7j2iL6QB+qbnjDXA1NHc0j9ez13Cwk4J0lNGJ0cf4BX2sKFr3AYx/AAAAAElFTkSuQmCC";
}