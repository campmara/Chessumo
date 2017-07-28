using UnityEngine;
using UnityEngine.SocialPlatforms;

public class ScoresButton : Button 
{
	protected override void OnEnable()
	{
		//GameManager.Instance.IntroduceFromSide(this.gameObject, 1.5f, false);
	}

	protected override void OnPress()
	{
		Social.ShowLeaderboardUI();
	}
}
