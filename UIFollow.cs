using UnityEngine;

namespace MyBox
{
    [ExecuteInEditMode]
    public class UIFollow : MonoBehaviour
    {
        public Transform ToFollow;
        public Vector2 Offset;

        [SerializeField] private bool _editTime = true;

        private RectTransform Transform
        {
            get { return _transform ? _transform : _transform = transform as RectTransform; }
        }

        private RectTransform _transform;

        private Camera Camera
        {
            get { return _camera ? _camera : _camera = Camera.main; }
        }

        private Camera _camera;

        private void LateUpdate()
        {
            if (!_editTime && !Application.isPlaying) return;

            if (ToFollow == null) return;
            if (Camera == null) return;

            Transform.anchorMax = Vector2.zero;
            Transform.anchorMin = Vector2.zero;

            var followPosition = ToFollow.position.Offset(Offset);
            Vector3 screenspace = Camera.WorldToScreenPoint(followPosition);
            Transform.anchoredPosition = screenspace;
        }
    }
}