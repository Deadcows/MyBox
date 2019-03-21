//#if UNITY_EDITOR
//using System.Diagnostics;
//using System.IO;
//using System.Linq;
//using MyBox.EditorTools;
//using UnityEditor;
//using UnityEngine;
//
//namespace MyBox.Internal
//{
//	public class MyBundleUtility : EditorWindow
//	{
//		#region Build Bandles
//
//		[MenuItem("Assets/Build Bundles")]
//		static void BuildAssetBundles()
//		{
//			string winPath = EditorUtility.OpenFolderPanel("Windows Output Folder", "", "");
//			if (string.IsNullOrEmpty(winPath)) return;
//			BuildPipeline.BuildAssetBundles(winPath, BuildAssetBundleOptions.UncompressedAssetBundle,
//				BuildTarget.StandaloneWindows);
//
//			string macPath = EditorUtility.OpenFolderPanel("OSX Output Folder", "", "");
//			if (string.IsNullOrEmpty(macPath)) return;
//			BuildPipeline.BuildAssetBundles(macPath, BuildAssetBundleOptions.UncompressedAssetBundle,
//				BuildTarget.StandaloneOSX);
//		}
//
//		#endregion
//
//
//		#region Windows Build
//
//		[MenuItem("Assets/Windows Build", priority = 1)]
//		private static void BuildWindows()
//		{
//			WindowsBuild(false);
//		}
//
//		[MenuItem("Assets/Windows Build Light", priority = 2)]
//		private static void BuildWindowsLight()
//		{
//			WindowsBuild(true);
//		}
//
//		private static void WindowsBuild(bool light)
//		{
//			string buildPath = EditorUtility.OpenFolderPanel("Windows Build Output Folder", "", "");
//			if (string.IsNullOrEmpty(buildPath)) return;
//
//			var exePath = Path.Combine(buildPath, "TheFinalStation.exe");
//			var dlcPath = Path.Combine(buildPath, "TheFinalStation_Data");
//
//			PlayerSettings.displayResolutionDialog = ResolutionDialogSetting.HiddenByDefault;
//			var scenesInBuild = EditorBuildSettings.scenes.Select(s => s.path).ToArray();
//
//			// Simplified build, only dlc and aid menu is built
//			if (light) scenesInBuild = new[] {scenesInBuild.First()};
//
//			BuildPipeline.BuildPlayer(scenesInBuild, exePath, BuildTarget.StandaloneWindows, BuildOptions.None);
//			BuildPipeline.BuildAssetBundles(dlcPath, BuildAssetBundleOptions.UncompressedAssetBundle,
//				BuildTarget.StandaloneWindows);
//
//			Process.Start(buildPath);
//		}
//
//		#endregion
//
//
//		//[MenuItem("Assets/Build Tool", priority = 3)]
//		private static void CreateWindow()
//		{
//			GetWindow<MyBundleUtility>().Show();
//		}
//
//
//		private bool _buildWindows = true;
//		private bool _buildWindowsDlc = true;
//		private bool _buildLinux = true;
//		private bool _buildLinuxDlc = true;
//		private bool _buildMacOs = true;
//		private bool _buildMacOsDlc = true;
//
//		public void OnGUI()
//		{
//			_buildWindows = EditorGUILayout.Toggle("Windows Build", _buildWindows);
//			_buildWindowsDlc = EditorGUILayout.Toggle("Windows Dlc", _buildWindowsDlc);
//			EditorGUILayout.Space();
//			_buildLinux = EditorGUILayout.Toggle("Linux Build", _buildLinux);
//			_buildLinuxDlc = EditorGUILayout.Toggle("Linux Dlc", _buildLinuxDlc);
//			EditorGUILayout.Space();
//			_buildMacOs = EditorGUILayout.Toggle("MacOS Build", _buildMacOs);
//			_buildMacOsDlc = EditorGUILayout.Toggle("MacOS Dlc", _buildMacOsDlc);
//
//			EditorGUILayout.Space();
//
//			if (GUILayout.Button("Build", MyGUI.ResizableToolbarButtonStyle, GUILayout.Height(40)))
//			{
//				string buildPath = EditorUtility.OpenFolderPanel("Build Output Folder", "", "");
//				if (string.IsNullOrEmpty(buildPath)) return;
//				var sep = Path.DirectorySeparatorChar;
//				buildPath += sep;
//
//				var windowsBuildPath = buildPath + "Windows" + sep + "TheFinalStation.exe";
//				var windowsDlcPath = buildPath + "DlcWindows" + sep + "TheFinalStation_Data";
//
//				var linuxBuildPath = buildPath + "Linux" + sep + "tfs.x86";
//				var linuxDlcPath = buildPath + "DlcLinux" + sep + "tfs_Data";
//
//				var macBuildPath = buildPath + "MacOS" + sep + "TheFinalStation.app";
//				var macDlcPath = buildPath + "DlcMacOS" + sep + "TheFinalStation.app" + sep + "Contents" + sep + "Data";
//
//
//				var scenesInBuild = EditorBuildSettings.scenes.Select(s => s.path).ToArray();
//
//				// Windows
//				PlayerSettings.displayResolutionDialog = ResolutionDialogSetting.HiddenByDefault;
//				if (_buildWindows) BuildPipeline.BuildPlayer(scenesInBuild, windowsBuildPath, BuildTarget.StandaloneWindows, BuildOptions.None);
//				if (_buildWindowsDlc)
//				{
//					Directory.CreateDirectory(windowsDlcPath);
//					BuildPipeline.BuildAssetBundles(windowsDlcPath, BuildAssetBundleOptions.UncompressedAssetBundle, BuildTarget.StandaloneWindows);
//				}
//
//
//				// Linux
//				PlayerSettings.displayResolutionDialog = ResolutionDialogSetting.Enabled;
//				if (_buildLinux) BuildPipeline.BuildPlayer(scenesInBuild, linuxBuildPath, BuildTarget.StandaloneLinuxUniversal, BuildOptions.None);
//				if (_buildLinuxDlc)
//				{
//					Directory.CreateDirectory(linuxDlcPath);
//					BuildPipeline.BuildAssetBundles(linuxDlcPath, BuildAssetBundleOptions.UncompressedAssetBundle, BuildTarget.StandaloneLinuxUniversal);
//				}
//
//				// MacOs
//				PlayerSettings.displayResolutionDialog = ResolutionDialogSetting.Enabled;
//				if (_buildMacOs) BuildPipeline.BuildPlayer(scenesInBuild, macBuildPath, BuildTarget.StandaloneOSX, BuildOptions.None);
//				if (_buildMacOsDlc)
//				{
//					Directory.CreateDirectory(macDlcPath);
//					BuildPipeline.BuildAssetBundles(macDlcPath, BuildAssetBundleOptions.UncompressedAssetBundle, BuildTarget.StandaloneOSX);
//				}
//
//				Process.Start(buildPath);
//			}
//		}
//
//
//		//[MenuItem("Assets/Full Build", priority = 2)]
//		//private static void BuildAll()
//		//{
//		//string currentVersion = GameManager.Instance.Settings.VersionText + GameManager.Instance.Settings.SubVersion;
//		//string nextVersion = GameManager.Instance.Settings.VersionText + (GameManager.Instance.Settings.SubVersion + 1);
//		//bool changeVersion = EditorUtility.DisplayDialog("Change Game version?",
//		//	"Current version is " + currentVersion + ". Change to " + nextVersion + "?", "Change", "No");
//
//		//if (changeVersion) GameManager.Instance.Settings.SubVersion++;
//		//}
//	}
//}
//#endif