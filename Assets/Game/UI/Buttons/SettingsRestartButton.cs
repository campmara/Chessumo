using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingsRestartButton : Button 
{
    private int numTimesReset = 0;

    protected override void OnPress()
	{
		numTimesReset++;
		if (numTimesReset >= 4)
		{
            AdManager.Instance.ShowVideoAd();
			numTimesReset = 0;
		}

        GameManager.Instance.BeginGame();
	}
}
