using UnityEngine;

namespace MyBox
{
	[ExecuteInEditMode]
	public class UIRelativePosition : MonoBehaviour
	{
		[MustBeAssigned] public RectTransform RelativeTo;
		
		[Separator("Set X/Y, with optional offset")]
		public OptionalFloat SetX = OptionalFloat.WithValue(0);
		public OptionalFloat SetY = OptionalFloat.WithValue(0);
		
		[Separator("0-1 point on Target rect")]
		public Vector2 Anchor = new Vector2(.5f, .5f);
		
		
		private RectTransform _transform;
		private Vector2 _latestSize;
		private Vector2 _latestPosition;
		
		private void Start()
		{
			_transform = transform as RectTransform;

			if (_transform == null) Debug.LogError(name + " Caused: Transform is not a RectTransform", this);
			if (!SetX.IsSet && !SetY.IsSet) Debug.LogError(name + " Caused: Check SetX and/or SetY for RelativePosition to work", this);
		}

		private void LateUpdate()
		{
			if (_transform == null) return;
			if (RelativeTo == null) return;
			
			var relativeToSize = RelativeTo.sizeDelta;
			var relativeToPosition = RelativeTo.anchoredPosition;
			if (_latestSize == relativeToSize && _latestPosition == relativeToPosition) return;
			_latestSize = relativeToSize;
			_latestPosition = relativeToPosition;

			var anchorOffsetX = relativeToSize.x * Anchor.x;
			var anchorOffsetY = relativeToSize.y * Anchor.y;
			var x = RelativeTo.offsetMin.x + anchorOffsetX + SetX.Value;
			var y = RelativeTo.offsetMax.y - anchorOffsetY + SetY.Value;

			var anchoredPosition = _transform.anchoredPosition;
			var finalPosition = new Vector2(SetX.IsSet ? x : anchoredPosition.x, SetY.IsSet ? y : anchoredPosition.y);
			_transform.anchoredPosition = finalPosition;
		}
		
#if UNITY_EDITOR
		[ButtonMethod]
		private void UpdateView()
		{
			_latestSize = Vector2.zero;
			UnityEditor.Undo.RecordObject(_transform, "UIRelativePosition.UpdateView");
			LateUpdate();
		}
#endif
	}
}