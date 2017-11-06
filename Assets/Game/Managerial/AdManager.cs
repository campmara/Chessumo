using System;
using UnityEngine;
using GoogleMobileAds;
using GoogleMobileAds.Api;

using UnityEngine.Advertisements;
using UnityEngine.Purchasing;

public class AdManager : MonoBehaviour, IStoreListener
{
	public static AdManager Instance;

	private static IStoreController m_StoreController;          // The Unity Purchasing system.
    private static IExtensionProvider m_StoreExtensionProvider; // The store-specific Purchasing subsystems.

	// remove ads product id for IAP
	public static string kProductIDRemoveAds =    "removeads";

#if UNITY_EDITOR
    	private const string GAME_ID = "unused";
#elif UNITY_ANDROID
        private const string GAME_ID = "1561516";
#elif UNITY_IPHONE
        private const string GAME_ID = "1561515";
#else
        private const string GAME_ID = "unexpected_platform";
#endif

	public delegate void AdsRemovedDelegate();
	public AdsRemovedDelegate OnAdsRemoved;

	public delegate void AdClosedDelegate();
	public AdClosedDelegate OnAdClosed;

	public delegate void AdShownDelegate();
	public AdShownDelegate OnAdShown;

	private void Awake()
	{
		if (Instance == null)
		{
			Instance = this;
		}

        Advertisement.Initialize(GAME_ID);

		// If we haven't set up the Unity Purchasing reference
		if (m_StoreController == null)
		{
			// Begin to configure our connection to Purchasing
			InitializePurchasing();
		}
	}

    public void ShowVideoAd()
    {
		// Don't show ads if user has paid to remove them.
		if (SaveDataManager.Instance.HasPaidToRemoveAds()) return;

		if (Advertisement.IsReady() && Advertisement.isInitialized)
		{
			OnAdShown();
			Advertisement.Show(new ShowOptions { resultCallback = AdClosedCallback });
		}
    }

	public void AdClosedCallback(ShowResult result)
	{
		OnAdClosed();
	}

	public void InitializePurchasing() 
	{
		if (IsIAPInitialized())
		{
			return;
		}
		
		var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());
		
		// Add a product to sell / restore by way of its identifier, associating the general identifier
		// with its store-specific identifiers.
		builder.AddProduct(kProductIDRemoveAds, ProductType.NonConsumable);
		
		// Kick off the remainder of the set-up with an asynchrounous call, passing the configuration 
		// and this class' instance. Expect a response either in OnInitialized or OnInitializeFailed.
		UnityPurchasing.Initialize(this, builder);
	}

	private bool IsIAPInitialized()
	{
		// Only say we are initialized if both the Purchasing references are set.
		return m_StoreController != null && m_StoreExtensionProvider != null;
	}

	public void BuyRemoveAds()
	{
		BuyProductID(kProductIDRemoveAds);
	}

	void BuyProductID(string productId)
	{
		if (IsIAPInitialized())
		{
			Product product = m_StoreController.products.WithID(productId);
			
			if (product != null && product.availableToPurchase)
			{
				Debug.Log(string.Format("Purchasing product asychronously: '{0}'", product.definition.id));
				m_StoreController.InitiatePurchase(product);
			}
			else
			{
				Debug.Log("BuyProductID: FAIL. Not purchasing product, either is not found or is not available for purchase");
			}
		}
		else
		{
			Debug.Log("BuyProductID FAIL. Not initialized.");
		}
	}

	public void RestorePurchases()
	{
		if (!IsIAPInitialized())
		{
			Debug.Log("RestorePurchases FAIL. Not initialized.");
			return;
		}
		
		if (Application.platform == RuntimePlatform.IPhonePlayer || 
			Application.platform == RuntimePlatform.OSXPlayer)
		{
			Debug.Log("RestorePurchases started ...");
			
			var apple = m_StoreExtensionProvider.GetExtension<IAppleExtensions>();
			apple.RestoreTransactions((result) => {
				Debug.Log("RestorePurchases continuing: " + result + ". If no further messages, no purchases available to restore.");
			});
		}
		else
		{
			Debug.Log("RestorePurchases FAIL. Not supported on this platform. Current = " + Application.platform);
		}
	}

	//  
	// --- IStoreListener
	//
	public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
	{
		Debug.Log("OnInitialized: PASS");
		m_StoreController = controller;
		m_StoreExtensionProvider = extensions;
	}
	
	public void OnInitializeFailed(InitializationFailureReason error)
	{
		Debug.Log("OnInitializeFailed InitializationFailureReason:" + error);
	}

	public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs args) 
	{
		if (String.Equals(args.purchasedProduct.definition.id, kProductIDRemoveAds, StringComparison.Ordinal))
		{
			Debug.Log(string.Format("ProcessPurchase: PASS. Product: '{0}'", args.purchasedProduct.definition.id));
			// TODO: PLAYER HAS PURCHASED REMOVE ADS, REMOVE ADS.
			SaveDataManager.Instance.OnPayToRemoveAds(); // Set playerprefs and tell em we've removed ads.
			OnAdsRemoved();
		}
		else 
		{
			Debug.Log(string.Format("ProcessPurchase: FAIL. Unrecognized product: '{0}'", args.purchasedProduct.definition.id));
		}

		return PurchaseProcessingResult.Complete;
	}

	public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
	{
		Debug.Log(string.Format("OnPurchaseFailed: FAIL. Product: '{0}', PurchaseFailureReason: {1}", product.definition.storeSpecificId, failureReason));
	}
}
