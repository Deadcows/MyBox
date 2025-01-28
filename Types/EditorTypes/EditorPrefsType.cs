#if UNITY_EDITOR
using System;
using UnityEditor;

namespace MyBox.EditorTools
{
	[Serializable]
	public class EditorPrefsType
	{
		public string Key { get; protected set; }
		
		/// <summary>
		/// Is this pref contains any value
		/// </summary>
		public bool IsSet => EditorPrefs.HasKey(Key);

		public void Delete() => EditorPrefs.DeleteKey(Key);
	}
}
#endif