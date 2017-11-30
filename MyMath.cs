using System;
using UnityEngine;

public static class MyMath
{
	
	public static float Snap(float val, float round)
	{
		return round * Mathf.Round(val / round);
	}

	public static bool Approximately(this float value, float compare)
	{
		return Mathf.Approximately(value, compare);
	}

	public static int SafeLength(this Array array)
	{
		if (array == null) return 0;
		return array.Length;
	}
}
