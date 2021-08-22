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
	}
}
