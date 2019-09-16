#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEngine;

namespace MyBox.Internal
{
	/// <summary>
	/// This pool is used to prevent warning message spamming. 
	/// </summary>
	public static class WarningsPool
	{
		public static bool Log(string message, Object target = null)
		{
			if (Pool.Contains(message)) return false;

			if (target != null) Debug.LogWarning(message, target);
			else Debug.LogWarning(message);

			Pool.Add(message);
			return true;
		}

		private static readonly HashSet<string> Pool = new HashSet<string>();
	}
}
#endif