#if UNITY_EDITOR
using System;
using System.IO;
using UnityEngine;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using UnityEditor;

namespace MyBox.Internal
{
    public static class MyBoxUtilities
    {
        private static readonly string ReleasesURL = "https://github.com/Deadcows/MyBox/releases";
        private static readonly string MyBoxPackageInfoURL = "https://raw.githubusercontent.com/Deadcows/MyBox/master/package.json";

        private static readonly string MyBoxPackageTag = "com.mybox";
        //public static readonly string MyBoxRepoLink = "https://github.com/Deadcows/MyBox.git";

        public static void OpenMyBoxGitInBrowser()
        {
            Application.OpenURL(ReleasesURL);
        }


        #region Get Current / Latest Versions

        public static async void GetMyBoxLatestVersionAsync(Action<MyBoxVersion> onVersionRetrieved)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    var packageJson = await client.GetStringAsync(MyBoxPackageInfoURL);

                    var versionRaw = RetrievePackageVersionOutOfJson(packageJson);
                    if (versionRaw == null)
                    {
                        Debug.LogWarning("MyBox was unable to parse package.json :(");
                        return;
                    }

                    var version = new MyBoxVersion(versionRaw);
                    if (onVersionRetrieved != null) onVersionRetrieved(version);
                }
            }
            catch (HttpRequestException requestException)
            {
                Debug.LogWarning("MyBox is unable to check version online :(. Exception is: " + requestException.Message);
            }
        }

        public static MyBoxVersion GetMyBoxInstalledVersion()
        {
            var packageJsonPath = PackageJsonPath;
            if (packageJsonPath == null)
            {
                Debug.LogWarning("MyBox is unable to check installed version :(");
                return null;
            }

            var packageJsonContents = File.ReadAllText(packageJsonPath);
            var versionRaw = RetrievePackageVersionOutOfJson(packageJsonContents);
            if (versionRaw == null)
            {
                Debug.LogWarning("MyBox was unable to parse package.json :(");
                return null;
            }

            var version = new MyBoxVersion(versionRaw);
            return version;
        }

        private static string RetrievePackageVersionOutOfJson(string json)
        {
            var versionLine = json.Split('\r', '\n').SingleOrDefault(l => l.Contains("version"));
            if (versionLine == null) return null;

            var matches = Regex.Matches(versionLine, "\"(.*?)\"");
            if (matches.Count <= 1 || matches[1].Value.IsNullOrEmpty()) return null;

            return matches[1].Value.Trim('"');
        }

        #endregion


        #region Update Git Packages

        /// <summary>
        /// Remove lock {} section out of manifest.json
        /// </summary>
        /// <returns>is there were any git packages</returns>
        public static bool UpdateGitPackages()
        {
            var manifestFilePath = ManifestJsonPath;
            var manifest = File.ReadAllLines(manifestFilePath).ToList();

            var cutFrom = -1;
            for (int i = 0; i < manifest.Count; i++)
            {
                if (!manifest[i].Contains("\"lock\": {")) continue;

                cutFrom = i;
                break;
            }

            if (cutFrom <= 0) return false;

            manifest[cutFrom - 1] = "}";
            var removeLinesCount = manifest.Count - cutFrom - 1;
            manifest.RemoveRange(cutFrom, removeLinesCount);

            try
            {
                using (StreamWriter sr = new StreamWriter(manifestFilePath))
                {
                    for (int i = 0; i < manifest.Count; i++)
                        sr.WriteLine(manifest[i]);
                }

                AssetDatabase.Refresh(ImportAssetOptions.Default);
            }
            catch (Exception ex)
            {
                Debug.LogWarning("MyBox is unable to rewrite packages.json to update git packages :(. Exception is: " + ex.Message);
            }

            return true;
        }

        #endregion


        #region Installed Via UPM

        public static bool InstalledViaUPM
        {
            get
            {
                if (_installedViaUPMChecked) return _installedViaUPM;

                if (ManifestJsonPath == null)
                {
                    Debug.LogWarning("MyBox is unable to find manifest.json file :(");
                    return false;
                }

                var manifest = File.ReadAllLines(ManifestJsonPath);
                _installedViaUPM = manifest.Any(l => l.Contains(MyBoxPackageTag));
                _installedViaUPMChecked = true;
                return _installedViaUPM;
            }
        }

        private static bool _installedViaUPM;
        private static bool _installedViaUPMChecked;

        #endregion


        #region Package Json Path

        private static string PackageJsonPath
        {
            get
            {
                if (_packageJsonPathChecked) return _packageJsonPath;

                var myBoxDirectory = MyBoxInternalPath.MyBoxDirectory;
                if (myBoxDirectory == null)
                {
                    Debug.LogWarning("MyBox is unable to find the path of the package :(");
                    _packageJsonPathChecked = true;
                    return null;
                }

                var packageJson = myBoxDirectory.GetFiles().SingleOrDefault(f => f.Name == "package.json");
                if (packageJson == null)
                {
                    Debug.LogWarning("MyBox is unable to find package.json file :(");
                    _packageJsonPathChecked = true;
                    return null;
                }

                _packageJsonPath = packageJson.FullName;
                _packageJsonPathChecked = true;
                return _packageJsonPath;
            }
        }

        private static string _packageJsonPath;
        private static bool _packageJsonPathChecked;

        #endregion


        #region Manifest JSON Path

        private static string ManifestJsonPath
        {
            get
            {
                if (_manifestJsonPathChecked) return _manifestJsonPath;

                var packageDir = Application.dataPath.Replace("Assets", "Packages");
                _manifestJsonPath = Directory.GetFiles(packageDir).SingleOrDefault(f => f.EndsWith("manifest.json"));
                _manifestJsonPathChecked = true;
                return _manifestJsonPath;
            }
        }

        private static string _manifestJsonPath;
        private static bool _manifestJsonPathChecked;

        #endregion
    }
}

#endif