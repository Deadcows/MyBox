#if UNITY_EDITOR
using Plugins.MyBox.Tools;

namespace MyBox.Internal
{
	using UnityEditor;
	using UnityEngine;

	//TODO: combine Layer Handler and Tag Handler together for performance? 
	//TODO: or add subscription for BeforePlaymodeComponentIteration event?
	//TODO: 		is there a way to log changes made before playmode after playmode (to handle "clear on play")?
	//TODO: 			this way it'll be possible to handle MustBeAssigned here as well
	[InitializeOnLoad]
	public class RequireLayerOtRagAttributeHandler
	{
		static RequireLayerOtRagAttributeHandler()
		{
			EditorApplication.playModeStateChanged += AutoSaveWhenPlaymodeStarts;
		}

		private static void AutoSaveWhenPlaymodeStarts(PlayModeStateChange obj)
		{
			if (EditorApplication.isPlayingOrWillChangePlaymode && !EditorApplication.isPlaying)
			{
				var components = Object.FindObjectsOfType<Component>();
				foreach (var component in components)
				{
					foreach (var attribute in component.GetType().GetCustomAttributes(true))
					{
						var layerAttribute = attribute as RequireLayerAttribute;
						if (layerAttribute != null)
						{
							var requiredLayer = LayerMask.NameToLayer(layerAttribute.Layer);
							if (component.gameObject.layer == requiredLayer) continue;

							Debug.LogWarning("Layer of " + component.name + " changed by RequireLayerAttribute to " + layerAttribute.Layer);
							component.gameObject.layer = requiredLayer;
							EditorUtility.SetDirty(component);
							
							continue;
						}

						var tagAttribute = attribute as RequireTagAttribute;
						if (tagAttribute != null)
						{
							if (component.CompareTag(tagAttribute.Tag)) continue;

							Debug.LogWarning("Tag of " + component.name + " changed by RequireTagAttribute to " + tagAttribute.Tag);
							component.gameObject.tag = tagAttribute.Tag;
							EditorUtility.SetDirty(component);
						}
					}
				}
			}
		}
	}
}
#endif
