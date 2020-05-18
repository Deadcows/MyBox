using System;
using MyBox.Internal;

namespace MyBox
{
	[Serializable]
	public class CollectionWrapper<T> : CollectionWrapperBase
	{
		public T[] Value;
	}
}

namespace MyBox.Internal
{
	using UnityEditor;
	using UnityEngine;

	[Serializable]
	public class CollectionWrapperBase {}
	
	[CustomPropertyDrawer(typeof(CollectionWrapperBase), true)]
	public class CollectionWrapperDrawer : PropertyDrawer
	{
		public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
			var collection = property.FindPropertyRelative("Value");
			return EditorGUI.GetPropertyHeight(collection, true);
		}

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			var collection = property.FindPropertyRelative("Value");
			EditorGUI.PropertyField(position, collection, label, true);
		}
	}
}