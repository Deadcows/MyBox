using UnityEngine;

public static class MyExtensions
{

	public static bool IsPrefabInstance(this GameObject go)
	{
#if UNITY_EDITOR
		return UnityEditor.PrefabUtility.GetPrefabType(go) == UnityEditor.PrefabType.Prefab;

#else
		return false;
#endif
	}

}
