using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace MyBox
{
	public static class MyCollections
	{
		/// <summary>
		/// Returns new array without element at index
		/// </summary>
		public static T[] RemoveAt<T>(this T[] array, int index)
		{
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
		/// Get random element in collection
		/// </summary>
		public static T GetRandom<T>(this IList<T> collection)
		{
			return collection[Random.Range(0, collection.Count - 1)];
		}

		/// <summary>
		/// Get random element in collection
		/// </summary>
		public static T GetRandom<T>(this T[] collection)
		{
			return collection[Random.Range(0, collection.Length - 1)];
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
				var random = Random.Range(0, indexes.Count);
				randoms[i] = collection[random];
				indexes.RemoveAt(random);
			}

			return randoms;
		}
		

		/// <summary>
		/// Is collection null or empty
		/// </summary>
		public static bool IsNullOrEmpty<T>(this IEnumerable<T> collection)
		{
			if (collection == null) return true;

			return !collection.Any();
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
		/// Get fixed index for looping sequences. i.e. -1 will result with last element index
		///
		/// Example (infinite loop first->last->first):
		/// i = myArray.NextIndex(i++);
		/// var nextItem = myArray[i];
		/// </summary>
		public static int NextIndex<T>(this T[] array, int desiredPosition)
		{
			if (array.Length == 0) return 0;
			if (desiredPosition < 0) return array.Length - 1;
			if (desiredPosition > array.Length - 1) return 0;
			return desiredPosition;
		}

		/// <returns>
		/// Returns -1 if none found
		/// </returns>
		public static int IndexOfItem<T>(this IEnumerable<T> items, T item)
		{
			var index = 0;

			foreach (var i in items)
			{
				if (Equals(i, item)) return index;
				++index;
			}

			return -1;
		}
	}
}