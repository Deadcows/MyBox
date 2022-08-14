using System.Collections.Generic;
using UnityEngine;
// ReSharper disable MemberCanBePrivate.Global

namespace MyBox
{
	public static class MyVectors
	{
		#region Set X/Y/Z

		// Set X

		public static Vector3 SetX(this Vector3 vector, float x) => new Vector3(x, vector.y, vector.z);

		public static Vector2 SetX(this Vector2 vector, float x) => new Vector2(x, vector.y);

		public static void SetX(this Transform transform, float x) => transform.position = transform.position.SetX(x);

		// Set Y

		public static Vector3 SetY(this Vector3 vector, float y) => new Vector3(vector.x, y, vector.z);
		
		public static Vector2 SetY(this Vector2 vector, float y) => new Vector2(vector.x, y);

		public static void SetY(this Transform transform, float y) => transform.position = transform.position.SetY(y);

		// Set Z

		public static Vector3 SetZ(this Vector3 vector, float z) => new Vector3(vector.x, vector.y, z);

		public static void SetZ(this Transform transform, float z) => transform.position = transform.position.SetZ(z);

		// Set XY

		public static Vector3 SetXY(this Vector3 vector, float x, float y) => new Vector3(x, y, vector.z);

		public static void SetXY(this Transform transform, float x, float y) => transform.position = transform.position.SetXY(x, y);

		// Set XZ

		public static Vector3 SetXZ(this Vector3 vector, float x, float z) => new Vector3(x, vector.y, z);

		public static void SetXZ(this Transform transform, float x, float z) => transform.position = transform.position.SetXZ(x, z);

		// Set YZ

		public static Vector3 SetYZ(this Vector3 vector, float y, float z) => new Vector3(vector.x, y, z);

		public static void SetYZ(this Transform transform, float y, float z) => transform.position = transform.position.SetYZ(y, z);
		
		#endregion


		#region Offset X/Y/Z

		public static Vector3 Offset(this Vector3 vector, Vector2 offset) => vector.OffsetXY(offset);
		
		// Offset X
		
		public static Vector3 OffsetX(this Vector3 vector, float x) => new Vector3(vector.x + x, vector.y, vector.z);

		public static Vector2 OffsetX(this Vector2 vector, float x) => new Vector2(vector.x + x, vector.y);

		public static void OffsetX(this Transform transform, float x) => transform.position = transform.position.OffsetX(x);

		// Offset Y
		
		public static Vector3 OffsetY(this Vector3 vector, float y) => new Vector3(vector.x, vector.y + y, vector.z);

		public static Vector2 OffsetY(this Vector2 vector, float y) => new Vector2(vector.x, vector.y + y);
		
		public static void OffsetY(this Transform transform, float y) => transform.position = transform.position.OffsetY(y);

		// Offset Z

		public static Vector3 OffsetZ(this Vector3 vector, float z) => new Vector3(vector.x, vector.y, vector.z + z);

		public static void OffsetZ(this Transform transform, float z) => transform.position = transform.position.OffsetZ(z);

		// Offset XY
		
		public static Vector3 OffsetXY(this Vector3 vector, float x, float y) => new Vector3(vector.x + x, vector.y + y, vector.z);
		
		public static Vector3 OffsetXY(this Vector3 vector, Vector2 offset) => vector.OffsetXY(offset.x, offset.y);

		public static Vector2 OffsetXY(this Vector2 vector, float x, float y) => new Vector2(vector.x + x, vector.y + y);
		
		public static void OffsetXY(this Transform transform, float x, float y) => transform.position = transform.position.OffsetXY(x, y);

		// Offset XZ

		public static Vector3 OffsetXZ(this Vector3 vector, float x, float z) => new Vector3(vector.x + x, vector.y, vector.z + z);
		
		public static Vector3 OffsetXZ(this Vector3 vector, Vector2 offset) => vector.OffsetXZ(offset.x, offset.y);

		public static void OffsetXZ(this Transform transform, float x, float z) => transform.position = transform.position.OffsetXZ(x, z);


		public static Vector3 OffsetYZ(this Vector3 vector, float y, float z) => new Vector3(vector.x, vector.y + y, vector.z + z);

		public static Vector3 OffsetYZ(this Vector3 vector, Vector2 offset) => vector.OffsetYZ(offset.x, offset.y);

		public static void OffsetYZ(this Transform transform, float y, float z) => transform.position = transform.position.OffsetYZ(y, z);

		#endregion


		#region Clamp X/Y

		// Clamp X
		
		public static Vector3 ClampX(this Vector3 vector, float min, float max) => vector.SetX(Mathf.Clamp(vector.x, min, max));

		public static Vector2 ClampX(this Vector2 vector, float min, float max) => vector.SetX(Mathf.Clamp(vector.x, min, max));

		public static void ClampX(this Transform transform, float min, float max) => transform.SetX(Mathf.Clamp(transform.position.x, min, max));

		// Clamp Y

		public static Vector3 ClampY(this Vector3 vector, float min, float max) => vector.SetY(Mathf.Clamp(vector.y, min, max));

		public static Vector2 ClampY(this Vector2 vector, float min, float max) => vector.SetY(Mathf.Clamp(vector.y, min, max));

		public static void ClampY(this Transform transform, float min, float max) => transform.SetY(Mathf.Clamp(transform.position.y, min, max));

		// Clamp Z

		public static Vector3 ClampZ(this Vector3 vector, float min, float max) => vector.SetZ(Mathf.Clamp(vector.z, min, max));

		public static void ClampZ(this Transform transform, float min, float max) => transform.SetZ(Mathf.Clamp(transform.position.z, min, max));

		#endregion


		#region Invert

		public static Vector3 InvertX(this Vector3 vector) => vector.SetX(-vector.x);
		public static Vector2 InvertX(this Vector2 vector) => vector.SetX(-vector.x);
		public static void InvertX(this Transform transform) => transform.SetX(-transform.position.x);
		
		
		public static Vector3 InvertY(this Vector3 vector) => vector.SetY(-vector.y);
		public static Vector2 InvertY(this Vector2 vector) => vector.SetY(-vector.y);
		public static void InvertY(this Transform transform) => transform.SetY(-transform.position.y);
		
		public static Vector3 InvertZ(this Vector3 vector) => vector.SetZ(-vector.z);
		public static void InvertZ(this Transform transform) => transform.SetZ(-transform.position.z);
		

		#endregion


		#region Convert

		public static Vector2 ToVector2(this Vector3 vector) => new Vector2(vector.x, vector.y);

		public static Vector3 ToVector3(this Vector2 vector) => new Vector3(vector.x, vector.y);


		public static Vector2 ToVector2(this Vector2Int vector) => new Vector2(vector.x, vector.y);

		public static Vector3 ToVector3(this Vector3Int vector) => new Vector3(vector.x, vector.y, vector.z);


		public static Vector2Int ToVector2Int(this Vector2 vector) 
			=> new Vector2Int(vector.x.RoundToInt(), vector.y.RoundToInt());

		public static Vector3Int ToVector3Int(this Vector3 vector) 
			=> new Vector3Int(vector.x.RoundToInt(), vector.y.RoundToInt(), vector.z.RoundToInt());

		#endregion


		#region Snap

		/// <summary>
		/// Snap to grid of snapValue
		/// </summary>
		public static Vector3 SnapValue(this Vector3 val, float snapValue) 
			=> new Vector3(val.x.Snap(snapValue), val.y.Snap(snapValue), val.z.Snap(snapValue));

		/// <summary>
		/// Snap to grid of snapValue
		/// </summary>
		public static Vector2 SnapValue(this Vector2 val, float snapValue) 
			=> new Vector2(val.x.Snap(snapValue), val.y.Snap(snapValue));

		/// <summary>
		/// Snap position to grid of snapValue
		/// </summary>
		public static void SnapPosition(this Transform transform, float snapValue) 
			=> transform.position = transform.position.SnapValue(snapValue);

		/// <summary>
		/// Snap to one unit grid
		/// </summary>
		public static Vector2 SnapToOne(this Vector2 vector) 
			=> new Vector2(vector.x.Round(), vector.y.Round());

		/// <summary>
		/// Snap to one unit grid
		/// </summary>
		public static Vector3 SnapToOne(this Vector3 vector) 
			=> new Vector3(vector.x.Round(), vector.y.Round(), vector.z.Round());

		#endregion


		#region Average

		public static Vector3 AverageVector(this Vector3[] vectors)
		{
			if (vectors.IsNullOrEmpty()) return Vector3.zero;

			float x = 0f, y = 0f, z = 0f;
			for (var i = 0; i < vectors.Length; i++)
			{
				x += vectors[i].x;
				y += vectors[i].y;
				z += vectors[i].z;
			}

			return new Vector3(x / vectors.Length, y / vectors.Length, z / vectors.Length);
		}

		public static Vector2 AverageVector(this Vector2[] vectors)
		{
			if (vectors.IsNullOrEmpty()) return Vector2.zero;

			float x = 0f, y = 0f;
			for (var i = 0; i < vectors.Length; i++)
			{
				x += vectors[i].x;
				y += vectors[i].y;
			}

			return new Vector2(x / vectors.Length, y / vectors.Length);
		}

		#endregion


		#region Approximately

		public static bool Approximately(this Vector3 vector, Vector3 compared, float threshold = 0.1f)
		{
			var xDiff = Mathf.Abs(vector.x - compared.x);
			var yDiff = Mathf.Abs(vector.y - compared.y);
			var zDiff = Mathf.Abs(vector.z - compared.z);

			return xDiff <= threshold && yDiff <= threshold && zDiff <= threshold;
		}

		public static bool Approximately(this Vector2 vector, Vector2 compared, float threshold = 0.1f)
		{
			var xDiff = Mathf.Abs(vector.x - compared.x);
			var yDiff = Mathf.Abs(vector.y - compared.y);

			return xDiff <= threshold && yDiff <= threshold;
		}

		#endregion


		#region Get Closest

		/// <summary>
		/// Finds the position closest to the given one.
		/// </summary>
		/// <param name="position">World position.</param>
		/// <param name="otherPositions">Other world positions.</param>
		/// <returns>Closest position.</returns>
		public static Vector3 GetClosest(this Vector3 position, IEnumerable<Vector3> otherPositions)
		{
			var closest = Vector3.zero;
			var shortestDistance = Mathf.Infinity;

			foreach (var otherPosition in otherPositions)
			{
				var distance = (position - otherPosition).sqrMagnitude;

				if (distance < shortestDistance)
				{
					closest = otherPosition;
					shortestDistance = distance;
				}
			}

			return closest;
		}

		public static Vector3 GetClosest(this IEnumerable<Vector3> positions, Vector3 position) => position.GetClosest(positions);

		#endregion


		#region To

		/// <summary>
		/// Get vector from source to destination
		/// </summary>
		public static Vector4 To(this Vector4 source, Vector4 destination) => destination - source;

		/// <summary>
		/// Get vector from source to destination
		/// </summary>
		public static Vector3 To(this Vector3 source, Vector3 destination) => destination - source;

		/// <summary>
		/// Get vector from source to destination
		/// </summary>
		public static Vector2 To(this Vector2 source, Vector2 destination) => destination - source;

		/// <summary>
		/// Get vector from source to target
		/// </summary>
		public static Vector3 To(this Component source, Component target) 
			=> source.transform.position.To(target.transform.position);

		/// <summary>
		/// Get vector from source to target
		/// </summary>
		public static Vector3 To(this Component source, GameObject target) 
			=> source.transform.position.To(target.transform.position);

		/// <summary>
		/// Get vector from source to target
		/// </summary>
		public static Vector3 To(this GameObject source, Component target) 
			=> source.transform.position.To(target.transform.position);

		/// <summary>
		/// Get vector from source to target
		/// </summary>
		public static Vector3 To(this GameObject source, GameObject target) 
			=> source.transform.position.To(target.transform.position);

		/// <summary>
		/// Get vector from source to target
		/// </summary>
		public static Vector3 To(this Vector3 source, GameObject target) 
			=> source.To(target.transform.position);

		/// <summary>
		/// Get vector from source to target
		/// </summary>
		public static Vector3 To(this Vector3 source, Component target) 
			=> source.To(target.transform.position);

		/// <summary>
		/// Get vector from source to destination
		/// </summary>
		public static Vector3 To(this GameObject source, Vector3 destination) 
			=> source.transform.position.To(destination);

		/// <summary>
		/// Get vector from source to destination
		/// </summary>
		public static Vector3 To(this Component source, Vector3 destination) 
			=> source.transform.position.To(destination);

		#endregion


		#region Pow

		/// <summary>
		/// Raise each component of the source Vector2 to the specified power.
		/// </summary>
		public static Vector2 Pow(this Vector2 source, float exponent) 
			=> new Vector2(Mathf.Pow(source.x, exponent), Mathf.Pow(source.y, exponent));

		/// <summary>
		/// Raise each component of the source Vector3 to the specified power.
		/// </summary>
		public static Vector3 Pow(this Vector3 source, float exponent) 
			=> new Vector3(Mathf.Pow(source.x, exponent), Mathf.Pow(source.y, exponent), Mathf.Pow(source.z, exponent));

		/// <summary>
		/// Raise each component of the source Vector3 to the specified power.
		/// </summary>
		public static Vector4 Pow(this Vector4 source, float exponent) 
			=> new Vector4(Mathf.Pow(source.x, exponent), Mathf.Pow(source.y, exponent), Mathf.Pow(source.z, exponent), Mathf.Pow(source.w, exponent));

		#endregion


		#region ScaleBy

		/// <summary>
		/// Immutably returns the result of the source vector multiplied with
		/// another vector component-wise.
		/// </summary>
		public static Vector2 ScaleBy(this Vector2 source, Vector2 right) => Vector2.Scale(source, right);

		/// <summary>
		/// Immutably returns the result of the source vector multiplied with
		/// another vector component-wise.
		/// </summary>
		public static Vector3 ScaleBy(this Vector3 source, Vector3 right) => Vector3.Scale(source, right);

		/// <summary>
		/// Immutably returns the result of the source vector multiplied with
		/// another vector component-wise.
		/// </summary>
		public static Vector4 ScaleBy(this Vector4 source, Vector4 right) => Vector4.Scale(source, right);

		#endregion
	}
}
