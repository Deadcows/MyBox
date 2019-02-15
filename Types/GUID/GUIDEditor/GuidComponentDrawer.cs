// ---------------------------------------------------------------------------- 
// Author: Unity Team
// Date:   28/09/2018
// Source: hhttps://github.com/Unity-Technologies/guid-based-reference
// ----------------------------------------------------------------------------

#if UNITY_EDITOR
using UnityEditor;

namespace MyBox.Internal
{
	[CustomEditor(typeof(GuidComponent))]
	public class GuidComponentDrawer : Editor
	{
		private GuidComponent guidComp;

		public override void OnInspectorGUI()
		{
			if (guidComp == null) guidComp = (GuidComponent) target;

			EditorGUILayout.LabelField("Guid:", guidComp.GuidString);
		}
	}
}
#endif