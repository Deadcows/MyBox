#if UNITY_EDITOR
namespace MyBox.Internal
{
    using UnityEditor;
    using UnityEngine;
    
    [CustomEditor(typeof(Object), true), CanEditMultipleObjects]
    public class UnityObjectEditor : Editor
    {
        private FoldoutAttributeHandler _foldout;
        private ButtonMethodHandler _buttonMethod; 
        
        private void OnEnable()
        {
            _foldout = new FoldoutAttributeHandler(target, serializedObject);
            _buttonMethod = new ButtonMethodHandler(target);
        }

        private void OnDisable()
        {
            _foldout?.OnDisable();
        }

        public override void OnInspectorGUI()
        {
            if (_foldout != null)
            {
                _foldout.Update();
                if (!_foldout.OverrideInspector) base.OnInspectorGUI();
                else _foldout.OnInspectorGUI();
            }

            _buttonMethod?.OnInspectorGUI();
        }
    }
}
#endif