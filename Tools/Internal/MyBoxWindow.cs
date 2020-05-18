#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace MyBox.Internal
{
	[InitializeOnLoad]
	public class MyBoxWindow : EditorWindow
	{
		public static bool AutoUpdateCheckIsEnabled = true;

		private static MyBoxVersion _installedVersion;
		private static MyBoxVersion _latestVersion;

		private static EditorWindow _windowInstance;
		

		static MyBoxWindow()
		{
			if (AutoUpdateCheckIsEnabled)
			{
				MyBoxUtilities.GetMyBoxLatestVersionAsync(version =>
				{
					_installedVersion = MyBoxUtilities.GetMyBoxInstalledVersion();
					_latestVersion = version;
					if (!_installedVersion.VersionsMatch(_latestVersion))
					{
						var versions = "Installed version: " + _installedVersion.AsSting + ". Latest version: " + _latestVersion.AsSting;
						Debug.Log("It's time to update MyBox :)! Use \"Tools/MyBox/Update MyBox\". " + versions);
					}
				});
			}
		}

		[MenuItem("Tools/MyBox/MyBox Window", priority = 1)]
		private static void MuBoxUpdateMenuItem()
		{
			_windowInstance = GetWindow<MyBoxWindow>();
			_windowInstance.titleContent = new GUIContent("Update MyBox");
		}

		private void OnEnable()
		{
			_windowInstance = this;

			_installedVersion = MyBoxUtilities.GetMyBoxInstalledVersion();
			MyBoxUtilities.GetMyBoxLatestVersionAsync(version =>
			{
				_latestVersion = version;
				if (_windowInstance != null) _windowInstance.Repaint();
			});
		}

		
		private void OnGUI()
		{
			EditorGUILayout.LabelField("You are using " + (MyBoxUtilities.InstalledViaUPM ? "PackageManager version!" : "Git version!"));
			if (!MyBoxUtilities.InstalledViaUPM) EditorGUILayout.LabelField("PackageManager version is easier to update ;)");
			
			EditorGUILayout.Space();
			EditorGUILayout.LabelField("Current version: " + (_installedVersion == null ? "..." : _installedVersion.AsSting));
			EditorGUILayout.LabelField("Latest version: " + (_latestVersion == null ? "..." : _latestVersion.AsSting));

			using (new EditorGUILayout.HorizontalScope())
			{
				if (GUILayout.Button("Update GIT packages", EditorStyles.toolbarButton))
				{
					if (!MyBoxUtilities.UpdateGitPackages()) 
						ShowNotification(new GUIContent("There is no git packages installed"));
				}

				if (GUILayout.Button("Open Git releases page", EditorStyles.toolbarButton))
				{
					MyBoxUtilities.OpenMyBoxGitInBrowser();
				}
			}
		}
	}
}
#endif