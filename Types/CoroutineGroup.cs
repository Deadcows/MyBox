using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyBox
{
	public class CoroutineGroup
	{
		public int ActiveCoroutinesAmount => _activeCoroutines.Count;
		public bool AnyProcessing => _activeCoroutines.Count > 0;

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
		
		private readonly MonoBehaviour _owner;
		private readonly List<Coroutine> _activeCoroutines = new List<Coroutine>();
			
		private IEnumerator DoStart(IEnumerator coroutine)
		{
			var started = _owner.StartCoroutine(coroutine);
			
			_activeCoroutines.Add(started);
			yield return started;
			_activeCoroutines.Remove(started);
		}
	}
}