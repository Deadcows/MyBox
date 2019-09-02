using System;

namespace MyBox
{
	[AttributeUsage(AttributeTargets.Class)]
	public class RequireTagAttribute : Attribute
	{
		public string Tag;

		public RequireTagAttribute(string tag)
		{
			Tag = tag;
		}
	}
}