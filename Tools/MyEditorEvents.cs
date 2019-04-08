#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEngine;

namespace MyBox.EditorTools
{
	[InitializeOnLoad]
	public class MyEditorEvents : UnityEditor.AssetModificationProcessor
	{
		/// <summary>
		/// Occurs on Scenes/Assets Save
		/// </summary>
		public static Action OnSave;

		/// <summary>
		/// Occurs on first frame in Playmode
		/// </summary>
		public static Action OnFirstFrame;


		#region OnSave

		private static string[] OnWillSaveAssets(string[] paths)
		{
			// Prefab creation enforces SaveAsset and this may cause unwanted dir cleanup
			if (paths.Length == 1 && paths[0].EndsWith(".prefab")) return paths;

			if (OnSave != null) OnSave();

			return paths;
		}

		#endregion


		#region OnFirstFrame

		static MyEditorEvents()
		{
			EditorApplication.update += CheckOnce;
		}

		private static void CheckOnce()
		{
			if (Application.isPlaying)
			{
				EditorApplication.update -= CheckOnce;
				if (OnFirstFrame != null) OnFirstFrame();
			}
		}

		#endregion
	}
}
#endif