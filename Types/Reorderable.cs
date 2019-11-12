using System;
using System.Collections.Generic;
using UnityEngine;


namespace MyBox
{
	#region Default Reordable Types

	[Serializable]
	public class ReorderableGameObject : Reorderable<GameObject>
	{
	}

	[Serializable]
	public class ReorderableGameObjectList : ReorderableList<GameObject>
	{
	}

	[Serializable]
	public class ReorderableTransform : Reorderable<Transform>
	{
	}

	[Serializable]
	public class ReorderableTransformList : ReorderableList<Transform>
	{
	}

	#endregion


	[Serializable]
	public class Reorderable<T> : Internal.ReorderableBase
	{
		public T[] Collection;

		public int Length
		{
			get { return Collection.Length; }
		}
		
		public T this[int i]
		{
			get { return Collection[i]; }
			set { Collection[i] = value; }
		}
	}

	[Serializable]
	public class ReorderableList<T> : Internal.ReorderableBase
	{
		public List<T> Collection;
	}
}

namespace MyBox.Internal
{
	[Serializable]
	public class ReorderableBase
	{
	}
}


#if UNITY_EDITOR
namespace MyBox.Internal
{
	using UnityEditor;
	using EditorTools;

	[CustomPropertyDrawer(typeof(ReorderableBase), true)]
	public class ReorderableTypePropertyDrawer : PropertyDrawer
	{
		private ReorderableCollection _reorderable;

		public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
			if (_reorderable == null)
				_reorderable = new ReorderableCollection(property.FindPropertyRelative("Collection"), true, true, property.displayName);
			
			return _reorderable != null ? _reorderable.Height : base.GetPropertyHeight(property, label);
		}

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			EditorGUI.BeginProperty(position, label, property);
			_reorderable.Draw(position);
			EditorGUI.EndProperty();
		}
	}
}
#endif