// ---------------------------------------------------------------------------- 
// Author: vexe
// Date:   24/06/2015
// Source: https://forum.unity.com/threads/finally-a-serializable-dictionary-for-unity-extracted-from-system-collections-generic.335797/
// ----------------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MyBox
{
	[Serializable]
	public class MyDictionary<TKey, TValue> : IDictionary<TKey, TValue>
	{
		[SerializeField, HideInInspector] private int[] _buckets;
		[SerializeField, HideInInspector] private int[] _hashCodes;
		[SerializeField, HideInInspector] private int[] _next;
		[SerializeField, HideInInspector] private int _count;
		[SerializeField, HideInInspector] private int _version;
		[SerializeField, HideInInspector] private int _freeList;
		[SerializeField, HideInInspector] private int _freeCount;
		[SerializeField, HideInInspector] private TKey[] _keys;
		[SerializeField, HideInInspector] private TValue[] _values;

		readonly IEqualityComparer<TKey> _comparer;

		// Mainly for debugging purposes - to get the key-value pairs display
		public Dictionary<TKey, TValue> AsDictionary
		{
			get { return new Dictionary<TKey, TValue>(this); }
		}

		public int Count
		{
			get { return _count - _freeCount; }
		}

		public TValue this[TKey key, TValue defaultValue]
		{
			get
			{
				int index = FindIndex(key);
				if (index >= 0) return _values[index];
				return defaultValue;
			}
		}

		public TValue this[TKey key]
		{
			get
			{
				int index = FindIndex(key);
				if (index >= 0) return _values[index];
				throw new KeyNotFoundException(key.ToString());
			}

			set { Insert(key, value, false); }
		}

		public MyDictionary() : this(0, null)
		{
		}

		public MyDictionary(int capacity) : this(capacity, null)
		{
		}

		public MyDictionary(IEqualityComparer<TKey> comparer) : this(0, comparer)
		{
		}

		public MyDictionary(int capacity, IEqualityComparer<TKey> comparer)
		{
			if (capacity < 0) throw new ArgumentOutOfRangeException("capacity");

			Initialize(capacity);

			_comparer = (comparer ?? EqualityComparer<TKey>.Default);
		}

		public MyDictionary(IDictionary<TKey, TValue> dictionary) : this(dictionary, null)
		{
		}


		public MyDictionary(IDictionary<TKey, TValue> dictionary, IEqualityComparer<TKey> comparer)
			: this((dictionary != null) ? dictionary.Count : 0, comparer)
		{
			if (dictionary == null) throw new ArgumentNullException("dictionary");

			foreach (KeyValuePair<TKey, TValue> current in dictionary)
				Add(current.Key, current.Value);
		}

		public bool ContainsValue(TValue value)
		{
			if (value == null)
			{
				for (int i = 0; i < _count; i++)
				{
					if (_hashCodes[i] >= 0 && _values[i] == null)
						return true;
				}
			}
			else
			{
				var defaultComparer = EqualityComparer<TValue>.Default;
				for (int i = 0; i < _count; i++)
				{
					if (_hashCodes[i] >= 0 && defaultComparer.Equals(_values[i], value))
						return true;
				}
			}

			return false;
		}

		public bool ContainsKey(TKey key)
		{
			return FindIndex(key) >= 0;
		}

		public void Clear()
		{
			if (_count <= 0)
				return;

			for (int i = 0; i < _buckets.Length; i++)
				_buckets[i] = -1;

			Array.Clear(_keys, 0, _count);
			Array.Clear(_values, 0, _count);
			Array.Clear(_hashCodes, 0, _count);
			Array.Clear(_next, 0, _count);

			_freeList = -1;
			_count = 0;
			_freeCount = 0;
			_version++;
		}

		public void Add(TKey key, TValue value)
		{
			Insert(key, value, true);
		}

		private void Resize(int newSize, bool forceNewHashCodes)
		{
			int[] bucketsCopy = new int[newSize];
			for (int i = 0; i < bucketsCopy.Length; i++)
				bucketsCopy[i] = -1;

			var keysCopy = new TKey[newSize];
			var valuesCopy = new TValue[newSize];
			var hashCodesCopy = new int[newSize];
			var nextCopy = new int[newSize];

			Array.Copy(_values, 0, valuesCopy, 0, _count);
			Array.Copy(_keys, 0, keysCopy, 0, _count);
			Array.Copy(_hashCodes, 0, hashCodesCopy, 0, _count);
			Array.Copy(_next, 0, nextCopy, 0, _count);

			if (forceNewHashCodes)
			{
				for (int i = 0; i < _count; i++)
				{
					if (hashCodesCopy[i] != -1)
						hashCodesCopy[i] = (_comparer.GetHashCode(keysCopy[i]) & 2147483647);
				}
			}

			for (int i = 0; i < _count; i++)
			{
				int index = hashCodesCopy[i] % newSize;
				nextCopy[i] = bucketsCopy[index];
				bucketsCopy[index] = i;
			}

			_buckets = bucketsCopy;
			_keys = keysCopy;
			_values = valuesCopy;
			_hashCodes = hashCodesCopy;
			_next = nextCopy;
		}

		private void Resize()
		{
			Resize(PrimeHelper.ExpandPrime(_count), false);
		}

		public bool Remove(TKey key)
		{
			if (key == null) throw new ArgumentNullException("key");

			int hash = _comparer.GetHashCode(key) & 2147483647;
			int index = hash % _buckets.Length;
			int num = -1;
			for (int i = _buckets[index]; i >= 0; i = _next[i])
			{
				if (_hashCodes[i] == hash && _comparer.Equals(_keys[i], key))
				{
					if (num < 0)
						_buckets[index] = _next[i];
					else
						_next[num] = _next[i];

					_hashCodes[i] = -1;
					_next[i] = _freeList;
					_keys[i] = default(TKey);
					_values[i] = default(TValue);
					_freeList = i;
					_freeCount++;
					_version++;
					return true;
				}

				num = i;
			}

			return false;
		}

		private void Insert(TKey key, TValue value, bool add)
		{
			if (key == null) throw new ArgumentNullException("key");

			if (_buckets == null)
				Initialize(0);

			int hash = _comparer.GetHashCode(key) & 2147483647;
			int index = hash % _buckets.Length;
			for (int i = _buckets[index]; i >= 0; i = _next[i])
			{
				if (_hashCodes[i] == hash && _comparer.Equals(_keys[i], key))
				{
					if (add)
						throw new ArgumentException("Key already exists: " + key);

					_values[i] = value;
					_version++;
					return;
				}
			}

			int num2;
			if (_freeCount > 0)
			{
				num2 = _freeList;
				_freeList = _next[num2];
				_freeCount--;
			}
			else
			{
				if (_count == _keys.Length)
				{
					Resize();
					index = hash % _buckets.Length;
				}

				num2 = _count;
				_count++;
			}

			_hashCodes[num2] = hash;
			_next[num2] = _buckets[index];
			_keys[num2] = key;
			_values[num2] = value;
			_buckets[index] = num2;
			_version++;

			//if (num3 > 100 && HashHelpers.IsWellKnownEqualityComparer(comparer))
			//{
			//    comparer = (IEqualityComparer<TKey>)HashHelpers.GetRandomizedEqualityComparer(comparer);
			//    Resize(entries.Length, true);
			//}
		}

		private void Initialize(int capacity)
		{
			int prime = PrimeHelper.GetPrime(capacity);

			_buckets = new int[prime];
			for (int i = 0; i < _buckets.Length; i++)
				_buckets[i] = -1;

			_keys = new TKey[prime];
			_values = new TValue[prime];
			_hashCodes = new int[prime];
			_next = new int[prime];

			_freeList = -1;
		}

		private int FindIndex(TKey key)
		{
			if (key == null) throw new ArgumentNullException("key");

			if (_buckets != null)
			{
				int hash = _comparer.GetHashCode(key) & 2147483647;
				for (int i = _buckets[hash % _buckets.Length]; i >= 0; i = _next[i])
				{
					if (_hashCodes[i] == hash && _comparer.Equals(_keys[i], key))
						return i;
				}
			}

			return -1;
		}

		public bool TryGetValue(TKey key, out TValue value)
		{
			int index = FindIndex(key);
			if (index >= 0)
			{
				value = _values[index];
				return true;
			}

			value = default(TValue);
			return false;
		}

		private static class PrimeHelper
		{
			private static readonly int[] Primes =
			{
				3,
				7,
				11,
				17,
				23,
				29,
				37,
				47,
				59,
				71,
				89,
				107,
				131,
				163,
				197,
				239,
				293,
				353,
				431,
				521,
				631,
				761,
				919,
				1103,
				1327,
				1597,
				1931,
				2333,
				2801,
				3371,
				4049,
				4861,
				5839,
				7013,
				8419,
				10103,
				12143,
				14591,
				17519,
				21023,
				25229,
				30293,
				36353,
				43627,
				52361,
				62851,
				75431,
				90523,
				108631,
				130363,
				156437,
				187751,
				225307,
				270371,
				324449,
				389357,
				467237,
				560689,
				672827,
				807403,
				968897,
				1162687,
				1395263,
				1674319,
				2009191,
				2411033,
				2893249,
				3471899,
				4166287,
				4999559,
				5999471,
				7199369
			};

			private static bool IsPrime(int candidate)
			{
				if ((candidate & 1) != 0)
				{
					int num = (int) Math.Sqrt(candidate);
					for (int i = 3; i <= num; i += 2)
					{
						if (candidate % i == 0)
						{
							return false;
						}
					}

					return true;
				}

				return candidate == 2;
			}

			public static int GetPrime(int min)
			{
				if (min < 0)
					throw new ArgumentException("min < 0");

				for (int i = 0; i < Primes.Length; i++)
				{
					int prime = Primes[i];
					if (prime >= min)
						return prime;
				}

				for (int i = min | 1; i < 2147483647; i += 2)
				{
					if (IsPrime(i) && (i - 1) % 101 != 0)
						return i;
				}

				return min;
			}

			public static int ExpandPrime(int oldSize)
			{
				int num = 2 * oldSize;
				if (num > 2146435069 && 2146435069 > oldSize)
				{
					return 2146435069;
				}

				return GetPrime(num);
			}
		}

		public ICollection<TKey> Keys
		{
			get { return _keys.Take(Count).ToArray(); }
		}

		public ICollection<TValue> Values
		{
			get { return _values.Take(Count).ToArray(); }
		}

		public void Add(KeyValuePair<TKey, TValue> item)
		{
			Add(item.Key, item.Value);
		}

		public bool Contains(KeyValuePair<TKey, TValue> item)
		{
			int index = FindIndex(item.Key);
			return index >= 0 &&
			       EqualityComparer<TValue>.Default.Equals(_values[index], item.Value);
		}

		public void CopyTo(KeyValuePair<TKey, TValue>[] array, int index)
		{
			if (array == null) throw new ArgumentNullException("array");

			if (index < 0 || index > array.Length)
				throw new ArgumentOutOfRangeException(string.Format("index = {0} array.Length = {1}", index, array.Length));

			if (array.Length - index < Count)
				throw new ArgumentException(
					string.Format(
						"The number of elements in the dictionary ({0}) is greater than the available space from index to the end of the destination array {1}.",
						Count, array.Length));

			for (int i = 0; i < _count; i++)
			{
				if (_hashCodes[i] >= 0)
					array[index++] = new KeyValuePair<TKey, TValue>(_keys[i], _values[i]);
			}
		}

		public bool IsReadOnly
		{
			get { return false; }
		}

		public bool Remove(KeyValuePair<TKey, TValue> item)
		{
			return Remove(item.Key);
		}

		public Enumerator GetEnumerator()
		{
			return new Enumerator(this);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<TKey, TValue>>.GetEnumerator()
		{
			return GetEnumerator();
		}

		public struct Enumerator : IEnumerator<KeyValuePair<TKey, TValue>>
		{
			private readonly MyDictionary<TKey, TValue> _dictionary;
			private readonly int _Version;
			private int _index;

			public KeyValuePair<TKey, TValue> Current { get; private set; }

			internal Enumerator(MyDictionary<TKey, TValue> dictionary) : this()
			{
				_dictionary = dictionary;
				_Version = dictionary._version;
				Current = new KeyValuePair<TKey, TValue>();
				_index = 0;
			}

			public bool MoveNext()
			{
				if (_Version != _dictionary._version)
					throw new InvalidOperationException(string.Format("Enumerator version {0} != Dictionary version {1}", _Version,
						_dictionary._version));

				while (_index < _dictionary._count)
				{
					if (_dictionary._hashCodes[_index] >= 0)
					{
						Current = new KeyValuePair<TKey, TValue>(_dictionary._keys[_index], _dictionary._values[_index]);
						_index++;
						return true;
					}

					_index++;
				}

				_index = _dictionary._count + 1;
				Current = default(KeyValuePair<TKey, TValue>);
				return false;
			}

			void IEnumerator.Reset()
			{
				if (_Version != _dictionary._version)
					throw new InvalidOperationException(string.Format("Enumerator version {0} != Dictionary version {1}", _Version,
						_dictionary._version));

				_index = 0;
				Current = default(KeyValuePair<TKey, TValue>);
			}

			object IEnumerator.Current
			{
				get { return Current; }
			}


			public void Dispose()
			{
			}
		}
	}
}