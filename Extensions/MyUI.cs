using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
// ReSharper disable MemberCanBePrivate.Global

namespace MyBox
{
	public static class MyUI
	{
		/// <summary>
		/// Toggle CanvasGroup Alpha, Interactable and BlocksRaycasts settings
		/// </summary>
		public static void SetCanvasState(CanvasGroup canvas, bool setOn)
		{
			canvas.alpha = setOn ? 1 : 0;
			canvas.interactable = setOn;
			canvas.blocksRaycasts = setOn;
		}

		/// <summary>
		/// Toggle CanvasGroup Alpha, Interactable and BlocksRaycasts settings
		/// </summary>
		public static void SetState(this CanvasGroup canvas, bool isOn) => SetCanvasState(canvas, isOn);
		
		
		/// <summary>
		/// Set width of RectTransform with sizeDelta.x
		/// </summary>
		public static void SetWidth(this RectTransform transform, float width) 
			=> transform.sizeDelta = transform.sizeDelta.SetX(width);

		/// <summary>
		/// Set height of RectTransform with sizeDelta.y
		/// </summary>
		public static void SetHeight(this RectTransform transform, float height) 
			=> transform.sizeDelta = transform.sizeDelta.SetY(height);

		
		public static void SetPositionX(this RectTransform transform, float x) 
			=> transform.anchoredPosition = transform.anchoredPosition.SetX(x);

		public static void SetPositionY(this RectTransform transform, float y) 
			=> transform.anchoredPosition = transform.anchoredPosition.SetY(y);

		public static void OffsetPositionX(this RectTransform transform, float x) 
			=> transform.anchoredPosition = transform.anchoredPosition.OffsetX(x);

		public static void OffsetPositionY(this RectTransform transform, float y) 
			=> transform.anchoredPosition = transform.anchoredPosition.OffsetY(y);
		
		
		/// <summary>
		/// Create EventTriggerType Callback entry and subscribe to EventTrigger
		/// </summary>
		public static EventTrigger.Entry OnEventSubscribe(this EventTrigger trigger, EventTriggerType eventType, System.Action<BaseEventData> callback)
		{
			EventTrigger.Entry entry = new EventTrigger.Entry();
			entry.eventID = eventType;
			entry.callback = new EventTrigger.TriggerEvent();
			entry.callback.AddListener(new UnityAction<BaseEventData>(callback));
			trigger.triggers.Add(entry);
			return entry;
		}

		/// <summary>
		/// Unsubscribe Callback entry from EventTrigger
		/// </summary>
		public static void OnEventUnsubscribe(this EventTrigger trigger, EventTrigger.Entry entry) => trigger.triggers.Add(entry);
		

		/// <summary>
		/// Adds the specified amount as Vector2 to the source RectTransform's both
		/// anchors.
		/// </summary>
		public static RectTransform ShiftAnchor(this RectTransform source, Vector2 delta)
		{
			source.anchorMin += delta;
			source.anchorMax += delta;
			return source;
		}
		
		/// <summary>
		/// Adds the specified amount to the source RectTransform's both anchors.
		/// </summary>
		public static RectTransform ShiftAnchor(this RectTransform source, float x, float y) => source.ShiftAnchor(new Vector2(x, y));


		/// <summary>
		/// Gets the average of the sum of the source RectTransform's anchors.
		/// Effectively the parent-relative position of the RectTransform.
		/// </summary>
		public static Vector2 GetAnchorCenter(this RectTransform source) => (source.anchorMin + source.anchorMax) / 2;

		/// <summary>
		/// Gets the result of the source RectTransform's anchorMax subtracted by its
		/// anchorMin.
		/// Effectively the parent-relative size of the RectTransform.
		/// </summary>
		public static Vector2 GetAnchorDelta(this RectTransform source) => source.anchorMax - source.anchorMin;
	}
}