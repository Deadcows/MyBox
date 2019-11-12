#if UNITY_EDITOR
using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using MyBox.EditorTools;
using UnityEditor;
using UnityEngine;

namespace MyBox.Internal
{
	[InitializeOnLoad]
	public class MyBoxUpdateWindow : EditorWindow
	{
		public static bool IsEnabled = true;

		private static Version _currentVersion;
		private static Version _latestVersion;

		
		private static EditorWindow _windowInstance;
		

		static MyBoxUpdateWindow()
		{
			return;
			if (!IsEnabled) return;

			CheckForUpdate();
		}


		[MenuItem("Tools/MyBox/Update window")]
		private static void MuBoxUpdateMenuItem()
		{
			_windowInstance = GetWindow<MyBoxUpdateWindow>();
			_windowInstance.titleContent = new GUIContent("Update MyBox");
		}


		private void OnEnable()
		{
			_windowInstance = this;
			CheckForUpdate();
		}


		private void OnGUI()
		{
			if (_currentVersion == null) return;

			EditorGUILayout.LabelField("Current version: " + _currentVersion.AsSting);
			EditorGUILayout.LabelField("Latest version: " + (_latestVersion == null ? "..." : _latestVersion.AsSting));

			bool anyUpdate = _latestVersion != null && !_currentVersion.FullMatch(_latestVersion);
			if (GUILayout.Button(anyUpdate ? "Update" : "Force Update", EditorStyles.toolbarButton))
			{
				if (!MyBoxUtilities.InstalledViaUPM) Application.OpenURL(MyBoxUtilities.ReleasesURL);
				else UpdatePackage();
			}
		}

		private static void CheckForUpdate()
		{
			CheckCurrentVersion();
			CheckOnlineVersionAsync();
		}


		private static void UpdatePackage()
		{
			// TODO: Latest version should be valid
			var manifestFile = MyBoxUtilities.ManifestJsonPath;
			var manifest = File.ReadAllLines(manifestFile);
			var myBoxLine = manifest.SingleOrDefault(l => l.Contains(MyBoxUtilities.MyBoxRepoLink));
			if (string.IsNullOrEmpty(myBoxLine)) return; // TODO: Exceptional

			var indexOfMyBoxLine = manifest.IndexOfItem(myBoxLine);
			var indent = myBoxLine.Substring(0, myBoxLine.IndexOf('"'));
			myBoxLine = myBoxLine.Trim();
			bool withComma = myBoxLine.EndsWith(",");

			var tagWrapped = "\"" + MyBoxUtilities.MyBoxPackageTag + "\"";
			var separator = ": ";
			var version = "#" + _latestVersion;
			var repoLinkWrapped = "\"" + MyBoxUtilities.MyBoxRepoLink + version + "\"";
			var comma = withComma ? "," : "";

			var newLine = indent + tagWrapped + separator + repoLinkWrapped + comma;
			manifest[indexOfMyBoxLine] = newLine;
		}


		private static async void CheckOnlineVersionAsync()
		{
			//TODO: Try Catch Exceptional
			using (HttpClient wc = new HttpClient())
			{
				var packageJson = await wc.GetStringAsync(MyBoxUtilities.MyBoxPackageInfoURL);
				_latestVersion = new Version(ParsePackageVersion(packageJson));
				if (_windowInstance != null) _windowInstance.Repaint();

				if (!_currentVersion.BaseVersionMatch(_latestVersion))
				{
					Debug.Log("It's time to update MyBox :)! Use \"Tools/MyBox/Update window\". Current version: " +
					          _currentVersion + ", new version: " + _latestVersion);
				}
			}
		}

		private static void CheckCurrentVersion()
		{
			var packageJsonPath = MyBoxUtilities.PackageJsonPath;
			if (packageJsonPath == null)
			{
				Debug.LogWarning("MyBox is unable to check installed version :(");
				return;
			}
			var packageJsonContents = File.ReadAllText(packageJsonPath);
			_currentVersion = new Version(ParsePackageVersion(packageJsonContents));
		}

		private static string ParsePackageVersion(string json)
		{
			var versionLine = json.Split('\r', '\n').SingleOrDefault(l => l.Contains("version"));
			if (versionLine == null) return null; //TODO: Exceptional
			var matches = Regex.Matches(versionLine, "\"(.*?)\"");
			if (matches.Count <= 1 || matches[1].Value.IsNullOrEmpty()) return null; //TODO: Exceptional

			return matches[1].Value.Trim('"');
		}
		

		#region Version Type
		
		[Serializable]
		private class Version
		{
			public string Major;
			public string Minor;
			public string Patch;

			public string AsSting;

			/// <param name="version">NUM.NUM.NUM format</param>
			public Version(string version)
			{
				AsSting = version;
				var v = version.Split('.');
				Major = v[0];
				Minor = v[1];
				Patch = v[2];
			}

			/// <summary>
			/// Major & Minor versions match, skip patch releases
			/// </summary>
			public bool BaseVersionMatch(Version version)
			{
				return Major == version.Major && Minor == version.Minor;
			}

			public bool FullMatch(Version version)
			{
				return BaseVersionMatch(version) && Patch == version.Patch;
			}
		}
		
		#endregion
	}
}
#endif