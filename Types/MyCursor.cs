using System;
using UnityEngine;

namespace MyBox
{
	[Serializable]
	public struct MyCursor
	{
		public Texture2D Texture;
		public Vector2 Hotspot;

		public void ApplyAsLockedCursor()
		{
			Cursor.visible = true;
			Cursor.lockState = CursorLockMode.Locked;
			Cursor.SetCursor(Texture, Hotspot, CursorMode.ForceSoftware);
		}
		
		public void ApplyAsFreeCursor()
		{
			Cursor.visible = true;
			Cursor.lockState = CursorLockMode.None;
			Cursor.SetCursor(Texture, Hotspot, CursorMode.ForceSoftware);
		}
		
		public void ApplyAsConfinedCursor()
		{
			Cursor.visible = true;
			Cursor.lockState = CursorLockMode.Confined;
			Cursor.SetCursor(Texture, Hotspot, CursorMode.ForceSoftware);
		}
	}
}

#if UNITY_EDITOR
namespace MyBox.Internal
{
	using UnityEngine;
	using UnityEditor;

	//[CustomPropertyDrawer(typeof(MyCursor), true)]
	public class MyCursorPropertyDrawer : PropertyDrawer
	{
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			var spriteRect = position;
			spriteRect.width -= 56;
			var buttonRect = position;
			buttonRect.width = 50;
			buttonRect.x += spriteRect.width;
			
			EditorGUI.PropertyField(spriteRect, property.FindPropertyRelative("Sprite"), label);
			if (GUI.Button(buttonRect, "Pop"))
			{
				PopupWindow.Show(position, new PopupExample(property));
			}
		}
	}
	
	public class PopupExample : PopupWindowContent
	{
		private readonly SerializedProperty _sprite;
		private readonly SerializedProperty _hotspot;
		
		public PopupExample(SerializedProperty property)
		{
			_sprite = property.FindPropertyRelative("Sprite");
			_hotspot = property.FindPropertyRelative("Hotspot");
		}

		public override Vector2 GetWindowSize()
		{
			return new Vector2(200, 150);
		}

		public override void OnGUI(Rect rect)
		{
			 //var cursorStyle = new GUIStyle();
			// cursorStyle.fixedWidth = 128;
			// cursorStyle.fixedHeight = 128;
			// cursorStyle.border = new RectOffset(2, 2, 2, 2);

			GUILayout.Label(_sprite.objectReferenceValue as Texture2D, GUILayout.Width(128), GUILayout.Height(128));
			Event e = Event.current;
			if (e.type == EventType.MouseDrag)
			{
				var buttonRect = GUILayoutUtility.GetLastRect();
				var pos = Event.current.mousePosition;
				var inside = new Vector2(pos.x - buttonRect.x, pos.y - buttonRect.y);
				_hotspot.vector2Value = inside;
				_hotspot.serializedObject.ApplyModifiedProperties();
				
				editorWindow.Repaint();
				Debug.Log(inside);
				Debug.Log(rect);
			}
			
			GUI.backgroundColor = Color.magenta;
			GUI.contentColor = Color.magenta;
			GUI.Label(new Rect(_hotspot.vector2Value.x, _hotspot.vector2Value.y, 14, 14), Texture2D.whiteTexture);
		}

		public override void OnOpen()
		{
			Debug.Log("Popup opened: " + this);
		}

		public override void OnClose()
		{
			Debug.Log("Popup closed: " + this);
		}
	}
}
#endif
