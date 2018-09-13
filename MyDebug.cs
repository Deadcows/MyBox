using System.Text;
using UnityEngine;

public static class MyDebug
{

	public static void LogArray<T>(T[] toLog)
	{
		StringBuilder sb = new StringBuilder();
		sb.Append("Log Array: ");
		sb.Append(typeof (T).Name + " (" + toLog.Length + ")\n");
		for (var i = 0; i < toLog.Length; i++)
		{
			sb.Append("\n" + i + ": " + toLog[i]);
		}
		Debug.Log(sb.ToString());
	}

	
	public static void DrawDebugBounds(MeshFilter mesh, Color color)
	{
		if (mesh == null) return;
		var renderer = mesh.GetComponent<MeshRenderer>();
		DrawDebugBounds(renderer, color);
	}
	
	public static void DrawDebugBounds(MeshRenderer renderer, Color color)
	{
		var bounds = renderer.bounds;
		DrawDebugBounds(bounds, color);
	}
	
	public static void DrawDebugBounds(Bounds bounds, Color color)
	{
		Vector3 v3Center = bounds.center;
		Vector3 v3Extents = bounds.extents;

		var v3FrontTopLeft = new Vector3(v3Center.x - v3Extents.x, v3Center.y + v3Extents.y, v3Center.z - v3Extents.z);  // Front top left corner
		var v3FrontTopRight = new Vector3(v3Center.x + v3Extents.x, v3Center.y + v3Extents.y, v3Center.z - v3Extents.z);  // Front top right corner
		var v3FrontBottomLeft = new Vector3(v3Center.x - v3Extents.x, v3Center.y - v3Extents.y, v3Center.z - v3Extents.z);  // Front bottom left corner
		var v3FrontBottomRight = new Vector3(v3Center.x + v3Extents.x, v3Center.y - v3Extents.y, v3Center.z - v3Extents.z);  // Front bottom right corner
		var v3BackTopLeft = new Vector3(v3Center.x - v3Extents.x, v3Center.y + v3Extents.y, v3Center.z + v3Extents.z);  // Back top left corner
		var v3BackTopRight = new Vector3(v3Center.x + v3Extents.x, v3Center.y + v3Extents.y, v3Center.z + v3Extents.z);  // Back top right corner
		var v3BackBottomLeft = new Vector3(v3Center.x - v3Extents.x, v3Center.y - v3Extents.y, v3Center.z + v3Extents.z);  // Back bottom left corner
		var v3BackBottomRight = new Vector3(v3Center.x + v3Extents.x, v3Center.y - v3Extents.y, v3Center.z + v3Extents.z);  // Back bottom right corner

		Debug.DrawLine(v3FrontTopLeft, v3FrontTopRight, color);
		Debug.DrawLine(v3FrontTopRight, v3FrontBottomRight, color);
		Debug.DrawLine(v3FrontBottomRight, v3FrontBottomLeft, color);
		Debug.DrawLine(v3FrontBottomLeft, v3FrontTopLeft, color);

		Debug.DrawLine(v3BackTopLeft, v3BackTopRight, color);
		Debug.DrawLine(v3BackTopRight, v3BackBottomRight, color);
		Debug.DrawLine(v3BackBottomRight, v3BackBottomLeft, color);
		Debug.DrawLine(v3BackBottomLeft, v3BackTopLeft, color);

		Debug.DrawLine(v3FrontTopLeft, v3BackTopLeft, color);
		Debug.DrawLine(v3FrontTopRight, v3BackTopRight, color);
		Debug.DrawLine(v3FrontBottomRight, v3BackBottomRight, color);
		Debug.DrawLine(v3FrontBottomLeft, v3BackBottomLeft, color);
	}
	
}