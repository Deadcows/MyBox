#if UNITY_EDITOR
using System;
using UnityEditor;
using MyBox.Internal;
using UnityEngine;

#endif

namespace MyBox
{
	public interface IPrepare
	{
		/// <summary>
		/// Prepare component to cache 
		/// </summary>
		/// <returns></returns>
		bool Prepare();
	}
}

#if UNITY_EDITOR

namespace MyBox.EditorTools
{
	[InitializeOnLoad]
	public class PrepareHandler
	{
		#region Menu Items Settings

		private const string PrepareItemName = "Tools/MyBox/IPrepare/Prepare";
		private const string BeforePlaymodeItemName = "Tools/MyBox/IPrepare/BeforePlaymode";
		private const string BeforeBuildItemName = "Tools/MyBox/IPrepare/BeforeBuild";
		private const string OnSaveItemName = "Tools/MyBox/IPrepare/OnSave";

		[MenuItem(PrepareItemName, priority = 109)]
		private static void MenuItemPrepare()
		{
			Prepare();
		}


		private static bool BeforePlaymode
		{
			get { return MyBoxSettings.IPrepareBeforePlaymode; }
			set { MyBoxSettings.IPrepareBeforePlaymode = value; }
		}

		private static bool BeforeBuild
		{
			get { return MyBoxSettings.IPrepareBeforeBuild; }
			set { MyBoxSettings.IPrepareBeforeBuild = value; }
		}

		private static bool OnSave
		{
			get { return MyBoxSettings.IPrepareOnSave; }
			set { MyBoxSettings.IPrepareOnSave = value; }
		}

		[MenuItem(BeforePlaymodeItemName, priority = 110)]
		private static void MenuItemBeforePlaymode()
		{
			BeforePlaymode = !BeforePlaymode;
		}

		[MenuItem(BeforePlaymodeItemName, true)]
		private static bool MenuItemValidationBeforePlaymode()
		{
			Menu.SetChecked(BeforePlaymodeItemName, BeforePlaymode);
			return true;
		}


		[MenuItem(BeforeBuildItemName, priority = 111)]
		private static void MenuItemBeforeBuild()
		{
			BeforeBuild = !BeforeBuild;
		}

		[MenuItem(BeforeBuildItemName, true)]
		private static bool MenuItemValidationBeforeBuild()
		{
			Menu.SetChecked(BeforeBuildItemName, BeforeBuild);
			return true;
		}


		[MenuItem(OnSaveItemName, priority = 112)]
		private static void MenuItemOnSave()
		{
			OnSave = !OnSave;
		}

		[MenuItem(OnSaveItemName, true)]
		private static bool MenuItemValidationOnSave()
		{
			Menu.SetChecked(OnSaveItemName, OnSave);
			return true;
		}

		#endregion

		public static Action OnPrepare;

		static PrepareHandler()
		{
			MyEditorEvents.BeforePlaymode += Prepare;
			MyEditorEvents.OnSave += Prepare;
			MyEditorEvents.BeforeBuild += Prepare;
		}

		public static void Prepare()
		{
			var toPrepare = MyExtensions.FindObjectsOfInterfaceAsComponents<IPrepare>();
			foreach (var prepare in toPrepare)
			{
				bool updated = prepare.Interface.Prepare();
				if (prepare.Component != null && updated)
				{
					EditorUtility.SetDirty(prepare.Component);
					// Log in case we don't know what makes scenes dirty
					Debug.Log(prepare.Component.name + " Prepared, changed", prepare.Component);
				}
			}

			if (OnPrepare != null) OnPrepare();
		}
	}
}

#endif