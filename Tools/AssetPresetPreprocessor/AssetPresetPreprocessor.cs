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
			if (_preprocessBase == null)
			{
				_preprocessBase = MyScriptableObject.LoadAssetsFromResources<AssetsPresetPreprocessBase>().FirstOrDefault();
				if (_preprocessBase == null) _preprocessBase = MyScriptableObject.LoadAssets<AssetsPresetPreprocessBase>().SingleOrDefault();
			}

			if (_preprocessBase == null) Debug.LogError("AssetsPresetPostprocessBase is null");

			return _preprocessBase != null;
		}
	}
}
#endif