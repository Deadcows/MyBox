using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
using UnityEngine.AI;

#endif

namespace MyBox
{
	public class ColliderGizmo : MonoBehaviour
	{
#if UNITY_EDITOR
		public Presets Preset;

		public Color CustomWireColor;
		public Color CustomFillColor;
		public Color CustomCenterColor;

		public float Alpha = 1.0f;
		public Color WireColor = new Color(.6f, .6f, 1f, .5f);
		public Color FillColor = new Color(.6f, .7f, 1f, .1f);
		public Color CenterColor = new Color(.6f, .7f, 1f, .7f);

		public bool DrawFill = true;
		public bool DrawWire = true;
		public bool DrawCenter;

		/// <summary>
		/// The radius of the center marker on your collider(s)
		/// </summary>
		public float CenterMarkerRadius = 1.0f;

		public bool IncludeChildColliders;

#if UNITY_AI_ENABLED
		private NavMeshObstacle _navMeshObstacle;
#endif

#if UNITY_PHYSICS2D_ENABLED
		private List<EdgeCollider2D> _edgeColliders2D;
		private List<BoxCollider2D> _boxColliders2D;
		private List<CircleCollider2D> _circleColliders2D;
#endif

#if UNITY_PHYSICS_ENABLED
		private List<BoxCollider> _boxColliders;
		private List<SphereCollider> _sphereColliders;
		private List<MeshCollider> _meshColliders;
#endif

		private readonly HashSet<Transform> _withColliders = new HashSet<Transform>();

		private Color _wireGizmoColor;
		private Color _fillGizmoColor;
		private Color _centerGizmoColor;

		private bool _initialized;


		private void OnDrawGizmos()
		{
			if (!enabled) return;
			if (!_initialized) Refresh();

			DrawColliders();
		}

		#region Refresh

		public void Refresh()
		{
			_initialized = true;

			_wireGizmoColor = new Color(WireColor.r, WireColor.g, WireColor.b, WireColor.a * Alpha);
			_fillGizmoColor = new Color(FillColor.r, FillColor.g, FillColor.b, FillColor.a * Alpha);
			_centerGizmoColor = new Color(CenterColor.r, CenterColor.g, CenterColor.b, CenterColor.a * Alpha);

			_withColliders.Clear();

#if UNITY_AI_ENABLED
			_navMeshObstacle = gameObject.GetComponent<NavMeshObstacle>();
#endif

#if UNITY_PHYSICS2D_ENABLED
			if (_edgeColliders2D != null) _edgeColliders2D.Clear();
			if (_boxColliders2D != null) _boxColliders2D.Clear();
			if (_circleColliders2D != null) _circleColliders2D.Clear();

			Collider2D[] colliders2d = IncludeChildColliders ? gameObject.GetComponentsInChildren<Collider2D>() : gameObject.GetComponents<Collider2D>();

			for (var i = 0; i < colliders2d.Length; i++)
			{
				var c = colliders2d[i];

				var box2d = c as BoxCollider2D;
				if (box2d != null)
				{
					if (_boxColliders2D == null) _boxColliders2D = new List<BoxCollider2D>();
					_boxColliders2D.Add(box2d);
					_withColliders.Add(box2d.transform);
					continue;
				}

				var edge = c as EdgeCollider2D;
				if (edge != null)
				{
					if (_edgeColliders2D == null) _edgeColliders2D = new List<EdgeCollider2D>();
					_edgeColliders2D.Add(edge);
					_withColliders.Add(edge.transform);
					continue;
				}

				var circle2d = c as CircleCollider2D;
				if (circle2d != null)
				{
					if (_circleColliders2D == null) _circleColliders2D = new List<CircleCollider2D>();
					_circleColliders2D.Add(circle2d);
					_withColliders.Add(circle2d.transform);
				}
			}
#endif

#if UNITY_PHYSICS_ENABLED
			if (_boxColliders != null) _boxColliders.Clear();
			if (_sphereColliders != null) _sphereColliders.Clear();
			if (_meshColliders != null) _meshColliders.Clear();

			Collider[] colliders = IncludeChildColliders ? gameObject.GetComponentsInChildren<Collider>() : gameObject.GetComponents<Collider>();

			for (var i = 0; i < colliders.Length; i++)
			{
				var c = colliders[i];

				var box = c as BoxCollider;
				if (box != null)
				{
					if (_boxColliders == null) _boxColliders = new List<BoxCollider>();
					_boxColliders.Add(box);
					_withColliders.Add(box.transform);
					continue;
				}

				var sphere = c as SphereCollider;
				if (sphere != null)
				{
					if (_sphereColliders == null) _sphereColliders = new List<SphereCollider>();
					_sphereColliders.Add(sphere);
					_withColliders.Add(sphere.transform);
				}

				var mesh = c as MeshCollider;
				if (mesh != null)
				{
					if (_meshColliders == null) _meshColliders = new List<MeshCollider>();
					_meshColliders.Add(mesh);
					_withColliders.Add(mesh.transform);
				}
			}
#endif
		}

		#endregion


		#region Drawers

#if UNITY_PHYSICS2D_ENABLED

		private void DrawEdgeCollider2D(EdgeCollider2D coll)
		{
			var target = coll.transform;
			var lossyScale = target.lossyScale;
			var position = target.position;

			Gizmos.color = WireColor;
			Vector3 previous = Vector2.zero;
			bool first = true;
			for (int i = 0; i < coll.points.Length; i++)
			{
				var collPoint = coll.points[i];
				Vector3 pos = new Vector3(collPoint.x * lossyScale.x, collPoint.y * lossyScale.y, 0);
				Vector3 rotated = target.rotation * pos;

				if (first) first = false;
				else
				{
					Gizmos.color = _wireGizmoColor;
					Gizmos.DrawLine(position + previous, position + rotated);
				}

				previous = rotated;

				DrawColliderGizmo(target.position + rotated, .05f);
			}
		}

		private void DrawBoxCollider2D(BoxCollider2D coll)
		{
			var target = coll.transform;
			Gizmos.matrix = Matrix4x4.TRS(target.position, target.rotation, target.lossyScale);
			DrawColliderGizmo(coll.offset, coll.size);
			Gizmos.matrix = Matrix4x4.identity;
		}

		private void DrawCircleCollider2D(CircleCollider2D coll)
		{
			var target = coll.transform;
			var offset = coll.offset;
			var scale = target.lossyScale;
			DrawColliderGizmo(target.position + new Vector3(offset.x, offset.y, 0.0f), coll.radius * Mathf.Max(scale.x, scale.y));
		}

#endif

#if UNITY_PHYSICS_ENABLED

		private void DrawBoxCollider(BoxCollider coll)
		{
			var target = coll.transform;
			Gizmos.matrix = Matrix4x4.TRS(target.position, target.rotation, target.lossyScale);
			DrawColliderGizmo(coll.center, coll.size);
			Gizmos.matrix = Matrix4x4.identity;
		}

		private void DrawSphereCollider(SphereCollider coll)
		{
			var target = coll.transform;
			var scale = target.lossyScale;
			var center = coll.center;
			var max = Mathf.Max(scale.x, Mathf.Max(scale.y, scale.z)); // to not use Mathf.Max version with params[]
			DrawColliderGizmo(target.position + new Vector3(center.x, center.y, 0.0f), coll.radius * max);
		}

		private void DrawMeshCollider(MeshCollider coll)
		{
			var target = coll.transform;

			if (DrawWire)
			{
				Gizmos.color = _wireGizmoColor;
				Gizmos.DrawWireMesh(coll.sharedMesh, target.position, target.rotation, target.localScale * 1.01f);
			}

			if (DrawFill)
			{
				Gizmos.color = _fillGizmoColor;
				Gizmos.DrawMesh(coll.sharedMesh, target.position, target.rotation, target.localScale * 1.01f);
			}
		}

#endif

#if UNITY_AI_ENABLED

		private void DrawNavMeshObstacle(NavMeshObstacle obstacle)
		{
			var target = obstacle.transform;

			if (obstacle.shape == NavMeshObstacleShape.Box)
			{
				Gizmos.matrix = Matrix4x4.TRS(target.position, target.rotation, target.lossyScale);
				DrawColliderGizmo(obstacle.center, obstacle.size);
				Gizmos.matrix = Matrix4x4.identity;
			}
			else
			{
				var scale = target.lossyScale;
				var center = obstacle.center;
				var max = Mathf.Max(scale.x, Mathf.Max(scale.y, scale.z)); // to not use Mathf.Max version with params[]
				DrawColliderGizmo(target.position + new Vector3(center.x, center.y, 0.0f), obstacle.radius * max);
			}
		}

#endif


		private void DrawColliders()
		{
			if (DrawCenter)
			{
				Gizmos.color = _centerGizmoColor;
				foreach (var withCollider in _withColliders)
				{
					Gizmos.DrawSphere(withCollider.position, CenterMarkerRadius);
				}
			}

			if (!DrawWire && !DrawFill) return;

#if UNITY_AI_ENABLED
			if (_navMeshObstacle != null) DrawNavMeshObstacle(_navMeshObstacle);
#endif

#if UNITY_PHYSICS2D_ENABLED
			if (_edgeColliders2D != null)
			{
				foreach (var edge in _edgeColliders2D)
				{
					if (edge == null) continue;
					DrawEdgeCollider2D(edge);
				}
			}

			if (_boxColliders2D != null)
			{
				foreach (var box in _boxColliders2D)
				{
					if (box == null) continue;
					DrawBoxCollider2D(box);
				}
			}

			if (_circleColliders2D != null)
			{
				foreach (var circle in _circleColliders2D)
				{
					if (circle == null) continue;
					DrawCircleCollider2D(circle);
				}
			}
#endif

#if UNITY_PHYSICS_ENABLED
			if (_boxColliders != null)
			{
				foreach (var box in _boxColliders)
				{
					if (box == null) continue;
					DrawBoxCollider(box);
				}
			}

			if (_sphereColliders != null)
			{
				foreach (var sphere in _sphereColliders)
				{
					if (sphere == null) continue;
					DrawSphereCollider(sphere);
				}
			}

			if (_meshColliders != null)
			{
				foreach (var mesh in _meshColliders)
				{
					if (mesh == null) continue;
					DrawMeshCollider(mesh);
				}
			}
#endif
		}


		private void DrawColliderGizmo(Vector3 position, Vector3 size)
		{
			if (DrawWire)
			{
				Gizmos.color = _wireGizmoColor;
				Gizmos.DrawWireCube(position, size);
			}

			if (DrawFill)
			{
				Gizmos.color = _fillGizmoColor;
				Gizmos.DrawCube(position, size);
			}
		}

		private void DrawColliderGizmo(Vector3 position, float radius)
		{
			if (DrawWire)
			{
				Gizmos.color = _wireGizmoColor;
				Gizmos.DrawWireSphere(position, radius);
			}

			if (DrawFill)
			{
				Gizmos.color = _fillGizmoColor;
				Gizmos.DrawSphere(position, radius);
			}
		}

		#endregion


		#region Change Preset

		public enum Presets
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
		}

		public void ChangePreset(Presets preset)
		{
			Preset = preset;

			switch (Preset)
			{
				case Presets.Red:
					WireColor = new Color32(143, 0, 21, 202);
					FillColor = new Color32(218, 0, 0, 37);
					CenterColor = new Color32(135, 36, 36, 172);
					break;

				case Presets.Blue:
					WireColor = new Color32(0, 116, 214, 202);
					FillColor = new Color32(0, 110, 218, 37);
					CenterColor = new Color32(57, 160, 221, 172);
					break;

				case Presets.Green:
					WireColor = new Color32(153, 255, 187, 128);
					FillColor = new Color32(153, 255, 187, 62);
					CenterColor = new Color32(153, 255, 187, 172);
					break;

				case Presets.Purple:
					WireColor = new Color32(138, 138, 234, 128);
					FillColor = new Color32(173, 178, 255, 26);
					CenterColor = new Color32(153, 178, 255, 172);
					break;

				case Presets.Yellow:
					WireColor = new Color32(255, 231, 35, 128);
					FillColor = new Color32(255, 252, 153, 100);
					CenterColor = new Color32(255, 242, 84, 172);
					break;

				case Presets.DirtySand:
					WireColor = new Color32(255, 170, 0, 60);
					FillColor = new Color32(180, 160, 80, 175);
					CenterColor = new Color32(255, 242, 84, 172);
					break;

				case Presets.Aqua:
					WireColor = new Color32(255, 255, 255, 120);
					FillColor = new Color32(0, 230, 255, 140);
					CenterColor = new Color32(255, 255, 255, 120);
					break;

				case Presets.White:
					WireColor = new Color32(255, 255, 255, 130);
					FillColor = new Color32(255, 255, 255, 130);
					CenterColor = new Color32(255, 255, 255, 130);
					break;

				case Presets.Lilac:
					WireColor = new Color32(255, 255, 255, 255);
					FillColor = new Color32(160, 190, 255, 140);
					CenterColor = new Color32(255, 255, 255, 130);
					break;


				case Presets.Custom:
					WireColor = CustomWireColor;
					FillColor = CustomFillColor;
					CenterColor = CustomCenterColor;
					break;
			}

			Refresh();
		}

		#endregion

#endif
	}
}


#if UNITY_EDITOR

namespace MyBox.Internal
{
	[CustomEditor(typeof(ColliderGizmo)), CanEditMultipleObjects]
	public class ColliderGizmoEditor : Editor
	{
		private SerializedProperty _enabledProperty;
		private SerializedProperty _alphaProperty;
		private SerializedProperty _drawWireProperty;
		private SerializedProperty _wireColorProperty;
		private SerializedProperty _drawFillProperty;
		private SerializedProperty _fillColorProperty;
		private SerializedProperty _drawCenterProperty;
		private SerializedProperty _centerColorProperty;
		private SerializedProperty _centerRadiusProperty;

		private SerializedProperty _includeChilds;

		private ColliderGizmo _target;

		private int _collidersCount;

		private void OnEnable()
		{
			_target = target as ColliderGizmo;

			_enabledProperty = serializedObject.FindProperty("m_Enabled");
			_alphaProperty = serializedObject.FindProperty("Alpha");

			_drawWireProperty = serializedObject.FindProperty("DrawWire");
			_wireColorProperty = serializedObject.FindProperty("WireColor");

			_drawFillProperty = serializedObject.FindProperty("DrawFill");
			_fillColorProperty = serializedObject.FindProperty("FillColor");

			_drawCenterProperty = serializedObject.FindProperty("DrawCenter");
			_centerColorProperty = serializedObject.FindProperty("CenterColor");
			_centerRadiusProperty = serializedObject.FindProperty("CenterMarkerRadius");

			_includeChilds = serializedObject.FindProperty("IncludeChildColliders");

			_collidersCount = CollidersCount();
		}


		public override void OnInspectorGUI()
		{
			Undo.RecordObject(_target, "CG_State");

			EditorGUILayout.PropertyField(_enabledProperty);

			EditorGUI.BeginChangeCheck();
			_target.Preset = (ColliderGizmo.Presets)EditorGUILayout.EnumPopup("Color Preset", _target.Preset);
			if (EditorGUI.EndChangeCheck())
			{
				foreach (var singleTarget in targets)
				{
					var gizmo = (ColliderGizmo)singleTarget;
					gizmo.ChangePreset(_target.Preset);
					EditorUtility.SetDirty(gizmo);
				}
			}

			_alphaProperty.floatValue = EditorGUILayout.Slider("Overall Transparency", _alphaProperty.floatValue, 0, 1);


			EditorGUI.BeginChangeCheck();
			using (new EditorGUILayout.HorizontalScope())
			{
				EditorGUILayout.PropertyField(_drawWireProperty);
				if (_drawWireProperty.boolValue) EditorGUILayout.PropertyField(_wireColorProperty, new GUIContent(""));
			}

			using (new EditorGUILayout.HorizontalScope())
			{
				EditorGUILayout.PropertyField(_drawFillProperty);
				if (_drawFillProperty.boolValue) EditorGUILayout.PropertyField(_fillColorProperty, new GUIContent(""));
			}

			using (new EditorGUILayout.HorizontalScope())
			{
				EditorGUILayout.PropertyField(_drawCenterProperty);
				if (_drawCenterProperty.boolValue)
				{
					EditorGUILayout.PropertyField(_centerColorProperty, GUIContent.none);
					EditorGUILayout.PropertyField(_centerRadiusProperty);
				}
			}


			if (EditorGUI.EndChangeCheck())
			{
				var presetProp = serializedObject.FindProperty("Preset");
				var customWireColor = serializedObject.FindProperty("CustomWireColor");
				var customFillColor = serializedObject.FindProperty("CustomFillColor");
				var customCenterColor = serializedObject.FindProperty("CustomCenterColor");

				presetProp.enumValueIndex = (int)ColliderGizmo.Presets.Custom;
				customWireColor.colorValue = _wireColorProperty.colorValue;
				customFillColor.colorValue = _fillColorProperty.colorValue;
				customCenterColor.colorValue = _centerColorProperty.colorValue;
			}

			EditorGUILayout.PropertyField(_includeChilds);

			int collidersCountCheck = CollidersCount();
			bool collidersCountChanged = collidersCountCheck != _collidersCount;
			_collidersCount = collidersCountCheck;

			if (GUI.changed || collidersCountChanged)
			{
				serializedObject.ApplyModifiedProperties();
				EditorUtility.SetDirty(_target);

				_target.Refresh();
			}
		}

		private int CollidersCount()
		{
			int result = 0;

			if (_includeChilds.boolValue)
			{
#if UNITY_PHYSICS_ENABLED
				result += _target.gameObject.GetComponentsInChildren<Collider>().Length;
#endif
#if UNITY_PHYSICS2D_ENABLED
				result += _target.gameObject.GetComponentsInChildren<Collider2D>().Length;
#endif
				return result;
			}

#if UNITY_PHYSICS_ENABLED
			result += _target.gameObject.GetComponents<Collider>().Length;
#endif
#if UNITY_PHYSICS2D_ENABLED
			result += _target.gameObject.GetComponents<Collider2D>().Length;
#endif
			return result;
		}
	}
}

#endif