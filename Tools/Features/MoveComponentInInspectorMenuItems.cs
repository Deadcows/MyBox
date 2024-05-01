#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace MyBox.Internal
{
	public static class MoveComponentInInspectorMenuItems
	{
		[MenuItem("CONTEXT/Component/Move To Top", priority = 501)]
		public static void MoveComponentToTop(MenuCommand item) =>
			MyComponentUtility.MoveComponentInspectorToTop((Component)item.context);

		[MenuItem("CONTEXT/Component/Move To Bottom", priority = 502)]
		public static void MoveComponentToBottom(MenuCommand item) =>
			MyComponentUtility.MoveComponentInspectorToBottom((Component)item.context);
	}
}
#endif