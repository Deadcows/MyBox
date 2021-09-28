using MyBox.Internal;
using UnityEngine;

namespace MyBox
{
	public class PlayerPrefsBool : PlayerPrefsType
	{
		public bool Value
		{
			get => PlayerPrefs.GetInt(Key, 0) == 1;
			set => PlayerPrefs.SetInt(Key, value ? 1 : 0);
		}
		
		public static PlayerPrefsBool WithKey(string key) => new PlayerPrefsBool(key);

		public PlayerPrefsBool(string key) => Key = key;
	}
}