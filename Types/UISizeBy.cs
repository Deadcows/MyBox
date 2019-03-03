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

		[Separator("CopyWidth/Height, Set optional offset")]
		public OptionalInt CopyWidth;

		public OptionalInt CopyHeight;

		[Separator("Optional Min/Max Width/Height")]
		public OptionalMinMax MinMaxWidth;

		public OptionalMinMax MinMaxHeight;

		private RectTransform _transform;
		private Vector2 _latestSize;

		private void Start()
		{
			_transform = transform as RectTransform;

			Debug.Assert(_transform != null, this);
			Debug.Assert(CopyWidth.IsSet || CopyHeight.IsSet, this);
		}

		private void LateUpdate()
		{
			if (CopySizeFrom == null) return;
			if (_transform == null) return;

			var copyFromSize = CopySizeFrom.sizeDelta;
			if (_latestSize == copyFromSize) return;
			_latestSize = copyFromSize;
			
			var toSize = _transform.sizeDelta;
			var x = CopyWidth.IsSet ? _latestSize.x + CopyWidth.Value : toSize.x;
			var y = CopyHeight.IsSet ? _latestSize.y + CopyHeight.Value : toSize.y;

			x = MinMaxWidth.GetFixed(x);
			y = MinMaxHeight.GetFixed(y);

			_transform.sizeDelta = new Vector2(x, y);
		}
	}
}