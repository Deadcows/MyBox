#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEngine;

namespace MyBox.EditorTools
{	
	[Serializable]
	public class EditorPrefsVector3 : EditorPrefsType
	{
		public Vector3 Value
		{
			get => new Vector3(EditorPrefs.GetFloat(Key+"x", 0), EditorPrefs.GetFloat(Key+"y", 0), EditorPrefs.GetFloat(Key+"z", 0));
			set
			{
				EditorPrefs.SetFloat(Key+"x", value.x);
				EditorPrefs.SetFloat(Key+"y", value.y);
				EditorPrefs.SetFloat(Key+"z", value.z);
			}
		}
		
		public EditorPrefsVector3(string key) => Key = key;
		
		public static EditorPrefsVector3 WithKey(string key) => new EditorPrefsVector3(key);
	}
}
#endif