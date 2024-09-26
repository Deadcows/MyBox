using JetBrains.Annotations;
using UnityEngine;

namespace MyBox
{
	[PublicAPI]
	public static class MyLayers
	{
		public static LayerMask AddLayer(LayerMask mask, int layer) 
			=> mask | (1 << layer);
		public static LayerMask AddLayer(LayerMask mask, string layer) 
			=> AddLayer(mask, LayerMask.NameToLayer(layer));

		public static LayerMask RemoveLayer(LayerMask mask, int layer) 
			=> mask & ~(1 << layer);
		public static LayerMask RemoveLayer(LayerMask mask, string layer) 
			=> RemoveLayer(mask, LayerMask.NameToLayer(layer));

		public static LayerMask ToggleLayer(LayerMask mask, int layer) 
			=> mask ^ (1 << layer);
		public static LayerMask ToggleLayer(LayerMask mask, string layer) 
			=> ToggleLayer(mask, LayerMask.NameToLayer(layer));

		public static LayerMask SetLayer(LayerMask mask, int layer, bool include) 
			=> include ? AddLayer(mask, layer) : RemoveLayer(mask, layer);
		public static LayerMask SetLayer(LayerMask mask, string layer, bool include) 
			=> SetLayer(mask, LayerMask.NameToLayer(layer), include);

		public static LayerMask ToLayerMask(int layer) 
			=> 1 << layer;
		public static LayerMask ToLayerMask(string layer) 
			=> ToLayerMask(LayerMask.NameToLayer(layer));

		public static bool LayerInMask(this LayerMask mask, int layer) 
			=> ((1 << layer) & mask) != 0;
		public static bool LayerInMask(this LayerMask mask, string layer) 
			=> LayerInMask(mask, LayerMask.NameToLayer(layer));
	}
}