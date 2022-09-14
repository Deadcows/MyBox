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
			get => _property.isExpanded;
			set => _property.isExpanded = value;
		}
		
		public float Height => _property.isExpanded ? _list.GetHeight() : EditorGUIUtility.singleLineHeight + 12;
		
		public SerializedProperty Property => _property;
		public ReorderableList ReorderableList => _list;

		public void Draw()
		{
			if (_property.isExpanded)
			{
				var headerRect = GUILayoutUtility.GetRect(0, 0, GUILayout.ExpandWidth(true));
				headerRect.height = 20;
				_list.DoLayoutList();
				DrawHeader(headerRect);
			}
			else DrawHeader(GUILayoutUtility.GetRect(0, 20, GUILayout.ExpandWidth(true)));
		}

		public void Draw(Rect rect)
		{
			if (_property.isExpanded) _list.DoList(rect);
			DrawHeader(rect);
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

		public Func<int, string> CustomElementName;


		private ReorderableList _list;
		private SerializedProperty _property;
		private readonly string _customHeader; 

		public ReorderableCollection(SerializedProperty property, bool withAddButton = true,
			bool withRemoveButton = true, string customHeader = null)
		{
			_property = property;
			_property.isExpanded = true;
			_customHeader = customHeader;
			
			CreateList(property, withAddButton, withRemoveButton, customHeader == null);
		}

		~ReorderableCollection()
		{
			_property = null;
			_list = null;
		}

		private void DrawHeader(Rect rect)
		{
			var headerRect = new Rect(rect);
			headerRect.height = EditorGUIUtility.singleLineHeight;
			
			ReorderableList.defaultBehaviours.DrawHeaderBackground(headerRect);
			
			headerRect.width -= 50;
			headerRect.x += 6;
			headerRect.y += 2;
			_property.isExpanded = EditorGUI.ToggleLeft(headerRect,
				(_customHeader != null ? _customHeader : _property.displayName) + "[" + _property.arraySize + "]",
				_property.isExpanded,
				EditorStyles.boldLabel);
		}

		private void CreateList(SerializedProperty property, bool withAddButton, bool withRemoveButton, bool displayHeader)
		{
			_list = new ReorderableList(property.serializedObject, property, true, displayHeader, withAddButton,
				withRemoveButton);
			_list.onChangedCallback += list => Apply();
			_list.onAddCallback += AddElement;
			_list.onRemoveCallback += RemoveElement;
			_list.onCanRemoveCallback += (list) => _list.count > 0;
			_list.drawElementCallback += DrawElement;
			_list.elementHeightCallback += GetElementHeight;
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

				if (element.propertyType == SerializedPropertyType.Generic)
				{
					rect.x += 12;
					rect.width -= 14;
					var genericsLabel = rect;
					genericsLabel.height = EditorGUIUtility.singleLineHeight;

					string displayName = CustomElementName != null ? 
						CustomElementName.Invoke(index) : 
						element.displayName;
					EditorGUI.LabelField(genericsLabel, displayName);
				}
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