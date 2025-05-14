using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace MyBox
{
	/// <summary>
	/// Apply to the MonoBehaviour field to assert that this field is assigned via inspector (not null, false, empty of zero) on playmode
	/// </summary>
	[AttributeUsage(AttributeTargets.Field)]
	public class MustBeAssignedAttribute : PropertyAttribute
	{
		public bool IgnorePrefabMode;

		public MustBeAssignedAttribute(bool ignorePrefabMode = false)
		{
			IgnorePrefabMode = ignorePrefabMode;
		}
	}
}

#if UNITY_EDITOR
namespace MyBox.Internal
{
	using System.Reflection;
	using UnityEditor;
#if UNITY_2021_2_OR_NEWER
	using UnityEditor.SceneManagement;
#else
	using UnityEditor.Experimental.SceneManagement;
#endif
	using EditorTools;

	[InitializeOnLoad]
	public class MustBeAssignedAttributeChecker
	{
		/// <summary>
		/// A way to conditionally disable the MustBeAssigned check
		/// </summary>
		public static Func<FieldInfo, Object, bool> ExcludeFieldFilter;

		static MustBeAssignedAttributeChecker()
		{
			MyEditorEvents.OnSave += AssertComponentsInScene;
			PrefabStage.prefabSaved += AssertComponentsInPrefab;
		}

		private static void AssertComponentsInScene()
		{
			#if UNITY_2020_1_OR_NEWER
			var behaviours = Object.FindObjectsOfType<MonoBehaviour>(true);
			#else
			var behaviours = Object.FindObjectsOfType<MonoBehaviour>();
			#endif
			// ReSharper disable once CoVariantArrayConversion
			AssertComponents(behaviours, false);

			if (MyBoxSettings.EnableSOCheck)
			{
				var scriptableObjects = MyScriptableObject.LoadAssets<ScriptableObject>();
				// ReSharper disable once CoVariantArrayConversion
				AssertComponents(scriptableObjects, false);
			}
		}

		private static void AssertComponentsInPrefab(GameObject prefab)
		{
			MonoBehaviour[] components = prefab.GetComponentsInChildren<MonoBehaviour>();
			// ReSharper disable once CoVariantArrayConversion
			AssertComponents(components, true);
		}

		private static void AssertComponents(Object[] objects, bool prefabMode)
		{
			var mustBeAssignedType = typeof(MustBeAssignedAttribute);
			foreach (var obj in objects)
			{
				if (obj == null) continue;
				
				Type typeOfScript = obj.GetType();
				var typeFields = typeOfScript.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

				foreach (FieldInfo field in typeFields)
				{
					if (!field.IsDefined(mustBeAssignedType, false)) continue;
					
					// Used by external systems to exclude specific fields.
					// Specifically for ConditionalFieldAttribute
					if (FieldIsExcluded(field, obj)) continue;
					if (prefabMode && ((MustBeAssignedAttribute)field.GetCustomAttribute(mustBeAssignedType)).IgnorePrefabMode) continue;
					AssertField(obj, typeOfScript, field);
				}
			}
		}
		
		private static void AssertField(Object targetObject, Type targetType, FieldInfo field)
		{
			object fieldValue = field.GetValue(targetObject);
			
			bool valueTypeWithDefaultValue = field.FieldType.IsValueType && Activator.CreateInstance(field.FieldType).Equals(fieldValue);
			if (valueTypeWithDefaultValue)
			{
				Debug.LogError($"{targetType.Name} caused: {field.Name} is Value Type with default value", targetObject);
				return;
			}

					
			bool nullReferenceType = fieldValue == null || fieldValue.Equals(null);
			if (nullReferenceType)
			{
				Debug.LogError($"{targetType.Name} caused: {field.Name} is not assigned (null value)", targetObject);
				return;
			}


			bool emptyString = field.FieldType == typeof(string) && (string) fieldValue == string.Empty;
			if (emptyString)
			{
				Debug.LogError($"{targetType.Name} caused: {field.Name} is not assigned (empty string)", targetObject);
				return;
			}

					
			var arr = fieldValue as Array;
			bool emptyArray = arr != null && arr.Length == 0;
			if (emptyArray)
			{
				Debug.LogError($"{targetType.Name} caused: {field.Name} is not assigned (empty array)", targetObject);
			}
		}
		
		private static bool FieldIsExcluded(FieldInfo field, Object behaviour)
		{
			if (ExcludeFieldFilter == null) return false;

			foreach (var filterDelegate in ExcludeFieldFilter.GetInvocationList())
			{
				var filter = filterDelegate as Func<FieldInfo, Object, bool>;
				if (filter != null && filter(field, behaviour)) return true;
			}

			return false;
		}
	}
}
#endif