using UnityEngine;

namespace MyBox
{
	public static class MyGizmos
	{
		public static void DrawArrow(Vector3 from, Vector3 direction, float headLength = 0.25f, float headAngle = 20.0f)
		{
#if UNITY_EDITOR
			Gizmos.DrawRay(from, direction);

			Vector3 right = Quaternion.LookRotation(direction) * Quaternion.Euler(180 + headAngle, 0, 180 + headAngle) * new Vector3(0, 0, 1);
			Vector3 left = Quaternion.LookRotation(direction) * Quaternion.Euler(180 - headAngle, 0, 180 - headAngle) * new Vector3(0, 0, 1);
			Gizmos.DrawRay(from + direction, right * headLength);
			Gizmos.DrawRay(from + direction, left * headLength);
#endif
		}

		public static void DrawSegment(Vector3 from, Vector3 to, float capSize = 0.1f)
		{
			Gizmos.DrawLine(from, to);
			Gizmos.DrawLine(from.OffsetY(-capSize), from.OffsetY(capSize));
			Gizmos.DrawLine(to.OffsetY(-capSize), to.OffsetY(capSize));
		}

#if UNITY_PHYSICS2D_ENABLED
		public static void DrawBoxCollider2D(BoxCollider2D collider, bool fill = true)
		{
			var target = collider.transform;
			
			Gizmos.matrix = Matrix4x4.TRS(target.position, target.rotation, target.lossyScale);
			if (fill) Gizmos.DrawCube(collider.offset, collider.size);
			else Gizmos.DrawWireCube(collider.offset, collider.size);
			Gizmos.matrix = Matrix4x4.identity;
		}
#endif
	}
}