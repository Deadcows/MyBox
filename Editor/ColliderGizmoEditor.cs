using UnityEngine;
using UnityEditor;

[CustomEditor( typeof(ColliderGizmo)), CanEditMultipleObjects]
public class ColliderGizmoEditor : Editor
{
	
	public override void OnInspectorGUI()
	{
		ColliderGizmo targetGizmo = (ColliderGizmo)target;

		Undo.RecordObject(targetGizmo, "CG_State");
		
		EditorGUILayout.PropertyField(serializedObject.FindProperty("m_Enabled"));
		
		var oldPreset = targetGizmo.Preset;
		targetGizmo.Preset = (ColliderGizmo.Presets)EditorGUILayout.EnumPopup("Color Preset", targetGizmo.Preset);
		if(targetGizmo.Preset != oldPreset)
		{
			foreach (var singleTarget in targets)
			{
				var gizmo = (ColliderGizmo) singleTarget;
				gizmo.ChangePreset(targetGizmo.Preset);
				EditorUtility.SetDirty(gizmo);
			}
		}

		var alpha = serializedObject.FindProperty("Alpha");
		alpha.floatValue = EditorGUILayout.Slider("Overall Transparency", alpha.floatValue, 0, 1);

		EditorGUI.BeginChangeCheck();

		var drawWire = serializedObject.FindProperty("DrawWire");
		var wireColor = serializedObject.FindProperty("WireColor");
		using (new EditorGUILayout.HorizontalScope())
		{
			EditorGUILayout.PropertyField(drawWire);
			if (drawWire.boolValue) EditorGUILayout.PropertyField(wireColor, new GUIContent(""));
		}

		var drawFill = serializedObject.FindProperty("DrawFill");
		var fillColor = serializedObject.FindProperty("FillColor");
		using (new EditorGUILayout.HorizontalScope())
		{
			EditorGUILayout.PropertyField(drawFill);
			if (drawFill.boolValue) EditorGUILayout.PropertyField(fillColor, new GUIContent(""));
		}

		var drawCenter = serializedObject.FindProperty("DrawCenter");
		var centerColor = serializedObject.FindProperty("CenterColor");
		using (new EditorGUILayout.HorizontalScope())
		{
			EditorGUILayout.PropertyField(drawCenter);
			if (drawCenter.boolValue)
			{
				EditorGUILayout.PropertyField(centerColor, new GUIContent(""));
			}
		}
		if (drawCenter.boolValue) EditorGUILayout.PropertyField(serializedObject.FindProperty("CenterMarkerRadius"));


		if (EditorGUI.EndChangeCheck())
		{
			var presetProp = serializedObject.FindProperty("Preset");
			var customWireColor = serializedObject.FindProperty("CustomWireColor");
			var customFillColor = serializedObject.FindProperty("CustomFillColor");
			var customCenterColor = serializedObject.FindProperty("CustomCenterColor");

			presetProp.enumValueIndex = (int)ColliderGizmo.Presets.Custom;
			customWireColor.colorValue = wireColor.colorValue;
			customFillColor.colorValue = fillColor.colorValue;
			customCenterColor.colorValue = centerColor.colorValue;
		}
		
		EditorGUILayout.PropertyField(serializedObject.FindProperty("IncludeChildColliders"));

		if (GUI.changed)
		{
			serializedObject.ApplyModifiedProperties();
			EditorUtility.SetDirty(targetGizmo);
		}
	}
	
}