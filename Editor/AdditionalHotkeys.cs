using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

public class AdditionalHotkeys
{
	private static EditorWindow _mouseOverWindow;

	//[MenuItem("Stuff/Select Inspector under mouse cursor (use hotkey) #&q")]
	//static void SelectLockableInspector()
	//{
	//	if (EditorWindow.mouseOverWindow.GetType().Name == "InspectorWindow")
	//	{
	//		_mouseOverWindow = EditorWindow.mouseOverWindow;
	//		Type type = Assembly.GetAssembly(typeof(Editor)).GetType("UnityEditor.InspectorWindow");
	//		Object[] findObjectsOfTypeAll = Resources.FindObjectsOfTypeAll(type);
	//		int indexOf = findObjectsOfTypeAll.ToList().IndexOf(_mouseOverWindow);
	//		EditorPrefs.SetInt("LockableInspectorIndex", indexOf);
	//	}
	//}

	[MenuItem("Window/Toggle Lock &q")]
	static void ToggleInspectorLock()
	{
		if (_mouseOverWindow == null)
		{
			if (!EditorPrefs.HasKey("LockableInspectorIndex"))
				EditorPrefs.SetInt("LockableInspectorIndex", 0);
			int i = EditorPrefs.GetInt("LockableInspectorIndex");

			Type type = Assembly.GetAssembly(typeof(Editor)).GetType("UnityEditor.InspectorWindow");
			Object[] findObjectsOfTypeAll = Resources.FindObjectsOfTypeAll(type);
			_mouseOverWindow = (EditorWindow)findObjectsOfTypeAll[i];
		}

		if (_mouseOverWindow != null && _mouseOverWindow.GetType().Name == "InspectorWindow")
		{
			Type type = Assembly.GetAssembly(typeof(Editor)).GetType("UnityEditor.InspectorWindow");
			PropertyInfo propertyInfo = type.GetProperty("isLocked");
			bool value = (bool)propertyInfo.GetValue(_mouseOverWindow, null);
			propertyInfo.SetValue(_mouseOverWindow, !value, null);
			_mouseOverWindow.Repaint();
		}
	}

	[MenuItem("Window/Clear Console #&c")]
	static void ClearConsole()
	{
		Type type = Assembly.GetAssembly(typeof(Editor)).GetType("UnityEditorInternal.LogEntries");
		type.GetMethod("Clear").Invoke(null, null);
	}
}
