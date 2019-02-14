using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayGameShortcut
{
	[MenuItem("Edit/Run _F5")] // shortcut key F5 to Play (and exit playmode also)
	static void PlayGame()
	{
		if (!Application.isPlaying)
		{
			EditorSceneManager.SaveScene(SceneManager.GetActiveScene(), "", false); // optional: save before run
		}
		EditorApplication.ExecuteMenuItem("Edit/Play");
	}
}
