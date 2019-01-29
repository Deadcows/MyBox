using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

/// <summary>
/// Helps to find and/or remove missing scripts on selected objects
/// </summary>
public class MissingScriptsCleaner : MonoBehaviour {

	[MenuItem("Tools/MyBox/Find Missing Scripts", false, 30)]
	static void FindMissingScripts()
	{
		CleanupSelectedObjects(true);
	}

	[MenuItem("Tools/MyBox/Cleanup Missing Scripts", false, 30)]
	static void CleanupMissingScripts()
	{
		CleanupSelectedObjects(false);
	}

	
	private static void CleanupSelectedObjects(bool logOnly)
	{
		int length = Selection.gameObjects.Length;
		
		for (int i = 0; i < length; i++)
		{
			CleanupOnObject(Selection.gameObjects[i], logOnly);

			var progressTitle = "Objects checked: " + i + "/" + length;
			if (EditorUtility.DisplayCancelableProgressBar("Check Missing Scripts", progressTitle, (float) i / length)) break;
		}
		EditorUtility.ClearProgressBar();
	}
	
	private static void CleanupOnObject(GameObject gameObject, bool logOnly)
	{
		var components = gameObject.GetComponents<Component>();
		
		SerializedObject componentSO = null;
		SerializedProperty componentProperty = null;

		bool dirty = false;
		int clearedCount = 0;
		for (int i = 0; i < components.Length; i++)
		{
			if (components[i] != null) continue;
			
			if (!logOnly)
			{
				if (componentSO == null)
				{
					componentSO = new SerializedObject(gameObject);
					componentProperty = componentSO.FindProperty("m_Component");
				}
				componentProperty.DeleteArrayElementAtIndex(i - clearedCount);
				clearedCount++;
				
				dirty = true;
				Debug.Log(gameObject.name + ": Removed Missing Script", gameObject);
			}
			else Debug.Log(gameObject.name + ": Missing Script found", gameObject);
		}

		if (dirty)
		{
			EditorUtility.SetDirty(gameObject);
			componentSO.ApplyModifiedProperties();
			EditorSceneManager.MarkSceneDirty(gameObject.scene);
		}

		foreach (Transform child in gameObject.transform)
		{
			CleanupOnObject(child.gameObject, logOnly);
		}
	}

}
