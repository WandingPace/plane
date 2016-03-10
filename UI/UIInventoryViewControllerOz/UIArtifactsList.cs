using System.Collections.Generic;
using UnityEngine;

public class UIArtifactsList : MonoBehaviour
{
    protected static Notify notify;

    private ArtifactCellData artifactToPurchase;
        //OrGem;	// temp reference, for use when purchasing or gemming a specific item

    private List<GameObject> childCells = new List<GameObject>();
    private GameObject grid;
    // wxj
    private string iapId = "";
    private List<ArtifactProtoData> sortedDataList = new List<ArtifactProtoData>();
    public GameObject viewController;
    //private SaleBanner saleBanner;
    //private NotificationSystem notificationSystem;

    private void Awake()
    {
        notify = new Notify(GetType().Name);

        grid = gameObject; // connect to this panel's grid automatically			
        sortedDataList = SortGridItemsByPriority(ArtifactStore.Artifacts);
        Initialize();
    }

    private void Start()
    {
        //notificationSystem = Services.Get<NotificationSystem>();
        Refresh();
    }

    public void Refresh()
    {
        // ensure that the sortedDataList has been initialized
        if (sortedDataList != null && sortedDataList.Count > 0)
        {
            /*
			if (saleBanner == null)
			{
				saleBanner = viewController.GetComponentsInChildren<SaleBanner>(true).First();
			}
			
			if ( saleBanner != null )
			{
				// set status of sale banner (show only if sale is active)
				saleBanner.SetSaleBannerStatus(gameObject.GetComponent<UIPanel>(), 
					transform.parent.gameObject, DiscountItemType.Artifact, saleBanner);
			}
			*/

            sortedDataList = SortGridItemsByPriority(sortedDataList);
            var cellIndex = 0;

            foreach (var childCell in childCells)
            {
                //int id = childCell.GetComponent<ArtifactCellData>()._data._id;
                //childCell.GetComponent<ArtifactCellData>().SetData(ArtifactStore.GetArtifactProtoData(id));	//.Powers[i]);
                //childCell.GetComponent<ArtifactCellData>().SetData(ArtifactStore.Artifacts[i]);
                childCell.GetComponent<ArtifactCellData>().SetData(sortedDataList[cellIndex]);
                childCell.name = GenerateCellLabel(sortedDataList[cellIndex]);
                cellIndex++;
            }

            transform.parent.GetComponent<UIScrollView>().ResetPosition();
        }
    }

    public void Initialize()
    {
        childCells = CreateCells(); // create cell GameObject for each
//		grid.GetComponent<UIGrid>().sorted = false;	//true;
        grid.GetComponent<UIGrid>().Reposition(); // reset/correct positioning of all objects inside grid
    }

    private List<GameObject> CreateCells()
    {
        var newObjs = new List<GameObject>();

        foreach (var artifactData in sortedDataList)
        {
            var panel = CreatePanel(artifactData, grid);
            panel.name = GenerateCellLabel(artifactData);
            newObjs.Add(panel);
        }

        return newObjs;
    }

    private string GenerateCellLabel(ArtifactProtoData artifactData)
    {
        return ("Cell_" + artifactData._sortPriority.ToString("D8") + "_" + artifactData._id.ToString("D8"));
    }

    private void OnArtifactCellPressed(GameObject cell)
    {
        //Services.Get<NotificationSystem>().ClearNotification(NotificationType.Modifier, cell.transform.parent.GetComponent<ArtifactCellData>()._data._id);
    }

    public void Reposition()
    {
        grid.GetComponent<UIGrid>().Reposition();
    }

    private List<ArtifactProtoData> SortGridItemsByPriority(List<ArtifactProtoData> list) //unsortedList)
    {
        //List<ArtifactProtoData> listToSort = unsortedList.ToList();
        //listToSort = listToSort.OrderBy(x => x._sortPriority).ToList(); 
        list.Sort((a1, a2) => a1._sortPriority.CompareTo(a2._sortPriority));
        return list; //listToSort;
    }

    private GameObject CreatePanel(ArtifactProtoData _data, GameObject _grid) //, string cellName)
    {
        var obj = (GameObject) Instantiate(Resources.Load("ArtifactStoreCellOz"));
            // instantiate objective cell from prefab
        obj.transform.parent = _grid.transform;
            // null passed in for parentCell, which means this is a regular size cell
        obj.transform.localScale = Vector3.one;
        obj.transform.rotation = grid.transform.rotation;
        obj.transform.localPosition = Vector3.zero;
        obj.GetComponent<ArtifactCellData>()._data = _data; // store reference to data for this objective
        obj.GetComponent<ArtifactCellData>().viewController = viewController;
            // pass on reference to view controller, for event response
        obj.GetComponent<ArtifactCellData>().scrollList = gameObject;
        //obj.GetComponent<SubPanel>().scrollList = this.gameObject;				// pass on reference to this script's GameObject, for triggering 'reposition' requests

        // move subpanel offscreen and turn it off
        //obj.GetComponent<SubPanel>().TurnSubPanelOff(obj.transform.Find("CellContents").gameObject);
        return obj;
    }

    private void OnPurchaseYes(bool isAll = false)
    {
        // set up shorter local identifiers, to keep code easy to read
        //UIInventoryViewControllerOz invViewCont = viewController.GetComponent<UIInventoryViewControllerOz>();		
        var playerStats = GameProfile.SharedInstance.Player;

        playerStats.PurchaseArtifact(artifactToPurchase._data._id, isAll); // buy it

        UIManagerOz.SharedInstance.PaperVC.UpdateCurrency();
        artifactToPurchase.Refresh();
            // ask cell to update its GUI rendering to match data, in case it was updated in the transaction
        //invViewCont.UpdateCurrency();								// will update coin and gem counts in UI				
        //UIConfirmDialogOz.onNegativeResponse -= OnPurchaseNo;
        //UIConfirmDialogOz.onPositiveResponse -= OnPurchaseYes;
    }

    // wxj, call when click upgrade one button
    public void CellBuyOneButtonPressed(GameObject button)
    {
        var artifactCellData = button.transform.parent.parent.parent.GetComponent<ArtifactCellData>();
        artifactToPurchase = artifactCellData;

        onPurchaseByCoins(artifactCellData._data._id);
        //invViewCont.UpdateCurrency();								// will update coin and gem counts in UI				
        artifactCellData.Refresh();
    }

    //void Update() { }	


    // wxj, call when click onekey upgrade button
    public void CellBuyAllButtonPressed(GameObject button)
    {
        //debug mode
        //GameProfile.SharedInstance.Player.Reset();

        var artifactCellData = button.transform.parent.parent.parent.GetComponent<ArtifactCellData>();
        artifactToPurchase = artifactCellData;

        // if onekey function enable
        if (!artifactCellData._data._onekey_iap_id.Equals("") && !artifactCellData._data._onekey_msg_key.Equals(""))
        {
//			UIConfirmDialogOz.onNegativeResponse += onOnekeyNo;
//			UIConfirmDialogOz.onPositiveResponse += onOnekeyYes;
//			UIManagerOz.SharedInstance.onekeyPurchaseDialog.ShowConfirmDialog(artifactToPurchase._data._onekey_msg_key, "Btn_No", "Btn_Yes");
            onOnekeyYes();
        }
        else
        {
            UIManagerOz.SharedInstance.okayDialog.ShowOkayDialog("Dialog_Msg_OneKey_Disable", "Btn_Ok");
        }

        artifactCellData.Refresh();
    }

    // wxj, confirm onekey upgrade
    private void onOnekeyYes()
    {
        UIConfirmDialogOz.onNegativeResponse -= onOnekeyNo;
        UIConfirmDialogOz.onPositiveResponse -= onOnekeyYes;

        //cell iap purchase
//		GoogleIABManager.artifactPurchaseSucceedEvent += onPurchaseSucceededByRMB;
//		GoogleIABManager.artifactPurchaseFailedEvent += onPurchaseFailedByRMB;

        if (artifactToPurchase._data._id < IAPWrapper.IAPS_ONEKEY_UPG.Length)
        {
            iapId = IAPWrapper.IAPS_ONEKEY_UPG[artifactToPurchase._data._id];
            //PurchaseUtil.purchaseProduct(iapId);
        }


        //debug, purchase succeeded
        //onPurchaseSucceededByRMB(iapId);
    }

    // wxj, cancel onekey upgrade
    private void onOnekeyNo()
    {
        UIConfirmDialogOz.onNegativeResponse -= onOnekeyNo;
        UIConfirmDialogOz.onPositiveResponse -= onOnekeyYes;
    }

    // wxj, buy artifact by coins
    private void onPurchaseByCoins(int artifactID)
    {
        Debug.Log("~~~~~~~~buyArtifactByCoins");

        // set up shorter local identifiers, to keep code easy to read
        //var invViewCont = viewController.GetComponent<UIInventoryViewControllerOz>();
        //CharacterStats activeCharacter = GameProfile.SharedInstance.GetActiveCharacter();
        var playerStats = GameProfile.SharedInstance.Player;

        Services.Get<NotificationSystem>().ClearNotification(NotificationType.Modifier, artifactID);
        Services.Get<NotificationSystem>().SetNotificationIconsForThisPage(UiScreenName.UPGRADES);

        //if (playerStats.IsArtifactPurchased(artifactID) == false) 	// check if already purchased
        if (playerStats.IsArtifactMaxedOut(artifactID) == false) // check if already purchased maximum allowed
        {
            if (playerStats.CanAffordArtifact(artifactID))
            {
                //UIConfirmDialogOz.onNegativeResponse += OnPurchaseNo;
                //UIConfirmDialogOz.onPositiveResponse += OnPurchaseYes;
                //UIManagerOz.SharedInstance.confirmDialog.ShowConfirmDialog(artifactCellData.data._title, "Purchase this modifier?", "No", "Yes");
                //UIManagerOz.SharedInstance.confirmDialog.ShowConfirmDialog(artifactCellData.data._title, "ModifierConfirmPurchasePrompt", "Btn_No", "Btn_Yes");
                //playerStats.PurchaseArtifact(artifactID);			
                OnPurchaseYes(); // buy it if we can afford it
                Services.Get<MenuTutorials>().SendEvent(3); // send message to menu tutorial
            }
            else
            {
                UIManagerOz.SharedInstance.StoreVC.BuyCoins();
                //UIManagerOz.SharedInstance.confirmDialog.ShowConfirmDialog("Need More Coins!","Would you like to get more coins?", "No", "Yes");
                //UIManagerOz.SharedInstance.confirmDialog.ShowConfirmDialog("Lbl_Dialogue_MoreCoins_Prompt", "Lbl_Dialogue_MoreCoins_Confirm", "Btn_No", "Btn_Yes");
            }
        }
//		else 														// it's maxed out, so let player know that nothing more to be done here
//		{
//			UIOkayDialogOz.onPositiveResponse += OnAlreadyMaxedOut;
//			UIManagerOz.SharedInstance.okayDialog.ShowOkayDialog("You've maxed out this modifier","Maximum rank is 5!", "Btn_Ok"); 					
//		}

        UIManagerOz.SharedInstance.PaperVC.UpdateCurrency();
    }

    // wxj, callback when onekey actifact purchase succeeded by RMB
    private void onPurchaseSucceededByRMB(string iapID)
    {
        //GoogleIABManager.artifactPurchaseSucceedEvent -= onPurchaseSucceededByRMB;
        //GoogleIABManager.artifactPurchaseFailedEvent -= onPurchaseFailedByRMB;

        if (iapID.Equals(iapId))
        {
            notify.Info("wxj, onekey " + iapID + " purchase succeed~!");
            OnPurchaseYes(true);
        }
    }

    // wxj, callback when onekey actifact purchase failed by RMB
    private void onPurchaseFailedByRMB(string iapID)
    {
        //GoogleIABManager.artifactPurchaseSucceedEvent -= onPurchaseSucceededByRMB;
        //GoogleIABManager.artifactPurchaseFailedEvent -= onPurchaseFailedByRMB;
    }
}


//	public void Refresh()
//	{
//		//ClearGrid(grid);														// kill all old objects under grid, prior to refresh
//		
//		List<ArtifactProtoData> sortedDataList = SortGridItemsByPriority(ArtifactStore.Artifacts);
//		
//		foreach (ArtifactProtoData artifactData in sortedDataList)
//		{
//			//GameObject obj = CreatePanel(artifactData, grid);
//			CreatePanel(artifactData, grid).name = GenerateCellLabel(artifactData);		
//		}		
//		
//		grid.GetComponent<UITable>().sorted = true;
//		//grid.GetComponent<UIGrid>().Reposition();								// reset/correct positioning of all objects inside grid
//		grid.GetComponent<UITable>().Reposition();								// reset/correct positioning of all objects inside table
//	}


//List<StoreItem> sortedDataList = SortGridItemsByPriority(Store.StoreItems);

//CreatePanel(powerData, grid);									//GameObject obj = CreatePanel(powerData, grid);		


//	private void ClearGrid(GameObject _grid)
//	{
//		UIDragPanelContents[] contentArray = _grid.GetComponentsInChildren<UIDragPanelContents>();
//		foreach (UIDragPanelContents contents in contentArray) 
//		{ 
//			//DestroyImmediate(contents.gameObject); 
//			contents.transform.parent = null;	// unparent first to remove bug when calling NGUI's UIGrid.Reposition(), because Destroy() is not immediate!
//			Destroy(contents.gameObject); 
//		}	
//	}	


//		if (!animating)
//		{
//			animating = true;
//			selectedCell = cell.transform.parent.gameObject.GetComponent<SubPanel>().OnCellPressed(cell, selectedCell);
//		}


//	public void AnimationDone()
//	{
//		//animating = false;
//	}	


//	private void OnPurchaseNo()
//	{
//		UIConfirmDialogOz.onNegativeResponse -= OnPurchaseNo;
//		UIConfirmDialogOz.onPositiveResponse -= OnPurchaseYes;
//	}


//	public void OnAlreadyMaxedOut() 
//	{
//		UIOkayDialogOz.onPositiveResponse -= OnAlreadyMaxedOut;
//	}	

//private GameObject selectedCell = null;
//private bool animating = false;


//	private void OnGemNo()
//	{
//		UIConfirmDialogOz.onNegativeResponse -= OnGemNo;
//		UIConfirmDialogOz.onPositiveResponse -= OnGemYes;
//	}
//	
//	private void OnGemYes()
//	{
//		// set up shorter local identifiers, to keep code easy to read
//		UIInventoryViewControllerOz invViewCont = viewController.GetComponent<UIInventoryViewControllerOz>();		
//		PlayerStats playerStats = GameProfile.SharedInstance.Player;		
//		
//		playerStats.GemArtifact(artifactToPurchase.data._id);	// gem it if we can afford it
//		invViewCont.UpdateCurrency();								// will update coin and gem counts in UI				
//		artifactToPurchase.Refresh();							// ask cell to update its GUI rendering to match data, in case it was updated in the transaction		
//		UIConfirmDialogOz.onNegativeResponse -= OnGemNo;
//		UIConfirmDialogOz.onPositiveResponse -= OnGemYes;
//	}	


//UIOkayDialogOz.onPositiveResponse += OnAlreadyMaxedOut;
//UIManagerOz.SharedInstance.okayDialog.ShowOkayDialog("Oops", "ModifierAlreadyGemmed", "ButtonOk");	//-- Show error dialog	


//	public void OnAlreadyGemmed() 
//	{
//		UIOkayDialogOz.onPositiveResponse -= OnAlreadyGemmed;
//	}	


//		else if (playerStats.IsArtifactGemmed(artifactID) == false)	// check if already gemmed	
//		{ 
//			if (playerStats.CanAffordArtifactGem(artifactID) == true)
//			{
//				artifactToPurchase = artifactCellData;
//				UIConfirmDialogOz.onNegativeResponse += OnGemNo;
//				UIConfirmDialogOz.onPositiveResponse += OnGemYes;
//				//UIManagerOz.SharedInstance.confirmDialog.ShowConfirmDialog(artifactCellData.data._title, "Gem this modifier?", "No", "Yes");
//				UIManagerOz.SharedInstance.confirmDialog.ShowConfirmDialog(artifactCellData.data._title, "GemThisModifierPrompt", "ButtonNo", "ButtonYes");
//				//playerStats.GemArtifact(artifactID);				// gem it if we can afford it
//			}
//			else
//			{
//				UIOkayDialogOz.onNegativeResponse += invViewCont.OnNeedMoreGemsNo;
//				UIOkayDialogOz.onPositiveResponse += invViewCont.OnNeedMoreGemsYes;
//				//UIManagerOz.SharedInstance.okayDialog.ShowOkayDialog("Need More Gems!","Pick up gems while running.", "OK");
//				UIManagerOz.SharedInstance.okayDialog.ShowOkayDialog("Need More Gems!", "NeedMoreGemsDescription", "ButtonOk");
//			}
//		}


//	private void OnArtifactCellPressed(GameObject cell)
//	{
//		SubPanel subPanel = cell.transform.parent.gameObject.GetComponent<SubPanel>();
//		
//		if (cell == selectedCell)		// just close it
//		{
//			subPanel.ResizeCell(selectedCell, false);
//			selectedCell = null;	
//		}
//		else
//		{
//			if (selectedCell != null)	// is there a selected cell? If so, close it.
//				subPanel.ResizeCell(selectedCell, false);
//			
//			// open the clicked cell
//			subPanel.ResizeCell(cell, true);
//			selectedCell = cell;
//		}
//	}


//int max = ArtifactStore.GetNumberOfArtifacts();
//for (int i = 0; i < max; i++) 
//{
//ArtifactProtoData artifactData = ArtifactStore.GetArtifactProtoData(i);


//			if (artifactData == null)
//			{
//				TR.WARN("artifact proto data for " + i + " is null");
//				continue;
//			}

//	//-----------------------------
//	
//	private int equippedArtifact = -1;
//	private int equippedPower = -1;
//	
//	public void SetEquippedArtifact(int artifactID) 
//	{
//		TR.LOG ("SetEquippedArtifact {0}", artifactID);
//		equippedArtifact = artifactID;
//		equippedPower = -1;
//		//UpdateEquippedCell("Pick an Artifact");
//	}
//	
//	private void UpdateArtifacts() 
//	{
//		//CellData[] cells = grid.GetComponentsInChildren<CellData>(true) as CellData[];
//		//foreach (CellData item in cells) 
//		//{
//		//	if (item == null) { continue; }
//		//	UpdateArtifactCellData(item, player, activeCharacter);
//		//}
//	}	
//	
//	//private void UpdateArtifactCellData(CellData cellData, PlayerStats player, CharacterStats activeCharacter) 
//	private void UpdateArtifactCellData(GameObject cell)
//	{
//		PlayerStats player = GameProfile.SharedInstance.Player;
//		CharacterStats activeCharacter = GameProfile.SharedInstance.GetActiveCharacter();
//		
//		//if (cellData == null) { return; }
//		
//		//GameObject newCell = cellData.gameObject;
//		//int artifactID = cellData.Data;
//		int artifactID = cell.transform.parent.GetComponent<ArtifactCellData>().data._id;
//		
//		ArtifactProtoData protoData = ArtifactStore.Artifacts[artifactID];
//		bool purchased = player.IsArtifactPurchased(artifactID);
//		bool equipped = activeCharacter.isArtifactEquipped(artifactID);
//		
//		//UpdateCellData(newCell, protoData._title, protoData._iconName, protoData._description, protoData._cost.ToString(), purchased, equipped);
//		UpdateCellData(cell, protoData._title, protoData._iconName, protoData._description, protoData._cost.ToString(), purchased, equipped);	
//	}	
//	
//	
//	private void UpdateCellData(GameObject newCell, string title, string iconName, string description, string coinValue, bool purchased, bool equipped)
//	{
//		GameObject go = HierarchyUtils.GetChildByName("Title", newCell);
//		if (go != null) 
//		{
//			UILabel titleLabel = go.GetComponent<UILabel>() as UILabel;
//			if (titleLabel != null) { titleLabel.text = title; }//titleLabel.color = ArtifactStore.colorForRarity(protoData._rarity);
//		}
//		
//		go = HierarchyUtils.GetChildByName("Icon", newCell);
//		if (go != null) 
//		{
//			UISprite iconSprite = go.GetComponent<UISprite>() as UISprite;
//			if (iconSprite != null) { iconSprite.spriteName = iconName; }
//		}
//		
//		go = HierarchyUtils.GetChildByName("Description", newCell);
//		if (go != null) 
//		{
//			UILabel desc = go.GetComponent<UILabel>() as UILabel;
//			if (desc != null) { desc.text = description; }
//		}
//		
//		go = HierarchyUtils.GetChildByName("Cost", newCell);
//		if (go != null) 
//		{
//			GameObject buyButton = HierarchyUtils.GetChildByName("BuyButton", newCell);
//			GameObject CoinDisplayIcon = HierarchyUtils.GetChildByName("CoinDisplayIcon", newCell);
//			UILabel cost = go.GetComponent<UILabel>() as UILabel;
//			if (cost != null)
//			{
//				if (purchased == true) 
//				{
//					if(CoinDisplayIcon != null) { NGUITools.SetActive(CoinDisplayIcon, false); }
//					
//					if (equipped == true) 
//					{
//						cost.text = "equipped";
//						if (buyButton != null) { NGUITools.SetActive(buyButton, false); }
//					}
//					else 
//					{
//						cost.text = "available";	//equip";
//						if (buyButton != null) { NGUITools.SetActive(buyButton, true); }
//					}
//				}
//				else 
//				{
//					if (buyButton != null) { NGUITools.SetActive(buyButton, true); }
//					cost.text = coinValue;
//					if (CoinDisplayIcon != null) { NGUITools.SetActive(CoinDisplayIcon, true); }
//				}
//			}
//		}
//	}
//}
//	

//UpdateArtifactCellData(cell);	//, playerStats, activeCharacter);	//UpdateArtifactCellData(cellData, playerStats, activeCharacter);

//	
//	public void OnArtifactCellPressed(GameObject cell) 
//	{
//		Debug.LogWarning("OnArtifactCellPressed called!");
//		
//		//-- Can we equip this?
//		bool closeDialog = false;
//		if(cell != null /*&& equipInSlot != ArtifactSlotType.Total*/) {
//			//-- equip it
//			
//			CellData cellData = cell.GetComponent<CellData>() as CellData;
//			if(cellData != null) {
//				int artifactID = cellData.Data;
//				CharacterStats activeCharacter = GameProfile.SharedInstance.GetActiveCharacter();
//				PlayerStats playerStats = GameProfile.SharedInstance.Player;
//				
//				//We don't care about "Equipping" in Oz. Just equip it. 
//			//	SetEquippedArtifact(activeCharacter.getArtifactForSlot(artifactID));
//				
//				if(playerStats.IsArtifactPurchased(artifactID) == false) {
//					//-- Can we afford it?
//					if(playerStats.CanAffordArtifact(artifactID) == false) {
//						UIConfirmDialogOz.onNegativeResponse += viewController.GetComponent<UIInventoryViewControllerOz>().OnNeedMoreCoinsNo;
//						UIConfirmDialogOz.onPositiveResponse += viewController.GetComponent<UIInventoryViewControllerOz>().OnNeedMoreCoinsYes;
//						UIManagerOz.SharedInstance.confirmDialog.ShowConfirmDialog("Need More Coins!","Would you like to get more coins?", "No", "Yes");
//						return;
//					}
//					
//					//-- Buy it if we can afford it.
//					playerStats.PurchaseArtifact(artifactID);
//					UpdateArtifactCellData(cellData, playerStats, activeCharacter);
//					viewController.GetComponent<UIInventoryViewControllerOz>().UpdateCurrency();
//				}
//				
//				// code below commented out by Alex, because it's not needed for Artifacts, since we can't equip them
//				/*
//				else if(GameProfile.SharedInstance.IsArtifactEquipped(artifactID, activeCharacter.characterId) == false) {
//					activeCharacter.equipArtifactForSlot(artifactID, equipInSlot);
//					if(characterSelectVC != null) {
//						characterSelectVC.UpdateCharacterCard(activeCharacter);	
//					}
//						
//					equippedArtifact = artifactID;
//					UpdateEquippedCell();
//					UpdateArtifactCellData(cellData, playerStats, activeCharacter);
//					GameProfile.SharedInstance.Serialize();
//					//closeDialog = true;
//				}
//				else {
//					//-- Can't equip this because its already equipped.
//					//-- Show error dialog
//					UIConfirmDialogOz.onPositiveResponse += OnAlreadyEquipped;
//					UIManagerOz.SharedInstance.confirmDialog.ShowConfirmDialog("Oops!", "That Artifact is already in use.", "Ok", "");
//					return;
//				}	
//				*/
//			}
//		}	
//		
//		if (closeDialog == true) { viewController.GetComponent<UIInventoryViewControllerOz>().OnBackButton(); }
//	}

//	public void OnGemItemPressed() 
//	{
//		int itemID = -1;
//		Buff newBuff = null;
//		if (equippedArtifact != -1) { return; }
//		
//		itemID = equippedPower;
//		if (itemID == -1) { return; }
//		
//		BasePower basePower = PowerStore.Powers[itemID];
//		if (basePower == null) { return; }
//		newBuff = new Buff(basePower.ProtoBuff.ToDict());
//		newBuff.itemID = itemID;
//		
//		int gemcost = GameProfile.SharedInstance.Player.GetBuffCost(BuffType.Powerup, itemID, newBuff);
//		if (gemcost < 0) { gemcost = 1; }
//		
//		if(GameProfile.SharedInstance.Player.specialCurrencyCount < gemcost)
//		{
//			UIConfirmDialogOz.onNegativeResponse += OnNeedMoreGemsNo;
//			UIConfirmDialogOz.onPositiveResponse += OnNeedMoreGemsYes;
//			UIManagerOz.SharedInstance.confirmDialog.ShowConfirmDialog("Need More Gems!","Would you like to get more Gems?", "No", "Yes");
//			return;
//		}
//		
//		//-- Augment the power.
//		GameProfile.SharedInstance.Player.specialCurrencyCount -= gemcost;
//		if (GameProfile.SharedInstance.Player.specialCurrencyCount < 0) { GameProfile.SharedInstance.Player.specialCurrencyCount = 0; }
//		GameProfile.SharedInstance.Player.CreateBuff(BuffType.Powerup, itemID, basePower.ProtoBuff);
//		GameProfile.SharedInstance.Serialize();
//		UpdateEquippedCell(null);
//		updateCurrency();
//	}	