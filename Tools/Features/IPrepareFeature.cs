namespace MyBox
{
	public interface IPrepare
	{
		bool Prepare();
	}
}

#if UNITY_EDITOR
namespace MyBox.Internal
{
	using UnityEngine;
	using UnityEditor;

	public class PrepareOnSave : AssetModificationProcessor
	{
		private const string MenuItemName = "Tools/MyBox/Run Prepare On Save";
		
		private static bool IsEnabled
		{
			get { return MyBoxSettings.IPrepareOnSave; }
			set { MyBoxSettings.IPrepareOnSave = value; }
		}

		[MenuItem(MenuItemName, priority = 100)]
		private static void MenuItem()
		{
			IsEnabled = !IsEnabled;
		}

		[MenuItem(MenuItemName, true)]
		private static bool MenuItemValidation()
		{
			Menu.SetChecked(MenuItemName, IsEnabled);
			return true;
		}
		
		private static string[] OnWillSaveAssets(string[] paths)
		{
			if (!IsEnabled) return paths;
			
			var toPrepare = MyExtensions.FindObjectsOfInterfaceAsComponents<IPrepare>();

			foreach (var prepare in toPrepare)
			{
				bool changed = prepare.Interface.Prepare();

				if (changed && prepare.Component != null)
				{
					EditorUtility.SetDirty(prepare.Component);
					Debug.Log(prepare.Component.name + "." + prepare.Component.GetType().Name + ": Changed on Prepare", prepare.Component);
				}
			}

			return paths;
		}
	}
}
#endif