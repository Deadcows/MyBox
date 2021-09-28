using System;
using MyBox.Internal;
using UnityEngine;

namespace MyBox
{
	[Serializable]
	public class PlayerPrefsString : PlayerPrefsType
	{
		public string Value
		{
			get => PlayerPrefs.GetString(Key, string.Empty);
			set => PlayerPrefs.SetString(Key, value);
		}
		
		public static PlayerPrefsString WithKey(string key) => new PlayerPrefsString(key);

		public PlayerPrefsString(string key) => Key = key;
	}
}