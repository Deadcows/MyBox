using UnityEngine;

namespace MyBox
{
	/// <summary>
	/// Pivot and Anchor of Target makes no difference.
	/// Current Pivot and "Target Anchor" property used for positioning.
	/// </summary>
	[ExecuteAlways]
	public class UIRelativePosition : MonoBehaviour
	{
		[MustBeAssigned] public RectTransform Target;

		[Separator("Set X/Y, with optional offset")]
		public OptionalFloat SetX = OptionalFloat.WithValue(0);

		public OptionalFloat SetY = OptionalFloat.WithValue(0);

		[Separator("0-1 point on Target rect")]
		public Vector2 TargetAnchor = new Vector2(.5f, .5f);


		private RectTransform _transform;
		private Vector2 _latestSize;
		private Vector3 _latestPosition;
		private bool _firstCall;
		
		private void Start()
		{
			_transform = transform as RectTransform;

			if (_transform == null) Debug.LogError(name + " Caused: Transform is not a RectTransform", this);
			if (!SetX.IsSet && !SetY.IsSet) Debug.LogError(name + " Caused: Check SetX and/or SetY for RelativePosition to work", this);
		}

		private void LateUpdate()
		{
			if (_transform == null) return;
			if (Target == null) return;
			if (!_firstCall)
			{
				// Position is zero on PrefabModeEntered?
				// ForceUpdateRectTransforms is not helping, but on second frame it's all ok
				_firstCall = true;
				return;
			}
			
			var relativeToSize = Target.sizeDelta;
			var relativeToPosition = Target.position;
			if (_latestSize == relativeToSize && _latestPosition == relativeToPosition) return;
			_latestSize = relativeToSize;
			_latestPosition = relativeToPosition;

			var scale = Target.lossyScale;
			var pivot = Target.pivot;
			var anchorOffsetX = relativeToSize.x * TargetAnchor.x;
			var anchorOffsetY = relativeToSize.y * TargetAnchor.y;
			var left = relativeToPosition.x - (relativeToSize.x * pivot.x * scale.x);
			var top = relativeToPosition.y + relativeToSize.y - (relativeToSize.y * pivot.y * scale.y);
			var x = left + anchorOffsetX + SetX.Value;
			var y = top - anchorOffsetY + SetY.Value;

			var localPosition = _transform.position;
			var finalPosition = new Vector2(SetX.IsSet ? (int)x : localPosition.x, SetY.IsSet ? (int)y : localPosition.y);
			_transform.position = finalPosition;
		}

#if UNITY_EDITOR
		private void OnValidate()
		{
			UpdateView();
		}

		[ButtonMethod]
		private void UpdateView()
		{
			_latestSize = Vector2.zero;
			_transform = transform as RectTransform;
			if (_transform == null) return;
			
			UnityEditor.Undo.RecordObject(_transform, "UIRelativePosition.UpdateView");
			LateUpdate();
		}
#endif
	}
}