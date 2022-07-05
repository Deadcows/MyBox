#if UNITY_EDITOR
using UnityEditor;

namespace MyBox.Internal
{
	[CustomEditor(typeof(GuidComponent))]
	public class GuidComponentDrawer : Editor
	{
		private GuidComponent _guid;

		public override void OnInspectorGUI()
		{
			if (_guid == null) _guid = (GuidComponent) target;
			
			using (new EditorGUI.DisabledScope(true)) EditorGUILayout.TextField("Guid:", _guid.GetGuid().ToString());
		}
	}
}
#endif