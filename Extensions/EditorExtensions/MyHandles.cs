#if UNITY_EDITOR
using JetBrains.Annotations;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

namespace MyBox.EditorTools
{
	[PublicAPI]
	public static class MyHandles
	{
		/// <summary>
		/// Draw BoxCollider
		/// </summary>
		public static void DrawBox(BoxCollider box, Color color, float thickness = 1f)
		{
			if (box == null) return;
			Handles.color = color;
			Handles.matrix = Matrix4x4.TRS(box.transform.position, box.transform.rotation, box.transform.lossyScale);
			Handles.DrawAAPolyLine(5, GetWireCubeVertices(box.center, box.size));
			Handles.matrix = Matrix4x4.identity;
			
			Vector3[] GetWireCubeVertices(Vector3 center, Vector3 size)
			{
				var half = size * 0.5f;
				var leftFarBottom = center + new Vector3(-half.x, -half.y, -half.z); 
				var rightFarBottom = center + new Vector3(half.x, -half.y, -half.z);
				var leftFarTop = center + new Vector3(-half.x, half.y, -half.z);
				var rightFarTop = center + new Vector3(half.x, half.y, -half.z);
				var leftNearBottom = center + new Vector3(-half.x, -half.y, half.z);
				var rightNearBottom = center + new Vector3(half.x, -half.y, half.z);
				var leftNearTop = center + new Vector3(-half.x, half.y, half.z);
				var rightNearTop = center + new Vector3(half.x, half.y, half.z);
				
				return new[]
				{
					leftFarBottom, rightFarBottom, rightFarTop, leftFarTop, leftFarBottom,
					leftNearBottom, rightNearBottom, rightNearTop, leftNearTop, leftNearBottom,
					rightNearBottom, rightFarBottom, rightFarTop, rightNearTop, 
					leftNearTop, leftFarTop
				};
			}
		}
		
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

#if UNITY_AI_ENABLED
        
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
        
#endif

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