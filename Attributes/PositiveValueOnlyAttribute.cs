using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace MyBox
{

    public class PositiveValueOnlyAttribute : PropertyAttribute
    {
    }

#if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(PositiveValueOnlyAttribute))]
    public class PositiveValueOnlyAttributeDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            SerializedPropertyType propertyType;
            if (GetPropertyTypeIfNumerical(out propertyType, property))
            {
                SetGUIFieldsForProperty(position, label, property, propertyType);
                SetPropertyToPositiveIfNegative(property, propertyType);

            } else
            {
                Debug.LogWarning(string.Format("Property <color=brown>{0}</color> in object <color=brown>{1}</color> is of wrong type. Expected: Any numerical hosting property.",
                    property.name, GetTargettedObjectFromProperty(property)));
            }
        }

        private void SetGUIFieldsForProperty(Rect position, GUIContent label, SerializedProperty property, SerializedPropertyType propertyType)
        {
            // TODO: REFACTOR!!!
            if (propertyType == SerializedPropertyType.Float)
            {
                property.floatValue = EditorGUI.FloatField(position, label, property.floatValue);
            } else if (propertyType == SerializedPropertyType.Integer)
            {
                property.intValue = EditorGUI.IntField(position, label, property.intValue);
            } else if (propertyType == SerializedPropertyType.Vector4)
            {
                property.vector4Value = EditorGUI.Vector4Field(position, label, property.vector4Value);
            } else if (propertyType == SerializedPropertyType.Vector3)
            {
                property.vector3Value = EditorGUI.Vector3Field(position, label, property.vector3Value);
            } else if (propertyType == SerializedPropertyType.Vector3Int)
            {
                property.vector3IntValue = EditorGUI.Vector3IntField(position, label, property.vector3IntValue);
            } else if (propertyType == SerializedPropertyType.Vector2)
            {
                property.vector2Value = EditorGUI.Vector2Field(position, label, property.vector2Value);
            } else if (propertyType == SerializedPropertyType.Vector2Int)
            {
                property.vector2IntValue = EditorGUI.Vector2IntField(position, label, property.vector2IntValue);
            } else if (propertyType == SerializedPropertyType.ArraySize)
            {
                property.arraySize = EditorGUI.IntField(position, label, property.arraySize);
            } else if (propertyType == SerializedPropertyType.Color)
            {
                property.colorValue = EditorGUI.ColorField(position, label, property.colorValue);
            }

            /// NEED_FIX
            //else if (propertyType == SerializedPropertyType.Quaternion) {
            //    SetQuaternionGUIField(position, label, property);
            //} 
        }

        /// NEED_FIX
        private void SetQuaternionGUIField(Rect position, GUIContent label, SerializedProperty property)
        {
            GUIContent[] guiContent = new GUIContent[4];
            guiContent[0] = new GUIContent("X");
            guiContent[1] = new GUIContent("Y");
            guiContent[2] = new GUIContent("Z");
            guiContent[3] = new GUIContent("W");


            float[] setValues = new float[4];
            Quaternion propQuaternion = property.quaternionValue;

            for (int i = 0; i < 4; ++i)
            {
                setValues[i] = propQuaternion[i];
            }

            EditorGUI.MultiFloatField(position, label, guiContent, setValues);
        }

        private void SetPropertyToPositiveIfNegative(SerializedProperty property, SerializedPropertyType propertyType)
        {
            // TODO: REFACTOR!!!
            if (propertyType == SerializedPropertyType.Float)
            {
                SetFloatPropertyToPositiveIfNegative(property);
            } else if (propertyType == SerializedPropertyType.Integer)
            {
                SetIntPropertyToPositiveIfNegative(property);
            } else if (propertyType == SerializedPropertyType.Vector4)
            {
                SetVector4ValuesToPositiveIfNegative(property);
            } else if (propertyType == SerializedPropertyType.Vector3)
            {
                SetVector3ValuesToPositiveIfNegative(property);
            } else if (propertyType == SerializedPropertyType.Vector3Int)
            {
                SetVector3IntValuesToPositiveIfNegative(property);
            } else if (propertyType == SerializedPropertyType.Vector2)
            {
                SetVector2ValuesToPositiveIfNegative(property);
            } else if (propertyType == SerializedPropertyType.Vector2Int)
            {
                SetVector2IntValuesToPositiveIfNegative(property);
            } else if (propertyType == SerializedPropertyType.Quaternion)
            {
                SetQuaternionPropertyToPositiveIfNegative(property);
            } else if (propertyType == SerializedPropertyType.ArraySize)
            {
                SetArraySizePropertyToPositiveIfNegative(property);
            } else if (propertyType == SerializedPropertyType.Color)
            {
                SetColorPropertyToPositiveIfNegative(property);
            }
        }

        #region Changing_Value_To_Positive_Functs

        private void SetColorPropertyToPositiveIfNegative(SerializedProperty property)
        {
            bool changedApplied = false;

            Color propColor = property.colorValue;

            // For each axis this vector has
            for (int i = 0; i < 3; ++i)
            {
                if (propColor[i] < 0f)
                {
                    propColor[i] = -(propColor[i]);
                    changedApplied = true;
                }
            }

            if (changedApplied)
            {
                WarnUserValueChangedToPositive(property);
            }

            property.colorValue = propColor;
        }

        private void SetArraySizePropertyToPositiveIfNegative(SerializedProperty property)
        {
            // One might wonder why would someone set the array size to negative...
            if (property.arraySize < 0)
            {
                property.arraySize = -(property.arraySize);
                WarnUserValueChangedToPositive(property);
            }
        }

        private void SetFloatPropertyToPositiveIfNegative(SerializedProperty property)
        {
            if (property.floatValue < 0f)
            {
                property.floatValue = -(property.floatValue);
                WarnUserValueChangedToPositive(property);
            }
        }

        private void SetIntPropertyToPositiveIfNegative(SerializedProperty property)
        {
            if (property.intValue < 0f)
            {
                property.intValue = -(property.intValue);
                WarnUserValueChangedToPositive(property);
            }
        }

        private void SetQuaternionPropertyToPositiveIfNegative(SerializedProperty property)
        {
            bool changedApplied = false;

            Quaternion propQuaternion = property.quaternionValue;

            // For each axis this vector has
            for (int i = 0; i < 4; ++i)
            {
                if (propQuaternion[i] < 0f)
                {
                    propQuaternion[i] = -(propQuaternion[i]);
                    changedApplied = true;
                }
            }

            if (changedApplied)
            {
                WarnUserValueChangedToPositive(property);
            }

            property.quaternionValue = propQuaternion;
        }

        #region Vectors

        // TODO: REFACTOR!!
        ///If possible (Repeating of codes here)

        private void SetVector4ValuesToPositiveIfNegative(SerializedProperty property)
        {
            bool changedApplied = false;

            Vector4 propV4 = property.vector4Value;

            // For each axis this vector has
            for (int i = 0; i < 4; ++i)
            {
                if (propV4[i] < 0f)
                {
                    propV4[i] = -(propV4[i]);
                    changedApplied = true;
                }
            }

            if (changedApplied)
            {
                WarnUserValueChangedToPositive(property);
            }

            property.vector4Value = propV4;
        }

        #region Vector3

        private void SetVector3ValuesToPositiveIfNegative(SerializedProperty property)
        {
            bool changedApplied = false;

            Vector3 propV3 = property.vector3Value;

            // For each axis this vector has
            for (int i = 0; i < 3; ++i)
            {
                if (propV3[i] < 0f)
                {
                    propV3[i] = -(propV3[i]);
                    changedApplied = true;
                }
            }

            if (changedApplied)
            {
                WarnUserValueChangedToPositive(property);
            }

            property.vector3Value = propV3;
        }

        private void SetVector3IntValuesToPositiveIfNegative(SerializedProperty property)
        {
            bool changedApplied = false;

            Vector3Int propV3Int = property.vector3IntValue;

            // For each axis this vector has
            for (int i = 0; i < 3; ++i)
            {
                if (propV3Int[i] < 0f)
                {
                    propV3Int[i] = -(propV3Int[i]);
                    changedApplied = true;
                }
            }

            if (changedApplied)
            {
                WarnUserValueChangedToPositive(property);
            }

            property.vector3IntValue = propV3Int;
        }

        #endregion

        #region Vector2

        private void SetVector2ValuesToPositiveIfNegative(SerializedProperty property)
        {
            bool changedApplied = false;

            Vector2 propV2 = property.vector2Value;

            // For each axis this vector has
            for (int i = 0; i < 2; ++i)
            {
                if (propV2[i] < 0f)
                {
                    propV2[i] = -(propV2[i]);
                    changedApplied = true;
                }
            }

            if (changedApplied)
            {
                WarnUserValueChangedToPositive(property);
            }

            property.vector2Value = propV2;
        }

        private void SetVector2IntValuesToPositiveIfNegative(SerializedProperty property)
        {
            bool changedApplied = false;

            Vector2Int propV2Int = property.vector2IntValue;

            // For each axis this vector has
            for (int i = 0; i < 2; ++i)
            {
                if (propV2Int[i] < 0f)
                {
                    propV2Int[i] = -(propV2Int[i]);
                    changedApplied = true;
                }
            }

            if (changedApplied)
            {
                WarnUserValueChangedToPositive(property);
            }

            property.vector2IntValue = propV2Int;
        }

        #endregion

        #endregion

        #endregion

        #region Util

        private void WarnUserValueChangedToPositive(SerializedProperty property)
        {
            Debug.LogWarning(string.Format("Property <color=brown>{0}</color> in object <color=brown>{1}</color> must be a positive value! Change: Value became positive.",
                    property.name, GetTargettedObjectFromProperty(property)));
        }

        private Object GetTargettedObjectFromProperty(SerializedProperty property)
        {
            return property.serializedObject.targetObject;
        }

        private bool GetPropertyTypeIfNumerical(out SerializedPropertyType propertyType, SerializedProperty property)
        {
            bool isNumerical = false;
            propertyType = property.propertyType;

            // TODO: REFACTOR!!!
            if (propertyType == SerializedPropertyType.Float)
            {
                isNumerical = true;
            } else if (propertyType == SerializedPropertyType.Integer)
            {
                isNumerical = true;
            } else if (propertyType == SerializedPropertyType.Vector4)
            {
                isNumerical = true;
            } else if (propertyType == SerializedPropertyType.Vector3)
            {
                isNumerical = true;
            } else if (propertyType == SerializedPropertyType.Vector3Int)
            {
                isNumerical = true;
            } else if (propertyType == SerializedPropertyType.Vector2)
            {
                isNumerical = true;
            } else if (propertyType == SerializedPropertyType.Vector2Int)
            {
                isNumerical = true;
            } else if (propertyType == SerializedPropertyType.Quaternion)
            {
                isNumerical = true;
            } else if (propertyType == SerializedPropertyType.ArraySize)
            {
                isNumerical = true;
            } else if (propertyType == SerializedPropertyType.Color)
            {
                isNumerical = true;
            }

            return isNumerical;
        }

        #endregion
    }
#endif
}
