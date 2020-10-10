using UnityEngine;

public class ScreenSize
{
	public static float GetScreenToWorldHeight
	{
		get
		{
			Vector2 top_right_corner = new Vector2(1, 1);
			Vector2 edge_vector = Camera.main.ViewportToWorldPoint(top_right_corner);
			var height = edge_vector.y * 2;
			return height;
		}
	}
	public static float GetScreenToWorldWidth
	{
		get
		{
			Vector2 top_right_corner = new Vector2(1, 1);
			Vector2 edge_vector = Camera.main.ViewportToWorldPoint(top_right_corner);
			var width = edge_vector.x * 2;
			return width;
		}
	}
	public static float GetScreenToWorldSmaller
	{
		get
		{
			return GetScreenToWorldWidth > GetScreenToWorldHeight ? GetScreenToWorldHeight : GetScreenToWorldWidth;
		}
	}
}