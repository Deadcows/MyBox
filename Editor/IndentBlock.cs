using UnityEditor;
using System;

namespace DeadcowBox
{
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
}
