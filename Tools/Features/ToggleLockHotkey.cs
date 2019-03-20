#if UNITY_EDITOR
using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace MyBox.Internal
{
	public static class ToggleLockHotkey
	{
		[MenuItem("Tools/MyBox/Toggle Lock &q")]
		static void ToggleInspectorLock()
		{
			if (_mouseOverWindow == null)
			{
				int i = EditorPrefs.GetInt("LockableInspectorIndex", 0);

				Type type = Assembly.GetAssembly(typeof(Editor)).GetType("UnityEditor.InspectorWindow");
				Object[] findObjectsOfTypeAll = Resources.FindObjectsOfTypeAll(type);
				_mouseOverWindow = (EditorWindow) findObjectsOfTypeAll[i];
			}

			if (_mouseOverWindow != null && _mouseOverWindow.GetType().Name == "InspectorWindow")
			{
				Type type = Assembly.GetAssembly(typeof(Editor)).GetType("UnityEditor.InspectorWindow");
				PropertyInfo propertyInfo = type.GetProperty("isLocked");
				if (propertyInfo == null) return;
				bool value = (bool) propertyInfo.GetValue(_mouseOverWindow, null);
				propertyInfo.SetValue(_mouseOverWindow, !value, null);
				_mouseOverWindow.Repaint();
			}
		}

		private static EditorWindow _mouseOverWindow;
	}
}
#endif