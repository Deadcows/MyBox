using UnityEngine;

namespace MyBox
{
    public class ReadOnlyAttribute : ConditionalFieldAttribute
    {
    }
}

#if UNITY_EDITOR
namespace MyBox.Internal
{
    using UnityEditor;

    [CustomPropertyDrawer(typeof(ReadOnlyAttribute))]
    public class ReadOnlyAttributeDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUI.GetPropertyHeight(property, label, true);
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (!(attribute is ReadOnlyAttribute conditional)) return;

            bool enabled = !ConditionalUtility.IsPropertyConditionMatch(property, conditional.Data);

            GUI.enabled = enabled;
            EditorGUI.PropertyField(position, property, label, true);
            GUI.enabled = true;
        }
    }
}
#endif