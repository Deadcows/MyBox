#if UNITY_EDITOR
using UnityEditor;

namespace MyBox.Internal
{
	[InitializeOnLoad]
	public class MyBoxFeatures
	{
		private const string AutoSaveMenuItemKey = "Tools/MyBox/AutoSave on play";
		private const string CleanupEmptyDirectoriesMenuItemKey = "Tools/MyBox/Clear empty directories On Save";
		private const string IPrepareMenuItemKey = "Tools/MyBox/Run Prepare on play";
		private const string CheckForUpdatesKey = "Tools/MyBox/Check for updates on start";

		static MyBoxFeatures()
		{
			AutoSaveIsEnabled = AutoSaveIsEnabled;
			CleanupEmptyDirectoriesIsEnabled = CleanupEmptyDirectoriesIsEnabled;
			IPrepareIsEnabled = IPrepareIsEnabled;
			CheckForUpdatesEnabled = CheckForUpdatesEnabled;
		}


		#region AutoSave

		private static bool AutoSaveIsEnabled
		{
			get => MyBoxSettings.AutoSaveEnabled;
			set
			{
				{
					MyBoxSettings.AutoSaveEnabled = value;
					AutoSaveFeature.IsEnabled = value;
				}
			}
		}

		[MenuItem(AutoSaveMenuItemKey, priority = 100)]
		private static void AutoSaveMenuItem()
		{
			AutoSaveIsEnabled = !AutoSaveIsEnabled;
		}

		[MenuItem(AutoSaveMenuItemKey, true)]
		private static bool AutoSaveMenuItemValidation()
		{
			Menu.SetChecked(AutoSaveMenuItemKey, AutoSaveIsEnabled);
			return true;
		}

		#endregion


		#region CleanupEmptyDirectories

		private static bool CleanupEmptyDirectoriesIsEnabled
		{
			get => MyBoxSettings.CleanEmptyDirectoriesFeature;
			set
			{
				{
					MyBoxSettings.CleanEmptyDirectoriesFeature = value;
					CleanEmptyDirectoriesFeature.IsEnabled = value;
				}
			}
		}

		[MenuItem(CleanupEmptyDirectoriesMenuItemKey, priority = 100)]
		private static void CleanupEmptyDirectoriesMenuItem()
		{
			CleanupEmptyDirectoriesIsEnabled = !CleanupEmptyDirectoriesIsEnabled;
		}

		[MenuItem(CleanupEmptyDirectoriesMenuItemKey, true)]
		private static bool CleanupEmptyDirectoriesMenuItemValidation()
		{
			Menu.SetChecked(CleanupEmptyDirectoriesMenuItemKey, CleanupEmptyDirectoriesIsEnabled);
			return true;
		}

		#endregion


		#region IPrepare

		private static bool IPrepareIsEnabled
		{
			get => MyBoxSettings.PrepareOnPlaymode;
			set
			{
				{
					MyBoxSettings.PrepareOnPlaymode = value;
					EditorTools.IPrepareFeature.IsEnabled = value;
				}
			}
		}

		[MenuItem(IPrepareMenuItemKey, priority = 100)]
		private static void IPrepareMenuItem()
		{
			IPrepareIsEnabled = !IPrepareIsEnabled;
		}

		[MenuItem(IPrepareMenuItemKey, true)]
		private static bool IPrepareMenuItemValidation()
		{
			Menu.SetChecked(IPrepareMenuItemKey, IPrepareIsEnabled);
			return true;
		}

		#endregion
		
		
		#region Check For Updates

		private static bool CheckForUpdatesEnabled
		{
			get => MyBoxSettings.CheckForUpdates;
			set
			{
				{
					MyBoxSettings.CheckForUpdates = value;
					MyBoxWindow.AutoUpdateCheckIsEnabled = value;
				}
			}
		}

		[MenuItem(CheckForUpdatesKey, priority = 100)]
		private static void CheckForUpdatesMenuItem()
		{
			CheckForUpdatesEnabled = !CheckForUpdatesEnabled;
		}

		[MenuItem(CheckForUpdatesKey, true)]
		private static bool CheckForUpdatesMenuItemValidation()
		{
			Menu.SetChecked(CheckForUpdatesKey, CheckForUpdatesEnabled);
			return true;
		}

		#endregion
	}
}
#endif