// ---------------------------------------------------------------------------- 
// Author: Kaynn, Yeo Wen Qin
// https://github.com/Kaynn-Cahya
// Date:   26/02/2019
// ----------------------------------------------------------------------------

using System;
using System.Text.RegularExpressions;
using UnityEngine;

namespace MyBox
{
	[AttributeUsage(AttributeTargets.Method)]
	public class ButtonMethodAttribute : PropertyAttribute
	{
	}
}

#if UNITY_EDITOR
namespace MyBox.Internal
{
	using System.Linq;
	using System.Collections.Generic;
	using System.Reflection;
	using UnityEditor;
	
	[CustomEditor(typeof(MonoBehaviour), true), CanEditMultipleObjects]
	public class ButtonMethodMonoBehaviourEditor : Editor
	{
		private List<MethodInfo> _methods;
		private MonoBehaviour _target;

		private void OnEnable()
		{
			_target = target as MonoBehaviour;
			if (_target == null) return;

			_methods = ButtonMethodHandler.CollectValidMembers(_target.GetType());
		}

		public override void OnInspectorGUI()
		{
			base.OnInspectorGUI();
			if (_methods == null) return;

			ButtonMethodHandler.OnInspectorGUI(_target, _methods);
		}
	}


	[CustomEditor(typeof(ScriptableObject), true), CanEditMultipleObjects]
	public class ButtonMethodScriptableObjectEditor : Editor
	{
		private List<MethodInfo> _methods;
		private ScriptableObject _target;

		private void OnEnable()
		{
			_target = target as ScriptableObject;
			if (_target == null) return;

			_methods = ButtonMethodHandler.CollectValidMembers(_target.GetType());
		}

		public override void OnInspectorGUI()
		{
			base.OnInspectorGUI();
			if (_methods == null) return;

			ButtonMethodHandler.OnInspectorGUI(_target, _methods);
		}
	}

	public static class ButtonMethodHandler
	{
		public static List<MethodInfo> CollectValidMembers(Type type)
		{
			List<MethodInfo> methods = null;

			var members = type.GetMembers(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
				.Where(IsButtonMethod);

			foreach (var member in members)
			{
				var method = member as MethodInfo;
				if (IsValidMember(method, member))
				{
					if (methods == null) methods = new List<MethodInfo>();
					methods.Add(method);
				}
			}

			return methods;
		}

		public static void OnInspectorGUI(UnityEngine.Object target, List<MethodInfo> methods)
		{
			EditorGUILayout.Space();

			foreach (MethodInfo method in methods)
			{
				if (GUILayout.Button(SplitCamelCase(method.Name))) InvokeMethod(target, method);
			}
		}

		private static void InvokeMethod(UnityEngine.Object target, MethodInfo method)
		{
			var result = method.Invoke(target, null);

			if (result != null)
			{
				var message = string.Format("{0} \nResult of Method '{1}' invocation on object {2}", result, method.Name, target.name);
				Debug.Log(message, target);
			}
		}

		private static bool IsValidMember(MethodInfo method, MemberInfo member)
		{
			if (method == null)
			{
				Debug.LogWarning(
					string.Format("Property <color=brown>{0}</color>.Reason: Member is not a method but has EditorButtonAttribute!",
						member.Name));
				return false;
			}

			if (method.GetParameters().Length > 0)
			{
				Debug.LogWarning(
					string.Format("Method <color=brown>{0}</color>.Reason: Methods with parameters is not supported by EditorButtonAttribute!",
						method.Name));
				return false;
			}

			return true;
		}

		private static bool IsButtonMethod(MemberInfo memberInfo)
		{
			return Attribute.IsDefined(memberInfo, typeof(ButtonMethodAttribute));
		}
		
		
		/// <summary>
		/// "CamelCaseString" => "Camel Case String"
		/// COPY OF MyString.SplitCamelCase()
		/// </summary>
		private static string SplitCamelCase(string camelCaseString)
		{
			if (string.IsNullOrEmpty(camelCaseString)) return camelCaseString;

			string camelCase = Regex.Replace(Regex.Replace(camelCaseString, @"(\P{Ll})(\P{Ll}\p{Ll})", "$1 $2"), @"(\p{Ll})(\P{Ll})", "$1 $2");
			string firstLetter = camelCase.Substring(0, 1).ToUpper();

			if (camelCaseString.Length > 1)
			{
				string rest = camelCase.Substring(1);

				return firstLetter + rest;
			}
			return firstLetter;
		}
	}
}
#endif