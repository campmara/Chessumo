using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RestartButton : Button 
{
	protected override void OnEnable()
	{
		//GameManager.Instance.IntroduceFromSide(this.gameObject, 1.7f, false);
	}

	protected override void OnPress()
	{
		GameManager.Instance.BeginGame();
		text.text = "RESTART";
	}

	public void ShowStart()
	{
		text.text = "START";
	}

	public void ShowReplay()
	{
		text.text = "PLAY AGAIN";
	}
}
