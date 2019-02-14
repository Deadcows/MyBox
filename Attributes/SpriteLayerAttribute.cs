// ---------------------------------------------------------------------------- 
// Author: Kaynn, Yeo Wen Qin
// https://github.com/Kaynn-Cahya
// Date:   11/02/2019
// ----------------------------------------------------------------------------

using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace MyBox
{
	public class SpriteLayerAttribute : PropertyAttribute
	{
	}

#if UNITY_EDITOR
	[CustomPropertyDrawer(typeof(SpriteLayerAttribute))]
	public class SpriteLayerAttributeDrawer : PropertyDrawer
	{
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			if (property.propertyType != SerializedPropertyType.Integer)
			{
				Debug.LogWarning(string.Format("Property <color=brown>{0}</color> in object <color=brown>{1}</color> is of wrong type. Expected: Int",
					property.name, GetTargettedObjectFromProperty(property)));
			}
			else
			{
				var spriteLayerNames = GetSpriteLayerNames();

				if (!ArrayIsNullOrEmpty(spriteLayerNames))
				{
					HandleSpriteLayerSelectionUI(position, property, label, spriteLayerNames);
				}
				else
				{
					// Shouldn't occur since Default layer exists all the time.
					Debug.LogWarning(string.Format(
						"Property <color=brown>{0}</color> in object <color=brown>{1}</color>.Reason: There is no sprite layers to select from!", property.name,
						property.name));
				}
			}
		}

		private void HandleSpriteLayerSelectionUI(Rect position, SerializedProperty property, GUIContent label, string[] spriteLayerNames)
		{
			EditorGUI.BeginProperty(position, label, property);

			// To show which sprite layer is currently selected.
			int currentSpriteLayerIndex;
			bool layerFound = TryGetSpriteLayerIndexFromProperty(out currentSpriteLayerIndex, spriteLayerNames, property);

			if (!layerFound)
			{
				// Set to default layer. (Previous layer was removed)
				Debug.Log(string.Format(
					"Property <color=brown>{0}</color> in object <color=brown>{1}</color> is set to the default layer. Reason: previously selected layer was removed.",
					property.name, GetTargettedObjectFromProperty(property)));
				property.intValue = 0;
				currentSpriteLayerIndex = 0;
			}

			int selectedSpriteLayerIndex = EditorGUI.Popup(position, label.text, currentSpriteLayerIndex, spriteLayerNames);

			// Change property value if user selects a new sprite layer.
			if (selectedSpriteLayerIndex != currentSpriteLayerIndex)
			{
				property.intValue = SortingLayer.NameToID(spriteLayerNames[selectedSpriteLayerIndex]);
			}

			EditorGUI.EndProperty();
		}

		#region Util

		private Object GetTargettedObjectFromProperty(SerializedProperty property)
		{
			return property.serializedObject.targetObject;
		}

		private bool TryGetSpriteLayerIndexFromProperty(out int index, string[] spriteLayerNames, SerializedProperty property)
		{
			// To keep the property's value consistent, after the layers have been sorted around.
			string layerName = SortingLayer.IDToName(property.intValue);

			// Return the index where on it matches.
			for (int i = 0; i < spriteLayerNames.Length; ++i)
			{
				if (spriteLayerNames[i].Equals(layerName))
				{
					index = i;
					return true;
				}
			}

			// The current layer was removed.
			index = -1;
			return false;
		}

		private bool ArrayIsNullOrEmpty<T>(T[] array)
		{
			bool isNullOrEmpty = false;

			if (array == null)
			{
				isNullOrEmpty = true;
			}
			else if (array.Length < 1)
			{
				isNullOrEmpty = true;
			}

			return isNullOrEmpty;
		}

		private string[] GetSpriteLayerNames()
		{
			string[] result = new string[SortingLayer.layers.Length];

			for (int i = 0; i < result.Length; ++i)
			{
				result[i] = SortingLayer.layers[i].name;
			}

			return result;
		}

		#endregion
	}
#endif
}