using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;

namespace MyBox
{
	/// <summary>
	/// Conditionally Show/Hide field in inspector, based on some other field value
	/// </summary>
	[AttributeUsage(AttributeTargets.Field)]
	public class ConditionalFieldAttribute : PropertyAttribute
	{
		public readonly string FieldToCheck;
		public readonly string[] CompareValues;
		public readonly bool Inverse;

		/// <param name="fieldToCheck">String name of field to check value</param>
		/// <param name="inverse">Inverse check result</param>
		/// <param name="compareValues">On which values field will be shown in inspector</param>
		public ConditionalFieldAttribute(string fieldToCheck, bool inverse = false, params object[] compareValues)
		{
			FieldToCheck = fieldToCheck;
			Inverse = inverse;
			CompareValues = compareValues.Select(c => c.ToString().ToUpper()).ToArray();
		}
	}
}

#if UNITY_EDITOR
namespace MyBox.Internal
{
	using UnityEditor;
	using EditorTools;

	[CustomPropertyDrawer(typeof(ConditionalFieldAttribute))]
	public class ConditionalFieldAttributeDrawer : PropertyDrawer
	{
		private bool _toShow = true;


		/// <summary>
		/// Key is Associated with drawer type (the T in [CustomPropertyDrawer(typeof(T))])
		/// Value is PropertyDrawer Type
		/// </summary>
		private static Dictionary<Type, Type> _allPropertyDrawersInDomain;


		private bool _initialized;
		private PropertyDrawer _customAttributeDrawer;
		private PropertyDrawer _customTypeDrawer;

		public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
			if (!(attribute is ConditionalFieldAttribute conditional)) return 0;

			Initialize(property);

			var propertyToCheck = ConditionalFieldUtility.FindRelativeProperty(property, conditional.FieldToCheck);
			_toShow = ConditionalFieldUtility.PropertyIsVisible(propertyToCheck, conditional.Inverse, conditional.CompareValues);
			if (!_toShow) return 0;

			if (_customAttributeDrawer != null) return _customAttributeDrawer.GetPropertyHeight(property, label);
			if (_customTypeDrawer != null) return _customTypeDrawer.GetPropertyHeight(property, label);

			return EditorGUI.GetPropertyHeight(property);
		}

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			if (!_toShow) return;

			if (_customAttributeDrawer != null) TryUseAttributeDrawer();
			else if (_customTypeDrawer != null) TryUseTypeDrawer();
			else EditorGUI.PropertyField(position, property, label, true);


			void TryUseAttributeDrawer()
			{
				try
				{
					_customAttributeDrawer.OnGUI(position, property, label);
				}
				catch (Exception e)
				{
					EditorGUI.PropertyField(position, property, label);
					LogWarning("Unable to use Custom Attribute Drawer " + _customAttributeDrawer.GetType() + " : " + e, property);
				}
			}

			void TryUseTypeDrawer()
			{
				try
				{
					_customTypeDrawer.OnGUI(position, property, label);
				}
				catch (Exception e)
				{
					EditorGUI.PropertyField(position, property, label);
					LogWarning("Unable to instantiate " + fieldInfo.FieldType + " : " + e, property);
				}
			}
		}


		private void Initialize(SerializedProperty property)
		{
			if (_initialized) return;

			CacheAllDrawersInDomain();

			TryGetCustomAttributeDrawer();
			TryGetCustomTypeDrawer();

			_initialized = true;


			void CacheAllDrawersInDomain()
			{
				if (!_allPropertyDrawersInDomain.IsNullOrEmpty()) return;

				_allPropertyDrawersInDomain = new Dictionary<Type, Type>();
				var propertyDrawerType = typeof(PropertyDrawer);

				var allDrawerTypesInDomain = AppDomain.CurrentDomain.GetAssemblies()
					.SelectMany(x => x.GetTypes())
					.Where(t => propertyDrawerType.IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract);

				foreach (var type in allDrawerTypesInDomain)
				{
					var drawerAttribute = CustomAttributeData.GetCustomAttributes(type).FirstOrDefault();
					if (drawerAttribute == null) continue;
					var associatedType = drawerAttribute.ConstructorArguments.FirstOrDefault().Value as Type;
					if (associatedType == null) continue;

					if (_allPropertyDrawersInDomain.ContainsKey(associatedType)) continue;
					_allPropertyDrawersInDomain.Add(associatedType, type);
				}
			}

			void TryGetCustomAttributeDrawer()
			{
				if (fieldInfo == null) return;
				//Get the second attribute flag
				var secondAttribute = (PropertyAttribute) fieldInfo.GetCustomAttributes(typeof(PropertyAttribute), false)
					.FirstOrDefault(a => !(a is ConditionalFieldAttribute));
				if (secondAttribute == null) return;
				var genericAttributeType = secondAttribute.GetType();

				//Get the associated attribute drawer
				if (!_allPropertyDrawersInDomain.ContainsKey(genericAttributeType)) return;

				var customAttributeDrawerType = _allPropertyDrawersInDomain[genericAttributeType];
				var customAttributeData = fieldInfo.GetCustomAttributesData().FirstOrDefault(a => a.AttributeType == secondAttribute.GetType());
				if (customAttributeData == null) return;


				//Create drawer for custom attribute
				try
				{
					_customAttributeDrawer = (PropertyDrawer) Activator.CreateInstance(customAttributeDrawerType);
					var attributeField = customAttributeDrawerType.GetField("m_Attribute", BindingFlags.Instance | BindingFlags.NonPublic);
					if (attributeField != null) attributeField.SetValue(_customAttributeDrawer, secondAttribute);
				}
				catch (Exception e)
				{
					LogWarning("Unable to construct drawer for " + secondAttribute.GetType() + " : " + e, property);
				}
			}

			void TryGetCustomTypeDrawer()
			{
				if (fieldInfo == null) return;
				// Skip checks for mscorlib.dll
				if (fieldInfo.FieldType.Module.ScopeName.Equals(typeof(int).Module.ScopeName)) return;


				// Of all property drawers in the assembly we need to find one that affects target type
				// or one of the base types of target type
				Type fieldDrawerType = null;
				Type fieldType = fieldInfo.FieldType;
				while (fieldType != null)
				{
					if (_allPropertyDrawersInDomain.ContainsKey(fieldType))
					{
						fieldDrawerType = _allPropertyDrawersInDomain[fieldType];
						break;
					}

					fieldType = fieldType.BaseType;
				}

				if (fieldDrawerType == null) return;

				//Create instances of each (including the arguments)
				try
				{
					_customTypeDrawer = (PropertyDrawer) Activator.CreateInstance(fieldDrawerType);
				}
				catch (Exception e)
				{
					LogWarning("No constructor available in " + fieldType + " : " + e, property);
					return;
				}

				//Reassign the attribute field in the drawer so it can access the argument values
				var attributeField = fieldDrawerType.GetField("m_Attribute", BindingFlags.Instance | BindingFlags.NonPublic);
				if (attributeField != null) attributeField.SetValue(_customTypeDrawer, attribute);
				var fieldInfoField = fieldDrawerType.GetField("m_FieldInfo", BindingFlags.Instance | BindingFlags.NonPublic);
				if (fieldInfoField != null) fieldInfoField.SetValue(_customTypeDrawer, fieldInfo);
			}
		}

		private void LogWarning(string log, SerializedProperty property)
		{
			var warning = "Property <color=brown>" + fieldInfo.Name + "</color>";
			if (fieldInfo != null && fieldInfo.DeclaringType != null)
				warning += " on behaviour <color=brown>" + fieldInfo.DeclaringType.Name + "</color>";
			warning += " caused: " + log;

			WarningsPool.LogWarning(warning, property.serializedObject.targetObject);
		}
	}

	public static class ConditionalFieldUtility
	{
		#region Property Is Visible

		public static bool PropertyIsVisible(SerializedProperty property, bool inverse, string[] compareAgainst)
		{
			if (property == null) return true;

			string asString = property.AsStringValue().ToUpper();

			if (compareAgainst != null && compareAgainst.Length > 0)
			{
				var matchAny = CompareAgainstValues(asString, compareAgainst, IsFlagsEnum());
				if (inverse) matchAny = !matchAny;
				return matchAny;
			}

			bool someValueAssigned = asString != "FALSE" && asString != "0" && asString != "NULL";
			if (someValueAssigned) return !inverse;

			return inverse;
			
			
			bool IsFlagsEnum()
			{
				if (property.propertyType != SerializedPropertyType.Enum) return false;
				var value = property.GetValue();
				if (value == null) return false;
				return value.GetType().GetCustomAttribute<FlagsAttribute>() != null;
			}
		}

		
		/// <summary>
		/// True if the property value matches any of the values in '_compareValues'
		/// </summary>
		private static bool CompareAgainstValues(string propertyValueAsString, string[] compareAgainst, bool handleFlags)
		{
			if (!handleFlags) return ValueMatches(propertyValueAsString);
			
			var separateFlags = propertyValueAsString.Split(',');
			foreach (var flag in separateFlags)
			{
				if (ValueMatches(flag.Trim())) return true;
			}

			return false;

			
			bool ValueMatches(string value)
			{
				foreach (var compare in compareAgainst) if (value == compare) return true;
				return false;
			}
		}

		#endregion


		#region Find Relative Property

		public static SerializedProperty FindRelativeProperty(SerializedProperty property, string propertyName)
		{
			if (property.depth == 0) return property.serializedObject.FindProperty(propertyName);

			var path = property.propertyPath.Replace(".Array.data[", "[");
			var elements = path.Split('.');

			var nestedProperty = NestedPropertyOrigin(property, elements);

			// if nested property is null = we hit an array property
			if (nestedProperty == null)
			{
				var cleanPath = path.Substring(0, path.IndexOf('['));
				var arrayProp = property.serializedObject.FindProperty(cleanPath);
				var target = arrayProp.serializedObject.targetObject;

				var who = "Property <color=brown>" + arrayProp.name + "</color> in object <color=brown>" + target.name + "</color> caused: ";
				var warning = who + "Array fields is not supported by [ConditionalFieldAttribute]. Consider to use <color=blue>CollectionWrapper</color>";

				WarningsPool.LogWarning(warning, target);

				return null;
			}

			return nestedProperty.FindPropertyRelative(propertyName);
		}

		// For [Serialized] types with [Conditional] fields
		private static SerializedProperty NestedPropertyOrigin(SerializedProperty property, string[] elements)
		{
			SerializedProperty parent = null;

			for (int i = 0; i < elements.Length - 1; i++)
			{
				var element = elements[i];
				int index = -1;
				if (element.Contains("["))
				{
					index = Convert.ToInt32(element.Substring(element.IndexOf("[", StringComparison.Ordinal))
						.Replace("[", "").Replace("]", ""));
					element = element.Substring(0, element.IndexOf("[", StringComparison.Ordinal));
				}

				parent = i == 0
					? property.serializedObject.FindProperty(element)
					: parent != null
						? parent.FindPropertyRelative(element)
						: null;

				if (index >= 0 && parent != null) parent = parent.GetArrayElementAtIndex(index);
			}

			return parent;
		}

		#endregion

		#region Behaviour Property Is Visible

		public static bool BehaviourPropertyIsVisible(MonoBehaviour behaviour, string propertyName, ConditionalFieldAttribute appliedAttribute)
		{
			if (string.IsNullOrEmpty(appliedAttribute.FieldToCheck)) return true;

			var so = new SerializedObject(behaviour);
			var property = so.FindProperty(propertyName);
			var targetProperty = FindRelativeProperty(property, appliedAttribute.FieldToCheck);

			return PropertyIsVisible(targetProperty, appliedAttribute.Inverse, appliedAttribute.CompareValues);
		}

		#endregion
	}
}
#endif