using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FaceDownStack : MonoBehaviour
{
	public int quarter;
	public Sprite[] sprites = new Sprite[4];
	public IList<int> stack = new List<int>();
	public int top;

	private SpriteRenderer spriteRenderer;
	private BoxCollider2D collider;
	private PlayerManager playerManager;

	public void SetQuarter(int quarter)
	{
		this.quarter = quarter;
		if (quarter <= 0)
			spriteRenderer.sprite = null;
		else
			spriteRenderer.sprite = sprites[quarter - 1];
	}

	private void Start()
	{
		spriteRenderer = GetComponent<SpriteRenderer>();
		collider = GetComponent<BoxCollider2D>();
	}

	private void Update()
	{
		var width = ScreenSize.GetScreenToWorldWidth;

		transform.localScale = Vector3.one * ScreenSize.GetScreenToWorldSmaller / 1.2f;
		transform.localPosition = new Vector3(width / 4f, 0f, 0f);

		collider.size = new Vector2(
			spriteRenderer.bounds.size.x / transform.localScale.x,
			spriteRenderer.bounds.size.y / transform.localScale.y);
	}

	private void OnMouseDown()
	{
		NetworkIdentity networkidentity = NetworkClient.connection.identity;
		playerManager = networkidentity.GetComponent<PlayerManager>();
		playerManager.CmdDrawCard(playerManager.playerId);
	}
}
