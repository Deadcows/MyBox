using System.Text.RegularExpressions;
using UnityEngine;

public static class MyInput
{

	#region Get Number Pressed

	public static bool GetNumberDown(int num)
	{
		Debug.Assert(num <= 9 && num >= 0);

		switch (num)
		{
			case 0:
				if (Input.GetKeyDown(KeyCode.Alpha0) || Input.GetKeyDown(KeyCode.Keypad0)) return true;
				break;
			case 1:
				if (Input.GetKeyDown(KeyCode.Alpha1) || Input.GetKeyDown(KeyCode.Keypad1)) return true;
				break;
			case 2:
				if (Input.GetKeyDown(KeyCode.Alpha2) || Input.GetKeyDown(KeyCode.Keypad2)) return true;
				break;
			case 3:
				if (Input.GetKeyDown(KeyCode.Alpha3) || Input.GetKeyDown(KeyCode.Keypad3)) return true;
				break;
			case 4:
				if (Input.GetKeyDown(KeyCode.Alpha4) || Input.GetKeyDown(KeyCode.Keypad4)) return true;
				break;
			case 5:
				if (Input.GetKeyDown(KeyCode.Alpha5) || Input.GetKeyDown(KeyCode.Keypad5)) return true;
				break;
			case 6:
				if (Input.GetKeyDown(KeyCode.Alpha6) || Input.GetKeyDown(KeyCode.Keypad6)) return true;
				break;
			case 7:
				if (Input.GetKeyDown(KeyCode.Alpha7) || Input.GetKeyDown(KeyCode.Keypad7)) return true;
				break;
			case 8:
				if (Input.GetKeyDown(KeyCode.Alpha8) || Input.GetKeyDown(KeyCode.Keypad8)) return true;
				break;
			case 9:
				if (Input.GetKeyDown(KeyCode.Alpha9) || Input.GetKeyDown(KeyCode.Keypad9)) return true;
				break;
		}

		return false;
	}

	public static int GetNumberDown(KeyCode key)
	{
		if (key == KeyCode.Alpha0 || key == KeyCode.Keypad0) return 0;
		if (key == KeyCode.Alpha1 || key == KeyCode.Keypad1) return 1;
		if (key == KeyCode.Alpha2 || key == KeyCode.Keypad2) return 2;
		if (key == KeyCode.Alpha3 || key == KeyCode.Keypad3) return 3;
		if (key == KeyCode.Alpha4 || key == KeyCode.Keypad4) return 4;
		if (key == KeyCode.Alpha5 || key == KeyCode.Keypad5) return 5;
		if (key == KeyCode.Alpha6 || key == KeyCode.Keypad6) return 6;
		if (key == KeyCode.Alpha7 || key == KeyCode.Keypad7) return 7;
		if (key == KeyCode.Alpha8 || key == KeyCode.Keypad8) return 8;
		if (key == KeyCode.Alpha9 || key == KeyCode.Keypad9) return 9;

		return -1;
	}

	public static int GetNumberDown()
	{
		if (Input.GetKeyDown(KeyCode.Alpha0) || Input.GetKeyDown(KeyCode.Keypad0)) return 0;
		if (Input.GetKeyDown(KeyCode.Alpha1) || Input.GetKeyDown(KeyCode.Keypad1)) return 1;
		if (Input.GetKeyDown(KeyCode.Alpha2) || Input.GetKeyDown(KeyCode.Keypad2)) return 2;
		if (Input.GetKeyDown(KeyCode.Alpha3) || Input.GetKeyDown(KeyCode.Keypad3)) return 3;
		if (Input.GetKeyDown(KeyCode.Alpha4) || Input.GetKeyDown(KeyCode.Keypad4)) return 4;
		if (Input.GetKeyDown(KeyCode.Alpha5) || Input.GetKeyDown(KeyCode.Keypad5)) return 5;
		if (Input.GetKeyDown(KeyCode.Alpha6) || Input.GetKeyDown(KeyCode.Keypad6)) return 6;
		if (Input.GetKeyDown(KeyCode.Alpha7) || Input.GetKeyDown(KeyCode.Keypad7)) return 7;
		if (Input.GetKeyDown(KeyCode.Alpha8) || Input.GetKeyDown(KeyCode.Keypad8)) return 8;
		if (Input.GetKeyDown(KeyCode.Alpha9) || Input.GetKeyDown(KeyCode.Keypad9)) return 9;

		return -1;
	}

	#endregion


	/// <summary>
	/// Convert KeyCode to read-friendly format
	/// </summary>
	public static string ToReadableString(this KeyCode key, bool full = false)
	{
		switch (key)
		{
			case KeyCode.Mouse0:
				return full ? "Left Mouse Button" : "LMB";
			case KeyCode.Mouse1:
				return full ? "Right Mouse Button" : "RMB";
			case KeyCode.Mouse2:
				return full ? "Middle Mouse Button" : "MMB";

			default:
				return Regex.Replace(key.ToString(), "(\\B[A-Z])", " $1");
		}
	}
	
}
