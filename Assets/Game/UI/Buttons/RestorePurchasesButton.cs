using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RestorePurchasesButton : Button 
{
	protected override void OnPress()
	{
		AdManager.Instance.RestorePurchases();
	}
}
