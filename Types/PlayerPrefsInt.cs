using System;
using MyBox.Internal;
using UnityEngine;

namespace MyBox
{
	[Serializable]
	public class PlayerPrefsInt : PlayerPrefsType
	{
		public int Value
		{
			get => PlayerPrefs.GetInt(Key, DefaultValue);
			set => PlayerPrefs.SetInt(Key, value);
		}

		public int DefaultValue;
		
		public static PlayerPrefsInt WithKey(string key, int defaultValue = 0) => new PlayerPrefsInt(key, defaultValue);

		public PlayerPrefsInt(string key, int defaultValue = 0)
		{
			Key = key;
			DefaultValue = defaultValue;
		}
	}
}