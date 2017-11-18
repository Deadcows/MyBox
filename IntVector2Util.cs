public class IntVector2Util
{

	/**
	 * Takes the given array of vectors and adds the given offest to them.
	 * This is useful if you have a set of points, say a movement path, but
	 * need to apply it to the current unit's position.
	 */
	public static IntVector2[] Offset(IntVector2[] input, IntVector2 offset)
	{
		IntVector2[] retval = new IntVector2[input.Length];
		for (int i = 0; i < input.Length; i++)
		{
			IntVector2 pos = input[i];
			retval[i] = new IntVector2(offset.X + pos.X, offset.Y + pos.Y);
		}
		return retval;
	}

	public static IntVector2 Offset(IntVector2 pos, IntVector2 offset)
	{
		return new IntVector2(pos.X + offset.X, pos.Y + offset.Y);
	}

	public static IntVector2 xRot0 = new IntVector2(1, 0);
	public static IntVector2 yRot0 = new IntVector2(0, 1);

	public static IntVector2 xRot90 = new IntVector2(0, -1);
	public static IntVector2 yRot90 = new IntVector2(1, 0);

	public static IntVector2 xRot180 = new IntVector2(-1, 0);
	public static IntVector2 yRot180 = new IntVector2(0, -1);

	public static IntVector2 xRot270 = new IntVector2(0, 1);
	public static IntVector2 yRot270 = new IntVector2(-1, 0);

	/**
	 * Rotates in 90 degree increments and then translates the points with the 
	 * given offset.
	   */
	public static IntVector2[] Rotate(IntVector2[] input, int rotation, IntVector2 offset)
	{
		IntVector2 xRot = xRot0;
		IntVector2 yRot = yRot0;
		switch (rotation)
		{
			case 0:
				//default
				break;
			case 90:
			case -270:
				xRot = xRot90;
				yRot = yRot90;
				break;
			case 180:
				xRot = xRot180;
				yRot = yRot180;
				break;
			case 270:
			case -90:
				xRot = xRot270;
				yRot = yRot270;
				break;
			default:
				throw new System.Exception("Rotate() requires 90 degree rotations.");
		}

		IntVector2[] retval = new IntVector2[input.Length];
		for (int i = 0; i < input.Length; i++)
		{
			IntVector2 pos = input[i];
			int x = pos.X * xRot.X + pos.Y * xRot.Y;
			int y = pos.X * yRot.X + pos.Y * yRot.Y;
			retval[i] = new IntVector2(x + offset.X, y + offset.Y);
		}
		return retval;
	}

	public static IntVector2 GetDirection(IntVector2 start, IntVector2 end)
	{
		if (start.X == end.X)
		{
			//no east/west
			if (start.Y == end.Y)
			{
				return IntVector2.Zero;
			}
			else if (start.Y < end.Y)
			{
				return IntVector2.North;
			}
			else
			{
				return IntVector2.South;
			}

		}
		else if (start.X < end.X)
		{
			//east
			if (start.Y == end.Y)
			{
				return IntVector2.East;
			}
			else if (start.Y < end.Y)
			{
				return IntVector2.Northeast;
			}
			else
			{
				return IntVector2.Southeast;
			}

		}
		else
		{
			//west
			if (start.Y == end.Y)
			{
				return IntVector2.West;
			}
			else if (start.Y < end.Y)
			{
				return IntVector2.Northwest;
			}
			else
			{
				return IntVector2.Southwest;
			}
		}
	}

}
