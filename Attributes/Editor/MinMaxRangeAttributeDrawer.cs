// ---------------------------------------------------------------------------- 
// Author: Richard Fine
// Source: https://bitbucket.org/richardfine/scriptableobjectdemo
// ----------------------------------------------------------------------------

using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(RangedFloat), true)]
public class MinMaxRangeAttributeDrawer : PropertyDrawer
{
	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
	{
		label = EditorGUI.BeginProperty(position, label, property);
		position = EditorGUI.PrefixLabel(position, label);

		SerializedProperty minProp = property.FindPropertyRelative("Min");
		SerializedProperty maxProp = property.FindPropertyRelative("Max");

		float minValue = minProp.floatValue;
		float maxValue = maxProp.floatValue;

		float rangeMin = 0;
		float rangeMax = 1;

		var ranges = (MinMaxRangeAttribute[])fieldInfo.GetCustomAttributes(typeof(MinMaxRangeAttribute), true);
		if (ranges.Length > 0)
		{
			rangeMin = ranges[0].Min;
			rangeMax = ranges[0].Max;
		}

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

		EditorGUI.EndProperty();
	}
}
