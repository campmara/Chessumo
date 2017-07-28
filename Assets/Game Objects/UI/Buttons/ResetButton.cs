using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetButton : Button 
{
	protected override void OnEnable()
	{
		//GameManager.Instance.IntroduceFromSide(this.gameObject, 1.7f, false);
	}

	protected override void OnPress()
	{
		GameManager.Instance.BeginGame();
	}
}
