using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuButton : Button 
{
	protected override void OnEnable()
	{
		GameManager.Instance.IntroduceFromSide(this.gameObject, 1.6f, false);
	}

	protected override void OnPress()
	{
		
	}
}
