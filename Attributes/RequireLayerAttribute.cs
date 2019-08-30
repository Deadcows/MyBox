
using System;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace MyBox.Internal
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

#if UNITY_EDITOR
namespace MyBox.Internal
{
	//TODO: combine Layer Handler and Tag Handler together for performance? 
	//TODO: or add subscription for BeforePlaymodeComponentIteration event?
	//TODO: 		is there a way to log changes made before playmode after playmode (to handle "clear on play")?
	//TODO: 			this way it'll be possible to handle MustBeAssigned here as well
	[InitializeOnLoad]
	class RequireLayerAttributeHandler
	{
		static RequireLayerAttributeHandler()
		{
			EditorApplication.playModeStateChanged += AutoSaveWhenPlaymodeStarts;
		}

		private static void AutoSaveWhenPlaymodeStarts(PlayModeStateChange obj)
		{
			if (EditorApplication.isPlayingOrWillChangePlaymode && !EditorApplication.isPlaying)
			{
				TimeTest.Start("Layer Check");

				var components = Object.FindObjectsOfType<Component>();
				foreach (var component in components)
				{
					foreach (var attribute in component.GetType().GetCustomAttributes(true))
					{
						//if (attribute is RequireLayerAttribute) 
						//	Debug.Log(component.name + " " + attribute.GetType().Name);
					}
				}
//#if UNITY_2019_2_OR_NEWER
//				var requireLayerCollection = TypeCache.GetTypesWithAttribute<RequireLayerAttribute>();
//				foreach (var type in requireLayerCollection)
//				{
//					var toCheck = UnityEngine.Object.FindObjectsOfType(type);
//					foreach (var behaviour in toCheck)
//					{
//						var component = behaviour as Component;
//						Debug.Log(component.gameObject.layer);
//					}
//				}
//#endif
				TimeTest.End();
				//AssetDatabase.SaveAssets();
			}
		}
	}

}
#endif