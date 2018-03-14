using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

public static class MyHandles
{

	public static void DrawDirectionalDottedLine(Vector3 fromPos, Vector3 toPos, float screenSpaceSize = 3, float arrowDensity = .5f)
	{
		Handles.DrawDottedLine(fromPos, toPos, screenSpaceSize);

		var direction = toPos - fromPos;
		var distance = Vector3.Distance(fromPos, toPos);
		var arrowsCount = (int)(distance / arrowDensity);
		var delta = 1f / arrowsCount;
		for (int i = 1; i <= arrowsCount; i++)
		{
			var currentDelta = delta * i;
			var currentPosition = Vector3.Lerp(fromPos, toPos, currentDelta);
			DrawTinyArrow(currentPosition, direction);
		}
	}

	public static void DrawTinyArrow(Vector3 position, Vector3 direction, float headLength = 0.25f, float headAngle = 20.0f)
	{
		Vector3 right = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 180 + headAngle, 0) * new Vector3(0, 0, 1);
		Vector3 left = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 180 - headAngle, 0) * new Vector3(0, 0, 1);
		Handles.DrawLine(position, position + right * headLength);
		Handles.DrawLine(position, position + left * headLength);
	}

	/// <summary>
	/// Draw arrowed gizmo in scene view to vizualize path
	/// </summary>
	/// <param name="path">Path to vizualize</param>
	/// <param name="screenSpaceSize">Size of dotted line</param>
	public static void VizualizePath(NavMeshPath path, float screenSpaceSize = 3)
	{
		var coreners = path.corners;
		for (var i = 1; i < coreners.Length; i++)
		{
			DrawDirectionalDottedLine(coreners[i - 1], coreners[i], screenSpaceSize);
		}
	}

	public static void DrawFlyPath(Vector3 pointA, Vector3 pointB, float height = 3)
	{
		var color = Handles.color;
		Handles.DrawBezier(pointA, pointB, pointA.OffsetY(height), pointB.OffsetY(height), color, null, 3);
	}

}
