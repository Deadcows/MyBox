using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

namespace MyBox
{
	public static class MyCollections
	{
		/// <summary>
		/// Returns new array without element at index
		/// </summary>
		public static T[] RemoveAt<T>(this T[] array, int index)
		{
			if (index < 0)
			{
				Debug.LogError("Index is less than zero. Array is not modified");
				return array;
			}

			if (index >= array.Length)
			{
				Debug.LogError("Index exceeds array length. Array is not modified");
				return array;
			}

			T[] newArray = new T[array.Length - 1];
			int index1 = 0;
			for (int index2 = 0; index2 < array.Length; ++index2)
			{
				if (index2 == index) continue;

				newArray[index1] = array[index2];
				++index1;
			}

			return newArray;
		}

		/// <summary>
		/// Returns new array with inserted empty element at index
		/// </summary>
		public static T[] InsertAt<T>(this T[] array, int index)
		{
			if (index < 0)
			{
				Debug.LogError("Index is less than zero. Array is not modified");
				return array;
			}

			if (index > array.Length)
			{
				Debug.LogError("Index exceeds array length. Array is not modified");
				return array;
			}

			T[] newArray = new T[array.Length + 1];
			int index1 = 0;
			for (int index2 = 0; index2 < newArray.Length; ++index2)
			{
				if (index2 == index) continue;

				newArray[index2] = array[index1];
				++index1;
			}

			return newArray;
		}


		/// <summary>
		/// Returns random element from collection
		/// </summary>
		public static T GetRandom<T>(this T[] collection)
		{
			return collection[UnityEngine.Random.Range(0, collection.Length)];
		}

		/// <summary>
		/// Returns random element from collection
		/// </summary>
		public static T GetRandom<T>(this IList<T> collection)
		{
			return collection[UnityEngine.Random.Range(0, collection.Count)];
		}

		/// <summary>
		/// Returns random element from collection
		/// </summary>
		public static T GetRandom<T>(this IEnumerable<T> collection)
		{
			return collection.ElementAt(UnityEngine.Random.Range(0, collection.Count()));
		}



		public static T[] GetRandomCollection<T>(this IList<T> collection, int amount)
		{
			if (amount > collection.Count)
			{
				Debug.LogError("GetRandomCollection Caused: source collection items count is less than randoms count");
				return null;
			}

			var randoms = new T[amount];
			var indexes = Enumerable.Range(0, amount).ToList();

			for (var i = 0; i < amount; i++)
			{
				var random = UnityEngine.Random.Range(0, indexes.Count);
				randoms[i] = collection[random];
				indexes.RemoveAt(random);
			}

			return randoms;
		}



		/// <summary>
		/// Is array null or empty
		/// </summary>
		public static bool IsNullOrEmpty<T>(this T[] collection)
		{
			if (collection == null) return true;

			return collection.Length == 0;
		}

		/// <summary>
		/// Is list null or empty
		/// </summary>
		public static bool IsNullOrEmpty<T>(this IList<T> collection)
		{
			if (collection == null) return true;

			return collection.Count == 0;
		}

		/// <summary>
		/// Is collection null or empty. IEnumerable is relatively slow. Use Array or List implementation if possible
		/// </summary>
		public static bool IsNullOrEmpty<T>(this IEnumerable<T> collection)
		{
			if (collection == null) return true;

			return !collection.Any();
		}



		/// <summary>
		/// Get next index for circular array. <br />
		/// -1 will result with last element index, Length + 1 is 0. <br />
		/// If step is more that 1, you will get correct offset <br />
		/// 
		/// <code>
		/// Example (infinite loop first->last->first):
		/// i = myArray.NextIndex(i++);
		/// var nextItem = myArray[i];
		/// </code>
		/// </summary>
		public static int NextIndexInCircle<T>(this T[] array, int desiredPosition)
		{
			if (array.IsNullOrEmpty())
			{
				Debug.LogError("NextIndexInCircle Caused: source array is null or empty");
				return -1;
			}

			var length = array.Length;
			if (length == 1) return 0;

			return (desiredPosition % length + length) % length;
		}


		/// <returns>
		/// Returns -1 if none found
		/// </returns>
		public static int IndexOfItem<T>(this IEnumerable<T> collection, T item)
		{
			if (collection == null)
			{
				Debug.LogError("IndexOfItem Caused: source collection is null");
				return -1;
			}

			var index = 0;
			foreach (var i in collection)
			{
				if (Equals(i, item)) return index;
				++index;
			}

			return -1;
		}

		/// <summary>
		/// Is Elements in two collections are the same
		/// </summary>
		public static bool ContentsMatch<T>(this IEnumerable<T> first, IEnumerable<T> second)
		{
			if (first.IsNullOrEmpty() && second.IsNullOrEmpty()) return true;
			if (first.IsNullOrEmpty() || second.IsNullOrEmpty()) return false;

			var firstCount = first.Count();
			var secondCount = second.Count();
			if (firstCount != secondCount) return false;

			foreach (var x1 in first)
			{
				if (!second.Contains(x1)) return false;
			}

			return true;
		}

		/// <summary>
		/// Is Keys in MyDictionary is the same as some collection
		/// </summary>
		public static bool ContentsMatchKeys<T1, T2>(this IDictionary<T1, T2> source, IEnumerable<T1> check)
		{
			if (source.IsNullOrEmpty() && check.IsNullOrEmpty()) return true;
			if (source.IsNullOrEmpty() || check.IsNullOrEmpty()) return false;

			return source.Keys.ContentsMatch(check);
		}

		/// <summary>
		/// Is Values in MyDictionary is the same as some collection
		/// </summary>
		public static bool ContentsMatchValues<T1, T2>(this IDictionary<T1, T2> source, IEnumerable<T2> check)
		{
			if (source.IsNullOrEmpty() && check.IsNullOrEmpty()) return true;
			if (source.IsNullOrEmpty() || check.IsNullOrEmpty()) return false;

			return source.Values.ContentsMatch(check);
		}

		/// <summary>
		/// Gets the value associated with the specified key if it exists, or
		/// return the default value for the value type if it doesn't.
		/// </summary>
		public static TValue GetOrDefault<TKey, TValue>(
			this IDictionary<TKey, TValue> source,
			TKey key,
			TValue customDefault = default(TValue))
		{
			if (!source.ContainsKey(key)) source[key] = customDefault;
			return source[key];
		}

		/// <summary>
		/// Gets the value associated with the specified key if it exists, or
		/// generate a value for the new key if it doesn't.
		/// </summary>
		public static TValue GetOrDefault<TKey, TValue>(
			this IDictionary<TKey, TValue> source,
			TKey key,
			System.Func<TValue> customDefaultGenerator)
		{
			if (!source.ContainsKey(key)) source[key] = customDefaultGenerator();
			return source[key];
		}

		/// <summary>
		/// Performs an action on each element of a collection.
		/// </summary>
		public static IEnumerable<T> ForEach<T>(this IEnumerable<T> source, System.Action<T> action)
		{
			foreach (T element in source) action(element);
			return source;
		}

		/// <summary>
		/// Performs a function on each element of a collection.
		/// </summary>
		public static IEnumerable<T> ForEach<T, R>(this IEnumerable<T> source, Func<T, R> func)
		{
			foreach (T element in source) func(element);
			return source;
		}

		/// <summary>
		/// Performs an action on each element of a collection with its index
		/// passed along.
		/// </summary>
		public static IEnumerable<T> ForEach<T>(this IEnumerable<T> source,
			System.Action<T, int> action)
		{
			int index = 0;
			foreach (T element in source) { action(element, index); ++index; }
			return source;
		}

		/// <summary>
		/// Performs an action on each element of a collection with its index
		/// passed along.
		/// </summary>
		public static IEnumerable<T> ForEach<T, R>(this IEnumerable<T> source,
			Func<T, int, R> func)
		{
			int index = 0;
			foreach (T element in source) { func(element, index); ++index; }
			return source;
		}

		/// <summary>
		/// Find the element of a collection that has the highest selected value.
		/// </summary>
		public static T MaxBy<T, S>(this IEnumerable<T> source, Func<T, S> selector)
			where S : IComparable<S>
		{
			if (source.IsNullOrEmpty())
			{
				Debug.LogError("MaxBy Caused: source collection is null or empty");
				return default(T);
			}
			return source.Aggregate((e, n) => selector(e).CompareTo(selector(n)) > 0 ? e : n);
		}

		/// <summary>
		/// Find the element of a collection that has the lowest selected value.
		/// </summary>
		public static T MinBy<T, S>(this IEnumerable<T> source, Func<T, S> selector)
			where S : IComparable<S>
		{
			if (source.IsNullOrEmpty())
			{
				Debug.LogError("MinBy Caused: source collection is null or empty");
				return default(T);
			}
			return source.Aggregate((e, n) => selector(e).CompareTo(selector(n)) < 0 ? e : n);
		}

		/// <summary>
		/// Convert a single element into an enumerable with the source as the
		/// single element.
		/// </summary>
		public static IEnumerable<T> AsEnumerable<T>(this T source) =>
			Enumerable.Empty<T>().Append(source);

		/// <summary>
		/// First index of an item that matches a predicate.
		/// </summary>
		public static int FirstIndex<T>(this IList<T> source, Predicate<T> predicate)
		{
			for (int i = 0; i < source.Count; ++i)
			{
				if (predicate(source[i])) return i;
			}
			return -1;
		}

		/// <summary>
		/// First index of an item that matches a predicate.
		/// </summary>
		public static int FirstIndex<T>(this IEnumerable<T> source, Predicate<T> predicate)
		{
			int index = 0;
			foreach (T e in source)
			{
				if (predicate(e)) return index;
				++index;
			}
			return -1;
		}

		/// <summary>
		/// Last index of an item that matches a predicate.
		/// </summary>
		public static int LastIndex<T>(this IList<T> source, Predicate<T> predicate)
		{
			for (int i = source.Count - 1; i >= 0; --i)
			{
				if (predicate(source[i])) return i;
			}
			return -1;
		}

		/// <summary>
		/// Projects each element of a sequence into a new form, with its index
		/// passed along the selector.
		/// </summary>
		public static IEnumerable<R> SelectWithIndex<T, R>(this IEnumerable<T> source,
			Func<T, int, R> selector)
		{
			int index = 0;
			return source.Select(e =>
			{
				var result = selector(e, index);
				++index;
				return result;
			});
		}

		/// <summary>
		/// Projects each element of a sequence to an IEnumerable<T> and flattens
		/// the resulting sequences into one sequence, with each element's index
		/// passed along the selector.
		/// </summary>
		public static IEnumerable<R> SelectManyWithIndex<T, R>(this IEnumerable<T> source,
			Func<T, int, IEnumerable<R>> selector)
		{
			int index = 0;
			return source.SelectMany(e =>
			{
				var result = selector(e, index);
				++index;
				return result;
			});
		}

		/// <summary>
		/// Returns random index from collection with weighted probabilities.
		/// </summary>
		public static int GetWeightedRandomIndex<T>(this IEnumerable<T> source,
			Func<T, double> weightSelector)
		{
			var weights = source.Select(weightSelector).Select(w => w < 0 ? 0 : w);
			var weightStages = weights
				.SelectWithIndex((w, i) => weights.Take(i + 1).Sum());
			var roll = MyCommonConstants.SystemRandom.NextDouble() * weights.Sum();
			return weightStages.FirstIndex(ws => ws > roll);
		}

		/// <summary>
		/// Returns random element from collection with weighted probabilities.
		/// </summary>
		public static T GetWeightedRandom<T>(this IList<T> source,
			Func<T, double> weightSelector) =>
			source[source.GetWeightedRandomIndex(weightSelector)];

		/// <summary>
		/// Returns random element from collection with weighted probabilities.
		/// </summary>
		public static T GetWeightedRandom<T>(this IEnumerable<T> source,
			Func<T, double> weightSelector) =>
			source.ElementAt(source.GetWeightedRandomIndex(weightSelector));
	}
}