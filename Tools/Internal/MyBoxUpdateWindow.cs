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


		private static readonly string MyBoxPackageInfoURL = "https://raw.githubusercontent.com/Deadcows/MyBox/master/package.json";
		private static readonly string ReleasesURL = "https://github.com/Deadcows/MyBox/releases";

		private static readonly string MyBoxPackageTag = "com.mybox";
		private static readonly string MyBoxRepoLink = "https://github.com/Deadcows/MyBox.git";


		private static bool _isUPMVersion;
		private static Version _currentVersion;
		private static Version _latestVersion;

		private static EditorWindow _windowInstance;

		static MyBoxUpdateWindow()
		{
			if (!IsEnabled) return;

			try
			{
				CheckForUpdate(true);
			}
			catch (Exception)
			{
			}
		}


		[MenuItem("Tools/MyBox/Update window")]
		private static void MuBoxUpdateMenuItem()
		{
			_windowInstance = GetWindow<MyBoxUpdateWindow>();
			_windowInstance.titleContent = new GUIContent("Update MyBox");
		}


		private void OnEnable()
		{
			CheckForUpdate();
			_isUPMVersion = IsUPMVersion();
		}


		private void OnGUI()
		{
			if (_currentVersion == null) return;

			EditorGUILayout.LabelField("Current version: " + _currentVersion.AsSting);
			EditorGUILayout.LabelField("Latest version: " + (_latestVersion == null ? "..." : _latestVersion.AsSting));

			bool anyUpdate = _latestVersion != null && _currentVersion.FullMatch(_latestVersion);
			if (GUILayout.Button(anyUpdate ? "Update" : "Force Update", EditorStyles.toolbarButton))
			{
				if (!_isUPMVersion) Application.OpenURL(ReleasesURL);
				else UpdatePackage();
			}
		}

		private static void CheckForUpdate(bool withLog = false)
		{
			CheckCurrentVersion();
			CheckOnlineVersionAsync(withLog);
		}


		private static void UpdatePackage()
		{
			// TODO: Latest version should be valid
			var manifestFile = GetPackagesManifest();
			var manifest = File.ReadAllLines(manifestFile);
			var myBoxLine = manifest.SingleOrDefault(l => l.Contains(MyBoxRepoLink));
			if (string.IsNullOrEmpty(myBoxLine)) return; // TODO: Exceptional

			var indexOfMyBoxLine = manifest.IndexOfItem(myBoxLine);
			var indent = myBoxLine.Substring(0, myBoxLine.IndexOf('"'));
			myBoxLine = myBoxLine.Trim();
			bool withComma = myBoxLine.EndsWith(",");

			var tagWrapped = "\"" + MyBoxPackageTag + "\"";
			var separator = ": ";
			var version = "#" + _latestVersion;
			var repoLinkWrapped = "\"" + MyBoxRepoLink + version + "\"";
			var comma = withComma ? "," : "";

			var newLine = indent + tagWrapped + separator + repoLinkWrapped + comma;
			manifest[indexOfMyBoxLine] = newLine;
		}

		private static bool IsUPMVersion()
		{
			var manifestFile = GetPackagesManifest();
			if (manifestFile == null) return false; // TODO: Exceptional

			var manifest = File.ReadAllLines(manifestFile);
			return manifest.Any(l => l.Contains(MyBoxPackageTag));
		}

		private static string GetPackagesManifest()
		{
			var packageDir = Application.dataPath.Replace("Assets", "Packages");
			return Directory.GetFiles(packageDir).SingleOrDefault(f => f.EndsWith("manifest.json"));
		}


		private static async void CheckOnlineVersionAsync(bool withLog)
		{
			//TODO: Try Catch Exceptional
			using (HttpClient wc = new HttpClient())
			{
				var packageJson = await wc.GetStringAsync(MyBoxPackageInfoURL);
				_latestVersion = new Version(ParsePackageVersion(packageJson));
				if (_windowInstance != null) _windowInstance.Repaint();

				if (!_currentVersion.BaseVersionMatch(_latestVersion) && withLog)
				{
					Debug.Log("It's time to update MyBox :)! Use \"Tools/MyBox/Update window\". Current version: " +
					          _currentVersion + ", new version: " + _latestVersion);
				}
			}
		}

		private static void CheckCurrentVersion()
		{
			var scriptPath = MyEditor.GetScriptAssetPath(MyBoxUpdateWindowLocation.Instance);
			var scriptDirectory = new DirectoryInfo(scriptPath);

			// Script is in MyBox/Tools/Internal so we need to get dir two steps up in hierarchy
			if (scriptDirectory.Parent == null || scriptDirectory.Parent.Parent == null) return; //TODO: Exceptional
			var myBoxDirectory = scriptDirectory.Parent.Parent;

			var packageJson = myBoxDirectory.GetFiles().SingleOrDefault(f => f.Name == "package.json");
			if (packageJson == null) return; //TODO: Exceptional

			_currentVersion = new Version(ParsePackageVersion(File.ReadAllText(packageJson.FullName)));
		}

		private static string ParsePackageVersion(string json)
		{
			var versionLine = json.Split('\r', '\n').SingleOrDefault(l => l.Contains("version"));
			if (versionLine == null) return null; //TODO: Exceptional
			var matches = Regex.Matches(versionLine, "\"(.*?)\"");
			if (matches.Count <= 1 || matches[1].Value.IsNullOrEmpty()) return null; //TODO: Exceptional

			return matches[1].Value.Trim('"');
		}

		[Serializable]
		private class Version
		{
			public string Major;
			public string Minor;
			public string Patch;

			public string AsSting;

			public Version(string version)
			{
				AsSting = version;
				var v = version.Split('.');
				Major = v[0];
				Minor = v[1];
				Patch = v[2];
			}

			public bool BaseVersionMatch(Version version)
			{
				return Major == version.Major && Minor == version.Minor;
			}

			public bool FullMatch(Version version)
			{
				return BaseVersionMatch(version) && Patch == version.Patch;
			}
		}
	}
}
#endif