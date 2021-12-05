using System;
using MyBox.Internal;
using UnityEngine;

namespace MyBox
{
	[Serializable]
	public class PlayerPrefsVector3 : PlayerPrefsType
	{
		public Vector3 Value
		{
			get => new Vector3(
				PlayerPrefs.GetFloat(Key+"x", DefaultValue.x), 
				PlayerPrefs.GetFloat(Key+"y", DefaultValue.y), 
				PlayerPrefs.GetFloat(Key+"z", DefaultValue.z));
			set
			{
				PlayerPrefs.SetFloat(Key+"x", value.x);
				PlayerPrefs.SetFloat(Key+"y", value.y);
				PlayerPrefs.SetFloat(Key+"z", value.z);
			}
		}
		public Vector3 DefaultValue;
		
		public static PlayerPrefsVector3 WithKey(string key, Vector3 defaultValue = new Vector3()) => new PlayerPrefsVector3(key, defaultValue);

		public PlayerPrefsVector3(string key, Vector3 defaultValue = new Vector3())
		{
			Key = key;
			DefaultValue = defaultValue;
		}
	}
}