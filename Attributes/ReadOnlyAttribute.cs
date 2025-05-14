#if !ODIN_INSPECTOR
using UnityEngine;

namespace MyBox
{
    public enum ReadOnly
    {
        Always, 
        PlayMode,
        EditTime
    }
    
    public class ReadOnlyAttribute : ConditionalFieldAttribute
    {
        public readonly ReadOnly ReadOnlyMode;
        
        public ReadOnlyAttribute(ReadOnly readOnly = ReadOnly.Always) => ReadOnlyMode = readOnly;
        
        /// <param name="fieldToCheck">String name of field to check value</param>
        /// <param name="inverse">Inverse check result</param>
        /// <param name="compareValues">On which values' field will be shown in inspector</param>
        public ReadOnlyAttribute(string fieldToCheck, bool inverse = false, params object[] compareValues) : base(fieldToCheck, inverse, compareValues)
        { }


        public ReadOnlyAttribute(string[] fieldToCheck, bool[] inverse = null, params object[] compare) : base(fieldToCheck, inverse, compare)
        { }

        public ReadOnlyAttribute(params string[] fieldToCheck) : base(fieldToCheck)
        { }
        
        public ReadOnlyAttribute(bool useMethod, string method, bool inverse = false) : base(useMethod, method, inverse)
        { }
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

            bool enabled = 
                conditional.ReadOnlyMode != ReadOnly.Always ? ReadOnlyModeEditable() : 
                    !ConditionalUtility.IsPropertyConditionMatch(property, conditional.Data);

            GUI.enabled = enabled;
            EditorGUI.PropertyField(position, property, label, true);
            GUI.enabled = true;

            bool ReadOnlyModeEditable()
            {
                return conditional.ReadOnlyMode switch
                {
                    ReadOnly.Always => false,
                    ReadOnly.PlayMode => !Application.isPlaying,
                    ReadOnly.EditTime => Application.isPlaying,
                    _ => false
                };
            }
        }
    }
}
#endif
#endif
