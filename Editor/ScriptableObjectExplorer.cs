using UnityEngine;
using UnityEditor;

public class ScriptableObjectExplorer : EditorWindow
{
	[MenuItem("Tools/SO Explorer")]
	public static void ShowWindow()
	{
		var window = GetWindow(typeof(ScriptableObjectExplorer));
		window.titleContent = new GUIContent("SO Explorer");
	}

	private Object _stashedObject;
	private Vector2 _scroll;
	private string _search;
	public void OnGUI()
	{
		_stashedObject = EditorGUILayout.ObjectField(_stashedObject, typeof(Component), true);
		if (_stashedObject == null) return;
		_search = EditorGUILayout.TextField(_search);

		using (new ScrollViewBlock(ref _scroll))
		{
			var so = new SerializedObject(_stashedObject);
			var props = so.GetIterator();

			while (props.Next(true))
			{
				EditorGUI.indentLevel = props.depth;
				if (string.IsNullOrEmpty(_search) || props.propertyPath.ToUpper().Contains(_search.ToUpper()))
					EditorGUILayout.LabelField(props.propertyType + ": " + props.propertyPath, props.depth == 0 ? EditorStyles.boldLabel : EditorStyles.label);
			}
		}
	}
}
