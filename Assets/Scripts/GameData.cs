using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameData : NetworkBehaviour
{
	[SerializeField] private int playerTurn = 0;
	public int playerCount = 0;
	public bool playerDrawn = false;
	public IList<int> pickCardPlayer = new List<int>();

	public bool inPickCardPeriod = false;
	[SerializeField] private float timer = 0f;

	public void EnterPickCardPeriod(int nextPlayer, float startTimer)
	{
		playerTurn = nextPlayer;
		inPickCardPeriod = true;
		timer = startTimer;
	}

	public void ExitPickCardPeriod()
	{
		if (pickCardPlayer.Count == 1)
		{

		}

		inPickCardPeriod = false;
		pickCardPlayer.Clear();
	}

	public void SetPlayerTurn(int player)
	{
		playerTurn = player;
	}
	public int GetPlayerTurn()
	{
		if (inPickCardPeriod)
			return -1;
		else
			return playerTurn;
	}
	
	public void SetPlayerCount(int count)
	{
		playerCount = count;
	}

	public void SetPlayerDrawn(bool drawn)
	{
		playerDrawn = drawn;
	}

	private void Update()
	{
		if (timer > 0f)
			timer -= Time.deltaTime;
		else if (inPickCardPeriod)
			ExitPickCardPeriod();
	}
}
