using UnityEngine;


public enum CCGPresets
{
	Custom,
	Red,
	Blue,
	Green,
	Purple,
	Yellow,
	Aqua, 
	White,
	Lilac,
	DirtySand
};


public class CrucialColliderGizmo : MonoBehaviour
{

	public CCGPresets selectedPreset;

	public Color savedCustomWireColor;
	public Color savedCustomFillColor;
	public Color savedCustomCenterColor;

	public float overallAlpha = 1.0f;
	public Color wireColor = new Color(.6f, .6f, 1f, .5f);
	public Color fillColor = new Color(.6f, .7f, 1f, .1f);
	public Color centerColor = new Color(.6f, .7f, 1f, .7f);

	public bool drawFill = true;
	public bool drawWire = true;

	public bool drawCenter;
	public float centerMarkerRadius = 1.0f;

	public float collider2D_ZDepth = 2.0f;

	public bool includeChildColliders;


	#region Change Preset

	public void ChangePreset(CCGPresets preset)
	{
		selectedPreset = preset;

		switch (selectedPreset)
		{
			case CCGPresets.Red:
				wireColor = new Color32(143, 0, 21, 202);
				fillColor = new Color32(218, 0, 0, 37);
				centerColor = new Color32(135, 36, 36, 172);
				break;

			case CCGPresets.Blue:
				wireColor = new Color32(0, 116, 214, 202);
				fillColor = new Color32(0, 110, 218, 37);
				centerColor = new Color32(57, 160, 221, 172);
				break;

			case CCGPresets.Green:
				wireColor = new Color32(153, 255, 187, 128);
				fillColor = new Color32(153, 255, 187, 62);
				centerColor = new Color32(153, 255, 187, 172);
				break;

			case CCGPresets.Purple:
				wireColor = new Color32(138, 138, 234, 128);
				fillColor = new Color32(173, 178, 255, 26);
				centerColor = new Color32(153, 178, 255, 172);
				break;

			case CCGPresets.Yellow:
				wireColor = new Color32(255, 231, 35, 128);
				fillColor = new Color32(255, 252, 153, 100);
				centerColor = new Color32(255, 242, 84, 172);
				break;

			case CCGPresets.DirtySand:
				wireColor = new Color32(255, 170, 0, 60);
				fillColor = new Color32(180, 160, 80, 175);
				centerColor = new Color32(255, 242, 84, 172);
				break;

			case CCGPresets.Aqua:
				wireColor = new Color32(255, 255, 255, 120);
				fillColor = new Color32(0, 230, 255, 140);
				centerColor = new Color32(255, 255, 255, 120);
				break;

			case CCGPresets.White:
				wireColor = new Color32(255, 255, 255, 130);
				fillColor = new Color32(255, 255, 255, 130);
				centerColor = new Color32(255, 255, 255, 130);
				break;

			case CCGPresets.Lilac:
				wireColor = new Color32(255, 255, 255, 255);
				fillColor = new Color32(160, 190, 255, 140);
				centerColor = new Color32(255, 255, 255, 130);
				break;


			case CCGPresets.Custom:
				wireColor = savedCustomWireColor;
				fillColor = savedCustomFillColor;
				centerColor = savedCustomCenterColor;
				break;
		}
	}

	#endregion


	void OnDrawGizmos()
	{
		if (!enabled) return;

		DrawColliders(gameObject);

		if (includeChildColliders)
		{
			Transform[] allTransforms = gameObject.GetComponentsInChildren<Transform>();
			for (int i = 0; i < allTransforms.Length; i++)
			{
				if (allTransforms[i].gameObject == gameObject)
					continue;
				DrawColliders(allTransforms[i].gameObject);
			}
		}
	}


	#region Drawers

	void DrawCollider(bool wired, EdgeCollider2D edgeCollider, Vector3 position, Vector3 scale, Transform targetTran)
	{
		if (!edgeCollider) return;

		Gizmos.color = wireColor;
		Vector3 prev = Vector2.zero;
		bool first = true;
		for (int i = 0; i < edgeCollider.points.Length; i++)
		{
			var coll = edgeCollider.points[i];
			Vector3 pos = new Vector3(coll.x * scale.x, coll.y * scale.y, 0);
			Vector3 rotated = targetTran.rotation * pos;

			if (first)
				first = false;
			else
			{
				Gizmos.DrawLine(position + prev, position + rotated);
			}
			prev = rotated;

			DrawColliderGizmo(wired, position + rotated, .05f);
		}
	}

	void DrawCollider(bool wired, BoxCollider2D boxCollider, Transform targetTran)
	{
		if (!boxCollider) return;

		Gizmos.matrix = Matrix4x4.TRS(targetTran.position, targetTran.rotation, targetTran.lossyScale);
		DrawColliderGizmo(wired, boxCollider.offset, boxCollider.size);
		Gizmos.matrix = Matrix4x4.identity;
	}

	void DrawCollider(bool wired, BoxCollider boxCollider, Transform targetTran)
	{
		if (!boxCollider) return;

		Gizmos.matrix = Matrix4x4.TRS(targetTran.position, targetTran.rotation, targetTran.lossyScale);
		DrawColliderGizmo(wired, boxCollider.center, boxCollider.size);
		Gizmos.matrix = Matrix4x4.identity;
	}

	void DrawCollider(bool wired, CircleCollider2D circleCollider, Vector3 position, Vector3 scale)
	{
		if (!circleCollider) return;

		DrawColliderGizmo(wired, position + new Vector3(circleCollider.offset.x, circleCollider.offset.y, 0.0f),
			circleCollider.radius * Mathf.Max(scale.x, scale.y));
	}

	void DrawCollider(bool wired, SphereCollider sphereCollider, Vector3 position, Vector3 scale)
	{
		if (!sphereCollider) return;

		DrawColliderGizmo(wired, position + new Vector3(sphereCollider.center.x, sphereCollider.center.y, 0.0f),
			sphereCollider.radius * Mathf.Max(scale.x, scale.y, scale.z));
	}

	#endregion


	void DrawColliders(GameObject hostGameObject)
	{

		Vector3 position = hostGameObject.transform.position;
		Vector3 trueScale = hostGameObject.transform.lossyScale;

		EdgeCollider2D collEdge = null;
		BoxCollider2D collBox2d = null;
		BoxCollider collbox = null;
		CircleCollider2D collCircle2d = null;
		SphereCollider collSphere = null;

		// Fastest way to get all types of colliders
		var colls2d = hostGameObject.GetComponents<Collider2D>();
		var colls = hostGameObject.GetComponents<Collider>();
		for (var i = 0; i < colls2d.Length; i++)
		{
			var c = colls2d[i];

			var box2d = c as BoxCollider2D;
			if (box2d != null)
			{
				collBox2d = box2d;
				continue;
			}
			var edge = c as EdgeCollider2D;
			if (edge != null)
			{
				collEdge = edge;
				continue;
			}
			var circle2d = c as CircleCollider2D;
			if (circle2d != null) collCircle2d = circle2d;
		}
		for (var i = 0; i < colls.Length; i++)
		{
			var c = colls[i];

			var box = c as BoxCollider;
			if (box != null)
			{
				collbox = box;
				continue;
			}
			var sphere = c as SphereCollider;
			if (sphere != null) collSphere = sphere;
		}


		Gizmos.color = new Color(wireColor.r, wireColor.g, wireColor.b, wireColor.a * overallAlpha);
		if (drawWire)
		{
			DrawCollider(true, collEdge, position, trueScale, hostGameObject.transform);
			DrawCollider(true, collBox2d, hostGameObject.transform);
			DrawCollider(true, collbox, hostGameObject.transform);
			DrawCollider(true, collCircle2d, position, trueScale);
			DrawCollider(true, collSphere, position, trueScale);
		}

		Gizmos.color = new Color(fillColor.r, fillColor.g, fillColor.b, fillColor.a * overallAlpha);
		if (drawFill)
		{
			DrawCollider(false, collEdge, position, trueScale, hostGameObject.transform);
			DrawCollider(false, collBox2d, hostGameObject.transform);
			DrawCollider(false, collbox, hostGameObject.transform);
			DrawCollider(false, collCircle2d, position, trueScale);
			DrawCollider(false, collSphere, position, trueScale);
		}

		if (drawCenter)
		{
			Gizmos.color = new Color(centerColor.r, centerColor.g, centerColor.b, centerColor.a * overallAlpha);
			Gizmos.DrawSphere(hostGameObject.transform.position, centerMarkerRadius);
		}
	}

	private void DrawColliderGizmo(bool wired, Vector3 position, Vector3 size)
	{
		if (wired)
			Gizmos.DrawWireCube(position, size);
		else
			Gizmos.DrawCube(position, size);
	}
	private void DrawColliderGizmo(bool wired, Vector3 position, float radius)
	{
		if (wired)
			Gizmos.DrawWireSphere(position, radius);
		else
			Gizmos.DrawSphere(position, radius);
	}

}