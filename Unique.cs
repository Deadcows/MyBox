using UnityEngine;
using System;

[ExecuteInEditMode]
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
		if (_instanceID == GetInstanceID()) return;
		
		if (_instanceID == 0) _instanceID = GetInstanceID();
		else
		{
			_instanceID = GetInstanceID();
			if (_instanceID < 0) GUID = string.Empty;
		}
	}
	
	private void Update()
	{
		if (Application.isPlaying) return;
		if (!GUID.IsNullOrEmpty()) return;

		GUID = !_cachedGuid.IsNullOrEmpty() ? _cachedGuid : Guid.NewGuid().ToString();

		UnityEditor.EditorUtility.SetDirty(this);
		UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(gameObject.scene);
		_cachedGuid = GUID;
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
