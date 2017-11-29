using UnityEditor;
using UnityEngine;

// Found Here: 
// http://answers.unity3d.com/questions/463207/how-do-you-make-a-custom-handle-respond-to-the-mou.html
public class DraggableHandler
{
	// internal state for DragHandle()
	static readonly int s_DragHandleHash = "DragHandleHash".GetHashCode();
	static Vector2 s_DragHandleMouseStart;
	static Vector2 s_DragHandleMouseCurrent;
	static Vector3 s_DragHandleWorldStart;
	static float s_DragHandleClickTime;
	static int s_DragHandleClickID;
	static readonly float s_DragHandleDoubleClickInterval = 0.5f;
	static bool s_DragHandleHasMoved;

	// externally accessible to get the ID of the most resently processed DragHandle
	public static int lastDragHandleID;

	public enum DragHandleResult
	{
		none = 0,

		LMBPress,
		LMBClick,
		LMBDoubleClick,
		LMBDrag,
		LMBRelease,

		RMBPress,
		RMBClick,
		RMBDoubleClick,
		RMBDrag,
		RMBRelease,
	};

	public static Vector3 DraggableHandle(Vector3 position, float handleSize, Quaternion rotation, Handles.CapFunction capFunc, Color colorSelected, out DragHandleResult result)
	{
		int id = GUIUtility.GetControlID(s_DragHandleHash, FocusType.Passive);
		lastDragHandleID = id;

		Vector3 screenPosition = Handles.matrix.MultiplyPoint(position);
		Matrix4x4 cachedMatrix = Handles.matrix;

		result = DragHandleResult.none;

		switch (Event.current.GetTypeForControl(id))
		{
			case EventType.MouseDown:
				if (HandleUtility.nearestControl == id && (Event.current.button == 0 || Event.current.button == 1))
				{
					GUIUtility.hotControl = id;
					s_DragHandleMouseCurrent = s_DragHandleMouseStart = Event.current.mousePosition;
					s_DragHandleWorldStart = position;
					s_DragHandleHasMoved = false;

					Event.current.Use();
					EditorGUIUtility.SetWantsMouseJumping(1);

					if (Event.current.button == 0)
						result = DragHandleResult.LMBPress;
					else if (Event.current.button == 1)
						result = DragHandleResult.RMBPress;
				}
				break;

			case EventType.MouseUp:
				if (GUIUtility.hotControl == id && (Event.current.button == 0 || Event.current.button == 1))
				{
					GUIUtility.hotControl = 0;
					Event.current.Use();
					EditorGUIUtility.SetWantsMouseJumping(0);

					if (Event.current.button == 0)
						result = DragHandleResult.LMBRelease;
					else if (Event.current.button == 1)
						result = DragHandleResult.RMBRelease;

					if (Event.current.mousePosition == s_DragHandleMouseStart)
					{
						bool doubleClick = (s_DragHandleClickID == id) &&
							(Time.realtimeSinceStartup - s_DragHandleClickTime < s_DragHandleDoubleClickInterval);

						s_DragHandleClickID = id;
						s_DragHandleClickTime = Time.realtimeSinceStartup;

						if (Event.current.button == 0)
							result = doubleClick ? DragHandleResult.LMBDoubleClick : DragHandleResult.LMBClick;
						else if (Event.current.button == 1)
							result = doubleClick ? DragHandleResult.RMBDoubleClick : DragHandleResult.RMBClick;
					}
				}
				break;

			case EventType.MouseDrag:
				if (GUIUtility.hotControl == id)
				{
					s_DragHandleMouseCurrent += new Vector2(Event.current.delta.x, -Event.current.delta.y);
					Vector3 position2 = Camera.current.WorldToScreenPoint(Handles.matrix.MultiplyPoint(s_DragHandleWorldStart))
						+ (Vector3)(s_DragHandleMouseCurrent - s_DragHandleMouseStart);
					position = Handles.matrix.inverse.MultiplyPoint(Camera.current.ScreenToWorldPoint(position2));

					if (Camera.current.transform.forward == Vector3.forward || Camera.current.transform.forward == -Vector3.forward)
						position.z = s_DragHandleWorldStart.z;
					if (Camera.current.transform.forward == Vector3.up || Camera.current.transform.forward == -Vector3.up)
						position.y = s_DragHandleWorldStart.y;
					if (Camera.current.transform.forward == Vector3.right || Camera.current.transform.forward == -Vector3.right)
						position.x = s_DragHandleWorldStart.x;

					if (Event.current.button == 0)
						result = DragHandleResult.LMBDrag;
					else if (Event.current.button == 1)
						result = DragHandleResult.RMBDrag;

					s_DragHandleHasMoved = true;

					GUI.changed = true;
					Event.current.Use();
				}
				break;

			case EventType.Repaint:
				Color currentColour = Handles.color;
				if (id == GUIUtility.hotControl && s_DragHandleHasMoved)
					Handles.color = colorSelected;

				Handles.matrix = Matrix4x4.identity;
				capFunc(id, screenPosition, rotation, handleSize, EventType.Repaint);
				Handles.matrix = cachedMatrix;

				Handles.color = currentColour;
				break;

			case EventType.Layout:
				Handles.matrix = Matrix4x4.identity;
				HandleUtility.AddControl(id, HandleUtility.DistanceToCircle(screenPosition, handleSize));
				Handles.matrix = cachedMatrix;
				break;
		}

		return position;
	}
}
