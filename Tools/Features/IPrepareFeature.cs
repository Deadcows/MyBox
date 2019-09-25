using System;

namespace MyBox
{
	/// <summary>
	/// Prepare() called on every MonoBehaviour by IPrepareFeature class. If Prepare() returns true, parent scene will be marked dirty 
	/// </summary>
	public interface IPrepare
	{
		bool Prepare();
	}
}

#if UNITY_EDITOR
namespace MyBox.Internal
{
	using System.Collections.Generic;
	using System.Linq;
	using UnityEngine;
	using UnityEditor;
	using UnityEditor.SceneManagement;
	using UnityEngine.SceneManagement;

	[InitializeOnLoad]
	public class IPrepareFeature
	{
		public static bool IsEnabled = true;

		public static Action OnPrepare;

		static IPrepareFeature()
		{
			EditorTools.MyEditorEvents.BeforePlaymode += PrepareOnPlay;
		}

		private static void PrepareOnPlay()
		{
			if (!IsEnabled) return;
			
			RunPrepare();
		}
		
		/// <summary>
		/// Calls Prepare() on any MonoBehaviour with IPrepare interface. If Prepare() returns true, parent scene will be marked dirty
		/// </summary>
		public static void RunPrepare()
		{
			if (OnPrepare != null) OnPrepare();
			
			var toPrepare = MyExtensions.FindObjectsOfInterfaceAsComponents<IPrepare>();

			HashSet<Scene> modifiedScenes = null;
			foreach (var prepare in toPrepare)
			{
				bool changed = prepare.Interface.Prepare();

				if (changed && prepare.Component != null)
				{
					if (modifiedScenes == null) modifiedScenes = new HashSet<Scene>();
					modifiedScenes.Add(prepare.Component.gameObject.scene);

					EditorUtility.SetDirty(prepare.Component);
					Debug.Log(prepare.Component.name + "." + prepare.Component.GetType().Name + ": Changed on Prepare", prepare.Component);
				}
			}

			if (modifiedScenes != null) EditorSceneManager.SaveScenes(modifiedScenes.ToArray());
		}
	}
}
#endif