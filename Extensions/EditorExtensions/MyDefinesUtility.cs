#if UNITY_EDITOR
namespace MyBox.EditorTools
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Reflection;
	using JetBrains.Annotations;
	using UnityEditor;

	/// <summary>
	/// Basically a copy-paste of Unity.VisualScripting.DefineUtility.
	/// Simplifies handling of PlayerSettings.Set(Get)ScriptingDefineSymbols
	/// </summary>
	[PublicAPI]
	public class MyDefinesUtility
	{
		private static IEnumerable<BuildTargetGroup> buildTargetGroups
		{
			get
			{
				return Enum.GetValues(typeof(BuildTargetGroup)).Cast<BuildTargetGroup>().Where
				(group =>
					group != BuildTargetGroup.Unknown &&
					typeof(BuildTargetGroup).GetField(group.ToString()).GetCustomAttribute(typeof(ObsoleteAttribute)) == null
				);
			}
		}

		public static bool AddDefine(string define)
		{
			var added = false;

			foreach (var group in buildTargetGroups)
			{
				var defines = PlayerSettings.GetScriptingDefineSymbolsForGroup(group).Split(';').Select(d => d.Trim())
					.ToList();

				if (!defines.Contains(define))
				{
					defines.Add(define);
					PlayerSettings.SetScriptingDefineSymbolsForGroup(group, string.Join(";", defines.ToArray()));
					added = true;
				}
			}

			return added;
		}

		public static bool RemoveDefine(string define)
		{
			var removed = false;

			foreach (var group in buildTargetGroups)
			{
				var defines = PlayerSettings.GetScriptingDefineSymbolsForGroup(group).Split(';').Select(d => d.Trim())
					.ToList();

				if (defines.Contains(define))
				{
					defines.Remove(define);
					PlayerSettings.SetScriptingDefineSymbolsForGroup(group, string.Join(";", defines.ToArray()));
					removed = true;
				}
			}

			return removed;
		}
	}
}
#endif