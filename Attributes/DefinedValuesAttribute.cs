using System;
using System.Linq;
using System.Reflection;
using MyBox.EditorTools;
using UnityEngine;

//TODO: Support for method returning (Str, Obj)[] collection for custom display values
//TODO: Test the assignment of the custom data classes (serialized structs with specific values?)
//TODO: Use the Methods returning enumerable collections
//TODO: Test the methods returning non-serializable objects
//TODO: What if the Value collection is changed? Add warning?
//TODO: Utilize WarningsPool to notify about any issues (or display warning instead of the field?)
//TODO: Refactoring

namespace MyBox
{
	/// <summary>
	/// Create Popup with predefined values for string, int or float property
	/// </summary>
	public class DefinedValuesAttribute : PropertyAttribute
	{
		public readonly object[] ValuesArray;
		public readonly string UseMethod;

		public DefinedValuesAttribute(params object[] definedValues)
		{
			ValuesArray = definedValues;
		}
		public DefinedValuesAttribute(string method)
		{
			UseMethod = method;
		}
	}
}

#if UNITY_EDITOR
namespace MyBox.Internal
{
	using UnityEditor;

	[CustomPropertyDrawer(typeof(DefinedValuesAttribute))]
	public class DefinedValuesAttributeDrawer : PropertyDrawer
	{
		private object[] _objects;
		private string[] _values;
		private Type _valueType;
		private bool _initialized;

		private void Initialize(UnityEngine.Object target, DefinedValuesAttribute defaultValuesAttribute)
		{
			if (_initialized) return;
			_initialized = true;
			
			var values = defaultValuesAttribute.ValuesArray;
			var methodName = defaultValuesAttribute.UseMethod;

			if (methodName.NotNullOrEmpty())
			{
				var valuesFromMethod = GetValuesFromMethod();
				if (valuesFromMethod.NotNullOrEmpty()) values = valuesFromMethod;
			}
			
			if (values.IsNullOrEmpty()) return;

			_objects = values;
			_valueType = values.First(v => v != null).GetType();
			_values = values.Select(v => v?.ToString() ?? "NULL").ToArray();

			
			object[] GetValuesFromMethod()
			{
				var type = target.GetType();
				var bindings = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;
				var method = type.GetMethods(bindings).SingleOrDefault(m => m.Name == methodName);
				if (method == null) return null;

				try
				{
					var result = method.Invoke(target, null);
					return result as object[];
				}
				catch
				{
					return null;
				}
			}
		}
		
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			Initialize(property.serializedObject.targetObject, (DefinedValuesAttribute)attribute);
			
			if (_values.IsNullOrEmpty() || _valueType != fieldInfo.FieldType)
			{
				EditorGUI.PropertyField(position, property, label);
				return;
			}
			
			bool isString = _valueType == typeof(string);
			bool isInt = _valueType == typeof(int);
			bool isFloat = _valueType == typeof(float);

			EditorGUI.BeginChangeCheck();
			var newIndex = EditorGUI.Popup(position, label.text, GetSelectedIndex(), _values);
			if (EditorGUI.EndChangeCheck()) ApplyNewValue(newIndex);


			int GetSelectedIndex()
			{
				object value = null;
				for (var i = 0; i < _values.Length; i++)
				{
					if (isString && property.stringValue == _values[i]) return i;
					if (isInt && property.intValue == Convert.ToInt32(_values[i])) return i;
					if (isFloat && Mathf.Approximately(property.floatValue, Convert.ToSingle(_values[i]))) return i;

					if (value == null) value = property.GetValue();
					if (value == _objects[i]) return i;
				}

				return 0;
			}

			void ApplyNewValue(int newValue)
			{
				if (isString) property.stringValue = _values[newValue];
				else if (isInt) property.intValue = Convert.ToInt32(_values[newValue]);
				else if (isFloat) property.floatValue = Convert.ToSingle(_values[newValue]);
				else
				{
					property.SetValue(_objects[newValue]);
					EditorUtility.SetDirty(property.serializedObject.targetObject);
				}
				
				property.serializedObject.ApplyModifiedProperties();
			}
		}
	}
}
#endif