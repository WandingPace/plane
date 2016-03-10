using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum ShopScreenName { Gems, Coins, CoinsGems, }	//Misc, CoinOffers, }	// In 'Store' UI screens, 'Misc' page includes Wallpaper, Character and MovieTickets

public class UIIAPMiniViewControllerOz : UIViewControllerOz
{
	public List<GameObject> storePanelGOs = new List<GameObject>();
	//public GameObject miniStorePanel;
	
	public ShopScreenName pageToLoad = ShopScreenName.CoinsGems;	// page actually loaded
	public ShopScreenName pageToLoadIfMoreSpecificNeeded = ShopScreenName.CoinsGems;	// not actually loaded but used to determine if from resurrect or gatcha
	public bool comingFromResurrectMenu = false;
	//public string pageToLoad;
	
	protected override void Awake() 
	{
		base.Awake();
	}		
	
	public override void appear() 
	{
		base.appear();
		
//		UIManagerOz.SharedInstance.UICamera.GetComponent<UICamera>().clipRaycasts = false;//20150519
		
		//NGUITools.SetActive(miniStorePanel, true);	

		//storePanelGOs[(int)pageToLoad].transform.localPosition = new Vector3(0.0f, -850.0f, 
		//	storePanelGOs[(int)pageToLoad].transform.localPosition.z);
		
		// set only appropriate panel active, make others inactive
		foreach (GameObject go in storePanelGOs)
			NGUITools.SetActive(go, false);
		NGUITools.SetActive(storePanelGOs[(int)pageToLoad], true);
		
//		iTween.MoveTo(UIManagerOz.SharedInstance.IAPMiniStoreVC.gameObject, new Vector3(0.0f, -850.0f, 
//			UIManagerOz.SharedInstance.IAPMiniStoreVC.gameObject.transform.localPosition.z), 1.0f);
		
//		iTween.MoveFrom(cameraGO, iTween.Hash(	// slide in mini store from bottom of screen
//			"position", new Vector3(0.0f, -850.0f, cameraGO.transform.localPosition.z),
//			"islocal", true,
//			"time", 0.5f,
//			"oncomplete", "CreateStore",
//			"oncompletetarget", gameObject));	
		
		CreateStore();
	}	
	
	public override void disappear()
	{
//		UIManagerOz.SharedInstance.UICamera.GetComponent<UICamera>().clipRaycasts = true;//20150519
		base.disappear();
	}
	
	private void CreateStore()
	{
		storePanelGOs[(int)pageToLoad].GetComponent<UIScrollView>().ResetPosition();
		
		if (UIStoreList.storeLoaded == false)					// request product list from store
			storePanelGOs[(int)pageToLoad].GetComponent<UIStoreList>().RequestStoreList();
		else if (UIStoreList.miniStoreScrollListGenerated == false)	// generate scroll list
			storePanelGOs[(int)pageToLoad].GetComponent<UIStoreList>().GenerateScrollList();		
	}
	
	public void OnEscapeButtonClickedModel()
	{
		if( UIManagerOz.escapeHandled ) return;
		UIManagerOz.escapeHandled = true;
		
		OnBackButtonClick();
	}	
	
	public void OnBackButtonClick()		// back button
	{
		//Dont block the player from playing the game
	//	if(SharingManagerBinding.IsShowingBusyIndicator)
	//		return;
		
		if (pageToLoadIfMoreSpecificNeeded == ShopScreenName.Gems)
		{
			if (comingFromResurrectMenu == true)		// for resurrect menu
				UIManagerOz.SharedInstance.inGameVC.resurrectMenu.OnBackButtonClick();
			else if (comingFromResurrectMenu == false)	// for start of run
			{
				if (GameProfile.SharedInstance.Player.GetGemCount() <= 0)					
					UIManagerOz.SharedInstance.inGameVC.sourceArtifactMethod = null;	// kill attempt to gem the ability, since didn't buy any gems
				
				UIManagerOz.SharedInstance.inGameVC.OnUnPaused(gameObject);	
				//GameController.SharedInstance.IsPaused = false;
				//UIManagerOz.SharedInstance.inGameVC.sourceArtifactMethod();
			}
		}
		
		//else if (pageToLoad == ShopScreenName.Gems)	// for gatcha menu
		//	UIManagerOz.SharedInstance.gatchVC.OnNextPressed();	
		
		disappear();	// close this thing
	}
}





//	public void SwitchToPanel(ShopScreenName panelScreenName)	// activate panel upon button selection, passing in ShopScreenName
//	{
//		pageToLoad = panelScreenName;
//	}
//	
//	public UIStoreList GetCurrentStoreList()
//	{
//		return storePanelGOs[(int)pageToLoad].GetComponent<UIStoreList>();
//	}

	
//	public override void OnEnable() 
//	{
//		gameObject.GetComponent<UIStoreList>().storeItemsToLoad = pageToLoad;
//	}
	


//	public void UpdateCurrency()
//	{
//		this.updateCurrency();
//	}	
//}

	
//	public void ShowScreen(ShopScreenName _name)
//	{
//		pageToLoad = _name;
//	}	

		//gemsPanel.GetComponent<UIArtifactsList>().Refresh();
		//coinsPanel.GetComponent<UIPowersList>().Refresh();		
		//miscPanel.GetComponent<UIStatsList>().Refresh();
		//coinOffersPanel.GetComponent<UIConsumablesList>().Refresh();
		
//		NGUITools.SetActive(gemsPanel, (screenToLoad == ShopScreenName.Gems) ? true : false);	
//		NGUITools.SetActive(coinsPanel, (screenToLoad == ShopScreenName.Coins) ? true : false);
//		NGUITools.SetActive(miscPanel, (screenToLoad == ShopScreenName.Misc) ? true : false);
//		NGUITools.SetActive(coinOffersPanel, (screenToLoad == ShopScreenName.CoinOffers) ? true : false);	

	
//	public GameObject gemsPanel = null;
//	public GameObject coinsPanel = null;
//	public GameObject miscPanel = null;
//	public GameObject coinOffersPanel = null;	