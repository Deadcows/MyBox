using UnityEngine;

namespace MyBox
{
	public class WaitForUnscaledSeconds : CustomYieldInstruction
	{
		private readonly float _waitTime;

		public override bool keepWaiting
		{
			get { return Time.realtimeSinceStartup < _waitTime; }
		}

		public WaitForUnscaledSeconds(float secondsToWait)
		{
			_waitTime = Time.realtimeSinceStartup + secondsToWait;
		}
	}
}

