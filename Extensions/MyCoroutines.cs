using System.Collections;
using System.Collections.Generic;
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
	
	public class CoroutineGroup
	{
		private readonly List<Coroutine> _activeCoroutines = new List<Coroutine>();

		public int ActiveCoroutinesAmount
		{
			get { return _activeCoroutines.Count; }
		}
		public bool AnyProcessing
		{
			get
			{
				return _activeCoroutines.Count > 0;
			}
		}
			
		private readonly MonoBehaviour _owner;
			
		public CoroutineGroup(MonoBehaviour owner)
		{
			_owner = owner;
		}

		public Coroutine StartCoroutine(IEnumerator coroutine)
		{
			return _owner.StartCoroutine(DoStart(coroutine));
		}

		public void StopAll()
		{
			for (var i = 0; i < _activeCoroutines.Count; i++)
				_owner.StopCoroutine(_activeCoroutines[i]);
		}
			
		private IEnumerator DoStart(IEnumerator coroutine)
		{
			var started = _owner.StartCoroutine(coroutine);
			
			_activeCoroutines.Add(started);
			yield return started;
			_activeCoroutines.Remove(started);
		}
	}
}

namespace MyBox.Internal
{
	internal class CoroutineOwner : MonoBehaviour
	{
	}
}