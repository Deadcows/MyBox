#if UNITY_EDITOR
using System;
using JetBrains.Annotations;
using UnityEngine;
using UnityEditor;

namespace MyBox.EditorTools
{
	/// <summary>
	/// Handles mouse click on scene objects in edit mode.
	/// Set up event, Single Click, Optionally - Cancellable and LayerMask
	/// Enable handler when needed (or call ToggleState)
	/// </summary>
	[PublicAPI]
	public class SceneClickHandler
	{
		/// <param name="onClick">Event called onClick on the scene</param>
		/// <param name="plane">Plane to read the input</param>
		/// <param name="singleClick">Single click handler will deactivate itself on click</param>
		/// <param name="cancellable">Cancellable handler will listen for Escape key to set Enabled to false</param>
		public SceneClickHandler(Action<Vector3> onClick, Plane plane, bool singleClick = false, bool cancellable = true)
			: this(onClick, singleClick, cancellable)
		{
			_usePlane = true;
			_plane = plane;
		}

		/// <param name="onClick">Event called onClick on the scene</param>
		/// <param name="singleClick">Single click handler will deactivate itself on click</param>
		/// <param name="cancellable">Cancellable handler will listen for Escape key to set Enabled to false</param>
		/// <param name="physics2d">Use Physics2d raycast instead of Physics</param>
		public SceneClickHandler(Action<Vector3> onClick, bool singleClick = false, bool cancellable = true,
			bool physics2d = false)
		{
			_onClick = onClick;

			SingleClickHandler = singleClick;
			UsePhysics2D = physics2d;
			Cancellable = cancellable;

#pragma warning disable 618
			SceneView.onSceneGUIDelegate += OnSceneGUI;
#pragma warning restore 618
		}

		~SceneClickHandler()
		{
#pragma warning disable 618
			SceneView.onSceneGUIDelegate -= OnSceneGUI;
#pragma warning restore 618
		}


		public bool Enabled
		{
			private get => _enabled;
			set
			{
				if (value) FocusSceneView();
				_enabled = value;
			}
		}

		// We need to focus on SceneView to handle cancellation
		// (OnSceneGUI not handling keyboard events otherwise)
		private void FocusSceneView()
		{
			if (SceneView.sceneViews.Count == 0) return;
			((SceneView)SceneView.sceneViews[0]).Focus();
		}

		/// <summary>
		/// Cancellable handler will listen for Escape key to set Enabled to false
		/// </summary>
		public bool Cancellable { get; set; }

		/// <summary>
		/// Single click handler will deactivate itself on click
		/// </summary>
		public bool SingleClickHandler { get; set; }
		
		public bool UsePhysics2D { get; set; }


		public void SetLayerMask(LayerMask mask) => LayerMask = mask;

		public void ToggleState() => Enabled = !Enabled;
		
		public Color HandleColor { get; set; } = Color.white;
		public float HandleRadius { get; set; } = .3f;
		
		public float CastRadius { get; set; }

		public LayerMask? LayerMask { get; set; }


		private readonly Action<Vector3> _onClick;

		private bool _enabled;
		private bool _usePlane;
		private Plane _plane;


		private void OnSceneGUI(SceneView sceneview)
		{
			if (!Enabled) return;

			var point = _usePlane ? GetPlanePoint() : UsePhysics2D ? GetRaycast2dPoint() : GetRaycastPoint();

			if (point.Hit != null)
			{
				var color = Handles.color;
				Handles.color = HandleColor;
				Handles.DrawWireDisc(point.Hit.Value, point.Normal, HandleRadius);
				Handles.color = color;

				if (Handles.Button(Vector3.zero, SceneView.currentDrawingSceneView.rotation, 30, 5000,
					    Handles.RectangleHandleCap))
					HandleClick(point.Hit.Value);
			}


			if (EscapeInput() && Cancellable) Enabled = false;
			return;


			(Vector3? Hit, Vector3 Normal) GetRaycastPoint()
			{
#if UNITY_PHYSICS_ENABLED
				var ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);

				if (CastRadius > 0)
				{
					if (LayerMask != null ? 
						    Physics.SphereCast(ray, CastRadius, out var hit, float.PositiveInfinity, LayerMask.Value.value) : 
						    Physics.SphereCast(ray, CastRadius, out hit))
						return (hit.point, hit.normal);
				}
				else
				{
					if (LayerMask != null ? 
						    Physics.Raycast(ray, out var hit, LayerMask.Value.value) : 
						    Physics.Raycast(ray, out hit))
						return (hit.point, hit.normal);
				}
				
#else
				WarningsPool.LogWarning("SceneClickHandler caused: PHYSICS is not enabled. Use Physics2d or Plane mode instead");
#endif		
				return (null, Vector3.zero);
			}
			
			(Vector3? Hit, Vector3 Normal) GetRaycast2dPoint()
			{
#if UNITY_PHYSICS2D_ENABLED
				var ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
				var hit = LayerMask != null
					? Physics2D.Raycast(ray.origin, ray.direction, LayerMask.Value.value)
					: Physics2D.Raycast(ray.origin, ray.direction);
				
				if (hit.collider != null) return (hit.point, hit.normal);
#else
				WarningsPool.LogWarning("SceneClickHandler caused: PHYSICS2D is not enabled. Use Physics or Plane mode instead");
#endif
				return (null, Vector3.zero);
			}
			
			(Vector3? Hit, Vector3 Normal) GetPlanePoint()
			{
				var ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
				if (_plane.Raycast(ray, out var enter)) return (ray.GetPoint(enter), _plane.normal);
				
				return (null, Vector3.zero);
			}
			
			bool EscapeInput() => Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.Escape;
			
			void HandleClick(Vector3 clickPoint)
			{
				if (SingleClickHandler) Enabled = false;

				_onClick?.Invoke(clickPoint);

				Event.current.Use();
				HandleUtility.Repaint();
			}
		}
	}
}
#endif