using System;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEditor;
using Object = UnityEngine.Object;

[InitializeOnLoad]
public class MustBeAssignedAttributeChecker {

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

				if (field.FieldType.IsValueType)
				{
					Debug.Assert(!Activator.CreateInstance(field.FieldType).Equals(propValue),
						myType.Name + " caused: " + field.Name + " is Value Type with default value", script);
				}

				else Debug.Assert(!(propValue == null || propValue.Equals(null)), 
					myType.Name + " caused: " + field.Name + " is not assigned", script);
			}
		}
	}
}
