#if UNITY_EDITOR
using System;
using System.IO;
using UnityEditor.Presets;
using UnityEngine;

namespace MyBox.Internal
{
	public class AssetsPresetPreprocessBase : ScriptableObject
	{
		public ConditionalPreset[] Presets;

		public string[] ExcludeProperties = {"SpriteBorder", "Pivot", "Alignment"};
	}

	[Serializable]
	public class ConditionalPreset
	{
		public string PathContains;
		public string TypeOf;
		public string Prefix;
		public string Postfix;
		
		public Preset Preset;

		public string[] PropertiesToApply;

		public bool Sample(string path)
		{
			var pathSet = !string.IsNullOrEmpty(PathContains);
			var typeSet = !string.IsNullOrEmpty(TypeOf);
			var prefixSet = !string.IsNullOrEmpty(Prefix);
			var postfixSet = !string.IsNullOrEmpty(Postfix);
			
			if (pathSet && !path.Contains(PathContains)) return false;
			
			var extension = Path.GetExtension(path);
			var filename = Path.GetFileNameWithoutExtension(path);
			if (extension == null || filename == null) return false;
			
			if (typeSet && !extension.Contains(TypeOf)) return false;
			
			if (prefixSet && !filename.StartsWith(Prefix)) return false;
			if (postfixSet && !filename.EndsWith(Postfix)) return false;
			
			return true;
		}
	}
}
#endif