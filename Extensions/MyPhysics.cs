using UnityEngine;

namespace MyBox
{
	public static class MyPhysics
	{
#if UNITY_PHYSICS_ENABLED
        
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
        
#endif

#if UNITY_PHYSICS2D_ENABLED
        
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
        
#endif
	}
}
