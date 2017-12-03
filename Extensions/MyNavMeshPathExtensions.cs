using UnityEngine;
using UnityEngine.AI;

public static class MyNavMeshPathExtensions
{

	/// <summary>
	/// Get length of path (combining all corners)
	/// </summary>
	/// <param name="path">Path to calculate</param>
	/// <returns>Lenght in Units</returns>
	public static float GetLength(this NavMeshPath path)
	{
		var corners = path.corners;
		float length = 0;
		for (var i = 1; i < corners.Length; i++)
		{
			length += Vector3.Distance(corners[i - 1], corners[i]);
		}
		return length;
	}

	/// <summary>
	/// Roughly calculate time to go all the path with given speed
	/// </summary>
	/// <param name="path">Path to calculate</param>
	/// <param name="speed">Speed of the agent</param>
	/// <returns>Time in seconds</returns>
	public static float GetTimeToPass(this NavMeshPath path, float speed)
	{
		var length = path.GetLength();
		float time = length / speed;
		time += path.corners.Length * .5f; // slowdown on corners offset
		return time;
	}

}
