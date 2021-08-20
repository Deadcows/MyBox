using UnityEngine;

namespace MyBox
{
	public static class MyPhysics
	{
		/// <summary>
		/// Sets the state of constraints for the source Rigidbody.
		/// </summary>
		public static Rigidbody ToggleConstraints(this Rigidbody source,
			RigidbodyConstraints constraints,
			bool state)
		{
			source.constraints = source.constraints.BitwiseToggle(constraints, state);
			return source;
		}

		/// <summary>
		/// Sets the state of constraints for the source Rigidbody2D.
		/// </summary>
		public static Rigidbody2D ToggleConstraints(this Rigidbody2D source,
			RigidbodyConstraints2D constraints,
			bool state)
		{
			source.constraints = source.constraints.BitwiseToggle(constraints, state);
			return source;
		}

		/// <summary>
		/// Gets a point with the same screen point on the specified Camera as the
		/// source point, but at the specified distance from said Camera.
		/// </summary>
		public static Vector3 SetCameraDepthFrom(this Vector3 worldPos,
			Camera projectingCamera,
			float distance,
			Camera.MonoOrStereoscopicEye eye = Camera.MonoOrStereoscopicEye.Mono)
		{
			var screenPoint = projectingCamera.WorldToScreenPoint(worldPos, eye);
			return projectingCamera.ScreenToWorldPoint(screenPoint.SetZ(distance),
				eye);
		}

		/// <summary>
		/// Sets the lossy scale of the source Transform.
		/// </summary>
		public static Transform SetLossyScale(this Transform source,
			Vector3 targetLossyScale)
		{
			var scaleFactorsToTarget = Vector3.Scale(targetLossyScale,
				new Vector3(1f / source.lossyScale.x,
					1f / source.lossyScale.y,
					1f / source.lossyScale.z));
			source.localScale = Vector3.Scale(source.localScale,
				scaleFactorsToTarget);
			return source;
		}
	}
}
