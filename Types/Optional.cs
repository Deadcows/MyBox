using System;
using System.Collections.Generic;
using MyBox.Internal;
using UnityEngine;


namespace MyBox
{
	[Serializable]
	public class OptionalFloat : Optional<float>
	{
		public static OptionalFloat WithValue(float value)
		{
			return new OptionalFloat {IsSet = true, Value = value};
		}
		
		public static implicit operator OptionalFloat(float val) => new OptionalFloat() {Value = val};
	}

	[Serializable]
	public class OptionalInt : Optional<int>
	{
		public static OptionalInt WithValue(int value)
		{
			return new OptionalInt {IsSet = true, Value = value};
		}
		public static implicit operator OptionalInt(int val) => new OptionalInt() {Value = val};
	}

	[Serializable]
	public class OptionalString : Optional<string>
	{
		public static OptionalString WithValue(string value)
		{
			return new OptionalString {IsSet = true, Value = value};
		}
		public static implicit operator OptionalString(string val) => new OptionalString() {Value = val};
	}

	[Serializable]
	public class OptionalKeyCode : Optional<KeyCode>
	{
		public static OptionalKeyCode WithValue(KeyCode value)
		{
			return new OptionalKeyCode {IsSet = true, Value = value};
		}
		public static implicit operator OptionalKeyCode(KeyCode val) => new OptionalKeyCode() {Value = val};
	}

	[Serializable]
	public class OptionalGameObject : Optional<GameObject>
	{
		public static implicit operator OptionalGameObject(GameObject val) => new OptionalGameObject() {Value = val};
	}

	[Serializable]
	public class OptionalComponent : Optional<Component>
	{
		public static implicit operator OptionalComponent(Component val) => new OptionalComponent() {Value = val};
	}
}

namespace MyBox.Internal
{
	[Serializable]
	public class Optional<T> : OptionalParent, IEquatable<T>, IEquatable<Optional<T>>
	{
		public bool IsSet;
		public T Value;
		 
		public static explicit operator T(Optional<T> val) => val.Value;

		public static implicit operator Optional<T>(T val) => new Optional<T>() {Value = val};

		public override string ToString() => Value.ToString();
		public bool Equals(T other) => Value.Equals(other);

		public bool Equals(Optional<T> other) =>
			IsSet == other.IsSet && Value.Equals(other.Value);

		public override bool Equals(object obj)
		{
			if (obj is Optional<T> ot) return Equals(ot);
			if (obj is T t) return Equals(t);
			return false;
		}

		public override int GetHashCode()
		{
			unchecked
			{
				return (IsSet.GetHashCode() * 397) ^ EqualityComparer<T>.Default.GetHashCode(Value);
			}
		}
	}

	[Serializable]
	public abstract class OptionalParent
	{
	}
}

#if UNITY_EDITOR
namespace MyBox.Internal
{
	using UnityEditor;
	
	[CustomPropertyDrawer(typeof(OptionalParent), true)]
	public class OptionalTypePropertyDrawer : PropertyDrawer
	{
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			EditorGUI.BeginProperty(position, label, property);
			position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);
			EditorGUI.indentLevel = 0; // PropertyDrawer Indent fix for nested inspectors

			var value = property.FindPropertyRelative("Value");
			var isSet = property.FindPropertyRelative("IsSet");

			var checkWidth = 14;
			var spaceWidth = 4;
			var valWidth = position.width - checkWidth - spaceWidth;

			position.width = checkWidth;
			isSet.boolValue = EditorGUI.Toggle(position, GUIContent.none, isSet.boolValue);

			position.x += checkWidth + spaceWidth;
			position.width = valWidth;
			if (isSet.boolValue) EditorGUI.PropertyField(position, value, GUIContent.none);
			EditorGUI.EndProperty();
		}
	}
}
#endif