using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif


public static class Extensions
{

	public static T RandomElement<T>(this List<T> elements)
	{
		return elements[Random.Range(0, elements.Count - 1)];
	}

    //TODO: To EditorExtensions
#if UNITY_EDITOR
	/// <summary>
	/// Get Prefab path in Asset Database
	/// </summary>
	/// <returns>Null if not a prefab</returns>
	public static string PrefabPath(this GameObject gameObject, bool withAssetName = true)
	{
		if (gameObject == null) return null;
		UnityEngine.Object currentBackgroundPrefab = PrefabUtility.GetPrefabParent(gameObject);
		return currentBackgroundPrefab != null ?
			!withAssetName ? Path.GetDirectoryName(AssetDatabase.GetAssetPath(currentBackgroundPrefab)) : AssetDatabase.GetAssetPath(currentBackgroundPrefab)
			: null;
	}

	public static string AsStringValue(this SerializedProperty property)
	{
		switch (property.propertyType)
		{
			case SerializedPropertyType.String:
				return property.stringValue;

			case SerializedPropertyType.Character:
			case SerializedPropertyType.Integer:
				if (property.type == "char")
				{
					return System.Convert.ToChar(property.intValue).ToString();
				}
				return property.intValue.ToString();

			case SerializedPropertyType.ObjectReference:
				return property.objectReferenceValue != null ? property.objectReferenceValue.ToString() : "null";

			case SerializedPropertyType.Boolean:
				return property.boolValue.ToString();

			case SerializedPropertyType.Enum:
				return property.enumNames[property.enumValueIndex];

			default:
				return string.Empty;
		}
	}

	public static void SetEditorIcon(this GameObject gameObject, bool textIcon, int iconIndex)
	{
		GUIContent[] icons = textIcon ? GetTextures("sv_label_", string.Empty, 0, 8) :
			GetTextures("sv_icon_dot", "_pix16_gizmo", 0, 16);

		var egu = typeof(EditorGUIUtility);
		var flags = BindingFlags.InvokeMethod | BindingFlags.Static | BindingFlags.NonPublic;
		var args = new object[] { gameObject, icons[iconIndex].image };
		var setIcon = egu.GetMethod("SetIconForObject", flags, null, new[] { typeof(UnityEngine.Object), typeof(Texture2D) }, null);
		setIcon.Invoke(null, args);
	}

	private static GUIContent[] GetTextures(string baseName, string postFix, int startIndex, int count)
	{
		GUIContent[] array = new GUIContent[count];
		for (int i = 0; i < count; i++)
		{
			array[i] = EditorGUIUtility.IconContent(baseName + (startIndex + i) + postFix);
		}
		return array;
	}

#endif


	private static void Swap<T>(ref T a, ref T b)
	{
		T x = a;
		a = b;
		b = x;
	}

	public static int ValidateIndex<T>(this T[] array, int desiredIndex)
	{
		if (array.Length == 0) return 0;
		if (desiredIndex < 0) return array.Length - 1;
		if (desiredIndex > array.Length - 1) return 0;
		return desiredIndex;
	}



	// Toggle layers lock

	//Tools.lockedLayers = 1 << LayerMask.NameToLayer("LayerName"); // Replace the whole value of lockedLayers. 
	//Tools.lockedLayers |= 1 << LayerMask.NameToLayer("LayerName"); // Add a value to lockedLayers. 
	//Tools.lockedLayers &= ~(1 << LayerMask.NameToLayer("LayerName")); // Remove a value from lockedLayers. 
	//Tools.lockedLayers ^= 1 << LayerMask.NameToLayer("LayerName")); // Toggle a value in lockedLayers.

	/// <summary>
	/// Clamp value to less than min or more than max
	/// </summary>
	public static float NotInRange(this float num, float min, float max)
	{
		if (min > max)
		{
			var x = min;
			min = max;
			max = x;
		}
		if (num < min || num > max) return num;

		float mid = (max - min) / 2;
		if (num > min)
			return num + mid < max ? min : max;
		else
			return num - mid > min ? max : min;
	}

	/// <summary>
	/// Clamp value to less than min or more than max
	/// </summary>
	public static int NotInRange(this int num, int min, int max)
	{
		return (int)((float)num).NotInRange(min, max);
	}

	/// <summary>
	/// Return point that is closer to num
	/// </summary>
	public static float ClosestPoint(this float num, float pointA, float pointB)
	{
		if (pointA > pointB)
		{
			var x = pointA;
			pointA = pointB;
			pointB = x;
		}

		float middle = (pointB - pointA) / 2;
		float withOffset = num.NotInRange(pointA, pointB) + middle;
		return (withOffset >= pointB) ? pointB : pointA;
	}

	/// <summary>
	/// Check if pointA closer to num than pointB
	/// </summary>
	public static bool ClosestPointIsA(this float num, float pointA, float pointB)
	{
		var closestPoint = num.ClosestPoint(pointA, pointB);
		return Mathf.Approximately(closestPoint, pointA);
	}


	public static bool LayerInMask(this LayerMask mask, int layer)
	{
		return (((1 << layer) & mask) != 0);
	}

	/// <summary>
	/// Removes multiple hits for single gameObject instances
	/// </summary>
	public static RaycastHit2D[] OneHitPerInstance(this RaycastHit2D[] hits)
	{
		if (hits == null) return null;
		return hits.GroupBy(h => h.transform.GetInstanceID()).Select(g => g.First()).ToArray();
	}

	/// <summary>
	/// Removes multiple hits for single gameObject instances
	/// </summary>
	public static Collider2D[] OneHitPerInstance(this Collider2D[] hits)
	{
		if (hits == null) return null;
		return hits.GroupBy(h => h.transform.GetInstanceID()).Select(g => g.First()).ToArray();
	}

	/// <summary>
	/// Removes multiple hits for single gameObject instances
	/// </summary>
	public static List<Collider2D> OneHitPerInstanceList(this Collider2D[] hits)
	{
		if (hits == null) return null;
		return hits.GroupBy(h => h.transform.GetInstanceID()).Select(g => g.First()).ToList();
	}

    public static void SetBodyState(this Rigidbody body, bool state)
    {
        body.isKinematic = !state;
        body.detectCollisions = state;
    }
}