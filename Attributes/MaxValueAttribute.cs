using MyBox.Internal;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using MyBox.EditorTools;
#endif

namespace MyBox
{
	public class MaxValueAttribute : AttributeBase
	{
		private readonly float _x;
		private readonly float _y;
		private readonly float _z;

		private readonly bool _vectorValuesSet;

		public MaxValueAttribute(float value)
		{
			_x = value;
		}

		public MaxValueAttribute(float x, float y, float z)
		{
			_x = x;
			_y = y;
			_z = z;
			_vectorValuesSet = true;
		}

#if UNITY_EDITOR
		private string _warning;

		public override void ValidateProperty(SerializedProperty property)
		{
			if (!property.IsNumerical())
			{
				if (_warning == null) _warning = property.name + " caused: [MaxValueAttribute] used with non-numeric property";
				return;
			}

			bool valueHandled = HandleValues(property);
			if (valueHandled) property.serializedObject.ApplyModifiedProperties();
		}

		public override bool OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			if (_warning == null) return false;
			EditorGUI.HelpBox(position, _warning, MessageType.Warning);
			return true;
		}

		public override float? OverrideHeight()
		{
			if (_warning == null) return null;
			return EditorGUIUtility.singleLineHeight;
		}
		

		#region Handle Value
		/// <returns>true if fixed</returns>
		private bool HandleValues(SerializedProperty property)
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
			var maxValue = _x;

			if (property.propertyType == SerializedPropertyType.Integer && property.intValue > maxValue)
			{
				property.intValue = (int) maxValue;
				return true;
			}

			if (property.propertyType == SerializedPropertyType.Float && property.floatValue > maxValue)
			{
				property.floatValue = maxValue;
				return true;
			}

			return false;
		}


		private bool HandleVectors(SerializedProperty property)
		{
			var x = _x;
			var y = _vectorValuesSet ? _y : x;
			var z = _vectorValuesSet ? _z : x;

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
			if (vector[0] > x)
			{
				vector[0] = x;
				handled = true;
			}

			if (vector[1] > y)
			{
				vector[1] = y;
				handled = true;
			}

			if (vector[2] > z)
			{
				vector[2] = z;
				handled = true;
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
			var x = (int) _x;
			var y = _vectorValuesSet ? (int) _y : x;
			var z = _vectorValuesSet ? (int) _z : x;

			if (property.propertyType == SerializedPropertyType.Vector2Int)
			{
				var vector = property.vector2IntValue;
				if (vector.x > x || vector.y > y)
				{
					property.vector2IntValue = new Vector2Int(
						vector.x > x ? x : vector.x,
						vector.y > y ? y : vector.y);
					return true;
				}

				return false;
			}

			if (property.propertyType == SerializedPropertyType.Vector3Int)
			{
				var vector = property.vector3IntValue;
				if (vector.x > x || vector.y > y || vector.z > z)
				{
					property.vector3IntValue = new Vector3Int(
						vector.x > x ? x : vector.x,
						vector.y > y ? y : vector.y,
						vector.z > z ? z : vector.z);
					return true;
				}

				return false;
			}

			return false;
		}
		#endregion
#endif
	}
}