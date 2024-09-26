#if UNITY_EDITOR && !MYBOX_DISABLE_INSPECTOR_OVERRIDE && !ODIN_INSPECTOR
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
            if (target == null) return;
            
            _foldout = new FoldoutAttributeHandler(target, serializedObject);
            _buttonMethod = new ButtonMethodHandler(target);
        }

        private void OnDisable()
        {
            _foldout?.OnDisable();
        }

        public override void OnInspectorGUI()
        {
            _buttonMethod?.OnBeforeInspectorGUI();
            
            if (_foldout != null)
            {
                _foldout.Update();
                if (!_foldout.OverrideInspector) base.OnInspectorGUI();
                else _foldout.OnInspectorGUI();
            }

            _buttonMethod?.OnAfterInspectorGUI();
        }
    }
}
#endif