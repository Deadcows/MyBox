using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public static class MyExtensions
{
    public static T RandomElement<T>(this List<T> elements)
    {
        return elements[Random.Range(0, elements.Count - 1)];
    }

    public static bool IsNullOrEmpty<T>(this IEnumerable<T> sequence)
    {
        if (sequence == null) return true;

        return !sequence.Any();
    }

    public static void Swap<T>(ref T a, ref T b)
    {
        T x = a;
        a = b;
        b = x;
    }

    /// <summary>
    /// Get fixed index for looping sequences. Where -1 will result with last element index
    /// </summary>
    public static int ValidateIndex<T>(this T[] array, int desiredIndex)
    {
        if (array.Length == 0) return 0;
        if (desiredIndex < 0) return array.Length - 1;
        if (desiredIndex > array.Length - 1) return 0;
        return desiredIndex;
    }

    public static T GetOrAddComponent<T>(this GameObject gameObject) where T : Component
    {
        return gameObject.GetComponent<T>() ?? gameObject.AddComponent<T>();
    }

    public static T GetOrAddComponent<T>(this Component component) where T : Component
    {
        return component.GetComponent<T>() ?? component.gameObject.AddComponent<T>();
    }

    public static List<Transform> GetObjectsOfLayerInChild(this GameObject gameObject, int layer)
    {
        List<Transform> list = new List<Transform>();
        CheckChilds(gameObject.transform, layer, list);
        return list;
    }

    private static void CheckChilds(Transform transform, int layer, List<Transform> childsCache)
    {
        foreach (Transform t in transform)
        {
            CheckChilds(t, layer, childsCache);

            if (t.gameObject.layer != layer) continue;
            childsCache.Add(t);
        }
    }


    public static List<Transform> GetObjectsOfLayerInChild(this Component component, int layer)
    {
        return component.gameObject.GetObjectsOfLayerInChild(layer);
    }


    /// <returns>
    ///   Returns -1 if none found
    /// </returns>
    public static int IndexOf<T>(this IEnumerable<T> items, T item)
    {
        var index = 0;

        foreach (var i in items)
        {
            if (Equals(i, item))
            {
                return index;
            }

            ++index;
        }

        return -1;
    }


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

        if (num > min) return num + mid < max ? min : max;
        return num - mid > min ? max : min;
    }

    /// <summary>
    /// Clamp value to less than min or more than max
    /// </summary>
    public static int NotInRange(this int num, int min, int max)
    {
        return (int) ((float) num).NotInRange(min, max);
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


    public static T[] OnePerInstance<T>(this T[] components) where T : Component
    {
        if (components == null || components.Length == 0) return null;
        return components.GroupBy(h => h.transform.GetInstanceID()).Select(g => g.First()).ToArray();
    }

    /// <summary>
    /// Removes multiple hits for single gameObject instances
    /// </summary>
    public static RaycastHit2D[] OneHitPerInstance(this RaycastHit2D[] hits)
    {
        if (hits == null || hits.Length == 0) return null;
        return hits.GroupBy(h => h.transform.GetInstanceID()).Select(g => g.First()).ToArray();
    }

    /// <summary>
    /// Removes multiple hits for single gameObject instances
    /// </summary>
    public static Collider2D[] OneHitPerInstance(this Collider2D[] hits)
    {
        if (hits == null || hits.Length == 0) return null;
        return hits.GroupBy(h => h.transform.GetInstanceID()).Select(g => g.First()).ToArray();
    }

    /// <summary>
    /// Removes multiple hits for single gameObject instances
    /// </summary>
    public static List<Collider2D> OneHitPerInstanceList(this Collider2D[] hits)
    {
        if (hits == null || hits.Length == 0) return null;
        return hits.GroupBy(h => h.transform.GetInstanceID()).Select(g => g.First()).ToList();
    }

    public static void SetBodyState(this Rigidbody body, bool state)
    {
        body.isKinematic = !state;
        body.detectCollisions = state;
    }


    public static bool IsPrefabInstance(this GameObject go)
    {
#if UNITY_EDITOR
        return UnityEditor.PrefabUtility.GetPrefabType(go) == UnityEditor.PrefabType.Prefab;
#else
		return false;
#endif
    }
}