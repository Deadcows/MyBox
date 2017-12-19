using System;
using UnityEngine;

public class ConditionallyEnabledGUIBlock : IDisposable
{
	private readonly bool _originalState;

	public ConditionallyEnabledGUIBlock(bool condition)
	{
		_originalState = GUI.enabled;
		GUI.enabled = condition;
	}

	public void Dispose()
	{
		GUI.enabled = _originalState;
	}
}
