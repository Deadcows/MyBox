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
		foreach (MonoBehaviour script in components)
		{
			Type myType = script.GetType();
			FieldInfo[] fields = myType.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
				.Where(field => field.IsDefined(typeof(MustBeAssignedAttribute), false)).ToArray();

			foreach (FieldInfo field in fields)
			{
				object propValue = field.GetValue(script);
				
				// Value Type with default value
				if (field.FieldType.IsValueType && Activator.CreateInstance(field.FieldType).Equals(propValue))
				{
					Debug.LogError($"{myType.Name} caused: {field.Name} is Value Type with default value", script.gameObject);
					continue;
				}

				// Null reference type
				if (propValue == null || propValue.Equals(null))
				{
					Debug.LogError($"{myType.Name} caused: {field.Name} is not assigned (null value)", script.gameObject);
					continue;
				}

				// Empty string
				if (field.FieldType == typeof(string) && (string) propValue == string.Empty)
				{
					Debug.LogError($"{myType.Name} caused: {field.Name} is not assigned (empty string)", script.gameObject);
					continue;
				}

				// Empty Array
				if (propValue is Array arr && arr.Length == 0)
				{
					Debug.LogError($"{myType.Name} caused: {field.Name} is not assigned (empty array)", script.gameObject);
				}
			}
		}
	}
}