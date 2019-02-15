#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

namespace MyBox.EditorTools
{
	// Interesting approach for nested editors from Adventure Game Unity tutorial
	// www.unity3d.com/ru/learn/tutorials/projects/adventure-game-tutorial/conditions?playlist=44381
	public abstract class EditorWithSubEditors<TEditor, TTarget> : Editor where TEditor : Editor where TTarget : Object
	{
		protected TEditor[] SubEditors;


		protected void CheckAndCreateSubEditors(TTarget[] subEditorTargets)
		{
			if (SubEditors != null && SubEditors.Length == subEditorTargets.Length)
				return;

			CleanupEditors();

			SubEditors = new TEditor[subEditorTargets.Length];

			for (int i = 0; i < SubEditors.Length; i++)
			{
				SubEditors[i] = CreateEditor(subEditorTargets[i]) as TEditor;
				SubEditorSetup(SubEditors[i]);
			}
		}


		protected void CleanupEditors()
		{
			if (SubEditors == null)
				return;

			for (int i = 0; i < SubEditors.Length; i++)
			{
				DestroyImmediate(SubEditors[i]);
			}

			SubEditors = null;
		}


		protected abstract void SubEditorSetup(TEditor editor);
	}
}
#endif