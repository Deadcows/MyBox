using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public static class Extensions
{

	public static T RandomElement<T>(this List<T> elements)
	{
		return elements[Random.Range(0, elements.Count - 1)];
	}

	public static bool IsNullOrEmpty<T>(this IEnumerable<T> sequence)
	{
		if (sequence == null) return true;

		return !sequence.Any();
	}

	public static void Swap<T>(ref T a, ref T b)
	{
		T x = a;
		a = b;
		b = x;
	}

	public static int ValidateIndex<T>(this T[] array, int desiredIndex)
	{
		if (array.Length == 0) return 0;
		if (desiredIndex < 0) return array.Length - 1;
		if (desiredIndex > array.Length - 1) return 0;
		return desiredIndex;
	}



	// Toggle layers lock

	//Tools.lockedLayers = 1 << LayerMask.NameToLayer("LayerName"); // Replace the whole value of lockedLayers. 
	//Tools.lockedLayers |= 1 << LayerMask.NameToLayer("LayerName"); // Add a value to lockedLayers. 
	//Tools.lockedLayers &= ~(1 << LayerMask.NameToLayer("LayerName")); // Remove a value from lockedLayers. 
	//Tools.lockedLayers ^= 1 << LayerMask.NameToLayer("LayerName")); // Toggle a value in lockedLayers.

	/// <summary>
	/// Clamp value to less than min or more than max
	/// </summary>
	public static float NotInRange(this float num, float min, float max)
	{
		if (min > max)
		{
			var x = min;
			min = max;
			max = x;
		}
		if (num < min || num > max) return num;

		float mid = (max - min) / 2;

		if (num > min) return num + mid < max ? min : max;
		return num - mid > min ? max : min;
	}

	/// <summary>
	/// Clamp value to less than min or more than max
	/// </summary>
	public static int NotInRange(this int num, int min, int max)
	{
		return (int)((float)num).NotInRange(min, max);
	}

	/// <summary>
	/// Return point that is closer to num
	/// </summary>
	public static float ClosestPoint(this float num, float pointA, float pointB)
	{
		if (pointA > pointB)
		{
			var x = pointA;
			pointA = pointB;
			pointB = x;
		}

		float middle = (pointB - pointA) / 2;
		float withOffset = num.NotInRange(pointA, pointB) + middle;
		return (withOffset >= pointB) ? pointB : pointA;
	}

	/// <summary>
	/// Check if pointA closer to num than pointB
	/// </summary>
	public static bool ClosestPointIsA(this float num, float pointA, float pointB)
	{
		var closestPoint = num.ClosestPoint(pointA, pointB);
		return Mathf.Approximately(closestPoint, pointA);
	}


	public static bool LayerInMask(this LayerMask mask, int layer)
	{
		return (((1 << layer) & mask) != 0);
	}

	/// <summary>
	/// Removes multiple hits for single gameObject instances
	/// </summary>
	public static RaycastHit2D[] OneHitPerInstance(this RaycastHit2D[] hits)
	{
		return hits?.GroupBy(h => h.transform.GetInstanceID()).Select(g => g.First()).ToArray();
	}

	/// <summary>
	/// Removes multiple hits for single gameObject instances
	/// </summary>
	public static Collider2D[] OneHitPerInstance(this Collider2D[] hits)
	{
		return hits?.GroupBy(h => h.transform.GetInstanceID()).Select(g => g.First()).ToArray();
	}

	/// <summary>
	/// Removes multiple hits for single gameObject instances
	/// </summary>
	public static List<Collider2D> OneHitPerInstanceList(this Collider2D[] hits)
	{
		return hits?.GroupBy(h => h.transform.GetInstanceID()).Select(g => g.First()).ToList();
	}

    public static void SetBodyState(this Rigidbody body, bool state)
    {
        body.isKinematic = !state;
        body.detectCollisions = state;
    }
}