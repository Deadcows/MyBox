using System.Collections;
using MyBox.Internal;
using UnityEngine;

namespace MyBox
{
	public static class MyCoroutines
	{
		private static CoroutineOwner CoroutineOwner
		{
			get
			{
				if (_coroutineOwner != null) return _coroutineOwner;

				var go = new GameObject("Static Coroutine Owner");
				Object.DontDestroyOnLoad(_coroutineOwner);
				go.hideFlags = HideFlags.HideAndDontSave;
				
				_coroutineOwner = go.AddComponent<CoroutineOwner>();

				return _coroutineOwner;
			}
		}

		private static CoroutineOwner _coroutineOwner;
		
		
		/// <summary>
		/// StartCoroutine without MonoBehaviour
		/// </summary>
		public static Coroutine StartCoroutine(this IEnumerator coroutine)
		{
			return CoroutineOwner.StartCoroutine(coroutine);
		}
	}
}

namespace MyBox.Internal
{
	internal class CoroutineOwner : MonoBehaviour {}
}