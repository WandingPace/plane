using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class UICharacterList : MonoBehaviour
{
	public GameObject viewController;
	private GameObject grid;	
	private List<GameObject> childCells = new List<GameObject>();	
	private CharacterCellData characterToPurchase;	// temp reference, for use when purchasing a specific hero
	private SaleBanner saleBanner;
	//private List<BasePower> sortedDataList = new List<BasePower>();
	//private NotificationSystem notificationSystem;
	
	
	
	
	
	
	private List<int> _updatedCharacterOrderIndex = new List<int>();
	
	protected static Notify notify;
	
	void Awake() 
	{ 
		notify = new Notify( this.GetType().Name );
		
		grid = gameObject.transform.Find("Grid").gameObject;			// connect to this panel's grid automatically	
		_sortGridItemsByPriority( GameProfile.SharedInstance.Characters );
		
		Initialize();
	}

	public void Initialize()
	{	
		childCells = CreateCells();											// create cell GameObject for each
//		grid.GetComponent<UIGrid>().sorted = false;
		grid.GetComponent<UIGrid>().Reposition();							// reset/correct positioning of all objects inside grid
	}
	
	void Start() 
	{ 
		grid.GetComponent<UIGrid>().Reposition();
		Refresh();
	}
	
	
	
	public void Refresh()
	{
		// ensure that characters have been loaded
		if ( GameProfile.SharedInstance.Characters != null && GameProfile.SharedInstance.Characters.Count > 0 )
		{
			if (saleBanner == null)
			{
				saleBanner = viewController.GetComponentsInChildren<SaleBanner>(true).First();
			}
			
			if ( saleBanner != null )
			{
				// set status of sale banner (show only if sale is active)
				saleBanner.SetSaleBannerStatus(gameObject.GetComponent<UIPanel>(), 
					transform.parent.gameObject, DiscountItemType.Character, saleBanner);
			}
				
			_sortGridItemsByPriority( GameProfile.SharedInstance.Characters );
			
			//foreach (CharacterStats data in GameProfile.SharedInstance.Characters)
	//		for(int i=0;i<GameProfile.SharedInstance.CharacterOrder.Count;i++)
				
			for ( int i = 0; i < _updatedCharacterOrderIndex.Count; i++ )			
			{
	//			int index = GameProfile.SharedInstance.CharacterOrder[i];
				
				int index = _updatedCharacterOrderIndex[i];
				
				if(index>=0 && index<GameProfile.SharedInstance.Characters.Count)
				{
					CharacterStats data = GameProfile.SharedInstance.Characters[index];
					
					notify.Debug( 
						"[UICharacterList] - Refresh - Data name: "
						+ data.displayName
						+ " - Character Id: "
						+ data.characterId
						+ " - SortPriority: "
						+ data.SortPriority
					);
					
					childCells[i].GetComponent<CharacterCellData>().SetData(data);
				}
			}
			
			gameObject.GetComponent<UIScrollView>().ResetPosition();	
		}
	}		

	private List<GameObject> CreateCells()
	{
		List<GameObject> newObjs = new List<GameObject>();

		//foreach (CharacterStats data in GameProfile.SharedInstance.Characters)
//		for(int i=0;i<GameProfile.SharedInstance.CharacterOrder.Count;i++)

		for ( int i = 0; i < _updatedCharacterOrderIndex.Count; i++ )
		{
//			int index = GameProfile.SharedInstance.CharacterOrder[i];
			
			int index = _updatedCharacterOrderIndex[i];
			
			if(index>=0 && index<GameProfile.SharedInstance.Characters.Count)
			{
				CharacterStats data = GameProfile.SharedInstance.Characters[index];
				newObjs.Add(CreatePanel(data, grid));
			}
		}
		
		return newObjs;
	}	
	
	private GameObject CreatePanel(CharacterStats data, GameObject _grid)
	{
		GameObject obj = (GameObject)Instantiate(Resources.Load("CharacterCellOz"));	// instantiate objective from prefab	
		obj.transform.parent = _grid.transform;
		obj.transform.localScale = Vector3.one;
		obj.transform.rotation = grid.transform.rotation;
		obj.transform.localPosition = Vector3.zero;
		obj.name = "cell" + data.characterId;
		obj.GetComponent<CharacterCellData>()._data = data;						// store reference to data for this objective
		obj.GetComponent<CharacterCellData>().viewController = viewController;	// pass on reference to view controller, for event response
		obj.GetComponent<CharacterCellData>().scrollList = this.gameObject;
		return obj;
	}

	private void OnPurchaseYes()
	{
		PlayerStats playerStats = GameProfile.SharedInstance.Player;
		
		playerStats.PurchaseHero(characterToPurchase._data.characterId);	// buy hero if we can afford it
		UIManagerOz.SharedInstance.PaperVC.UpdateCurrency();
		//invViewCont.UpdateCurrency();									// will update coin and gem counts in UI				
		characterToPurchase.Refresh();		
	}		
	
	public void CellBuyButtonPressed(GameObject cell)		// public void OnPowerItemPressed(GameObject cell) 
	{
		//notify.Debug("CellBuyButtonPressed called at: " + Time.realtimeSinceStartup.ToString());
		
		// set up shorter local identifiers, to keep code easy to read
        //UIWorldOfOzViewControllerOz invViewCont = viewController.GetComponent<UIWorldOfOzViewControllerOz>();	
		CharacterCellData characterCellData = cell.transform.parent.parent.parent.GetComponent<CharacterCellData>();		
		int id = characterCellData._data.characterId;

		//Services.Get<NotificationSystem>().ClearNotification(NotificationType.Powerup, powerID);		
		//Services.Get<NotificationSystem>().SetNotificationIconsForThisPage(UiScreenName.UPGRADES);	
		//UnityEngine.Debug.Log("characterCellData:"+characterCellData);

		
		notify.Debug("IsHeroPurchased call "+GameProfile.SharedInstance.Player.IsHeroPurchased(id));
		if (GameProfile.SharedInstance.Player.IsHeroPurchased(id) == false) 		// check if already purchased
		{
			if(id == 3)
			{
				Debug.LogError(" need to real money !!");
				characterToPurchase = characterCellData;	
			//  PurchaseUtil.purchaseChinaGirl();
			//	purchaseChinaGirlSuccess();
				return;
				
			}
			
			if (GameProfile.SharedInstance.Player.CanAffordHero(id) == true)
			{	notify.Debug(" purchased => id:"+id);
				characterToPurchase = characterCellData;				
				OnPurchaseYes();		// buy it if we can afford it
			}
			else
			{
//				UIConfirmDialogOz.onNegativeResponse += invViewCont.OnNeedMoreCoinsNo;
//				UIConfirmDialogOz.onPositiveResponse += invViewCont.OnNeedMoreCoinsYes;
				UIManagerOz.SharedInstance.confirmDialog.ShowConfirmDialog("Lbl_Dialogue_MoreCoins_Prompt", "Btn_No", "Btn_Yes");
			}
		}
		else if (GameProfile.SharedInstance.Player.activePlayerCharacter != id) 	// check if character is already equipped
		{
			notify.Debug("already purchased => equipped:"+id);
			GameProfile.SharedInstance.Player.activePlayerCharacter = id;
			
            //CharacterStats character = GameProfile.SharedInstance.Characters[ id ];
			//			AnalyticsInterface.LogGameAction( "character", "equipped", character.displayName, "", 0 );
			
			GamePlayer.SharedInstance.doSetupCharacterDelayed(0.2f);
			
			GameProfile.SharedInstance.Serialize();
			
			// ask all power cells to update their GUI renderings, to match updated data
			CharacterCellData[] allCharacterCells = grid.GetComponentsInChildren<CharacterCellData>();
			foreach (CharacterCellData characterCell in allCharacterCells) 
			{ 
				characterCell.Refresh(); 
			}
		}

		UIManagerOz.SharedInstance.PaperVC.UpdateCurrency();
		characterCellData.Refresh();									// ask cell to update its GUI rendering to match data, in case it was updated in the transaction
	}
	
	private void _sortGridItemsByPriority( List<CharacterStats> characterList )
	{
		//characterList.Sort( ( a1, a2 ) => a2.SortPriority.CompareTo( a1.SortPriority ) );
		
		//List<CharacterStats> tempCharList = characterList.OrderBy(a => a.SortPriority);
		//List<CharacterStats> tempCharList = characterList.Sort( (a1, a2) => a1.SortPriority.CompareTo( a2.SortPriority ) );
		
		SortedList<int, int> sortList = new SortedList<int, int>();
	
		_updatedCharacterOrderIndex.Clear();
		
		foreach ( CharacterStats character in characterList )
		{
			sortList.Add( character.SortPriority, character.characterId );
			
			//_updatedCharacterOrderIndex.Add( character.characterId );
			//notify.Debug( "[UICharacterList] - _sortGridItemsByPriorityList - character Id: " + character.characterId.ToString() );
		}
		
		foreach (KeyValuePair<int, int> characterIndex in sortList )
		{
			_updatedCharacterOrderIndex.Add( characterIndex.Value );
		}
	}
}
	



		
		//PowerCellData powerCellData = cell.transform.parent.GetComponent<PowerCellData>();
		
		//CharacterStats activeCharacter = GameProfile.SharedInstance.GetActiveCharacter();
		//PlayerStats playerStats = GameProfile.SharedInstance.Player;
		


				//UIManagerOz.SharedInstance.confirmDialog.ShowConfirmDialog("Lbl_Dialogue_MoreCoins_Prompt","Lbl_Dialogue_MoreCoins_Confirm", "Btn_No", "Btn_Yes");



		//invViewCont.UpdateCurrency();								// will update coin and gem counts in UI				


//		else 	//-- Can't equip this because its already equipped.
//		{
//			UIOkayDialogOz.onPositiveResponse += OnAlreadyEquipped;
//			UIManagerOz.SharedInstance.okayDialog.ShowOkayDialog("Oops!", "That PowerUp is already in use.", "Btn_Ok");	//-- Show error dialog
//		}
		


//			equippedPowerupCell.GetComponent<EquippedPowerupCell>().UpdateEquippedCell(PowerStore.Powers[powerID]);	// change icon next to character
			//invViewCont.characterSelectVC.UpdateCharacterCard(activeCharacter);
			//UIManagerOz.SharedInstance.characterSelectVC.UpdateCharacterCard(activeCharacter);



				//UIConfirmDialogOz.onNegativeResponse += OnPurchaseNo;
				//UIConfirmDialogOz.onPositiveResponse += OnPurchaseYes;
				//UIManagerOz.SharedInstance.confirmDialog.ShowConfirmDialog(powerCellData.data.Title, "Purchase this powerup?", "Btn_No", "Btn_Yes");
				//playerStats.PurchasePower(powerID);	


//	public void CellButtonPressed(GameObject button)
//	{
//		int id = button.transform.parent.parent.parent.gameObject.GetComponent<CharacterCellData>()._data.characterId;
//		
//		if (!GameProfile.SharedInstance.Player.IsHeroPurchased(id) && GameProfile.SharedInstance.Player.CanAffordHero(id))
//		{
//			GameProfile.SharedInstance.Player.PurchaseHero(id);
//		}
//		
//		if(GameProfile.SharedInstance.Player.IsHeroPurchased(id))
//		{
//			GameProfile.SharedInstance.Player.activePlayerCharacter = id;
//		}
//		
//		Refresh();
//	}



//	public void Refresh()
//	{
//		int id = 0;
//		foreach (GameObject childCell in childCells)
//		{
//			if (childCell != null)
//			{	
//				childCell.GetComponent<CharacterCellData>().SetData(GameProfile.SharedInstance.Characters[id]);
//			}
//			id++;
//		}	
//	}	