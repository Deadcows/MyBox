using System.Text;
using UnityEngine;

public class MyDebug
{

	public static void LogArray<T>(T[] toLog)
	{
		StringBuilder sb = new StringBuilder();
		sb.Append("Log Array: ");
		sb.Append(typeof (T).Name + " (" + toLog.Length + ")\n");
		for (var i = 0; i < toLog.Length; i++)
		{
			sb.Append("\n" + i + ": " + toLog[i].ToString());
		}
		Debug.Log(sb.ToString());
	}
}