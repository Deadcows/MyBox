using UnityEngine;

namespace MyBox
{
    /// <summary>
    /// Field will be Read-Only in Edit mode
    /// </summary>
    public class RuntimeFieldAttribute : PropertyAttribute
    {
    }
}

#if UNITY_EDITOR
namespace MyBox.Internal
{
    using UnityEditor;
	
    [CustomPropertyDrawer(typeof(RuntimeFieldAttribute))]
    public class RuntimeFieldAttributeDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUI.GetPropertyHeight(property, label, true);
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (!Application.isPlaying) GUI.enabled = false;
            EditorGUI.PropertyField(position, property, label, true);
            GUI.enabled = true;
        }
    }
}
#endif