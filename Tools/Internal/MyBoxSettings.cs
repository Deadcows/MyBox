#if UNITY_EDITOR
using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace MyBox.Internal
{
    [Serializable]
    [FilePath("ProjectSettings/MyBox Settings.asset", FilePathAttribute.Location.ProjectFolder)]
    public class MyBoxSettings : ScriptableSingleton<MyBoxSettings>
    {
        [SerializeField] private bool _autoSaveEnabled = true;
        [SerializeField] private bool _cleanEmptyDirectoriesFeature;
        [SerializeField] private bool _prepareOnPlaymode = true;
        [SerializeField] private bool _checkForUpdates = true;


        public static bool AutoSaveEnabled
        {
            get => instance._autoSaveEnabled;
            set
            {
                if (instance._autoSaveEnabled == value) return;
                instance._autoSaveEnabled = value;
                instance.Save(true);
            }
        }

        public static bool CleanEmptyDirectoriesFeature
        {
            get => instance._cleanEmptyDirectoriesFeature;
            set
            {
                if (instance._cleanEmptyDirectoriesFeature == value) return;
                instance._cleanEmptyDirectoriesFeature = value;
                instance.Save(true);
            }
        }

        public static bool PrepareOnPlaymode
        {
            get => instance._prepareOnPlaymode;
            set
            {
                if (instance._prepareOnPlaymode == value) return;
                instance._prepareOnPlaymode = value;
                instance.Save(true);
            }
        }

        public static bool CheckForUpdates
        {
            get => instance._checkForUpdates;
            set
            {
                if (instance._checkForUpdates == value) return;
                instance._checkForUpdates = value;
                instance.Save(true);
            }
        }

        private void Awake()
        {
            CopyOldSettings();

            void CopyOldSettings()
            {
                var oldSettingsPath = "ProjectSettings/MyBoxSettings.asset";
                if (!File.Exists(oldSettingsPath)) return;
                
                var settingsLines = File.ReadAllLines(oldSettingsPath);
                foreach (var settingsLine in settingsLines)
                {
                    if (!settingsLine.Contains(":")) continue;
                    var setting = settingsLine.Split(':');
                    switch (setting[0].Trim())
                    {
                        case "_autoSaveEnabled":
                            _autoSaveEnabled = setting[1].Trim() == "1";
                            continue;
                        case "_cleanEmptyDirectoriesFeature":
                            _cleanEmptyDirectoriesFeature = setting[1].Trim() == "1";
                            continue;
                        case "_prepareOnPlaymode":
                            _prepareOnPlaymode = setting[1].Trim() == "1";
                            continue;
                        case "_checkForUpdates":
                            _checkForUpdates = setting[1].Trim() == "1";
                            continue;
                    }
                }
                Save(true);
                File.Delete(oldSettingsPath);
                Debug.Log("MyBox Settings file updated");
            }
        }
    }
}
#endif