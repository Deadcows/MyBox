using UnityEngine;

public static class MyVectorsExtensions
{
	#region Set X/Y/Z

	public static Vector3 SetX(this Vector3 vector, float x)
	{
		return new Vector3(x, vector.y, vector.z);
	}

	public static Vector2 SetX(this Vector2 vector, float x)
	{
		return new Vector2(x, vector.y);
	}

	public static void SetX(this Transform transform, float x)
	{
		transform.position = transform.position.SetX(x);
	}


	public static Vector3 SetY(this Vector3 vector, float y)
	{
		return new Vector3(vector.x, y, vector.z);
	}

	public static Vector2 SetY(this Vector2 vector, float y)
	{
		return new Vector2(vector.x, y);
	}

	public static void SetY(this Transform transform, float y)
	{
		transform.position = transform.position.SetY(y);
	}


	public static Vector3 SetZ(this Vector3 vector, float z)
	{
		return new Vector3(vector.x, vector.y, z);
	}

	public static void SetZ(this Transform transform, float z)
	{
		transform.position = transform.position.SetZ(z);
	}


	public static Vector3 SetXY(this Vector3 vector, float x, float y)
	{
		return new Vector3(x, y, vector.z);
	}

	public static void SetXY(this Transform transform, float x, float y)
	{
		transform.position = transform.position.SetXY(x, y);
	}


	public static Vector3 SetXZ(this Vector3 vector, float x, float z)
	{
		return new Vector3(x, vector.y, z);
	}

	public static void SetXZ(this Transform transform, float x, float z)
	{
		transform.position = transform.position.SetXZ(x, z);
	}


	public static Vector3 SetYZ(this Vector3 vector, float y, float z)
	{
		return new Vector3(vector.x, y, z);
	}

	public static void SetYZ(this Transform transform, float y, float z)
	{
		transform.position = transform.position.SetYZ(y, z);
	}

	#endregion


	#region Offset X/Y/Z
	
	public static Vector3 Offset(this Vector3 vector, Vector2 offset)
	{
		return new Vector3(vector.x + offset.x, vector.y + offset.y, vector.z);
	}
	

	public static Vector3 OffsetX(this Vector3 vector, float x)
	{
		return new Vector3(vector.x + x, vector.y, vector.z);
	}

	public static Vector2 OffsetX(this Vector2 vector, float x)
	{
		return new Vector2(vector.x + x, vector.y);
	}

	public static void OffsetX(this Transform transform, float x)
	{
		transform.position = transform.position.OffsetX(x);
	}


	public static Vector2 OffsetY(this Vector2 vector, float y)
	{
		return new Vector2(vector.x, vector.y + y);
	}

	public static Vector3 OffsetY(this Vector3 vector, float y)
	{
		return new Vector3(vector.x, vector.y + y, vector.z);
	}

	public static void OffsetY(this Transform transform, float y)
	{
		transform.position = transform.position.OffsetY(y);
	}


	public static Vector3 OffsetZ(this Vector3 vector, float z)
	{
		return new Vector3(vector.x, vector.y, vector.z + z);
	}

	public static void OffsetZ(this Transform transform, float z)
	{
		transform.position = transform.position.OffsetZ(z);
	}


	public static Vector3 OffsetXY(this Vector3 vector, float x, float y)
	{
		return new Vector3(vector.x + x, vector.y + y, vector.z);
	}

	public static void OffsetXY(this Transform transform, float x, float y)
	{
		transform.position = transform.position.OffsetXY(x, y);
	}

	public static Vector2 OffsetXY(this Vector2 vector, float x, float y)
	{
		return new Vector2(vector.x + x, vector.y + y);
	}


	public static Vector3 OffsetXZ(this Vector3 vector, float x, float z)
	{
		return new Vector3(vector.x + x, vector.y, vector.z + z);
	}

	public static void OffsetXZ(this Transform transform, float x, float z)
	{
		transform.position = transform.position.OffsetXZ(x, z);
	}


	public static Vector3 OffsetYZ(this Vector3 vector, float y, float z)
	{
		return new Vector3(vector.x, vector.y + y, vector.z + z);
	}

	public static void OffsetYZ(this Transform transform, float y, float z)
	{
		transform.position = transform.position.OffsetYZ(y, z);
	}

	#endregion


	#region Clamp X/Y

	public static Vector3 ClampX(this Vector3 vector, float min, float max)
	{
		return vector.SetX(Mathf.Clamp(vector.x, min, max));
	}

	public static Vector2 ClampX(this Vector2 vector, float min, float max)
	{
		return vector.SetX(Mathf.Clamp(vector.x, min, max));
	}

	public static void ClampX(this Transform transform, float min, float max)
	{
		transform.SetX(Mathf.Clamp(transform.position.x, min, max));
	}


	public static Vector3 ClampY(this Vector3 vector, float min, float max)
	{
		return vector.SetY(Mathf.Clamp(vector.x, min, max));
	}

	public static Vector2 ClampY(this Vector2 vector, float min, float max)
	{
		return vector.SetY(Mathf.Clamp(vector.x, min, max));
	}

	public static void ClampY(this Transform transform, float min, float max)
	{
		transform.SetY(Mathf.Clamp(transform.position.x, min, max));
	}

	#endregion


	#region Invert

	public static Vector2 InvertX(this Vector2 vector)
	{
		return new Vector2(-vector.x, vector.y);
	}

	public static Vector2 InvertY(this Vector2 vector)
	{
		return new Vector2(vector.x, -vector.y);
	}

	#endregion


	#region Convert

	public static Vector2 ToVector2(this Vector3 vector)
	{
		return new Vector2(vector.x, vector.y);
	}

	public static Vector3 ToVector3(this Vector2 vector)
	{
		return new Vector3(vector.x, vector.y);
	}

	#endregion


	#region Snap

	/// <summary>
	/// Snap to grid of snapValue
	/// </summary>
	public static Vector3 SnapValue(this Vector3 val, float snapValue)
	{
		return new Vector3(
			MyMath.Snap(val.x, snapValue),
			MyMath.Snap(val.y, snapValue),
			MyMath.Snap(val.z, snapValue));
	}

	/// <summary>
	/// Snap to grid of snapValue
	/// </summary>
	public static Vector2 SnapValue(this Vector2 val, float snapValue)
	{
		return new Vector2(
			MyMath.Snap(val.x, snapValue),
			MyMath.Snap(val.y, snapValue));
	}

	/// <summary>
	/// Snap position to grid of snapValue
	/// </summary>
	public static void SnapPosition(this Transform transform, float snapValue)
	{
		transform.position = transform.position.SnapValue(snapValue);
	}

	/// <summary>
	/// Snap to one unit grid
	/// </summary>
	public static Vector2 SnapToOne(this Vector2 vector)
	{
		return new Vector2(Mathf.Round(vector.x), Mathf.Round(vector.y));
	}

	/// <summary>
	/// Snap to one unit grid
	/// </summary>
	public static Vector3 SnapToOne(this Vector3 vector)
	{
		return new Vector3(Mathf.Round(vector.x), Mathf.Round(vector.y), Mathf.Round(vector.z));
	}

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
}