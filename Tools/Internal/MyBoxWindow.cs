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
		private static MyBoxVersion _installedVersion;
		private static MyBoxVersion _latestVersion;

		private static EditorWindow _windowInstance;

		private const string UrlSymbol = "\u279a"; 
		private GUIStyle _titleStyle;
		private GUIStyle _buttonStyle;
		private GUIStyle _sponsorButtonStyle;
		private GUIContent _externalDocIcon;
		private GUIStyle _externalDocStyle;
		private Texture2D _odinLogo;

		private AddRequest _updateRequest;


		static MyBoxWindow()
		{
			if (MyBoxSettings.CheckForUpdates) MyEditorEvents.OnEditorStarts += CheckForUpdates;
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
					var message = "It's time to update MyBox :)! Use \"Tools/MyBox/MyBox Window\" for more info. " + versions;
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

		private void InitializeStyles()
		{
			if (_titleStyle == null)
			{
				_titleStyle = new GUIStyle(EditorStyles.boldLabel);
				_titleStyle.fontSize = 42;
				_titleStyle.fontStyle = FontStyle.Bold;
				_titleStyle.alignment = TextAnchor.MiddleCenter;
			}

			if (_buttonStyle == null)
			{
				_buttonStyle = new GUIStyle(MyGUI.HelpBoxStyle);
				_buttonStyle.normal.textColor = GUI.skin.textField.normal.textColor;
				_buttonStyle.hover.textColor = EditorGUIUtility.isProSkin ? Color.white : MyGUI.Colors.Gray;
			}

			if (_sponsorButtonStyle == null)
			{
				_sponsorButtonStyle = new GUIStyle(_buttonStyle);
				_sponsorButtonStyle.normal.textColor = EditorStyles.centeredGreyMiniLabel.normal.textColor;
				_sponsorButtonStyle.fontSize *= 2;
				_sponsorButtonStyle.fontStyle = FontStyle.Bold;
			}

			if (_externalDocIcon == null)
			{
				_externalDocIcon = new GUIContent(MyGUI.EditorIcons.Help);
				_externalDocIcon.tooltip = "Open external documentation " + UrlSymbol;
			}

			if (_externalDocStyle == null)
			{
				_externalDocStyle = new GUIStyle(EditorStyles.iconButton);
				_externalDocStyle.margin = new RectOffset(0, 0, 3, 0);
			}
		}

		private bool GuiEnabledGlobal => !EditorApplication.isCompiling;

		private void OnGUI()
		{
			GUI.enabled = GuiEnabledGlobal;
			wantsMouseMove = true;
			
			InitializeStyles();

			var buttonWidth = GUILayout.Width(120);
			var buttonHeight = GUILayout.Height(30);

			if (Event.current.type == EventType.MouseMove) Repaint();

			EditorGUILayout.Space();
			EditorGUILayout.LabelField("MyBox", _titleStyle, GUILayout.Height(60));

			using (new EditorGUILayout.HorizontalScope())
			{
				GUILayout.FlexibleSpace();

				if (GUILayout.Button("  Github Page " + UrlSymbol, _buttonStyle, buttonWidth, buttonHeight))
					Application.OpenURL("https://github.com/Deadcows/MyBox");

				if (GUILayout.Button("  Attributes " + UrlSymbol, _buttonStyle, buttonWidth, buttonHeight))
					Application.OpenURL("https://github.com/Deadcows/MyBox/wiki/Attributes");

				if (GUILayout.Button("  Extensions " + UrlSymbol, _buttonStyle, buttonWidth, buttonHeight))
					Application.OpenURL("https://github.com/Deadcows/MyBox/tree/master/Extensions");

				if (GUILayout.Button("  Tools, Features " + UrlSymbol, _buttonStyle, buttonWidth, buttonHeight))
					Application.OpenURL("https://github.com/Deadcows/MyBox/wiki/Tools-and-Features");

				GUILayout.FlexibleSpace();
			}

			MyGUI.DrawLine(Color.white, true);

			EditorGUILayout.LabelField("MyBox Settings", new GUIStyle(EditorStyles.centeredGreyMiniLabel));

			using (new EditorGUILayout.HorizontalScope())
			{
				GUILayout.FlexibleSpace();
				EditorGUILayout.Space(50);
				
				using (new EditorGUILayout.VerticalScope())
				{
					using (new EditorGUILayout.HorizontalScope())
					{
						MyBoxSettings.CheckForUpdates = EditorGUILayout.Toggle("Check for Updates: ", MyBoxSettings.CheckForUpdates);
						GUILayout.FlexibleSpace();
					}
					
					using (new EditorGUILayout.HorizontalScope())
					{
						DrawInspectorOverrideSetting();
						GUILayout.FlexibleSpace();
					}
					
					using (new EditorGUILayout.HorizontalScope())
					{
						DrawUnityEventOverrideSetting();
						GUILayout.FlexibleSpace();
					}
					
					using (new EditorGUILayout.HorizontalScope())
					{
						var label = new GUIContent("AutoSave on Play: ", "Save changes in opened scenes before Playmode. " +
						                                                 "\nUnity crashes from time to time, you know...");
						MyBoxSettings.AutoSaveEnabled = EditorGUILayout.Toggle(label, MyBoxSettings.AutoSaveEnabled);
						GUILayout.FlexibleSpace();
					}

					using (new EditorGUILayout.HorizontalScope())
					{
						var label = new GUIContent("Clean Empty Folders: ", "Delete empty folders in project on Save. " +
						                                                    "\nIt handles VCS issue with .meta files for empty folders");
						MyBoxSettings.CleanEmptyDirectoriesFeature = EditorGUILayout.Toggle(label, MyBoxSettings.CleanEmptyDirectoriesFeature);
						GUILayout.FlexibleSpace();
					}
				}

				EditorGUILayout.Space(30);
				using (new EditorGUILayout.VerticalScope())
				{
					EditorGUILayout.LabelField("Performance settings", EditorStyles.miniLabel);
					
					using (new EditorGUILayout.HorizontalScope())
					{
						var label = new GUIContent("Prepare on Playmode: ", "Allows to use IPrepare interface with Prepare() method called automatically." +
						                                                    "\nSlightly increases project Save time.");
						MyBoxSettings.PrepareOnPlaymode = EditorGUILayout.Toggle(label, MyBoxSettings.PrepareOnPlaymode);
						if (GUILayout.Button(_externalDocIcon, _externalDocStyle, GUILayout.Height(18)))
							Application.OpenURL("https://github.com/Deadcows/MyBox/wiki/Tools-and-Features#iprepare");
						GUILayout.FlexibleSpace();
					}
					
					using (new EditorGUILayout.HorizontalScope())
					{
						var label = new GUIContent("SO processing: ", "Allows [AutoProperty] and [MustBeAssigned] Attributes to work with Scriptable Objects." +
						                                              "\nMight increase project Save time for a few seconds.");
						MyBoxSettings.EnableSOCheck = EditorGUILayout.Toggle(label, MyBoxSettings.EnableSOCheck);
						GUILayout.FlexibleSpace();
					}
				}
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

				GUI.enabled = GuiEnabledGlobal && (_updateRequest == null || _updateRequest.IsCompleted);
				var updateOrInstall = MyBoxUtilities.InstalledViaUPM ? "Update" : "Install";
				if (GUILayout.Button(updateOrInstall + " UPM version", _buttonStyle, buttonWidth, buttonHeight))
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

				GUI.enabled = GuiEnabledGlobal;

				if (GUILayout.Button("  How to Update " + UrlSymbol, _buttonStyle, buttonWidth, buttonHeight))
					Application.OpenURL("https://github.com/Deadcows/MyBox/wiki/Installation");

				if (GUILayout.Button("  Releases " + UrlSymbol, _buttonStyle, buttonWidth, buttonHeight))
					Application.OpenURL("https://github.com/Deadcows/MyBox/releases");

				if (GUILayout.Button("  Changelog " + UrlSymbol, _buttonStyle, buttonWidth, buttonHeight))
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
			EditorGUILayout.LabelField("MyGUI.EditorIcons References", new GUIStyle(EditorStyles.centeredGreyMiniLabel));
			using (new EditorGUILayout.HorizontalScope())
			{
				GUILayout.FlexibleSpace();
				DrawIcons();
				GUILayout.FlexibleSpace();
			}

			EditorGUILayout.Space(26);
			using (new EditorGUILayout.HorizontalScope())
			{
				GUILayout.FlexibleSpace();
				if (GUILayout.Button("support my work \u2665", _sponsorButtonStyle,GUILayout.Width(260), GUILayout.Height(42)))
					Application.OpenURL("https://ko-fi.com/mybox");
				GUILayout.FlexibleSpace();
			}
		}
		
		private void DrawInspectorOverrideSetting()
		{
			const string define = "MYBOX_DISABLE_INSPECTOR_OVERRIDE";
			var label = new GUIContent("Inspector override: ", 
				"Custom UnityObject inspector enables [ButtonMethod] and [Foldout] attributes to work. " +
			         "Disable to support some other libraries that override UnityObject inspector, " +
						" like NaughtyAttributes or OdinInspector");
			
			// ReSharper disable once RedundantAssignment
			bool overrideEnabled = true;
#if MYBOX_DISABLE_INSPECTOR_OVERRIDE || ODIN_INSPECTOR
			overrideEnabled = false;
#endif
#if ODIN_INSPECTOR
			bool wasEnabled = GUI.enabled;
			GUI.enabled = false;
#endif						
						
			EditorGUI.BeginChangeCheck();
			// ReSharper disable once ConditionIsAlwaysTrueOrFalse
			overrideEnabled = EditorGUILayout.Toggle(label, overrideEnabled);
#if ODIN_INSPECTOR
			GUI.enabled = wasEnabled;
			if (_odinLogo == null) _odinLogo = ImageStringConverter.ImageFromString(IconOdin, 32, 32);
			var odinContext = new GUIContent(_odinLogo, "Odin Inspector is detected. " +
			                                            "MyBox will disable some of its features to prevent conflicts");
			EditorGUILayout.LabelField(odinContext, GUILayout.Width(32));
#endif
						
			if (EditorGUI.EndChangeCheck() && 
			    EditorUtility.DisplayDialog("Toggle \"" + define + "\" define symbol", 
				    "This change will cause recompilation, continue?", "Ok", "Cancel"))
			{
				if (overrideEnabled) MyDefinesUtility.RemoveDefine(define);
				else MyDefinesUtility.AddDefine(define);
			}
		}
		
		private void DrawUnityEventOverrideSetting()
		{
			const string define = "MYBOX_DISABLE_UNITYEVENT_OVERRIDE";
			var label = new GUIContent("UnityEvent override: ", 
				"Custom UnityEvent inspector allows UnityEvent to be smaller and collapsable. " +
				"It is optional, so you could use your own UnityEventDrawer.");
			
			// ReSharper disable once RedundantAssignment
			bool overrideEnabled = true;
#if MYBOX_DISABLE_UNITYEVENT_OVERRIDE || ODIN_INSPECTOR
			overrideEnabled = false;
#endif
#if ODIN_INSPECTOR
			bool wasEnabled = GUI.enabled;
			GUI.enabled = false;
#endif						

			EditorGUI.BeginChangeCheck();
			// ReSharper disable once ConditionIsAlwaysTrueOrFalse
			overrideEnabled = EditorGUILayout.Toggle(label, overrideEnabled);
#if ODIN_INSPECTOR
			GUI.enabled = wasEnabled;
			if (_odinLogo == null) _odinLogo = ImageStringConverter.ImageFromString(IconOdin, 32, 32);
			var odinContext = new GUIContent(_odinLogo, "Odin Inspector is detected. " +
			                                            "CollapsableEventDrawer not works well with Odin, so it is disabled");
			EditorGUILayout.LabelField(odinContext, GUILayout.Width(32));
#endif
						
			if (EditorGUI.EndChangeCheck() && 
			    EditorUtility.DisplayDialog("Toggle \"" + define + "\" define symbol", 
				    "This change will cause recompilation, continue?", "Ok", "Cancel"))
			{
				if (overrideEnabled) MyDefinesUtility.RemoveDefine(define);
				else MyDefinesUtility.AddDefine(define);
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
			int width = 22;
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
		
		
		private const string IconOdin =
			"iVBORw0KGgoAAAANSUhEUgAAAEAAAABACAMAAACdt4HsAAAAk1BMVEUAAADyYj7yYj7yYj7yYj7yYj7yYj7yYj7yYj7yYj7yYj7yYj7yYj7yYj7yYj7yYj7yYj7yYj7yYj7yYj7yYj7yYj7yYj7yYj7yYj7yYj7yYj7yYj7yYj7yYj7yYj7yYj7yYj7yYj7yYj7yYj7yYj7yYj7yYj7yYj7yYj7yYj7yYj7yYj7yYj7yYj7yYj7yYj7yYj65msSDAAAAMHRSTlMArmbTSmE0JBK1XT8YBurn4sqiKfbuinp1RJqGWFAN+tnFqYBrOtW8lJEe8s7BLnHosY0IAAACRUlEQVRYw+3W23aCMBAF0BFEKKKiIOD9hlqt2vn/r2swlZnoMglvfeh+6tG1Kjm5EPj3l5URT9GsB81MMOcxxxCaGWJ6pnROMQ6giQIRpxSnImbQQDRGoU3jqXTA3ggrLo2ncgRrbZQKGR2UPLB1QSn1qxQMUIqX1g0+tKr4WcdPsOKvsDYB2CPpWDZIXDmFgn2PC+R2olHGAaPeAbn0rDzRwLwec5Sox2CMaN9jssIncyh57ILeBz5bVyuRbGwbJDPYW/fYW1P9MT4kMMXd4JHGEbw3Y93l7O/bDLI6Tm0aTH3+NHPx3TKmHi0azGUffBc5dRzCGyGt4F6Vt6xH4VjHb2ODE9pVtIs6aOiRahpV8RSyHo/qvh7BC/4DX+d7PMiThXoMBvSI2gbL31jQTpSnuqfrMVS/DOVQqccTCJs6FsCpj9et40ie7/Rxlw9Tlak/dXoM9Rv7dwNssc9fe9zzJUDxAiAis6SpXgB3pZU+EQ0P2QoKEv8u6QjLjB+XxEMmBIft/l6MXIrUo9Ig2fIzrEvlvEh9/vKleiMYsaYSfG9La5DJ2Vz1A/hAjTY1yKsZstMrRJ2DnKJS/ac5Pz/XqLW7N9hX3gLRF+1hOuI0Pc5bSFY+W/2ZaMNkGMF+sWg/zDvqOdbdec4vL/Sk0HNIWfjw5KCcAM3tWBuKufshXQrQuFEbCSgSu1sGFTq+utxh07e5ZYSoY347LmPUMd8yMtQx3zL2aGJ4O15Rz3TL8LCBEl4k2MjtdQrdlr2t68C/P+QHQ7UkQZLgKFoAAAAASUVORK5CYII=";
	}
}
#endif