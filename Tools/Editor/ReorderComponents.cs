using UnityEngine;
using UnityEditor;

/// <summary>
/// Editor Window to reorder components of selected object
/// </summary>
[InitializeOnLoad]
[ExecuteInEditMode]
public class ReorderComponents : EditorWindow
{

	// ReSharper disable once UnusedMember.Local
	[MenuItem("Tools/Reorder Components")]
	private static void ShowWindow()
	{
		EditorWindow windowHndle = GetWindow(typeof(ReorderComponents));
		windowHndle.autoRepaintOnSceneChange = true;
	}

	// ReSharper disable once UnusedMember.Local
	private void OnInspectorUpdate()
	{
		Repaint();
	}


	#region Styles

	private static Texture2D ButtonActiveBackground
	{
		get
		{
			Texture2D texture = new Texture2D(1, 1);
			texture.SetPixel(0, 0, Color.gray);
			texture.Apply();
			return texture;
		}
	}

	private static GUIStyle ButtonActiveStyle
	{
		get
		{
			GUIStyle style = new GUIStyle();
			style.alignment = TextAnchor.MiddleLeft;
			style.fixedHeight = 25;
			style.margin = new RectOffset(4, 4, 4, 4);
			style.normal.background = ButtonActiveBackground;
			style.normal.textColor = Color.gray;
			return style;
		}
	}

	private static GUIStyle ButtonStyle
	{
		get
		{
			GUIStyle style = new GUIStyle(EditorStyles.toolbarButton);
			style.alignment = TextAnchor.MiddleLeft;
			style.fixedHeight = 25;
			style.margin = new RectOffset(4, 4, 4, 4);
			return style;
		}
	}

	private static GUIStyle ButtonDisabledStyle
	{
		get
		{
			GUIStyle style = new GUIStyle(EditorStyles.toolbarButton);
			style.alignment = TextAnchor.MiddleLeft;
			style.fixedHeight = 25;
			style.margin = new RectOffset(4, 4, 4, 4);
			style.normal.textColor = Color.gray;
			return style;
		}
	}

	#endregion


	private int _activeButton;
	private int _tempButtonIndex;
	private int _lastButtonIndex;
	private bool _mouseDown;
	private int[] _newIndexes;

	// ReSharper disable once UnusedMember.Local
	private void OnGUI()
	{
		Transform currentTransform = Selection.activeTransform;
		if (currentTransform == null) return;

		Component[] comps = currentTransform.GetComponents<Component>();

		
		// Check if mouse button gets pressed down and see which button is below mouse
		if (!_mouseDown && Event.current.type == EventType.MouseDown)
		{
			_activeButton = Mathf.FloorToInt(Event.current.mousePosition.y / 29);
			
			bool transformClick = comps[_activeButton].GetType().ToString() == "UnityEngine.Transform";

			if (transformClick)
			{
				_mouseDown = false;
			}
			else
			{
				_mouseDown = true;
				_lastButtonIndex = _activeButton;

				_newIndexes = new int[comps.Length];
				for (int i = 0; i < comps.Length; i++)
				{
					_newIndexes[i] = i;
				}
			}
		}

		// Mouse button released
		if (_mouseDown && Event.current.type == EventType.MouseUp)
		{
			_mouseDown = false;

			// Reorder components
			int positionsToMove = Mathf.RoundToInt(Mathf.Abs(_tempButtonIndex - _activeButton));

			if (positionsToMove > 0)
			{
				int direction = (_tempButtonIndex - _activeButton) / Mathf.Abs(_tempButtonIndex - _activeButton);

				for (int i = 0; i < positionsToMove; i++)
				{
					if (direction > 0)
					{
						UnityEditorInternal.ComponentUtility.MoveComponentDown(comps[_activeButton + i]);
					}
					if (direction < 0)
					{
						UnityEditorInternal.ComponentUtility.MoveComponentUp(comps[_activeButton - i]);
					}

					comps = currentTransform.GetComponents<Component>();
				}
			}
		}


		// Draw buttons
		for (int i = 0; i < comps.Length; i++)
		{
			int j = i;

			if (_mouseDown)
			{
				j = _newIndexes[i];
			}

			string componentName = comps[j].GetType().ToString();

			GUIStyle style = ButtonStyle;

			if (_mouseDown && i == _tempButtonIndex) { style = ButtonActiveStyle; }
			if (componentName == "UnityEngine.Transform") { style = ButtonDisabledStyle; }

			GUILayout.Button(componentName, style);
		}

		// If mouse button is down, draw the extra button
		if (_mouseDown)
		{
			Rect buttonPosition = new Rect(Event.current.mousePosition.x - Screen.width / 2f, Event.current.mousePosition.y - 12.5f, Screen.width - 10, 25);
			GUI.Button(buttonPosition, comps[_activeButton].GetType().ToString(), ButtonStyle);
		}
		

		// Get index for the new temp button
		if (_mouseDown)
		{
			int tmp = Mathf.FloorToInt(Event.current.mousePosition.y / 29);

			if (tmp > comps.Length - 1)
			{
				tmp = comps.Length - 1;
			}

			if (tmp < 0)
			{
				tmp = 0;
			}

			if (comps[tmp].GetType().ToString() != "UnityEngine.Transform")
			{
				_tempButtonIndex = tmp;
			}

			if (_tempButtonIndex != _lastButtonIndex)
			{
				int temp = _newIndexes[_lastButtonIndex];
				_newIndexes[_lastButtonIndex] = _newIndexes[_tempButtonIndex];
				_newIndexes[_tempButtonIndex] = temp;

				_lastButtonIndex = _tempButtonIndex;
			}
		}

	}
}
