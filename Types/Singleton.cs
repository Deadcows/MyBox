using System;
using UnityEngine;

namespace MyBox
{
	public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour
	{
		public static T Instance
		{
			get
			{
				if (_instance == null) _instance = FindObjectOfType<T>();
				if (_instance == null) Debug.LogError("Singleton of type : " + typeof(T).Name + " not found on scene");

				return _instance;
			}
		}
		private static T _instance;

		/// <summary>
		/// Is DontDestroyOnLoad should be applied to initialized singleton
		/// </summary>
		protected virtual bool PersistentSingleton => true;
		
		protected void InitializeSingleton()
		{
			if (_instance == null)
			{
				_instance = (T)Convert.ChangeType(this, typeof(T));
				if (PersistentSingleton) DontDestroyOnLoad(_instance);
			}
			else
			{
				Debug.LogWarning($"Another instance of Singleton<{typeof(T).Name}> detected on GO {name}. Destroyed", gameObject);
				Destroy(this);
			}
		}
	}
}