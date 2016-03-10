//using UnityEngine;
//using System.Collections;
//using System.Collections.Generic;
//
//public class UIMapViewControllerOz : UIViewControllerOz
//{
//	public List<GameObject> mapPanelGOs = new List<GameObject>();
//	public List<UISprite> mapPinSprites = new List<UISprite>();
//	
//	private int currentLocation = EnvironmentSetManager.WhimsyWoodsId;	
//	
//	private List<MapLocation> mapLocationsDataList = null;
//	
//	private int locationBeingShown = EnvironmentSetManager.WhimsyWoodsId;	
//	
//	//public int downloadingLocationID = -1;	// id of location being downloaded.  -1 = none (moved to DownloadManager now)
//	public int fastTravelID = -1;			// id of location being fast traveled to.  -1 = none	
//	
//	public override void appear()
//	{
//		base.appear();
//		
//		currentLocation = EnvironmentSetManager.SharedInstance.CurrentEnvironmentSet.SetId;
//		
//		if (mapLocationsDataList == null)
//			mapLocationsDataList = InitializeMapLocationDataList();
//		
//		// set all panels inactive on start 
//		foreach (GameObject go in mapPanelGOs)
//			NGUITools.SetActive(go, false);
//		
//		NGUITools.SetActive(mapPanelGOs[currentLocation], true);			
//				
//		// refresh panels prior to showing
//		foreach (GameObject go in mapPanelGOs)
//			go.GetComponent<UIMapList>().Refresh(RefreshIsAvailableStatus());
//		
////		HeaderLocationName.GetComponent<UILocalize>().SetKey(GetLocalizationKeyForLocation(currentLocation)); - remove step N.N.
////		HeaderLocationSubtext.GetComponent<UILocalize>().SetKey("Ttl_Sub_CurLoc");	// Ttl_Sub_DownloadFree	
////		SpriteLocationIcon.GetComponent<UISprite>().spriteName = GetIconForLocation(currentLocation);
//		
//		// go to page showing your current location, when you go into the map
//		locationBeingShown = currentLocation;
//		//GoToMapLocation(locationBeingShown); - remove step N.N.		
//		SetPinButtonsActiveStatus(true);
//		SetPinColors();
//		
//		Services.Get<AppCounters>().AddMapView();	//UIManagerOz.SharedInstance.AddMapView();
////		notify.Debug("countMapViews = " + UIManagerOz.SharedInstance.countMapViews);
//		
//		if (Services.Get<AppCounters>().countMapViews % 5 == 0 && !(DownloadManager.HaveAllLocationsBeenDownloaded()))		
//		{
//			//UIDownloadDialogOz.onPositiveResponse += ShowConfirmDialog;
//			NGUITools.SetActive(UIManagerOz.SharedInstance.downloadDialog.gameObject, true);
//			//UIManagerOz.SharedInstance.downloadDialogVC.appear();		// show periodic dialog prompt to download additional locations, if not done yet
//		}
//	}
//	
//	public void SetPinButtonsActiveStatus(bool active)
//	{
//		foreach (UISprite buttonSprite in mapPinSprites)
//			buttonSprite.gameObject.GetComponent<BoxCollider>().enabled = active;
//	}
//	
//	public void SelectSubPanel(GameObject pin)	//ToggleSubPanel(GameObject pin)
//	{
//		switch (pin.name)
//		{
//			case "SpritePinWW":
//				locationBeingShown = EnvironmentSetManager.WhimsyWoodsId;
//				break;
//			case "SpritePinDF":
//				locationBeingShown = EnvironmentSetManager.DarkForestId;
//				break;
//			case "SpritePinWC":
//				locationBeingShown = EnvironmentSetManager.YellowBrickRoadId;
//				break;			
//		}
//
//		CloseMapLocationPanels();	
//		
//		NGUITools.SetActive(mapPanelGOs[locationBeingShown], true);	// open this subpanel
//		mapLocationsDataList = RefreshIsAvailableStatus();			// refresh 'is available' status for each location
//		mapPanelGOs[locationBeingShown].GetComponent<UIMapList>().Refresh(mapLocationsDataList); // refresh the panel to be displayed
//	}
//
//	public void CloseMapLocationPanels()
//	{
//		foreach (GameObject go in mapPanelGOs)
//			NGUITools.SetActive(go, false);
//	}	
//	
//	public void SetPinColors()
//	{
//		for (int i=1; i<=3; i++)
//		{
//			if (i == currentLocation)	//EnvironmentSetManager.SharedInstance.IsLocallyAvailable(i))
//				mapPinSprites[i].spriteName = "map_pin_green";
//			else
//				mapPinSprites[i].spriteName = "map_pin_blue";
//			
//			mapPinSprites[i].color = Color.white;	// reset to white (not grayed out)
//		}
//	}
//	
//	public List<MapLocation> GetLocationDataList()
//	{
//		return mapLocationsDataList;
//	}
//	
//	private List<MapLocation> RefreshIsAvailableStatus()
//	{
//		if (mapLocationsDataList != null)
//		{
//			foreach (MapLocation loc in mapLocationsDataList)
//				loc.isLocallyAvailable = EnvironmentSetManager.SharedInstance.IsLocallyAvailable(loc.id);
//		}
//		
//		return mapLocationsDataList;
//	}
//	
//	public void RefreshIsAvailableStatusAndButtonCollidersAmongOtherThings()
//	{
//		mapLocationsDataList = RefreshIsAvailableStatus();			// refresh 'is available' status for each location
//		mapPanelGOs[locationBeingShown].GetComponent<UIMapList>().Refresh(mapLocationsDataList); // refresh the panel to be displayed		
//	}
//	
//	private List<MapLocation> InitializeMapLocationDataList()
//	{
//		mapLocationsDataList = new List<MapLocation>();
//		
//		MapLocation newMapLocWW = new MapLocation();	// ww
//		newMapLocWW.title = GetLocalizationKeyForLocation(EnvironmentSetManager.WhimsyWoodsId);
//		newMapLocWW.cost = 12000;
//		newMapLocWW.icon = GetIconForLocation(EnvironmentSetManager.WhimsyWoodsId);
//		newMapLocWW.id = EnvironmentSetManager.WhimsyWoodsId;
//		newMapLocWW.isLocallyAvailable = EnvironmentSetManager.SharedInstance.IsLocallyAvailable(EnvironmentSetManager.WhimsyWoodsId);
//		mapLocationsDataList.Add(newMapLocWW);
//				
//		MapLocation newMapLocDF = new MapLocation();	// df
//		newMapLocDF.title = GetLocalizationKeyForLocation(EnvironmentSetManager.DarkForestId);
//		newMapLocDF.cost = 10000;
//		newMapLocDF.icon = GetIconForLocation(EnvironmentSetManager.DarkForestId);
//		newMapLocDF.id = EnvironmentSetManager.DarkForestId;
//		newMapLocDF.isLocallyAvailable = EnvironmentSetManager.SharedInstance.IsLocallyAvailable(EnvironmentSetManager.DarkForestId);
//		mapLocationsDataList.Add(newMapLocDF);
//		
//		MapLocation newMapLocYBR = new MapLocation();	// ybr
//		newMapLocYBR.title = GetLocalizationKeyForLocation(EnvironmentSetManager.YellowBrickRoadId);
//		newMapLocYBR.cost = 15000;
//		newMapLocYBR.icon = GetIconForLocation(EnvironmentSetManager.YellowBrickRoadId);
//		newMapLocYBR.id = EnvironmentSetManager.YellowBrickRoadId;
//		newMapLocYBR.isLocallyAvailable = EnvironmentSetManager.SharedInstance.IsLocallyAvailable(EnvironmentSetManager.YellowBrickRoadId);
//		mapLocationsDataList.Add(newMapLocYBR);		
//		
//		return mapLocationsDataList;
//	}
//	
//	public string GetIconForLocation(int environmentID)
//	{
//		switch (environmentID)
//		{
//			case EnvironmentSetManager.WhimsyWoodsId:
//				return "icon_map_whimsiewoods";
//			case EnvironmentSetManager.DarkForestId:
//				return "icon_map_darkforest";
//			case EnvironmentSetManager.YellowBrickRoadId:
//				return "icon_map_winkiecountry";	
//		}
//		return "icon_map_whimsiewoods";
//	}
//	
//	public string GetLocalizationKeyForLocation(int environmentID)
//	{
//		switch (environmentID)
//		{
//			case EnvironmentSetManager.WhimsyWoodsId:
//				return "Loc_WhimsieWoods";
//			case EnvironmentSetManager.DarkForestId:
//				return "Loc_DarkForest";
//			case EnvironmentSetManager.YellowBrickRoadId:
//				return "Loc_WinkieCountry";
////		case EnvironmentSetManager.WhimsyWoodsId:
////				return "Loc_ChinaTown";
////		case EnvironmentSetManager.WhimsyWoodsId:
////				return "Loc_EmeraldCity";
////		case EnvironmentSetManager.WhimsyWoodsId:
////				return "Loc_GlindasCastle";
//		}
//		return "";
//	}
//	
//	public void OnNeedMoreCoinsNo() // use in main menu area only, goes to full store
//	{
//		//UIConfirmDialogOz.ClearEventHandlers();
//		UIConfirmDialogOz.onNegativeResponse -= OnNeedMoreCoinsNo;
//		UIConfirmDialogOz.onPositiveResponse -= OnNeedMoreCoinsYes;
//	}
//	
//	public void OnNeedMoreCoinsYes() // use in main menu area only, goes to full store
//	{
//		//UIConfirmDialogOz.ClearEventHandlers();
//		UIConfirmDialogOz.onNegativeResponse -= OnNeedMoreCoinsNo;
//		UIConfirmDialogOz.onPositiveResponse -= OnNeedMoreCoinsYes;
//		//notify.Info ("FAKE BUY IAP FOR 1000 coins");
//		//GameProfile.SharedInstance.Player.coinCount += 1000;
//		//updateCurrency();
//		
//		// send player to store
//		//UIManagerOz.SharedInstance.PaperVC.GoToStore(ShopScreenName.Coins);
//	}		
//}
//
//
//
//
////	public void UpdateCurrency()
////	{
////		this.updateCurrency();
////	}	
////}
//
//	
////	public GameObject HeaderLocationName;
////	public GameObject HeaderLocationSubtext;	remove step N.N.
////	public GameObject SpriteLocationIcon;
////	public GameObject ArrowIcon;
//	
//		
////		if (mapPanelGOs[locationBeingShown].active == true)
////		{
////			//NGUITools.SetActive(mapPanelGOs[currentLocation], false);
////			CloseMapLocationPanels();			// close all subpanels
////			//SetPinButtonsActiveStatus(true);	// reactivate pin buttons - remove step N.N.
////			//SetArrowDirection(false); - remove step N.N.
////		}
////		else
////		{
//		
//		
//			//SetPinButtonsActiveStatus(false); - remove step N.N.
//			//SetArrowDirection(true); - remove step N.N.
////		}		
//
//		
////	public void OnPinClick(GameObject pin) - remove step N.N.
////	{
////		switch (pin.name)
////		{
////			case "SpritePinWW":
////				locationBeingShown = EnvironmentSetManager.WhimsyWoodsId;
////				break;
////			case "SpritePinDF":
////				locationBeingShown = EnvironmentSetManager.DarkForestId;
////				break;
////			case "SpritePinWC":
////				locationBeingShown = EnvironmentSetManager.YellowBrickRoadId;
////				break;			
////		}
////		
////		//SetPinButtonsActiveStatus(false);
////		GoToMapLocation(locationBeingShown);
////	}
//	
////	private void SetArrowDirection(bool up) - remove step N.N.
////	{
////		float mult = up ? -1.0f : 1.0f;
////		
////		iTween.ScaleTo(ArrowIcon, new Vector3(ArrowIcon.transform.localScale.x, 46.0f * mult, 1.0f), 0.1f);	
////	}
//	
//
////	public void GoToMapLocation(int locationID) - remove N.N.
////	{
////		//HeaderLocationName.GetComponent<UILocalize>().SetKey(GetLocalizationKeyForLocation(locationID)); - remove step N.N.
////		
////		if (locationID == currentLocation)
////			LabelDescription.GetComponent<UILocalize>().SetKey("Ttl_Sub_CurLoc");	// Ttl_Sub_DownloadFree			
////		//else if (EnvironmentSetManager.SharedInstance.IsLocallyAvailable(locationID) == false)
////		//		HeaderLocationSubtext.GetComponent<UILocalize>().SetKey("Lbl_Download");	//Ttl_Sub_DownloadFree");
////		//else
////		//	HeaderLocationSubtext.GetComponent<UILocalize>().SetKey("Lbl_FastTravel");
////		else
////			LabelDescription.GetComponent<UILabel>().text = "Lbl_FastTravel";
////			// clear it if it's not the current location
////		
////		//SpriteLocationIcon.GetComponent<UISprite>().spriteName = GetIconForLocation(locationID); - remove step N.N.
////
////		//CloseMapLocationPanels();		
////		//NGUITools.SetActive(mapPanelGOs[locationID], true);		// open this location subpanel	
////		//SetArrowDirection(true);
////	}
//
//
//
//	
////	public void SwitchToPanel(int environmentID)	// activate panel upon button selection, passing in 'int environmentID'
////	{
////		locationID = environmentID;
////	}
//	
//
////	public void OnWhimsieNo()
////	{
////		UIConfirmDialogOz.onNegativeResponse -= OnWhimsieNo;
////		UIConfirmDialogOz.onPositiveResponse -= OnWhimsieYes;
////	}
////	
////	public void OnWhimsieYes()
////	{
////		OnWhimsieNo();	// disconnect handlers
////		
////		//NGUITools.SetActive(buttonWhimsieWoods.gameObject, false);
////		GamePlayer.SharedInstance.StartFastTravel(EnvironmentSetManager.WhimsyWoodsId, 1000f);
////		UIManagerOz.SharedInstance.okayDialog.ShowOkayDialog("Whimsie Woods", "Press play to travel", "Btn_Ok");
////	}
////	
////	public void OnDarkNo()
////	{
////		UIConfirmDialogOz.onNegativeResponse -= OnDarkNo;
////		UIConfirmDialogOz.onPositiveResponse -= OnDarkYes;
////	}
////	
////	public void OnDarkYes()
////	{
////		OnDarkNo();		// disconnect handlers
////		
////		//NGUITools.SetActive(buttonDarkForest.gameObject, false);	
////		GamePlayer.SharedInstance.StartFastTravel(EnvironmentSetManager.DarkForestId, 1000f);
////		UIManagerOz.SharedInstance.okayDialog.ShowOkayDialog("The Dark Forest", "Press play to travel", "Btn_Ok");
////	}
////	
////	public void OnWinkieNo()
////	{
////		UIConfirmDialogOz.onNegativeResponse -= OnWinkieNo;
////		UIConfirmDialogOz.onPositiveResponse -= OnWinkieYes;
////	}
////	
////	public void OnWinkieYes()
////	{
////		OnWinkieNo();	// disconnect handlers
////		
////		//NGUITools.SetActive(buttonWinkieCountry.gameObject, false);	
////		GamePlayer.SharedInstance.StartFastTravel(EnvironmentSetManager.YellowBrickRoadId, 1000f);
////		UIManagerOz.SharedInstance.okayDialog.ShowOkayDialog("Winkie Country", "Press play to travel", "Btn_Ok");
////	}	
////	
////	public void OnWhimsieClicked(GameObject button)
////	{
////		UIConfirmDialogOz.onNegativeResponse += OnWhimsieNo;
////		UIConfirmDialogOz.onPositiveResponse += OnWhimsieYes;
////		UIManagerOz.SharedInstance.confirmDialog.ShowConfirmDialog("Whimsie Woods", "Activate fast travel?", "Btn_No", "Btn_Yes");
////	}
////	
////	public void OnDarkClicked(GameObject button)
////	{
////		UIConfirmDialogOz.onNegativeResponse += OnDarkNo;
////		UIConfirmDialogOz.onPositiveResponse += OnDarkYes;
////		UIManagerOz.SharedInstance.confirmDialog.ShowConfirmDialog("The Dark Forest","Activate fast travel?", "Btn_No", "Btn_Yes");
////	}
////	
////	public void OnYellowClicked(GameObject button)
////	{
////		UIConfirmDialogOz.onNegativeResponse += OnWinkieNo;
////		UIConfirmDialogOz.onPositiveResponse += OnWinkieYes;
////		UIManagerOz.SharedInstance.confirmDialog.ShowConfirmDialog("Winkie Country","Activate fast travel?", "Btn_No", "Btn_Yes");
////	}	
////}
//
//
//
//	
//
//
//
//	/*
//	
//	// " (map_pin_green)";
//	// map_pin_grey
//	
//	public override void appear() 
//	{
//		base.appear();
//		
//		if (EnvironmentSetManager.SharedInstance.CurrentEnvironmentSet.SetId == EnvironmentSetManager.WhimsyWoodsId)
//		{
//			NGUITools.SetActive(buttonWhimsieWoods, false);		
//			NGUITools.SetActive(buttonDarkForest, true);
//			NGUITools.SetActive(buttonWinkieCountry, true);
//			
//			NGUITools.SetActive(locIconWhimsieWoods, true);
//			NGUITools.SetActive(locIconDarkForest, false);
//			NGUITools.SetActive(locIconWinkieCountry, false);
//		}
//		else if (EnvironmentSetManager.SharedInstance.CurrentEnvironmentSet.SetId == EnvironmentSetManager.DarkForestId)
//		{
//			NGUITools.SetActive(buttonWhimsieWoods, true);			
//			NGUITools.SetActive(buttonDarkForest, false);
//			NGUITools.SetActive(buttonWinkieCountry, true);
//			
//			NGUITools.SetActive(locIconWhimsieWoods, false);
//			NGUITools.SetActive(locIconDarkForest, true);
//			NGUITools.SetActive(locIconWinkieCountry, false);			
//		}	
//		else if (EnvironmentSetManager.SharedInstance.CurrentEnvironmentSet.SetId == EnvironmentSetManager.YellowBrickRoadId)
//		{
//			NGUITools.SetActive(buttonWhimsieWoods, true);			
//			NGUITools.SetActive(buttonDarkForest, true);
//			NGUITools.SetActive(buttonWinkieCountry, false);
//			
//			NGUITools.SetActive(locIconWhimsieWoods, false);
//			NGUITools.SetActive(locIconDarkForest, false);
//			NGUITools.SetActive(locIconWinkieCountry, true);			
//		}
//	}
//
////	private void ActivatePanel(GameObject _panel)
////	{
////		if (_panel == darkPanel)
////		{
////			NGUITools.SetActive(darkPanel, true);
////			NGUITools.SetActive(whimsiePanel, false);
////		}
////		else if (_panel == whimsiePanel)
////		{
////			NGUITools.SetActive(darkPanel, false);
////			NGUITools.SetActive(whimsiePanel, true);
////		}
////			
////		
////		activePanel = _panel;
////	}		
//		
////	public void OnActivateDark(bool _value)
////	{
////		NGUITools.SetActive(darkPanel, _value);
////		if (_value) { activePanel = darkPanel; }
////	}	
////	
////	public void OnActivateWhimsie(bool _value)
////	{
////		NGUITools.SetActive(whimsiePanel, _value);
////		if (_value) { activePanel = whimsiePanel; }
////	}	
//	*/
//	
//
//
//
////    public GameObject buttonWhimsieWoods;
////	public GameObject buttonDarkForest;	
////	public GameObject buttonWinkieCountry;	
////	
////    public GameObject locIconWhimsieWoods;
////	public GameObject locIconDarkForest;	
////	public GameObject locIconWinkieCountry;	
//
//
//
//
//
////		buttonDarkForest.transform.localPosition = new Vector3(50000.2159623f, 
////			buttonDarkForest.transform.localPosition.y,
////			buttonDarkForest.transform.localPosition.z);
//
//		
//		//activePanel = whimsiePanel;
//		//ActivatePanel(activePanel);
////		OnActivateWhimsie(true);
////		OnActivateDark(false);
//
//
////			buttonWhimsieForest.transform.localPosition = new Vector3(0.2159627f,
////				buttonWhimsieForest.transform.localPosition.y,
////				buttonWhimsieForest.transform.localPosition.z);			
////			
////			buttonDarkForest.transform.localPosition = new Vector3(50000.2159623f, 
////				buttonDarkForest.transform.localPosition.y,
////				buttonDarkForest.transform.localPosition.z);
//
////	
////	public UIInGameViewControllerOz inGameVC = null;	
////	public static event voidClickedHandler onPlayClickedHandler = null;
////	
////	public void OnPlayClicked() 
////	{
////		if (MainGameCamera != null) { MainGameCamera.enabled = true; }
////		disappear();
////		if(inGameVC != null) { inGameVC.appear(); } 
////		if (onPlayClickedHandler != null) { onPlayClickedHandler(); }	//-- Notify an object that is listening for this event.
////	}	
//
//	
////	public override void Start()
////	{
////		base.Start();
////	}
//
//
////		if (EnvironmentSetManager.SharedInstance.CurrentEnvironmentSet.SetId == EnvironmentSetManager.WhimsyWoodsId)
////		{
////			NGUITools.SetActive(buttonWhimsieForest.gameObject, false);		
////			NGUITools.SetActive(buttonDarkForest.gameObject, true);
////		}
////		if (EnvironmentSetManager.SharedInstance.CurrentEnvironmentSet.SetId == EnvironmentSetManager.DarkForestId)
////		{
////			NGUITools.SetActive(buttonWhimsieForest.gameObject, true);			
////			NGUITools.SetActive(buttonDarkForest.gameObject, false);
////		}	
////			
//		//activePanel = whimsiePanel;
//		//ActivatePanel(activePanel);
////		OnActivateWhimsie(true);
////		OnActivateDark(false);
//		//targetGO
//		
//		
//		//GameObject lineGO = (GameObject)Instantiate(Resources.Load("LineBetweenTransforms"));
//			
//		//LineBetweenTransforms line = lineGO.GetComponent<LineBetweenTransforms>();
//		//GameObject whimButtonGO = GameObject.Find("Image Button Whimsie");
//		//GameObject imageButtonGO = GameObject.Find("Image Button");		
//		//line.SetLineEnds(whimButtonGO.transform, imageButtonGO.transform);