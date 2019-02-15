using UnityEngine;

namespace MyBox
{
	/// <summary>
	/// Use to wrap values, that may be changed once per frame to prevent extra calculations
	/// </summary>
	public struct Cached<T>
	{
		public bool IsCached
		{
			get
			{
				if (!_cachedOnce) return false;
				return Time.frameCount == _cacheFrame;
			}
		}

		public T Value
		{
			get
			{
				if (!IsCached) Debug.LogError("Value is not cached. Use IsCached");
				return _value;
			}
			set
			{
				_value = value;
				_cachedOnce = true;
				_cacheFrame = Time.frameCount;
			}
		}

		private T _value;
		private int _cacheFrame;
		private bool _cachedOnce;
	}
}