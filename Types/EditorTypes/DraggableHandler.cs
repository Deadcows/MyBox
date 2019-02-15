#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace MyBox.EditorTools
{
	// Found Here: 
	// http://answers.unity3d.com/questions/463207/how-do-you-make-a-custom-handle-respond-to-the-mou.html
	public class DraggableHandler
	{
		public static int LastDragHandleId;


		private static readonly int DragHandleHash = "DragHandleHash".GetHashCode();
		private const float DragHandleDoubleClickInterval = 0.5f;

		private static Vector2 _dragHandleMouseStart;
		private static Vector2 _dragHandleMouseCurrent;
		private static Vector3 _dragHandleWorldStart;
		private static float _dragHandleClickTime;
		private static int _dragHandleClickId;
		private static bool _dragHandleHasMoved;


		public enum DragHandleResult
		{
			None = 0,

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

		public static Vector3 DraggableHandle(Vector3 position, out DragHandleResult result)
		{
			return DraggableHandle(position, .5f, Quaternion.identity, Handles.SphereHandleCap, Color.green, out result);
		}

		public static Vector3 DraggableHandle(Vector3 position, float handleSize, Quaternion rotation, Handles.CapFunction capFunc, Color colorSelected,
			out DragHandleResult result, bool allowRightButton = false)
		{
			result = DragHandleResult.None;

			int id = GUIUtility.GetControlID(DragHandleHash, FocusType.Passive);
			LastDragHandleId = id;

			Vector3 screenPosition = Handles.matrix.MultiplyPoint(position);
			Matrix4x4 cachedMatrix = Handles.matrix;

			bool leftMouseUsed = Event.current.button == 0;
			bool rightMouseUsed = Event.current.button == 1;
			bool validControl = leftMouseUsed || (rightMouseUsed && allowRightButton);

			switch (Event.current.GetTypeForControl(id))
			{
				case EventType.MouseDown:
					if (HandleUtility.nearestControl == id && validControl)
					{
						GUIUtility.hotControl = id;
						Event.current.Use();
						EditorGUIUtility.SetWantsMouseJumping(1);

						result = leftMouseUsed ? DragHandleResult.LMBPress : DragHandleResult.RMBPress;

						_dragHandleMouseCurrent = _dragHandleMouseStart = Event.current.mousePosition;
						_dragHandleWorldStart = position;
						_dragHandleHasMoved = false;
					}

					break;

				case EventType.MouseUp:
					if (GUIUtility.hotControl == id && validControl)
					{
						GUIUtility.hotControl = 0;
						Event.current.Use();
						EditorGUIUtility.SetWantsMouseJumping(0);

						result = leftMouseUsed ? DragHandleResult.LMBRelease : DragHandleResult.RMBRelease;

						if (Event.current.mousePosition == _dragHandleMouseStart)
						{
							bool doubleClick = (_dragHandleClickId == id) &&
							                   (Time.realtimeSinceStartup - _dragHandleClickTime < DragHandleDoubleClickInterval);

							_dragHandleClickId = id;
							_dragHandleClickTime = Time.realtimeSinceStartup;

							if (!doubleClick) result = leftMouseUsed ? DragHandleResult.LMBClick : DragHandleResult.RMBClick;
							else result = leftMouseUsed ? DragHandleResult.LMBDoubleClick : DragHandleResult.RMBDoubleClick;
						}
					}

					break;

				case EventType.MouseDrag:
					if (GUIUtility.hotControl == id && validControl)
					{
						_dragHandleMouseCurrent += new Vector2(Event.current.delta.x, -Event.current.delta.y);
						Vector3 position2 = Camera.current.WorldToScreenPoint(Handles.matrix.MultiplyPoint(_dragHandleWorldStart))
						                    + (Vector3) (_dragHandleMouseCurrent - _dragHandleMouseStart);
						position = Handles.matrix.inverse.MultiplyPoint(Camera.current.ScreenToWorldPoint(position2));

						if (Camera.current.transform.forward == Vector3.forward || Camera.current.transform.forward == -Vector3.forward)
							position.z = _dragHandleWorldStart.z;
						if (Camera.current.transform.forward == Vector3.up || Camera.current.transform.forward == -Vector3.up)
							position.y = _dragHandleWorldStart.y;
						if (Camera.current.transform.forward == Vector3.right || Camera.current.transform.forward == -Vector3.right)
							position.x = _dragHandleWorldStart.x;

						result = leftMouseUsed ? DragHandleResult.LMBDrag : DragHandleResult.RMBDrag;

						_dragHandleHasMoved = true;

						GUI.changed = true;
						Event.current.Use();
					}

					break;

				case EventType.Repaint:
					Color currentColour = Handles.color;
					if (id == GUIUtility.hotControl && _dragHandleHasMoved)
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

	public static class DraggableHandlerExtensions
	{
		public static bool IsMousePress(this DraggableHandler.DragHandleResult result)
		{
			return result == DraggableHandler.DragHandleResult.LMBPress || result == DraggableHandler.DragHandleResult.RMBPress;
		}

		public static bool IsMouseRelease(this DraggableHandler.DragHandleResult result)
		{
			return result == DraggableHandler.DragHandleResult.LMBRelease || result == DraggableHandler.DragHandleResult.RMBRelease;
		}

		public static bool IsMouseDrag(this DraggableHandler.DragHandleResult result)
		{
			return result == DraggableHandler.DragHandleResult.LMBDrag || result == DraggableHandler.DragHandleResult.RMBDrag;
		}
	}
}
#endif