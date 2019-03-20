// ---------------------------------------------------------------------------- 
// Author: Matthew Miner
// https://github.com/mminer/scene-view-bookmarks/blob/master/SceneViewBookmarker.cs
// Date:   26/01/2018
// ----------------------------------------------------------------------------

#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEngine;

namespace MyBox.Internal
{
	[Serializable]
	internal struct SceneViewBookmark
	{
		public Vector3 Position;
		public Quaternion Rotation;
		public float Size;

		public SceneViewBookmark(SceneView sceneView)
		{
			Position = sceneView.pivot;
			Rotation = sceneView.rotation;
			Size = sceneView.size;
		}
	}

	internal static class SceneViewBookmarkHotkeys
	{
		private const int UndoSlot = 0;

		private static void BookmarkSceneView(int slot)
		{
			var bookmark = new SceneViewBookmark(SceneView.lastActiveSceneView);
			
			var key = GetKey(slot);
			var json = JsonUtility.ToJson(bookmark);
			EditorPrefs.SetString(key, json);

			if (slot != UndoSlot) Debug.Log("Scene view bookmarked in slot " + slot + ".");
		}

		private static void RestoreSceneBookmark(int slot)
		{
			// Bookmark the current scene view so that we can easily return to it later.
			if (slot != UndoSlot) BookmarkSceneView(UndoSlot);
			
			var key = GetKey(slot);
			var json = EditorPrefs.GetString(key);
			var bookmark = JsonUtility.FromJson<SceneViewBookmark>(json);

			var sceneView = SceneView.lastActiveSceneView;
			sceneView.pivot = bookmark.Position;
			sceneView.rotation = bookmark.Rotation;
			sceneView.size = bookmark.Size;
			sceneView.Repaint();
		}
		
		private static bool BookmarkExists(int slot)
		{
			return EditorPrefs.HasKey(GetKey(slot));
		}

		private static string GetKey(int slot)
		{
			return "SceneViewBookmark" + slot;
		}

		
		#region Menu Items

		[MenuItem("Tools/MyBox/Bookmark Scene View 1 &1", false, 5000)]
		private static void BookmarkSceneView1()
		{
			BookmarkSceneView(1);
		}

		[MenuItem("Tools/MyBox/Bookmark Scene View 2 &2", false, 5000)]
		private static void BookmarkSceneView2()
		{
			BookmarkSceneView(2);
		}

		[MenuItem("Tools/MyBox/Bookmark Scene View 3 &3", false, 5000)]
		private static void BookmarkSceneView3()
		{
			BookmarkSceneView(3);
		}

		[MenuItem("Tools/MyBox/Bookmark Scene View 4 &4", false, 5000)]
		private static void BookmarkSceneView4()
		{
			BookmarkSceneView(4);
		}

		[MenuItem("Tools/MyBox/Bookmark Scene View 5 &5", false, 5000)]
		private static void BookmarkSceneView5()
		{
			BookmarkSceneView(5);
		}

		[MenuItem("Tools/MyBox/Bookmark Scene View 6 &6", false, 5000)]
		private static void BookmarkSceneView6()
		{
			BookmarkSceneView(6);
		}

		[MenuItem("Tools/MyBox/Bookmark Scene View 7 &7", false, 5000)]
		private static void BookmarkSceneView7()
		{
			BookmarkSceneView(7);
		}

		[MenuItem("Tools/MyBox/Bookmark Scene View 8 &8", false, 5000)]
		private static void BookmarkSceneView8()
		{
			BookmarkSceneView(8);
		}

		[MenuItem("Tools/MyBox/Bookmark Scene View 9 &9", false, 5000)]
		private static void BookmarkSceneView9()
		{
			BookmarkSceneView(9);
		}

		[MenuItem("Tools/MyBox/Move Scene View To Bookmark 1 #1", false, 5001)]
		private static void MoveSceneViewToBookmark1()
		{
			RestoreSceneBookmark(1);
		}

		[MenuItem("Tools/MyBox/Move Scene View To Bookmark 2 #2", false, 5001)]
		private static void MoveSceneViewToBookmark2()
		{
			RestoreSceneBookmark(2);
		}

		[MenuItem("Tools/MyBox/Move Scene View To Bookmark 3 #3", false, 5001)]
		private static void MoveSceneViewToBookmark3()
		{
			RestoreSceneBookmark(3);
		}

		[MenuItem("Tools/MyBox/Move Scene View To Bookmark 4 #4", false, 5001)]
		private static void MoveSceneViewToBookmark4()
		{
			RestoreSceneBookmark(4);
		}

		[MenuItem("Tools/MyBox/Move Scene View To Bookmark 5 #5", false, 5001)]
		private static void MoveSceneViewToBookmark5()
		{
			RestoreSceneBookmark(5);
		}

		[MenuItem("Tools/MyBox/Move Scene View To Bookmark 6 #6", false, 5001)]
		private static void MoveSceneViewToBookmark6()
		{
			RestoreSceneBookmark(6);
		}

		[MenuItem("Tools/MyBox/Move Scene View To Bookmark 7 #7", false, 5001)]
		private static void MoveSceneViewToBookmark7()
		{
			RestoreSceneBookmark(7);
		}

		[MenuItem("Tools/MyBox/Move Scene View To Bookmark 8 #8", false, 5001)]
		private static void MoveSceneViewToBookmark8()
		{
			RestoreSceneBookmark(8);
		}

		[MenuItem("Tools/MyBox/Move Scene View To Bookmark 9 #9", false, 5001)]
		private static void MoveSceneViewToBookmark9()
		{
			RestoreSceneBookmark(9);
		}

		[MenuItem("Tools/MyBox/Return To Previous Scene View #0", false, 5002)]
		private static void ReturnToPreviousSceneView()
		{
			RestoreSceneBookmark(UndoSlot);
		}

		

		[MenuItem("Tools/MyBox/Move Scene View To Bookmark 1 #1", true)]
		private static bool ValidateMoveSceneViewToBookmark1()
		{
			return BookmarkExists(1);
		}

		[MenuItem("Tools/MyBox/Move Scene View To Bookmark 2 #2", true)]
		private static bool ValidateMoveSceneViewToBookmark2()
		{
			return BookmarkExists(2);
		}

		[MenuItem("Tools/MyBox/Move Scene View To Bookmark 3 #3", true)]
		private static bool ValidateMoveSceneViewToBookmark3()
		{
			return BookmarkExists(3);
		}

		[MenuItem("Tools/MyBox/Move Scene View To Bookmark 4 #4", true)]
		private static bool ValidateMoveSceneViewToBookmark4()
		{
			return BookmarkExists(4);
		}

		[MenuItem("Tools/MyBox/Move Scene View To Bookmark 5 #5", true)]
		private static bool ValidateMoveSceneViewToBookmark5()
		{
			return BookmarkExists(5);
		}

		[MenuItem("Tools/MyBox/Move Scene View To Bookmark 6 #6", true)]
		private static bool ValidateMoveSceneViewToBookmark6()
		{
			return BookmarkExists(6);
		}

		[MenuItem("Tools/MyBox/Move Scene View To Bookmark 7 #7", true)]
		private static bool ValidateMoveSceneViewToBookmark7()
		{
			return BookmarkExists(7);
		}

		[MenuItem("Tools/MyBox/Move Scene View To Bookmark 8 #8", true)]
		private static bool ValidateMoveSceneViewToBookmark8()
		{
			return BookmarkExists(8);
		}

		[MenuItem("Tools/MyBox/Move Scene View To Bookmark 9 #9", true)]
		private static bool ValidateMoveSceneViewToBookmark9()
		{
			return BookmarkExists(9);
		}

		[MenuItem("Tools/MyBox/Return To Previous Scene View #0", true)]
		private static bool ValidateReturnToPreviousSceneView()
		{
			return BookmarkExists(UndoSlot);
		}

		#endregion
	}
}
#endif