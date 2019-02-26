using System;
using System.Linq;
using System.Collections.Generic;

using System.Reflection;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace MyBox
{
    [AttributeUsage(AttributeTargets.Method)]
    public class EditorInvokableAttribute : PropertyAttribute
    {
    }
}

#if UNITY_EDITOR
namespace MyBox.Internal
{
    [CustomEditor(typeof(MonoBehaviour), true)]
    public class EditorInvokableAttributeEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            var monoBehaviourTarget = target as MonoBehaviour;

            var methods = GetAllEditorButtonAttributedMembersFromTargetMonoBehaviour(monoBehaviourTarget);

            foreach (MemberInfo memberInfo in methods)
            {
                InvokeMethodIfUserInputButton(memberInfo, monoBehaviourTarget);
            }
        }

        private void InvokeMethodIfUserInputButton(MemberInfo memberInfo, MonoBehaviour target)
        {
            if (GUILayout.Button(memberInfo.Name))
            {
                InvokeIfMemberIsMethod(memberInfo);
            }
        }

        private void InvokeIfMemberIsMethod(MemberInfo member)
        {

            MethodInfo method = member as MethodInfo;
            if (method != null)
            {
                InvokeMethodIfNonParameterized(method);
            } else
            {
                // Liekly to not occur due to the AttributeUsage applied to the attribute.
                Debug.LogWarning(string.Format("Property <color=brown>{0}</color>.Reason: Member is not a method but has EditorButtonAttribute!", member.Name));
            }
        }

        private void InvokeMethodIfNonParameterized(MethodInfo method)
        {
            if (method.GetParameters().Length > 0)
            {
                Debug.LogWarning(string.Format("Method <color=brown>{0}</color>.Reason: Methods with parameters is not supported by EditorButtonAttribute!", method.Name));
            } else
            {
                var returnObj = method.Invoke(target, null);

                if (returnObj != null)
                {
                    Debug.Log(string.Format("Method '{0}' returned: {1}", method.Name, returnObj.ToString()));
                }
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
            return Attribute.IsDefined(memberInfo, typeof(EditorInvokableAttribute));
        }
        #endregion
    }
}
#endif
