#if UNITY_EDITOR
using MyBox.EditorTools;
using UnityEngine;
using System.IO;

namespace MyBox.Internal
{
    /// <summary>
    /// SO is needed to determine the path to this script.
    /// Thereby it's used to get relative path to MyBox
    /// </summary>
    public class MyBoxInternalPath : ScriptableObject
    {
        /// <summary>
        /// Absolute path to MyBox folder
        /// </summary>
        public static DirectoryInfo MyBoxDirectory
        {
            get
            {
                if (_directoryChecked) return _myBoxDirectory;
                
                var internalPath = MyEditor.GetScriptAssetPath(Instance);
                var scriptDirectory = new DirectoryInfo(internalPath);

                // Script is in MyBox/Tools/Internal so we need to get dir two steps up in hierarchy
                if (scriptDirectory.Parent == null || scriptDirectory.Parent.Parent == null)
                {
                    _directoryChecked = true;
                    return null;
                }

                _myBoxDirectory = scriptDirectory.Parent.Parent;
                _directoryChecked = true;
                return _myBoxDirectory;
            }
        }

        private static DirectoryInfo _myBoxDirectory;
        private static bool _directoryChecked;

        private static MyBoxInternalPath Instance
        {
            get
            {
                if (_instance != null) return _instance;
                return _instance = CreateInstance<MyBoxInternalPath>();
            }
        }

        private static MyBoxInternalPath _instance;
    }
}
#endif