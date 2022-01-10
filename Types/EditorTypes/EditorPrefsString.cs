#if UNITY_EDITOR
using System;
using UnityEditor;

namespace MyBox.EditorTools
{
	[Serializable]
	public class EditorPrefsString : EditorPrefsType
	{
		public string Value
		{
			get => EditorPrefs.GetString(Key, DefaultValue);
			set => EditorPrefs.SetString(Key, value);
		}

		public string DefaultValue;
		
		public static EditorPrefsString WithKey(string key, string defaultValue = "") => new EditorPrefsString(key, defaultValue);
		
		public EditorPrefsString(string key, string defaultValue = "")
		{
			Key = key;
			DefaultValue = defaultValue;
		} 
	}
}
#endif