using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FaceUpCard : NetworkBehaviour
{
	public int value = -1;
	public int lastValue = -1;

	private PlayerManager playerManager;
	private SpriteRenderer spriteRenderer;
	private BoxCollider2D collider;
	private CardSprites cardSprites;

	public void SetValue(int val)
	{
		lastValue = value;
		value = val;
		
		if (value < 0 || value > 12)
			spriteRenderer.sprite = null;
		else
			spriteRenderer.sprite = cardSprites.GetSprite(value);
	}

	private void Start()
	{
		cardSprites = GameObject.Find("Card Sprites").GetComponent<CardSprites>();
		spriteRenderer = GetComponent<SpriteRenderer>();
		collider = GetComponent<BoxCollider2D>();
		SetValue(-1);
	}

	private void Update()
	{
		transform.position = new Vector3(transform.position.x, 0f, transform.position.z);

		collider.size = new Vector2(
			spriteRenderer.bounds.size.x / transform.parent.transform.localScale.x / transform.localScale.x,
			spriteRenderer.bounds.size.y / transform.parent.transform.localScale.y / transform.localScale.y);
	}

	private void OnMouseDown()
	{
		NetworkIdentity networkidentity = NetworkClient.connection.identity;
		playerManager = networkidentity.GetComponent<PlayerManager>();
		playerManager.CmdPickCard(playerManager.playerId);
	}
}
