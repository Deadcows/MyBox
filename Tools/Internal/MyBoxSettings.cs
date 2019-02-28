#if UNITY_EDITOR
using System;
using System.IO;
using UnityEngine;
using Object = UnityEngine.Object;

namespace MyBox.Internal
{
	[Serializable]
	public class MyBoxSettings : ScriptableObject
	{
		[SerializeField] private bool _autoSaveEnabled = true;
		[SerializeField] private bool _cleanEmptyDirectoriesFeature = true;

		public static bool AutoSaveEnabled
		{
			get { return Instance._autoSaveEnabled; }
			set
			{
				if (Instance._autoSaveEnabled == value) return;
				Instance._autoSaveEnabled = value;
				Save();
			}
		}

		public static bool CleanEmptyDirectoriesFeature
		{
			get { return Instance._cleanEmptyDirectoriesFeature; }
			set
			{
				if (Instance._cleanEmptyDirectoriesFeature == value) return;
				Instance._cleanEmptyDirectoriesFeature = value;
				Save();
			}
		}


		#region Instance

		private static MyBoxSettings Instance
		{
			get
			{
				if (_instance != null) return _instance;
				_instance = LoadOrCreate();
				return _instance;
			}
		}

		private const string Directory = "ProjectSettings";
		private const string Path = Directory + "/MyBoxSettings.asset";
		private static MyBoxSettings _instance;

		private static void Save()
		{
			var instance = _instance;
			if (!System.IO.Directory.Exists(Directory)) System.IO.Directory.CreateDirectory(Directory);
			try
			{
				UnityEditorInternal.InternalEditorUtility.SaveToSerializedFileAndForget(new Object[] {instance}, Path, true);
			}
			catch (Exception ex)
			{
				Debug.LogError("Unable to save MyBoxSettings!\n" + ex);
			}
		}

		private static MyBoxSettings LoadOrCreate()
		{
			var settings = !File.Exists(Path) ? CreateNewSettings() : LoadSettings();
			if (settings == null)
			{
				DeleteFile(Path);
				settings = CreateNewSettings();
			}

			settings.hideFlags = HideFlags.HideAndDontSave;

			return settings;
		}


		private static MyBoxSettings LoadSettings()
		{
			MyBoxSettings settingsInstance;
			try
			{
				settingsInstance = (MyBoxSettings) UnityEditorInternal.InternalEditorUtility.LoadSerializedFileAndForget(Path)[0];
			}
			catch (Exception ex)
			{
				Debug.LogError("Unable to read MyBoxSettings, set to defaults" + ex);
				settingsInstance = null;
			}

			return settingsInstance;
		}

		private static MyBoxSettings CreateNewSettings()
		{
			_instance = CreateInstance<MyBoxSettings>();
			Save();

			return _instance;
		}

		private static void DeleteFile(string path)
		{
			if (!File.Exists(path)) return;

			var attributes = File.GetAttributes(path);
			if ((attributes & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
				File.SetAttributes(path, attributes & ~FileAttributes.ReadOnly);

			File.Delete(path);
		}

		#endregion
	}
}
#endif