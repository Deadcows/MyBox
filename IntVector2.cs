using UnityEngine;

[System.Serializable]
public struct IntVector2
{

	public static IntVector2 Zero = new IntVector2(0, 0);
	public static IntVector2 Negative1 = new IntVector2(-1, -1);
	public static IntVector2 One = new IntVector2(1, 1);

	public static IntVector2 North = new IntVector2(0, 1);
	public static IntVector2 South = new IntVector2(0, -1);
	public static IntVector2 East = new IntVector2(1, 0);
	public static IntVector2 West = new IntVector2(-1, 0);
	public static IntVector2 Northeast = new IntVector2(1, 1);
	public static IntVector2 Northwest = new IntVector2(-1, 1);
	public static IntVector2 Southeast = new IntVector2(1, -1);
	public static IntVector2 Southwest = new IntVector2(-1, -1);

	public int X;
	public int Y;

	public IntVector2(int x1, int y1)
	{
		X = x1;
		Y = y1;
	}

	public IntVector2 Right()
	{
		return new IntVector2(X + 1, Y);
	}

	public IntVector2 Left()
	{
		return new IntVector2(X - 1, Y);
	}

	public IntVector2 Up()
	{
		return new IntVector2(X, Y + 1);
	}

	public IntVector2 Down()
	{
		return new IntVector2(X, Y - 1);
	}

	public IntVector2 UpRight()
	{
		return new IntVector2(X + 1, Y + 1);
	}

	public IntVector2 UpLeft()
	{
		return new IntVector2(X - 1, Y + 1);
	}

	public IntVector2 DownRight()
	{
		return new IntVector2(X + 1, Y - 1);
	}

	public IntVector2 DownLeft()
	{
		return new IntVector2(X - 1, Y - 1);
	}

	public override string ToString()
	{
		return "[" + X + "," + Y + "]";
	}

	public static explicit operator IntVector2(Vector2 v)
	{
		return new IntVector2((int)v.x, (int)v.y);
	}

	public override bool Equals(object obj)
	{
		return obj is IntVector2 && this == (IntVector2)obj;
	}

	public override int GetHashCode()
	{
		return X.GetHashCode() ^ Y.GetHashCode();
	}
	public static bool operator ==(IntVector2 a, IntVector2 b)
	{
		return a.X == b.X && a.Y == b.Y;
	}
	public static bool operator !=(IntVector2 a, IntVector2 b)
	{
		return !(a == b);
	}

}