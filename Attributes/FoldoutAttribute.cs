// ---------------------------------------------------------------------------- 
// Author: Dimitry, PixeyeHQ
// Project : UNITY FOLDOUT
// https://github.com/PixeyeHQ/InspectorFoldoutGroup
// Contacts : Pix - ask@pixeye.games
// Website : http://www.pixeye.games
// ----------------------------------------------------------------------------

using UnityEngine;

namespace MyBox
{
    public class FoldoutAttribute : PropertyAttribute
    {
        public readonly string Name;
        public readonly bool FoldEverything;

        /// <summary>Adds the property to the specified foldout group.</summary>
        /// <param name="name">Name of the foldout group.</param>
        /// <param name="foldEverything">Toggle to put all properties to the specified group</param>
        public FoldoutAttribute(string name, bool foldEverything = false)
        {
            FoldEverything = foldEverything;
            Name = name;
        }
    }
}

#if UNITY_EDITOR
namespace MyBox.Internal
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using UnityEditor;

    public class FoldoutAttributeHandler
    {
        private readonly Dictionary<string, CacheFoldProp> _cacheFoldouts = new Dictionary<string, CacheFoldProp>();
        private readonly List<SerializedProperty> _props = new List<SerializedProperty>();
        private bool _initialized;

        private readonly UnityEngine.Object _target;
        private readonly SerializedObject _serializedObject;
        
        public bool OverrideInspector => _props.Count > 0;
        
        public FoldoutAttributeHandler(UnityEngine.Object target, SerializedObject serializedObject)
        {
            _target = target;
            _serializedObject = serializedObject;
        }

        public void OnDisable()
        {
            if (_target == null) return;

            foreach (var c in _cacheFoldouts)
            {
                EditorPrefs.SetBool(string.Format($"{c.Value.Attribute.Name}{c.Value.Properties[0].name}{_target.name}"), c.Value.Expanded);
                c.Value.Dispose();
            }
        }

        public void Update()
        {
            _serializedObject.Update();
            Setup();
        }

        public void OnInspectorGUI()
        {
            Header();
            Body();

            _serializedObject.ApplyModifiedProperties();
        }

        private void Header()
        {
            using (new EditorGUI.DisabledScope("m_Script" == _props[0].propertyPath))
            {
                EditorGUILayout.Space();
                EditorGUILayout.PropertyField(_props[0], true);
                EditorGUILayout.Space();
            }
        }

        private void Body()
        {
            foreach (var pair in _cacheFoldouts)
            {
                EditorGUILayout.BeginVertical(StyleFramework.Box);
                Foldout(pair.Value);
                EditorGUILayout.EndVertical();

                EditorGUI.indentLevel = 0;
            }

            EditorGUILayout.Space();

            for (var i = 1; i < _props.Count; i++)
            {
                EditorGUILayout.PropertyField(_props[i], true);
            }

            EditorGUILayout.Space();
        }

        private void Foldout(CacheFoldProp cache)
        {
            cache.Expanded = EditorGUILayout.Foldout(cache.Expanded, cache.Attribute.Name, true, StyleFramework.FoldoutHeader);
            var rect = GUILayoutUtility.GetLastRect();
            rect.x -= 18;
            rect.y -= 4;
            rect.height += 8;
            rect.width += 22;
            EditorGUI.LabelField(rect, GUIContent.none, EditorStyles.helpBox);
            
            if (cache.Expanded)
            {
                EditorGUILayout.Space(2);
                
                foreach (var property in cache.Properties)
                {
                    EditorGUILayout.BeginVertical(StyleFramework.BoxChild);
                    EditorGUILayout.PropertyField(property, new GUIContent(ObjectNames.NicifyVariableName(property.name)), true);
                    EditorGUILayout.EndVertical();
                }
            }
        }
        
        private void Setup()
        {
            if (_initialized) return;

            FoldoutAttribute prevFold = default;

            var length = EditorTypes.Get(_target, out var objectFields);

            for (var i = 0; i < length; i++)
            {
                #region FOLDERS

                var fold = Attribute.GetCustomAttribute(objectFields[i], typeof(FoldoutAttribute)) as FoldoutAttribute;
                CacheFoldProp c;
                if (fold == null)
                {
                    if (prevFold != null && prevFold.FoldEverything)
                    {
                        if (!_cacheFoldouts.TryGetValue(prevFold.Name, out c))
                        {
                            _cacheFoldouts.Add(prevFold.Name,
                                new CacheFoldProp {Attribute = prevFold, Types = new HashSet<string> {objectFields[i].Name}});
                        }
                        else
                        {
                            c.Types.Add(objectFields[i].Name);
                        }
                    }

                    continue;
                }

                prevFold = fold;

                if (!_cacheFoldouts.TryGetValue(fold.Name, out c))
                {
                    var expanded = EditorPrefs.GetBool(string.Format($"{fold.Name}{objectFields[i].Name}{_target.name}"), false);
                    _cacheFoldouts.Add(fold.Name,
                        new CacheFoldProp {Attribute = fold, Types = new HashSet<string> {objectFields[i].Name}, Expanded = expanded});
                }
                else c.Types.Add(objectFields[i].Name);

                #endregion
            }

            var property = _serializedObject.GetIterator();
            var next = property.NextVisible(true);
            if (next)
            {
                do
                {
                    HandleFoldProp(property);
                } while (property.NextVisible(false));
            }

            _initialized = true;
        }

        private void HandleFoldProp(SerializedProperty prop)
        {
            bool shouldBeFolded = false;

            foreach (var pair in _cacheFoldouts)
            {
                if (pair.Value.Types.Contains(prop.name))
                {
                    var pr = prop.Copy();
                    shouldBeFolded = true;
                    pair.Value.Properties.Add(pr);

                    break;
                }
            }

            if (shouldBeFolded == false)
            {
                var pr = prop.Copy();
                _props.Add(pr);
            }
        }

        private class CacheFoldProp
        {
            public HashSet<string> Types = new HashSet<string>();
            public readonly List<SerializedProperty> Properties = new List<SerializedProperty>();
            public FoldoutAttribute Attribute;
            public bool Expanded;

            public void Dispose()
            {
                Properties.Clear();
                Types.Clear();
                Attribute = null;
            }
        }
    }


    static class StyleFramework
    {
        public static readonly GUIStyle Box;
        public static readonly GUIStyle BoxChild;
        public static readonly GUIStyle FoldoutHeader;

        static StyleFramework()
        {
            FoldoutHeader = new GUIStyle(EditorStyles.foldout);
            FoldoutHeader.overflow = new RectOffset(-10, 0, 3, 0);
            FoldoutHeader.padding = new RectOffset(20, 0, 0, 0);
            FoldoutHeader.border = new RectOffset(2, 2, 2, 2);

            Box = new GUIStyle(GUI.skin.box);
            Box.padding = new RectOffset(18, 0, 4, 4);

            BoxChild = new GUIStyle(GUI.skin.box);
        }
    }

    static class EditorTypes
    {
        private static readonly Dictionary<int, List<FieldInfo>> Fields = new Dictionary<int, List<FieldInfo>>(FastComparable.Default);

        public static int Get(Object target, out List<FieldInfo> objectFields)
        {
            var t = target.GetType();
            var hash = t.GetHashCode();

            if (!Fields.TryGetValue(hash, out objectFields))
            {
                var typeTree = GetTypeTree(t);
                objectFields = target.GetType()
                    .GetFields(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.NonPublic)
                    .OrderByDescending(x => typeTree.IndexOf(x.DeclaringType))
                    .ToList();
                Fields.Add(hash, objectFields);
            }

            return objectFields.Count;
        }
        
        static IList<Type> GetTypeTree(Type t)
        {
            var types = new List<Type>();
            while (t.BaseType != null)
            {
                types.Add(t);
                t = t.BaseType;
            }

            return types;
        }
    }


    internal class FastComparable : IEqualityComparer<int>
    {
        public static readonly FastComparable Default = new FastComparable();

        public bool Equals(int x, int y)
        {
            return x == y;
        }

        public int GetHashCode(int obj)
        {
            return obj.GetHashCode();
        }
    }
}
#endif
