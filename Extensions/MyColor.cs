using UnityEngine;
using UnityEngine.UI;

namespace MyBox
{
	public static class MyColor
	{
		public static Color RandomBright => new Color(Random.Range(.4f, 1), Random.Range(.4f, 1), Random.Range(.4f, 1));

		public static Color RandomDim => new Color(Random.Range(.4f, .6f), Random.Range(.4f, .8f), Random.Range(.4f, .8f));

		public static Color RandomColor => new Color(Random.Range(.1f, .9f), Random.Range(.1f, .9f), Random.Range(.1f, .9f));

		/// <summary>
		/// Returns new Color with Alpha set to a
		/// </summary>
		public static Color WithAlphaSetTo(this Color color, float a) => new Color(color.r, color.g, color.b, a);

		/// <summary>
		/// Set Alpha of Image.Color
		/// </summary>
		public static void SetAlpha(this Graphic graphic, float a) => graphic.color = graphic.color.WithAlphaSetTo(a);

		/// <summary>
		/// Set Alpha of Renderer.Color
		/// </summary>
		public static void SetAlpha(this SpriteRenderer renderer, float a) => renderer.color = renderer.color.WithAlphaSetTo(a);

		/// <summary>
		/// To string of "#b5ff4f" format
		/// </summary>
		public static string ToHex(this Color color) => $"#{(int)(color.r * 255):X2}{(int)(color.g * 255):X2}{(int)(color.b * 255):X2}";

		private const float LightOffset = 0.0625f;

		/// <summary>
		/// Returns a color lighter than the given color.
		/// </summary>
		public static Color Lighter(this Color color) => color.BrightnessOffset(LightOffset);

		/// <summary>
		/// Returns a color darker than the given color.
		/// </summary>
		public static Color Darker(this Color color) => color.BrightnessOffset(-LightOffset);

		/// <summary>
		/// Brightness offset with 1 is brightest and -1 is darkest
		/// </summary>
		public static Color BrightnessOffset(this Color color, float offset) 
			=> new Color(color.r + offset, color.g + offset, color.b + offset, color.a);

		/// <summary>
		/// Converts a HTML color string into UnityEngine.Color. 
		/// See UnityEngine.ColorUtility.TryParseHtmlString for conversion conditions.
		/// </summary>
		public static Color ToUnityColor(this string source)
		{
			ColorUtility.TryParseHtmlString(source, out Color res);
			return res;
		}
	}
}