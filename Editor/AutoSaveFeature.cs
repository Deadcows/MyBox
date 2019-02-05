using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

namespace EckTechGames
{
	[InitializeOnLoad]
	public class AutoSaveFeature
	{
		private const string AutoSaveFeatureKey = "MyBox.AutoSaveEnabled";
		private const string MenuItemName = "Tools/MyBox/AutoSave on Play";

		private static bool _enabled;

		[MenuItem(MenuItemName, priority = 100)]
		private static void MenuItem()
		{
			_enabled = !_enabled;

			EditorPrefs.SetBool(AutoSaveFeatureKey, _enabled);
		}

		[MenuItem(MenuItemName, true)]
		private static void MenuItemValidation()
		{
			_enabled = EditorPrefs.GetBool(AutoSaveFeatureKey, true);
			Menu.SetChecked(MenuItemName, _enabled);
		}


		static AutoSaveFeature()
		{
			EditorApplication.playModeStateChanged += AutoSaveWhenPlaymodeStarts;
		}

		private static void AutoSaveWhenPlaymodeStarts(PlayModeStateChange obj)
		{
			if (!_enabled) return;

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