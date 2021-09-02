using System;
using MyBox.Internal;
using UnityEngine;

namespace MyBox
{
	[Serializable]
	public class PlayerPrefsFloat : PlayerPrefsType
	{
		public float Value
		{
			get => PlayerPrefs.GetFloat(Key, 0);
			set => PlayerPrefs.SetFloat(Key, value);
		}

		public static PlayerPrefsFloat WithKey(string key) => new PlayerPrefsFloat(key);

		public PlayerPrefsFloat(string key) => Key = key;
	}
}