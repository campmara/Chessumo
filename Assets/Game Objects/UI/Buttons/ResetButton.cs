using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetButton : Button 
{
	protected override void OnPress()
	{
		GameManager.Instance.BeginGame();
	}
}
