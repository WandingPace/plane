using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public struct IAP_DATA 
{
	public string productID;
	
	public string title;
	public string description;
	public string price;
	public string iconname;	
	public int priority;	
	
	public CostType costType;
	public int costValueOrID;
	
	public CostType costTypeSecondary;	// for when there is a secondary item type in the bundle
	public int costValueSecondary;
	
	public string unformattedPrice;		// For analytics tracking
	public string currencyCode;			// For analytics tracking
	
	public bool dataIsActualStoreData;	// turns true to signify that this is not just default data
}

//public class StoreItem
//{
//	public string title = "";	
//	public string description = "";
//	public string icon = "";
//	public CostType costType = CostType.Coin;
//	public int cost = 1;	// cents, so 100 = $1.00
//	public int markDownCost = 1;
//	public StoreItemType itemType = StoreItemType.CoinBundle;
//	public int itemQuantity = 1;	
//	public int sortPriority = 0;	
//	public int id = 0;
//}

/// <summary>
/// IAP wrapper. Tries to abstract the different store platforms
/// </summary>
public class IAPWrapper : MonoBehaviour
{
    protected static Notify notify;
    static public IAPWrapper SharedInstance = null;

	const string COINS_MIN = "coinpacka";
	const string COINS_SMALL = "coinpackb";
	const string COINS_MEDIUM = "coinpackc";
	const string COINS_LARGE = "coinpackd";
	const string COINS_HUGE = "coinpacke";
	const string COINS_GIGANTIC = "coinpackf";
	const string GEMS_MIN = "gempacka";
	const string GEMS_SMALL = "gempackb";
	const string GEMS_MEDIUM = "gempackc";
	const string GEMS_LARGE = "gempackd";
	const string GEMS_HUGE = "gempacke";
	const string GEMS_GIGANTIC = "gempackf";
	const string STARTER_PACK_MIN = "starterpacka";
	
	static string IAP_COINS_MIN;
	static string IAP_COINS_SMALL;
	static string IAP_COINS_MEDIUM;
	static string IAP_COINS_LARGE;
	static string IAP_COINS_HUGE;
	static string IAP_COINS_GIGANTIC;
	static string IAP_GEMS_MIN;
	static string IAP_GEMS_SMALL;
	static string IAP_GEMS_MEDIUM ;
	static string IAP_GEMS_LARGE;
	static string IAP_GEMS_HUGE;
	static string IAP_GEMS_GIGANTIC;
	static string IAP_STARTER_PACK_MIN;
	
	// wxj, iaps for onekey purchase artifacts
	public static string[] IAPS_ONEKEY_UPG = {
		"2400001",	// double coins
		"2400002"	// upgrade power
	};

	
	public static bool isIAPCalled = false;
	

	static List<string> ProductIdentifiers = null;
	public static Dictionary<string, IAP_DATA> iapTable = null;
	
	private StoreBridgeBase storeBridge;
	
	// Get the IAP data through IAPWrapper.iapTable 
	public delegate void ProductListReceivedHandler();
	protected static event ProductListReceivedHandler onProductListReceivedEvent = null;

	public void RegisterForProductListReceived( ProductListReceivedHandler delg) {
		onProductListReceivedEvent += delg; }	
	public void UnregisterForProductListReceived( ProductListReceivedHandler delg) {
		onProductListReceivedEvent -= delg; }

	public delegate void ProductListFailedHandler(string error, bool isErrorLocalized );
	protected static event ProductListFailedHandler onProductListFailedEvent = null;

	public void RegisterForProductListFailed( ProductListFailedHandler delg) {
		onProductListFailedEvent += delg; }	
	public void UnregisterForProductListFailed( ProductListFailedHandler delg) {
		onProductListFailedEvent -= delg; }
	
	
	public delegate void PurchaseSuccessfulHandler(string productIdentifier );
	protected static event PurchaseSuccessfulHandler onPurchaseSuccessfulEvent = null;	

	public void RegisterForPurchaseSuccessful( PurchaseSuccessfulHandler delg) {
		onPurchaseSuccessfulEvent += delg; }	
	public void UnregisterForPurchaseSuccessful( PurchaseSuccessfulHandler delg) {
		onPurchaseSuccessfulEvent -= delg; }	
	
	
	public delegate void PurchaseFailedHandler(string error, bool isErrorLocalized );
	protected static event PurchaseFailedHandler onPurchaseFailedEvent = null;

	public void RegisterForPurchaseFailed( PurchaseFailedHandler delg) {
		onPurchaseFailedEvent += delg; }	
	public void UnregisterForPurchaseFailed( PurchaseFailedHandler delg) {
		onPurchaseFailedEvent -= delg; }	
	
	
	public delegate void PurchaseCancelledHandler(string error, bool isErrorLocalized );
	protected static event PurchaseCancelledHandler onPurchaseCancelledEvent = null;

	public void RegisterForPurchaseCancelled( PurchaseCancelledHandler delg) {
		onPurchaseCancelledEvent += delg; }	
	public void UnregisterForPurchaseCancelled( PurchaseCancelledHandler delg) {
		onPurchaseCancelledEvent -= delg; }		
	
	
	// late purchases can happen in google play store
	// 1st session user buys the itmem but a crash or network errors fails to consume the item
	// 2nd session we see he has an unconsumed purchase, consume it and inform the user through these callbacks
	
	public delegate void LatePurchaseSuccessfulHandler(string productIdentifier );
	protected static event LatePurchaseSuccessfulHandler onLatePurchaseSuccessfulEvent = null;

	public void RegisterForLatePurchaseSuccessful( LatePurchaseSuccessfulHandler delg) {
		onLatePurchaseSuccessfulEvent += delg; }	
	public void UnregisterForLatePurchaseSuccessful( LatePurchaseSuccessfulHandler delg) {
		onLatePurchaseSuccessfulEvent -= delg; }
	
	
	public delegate void LatePurchaseFailedHandler(string error, bool isErrorLocalized );
	protected static event LatePurchaseFailedHandler onLatePurchaseFailedEvent = null;

	public void RegisterForLatePurchaseFailed( LatePurchaseFailedHandler delg) {
		onLatePurchaseFailedEvent += delg; }	
	public void UnregisterForLatePurchaseSuccessful( LatePurchaseFailedHandler delg) {
		onLatePurchaseFailedEvent -= delg; }
	
	private bool eventsHooked;
	
//	private bool doBasicTests = false;
	
	// these goofy codes are a publishing requirement that we add to product ids on android
#if UNITY_ANDROID
	static Dictionary<string, string> GoogleGoofyCode = new Dictionary<string, string> ()
	{
		{ COINS_MIN ,"2207218"},
		{ COINS_SMALL ,"2207219"},	
		{ COINS_MEDIUM, "2207220"},
		{ COINS_LARGE, "2207221"},
		{ COINS_HUGE, "2207222"},
		{ COINS_GIGANTIC, "2207223"},
		{ GEMS_MIN, "2207224"},
		{ GEMS_SMALL, "2207225"},
		{ GEMS_MEDIUM, "2207226"},
		{ GEMS_LARGE, "2207227"},
		{ GEMS_HUGE, "2207228"},
		{ STARTER_PACK_MIN, "2207229"},
		{ GEMS_GIGANTIC, "2207522"},
	};
	
	static Dictionary<string, string> AmazonGoofyCode = new Dictionary<string, string> ()
	{
		{ COINS_MIN ,"2207230"},
		{ COINS_SMALL ,"2207231"},	
		{ COINS_MEDIUM, "2207232"},
		{ COINS_LARGE, "2207233"},
		{ COINS_HUGE, "2207234"},
		{ COINS_GIGANTIC, "2207235"},
		{ GEMS_MIN, "2207236"},
		{ GEMS_SMALL, "2207237"},
		{ GEMS_MEDIUM, "2207238"},
		{ GEMS_LARGE, "2207239"},
		{ GEMS_HUGE, "2207240"},
		{ STARTER_PACK_MIN, "2207241"},
		{ GEMS_GIGANTIC, "2207521"},
	};
#endif
	
	static string computeProductId( string bundleId, string baseProductId)
	{
		string connector = ".";	
		string result = bundleId + connector + baseProductId;
#if UNITY_ANDROID
		// we add these goofy codes that differ between amazon and google
		string androidStore = Settings.GetString("android-store","");
		if ( androidStore == "amazon")
		{
			if ( AmazonGoofyCode.TryGetValue(baseProductId, out connector) )
			{
				connector = "." + connector + ".";	
			}
			result = bundleId + connector + baseProductId;
		}
		else if (androidStore == "google")
		{
			if ( GoogleGoofyCode.TryGetValue(baseProductId, out connector) )
			{
				connector = "." + connector + ".";	

			}	
			result = bundleId + connector + baseProductId;
			
			// this weirdness is becaues for dev builds, I set the product ids to unmanaged when they should be managed,
			// so I added a ".pm" suffix to the new propely managed product ids
			result += Settings.GetString("google-product-suffix","");
		}
		//turns out android wants these all lowercase
		result = result.ToLower();
#endif
		return result;
	}
	
	private static void InitIapTable()
	{		
		
		
		IAP_COINS_MIN =  "2207218";
		IAP_COINS_SMALL =  "2207219";
		IAP_COINS_MEDIUM = "2207220";
		IAP_COINS_LARGE = "2207221";
		IAP_COINS_HUGE = "2207222";
		IAP_COINS_GIGANTIC = "2207223";
		
		IAP_GEMS_MIN = "2207224";
		IAP_GEMS_SMALL = "2207225";
		IAP_GEMS_MEDIUM = "2207226";
		IAP_GEMS_LARGE = "2207227";
		IAP_GEMS_HUGE = "2207228";
		IAP_GEMS_GIGANTIC = "2207522";
		IAP_STARTER_PACK_MIN = "2207229";

		

		ProductIdentifiers = new List<string> ();
		
		
		
		ProductIdentifiers.Add(IAP_COINS_SMALL);
		ProductIdentifiers.Add(IAP_COINS_MEDIUM);
		ProductIdentifiers.Add(IAP_COINS_LARGE);
		
		ProductIdentifiers.Add(IAP_COINS_MIN);
		ProductIdentifiers.Add(IAP_COINS_HUGE);
		ProductIdentifiers.Add(IAP_COINS_GIGANTIC);
		

		
		ProductIdentifiers.Add(IAP_GEMS_SMALL);
		ProductIdentifiers.Add(IAP_GEMS_MEDIUM);
		ProductIdentifiers.Add(IAP_GEMS_LARGE);
		
		ProductIdentifiers.Add(IAP_GEMS_MIN);
		ProductIdentifiers.Add(IAP_GEMS_HUGE);
		ProductIdentifiers.Add(IAP_GEMS_GIGANTIC);
		
		
		
		ProductIdentifiers.Add(IAP_STARTER_PACK_MIN);
		
		ProductIdentifiers.Add(IAP_COINS_SMALL);
		ProductIdentifiers.Add(IAP_COINS_MEDIUM);
		ProductIdentifiers.Add(IAP_COINS_LARGE);
	

	
		
		
		iapTable = new Dictionary<string, IAP_DATA>();
		
		IAP_DATA cd = new IAP_DATA();
		
		
		/// ALEX FIXME TO HANDLE 5 GEMS and 1,500 COINS
		cd = new IAP_DATA();
		cd.costType = CostType.Special;
		cd.costValueOrID = 10;
		cd.costTypeSecondary = CostType.Coin;
		cd.costValueSecondary = 10000;			
        cd.iconname = "store_gem";//"icon_starterpack";
		cd.title = "Sto_IAP_CoinPack_Desc_5";
		cd.description = "";
		cd.price = " 10.0";
		cd.priority = 200 - 70;
		cd.productID = IAP_STARTER_PACK_MIN;
		cd.dataIsActualStoreData = false;		
		iapTable.Add(IAP_STARTER_PACK_MIN, cd);
		
		cd = new IAP_DATA();
		cd.costType = CostType.Special;
		cd.costValueOrID = 8;
		cd.costTypeSecondary = CostType.Special;
		cd.costValueSecondary = 0;			
        cd.iconname = "store_gem";//"gembundle_01";
		cd.title = "Sto_IAP_GemPack_Desc_1";
		cd.description = "";
		cd.price = " 6.0";
		cd.priority = 200 - 51;		
		cd.productID = IAP_GEMS_SMALL;
		cd.dataIsActualStoreData = false;		
		iapTable.Add(IAP_GEMS_SMALL, cd);
		
		cd = new IAP_DATA();
		cd.costType = CostType.Special;
		cd.costValueOrID = 14;
		cd.costTypeSecondary = CostType.Special;
		cd.costValueSecondary = 0;			
        cd.iconname = "store_gem";//"gembundle_02";
		cd.title = "Sto_IAP_GemPack_Desc_2";
		cd.description = "";
		cd.price = " 10.0";
		cd.priority = 200 - 41;		
		cd.productID = IAP_GEMS_MEDIUM;
		cd.dataIsActualStoreData = false;		
		iapTable.Add(IAP_GEMS_MEDIUM, cd);
		
		cd = new IAP_DATA();
		cd.costType = CostType.Special;
		cd.costValueOrID = 22;
		cd.costTypeSecondary = CostType.Special;
		cd.costValueSecondary = 0;			
		cd.iconname = "gembundle_03";
		cd.title = "Sto_IAP_GemPack_Desc_3";
		cd.description = "";
		cd.price = " 15.0";
		cd.priority = 200 - 31;		
		cd.productID = IAP_GEMS_LARGE;
		cd.dataIsActualStoreData = false;		
		iapTable.Add(IAP_GEMS_LARGE, cd);
		
		cd = new IAP_DATA();
		cd.costType = CostType.Special;
		cd.costValueOrID = 40;
		cd.costTypeSecondary = CostType.Special;
		cd.costValueSecondary = 0;			
		cd.iconname = "gembundle_04";
		cd.title = "Sto_IAP_GemPack_Desc_4";
		cd.description = "";
		cd.price = " 30.0";
		cd.priority = 200 - 51;		
		cd.productID = IAP_GEMS_MIN;
		cd.dataIsActualStoreData = false;		
		iapTable.Add(IAP_GEMS_MIN, cd);
		
		cd = new IAP_DATA();
		cd.costType = CostType.Special;
		cd.costValueOrID = 90;
		cd.costTypeSecondary = CostType.Special;
		cd.costValueSecondary = 0;			
		cd.iconname = "gembundle_05";
		cd.title = "Sto_IAP_GemPack_Desc_5";
		cd.description = "";
		cd.price = " 60.0";
		cd.priority = 200 - 51;		
		cd.productID = IAP_GEMS_HUGE;
		cd.dataIsActualStoreData = false;		
		iapTable.Add(IAP_GEMS_HUGE, cd);
		
		cd = new IAP_DATA();
		cd.costType = CostType.Special;
		cd.costValueOrID = 150;
		cd.costTypeSecondary = CostType.Special;
		cd.costValueSecondary = 0;			
		cd.iconname = "gembundle_06";
		cd.title = "Sto_IAP_GemPack_Desc_6";
		cd.description = "";
		cd.price = " 90.0";
		cd.priority = 200 - 51;		
		cd.productID = IAP_GEMS_GIGANTIC;
		cd.dataIsActualStoreData = false;		
		iapTable.Add(IAP_GEMS_GIGANTIC, cd);
			
		
		
		
		cd = new IAP_DATA();
		cd.costType = CostType.Coin;
		cd.costValueOrID = 8000;
		cd.costTypeSecondary = CostType.Coin;
		cd.costValueSecondary = 0;			
        cd.iconname = "store_coin" ;//"coinbundle_01";
		cd.title = "Sto_IAP_CoinPack_Desc_1";
		cd.description = "";
		cd.price = " 6.0";
		cd.priority = 200 - 50;		
		cd.productID = IAP_COINS_SMALL;
		cd.dataIsActualStoreData = false;		
		iapTable.Add(IAP_COINS_SMALL, cd);
		
		cd = new IAP_DATA();
		cd.costType = CostType.Coin;
		cd.costValueOrID = 14000;
		cd.costTypeSecondary = CostType.Coin;
		cd.costValueSecondary = 0;			
		cd.iconname = "coinbundle_02";
		cd.title = "Sto_IAP_CoinPack_Desc_2";
		cd.description = "";
		cd.price = " 10.0";
		cd.priority = 200 - 40;		
		cd.productID = IAP_COINS_MEDIUM;
		cd.dataIsActualStoreData = false;		
		iapTable.Add(IAP_COINS_MEDIUM, cd);
		
		cd = new IAP_DATA();
		cd.costType = CostType.Coin;
		cd.costValueOrID = 22000;
		cd.costTypeSecondary = CostType.Coin;
		cd.costValueSecondary = 0;			
		cd.iconname = "coinbundle_03";
		cd.title = "Sto_IAP_CoinPack_Desc_3";
		cd.description = "";
		cd.price = " 15.0";
		cd.priority = 200 - 30;		
		cd.productID = IAP_COINS_LARGE;
		cd.dataIsActualStoreData = false;		
		iapTable.Add(IAP_COINS_LARGE, cd);
		
		
		cd = new IAP_DATA();
		cd.costType = CostType.Coin;
		cd.costValueOrID = 40000;
		cd.costTypeSecondary = CostType.Coin;
		cd.costValueSecondary = 0;			
		cd.iconname = "coinbundle_04";
		cd.title = "Sto_IAP_CoinPack_Desc_4";
		cd.description = "";
		cd.price = " 30.0";
		cd.priority = 200 - 30;		
		cd.productID = IAP_COINS_MIN;
		cd.dataIsActualStoreData = false;		
		iapTable.Add(IAP_COINS_MIN, cd);
		
		
		cd = new IAP_DATA();
		cd.costType = CostType.Coin;
		cd.costValueOrID = 90000;
		cd.costTypeSecondary = CostType.Coin;
		cd.costValueSecondary = 0;			
		cd.iconname = "coinbundle_05";
		cd.title = "Sto_IAP_CoinPack_Desc_6";
		cd.description = "";
		cd.price = " 60.0";
		cd.priority = 200 - 30;		
		cd.productID = IAP_COINS_HUGE;
		cd.dataIsActualStoreData = false;		
		iapTable.Add(IAP_COINS_HUGE, cd);
		
		
		cd = new IAP_DATA();
		cd.costType = CostType.Coin;
		cd.costValueOrID = 150000;
		cd.costTypeSecondary = CostType.Coin;
		cd.costValueSecondary = 0;			
		cd.iconname = "coinbundle_cityofcoins";
		cd.title = "Sto_IAP_CoinPack_Desc_7";
		cd.description = "";
		cd.price = " 90.0";
		cd.priority = 200 - 30;		
		cd.productID = IAP_COINS_GIGANTIC;
		cd.dataIsActualStoreData = false;		
		iapTable.Add(IAP_COINS_GIGANTIC, cd);
		
		int IS_LARGE_PAY=PlayerPrefs.GetInt("IS_LARGE_PAY");
		if(IS_LARGE_PAY==0)
		{
			iapTable.Remove(IAP_COINS_GIGANTIC);
			iapTable.Remove(IAP_COINS_MIN);
			iapTable.Remove(IAP_COINS_HUGE);
			iapTable.Remove(IAP_GEMS_MIN);
			iapTable.Remove(IAP_GEMS_GIGANTIC);
			iapTable.Remove(IAP_GEMS_HUGE);
			
		}
		
	}

	void Awake()
	{
		notify = new Notify(this.GetType().Name);
		notify.Debug("bundleid = " + BundleInfo.GetBundleId());

        SharedInstance = this;
	}

	void Start()
	{
	}

    public void Initialize()
    {
        InitIapTable();
#if UNITY_EDITOR
        storeBridge = new StoreBridgeBase();
#elif UNITY_IPHONE
		storeBridge = new StoreBridgeBase();
#elif UNITY_ANDROID

		storeBridge = new StoreBridgeBase();

#endif
        storeBridge.RegisterForProductListReceived(productListReceived);
        storeBridge.RegisterForProductListFailed(productListFailed);
        storeBridge.RegisterForPurchaseSuccessful(purchaseSuccessful);
        storeBridge.RegisterForPurchaseFailed(purchaseFailed);
        storeBridge.RegisterForPurchaseCancelled(purchaseCancelled);
        storeBridge.RegisterForLatePurchaseSuccessful(latePurchaseSuccesful);
        storeBridge.RegisterForLatePurchaseFailed(latePurchaseFailed);

        HookEvents();
        //		doBasicTests = Settings.GetBool("iap-test", false);
        //		if (doBasicTests)
        //		{
        //			// 2 calls below are for testing and need to be removed by Alex when GUI is ready
        //			HookEvents ();
        //			storeBridge.RequestProductList(ProductIdentifiers.ToArray(), ref iapTable);
        //		}
    }

    /// <summary>
	/// Hooks the native store events. - deliberating if this should be made private and done automatically
	/// </summary>
	private void HookEvents()
	{
		storeBridge.HookEvents();
		eventsHooked = true;
	}
	
	private void UnhookEvents()
	{
		storeBridge.UnhookEvents();
		eventsHooked = false;
	}
	
	public void RequestProductList()
	{
		notify.Debug ("RequestProductList");
		if (!eventsHooked)
		{
			HookEvents();	
		}
		storeBridge.RequestProductList(ProductIdentifiers.ToArray(), ref iapTable);
	}
	
	private void productListReceived(Dictionary<string, IAP_DATA> newTable)
	{
		notify.Debug("IAPWrapper.productListReceived");
		iapTable = newTable;
		if (onProductListReceivedEvent != null)
		{
			onProductListReceivedEvent();	
		}
		
//		if (doBasicTests)
//		{
//			notify.Debug("automatically buying {0}", ProductIdentifiers[0]);
//			storeBridge.PurchaseProduct( ProductIdentifiers[0]);
//		}
	}
	
	private void productListFailed(string error, bool isErrorLocalized)
	{		
		notify.Debug("ProductListFailed {0}", error);
		if (onProductListFailedEvent != null)
		{
			onProductListFailedEvent(error, isErrorLocalized);
		}
	}
	
	private void purchaseSuccessful(string productId)
	{
		notify.Debug("purchaseSuccessful {0}", productId);
		if ( onPurchaseSuccessfulEvent != null)
		{
			onPurchaseSuccessfulEvent(productId);	
		}
	}
	
	private void purchaseCancelled(string error, bool isErrorLocalized)
	{
		notify.Debug("purchaseCancelled {0}", error);
		if ( onPurchaseCancelledEvent != null)
		{
			onPurchaseCancelledEvent(error, isErrorLocalized);	
		}
	}
	
	private void purchaseFailed(string error, bool isErrorLocalized)
	{
		notify.Debug("purchaseFailed {0}", error);
		if ( onPurchaseFailedEvent != null)
		{
			onPurchaseFailedEvent(error, isErrorLocalized);	
		}
	}
	
	// this can trigger in google play, when he bought the item but a network error or crash failed to consume the item
	// so this can trigger after you get the product lit
	private void latePurchaseSuccesful(string productId)
	{
		notify.Debug("latePurchaseSuccesful {0}", productId);
		if ( onLatePurchaseSuccessfulEvent != null)
		{
			onLatePurchaseSuccessfulEvent(productId);	
		}
	}
	
	private void latePurchaseFailed(string error, bool isErrorLocalized)
	{
		notify.Debug("latePurchaseFailed {0}", error);
		if ( onLatePurchaseFailedEvent != null)
		{
			onLatePurchaseFailedEvent(error, isErrorLocalized);	
		}
	}
	
	/// <summary>
	/// Purchases the product.  Wait for the PurchaseSuccesful or PurchaseFailed to get informed if it succeeds or fails
	/// </summary>
	/// <param name='productId'>
	/// Product identifier.
	/// </param>
	public void PurchaseProduct(string productId)
	{
		 
	}
}
