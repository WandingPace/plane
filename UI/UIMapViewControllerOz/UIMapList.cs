//using UnityEngine;
//using System;
//using System.Collections;
//using System.Collections.Generic;
//using System.Linq;
//
//public class UIMapList : MonoBehaviour 
//{
//	public GameObject viewController;
//	public string mapLocationToLoad;
//	
//	private int environmentID = EnvironmentSetManager.WhimsyWoodsId;
//	private GameObject grid;
//	private List<GameObject> childCells = new List<GameObject>();
//	
//	private int travelToLocation = EnvironmentSetManager.WhimsyWoodsId;
//
//	void Start() 
//	{ 
//		switch (mapLocationToLoad)
//		{
//			case "ww":
//				environmentID = EnvironmentSetManager.WhimsyWoodsId;
//				break;
//			case "df":
//				environmentID = EnvironmentSetManager.DarkForestId;
//				break;
//			case "ybr":
//				environmentID = EnvironmentSetManager.YellowBrickRoadId;
//				break;
//		}
//		
//		//Initialize();
//	}
//	
//	public void OnMapCellClicked(GameObject button)
//	{
//		MapCellData data = button.transform.parent.parent.GetComponent<MapCellData>();	
//		//string locKey = viewController.GetComponent<UIMapViewControllerOz>().GetLocalizationKeyForLocation(data._data.id);
//		
//		if (DownloadManager.HaveAllLocationsBeenDownloaded())	//EnvironmentSetManager.SharedInstance.IsLocallyAvailable(data._data.id))
//		{
//			if (GameProfile.SharedInstance.Player.coinCount < data._data.cost)
//			{
//				UIConfirmDialogOz.onNegativeResponse += viewController.GetComponent<UIMapViewControllerOz>().OnNeedMoreCoinsNo;
//				UIConfirmDialogOz.onPositiveResponse += viewController.GetComponent<UIMapViewControllerOz>().OnNeedMoreCoinsYes;
//				//UIManagerOz.SharedInstance.confirmDialog.ShowConfirmDialog("Lbl_Dialogue_MoreCoins_Prompt", "Lbl_Dialogue_MoreCoins_Confirm", "Btn_No", "Btn_Yes");	
//				UIManagerOz.SharedInstance.confirmDialog.ShowConfirmDialog("Lbl_Dialogue_MoreCoins_Prompt", "Btn_No", "Btn_Yes");	
//			}
//			else
//			{
//				UIConfirmDialogOz.onNegativeResponse += OnConfirmNo;
//				UIConfirmDialogOz.onPositiveResponse += OnConfirmYes;
//				//UIManagerOz.SharedInstance.confirmDialog.ShowConfirmDialog(locKey, "Lbl_ActivateFastTravel", "Btn_No", "Btn_Yes");
//				UIManagerOz.SharedInstance.confirmDialog.ShowConfirmDialog("Lbl_ActivateFastTravel", "Btn_No", "Btn_Yes");
//				travelToLocation = data._data.id;	// store ID, in case confirmed
//			}
//		}
//		else 	// download
//		{
//			DownloadManager.DownloadAllLocations();
//			//UIManagerOz.SharedInstance.okayDialog.ShowOkayDialog(locKey, "Loc_Downloading", "Btn_Ok");
//			UIManagerOz.SharedInstance.okayDialog.ShowOkayDialog("Loc_Downloading", "Btn_Ok");
//		}
//	}
//
//	public void OnConfirmNo()
//	{
//		UIConfirmDialogOz.onNegativeResponse -= OnConfirmNo;
//		UIConfirmDialogOz.onPositiveResponse -= OnConfirmYes;
//	}
//	
//	public void OnConfirmYes()
//	{
//		OnConfirmNo();	// disconnect handlers
//		
//		//button.GetComponent<BoxCollider>().enabled = false;		// disable button once fast travel is selected
//		//NGUITools.SetActive(buttonWinkieCountry.gameObject, false);	
//		GamePlayer.SharedInstance.StartFastTravel(travelToLocation);//, 1000f);	// enable fast travel
//		//string locKey = viewController.GetComponent<UIMapViewControllerOz>().GetLocalizationKeyForLocation(travelToLocation);
//		//UIManagerOz.SharedInstance.okayDialog.ShowOkayDialog(locKey, "Lbl_PressPlayToTravel", "Btn_Ok");
//		UIManagerOz.SharedInstance.okayDialog.ShowOkayDialog("Lbl_PressPlayToTravel", "Btn_Ok");
//				
//		//UIManagerOz.SharedInstance.mapVC.fastTravelID = travelToLocation;			// set value in UIMapViewControllerOz
//		
//		//UIManagerOz.SharedInstance.mapVC.RefreshIsAvailableStatusAndButtonCollidersAmongOtherThings();		
//	}	
//		
//	public void Refresh(List<MapLocation> mapLocationDataList)
//	{
//		foreach (GameObject childCell in childCells)
//		{
//			MapCellData childData = childCell.GetComponent<MapCellData>();
//			
//			foreach (MapLocation locationData in mapLocationDataList)
//			{
//				if (childData._data.id == locationData.id)
//					childData.SetData(locationData);
//			}
//		}
//	}
//	
//	private void Initialize()
//	{
//		grid = gameObject.transform.Find("Grid").gameObject; 	// connect to this panel's grid automatically
//		ClearGrid(grid);										// kill all old objectives just in case		
//		childCells = CreateCells();								// create cell GameObjects for all objectives
//		grid.GetComponent<UIGrid>().Reposition();				// reset/correct positioning of all objects inside grid
//	}
//
//	private List<GameObject> CreateCells()
//	{
//		List<GameObject> newObjs = new List<GameObject>();
//		//List<StoreItem> sortedDataList = SortGridItemsByPriority(Store.StoreItems);
//		
//		foreach (MapLocation data in viewController.GetComponent<UIMapViewControllerOz>().GetLocationDataList())	//sortedDataList)
//		{
//			if (data.id == environmentID)
//				newObjs.Add(CreateMapLocationPanel(data));
//		}
//		return newObjs;
//	}
//	
//	private GameObject CreateMapLocationPanel(MapLocation mapLocationData)
//	{
//		// instantiate objective from prefab
//		GameObject obj = (GameObject)Instantiate(Resources.Load("MapCellOz"));	
//		obj.transform.parent = grid.transform;
//		obj.transform.localPosition = Vector3.zero;		
//		obj.transform.localScale = Vector3.one;
//		obj.transform.rotation = grid.transform.rotation;
//		obj.GetComponent<MapCellData>().SetData(mapLocationData);			// keep reference to data for this store item
//		obj.GetComponent<MapCellData>().viewController = viewController;	// pass on reference to view controller, for event response		
//		obj.GetComponent<MapCellData>().mapList = this;						// pass on reference to this script, for event response		
//		return obj;
//	}
//	
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
//	
//	private List<MapLocation> SortGridItemsByPriority(List<MapLocation> unsortedList)
//	{
//		List<MapLocation> listToSort = unsortedList.ToList();
//		listToSort = listToSort.OrderBy(x => x.sortPriority).ToList(); 
//		return listToSort;
//	}
//}
//
//
//
//		//ResourceManager.SharedInstance.RegisterForOnAssetBundleLoadSuccess(OnAssetBundleLoadedSuccess);
//		
//		//sortedDataList = SortGridItemsByPriority(Store.StoreItems);	
//		//myDataList.Clear();
//		
//
//
//			
//				//GamePlayer.SharedInstance.StartFastTravel(environmentID, 1000f);	// enable fast travel
//				//UIManagerOz.SharedInstance.okayDialog.ShowOkayDialog(locKey, "Lbl_PressPlayToTravel", "Btn_Ok");
//				//button.GetComponent<BoxCollider>().enabled = false;		// disable button once fast travel is selected
//
//			
////			if (data._data.id == EnvironmentSetManager.DarkForestId)
////				StartDownload(EnvironmentSetManager.DarkForestId, true);
////			else if (data._data.id == EnvironmentSetManager.YellowBrickRoadId)
////				StartDownload(EnvironmentSetManager.YellowBrickRoadId, true);
//			
//			//button.GetComponent<BoxCollider>().enabled = false;		// disable button once download is selected
//		
////	public void OnAssetBundleLoadedSuccess(string assetBundleName, int version, bool downloadOnly)
////	{	
////		UIManagerOz.SharedInstance.mapVC.RefreshIsAvailableStatusAndButtonCollidersAmongOtherThings();
////	}	
//
//	// EnvironmentSetManager.SharedInstance.CurrentEnvironmentSet.SetId
//
//	
////	public List<StoreItem> GetMyStoreItemsBasedOnType(StoreItemType myType)
////	{
////		List<StoreItem> itemsList = new List<StoreItem>();
////		
////		foreach (StoreItem storeItem in sortedDataList)
////		{
////			if (storeItem.itemType == myType)
////				itemsList.Add(storeItem);
////		}
////		
////		return itemsList;
////	}
//
//	
//	//List<StoreItem> sortedDataList = new List<StoreItem>();
//	//List<StoreItem> myDataList = new List<StoreItem>();	
//	//private StoreCellData storeItemToPurchase;	// temp reference, for use when purchasing a specific item
//	
//
//	/*
//	private void OnPurchaseNo()
//	{
//		UIConfirmDialogOz.onNegativeResponse -= OnPurchaseNo;
//		UIConfirmDialogOz.onPositiveResponse -= OnPurchaseYes;
//	}
//	
//	private void OnPurchaseYes()
//	{
//		// set up shorter local identifiers, to keep code easy to read
//		UIIAPViewControllerOz viewContScript = viewController.GetComponent<UIIAPViewControllerOz>();
//		UIIAPMiniViewControllerOz miniViewContScript = viewController.GetComponent<UIIAPMiniViewControllerOz>();	// fopr mini store
//		PlayerStats playerStats = GameProfile.SharedInstance.Player;
//		
//		playerStats.PurchaseStoreItem(storeItemToPurchase._data.id);	// buy it if we can afford it
//		
//		if (viewContScript != null)
//			viewContScript.UpdateCurrency();							// will update coin and gem counts in UI				
//		else if (miniViewContScript != null)
//			miniViewContScript.UpdateCurrency();						// for mini store
//		
//		//consumableToPurchase.Refresh();								// ask cell to update its GUI rendering to match data, in case it was updated in the transaction		
//		UIConfirmDialogOz.onNegativeResponse -= OnPurchaseNo;
//		UIConfirmDialogOz.onPositiveResponse -= OnPurchaseYes;
//	}		
//	
//	public void OnStoreCellPressed(GameObject cell) 
//	{
//		// set up shorter local identifiers, to keep code easy to read
//		UIIAPViewControllerOz viewContScript = viewController.GetComponent<UIIAPViewControllerOz>();	
//		UIIAPMiniViewControllerOz miniViewContScript = viewController.GetComponent<UIIAPMiniViewControllerOz>();	// for mini store
//		//StoreCellData storeCellData = cell.transform.parent.GetComponent<StoreCellData>();
//		StoreCellData storeCellData = cell.transform.parent.transform.parent.GetComponent<StoreCellData>();
//		int storeItemID = storeCellData._data.id;
//		CharacterStats activeCharacter = GameProfile.SharedInstance.GetActiveCharacter();
//		PlayerStats playerStats = GameProfile.SharedInstance.Player;
//
//		storeItemToPurchase = storeCellData;
//		UIConfirmDialogOz.onNegativeResponse += OnPurchaseNo;
//		UIConfirmDialogOz.onPositiveResponse += OnPurchaseYes;
//			
//		Decimal cost = storeCellData._data.cost / 100.00M;
//	 	//string costString = String.Format("{0:C}", cost);
//		
//		if (miniViewContScript != null)
//			UIConfirmDialogOz.onNegativeResponse += CancelContinue;			// if in resurrect menu, go straight back into the post-game if gems not purchased
//			UIConfirmDialogOz.onPositiveResponse += DoContinue; 			// if in resurrect menu, go straight back into the game if gems purchased
//		
//		UIManagerOz.SharedInstance.confirmDialog.ShowConfirmPurchaseDialog(storeCellData._data.title, "StoreConfirmPurchasePrompt", "No", "Yes", cost);
//
//		if (viewContScript != null)
//			viewContScript.UpdateCurrency();								// will update coin and gem counts in UI				
//		else if (miniViewContScript != null)
//			miniViewContScript.UpdateCurrency();							// for mini store
//		
//		storeCellData.Refresh();											// ask cell to update its GUI rendering to match data, in case it was updated in transaction
//	}
//	
//	public void DoContinue() 	// if in-game, in resurrect menu, on positive response, kill the mini store and continue immediately
//	{
//		UIConfirmDialogOz.onNegativeResponse -= CancelContinue;
//		UIConfirmDialogOz.onPositiveResponse -= DoContinue;		
//		
//		UIIAPMiniViewControllerOz miniViewContScript = viewController.GetComponent<UIIAPMiniViewControllerOz>();	// for mini store	
//		
//		if (miniViewContScript != null)
//		{
//			miniViewContScript.disappear();
//			
//			// you bought some gems, so continue with resurrect immediately.
//			miniViewContScript.inGameVC.resurrectMenu.OnResurrect();
//		}
//	}
//	
//	public void CancelContinue() 	// if in-game, in resurrect menu, on negative response, kill the mini store and go to end game
//	{
//		UIConfirmDialogOz.onNegativeResponse -= CancelContinue;
//		UIConfirmDialogOz.onPositiveResponse -= DoContinue; 			
//		
//		UIIAPMiniViewControllerOz miniViewContScript = viewController.GetComponent<UIIAPMiniViewControllerOz>();	// for mini store	
//		
//		if (miniViewContScript != null)
//		{
//			miniViewContScript.disappear();			
//			
//			//if you don't want any gems, just die. no resurrection for you.
//			miniViewContScript.inGameVC.resurrectMenu.chooseToResurrect = false;
//	 		miniViewContScript.inGameVC.OnDiePostGame();
//		}
//	}	
//	
//	public void DoGatchaSomething() 	// if in-game, in gatcha screen, on positive response, do something
//	{
//		UIIAPMiniViewControllerOz miniViewContScript = viewController.GetComponent<UIIAPMiniViewControllerOz>();	// for mini store	
//		
//		if (miniViewContScript != null)
//		{
//			miniViewContScript.inGameVC.resurrectMenu.OnResurrect();
//		}
//	}	
//	
//	public void OnAlreadyMaxedOut() 
//	{
//		UIOkayDialogOz.onPositiveResponse -= OnAlreadyMaxedOut;
//	}		
//	
//	//void Update() { }
//}
//
//	 */
//
//		
//		//if (playerStats.IsConsumableMaxedOut(consumableID) == false) 	// check if already purchased maximum allowed
//		//{
////			if (playerStats.CanAffordConsumable(consumableID) == true)
////			{
//
//					//"Purchase the " + storeCellData._data.title + " for " + costString + "?", "No", "Yes");	
//
//				//UIManagerOz.SharedInstance.confirmDialog.ShowConfirmDialog(storeCellData._data.title, "Purchase the "
//				//	+ storeCellData._data.title + " for " + costString + "?", "No", "Yes");	
//				//playerStats.PurchaseConsumable(consumableID);			// buy it if we can afford it
////			}
////			else
////			{
////				UIConfirmDialogOz.onNegativeResponse += viewContScript.OnNeedMoreCoinsNo;
////				UIConfirmDialogOz.onPositiveResponse += viewContScript.OnNeedMoreCoinsYes;
////				UIManagerOz.SharedInstance.confirmDialog.ShowConfirmDialog("Need More Coins!","Would you like to get more coins?", "No", "Yes");
////			}
////		}
////		else
////		{
////			UIOkayDialogOz.onPositiveResponse += OnAlreadyMaxedOut;
////			UIManagerOz.SharedInstance.okayDialog.ShowOkayDialog("You've maxed out this consumable","Maximum " 
////				+ ConsumableStore.maxOfEachConsumable.ToString() + " in inventory at once!", "OK"); 
////		}
//		
