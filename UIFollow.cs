using UnityEngine;

namespace MyBox
{
	[ExecuteInEditMode]
	public class UIFollow : MonoBehaviour
	{
		public Transform ToFollow;
		public Vector2 Offset;

		private RectTransform Transform => _transform ? _transform : (_transform = transform as RectTransform);
		private RectTransform _transform;

		private Camera Camera => _camera ? _camera : (_camera = Camera.main);
		private Camera _camera;

		private void Update()
		{
			if (ToFollow == null) return;

			Transform.anchorMax = Vector2.zero;
			Transform.anchorMin = Vector2.zero;

			var followPosition = ToFollow.position.Offset(Offset);

			Vector3 screenspace = Camera.WorldToScreenPoint(followPosition);
			Transform.anchoredPosition = screenspace;
		}
	}
}