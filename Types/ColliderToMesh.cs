using System.Collections.Generic;
using UnityEngine;

namespace MyBox
{
	[RequireComponent(typeof(PolygonCollider2D))]
	[RequireComponent(typeof(MeshFilter))]
	[RequireComponent(typeof(MeshRenderer))]
	public class ColliderToMesh : MonoBehaviour
	{
		private void Start()
		{
			FillShape();
		}


		public void FillShape()
		{
			PolygonCollider2D pc2 = gameObject.GetComponent<PolygonCollider2D>();
			int pointCount = pc2.GetTotalPointCount();

			MeshFilter mf = GetComponent<MeshFilter>();
			Mesh mesh = new Mesh();
			Vector2[] points = pc2.points;
			Vector3[] vertices = new Vector3[pointCount];
			for (int j = 0; j < pointCount; j++)
			{
				Vector2 actual = points[j];
				vertices[j] = new Vector3(actual.x, actual.y, 0);
			}

			Triangulator tr = new Triangulator(points);
			int[] triangles = tr.Triangulate();
			mesh.vertices = vertices;
			mesh.triangles = triangles;
			mf.mesh = mesh;

			mf.sharedMesh.RecalculateBounds();
		}

		public void ClearShape()
		{
			MeshFilter mf = GetComponent<MeshFilter>();
			mf.mesh = null;
		}


		#region Triangulator Class

		private class Triangulator
		{
			private readonly List<Vector2> m_points;

			public Triangulator(Vector2[] points)
			{
				m_points = new List<Vector2>(points);
			}

			public int[] Triangulate()
			{
				List<int> indices = new List<int>();

				int n = m_points.Count;
				if (n < 3)
					return indices.ToArray();

				int[] V = new int[n];
				if (Area() > 0)
				{
					for (int v = 0; v < n; v++)
						V[v] = v;
				}
				else
				{
					for (int v = 0; v < n; v++)
						V[v] = (n - 1) - v;
				}

				int nv = n;
				int count = 2 * nv;
				for (int v = nv - 1; nv > 2;)
				{
					if ((count--) <= 0)
						return indices.ToArray();

					int u = v;
					if (nv <= u)
						u = 0;
					v = u + 1;
					if (nv <= v)
						v = 0;
					int w = v + 1;
					if (nv <= w)
						w = 0;

					if (Snip(u, v, w, nv, V))
					{
						int s, t;
						var a = V[u];
						var b = V[v];
						var c = V[w];
						indices.Add(a);
						indices.Add(b);
						indices.Add(c);

						for (s = v, t = v + 1; t < nv; s++, t++)
							V[s] = V[t];
						nv--;
						count = 2 * nv;
					}
				}

				indices.Reverse();
				return indices.ToArray();
			}

			private float Area()
			{
				int n = m_points.Count;
				float A = 0.0f;
				for (int p = n - 1, q = 0; q < n; p = q++)
				{
					Vector2 pval = m_points[p];
					Vector2 qval = m_points[q];
					A += pval.x * qval.y - qval.x * pval.y;
				}

				return (A * 0.5f);
			}

			private bool Snip(int u, int v, int w, int n, int[] V)
			{
				int p;
				Vector2 A = m_points[V[u]];
				Vector2 B = m_points[V[v]];
				Vector2 C = m_points[V[w]];
				if (Mathf.Epsilon > (((B.x - A.x) * (C.y - A.y)) - ((B.y - A.y) * (C.x - A.x))))
					return false;
				for (p = 0; p < n; p++)
				{
					if ((p == u) || (p == v) || (p == w))
						continue;
					Vector2 P = m_points[V[p]];
					if (InsideTriangle(A, B, C, P))
						return false;
				}

				return true;
			}

			private bool InsideTriangle(Vector2 A, Vector2 B, Vector2 C, Vector2 P)
			{
				var ax = C.x - B.x;
				var ay = C.y - B.y;
				var bx = A.x - C.x;
				var @by = A.y - C.y;
				var cx = B.x - A.x;
				var cy = B.y - A.y;
				var apx = P.x - A.x;
				var apy = P.y - A.y;
				var bpx = P.x - B.x;
				var bpy = P.y - B.y;
				var cpx = P.x - C.x;
				var cpy = P.y - C.y;

				var aCROSSbp = ax * bpy - ay * bpx;
				var cCROSSap = cx * apy - cy * apx;
				var bCROSScp = bx * cpy - @by * cpx;

				return ((aCROSSbp >= 0.0f) && (bCROSScp >= 0.0f) && (cCROSSap >= 0.0f));
			}
		}

		#endregion
	}
}