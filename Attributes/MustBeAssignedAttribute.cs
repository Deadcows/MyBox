using System;
using UnityEngine;

#if UNITY_EDITOR
using System.Linq;
using System.Reflection;
using UnityEditor;
using Object = UnityEngine.Object;
#endif

namespace MyBox
{
	/// <summary>
	/// Apply to reference type property in MonoBehaviour to check if it is null at game start
	/// </summary>
	public class MustBeAssignedAttribute : PropertyAttribute
	{
	}
}

#if UNITY_EDITOR
namespace MyBox.Internal
{
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
						Debug.LogError(string.Format("{0} caused: {1} is Value Type with default value", typeOfScript.Name, field.Name),
							behaviour.gameObject);
						continue;
					}

					// Null reference type
					if (propValue == null || propValue.Equals(null))
					{
						Debug.LogError(string.Format("{0} caused: {1} is not assigned (null value)", typeOfScript.Name, field.Name),
							behaviour.gameObject);
						continue;
					}

					// Empty string
					if (field.FieldType == typeof(string) && (string) propValue == string.Empty)
					{
						Debug.LogError(string.Format("{0} caused: {1} is not assigned (empty string)", typeOfScript.Name, field.Name),
							behaviour.gameObject);
						continue;
					}

					// Empty Array
					var arr = propValue as Array;
					if (arr != null && arr.Length == 0)
					{
						Debug.LogError(string.Format("{0} caused: {1} is not assigned (empty array)", typeOfScript.Name, field.Name),
							behaviour.gameObject);
					}
				}
			}
		}

		private class ConditionalFieldChecker
		{
			private readonly Type _conditionallyVisibleType = typeof(ConditionalFieldAttribute);

			public bool IsVisible(FieldInfo field, MonoBehaviour behaviour)
			{
				if (_conditionallyVisibleType == null) return true;
				if (!field.IsDefined(_conditionallyVisibleType, false)) return true;

				// Get a specific attribute of this field
				var conditionalFieldAttribute = field.GetCustomAttributes(_conditionallyVisibleType, false)
					.Select(a => a as ConditionalFieldAttribute)
					.SingleOrDefault();

				return conditionalFieldAttribute == null ||
				       conditionalFieldAttribute.CheckBehaviourPropertyVisible(behaviour, field.Name);
			}
		}
	}
}
#endif