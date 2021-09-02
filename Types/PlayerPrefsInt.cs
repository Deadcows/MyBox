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
			get => PlayerPrefs.GetInt(Key, 0);
			set => PlayerPrefs.SetInt(Key, value);
		}
		
		public static PlayerPrefsInt WithKey(string key) => new PlayerPrefsInt(key);

		public PlayerPrefsInt(string key) => Key = key;
	}
}