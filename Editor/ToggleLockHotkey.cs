using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

public class ToggleLockHotkey
{
	private static EditorWindow _mouseOverWindow;

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
			if (propertyInfo == null) return;
			bool value = (bool)propertyInfo.GetValue(_mouseOverWindow, null);
			propertyInfo.SetValue(_mouseOverWindow, !value, null);
			_mouseOverWindow.Repaint();
		}
	}
}