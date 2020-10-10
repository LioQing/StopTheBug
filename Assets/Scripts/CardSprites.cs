using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardSprites : MonoBehaviour
{
	[SerializeField] private Sprite[] card_sprites = new Sprite[13];

	public Sprite GetSprite(char id = ' ')
	{
		if (id == 'A') return card_sprites[1];
		else if (id == 'J') return card_sprites[10];
		else if (id == 'Q') return card_sprites[11];
		else if (id == 'K') return card_sprites[12];
		else if (char.IsDigit(id)) return card_sprites[int.Parse(id.ToString())];
		else return card_sprites[0];
	}

	public Sprite GetSprite(int index)
	{
		return card_sprites[index];
	}
}
