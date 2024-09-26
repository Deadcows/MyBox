// ---------------------------------------------------------------------------- 
// Author: ByronMayne
// https://gist.github.com/ByronMayne/70a46e73f3af7fb9fec7437174dd4858
// https://gist.github.com/ByronMayne
// https://github.com/ByronMayne
// ----------------------------------------------------------------------------

// TODO: For some reason Odin is not drawing contents of the UnityEvent from time to time. Discard the drawer for now
#if UNITY_EDITOR && !MYBOX_DISABLE_UNITYEVENT_OVERRIDE && !ODIN_INSPECTOR
namespace MyBox.Internal
{
	using System.Collections.Generic;
	using System.Reflection;
	using UnityEditor;
	using UnityEditorInternal;
	using UnityEngine;
	using UnityEngine.Events;

	[CustomPropertyDrawer(typeof(UnityEvent))]
	public class CollapsableEventDrawer : UnityEventDrawer
	{
		private class Styles
		{
			public readonly GUIStyle preButton = "RL FooterButton";
			public GUIContent iconToolbarPlus = EditorGUIUtility.IconContent("Toolbar Plus", "|Add to list");
		}

		private const string CALLS_PROPERTY_PATH = "m_PersistentCalls.m_Calls";
		private const string GET_STATE_METHOD_NAME = "GetState";
		private const string REORDERABLE_LIST_FIELD_NAME = "m_ReorderableList";
		private const float BUTTON_WIDTH = 30;
		private const float BUTTON_SPACING = 30;
		private static readonly BindingFlags NON_PULIC_INSTANCE_FLAGS = BindingFlags.NonPublic | BindingFlags.Instance;


		// Points the field inside Sate which has the reorderable list. 
		private static FieldInfo _stateReorderableListFieldInfo;

		// Points to the GetState method defined in UnityEventDrawer. 
		private static MethodInfo _getStateMethod;

		// Cached array for using reflection 
		private static object[] _getStateArgs = new object[1];

		// A class that contains all the custom styling we need
		private static Styles _styles;


		// True if we have persistent calls and false if we don't.
		private bool _hasPersistentCalls;

		// Holds all our reorderable lists that belong to our state 
		private IDictionary<State, ReorderableList> _lists;


		/// <summary>
		/// Used to initialize our values.
		/// </summary>
		public CollapsableEventDrawer()
		{
			_lists = new Dictionary<State, ReorderableList>();
		}


		/// <summary>
		/// A wrapper around the GetState function that is private in <see cref="UnityEventDrawer"/>
		/// </summary>
		private State GetState(SerializedProperty property)
		{
			if (_getStateMethod == null)
			{
				_getStateMethod = typeof(UnityEventDrawer).GetMethod(GET_STATE_METHOD_NAME, NON_PULIC_INSTANCE_FLAGS);
			}

			_getStateArgs[0] = property;
			return (State) _getStateMethod.Invoke(this, _getStateArgs);
		}

		/// <summary>
		/// Tries to get the cached ReorderableList from the state. 
		/// </summary>
		private ReorderableList GetList(SerializedProperty property)
		{
			State state = GetState(property);
			if (_lists.ContainsKey(state))
			{
				return _lists[state];
			}

			ReorderableList list = GetReorderableListFromState(state);
			list.draggable = true;
			_lists[state] = list;
			return list;
		}


		/// <summary>
		/// Returns back the number of calls that are currently in the Unity Event. 
		/// </summary>
		private bool HasPersistentCalls(SerializedProperty property)
		{
			return property.FindPropertyRelative(CALLS_PROPERTY_PATH).arraySize > 0;
		}

		/// <summary>
		/// Returns to Unity to tell it how much space we should use to draw. If we
		/// have one element we only reserve 32 units. 16 for the element itself and
		/// 16 for spacing from the next element. If we any calls we let Unity
		/// draw the default. 
		/// </summary>
		public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
			ReorderableList list = GetList(property);
			_hasPersistentCalls = HasPersistentCalls(property);
			if (!_hasPersistentCalls)
			{
				property.isExpanded = false;
				list.elementHeight = 0;
				list.displayAdd = false;
				list.displayRemove = false;
				return EditorGUIUtility.singleLineHeight * 2f;
			}

			if (!property.isExpanded)
			{
				list.elementHeight = 0;
				list.displayAdd = false;
				list.displayRemove = false;
				return EditorGUIUtility.singleLineHeight;
			}

			list.elementHeight = 43;
			list.displayAdd = true;
			list.displayRemove = true;
			return base.GetPropertyHeight(property, label);
		}

		/// <summary>
		/// Used to draw our element override a bit of the Unity logic in the case
		/// of having no elements. 
		/// </summary>
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			if (_styles == null)
			{
				_styles = new Styles();
			}

			SerializedProperty elements = property.FindPropertyRelative(CALLS_PROPERTY_PATH);
			float heightTemp = position.height;
			position.height = EditorGUIUtility.singleLineHeight;
			if (!_hasPersistentCalls)
			{
				base.OnGUI(position, property, label);
				position.x += position.width - BUTTON_SPACING;
				position.width = BUTTON_WIDTH;
				if (GUI.Button(position, _styles.iconToolbarPlus, _styles.preButton))
				{
					State state = GetState(property);
					ReorderableList list = GetReorderableListFromState(state);
					list.onAddCallback(list);
					state.lastSelectedIndex = 0;
					property.isExpanded = true;
				}

				return;
			}

			property.isExpanded = EditorGUI.BeginFoldoutHeaderGroup(position, property.isExpanded,
						$"[{elements.arraySize}] {property.displayName} ()");
			position.height = heightTemp;
			if (property.isExpanded)
			{
				base.OnGUI(position, property, label);
			}

			EditorGUI.EndFoldoutHeaderGroup();
		}
		/// <summary>
		/// Gets the internal instance of the <see cref="ReorderableList"/> that exists
		/// in the state. 
		/// </summary>
		private static ReorderableList GetReorderableListFromState(State state)
		{
			if (_stateReorderableListFieldInfo == null)
			{
				_stateReorderableListFieldInfo = typeof(State).GetField(REORDERABLE_LIST_FIELD_NAME, NON_PULIC_INSTANCE_FLAGS);
			}

			return (ReorderableList) _stateReorderableListFieldInfo.GetValue(state);
		}
	}
}
#endif
