using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using System.Collections.ObjectModel;
using System.Reflection;
using MyBox.Internal;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace MyBox
{
	/// <summary>
	/// Conditionally Show/Hide field in inspector, based on some other field value 
	/// </summary>
	[AttributeUsage(AttributeTargets.Field)]
	public class ConditionalFieldAttribute : PropertyAttribute
	{
		private readonly string _fieldToCheck;
		private readonly object[] _compareValues;
		private readonly bool _inverse;

		/// <param name="fieldToCheck">String name of field to check value</param>
		/// <param name="inverse">Inverse check result</param>
		/// <param name="compareValues">On which values field will be shown in inspector</param>
		public ConditionalFieldAttribute(string fieldToCheck, bool inverse = false, params object[] compareValues)
		{
			_fieldToCheck = fieldToCheck;
			_inverse = inverse;
			_compareValues = compareValues;
		}

#if UNITY_EDITOR
		public bool CheckBehaviourPropertyVisible(MonoBehaviour behaviour, string propertyName)
		{
			if (string.IsNullOrEmpty(_fieldToCheck)) return true;

			var so = new SerializedObject(behaviour);
			var property = so.FindProperty(propertyName);

			return CheckPropertyVisible(property);
		}


		public bool CheckPropertyVisible(SerializedProperty property)
		{
			var conditionProperty = FindRelativeProperty(property, _fieldToCheck);
			if (conditionProperty == null) return true;

			string asString = AsStringValue(conditionProperty).ToUpper();

			if (_compareValues != null && _compareValues.Length > 0)
			{
				var matchAny = CompareAgainstValues(asString);
				if (_inverse) matchAny = !matchAny;
				return matchAny;
			}

			bool someValueAssigned = asString != "FALSE" && asString != "0" && asString != "NULL";
			if (someValueAssigned) return !_inverse;

			return _inverse;
		}

		/// <summary>
		/// True if the property value matches any of the values in '_compareValues'
		/// </summary>
		private bool CompareAgainstValues(string propertyValueAsString)
		{
			foreach (object valueToCompare in _compareValues)
			{
				bool valueMatches = valueToCompare.ToString().ToUpper() == propertyValueAsString;

				// One of the value is equals to the property value.
				if (valueMatches) return true;
			}

			// None of the value is equals to the property value.
			return false;
		}


		private SerializedProperty FindRelativeProperty(SerializedProperty property, string toGet)
		{
			if (property.depth == 0) return property.serializedObject.FindProperty(toGet);

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
				var warning = who + "Array fields is not supported by [ConditionalFieldAttribute]";

				WarningsPool.Log(warning, target);

				return null;
			}

			return nestedProperty.FindPropertyRelative(toGet);
		}

		// For [Serialized] types with [Conditional] fields
		private SerializedProperty NestedPropertyOrigin(SerializedProperty property, string[] elements)
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


		private string AsStringValue(SerializedProperty prop)
		{
			switch (prop.propertyType)
			{
				case SerializedPropertyType.String:
					return prop.stringValue;

				case SerializedPropertyType.Character:
				case SerializedPropertyType.Integer:
					if (prop.type == "char") return Convert.ToChar(prop.intValue).ToString();
					return prop.intValue.ToString();

				case SerializedPropertyType.ObjectReference:
					return prop.objectReferenceValue != null ? prop.objectReferenceValue.ToString() : "null";

				case SerializedPropertyType.Boolean:
					return prop.boolValue.ToString();

				case SerializedPropertyType.Enum:
					return prop.enumNames[prop.enumValueIndex];

				default:
					return string.Empty;
			}
		}
#endif
	}
}

#if UNITY_EDITOR
namespace MyBox.Internal
{
	[CustomPropertyDrawer(typeof(ConditionalFieldAttribute))]
	public class ConditionalFieldAttributeDrawer : PropertyDrawer
	{
		private bool _multipleAttributes;

		private PropertyAttribute _genericAttribute;
		private PropertyDrawer _genericDrawerInstance;
		private Type _genericDrawerType;

		private static IEnumerable<Type> _typesCache;

		private void LogWarning(string log, SerializedProperty property)
		{
			var warning = "Property <color=brown>" + fieldInfo.Name + "</color>";
			if (fieldInfo.DeclaringType != null) warning += " on behaviour <color=brown>" + fieldInfo.DeclaringType.Name + "</color>";
			warning += " caused: " + log;

			WarningsPool.Log(warning, property.serializedObject.targetObject);
		}

		private void GetPropertyDrawerType(SerializedProperty property)
		{
			if (_genericDrawerInstance != null) return;

			//Get the second attribute flag
			try
			{
				_genericAttribute = (PropertyAttribute) fieldInfo.GetCustomAttributes(typeof(PropertyAttribute), false)[1];

				if (_genericAttribute is ContextMenuItemAttribute || _genericAttribute is SeparatorAttribute | _genericAttribute is AutoPropertyAttribute)
				{
					LogWarning("Does not work with" + _genericAttribute.GetType(), property);
					return;
				}

				if (_genericAttribute is TooltipAttribute) return;
			}
			catch (Exception e)
			{
				LogWarning("Can't find stacked propertyAttribute after ConditionalProperty: " + e, property);
				return;
			}

			//Get the associated attribute drawer
			try
			{
				if (_typesCache == null)
				{
					_typesCache = AppDomain.CurrentDomain.GetAssemblies().SelectMany(x => x.GetTypes())
						.Where(x => typeof(PropertyDrawer).IsAssignableFrom(x) && !x.IsInterface && !x.IsAbstract);
				}

				_genericDrawerType = _typesCache.First(x =>
					(Type) CustomAttributeData.GetCustomAttributes(x).First().ConstructorArguments.First().Value == _genericAttribute.GetType());
			}
			catch (Exception e)
			{
				LogWarning("Can't find property drawer from CustomPropertyAttribute of " + _genericAttribute.GetType() + " : " + e, property);
				return;
			}

			//Create instances of each (including the arguments)
			try
			{
				_genericDrawerInstance = (PropertyDrawer) Activator.CreateInstance(_genericDrawerType);
				//Get arguments
				IList<CustomAttributeTypedArgument> attributeParams = fieldInfo.GetCustomAttributesData()[1].ConstructorArguments;
				IList<CustomAttributeTypedArgument> unpackedParams = new List<CustomAttributeTypedArgument>();
				//Unpack any params object[] args
				foreach (CustomAttributeTypedArgument singleParam in attributeParams)
				{
					if (singleParam.Value.GetType() == typeof(ReadOnlyCollection<CustomAttributeTypedArgument>))
					{
						foreach (CustomAttributeTypedArgument unpackedSingleParam in (ReadOnlyCollection<CustomAttributeTypedArgument>) singleParam.Value)
						{
							unpackedParams.Add(unpackedSingleParam);
						}
					}
					else
					{
						unpackedParams.Add(singleParam);
					}
				}

				object[] attributeParamsObj = unpackedParams.Select(x => x.Value).ToArray();

				if (attributeParamsObj.Any())
				{
					_genericAttribute = (PropertyAttribute) Activator.CreateInstance(_genericAttribute.GetType(), attributeParamsObj);
				}
				else
				{
					_genericAttribute = (PropertyAttribute) Activator.CreateInstance(_genericAttribute.GetType());
				}
			}
			catch (Exception e)
			{
				LogWarning("no constructor available in " + _genericAttribute.GetType() + " : " + e, property);
				return;
			}

			//Reassign the attribute field in the drawer so it can access the argument values
			try
			{
				_genericDrawerType.GetField("m_Attribute", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(_genericDrawerInstance, _genericAttribute);
			}
			catch (Exception e)
			{
				LogWarning("Unable to assign attribute to " + _genericDrawerInstance.GetType() + " : " + e, property);
			}
		}

		private ConditionalFieldAttribute Attribute
		{
			get { return _attribute ?? (_attribute = attribute as ConditionalFieldAttribute); }
		}

		private ConditionalFieldAttribute _attribute;

		private bool _toShow = true;

		public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
			if (fieldInfo.GetCustomAttributes(typeof(PropertyAttribute), false).Count() > 1)
			{
				_multipleAttributes = true;
				GetPropertyDrawerType(property);
			}

			_toShow = Attribute.CheckPropertyVisible(property);
			if (!_toShow) return 0;

			if (_genericDrawerInstance != null)
				return _genericDrawerInstance.GetPropertyHeight(property, label);
			return EditorGUI.GetPropertyHeight(property);
		}

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			if (!_toShow) return;

			if (_multipleAttributes && _genericDrawerInstance != null)
			{
				try
				{
					_genericDrawerInstance.OnGUI(position, property, label);
				}
				catch (Exception e)
				{
					EditorGUI.PropertyField(position, property, label);
					LogWarning("Unable to instantiate " + _genericAttribute.GetType() + " : " + e, property);
				}
			}
			else
			{
				EditorGUI.PropertyField(position, property, label);
			}
		}
	}
}
#endif