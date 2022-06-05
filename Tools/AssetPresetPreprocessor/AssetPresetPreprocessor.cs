#if UNITY_EDITOR
using System.Linq;
using MyBox.EditorTools;
using UnityEditor;

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
				if (preset.Preset == null) continue;
				if (!preset.Sample(assetPath)) continue;
				if (!preset.Preset.CanBeAppliedTo(assetImporter)) continue;

				context.DependsOnSourceAsset(assetPath);
				preset.Preset.ApplyTo(assetImporter, preset.PropertiesToApply);
				return;
			}
		}

		private bool PreloadBase()
		{
			if (_preprocessBaseChecked) return _preprocessBase != null;
			if (_preprocessBase == null)
			{
				_preprocessBase = MyScriptableObject.LoadAssetsFromResources<AssetsPresetPreprocessBase>().FirstOrDefault();
				if (_preprocessBase == null) _preprocessBase = MyScriptableObject.LoadAssets<AssetsPresetPreprocessBase>().SingleOrDefault();
				
				_preprocessBaseChecked = true;
			}

			return _preprocessBase != null;
		}
	}
}
#endif