using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;


[InitializeOnLoad]
public class EmptyDirectoriesCleaner : UnityEditor.AssetModificationProcessor
{

	public static string[] OnWillSaveAssets(string[] paths)
	{
		List<DirectoryInfo> emptyDirs = GetEmptyDirectories();

		if (emptyDirs != null && emptyDirs.Count > 0)
		{
			DeleteAllEmptyDirAndMeta(ref emptyDirs);

			Debug.Log("[Clean] Cleaned Empty Directories on Save");
		}

		return paths;
	}


	private static void DeleteAllEmptyDirAndMeta(ref List<DirectoryInfo> emptyDirs)
	{
		foreach (var dirInfo in emptyDirs)
		{
			AssetDatabase.MoveAssetToTrash(GetRelativePathFromCd(dirInfo.FullName));
			Debug.Log("Empty dirrectory removed at: " + dirInfo.FullName);
		}

		emptyDirs = null;
	}


	private static List<DirectoryInfo> GetEmptyDirectories()
	{
		var emptyDirectories = new List<DirectoryInfo>();

		var assetDir = new DirectoryInfo(Application.dataPath);

		WalkDirectoryTree(assetDir, (dirInfo, areSubDirsEmpty) =>
		{
			bool isDirEmpty = areSubDirsEmpty && DirHasNoFile(dirInfo);
			if (isDirEmpty)
				emptyDirectories.Add(dirInfo);
			return isDirEmpty;
		});

		return emptyDirectories;
	}

	// return: Is this directory empty?
	delegate bool IsEmptyDirectory(DirectoryInfo dirInfo, bool areSubDirsEmpty);

	// return: Is this directory empty?
	static bool WalkDirectoryTree(DirectoryInfo root, IsEmptyDirectory pred)
	{
		DirectoryInfo[] subDirs = root.GetDirectories();

		bool areSubDirsEmpty = true;
		foreach (DirectoryInfo dirInfo in subDirs)
		{
			if (false == WalkDirectoryTree(dirInfo, pred))
				areSubDirsEmpty = false;
		}

		bool isRootEmpty = pred(root, areSubDirsEmpty);
		return isRootEmpty;
	}

	static bool DirHasNoFile(DirectoryInfo dirInfo)
	{
		FileInfo[] files = null;

		try
		{
			files = dirInfo.GetFiles("*.*");
			files = files.Where(x => !IsMetaFile(x.Name)).ToArray();
		}
		catch (Exception)
		{
		}

		return files == null || files.Length == 0;
	}

	static string GetRelativePathFromCd(string filespec)
	{
		return GetRelativePath(filespec, Directory.GetCurrentDirectory());
	}

	private static string GetRelativePath(string filespec, string folder)
	{
		Uri pathUri = new Uri(filespec);
		// Folders must end in a slash
		if (!folder.EndsWith(Path.DirectorySeparatorChar.ToString()))
		{
			folder += Path.DirectorySeparatorChar;
		}
		Uri folderUri = new Uri(folder);
		return Uri.UnescapeDataString(folderUri.MakeRelativeUri(pathUri).ToString().Replace('/', Path.DirectorySeparatorChar));
	}

	
	static bool IsMetaFile(string path)
	{
		return path.EndsWith(".meta");
	}
}
