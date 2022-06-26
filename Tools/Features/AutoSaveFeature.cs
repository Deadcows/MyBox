#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

namespace MyBox.Internal
{
	[InitializeOnLoad]
	public class AutoSaveFeature
	{
		static AutoSaveFeature()
		{
			EditorApplication.playModeStateChanged += AutoSaveWhenPlaymodeStarts;
		}

		private static void AutoSaveWhenPlaymodeStarts(PlayModeStateChange obj)
		{
			if (!MyBoxSettings.AutoSaveEnabled) return;

			if (EditorApplication.isPlayingOrWillChangePlaymode && !EditorApplication.isPlaying)
			{
				for (var i = 0; i < SceneManager.sceneCount; i++)
				{
					var scene = SceneManager.GetSceneAt(i);
					if (scene.isDirty) EditorSceneManager.SaveScene(scene);
				}

				AssetDatabase.SaveAssets();
			}
		}
	}
}
#endif