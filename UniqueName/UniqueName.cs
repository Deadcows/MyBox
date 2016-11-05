using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Ensures that name of GameObject is unique
/// </summary>
[DisallowMultipleComponent]
public class UniqueName : MonoBehaviour
{
#if UNITY_EDITOR
	private static readonly List<string> _registredNames = new List<string>();

	private void Start()
	{
		if (_registredNames.Contains(name))
			Debug.LogError("Name " + name + " is not Unique!", this);

		_registredNames.Add(name);
	}

	private void OnLevelWasLoaded(int scene)
	{
		if (_registredNames != null)
			_registredNames.Clear();
	}
#endif
}
