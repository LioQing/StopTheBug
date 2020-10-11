using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameData : NetworkBehaviour
{
	public int playerTurn = 0;
	public int playerCount = 0;
	public bool playerDrawn = false;

	public void SetPlayerTurn(int player)
	{
		playerTurn = player;
	}
	
	public void SetPlayerCount(int count)
	{
		playerCount = count;
	}

	public void SetPlayerDrawn(bool drawn)
	{
		playerDrawn = drawn;
	}
}
