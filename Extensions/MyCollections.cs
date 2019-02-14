using UnityEngine;
using System.Linq;
using Object = UnityEngine.Object;

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

	public static I[] FindObjectsOfInterface<I>() where I : class
	{
		var monoBehaviours = Object.FindObjectsOfType<Transform>();

		return monoBehaviours.Select(behaviour => behaviour.GetComponent(typeof(I))).OfType<I>().ToArray();
	}

	public static ComponentOfInterface<I>[] FindObjectsOfInterfaceAsComponents<I>() where I : class
	{
		return Object.FindObjectsOfType<Component>()
			.Where(c => c is I)
			.Select(c => new ComponentOfInterface<I>(c, c as I)).ToArray();
	}
	
	public struct ComponentOfInterface<I>
	{
		public readonly Component Component;
		public readonly I Interface;

		public ComponentOfInterface(Component component, I @interface)
		{
			Component = component;
			Interface = @interface;
		}
	}
}