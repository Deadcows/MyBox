using System;
using UnityEngine;
using UnityQuery;

public static class MyMath
{
	
	public static Vector3 SnapValue(Vector3 val, float snapValue)
	{
		return new Vector3(
			Snap(val.x, snapValue), 
			Snap(val.y, snapValue), 
			Snap(val.z, snapValue));
	}

	public static float Snap(float val, float round)
	{
		return round * Mathf.Round(val / round);
	}

	public static bool Approximately(this float value, float compare)
	{
		return Mathf.Approximately(value, compare);
	}

	public static Vector3 AverageVector(this Vector3[] vectors)
	{
		if (vectors.IsNullOrEmpty()) return Vector3.zero;

		float x = 0f;
		float y = 0f;
		float z = 0f;
		for (var i = 0; i < vectors.Length; i++)
		{
			x += vectors[i].x;
			y += vectors[i].y;
			z += vectors[i].z;
		}
		return new Vector3(x / vectors.Length, y / vectors.Length, z / vectors.Length);
	}

	public static int SafeLength(this Array array)
	{
		if (array == null) return 0;
		return array.Length;
	}
	
}
