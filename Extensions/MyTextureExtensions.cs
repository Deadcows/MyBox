using UnityEngine;

public static class MyTextureExtensions
{
	public static Sprite AsSprite(this Texture2D texture)
	{
		var rect = new Rect(0, 0, texture.width, texture.height);
		var pivot = new Vector2(0.5f, 0.5f);
		return Sprite.Create(texture, rect, pivot);
	}
}
