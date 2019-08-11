using System;
using System.Linq;
using UnityEngine;

#if UNITY_EDITOR
using MyBox.EditorTools;
using UnityEditor;
using UnityEditor.Animations;
#endif

namespace MyBox
{
	[Serializable]
	public class AnimationStateReference
	{
		public string StateName
		{
			get { return _stateName; }
		}

		public bool Assigned
		{
			get { return _assigned; }
		}

#pragma warning disable 0649
		[SerializeField] private string _stateName = String.Empty;
		[SerializeField] private bool _assigned;
#pragma warning restore 0649
	}

	public static class AnimationStateReferenceExtension
	{
		public static void Play(this Animator animator, AnimationStateReference state)
		{
			if (!state.Assigned) return;
			animator.Play(state.StateName);
		}
	}
}

#if UNITY_EDITOR
namespace MyBox.Internal
{
	[CustomPropertyDrawer(typeof(AnimationStateReference))]
	public class AnimationStateReferenceDrawer : PropertyDrawer
	{
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			Initialize(property);

			EditorGUI.BeginProperty(position, label, property);

			if (_animator == null)
			{
				EditorGUI.LabelField(position, label, _animatorNotFound);
				return;
			}

			var popupRect = position;
			popupRect.width -= 30;

			var refreshRect = position;
			refreshRect.width = 26;
			refreshRect.x += popupRect.width;


			var state = EditorGUI.Popup(popupRect, label, CurrentIndex(), _states.Select(s => new GUIContent(s)).ToArray());
			_stateName.stringValue = _states[state];
			_assigned.boolValue = state > 0;

			if (GUI.Button(refreshRect, "↺")) UpdateStates();

			EditorGUI.EndProperty();
		}

		private void Initialize(SerializedProperty property)
		{
			if (_animator == null)
			{
				var mb = property.GetParent() as MonoBehaviour;
				if (mb != null)
				{
					_animator = mb.GetComponent<Animator>();
					UpdateStates();
				}
			}

			if (_stateName == null) _stateName = property.FindPropertyRelative("_stateName");
			if (_assigned == null) _assigned = property.FindPropertyRelative("_assigned");

			if (_animatorNotFound == null) _animatorNotFound = new GUIContent("Animator not found");
		}

		private void UpdateStates()
		{
			_states = _defaultState;
			if (_animator == null) return;
			var controller = _animator.runtimeAnimatorController as AnimatorController;
			if (controller != null)
			{
				var states = controller.layers.SelectMany(l => l.stateMachine.states).Select(s => (s.state.name)).Distinct();
				_states = _states.Concat(states).ToArray();
			}
		}

		private int CurrentIndex()
		{
			var index = _states.IndexOfItem(_stateName.stringValue);
			if (index < 0) index = 0;
			return index;
		}

		private SerializedProperty _stateName;
		private SerializedProperty _assigned;
		private GUIContent _animatorNotFound;

		private Animator _animator;
		private readonly string[] _defaultState = {"Not Assigned"};

		private string[] _states = new string[1];
	}
}
#endif