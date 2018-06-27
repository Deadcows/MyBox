using UnityEngine;
using UnityEditor;
using System.IO;
using System.Reflection;

public static class MyEditorExtensions
{

	/// <summary>
	/// Get Prefab path in Asset Database
	/// </summary>
	/// <returns>Null if not a prefab</returns>
	public static string PrefabPath(this GameObject gameObject, bool withAssetName = true)
	{
		if (gameObject == null) return null;

		Object prefabParent = PrefabUtility.GetCorrespondingObjectFromSource(gameObject);
		if (prefabParent == null) return null;
		var assetPath = AssetDatabase.GetAssetPath(prefabParent);

		return !withAssetName ? Path.GetDirectoryName(assetPath) : assetPath;
	}

	/// <summary>
	/// Get string representation of serialized property, even for non-string fields
	/// </summary>
	public static string AsStringValue(this SerializedProperty property)
	{
		switch (property.propertyType)
		{
			case SerializedPropertyType.String:
				return property.stringValue;

			case SerializedPropertyType.Character:
			case SerializedPropertyType.Integer:
				if (property.type == "char") return System.Convert.ToChar(property.intValue).ToString();
				return property.intValue.ToString();

			case SerializedPropertyType.ObjectReference:
				return property.objectReferenceValue != null ? property.objectReferenceValue.ToString() : "null";

			case SerializedPropertyType.Boolean:
				return property.boolValue.ToString();

			case SerializedPropertyType.Enum:
				return property.enumNames[property.enumValueIndex];

			default:
				return string.Empty;
		}
	}

	/// <summary>
	/// Set Editor Icon (the one that appear in SceneView)
	/// </summary>
	public static void SetEditorIcon(this GameObject gameObject, bool textIcon, int iconIndex)
	{
		GUIContent[] icons = textIcon ? 
			GetTextures("sv_label_", string.Empty, 0, 8) :
			GetTextures("sv_icon_dot", "_pix16_gizmo", 0, 16);

		var egu = typeof(EditorGUIUtility);
		var flags = BindingFlags.InvokeMethod | BindingFlags.Static | BindingFlags.NonPublic;
		var args = new object[] { gameObject, icons[iconIndex].image };
		var setIconMethod = egu.GetMethod("SetIconForObject", flags, null, new[] { typeof(Object), typeof(Texture2D) }, null);
		if (setIconMethod != null) setIconMethod.Invoke(null, args);
	}

	private static GUIContent[] GetTextures(string baseName, string postFix, int startIndex, int count)
	{
		GUIContent[] array = new GUIContent[count];
		for (int i = 0; i < count; i++)
		{
			array[i] = EditorGUIUtility.IconContent(baseName + (startIndex + i) + postFix);
		}
		return array;
	}

}
