using System;
using MyBox.Internal;
using UnityEngine;

namespace MyBox
{
	[Serializable]
	public class PlayerPrefsVector2Int : PlayerPrefsType
	{
		public Vector2Int Value
		{
			get => new Vector2Int(
				PlayerPrefs.GetInt(Key + "x", DefaultValue.x),
				PlayerPrefs.GetInt(Key + "y", DefaultValue.y));
			set
			{
				PlayerPrefs.SetInt(Key + "x", value.x);
				PlayerPrefs.SetInt(Key + "y", value.y);
			}
		}

		public Vector2Int DefaultValue;

		public static PlayerPrefsVector2Int WithKey(string key, Vector2Int defaultValue = new Vector2Int()) =>
			new PlayerPrefsVector2Int(key, defaultValue);

		public PlayerPrefsVector2Int(string key, Vector2Int defaultValue = new Vector2Int())
		{
			Key = key;
			DefaultValue = defaultValue;
		}
	}
}