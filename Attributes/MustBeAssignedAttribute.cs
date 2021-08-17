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
	using UnityEditor.Experimental.SceneManagement;
	using EditorTools;

	[InitializeOnLoad]
	public class MustBeAssignedAttributeChecker
	{
		/// <summary>
		/// A way to conditionally disable MustBeAssigned check
		/// </summary>
		public static Func<FieldInfo, UnityEngine.Object, bool> ExcludeFieldFilter;

		static MustBeAssignedAttributeChecker()
		{
			MyEditorEvents.OnSave += AssertComponentsInScene;
			PrefabStage.prefabSaved += AssertComponentsInPrefab;
		}

		private static void AssertComponentsInScene()
		{
			//TODO: Optimize here, we need to check only instantiated behaviours AND ScriptableObject assets
			AssertComponent(MyEditor.GetAllUnityObjects(), true);
		}

		private static void AssertComponentsInPrefab(GameObject prefab)
		{
			MonoBehaviour[] components = prefab.GetComponentsInChildren<MonoBehaviour>();
			// ReSharper disable once CoVariantArrayConversion
			AssertComponent(components, false);
		}

		private static void AssertComponent(UnityEngine.Object[] objects, bool excludePrefabs)
		{
			foreach (var obj in objects)
			{
				if (obj == null) continue;
				if (!(obj.Is<MonoBehaviour>() || obj.Is<ScriptableObject>())) continue;
				
				Type typeOfScript = obj.GetType();
				FieldInfo[] mustBeAssignedFields = typeOfScript
					.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
					.Where(field => field.IsDefined(typeof(MustBeAssignedAttribute), false)).ToArray();
				if (mustBeAssignedFields.IsNullOrEmpty()) continue;

				if (excludePrefabs && PrefabUtility.IsPartOfPrefabAsset(obj)) continue;
				
				foreach (FieldInfo field in mustBeAssignedFields)
				{
					object propValue = field.GetValue(obj);

					// Used by external systems to exclude specific fields.
					// Specifically for ConditionalFieldAttribute
					if (FieldIsExcluded(field, obj)) continue;

					
					bool valueTypeWithDefaultValue = field.FieldType.IsValueType && Activator.CreateInstance(field.FieldType).Equals(propValue);
					if (valueTypeWithDefaultValue)
					{
						Debug.LogError($"{typeOfScript.Name} caused: {field.Name} is Value Type with default value",
							obj);
						continue;
					}

					
					bool nullReferenceType = propValue == null || propValue.Equals(null);
					if (nullReferenceType)
					{
						Debug.LogError($"{typeOfScript.Name} caused: {field.Name} is not assigned (null value)",
							obj);
						continue;
					}


					bool emptyString = field.FieldType == typeof(string) && (string) propValue == string.Empty;
					if (emptyString)
					{
						Debug.LogError($"{typeOfScript.Name} caused: {field.Name} is not assigned (empty string)",
							obj);
						continue;
					}

					
					var arr = propValue as Array;
					bool emptyArray = arr != null && arr.Length == 0;
					if (emptyArray)
					{
						Debug.LogError($"{typeOfScript.Name} caused: {field.Name} is not assigned (empty array)",
							obj);
					}
				}
			}
		}
		
		private static bool FieldIsExcluded(FieldInfo field, UnityEngine.Object behaviour)
		{
			if (ExcludeFieldFilter == null) return false;

			foreach (var filterDelegate in ExcludeFieldFilter.GetInvocationList())
			{
				var filter = filterDelegate as Func<FieldInfo, UnityEngine.Object, bool>;
				if (filter != null && filter(field, behaviour)) return true;
			}

			return false;
		}
	}
}
#endif