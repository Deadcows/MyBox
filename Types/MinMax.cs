// ---------------------------------------------------------------------------- 
// Author: Kaynn, Yeo Wen Qin
// https://github.com/Kaynn-Cahya
// Date:   03/08/2019
// ----------------------------------------------------------------------------

using System;
using UnityEngine;

namespace MyBox
{
	[Serializable]
	public struct MinMaxFloat
	{
		public float Min;
		public float Max;
		
		public MinMaxFloat(float min, float max)
		{
			Min = min;
			Max = max;
		}
	}

	[Serializable]
	public struct MinMaxInt
	{
		public int Min;
		public int Max;
		
		public MinMaxInt(int min, int max)
		{
			Min = min;
			Max = max;
		}
	}

	public static class MinMaxExtensions
	{
		/// <summary>
		/// Clamp value between MinMax values
		/// </summary>
		public static int Clamp(this MinMaxInt minMax, int value)
		{
			return Mathf.Clamp(value, minMax.Min, minMax.Max);
		}
		
		/// <summary>
		/// Clamp value between MinMax values
		/// </summary>
		public static float Clamp(this MinMaxFloat minMax, float value)
		{
			return Mathf.Clamp(value, minMax.Min, minMax.Max);
		}
		
		
		/// <summary>
		/// Distance from Min to Max
		/// </summary>
		public static int Length(this MinMaxInt minMax)
		{
			return minMax.Max - minMax.Min;
		}
		
		/// <summary>
		/// Distance from Min to Max
		/// </summary>
		public static float Length(this MinMaxFloat minMax)
		{
			return minMax.Max - minMax.Min;
		}
		
		
		/// <summary>
		/// Point between Min and Max
		/// </summary>
		public static int MidPoint(this MinMaxInt minMax)
		{
			return minMax.Min + minMax.Length() / 2;
		}
		
		/// <summary>
		/// Point between Min and Max
		/// </summary>
		public static float MidPoint(this MinMaxFloat minMax)
		{
			return minMax.Min + minMax.Length() / 2f;
		}
		
		
		/// <summary>
		/// Lerp from Min to Max
		/// </summary>
		public static float Lerp(this MinMaxFloat minMax, float value)
		{
			return Mathf.Lerp(minMax.Min, minMax.Max, value);
		}
		
		/// <summary>
		/// Lerp from Min to Max
		/// </summary>
		public static float LerpUnclamped(this MinMaxFloat minMax, float value)
		{
			return Mathf.LerpUnclamped(minMax.Min, minMax.Max, value);
		}
	}
}

#if UNITY_EDITOR
namespace MyBox.Internal
{
	using UnityEditor;

	[CustomPropertyDrawer(typeof(MinMaxInt), true)]
	public class MinMaxIntDrawer : PropertyDrawer
	{
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			EditorGUI.BeginProperty(position, label, property);
			position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);
			EditorGUI.indentLevel = 0; // PropertyDrawer Indent fix for nested inspectors

			var minProp = property.FindPropertyRelative("Min");
			var maxProp = property.FindPropertyRelative("Max");

			var minMaxLabel = "Min : Max";
			var labelWidth = 58;
			var spaceWidth = 4;
			var valWidth = (position.width / 2) - (labelWidth / 2f) - (spaceWidth * 4);

			position.width = valWidth;
			EditorGUI.PropertyField(position, minProp, GUIContent.none);

			position.x += valWidth + spaceWidth;
			position.width = labelWidth;
			EditorGUI.LabelField(position, minMaxLabel);

			position.x += labelWidth + spaceWidth;
			position.width = valWidth;
			EditorGUI.PropertyField(position, maxProp, GUIContent.none);

			if (GUI.changed)
			{
				if (maxProp.intValue < minProp.intValue) maxProp.intValue = minProp.intValue;

				property.serializedObject.ApplyModifiedProperties();
			}

			EditorGUI.EndProperty();
		}
	}


	[CustomPropertyDrawer(typeof(MinMaxFloat), true)]
	public class MinMaxFloatDrawer : PropertyDrawer
	{
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			EditorGUI.BeginProperty(position, label, property);
			position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);
			EditorGUI.indentLevel = 0; // PropertyDrawer Indent fix for nested inspectors

			var minProp = property.FindPropertyRelative("Min");
			var maxProp = property.FindPropertyRelative("Max");

			var minMaxLabel = "Min : Max";
			var labelWidth = 58;
			var spaceWidth = 4;
			var valWidth = (position.width / 2) - (labelWidth / 2f) - (spaceWidth * 4);

			position.width = valWidth;
			EditorGUI.PropertyField(position, minProp, GUIContent.none);

			position.x += valWidth + spaceWidth;
			position.width = labelWidth;
			EditorGUI.LabelField(position, minMaxLabel);

			position.x += labelWidth + spaceWidth;
			position.width = valWidth;
			EditorGUI.PropertyField(position, maxProp, GUIContent.none);

			if (GUI.changed)
			{
				if (maxProp.floatValue < minProp.floatValue) maxProp.floatValue = minProp.floatValue;

				property.serializedObject.ApplyModifiedProperties();
			}

			EditorGUI.EndProperty();
		}
	}
}
#endif