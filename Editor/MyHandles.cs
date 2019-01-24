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
	/// Draw arrowed gizmo in scene view to visualize path
	/// </summary>
	/// <param name="path">Path to visualize</param>
	/// <param name="screenSpaceSize">Size of dotted line</param>
	public static void VisualizePath(NavMeshPath path, float screenSpaceSize = 3)
	{
		var corners = path.corners;
		for (var i = 1; i < corners.Length; i++)
		{
			DrawDirectionalDottedLine(corners[i - 1], corners[i], screenSpaceSize);
		}
	}

	public static void DrawFlyPath(Vector3 pointA, Vector3 pointB, float height = 3)
	{
		var color = Handles.color;
		var pointAOffset = new Vector3(pointA.x, pointA.y + height, pointA.z);
		var pointBOffset = new Vector3(pointB.x, pointB.y + height, pointB.z);
		Handles.DrawBezier(pointA, pointB, pointAOffset, pointBOffset, color, null, 3);
	}

}
