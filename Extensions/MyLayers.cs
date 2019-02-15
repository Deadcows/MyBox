using UnityEngine;

namespace MyBox
{
	public static class MyLayers
	{
		// Toggle layers lock
		//Tools.lockedLayers = 1 << LayerMask.NameToLayer("LayerName"); // Replace the whole value of lockedLayers. 
		//Tools.lockedLayers |= 1 << LayerMask.NameToLayer("LayerName"); // Add a value to lockedLayers. 
		//Tools.lockedLayers &= ~(1 << LayerMask.NameToLayer("LayerName")); // Remove a value from lockedLayers. 
		//Tools.lockedLayers ^= 1 << LayerMask.NameToLayer("LayerName")); // Toggle a value in lockedLayers.


		public static LayerMask ToLayerMask(int layer)
		{
			return 1 << layer;
		}

		public static bool LayerInMask(this LayerMask mask, int layer)
		{
			return ((1 << layer) & mask) != 0;
		}
	}
}