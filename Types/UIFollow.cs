using UnityEngine;

namespace MyBox
{
	[ExecuteInEditMode]
	public class UIFollow : MonoBehaviour
	{
		public Transform ToFollow;
		public Vector2 Offset;
		public Camera GameCamera;

#pragma warning disable 0649
		[SerializeField, Tooltip("Hide Canvas when Following Panel is offscreen")]
		private bool _hideOffscreen;
#pragma warning restore 0649
		[SerializeField, ConditionalField("_hideOffscreen")]
		private Canvas _canvas;

		[SerializeField] private bool _editTime = true;

		
		public bool IsOffscreen
		{
			get { return OffscreenOffset != Vector2.zero; }
		}
		
		private RectTransform Transform
		{
			get { return _transform ? _transform : _transform = transform as RectTransform; }
		}
		private RectTransform _transform;

		public Vector2 OffscreenOffset
		{
			get
			{
				var rect = Transform.rect;

				var halfWidth = rect.width / 2;
				var offX = 0f;
				var anchoredPosition = Transform.anchoredPosition;
				var minX = anchoredPosition.x + halfWidth;
				var maxX = anchoredPosition.x - halfWidth - Screen.width;
				if (minX < 0) offX = minX;
				else if (maxX > 0) offX = maxX;

				var halfHeight = rect.height / 2;
				var offY = 0f;
				var minY = anchoredPosition.y + halfHeight;
				var maxY = anchoredPosition.y - halfHeight - Screen.height;
				if (minY < 0) offY = minY;
				else if (maxY > 0) offY = maxY;
				return new Vector2(offX, offY);
			}
		}


		private void LateUpdate()
		{
			if (!_editTime && !Application.isPlaying) return;

			if (ToFollow == null) return;
			if (GameCamera == null)
			{
				GameCamera = Camera.main;
				if (GameCamera == null)
				{
					WarningsPool.LogWarning(name + ".UIFollow Caused: Main Camera not found. Assign Camera manually", this);
					return;
				}
			}

			Transform.anchorMax = Vector2.zero;
			Transform.anchorMin = Vector2.zero;

			var followPosition = ToFollow.position.Offset(Offset);
			Vector3 screenspace = GameCamera.WorldToScreenPoint(followPosition);
			Transform.anchoredPosition = screenspace;

			ToggleCanvasOffscreen();
		}

		private void ToggleCanvasOffscreen()
		{
			if (!_hideOffscreen) return;
			_canvas.enabled = !IsOffscreen;
		}

#if UNITY_EDITOR
		private void OnValidate()
		{
			if (_hideOffscreen && _canvas == null)
			{
				_canvas = GetComponentInChildren<Canvas>();
				if (_canvas == null) _canvas = GetComponentInParent<Canvas>();

				Debug.LogError(name + " Caused: UIFollow with HideOffscreen cant found Canvas");
			}
		}
#endif
	}
}