using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;


/// <summary>
/// Helper Functions for UnityEditor.UI
/// </summary>
public static class MyUI
{
	/// <summary>
	/// Switch CanvasGroup visibility and input state
	/// </summary>
	public static void SetCanvasState(CanvasGroup canvas, bool setOn)
	{
		canvas.alpha = setOn ? 1 : 0;
		canvas.interactable = setOn;
		canvas.blocksRaycasts = setOn;
	}


	/// <summary>
	/// Switch canvas visibility and interactivity state
	/// </summary>
	public static void SetState(this CanvasGroup _canvas, bool isOn)
	{
		SetCanvasState(_canvas, isOn);
	}


	public static void OnEvent(this EventTrigger trigger, EventTriggerType eventType, System.Action<BaseEventData> callback)
	{
		EventTrigger.Entry entry = new EventTrigger.Entry();
		entry.eventID = eventType;
		entry.callback = new EventTrigger.TriggerEvent();
		entry.callback.AddListener(new UnityAction<BaseEventData>(callback));
		trigger.triggers.Add(entry);
	}

	#region Fade Image

	public static void FadeImage(MonoBehaviour initiator, Image image, bool fadeIn, float speed)
	{
		initiator.StartCoroutine(FadeImage(image, fadeIn, speed));
	}


	public static IEnumerator FadeImage(Image image, bool fadeIn, float speed)
	{
		Color from = image.color;
		Color to = from;

		from.a = (fadeIn) ? 0 : 1;
		to.a = (fadeIn) ? 1 : 0;

		float elapsed = 0;

		while (elapsed <= speed)
		{
			elapsed += Time.deltaTime;

			image.color = Color.Lerp(from, to, elapsed / speed);

			yield return null;
		}

		image.color = to;
	}

	#endregion
}