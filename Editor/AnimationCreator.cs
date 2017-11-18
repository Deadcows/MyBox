using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;
using UnityQuery;

public class AnimationCreator : EditorWindow
{

	[MenuItem("Tools/Embedded Animation Creator", false, 50)]
	// ReSharper disable once UnusedMember.Local
	private static void CreateWindow()
	{
		_instance = GetWindow<AnimationCreator>();
		_instance.Show();
	}

	private static AnimationCreator _instance;
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

		using (new EditorGUILayout.HorizontalScope())
		{
			_newClipName = EditorGUILayout.TextField("New Clip Name", _newClipName);
			if (!_newClipName.IsNullOrEmpty())
			{
				if (GUILayout.Button("+", MyGUI.ResizableToolbarButtonStyle, GUILayout.Width(20), GUILayout.Height(20)))
				{
					AnimationClip newClip = new AnimationClip();
					newClip.name = _newClipName;
					_newClipName = string.Empty;

					AssetDatabase.AddObjectToAsset(newClip, _currentAnimator);
					// Reimport the asset after adding an object.
					// Otherwise the change only shows up when saving the project

					_currentAnimator.AddMotion(newClip);
					AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(newClip));

				}
			}
			
		}
			
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
		// This will only get called 10 times per second.
		Repaint();
	}

	public void OnSelectionChange()
	{
		_currentAnimator = Selection.activeObject as AnimatorController;
	}
	

}