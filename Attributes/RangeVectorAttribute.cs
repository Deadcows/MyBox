using System;
using UnityEditor;
using UnityEngine;

namespace MyBox
{
  [AttributeUsage(AttributeTargets.Field)]
  public sealed class RangeVectorAttribute : PropertyAttribute
  {
    #region fields

    public readonly Vector3 min = Vector3.zero;
    public readonly Vector3 max = Vector3.zero;

    #endregion

    #region constructors

    /// <summary>
    ///   <para>Attribute used to make a Vector variable in a script be restricted to a specific range.</para>
    /// </summary>
    /// <param name="min">The array with the minimum allowed values.</param>
    /// <param name="max">The array with the maximum allowed values.</param>
    public RangeVectorAttribute(float[] min, float[] max)
    {
      if (min.Length > 3 || max.Length > 3)
      {
        throw new ArgumentException("min and max must be of length 3 or less");
      }

      switch (min.Length)
      {
        case 3:
          this.min.x = min[0];
          this.min.y = min[1];
          this.min.z = min[2];
          break;
        case 2:
          this.min.x = min[0];
          this.min.y = min[1];
          break;
        case 1:
          this.min.x = min[0];
          break;
      }

      switch (max.Length)
      {
        case 3:
          this.max.x = max[0];
          this.max.y = max[1];
          this.max.z = max[2];
          break;
        case 2:
          this.max.x = max[0];
          this.max.y = max[1];
          break;
        case 1:
          this.max.x = max[0];
          break;
      }
    }

    #endregion
  }
}