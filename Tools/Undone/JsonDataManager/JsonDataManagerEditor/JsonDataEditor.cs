//#if UNITY_EDITOR
//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Linq;
//using MyBox.EditorTools;
//using UnityEditor;
//using UnityEngine;
//
//namespace MyBox.Internal
//{
//	public class JsonDataEditor : EditorWindow
//	{
//		//[MenuItem("Tools/MyBox/Settings Editor", false, 50)]
//		private static void Initialize()
//		{
//			var window = GetWindow<JsonDataEditor>();
//			window.Show();
//		}
//
//		private void OnEnable()
//		{
//			hideFlags = HideFlags.HideAndDontSave;
//			if (!SettingsLoaded) LoadSettings();
//		}
//
//
//		#region Load Settings
//
//		private bool SettingsLoaded
//		{
//			get { return !(_settings == null || _settings.Length == 0); }
//		}
//
//		private string SettingsFolder
//		{
//			get { return Application.dataPath + "/" + JsonDataManager.EditorSettingsFolder; }
//		}
//
//		private string[] SettingsFiles
//		{
//			get { return Directory.GetFiles(SettingsFolder).Where(f => f.EndsWith(".json")).ToArray(); }
//		}
//
//		private void LoadSettings()
//		{
//			_settings = GetValidSettingsData();
//			GUI.FocusControl(string.Empty);
//		}
//
//		private SettingsData[] GetValidSettingsData()
//		{
//			var settings = new List<SettingsData>();
//			var settingsPaths = SettingsFiles;
//			var hideAttributeType = typeof(HideInJsonEditorAttribute);
//
//			for (var i = 0; i < settingsPaths.Length; i++)
//			{
//				var path = settingsPaths[i];
//				var typeName = Path.GetFileNameWithoutExtension(settingsPaths[i]);
//				if (string.IsNullOrEmpty(typeName)) continue;
//				var settingsType = GetTypeByName(typeName);
//				if (settingsType == null) continue;
//				if (Attribute.IsDefined(settingsType, hideAttributeType)) continue;
//
//				var settingsData = CreateInstance<SettingsData>();
//				settingsData.Initialize(path);
//				settings.Add(settingsData);
//			}
//
//			return settings.ToArray();
//		}
//
//		private Type GetTypeByName(string typeName)
//		{
//			return (from assembly in AppDomain.CurrentDomain.GetAssemblies()
//				from type in assembly.GetTypes()
//				where type.Name == typeName
//				select type).SingleOrDefault();
//		}
//
//		private void CreateNewSettingsFile(string typeName)
//		{
//			var targetType = GetTypeByName(typeName);
//			if (targetType == null || !targetType.IsSubclassOf(typeof(ScriptableObject))) return;
//
//			bool exists = SettingsFiles.Any(f => f.Contains(typeName));
//			if (exists) return;
//
//			var instance = CreateInstance(targetType.Name);
//			var jsonString = JsonUtility.ToJson(instance, true);
//			var filePath = Path.Combine(SettingsFolder, typeName + ".json");
//
//			File.WriteAllText(filePath, jsonString);
//			AssetDatabase.Refresh();
//
//			LoadSettings();
//		}
//
//		#endregion
//
//
//		private bool IsChanged
//		{
//			get { return _settings != null && _settings.Any(s => s.Changed); }
//		}
//
//		[SerializeField] private SettingsData[] _settings;
//
//		private Vector2 _scroll;
//		private string _newSettingsFile;
//
//
//		private void OnGUI()
//		{
//			CheckData();
//
//			DrawSaveLoadButtons();
//
//			DrawSettingsInspectors();
//
//			DrawCreateSettingsFileButton();
//		}
//
//		private void CheckData()
//		{
//			if (!SettingsLoaded || _settings.Any(s => s == null))
//				LoadSettings();
//		}
//
//		#region Save Load Buttons
//
//		private void DrawSaveLoadButtons()
//		{
//			var dirty = IsChanged;
//			GUI.enabled = dirty;
//			var color = GUI.color;
//
//			EditorGUILayout.Space();
//			EditorGUILayout.Space();
//			using (new EditorGUILayout.HorizontalScope())
//			{
//				EditorGUILayout.Space();
//				if (dirty) GUI.color = Color.green;
//				if (GUILayout.Button("Save Changes", EditorStyles.toolbarButton)) SaveChanges();
//				GUI.color = color;
//				GUI.enabled = true;
//				EditorGUILayout.Space();
//				if (GUILayout.Button("Reload Settings", EditorStyles.toolbarButton)) ResetChanges();
//				EditorGUILayout.Space();
//			}
//
//			EditorGUILayout.Space();
//		}
//
//		private void SaveChanges()
//		{
//			foreach (var setting in _settings)
//			{
//				if (setting.Changed) setting.SaveChanges();
//			}
//		}
//
//		private void ResetChanges()
//		{
//			foreach (var setting in _settings)
//			{
//				if (setting.Changed) setting.ResetChanges();
//			}
//
//			Repaint();
//		}
//
//		#endregion
//
//
//		#region Settings inspectors
//
//		private void DrawSettingsInspectors()
//		{
//			using (var scroll = new EditorGUILayout.ScrollViewScope(_scroll))
//			{
//				_scroll = scroll.scrollPosition;
//
//				for (var i = 0; i < _settings.Length; i++)
//				{
//					var settings = _settings[i];
//
//					settings.Foldout = EditorGUILayout.Foldout(settings.Foldout, settings.Name);
//
//					if (settings.Foldout)
//					{
//						settings.DrawInspector();
//					}
//
//					MyGUI.DrawLine(MyGUI.Gray, true);
//				}
//			}
//		}
//
//		#endregion
//
//
//		#region Create Settings File
//
//		private void DrawCreateSettingsFileButton()
//		{
//			using (new EditorGUILayout.HorizontalScope())
//			{
//				_newSettingsFile = EditorGUILayout.TextField(_newSettingsFile);
//				if (GUILayout.Button("Create Setting File", EditorStyles.toolbarButton))
//				{
//					CreateNewSettingsFile(_newSettingsFile);
//					_newSettingsFile = string.Empty;
//				}
//			}
//		}
//
//		#endregion
//
//
//		#region Settings Data
//
//		[Serializable]
//		private class SettingsData : ScriptableObject
//		{
//			public string Name
//			{
//				get { return _name; }
//			}
//
//			public bool Changed
//			{
//				get { return _changed; }
//			}
//
//			public bool Foldout
//			{
//				get { return _foldout; }
//				set
//				{
//					if (_foldout == value) return;
//					EditorPrefs.SetBool(FoldStateKey, value);
//					_foldout = value;
//				}
//			}
//
//			private string FoldStateKey
//			{
//				get { return string.Format("JsonDataEditor_{0}_foldState", _name); }
//			}
//
//
//			[SerializeField] private bool _changed;
//
//			[SerializeField] private string _name;
//			[SerializeField] private ScriptableObject _so;
//			[SerializeField] private Editor _editor;
//			[SerializeField] private string _filePath;
//
//			[SerializeField] private bool _foldout;
//
//
//			private void OnEnable()
//			{
//				hideFlags = HideFlags.HideAndDontSave;
//			}
//
//			public void DrawInspector()
//			{
//				EditorGUI.BeginChangeCheck();
//				_editor.OnInspectorGUI();
//				if (EditorGUI.EndChangeCheck()) _changed = true;
//			}
//
//			public void Initialize(string path)
//			{
//				_filePath = path;
//				_name = Path.GetFileNameWithoutExtension(path);
//				_foldout = EditorPrefs.GetBool(FoldStateKey, false);
//
//				LoadFromFile();
//			}
//
//			public void ResetChanges()
//			{
//				_changed = false;
//
//				LoadFromFile();
//			}
//
//			public void SaveChanges()
//			{
//				_changed = false;
//				var jsonString = JsonUtility.ToJson(_so, true);
//
//				File.WriteAllText(_filePath, jsonString);
//
//				AssetDatabase.Refresh();
//			}
//
//			private void LoadFromFile()
//			{
//				_so = LoadObject(_filePath);
//				_editor = Editor.CreateEditor(_so);
//
//				_so.hideFlags = HideFlags.DontSave;
//				_editor.hideFlags = HideFlags.HideAndDontSave;
//			}
//
//			private ScriptableObject LoadObject(string path)
//			{
//				var contents = File.ReadAllText(path);
//				var obj = CreateInstance(Name);
//				JsonUtility.FromJsonOverwrite(contents, obj);
//
//				return obj;
//			}
//		}
//
//		#endregion
//	}
//}
//#endif