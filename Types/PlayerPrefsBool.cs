using MyBox.Internal;
using UnityEngine;

namespace MyBox
{
	public class PlayerPrefsBool : PlayerPrefsType
	{
		public bool Value
		{
			get => PlayerPrefs.GetInt(Key, DefaultValue ? 1 : 0) == 1;
			set => PlayerPrefs.SetInt(Key, value ? 1 : 0);
		}
		public bool DefaultValue;
		
		public static PlayerPrefsBool WithKey(string key, bool defaultValue = false) => new PlayerPrefsBool(key);

		public PlayerPrefsBool(string key, bool defaultValue = false)
		{
			Key = key;
			DefaultValue = defaultValue;
		}
	}
}