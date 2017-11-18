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
