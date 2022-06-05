using System;
using UnityEngine;

namespace MyBox.Internal
{
	[Serializable]
	public abstract class PlayerPrefsType
	{
		public string Key { get; protected set; }
		
		public bool IsSet => PlayerPrefs.HasKey(Key);

		public void Delete() => PlayerPrefs.DeleteKey(Key);
	}
}