using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FaceUpStack : MonoBehaviour
{
	private SpriteRenderer spriteRenderer;
	private BoxCollider2D collider;

	private void Start()
	{
		spriteRenderer = GetComponent<SpriteRenderer>();
		collider = GetComponent<BoxCollider2D>();
	}

	private void Update()
	{
		var width = ScreenSize.GetScreenToWorldWidth;

		transform.localScale = Vector3.one * ScreenSize.GetScreenToWorldSmaller / 1.2f;
		transform.localPosition = new Vector3(-width / 6f, 6 * spriteRenderer.bounds.size.y / 48f, 0f);

		collider.size = new Vector2(
			spriteRenderer.bounds.size.x / transform.localScale.x,
			spriteRenderer.bounds.size.y / transform.localScale.y);
	}
}
