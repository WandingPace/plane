using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIMainMenuViewControllerOz : UIViewControllerOz
{	
	private enum ButtonAction
	{
		ShowNextView,
		ShowOfferWall,
		ShowReferralStore
	};
	
	public GameObject settingsButton;
	public GameObject settingsButtonSprite;
	public GameObject moreDisneyButton;	
	public GameObject moreDisneyButtonSprite;	
	private NotificationIcons notificationIcons;	
	
	public string MenuButtonToPress { get; set; } // For push notifications and Burstly Notification Ads
	
	
	protected override void Awake() 
	{ 
		base.Awake();
		notificationIcons = gameObject.GetComponent<NotificationIcons>();
	}	
	
	protected override void Start() 
	{ 
		base.Start();
		//notificationSystem = Services.Get<NotificationSystem>();
		
		if (Settings.GetString("android-store","") == "google")		// remove 'More Disney' button if Amazon build
		{
			settingsButton.transform.localPosition = moreDisneyButton.transform.localPosition;
			settingsButtonSprite.transform.localPosition = moreDisneyButtonSprite.transform.localPosition;
			Destroy(moreDisneyButton);
			Destroy(moreDisneyButtonSprite);
		}
	}		

	public override void appear()
	{
		base.appear();		
		
		GameController.SharedInstance.MenusEntered();
		SetSettingsMoreDisneyButtons(true);
	
		
		UIManagerOz.SharedInstance.PaperVC.SetCurrentPage(UIManagerOz.SharedInstance.mainVC);
		UIManagerOz.SharedInstance.PaperVC.SetPageName("Ttl_Menu2", "");	
		UIManagerOz.SharedInstance.PaperVC.UpdateCurrency();
		
		//SharingManagerBinding.ShowBurstlyBannerAd( "main_menu", true );
		
		Services.Get<NotificationSystem>().SetNotificationIconsForThisPage(UiScreenName.MAIN);	
		
		//AnalyticsInterface.SetCanUseNetwork( true ); // Resume network activity after the run
		
		AudioManager.SharedInstance.StopFX();
		AudioManager.SharedInstance.SwitchToMainMenuMusic(0.2f);
		AudioManager.SharedInstance.FadeMusicMultiplier(0.2f,0.6f);
		PopupNotification.EnableNotifications(true);
		
		//		SharingManagerBinding.SetCurrentScreenName( "main_menu" );
		
		if ( MenuButtonToPress != null )
		{
			ClickButtonByName( MenuButtonToPress );
			MenuButtonToPress = null;
		}
	}
		
	public override void disappear()
	{
		SetSettingsMoreDisneyButtons(false);
		//SharingManagerBinding.ShowBurstlyBannerAd( "main_menu", false );
		
		base.disappear();
	}	
	
	public void OnButtonClick(GameObject button)
	{	
		ClickButtonByName( button.name );
	}	
	
	public void ClickButtonByName( string buttonName )
	{
		notify.Debug( "ClickButtonByName " + buttonName );
		
		UIViewControllerOz nextViewController = UIManagerOz.SharedInstance.mainVC;
		
		ButtonAction buttonAction = ButtonAction.ShowNextView;
		
		switch (buttonName)
		{
			case "UpgradesButton":		// store button
			//AnalyticsInterface.LogNavigationActionEvent( "Store", "Main Menu", "Store" );
			
				UIManagerOz.SharedInstance.inventoryVC.LoadThisPageNextTime(UpgradesScreenName.PowerUps);
				nextViewController = UIManagerOz.SharedInstance.inventoryVC;	// go to upgrades/store page
				break;
			
			case "ChallengesButton":	// challenges button
			//	AnalyticsInterface.LogNavigationActionEvent( "Challenges", "Main Menu", "Challenges" );
			
				nextViewController = UIManagerOz.SharedInstance.ObjectivesVC;	// go to challenges page
				break;
			
			case "LegendaryChallengesButton":	// legendary challenges button
			//	AnalyticsInterface.LogNavigationActionEvent( "LegendaryChallenges", "Main Menu", "Challenges-Legendary" );
			
//				UIManagerOz.SharedInstance.ObjectivesVC.LoadThisPageNextTime(ObjectivesScreenName.Legendary);
				nextViewController = UIManagerOz.SharedInstance.ObjectivesVC;	// go to the challenges/legendary page
				break;
			
			case "TeamChallengesButton":	// team challenges button
			//AnalyticsInterface.LogNavigationActionEvent( "TeamChallenges", "Main Menu", "Challenges-Team" );

//				UIManagerOz.SharedInstance.ObjectivesVC.LoadThisPageNextTime(ObjectivesScreenName.Team);
				nextViewController = UIManagerOz.SharedInstance.ObjectivesVC;	// go to the challenges/team page
				break;
			
			case "LeaderboardsButton":	// social button
				
				//PurchaseUtil.notifyToPurchaseGiftParcel();
				return;
			
			case "StatsButton":			// stats button
			//AnalyticsInterface.LogNavigationActionEvent( "Stats", "Main Menu", "Stats" );
			
				nextViewController = UIManagerOz.SharedInstance.statsVC;		// go to stats page
				break;

			case "AbilitiesButton":	// store/abilities button
			//	AnalyticsInterface.LogNavigationActionEvent( "Abilities", "Main Menu", "Store-Abilities" );
			
				UIManagerOz.SharedInstance.inventoryVC.LoadThisPageNextTime(UpgradesScreenName.Artifacts);
				nextViewController = UIManagerOz.SharedInstance.inventoryVC;	// go to the abilities page
				break;
			
			case "UtilitiesButton": // store/utilities button
			//	AnalyticsInterface.LogNavigationActionEvent( "More Coins", "Main Menu", "Store-Utilities" );
			
				UIManagerOz.SharedInstance.inventoryVC.LoadThisPageNextTime(UpgradesScreenName.Consumables);
				nextViewController = UIManagerOz.SharedInstance.inventoryVC;	// go to the utilities page
				break;
			
			case "MoreCoins":			// store/more coins button
			//AnalyticsInterface.LogNavigationActionEvent( "More Coins", "Main Menu", "Store-More Coins" );
			//	PurchaseUtil.bIAnalysis("Clice_More_Coins");
				UIManagerOz.SharedInstance.inventoryVC.LoadThisPageNextTime(UpgradesScreenName.MoreCoins);
				nextViewController = UIManagerOz.SharedInstance.inventoryVC;	// go to more coins page
				break;
			
			case "SettingsButton":		// settings button
			//AnalyticsInterface.LogNavigationActionEvent( "Settings", "Main Menu", "Settings" );
			
				nextViewController = UIManagerOz.SharedInstance.settingsVC;		// go to settings	
				break;
			
			case "MoreGamesButton":		// more games button
			//AnalyticsInterface.LogNavigationActionEvent( "More Disney", "Main Menu", "Referral Store" );
			
				buttonAction = ButtonAction.ShowReferralStore;
				break;
			
			case "CharactersButton":		// characters button
			//AnalyticsInterface.LogNavigationActionEvent( "Characters", "Main Menu", "Characters" );
				
//				UIManagerOz.SharedInstance.worldOfOzVC.LoadPageCharacters(WorldOfOzScreenName.Characters);
				nextViewController = UIManagerOz.SharedInstance.worldOfOzVC;	// go to character's page
				break;
			
			case "WorldofOzButton":		// world of oz button
			//	AnalyticsInterface.LogNavigationActionEvent( "World Of Oz", "Main Menu", "World Of Oz" );
			
//				UIManagerOz.SharedInstance.worldOfOzVC.LoadPageCharacters(WorldOfOzScreenName.Environments);	
				nextViewController = UIManagerOz.SharedInstance.worldOfOzVC;	// go to world of oz page
				break;				
		}
	
		bool networkReachable = (Application.internetReachability != NetworkReachability.NotReachable);
		
		if ( buttonAction == ButtonAction.ShowOfferWall )
		{
			 
				UIManagerOz.SharedInstance.okayDialog.ShowOkayDialog("Msg_NeedConnect", "Btn_Ok", false);	
		}
		else if ( buttonAction == ButtonAction.ShowReferralStore )
		{
			if (networkReachable) {
			}
				 
			else 
				UIManagerOz.SharedInstance.okayDialog.ShowOkayDialog("Msg_NeedConnect", "Btn_Ok", false);				
		}
		else
		{
			//UIManagerOz.SharedInstance.PaperVC.SetCurrentPage(nextViewController);
			nextViewController.appear();
			disappear();
		}
	}
	
	public void SetSettingsMoreDisneyButtons(bool visible)
	{
		NGUITools.SetActive(settingsButton, visible);
		
		if (moreDisneyButton != null)
			NGUITools.SetActive(moreDisneyButton, visible);
	}
	
	public void SetNotificationIcon(int buttonID, int iconValue)		// update actual icon onscreen
	{
		notificationIcons.SetNotification(buttonID, iconValue);
	}
}





//	public void OnNatClick()
//	{
//		UIManagerOz.SharedInstance.natVC.appear();
//		disappear();
//	}


	
//	public void OnButtonClick(GameObject button)
//	{	
//		UIManagerOz.SharedInstance.PaperVC.OnButtonClick(button);
//	}

	
//	public List<GameObject> buttonGOs = new List<GameObject>();		// for main menu buttons
//	private List<UISprite> buttonSprites = new List<UISprite>();
//	private List<UISprite> buttonBGs = new List<UISprite>();
//	private List<UISprite> buttonFrames = new List<UISprite>();
//	private List<UILabel> buttonLabels = new List<UILabel>();		
//	private List<BoxCollider> buttonColliders = new List<BoxCollider>();	
//		
//	public override void Start()	
//	{
//		base.Start();
//	
//		// find icons and backgrounds for all buttons on main menu
//		foreach (GameObject buttonGO in buttonGOs)
//		{
//			buttonSprites.Add(buttonGO.transform.Find("Sprite").GetComponent<UISprite>());	
//			buttonBGs.Add(buttonGO.transform.Find("Background").GetComponent<UISprite>());
//			buttonFrames.Add(buttonGO.transform.Find("Frame").GetComponent<UISprite>());
//			buttonLabels.Add(buttonGO.transform.Find("Label").GetComponent<UILabel>());
//			buttonColliders.Add(buttonGO.GetComponent<BoxCollider>());	
//		}		
//	}
//	
//	
//	private void ShowMainMenuButtons(bool show) 
//	{
//		for (int i=0; i<buttonGOs.Count; i++)
//		{
//			buttonSprites[i].enabled = show;
//			buttonBGs[i].enabled = show;
//			buttonColliders[i].enabled = show;
//			buttonFrames[i].enabled = show;
//			buttonLabels[i].enabled = show;
//		}
//	}	
//}



//	public UIMapViewControllerOz mapVC = null;
//	public UICharacterSelectViewControllerOz characterSelectVC = null;
//	public UILeaderboardViewControllerOz leaderboardVC = null;
//	public UIObjectivesViewControllerOz ObjectivesVC = null;	
//	public UIIAPViewControllerOz IAPStoreVC = null;
//	public UIMoreGamesViewControllerOz moreGamesVC = null;
//	public UISettingsViewControllerOz settingsVC = null;
//	public UIStatViewControllerOz statsVC = null;
//
//	public UILabel currentCharacterLabel = null;
//	public UISprite currentCharacterSprite = null;




//	public void OnButtonClick(GameObject button)
//	{
//		UIViewControllerOz viewController = this;
//		
//		if (button.name == "MapButton") { viewController = mapVC; }
//		else if (button.name == "UpgradesButton") { viewController = characterSelectVC; }		
//		else if (button.name == "LeaderBoardsButton") { viewController = leaderboardVC; }			
//		else if (button.name == "ObjectivesButton") { viewController = ObjectivesVC; }
//		else if (button.name == "MoreCoinsButton") { viewController = IAPStoreVC; }	
//		else if (button.name == "MoreGamesButton") { viewController = moreGamesVC; }	
//		else if (button.name == "SettingsButton") { viewController = settingsVC; }	
//		//else if (button.name == "StatsButton") { viewController = statsVC; }			
//		
//		disappear();
//		viewController.previousViewController = this;
//		viewController.appear();	
//	}



		//notify.Debug("UIMainMenuViewController.appear");


		//if(paperViewController != null) { paperViewController.SetPlayButtonCallback(this.gameObject, "OnPlayClicked"); }


//		//-- Make sure our current char portrait and name are set.
//		CharacterStats currentCharacter = GameProfile.SharedInstance.GetActiveCharacter();
//		if(currentCharacter != null) 
//		{
//			if(currentCharacterLabel != null) { currentCharacterLabel.text = currentCharacter.displayName; }
//			if(currentCharacterSprite != null) 
//			{
//				GameProfile.ProtoCharacterVisual protoVisual = GameProfile.SharedInstance.ProtoCharacterVisuals[currentCharacter.protoVisualIndex];
//				if(protoVisual != null && protoVisual.portraitSpriteName != null) { currentCharacterSprite.spriteName = protoVisual.portraitSpriteName; }
//			}
//		}
//		linkupPaperVC();


	
//	public UIInGameViewControllerOz inGameVC = null;	
//	public static event voidClickedHandler 	onPlayClickedHandler = null;
//	
//	public void OnPlayClicked() 
//	{
//		if(MainGameCamera != null) { MainGameCamera.enabled = true; }
//		disappear();
//		if(inGameVC != null) { inGameVC.appear(); } 					//ShowObject(activePowerIcon.gameObject, false, false);
//		if(onPlayClickedHandler != null) { onPlayClickedHandler(); }	//-- Notify an object that is listening for this event.	
//	}
	
//	public override void Start()
//	{
//		notify.Warning("UIMainMenuViewController.start");
//		base.Start();
//	}

//
//	public void OnShowMap() 
//	{
//		if(mapVC != null) 
//		{
//			disappear();
//			mapVC.previousViewController = this;
//			mapVC.appear();
//		}
//	}
//	
//	public void OnCharacterSelectClicked() 
//	{
//		if (characterSelectVC != null) 	//-- show characters.
//		{
//			disappear();
//			characterSelectVC.previousViewController = this;
//			characterSelectVC.appear();
//		}
//	}
//
//	public void OnShowLeaderboards()
//	{
//		if(leaderboardVC != null) 
//		{
//			disappear();
//			leaderboardVC.previousViewController = this;
//			leaderboardVC.appear();
//		}
//	}
//	
//	public void OnObjectives()
//	{
//		if (ObjectivesVC != null) 
//		{
//			disappear();
//			
//			ObjectivesVC.previousViewController = this;
//			ObjectivesVC.appear();
//		}
//	}
//	
//	public void OnIAPStore() 
//	{
//		if (IAPStoreVC != null) 
//		{
//			disappear();
//			IAPStoreVC.previousViewController = this;
//			IAPStoreVC.appear();
//		}
//	}	
//	
//	public void OnMoreGames() 
//	{
//		if (moreGamesVC != null) 
//		{
//			disappear();
//			moreGamesVC.previousViewController = this;
//			moreGamesVC.appear();
//		}
//	}
//	
//	public void OnSettings() 
//	{
//		if (settingsVC != null) 
//		{
//			disappear();
//			settingsVC.previousViewController = this;
//			settingsVC.appear();
//		}
//	}
//
//	public void OnShowStats() 
//	{
//		if (statsVC != null) 
//		{
//			disappear();
//			statsVC.previousViewController = this;
//			statsVC.appear();
//		}
//	}	
//
//	
	//public UIGIftsViewController GiftsVC = null;	

//	public void OnGiftStore() {
//		if(GiftsVC != null) {
//			disappear();
//			
//			GiftsVC.previousViewController = this;
//			GiftsVC.appear();
//		}
//	}
	