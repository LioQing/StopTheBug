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
		SetValue(-1);
	}

	private void Update()
	{
		transform.position = new Vector3(transform.position.x, 0f, transform.position.z);
	}

	private void OnMouseDown()
	{
		NetworkIdentity networkidentity = NetworkClient.connection.identity;
		playerManager = networkidentity.GetComponent<PlayerManager>();
		playerManager.CmdPickCard(playerManager.playerId);
	}
}
