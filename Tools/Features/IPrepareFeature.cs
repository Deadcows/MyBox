using MyBox.EditorTools;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

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
	using System.Collections.Generic;
	using System.Linq;
	using UnityEngine;
	using UnityEditor;

	[InitializeOnLoad]
	public class PrepareOnSave
	{
		private const string IPrepareMenuItemKey = "Tools/MyBox/Run Prepare On Save";

		private static bool IPrepareIsEnabled
		{
			get { return MyBoxSettings.PrepareOnPlaymode; }
			set { MyBoxSettings.PrepareOnPlaymode = value; }
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

		static PrepareOnSave()
		{
			MyEditorEvents.BeforePlaymode += Prepare;
		}

		static void Prepare()
		{
			if (!IPrepareIsEnabled) return;
			
			var toPrepare = MyExtensions.FindObjectsOfInterfaceAsComponents<IPrepare>();

			HashSet<Scene> modifiedScenes = null;
			foreach (var prepare in toPrepare)
			{
				bool changed = prepare.Interface.Prepare();

				if (changed && prepare.Component != null)
				{
					if (modifiedScenes == null) modifiedScenes = new HashSet<Scene>();
					modifiedScenes.Add(prepare.Component.gameObject.scene);

					EditorUtility.SetDirty(prepare.Component);
					Debug.Log(prepare.Component.name + "." + prepare.Component.GetType().Name + ": Changed on Prepare", prepare.Component);
				}
			}

			if (modifiedScenes != null) EditorSceneManager.SaveScenes(modifiedScenes.ToArray());
		}
	}
}
#endif