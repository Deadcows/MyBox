using System.Collections;
using UnityEngine;

namespace MyBox
{
	public struct ParallelCoroutineGroup
	{
		private readonly MonoBehaviour _owner;
		private int _processingCount;

		public ParallelCoroutineGroup(MonoBehaviour owner)
		{
			_owner = owner;
			_processingCount = 0;
		}

		public void StartInGroup(IEnumerator coroutine)
		{
			_processingCount++;
			_owner.StartCoroutine(DoParallel(coroutine));
		}

		public bool IsProcessing
		{
			get { return _processingCount > 0; }
		}

		private IEnumerator DoParallel(IEnumerator coroutine)
		{
			yield return _owner.StartCoroutine(coroutine);

			_processingCount--;
		}
	}
}