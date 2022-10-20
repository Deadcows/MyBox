#if UNITY_EDITOR
using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace MyBox.Internal
{
	public static class ToggleInspectorDebugHotkey
	{
		[MenuItem("Tools/MyBox/Toggle Console Debug &d")]
		static void ToggleInspectorDebug()
		{
			Type inspectorWindowType = Assembly.GetAssembly(typeof(Editor)).GetType("UnityEditor.InspectorWindow");

			if (_inspectorWindow == null)
			{
				Object[] findObjectsOfTypeAll = Resources.FindObjectsOfTypeAll(inspectorWindowType);
				_inspectorWindow = (EditorWindow) findObjectsOfTypeAll[0];
			}

			if (_inspectorWindow != null && _inspectorWindow.GetType().Name == "InspectorWindow")
			{
				FieldInfo inspectorMode = inspectorWindowType.GetField(
					"m_InspectorMode", 
					BindingFlags.NonPublic | BindingFlags.Instance);
				MethodInfo setModeMethod = inspectorWindowType.GetMethod(
					"SetMode", 
					BindingFlags.NonPublic | BindingFlags.Instance);
				
				if (inspectorMode == null || setModeMethod == null) return;
				
				InspectorMode mode = (InspectorMode)inspectorMode.GetValue(_inspectorWindow);
				mode = mode == InspectorMode.Normal ? InspectorMode.Debug : InspectorMode.Normal;

				setModeMethod.Invoke(_inspectorWindow, new object[] { mode });
				_inspectorWindow.Repaint();
			}
		}

		private static EditorWindow _inspectorWindow;
	}
}
#endif