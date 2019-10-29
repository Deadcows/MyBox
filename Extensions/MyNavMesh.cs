using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace MyBox
{
	public static class MyNavMesh
	{
		/// <summary>
		/// Get length of path (combining all corners)
		/// </summary>
		/// <param name="path">Path to calculate</param>
		/// <returns>Length in Units</returns>
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
		/// Roughly calculate time to traverse the path with given speed
		/// </summary>
		/// <param name="path">Path to calculate</param>
		/// <param name="speed">Speed of the agent</param>
		/// <returns>Time in seconds</returns>
		public static float GetTimeToPass(this NavMeshPath path, float speed)
		{
			var length = path.GetLength();
			float time = length / speed;
			time += (path.corners.Length - 1) * .5f; // slowdown on corners offset
			return time;
		}

		/// <summary>
		/// Get point on path
		/// </summary>
		/// <param name="path">Path to calculate</param>
		/// <param name="rate">Percent on path, from 0 to 1</param>
		public static Vector3 GetPointOnPath(this NavMeshPath path, float rate)
		{
			rate = Mathf.Clamp01(rate);
			var length = path.GetLength();
			float elapsedRate = 0;
			for (var i = 1; i < path.corners.Length; i++)
			{
				var from = path.corners[i - 1];
				var to = path.corners[i];
				var pieceLength = Vector3.Distance(from, to);
				var pieceRate = pieceLength / length;
				elapsedRate += pieceRate;

				if (rate <= elapsedRate)
				{
					var rateOffset = elapsedRate - rate;
					var rateOnPiece = 1 - rateOffset / pieceRate;
					return Vector3.Lerp(from, to, rateOnPiece);
				}
			}

			return path.corners[path.corners.Length - 1];
		}
		
		/// <summary>
		/// Split path on points with defined distance
		/// </summary>
		/// <param name="path">Path to calculate</param>
		/// <param name="distance">Distance between points on path</param>
		public static IEnumerable<Vector3> GetPointsOnPath(this NavMeshPath path, float distance = 1)
		{
			float pieceTraversedDistance = 0;
			for (var i = 1; i < path.corners.Length; i++)
			{
				var from = path.corners[i - 1];
				var to = path.corners[i];
				var pieceLength = Vector3.Distance(from, to);

				while (pieceTraversedDistance < pieceLength + distance)
				{
					var pointRatio = pieceTraversedDistance / pieceLength;
					yield return Vector3.Lerp(from, to, pointRatio);
					pieceTraversedDistance += distance;
				}
				pieceTraversedDistance -= pieceLength;
			}
		}
	}
}