#if UNITY_EDITOR
using System;

namespace MyBox.Internal
{
    [Serializable]
    public class MyBoxVersion
    {
        public string Major;
        public string Minor;
        public string Patch;

        public string AsSting;

        /// <param name="version">NUM.NUM.NUM format</param>
        public MyBoxVersion(string version)
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
        public bool BaseVersionMatch(MyBoxVersion version)
        {
            return Major == version.Major && Minor == version.Minor;
        }

        public bool VersionsMatch(MyBoxVersion version)
        {
            return BaseVersionMatch(version) && Patch == version.Patch;
        }
    }
}
#endif