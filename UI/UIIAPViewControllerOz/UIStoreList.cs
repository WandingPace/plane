using System.Collections.Generic;
using UnityEngine;

//  IAPWrapper.iapTable
// 	IAPWrapper.PurchaseProduct (string productId)

//public enum CostType { Coin = 0, Special = 1, RealMoney = 2, GetCoinsBack = 3, Total }

public enum StoreItemType
{
    CoinBundle,
    GemBundle,
    Wallpaper,
    Character,
    MovieTickets,
    CoinOffers
} // Burstly now handles Wallpaper, MovieTickets and CoinOffers

public class UIStoreList : MonoBehaviour
{
    public static bool storeLoaded; // for product list loaded from store via network
    public static bool fullStoreScrollListGenerated; // for scroll list in regular 'more coins' store
    public static bool miniStoreScrollListGenerated; // for scroll list in IAPminiStore
    protected static Notify notify;
    private List<GameObject> childCells = new List<GameObject>();
    public UILabel connectingToStore;
    //public string storeItemsToLoad;	
    private GameObject grid;
    //private SaleBanner saleBanner;
    //private NotificationSystem notificationSystem;

    private IAPWrapper iapWrapper;

    private List<IAP_DATA> myDataList = new List<IAP_DATA>();
        //private List<StoreItem> myDataList = new List<StoreItem>();

    private List<IAP_DATA> sortedDataList = new List<IAP_DATA>();
    private StoreCellData storeItemToPurchase; // temp reference, for use when purchasing a specific item
    public GameObject viewController;

    private void Awake()
    {
        notify = new Notify(GetType().Name);
        grid = gameObject; // connect to this panel's grid automatically
        iapWrapper = GameObject.Find("NonVisibleObjects/IAPWrapper").GetComponent<IAPWrapper>();
        connectingToStore.enabled = false;
        ReceivedProductList();
    }

    private void Start()
    {
        //notificationSystem = Services.Get<NotificationSystem>();
    }

    private void OnDisable()
    {
        if (connectingToStore != null)
            connectingToStore.enabled = false; // hide 'connecting to store' text
    }

    public void Refresh()
    {
        //if (saleBanner == null)
        //{
        //	saleBanner = viewController.GetComponentsInChildren<SaleBanner>(true).First();
        //}

        // set status of sale banner (show only if sale is active)
        //saleBanner.SetSaleBannerStatus(gameObject.GetComponent<UIPanel>(), 
        //	transform.parent.gameObject, DiscountItemType.StoreItem, saleBanner);
        //saleBanner.gameObject.active = false;

        foreach (var childCell in childCells)
            childCell.GetComponent<StoreCellData>().Refresh();

        //gameObject.GetComponent<UIScrollView>().ResetPosition();		
    }

    public void RequestStoreList()
    {
        notify.Debug("RequestStoreList called in UIStoreList");
        if (Application.internetReachability != NetworkReachability.NotReachable)
        {
            connectingToStore.enabled = true; // show 'Connecting to store.." text

            // connect handlers
            iapWrapper.RegisterForProductListReceived(ReceivedProductList);
            iapWrapper.RegisterForProductListFailed(StoreListNotReceived);
            iapWrapper.RequestProductList();
        }
        else
            UIManagerOz.SharedInstance.okayDialog.ShowOkayDialog("Lbl_Sto_NoAccess", "Btn_Ok");
    }

    public void StoreListNotReceived(string error, bool isErrorLocalized)
    {
        notify.Debug("StoreListNotReceived called in UIStoreList");
        //notify.Debug("Alex fixme: check isErrorLocalized, remove warning when done");

#if ! UNITY_ANDROID
    //	SharingManagerBinding.HideBusyIndicator();
#endif

#if UNITY_ANDROID
        Invoke("HideBusyIndicator", 0.1f);
            // unhiding the busy indicator is broken on android on some devices, need slight delay
#endif
        connectingToStore.enabled = false; // hide 'connecting to store' text

        // disconnect handlers		
        iapWrapper.UnregisterForProductListReceived(ReceivedProductList);
        iapWrapper.UnregisterForProductListFailed(StoreListNotReceived);
        //UnregisterHandlers();
        //UIManagerOz.SharedInstance.okayDialog.ShowOkayDialog("Msg_DownloadFail", "", "Btn_Ok");	//Product list request failed

//#if UNITY_ANDROID
//		error = StripGoogleParens(error);
//#endif			
//		
//		if (isErrorLocalized)
//		{
//			if (error.Contains("Error refreshing inventory"))
//				UIManagerOz.SharedInstance.okayDialog.ShowOkayDialog("Lbl_Sto_NoStoreConn", "Btn_Ok");	//Product list request failed, show generic error
//			else
//				UIManagerOz.SharedInstance.okayDialog.ShowOkayDialog(error, "Btn_Ok", true);			//Product list request failed, show localized error
//		}
//		else
        UIManagerOz.SharedInstance.okayDialog.ShowOkayDialog("Lbl_Sto_NoStoreConn", "Btn_Ok");
            //Product list request failed, show generic error
    }

    private string StripGoogleParens(string message)
    {
        // google store adds a message in parentheses at the end of every message, get rid of it
        var returnMessage = "";
        var startParen = message.IndexOf("(");
        var endParen = message.LastIndexOf(")");

        if (startParen != -1 && endParen != -1)
            returnMessage = message.Remove(startParen, endParen - startParen + 1);

        return returnMessage;
    }

    public void HideBusyIndicator()
    {
        //	SharingManagerBinding.HideBusyIndicator();
    }

    public void ReceivedProductList() // store list received	
    {
        notify.Debug("ReceivedProductList called in UIStoreList");

        // disconnect handlers		
        iapWrapper.UnregisterForProductListReceived(ReceivedProductList);
        iapWrapper.UnregisterForProductListFailed(StoreListNotReceived);

        connectingToStore.enabled = false; // hide 'connecting to store' text

        storeLoaded = true;

        if (gameObject.activeSelf) // only generate scroll list if this page is still being displayed
        {
            GenerateScrollList();
        }
    }

    public void GenerateScrollList() // store list received
    {
        notify.Debug("GenerateScrollList called in UIStoreList");

        // set flag, so this scroll list doesn't get generated again
        if (viewController == UIManagerOz.SharedInstance.IAPMiniStoreVC.gameObject)
            miniStoreScrollListGenerated = true;
        else if (viewController == UIManagerOz.SharedInstance.inventoryVC.gameObject)
            fullStoreScrollListGenerated = true;

        myDataList = GetFullStoreItemList();
        notify.Debug("myDataList.Count = " + myDataList.Count);
        sortedDataList = SortGridItemsByPriority(myDataList);
        notify.Debug("sortedDataList.Count = " + sortedDataList.Count);
        Initialize();

        // reposition scroll list to top
        transform.parent.GetComponent<UIScrollView>().ResetPosition();

        //UIManagerOz.SharedInstance.okayDialog.ShowOkayDialog("Product list received", "", "Btn_Ok");		
    }

    public List<IAP_DATA> GetFullStoreItemList()
    {
        notify.Debug("GetFullStoreItemList called in UIStoreList, with IAPWrapper.iapTable.Count = " +
                     IAPWrapper.iapTable.Count);

        var itemsList = new List<IAP_DATA>();

        Debug.Log("iapTable:" + IAPWrapper.iapTable.Count);
        foreach (var storeItem in IAPWrapper.iapTable) //sortedDataList)
        {
            var weeklyDiscountList = Services.Get<Store>().GetWeeklyDiscounts();

            var storeIap = storeItem.Value;

            foreach (var discount in weeklyDiscountList)
            {
                foreach (var discountItem in discount._itemList)
                {
                    if (storeItem.Value.productID.Contains(discountItem.ShortCode))
                    {
                        storeIap.priority = discountItem._salePriority;
                    }
                }
            }

            itemsList.Add(storeIap);
        }

        return itemsList;
    }

    public void Initialize()
    {
        notify.Debug("Initialize called in UIStoreList");
        childCells = CreateCells(); // create cell GameObjects for all objectives
        notify.Debug("Post-init, childCells.Count = " + childCells.Count);
        //add by lichuang
        //grid.GetComponent<UIGrid>().sorted = false;	//true;
        //grid.GetComponent<UIGrid>().Reposition();	
        //end
//		grid.GetComponent<UITable>().sorted = false;	//true;
//		grid.GetComponent<UITable>().Reposition();		
        //grid.GetComponent<UIGrid>().sorted = false;	//true;
        //grid.GetComponent<UIGrid>().Reposition();				// reset/correct positioning of all objects inside grid
    }

    private List<GameObject> CreateCells()
    {
        notify.Debug("CreateCells called in UIStoreList");

        var newObjs = new List<GameObject>();

        foreach (var data in sortedDataList) // myDataList)
            newObjs.Add(CreateStoreItemPanel(data));

        return newObjs;
    }

    private List<IAP_DATA> SortGridItemsByPriority(List<IAP_DATA> list)
    {
        notify.Debug("SortGridItemsByPriority called in UIStoreList");

        //List<IAP_DATA> listToSort = unsortedList.ToList();
        //listToSort = listToSort.OrderBy(x => x.priority).ToList(); 
        //listToSort.Sort((x,y) => x.priority.CompareTo(y.priority));
        //listToSort.Sort((IAP_DATA data1, IAP_DATA data2) => { return data1.priority.CompareTo(data2.priority); } );
        //listToSort = listToSort.OrderBy(data => data.priority).ToList();
        //pG.aL = pG.aL.OrderBy(a => a.number).ToList();

        //	list.Sort((a1, a2) => a1.priority.CompareTo(a2.priority));
        return list; //listToSort;
    }

    //private GameObject CreateStoreItemPanel(StoreItem storeItemData)
    private GameObject CreateStoreItemPanel(IAP_DATA storeItemData)
    {
        notify.Debug("CreateStoreItemPanel called in UIStoreList");

        // instantiate objective from prefab
        //add by lichuang
        //GameObject obj = (GameObject)Instantiate(Resources.Load("StoreCellOz"));	
        var obj = (GameObject) Instantiate(Resources.Load("StoreCellOzNew"));
        //end
        obj.transform.parent = grid.transform;
        obj.transform.localPosition = Vector3.zero;
        obj.transform.localScale = Vector3.one;
//		obj.transform.rotation = grid.transform.rotation;
        obj.GetComponent<StoreCellData>().SetData(storeItemData); // keep reference to data for this store item

        return obj;
    }

//	private void DisconnectConfirmHandlers()
//	{
//		UIConfirmDialogOz.onNegativeResponse -= OnPurchaseNo;
//		UIConfirmDialogOz.onPositiveResponse -= OnPurchaseYes;			
//	}	
//	
//	private void OnPurchaseNo()
//	{
//		DisconnectConfirmHandlers();	
//	}

    private void OnPurchaseYes()
    {
        //DisconnectConfirmHandlers();		// disabled since we are no longer confirming purchases prior to going to app store

        //Make sure that we clicked an item first... if this somehow gets called twice in a row (iOS registers two touches,
        //		for example), then we get a crash.
        if (storeItemToPurchase != null)
        {
            iapWrapper.PurchaseProduct(storeItemToPurchase._data.productID);
            storeItemToPurchase = null; // clear just in case
        }
        else
        {
            notify.Warning("WARNING: storeItemToPurchase is null!!!");
        }
    }

    public void OnStoreBuyPressed(GameObject cell)
    {
        storeItemToPurchase = cell.transform.parent.parent.parent.GetComponent<StoreCellData>();

        Services.Get<NotificationSystem>().ClearNotification(NotificationType.MoreCoins, 0);
            //storeItemToPurchase._data.productID);
        Services.Get<NotificationSystem>().SetNotificationIconsForThisPage(UiScreenName.UPGRADES);

        OnPurchaseYes(); // go straight to app store's confirmation dialog, no need to show our own

//		UIConfirmDialogOz.onNegativeResponse += OnPurchaseNo;
//		UIConfirmDialogOz.onPositiveResponse += OnPurchaseYes;

        //UIManagerOz.SharedInstance.confirmDialog.ShowConfirmDialog(storeItemToPurchase._data.title,	"Lbl_Dialogue_Purchase_Confirm", "Btn_No", "Btn_Yes");
        //, storeItemToPurchase._data.price);	//ShowConfirmPurchaseDialog
        //UIManagerOz.SharedInstance.confirmDialog.ShowConfirmDialog("Lbl_Dialogue_Purchase_Prompt", "Btn_No", "Btn_Yes");
//		string prompt = string.Format(Localization.SharedInstance.Get("Lbl_Dialogue_Purchase_Prompt"), storeItemToPurchase._data.title);
//		UIManagerOz.SharedInstance.confirmDialog.ShowConfirmDialog(prompt, "Btn_No", "Btn_Yes");
    }

    public void DoContinue()
        // if in-game, in resurrect menu, on positive response, kill the mini store and continue immediately
    {
        UIConfirmDialogOz.onNegativeResponse -= CancelContinue;
        UIConfirmDialogOz.onPositiveResponse -= DoContinue;

#pragma warning disable 
        var miniViewContScript = viewController.GetComponent<UIIAPMiniViewControllerOz>(); // for mini store	
#pragma warning restore 
        if (miniViewContScript != null)
        {
            miniViewContScript.disappear();

            //miniViewContScript.inGameVC.resurrectMenu.OnResurrect();			// you bought some gems, so continue with resurrect immediately.
            //miniViewContScript.inGameVC.resurrectMenu.StartResurrectTimer();	// restart resurrect timer
            UIManagerOz.SharedInstance.inGameVC.resurrectMenu.StartResurrectTimer(); // restart resurrect timer
        }
    }

    public void CancelContinue()
        // if in-game, in resurrect menu, on negative response, kill the mini store and go to gatcha 	//end game
    {
        UIConfirmDialogOz.onNegativeResponse -= CancelContinue;
        UIConfirmDialogOz.onPositiveResponse -= DoContinue;
#pragma warning disable
        var miniViewContScript = viewController.GetComponent<UIIAPMiniViewControllerOz>(); // for mini store	
#pragma warning restore
        if (miniViewContScript != null)
        {
            miniViewContScript.disappear();

            //if you don't want any gems, just die. no resurrection for you.
            //miniViewContScript.inGameVC.resurrectMenu.chooseToResurrect = false;
            //miniViewContScript.inGameVC.OnDiePostGame();
        }
    }
}


//		iapWrapper.RegisterForPurchaseSuccessful(PurchaseSuccessful);
//		iapWrapper.RegisterForPurchaseFailed(PurchaseFailed);	
//		iapWrapper.RegisterForPurchaseCancelled(PurchaseCancelled);


//
//	private void DisconnectPurchaseHandlers()
//	{
//		iapWrapper.UnregisterForPurchaseSuccessful(PurchaseSuccessful);
//		iapWrapper.UnregisterForPurchaseFailed(PurchaseFailed);		
//		iapWrapper.UnregisterForPurchaseCancelled(PurchaseCancelled);		
//	}
//	
//	private void PurchaseSuccessful(string productIdentifier)
//	{
//		DisconnectPurchaseHandlers();
//		
//		//System.Diagnostics.StackTrace t = new System.Diagnostics.StackTrace();
//		//notify.Debug(t);
//		
//		IAP_DATA productData = IAPWrapper.iapTable[productIdentifier];
//		
//		// give player appropriate quantity of primary item purchased
//		if (productData.costType == CostType.Coin) 
//			GameProfile.SharedInstance.Player.coinCount += productData.costValueOrID;
//		else if (productData.costType == CostType.Special) 
//			GameProfile.SharedInstance.Player.specialCurrencyCount += productData.costValueOrID;			
//		
//		// give player appropriate quantity of secondary item purchased
//		if (productData.costTypeSecondary == CostType.Coin) 
//			GameProfile.SharedInstance.Player.coinCount += productData.costValueSecondary;
//		else if (productData.costTypeSecondary == CostType.Special) 
//			GameProfile.SharedInstance.Player.specialCurrencyCount += productData.costValueSecondary;		
//		
//		GameProfile.SharedInstance.Serialize();
//		UIManagerOz.SharedInstance.PaperVC.UpdateCurrency();
//		Services.Get<NotificationSystem>().SetNotificationIconsForThisPage(UiScreenName.UPGRADES);
//		
//		AnalyticsInterface.LogPaymentActionEvent( productData );
//		
//		//UIManagerOz.SharedInstance.okayDialog.ShowOkayDialog("Purchase successful", "for item with id: " + productIdentifier, "Btn_Ok");
//		UIManagerOz.SharedInstance.okayDialog.ShowOkayDialog("Msg_ThankYouPurchase", "Btn_Ok");
//	}
//	
//	private void PurchaseFailed(string error, bool isErrorLocalized)
//	{
//		//notify.Debug("Alex fixme: check isErrorLocalized, remove warning when done");
//		
//		DisconnectPurchaseHandlers();
//		
//		//UIManagerOz.SharedInstance.okayDialog.ShowOkayDialog("Purchase failed", "for item with id: " + productIdentifier, "Btn_Ok");
//		
//		if (isErrorLocalized)
//			UIManagerOz.SharedInstance.okayDialog.ShowOkayDialog(error, "Btn_Ok", true);			//Product list request failed, show localized error
//		else
//			UIManagerOz.SharedInstance.okayDialog.ShowOkayDialog("Msg_InternalError", "Btn_Ok");	//Lbl_Sto_NoStoreConn
//	}	
//	
//	private void PurchaseCancelled(string error, bool isErrorLocalized)
//	{
//		DisconnectPurchaseHandlers();
//		
//		if (isErrorLocalized)
//			UIManagerOz.SharedInstance.okayDialog.ShowOkayDialog(error, "Btn_Ok", true);			//Purchase was cancelled, show localized error
//		else
//			UIManagerOz.SharedInstance.okayDialog.ShowOkayDialog("Msg_InternalError", "Btn_Ok");		
//	}	


//	private void ClearGrid(GameObject _grid)
//	{
//		UIDragPanelContents[] gridItems = _grid.GetComponentsInChildren<UIDragPanelContents>();
//		foreach (UIDragPanelContents contents in gridItems) 
//		{ 
//			//DestroyImmediate(contents.gameObject); 
//			contents.transform.parent = null;					// unparent first to remove bug when calling NGUI's UIGrid.Reposition(), because Destroy() is not immediate!
//			Destroy(contents.gameObject); 
//		}	
//	}


//	public void Refresh()
//	{
//		int i=0;
//
//		foreach (GameObject childCell in childCells)
//		{
//			childCell.GetComponent<StoreCellData>().SetData(sortedDataList[i]);	//Store.StoreItems[i]);
//			i++;
//		}		
//	}	

//	public void GenerateScrollList()	// store list received
//	{
//		//UnregisterHandlers();
//
////		switch (storeItemsToLoad)
////		{
//////			case "coins":
//////				myDataList.AddRange(GetMyStoreItemsBasedOnType(CostType.Coin));	//StoreItemType.CoinBundle));			
//////				break;			
//////			case "gems":
//////				myDataList.AddRange(GetMyStoreItemsBasedOnType(CostType.Special));	//StoreItemType.GemBundle));
//////				break;
////			case "coinsgems":
////				myDataList.AddRange(GetMyStoreItemsBasedOnType(CostType.Coin));	//StoreItemType.CoinBundle));			
////				myDataList.AddRange(GetMyStoreItemsBasedOnType(CostType.Special));	//StoreItemType.GemBundle));
////				break;		
////			default:
////				break;
////		}
//	
//		UIManagerOz.SharedInstance.okayDialog.ShowOkayDialog("Product list received", "", "Btn_Ok");		
//		myDataList = GetFullStoreItemList();
//		sortedDataList = SortGridItemsByPriority(myDataList);	
//		Initialize();	
//	}	

//	public List<IAP_DATA> GetMyStoreItemsBasedOnType(CostType myType)	//StoreItemType myType)	//StoreItem
//	{
//		//List<StoreItem> itemsList = new List<StoreItem>();
//		List<IAP_DATA> itemsList = new List<IAP_DATA>();
//		
//		//foreach (StoreItem storeItem in Store.StoreItems)	//sortedDataList)
//		foreach (KeyValuePair<string,IAP_DATA> storeItem in IAPWrapper.iapTable)	//sortedDataList)
//		{
//			if (storeItem.Value.costType == myType)
//				itemsList.Add(storeItem.Value);
//		}
//		
//		return itemsList;
//	}

//	public void UnregisterHandlers()
//	{
//		iapWrapper.UnregisterForProductListReceived(GenerateScrollList);
//		iapWrapper.UnregisterForProductListFailed(StoreListNotReceived);
//	}


//	private List<StoreItem> SortGridItemsByPriority(List<StoreItem> unsortedList)
//	{
//		List<StoreItem> listToSort = unsortedList.ToList();
//		listToSort = listToSort.OrderBy(x => x.sortPriority).ToList(); 
//		return listToSort;
//	}	


//			case "misc":
//				myDataList.AddRange(GetMyStoreItemsBasedOnType(StoreItemType.Wallpaper));
//				myDataList.AddRange(GetMyStoreItemsBasedOnType(StoreItemType.Character));
//				myDataList.AddRange(GetMyStoreItemsBasedOnType(StoreItemType.MovieTickets));
//				break;
//			case "coinoffers":
//				myDataList.AddRange(GetMyStoreItemsBasedOnType(StoreItemType.CoinOffers));
//				break;	


//	public void OnStoreCellPressed(GameObject cell) 
//	{
//		// set up shorter local identifiers, to keep code easy to read
//		//UIIAPViewControllerOz viewContScript = viewController.GetComponent<UIIAPViewControllerOz>();	
//		UIIAPMiniViewControllerOz miniViewContScript = viewController.GetComponent<UIIAPMiniViewControllerOz>();	// for mini store
//		//StoreCellData storeCellData = cell.transform.parent.GetComponent<StoreCellData>();
//		StoreCellData storeCellData = cell.transform.parent.transform.parent.GetComponent<StoreCellData>();
//		//int storeItemID = storeCellData._data.id;
//		//CharacterStats activeCharacter = GameProfile.SharedInstance.GetActiveCharacter();
//		//PlayerStats playerStats = GameProfile.SharedInstance.Player;
//
//		storeItemToPurchase = storeCellData;
//		UIConfirmDialogOz.onNegativeResponse += OnPurchaseNo;
//		UIConfirmDialogOz.onPositiveResponse += OnPurchaseYes;
//			
//		//Decimal cost = storeCellData._data.cost / 100.00M;
//	 	//string costString = String.Format("{0:C}", cost);
//		
//		//if (miniViewContScript != null)
//		//if (miniViewContScript != null && storeCellData._data.itemType == StoreItemType.GemBundle)	// if in mini store, from resurrect menu, here to purchase gems
////		if (miniViewContScript != null && storeCellData._data.costType == CostType.Special)	// if in mini store, from resurrect menu, here to purchase gems
//
////		{
//			//UIConfirmDialogOz.onNegativeResponse += CancelContinue;			// if in resurrect menu, go straight back into the post-game if gems not purchased
//			//UIConfirmDialogOz.onPositiveResponse += DoContinue; 			// if in resurrect menu, go straight back into the game if gems purchased
////		}
//	
//		UIManagerOz.SharedInstance.confirmDialog.ShowConfirmPurchaseDialog(storeCellData._data.title, "Lbl_Dialogue_Purchase_Confirm", "Btn_No", "Btn_Yes", storeCellData._data.price);			
//			
////		UIManagerOz.SharedInstance.PaperVC.UpdateCurrency();
//		//if (viewContScript != null)
//		//	viewContScript.UpdateCurrency();								// will update coin and gem counts in UI				
//		//else if (miniViewContScript != null)
//		//	miniViewContScript.UpdateCurrency();							// for mini store
//		
////		storeCellData.Refresh();											// ask cell to update its GUI rendering to match data, in case it was updated in transaction
//	}


// set up shorter local identifiers, to keep code easy to read
//UIIAPViewControllerOz viewContScript = viewController.GetComponent<UIIAPViewControllerOz>();
//#pragma warning disable
//UIIAPMiniViewControllerOz miniViewContScript = viewController.GetComponent<UIIAPMiniViewControllerOz>();	// for mini store
//#pragma warning restore
//PlayerStats playerStats = GameProfile.SharedInstance.Player;

//		playerStats.PurchaseStoreItem(storeItemToPurchase._data.id);	// buy it if we can afford it

//UIManagerOz.SharedInstance.PaperVC.UpdateCurrency();
//if (viewContScript != null)
//	viewContScript.UpdateCurrency();							// will update coin and gem counts in UI				
//else if (miniViewContScript != null)
//	miniViewContScript.UpdateCurrency();						// for mini store

//consumableToPurchase.Refresh();								// ask cell to update its GUI rendering to match data, in case it was updated in the transaction		


//List<StoreItem> sortedDataList = SortGridItemsByPriority(Store.StoreItems);


//ClearGrid(grid);										// kill all old objectives just in case			

//				case "gems":
//				myDataList.AddRange(GetMyStoreItemsBasedOnType(StoreItemType.GemBundle));
//				break;
//			case "coins":
//				myDataList.AddRange(GetMyStoreItemsBasedOnType(StoreItemType.CoinBundle));
//				break;
//			case "misc":
//				myDataList.AddRange(GetMyStoreItemsBasedOnType(StoreItemType.Wallpaper));
//				myDataList.AddRange(GetMyStoreItemsBasedOnType(StoreItemType.Character));
//				myDataList.AddRange(GetMyStoreItemsBasedOnType(StoreItemType.MovieTickets));
//				break;
//	

//	public void OnAlreadyMaxedOut() 
//	{
//		UIOkayDialogOz.onPositiveResponse -= OnAlreadyMaxedOut;
//	}		

//void Update() { }


//	public void DoGatchaSomething() 	// if in-game, in gatcha screen, on positive response, do something
//	{
//		UIIAPMiniViewControllerOz miniViewContScript = viewController.GetComponent<UIIAPMiniViewControllerOz>();	// for mini store	
//		
//		if (miniViewContScript != null)
//		{
//			miniViewContScript.inGameVC.resurrectMenu.OnResurrect();
//		}
//	}	


//if (playerStats.IsConsumableMaxedOut(consumableID) == false) 	// check if already purchased maximum allowed
//{
//			if (playerStats.CanAffordConsumable(consumableID) == true)
//			{

//"Purchase the " + storeCellData._data.title + " for " + costString + "?", "No", "Yes");	

//UIManagerOz.SharedInstance.confirmDialog.ShowConfirmDialog(storeCellData._data.title, "Purchase the "
//	+ storeCellData._data.title + " for " + costString + "?", "No", "Yes");	
//playerStats.PurchaseConsumable(consumableID);			// buy it if we can afford it
//			}
//			else
//			{
//				UIConfirmDialogOz.onNegativeResponse += viewContScript.OnNeedMoreCoinsNo;
//				UIConfirmDialogOz.onPositiveResponse += viewContScript.OnNeedMoreCoinsYes;
//				UIManagerOz.SharedInstance.confirmDialog.ShowConfirmDialog("Need More Coins!","Would you like to get more coins?", "No", "Yes");
//			}
//		}
//		else
//		{
//			UIOkayDialogOz.onPositiveResponse += OnAlreadyMaxedOut;
//			UIManagerOz.SharedInstance.okayDialog.ShowOkayDialog("You've maxed out this consumable","Maximum " 
//				+ ConsumableStore.maxOfEachConsumable.ToString() + " in inventory at once!", "OK"); 
//		}