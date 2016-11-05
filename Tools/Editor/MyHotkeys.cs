using System;
using System.Reflection;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

/// <summary>
/// Additional hotkeys:
///		Toggle Inspector Lock	- Alt + Q
///		Clear Console			- Shift + Alt + C
///		Enter / Exit playmode	- F5
/// </summary>
public class MyHotkeys
{
	private static EditorWindow _mouseOverWindow;

	// ReSharper disable once UnusedMember.Local
	[MenuItem("Window/Toggle Inspector Lock &q")]
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

	// ReSharper disable once UnusedMember.Local
	[MenuItem("Window/Clear Console #&c")]
	static void ClearConsole()
	{
		Type type = Assembly.GetAssembly(typeof(Editor)).GetType("UnityEditorInternal.LogEntries");
		type.GetMethod("Clear").Invoke(null, null);
	}

	// ReSharper disable once UnusedMember.Local
	[MenuItem("Window/Run _F5")]
	static void PlayGame()
	{
		if (!Application.isPlaying)
		{
			EditorSceneManager.SaveScene(SceneManager.GetActiveScene(), "", false); // optional: save before run
		}
		EditorApplication.ExecuteMenuItem("Edit/Play");
	}
}
