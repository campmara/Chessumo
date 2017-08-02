using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RestartButton : Button 
{
	private bool gameStartedForTheFirstTime = false;
	private int numTimesReset = 0;

	protected override void OnEnable()
	{
		//GameManager.Instance.IntroduceFromSide(this.gameObject, 1.7f, false);
	}

	protected override void OnPress()
	{
		if (!gameStartedForTheFirstTime)
		{
			// Show the banner ad.
			AdManager.Instance.Banner.Show();
			gameStartedForTheFirstTime = true;
		}

		if (text.text == "RESTART")
		{
			numTimesReset++;
			if (numTimesReset >= 4)
			{
				AdManager.Instance.Interstitial.Show();
				numTimesReset = 0;
			}
		}
		else
		{
			text.text = "RESTART";
		}

		GameManager.Instance.BeginGame();
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
