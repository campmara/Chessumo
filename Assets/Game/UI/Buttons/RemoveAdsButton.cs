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
		}

		AdManager.Instance.OnAdsRemoved += OnUserPaidToRemoveAds;
	}

	protected override void OnPress()
	{
		if (SaveDataManager.Instance.HasPaidToRemoveAds())
		{
			spriteRenderer.color = disabledColor;
		}
		else
		{
			AdManager.Instance.BuyRemoveAds();
		}
	}

	private void OnUserPaidToRemoveAds()
	{
		spriteRenderer.color = disabledColor;
	}
}
