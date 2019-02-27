#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;


namespace MyBox.Internal
{
	public class CleanEmptyDirectoriesFeature : UnityEditor.AssetModificationProcessor
	{
		private const string MenuItemName = "Tools/MyBox/Clear Empty Directories On Save";

		private static bool IsEnabled
		{
			get { return MyBoxSettings.CleanEmptyDirectoriesFeature; }
			set { MyBoxSettings.CleanEmptyDirectoriesFeature = value; }
		}

		[MenuItem(MenuItemName, priority = 100)]
		private static void MenuItem()
		{
			IsEnabled = !IsEnabled;
		}

		[MenuItem(MenuItemName, true)]
		private static bool MenuItemValidation()
		{
			Menu.SetChecked(MenuItemName, IsEnabled);
			return true;
		}

		
		public static string[] OnWillSaveAssets(string[] paths)
		{
			if (!IsEnabled) return paths;
			
			List<DirectoryInfo> emptyDirectories = GetEmptyDirectories();
			if (emptyDirectories == null) return paths;
			
			foreach (var emptyDirectory in emptyDirectories) DeleteEmptyDirectory(emptyDirectory);

			return paths;
		}

		private static List<DirectoryInfo> GetEmptyDirectories()
		{
			var emptyDirectories = new List<DirectoryInfo>();
			var assetDirectory = new DirectoryInfo(Application.dataPath);

			WalkDirectoryTree(assetDirectory, (directoryInfo, areSubDirectoriesEmpty) =>
			{
				bool isEmpty = areSubDirectoriesEmpty && DirectoryIsEmpty(directoryInfo);
				if (isEmpty) emptyDirectories.Add(directoryInfo);
				return isEmpty;
			});

			return emptyDirectories;
		}

		private static bool DirectoryIsEmpty(DirectoryInfo dirInfo)
		{
			FileInfo[] files = null;

			try
			{
				files = dirInfo.GetFiles("*.*");
				files = files.Where(x => !x.Name.EndsWith(".meta")).ToArray();
			}
			catch (Exception ex)
			{
				Debug.LogException(ex);
			}

			return files == null || files.Length == 0;
		}


		private static void DeleteEmptyDirectory(DirectoryInfo emptyDirectory)
		{
			var relativePath = GetRelativePath(emptyDirectory.FullName, Directory.GetCurrentDirectory());
			AssetDatabase.MoveAssetToTrash(relativePath);
			Debug.Log("Empty directory removed at: " + emptyDirectory.FullName);
		}


		// return: Is this directory empty?
		private delegate bool IsEmptyDirectory(DirectoryInfo dirInfo, bool areSubDirectoriesEmpty);

		// return: Is this directory empty?
		private static bool WalkDirectoryTree(DirectoryInfo root, IsEmptyDirectory pred)
		{
			DirectoryInfo[] subDirectories = root.GetDirectories();

			bool areSubDirsEmpty = true;
			foreach (DirectoryInfo dirInfo in subDirectories)
			{
				if (!WalkDirectoryTree(dirInfo, pred))
					areSubDirsEmpty = false;
			}

			bool isRootEmpty = pred(root, areSubDirsEmpty);
			return isRootEmpty;
		}

		private static string GetRelativePath(string file, string folder)
		{
			Uri pathUri = new Uri(file);
			// Folders must end in a slash
			if (!folder.EndsWith(Path.DirectorySeparatorChar.ToString()))
				folder += Path.DirectorySeparatorChar;

			Uri folderUri = new Uri(folder);
			return Uri.UnescapeDataString(folderUri.MakeRelativeUri(pathUri).ToString().Replace('/', Path.DirectorySeparatorChar));
		}
	}
}
#endif