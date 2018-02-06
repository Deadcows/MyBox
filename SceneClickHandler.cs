using System;
using UnityEngine;
using UnityEditor;

public class SceneClickHandler
{
	public SceneClickHandler(Action<Vector3> onClick, bool singleClick)
	{
		_singleClickHandler = singleClick;
		_onClick = onClick;

		SceneView.onSceneGUIDelegate += OnSceneGUI;
	}
	
	~SceneClickHandler()
	{
		SceneView.onSceneGUIDelegate -= OnSceneGUI;
	}

	public void ToggleState() => Enabled = !Enabled;
	public bool Enabled { get; set; }

	private readonly bool _singleClickHandler;
	private readonly Action<Vector3> _onClick;

	private void OnSceneGUI(SceneView sceneview)
	{
		if (!Enabled) return;

		RaycastHit hit;
		var ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
		if (Physics.Raycast(ray, out hit))
		{
			Handles.DrawWireCube(hit.point, Vector3.one * .2f);
		}

		if (Handles.Button(Vector3.zero, SceneView.currentDrawingSceneView.rotation, 30, 5000, Handles.RectangleHandleCap))
		{
			if (_singleClickHandler) Enabled = false;

			_onClick(hit.point);
			
			Event.current.Use();
			HandleUtility.Repaint();
		}
	}

}
