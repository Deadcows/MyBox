using System;
using System.Linq;
using UnityEngine;
using MyBox.Internal;

namespace MyBox
{
	/// <summary>
	/// Conditionally Show/Hide field in inspector, based on some other field or property value
	/// </summary>
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
	public class ConditionalFieldAttribute : PropertyAttribute
	{
		public bool IsSet => Data != null && Data.IsSet;
		public readonly ConditionalData Data;

		/// <param name="fieldToCheck">String name of field to check value</param>
		/// <param name="inverse">Inverse check result</param>
		/// <param name="compareValues">On which values field will be shown in inspector</param>
		public ConditionalFieldAttribute(string fieldToCheck, bool inverse = false, params object[] compareValues)
			=> Data = new ConditionalData(fieldToCheck, inverse, compareValues);

		
		public ConditionalFieldAttribute(string[] fieldToCheck, bool[] inverse = null, params object[] compare)
			=> Data = new ConditionalData(fieldToCheck, inverse, compare);

		public ConditionalFieldAttribute(params string[] fieldToCheck) => Data = new ConditionalData(fieldToCheck);
		public ConditionalFieldAttribute(bool useMethod, string method, bool inverse = false) 
			=> Data = new ConditionalData(useMethod, method, inverse);
	}
}

#if UNITY_EDITOR
namespace MyBox.Internal
{
	using UnityEditor;

	[CustomPropertyDrawer(typeof(ConditionalFieldAttribute))]
	public class ConditionalFieldAttributeDrawer : PropertyDrawer
	{
		private bool _toShow = true;
		private bool _initialized;
		private PropertyDrawer _customPropertyDrawer;

		public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
			if (!(attribute is ConditionalFieldAttribute conditional)) return EditorGUI.GetPropertyHeight(property);
			
			CachePropertyDrawer(property);
			_toShow = ConditionalUtility.IsPropertyConditionMatch(property, conditional.Data);
			if (!_toShow) return -2;

			if (_customPropertyDrawer != null) return _customPropertyDrawer.GetPropertyHeight(property, label);
			return EditorGUI.GetPropertyHeight(property);
		}

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			if (!_toShow) return;

			if (!CustomDrawerUsed()) EditorGUI.PropertyField(position, property, label, true);

			
			bool CustomDrawerUsed()
			{
				if (_customPropertyDrawer == null) return false;
				
				try
				{
					_customPropertyDrawer.OnGUI(position, property, label);
					return true;
				}
				catch (Exception e)
				{
					WarningsPool.LogWarning(property,
						"Unable to use CustomDrawer of type " + _customPropertyDrawer.GetType() + ": " + e,
						property.serializedObject.targetObject);

					return false;
				}
			}
		}
		
		/// <summary>
		/// Try to find and cache any PropertyDrawer or PropertyAttribute on the field
		/// </summary>
		private void CachePropertyDrawer(SerializedProperty property)
		{
			if (_initialized) return;
			_initialized = true;
			if (fieldInfo == null) return;

			var customDrawer = CustomDrawerUtility.GetPropertyDrawerForProperty(property, fieldInfo, attribute);
			if (customDrawer == null) customDrawer = TryCreateAttributeDrawer();

			_customPropertyDrawer = customDrawer;
			
			
			// Try to get drawer for any other Attribute on the field
			PropertyDrawer TryCreateAttributeDrawer()
			{
				var secondAttribute = TryGetSecondAttribute();
				if (secondAttribute == null) return null;
				
				var attributeType = secondAttribute.GetType();
				var customDrawerType = CustomDrawerUtility.GetPropertyDrawerTypeForFieldType(attributeType);
				if (customDrawerType == null) return null;

				return CustomDrawerUtility.InstantiatePropertyDrawer(customDrawerType, fieldInfo, secondAttribute);
				
				
				//Get second attribute if any
				Attribute TryGetSecondAttribute()
				{
					return (PropertyAttribute)fieldInfo.GetCustomAttributes(typeof(PropertyAttribute), false)
						.FirstOrDefault(a => !(a is ConditionalFieldAttribute));
				}
			}
		}
	}
}
#endif
