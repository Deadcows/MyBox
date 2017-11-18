using System.Collections.Generic;
using System.Reflection;
using System.Text;
using UnityEngine;
using UnityEditor;

public class SerializedPropertyViewer : EditorWindow
{
	public class SPData
	{
		public int depth;
		public string info;
		public string val;
		public int oid;

		public SPData(int d, string i, string v, int o)
		{
			if (d < 0)
			{
				d = 0;
			}
			depth = d;
			info = i;
			val = v;
			oid = o;
		}
	}

	[MenuItem("Tools/SP Viewer")]
	static void Init()
	{
		// Get existing open window or if none, make a new one:
		SerializedPropertyViewer window = (SerializedPropertyViewer)EditorWindow.GetWindow(typeof(SerializedPropertyViewer));
		window.titleContent = new GUIContent("SP Viewer");
		window.Show();
	}

	UnityEngine.Object obj;

	Vector2 scrollPos;
	List<SPData> data;
	bool dirty = true;
	string searchStr = "";
	string searchStrRep;

	bool debugMode;
	public static GUIStyle richTextStyle;


	void OnGUI()
	{
		if (richTextStyle == null)
		{
			//EditorStyles does not exist in Constructor??
			richTextStyle = new GUIStyle(EditorStyles.label);
			richTextStyle.richText = true;
		}

		UnityEngine.Object newObj = EditorGUILayout.ObjectField("Object:", obj, typeof(UnityEngine.Object), true);
		debugMode = EditorGUILayout.Toggle("Debug Mode", debugMode);
		if ((GUILayout.Button("Refresh")))
		{
			obj = null;
		}

		string newSearchStr = EditorGUILayout.TextField("Search:", searchStr);
		if (newSearchStr != searchStr)
		{
			searchStr = newSearchStr;
			searchStrRep = "<color=green>" + searchStr + "</color>";
			dirty = true;
		}
		if (obj != newObj)
		{
			obj = newObj;
			dirty = true;
		}
		if (data == null)
		{
			dirty = true;
		}
		if (dirty == true)
		{
			dirty = false;
			searchObject(obj);
		}
		scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
		foreach (SPData line in data)
		{
			EditorGUI.indentLevel = line.depth;
			if (line.oid > 0)
			{
				GUILayout.BeginHorizontal();

			}
			EditorGUILayout.SelectableLabel(line.info, richTextStyle, GUILayout.Height(20));
			if (line.oid > 0)
			{
				if (GUILayout.Button(">>", GUILayout.Width(50)))
				{
					Selection.activeInstanceID = line.oid;
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
			foreach (SPData line in data)
			{
				sb.Append(paddingHash[line.depth]);
				sb.Append(line.info);
				sb.Append("\n");
			}
			EditorGUIUtility.systemCopyBuffer = sb.ToString();
		}
	}

	void searchObject(UnityEngine.Object obj)
	{
		data = new List<SPData>();
		if (obj == null)
		{
			return;
		}
		SerializedObject so = new SerializedObject(obj);
		if (debugMode)
		{
			PropertyInfo inspectorModeInfo = typeof(SerializedObject).GetProperty("inspectorMode", BindingFlags.NonPublic | BindingFlags.Instance);
			inspectorModeInfo.SetValue(so, InspectorMode.Debug, null);
		}

		SerializedProperty iterator = so.GetIterator();
		search(iterator, 0);
	}

	void search(SerializedProperty prop, int depth)
	{
		logProperty(prop);
		while (prop.Next(true))
		{
			logProperty(prop);
		}
	}


	void logProperty(SerializedProperty prop)
	{
		string strVal = getStringValue(prop);
		string propDesc = prop.propertyPath + " type:" + prop.type + " name:" + prop.name + " val:" + strVal + " isArray:" + prop.isArray;
		if (searchStr.Length > 0)
		{
			propDesc = propDesc.Replace(searchStr, searchStrRep);
		}
		data.Add(new SPData(prop.depth, propDesc, strVal, getObjectID(prop)));
	}

	int getObjectID(SerializedProperty prop)
	{
		if (prop.propertyType == SerializedPropertyType.ObjectReference && prop.objectReferenceValue != null)
		{
			return prop.objectReferenceValue.GetInstanceID();
		}
		return 0;
	}

	string getStringValue(SerializedProperty prop)
	{
		switch (prop.propertyType)
		{
			case SerializedPropertyType.String:
				return prop.stringValue;
			case SerializedPropertyType.Character: //this isn't really a thing, chars are ints!
			case SerializedPropertyType.Integer:
				if (prop.type == "char")
				{
					return System.Convert.ToChar(prop.intValue).ToString();
				}
				return prop.intValue.ToString();
			case SerializedPropertyType.ObjectReference:
				if (prop.objectReferenceValue != null)
				{
					return prop.objectReferenceValue.ToString();
				}
				else
				{
					return "(null)";
				}
			case SerializedPropertyType.Float:
				return prop.floatValue.ToString();
			default:
				return "";
		}
	}
}
