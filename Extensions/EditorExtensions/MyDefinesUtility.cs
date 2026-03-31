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
#if UNITY_2023_1_OR_NEWER
				var gr = UnityEditor.Build.NamedBuildTarget.FromBuildTargetGroup(group);
#else
				var gr = group;
#endif
				var defines = GetDefines(gr);

				if (!defines.Contains(define))
				{
					defines.Add(define);
					SetDefines(gr, defines);
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
#if UNITY_2023_1_OR_NEWER
				var gr = UnityEditor.Build.NamedBuildTarget.FromBuildTargetGroup(group);
#else
				var gr = group;
#endif
				var defines = GetDefines(gr);

				if (defines.Contains(define))
				{
					defines.Remove(define);
					SetDefines(gr, defines);
					removed = true;
				}
			}

			return removed;
		}
		
#if UNITY_2023_1_OR_NEWER
		private static List<string> GetDefines(UnityEditor.Build.NamedBuildTarget namedBuildTarget)
		{
				PlayerSettings.GetScriptingDefineSymbols(namedBuildTarget, out var definesCol);
				return definesCol.ToList();
		}
#else
		private static List<string> GetDefines(BuildTargetGroup group)
		{
				return PlayerSettings.GetScriptingDefineSymbolsForGroup(group).Split(';').Select(d => d.Trim())
					.ToList();
		}
#endif

#if UNITY_2023_1_OR_NEWER
		private static void SetDefines(UnityEditor.Build.NamedBuildTarget namedBuildTarget, List<string> defines)
		{
			PlayerSettings.SetScriptingDefineSymbols(namedBuildTarget, defines.ToArray());
		}
#else
		private static void SetDefines(BuildTargetGroup group, List<string> defines)
		{
			PlayerSettings.SetScriptingDefineSymbolsForGroup(group, string.Join(";", defines.ToArray()));
		}
#endif
	}
}
#endif
