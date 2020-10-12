using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PeriodIndicator : MonoBehaviour
{
	private SpriteRenderer faceDownStackSprite;
	private RectTransform canvasRect;
	private Text text;

	private void Start()
	{
		text = GetComponent<Text>();
		faceDownStackSprite = GameObject.Find("Face Down Stack").GetComponent<SpriteRenderer>();
		canvasRect = transform.parent.GetComponent<RectTransform>();
	}

	private void Update()
	{
		text.fontSize = (int)Camera.main.WorldToScreenPoint(faceDownStackSprite.transform.localScale).x / 32;
		transform.localPosition = new Vector3(0f, -canvasRect.rect.height / 6, 0f);
	}

	public void SetText(string str)
	{
		text.text = str;
	}
}
