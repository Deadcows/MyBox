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
			get => new Vector3(PlayerPrefs.GetFloat(Key+"x", 0), PlayerPrefs.GetFloat(Key+"y", 0), PlayerPrefs.GetFloat(Key+"z", 0));
			set
			{
				PlayerPrefs.SetFloat(Key+"x", value.x);
				PlayerPrefs.SetFloat(Key+"y", value.y);
				PlayerPrefs.SetFloat(Key+"z", value.z);
			}
		}
		
		public static PlayerPrefsVector3 WithKey(string key) => new PlayerPrefsVector3(key);

		public PlayerPrefsVector3(string key) => Key = key;
	}
}