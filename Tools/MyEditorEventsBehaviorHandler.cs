using System;
using UnityEngine;

namespace MyBox.Internal
{
	public class MyEditorEventsBehaviorHandler : MonoBehaviour
	{
		public static void InitializeInstance()
		{
			if (_instance != null) return;
			
			var handlerGameObject = new GameObject("MyEditorEventsBehaviorHandler");
			_instance = handlerGameObject.AddComponent<MyEditorEventsBehaviorHandler>();
			if (Application.isPlaying) DontDestroyOnLoad(_instance.gameObject);
			handlerGameObject.hideFlags = HideFlags.HideAndDontSave;
		}
		private static MyEditorEventsBehaviorHandler _instance;
		
		public static event Action OnGUIEvent;
		public static event Action OnUpdate;

		private void OnGUI() => OnGUIEvent?.Invoke();
		private void Update() => OnUpdate?.Invoke();
		
		private void OnDisable() => OnGUIEvent = null;
	}
}