using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace MyBox
{
	/// <summary>
	/// StartShake and StopShake for transform. Use for screenshake on camera transform for instance
	/// </summary>
	public static class TransformShakeExtension
	{
		/// <summary>
		/// Coroutine and Initial Position for transform 
		/// </summary>
		private static Dictionary<Transform, Tuple<Coroutine, Vector3>> _activeShakingTransforms;
		private static float _shakeAmplitude = .15f;
		
		/// <summary>
		/// Shake transform, like for screenshake on camera transform
		/// </summary>
		/// <param name="transform">Transform to shake</param>
		/// <param name="time">Shake time. -1 for infinite shaking</param>
		/// <param name="shakeBounds">Shake amplitude in units</param>
		/// <param name="useUnscaledTime">Shake time should be affected by Time.timeScale or not</param>
		public static void StartShake(this Transform transform, float time = 0.1f, float shakeBounds = 0.1f, bool useUnscaledTime = true)
		{
			if (_activeShakingTransforms == null) _activeShakingTransforms = new Dictionary<Transform, Tuple<Coroutine, Vector3>>();

			BreakShakeIfAny(transform);
			
			var coroutine = TransformShakeCoroutine(transform, time, shakeBounds, useUnscaledTime).StartCoroutine();
			_activeShakingTransforms.Add(transform, new Tuple<Coroutine, Vector3>(coroutine, transform.position));
		}

		/// <summary>
		/// Stop Shake for transform, if shaking now
		/// </summary>
		public static void StopShake(this Transform transform)
		{
			BreakShakeIfAny(transform);
		} 
		
		
		private static IEnumerator TransformShakeCoroutine(Transform transform, float shakeTime, float bounds, bool useUnscaledTime)
		{
			Vector3 initialPosition = transform.position;
			
			float elapsed = 0;
			while (shakeTime < 0 || elapsed < shakeTime)
			{
				yield return null;

				elapsed += useUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;
				
				float xShake = Random.value * _shakeAmplitude * 2 - _shakeAmplitude;
				float yShake = Random.value * _shakeAmplitude * 2 - _shakeAmplitude;
				
				Vector3 newPosition = transform.position;
				newPosition.x += xShake;
				newPosition.y += yShake;

				newPosition.x = Mathf.Clamp(newPosition.x, initialPosition.x - bounds, initialPosition.x + bounds);
				newPosition.y = Mathf.Clamp(newPosition.y, initialPosition.y - bounds, initialPosition.y + bounds);

				transform.position = newPosition;
			}

			transform.position = initialPosition;
			_activeShakingTransforms.Remove(transform);
		}

		private static void BreakShakeIfAny(Transform transform)
		{
			if (_activeShakingTransforms == null || !_activeShakingTransforms.ContainsKey(transform)) return;

			var shakeData = _activeShakingTransforms[transform];
			MyCoroutines.StopCoroutine(shakeData.Item1);
			transform.position = shakeData.Item2;
			
			_activeShakingTransforms.Remove(transform);
		}
	}
}