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
		public OptionalInt CopyWidth = OptionalInt.WithValue(0);
		public OptionalInt CopyHeight = OptionalInt.WithValue(0);

		[Separator("Optional Min/Max Width/Height")]
		public OptionalMinMax MinMaxWidth;
		public OptionalMinMax MinMaxHeight;


		private RectTransform _transform;
		private Vector2 _latestSize;

		private void Start()
		{
			_transform = transform as RectTransform;

			if (_transform == null) Debug.LogError(name + " Caused: Transform is not a RectTransform", this);
			if (!CopyWidth.IsSet && !CopyHeight.IsSet) Debug.LogError(name + " Caused: You must set CopyWidth or CopyHeight for UISizeBy to work", this);
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

#if UNITY_EDITOR
		[ButtonMethod]
		private void UpdateView()
		{
			_latestSize = Vector2.zero;
			UnityEditor.Undo.RecordObject(_transform, "UISizeBy.UpdateView");
			LateUpdate();
		}
#endif
	}
}