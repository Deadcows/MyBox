using UnityEngine;
using DeadcowBox;
using UnityEditor;

[CustomPropertyDrawer(typeof(ScriptableObjectInspectorAttribute))]
public class ScriptableObjectInspectorDrawer : PropertyDrawer
{

	private static bool _folded;

	// Draw the property inside the given rect
	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
	{
		var e = Editor.CreateEditor(property.objectReferenceValue);
		position.height = EditorGUI.GetPropertyHeight(property);

		if (e != null)
		{
			var w = position.width;
			position.width = 20;
			if (GUI.Button(position, _folded ? "+" : "-", EditorStyles.toolbarButton))
				_folded = !_folded;
			position.width = w - 25;
			position.x += 25;
			EditorGUIUtility.labelWidth -= 25;
			EditorGUI.PropertyField(position, property);
			EditorGUIUtility.labelWidth += 25;
			position.x -= 25;
			position.width = w;
		}
		else EditorGUI.PropertyField(position, property);

		position.y += EditorGUI.GetPropertyHeight(property);

		if (_folded) return;
		
		if (e != null)
		{
			position.x += 20;
			position.width -= 40;

			position = MyGUI.DrawLine(MyGUI.Gray, position);

			var so = e.serializedObject;
			so.Update();

			var prop = so.GetIterator();
			prop.NextVisible(true);
            while (prop.NextVisible(true))
			{
				position.height = EditorGUI.GetPropertyHeight(prop);
				EditorGUI.PropertyField(position, prop);
				position.y += EditorGUI.GetPropertyHeight(prop) + 4;
			}
			if (GUI.changed)
				so.ApplyModifiedProperties();

			position.y -= 4;
			MyGUI.DrawLine(MyGUI.Gray, position);
		}
	}

	public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
	{
		float height = base.GetPropertyHeight(property, label);
		if (_folded || property.objectReferenceValue == null) return height;

		height += EditorGUI.GetPropertyHeight(property, label);
		var e = Editor.CreateEditor(property.objectReferenceValue);
		if (e != null)
		{
			var so = e.serializedObject;
			var prop = so.GetIterator();
			prop.NextVisible(true);
			while (prop.NextVisible(true)) height += EditorGUI.GetPropertyHeight(prop);
		}
		return height + 40;
	}
}
