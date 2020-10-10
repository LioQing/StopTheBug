using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandCard : NetworkBehaviour
{
	public int value = -1;
	public int order = 0;

	private PlayerManager playerManager;
	private CardSprites cardSprites;
	private Collider2D faceUpCollider;
	private SpriteRenderer spriteRenderer;
	private Vector3 lockedPosition;
	private bool isDragging = false;
	private bool isOnStack = false;

	public void SetValue(int val)
	{
		value = val;
		if (value < 0 || value > 12)
			spriteRenderer.sprite = null;
		else
			spriteRenderer.sprite = cardSprites.GetSprite(value);
	}

	private void Start()
	{
		cardSprites = GameObject.Find("Card Sprites").GetComponent<CardSprites>();
		faceUpCollider = GameObject.Find("Face Up Stack").GetComponent<Collider2D>();
		order = transform.GetSiblingIndex();
		spriteRenderer = GetComponent<SpriteRenderer>();
		SetValue(-1);
	}

	private void Update()
	{
		var width = ScreenSize.GetScreenToWorldWidth;
		var height = ScreenSize.GetScreenToWorldHeight;
		var smaller_size = ScreenSize.GetScreenToWorldSmaller;

		transform.localScale = Vector3.one * smaller_size / 1.3f;
		if (!isDragging && order != 0)
			transform.localPosition = new Vector3(-width / 2f + order * width / 5f, -height / 2f + spriteRenderer.bounds.size.y / 2f, 0f);
		else if (!isDragging && order == 0)
			transform.localPosition = new Vector3(width / 4f, smaller_size / 4f, 0f);

		GetComponent<BoxCollider2D>().size = new Vector2(
			spriteRenderer.bounds.size.x / transform.localScale.x,
			spriteRenderer.bounds.size.y / transform.localScale.y);
	}

	private void OnMouseDown()
	{
		if (value < 0 || value > 12)
			return;

		lockedPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, transform.localPosition.z);
	}

	private void OnMouseDrag()
	{
		if (value < 0 || value > 12)
			return;

		isDragging = true;

		var mousePos = Input.mousePosition;
		mousePos.z = 10f;
		transform.localPosition = Camera.main.ScreenToWorldPoint(mousePos);
	}

	private void OnMouseUp()
	{
		if (value < 0 || value > 12)
			return;

		isDragging = false;
		NetworkIdentity networkIdentity = NetworkClient.connection.identity;
		playerManager = networkIdentity.GetComponent<PlayerManager>();

		if (isOnStack)
		{
			playerManager.CmdPutCardOnStack(order);

			return;
		}

		// get the closest card
		var closestDistance = float.MaxValue;
		Transform closestCard = transform;
		for (var i = 0; i < transform.parent.childCount; ++i)
		{
			var other_card = transform.parent.GetChild(i);

			float distance;
			if (other_card == transform)
			{
				distance = (lockedPosition - transform.localPosition).sqrMagnitude;
			}
			else
			{
				distance = (other_card.localPosition - transform.localPosition).sqrMagnitude;
			}

			if (distance < closestDistance)
			{
				closestDistance = distance;
				closestCard = other_card;
			}
		}

		// assign sprite
		if (closestCard == transform)
			return;

		playerManager.CmdSwapHandCard(order, closestCard.GetComponent<HandCard>().order);
	}

	private void OnTriggerEnter2D(Collider2D other)
	{
		if (value < 0 || value > 12)
			return;

		if (other == faceUpCollider)
		{
			isOnStack = true;
		}
	}

	private void OnTriggerExit2D(Collider2D other)
	{
		if (other == faceUpCollider)
		{
			isOnStack = false;
		}
	}
}
