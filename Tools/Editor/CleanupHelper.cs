using UnityEditor;
using UnityEngine;

public class CleanupHelper : MonoBehaviour {

	[MenuItem("Tools/Cleanup Missing Scripts")]
	static void CleanupMissingScripts()
	{
		for (int i = 0; i < Selection.gameObjects.Length; i++)
		{
			CleanupOnObject(Selection.gameObjects[i]);
		}
	}

	private static void CleanupOnObject(GameObject gameObject)
	{
		var components = gameObject.GetComponents<Component>();

		var serializedObject = new SerializedObject(gameObject);
		var prop = serializedObject.FindProperty("m_Component");

		int r = 0;
		for (int j = 0; j < components.Length; j++)
		{
			if (components[j] == null)
			{
				prop.DeleteArrayElementAtIndex(j - r);
				r++;

				EditorUtility.SetDirty(gameObject);
				Debug.Log(gameObject.name + ": Removed Missing Script");
			}
		}

		serializedObject.ApplyModifiedProperties();

		foreach (Transform child in gameObject.transform)
		{
			CleanupOnObject(child.gameObject);
		}
	}

}
