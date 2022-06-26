#if UNITY_EDITOR
using System;
using System.IO;
using UnityEngine;

namespace MyBox.Internal
{
	public static class MyBoxSettings
	{
		public static bool AutoSaveEnabled
		{
			get => Data.AutoSaveEnabled;
			set
			{
				if (Data.AutoSaveEnabled == value) return;
				Data.AutoSaveEnabled = value;
				SaveData(Data);
			}
		}

		public static bool CleanEmptyDirectoriesFeature
		{
			get => Data.CleanEmptyDirectoriesFeature;
			set
			{
				if (Data.CleanEmptyDirectoriesFeature == value) return;
				Data.CleanEmptyDirectoriesFeature = value;
				SaveData(Data);
			}
		}

		public static bool PrepareOnPlaymode
		{
			get => Data.PrepareOnPlaymode;
			set
			{
				if (Data.PrepareOnPlaymode == value) return;
				Data.PrepareOnPlaymode = value;
				SaveData(Data);
			}
		}

		public static bool EnableSOCheck
		{
			get => Data.EnableSOCheck;
			set
			{
				if (Data.EnableSOCheck == value) return;
				Data.EnableSOCheck = value;
				SaveData(Data);
			}
		}

		public static bool CheckForUpdates
		{
			get => Data.CheckForUpdates;
			set
			{
				if (Data.CheckForUpdates == value) return;
				Data.CheckForUpdates = value;
				SaveData(Data);
			}
		}

		
		[Serializable]
		private class MyBoxSettingsData
		{
			// ReSharper disable MemberHidesStaticFromOuterClass
			public bool AutoSaveEnabled = true;
			public bool CleanEmptyDirectoriesFeature;
			public bool PrepareOnPlaymode = true;
			public bool EnableSOCheck = true;
			public bool CheckForUpdates = true;
			// ReSharper restore MemberHidesStaticFromOuterClass
		}

		private static MyBoxSettingsData Data => _data ?? (_data = LoadData());
		private static MyBoxSettingsData _data; 
		
		
		#region Save Load

		private static readonly string Directory = "ProjectSettings";
		private static readonly string Path = Directory + "/MyBoxSettings.asset";

		private static MyBoxSettingsData LoadData()
		{
			if (!File.Exists(Path)) return new MyBoxSettingsData();

			MyBoxSettingsData data;
			try
			{
				var jsonData = File.ReadAllText(Path);
				data = JsonUtility.FromJson<MyBoxSettingsData>(jsonData);
			}
			catch
			{
				data = new MyBoxSettingsData();
				TryConvertOndSettingsToNew(data);
				SaveData(data);
			} 
			return data;
		}
		
		private static void TryConvertOndSettingsToNew(MyBoxSettingsData data)
		{
			var fileContents = File.ReadAllLines(Path);
			foreach (var content in fileContents)
			{
				var value = content.Split(':');
				if (value[0].Contains("_autoSaveEnabled")) data.AutoSaveEnabled = int.Parse(value[1]) == 1;
				if (value[0].Contains("_cleanEmptyDirectoriesFeature")) data.CleanEmptyDirectoriesFeature = int.Parse(value[1]) == 1;
				if (value[0].Contains("_prepareOnPlaymode")) data.PrepareOnPlaymode = int.Parse(value[1]) == 1;
				if (value[0].Contains("_checkForUpdates")) data.CheckForUpdates = int.Parse(value[1]) == 1;
			}
		}
		
		private static void SaveData(MyBoxSettingsData data)
		{
			if (!System.IO.Directory.Exists(Directory)) System.IO.Directory.CreateDirectory(Directory);
			try
			{
				File.WriteAllText(Path, JsonUtility.ToJson(data, true));
			}
			catch (Exception ex)
			{
				Debug.LogError("Unable to save MyBoxSettings!\n" + ex);
			}
		}

		#endregion
	}
}
#endif