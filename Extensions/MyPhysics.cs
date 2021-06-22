using UnityEngine;

namespace MyBox
{
	public static partial class MyPhysics
	{
		/// <summary>
		/// Sets the state of constraints for the source Rigidbody.
		/// </summary>
		public static Rigidbody ToggleConstraints(this Rigidbody source,
			RigidbodyConstraints constraints,
			bool state)
		{
			if (state) source.constraints = source.constraints | constraints;
			else source.constraints = source.constraints & ~constraints;
			return source;
		}

		/// <summary>
		/// Sets the state of constraints for the source Rigidbody2D.
		/// </summary>
		public static Rigidbody2D ToggleConstraints(this Rigidbody2D source,
			RigidbodyConstraints2D constraints,
			bool state)
		{
			if (state) source.constraints = source.constraints | constraints;
			else source.constraints = source.constraints & ~constraints;
			return source;
		}
	}
}
