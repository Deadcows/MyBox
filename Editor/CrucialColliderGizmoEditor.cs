using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor( typeof(CrucialColliderGizmo))]
public class CrucialColliderGizmoEditor : Editor
{
	private bool changedPreset;
	private CCGPresets oldPreset;

	private Color oldWireColor;
	private Color oldFillColor;
	private Color oldCenterColor;

	private bool  oldDrawWire;
	private bool  oldDrawFill;
	private bool  oldDrawCenter;
	private bool  oldIncludeChildColliders;

	private bool  oldEnabled;

	private float oldOverallAlpha;

	public override void OnInspectorGUI()
	{
		CrucialColliderGizmo target = (CrucialColliderGizmo)this.target;

		Undo.RecordObject(target, "CCG_State");

		EditorGUILayout.BeginVertical();

		oldEnabled = target.enabled;
		target.enabled = EditorGUILayout.Toggle("Enabled", target.enabled);
		if(oldEnabled != target.enabled)
		{
			EditorUtility.SetDirty(target);
		}

		oldPreset = target.selectedPreset;
		target.selectedPreset = (CCGPresets)EditorGUILayout.EnumPopup(new GUIContent("Color Preset", "Presets color profiles that, when selected, will update your colors to preset values."), target.selectedPreset);
		if(target.selectedPreset != oldPreset)
		{
			changedPreset = true;
			target.ChangePreset(target.selectedPreset);
			//switch (target.selectedPreset)
			//{
				
				//case CCGPresets.Red:
				//	target.wireColor = new Color(143.0f / 255.0f, 0.0f, 21.0f / 255.0f, 202.0f / 255.0f);
				//	target.fillColor = new Color(218.0f / 255.0f, 0.0f, 0.0f, 37.0f / 255.0f);
				//	target.centerColor = new Color(135.0f / 255.0f, 36.0f / 255.0f, 36.0f / 255.0f, 172.0f / 255.0f);
				//	break;

				//case CCGPresets.Blue:
				//	target.wireColor = new Color(0.0f, 116.0f / 255.0f, 214.0f / 255.0f, 202.0f / 255.0f);
				//	target.fillColor = new Color(0.0f, 110.0f / 255.0f, 218.0f / 255.0f, 37.0f / 255.0f);
				//	target.centerColor = new Color(57.0f / 255.0f, 160.0f / 255.0f, 221.0f / 255.0f, 172.0f / 255.0f);
				//	break;

				//case CCGPresets.Green:
				//	target.wireColor = new Color(153.0f / 255.0f,1.0f, 187.0f / 255.0f, 128.0f / 255.0f);
				//	target.fillColor = new Color(153.0f / 255.0f,1.0f, 187.0f / 255.0f, 62.0f / 255.0f);
				//	target.centerColor = new Color(153.0f / 255.0f,1.0f, 187.0f / 255.0f, 172.0f / 255.0f);
				//	break;

				//case CCGPresets.Purple:
				//	target.wireColor = new Color(138.0f / 255.0f, 138.0f / 255.0f, 234.0f / 255.0f, 128.0f / 255.0f);
				//	target.fillColor = new Color(173.0f / 255.0f, 178.0f / 255.0f,1.0f, 26.0f / 255.0f);
				//	target.centerColor = new Color(153.0f / 255.0f, 178.0f / 255.0f,1.0f, 172.0f / 255.0f);
				//	break;

				//case CCGPresets.Yellow:
				//	target.wireColor = new Color(255.0f / 255.0f, 231.0f / 255.0f, 35.0f / 255.0f, 128.0f / 255.0f);
				//	target.fillColor = new Color(255.0f / 255.0f, 252.0f / 255.0f, 153.0f / 255.0f, 100.0f / 255.0f);
				//	target.centerColor = new Color(255.0f / 255.0f, 242.0f / 255.0f, 84.0f / 255.0f, 172.0f / 255.0f);
				//	break;
				
				//case CCGPresets.Custom:
				//	target.wireColor = target.savedCustomWireColor;
				//	target.fillColor = target.savedCustomFillColor;
				//	target.centerColor = target.savedCustomCenterColor;
				//	break;
			//}
			EditorUtility.SetDirty(target);
		}
		oldOverallAlpha = target.overallAlpha;
		target.overallAlpha = EditorGUILayout.Slider("Overall Transparency", target.overallAlpha, 0, 1);
		if(oldOverallAlpha != target.overallAlpha)
		{
			EditorUtility.SetDirty(target);
		}

		oldWireColor = target.wireColor;
		target.wireColor = EditorGUILayout.ColorField(new GUIContent("Wire Color", "The color and alpha of the wire frame representing your collider(s)."), target.wireColor);
		if(target.wireColor != oldWireColor && !changedPreset)
		{
			target.selectedPreset = CCGPresets.Custom;
		}
		if(target.selectedPreset == CCGPresets.Custom)
		{
			target.savedCustomWireColor = target.wireColor;
			EditorUtility.SetDirty(target);
		}

		oldFillColor = target.fillColor;
		target.fillColor = EditorGUILayout.ColorField(new GUIContent("Fill Color", "The color and alpha of the fill on the faces of your collider(s)."), target.fillColor);
		if(target.fillColor != oldFillColor && !changedPreset)
		{
			target.selectedPreset = CCGPresets.Custom;
		}
		if(target.selectedPreset == CCGPresets.Custom)
		{
			target.savedCustomFillColor = target.fillColor;
			EditorUtility.SetDirty(target);
		}

		oldCenterColor = target.centerColor;
		target.centerColor = EditorGUILayout.ColorField(new GUIContent("Center Color", "The color and alpha of the center marker for your collider(s)."), target.centerColor);
		if(target.centerColor != oldCenterColor && !changedPreset)
		{
			target.selectedPreset = CCGPresets.Custom;
		}
		if(target.selectedPreset == CCGPresets.Custom)
		{
			target.savedCustomCenterColor = target.centerColor;
			EditorUtility.SetDirty(target);
		}

		oldDrawFill = target.drawFill;
		target.drawFill = EditorGUILayout.Toggle("Draw Fill", target.drawFill);
		if(oldDrawFill != target.drawFill)
		{
			EditorUtility.SetDirty(target);
		}

		oldDrawWire = target.drawWire;
		target.drawWire = EditorGUILayout.Toggle("Draw Wire", target.drawWire);
		if(oldDrawWire != target.drawWire)
		{
			EditorUtility.SetDirty(target);
		}

		oldDrawCenter = target.drawCenter;
		target.drawCenter = EditorGUILayout.Toggle("Draw Center", target.drawCenter);
		if(oldDrawCenter != target.drawCenter)
		{
			EditorUtility.SetDirty(target);
		}

		target.centerMarkerRadius = EditorGUILayout.FloatField(new GUIContent("Center Marker Radius", "The radius of the center marker on your collider(s)."), target.centerMarkerRadius);
		target.collider2D_ZDepth = EditorGUILayout.FloatField(new GUIContent("2D Collider Z Depth", "The thickness, or Z axis length, of your 2D collider elements. This is just to help assist in 3D visualization of 2D colliders, as the visual thickness of this representation will not reflect any change in the collider's behavior at runtime."), target.collider2D_ZDepth);
		oldIncludeChildColliders = target.includeChildColliders;
		target.includeChildColliders = EditorGUILayout.Toggle("Include Child Colliders", target.includeChildColliders);
		if(oldIncludeChildColliders != target.includeChildColliders)
		{
			EditorUtility.SetDirty(target);
		}
		changedPreset = false;
		EditorGUIUtility.labelWidth = 100;
		EditorGUIUtility.fieldWidth = 70;

		EditorGUILayout.EndVertical();
	}
}