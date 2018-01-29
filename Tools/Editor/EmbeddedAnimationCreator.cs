using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;
using UnityQuery;

public class EmbeddedAnimationCreator : EditorWindow
{

	[MenuItem("Tools/MyBox/Embedded Animation Creator", false, 50)]
	private static void CreateWindow()
	{
		_instance = GetWindow<EmbeddedAnimationCreator>();
		_instance.Show();
	}

	private static EmbeddedAnimationCreator _instance;
	private AnimatorController _currentAnimator;

	private string _newClipName = string.Empty;

	public void OnGUI()
	{
		EditorGUILayout.Space();

		if (_currentAnimator == null)
		{
			EditorGUILayout.HelpBox("Select Animation Controller", MessageType.Warning);
			return;
		}

		DrawAnimationClips();

		EditorGUILayout.BeginHorizontal();
		_newClipName = EditorGUILayout.TextField("New Clip Name", _newClipName);
		if (_newClipName.IsNullOrEmpty()) return;
			
		if (GUILayout.Button("+", MyGUI.ResizableToolbarButtonStyle, GUILayout.Width(20), GUILayout.Height(20)))
		{
			InsertNewClip();
		}
		EditorGUILayout.EndHorizontal();
	}

	private void InsertNewClip()
	{
		AnimationClip newClip = new AnimationClip();
		newClip.name = _newClipName;
		_newClipName = string.Empty;

		AssetDatabase.AddObjectToAsset(newClip, _currentAnimator);
		_currentAnimator.AddMotion(newClip);

		// Reimport the asset after adding an object.
		// Otherwise the change only shows up when saving the project
		AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(newClip));
	}

	private void DrawAnimationClips()
	{
		if (_currentAnimator.animationClips.IsNullOrEmpty()) return;
		
		for (var i = 0; i < _currentAnimator.animationClips.Length; i++)
		{
			EditorGUILayout.LabelField(i + " Clip: " + _currentAnimator.animationClips[i].name);
		}
	}

	public void OnInspectorUpdate()
	{
		Repaint();
	}

	public void OnSelectionChange()
	{
		_currentAnimator = Selection.activeObject as AnimatorController;
	}
}