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

	public HandCard[] handCards = new HandCard[5];
	public FaceDownStack faceDownStack;
	public FaceUpCard faceUpCard;
	public GameData gameData;
	public int playerId;
	
	private bool clientSynced = false;
	private bool serverStarted = false;

	public override void OnStartClient()
	{
		base.OnStartClient();

		var handCardsParent = GameObject.Find("Hand Cards").transform;
		for (var i = 0; i < handCardsParent.childCount; ++i)
		{
			handCards[i] = handCardsParent.GetChild(i).GetComponent<HandCard>();
		}
		faceDownStack = GameObject.Find("Face Down Stack").GetComponent<FaceDownStack>();
		faceUpCard = GameObject.Find("Face Up Stack").transform.GetChild(0).GetComponent<FaceUpCard>();
		gameData = GameObject.Find("Game Data").GetComponent<GameData>();
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

			serverStarted = true;
		}

		if (!clientSynced && hasAuthority)
		{
			CmdAddPlayer();
			CmdClientSync();
			Debug.Log(playerId);

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
	public void CmdDrawCard()
	{
		if (handCards[0].value >= 0 && handCards[0].value <= 12 || faceDownStack.top < 0)
			return;

		var tmp = faceDownStack.top--;
		RpcDrawCard(faceDownStack.stack[tmp], faceDownStack.top);
	}
	[ClientRpc]
	private void RpcDrawCard(int val, int top)
	{
		handCards[0].SetValue(val);
		faceDownStack.SetQuarter(Mathf.FloorToInt(top / 48f * 4f) + 1);
		faceDownStack.top = top;
		Debug.Log(faceDownStack.top);
	}

	[Command]
	public void CmdSwapHandCard(int order1, int order2, int order1Val, int order2Val)
	{
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
		if (hasAuthority)
		{
			var tmp = handCards[order1].value;
			handCards[order1].SetValue(handCards[order2].value);
			handCards[order2].SetValue(tmp);
		}
		else
		{
			if (order0Changed)
				handCards[0].SetValue(order0Val);
		}
	}

	[Command]
	public void CmdDiscardCard(int id, int order, int handCardVal)
	{
		if (id != gameData.playerTurn)
			return;

		gameData.SetPlayerTurn((id + 1) % gameData.playerCount);
		RpcDiscardCard(order, handCardVal);
	}
	[ClientRpc]
	private void RpcDiscardCard(int order, int val)
	{
		faceUpCard.SetValue(val);

		if (hasAuthority)
		{
			handCards[order].SetValue(-1);
		}
		else
		{
			if (order == 0)
				handCards[0].SetValue(-1);
		}
	}
}
