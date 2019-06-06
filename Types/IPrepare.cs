#if UNITY_EDITOR
using System;
using UnityEditor;
#endif

namespace MyBox
{
	public interface IPrepare
	{
		void Prepare();
	}
}

#if UNITY_EDITOR

namespace MyBox.EditorTools
{
	[InitializeOnLoad]
	public class PrepareHandler
	{
		public static Action OnPrepare;

		static PrepareHandler()
		{
			MyEditorEvents.BeforePlaymode += Prepare;
			MyEditorEvents.BeforeBuild += Prepare;
			MyEditorEvents.OnSave += Prepare;
		}

		public static void Prepare()
		{
			string prepareUndoKey = "PrepareProcessUndo";

			var toPrepare = MyExtensions.FindObjectsOfInterfaceAsComponents<IPrepare>();
			foreach (var prepare in toPrepare)
			{
				Undo.RecordObject(prepare.Component, prepareUndoKey);
				prepare.Interface.Prepare();
				if (prepare.Component != null) EditorUtility.SetDirty(prepare.Component);
			}

			if (OnPrepare != null) OnPrepare();
		}
	}
}

#endif