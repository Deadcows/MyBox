using System;
using UnityEngine;

namespace MyBox.Internal
{
	[ExecuteInEditMode]
	public class MyEditorEventsBehaviorHandler : MonoBehaviour
	{
		public static void InitializeInstance()
		{
			if (_instance != null) return;
			_instance = FindAnyObjectByType<MyEditorEventsBehaviorHandler>();
			if (_instance != null) return;
			
			var handlerGameObject = new GameObject("MyEditorEventsBehaviorHandler");
			_instance = handlerGameObject.AddComponent<MyEditorEventsBehaviorHandler>();
			if (Application.isPlaying) DontDestroyOnLoad(_instance);
			handlerGameObject.hideFlags = HideFlags.HideAndDontSave;
		}
		private static MyEditorEventsBehaviorHandler _instance;

		private void AssureSingleInstance()
		{
			if (_instance == null) _instance = this;
			else if (_instance != this) DestroyImmediate(gameObject);
		}

		private void Awake() => AssureSingleInstance();
		private void OnEnable() => AssureSingleInstance();
		
		
		public static event Action OnGUIEvent;
		public static event Action OnUpdate;


		private void OnGUI() => OnGUIEvent?.Invoke();
		private void Update() => OnUpdate?.Invoke();
	}
}