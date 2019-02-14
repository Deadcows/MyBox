using UnityEngine;
using System;

namespace MyBox
{
	[ExecuteInEditMode, DisallowMultipleComponent]
	public class Unique : MonoBehaviour, ISerializationCallbackReceiver
	{
		[ReadOnly] public string GUID;


#if UNITY_EDITOR

		/// <summary>
		/// CachedGUID is used to prevent ID loss on "Revert to prefab"
		/// </summary>
		private string _cachedGuid;

		/// <summary>
		/// InstanceID used to detect object duplication
		/// </summary>
		[SerializeField, HideInInspector] private int _instanceID;


		private void Awake()
		{
			if (Application.isPlaying) return;
			var actualId = GetInstanceID();

			// if you'll start editor or reload scene, framecount will be zero. 
			// We may cache newly assigned ID here
			// TODO: This is not working if you'll unload and load scene in multiscene mode! 
				// Need a way to detect if scene was loaded right before awake call...?
				// Separate script to track loaded scenes and in which frame which scene was loaded?
				// But SceneManager.sceneLoaded will be called after Awake. Put this logic to Start?
			if (Time.frameCount == 0) _instanceID = actualId;
			if (_instanceID == actualId) return;

			// object duplication = cached id not mach current id and cached != 0
			if (_instanceID != 0) GUID = string.Empty;
			_instanceID = actualId;
		}

		private void Update()
		{
			if (Application.isPlaying) return;
			if (!GUID.IsNullOrEmpty()) return;

			GUID = !_cachedGuid.IsNullOrEmpty() ? _cachedGuid : Guid.NewGuid().ToString();

			_cachedGuid = GUID;
			UnityEditor.EditorUtility.SetDirty(this);
			UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(gameObject.scene);
		}

#endif

		#region ISerializationCallbackReceiver 

		public void OnBeforeSerialize()
		{
#if UNITY_EDITOR
			if (!gameObject.IsPrefabInstance()) return;

			GUID = string.Empty;
			_instanceID = 0;
#endif
		}

		public void OnAfterDeserialize()
		{
		}

		#endregion
	}
}