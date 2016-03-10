using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CharacterCellData : MonoBehaviour
{
	public GameObject viewController;
	public GameObject scrollList;									// reference to the scroll list that this cell is parented under the grid/table of		
	//public GameObject darkener;
	//public GameObject checkmark;
	
	public UISprite background;
	
	public List<GameObject> buttons = new List<GameObject>();
	
	public CharacterStats _data;											// reference to powerup data
	
	public UISprite SaleBurstSprite;
	public UISprite SaleSleeveSprite;
	public UISprite SaleOldCoinSprite;
	public UISprite SaleSlashOutSprite;
	public UILabel SaleSlashOutSprite_alt;
	public UILabel SaleOldCostLabel;
	
	public UILabel DefaultCostLabel;
	public UILabel SaleNewCostLabel;
	public UILabel DefaultBuyLabel;
	
	private NotificationSystem notificationSystem;
	private NotificationIcons notificationIcons;		
	
	protected static Notify notify;
	
	void Awake()
	{
		if (notify == null)
			notify = new Notify("CharacterCellData");
		
		notificationIcons = gameObject.GetComponent<NotificationIcons>();		
	}
	
	void Start()
	{
		Destroy(gameObject.GetComponent<UIPanel>());				// kill auto-attached UIPanel component
		
		notificationSystem = Services.Get<NotificationSystem>();
		
		_toggleSaleDisplay( false ); // initialize sale sprites to false
		
		if (_data != null)// && viewController != null)
			Refresh();												// populate fields
	}

	public void SetData(CharacterStats data)
	{
		_data = data;
		Refresh();													// populate fields	
	}
	
	private void _toggleSaleDisplay(bool active)
	{
		/*
		gameObject.transform.Find( "CellContents/GraphicsAnchor/sale_Burst" ).GetComponent<UISprite>().enabled = active;
		gameObject.transform.Find( "CellContents/GraphicsAnchor/sale_IconSleeve").GetComponent<UISprite>().enabled = active;
		gameObject.transform.Find( "CellContents/GraphicsAnchor/sale_NEW_CoinDisplayIcon").GetComponent<UISprite>().enabled = active;
		gameObject.transform.Find( "CellContents/GraphicsAnchor/sale_OLD_LabelCost").GetComponent<UILabel>().enabled = active;
		gameObject.transform.Find( "CellContents/GraphicsAnchor/sale_SlashOut").GetComponent<UISprite>().enabled = active;					
		*/		
		// disable sale related elements if artifact is already maxed out
		if (GameProfile.SharedInstance.Player.IsHeroPurchased(_data.characterId))
		{
			active = false;	
		}
		
		SaleBurstSprite.enabled = active;
		SaleOldCoinSprite.enabled = active;
		SaleSlashOutSprite.enabled = active;
		SaleSlashOutSprite_alt.enabled = active;
		SaleSleeveSprite.enabled = active;
		SaleOldCostLabel.enabled = active;
		SaleNewCostLabel.enabled = active;
		
		DefaultBuyLabel.enabled = !active;
		DefaultCostLabel.enabled = !active;	
	}
	
	public void Refresh()
	{
		if (_data != null)// && viewController != null)
		{	
			// populate fields from data
			//gameObject.transform.Find("CellContents/GraphicsAnchor/ImageButton").GetComponent<UIButtonMessage>().target = scrollList;	// link up 'buy' button
			foreach (GameObject go in buttons)
				go.GetComponent<UIButtonMessage>().target = scrollList;	// link up 'buy' button
			
			gameObject.transform.Find("CellContents").GetComponent<UIButtonMessage>().target = viewController;			
			gameObject.transform.Find("CellContents/FontAnchor/LabelTitle").GetComponent<UILocalize>().SetKey(_data.displayName);
			//gameObject.transform.Find("CellContents/FontAnchor/LabelDescription").GetComponent<UILocalize>().SetKey(_data.Description);

			// Default behaivor, turn off sale related elements.
			/*
			gameObject.transform.Find( "CellContents/GraphicsAnchor/sale_Burst" ).GetComponent<UISprite>().enabled = false;
			gameObject.transform.Find( "CellContents/GraphicsAnchor/sale_IconSleeve").GetComponent<UISprite>().enabled = false;
			gameObject.transform.Find( "CellContents/GraphicsAnchor/sale_OLD_CoinDisplayIcon").GetComponent<UISprite>().enabled = false;
			gameObject.transform.Find( "CellContents/GraphicsAnchor/sale_OLD_LabelCost").GetComponent<UILabel>().enabled = false;
			gameObject.transform.Find( "CellContents/GraphicsAnchor/sale_SlashOut").GetComponent<UISprite>().enabled = false;			
			*/
			SetNotificationIcon();	// show notification icon if can afford to purchase this item
			
			// set status and icon
			if (GameProfile.SharedInstance.Player.IsHeroPurchased(_data.characterId) == false) 	//-- Check if purchased
			{
				gameObject.transform.Find("CellContents/FontAnchor/LabelBuy").GetComponent<UILabel>().enabled = true;
				gameObject.transform.Find("CellContents/FontAnchor/LabelBuy").GetComponent<UILocalize>().SetKey("Lbl_Buy");	// set localized 'buy' button text
				gameObject.transform.Find("CellContents/FontAnchor/LabelEquipped").GetComponent<UILabel>().enabled = false;	//.text = "";	//.SetKey("");
				
		
					_toggleSaleDisplay( false );
					DefaultCostLabel.text = _data.unlockCost.ToString();
					//gameObject.transform.Find("CellContents/FontAnchor/LabelCost").GetComponent<UILabel>().text = _data.unlockCost.ToString();	// show price & coin icon if not					
				
				gameObject.transform.Find("CellContents/IconAnchor/SpriteIcon").GetComponent<UISprite>().spriteName = _data.IconName;
				gameObject.transform.Find("CellContents/GraphicsAnchor/CoinDisplayIcon").GetComponent<UISprite>().enabled = true;
				
				EnableButton(0);
				
				SetBackgroundDarkening(false);
				if(_data.characterId == 3)
				{
					
					_toggleSaleDisplay(true);
					SaleOldCostLabel.text = "90000";
					gameObject.transform.Find( "CellContents/GraphicsAnchor/sale_NEW_CoinDisplayIcon").GetComponent<UISprite>().enabled = false;
					
				}
				
				
			}
			else if (GameProfile.SharedInstance.GetActiveCharacter().characterId == _data.characterId)	//-- Check if equipped also
			{
				_toggleSaleDisplay( false );
				
				gameObject.transform.Find("CellContents/FontAnchor/LabelBuy").GetComponent<UILabel>().enabled = false;	//.SetKey("Lbl_Equipped"); // set localized 'active' button text
				gameObject.transform.Find("CellContents/FontAnchor/LabelEquipped").GetComponent<UILabel>().enabled = true;
				gameObject.transform.Find("CellContents/FontAnchor/LabelEquipped").GetComponent<UILocalize>().SetKey("Lbl_Purchased");	//Lbl_Equipped");
				gameObject.transform.Find("CellContents/FontAnchor/LabelCost").GetComponent<UILabel>().text = " ";
				gameObject.transform.Find("CellContents/IconAnchor/SpriteIcon").GetComponent<UISprite>().spriteName = _data.IconName;
				gameObject.transform.Find("CellContents/GraphicsAnchor/CoinDisplayIcon").GetComponent<UISprite>().enabled = false;	//.spriteName = "tools_1x1_empty_sprite";	//checkbox_checked";
				gameObject.transform.Find("CellContents/FontAnchor/LabelDescription").GetComponent<UILabel>().color = new Color(52f/255f, 48f/255f, 45f/255f, 1f);
				
				EnableButton(2);
				
				SetBackgroundDarkening(true);	// darken background when equipped
			}
			else 																		// purchased, but not equipped
			{
				_toggleSaleDisplay( false );
				
				gameObject.transform.Find("CellContents/FontAnchor/LabelBuy").GetComponent<UILabel>().enabled = true;
				gameObject.transform.Find("CellContents/FontAnchor/LabelBuy").GetComponent<UILocalize>().SetKey("Lbl_Equip");	// set localized 'equip' button text
				gameObject.transform.Find("CellContents/FontAnchor/LabelEquipped").GetComponent<UILabel>().enabled = true;
				gameObject.transform.Find("CellContents/FontAnchor/LabelEquipped").GetComponent<UILocalize>().SetKey("Lbl_Purchased");
				gameObject.transform.Find("CellContents/FontAnchor/LabelCost").GetComponent<UILabel>().text = " ";
				gameObject.transform.Find("CellContents/IconAnchor/SpriteIcon").GetComponent<UISprite>().spriteName = _data.IconName;
				gameObject.transform.Find("CellContents/GraphicsAnchor/CoinDisplayIcon").GetComponent<UISprite>().enabled = false;	//.spriteName = "tools_1x1_empty_sprite";	//checkbox_unchecked";
				gameObject.transform.Find("CellContents/FontAnchor/LabelDescription").GetComponent<UILabel>().color = new Color(87f/255f, 78f/255f, 69f/255f, 1f);
				
				EnableButton(1);
				
				SetBackgroundDarkening(false);
			}			
		}
		else
			notify.Warning("CharacterStats (data) or UIInventoryViewControllerOz (viewController) is null in PowerCellData!");
	}
	
	public void SetNotificationIcon()
	{
		if (notificationSystem == null)
			notificationSystem = Services.Get<NotificationSystem>();
		
		bool enable = notificationSystem.GetNotificationStatusForThisCell(NotificationType.Character, _data.characterId);
		notificationIcons.SetNotification(0, (enable) ? 0 : -1);
	}	
	
	private void EnableButton(int id)
	{
		foreach (GameObject go in buttons)
			NGUITools.SetActive(go, false);
		
		NGUITools.SetActive(buttons[id], true);
	}
	
	private void SetBackgroundDarkening(bool darker)
	{
		Color targetColor = (darker) ? new Color(206f/255f, 179f/255f, 137f/255f, 1f) : Color.white;
		background.color = targetColor;
	}
}
