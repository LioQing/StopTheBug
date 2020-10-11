using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Mirror.Examples.Additive;

public class PlayerManager : NetworkBehaviour
{
	// synced var:
	// 1. handCards[0] - card drawn
	// 2. faceUpCard.stack
	// 3. faceDownStack.stack
	// 4. faceDownStack.top

	// server only data:
	// 1. gameData

	public HandCard[] handCards = new HandCard[5];
	public FaceDownStack faceDownStack = null;
	public FaceUpCard faceUpCard = null;
	public GameData gameData = null;
	public int playerId;

	private bool clientSynced = false;
	private bool serverStarted = false;

	public override void OnStartClient()
	{
		base.OnStartClient();

		var handCardsParent = GameObject.Find("Hand Cards").transform;
		for (var i = 0; i < 5; ++i)
		{
			handCards[i] = handCardsParent.GetChild(i).GetComponent<HandCard>();
		}

		faceDownStack = GameObject.Find("Face Down Stack").GetComponent<FaceDownStack>();
		faceUpCard = GameObject.Find("Face Up Stack").transform.GetChild(0).GetComponent<FaceUpCard>();
	}

	[Server]
	public override void OnStartServer()
	{
		base.OnStartServer();

		gameData = GameObject.Find("Game Data").GetComponent<GameData>();
	}

	public override void OnStopClient()
	{
		base.OnStopClient();

		foreach (var card in handCards)
		{
			Destroy(card.gameObject);
		}

		Destroy(faceUpCard.gameObject);
	}

	[Server]
	public override void OnStopServer()
	{
		base.OnStopServer();

		Destroy(gameData.gameObject);
	}



	private void Update()
	{
		if (!serverStarted && isServer)
		{
			faceDownStack.stack.Clear();
			for (var i = 1; i <= 12; ++i)
			{
				for (var j = 0; j < 4; ++j)
					faceDownStack.stack.Add(i);
			}
			faceDownStack.top = faceDownStack.stack.Count - 1;

			faceUpCard.stack.Clear();

			gameData.SetPlayerDrawn(false);
			gameData.SetPlayerTurn(0);

			serverStarted = true;
		}

		if (!clientSynced && hasAuthority)
		{
			CmdAddPlayer();
			CmdClientSync();
			for (var i = 1; i <= 4; ++i)
			{
				handCards[i].SetValue(0);
			}

			clientSynced = true;
		}
	}




	[Command]
	public void CmdAddPlayer()
	{
		RpcTargetPlayerId(connectionToClient, gameData.playerCount);
		gameData.SetPlayerCount(gameData.playerCount + 1);
	}
	[TargetRpc]
	private void RpcTargetPlayerId(NetworkConnection target, int id)
	{
		playerId = id;
	}





	[Command]
	public void CmdClientSync()
	{
		RpcClientSync(handCards[0].value, (List<int>)faceUpCard.stack, (List<int>)faceDownStack.stack, faceDownStack.top, faceUpCard.value);
	}
	[ClientRpc]
	private void RpcClientSync(int order0Val, List<int> faceUp, List<int> faceDown, int faceDownTop, int faceUpCardVal)
	{
		handCards[0].SetValue(order0Val);
		faceUpCard.stack = faceUp;
		faceDownStack.stack = faceDown;
		faceDownStack.top = faceDownTop;
		faceUpCard.SetValue(faceUpCardVal);
		faceDownStack.SetQuarter(Mathf.FloorToInt(faceDownTop / 48f * 4f) + 1);
	}





	[Command]
	public void CmdDrawCard(int id)
	{
		if (handCards[0].value >= 0 && handCards[0].value <= 12 || faceDownStack.top < 0 || id != gameData.playerTurn)
			return;

		if (gameData.playerDrawn)
			return;

		gameData.SetPlayerDrawn(true);
		var tmp = faceDownStack.top--;
		RpcDrawCard(faceDownStack.stack[tmp], faceDownStack.top);
	}
	[ClientRpc]
	private void RpcDrawCard(int val, int top)
	{
		if (hasAuthority)
			handCards[0].SetValue(val);
		else
			handCards[0].SetValue(0);

		faceDownStack.SetQuarter(Mathf.FloorToInt(top / 48f * 4f) + 1);
		faceDownStack.top = top;
	}



	[Command]
	public void CmdSwapHandCard(int id, int order1, int order2, int order1Val, int order2Val)
	{
		if ((order1 == 0 || order2 == 0) && id != gameData.playerTurn)
		{
			RpcTargetRejectSwapCard(connectionToClient, order1, order2, order1Val, order2Val);
			return;
		}

		if (order1 == 0)
			RpcSwapHandCard(order1, order2, true, order2Val);
		else if (order2 == 0)
			RpcSwapHandCard(order1, order2, true, order1Val);
		else
			RpcSwapHandCard(order1, order2, false, -1);
	}
	[ClientRpc]
	private void RpcSwapHandCard(int order1, int order2, bool order0Changed, int order0Val)
	{
		if (!hasAuthority)
		{
			if (order0Changed)
			{
				if (order0Val >= 0 && order0Val <= 12)
					handCards[0].SetValue(0);
				else
					handCards[0].SetValue(order0Val);
			}
		}
	}
	[TargetRpc]
	private void RpcTargetRejectSwapCard(NetworkConnection target, int order1, int order2, int order1Val, int order2Val)
	{
		handCards[order1].SetValue(order1Val);
		handCards[order2].SetValue(order2Val);
	}



	[Command]
	public void CmdDiscardCard(int id, int order, int handCardVal)
	{
		if (id != gameData.playerTurn)
		{
			RpcTargetRejectDiscard(connectionToClient, order, handCardVal);

			return;
		}

		gameData.SetPlayerTurn((id + 1) % gameData.playerCount);
		RpcDiscardCard(order, handCardVal, gameData.playerDrawn);
		gameData.SetPlayerDrawn(false);
	}
	[ClientRpc]
	private void RpcDiscardCard(int order, int val, bool drawn)
	{
		faceUpCard.SetValue(val);

		if (hasAuthority)
		{
			handCards[order].SetValue(-1);

			if (!drawn && handCards[0].value >= 0 && handCards[0].value <= 12)
			{
				CmdForceDraw(order);
				for (var i = 1; i <= 4; ++i)
				{
					if (i == order || handCards[i].value >= 0 && handCards[i].value <= 12)
						continue;

					handCards[i].SetValue(handCards[0].value);
					handCards[0].SetValue(-1);
				}
			}
			else if (handCards[0].value >= 0 && handCards[0].value <= 12)
			{
				handCards[order].SetValue(handCards[0].value);
				handCards[0].SetValue(-1);
			}
			else
			{
				CmdForceDraw(order);
			}
		}
		else
		{
			handCards[0].SetValue(-1);
		}
	}
	[TargetRpc]
	private void RpcTargetRejectDiscard(NetworkConnection target, int order, int val)
	{
		handCards[order].SetValue(val);
	}
	[Command]
	private void CmdForceDraw(int order)
	{
		var tmp = faceDownStack.top--;
		RpcForceDraw(order, faceDownStack.stack[tmp], faceDownStack.top);
	}
	[ClientRpc]
	private void RpcForceDraw(int order, int val, int top)
	{
		if (hasAuthority)
			handCards[order].SetValue(val);
		else
			handCards[0].SetValue(-1);

		faceDownStack.SetQuarter(Mathf.FloorToInt(top / 48f * 4f) + 1);
		faceDownStack.top = top;
	}
}
