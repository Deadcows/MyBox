#if UNITY_EDITOR
using System.Collections.Generic;
using System.Linq;
using MyBox.EditorTools;
using UnityEditor;
using UnityEditor.Presets;
using UnityEngine;

namespace MyBox.Internal
{
	[CustomEditor(typeof(AssetsPresetPreprocessBase))]
	public class AssetsPresetPreprocessEditor : Editor
	{
		[MenuItem("Tools/MyBox/Postprocess Preset Tool", false, 50)]
		private static void SelectBase()
		{
			var presetBase = MyScriptableObject.LoadAssetsFromResources<AssetsPresetPreprocessBase>().FirstOrDefault();
			if (presetBase == null)
			{
				presetBase = MyScriptableObject.CreateAssetWithFolderDialog<AssetsPresetPreprocessBase>("AssetsPresetPostprocessBase");
			}

			if (presetBase != null) Selection.activeObject = presetBase;
		}

		private Vector2 _scrollPos;
		private GUIStyle _labelStyle;

		private AssetsPresetPreprocessBase _target;
		private ReorderableCollection _reorderableBase;
		private SerializedProperty _presets;
		private SerializedProperty _exclude;

		private void OnEnable()
		{
			_labelStyle = new GUIStyle(EditorStyles.label);
			_labelStyle.richText = true;

			_target = target as AssetsPresetPreprocessBase;
			
			_presets = serializedObject.FindProperty("Presets");
			_exclude = serializedObject.FindProperty("ExcludeProperties");
			_reorderableBase = new ReorderableCollection(_presets);

			_reorderableBase.CustomDrawerHeight += PresetDrawerHeight;
			_reorderableBase.CustomDrawer += PresetDrawer;
			_reorderableBase.CustomAdd += CustomAdd;
		}

		private void OnDisable()
		{
			if (_reorderableBase == null) return;
			_reorderableBase.CustomDrawerHeight -= PresetDrawerHeight;
			_reorderableBase.CustomDrawer -= PresetDrawer;
			_reorderableBase.CustomAdd -= CustomAdd;
			_reorderableBase = null;
		}

		private int PresetDrawerHeight(int index)
		{
			return (int) (EditorGUIUtility.singleLineHeight * 2 + 4);
		}

		private bool CustomAdd(int index)
		{
			EditorApplication.delayCall += () =>
			{
				var newElement = _presets.GetArrayElementAtIndex(index);
				newElement.FindPropertyRelative("PathContains").stringValue = string.Empty;
				newElement.FindPropertyRelative("TypeOf").stringValue = string.Empty;
				newElement.FindPropertyRelative("Prefix").stringValue = string.Empty;
				newElement.FindPropertyRelative("Postfix").stringValue = string.Empty;
				newElement.FindPropertyRelative("Preset").objectReferenceValue = null;
				newElement.serializedObject.ApplyModifiedProperties();
			};
			return false;
		}

		private void PresetDrawer(SerializedProperty property, Rect rect, int index)
		{
			var properties = new PresetProperties(property);
			DrawPresetColourLine(rect, properties.Preset.objectReferenceValue as Preset);
			rect.width -= 6;
			rect.x += 6;


			EditorGUI.BeginChangeCheck();

			rect.height = EditorGUIUtility.singleLineHeight;
			var labelWidth = 24;
			var betweenFields = 6;

			var firstLineRect = new Rect(rect);
			var flRatio = (rect.width - (labelWidth * 2 + betweenFields)) / 5;
			firstLineRect.width = flRatio * 3;

			EditorGUI.LabelField(firstLineRect, "PC:");
			firstLineRect.x += labelWidth;
			EditorGUI.PropertyField(firstLineRect, properties.PathContains, GUIContent.none);

			firstLineRect.x += firstLineRect.width + betweenFields;
			firstLineRect.width = flRatio * 2;
			EditorGUI.LabelField(firstLineRect, "FT:");
			firstLineRect.x += labelWidth;
			EditorGUI.PropertyField(firstLineRect, properties.TypeOf, GUIContent.none);


			rect.y += EditorGUIUtility.singleLineHeight + 2;
			var secondLineRect = new Rect(rect);
			var slRatio = (rect.width - (labelWidth * 3 + betweenFields * 2)) / 10;

			var halfW = flRatio * 3 / 2 - (labelWidth / 2f) - (betweenFields / 2f);
			secondLineRect.width = halfW;
			EditorGUI.LabelField(secondLineRect, "Pr:");
			secondLineRect.x += labelWidth;
			EditorGUI.PropertyField(secondLineRect, properties.Prefix, GUIContent.none);

			secondLineRect.x += secondLineRect.width + betweenFields;
			secondLineRect.width = halfW;
			EditorGUI.LabelField(secondLineRect, "Po:");
			secondLineRect.x += labelWidth;
			EditorGUI.PropertyField(secondLineRect, properties.Postfix, GUIContent.none);

			secondLineRect.x += secondLineRect.width + betweenFields;
			secondLineRect.width = slRatio * 4;
			secondLineRect.x += labelWidth;
			
			EditorGUI.PropertyField(secondLineRect, properties.Preset, GUIContent.none);


			if (EditorGUI.EndChangeCheck()) property.serializedObject.ApplyModifiedProperties();
		}


		private struct PresetProperties
		{
			public readonly SerializedProperty PathContains;
			public readonly SerializedProperty TypeOf;
			public readonly SerializedProperty Prefix;
			public readonly SerializedProperty Postfix;

			public readonly SerializedProperty Preset;

			public PresetProperties(SerializedProperty baseProperty)
			{
				PathContains = baseProperty.FindPropertyRelative("PathContains");
				TypeOf = baseProperty.FindPropertyRelative("TypeOf");
				Prefix = baseProperty.FindPropertyRelative("Prefix");
				Postfix = baseProperty.FindPropertyRelative("Postfix");
				Preset = baseProperty.FindPropertyRelative("Preset");
			}
		}

		private void DrawPresetColourLine(Rect rect, Preset preset)
		{
			var cRect = new Rect(rect);
			cRect.width = 6;
			cRect.height -= 2;

			Color color = MyGUI.Brown;
			if (preset == null) color = Color.red;
			else
			{
				var presetType = preset.GetTargetTypeName();
				if (presetType.Contains("Texture")) color = MyGUI.Blue;
				else if (presetType.Contains("Audio")) color = MyGUI.Red;
			}

			MyGUI.DrawColouredRect(cRect, color);
			EditorGUI.LabelField(cRect, GUIContent.none);
		}

		public override void OnInspectorGUI()
		{
			serializedObject.Update();
			EditorGUILayout.Space();
			EditorGUILayout.LabelField("First match will be applied");
			EditorGUILayout.LabelField("Assets/...<b>[PC:Path Contains]</b>.../", _labelStyle);
			EditorGUILayout.LabelField("<b>[Pr:Prefix]</b>...<b>[Po:Postfix]</b>.<b>[FT:File Type]</b>", _labelStyle);
			EditorGUILayout.Space();

			_scrollPos = GUILayout.BeginScrollView(_scrollPos);

			_reorderableBase.Draw();

			EditorGUILayout.Space();

			EditorGUI.BeginChangeCheck();
			EditorGUILayout.PropertyField(_exclude, true);
			if (EditorGUI.EndChangeCheck())
			{
				EditorApplication.delayCall += UpdateExcludes;
			}

			EditorGUILayout.Space();

			if (GUILayout.Button("Update Excludes", EditorStyles.toolbarButton)) UpdateExcludes();

			GUILayout.EndScrollView();

			serializedObject.ApplyModifiedProperties();
		}

		private void UpdateExcludes()
		{
			foreach (var preset in _target.Presets)
			{
				if (preset.Preset == null) continue;

				UpdateExcludesOnPreset(preset);
			}
		}

		private void UpdateExcludesOnPreset(ConditionalPreset preset)
		{
			var toApply = new List<string>();
			foreach (var modification in preset.Preset.PropertyModifications)
			{
				var path = modification.propertyPath;
				bool exclude = false;
				for (var i = 0; i < _target.ExcludeProperties.Length; i++)
				{
					var excludePath = _target.ExcludeProperties[i];
					if (path.Contains(excludePath))
					{
						exclude = true;
						break;
					}
				}
				if (!exclude) toApply.Add(path);

				serializedObject.ApplyModifiedProperties();
			}

			preset.PropertiesToApply = toApply.ToArray();
			EditorUtility.SetDirty(target);
		}
	}
}
#endif