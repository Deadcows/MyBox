using UnityEngine;

namespace MyBox
{
	public class SeparatorAttribute : PropertyAttribute
	{
		public readonly string Title;
		public readonly bool WithOffset;


		public SeparatorAttribute()
		{
			Title = "";
		}

		public SeparatorAttribute(string title, bool withOffset = false)
		{
			Title = title;
			WithOffset = withOffset;
		}
	}
}

#if UNITY_EDITOR
namespace MyBox.Internal
{
	using UnityEditor;
	
	[CustomPropertyDrawer(typeof(SeparatorAttribute))]
	public class SeparatorAttributeDrawer : DecoratorDrawer
	{
		private SeparatorAttribute Separator => (SeparatorAttribute) attribute;
		
		public override float GetHeight() => Separator.WithOffset ? 40 : Separator.Title.IsNullOrEmpty() ? 28 : 32;
		
		public override void OnGUI(Rect position)
		{
			var title = Separator.Title;
			if (title.IsNullOrEmpty())
			{
				position.height = 1;
				position.y += 14;
				GUI.Box(position, string.Empty);
			}
			else
			{
				Vector2 textSize = GUI.skin.label.CalcSize(new GUIContent(title));
				float separatorWidth = (position.width - textSize.x) / 2 - 5;
				position.y += 19;

				GUI.Box(new Rect(position.xMin, position.yMin, separatorWidth, 1), string.Empty);
				GUI.Label(new Rect(position.xMin + separatorWidth + 5, position.yMin - 10, textSize.x, 20), title);
				GUI.Box(new Rect(position.xMin + separatorWidth + 10 + textSize.x, position.yMin, separatorWidth, 1), "");
			}
		}
	}
}
#endif