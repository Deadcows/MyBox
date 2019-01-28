// ---------------------------------------------------------------------------- 
// Author: Richard Fine
// Source: https://bitbucket.org/richardfine/scriptableobjectdemo
// ----------------------------------------------------------------------------

using System;

public class MinMaxRangeAttribute : Attribute
{
	public MinMaxRangeAttribute(float min, float max)
	{
		Min = min;
		Max = max;
	}
	public float Min { get; private set; }
	public float Max { get; private set; }
}

[Serializable]
public struct RangedFloat
{
	public float Min;
	public float Max;
}