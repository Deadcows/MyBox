using System;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEditor.Build;
using UnityEditor;

public class JsonDataSettingsTransfer : IPostprocessBuild
{
	public int callbackOrder
	{
		get { return 0; }
	}


	public void OnPostprocessBuild(BuildTarget target, string path)
	{
		OnBuild(path);
	}

	private void OnBuild(string path)
	{
		var buildPath = BuildSettingsPath(path);
		var editorPath = AssetSettingsPath();

		var settings = SettingsFileNames(editorPath);
		Directory.CreateDirectory(buildPath);
		foreach (var setting in settings)
		{
			var filename = Path.GetFileName(setting);
			var newFilepath = Path.Combine(buildPath, filename);
			File.Copy(setting, newFilepath);
		}

		Debug.Log(string.Format("JsonDataSettingsTransfer moved {0} settings files to {1}", settings.Length, buildPath));
	}

	private string BuildSettingsPath(string exePath)
	{
		var basePath = Path.GetDirectoryName(exePath);
		var gameFolder = Path.GetFileNameWithoutExtension(exePath) + "_Data";
		var fullPath = Path.Combine(basePath, gameFolder);
		var settingsPath = Path.Combine(fullPath, JsonDataManager.BuildSettingsFolder);

		return settingsPath;
	}

	private string AssetSettingsPath()
	{
		return Path.Combine(Application.dataPath, JsonDataManager.EditorSettingsFolder);
	}

	private string[] SettingsFileNames(string path)
	{
		return Directory.GetFiles(path).Where(p => p.EndsWith(".json")).ToArray();
	}
}