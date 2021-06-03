#if UNITY_EDITOR
using MyBox.EditorTools;
using UnityEditor;
using UnityEditor.PackageManager.Requests;
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

		private GUIStyle _titleStyle;
		private GUIStyle _buttonStyle;

		private AddRequest _updateRequest;


		static MyBoxWindow()
		{
			if (AutoUpdateCheckIsEnabled) MyEditorEvents.OnEditorStarts += CheckForUpdates;
		}

		private static void CheckForUpdates()
		{
			MyEditorEvents.OnEditorStarts -= CheckForUpdates;
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


		[MenuItem("Tools/MyBox/MyBox Window", priority = 1)]
		private static void MyBoxWindowMenuItem()
		{
			_windowInstance = GetWindow<MyBoxWindow>();
			_windowInstance.titleContent = new GUIContent("MyBox");
			_windowInstance.minSize = new Vector2(590, 520);
			_windowInstance.maxSize = new Vector2(590, 520);
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
			if (_titleStyle == null)
			{
				_titleStyle = new GUIStyle(EditorStyles.boldLabel);
				_titleStyle.fontSize = 42;
				_titleStyle.fontStyle = FontStyle.BoldAndItalic;
				_titleStyle.alignment = TextAnchor.MiddleCenter;
			}

			if (_buttonStyle == null)
			{
				_buttonStyle = new GUIStyle(MyGUI.HelpBoxStyle);
				_buttonStyle.hover.textColor = MyGUI.Colors.Blue;
			}

			var buttonWidth = 120;
			var buttonHeight = 30;
			var leftOffset = 20;


			wantsMouseMove = true;
			if (Event.current.type == EventType.MouseMove) Repaint();


			//buttonStyle.hover.background = buttonStyle.active.background.WithSolidColor(Color.red);

			EditorGUILayout.Space();


			EditorGUILayout.LabelField("MyBox", _titleStyle, GUILayout.Height(60));

			using (new EditorGUILayout.HorizontalScope())
			{
				GUILayout.FlexibleSpace();

				if (GUILayout.Button("  Github Page ↗", _buttonStyle, GUILayout.Width(buttonWidth), GUILayout.Height(buttonHeight)))
					Application.OpenURL("https://github.com/Deadcows/MyBox");

				if (GUILayout.Button("  Attributes ↗", _buttonStyle, GUILayout.Width(buttonWidth), GUILayout.Height(buttonHeight)))
					Application.OpenURL("https://github.com/Deadcows/MyBox/wiki/Attributes");

				if (GUILayout.Button("  Extensions ↗", _buttonStyle, GUILayout.Width(buttonWidth), GUILayout.Height(buttonHeight)))
					Application.OpenURL("https://github.com/Deadcows/MyBox/tree/master/Extensions");

				if (GUILayout.Button("  Tools, Features ↗", _buttonStyle, GUILayout.Width(buttonWidth), GUILayout.Height(buttonHeight)))
					Application.OpenURL("https://github.com/Deadcows/MyBox/wiki/Tools-and-Features");

				GUILayout.FlexibleSpace();
			}

			MyGUI.DrawLine(Color.white, true);

			EditorGUILayout.LabelField("MyBox Settings", new GUIStyle(EditorStyles.centeredGreyMiniLabel));

			using (new EditorGUILayout.HorizontalScope())
			{
				EditorGUILayout.Space(leftOffset);
				MyBoxSettings.CheckForUpdates = EditorGUILayout.Toggle("Check for Updates: ", MyBoxSettings.CheckForUpdates);
				GUILayout.FlexibleSpace();
			}

			using (new EditorGUILayout.HorizontalScope())
			{
				EditorGUILayout.Space(leftOffset);
				MyBoxSettings.AutoSaveEnabled = EditorGUILayout.Toggle("AutoSave on Play: ", MyBoxSettings.AutoSaveEnabled);
				GUILayout.FlexibleSpace();
			}

			using (new EditorGUILayout.HorizontalScope())
			{
				EditorGUILayout.Space(leftOffset);
				MyBoxSettings.CleanEmptyDirectoriesFeature = EditorGUILayout.Toggle("Clean Empty Folders: ", MyBoxSettings.CleanEmptyDirectoriesFeature);
				GUILayout.FlexibleSpace();
			}

			using (new EditorGUILayout.HorizontalScope())
			{
				EditorGUILayout.Space(leftOffset);
				MyBoxSettings.PrepareOnPlaymode = EditorGUILayout.Toggle("Prepare on Playmode: ", MyBoxSettings.PrepareOnPlaymode);
				if (GUILayout.Button(MyGUI.EditorIcons.Help, EditorStyles.label, GUILayout.Height(18)))
					Application.OpenURL("https://github.com/Deadcows/MyBox/wiki/Tools-and-Features#iprepare");
				GUILayout.FlexibleSpace();
			}

			MyGUI.DrawLine(Color.white, true);

			using (new EditorGUILayout.HorizontalScope())
			{
				var current = _installedVersion == null ? "..." : _installedVersion.AsSting;
				var latest = _latestVersion == null ? "..." : _latestVersion.AsSting;
				var installationType = MyBoxUtilities.InstalledViaUPM ? "UPM" : "(not UPM)";
				var versionStyle = new GUIStyle(EditorStyles.miniBoldLabel);
				versionStyle.alignment = TextAnchor.MiddleCenter;
				EditorGUILayout.LabelField($@"current: {current} {installationType}. latest: {latest}", versionStyle);
			}

			using (new EditorGUILayout.HorizontalScope())
			{
				GUILayout.FlexibleSpace();

				GUI.enabled = _updateRequest == null || _updateRequest.IsCompleted;
				var updateOrInstall = MyBoxUtilities.InstalledViaUPM ? "Update" : "Install";
				if (GUILayout.Button(updateOrInstall + " UPM version", _buttonStyle, GUILayout.Width(buttonWidth), GUILayout.Height(buttonHeight)))
				{
					if (MyBoxUtilities.InstalledViaUPM) AddPackage();
					else
					{
						if (EditorUtility.DisplayDialog(
							"Warning before installation",
							"When UPM version will be imported you should delete current installation of MyBox",
							"Ok, Install UPM version!", "Nah, keep it as it is")) AddPackage();
					}

					void AddPackage()
					{
						_updateRequest = UnityEditor.PackageManager.Client.Add("https://github.com/Deadcows/MyBox.git");
					}
				}

				GUI.enabled = true;

				if (GUILayout.Button("  How to Update ↗", _buttonStyle, GUILayout.Width(buttonWidth), GUILayout.Height(buttonHeight)))
					Application.OpenURL("https://github.com/Deadcows/MyBox/wiki/Installation");

				if (GUILayout.Button("  Releases ↗", _buttonStyle, GUILayout.Width(buttonWidth), GUILayout.Height(buttonHeight)))
					Application.OpenURL("https://github.com/Deadcows/MyBox/releases");

				if (GUILayout.Button("  Changelog ↗", _buttonStyle, GUILayout.Width(buttonWidth), GUILayout.Height(buttonHeight)))
					Application.OpenURL("https://github.com/Deadcows/MyBox/blob/master/CHANGELOG.md");

				GUILayout.FlexibleSpace();
			}

			MyGUI.DrawLine(Color.white, true);

			EditorGUILayout.LabelField("MyGUI.Colors References", new GUIStyle(EditorStyles.centeredGreyMiniLabel));
			using (new EditorGUILayout.HorizontalScope())
			{
				GUILayout.FlexibleSpace();
				DrawColors();
				GUILayout.FlexibleSpace();
			}

			EditorGUILayout.Space();
			EditorGUILayout.LabelField("MyGUI.EditorIcons References + with black color tint", new GUIStyle(EditorStyles.centeredGreyMiniLabel));
			using (new EditorGUILayout.HorizontalScope())
			{
				GUILayout.FlexibleSpace();
				DrawIcons();
				GUILayout.FlexibleSpace();
			}

			using (new EditorGUILayout.HorizontalScope())
			{
				GUILayout.FlexibleSpace();
				var c = GUI.contentColor;
				GUI.contentColor = Color.black;
				DrawIcons();
				GUI.contentColor = c;
				GUILayout.FlexibleSpace();
			}

			EditorGUILayout.Space(40);
			using (new EditorGUILayout.HorizontalScope())
			{
				GUILayout.FlexibleSpace();
				if (GUILayout.Button(SponsorButton, EditorStyles.label, GUILayout.Height(32)))
					Application.OpenURL("https://www.buymeacoffee.com/andrewrumak");
				GUILayout.FlexibleSpace();
			}
		}

		private void DrawColors()
		{
			int width = 24;
			int height = (int) EditorGUIUtility.singleLineHeight;

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

		private Texture2D SponsorButton => _sponsorButton != null ? _sponsorButton : 
			_sponsorButton = ImageStringConverter.ImageFromString(SponsorButtonString, 160, 32);
		private Texture2D _sponsorButton;
		private const string SponsorButtonString =
			"iVBORw0KGgoAAAANSUhEUgAAAKAAAAAgCAMAAACioYPHAAABI1BMVEUAAAAAABgBABcBABgAABgFBRYAABcAABcAABcBABcBABcAABgBABcAABcAABcAABcAABgBABcAABcAABcAABcAABcBABcBABcBABcAABcAAhcBABcBABcAABcAABcBABgAABgBABcAABgAABcBABcBABcEBhYAABcAABcBABgGBBYAABcAABcAABcBABcBABcBARcAABgBARcBABcBABcBABcHBhUBABgAABgBABf//QABABf42gCMjQD/+QASEhT02wD//wAAACP/5AD/5gD/3gD/9gBtcgD9+QDl2wCLhAB9bQD/8gAAACL//wD/5wD/+QD+1QAcJg62qwAxPwr//wCpnwD//QAAABf/3wD/3QAAABwAACH/8AD//gD/5wD/1gDodj3PAAAAWHRSTlMAdHvn4xLEkzLuSCmkmi6JIhzy1dBgPRdqXFKNZDoE/PnMqaCBTTYK2oUHrG9XRLA/9si8jyUNurVB3b6HVIxlYfLJx6uil2ZYUENANu3GiXFxamhXSTsb7IcgmAAABIRJREFUWMPt19dy2kAYBeCj3guI3qvpxQXs9N57PzGkvP9TRBIhcW5M8PiCmeSbQSCttHv+HXYH8N8/ZVysG4wkU04bu8chUw1HO9Ic+4BsYucEpJNrT8vTds6bqvSxazq9FNcUnSZ2UdYqjzNuFlB4hN2SVRloQiGju64+HfUmZLOPXTKgKvGMayoF7BIeAlm9nR8KQi5vugqQDLDWH/Uxx+UxEwDGM2yDagJ/mCUNrGV40GW3gEvi0gMSW+5kTZLdw2axJYq9I1tNdckW1kakOiH3cDnyHAJjethKyWkYXFks96uU8vhFoAns0cHlENiOUqaxHSWDrG/pmbLfB6Z/LJEiw0s+WyjngfIQyPsIVRDRM8g3RsBwUEZIk4sVxKxeSemsM0lNBcAVUSvO4TETpSxEg57IJcCUUz42anDd2/MPn/YWOfxmV9ExA7mDLisIiCtxfPsgTnItOSQ5LZE1C9BImohkSMopBZE0ZR4BIkM2bFpR0VE9KslKluQxNuot9uJ0N69+/na/sGifzV7t7pN1gAE6lGFTBxRKcUAvbHEDg80rYYYsBwkO45UvsyDQWBWdDKIviEn7muqwc8IsMOAMYaU5k4pH/ziJjUrLGw8fPLj3+fT087dXt8K0v51QmvQcTixqyNBBMupOYOvn7FCHSFbAJvJM6IwfzVNEmQ4iLof+voluDapt0T2sVgCVCuAQLfalQxgn2Ki9vPHy++fYt9dvF2d3HSNAnKjINESOQBWAzBzmShTwIGobYMxe2CqGp74FHDMBjWlkGz5GzMNCnzYGtZaRDWQAQQ3hTfvhA+CgQbeycY/Ql49unsb5Tr++67GPXzq0V1OlUkeKFqLzIjlF/RDIRTMpsgSBOXhkkIXcAKT98EUdafpIxwvC5BCWwWSjegJMaylAl8gB5iRLyBEbJCi8+XK6CvioYfzRIgIoV7saw89M5KjOREq1kUcBaLENONSh0YKUzESVlAG5poishu8qkIlqcGtRXeG1JJ2ZRjb8HlmPl4uXBWoqNqjQu70OeONugN/2aLTEY9JNMxeQMiWSWns/PMSr3wJSVFBkoUgmMOIkXjtVpljy6AIViZ7NQZOeWNt3E1WSTTk8aqoECFE3yt/8AE3avwI+fXENv1lyPcmqY8ElaYo8SmiHbaXOpAisssEsRXWQRxK7rPcBdGwp16mTeYQyEg0RmJB1HTNKHEAyWI9Kl9nTGET3bZQ6ePwz4J2PyybOUDD3K/FAgh6dRSSyhIg+xk+F4hj6YUo482CpjFh/tirVQrS3FNJsc2hyjHxKLQBFuelisxPpyc+AV58uWzjfMVtM40LavIKazRLoYCvO8tmdOOC3e4VFCecq03Np4kIKzMCYsABOsBVx+ezqKuD99GLD4DbhMYsL2aOZoMjcnB62kl7euPo9Dvjw1kLHubqGzxQupmJIBrMHSZkutmIu3t/8+iX09fGtxYbZ0UhmcEEFUkCZ7GE7Cd6d374eeqLIBs7X0eojXJjlAvB1bEujLAiiIAi1Hf1XjFyXCy4WVMfYET8Aukjku5lK/zUAAAAASUVORK5CYII=";
	}
}
#endif