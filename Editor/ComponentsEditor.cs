using UnityEngine;
using UnityEditor;

public class ComponentsEditor : EditorWindow
{
	[MenuItem("Tools/Components Viewer")]
	public static void ShowWindow()
	{
		var window = GetWindow(typeof(ComponentsEditor));
		window.titleContent = new GUIContent("Components Viewer");
	}

	private GameObject _stashedObject;
	private Vector2 _scroll;
	public void OnGUI()
	{
		_stashedObject = EditorGUILayout.ObjectField(_stashedObject, typeof(GameObject), true) as GameObject;
		if (_stashedObject == null) return;

		var components = _stashedObject.GetComponents<Component>();
		using (new ScrollViewBlock(ref _scroll))
		{
			for (var i = 0; i < components.Length; i++)
			{
				var component = components[i];

				if (GUILayout.Button("- " + component.GetType().Name))
				{
					Destroy(component);
					EditorUtility.SetDirty(_stashedObject);
				}
			}
		}
	}

}