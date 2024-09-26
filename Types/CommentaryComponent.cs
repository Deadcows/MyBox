using System;
using UnityEngine;

namespace MyBox.Internal
{
	public class CommentaryComponent : MonoBehaviour
	{
#if UNITY_EDITOR
		[Serializable]
		public struct Entry
		{
			public string EditorCommentary;
			public UnityEditor.MessageType Type;
		}

		public Entry[] Entries;
#endif
	}
}

#if UNITY_EDITOR
namespace MyBox.Internal
{
	using UnityEditor;
	using EditorTools;

	[CustomEditor(typeof(CommentaryComponent))]
	public class CommentaryDrawer : Editor
	{
		private CommentaryComponent _commentary;
		private GUIContent _boxContent;

		private bool _editMode;

		public override void OnInspectorGUI()
		{
			if (_commentary == null) _commentary = (CommentaryComponent)target;
			if (_commentary.Entries == null)
			{
				_commentary.Entries = Array.Empty<CommentaryComponent.Entry>();
				EditorUtility.SetDirty(_commentary);
			}

			var displayMode = !_editMode && _commentary.Entries.Length > 0;
			if (displayMode) DrawCommentariesDisplayMode();
			else DrawCommentariesEditMode();
		}

		private void DrawCommentariesDisplayMode()
		{
			var e = Event.current;
			var mousePosition = e.mousePosition;
			var isClick = e.button == 0 && e.isMouse;

			bool firstEntry = true;
			foreach (var entry in _commentary.Entries)
			{
				if (!firstEntry) EditorGUILayout.Space(2);
				firstEntry = false;
				EditorGUILayout.HelpBox(entry.EditorCommentary, entry.Type);
				if (GUILayoutUtility.GetLastRect().Contains(mousePosition) && isClick) _editMode = true;
			}
		}
		
		private void DrawCommentariesEditMode()
		{
			for (var i = 0; i < _commentary.Entries.Length; i++)
			{
				var entry = _commentary.Entries[i];
				using (new EditorGUILayout.HorizontalScope())
				{
					using (new GUILayout.VerticalScope(GUILayout.Width(40)))
					{
						GUILayout.Space(4);
						if (GUILayout.Button(GetIcon(entry.Type), EditorStyles.helpBox, GUILayout.Width(40), GUILayout.Height(36)))
						{
							_commentary.Entries[i].Type = NextType(entry.Type);
						}

						if (GUILayout.Button(MyGUI.Characters.Cross, GUILayout.Width(40)))
						{
							var index = i;
							EditorApplication.delayCall += () =>
							{
								_commentary.Entries = _commentary.Entries.RemoveAt(index);
								EditorUtility.SetDirty(_commentary);
								Repaint();
							};
						}
					}

					_commentary.Entries[i].EditorCommentary = EditorGUILayout.TextArea(entry.EditorCommentary, EditorStyles.helpBox);
				}
			}

			EditorGUILayout.Space();
			using (new GUILayout.HorizontalScope())
			{
				if (_commentary.Entries.Length > 0 && GUILayout.Button(MyGUI.Characters.Check, GUILayout.Width(40))) _editMode = false;
				GUILayout.FlexibleSpace();
				if (GUILayout.Button("+", GUILayout.Width(40)))
				{
					Array.Resize(ref _commentary.Entries, _commentary.Entries.Length + 1);
					_editMode = true;
					EditorUtility.SetDirty(_commentary);
				}
			}


			if (GUI.changed) EditorUtility.SetDirty(target);
		}

		private GUIContent GetIcon(MessageType type)
		{
			if (type == MessageType.Info) return EditorGUIUtility.IconContent("console.infoicon");
			if (type == MessageType.Warning) return EditorGUIUtility.IconContent("console.warnicon");
			if (type == MessageType.Error) return EditorGUIUtility.IconContent("console.erroricon");
			return new GUIContent("No icon");
		}

		private MessageType NextType(MessageType type)
		{
			if (type == MessageType.Info) return MessageType.Warning;
			if (type == MessageType.Warning) return MessageType.Error;
			if (type == MessageType.Error) return MessageType.None;
			return MessageType.Info;
		}
	}
}
#endif