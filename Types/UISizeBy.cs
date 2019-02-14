using UnityEngine;

namespace MyBox
{
    /// <summary>
    /// Set size of RectTransform by some other RectTransform
    /// </summary>
    [ExecuteInEditMode]
    public class UISizeBy : MonoBehaviour
    {
        [MustBeAssigned] public RectTransform CopySizeFrom;

        [Separator]
        public bool CopyWidth = true;
        public bool CopyHeight;

        [ConditionalField("CopyWidth")]
        public float AdditionalWidth;
        [ConditionalField("CopyHeight")]
        public float AdditionalHeight;

        private RectTransform _transform;

        private void Start()
        {
            _transform = transform as RectTransform;

            Debug.Assert(CopyWidth || CopyHeight, this);
        }


        private void Update()
        {
            if (CopySizeFrom == null) return;
            if (_transform == null) return;

            var fromSize = CopySizeFrom.sizeDelta;
            var toSize = _transform.sizeDelta;
            var x = CopyWidth ? fromSize.x + AdditionalWidth : toSize.x;
            var y = CopyHeight ? fromSize.y + AdditionalHeight : toSize.y;
            _transform.sizeDelta = new Vector2(x, y);
        }
    }
}