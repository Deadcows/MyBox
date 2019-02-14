using System;
using System.Reflection;
using UnityEditor;

#if UNITY_EDITOR
using UnityEngine;
#endif

namespace MyBox
{
	/// <summary>
	/// This attribute can only be applied to fields because its
	/// associated PropertyDrawer only operates on fields (either
	/// public or tagged with the [SerializeField] attribute) in
	/// the target MonoBehaviour.
	/// </summary>
	[AttributeUsage(AttributeTargets.Field)]
	public class ButtonAttribute : PropertyAttribute
	{
		public readonly string MethodName;

		public ButtonAttribute(string MethodName)
		{
			this.MethodName = MethodName;
		}
	}

#if UNITY_EDITOR
	[CustomPropertyDrawer(typeof(ButtonAttribute))]
	public class ButtonAttributeDrawer : PropertyDrawer
	{
		private MethodInfo _eventMethodInfo;
		private ButtonAttribute _attribute;

		public override void OnGUI(Rect position, SerializedProperty prop, GUIContent label)
		{
			const int offset = 20;
			var width = position.width - offset * 2;

			Rect buttonRect = new Rect(position.x + offset, position.y, width, position.height);
			if (GUI.Button(buttonRect, label.text, EditorStyles.toolbarButton)) Invoke(prop.serializedObject.targetObject);
		}

		private void Invoke(UnityEngine.Object target)
		{
			if (_attribute == null) _attribute = (ButtonAttribute) attribute;

			Type eventOwnerType = target.GetType();
			string eventName = _attribute.MethodName;

			if (_eventMethodInfo == null)
				_eventMethodInfo = eventOwnerType.GetMethod(eventName,
					BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);

			if (_eventMethodInfo != null) _eventMethodInfo.Invoke(target, null);
			else
				Debug.LogWarning(string.Format("InspectorButtonAttribute caused: Unable to find method {0} in {1}",
					eventName, eventOwnerType));
		}
	}
#endif
}