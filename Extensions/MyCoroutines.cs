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
				Object.DontDestroyOnLoad(go);
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

		/// <summary>
		/// Start next coroutine after this one
		/// </summary>
		public static Coroutine StartNext(this Coroutine coroutine, IEnumerator nextCoroutine)
		{
			return StartCoroutine(StartNextCoroutine(coroutine, nextCoroutine));
		}
		
		/// <summary>
		/// Stop coroutine started with MyCoroutines.StartCoroutine
		/// </summary>
		public static void StopCoroutine(Coroutine coroutine)
		{
			CoroutineOwner.StopCoroutine(coroutine);
		}

		/// <summary>
		/// Stop all coroutines started with MyCoroutines.StartCoroutine
		/// </summary>
		public static void StopAllCoroutines()
		{
			CoroutineOwner.StopAllCoroutines();
		}

		/// <summary>
		/// CoroutineGroup allows to start bunch coroutines in one group
		/// and check the amount of running coroutines (or if there is any of them)
		/// </summary>
		public static CoroutineGroup CreateGroup(MonoBehaviour owner = null)
		{
			return new CoroutineGroup(owner != null ? owner : CoroutineOwner);
		}
		
		
		private static IEnumerator StartNextCoroutine(Coroutine coroutine, IEnumerator nextCoroutine)
		{
			yield return coroutine;
			yield return StartCoroutine(nextCoroutine);
		}
	}
}

namespace MyBox.Internal
{
	internal class CoroutineOwner : MonoBehaviour
	{
	}
}