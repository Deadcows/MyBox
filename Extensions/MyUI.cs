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



	public static Texture2D WhiteTexture
	{
		get
		{
			if ((UnityEngine.Object)_whiteTexture == (UnityEngine.Object)null)
			{
				_whiteTexture = new Texture2D(1, 1);
				_whiteTexture.SetPixel(0, 0, Color.white);
				_whiteTexture.Apply();
			}
			return _whiteTexture;
		}
	}

	private static Texture2D _whiteTexture;



	#region Fade Canvas

	/// <summary>
	/// Smootly Fades Canvas Group
	/// </summary>
	/// <param name="initiator">Container for the coroutine</param>
	/// <param name="canvas">Canvas to fade</param>
	/// <param name="fadeIn">Fade In or Out</param>
	/// <param name="speed">Time to fade in seconds</param>
	/// <param name="changeStateOnFade">Set interactable and blockingRaycasts on fade end if true</param>
	public static void FadeCanvas(MonoBehaviour initiator, CanvasGroup canvas, bool fadeIn, float speed,
		bool changeStateOnFade = true)
	{
		initiator.StartCoroutine(FadeCanvas(canvas, fadeIn, speed, changeStateOnFade));
	}

	/// <summary>
	/// Smootly Fades Canvas Group
	/// </summary>
	/// <param name="canvas">Canvas to fade</param>
	/// <param name="fadeIn">Fade In or Out</param>
	/// <param name="speed">Time to fade in seconds</param>
	/// <param name="changeStateOnFade">Set interactable and blockingRaycasts on fade end if true</param>
	public static IEnumerator FadeCanvas(CanvasGroup canvas, bool fadeIn, float speed,
		bool changeStateOnFade = true)
	{
		float elapsed = 0;
		float fadeTo = fadeIn ? 1 : 0;

		while (elapsed <= speed) 
		{ 
			elapsed += Time.deltaTime;
			canvas.alpha = fadeTo * (speed / elapsed);

			yield return null;
		}

		if (changeStateOnFade)
		{
			canvas.interactable = fadeIn;
			canvas.blocksRaycasts = fadeIn;
		}
	}

	#endregion


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
