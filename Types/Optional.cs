using System;
using MyBox.Internal;
using UnityEngine;

#region Optional premade types

namespace MyBox
{
	[Serializable]
	public class OptionalFloat : Optional<float>
	{
		public OptionalFloat(float value, bool enabledByDefault = false)
		{
			IsSet = enabledByDefault;
			Value = value;
		}

		public static OptionalFloat WithValue(float value) => new(value, true);
	}

	[Serializable]
	public class OptionalInt : Optional<int>
	{
		public OptionalInt(int value, bool enabledByDefault = false)
		{
			IsSet = enabledByDefault;
			Value = value;
		}

		public static OptionalInt WithValue(int value) => new(value, true);
	}

	[Serializable]
	public class OptionalBool : Optional<bool>
	{
		public OptionalBool(bool value, bool enabledByDefault = false)
		{
			IsSet = enabledByDefault;
			Value = value;
		}

		public static OptionalBool WithValue(bool value) => new(value, true);
	}

	[Serializable]
	public class OptionalVector2 : Optional<Vector2>
	{
		public OptionalVector2(Vector2 value, bool enabledByDefault = false)
		{
			IsSet = enabledByDefault;
			Value = value;
		}

		public static OptionalVector2 WithValue(Vector2 value) => new(value, true);
	}

	[Serializable]
	public class OptionalVector3 : Optional<Vector3>
	{
		public OptionalVector3(Vector3 value, bool enabledByDefault = false)
		{
			IsSet = enabledByDefault;
			Value = value;
		}

		public static OptionalVector3 WithValue(Vector3 value) => new(value, true);
	}

	[Serializable]
	public class OptionalVector2Int : Optional<Vector2Int>
	{
		public OptionalVector2Int(Vector2Int value, bool enabledByDefault = false)
		{
			IsSet = enabledByDefault;
			Value = value;
		}

		public static OptionalVector2Int WithValue(Vector2Int value) => new(value, true);
	}

	[Serializable]
	public class OptionalVector3Int : Optional<Vector3Int>
	{
		public OptionalVector3Int(Vector3Int value, bool enabledByDefault = false)
		{
			IsSet = enabledByDefault;
			Value = value;
		}

		public static OptionalVector3Int WithValue(Vector3Int value) => new(value, true);
	}

	[Serializable]
	public class OptionalString : Optional<string>
	{
		public OptionalString(string value, bool enabledByDefault = false)
		{
			IsSet = enabledByDefault;
			Value = value;
		}

		public static OptionalString WithValue(string value) => new(value, true);
	}

	[Serializable]
	public class OptionalAnimationCurve : Optional<AnimationCurve>
	{
		public OptionalAnimationCurve(AnimationCurve value, bool enabledByDefault = false)
		{
			IsSet = enabledByDefault;
			Value = value;
		}
	}

	[Serializable]
	public class OptionalKeyCode : Optional<KeyCode>
	{
		public OptionalKeyCode(KeyCode value, bool enabledByDefault = false)
		{
			IsSet = enabledByDefault;
			Value = value;
		}

		public static OptionalKeyCode WithValue(KeyCode value) => new(value, true);
	}
	
	[Serializable]
	public class OptionalColor : Optional<Color>
	{
		public OptionalColor(Color value, bool enabledByDefault = false)
		{
			IsSet = enabledByDefault;
			Value = value;
		}
		public OptionalColor(float r, float g, float b,float a = 1, bool enabledByDefault = false)
		{
			IsSet = enabledByDefault;
			Value = new Color(r, g, b, a);
		}

		public static OptionalColor WithValue(Color value) => new(value, true);
	}

	[Serializable]
	public class OptionalGameObject : Optional<GameObject>
	{
	}

	[Serializable]
	public class OptionalComponent : Optional<Component>
	{
	}
}

#endregion

namespace MyBox.Internal
{
	[Serializable]
	public class Optional<T> : OptionalParent
	{
		public bool IsSet;
		public T Value;
	}

	[Serializable]
	public class OptionalParent
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
			var indentLevelBackup = EditorGUI.indentLevel; // Backup indent before fix
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
			EditorGUI.indentLevel = indentLevelBackup; // restore indent backup
		}
	}
}
#endif