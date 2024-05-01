using System;
using JetBrains.Annotations;
using UnityEngine;

namespace MyBox
{
	/// <summary>
	/// MonoBehaviour with this attribute will be moved to top in the Inspector when added
	/// </summary>
	[AttributeUsage(AttributeTargets.Class), PublicAPI]
	public class TopmostComponentAttribute : Attribute
	{
	}
}

#if UNITY_EDITOR
namespace MyBox.Internal
{
	public static class TopmostComponentHandler
	{
		[UnityEditor.InitializeOnLoadMethod]
		private static void TestAddComponentReaction()
		{
			UnityEditor.ObjectFactory.componentWasAdded -= OnComponentAdded;
			UnityEditor.ObjectFactory.componentWasAdded += OnComponentAdded;
			
			void OnComponentAdded(Component component)
			{
				if (component.GetType().IsDefined(typeof(TopmostComponentAttribute), true)) 
					MyComponentUtility.MoveComponentInspectorToTop(component);
			}
		}
	}
}
#endif