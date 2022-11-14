using System.Collections.Generic;
using UnityEngine;

namespace MyBox
{
	/// <summary>
	/// Use to display inspector of property object
	/// </summary>
	public class DisplayInspectorAttribute : PropertyAttribute
	{
		public readonly bool DisplayScript;

		public DisplayInspectorAttribute(bool displayScriptField = true)
		{
			DisplayScript = displayScriptField;
		}
	}
}

#if UNITY_EDITOR
namespace MyBox.Internal
{
	using EditorTools;
	using UnityEditor;

	[CustomPropertyDrawer(typeof(DisplayInspectorAttribute))]
	public class DisplayInspectorAttributeDrawer : PropertyDrawer
	{
		private ButtonMethodHandler _buttonMethods;
		private EditorPrefsBool _foldout;

		private readonly Dictionary<Object, SerializedObject> _targets = new Dictionary<Object, SerializedObject>();

		private SerializedObject GetTargetSO(Object targetObject)
		{
			SerializedObject target;
			if (_targets.ContainsKey(targetObject)) target = _targets[targetObject];
			else
			{
				_targets.Add(targetObject, new SerializedObject(targetObject));
				target = _targets[targetObject];
			}

			target.Update();
			return target;
		}
		
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			bool notValidType = property.propertyType != SerializedPropertyType.ObjectReference;
			if (notValidType)
			{
				EditorGUI.LabelField(position, label.text, "Use [DisplayInspector] with MB or SO");
				return;
			}
			
			position.height = EditorGUIUtility.singleLineHeight;
			bool displayScript = ((DisplayInspectorAttribute)attribute).DisplayScript;
			if (displayScript || property.objectReferenceValue == null)
			{
				// Draw foldout only if Script line drawn and there is content to hide (ref assigned)
				if (property.objectReferenceValue != null)
				{
					// Workaround to make label clickable, accurately aligned and property field click is not triggering foldout
					var foldRect = new Rect(position);
					foldRect.width = EditorGUIUtility.labelWidth;
					_foldout.Value = EditorGUI.Foldout(foldRect, _foldout.Value, new GUIContent(""), true, StyleFramework.FoldoutHeader);
					EditorGUI.PropertyField(position, property, label);
					if (GUI.changed) property.serializedObject.ApplyModifiedProperties();
					
					if (!_foldout.Value) return;
				}
				else
				{
					EditorGUI.PropertyField(position, property, label);
					if (GUI.changed) property.serializedObject.ApplyModifiedProperties();
				}
			}
			if (property.objectReferenceValue == null) return;

			if (_buttonMethods == null) _buttonMethods = new ButtonMethodHandler(property.objectReferenceValue);

			if (displayScript) position.y += position.height + 4;
			var startY = position.y - 2;
			float startX = position.x;

			var target = GetTargetSO(property.objectReferenceValue);
			var propertyObject = target.GetIterator();
			propertyObject.Next(true);
			propertyObject.NextVisible(true);

			var xPos = position.x + 10;
			var width = position.width - 10;

			bool expandedReorderable = false;
			while (propertyObject.NextVisible(propertyObject.isExpanded && !expandedReorderable))
			{
#if UNITY_2020_2_OR_NEWER
				expandedReorderable = propertyObject.isExpanded && propertyObject.isArray &&
				                      !propertyObject.IsAttributeDefined<NonReorderableAttribute>();
#endif
				position.x = xPos + 10 * propertyObject.depth;
				position.width = width - 10 * propertyObject.depth;

				position.height = EditorGUI.GetPropertyHeight(propertyObject, expandedReorderable);
				EditorGUI.PropertyField(position, propertyObject, expandedReorderable);

				position.y += position.height + 4;
			}

			if (!_buttonMethods.TargetMethods.IsNullOrEmpty())
			{
				foreach (var method in _buttonMethods.TargetMethods)
				{
					position.height = EditorGUIUtility.singleLineHeight;
					if (GUI.Button(position, method.Name)) _buttonMethods.Invoke(method.Method);
					position.y += position.height;
				}
			}

			var bgRect = position;
			bgRect.y = startY;
			bgRect.x = startX - 12;
			bgRect.width = 11;
			bgRect.height = position.y - startY;
			if (_buttonMethods.Amount > 0) bgRect.height += 5;

			DrawColouredRect(bgRect, new Color(.6f, .6f, .8f, .5f));


			target.ApplyModifiedProperties();
			property.serializedObject.ApplyModifiedProperties();
		}

		private bool IsFolded(SerializedProperty property)
		{
			if(_foldout == null)
			{
				_foldout = new EditorPrefsBool("DisplayInspectorFoldout" +
			                                 property.GetParent().GetType().Name +
			                                 property.propertyPath);
			}
			return _foldout.Value;
		}
		
		public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
			bool notValidType = property.propertyType != SerializedPropertyType.ObjectReference;
			bool displayScript = ((DisplayInspectorAttribute)attribute).DisplayScript;
			if (notValidType || property.objectReferenceValue == null || (displayScript && !IsFolded(property))) return base.GetPropertyHeight(property, label);
			
			if (_buttonMethods == null) _buttonMethods = new ButtonMethodHandler(property.objectReferenceValue);
			float height = displayScript ? EditorGUI.GetPropertyHeight(property) + 4 : 0;

			var target = GetTargetSO(property.objectReferenceValue);
			var propertyObject = target.GetIterator();
			propertyObject.Next(true);
			propertyObject.NextVisible(true);

			bool expandedReorderable = false;
			while (propertyObject.NextVisible(propertyObject.isExpanded && !expandedReorderable))
			{
#if UNITY_2020_2_OR_NEWER
				expandedReorderable = propertyObject.isExpanded && propertyObject.isArray &&
				                      !propertyObject.IsAttributeDefined<NonReorderableAttribute>();
#endif
				height += EditorGUI.GetPropertyHeight(propertyObject, expandedReorderable) + 4;
			}

			if (_buttonMethods.Amount > 0) height += 4 + _buttonMethods.Amount * EditorGUIUtility.singleLineHeight;
			return height;
		}

		private void DrawColouredRect(Rect rect, Color color)
		{
			var defaultBackgroundColor = GUI.backgroundColor;
			GUI.backgroundColor = color;
			GUI.Box(rect, "");
			GUI.backgroundColor = defaultBackgroundColor;
		}
	}
}
#endif
