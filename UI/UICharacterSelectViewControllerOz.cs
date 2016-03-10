using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UICharacterSelectViewControllerOz : UIViewControllerOz
{
	public UICenterOnChild centeredCharacter = null;
	//public UIInventoryViewControllerOz inventoryVC = null;
	
	private List<Transform> characterCards = new List<Transform>();
	
	public GameObject buttons = null;
	
	protected override void Start() 
	{
		base.Start();
		
		//-- Cache stuff here.
		characterCards.Clear();
		CharacterCardOz[] cards = GetComponentsInChildren<CharacterCardOz>();
		
		foreach (CharacterCardOz item in cards) 
			characterCards.Add (item.transform); 
	}
	
	public override void appear() 
	{
		base.appear();
		//if (paperViewController != null) { paperViewController.SetPlayButtonCallback(this.gameObject, "OnPlayClicked"); }
		foreach (CharacterStats character in GameProfile.SharedInstance.Characters) { UpdateCharacterCard(character); }
		if (buttons != null) { SetButtons(true); }
		//RecenterOnSelectedCharacter(); --COMMENTED OUT SINCE DON'T HAVE DRAGGABLE CHARACTER SELECT
	}

	private bool SetActiveCharacter() 
	{
		PlayerStats player = GameProfile.SharedInstance.Player;
		
		//-- Choose the current hero if its unlocked.
		CharacterStats selectedHero = GetSelectedHero();
		if (selectedHero != null && selectedHero.unlocked == true) 
		{
			player.activePlayerCharacter = selectedHero.characterId;
			return true;
		}
		return false;
	}
	
	public void UpdateCharacterCard(CharacterStats characterStat) 
	{
		int characterSlot = characterStat.characterId;
		if (characterCards == null || characterSlot < 0 || characterSlot >= characterCards.Count) { return; }
		
		Transform cardXform = characterCards[characterSlot];
		if (cardXform == null) { return; }
		
		CharacterCardOz card = cardXform.GetComponent<CharacterCardOz>() as CharacterCardOz;
		if (card == null) { return; }
		
		card.UpdateUI(characterStat);
	}
	
	public void Update() 
	{
		//CharacterStats selectedHero = GetSelectedHero();
		//if (selectedHero != null) { paperViewController.ShowPlayButton(selectedHero.unlocked); }
	}
	
	private CharacterStats GetSelectedHero() 
	{
		CharacterCardOz characterCard = null;
		if(centeredCharacter != null && centeredCharacter.centeredObject != null)	// && paperViewController != null) 
		{
			characterCard = centeredCharacter.centeredObject.GetComponent<CharacterCardOz>() as CharacterCardOz;
			if (characterCard != null && 
				characterCard.CharacterID >=0 && 
				characterCard.CharacterID < GameProfile.SharedInstance.Characters.Count) 
			{
				return GameProfile.SharedInstance.Characters[characterCard.CharacterID];
			}
		}
		return null;
	}
	
	public void OnUnlockCharacter()
	{
		//-- Get the selectedHero.
		CharacterStats selectedHero = GetSelectedHero();
		if (selectedHero.unlocked == true) { return; }
		
		//-- TODO: Propmpt to buy, can afford
		if (GameProfile.SharedInstance.Player.CanAffordHero(selectedHero.characterId) == false) 
		{
			//UIOkayDialogOz.onNegativeResponse += OnNeedMoreCoinsNo;
//			UIOkayDialogOz.onPositiveResponse += OnNeedMoreCoinsYes;
			//UIManagerOz.SharedInstance.confirmDialog.ShowConfirmDialog("Lbl_Dialogue_MoreCoins_Prompt","Lbl_Dialogue_MoreCoins_Confirm", "Btn_No", "Btn_Yes");
			return;
		}
		BuyCharacter(selectedHero);
	}
	
	private void BuyCharacter(CharacterStats hero) 
	{
		PlayerStats player = GameProfile.SharedInstance.Player;
		if (player == null || hero == null) { return; }
		player.PurchaseHero(hero.characterId);
		if (player.IsHeroPurchased(hero.characterId) == false) { return; }
		
		UIManagerOz.SharedInstance.PaperVC.UpdateCurrency();
		UpdateCharacterCard(hero);
	}

	public void OnButtonClicked(GameObject sender) 
	{
		switch(sender.name)
		{
			case "PowerupsButton":
				GoToInventory(UpgradesScreenName.PowerUps);
				break;				
			case "ModifiersButton":
				GoToInventory(UpgradesScreenName.Artifacts);
				break;
			case "ConsumableButton":
				GoToInventory(UpgradesScreenName.Consumables);
				break;
			case "MoreCoinsButton":	//StatsButton":
				GoToInventory(UpgradesScreenName.MoreCoins);	//.Stats);
				break;				
		}
	}
	
	private void GoToInventory(UpgradesScreenName _screen)
	{
		//if (inventoryVC == null) { return; }	
		//inventoryVC.SwitchToPanel(_screen);
		//inventoryVC.appear();
		//if (buttons != null) { SetButtons(false); }
		SetButtons(false);
	}
	
	public void SetButtons(bool state)
	{
		buttons.SetActive(state);
		//paperViewController.ShowPlayButton(showPlayButton);
		//paperViewController.SetPlayButtonCallback(gameObject, "OnPlayClicked");
	}
}


	
//	public override void OnBackButton()
//	{
//		base.OnBackButton();
//	}
	


	//public static event voidClickedHandler onPlayClickedHandler = null;
	//public UIInGameViewControllerOz inGameVC = null;
	


//	
//	public void OnPlayClicked() 
//	{
//		Debug.LogWarning("OnPlayClicked in UICharacterSelectViewControllerOz!");		
////		if (SetActiveCharacter() == true) 
////		{
////			if (GameController.SharedInstance != null && GameController.SharedInstance.Player != null) 
////			{
////				GameController.SharedInstance.Player.doSetupCharacter();	
////			}
////		} 
////		else { return; } //-- If we can't set the active player, prevent PLAYING.
//		
//		if (MainGameCamera != null) { MainGameCamera.enabled = true; }
//		disappear();
//		if (inGameVC != null) { inGameVC.appear(); } //ShowObject(activePowerIcon.gameObject, false, false);
//		if (onPlayClickedHandler != null) { onPlayClickedHandler(); } //-- Notify an object that is listening for this event.
//	}
		
//		if (sender.name == "ModifiersButton") { GoToInventory(ScreenName.Artifacts); }
//		if (sender.name == "PowerupsButton") { GoToInventory(ScreenName.PowerUps); }
//		if (sender.name == "StatsButton") { GoToInventory(ScreenName.Stats); }
//		if (sender.name == "StoreButton") { GoToInventory(ScreenName.Store); }		

//	public void OnModifiersClicked(GameObject sender) 
//	{
//		GoToInventory(ScreenName.Artifacts);
//	}
//
//	public void OnPowerupsClicked(GameObject sender)
//	{
//		GoToInventory(ScreenName.PowerUps);
//	}
//	
//	public void OnStatsClicked(GameObject sender) 
//	{
//		GoToInventory(ScreenName.Stats);
//	}
//	
//	public void OnStoreClicked(GameObject sender) 
//	{
//		GoToInventory(ScreenName.Store);
//	}	
	

//	public void OnModifiersClicked(GameObject sender) 
//	{
//		if (inventoryVC == null) { return; }
//		//inventoryVC.equipInSlot = ArtifactSlotType.Total;	//CharacterEquipSlotPressed(sender, ArtifactSlotType.Three);
//		//disappear();
//		CharacterStats activeCharacter = GameProfile.SharedInstance.GetActiveCharacter();
//		//inventoryVC.SetEquippedArtifact(activeCharacter.getArtifactForSlot(ArtifactSlotType.Total));	//slotIndex));
//		GoToInventory(ScreenName.Artifacts);	//.ShowArtifacts();
//	}
//
//	public void OnPowerupsClicked(GameObject sender)
//	{
//		if (inventoryVC == null) { return; }
//		//disappear();
//		CharacterStats activeCharacter = GameProfile.SharedInstance.GetActiveCharacter();
//		//inventoryVC.SetEquippedPower(activeCharacter.powerID);
//		GoToInventory(ScreenName.PowerUps);	//.ShowPowerups();
//	}
//	
//	public void OnStatsClicked(GameObject sender) 
//	{
//		GoToInventory(ScreenName.Stats);	//.ShowStats();
//	}

//	private void CharacterEquipSlotPressed(GameObject sender, ArtifactSlotType slotIndex) 
//	{
//		if (inventoryVC == null) { return; }
//		inventoryVC.equipInSlot = slotIndex;
//		//disappear();
//		CharacterStats activeCharacter = GameProfile.SharedInstance.GetActiveCharacter();
//		inventoryVC.SetEquippedArtifact(activeCharacter.getArtifactForSlot(slotIndex));
//		inventoryVC.ShowScreen(ScreenName.Artifacts);	//.ShowArtifacts();
//		inventoryVC.appear();
//	}

//	public void OnCharacterEquipSlotOnePressed(GameObject sender) 
//	{
//		CharacterEquipSlotPressed(sender, ArtifactSlotType.One);
//	}
//	
//	public void OnCharacterEquipSlotTwoPressed(GameObject sender) 
//	{
//		CharacterEquipSlotPressed(sender, ArtifactSlotType.Two);
//	}
//	
	
//	--COMMENTED OUT SINCE DON'T HAVE DRAGGABLE CHARACTER SELECT
//	UIScrollView mDrag;
//	private void RecenterOnSelectedCharacter()
//	{
//		if (mDrag == null && characterCards != null && characterCards.Count >= 1)
//		{
//			mDrag = NGUITools.FindInParents<UIScrollView>(characterCards[0].gameObject);
//
//			if (mDrag == null)
//			{
//				return;
//			}
//		}
//		if (mDrag.panel == null) 
//			return;
//		
//		PlayerStats player = GameProfile.SharedInstance.Player;
//		if(player == null || characterCards == null) 
//			return;
//		
//		Transform closest = characterCards[player.activePlayerCharacter];
//		if(closest == null) 
//			return;
//		
//		// Calculate the panel's center in world coordinates
//		Vector4 clip = mDrag.panel.clipRange;
//		Transform dt = mDrag.panel.cachedTransform;
//		Vector3 center = dt.localPosition;
//		center.x += clip.x;
//		center.y += clip.y;
//		center = dt.parent.TransformPoint(center);
//		mDrag.currentMomentum = Vector3.zero;
//		
//		if (closest != null)
//		{
//			// Figure out the difference between the chosen child and the panel's center in local coordinates
//			Vector3 cp = dt.InverseTransformPoint(closest.position);
//			Vector3 cc = dt.InverseTransformPoint(center);
//			Vector3 offset = cp - cc;
//
//			// Offset shouldn't occur if blocked by a zeroed-out scale
//			if (mDrag.scale.x == 0f) offset.x = 0f;
//			if (mDrag.scale.y == 0f) offset.y = 0f;
//			if (mDrag.scale.z == 0f) offset.z = 0f;
//
//			// Spring the panel to this calculated position
//			//mDrag.MoveRelative(offset);
//			SpringPanel.Begin(mDrag.gameObject, dt.localPosition - offset, 8f);
//		}
//	}
