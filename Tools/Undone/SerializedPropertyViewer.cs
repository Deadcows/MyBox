#if UNITY_EDITOR
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Text;
using UnityEngine;
using UnityEditor;

namespace MyBox.Internal
{
	//TODO: Make it prettier and add to Docs
	public class SerializedPropertyViewer : EditorWindow
	{
		private const string MenuItem = "Show Serialized Properties";
		
		[MenuItem("CONTEXT/Component/" + MenuItem)]
		static void ContextMenuItem(MenuCommand command)
		{
			OpenPropertyViewer(command.context);
		}
		
		[MenuItem("Assets/" + MenuItem)]
		private static void AssetsMenuItem()
		{
			OpenPropertyViewer(Selection.activeObject);
		}
		
		[MenuItem("Assets/" + MenuItem, true)]
		private static bool AssetsMenuItemValidator()
		{
			return Selection.activeObject != null;
		}

		private static void OpenPropertyViewer(Object target)
		{
			var propertyViewer = (SerializedPropertyViewer) GetWindow(typeof(SerializedPropertyViewer));
			propertyViewer.titleContent = new GUIContent("Property Explorer");

			propertyViewer.Initialize(target);
			propertyViewer.Show();
		}
		

		private struct PropertyData
		{
			public readonly int Depth;
			public readonly string Info;
			public readonly int ObjectId;

			public PropertyData(int depth, string info, int objectId)
			{
				if (depth < 0) depth = 0;
				Depth = depth;
				Info = info;
				ObjectId = objectId;
			}
		}


		private Object _target;
		private SerializedObject _targetSO;
		private readonly List<PropertyData> _propertiesData = new List<PropertyData>();


		private bool _debugMode;
		private string _searchString = string.Empty;
		private string _highlightedSearchString = string.Empty;
		private Vector2 _scrollPos;


		private static GUIStyle _richTextStyle;

		private void Initialize(Object target)
		{
			_target = target;
			_targetSO = new SerializedObject(_target);

			if (_richTextStyle == null)
			{
				_richTextStyle = new GUIStyle(EditorStyles.label);
				_richTextStyle.richText = true;
			}

			CollectProperties();
		}


		private void OnGUI()
		{
			if (_target == null) Close();
			if (_targetSO == null || _propertiesData.Count == 0 || _richTextStyle == null) Initialize(_target);

			var debug = EditorGUILayout.Toggle("Debug Mode", _debugMode);
			if (debug != _debugMode)
			{
				_debugMode = debug;
				CollectProperties();
			}

			string searchString = EditorGUILayout.TextField("Search:", _searchString);
			if (searchString != _searchString)
			{
				_searchString = searchString;
				_highlightedSearchString = "<color=olive><b>" + _searchString + "</b></color>";
				CollectProperties();
			}


			_scrollPos = EditorGUILayout.BeginScrollView(_scrollPos);
			foreach (PropertyData property in _propertiesData)
			{
				EditorGUI.indentLevel = property.Depth;
				if (property.ObjectId > 0)
				{
					GUILayout.BeginHorizontal();
				}

				EditorGUILayout.SelectableLabel(property.Info, _richTextStyle, GUILayout.Height(20));
				if (property.ObjectId > 0)
				{
					if (GUILayout.Button(">Ping>", GUILayout.Width(50)))
					{
						Selection.activeInstanceID = property.ObjectId;
					}

					GUILayout.EndHorizontal();
				}
			}

			EditorGUILayout.EndScrollView();


			if (GUILayout.Button("Copy To Clipboard"))
			{
				StringBuilder sb = new StringBuilder();
				Dictionary<int, string> paddingHash = new Dictionary<int, string>();
				string padding = "";
				for (int i = 0; i < 40; i++)
				{
					paddingHash[i] = padding;
					padding += " ";
				}

				foreach (PropertyData line in _propertiesData)
				{
					sb.Append(paddingHash[line.Depth]);
					sb.Append(line.Info);
					sb.Append("\n");
				}

				EditorGUIUtility.systemCopyBuffer = sb.ToString();
			}
		}

		private void CollectProperties()
		{
			_propertiesData.Clear();
			if (_targetSO == null) return;

			var inspectorMode = _debugMode ? InspectorMode.Debug : InspectorMode.Normal;
			PropertyInfo inspectorModeInfo = typeof(SerializedObject).GetProperty("inspectorMode", BindingFlags.NonPublic | BindingFlags.Instance);
			if (inspectorModeInfo != null) inspectorModeInfo.SetValue(_targetSO, inspectorMode, null);


			var iterator = _targetSO.GetIterator();

			LogPropertyData(iterator);
			while (iterator.Next(true))
			{
				LogPropertyData(iterator);
			}
		}

		private readonly StringBuilder _descriptionBuilder = new StringBuilder();

		private void LogPropertyData(SerializedProperty property)
		{
			var value = AsStringValue(property);

			// This ugly construction is to prevent massive allocations and process selection on all text except <color> tags
			_descriptionBuilder.Length = 0;
			_descriptionBuilder.Append(property.propertyPath).Append(ProcessSelection(" — Type: "));
			if (property.isArray) _descriptionBuilder.Append("<color=maroon>").Append(ProcessSelection("[Array]")).Append("</color> ");
			_descriptionBuilder.Append("<color=blue>").Append(ProcessSelection(property.type)).Append("</color>");

			_descriptionBuilder.Append(ProcessSelection(", Name:")).Append("<color=green>").Append(ProcessSelection(property.name))
				.Append("</color>");
			if (!string.IsNullOrEmpty(value))
			{
				_descriptionBuilder.Append(ProcessSelection(", Value:"))
					.Append("<color=navy>").Append(ProcessSelection(value)).Append("</color>");
			}


			string description = _descriptionBuilder.ToString();


			bool isObject = property.propertyType == SerializedPropertyType.ObjectReference && property.objectReferenceValue != null;
			int propertyId = isObject ? property.objectReferenceValue.GetInstanceID() : 0;


			_propertiesData.Add(new PropertyData(property.depth, description, propertyId));
		}

		private string ProcessSelection(string str)
		{
			if (_searchString.Length <= 0) return str;
			return str.Replace(_searchString, _highlightedSearchString);
		}


		private string AsStringValue(SerializedProperty property)
		{
			switch (property.propertyType)
			{
				case SerializedPropertyType.String:
					return property.stringValue;

				case SerializedPropertyType.Character:
				case SerializedPropertyType.Integer:
					if (property.type == "char")
					{
						if (property.intValue < char.MinValue || property.intValue > char.MaxValue)
							return "Char with invalid value of " + property.intValue;
						return System.Convert.ToChar(property.intValue).ToString();
					}

					return property.intValue.ToString();

				case SerializedPropertyType.ObjectReference:
					return property.objectReferenceValue != null ? property.objectReferenceValue.ToString() : "null";

				case SerializedPropertyType.Float:
					return property.floatValue.ToString(CultureInfo.InvariantCulture);

				case SerializedPropertyType.Boolean:
					return property.boolValue.ToString();

				case SerializedPropertyType.Enum:
					return property.enumNames[property.enumValueIndex];

				default:
					return string.Empty;
			}
		}
	}
}
#endif