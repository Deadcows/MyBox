// ---------------------------------------------------------------------------- 
// Author: Kaynn, Yeo Wen Qin
// https://github.com/Kaynn-Cahya
// Date:   17/02/2019
// ----------------------------------------------------------------------------

using UnityEngine;

namespace MyBox
{
	public class PositiveValueOnlyAttribute : PropertyAttribute
	{
	}
}

#if UNITY_EDITOR
namespace MyBox.Internal
{
	using UnityEditor;

	[CustomPropertyDrawer(typeof(PositiveValueOnlyAttribute))]
	public class PositiveValueOnlyAttributeDrawer : PropertyDrawer
	{
		public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
			return EditorGUI.GetPropertyHeight(property);
		}

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			if (!IsNumerical(property.propertyType))
			{
				DrawColouredRect(position, RedColour);
				EditorGUI.LabelField(position, new GUIContent("", "[PositiveValueOnly] used with non-numeric property"));
			}
			else
			{
				if (HandleNegativeValues(property)) property.serializedObject.ApplyModifiedProperties();
			}

			EditorGUI.PropertyField(position, property, true);
		}


		/// <summary>
		/// Set number value to 0 if less than 0
		/// </summary>
		/// <returns>true if fixed</returns>
		private bool HandleNegativeValues(SerializedProperty property)
		{
			switch (property.propertyType)
			{
				case SerializedPropertyType.Float:
				case SerializedPropertyType.Integer:
					return HandleNumerics(property);

				case SerializedPropertyType.Vector2:
				case SerializedPropertyType.Vector3:
				case SerializedPropertyType.Vector4:
					return HandleVectors(property);

				case SerializedPropertyType.Vector2Int:
				case SerializedPropertyType.Vector3Int:
					return HandleIntVectors(property);
			}

			return false;
		}


		private bool HandleNumerics(SerializedProperty property)
		{
			if (property.propertyType == SerializedPropertyType.Integer && property.intValue < 0)
			{
				property.intValue = 0;
				return true;
			}

			if (property.propertyType == SerializedPropertyType.Float && property.floatValue < 0)
			{
				property.floatValue = 0;
				return true;
			}

			return false;
		}


		private bool HandleVectors(SerializedProperty property)
		{
			Vector4 vector = Vector4.zero;
			switch (property.propertyType)
			{
				case SerializedPropertyType.Vector2:
					vector = property.vector2Value;
					break;
				case SerializedPropertyType.Vector3:
					vector = property.vector3Value;
					break;
				case SerializedPropertyType.Vector4:
					vector = property.vector4Value;
					break;
			}

			bool handled = false;
			for (int i = 0; i < 4; ++i)
			{
				if (vector[i] < 0f)
				{
					vector[i] = 0;
					handled = true;
				}
			}

			switch (property.propertyType)
			{
				case SerializedPropertyType.Vector2:
					property.vector2Value = vector;
					break;
				case SerializedPropertyType.Vector3:
					property.vector3Value = vector;
					break;
				case SerializedPropertyType.Vector4:
					property.vector4Value = vector;
					break;
			}

			return handled;
		}


		private bool HandleIntVectors(SerializedProperty property)
		{
			if (property.propertyType == SerializedPropertyType.Vector2Int)
			{
				var vector = property.vector2IntValue;
				if (vector.x > 0 && vector.y > 0) return false;
				property.vector2IntValue = new Vector2Int(
					vector.x < 0 ? 0 : vector.x,
					vector.y < 0 ? 0 : vector.y);
				return true;
			}

			if (property.propertyType == SerializedPropertyType.Vector3Int)
			{
				var vector = property.vector3IntValue;
				if (vector.x > 0 && vector.y > 0 && vector.z > 0) return false;
				property.vector3Value = new Vector3(
					vector.x < 0 ? 0 : vector.x,
					vector.y < 0 ? 0 : vector.y,
					vector.z < 0 ? 0 : vector.z);
				return true;
			}

			return false;
		}


		private bool IsNumerical(SerializedPropertyType propertyType)
		{
			switch (propertyType)
			{
				case SerializedPropertyType.Float:
				case SerializedPropertyType.Integer:
				case SerializedPropertyType.Vector2:
				case SerializedPropertyType.Vector3:
				case SerializedPropertyType.Vector4:
				case SerializedPropertyType.Vector2Int:
				case SerializedPropertyType.Vector3Int:
					return true;

				default: return false;
			}
		}

		/// <summary>
		/// Draw Rect filled with Color
		/// COPY OF MyGUI.DrawColouredRect()
		/// </summary>
		private static void DrawColouredRect(Rect rect, Color color)
		{
			var defaultBackgroundColor = GUI.backgroundColor;
			GUI.backgroundColor = color;
			GUI.Box(rect, "");
			GUI.backgroundColor = defaultBackgroundColor;
		}

		/// <summary>
		/// COPY OF MyGUI.Red
		/// </summary>
		private static readonly Color RedColour = new Color(.8f, .6f, .6f);
	}
}
#endif