#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

namespace MyBox.EditorTools
{
	public static class MyHandles
	{
		/// <summary>
		/// Draw line with arrows showing direction
		/// </summary>
		public static void DrawDirectionalLine(Vector3 fromPos, Vector3 toPos, float screenSpaceSize = 3, float arrowsDensity = .5f)
		{
			var arrowSize = screenSpaceSize / 4;

			Handles.DrawLine(fromPos, toPos);

			var direction = toPos - fromPos;
			var distance = Vector3.Distance(fromPos, toPos);
			var arrowsCount = (int) (distance / arrowsDensity);
			var delta = 1f / arrowsCount;
			for (int i = 1; i <= arrowsCount; i++)
			{
				var currentDelta = delta * i;
				var currentPosition = Vector3.Lerp(fromPos, toPos, currentDelta);
				DrawTinyArrow(currentPosition, direction, arrowSize);
			}
		}

		/// <summary>
		/// Draw arrow in position to direction
		/// </summary>
		public static void DrawTinyArrow(Vector3 position, Vector3 direction, float headLength = 0.25f, float headAngle = 20.0f)
		{
			var lookRotation = Quaternion.LookRotation(direction);
			var rightVector = new Vector3(0, 0, 1);
			Vector3 right = lookRotation * Quaternion.Euler(0, 180 + headAngle, 0) * rightVector;
			Vector3 left = lookRotation * Quaternion.Euler(0, 180 - headAngle, 0) * rightVector;
			Handles.DrawLine(position, position + right * headLength);
			Handles.DrawLine(position, position + left * headLength);
		}


		/// <summary>
		/// Draw arrowed gizmo in scene view to visualize path
		/// </summary>
		public static void VisualizePath(NavMeshPath path)
		{
			var corners = path.corners;
			for (var i = 1; i < corners.Length; i++)
			{
				var cornerA = corners[i - 1];
				var cornerB = corners[i];
				var size = HandleUtility.GetHandleSize(new Vector3(cornerA.x, cornerA.y, cornerA.z));
				DrawDirectionalLine(cornerA, cornerB, size, size);
			}
		}

		/// <summary>
		/// Draw flying path of height prom pointA to pointB
		/// </summary>
		public static void DrawFlyPath(Vector3 pointA, Vector3 pointB, float height = 3)
		{
			var color = Handles.color;
			var pointAOffset = new Vector3(pointA.x, pointA.y + height, pointA.z);
			var pointBOffset = new Vector3(pointB.x, pointB.y + height, pointB.z);
			Handles.DrawBezier(pointA, pointB, pointAOffset, pointBOffset, color, null, 3);
		}
	}
}
#endif