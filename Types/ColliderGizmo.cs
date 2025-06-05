using UnityEngine;

#if UNITY_EDITOR
using MyBox.Internal;
using UnityEditor;
#endif

namespace MyBox
{
	[ExecuteInEditMode]
	public class ColliderGizmo : MonoBehaviour
	{
#if UNITY_EDITOR
		public Presets Preset;

		[Range(0, 1)]
		public float Alpha = 1.0f;
		
		//public OptionalColor WireColor2 = new(.6f, .6f, 1f, .5f, true);
		public Color WireColor = new(.6f, .6f, 1f, .5f);
		public Color FillColor = new(.6f, .7f, 1f, .1f);
		public Color CenterColor = new(.6f, .7f, 1f, .7f);

		public bool DrawFill = true;
		public bool DrawWire = true;
		public bool DrawCenter;

		private Color CurrentWireColor => WireColor.WithAlphaSetTo(WireColor.a * Alpha);
		private Color CurrentFillColor => FillColor.WithAlphaSetTo(FillColor.a * Alpha);
		private Color CurrentCenterColor => CenterColor.WithAlphaSetTo(CenterColor.a * Alpha);
		
		/// <summary>
		/// The radius of the center marker on your collider(s)
		/// </summary>
		public float CenterMarkerRadius = 1.0f;

		public bool IncludeChildColliders;

		private readonly ColliderGizmo2dDrawer _drawer2d = new();
		private readonly ColliderGizmo3dDrawer _drawer3d = new();
		private readonly ColliderGizmoNavMeshDrawer _drawerNavMesh = new();

		
		private void OnEnable() => Refresh();

		private void OnDrawGizmos()
		{
			if (enabled) DrawColliders();
		}

		public void Refresh()
		{
			_drawer2d.RefreshReferences(this);
			_drawer3d.RefreshReferences(this);
			_drawerNavMesh.RefreshReferences(this);
		}


		#region Drawers

		private void DrawColliders()
		{
			_drawer2d.DrawGizmos(this);
			_drawer3d.DrawGizmos(this);
			_drawerNavMesh.DrawGizmos(this);
		}

		public void DrawWireLine(Vector3 from, Vector3 to)
		{
			if (!DrawWire) return;

			Gizmos.color = CurrentWireColor;
			Gizmos.DrawLine(from, to);
		}
		
		public void DrawBox(Vector3 position, Vector3 size)
		{
			if (DrawWire)
			{
				Gizmos.color = CurrentWireColor;
				Gizmos.DrawWireCube(position, size);
			}

			if (DrawFill)
			{
				Gizmos.color = CurrentFillColor;
				Gizmos.DrawCube(position, size);
			}
			
			if (DrawCenter)
			{
				Gizmos.color = CurrentCenterColor;
				Gizmos.DrawSphere(position, CenterMarkerRadius);
			}
		}

		public void DrawSphere(Vector3 position, float radius)
		{
			if (DrawWire)
			{
				Gizmos.color = CurrentWireColor;
				Gizmos.DrawWireSphere(position, radius);
			}

			if (DrawFill)
			{
				Gizmos.color = CurrentFillColor;
				Gizmos.DrawSphere(position, radius);
			}
			
			if (DrawCenter)
			{
				Gizmos.color = CurrentCenterColor;
				Gizmos.DrawSphere(position, CenterMarkerRadius);
			}
		}
		
		public void DrawMesh(Mesh mesh, Vector3 position, Quaternion rotation, Vector3 scale)
		{
			if (DrawWire)
			{
				Gizmos.color = CurrentWireColor;
				Gizmos.DrawWireMesh(mesh, position, rotation, scale);
			}

			if (DrawFill)
			{
				Gizmos.color = CurrentFillColor;
				Gizmos.DrawMesh(mesh, position, rotation, scale);
			}
			
			if (DrawCenter)
			{
				Gizmos.color = CurrentCenterColor;
				Gizmos.DrawSphere(position, CenterMarkerRadius);
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
					break;
			}

			Refresh();
		}

		#endregion

		
		[InitializeOnLoadMethod]
		private static void RefreshOnComponentsChange()
		{
			ObjectFactory.componentWasAdded += OnComponentWasAdded;

			void OnComponentWasAdded(Component component)
			{
				if (IsValidForGizmoComponent())
				{
					Debug.Log("was added valid: " + component.GetType().Name);
					EditorApplication.delayCall += () =>
					{
						var gizmo = component.GetComponentInParent<ColliderGizmo>();
						Debug.LogWarning("delay call: " + gizmo, gizmo);
						if (gizmo) gizmo.Refresh();
					};
					
					var gizmo = component.GetComponentInParent<ColliderGizmo>();
					if (gizmo)
					{
						Debug.LogWarning("shoud work..");
						gizmo.Refresh();
					}
				}
			
				bool IsValidForGizmoComponent()
				{
#if UNITY_PHYSICS_ENABLED
					if (component is Collider) return true;
#endif
#if UNITY_PHYSICS2D_ENABLED
					if (component is Collider2D) return true;
#endif
#if UNITY_AI_ENABLED
					if (component is UnityEngine.AI.NavMeshObstacle) return true;
#endif
					return false;
				}
			}
		}
#endif
	}
}


#if UNITY_EDITOR

namespace MyBox.Internal
{
	[CustomEditor(typeof(ColliderGizmo)), CanEditMultipleObjects]
	public class ColliderGizmoEditor : Editor
	{
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

		private void OnEnable()
		{
			_target = target as ColliderGizmo;
			
			_alphaProperty = serializedObject.FindProperty(nameof(ColliderGizmo.Alpha));

			_drawWireProperty = serializedObject.FindProperty(nameof(ColliderGizmo.DrawWire));
			_wireColorProperty = serializedObject.FindProperty(nameof(ColliderGizmo.WireColor));

			_drawFillProperty = serializedObject.FindProperty(nameof(ColliderGizmo.DrawFill));
			_fillColorProperty = serializedObject.FindProperty(nameof(ColliderGizmo.FillColor));

			_drawCenterProperty = serializedObject.FindProperty(nameof(ColliderGizmo.DrawCenter));
			_centerColorProperty = serializedObject.FindProperty(nameof(ColliderGizmo.CenterColor));
			_centerRadiusProperty = serializedObject.FindProperty(nameof(ColliderGizmo.CenterMarkerRadius));

			_includeChilds = serializedObject.FindProperty(nameof(ColliderGizmo.IncludeChildColliders));
		}

		

		public override void OnInspectorGUI()
		{
			Undo.RecordObject(_target, "Collider Gizmo updated");

			EditorGUI.BeginChangeCheck();
			_target.Preset = (ColliderGizmo.Presets)EditorGUILayout.EnumPopup("Color Preset", _target.Preset);
			if (EditorGUI.EndChangeCheck())
			{
				foreach (var singleTarget in targets)
				{
					((ColliderGizmo)singleTarget).ChangePreset(_target.Preset);
				}
				serializedObject.ApplyModifiedProperties();
			}

			EditorGUILayout.PropertyField(_alphaProperty, new GUIContent("Overall Transparency"));
			if (GUI.changed) serializedObject.ApplyModifiedProperties();

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
				presetProp.enumValueIndex = (int)ColliderGizmo.Presets.Custom;
				serializedObject.ApplyModifiedProperties();
			}

			EditorGUI.BeginChangeCheck();
			EditorGUILayout.PropertyField(_includeChilds);

			if (GUI.changed) serializedObject.ApplyModifiedProperties();

			if (EditorGUI.EndChangeCheck())
			{
				foreach (var singleTarget in targets)
				{
					((ColliderGizmo)singleTarget).Refresh();
				}
				serializedObject.ApplyModifiedProperties();
			}
		}
	}
}

#endif

#region ColliderGizmo2dDrawer

#if UNITY_EDITOR
namespace MyBox.Internal
{
	using UnityEngine;
	
	public class ColliderGizmo2dDrawer
	{
#if UNITY_PHYSICS2D_ENABLED
		private Collider2D[] _colliders;
#endif

		public void RefreshReferences(ColliderGizmo target)
		{
#if UNITY_PHYSICS2D_ENABLED
			_colliders = target.IncludeChildColliders ? 
				target.gameObject.GetComponentsInChildren<Collider2D>() : 
				target.gameObject.GetComponents<Collider2D>();
#endif
		}

		public void DrawGizmos(ColliderGizmo target)
		{
#if UNITY_PHYSICS2D_ENABLED
			if (_colliders == null) return;
			
			foreach (var collider in _colliders)
			{
				if (!collider || !collider.enabled) continue;
				
				if (collider is BoxCollider2D box) DrawBoxCollider2D(target, box);
				else if (collider is CircleCollider2D circle) DrawCircleCollider2D(target, circle);
				else if (collider is CapsuleCollider2D capsule) DrawCapsuleCollider2D(target, capsule);
				else if (collider is EdgeCollider2D edge) DrawEdgeCollider2D(target, edge);
			}
			
			
			void DrawBoxCollider2D(ColliderGizmo gizmo, BoxCollider2D coll)
			{
				var t = coll.transform;
				Gizmos.matrix = Matrix4x4.TRS(t.position, t.rotation, t.lossyScale);
				gizmo.DrawBox(coll.offset, coll.size);
				Gizmos.matrix = Matrix4x4.identity;
			}
			
			void DrawCircleCollider2D(ColliderGizmo gizmo, CircleCollider2D coll)
			{
				var t = coll.transform;
				var offset = coll.offset;
				var scale = t.lossyScale;
				gizmo.DrawSphere(t.position + new Vector3(offset.x, offset.y, 0.0f), coll.radius * Mathf.Max(scale.x, scale.y));
			}
			
			void DrawCapsuleCollider2D(ColliderGizmo gizmo, CapsuleCollider2D coll)
			{
				var t = coll.transform;
				Gizmos.matrix = Matrix4x4.TRS(t.position, t.rotation, t.lossyScale);
				gizmo.DrawBox(coll.offset, coll.size);
				Gizmos.matrix = Matrix4x4.identity;
			}
			
			void DrawEdgeCollider2D(ColliderGizmo gizmo, EdgeCollider2D coll)
			{
				var t = coll.transform;
				var lossyScale = t.lossyScale;
				var position = t.position;

				Vector3 previous = Vector2.zero;
				bool first = true;
				for (int i = 0; i < coll.points.Length; i++)
				{
					var collPoint = coll.points[i];
					Vector3 pos = new Vector3(collPoint.x * lossyScale.x, collPoint.y * lossyScale.y, 0);
					Vector3 rotated = t.rotation * pos;

					if (first) first = false;
					else gizmo.DrawWireLine(position + previous, position + rotated);

					previous = rotated;
					gizmo.DrawSphere(t.position + rotated, .05f);
				}
			}
#endif
		}
	}
}
#endif

#endregion

#region ColliderGizmo3dDrawer

#if UNITY_EDITOR
namespace MyBox.Internal
{
	using UnityEngine;
	
	public class ColliderGizmo3dDrawer
	{
#if UNITY_PHYSICS_ENABLED
		private Collider[] _colliders;
#endif
		
		public void RefreshReferences(ColliderGizmo target)
		{
#if UNITY_PHYSICS_ENABLED
			_colliders = target.IncludeChildColliders ? 
				target.gameObject.GetComponentsInChildren<Collider>() : 
				target.gameObject.GetComponents<Collider>();
#endif
		}

		public void DrawGizmos(ColliderGizmo target)
		{
#if UNITY_PHYSICS_ENABLED
			if (_colliders == null) return;
			
			foreach (var collider in _colliders)
			{
				if (!collider || !collider.enabled) continue;
				
				if (collider is BoxCollider box) DrawBoxCollider(target, box);
				else if (collider is SphereCollider sphere) DrawSphereCollider(target, sphere);
				else if (collider is CapsuleCollider capsule) DrawCapsuleCollider(target, capsule);
				else if (collider is MeshCollider mesh) DrawMeshCollider(target, mesh);
			}
			
			
			void DrawBoxCollider(ColliderGizmo gizmo, BoxCollider coll)
			{
				var t = coll.transform;
				Gizmos.matrix = Matrix4x4.TRS(t.position, t.rotation, t.lossyScale);
				gizmo.DrawBox(coll.center, coll.size);
				Gizmos.matrix = Matrix4x4.identity;
			}
			
			void DrawSphereCollider(ColliderGizmo gizmo, SphereCollider coll)
			{
				var t = coll.transform;
				var scale = t.lossyScale;
				var center = coll.center;
				var max = Mathf.Max(scale.x, Mathf.Max(scale.y, scale.z)); // to not use Mathf.Max version with params[]
				gizmo.DrawSphere(t.position + new Vector3(center.x, center.y, 0.0f), coll.radius * max);
			}
			
			void DrawCapsuleCollider(ColliderGizmo gizmo, CapsuleCollider coll)
			{
				var t = coll.transform;
				Gizmos.matrix = Matrix4x4.TRS(t.position, t.rotation, t.lossyScale);
				
				var diameter = coll.radius * 2f;
				Vector3 size;

				if (coll.direction == 0) size = new Vector3(coll.height, diameter, diameter); // x axis
				else if (coll.direction == 1) size = new Vector3(diameter, coll.height, diameter); // y axis
				else size = new Vector3(diameter, diameter, coll.height); // z axis

				gizmo.DrawBox(coll.center, size);
				Gizmos.matrix = Matrix4x4.identity;
			}
			
			void DrawMeshCollider(ColliderGizmo gizmo, MeshCollider coll)
			{
				var t = coll.transform;
				gizmo.DrawMesh(coll.sharedMesh, t.position, t.rotation, t.localScale * 1.01f);
			}
#endif
		}
	}
}
#endif

#endregion

#region ColliderGizmoNavMeshDrawer

#if UNITY_EDITOR
namespace MyBox.Internal
{
	using UnityEngine;
	
	public class ColliderGizmoNavMeshDrawer
	{
#if UNITY_AI_ENABLED
		private UnityEngine.AI.NavMeshObstacle _navMeshObstacle;
#endif
		
		public void RefreshReferences(ColliderGizmo target)
		{
#if UNITY_AI_ENABLED
			_navMeshObstacle = target.IncludeChildColliders ? 
				target.GetComponentInChildren<UnityEngine.AI.NavMeshObstacle>() :
				target.GetComponent<UnityEngine.AI.NavMeshObstacle>();
#endif
		}

		public void DrawGizmos(ColliderGizmo target)
		{
#if UNITY_AI_ENABLED
			if (!_navMeshObstacle || !_navMeshObstacle.enabled) return;
			
			DrawNavMeshObstacle(target, _navMeshObstacle);
			
			
			void DrawNavMeshObstacle(ColliderGizmo gizmo, UnityEngine.AI.NavMeshObstacle obstacle)
			{
				var t = obstacle.transform;
				if (obstacle.shape == UnityEngine.AI.NavMeshObstacleShape.Box)
				{
					Gizmos.matrix = Matrix4x4.TRS(t.position, t.rotation, t.lossyScale);
					gizmo.DrawBox(obstacle.center, obstacle.size);
					Gizmos.matrix = Matrix4x4.identity;
				}
				else
				{
					var scale = t.lossyScale;
					var center = obstacle.center;
					var max = Mathf.Max(scale.x, Mathf.Max(scale.y, scale.z));
					gizmo.DrawSphere(t.position + new Vector3(center.x, center.y, 0.0f), obstacle.radius * max);
				}
			}
#endif
		}
	}
}
#endif

#endregion