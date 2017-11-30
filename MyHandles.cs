using UnityEditor;
using UnityEngine;

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
}
