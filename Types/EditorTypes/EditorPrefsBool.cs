#if UNITY_EDITOR
using System;
using UnityEditor;

namespace MyBox.EditorTools
{
	[Serializable]
	public class EditorPrefsBool : EditorPrefsType
	{
		public bool Value
		{
			get => EditorPrefs.GetBool(Key, DefaultValue);
			set => EditorPrefs.SetBool(Key, value);
		}

		public bool DefaultValue;
		
		public static EditorPrefsBool WithKey(string key, bool defaultValue = false) => new EditorPrefsBool(key, defaultValue);

		public EditorPrefsBool(string key, bool defaultValue = false)
		{
			Key = key;
			DefaultValue = defaultValue;
		} 
	}
}
#endif