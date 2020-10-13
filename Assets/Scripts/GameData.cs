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

	public int discarder = -1;
	public bool inPickCardStreak = false;
	public bool inPickCardPeriod = false;
	public float timer = 0f;
	private int tmpPlayerTurn;

	private PlayerManager playerManager;

	public void EnterPickCardPeriod(int nextPlayer, float startTimer, int discarder)
	{
		this.discarder = discarder;
		tmpPlayerTurn = nextPlayer;
		inPickCardPeriod = true;
		timer = startTimer;
	}

	public void EnterPickCardPeriodInStreak(float startTimer, int discarder)
	{
		this.discarder = discarder;
		inPickCardPeriod = true;
		timer = startTimer;
	}

	public void ExitPickCardPeriod()
	{
		NetworkIdentity networkidentity = NetworkClient.connection.identity;
		playerManager = networkidentity.GetComponent<PlayerManager>();

		if (pickCardPlayer.Count == 1)
		{
			inPickCardStreak = true;
			playerManager.CmdPlayerPickCard(pickCardPlayer[0]);
			playerTurn = pickCardPlayer[0];
		}
		else
		{
			playerTurn = tmpPlayerTurn;
			inPickCardStreak = false;
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
		tmpPlayerTurn = player;
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
