using System.Collections.Generic;
using UnityEngine;
using System;

namespace MyBox
{
	/// <summary>
	/// Class to handle registering and accessing objects by GUID
	/// </summary>
	public class GuidManager
	{
		// for each GUID we need to know the Game Object it references
		// and an event to store all the callbacks that need to know when it is destroyed
		private struct GuidInfo
		{
			public GameObject GameObject;

			public event Action<GameObject> OnAdd;
			public event Action OnRemove;

			public GuidInfo(GuidComponent comp)
			{
				GameObject = comp.gameObject;
				OnRemove = null;
				OnAdd = null;
			}

			public void HandleAddCallback() => OnAdd?.Invoke(GameObject);

			public void HandleRemoveCallback() => OnRemove?.Invoke();
		}

		
		public static bool Add(GuidComponent guidComponent) => Instance.InternalAdd(guidComponent);

		public static void Remove(Guid guid) => Instance.InternalRemove(guid);

		public static GameObject ResolveGuid(Guid guid, Action<GameObject> onAddCallback, Action onRemoveCallback)
		{
			return Instance.ResolveGuidInternal(guid, onAddCallback, onRemoveCallback);
		}

		public static GameObject ResolveGuid(Guid guid, Action onDestroyCallback)
		{
			return Instance.ResolveGuidInternal(guid, null, onDestroyCallback);
		}

		public static GameObject ResolveGuid(Guid guid)
		{
			return Instance.ResolveGuidInternal(guid, null, null);
		}
		

		private static GuidManager Instance => _instance ?? (_instance = new GuidManager());
		private static GuidManager _instance;
		
		private readonly Dictionary<Guid, GuidInfo> _guidToObjectMap = new Dictionary<Guid, GuidInfo>();

		
		private bool InternalAdd(GuidComponent guidComponent)
		{
			Guid guid = guidComponent.GetGuid();
			GuidInfo info = new GuidInfo(guidComponent);

			if (!_guidToObjectMap.ContainsKey(guid))
			{
				_guidToObjectMap.Add(guid, info);
				return true;
			}

			GuidInfo existingInfo = _guidToObjectMap[guid];
			if (existingInfo.GameObject != null && existingInfo.GameObject != guidComponent.gameObject)
			{
				// normally, a duplicate GUID is a big problem, means you won't necessarily be referencing what you expect
				if (Application.isPlaying)
				{
					Debug.AssertFormat(false, guidComponent,
						"Guid Collision Detected between {0} and {1}.\nAssigning new Guid. Consider tracking runtime instances using a direct reference or other method.",
						(_guidToObjectMap[guid].GameObject != null ? _guidToObjectMap[guid].GameObject.name : "NULL"), (guidComponent != null ? guidComponent.name : "NULL"));
				}
				else
				{
					// however, at editor time, copying an object with a GUID will duplicate the GUID resulting in a collision and repair.
					// we warn about this just for pedantry reasons, and so you can detect if you are unexpectedly copying these components
					Debug.LogWarningFormat(guidComponent, "Guid Collision Detected while creating {0}.\nAssigning new Guid.",
						(guidComponent != null ? guidComponent.name : "NULL"));
				}

				return false;
			}

			// if we already tried to find this GUID, but haven't set the game object to anything specific, copy any OnAdd callbacks then call them
			existingInfo.GameObject = info.GameObject;
			existingInfo.HandleAddCallback();
			_guidToObjectMap[guid] = existingInfo;
			return true;
		}

		private void InternalRemove(Guid guid)
		{
			if (_guidToObjectMap.TryGetValue(guid, out var info))
			{
				// trigger all the destroy delegates that have registered
				info.HandleRemoveCallback();
			}

			_guidToObjectMap.Remove(guid);
		}

		// nice easy api to find a GUID, and if it works, register an on destroy callback
		// this should be used to register functions to cleanup any data you cache on finding
		// your target. Otherwise, you might keep components in memory by referencing them
		private GameObject ResolveGuidInternal(Guid guid, Action<GameObject> onAddCallback, Action onRemoveCallback)
		{
			if (_guidToObjectMap.TryGetValue(guid, out var info))
			{
				if (onAddCallback != null)
				{
					info.OnAdd += onAddCallback;
				}

				if (onRemoveCallback != null)
				{
					info.OnRemove += onRemoveCallback;
				}

				_guidToObjectMap[guid] = info;
				return info.GameObject;
			}

			if (onAddCallback != null) info.OnAdd += onAddCallback;

			if (onRemoveCallback != null) info.OnRemove += onRemoveCallback;

			_guidToObjectMap.Add(guid, info);

			return null;
		}
	}
}