#if UNITY_EDITOR
using System.IO;
using UnityEngine;
using System.Linq;

namespace MyBox.Internal
{
    public static class MyBoxUtilities
    {
        public static readonly string MyBoxPackageInfoURL = "https://raw.githubusercontent.com/Deadcows/MyBox/master/package.json";
        public static readonly string ReleasesURL = "https://github.com/Deadcows/MyBox/releases";

        public static readonly string MyBoxPackageTag = "com.mybox";
        public static readonly string MyBoxRepoLink = "https://github.com/Deadcows/MyBox.git";


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
        

        public static string PackageJsonPath
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

        
        public static string ManifestJsonPath
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
    }
}

#endif