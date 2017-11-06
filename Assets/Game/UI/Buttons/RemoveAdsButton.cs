using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RemoveAdsButton : MonoBehaviour
{
	private Button button;

	void Awake()
	{
		button = GetComponent(typeof(Button)) as Button;

		if (SaveDataManager.Instance.HasPaidToRemoveAds())
		{
			button.interactable = false;
		}

		AdManager.Instance.OnAdsRemoved += OnUserPaidToRemoveAds;
	}

	public void OnPress()
	{
		if (SaveDataManager.Instance.HasPaidToRemoveAds())
		{
			button.interactable = false;
		}
		else
		{
			AdManager.Instance.BuyRemoveAds();
		}
	}

	private void OnUserPaidToRemoveAds()
	{
		button.interactable = false;
	}
}
