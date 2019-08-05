using System;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;

#endif

namespace MyBox {
    public class MinMaxAttribute : Attribute {
        
    }

    [Serializable]
    public struct MinMaxFloat {
        public float Min;

        public float Max;
    }

    [Serializable]
    public struct MinMaxInt {
        public int Min;
        public int Max;
    }
}

#if UNITY_EDITOR
namespace MyBox.Internal {

    [CustomPropertyDrawer(typeof(MinMaxInt), true)]
    public class MinMaxIntAttributeDrawer : PropertyDrawer {

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {

            return EditorGUI.GetPropertyHeight(property);
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            EditorGUI.PropertyField(position, property, label, true);

            SerializedProperty minProp = property.FindPropertyRelative("Min");
            SerializedProperty maxProp = property.FindPropertyRelative("Max");

            int minValue = minProp.intValue;
            int maxValue = maxProp.intValue;

            if (minValue > maxValue) {
                SwapValues(ref minValue, ref maxValue);

                minProp.intValue = minValue;
                maxProp.intValue = maxValue;

                WarnAboutPropertyWithMessage(property, " 'Min' must be lower than 'Max'!");
            } else if (minValue == maxValue) {
                WarnAboutPropertyWithMessage(property, " 'Min' and 'Max' have the same value!");
            }
        }

        private static void WarnAboutPropertyWithMessage(SerializedProperty property, string message) {
            var startErrorMessage = string.Format("Property <color=brown>{0}</color> in object <color=brown>{1}</color> caused: ", property.name, property.serializedObject.targetObject);
            Debug.LogWarning(startErrorMessage + message);
        }

        private static void SwapValues(ref int valueA, ref int valueB) {
            var temp = valueA;
            valueA = valueB;
            valueB = temp;
        }
    }


    [CustomPropertyDrawer(typeof(MinMaxFloat), true)]
    public class MinMaxFloatAttributeDrawer : PropertyDrawer {

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {

            return EditorGUI.GetPropertyHeight(property);
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            EditorGUI.PropertyField(position, property, label, true);

            SerializedProperty minProp = property.FindPropertyRelative("Min");
            SerializedProperty maxProp = property.FindPropertyRelative("Max");

            float minValue = minProp.floatValue;
            float maxValue = maxProp.floatValue;

            if (minValue > maxValue) {
                SwapValues(ref minValue, ref maxValue);

                minProp.floatValue = minValue;
                maxProp.floatValue = maxValue;

                WarnAboutPropertyWithMessage(property, " 'Min' must be lower than 'Max'!");
            } else if (minValue == maxValue) {
                WarnAboutPropertyWithMessage(property, " 'Min' and 'Max' have the same value!");
            }
        }

        private static void SwapValues(ref float valueA, ref float valueB) {
            var temp = valueA;
            valueA = valueB;
            valueB = temp;
        }

        private static void WarnAboutPropertyWithMessage(SerializedProperty property, string message) {
            var startErrorMessage = string.Format("Property <color=brown>{0}</color> in object <color=brown>{1}</color> caused: ", property.name, property.serializedObject.targetObject);
            Debug.LogWarning(startErrorMessage + message);
        }
    }
}
#endif