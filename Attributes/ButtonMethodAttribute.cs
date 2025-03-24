// ---------------------------------------------------------------------------- 
// Author: Kaynn, Yeo Wen Qin
// https://github.com/Kaynn-Cahya
// Date:   26/02/2019
// ----------------------------------------------------------------------------

using System;
using JetBrains.Annotations;
using MyBox.Internal;
using UnityEngine;
using Object = UnityEngine.Object;

namespace MyBox
{
	[AttributeUsage(AttributeTargets.Method)] 
	[PublicAPI]
	public class ButtonMethodAttribute : PropertyAttribute
	{
		public readonly ButtonMethodDrawOrder DrawOrder;
		public readonly ConditionalData Condition;
		
		public ButtonMethodAttribute(ButtonMethodDrawOrder drawOrder = ButtonMethodDrawOrder.AfterInspector) => DrawOrder = drawOrder;

		public ButtonMethodAttribute(ButtonMethodDrawOrder drawOrder, string fieldToCheck, bool inverse = false, params object[] compareValues)
			=> (DrawOrder, Condition) = (drawOrder, new ConditionalData(fieldToCheck, inverse, compareValues));

		public ButtonMethodAttribute(ButtonMethodDrawOrder drawOrder, string[] fieldToCheck, bool[] inverse = null, params object[] compare)
			=> (DrawOrder, Condition) = (drawOrder, new ConditionalData(fieldToCheck, inverse, compare));

		public ButtonMethodAttribute(ButtonMethodDrawOrder drawOrder, params string[] fieldToCheck) 
			=> (DrawOrder, Condition) = (drawOrder, new ConditionalData(fieldToCheck));
		
		public ButtonMethodAttribute(ButtonMethodDrawOrder drawOrder, bool useMethod, string method, bool inverse = false) 
			=> (DrawOrder, Condition) = (drawOrder, new ConditionalData(useMethod, method, inverse));
	}

	public enum ButtonMethodDrawOrder
	{
		BeforeInspector, 
		AfterInspector
	}
}

#if UNITY_EDITOR
namespace MyBox.Internal
{
	using System.Linq;
	using System.Collections.Generic;
	using System.Reflection;
	using UnityEditor;
	
	public class ButtonMethodHandler
	{
		public readonly List<(MethodInfo Method, string Name, ButtonMethodDrawOrder Order, ConditionalData Condition)> TargetMethods;
		public int Amount => TargetMethods?.Count ?? 0;
		
		private readonly Object _target;

		public ButtonMethodHandler(Object target)
		{
			_target = target;
			
			var type = target.GetType();
			var bindings = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;
			var members = type.GetMembers(bindings).Where(IsButtonMethod);

			foreach (var member in members)
			{
				var method = member as MethodInfo;
				if (method == null) continue;
				
				if (IsValidMember(method, member))
				{
					var attribute = (ButtonMethodAttribute)Attribute.GetCustomAttribute(method, typeof(ButtonMethodAttribute));
					if (TargetMethods == null) TargetMethods = new List<(MethodInfo, string, ButtonMethodDrawOrder, ConditionalData)>();
					TargetMethods.Add((method, method.Name.SplitCamelCase(), attribute.DrawOrder, attribute.Condition));
				}
			}
		}

		public void OnBeforeInspectorGUI()
		{
			if (TargetMethods == null) return;
			
			bool anyDrawn = false;
			foreach (var method in TargetMethods)
			{
				if (method.Order != ButtonMethodDrawOrder.BeforeInspector) continue;
				if (method.Condition != null && !ConditionalUtility.IsConditionMatch(_target, method.Condition)) continue;
				
				anyDrawn = true;
				if (GUILayout.Button(method.Name)) InvokeMethod(_target, method.Method);
			}
			
			if (anyDrawn) EditorGUILayout.Space();
		}

		public void OnAfterInspectorGUI()
		{
			if (TargetMethods == null) return;
			bool anyDrawn = false;

			foreach (var method in TargetMethods)
			{
				if (method.Order != ButtonMethodDrawOrder.AfterInspector) continue;
				if (method.Condition != null && !ConditionalUtility.IsConditionMatch(_target, method.Condition)) continue;
				
				if (!anyDrawn)
				{
					EditorGUILayout.Space();
					anyDrawn = true;
				}
				
				if (GUILayout.Button(method.Name)) InvokeMethod(_target, method.Method);
			}
		}
		
		public void Invoke(MethodInfo method) => InvokeMethod(_target, method);

		
		private void InvokeMethod(Object target, MethodInfo method)
		{
			var result = method.Invoke(target, null);

			if (result != null)
			{
				var message = $"{result} \nResult of Method '{method.Name}' invocation on object {target.name}";
				Debug.Log(message, target);
			}
		}
		
		private bool IsButtonMethod(MemberInfo memberInfo)
		{
			return Attribute.IsDefined(memberInfo, typeof(ButtonMethodAttribute));
		}
			
		private bool IsValidMember(MethodInfo method, MemberInfo member)
		{
			if (method == null)
			{
				Debug.LogWarning(
					$"Property <color=brown>{member.Name}</color>.Reason: Member is not a method but has EditorButtonAttribute!");
				return false;
			}

			if (method.GetParameters().Length > 0)
			{
				Debug.LogWarning(
					$"Method <color=brown>{method.Name}</color>.Reason: Methods with parameters is not supported by EditorButtonAttribute!");
				return false;
			}

			return true;
		}
	}
}
#endif