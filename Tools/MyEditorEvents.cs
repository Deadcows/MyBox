#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEditor.Build;
using UnityEngine;
using UnityEditor.Build.Reporting;

namespace MyBox.EditorTools
{
	[InitializeOnLoad]
	public class MyEditorEvents : UnityEditor.AssetModificationProcessor, IPreprocessBuildWithReport
	{
		/// <summary>
		/// Occurs on Scenes/Assets Save
		/// </summary>
		public static Action OnSave;

		/// <summary>
		/// Occurs on first frame in Playmode
		/// </summary>
		public static Action OnFirstFrame;

		public static Action BeforePlaymode;

		public static Action BeforeBuild;


		static MyEditorEvents()
		{
			EditorApplication.update += CheckOnce;
			EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
		}


		/// <summary>
		/// On Editor Save
		/// </summary>
		private static string[] OnWillSaveAssets(string[] paths)
		{
			// Prefab creation enforces SaveAsset and this may cause unwanted dir cleanup
			if (paths.Length == 1 && (paths[0] == null || paths[0].EndsWith(".prefab"))) return paths;

			if (OnSave != null) OnSave();

			return paths;
		}

		/// <summary>
		/// On First Frame
		/// </summary>
		private static void CheckOnce()
		{
			if (Application.isPlaying)
			{
				EditorApplication.update -= CheckOnce;
				if (OnFirstFrame != null) OnFirstFrame();
			}
		}

		/// <summary>
		/// On Before Playmode
		/// </summary>
		private static void OnPlayModeStateChanged(PlayModeStateChange state)
		{
			if (state == PlayModeStateChange.ExitingEditMode && BeforePlaymode != null) BeforePlaymode();
		}

		/// <summary>
		/// Before Build
		/// </summary>
		public void OnPreprocessBuild(BuildReport report)
		{
			if (BeforeBuild != null) BeforeBuild();
		}

		public int callbackOrder
		{
			get { return 0; }
		}
	}
}
#endif