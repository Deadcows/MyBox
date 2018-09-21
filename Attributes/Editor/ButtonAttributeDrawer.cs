// ---------------------------------------------------------------------------- 
// Author: Ghat Smith
// Source: https://gist.github.com/GhatSmith/2da8a581bcc6750300d718562f31e63f
// ----------------------------------------------------------------------------

using System;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(ButtonAttribute))]
public sealed class ButtonDrawer : PropertyDrawer
{
	private ButtonAttribute typedAttribute;

	public ButtonAttribute TypedAttribute
	{
		get
		{
			if (typedAttribute == null) typedAttribute = attribute as ButtonAttribute;
			return typedAttribute;
		}
	}

	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
	{
		if (TypedAttribute.DisplayVariable)
		{
			EditorGUI.PropertyField(position, property, label, true);
			position.y += EditorGUI.GetPropertyHeight(property, label, true);
			position.y += 2; // add little space above helpbox
		}

		string tooltip = TypedAttribute.Tooltip;
		if (TypedAttribute.OnlyInPlayMode && !Application.isPlaying)
		{
			EditorGUI.BeginDisabledGroup(true);
			if (string.IsNullOrEmpty(tooltip)) tooltip = "Button only available in play mode";
			else tooltip = TypedAttribute.Tooltip + " (button only available in play mode)";
		}

		position.height = EditorGUI.GetPropertyHeight(property, label, true);
		if (GUI.Button(position, new GUIContent(TypedAttribute.ButtonName, tooltip)))
		{
			var objectReferenceValue = property.serializedObject.targetObject;
			var type = objectReferenceValue.GetType();
			var bindingAttr = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

			try
			{
				var method = type.GetMethod(TypedAttribute.Function, bindingAttr);
				method.Invoke(objectReferenceValue, TypedAttribute.Parameters);
			}
			catch (AmbiguousMatchException)
			{
				var function = $"{type.Name}.{TypedAttribute.Function}";
				var message = $"{function} : AmbiguousMatchException. " +
				              $"Unable to determine which overloaded function is called for {function}. " +
				              "Please delete overloading function";

				Debug.LogError(message, objectReferenceValue);
			}
			catch (ArgumentException)
			{
				var parameters = string.Join(", ", TypedAttribute.Parameters.Select(c => c.ToString()).ToArray());
				var function = $"{type.Name}.{TypedAttribute.Function}";
				var message = $"{function} : ArgumentException. " +
				              $"You can't pass argument {parameters} to the function {function}. " +
				              "Please verify the types of the arguments";

				Debug.LogError(message, objectReferenceValue);
			}
			catch (NullReferenceException)
			{
				var function = $"{type.Name}.{TypedAttribute.Function}";
				var message = $"{function} : NullReferenceException. " +
				              $"Undefined function. Please verify if function is defined in {function}";

				Debug.LogError(message, objectReferenceValue);
			}
			catch (TargetParameterCountException)
			{
				var parameters = string.Join(", ", TypedAttribute.Parameters.Select(c => c.ToString()).ToArray());
				var function = $"{type.Name}.{TypedAttribute.Function}";
				var message = $"{function} : TargetParameterCountException. " +
				              $"You can't pass argument {parameters} to the function {function}. " +
				              "Please verify the number of the parameters given to the function ";

				Debug.LogError(message, objectReferenceValue);
			}
		}

		if (TypedAttribute.OnlyInPlayMode && !Application.isPlaying) EditorGUI.EndDisabledGroup();

	}

	public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
	{
		if (TypedAttribute.DisplayVariable) return EditorGUI.GetPropertyHeight(property, label, true) + GetButtonHeight();
		return GetButtonHeight();

		float GetButtonHeight()
		{
			GUIContent content = new GUIContent(TypedAttribute.ButtonName);

			GUIStyle style = GUI.skin.box;
			style.alignment = TextAnchor.MiddleCenter;

			// Compute how large the button needs to be.        
			Vector2 size = style.CalcSize(content);
			return size.y;
		}
	}
}