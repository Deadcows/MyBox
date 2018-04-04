public static class MyCollections
{

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
}
