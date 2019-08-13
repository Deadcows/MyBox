#if UNITY_EDITOR
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using MyBox.EditorTools;
using UnityEditor;
using UnityEngine;

namespace MyBox.Internal
{
	public class MyBoxUpdate : EditorWindow
	{
		private readonly string MyBoxPackageInfoURL = "https://raw.githubusercontent.com/Deadcows/MyBox/master/package.json";
		private readonly string ReleasesURL = "https://github.com/Deadcows/MyBox/releases";

		private readonly string MyBoxPackageTag = "com.mybox";
		private readonly string MyBoxRepoLink = "https://github.com/Deadcows/MyBox.git";
		
		[MenuItem("Tools/MyBox/Check for updates")]
		private static void MuBoxUpdateMenuItem()
		{
			GetWindow<MyBoxUpdate>();
		}
		
		private string _currentVersion;
		private string _latestVersion;
		private void OnGUI()
		{
			if (GUILayout.Button("Check for updates", EditorStyles.toolbarButton))
			{
				CheckCurrentVersion();
				CheckOnlineVersionAsync();

				Debug.Log(IsUPMVersion() ? "UPM" : "Raw installation");
			}
			_currentVersion = EditorGUILayout.TextField(_currentVersion);
			_latestVersion = EditorGUILayout.TextField(_latestVersion);
			
			if (GUILayout.Button("Update", EditorStyles.toolbarButton))
			{
				if (!IsUPMVersion()) Application.OpenURL(ReleasesURL);
				else UpdatePackage();
			}
		}

		private bool IsUPMVersion()
		{
			var manifestFile = GetPackagesManifest();
			if (manifestFile == null) return false; // TODO: Exceptional
			
			var manifest = File.ReadAllLines(manifestFile);
			return manifest.Any(l => l.Contains(MyBoxPackageTag));
		}

		private void UpdatePackage()
		{
			// TODO: Latest version should be valid
			var manifestFile = GetPackagesManifest();
			var manifest = File.ReadAllLines(manifestFile);
			var myBoxLine = manifest.SingleOrDefault(l => l.Contains(MyBoxRepoLink));
			if (myBoxLine.IsNullOrEmpty()) return; // TODO: Exceptional
	
			var indexOfMyBoxLine = manifest.IndexOfItem(myBoxLine);
			myBoxLine = myBoxLine.Trim();
			bool withComma = myBoxLine.EndsWith(",");

			var tagWrapped = "\"" + MyBoxPackageTag + "\"";
			var separator = ": ";
			var version = "#" + _latestVersion;
			var repoLinkWrapped = "\"" + MyBoxRepoLink + version + "\"";
			var comma = withComma ? "," : "";

			MyDebug.LogArray(manifest);
			
			var newLine = tagWrapped + separator + version + repoLinkWrapped + comma;
			manifest[indexOfMyBoxLine] = newLine;

			Debug.Log(newLine);
			MyDebug.LogArray(manifest);
		}

		private string GetPackagesManifest()
		{
			var packageDir = Application.dataPath.Replace("Assets", "Packages");
			return Directory.GetFiles(packageDir).SingleOrDefault(f => f.EndsWith("manifest.json"));
		}
		
		
		#region Get Versions

		private async void CheckOnlineVersionAsync()
		{
			//TODO: Try Catch Exceptional
			using (HttpClient wc = new HttpClient())
			{
				var packageJson = await wc.GetStringAsync(MyBoxPackageInfoURL);
				_latestVersion = ParsePackageVersion(packageJson);
				Repaint();
			}
		}
		
		private void CheckCurrentVersion()
		{
			var scriptPath = MyEditor.GetScriptAssetPath(this);
			var scriptDirectory = new DirectoryInfo(scriptPath);
			
			// Script is in MyBox/Tools/Internal so we need to get dir two steps up in hierarchy
			if (scriptDirectory.Parent == null || scriptDirectory.Parent.Parent == null) return; //TODO: Exceptional
			var myBoxDirectory = scriptDirectory.Parent.Parent;
			
			var packageJson = myBoxDirectory.GetFiles().SingleOrDefault(f => f.Name == "package.json");
			if (packageJson == null) return; //TODO: Exceptional

			_currentVersion = ParsePackageVersion(File.ReadAllText(packageJson.FullName));
		}

		private string ParsePackageVersion(string json)
		{
			var versionLine = json.Split('\r', '\n').SingleOrDefault(l => l.Contains("version"));
			if (versionLine == null) return null; //TODO: Exceptional
			var matches = Regex.Matches(versionLine, "\"(.*?)\"");
			if (matches.Count <= 1 || matches[1].Value.IsNullOrEmpty()) return null; //TODO: Exceptional

			return matches[1].Value.Trim('"');
		}

		#endregion
		
		
	}
}
#endif