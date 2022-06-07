#if UNITY_EDITOR
using System;
using System.Reflection;
using MyBox.EditorTools;
using UnityEditor;

namespace MyBox.Internal
{
	public static class ConditionalUtility
	{
		public static bool IsConditionMatch(UnityEngine.Object obj, ConditionalData condition)
		{
			var so = new SerializedObject(obj);

			foreach (var fieldCondition in condition)
			{
				bool passed = IsConditionMatch(
					so.FindProperty(fieldCondition.Field),
					fieldCondition.Inverse,
					fieldCondition.CompareAgainst);
				if (!passed) return false;
			}
			
			return IsMethodConditionMatch(obj, condition);
		}
		
		public static bool IsBehaviourConditionMatch(UnityEngine.Object obj, string propertyName, ConditionalData condition)
		{
			var so = new SerializedObject(obj);
			var property = so.FindProperty(propertyName);

			return IsPropertyConditionMatch(property, condition);
		}

		public static bool IsPropertyConditionMatch(SerializedProperty property, ConditionalData conditional)
		{
			foreach (var fieldCondition in conditional)
			{
				bool passed = IsConditionMatch(
					FindRelativeProperty(property, fieldCondition.Field),
					fieldCondition.Inverse,
					fieldCondition.CompareAgainst);
				if (!passed) return false;
			}

			return IsMethodConditionMatch(property.GetParent(), conditional);
		}

		
		private static bool IsMethodConditionMatch(object owner, ConditionalData condition)
		{
			if (!condition.WithMethodCondition) return true;
			
			var predicateMethod = condition.GetMethodCondition(owner);
			if (predicateMethod == null) return true;
			
			bool match = (bool)predicateMethod.Invoke(owner, null);
			if (condition.Inverse) match = !match;
			return match;
		}
		
		private static bool IsConditionMatch(SerializedProperty property, bool inverse, string[] compareAgainst)
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

			if (propertyValueAsString == "-1") //Handle Everything
				return true;
			if (propertyValueAsString == "0") //Handle Nothing
				return false;

			var separateFlags = propertyValueAsString.Split(',');
			foreach (var flag in separateFlags)
			{
				if (ValueMatches(flag.Trim())) return true;
			}

			return false;


			bool ValueMatches(string value)
			{
				foreach (var compare in compareAgainst)
					if (value == compare)
						return true;
				return false;
			}
		}
		
		/// <summary>
		/// Get the other Property which is stored alongside with specified Property, by name
		/// </summary>
		private static SerializedProperty FindRelativeProperty(SerializedProperty property, string propertyName)
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
	}
}
#endif