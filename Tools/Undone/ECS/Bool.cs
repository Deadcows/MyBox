/*
using System;

/// <summary>
/// Blittable boolean representation
/// </summary>
public struct Bool
{
	private byte _value;

	public static implicit operator Bool(bool b)
	{
		return new Bool { _value = Convert.ToByte(b) };
	}

	public static implicit operator bool(Bool b)
	{
		return b._value == 1;
	}
}
*/