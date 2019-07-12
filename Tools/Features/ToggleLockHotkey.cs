// ---------------------------------------------------------------------------- 
// Author: Abomb
// https://forum.unity.com/threads/shortcut-key-for-lock-inspector.95815/#post-1056603
// Date:   09/10/2012
// ----------------------------------------------------------------------------

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
			Type inspectorWindowType = Assembly.GetAssembly(typeof(Editor)).GetType("UnityEditor.InspectorWindow");

			if (_inspectorWindow == null)
			{
				Object[] findObjectsOfTypeAll = Resources.FindObjectsOfTypeAll(inspectorWindowType);
				_inspectorWindow = (EditorWindow) findObjectsOfTypeAll[0];
			}

			if (_inspectorWindow != null && _inspectorWindow.GetType().Name == "InspectorWindow")
			{
				PropertyInfo isLockedPropertyInfo = inspectorWindowType.GetProperty("isLocked");
				if (isLockedPropertyInfo == null) return;

				bool value = (bool) isLockedPropertyInfo.GetValue(_inspectorWindow, null);
				isLockedPropertyInfo.SetValue(_inspectorWindow, !value, null);

				_inspectorWindow.Repaint();
			}
		}

		private static EditorWindow _inspectorWindow;
	}
}
#endif