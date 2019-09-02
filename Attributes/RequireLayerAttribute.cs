using System;

namespace MyBox
{
	[AttributeUsage(AttributeTargets.Class)]
	public class RequireLayerAttribute : Attribute
	{
		public string Layer;

		public RequireLayerAttribute(string layer)
		{
			Layer = layer;
		}
	}
}