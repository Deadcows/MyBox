#if UNITY_EDITOR
using System;
using UnityEditor;

namespace MyBox.EditorTools
{
	[Serializable]
	public class EditorPrefsFloat : EditorPrefsType
	{
		public float Value
		{
			get => EditorPrefs.GetFloat(Key, 0);
			set => EditorPrefs.GetFloat(Key, value);
		}
		
		public EditorPrefsFloat(string key) => Key = key;
		
		public static EditorPrefsFloat WithKey(string key) => new EditorPrefsFloat(key);
	}
}
#endif