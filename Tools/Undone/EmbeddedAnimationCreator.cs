#if UNITY_EDITOR
using MyBox.EditorTools;
using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;

namespace MyBox.Internal
{
	public class EmbeddedAnimationCreator : EditorWindow
	{
		//[MenuItem("Tools/MyBox/Embedded Animation Creator", false, 50)]
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
			if (string.IsNullOrEmpty(_newClipName)) return;

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
			var clips = _currentAnimator.animationClips;
			if (clips == null || clips.Length == 0) return;

			for (var i = 0; i < clips.Length; i++)
			{
				EditorGUILayout.LabelField(i + " Clip: " + clips[i].name);
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
}
#endif