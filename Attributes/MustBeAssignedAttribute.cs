using System;
using UnityEngine;

namespace MyBox
{
	/// <summary>
	/// Apply to MonoBehaviour field to assert that this field is assigned via inspector (not null, false, empty of zero) on playmode
	/// </summary>
	[AttributeUsage(AttributeTargets.Field)]
	public class MustBeAssignedAttribute : PropertyAttribute
	{
	}
}

#if UNITY_EDITOR
namespace MyBox.Internal
{
	using System.Linq;
	using System.Reflection;
	using UnityEditor;
	using Object = UnityEngine.Object;

	[InitializeOnLoad]
	public class MustBeAssignedAttributeChecker
	{
		public static Func<FieldInfo, MonoBehaviour, bool> ExcludeFieldFilter;

		static MustBeAssignedAttributeChecker()
		{
			EditorApplication.update += CheckOnce;
		}

		private static void CheckOnce()
		{
			if (Application.isPlaying)
			{
				EditorApplication.update -= CheckOnce;
				AssertComponents();
			}
		}

		private static bool FieldExcluded(FieldInfo field, MonoBehaviour behaviour)
		{
			if (ExcludeFieldFilter == null) return false;

			foreach (var filterDelegate in ExcludeFieldFilter.GetInvocationList())
			{
				var filter = filterDelegate as Func<FieldInfo, MonoBehaviour, bool>;
				if (filter != null && filter(field, behaviour)) return true;
			}

			return false;
		}

		private static void AssertComponents()
		{
			MonoBehaviour[] components = Object.FindObjectsOfType<MonoBehaviour>();

			foreach (MonoBehaviour behaviour in components)
			{
				Type typeOfScript = behaviour.GetType();
				FieldInfo[] mustBeAssignedFields = typeOfScript
					.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
					.Where(field => field.IsDefined(typeof(MustBeAssignedAttribute), false)).ToArray();

				foreach (FieldInfo field in mustBeAssignedFields)
				{
					object propValue = field.GetValue(behaviour);

					// Used by external systems to exclude specific fields.
					// Specifically for ConditionalFieldAttribute
					if (FieldExcluded(field, behaviour)) continue;

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
	}
}
#endif