using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

public static class MyAI
{

	public static void DrawLines(this NavMeshPath path, float screenSpaceSize = 3)
	{
		var coreners = path.corners;
		for (var i = 1; i < coreners.Length; i++)
		{
			Handles.DrawDottedLine(coreners[i - 1], coreners[i], screenSpaceSize);
		}
	}

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

	public static float GetTimeToPass(this NavMeshPath path, float speed)
	{
		var length = path.GetLength();
		float time = length / speed;
		time += path.corners.Length * .5f; // slowdown on corners offset
		return time;
	}

}
