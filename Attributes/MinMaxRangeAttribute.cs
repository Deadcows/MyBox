// ---------------------------------------------------------------------------- 
// Author: Richard Fine
// Source: https://bitbucket.org/richardfine/scriptableobjectdemo
// ----------------------------------------------------------------------------

using System;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;

#endif

namespace MyBox
{
    public class MinMaxRangeAttribute : PropertyAttribute
    {
        public MinMaxRangeAttribute(float min, float max)
        {
            Min = min;
            Max = max;
        }

        public readonly float Min;
        public readonly float Max;
    }

    [Serializable]
    public struct RangedFloat
    {
        public float Min;
        public float Max;

        public RangedFloat(float a, float b)
        {
            Min = a;
            Max = b;
        }
    }

    [Serializable]
    public struct RangedInt
    {
        public int Min;
        public int Max;

        public RangedInt(int a, int b)
        {
            Min = a;
            Max = b;
        }
    }

    public static class RangedExtensions 
	{
		public static float LerpFromRange(this RangedFloat ranged, float t)
		{
			return Mathf.Lerp(ranged.Min, ranged.Max, t);
		}
		
		public static float LerpFromRangeUnclamped(this RangedFloat ranged, float t)
		{
			return Mathf.LerpUnclamped(ranged.Min, ranged.Max, t);
		}		
		
		public static float LerpFromRange(this RangedInt ranged, float t)
		{
			return Mathf.Lerp(ranged.Min, ranged.Max, t);
		}
		
		public static float LerpFromRangeUnclamped(this RangedInt ranged, float t)
		{
			return Mathf.LerpUnclamped(ranged.Min, ranged.Max, t);
		}
	}
}

#if UNITY_EDITOR
namespace MyBox.Internal
{
	[CustomPropertyDrawer(typeof(MinMaxRangeAttribute))]
	public class MinMaxRangeIntAttributeDrawer : PropertyDrawer
	{
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
            label = EditorGUI.BeginProperty(position, label, property);
            position = EditorGUI.PrefixLabel(position, label);

            SerializedProperty minProp = property.FindPropertyRelative("Min");
			SerializedProperty maxProp = property.FindPropertyRelative("Max");

            bool ifInt = false;

            float minValue;
            float maxValue;

            if (minProp.propertyType is SerializedPropertyType.Integer) ifInt = true;

            float rangeMin = 0;
            float rangeMax = 1;

            MinMaxRangeAttribute attr = (MinMaxRangeAttribute)base.attribute;

            rangeMin = attr.Min;
            rangeMin = attr.Max;

            if (ifInt)
            {
                minValue = minProp.intValue;
                maxValue = maxProp.intValue;

                const float rangeBoundsLabelWidth = 40f;

                var rangeBoundsLabel1Rect = new Rect(position);
                rangeBoundsLabel1Rect.width = rangeBoundsLabelWidth;
                GUI.Label(rangeBoundsLabel1Rect, new GUIContent(minValue.ToString("F0")));
                position.xMin += rangeBoundsLabelWidth;

                var rangeBoundsLabel2Rect = new Rect(position);
                rangeBoundsLabel2Rect.xMin = rangeBoundsLabel2Rect.xMax - rangeBoundsLabelWidth;
                GUI.Label(rangeBoundsLabel2Rect, new GUIContent(maxValue.ToString("F0")));
                position.xMax -= rangeBoundsLabelWidth;

                EditorGUI.BeginChangeCheck();
                EditorGUI.MinMaxSlider(position, ref minValue, ref maxValue, rangeMin, rangeMax);
                if (EditorGUI.EndChangeCheck())
                {
                    minProp.intValue = Mathf.RoundToInt(minValue);
                    maxProp.intValue = Mathf.RoundToInt(maxValue);
                }
            }
            else
            {
                minValue = minProp.floatValue;
                maxValue = maxProp.floatValue;

                const float rangeBoundsLabelWidth = 40f;

                var rangeBoundsLabel1Rect = new Rect(position);
                rangeBoundsLabel1Rect.width = rangeBoundsLabelWidth;
                GUI.Label(rangeBoundsLabel1Rect, new GUIContent(minValue.ToString("F2")));
                position.xMin += rangeBoundsLabelWidth;

                var rangeBoundsLabel2Rect = new Rect(position);
                rangeBoundsLabel2Rect.xMin = rangeBoundsLabel2Rect.xMax - rangeBoundsLabelWidth;
                GUI.Label(rangeBoundsLabel2Rect, new GUIContent(maxValue.ToString("F2")));
                position.xMax -= rangeBoundsLabelWidth;

                EditorGUI.BeginChangeCheck();
                EditorGUI.MinMaxSlider(position, ref minValue, ref maxValue, rangeMin, rangeMax);
                if (EditorGUI.EndChangeCheck())
                {
                    minProp.floatValue = minValue;
                    maxProp.floatValue = maxValue;
                }
            }
            EditorGUI.EndProperty();
        }
    }
}
#endif