using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public static class MyCoroutine
{

	#region Parallel group management

	/// <summary>
	/// Add coroutine to a parallel group and start it
	/// </summary>
	public static void AsParallelGroup(this IEnumerator coroutine, MonoBehaviour parent, string groupName)
	{
		if (!Runners.ContainsKey(groupName))
			Runners.Add(groupName, 0);

		Runners[groupName]++;
		parent.StartCoroutine(DoParallel(coroutine, parent, groupName));
	}

	/// <summary>
	/// Get amount of coroutines, that is processing (not finished) in group
	/// </summary>
	public static int GroupProcessing(string groupName)
	{
		return Runners.ContainsKey(groupName) ? Runners[groupName] : 0;
	}

	/// <summary>
	/// Wait till all coroutines in group is finisged
	/// </summary>
	public static IEnumerator WaitGroupProcessed(string groupName)
	{
		while (GroupProcessing(groupName) > 0)
			yield return null;
	}


	private static readonly Dictionary<string, int> Runners = new Dictionary<string, int>();

	private static IEnumerator DoParallel(IEnumerator coroutine, MonoBehaviour parent, string groupName)
	{
		yield return parent.StartCoroutine(coroutine);

		Runners[groupName]--;
	}

	#endregion

	
	/// <summary>
	/// Invoke Action on Delay
	/// </summary>
	public static IEnumerator DelayedAction(float waitSeconds, Action action, bool unscaled = false)
	{
		if (unscaled) yield return new WaitForUnscaledSeconds(waitSeconds);
		else yield return new WaitForSeconds(waitSeconds);

		if (action != null) action.Invoke();
	}

	/// <summary>
	/// Invoke Action on Delay
	/// </summary>
	public static Coroutine DelayedAction(this MonoBehaviour invoker, float waitSeconds, Action action, bool unscaled = false)
	{
		return invoker.StartCoroutine(DelayedAction(waitSeconds, action, unscaled));
	}

	/// <summary>
	/// Invoke Action next frame
	/// </summary>
	public static IEnumerator DelayedAction(Action action)
	{
		yield return null;

		if (action != null) action.Invoke();
	}

	/// <summary>
	/// Invoke Action next frame
	/// </summary>
	public static Coroutine DelayedAction(this MonoBehaviour invoker, Action action)
	{
		return invoker.StartCoroutine(DelayedAction(action));
	}


	/// <summary>
	/// Set GO as selected next frame (EventSystem.current.SetSelectedGameObject)
	/// </summary>
	public static IEnumerator DelayedUiSelection(GameObject objectToSelect)
	{
		yield return null;
		EventSystem.current.SetSelectedGameObject(null);
		EventSystem.current.SetSelectedGameObject(objectToSelect);
	}
	
	/// <summary>
	/// Set GO as selected next frame (EventSystem.current.SetSelectedGameObject)
	/// </summary>
	public static Coroutine DelayedUiSelection(this MonoBehaviour invoker, GameObject objectToSelect)
	{
		return invoker.StartCoroutine(DelayedUiSelection(objectToSelect));
	}
}