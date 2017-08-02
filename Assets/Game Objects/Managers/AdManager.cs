using System;
using UnityEngine;
using GoogleMobileAds;
using GoogleMobileAds.Api;

public class AdManager : MonoBehaviour 
{
	public static AdManager Instance;

	#if UNITY_EDITOR
		const string bannerAdUnitID = "unused";
	#elif UNITY_ANDROID
		const string bannerAdUnitID = "ca-app-pub-3659931559103391/1881623548";
	#elif UNITY_IPHONE
		const string bannerAdUnitID = "ca-app-pub-3659931559103391/2531881309";
	#else
		const string bannerAdUnitID = "unexpected_platform";
	#endif


	#if UNITY_EDITOR
		const string interstitialAdUnitID = "unused";
	#elif UNITY_ANDROID
		const string interstitialAdUnitID = "ca-app-pub-3659931559103391/1690051859";
	#elif UNITY_IPHONE
		const string interstitialAdUnitID = "ca-app-pub-3659931559103391/3805789747";
	#else
		const string interstitialAdUnitID = "unexpected_platform";
	#endif

	public InterstitialAd Interstitial { get { return interstitial; } }
	private InterstitialAd interstitial;

	public BannerView Banner { get { return banner; } }
	private BannerView banner;

	void Awake()
	{
		if (Instance == null)
		{
			Instance = this;
		}

		RequestBanner();
		RequestInterstitial();
	}

	private void RequestBanner()
	{
		// Create a 320x50 banner at the top of the screen.
		banner = new BannerView(bannerAdUnitID, AdSize.Banner, AdPosition.Bottom);
		// Create an empty ad request.
		AdRequest request = new AdRequest.Builder().Build();
		// Load the banner with the request.
		banner.LoadAd(request);
		banner.Hide();
	}

	private void RequestInterstitial()
	{
		// Initialize an InterstitialAd.
		interstitial = new InterstitialAd(interstitialAdUnitID);
		interstitial.OnAdClosed += HandleOnAdClosed;
		// Create an empty ad request.
		AdRequest request = new AdRequest.Builder().Build();
		// Load the interstitial with the request.
		interstitial.LoadAd(request);
	}

	private void OnDestroy()
	{
		banner.Destroy();
		interstitial.Destroy();
	}

	public void HandleOnAdClosed(object sender, EventArgs args)
	{
		interstitial.OnAdClosed -= HandleOnAdClosed;
		RequestInterstitial();
	}
}
