using System;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEditor;
using Object = UnityEngine.Object;

[InitializeOnLoad]
public class MustBeAssignedAttributeChecker
{
	static MustBeAssignedAttributeChecker()
	{
		EditorApplication.update += CheckOnce;
	}

	private static void CheckOnce()
	{
		if (Application.isPlaying)
		{
			EditorApplication.update -= CheckOnce;
			CheckComponents();
		}
	}


	private static void CheckComponents()
	{
		MonoBehaviour[] scripts = Object.FindObjectsOfType<MonoBehaviour>();
		AssertComponents(scripts);
	}


	private static void AssertComponents(MonoBehaviour[] components)
	{
		ConditionalFieldChecker conditionalFieldChecker = new ConditionalFieldChecker();


		foreach (MonoBehaviour behaviour in components)
		{
			Type typeOfScript = behaviour.GetType();
			FieldInfo[] mustBeAssignedFields = typeOfScript
				.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
				.Where(field => field.IsDefined(typeof(MustBeAssignedAttribute), false)).ToArray();

			foreach (FieldInfo field in mustBeAssignedFields)
			{
				object propValue = field.GetValue(behaviour);

				// Used to check ConditionalFieldAttribute
				if (!conditionalFieldChecker.IsVisible(field, behaviour)) continue;

				// Value Type with default value
				if (field.FieldType.IsValueType && Activator.CreateInstance(field.FieldType).Equals(propValue))
				{
					Debug.LogError($"{typeOfScript.Name} caused: {field.Name} is Value Type with default value",
						behaviour.gameObject);
					continue;
				}

				// Null reference type
				if (propValue == null || propValue.Equals(null))
				{
					Debug.LogError($"{typeOfScript.Name} caused: {field.Name} is not assigned (null value)",
						behaviour.gameObject);
					continue;
				}

				// Empty string
				if (field.FieldType == typeof(string) && (string) propValue == string.Empty)
				{
					Debug.LogError($"{typeOfScript.Name} caused: {field.Name} is not assigned (empty string)",
						behaviour.gameObject);
					continue;
				}

				// Empty Array
				if (propValue is Array arr && arr.Length == 0)
				{
					Debug.LogError($"{typeOfScript.Name} caused: {field.Name} is not assigned (empty array)",
						behaviour.gameObject);
				}
			}
		}
	}

	private class ConditionalFieldChecker
	{
		private readonly Type _conditionallyVisibleType;
		private readonly MethodInfo _conditionallyVisibleMethod;

		private const string TypePath = "ConditionalFieldAttribute, Assembly-CSharp-firstpass, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null";
		private const string MethodPath = "CheckBehaviourPropertyVisible";

		public ConditionalFieldChecker()
		{
			_conditionallyVisibleType = Type.GetType(TypePath);
			if (_conditionallyVisibleType != null) _conditionallyVisibleMethod = _conditionallyVisibleType.GetMethod(MethodPath);
		}

		public bool IsVisible(FieldInfo field, MonoBehaviour behaviour)
		{
			// ConditionalFieldAttribute is in assembly
			if (_conditionallyVisibleType == null) return true;
			// ConditionalFieldAttribute is defined for this field
			if (!field.IsDefined(_conditionallyVisibleType)) return true;

			// Get a specific attribute of this field
			var conditionalFieldAttribute = field.GetCustomAttribute(_conditionallyVisibleType);

			var visible = _conditionallyVisibleMethod.Invoke(conditionalFieldAttribute, new object[] {behaviour, field.Name}) as bool?;

			return visible == null || visible.Value;
		}
	}
}