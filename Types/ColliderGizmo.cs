using System;
using JetBrains.Annotations;
using UnityEngine;

#if UNITY_EDITOR
using MyBox.Internal;
using UnityEditor;
#endif

namespace MyBox
{
	[Serializable]
	public class ColliderGizmoPreset
	{
		public OptionalColor WireColor = new(.6f, .6f, 1f, .5f, true);
		public OptionalColor FillColor = new(.6f, .7f, 1f, .1f, true);
		public OptionalColor CenterColor = new(.6f, .7f, 1f, .7f, true);

		public static ColliderGizmoPreset FromPremadePreset(ColliderGizmo.Presets preset)
		{
			var result = new ColliderGizmoPreset();
			switch (preset)
			{
				case ColliderGizmo.Presets.Red:
					result.WireColor = new OptionalColor(new Color32(143, 0, 21, 202), true);
					result.FillColor = new OptionalColor(new Color32(218, 0, 0, 37), true);
					result.CenterColor = new OptionalColor(new Color32(135, 36, 36, 172));
					break;

				case ColliderGizmo.Presets.Blue:
					result.WireColor = new OptionalColor(new Color32(0, 116, 214, 202), true);
					result.FillColor = new OptionalColor(new Color32(0, 110, 218, 37), true);
					result.CenterColor = new OptionalColor(new Color32(57, 160, 221, 172));
					break;

				case ColliderGizmo.Presets.Green:
					result.WireColor = new OptionalColor(new Color32(153, 255, 187, 128), true);
					result.FillColor = new OptionalColor(new Color32(153, 255, 187, 62), true);
					result.CenterColor = new OptionalColor(new Color32(153, 255, 187, 172));
					break;

				case ColliderGizmo.Presets.Purple:
					result.WireColor = new OptionalColor(new Color32(138, 138, 234, 128), true);
					result.FillColor = new OptionalColor(new Color32(173, 178, 255, 26), true);
					result.CenterColor = new OptionalColor(new Color32(153, 178, 255, 172));
					break;

				case ColliderGizmo.Presets.Yellow:
					result.WireColor = new OptionalColor(new Color32(255, 231, 35, 128), true);
					result.FillColor = new OptionalColor(new Color32(255, 252, 153, 100), true);
					result.CenterColor = new OptionalColor(new Color32(255, 242, 84, 172));
					break;

				case ColliderGizmo.Presets.DirtySand:
					result.WireColor = new OptionalColor(new Color32(255, 170, 0, 60), true);
					result.FillColor = new OptionalColor(new Color32(180, 160, 80, 175), true);
					result.CenterColor = new OptionalColor(new Color32(255, 242, 84, 172));
					break;

				case ColliderGizmo.Presets.Aqua:
					result.WireColor = new OptionalColor(new Color32(255, 255, 255, 120), true);
					result.FillColor = new OptionalColor(new Color32(0, 230, 255, 140), true);
					result.CenterColor = new OptionalColor(new Color32(255, 255, 255, 120));
					break;

				case ColliderGizmo.Presets.White:
					result.WireColor = new OptionalColor(new Color32(255, 255, 255, 130), true);
					result.FillColor = new OptionalColor(new Color32(255, 255, 255, 130), true);
					result.CenterColor = new OptionalColor(new Color32(255, 255, 255, 130));
					break;

				case ColliderGizmo.Presets.Lilac:
					result.WireColor = new OptionalColor(new Color32(255, 255, 255, 255), true);
					result.FillColor = new OptionalColor(new Color32(160, 190, 255, 140), true);
					result.CenterColor = new OptionalColor(new Color32(255, 255, 255, 130));
					break;


				case ColliderGizmo.Presets.Custom:
					break;
			}
			
			return result;
		}
	}
	
	[ExecuteInEditMode]
	public class ColliderGizmo : MonoBehaviour
	{
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
		
		[PublicAPI]
		public void ChangePreset(ColliderGizmoPreset preset, bool lockChanges = false)
		{
#if UNITY_EDITOR			
			WireColor = preset.WireColor.Value;
			DrawWire = preset.WireColor.IsSet;
			
			FillColor = preset.FillColor.Value;
			DrawFill = preset.FillColor.IsSet;
			
			CenterColor = preset.CenterColor.Value;
			DrawCenter = preset.CenterColor.IsSet;
			
			LockChanges = lockChanges;
#endif			
		}
		
		[PublicAPI]
		public void ChangePreset(Presets preset, bool lockChanges = false)
		{
#if UNITY_EDITOR			
			Preset = preset;
			if (preset == Presets.Custom)
			{
				LockChanges = false;
				return;
			}
			var presetColors = ColliderGizmoPreset.FromPremadePreset(preset);
			ChangePreset(presetColors, lockChanges);
#endif
		}
		
#if UNITY_EDITOR
		public Presets Preset;

		[Range(0, 1)]
		public float Alpha = 1.0f;
		
		public Color WireColor = new(.6f, .6f, 1f, .5f);
		public Color FillColor = new(.6f, .7f, 1f, .1f);
		public Color CenterColor = new(.6f, .7f, 1f, .7f);

		public bool DrawFill = true;
		public bool DrawWire = true;
		public bool DrawCenter;

		[SerializeField, HideInInspector]
		public bool LockChanges;

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

		
		private void OnEnable() => RefreshReferences();

		private void OnDrawGizmos()
		{
			if (enabled) DrawColliders();
		}

		public void RefreshReferences()
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

		
		[InitializeOnLoadMethod]
		private static void RefreshOnComponentsChange()
		{
			ObjectFactory.componentWasAdded += OnComponentWasAdded;

			void OnComponentWasAdded(Component component)
			{
				if (IsValidForGizmoComponent())
				{
					EditorApplication.delayCall += () =>
					{
						var gizmo = component.GetComponentInParent<ColliderGizmo>();
						if (gizmo) gizmo.RefreshReferences();
					};
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

#region Collider Gizmo Editor

#if UNITY_EDITOR
namespace MyBox.Internal
{
	[CustomEditor(typeof(ColliderGizmo)), CanEditMultipleObjects]
	public class ColliderGizmoEditor : Editor
	{
		private struct SerializedProperties
		{
			public SerializedProperty LockChanges;
			public SerializedProperty Preset;
			public SerializedProperty IncludeChild;
			public SerializedProperty Alpha;
			
			public SerializedProperty DrawWire;
			public SerializedProperty WireColor;
			
			public SerializedProperty DrawFill;
			public SerializedProperty FillColor;
			
			public SerializedProperty DrawCenter;
			public SerializedProperty CenterColor;
			public SerializedProperty CenterRadius;
		}

		private SerializedProperties _property;
		
		
		private void OnEnable()
		{
			_property = new SerializedProperties
			{
				LockChanges = serializedObject.FindProperty(nameof(ColliderGizmo.LockChanges)),
				Preset = serializedObject.FindProperty(nameof(ColliderGizmo.Preset)),
				IncludeChild = serializedObject.FindProperty(nameof(ColliderGizmo.IncludeChildColliders)),
				Alpha = serializedObject.FindProperty(nameof(ColliderGizmo.Alpha)),

				DrawWire = serializedObject.FindProperty(nameof(ColliderGizmo.DrawWire)),
				WireColor = serializedObject.FindProperty(nameof(ColliderGizmo.WireColor)),

				DrawFill = serializedObject.FindProperty(nameof(ColliderGizmo.DrawFill)),
				FillColor = serializedObject.FindProperty(nameof(ColliderGizmo.FillColor)),

				DrawCenter = serializedObject.FindProperty(nameof(ColliderGizmo.DrawCenter)),
				CenterColor = serializedObject.FindProperty(nameof(ColliderGizmo.CenterColor)),
				CenterRadius = serializedObject.FindProperty(nameof(ColliderGizmo.CenterMarkerRadius))
			};
		}
		
		public override void OnInspectorGUI()
		{
			if (_property.LockChanges.boolValue)
			{
				EditorGUILayout.HelpBox("This Collider Gizmo is controlled by script", MessageType.Info);
				if (GUILayout.Button("Unlock"))
				{
					_property.LockChanges.boolValue = false;
					serializedObject.ApplyModifiedProperties();
				}
				return;
			}
			EditorGUI.BeginChangeCheck();
			EditorGUILayout.PropertyField(_property.Preset, new GUIContent("Color Preset"));
			if (EditorGUI.EndChangeCheck())
			{
				serializedObject.ApplyModifiedProperties();
				foreach (var singleTarget in targets)
				{
					var cg = (ColliderGizmo)singleTarget;
					Undo.RecordObject(cg, "Collider Gizmo updated");
					cg.ChangePreset(cg.Preset);
				}
				serializedObject.ApplyModifiedProperties();
			}

			EditorGUILayout.PropertyField(_property.Alpha, new GUIContent("Overall Transparency"));
			if (GUI.changed) serializedObject.ApplyModifiedProperties();
			
			var checkWidth = EditorGUIUtility.labelWidth + 20;

			EditorGUI.BeginChangeCheck();
			using (new EditorGUILayout.HorizontalScope())
			{
				EditorGUILayout.PropertyField(_property.DrawWire, GUILayout.Width(checkWidth));
				GUI.enabled = _property.DrawWire.boolValue;
				EditorGUILayout.PropertyField(_property.WireColor, new GUIContent(""));
				GUI.enabled = true;
			}

			using (new EditorGUILayout.HorizontalScope())
			{
				EditorGUILayout.PropertyField(_property.DrawFill, GUILayout.Width(checkWidth));
				GUI.enabled = _property.DrawFill.boolValue;
				EditorGUILayout.PropertyField(_property.FillColor, new GUIContent(""));
				GUI.enabled = true;
			}

			using (new EditorGUILayout.HorizontalScope())
			{
				EditorGUILayout.PropertyField(_property.DrawCenter, GUILayout.Width(checkWidth));
				GUI.enabled = _property.DrawCenter.boolValue;
				EditorGUILayout.PropertyField(_property.CenterColor, new GUIContent(""));
				EditorGUILayout.Space(10);
				EditorGUILayout.LabelField("Ø", GUILayout.Width(16));
				EditorGUILayout.PropertyField(_property.CenterRadius, GUIContent.none, GUILayout.Width(30));
				GUI.enabled = true;
			}


			if (EditorGUI.EndChangeCheck())
			{
				var presetProp = serializedObject.FindProperty("Preset");
				presetProp.enumValueIndex = (int)ColliderGizmo.Presets.Custom;
				serializedObject.ApplyModifiedProperties();
			}

			EditorGUI.BeginChangeCheck();
			EditorGUILayout.PropertyField(_property.IncludeChild);

			if (GUI.changed) serializedObject.ApplyModifiedProperties();

			if (EditorGUI.EndChangeCheck())
			{
				foreach (var singleTarget in targets)
				{
					((ColliderGizmo)singleTarget).RefreshReferences();
				}
				serializedObject.ApplyModifiedProperties();
			}
		}
	}
}

#endif

#endregion