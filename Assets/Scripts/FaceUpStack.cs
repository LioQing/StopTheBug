using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FaceUpStack : MonoBehaviour
{
	private SpriteRenderer spriteRenderer;

	private void Start()
	{
		spriteRenderer = GetComponent<SpriteRenderer>();
	}

	private void Update()
	{
		var width = ScreenSize.GetScreenToWorldWidth;

		transform.localScale = Vector3.one * ScreenSize.GetScreenToWorldSmaller / 1.2f;
		transform.localPosition = new Vector3(-width / 4f, 6 * spriteRenderer.bounds.size.y / 48f, 0f);
	}
}
