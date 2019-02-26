// ---------------------------------------------------------------------------- 
// Author: Kaynn, Yeo Wen Qin
// https://github.com/Kaynn-Cahya
// Date:   26/02/2019
// ----------------------------------------------------------------------------

using System;
using UnityEngine;

#if UNITY_EDITOR
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
#endif

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
	[CustomEditor(typeof(MonoBehaviour), true)]
	public class ButtonMethodAttributeEditor : Editor
	{
		private List<MethodInfo> _methods;
		private MonoBehaviour _target;

		private void OnEnable()
		{
			_target = target as MonoBehaviour;
			var members = GetAllEditorButtonAttributedMembersFromTargetMonoBehaviour(_target);
			
			foreach (var memberInfo in members)
			{
				var method = memberInfo as MethodInfo;
				if (method == null)
				{
					Debug.LogWarning(string.Format("Property <color=brown>{0}</color>.Reason: Member is not a method but has EditorButtonAttribute!",
						memberInfo.Name));
					continue;
				}

				if (method.GetParameters().Length > 0)
				{
					Debug.LogWarning(string.Format("Method <color=brown>{0}</color>.Reason: Methods with parameters is not supported by EditorButtonAttribute!",
						method.Name));
					continue;
				}

				if (_methods == null) _methods = new List<MethodInfo>();
				_methods.Add(method);
			}
		}

		public override void OnInspectorGUI()
		{
			base.OnInspectorGUI();

			if (_methods == null) return;
			foreach (MethodInfo memberInfo in _methods)
			{
				DrawMethodButton(memberInfo);
			}
		}

		private void DrawMethodButton(MethodInfo methodInfo)
		{
			if (GUILayout.Button(methodInfo.Name))
			{
				InvokeMethod(methodInfo);
			}
		}

		private void InvokeMethod(MethodInfo method)
		{
			var result = method.Invoke(target, null);
			
			if (result != null)
			{
				var message = string.Format("{0} \nResult of Method '{1}' invocation on object {2}", result, method.Name, _target.name);
				Debug.Log(message , _target);
			}
		}

		#region Util

		private IEnumerable<MemberInfo> GetAllEditorButtonAttributedMembersFromTargetMonoBehaviour(MonoBehaviour target)
		{
			return target.GetType()
				.GetMembers(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
				.Where(MemberInfoHasEditorInvokableAttribute);
		}

		private bool MemberInfoHasEditorInvokableAttribute(MemberInfo memberInfo)
		{
			return Attribute.IsDefined(memberInfo, typeof(ButtonMethodAttribute));
		}

		#endregion
	}
}
#endif