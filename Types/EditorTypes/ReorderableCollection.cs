#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace MyBox.EditorTools
{
	public class ReorderableCollection
	{
		public bool IsExpanded
		{
			get { return _property.isExpanded; }
			set { _property.isExpanded = value; }
		}

		public void Draw()
		{
			if (!_property.isExpanded) DrawHeader();
			else _list.DoLayoutList();
		}

		
		public Action<SerializedProperty, Rect, int> CustomDrawer;
		/// <summary>
		/// Return Height, Receive element index.
		/// Use EditorApplication.delayCall to perform custom logic
		/// </summary>
		public Func<int, int> CustomDrawerHeight;
		/// <summary>
		/// Return false to perform default logic, Receive element index.
		/// Use EditorApplication.delayCall to perform custom logic.
		/// </summary>
		public Func<int, bool> CustomAdd;
		/// <summary>
		/// Return false to perform default logic, Receive element index.
		/// Use EditorApplication.delayCall to perform custom logic.
		/// </summary>
		public Func<int, bool> CustomRemove;

		
		private ReorderableList _list;
		private SerializedProperty _property;

		public ReorderableCollection(SerializedProperty property, bool withAddButton = true,
			bool withRemoveButton = true)
		{
			_property = property;
			_property.isExpanded = true;
			
			CreateList(property, withAddButton, withRemoveButton);
		}

		~ReorderableCollection()
		{
			_property = null;
			_list = null;
		}

		private void DrawHeader()
		{
			EditorGUILayout.BeginHorizontal();
			_property.isExpanded = EditorGUILayout.ToggleLeft(string.Format("{0}[]", _property.displayName),
				_property.isExpanded,
				EditorStyles.boldLabel);
			EditorGUILayout.LabelField(string.Format("size: {0}", _property.arraySize));
			EditorGUILayout.EndHorizontal();
		}

		private void CreateList(SerializedProperty property, bool withAddButton, bool withRemoveButton)
		{
			_list = new ReorderableList(property.serializedObject, property, true, true, withAddButton,
				withRemoveButton);
			_list.onChangedCallback += list => Apply();
			_list.onAddCallback += AddElement;
			_list.onRemoveCallback += RemoveElement;
			_list.drawHeaderCallback += DrawElementHeader;
			_list.onCanRemoveCallback += (list) => _list.count > 0;
			_list.drawElementCallback += DrawElement;
			_list.elementHeightCallback += GetElementHeight;
		}

		private void DrawElementHeader(Rect rect)
		{
			_property.isExpanded =
				EditorGUI.ToggleLeft(rect, _property.displayName, _property.isExpanded, EditorStyles.boldLabel);
		}

		private void AddElement(ReorderableList list)
		{
			if (CustomAdd == null || !CustomAdd(_property.arraySize))
				ReorderableList.defaultBehaviours.DoAddButton(list);
		}
		
		private void RemoveElement(ReorderableList list)
		{
			if (CustomRemove == null || !CustomRemove(list.index))
				ReorderableList.defaultBehaviours.DoRemoveButton(list);
		}

		private void DrawElement(Rect rect, int index, bool active, bool focused)
		{
			EditorGUI.BeginChangeCheck();
			
			var property = _property.GetArrayElementAtIndex(index);
			rect.height = GetElementHeight(index);
			rect.y += 1;

			if (CustomDrawer != null) CustomDrawer(property, rect, index);
			else
			{
				var element = _property.GetArrayElementAtIndex(index);
				var newRect = rect;
				newRect.x += 20;
				
				if (element.propertyType == SerializedPropertyType.Generic) EditorGUI.LabelField(newRect, element.displayName);
				EditorGUI.PropertyField(rect, property, GUIContent.none, true);
			}

			_list.elementHeight = rect.height + 4.0f;
			if (EditorGUI.EndChangeCheck()) Apply();
		}

		private float GetElementHeight(int index)
		{
			if (CustomDrawerHeight != null) return CustomDrawerHeight(index);
			
			var element = _property.GetArrayElementAtIndex(index);
			var height = EditorGUI.GetPropertyHeight(element, GUIContent.none, true);
			return Mathf.Max(EditorGUIUtility.singleLineHeight, height + 4.0f);
		}

		private void Apply()
		{
			_property.serializedObject.ApplyModifiedProperties();
		}
	}
}
#endif