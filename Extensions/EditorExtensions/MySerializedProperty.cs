#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using JetBrains.Annotations;
using UnityEditor;
using Object = UnityEngine.Object;

namespace MyBox.EditorTools
{
	[PublicAPI]
	public static class MySerializedProperty
	{
		#region Collections Handling

		/// <summary>
		/// Is this property an array element, not just a member of hierarchy with a collection
		/// </summary>
		public static bool IsArrayElement(this SerializedProperty property)
		{
			var pathParts = property.propertyPath.Split('.');
			return pathParts.Length > 1 && pathParts[pathParts.Length - 2] == "Array";
		}

		/// <summary>
		/// Get array of property childs, if parent property is array
		/// </summary>
		public static SerializedProperty[] AsArray(this SerializedProperty property)
		{
			List<SerializedProperty> items = new List<SerializedProperty>();
			for (int i = 0; i < property.arraySize; i++)
				items.Add(property.GetArrayElementAtIndex(i));
			return items.ToArray();
		}

		/// <summary>
		/// Get array of property childs casted to specific type
		/// </summary>
		public static T[] AsArray<T>(this SerializedProperty property)
		{
			var propertiesArray = property.AsArray();
			return propertiesArray.Select(s => s.objectReferenceValue).OfType<T>().ToArray();
		}

		/// <summary>
		/// Get array of property childs, if parent property is array
		/// </summary>
		public static IEnumerable<SerializedProperty> AsIEnumerable(this SerializedProperty property)
		{
			for (int i = 0; i < property.arraySize; i++)
				yield return property.GetArrayElementAtIndex(i);
		}

		/// <summary>
		/// Replace array contents of SerializedProperty with another array
		/// </summary>
		public static void ReplaceArray(this SerializedProperty property, Object[] newElements)
		{
			property.arraySize = 0;
			property.serializedObject.ApplyModifiedProperties();
			property.arraySize = newElements.Length;
			for (var i = 0; i < newElements.Length; i++)
			{
				property.GetArrayElementAtIndex(i).objectReferenceValue = newElements[i];
			}

			property.serializedObject.ApplyModifiedProperties();
		}

		/// <summary>
		/// If property is array, insert new element at the end and get it as a property
		/// </summary>
		public static SerializedProperty NewElement(this SerializedProperty property)
		{
			int newElementIndex = property.arraySize;
			property.InsertArrayElementAtIndex(newElementIndex);
			return property.GetArrayElementAtIndex(newElementIndex);
		}

		#endregion

		/// <summary>
		/// Property is float, int, vector or int vector
		/// </summary>
		public static bool IsNumerical(this SerializedProperty property)
		{
			var propertyType = property.propertyType;
			switch (propertyType)
			{
				case SerializedPropertyType.Float:
				case SerializedPropertyType.Integer:
				case SerializedPropertyType.Vector2:
				case SerializedPropertyType.Vector3:
				case SerializedPropertyType.Vector4:
				case SerializedPropertyType.Vector2Int:
				case SerializedPropertyType.Vector3Int:
					return true;

				default: return false;
			}
		}

		/// <summary>
		/// Get string representation of serialized property
		/// </summary>
		public static string AsStringValue(this SerializedProperty property)
		{
			switch (property.propertyType)
			{
				case SerializedPropertyType.String:
					return property.stringValue;

				case SerializedPropertyType.Character:
				case SerializedPropertyType.Integer:
					if (property.type == "char") return Convert.ToChar(property.intValue).ToString();
					return property.intValue.ToString();

				case SerializedPropertyType.ObjectReference:
					return property.objectReferenceValue != null ? property.objectReferenceValue.ToString() : "null";

				case SerializedPropertyType.Boolean:
					return property.boolValue.ToString();

				case SerializedPropertyType.Enum:
                    return property.GetValue().ToString();

                default:
					return string.Empty;
			}
		}

		/// <summary>
		/// Combination of Owner Type and Property Path hashes
		/// </summary>
		public static int GetUniquePropertyId(this SerializedProperty property) 
			=> property.serializedObject.targetObject.GetType().GetHashCode() 
			   + property.propertyPath.GetHashCode();
		
		/// <summary>
		/// Property path for collection without ".Array.data[x]" in it
		/// </summary>
		public static string GetFixedPropertyPath(this SerializedProperty property) => property.propertyPath.Replace(".Array.data[", "[");


		/// <summary>
		/// Get FieldInfo out of SerializedProperty
		/// </summary>
		public static FieldInfo GetFieldInfo(this SerializedProperty property)
		{
			var targetObject = property.serializedObject.targetObject;
			var targetType = targetObject.GetType();
			
			FieldInfo fieldInfo = null;
			while (targetType != null)
			{
				fieldInfo = targetType.GetField(property.propertyPath);
				if (fieldInfo != null) break;
				targetType = targetType.BaseType; 
			}
			
			return fieldInfo;
		}

		/// <summary>
		/// Get raw object value out of the SerializedProperty
		/// </summary>
		public static object GetValue(this SerializedProperty property)
		{
			if (property == null) return null;

			object obj = property.serializedObject.targetObject;
			var elements = property.GetFixedPropertyPath().Split('.');
			foreach (var element in elements)
			{
				if (element.Contains("["))
				{
					var elementName = element.Substring(0, element.IndexOf("[", StringComparison.Ordinal));
					var index = Convert.ToInt32(element.Substring(element.IndexOf("[", StringComparison.Ordinal)).Replace("[", "").Replace("]", ""));
					obj = GetValueByArrayFieldName(obj, elementName, index);
				}
				else obj = GetValueByFieldName(obj, element);
			}
			return obj;


			object GetValueByArrayFieldName(object source, string name, int index)
			{
				if (!(GetValueByFieldName(source, name) is IEnumerable enumerable)) return null;
				var enumerator = enumerable.GetEnumerator();

				for (var i = 0; i <= index; i++) if (!enumerator.MoveNext()) return null;
				return enumerator.Current;
			}

			// Search "source" object for a field with "name" and get it's value
			object GetValueByFieldName(object source, string name)
			{
				if (source == null)  return null;
				var type = source.GetType();

				while (type != null)
				{
					var fieldInfo = type.GetField(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
					if (fieldInfo != null) return fieldInfo.GetValue(source);

					var propertyInfo = type.GetProperty(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
					if (propertyInfo != null) return propertyInfo.GetValue(source, null);

					type = type.BaseType;
				}
				return null;
			}
		}


		/// <summary>
		/// Set raw object value to the SerializedProperty
		/// </summary>
		public static void SetValue(this SerializedProperty property,object value)
		{
			GetFieldInfo(property).SetValue(property.serializedObject.targetObject, value);
		}

		/// <summary>
		/// Is specific attribute defined on SerializedProperty
		/// </summary>
		/// <param name="property"></param>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public static bool IsAttributeDefined<T>(this SerializedProperty property) where T : Attribute
		{
			var fieldInfo = property.GetFieldInfo();
			if (fieldInfo == null) return false;
			return Attribute.IsDefined(fieldInfo, typeof(T));
		}

		/// <summary>
		/// Repaint inspector window where this property is displayed
		/// </summary>
		public static void Repaint(this SerializedProperty property)
		{
			foreach (var item in ActiveEditorTracker.sharedTracker.activeEditors)
			{
				if (item.serializedObject == property.serializedObject)
				{
					item.Repaint();
					return;
				}
			}
		}
		
		public static void ApplyModifiedProperties(this SerializedProperty property) => property.serializedObject.ApplyModifiedProperties();

		#region SerializedProperty Get Parent

		// Found here http://answers.unity.com/answers/425602/view.html
		// Update here https://gist.github.com/AdrienVR/1548a145c039d2fddf030ebc22f915de to support inherited private members.
		/// <summary>
		/// Get parent object of SerializedProperty
		/// </summary>
		public static object GetParent(this SerializedProperty prop)
		{
			var path = prop.propertyPath.Replace(".Array.data[", "[");
			object obj = prop.serializedObject.targetObject;
			var elements = path.Split('.');
			foreach (var element in elements.Take(elements.Length - 1))
			{
				if (element.Contains("["))
				{
					var elementName = element.Substring(0, element.IndexOf("[", StringComparison.Ordinal));
					var index = Convert.ToInt32(element.Substring(element.IndexOf("[", StringComparison.Ordinal)).Replace("[", "").Replace("]", ""));
					obj = GetValueAt(obj, elementName, index);
				}
				else
				{
					obj = GetFieldValue(obj, element);
				}
			}

			return obj;
			
			
			object GetValueAt(object source, string name, int index)
			{
				var enumerable = GetFieldValue(source, name) as IEnumerable;
				if (enumerable == null) return null;

				var enm = enumerable.GetEnumerator();
				
				while (index-- >= 0)
					enm.MoveNext();
				return enm.Current;
			}
			
			object GetFieldValue(object source, string name)
			{
				if (source == null)
					return null;

				foreach (var type in GetHierarchyTypes(source.GetType()))
				{
					var f = type.GetField(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
					if (f != null)
						return f.GetValue(source);

					var p = type.GetProperty(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
					if (p != null)
						return p.GetValue(source, null);
				}

				return null;
			
			
				IEnumerable<Type> GetHierarchyTypes(Type sourceType)
				{
					yield return sourceType;
					while (sourceType.BaseType != null)
					{
						yield return sourceType.BaseType;
						sourceType = sourceType.BaseType;
					}
				}
			}
		}
		#endregion
	}
}
#endif
