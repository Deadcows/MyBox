#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

namespace MyBox.Internal
{
	public class AnimationCreator
	{
		/// <summary>
		/// Create .controller asset at path and assign with targetObject.Animator
		/// </summary>
		/// <param name="targetObject">Object to add Animation Controller</param>
		/// <param name="path">Path to save controller</param>
		/// <param name="clips">Create .anim assets with given names and assign to .controller.
		/// Names with + on end will generate as looping clips</param>
		public static void CreateAnimationControllerAsset(GameObject targetObject, string path, params string[] clips)
		{
			var animator = targetObject.GetComponent<Animator>();
			if (animator == null) animator = targetObject.AddComponent<Animator>();
			if (animator.runtimeAnimatorController != null)
			{
				Debug.LogWarning("Target already contains Animator with Controller");
				return;
			}

			var controllerPath = AssetDatabase.GenerateUniqueAssetPath(path + "Animation.controller");
			var controller = AnimatorController.CreateAnimatorControllerAtPath(controllerPath);

			if (clips != null)
			{
				for (var i = 0; i < clips.Length; i++)
				{
					var clipName = clips[i];
					var clip = new AnimationClip();

					if (clipName.EndsWith("+"))
					{
						var clipSO = new SerializedObject(clip);
						var clipSettingsProp = clipSO.FindProperty("m_AnimationClipSettings");
						var loopProp = clipSettingsProp.FindPropertyRelative("m_LoopTime");

						loopProp.boolValue = true;
						clipSO.ApplyModifiedProperties();

						clipName = clipName.TrimEnd('+');
					}

					var clipPath = AssetDatabase.GenerateUniqueAssetPath(path + clipName + ".anim");
					AssetDatabase.CreateAsset(clip, clipPath);

					var motion = controller.AddMotion(clip);
					motion.name = clipName;

					EditorUtility.SetDirty(clip);
				}
			}

			animator.runtimeAnimatorController = controller;

			EditorUtility.SetDirty(controller);
			EditorUtility.SetDirty(targetObject);
		}
	}
}
#endif