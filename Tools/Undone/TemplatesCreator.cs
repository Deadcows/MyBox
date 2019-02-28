#if UNITY_EDITOR
using System.IO;
using UnityEngine;
using UnityEditor;

namespace MyBox.Internal
{
	public static class TemplatesCreator
	{
		private const string ComponentSystemTemplate =
			"using Unity.Entities;\n" +
			"\n" +
			"class ##ClassName## : ComponentSystem\n" +
			"{\n" +
			"#pragma warning disable 649\n" +
			"\tprivate struct Group\n" +
			"\t{\n" +
			"\t\tpublic readonly int Length;\n" +
			"\t}\n" +
			"\t[Inject] private Group _group;\n" +
			"#pragma warning restore 649\n" +
			"\t\n" +
			"\tprotected override void OnUpdate()\n" +
			"\t{\n" +
			"\t\tfor (int i = 0; i < _group.Length; i++)\n" +
			"\t\t{\n" +
			"\t\t\t\n" +
			"\t\t}\n" +
			"\t}\n" +
			"}\n";

		private const string ComponentTemplate =
			"using UnityEngine;\n" +
			"\n" +
			"public class ##ClassName## : MonoBehaviour\n" +
			"{\n" +
			"}\n";


		//[MenuItem("Assets/Create/ECS System", false, -100)]
		public static void CreateComponentSystemMenuItem()
		{
			CreateFromTemplate(ComponentSystemTemplate);
		}

		//[MenuItem("Assets/Create/ECS Component", false, -99)]
		public static void CreateComponentMenuItem()
		{
			CreateFromTemplate(ComponentTemplate);
		}


		private static void CreateFromTemplate(string template)
		{
			var processor = ScriptableObject.CreateInstance<TemplateProcessor>();
			processor.Template = template;
			var icon = EditorGUIUtility.IconContent("cs Script Icon").image as Texture2D;
			ProjectWindowUtil.StartNameEditingIfProjectWindowExists(0, processor, "NewSystem.cs", icon, "");
		}

		private class TemplateProcessor : UnityEditor.ProjectWindowCallback.EndNameEditAction
		{
			public string Template;

			public override void Action(int instanceId, string pathName, string resourceFile)
			{
				string baseFile = Path.GetFileNameWithoutExtension(pathName);
				string fullPath = Path.GetFullPath(pathName);

				var text = Template.Replace("##ClassName##", baseFile);
				File.WriteAllText(fullPath, text);

				AssetDatabase.ImportAsset(pathName);
				ProjectWindowUtil.ShowCreatedAsset(AssetDatabase.LoadAssetAtPath(pathName, typeof(Object)));
			}
		}
	
	}
}
#endif