using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

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
	[CustomPropertyDrawer(typeof(SeparatorAttribute))]
	public class SeparatorAttributeDrawer : DecoratorDrawer
	{
		private SeparatorAttribute separatorAttribute
		{
			get { return ((SeparatorAttribute) attribute); }
		}

		public override void OnGUI(Rect _position)
		{
			if (separatorAttribute.Title == "")
			{
				_position.height = 1;
				_position.y += 19;
				GUI.Box(_position, "");
			}
			else
			{
				Vector2 textSize = GUI.skin.label.CalcSize(new GUIContent(separatorAttribute.Title));
				float separatorWidth = (_position.width - textSize.x) / 2.0f - 5.0f;
				_position.y += 19;

				GUI.Box(new Rect(_position.xMin, _position.yMin, separatorWidth, 1), "");
				GUI.Label(new Rect(_position.xMin + separatorWidth + 5.0f, _position.yMin - 8.0f, textSize.x, 20), separatorAttribute.Title);
				GUI.Box(new Rect(_position.xMin + separatorWidth + 10.0f + textSize.x, _position.yMin, separatorWidth, 1), "");
			}
		}

		public override float GetHeight()
		{
			return separatorAttribute.WithOffset ? 36.0f : 26f;
		}
	}
}
#endif