using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemoveAdsButton : Button 
{
	[SerializeField] protected Color disabledColor;

	void Awake()
	{
		if (SaveDataManager.Instance.HasPaidToRemoveAds())
		{
			spriteRenderer.color = disabledColor;
			text.text = "ADS REMOVED";
		}

		AdManager.Instance.OnAdsRemoved += OnUserPaidToRemoveAds;
	}

	protected override void OnPress()
	{
		if (SaveDataManager.Instance.HasPaidToRemoveAds())
		{
			spriteRenderer.color = disabledColor;
			text.text = "ADS REMOVED";
		}
		else
		{
			AdManager.Instance.BuyRemoveAds();
		}
	}

	private void OnUserPaidToRemoveAds()
	{
		spriteRenderer.color = disabledColor;
		text.text = "ADS REMOVED";
	}
}
