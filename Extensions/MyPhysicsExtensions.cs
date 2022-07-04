using UnityEngine;

namespace MyBox
{
	public static class MyPhysicsExtensions
	{
#if UNITY_PHYSICS_ENABLED
        
		/// <summary>
		/// Sets the state of the source enum, chosen by the 1-bits in the specified bit mask.
		/// </summary>
		public static RigidbodyConstraints BitwiseToggle(this RigidbodyConstraints source, RigidbodyConstraints bitMask, bool state) 
			=> state ? (source | bitMask) : (source & ~bitMask);
        
#endif
        
#if UNITY_PHYSICS2D_ENABLED
        
		/// <summary>
		/// Sets the state of the source enum, chosen by the 1-bits in the specified bit mask.
		/// </summary>
		public static RigidbodyConstraints2D BitwiseToggle(this RigidbodyConstraints2D source, RigidbodyConstraints2D bitMask, bool state)
			=> state ? (source | bitMask) : (source & ~bitMask);
        
#endif
	}
}