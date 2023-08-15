using System;
using MyBox.Internal;
using UnityEngine;

namespace MyBox
{
	[Serializable]
	public class PlayerPrefsVector3Int : PlayerPrefsType
	{
		public Vector3Int Value
		{
			get => new Vector3Int(
				PlayerPrefs.GetInt(Key + "x", DefaultValue.x),
				PlayerPrefs.GetInt(Key + "y", DefaultValue.y),
				PlayerPrefs.GetInt(Key + "z", DefaultValue.z));
			set
			{
				PlayerPrefs.SetInt(Key + "x", value.x);
				PlayerPrefs.SetInt(Key + "y", value.y);
				PlayerPrefs.SetInt(Key + "z", value.z);
			}
		}

		public Vector3Int DefaultValue;

		public static PlayerPrefsVector3Int WithKey(string key, Vector3Int defaultValue = new Vector3Int()) =>
			new PlayerPrefsVector3Int(key, defaultValue);

		public PlayerPrefsVector3Int(string key, Vector3Int defaultValue = new Vector3Int())
		{
			Key = key;
			DefaultValue = defaultValue;
		}
	}
}