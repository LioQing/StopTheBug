using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NumberIndicator : MonoBehaviour
{
	private FaceDownStack faceDownStack;
	private SpriteRenderer faceDownStackSprite;
	private Text text;

	private void Start()
	{
		text = GetComponent<Text>();
		faceDownStack = GameObject.Find("Face Down Stack").GetComponent<FaceDownStack>();
		faceDownStackSprite = GameObject.Find("Face Down Stack").GetComponent<SpriteRenderer>();
	}

	private void Update()
	{
		text.fontSize = (int)Camera.main.WorldToScreenPoint(faceDownStackSprite.transform.localScale).x / 16;
		transform.position = Camera.main.WorldToScreenPoint(new Vector3(faceDownStackSprite.transform.position.x + faceDownStackSprite.bounds.size.x / 1.2f, 0, 0));
		text.text = $"{faceDownStack.top + 1}";
	}
}
