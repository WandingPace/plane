using UnityEngine;
using System.Collections;

public class UINatViewControllerOz : UIViewControllerOz
{	
	public override void appear() 
	{
		base.appear();		
	}
}











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
	