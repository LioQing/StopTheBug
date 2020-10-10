using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PlayerManager : NetworkBehaviour
{
	public HandCard[] handCards = new HandCard[5];
	public FaceDownStack faceDownStack;
	public FaceUpCard faceUpCard;

	private IList<int> faceUpStackData = new List<int>();
	private IList<int> faceDownStackData = new List<int>();
	private int faceDownStackTop;

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

		faceDownStack.SetQuarter((int)(faceDownStackTop / 48f * 4f) + 1);
	}

	[Server]
	public override void OnStartServer()
	{
		base.OnStartServer();

		faceDownStackData.Clear();
		for (var i = 1; i <= 12; ++i)
		{
			for (var j = 0; j < 4; ++j)
				faceDownStackData.Add(i);
		}
		faceDownStackTop = faceDownStackData.Count - 1;

		faceUpStackData.Clear();
	}

	[Command]
	public void CmdDrawCard()
	{
		if (handCards[0].value >= 0 && handCards[0].value <= 12 || faceDownStackTop < 0)
			return;

		var tmp = faceDownStackTop--;
		RpcDrawCard(faceDownStackData[tmp], Mathf.FloorToInt(faceDownStackTop / 48f * 4f) + 1);
	}
	[ClientRpc]
	private void RpcDrawCard(int val, int stackQuarter)
	{
		handCards[0].SetValue(val);
		faceDownStack.SetQuarter(stackQuarter);
	}

	[Command]
	public void CmdSwapHandCard(int order1, int order2)
	{
		if (order1 == 0)
			RpcSwapHandCard(order1, order2, true, handCards[order2].value);
		else if (order2 == 0)
			RpcSwapHandCard(order1, order2, true, handCards[order1].value);
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
	public void CmdPutCardOnStack(int order)
	{
		RpcCardPutOnStack(order, handCards[order].value);
	}
	[ClientRpc]
	private void RpcCardPutOnStack(int order, int val)
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
