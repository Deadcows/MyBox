#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

namespace MyBox.Internal
{
	public static class NodeEditorUtils
	{
		public static GUIStyle SplitterStyle
		{
			get
			{
				if (_splitterStyle == null)
				{
					_splitterStyle = new GUIStyle
					{
						normal = {background = EditorGUIUtility.whiteTexture},
						stretchWidth = true,
						margin = new RectOffset(0, 0, 7, 7)
					};
				}
				
				return _splitterStyle;
			}
		}
		private static GUIStyle _splitterStyle;

		public static GUIStyle LineStyle
		{
			get
			{
				if (_lineStyle == null)
				{
					_lineStyle = new GUIStyle
					{
						normal = {background = EditorGUIUtility.whiteTexture},
						stretchWidth = true,
						margin = new RectOffset(0, 0, 0, 0)
					};
				}

				return _lineStyle;
			}
		}
		private static GUIStyle _lineStyle;

		
		private static readonly Color SplitterColor = EditorGUIUtility.isProSkin ? new Color(0.157f, 0.157f, 0.157f) : new Color(0.5f, 0.5f, 0.5f);
		
		
		/// <summary>
		/// Draws a curve from the start to the end rectangles.
		/// </summary>
		public static void DrawNodeCurve(Rect start, Rect end)
		{
			var endPos = new Vector3(end.x, end.y + end.height / 2, 0);

			DrawNodeCurve(start, endPos);
		}

		/// <summary>
		/// Draws a curve from the start to the end position.
		/// </summary>
		public static void DrawNodeCurve(Rect start, Vector3 endPosition)
		{
			var startPosition = new Vector3(start.x + start.width, start.y + start.height / 2, 0);
			var startTangent = startPosition + Vector3.right * 50;
			var endTangent = endPosition + Vector3.left * 50;
			var shadowColor = new Color(0, 0, 0, 0.06f);

			for (int i = 0; i < 3; i++)
			{
				// Draw a shadow
				Handles.DrawBezier(startPosition, endPosition, startTangent, endTangent, shadowColor, null, (i + 1) * 5);
			}

			Handles.DrawBezier(startPosition, endPosition, startTangent, endTangent, Color.black, null, 1);

			var oldColor = Handles.color;

			Handles.color = new Color(.5f, 0.1f, 0.1f);

			Handles.DrawSolidDisc(
				(startPosition + endPosition) / 2,
				Vector3.forward,
				5);

			Handles.color = Color.black;

			Handles.DrawWireDisc(
				(startPosition + endPosition) / 2,
				Vector3.forward,
				5);

			Handles.color = oldColor;
		}


		// GUILayout Style
		public static void Splitter(Color rgb, float thickness = 1)
		{
			Rect position = GUILayoutUtility.GetRect(GUIContent.none, SplitterStyle, GUILayout.Height(thickness));

			if (Event.current.type == EventType.Repaint)
			{
				Color restoreColor = GUI.color;
				GUI.color = rgb;
				SplitterStyle.Draw(position, false, false, false, false);
				GUI.color = restoreColor;
			}
		}

		public static void Splitter(float thickness, GUIStyle splitterStyle)
		{
			Rect position = GUILayoutUtility.GetRect(GUIContent.none, splitterStyle, GUILayout.Height(thickness));

			if (Event.current.type == EventType.Repaint)
			{
				Color restoreColor = GUI.color;
				GUI.color = SplitterColor;
				splitterStyle.Draw(position, false, false, false, false);
				GUI.color = restoreColor;
			}
		}

		public static void VerticalLine()
		{
			VerticalLine(SplitterColor, 2);
		}

		public static void VerticalLine(Color color, float thickness = 1)
		{
			Rect position = GUILayoutUtility.GetRect(
				GUIContent.none,
				LineStyle,
				GUILayout.Width(thickness),
				GUILayout.ExpandHeight(true));

			if (Event.current.type == EventType.Repaint)
			{
				Color restoreColor = GUI.color;
				GUI.color = color;
				LineStyle.Draw(position, false, false, false, false);
				GUI.color = restoreColor;
			}
		}

		public static void VerticalLine(float thickness, GUIStyle splitterStyle)
		{
			Rect position = GUILayoutUtility.GetRect(GUIContent.none, splitterStyle, GUILayout.Width(thickness));

			if (Event.current.type == EventType.Repaint)
			{
				Color restoreColor = GUI.color;
				GUI.color = SplitterColor;
				splitterStyle.Draw(position, false, false, false, false);
				GUI.color = restoreColor;
			}
		}

		public static void Splitter(float thickness = 1)
		{
			Splitter(thickness, SplitterStyle);
		}

		// GUI Style
		public static void Splitter(Rect position)
		{
			if (Event.current.type == EventType.Repaint)
			{
				Color restoreColor = GUI.color;
				GUI.color = SplitterColor;
				SplitterStyle.Draw(position, false, false, false, false);
				GUI.color = restoreColor;
			}
		}
	}
}
#endif