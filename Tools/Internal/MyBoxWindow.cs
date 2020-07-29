#if UNITY_EDITOR
using MyBox.EditorTools;
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
						var message = "It's time to update MyBox :)! Use \"Tools/MyBox/Update MyBox\". " + versions;
						WarningsPool.Log(message);
					}
				});
			}
		}

		[MenuItem("Tools/MyBox/MyBox Window", priority = 1)]
		private static void MyBoxWindowMenuItem()
		{
			_windowInstance = GetWindow<MyBoxWindow>();
			_windowInstance.titleContent = new GUIContent("Update MyBox");
			_windowInstance.minSize = new Vector2(580, 350);
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

			EditorGUILayout.Space();
			EditorGUILayout.Space();
			EditorGUILayout.Space();
			EditorGUILayout.LabelField("MyGUI.Colors:");
			using (new EditorGUILayout.HorizontalScope())
			{
				DrawColors();
			}
			
			EditorGUILayout.Space();
			EditorGUILayout.LabelField("MyGUI.EditorIcons:");
			using (new EditorGUILayout.HorizontalScope())
			{
				DrawIcons();
			}

			using (new EditorGUILayout.HorizontalScope())
			{
				var c = GUI.contentColor;
				GUI.contentColor = Color.black;
				DrawIcons();
				GUI.contentColor = c;
			}
		}

		private void DrawColors()
		{
			int width = 24;
			int height = (int)EditorGUIUtility.singleLineHeight;
			
			var content = new GUIContent("", "MyGUI.Colors.Red");
			EditorGUILayout.LabelField(content, GUILayout.Width(width));
			MyGUI.DrawBackgroundBox(MyGUI.Colors.Red, height);
			
			content = new GUIContent("", "MyGUI.Colors.Green");
			EditorGUILayout.LabelField(content, GUILayout.Width(width));
			MyGUI.DrawBackgroundBox(MyGUI.Colors.Green, height);
			
			content = new GUIContent("", "MyGUI.Colors.Blue");
			EditorGUILayout.LabelField(content, GUILayout.Width(width));
			MyGUI.DrawBackgroundBox(MyGUI.Colors.Blue, height);
			
			content = new GUIContent("", "MyGUI.Colors.Gray");
			EditorGUILayout.LabelField(content, GUILayout.Width(width));
			MyGUI.DrawBackgroundBox(MyGUI.Colors.Gray, height);
			
			content = new GUIContent("", "MyGUI.Colors.Yellow");
			EditorGUILayout.LabelField(content, GUILayout.Width(width));
			MyGUI.DrawBackgroundBox(MyGUI.Colors.Yellow, height);
			
			content = new GUIContent("", "MyGUI.Colors.Brown");
			EditorGUILayout.LabelField(content, GUILayout.Width(width));
			MyGUI.DrawBackgroundBox(MyGUI.Colors.Brown, height);
		}
		
		private void DrawIcons()
		{
			int width = 24;
			var content = new GUIContent(MyGUI.EditorIcons.Plus);
			content.tooltip = "MyGUI.EditorIcons.Plus";
			EditorGUILayout.LabelField(content, GUILayout.Width(width));
				
			content = new GUIContent(MyGUI.EditorIcons.Minus);
			content.tooltip = "MyGUI.EditorIcons.Minus";
			EditorGUILayout.LabelField(content, GUILayout.Width(width));
				
			content = new GUIContent(MyGUI.EditorIcons.Refresh);
			content.tooltip = "MyGUI.EditorIcons.Refresh";
			EditorGUILayout.LabelField(content, GUILayout.Width(width));
				
			content = new GUIContent(MyGUI.EditorIcons.ConsoleInfo);
			content.tooltip = "MyGUI.EditorIcons.ConsoleInfo";
			EditorGUILayout.LabelField(content, GUILayout.Width(width));
				
			content = new GUIContent(MyGUI.EditorIcons.ConsoleWarning);
			content.tooltip = "MyGUI.EditorIcons.ConsoleWarning";
			EditorGUILayout.LabelField(content, GUILayout.Width(width));
				
			content = new GUIContent(MyGUI.EditorIcons.ConsoleError);
			content.tooltip = "MyGUI.EditorIcons.ConsoleError";
			EditorGUILayout.LabelField(content, GUILayout.Width(width));
				
			content = new GUIContent(MyGUI.EditorIcons.Check);
			content.tooltip = "MyGUI.EditorIcons.Check";
			EditorGUILayout.LabelField(content, GUILayout.Width(width));
				
			content = new GUIContent(MyGUI.EditorIcons.Cross);
			content.tooltip = "MyGUI.EditorIcons.Cross";
			EditorGUILayout.LabelField(content, GUILayout.Width(width));
				
			content = new GUIContent(MyGUI.EditorIcons.Dropdown);
			content.tooltip = "MyGUI.EditorIcons.Dropdown";
			EditorGUILayout.LabelField(content, GUILayout.Width(width));
				
			content = new GUIContent(MyGUI.EditorIcons.EyeOn);
			content.tooltip = "MyGUI.EditorIcons.EyeOn";
			EditorGUILayout.LabelField(content, GUILayout.Width(width));
				
			content = new GUIContent(MyGUI.EditorIcons.EyeOff);
			content.tooltip = "MyGUI.EditorIcons.EyeOff";
			EditorGUILayout.LabelField(content, GUILayout.Width(width));
				
			content = new GUIContent(MyGUI.EditorIcons.Zoom);
			content.tooltip = "MyGUI.EditorIcons.Zoom";
			EditorGUILayout.LabelField(content, GUILayout.Width(width));
				
			content = new GUIContent(MyGUI.EditorIcons.Help);
			content.tooltip = "MyGUI.EditorIcons.Help";
			EditorGUILayout.LabelField(content, GUILayout.Width(width));
				
			content = new GUIContent(MyGUI.EditorIcons.Favourite);
			content.tooltip = "MyGUI.EditorIcons.Favourite";
			EditorGUILayout.LabelField(content, GUILayout.Width(width));
				
			content = new GUIContent(MyGUI.EditorIcons.Label);
			content.tooltip = "MyGUI.EditorIcons.Label";
			EditorGUILayout.LabelField(content, GUILayout.Width(width));
				
			content = new GUIContent(MyGUI.EditorIcons.Settings);
			content.tooltip = "MyGUI.EditorIcons.Settings";
			EditorGUILayout.LabelField(content, GUILayout.Width(width));
				
			content = new GUIContent(MyGUI.EditorIcons.SettingsPopup);
			content.tooltip = "MyGUI.EditorIcons.SettingsPopup";
			EditorGUILayout.LabelField(content, GUILayout.Width(width));
				
			content = new GUIContent(MyGUI.EditorIcons.SettingsMixer);
			content.tooltip = "MyGUI.EditorIcons.SettingsMixer";
			EditorGUILayout.LabelField(content, GUILayout.Width(width));
				
			content = new GUIContent(MyGUI.EditorIcons.Circle);
			content.tooltip = "MyGUI.EditorIcons.Circle";
			EditorGUILayout.LabelField(content, GUILayout.Width(width));
				
			content = new GUIContent(MyGUI.EditorIcons.CircleYellow);
			content.tooltip = "MyGUI.EditorIcons.CircleYellow";
			EditorGUILayout.LabelField(content, GUILayout.Width(width));
				
			content = new GUIContent(MyGUI.EditorIcons.CircleDotted);
			content.tooltip = "MyGUI.EditorIcons.CircleDotted";
			EditorGUILayout.LabelField(content, GUILayout.Width(width));
				
			content = new GUIContent(MyGUI.EditorIcons.CircleRed);
			content.tooltip = "MyGUI.EditorIcons.CircleRed";
			EditorGUILayout.LabelField(content, GUILayout.Width(width));
		}
	}
}
#endif