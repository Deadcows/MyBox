#if UNITY_EDITOR
namespace MyBox.EditorTools
{
	using UnityEditor;
	using UnityEngine;
	using UnityEngine.Audio;

	public static class MyEditorAudio
	{
		//TODO: Is it ok to always force _objectToPlayAlong?
		//TODO: It is possible to play several clips at a time, should I do it?

#if UNITY_2023_2_OR_NEWER
		public static void Play(AudioResource clip, float volume = 1)
#else
		public static void Play(AudioClip clip, float volume = 1)
#endif
		{
			if (_previewSource == null) _previewSource = CreatePreviewAudioSource();

#if UNITY_2023_2_OR_NEWER
			_previewSource.resource = clip;
#else
			_previewSource.clip = clip;
#endif
			
			_previewSource.volume = volume;
			_previewSource.Play();

			_objectToPlayAlong = Selection.activeGameObject;
			_editorAudioPreviousState = EditorUtility.audioMasterMute;
			_wasPlaying = true;
			EditorUtility.audioMasterMute = false;
			EditorApplication.update += CheckSelectionWhilePlaying;

			// To force Stop it GameObject selection changed
			void CheckSelectionWhilePlaying()
			{
				if (_previewSource != null)
				{
					var isPlaying = _previewSource.isPlaying;
					bool playingButUnfocused = isPlaying && Selection.activeGameObject != _objectToPlayAlong;
					bool finishedPlay = _wasPlaying && !isPlaying;

					if (playingButUnfocused || finishedPlay) Stop();
					else return;
				}

				EditorApplication.update -= CheckSelectionWhilePlaying;
			}

			AudioSource CreatePreviewAudioSource()
			{
				var gameObject = new GameObject
				{
					name = "PreviewAudioSource140980571",
					hideFlags = HideFlags.HideInHierarchy | HideFlags.DontSaveInEditor | HideFlags.DontSaveInBuild
				};

				var previewSource = gameObject.AddComponent<AudioSource>();
				previewSource.playOnAwake = false;
				return previewSource;
			}
		}

		public static void SetVolume(float volume)
		{
			if (_previewSource != null) _previewSource.volume = volume;
		}

		public static bool IsPlaying() => _previewSource != null && _previewSource.isPlaying;

		public static void Stop()
		{
			if (!_wasPlaying) return;

			_previewSource.Stop();
			EditorUtility.audioMasterMute = _editorAudioPreviousState;
			_wasPlaying = false;
		}

		private static bool _editorAudioPreviousState;
		private static bool _wasPlaying;
		private static AudioSource _previewSource;
		private static GameObject _objectToPlayAlong;


		// It is possible to directly play audio through Editor, but this method not allows to change volume
		// Leaving it here for the reference
		// public static void PlayClip(AudioClip clip)
		// {
		// 	if (IsClipPlaying()) StopClip();
		// 		
		// 	Assembly unityEditorAssembly = typeof(AudioImporter).Assembly;
		// 	Type audioUtilClass = unityEditorAssembly.GetType("UnityEditor.AudioUtil");
		// 	MethodInfo method = audioUtilClass.GetMethod("PlayPreviewClip", BindingFlags.Static | BindingFlags.Public);
		// 	method.Invoke(null, new object[] { clip, 0, false });
		// }
		// 	
		// public static void StopClip()
		// {
		// 	Assembly unityEditorAssembly = typeof(AudioImporter).Assembly;
		// 		
		// 	Type audioUtilClass = unityEditorAssembly.GetType("UnityEditor.AudioUtil");
		// 	MethodInfo method = audioUtilClass.GetMethod("StopAllPreviewClips", BindingFlags.Static | BindingFlags.Public);
		// 	method.Invoke(null, new object[] { });
		// }
		// 	
		// public static bool IsClipPlaying()
		// {
		// 	Assembly unityEditorAssembly = typeof(AudioImporter).Assembly;
		// 		
		// 	Type audioUtilClass = unityEditorAssembly.GetType("UnityEditor.AudioUtil");
		// 	MethodInfo method = audioUtilClass.GetMethod("IsPreviewClipPlaying", BindingFlags.Static | BindingFlags.Public);
		// 	var playing = method.Invoke(null, new object[] { });
		// 	return (bool)playing;
		// }
	}
}
#endif