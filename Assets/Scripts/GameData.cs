using Mirror;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameData : NetworkBehaviour
{
	[SerializeField] private int playerTurn = 0;
	public int playerCount = 0;
	public bool playerDrawn = false;
	public IList<int> pickCardPlayer = new List<int>();

	public bool inPickCardPeriod = false;
	[SerializeField] private float timer = 0f;

	private PlayerManager playerManager;

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

	public void ResetTimer()
	{
		timer = 0f;
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
		NetworkIdentity networkidentity = NetworkClient.connection.identity;
		playerManager = networkidentity.GetComponent<PlayerManager>();
		if (playerManager.playerId != 0)
			return;

		if (timer > 0f)
		{
			timer -= Time.deltaTime;
			SetPickCardIndicator($"Pick Card:\n{(int)timer} Seconds Left");
		}
		else if (inPickCardPeriod)
		{
			ExitPickCardPeriod();
			SetDrawDiscardCardIndicator();
		}
		else
		{
			SetDrawDiscardCardIndicator();
		}
	}

	private void SetPickCardIndicator(string str)
	{
		NetworkIdentity networkidentity = NetworkClient.connection.identity;
		playerManager = networkidentity.GetComponent<PlayerManager>();
		playerManager.CmdSetPickCardIndicator(str, pickCardPlayer.ToList());
	}
	
	private void SetDrawDiscardCardIndicator()
	{
		NetworkIdentity networkidentity = NetworkClient.connection.identity;
		playerManager = networkidentity.GetComponent<PlayerManager>();
		playerManager.CmdSetDrawDiscardCardIndicator(playerTurn);
	}
}
