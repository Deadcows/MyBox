using UnityEditor;
using System;

public class IndentBlock : IDisposable
{
	public IndentBlock()
	{
		EditorGUI.indentLevel++;
	}

	public void Dispose()
	{
		EditorGUI.indentLevel--;
	}
}