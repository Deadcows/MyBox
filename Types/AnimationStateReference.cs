using System;
using System.Linq;
using UnityEngine;

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
		
		public Animator Animator
		{
			get { return _linkedAnimator; }
		}

#pragma warning disable 0649
		[SerializeField] private string _stateName = String.Empty;
		[SerializeField] private bool _assigned;
		[SerializeField] private Animator _linkedAnimator;
#pragma warning restore 0649
	}

	public static class AnimationStateReferenceExtension
	{
		public static void Play(this Animator animator, AnimationStateReference state)
		{
			if (!state.Assigned) return;
			animator.Play(state.StateName);
		}
		
		public static void Play(this AnimationStateReference  state)
		{
			if (!state.Assigned) return;
			state.Animator.Play(state.StateName);
		}
	}
}

#if UNITY_EDITOR
namespace MyBox.Internal
{
	using EditorTools;
	using UnityEditor;
	using UnityEditor.Animations;

	[CustomPropertyDrawer(typeof(AnimationStateReference))]
	public class AnimationStateReferenceDrawer : PropertyDrawer
	{
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			Initialize(property);

			EditorGUI.BeginProperty(position, label, property);

			var widthWithoutRefresh = position.width - 34;

			var stateRect = position;
			stateRect.width = widthWithoutRefresh / 4 * 3;

			var animatorRect = position;
			animatorRect.width = widthWithoutRefresh / 4;
			animatorRect.x += stateRect.width + 4;

			var refreshRect = animatorRect;
			refreshRect.width = 26;
			refreshRect.x += animatorRect.width + 4;


			var state = EditorGUI.Popup(stateRect, label, CurrentIndex(), _states.Select(s => new GUIContent(s)).ToArray());
			_stateName.stringValue = _states[state];
			_assigned.boolValue = state > 0;

			EditorGUI.BeginChangeCheck();
			EditorGUI.ObjectField(animatorRect, _animatorProperty, GUIContent.none);
			if (EditorGUI.EndChangeCheck()) UpdateStates();

			if (GUI.Button(refreshRect, "↺")) UpdateStates();

			EditorGUI.EndProperty();
		}

		private void Initialize(SerializedProperty property)
		{
			if (_animatorNotFound == null) _animatorNotFound = new GUIContent("Animator not found");
			if (_stateName == null) _stateName = property.FindPropertyRelative("_stateName");
			if (_assigned == null) _assigned = property.FindPropertyRelative("_assigned");

			if (_animatorProperty == null)
			{
				_animatorProperty = property.FindPropertyRelative("_linkedAnimator");
				if (_animatorProperty.objectReferenceValue == null)
				{
					var mb = property.GetParent() as MonoBehaviour;
					if (mb != null)
					{
						var animator = mb.GetComponentInChildren<Animator>(true);
						if (animator != null)
						{
							_animatorProperty.objectReferenceValue = animator;
							_animatorProperty.serializedObject.ApplyModifiedProperties();
						}
					}
				}

				UpdateStates();
			}
		}

		private void UpdateStates()
		{
			_states = _defaultState;
			if (_animatorProperty.objectReferenceValue == null) return;
			var animator = (Animator) _animatorProperty.objectReferenceValue;
			var controller = animator.runtimeAnimatorController as AnimatorController;
			if (controller != null)
			{
				var states = controller.layers.SelectMany(l => l.stateMachine.states)
					.Select(s => (s.state.name)).Distinct();
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
		private SerializedProperty _animatorProperty;
		private GUIContent _animatorNotFound;

		private readonly string[] _defaultState = {"Not Assigned"};
		private string[] _states = new string[1];
	}
}
#endif