using UnityEngine;

namespace MyBox
{
	public class ReadOnlyAttribute : PropertyAttribute
	{
		public readonly bool OnlyOnPlaymode;

		/// <param name="onlyOnPlaymode">
		/// 	If you want to keep this field editable during EditMode and read-only during PlayMode
		/// </param>
		public ReadOnlyAttribute(bool onlyOnPlaymode = false)
		{
			OnlyOnPlaymode = onlyOnPlaymode;
		}
	}
}

#if UNITY_EDITOR
namespace MyBox.Internal
{
	using UnityEditor;
	
	[CustomPropertyDrawer(typeof(ReadOnlyAttribute))]
	public class ReadOnlyAttributeDrawer : PropertyDrawer
	{
		public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
			return EditorGUI.GetPropertyHeight(property, label, true);
		}

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			bool editable = !Application.isPlaying && ((ReadOnlyAttribute) attribute).OnlyOnPlaymode;
			if (!editable) GUI.enabled = false;
			EditorGUI.PropertyField(position, property, label, true);
			GUI.enabled = true;
		}
	}
}
#endif