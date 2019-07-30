using UnityEditor;

namespace MyBox.Internal
{
	[InitializeOnLoad]
	public class MyBoxFeatures
	{
		private const string AutoSaveMenuItemKey = "Tools/MyBox/AutoSave on Play";
		private const string CleanupEmptyDirectoriesMenuItemKey = "Tools/MyBox/Clear Empty Directories On Save";
		private const string IPrepareMenuItemKey = "Tools/MyBox/Run Prepare On Save";

		static MyBoxFeatures()
		{
			AutoSaveIsEnabled = AutoSaveIsEnabled;
			CleanupEmptyDirectoriesIsEnabled = CleanupEmptyDirectoriesIsEnabled;
			IPrepareIsEnabled = IPrepareIsEnabled;
		}


		#region AutoSave

		private static bool AutoSaveIsEnabled
		{
			get { return MyBoxSettings.AutoSaveEnabled; }
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
			get { return MyBoxSettings.CleanEmptyDirectoriesFeature; }
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
			get { return MyBoxSettings.PrepareOnPlaymode; }
			set
			{
				{
					MyBoxSettings.PrepareOnPlaymode = value;
					PrepareOnSave.IsEnabled = value;
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
	}
}