using System.IO;
using UnityEditor;
using UnityEngine;

public class MyToolbox : EditorWindow
{

	private static MyToolbox Instance
	{
		set
		{
			if (_instance == value) return;
			_instance = value;
			if (value == null) return;
			LoadResources(value);
		}
		get { return _instance; }
	}
	private static MyToolbox _instance;

	private static void LoadResources(ScriptableObject relativeObject)
	{
		var asset = MonoScript.FromScriptableObject(relativeObject);
		var assetFolder = Path.GetDirectoryName(AssetDatabase.GetAssetPath(asset));
		if (assetFolder == null) return;

		_icoFold = AssetDatabase.LoadAssetAtPath<Texture2D>(Path.Combine(assetFolder, "_icoFold.png"));
	}

	[MenuItem("Tools/MyToolbox", false, 50)]
	private static void CreateWindow()
	{
		Instance = GetWindow<MyToolbox>();
		Instance.Show();
	}


	private static Texture2D _icoFold;


	#region Create/Enable

	void OnEnable()
	{
		Instance = this;
		SceneView.onSceneGUIDelegate -= OnSceneGUI;
		SceneView.onSceneGUIDelegate += OnSceneGUI;

		//EditorApplication.hierarchyWindowChanged += DetectSceneLoaded;
	}
	
	void OnDisable()
	{
		Instance = null;
		SceneView.onSceneGUIDelegate -= OnSceneGUI;

		//EditorApplication.hierarchyWindowChanged -= DetectSceneLoaded;
	}

	//private static string _loadedScene = string.Empty;
	//private void DetectSceneLoaded()
	//{
	//	if (Application.isPlaying) return;
	//	if (SceneManager.GetActiveScene().name != _loadedScene)
	//	{
	//		_loadedScene = SceneManager.GetActiveScene().name;

	//		//OnReorder();
	//	}
	//}

	#endregion


	private void OnGUI()
	{
		EditorGUILayout.Space();
		using (new EditorGUILayout.HorizontalScope())
		{
			EditorGUILayout.Space();
			FoldHierarchyButton();
			EditorGUILayout.Space();
			WrapSelected();
			EditorGUILayout.Space();
			ResetPosition();
			EditorGUILayout.Space();

			if (Selection.activeGameObject != null)
			{
				if (GUILayout.Button("Show Hidden", EditorStyles.toolbarButton))
				{
					var components = Selection.activeGameObject.GetComponents<Component>();
					foreach (var component in components)
					{
						component.hideFlags = HideFlags.None;
					}
				}
				
			}
		}
	}

	#region Fold Hierarchy

	private static void FoldHierarchyButton()
	{
		if (!GUILayout.Button(_icoFold, MyGUI.ResizableToolbarButtonStyle, GUILayout.Height(32), GUILayout.Width(32))) return;

		MyEditor.FoldSceneHierarchy();
	}

	#endregion

	#region Selection to Parent

	private static void WrapSelected()
	{
		if (!GUILayout.Button("Wrap", MyGUI.ResizableToolbarButtonStyle, GUILayout.Height(32), GUILayout.Width(40))) return;

		var selected = Selection.gameObjects;
		if (selected.IsNullOrEmpty()) return;

		var parentObject = new GameObject("Temp Parent");
		Undo.RegisterCreatedObjectUndo(parentObject, "ParentingTool");
		foreach (var gameObject in Selection.gameObjects)
		{
			Undo.SetTransformParent(gameObject.transform, parentObject.transform, "ParentingTool");
		}
		MyEditor.InitiateObjectRename(parentObject);
	}

	#endregion

	#region Reset Position

	private static void ResetPosition()
	{
		if (!GUILayout.Button("Reset", MyGUI.ResizableToolbarButtonStyle, GUILayout.Height(32), GUILayout.Width(40))) return;

		var selection = Selection.activeGameObject;
		if (selection == null) return;
		Undo.RecordObject(selection.transform, "ResetPosition");
		selection.transform.position = Vector3.zero;
		EditorUtility.SetDirty(selection.transform);
		Undo.FlushUndoRecordObjects();
	}

	#endregion


	private void OnSceneGUI(SceneView sceneView)
	{
		
	}

}
