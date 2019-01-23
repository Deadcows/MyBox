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
        // Used to check ConditionalFieldAttribute
        var conditionallyVisibleType = 
            Type.GetType("ConditionalFieldAttribute, Assembly-CSharp-firstpass, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null");
        MethodInfo conditionallyVisibleMethod = null;
        if (conditionallyVisibleType != null) conditionallyVisibleMethod = conditionallyVisibleType.GetMethod("CheckBehaviourPropertyVisible");
        //


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
                if (conditionallyVisibleType != null && field.IsDefined(conditionallyVisibleType))
                {
                    var conditionalFieldAttribute = field.GetCustomAttribute(conditionallyVisibleType);

                    var visible = conditionallyVisibleMethod.Invoke(conditionalFieldAttribute, new object[] {behaviour, field.Name}) as bool?;
                    if (!visible.Value) continue;
                }

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
}