// ---------------------------------------------------------------------------- 
// Author: Matthew Miner
// https://github.com/mminer/selection-history-navigator/blob/master/SelectionHistoryNavigator.cs
// Date:   22/04/2018
// ----------------------------------------------------------------------------

#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace MyBox.Internal
{
	[InitializeOnLoad]
	public static class SelectionHistoryHotkeys
	{
		/// <summary>
		/// Adds Back and Forward items to the Edit > Selection menu to navigate between Hierarchy and Project pane selections.
		/// </summary>
		private static Object _activeSelection;

		private static bool _ignoreNextSelectionChangedEvent;
		private static readonly Stack<Object> NextSelections = new Stack<Object>();
		private static readonly Stack<Object> PreviousSelections = new Stack<Object>();

		static SelectionHistoryHotkeys()
		{
			Selection.selectionChanged += SelectionChangedHandler;
		}

		private static void SelectionChangedHandler()
		{
			if (_ignoreNextSelectionChangedEvent)
			{
				_ignoreNextSelectionChangedEvent = false;
				return;
			}

			if (_activeSelection != null) PreviousSelections.Push(_activeSelection);

			_activeSelection = Selection.activeObject;
			NextSelections.Clear();
		}


		private const string BackMenuLabel = "Tools/MyBox/Back %#[";
		private const string ForwardMenuLabel = "Tools/MyBox/Forward %#]";

		[MenuItem(BackMenuLabel)]
		private static void Back()
		{
			if (_activeSelection != null) NextSelections.Push(_activeSelection);

			Selection.activeObject = PreviousSelections.Pop();
			_activeSelection = Selection.activeObject;
			_ignoreNextSelectionChangedEvent = true;
		}

		[MenuItem(ForwardMenuLabel)]
		private static void Forward()
		{
			if (_activeSelection != null) PreviousSelections.Push(_activeSelection);

			Selection.activeObject = NextSelections.Pop();
			_activeSelection = Selection.activeObject;
			_ignoreNextSelectionChangedEvent = true;
		}

		[MenuItem(BackMenuLabel, true)]
		static bool ValidateBack()
		{
			return PreviousSelections.Count > 0;
		}

		[MenuItem(ForwardMenuLabel, true)]
		static bool ValidateForward()
		{
			return NextSelections.Count > 0;
		}
	}
}
#endif