using UnityEngine;
using System;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace MyBox
{
	/// <summary>
	/// This call is the type used by any other code to hold a reference to an object by GUID
	/// If the target object is loaded, it will be returned, otherwise, NULL will be returned
	/// This always works in Game Objects, so calling code will need to use GetComponent<>
	/// or other methods to track down the specific objects need by any given system
	/// Ideally this would be a struct, but we need the ISerializationCallbackReciever
	/// </summary>
	[Serializable]
	public class GuidReference : ISerializationCallbackReceiver
	{
		// cache the referenced Game Object if we find one for performance
		private GameObject cachedReference;
		private bool isCacheSet;

		// store our GUID in a form that Unity can save
		[SerializeField] private byte[] serializedGuid;
		private Guid guid;

#if UNITY_EDITOR
		// decorate with some extra info in Editor so we can inform a user of what that GUID means
		[SerializeField] private string cachedName;
		[SerializeField] private SceneAsset cachedScene;
#endif

		// Set up events to let users register to cleanup their own cached references on destroy or to cache off values
		public event Action<GameObject> OnGuidAdded = delegate(GameObject go) { };
		public event Action OnGuidRemoved = delegate() { };

		// create concrete delegates to avoid boxing. 
		// When called 10,000 times, boxing would allocate ~1MB of GC Memory
		private Action<GameObject> addDelegate;
		private Action removeDelegate;

		// optimized accessor, and ideally the only code you ever call on this class
		public GameObject gameObject
		{
			get
			{
				if (isCacheSet)
				{
					return cachedReference;
				}

				cachedReference = GuidManager.ResolveGuid(guid, addDelegate, removeDelegate);
				isCacheSet = true;
				return cachedReference;
			}

			private set { }
		}

		public GuidReference()
		{
		}

		public GuidReference(GuidComponent target)
		{
			guid = target.GetGuid();
		}

		private void GuidAdded(GameObject go)
		{
			cachedReference = go;
			OnGuidAdded(go);
		}

		private void GuidRemoved()
		{
			cachedReference = null;
			isCacheSet = false;
			OnGuidRemoved();
		}

		//convert system guid to a format unity likes to work with
		public void OnBeforeSerialize()
		{
			serializedGuid = guid.ToByteArray();
		}

		// convert from byte array to system guid and reset state
		public void OnAfterDeserialize()
		{
			cachedReference = null;
			isCacheSet = false;
			if (serializedGuid == null || serializedGuid.Length != 16)
			{
				serializedGuid = new byte[16];
			}

			guid = new System.Guid(serializedGuid);
			addDelegate = GuidAdded;
			removeDelegate = GuidRemoved;
		}
	}
}