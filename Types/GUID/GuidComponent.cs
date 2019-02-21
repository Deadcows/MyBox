// ---------------------------------------------------------------------------- 
// Author: Unity Team
// Date:   28/09/2018
// Source: hhttps://github.com/Unity-Technologies/guid-based-reference
// ----------------------------------------------------------------------------

using System;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace MyBox
{
#pragma warning disable 618
	// This component gives a GameObject a stable, non-replicatable Globally Unique IDentifier.
	// It can be used to reference a specific instance of an object no matter where it is.
	// This can also be used for other systems, such as Save/Load game
	[ExecuteInEditMode, DisallowMultipleComponent]
	public class GuidComponent : MonoBehaviour, ISerializationCallbackReceiver
	{
		public Guid Guid
		{
			get
			{
				if (guid == Guid.Empty && serializedGuid != null && serializedGuid.Length == 16)
					guid = new Guid(serializedGuid);

				return guid;
			}
		}

		public string GuidString
		{
			get { return Guid.ToString(); }
		}


		private Guid guid = Guid.Empty;

		// Unity's serialization system doesn't know about System.Guid, so we convert to a byte array
		// Fun fact, we tried using strings at first, but that allocated memory and was twice as slow
		[SerializeField] private byte[] serializedGuid;

		// When deserializaing or creating this component, we want to either restore our serialized GUID
		// or create a new one.
		private void CreateGuid()
		{
			// if our serialized data is invalid, then we are a new object and need a new GUID
			if (serializedGuid == null || serializedGuid.Length != 16)
			{
				guid = Guid.NewGuid();
				serializedGuid = guid.ToByteArray();

#if UNITY_EDITOR
				// If we are creating a new GUID for a prefab instance of a prefab, but we have somehow lost our prefab connection
				// force a save of the modified prefab instance properties
				PrefabType prefabType = PrefabUtility.GetPrefabType(this);
				if (prefabType == PrefabType.PrefabInstance)
				{
					PrefabUtility.RecordPrefabInstancePropertyModifications(this);
				}
#endif
			}
			else if (guid == Guid.Empty)
			{
				// otherwise, we should set our system guid to our serialized guid
				guid = new Guid(serializedGuid);
			}

			// register with the GUID Manager so that other components can access this
			if (guid != Guid.Empty)
			{
				if (!GuidManager.Add(this))
				{
					// if registration fails, we probably have a duplicate or invalid GUID, get us a new one.
					serializedGuid = null;
					guid = Guid.Empty;
					CreateGuid();
				}
			}
		}

		// We cannot allow a GUID to be saved into a prefab, and we need to convert to byte[]
		public void OnBeforeSerialize()
		{
#if UNITY_EDITOR
			// This lets us detect if we are a prefab instance or a prefab asset.
			// A prefab asset cannot contain a GUID since it would then be duplicated when instanced.
			PrefabType prefabType = PrefabUtility.GetPrefabType(this);
			if (prefabType == PrefabType.Prefab || prefabType == PrefabType.ModelPrefab)
			{
				serializedGuid = new byte[0];
				guid = Guid.Empty;
			}
			else
#endif
			{
				if (guid != Guid.Empty)
				{
					serializedGuid = guid.ToByteArray();
				}
			}
		}

		// On load, we can go head a restore our system guid for later use
		public void OnAfterDeserialize()
		{
			if (serializedGuid != null && serializedGuid.Length == 16)
			{
				guid = new Guid(serializedGuid);
			}
		}


		private void OnValidate()
		{
#if UNITY_EDITOR
			// similar to on Serialize, but gets called on Copying a Component or Applying a Prefab
			// at a time that lets us detect what we are
			PrefabType prefabType = PrefabUtility.GetPrefabType(this);
			if (prefabType == PrefabType.Prefab || prefabType == PrefabType.ModelPrefab)
			{
				serializedGuid = null;
				guid = Guid.Empty;
			}
			else
#endif
			{
				CreateGuid();
			}
		}


		private void Awake()
		{
			CreateGuid();
		}

		public void OnDestroy()
		{
			GuidManager.Remove(guid);
		}
	}
#pragma warning restore 618
}