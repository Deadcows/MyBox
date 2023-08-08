using System;
using UnityEngine;

namespace MyBox
{
    [AttributeUsage(AttributeTargets.Field)]
    public sealed class RangeVectorAttribute : PropertyAttribute
    {
        #region fields

        public readonly Vector3 min = Vector3.zero;
        public readonly Vector3 max = Vector3.zero;
        public bool Valid { get; } = true;

        #endregion

        #region constructors

        /// <summary>
        ///   <para>Attribute used to make a Vector variable in a script be restricted to a specific range.</para>
        /// </summary>
        /// <param name="min">The array with the minimum allowed values.</param>
        /// <param name="max">The array with the maximum allowed values.</param>
        public RangeVectorAttribute(float[] min, float[] max)
        {
            if (min.Length > 3 || max.Length > 3)
            {
                Valid = false;
                return;
            }

            switch (min.Length)
            {
                case 3:
                    this.min.x = min[0];
                    this.min.y = min[1];
                    this.min.z = min[2];
                    break;
                case 2:
                    this.min.x = min[0];
                    this.min.y = min[1];
                    break;
                case 1:
                    this.min.x = min[0];
                    break;
            }

            switch (max.Length)
            {
                case 3:
                    this.max.x = max[0];
                    this.max.y = max[1];
                    this.max.z = max[2];
                    break;
                case 2:
                    this.max.x = max[0];
                    this.max.y = max[1];
                    break;
                case 1:
                    this.max.x = max[0];
                    break;
            }
        }

        #endregion
    }
}

#if UNITY_EDITOR
namespace MyBox.Internal
{
    using UnityEditor;

    [CustomPropertyDrawer(typeof(RangeVectorAttribute))]
    public class RangeVectorAttributeDrawer : PropertyDrawer
    {
        private RangeVectorAttribute Attribute => attribute as RangeVectorAttribute;
        private float ClampedX(float value) => Mathf.Clamp(value, Attribute.min.x, Attribute.max.x);
        private float ClampedY(float value) => Mathf.Clamp(value, Attribute.min.y, Attribute.max.y);
        private float ClampedZ(float value) => Mathf.Clamp(value, Attribute.min.z, Attribute.max.z);

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var valueChanged = HandleChanges(position, property, label);
            if (valueChanged || GUI.changed) property.serializedObject.ApplyModifiedProperties();
        }

        private bool HandleChanges(Rect position, SerializedProperty property, GUIContent label)
        {
            if (!Attribute.Valid)
            {
                DisplayWarning("min/max must be of length 3 or less");
                return false;
            }

            if (property.propertyType == SerializedPropertyType.Vector2)
            {
                var val = EditorGUI.Vector2Field(position, label, property.vector2Value);

                val.x = ClampedX(val.x);
                val.y = ClampedY(val.y);
                if (property.vector2Value != val)
                {
                    property.vector2Value = val;
                    return true;
                }
                return false;
            }

            if (property.propertyType == SerializedPropertyType.Vector2Int)
            {
                var val = EditorGUI.Vector2IntField(position, label, property.vector2IntValue);

                val.x = Mathf.RoundToInt(ClampedX(val.x));
                val.y = Mathf.RoundToInt(ClampedY(val.y));
                if (property.vector2IntValue != val)
                {
                    property.vector2IntValue = val;
                    return true;
                }
                return false;
            }
            
            
            if (property.propertyType == SerializedPropertyType.Vector3)
            {
                var val = EditorGUI.Vector3Field(position, label, property.vector3Value);

                val.x = ClampedX(val.x);
                val.y = ClampedY(val.y);
                val.z = ClampedZ(val.z);
                if (property.vector3Value != val)
                {
                    property.vector3Value = val;
                    return true;
                }
                return false;
            }
            
            if (property.propertyType == SerializedPropertyType.Vector3Int)
            {
                var val = EditorGUI.Vector3IntField(position, label, property.vector3IntValue);

                val.x = Mathf.RoundToInt(ClampedX(val.x));
                val.y = Mathf.RoundToInt(ClampedY(val.y));
                val.z = Mathf.RoundToInt(ClampedZ(val.z));
                if (property.vector3IntValue != val)
                {
                    property.vector3IntValue = val;
                    return true;
                }
                return false;
            }
            
            DisplayWarning("should be used with Vector2/3 or Vector2/3Int");
            return false;

            void DisplayWarning(string message)
            {
                message = property.name + " caused: [RangeVector] " + message;
                EditorGUI.HelpBox(position, message, MessageType.Warning);
            }
        }
    }
}
#endif