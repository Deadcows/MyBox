#if UNITY_EDITOR
using System.Linq;
using MyBox.EditorTools;
using UnityEditor;
using UnityEngine;

namespace MyBox.Internal
{
	public class AssetPresetPreprocessor : AssetPostprocessor
	{
		private static AssetsPresetPreprocessBase _preprocessBase;
		private static bool _preprocessBaseChecked;

		private void OnPreprocessAsset()
		{
			if (!PreloadBase()) return;

			foreach (var preset in _preprocessBase.Presets)
			{
				if (!preset.Sample(assetPath)) continue;

				if (!preset.Preset.CanBeAppliedTo(assetImporter))
				{
					Debug.LogError("Preset " + preset.Preset + " cannot be applied to object at path " + assetPath);
				}
				else
				{
					preset.Preset.ApplyTo(assetImporter);
					return;
				}
			}
		}

		private bool PreloadBase()
		{
			if (_preprocessBaseChecked) return false;
			
			if (_preprocessBase == null)
			{
				_preprocessBase = MyScriptableObject.LoadAssetsFromResources<AssetsPresetPreprocessBase>().FirstOrDefault();
				if (_preprocessBase == null) _preprocessBase = MyScriptableObject.LoadAssets<AssetsPresetPreprocessBase>().SingleOrDefault();
			}

			_preprocessBaseChecked = true;
			return _preprocessBase != null;
		}
	}
}
#endif