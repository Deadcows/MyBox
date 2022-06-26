#if UNITY_EDITOR
using UnityEditor;

namespace MyBox.Internal
{
	public static class MyBoxMenuItems
	{
		private const string Base = "Tools/MyBox/";
		private const string AutoSaveMenuItemKey = Base + "AutoSave on play";
		private const string CleanupEmptyDirectoriesMenuItemKey = Base + "Clear empty directories On Save";
		private const string PrepareMenuItemKey = Base + "Run Prepare on play";
		private const string CheckForUpdatesKey = Base + "Check for updates on start";


		#region AutoSave

		[MenuItem(AutoSaveMenuItemKey, priority = 100)]
		private static void AutoSaveMenuItem()
			=> MyBoxSettings.AutoSaveEnabled = !MyBoxSettings.AutoSaveEnabled;

		[MenuItem(AutoSaveMenuItemKey, true)]
		private static bool AutoSaveMenuItemValidation()
		{
			Menu.SetChecked(AutoSaveMenuItemKey, MyBoxSettings.AutoSaveEnabled);
			return true;
		}

		#endregion


		#region CleanupEmptyDirectories

		[MenuItem(CleanupEmptyDirectoriesMenuItemKey, priority = 100)]
		private static void CleanupEmptyDirectoriesMenuItem()
			=> MyBoxSettings.CleanEmptyDirectoriesFeature = !MyBoxSettings.CleanEmptyDirectoriesFeature;

		[MenuItem(CleanupEmptyDirectoriesMenuItemKey, true)]
		private static bool CleanupEmptyDirectoriesMenuItemValidation()
		{
			Menu.SetChecked(CleanupEmptyDirectoriesMenuItemKey, MyBoxSettings.CleanEmptyDirectoriesFeature);
			return true;
		}

		#endregion


		#region Prepare

		[MenuItem(PrepareMenuItemKey, priority = 100)]
		private static void PrepareMenuItem()
			=> MyBoxSettings.PrepareOnPlaymode = !MyBoxSettings.PrepareOnPlaymode;

		[MenuItem(PrepareMenuItemKey, true)]
		private static bool PrepareMenuItemValidation()
		{
			Menu.SetChecked(PrepareMenuItemKey, MyBoxSettings.PrepareOnPlaymode);
			return true;
		}

		#endregion


		#region Check For Updates

		[MenuItem(CheckForUpdatesKey, priority = 100)]
		private static void CheckForUpdatesMenuItem()
			=> MyBoxSettings.CheckForUpdates = !MyBoxSettings.CheckForUpdates;

		[MenuItem(CheckForUpdatesKey, true)]
		private static bool CheckForUpdatesMenuItemValidation()
		{
			Menu.SetChecked(CheckForUpdatesKey, MyBoxSettings.CheckForUpdates);
			return true;
		}

		#endregion
	}
}
#endif