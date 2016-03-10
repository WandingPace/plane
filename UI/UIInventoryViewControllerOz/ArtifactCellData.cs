using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class ArtifactCellData : MonoBehaviour 
{
	public GameObject viewController;
	public GameObject scrollList;									// reference to the scroll list that this cell is parented under the grid/table of
	public ArtifactProtoData _data;									// reference to artifact data	
	
	public List<UISprite> rankSprites = new List<UISprite>();
	public List<UISprite> starSprites = new List<UISprite>();
	
	public List<GameObject> buttons = new List<GameObject>();
	
	public UISprite SaleBurstSprite;
	public UISprite SaleSleeveSprite;
	public UISprite SaleOldCoinSprite;
	public UISprite SaleSlashOutSprite;
	public UILabel SaleSlashOutSprite_alt;
	public UILabel SaleOldCostLabel;
	
	public UILabel DefaultCostLabel;
	public UILabel SaleNewCostLabel;
	public UILabel DefaultBuyLabel;
	
	// wxj
	public UISprite onekeyFrame;
	public UILabel onekeyLabel;
	
	
	private NotificationSystem notificationSystem;
	private NotificationIcons notificationIcons;	
	
	protected static Notify notify;
	
	void Awake()
	{
		if (notify == null)
			notify = new Notify("ArtifactCellData");
		
		notificationIcons = gameObject.GetComponent<NotificationIcons>();
	}
	
	void Start()
	{
		Destroy(gameObject.GetComponent<UIPanel>());				// kill auto-attached UIPanel component
			
		notificationSystem = Services.Get<NotificationSystem>();		
		
		_toggleSaleDisplay( false );
		
		if (_data != null && viewController != null)
			Refresh();												// populate fields
	}

	public void SetData(ArtifactProtoData data)
	{
		_data = data;
		Refresh();													// populate fields	
	}
	
	private void _toggleSaleDisplay( bool active )
	{	
		// Default behaivor, turn off sale related elements.
		/*
		gameObject.transform.Find( "CellContents/GraphicsAnchor/sale_Burst" ).GetComponent<UISprite>().enabled = active;
		gameObject.transform.Find( "CellContents/GraphicsAnchor/sale_IconSleeve").GetComponent<UISprite>().enabled = active;
		gameObject.transform.Find( "CellContents/GraphicsAnchor/sale_OLD_CoinDisplayIcon").GetComponent<UISprite>().enabled = active;
		gameObject.transform.Find( "CellContents/GraphicsAnchor/sale_OLD_LabelCost").GetComponent<UILabel>().enabled = active;
		gameObject.transform.Find( "CellContents/GraphicsAnchor/sale_SlashOut").GetComponent<UISprite>().enabled = active;	
		*/
		
		// disable sale related elements if artifact is already maxed out
		if (GameProfile.SharedInstance.Player.IsArtifactMaxedOut(_data._id))
		{
			active = false;	
		}

		SaleBurstSprite.enabled = active;
		SaleOldCoinSprite.enabled = active;
		//wxj
		//SaleSlashOutSprite.enabled = active;
		SaleSlashOutSprite_alt.enabled = active;
		SaleSleeveSprite.enabled = active;
		SaleOldCostLabel.enabled = active;
		SaleNewCostLabel.enabled = active;
		
		DefaultBuyLabel.enabled = !active;
		DefaultCostLabel.enabled = !active;
	}
	
	public void Refresh()
	{
		
		if (_data != null && viewController != null)
		{
			
			// populate fields from data
			//gameObject.transform.Find("CellContents/GraphicsAnchor/ImageButton").GetComponent<UIButtonMessage>().target = scrollList;	// link up 'buy' button
			foreach (GameObject go in buttons)
				go.GetComponent<UIButtonMessage>().target = scrollList;	// link up 'buy' button			
		
			gameObject.transform.Find("CellContents").GetComponent<UIButtonMessage>().target = viewController;	
		
			//gameObject.transform.Find("CellContents/Title").GetComponent<UILabel>().text = data._title;					
			gameObject.transform.Find("CellContents/FontAnchor/LabelTitle").GetComponent<UILocalize>().SetKey(_data._title);

			gameObject.transform.Find("CellContents/IconAnchor/SpriteIcon").GetComponent<UISprite>().spriteName = _data._iconName;	
		
			
			gameObject.transform.Find("CellContents/FontAnchor/LabelDescription").GetComponent<UILocalize>().SetKey(GenerateDescriptionKey(_data));
		
			gameObject.transform.Find("CellContents/FontAnchor/LabelDescriptionGemmed").GetComponent<UILocalize>().SetKey(_data._buffDescription);	
			
			SetNotificationIcon();	// show notification icon if can afford to purchase this item and it hasn't been cleared
			
			SetRankSprites();
			
			
			// set status and icon
			if (GameProfile.SharedInstance.Player.IsArtifactMaxedOut(_data._id) == false) 	//-- Check if can still be purchased
			{
				//gameObject.transform.Find("CellContents/Cost").GetComponent<UILabel>().text = data._cost.ToString();	// show price & coin icon if not
				//gameObject.transform.Find("CellContents/FontAnchor/LabelBuy").GetComponent<UILabel>().enabled = true;
				DefaultBuyLabel.enabled = true;
				
				//if discount was applied, use that discount text
				if ( _data.ServerCost_Lvl1 > 0 )
				{
					//gameObject.transform.Find("CellContents/FontAnchor/LabelCost").GetComponent<UILabel>().text = GetCostForLevel(_data, true).ToString();	// show price & coin icon if not
					
					// If the "discount" cost is less than the default cost, apply markups, strikeouts etc.
					if ( GetCostForLevel( _data, true ) < GetCostForLevel( _data ) )
					{
						_toggleSaleDisplay( true );
						//gameObject.transform.Find( "CellContents/GraphicsAnchor/sale_OLD_LabelCost").GetComponent<UILabel>().text = GetCostForLevel(_data, false).ToString();
						SaleOldCostLabel.text = GetCostForLevel( _data, false ).ToString();
						SaleNewCostLabel.text = GetCostForLevel( _data, true ).ToString();
					}
					else
					{
						DefaultCostLabel.text = GetCostForLevel( _data, true ).ToString();
						_toggleSaleDisplay( false );
					}
				}
				else
				{
					_toggleSaleDisplay( false );
					
					//gameObject.transform.Find("CellContents/FontAnchor/LabelCost").GetComponent<UILabel>().text = GetCostForLevel(_data).ToString();	// show price & coin icon if not
					DefaultCostLabel.text = GetCostForLevel( _data ).ToString();
				}
				gameObject.transform.Find("CellContents/GraphicsAnchor/CoinDisplayIcon").GetComponent<UISprite>().enabled = true;		
				gameObject.transform.Find("CellContents/GraphicsAnchor/CoinDisplayIcon").GetComponent<UISprite>().spriteName = "currency_coin";
				//gameObject.transform.Find("CellContents/Description").GetComponent<UILabel>().text = data._description;
				//gameObject.transform.Find("CellContents/SubPanel/Description").GetComponent<UILocalize>().SetKey(data._description);
				//gameObject.transform.Find("CellContents/FontAnchor/LabelDescription").GetComponent<UILocalize>().SetKey(GenerateDescriptionKey(_data));
				//gameObject.transform.Find("CellContents/FontAnchor/LabelDescriptionGemmed").GetComponent<UILocalize>().SetKey(GenerateGemmedKey(_data));
				
				notify.Debug("wxj, artifact cell before set botton ");
				// wxj, set onekey button enable
				if(!_data._onekey_iap_id.Equals("") && !_data._onekey_msg_key.Equals(""))
				{
					notify.Debug("wxj, artifact cell botton onekey ");
					EnableButton(0, 2);	// show 'buy' button
					notify.Debug("wxj, ~~~~~1 ");
					onekeyFrame.enabled = true;
					onekeyLabel.enabled = true;
//					gameObject.transform.Find("CellContents/GraphicsAnchor/Frame_onekey").GetComponent<UISprite>().enabled = true;
//					UnityEngine.Debug.Log("wxj, ~~~~~2 ");
//					gameObject.transform.Find("CellContents/FontAnchor/LabelBuy_onekey").GetComponent<UILabel>().enabled = true;
//					UnityEngine.Debug.Log("wxj, ~~~~~3 ");
				}
				else
				{
					notify.Debug("wxj,artifact cell botton no onekey ");
					EnableButton(0);	// show 'buy' button
					notify.Debug("wxj, ~~~~~1 ");
					onekeyFrame.enabled = false;
					onekeyLabel.enabled = false;
//					gameObject.transform.Find("CellContents/GraphicsAnchor/Frame_onekey").GetComponent<UISprite>().enabled = false;
//					UnityEngine.Debug.Log("wxj, ~~~~~2 ");
//					gameObject.transform.Find("CellContents/FontAnchor/LabelBuy_onekey").GetComponent<UILabel>().enabled = false;
//					UnityEngine.Debug.Log("wxj, ~~~~~3 ");
				}
				notify.Debug("wxj, artifact cell after set botton ");
				
			}
			else 																			// maxed out
			{
				_toggleSaleDisplay( false );
				
				//gameObject.transform.Find("CellContents/FontAnchor/LabelBuy").GetComponent<UILabel>().enabled = false;
				//gameObject.transform.Find("CellContents/FontAnchor/LabelCost").GetComponent<UILabel>().text = " ";
				DefaultBuyLabel.enabled = false;
				DefaultCostLabel.text = " ";
				gameObject.transform.Find("CellContents/GraphicsAnchor/CoinDisplayIcon").GetComponent<UISprite>().enabled = false;	//.spriteName = "tools_1x1_empty_sprite"; // "empty1x1";
				//gameObject.transform.Find("CellContents/Description").GetComponent<UILabel>().text = "GEMMED";
				//gameObject.transform.Find("CellContents/FontAnchor/LabelDescription").GetComponent<UILocalize>().SetKey(_data._buffDescription);
				
				
				
				// wxj, set onekey button enable
				if(!_data._onekey_iap_id.Equals("") && !_data._onekey_msg_key.Equals(""))
				{
					EnableButton(1, 3);	// switch to 'checkmark' button if maxed out...
					onekeyFrame.enabled = true;
					onekeyLabel.enabled = false;
//					gameObject.transform.Find("CellContents/GraphicsAnchor/Frame_onekey").GetComponent<UISprite>().enabled = true;
//					gameObject.transform.Find("CellContents/FontAnchor/LabelBuy_onekey").GetComponent<UILabel>().enabled = false;
				}
				else
				{
					EnableButton(1);	// switch to 'checkmark' button if maxed out...
					onekeyFrame.enabled = false;
					onekeyLabel.enabled = false;
//					gameObject.transform.Find("CellContents/GraphicsAnchor/Frame_onekey").GetComponent<UISprite>().enabled = false;
//					gameObject.transform.Find("CellContents/FontAnchor/LabelBuy_onekey").GetComponent<UILabel>().enabled = false;
				}
				
				
			}			
		}
		else
			notify.Warning("ArtifactProtoData (data) or UIInventoryViewControllerOz (viewController) is null in ArtifactCellData!");
	}
	
	private string GenerateDescriptionKey(ArtifactProtoData data)
	{
		int level = GameProfile.SharedInstance.Player.GetArtifactLevel(data._id);
		string key = data._description.Substring(0, data._description.Length - 1);
		
		if (level != 5)
			key = key + (level+1).ToString();	// levels 0-4, lop off the last digit and replace it with the level+1
		else
		{
			key = key + (level).ToString();		// level 5, show the level 5 description again
			//key = data._description.Substring(0, data._description.Length - 5);	// level 5, use the gemmed description
			//key = key + "Gem";
		}
		return key;
	}

	private int GetCostForLevel(ArtifactProtoData data, bool isDiscount = false)
	{	
		int level = GameProfile.SharedInstance.Player.GetArtifactLevel(data._id);
		
		if ( isDiscount )
		{
			switch ( level )
			{
				case 0:
					return data.ServerCost_Lvl1;
				case 1:
					return data.ServerCost_Lvl2;
				case 2:
					return data.ServerCost_Lvl3;
				case 3:
					return data.ServerCost_Lvl4;
				case 4:
					return data.ServerCost_Lvl5;
				default:
					return data.ServerCost_Lvl5;
			}
		}
		else
		{
			switch (level)
			{
			case 0:
				return data._cost_lv1;
			case 1:
				return data._cost_lv2;
			case 2:
				return data._cost_lv3;
			case 3:
				return data._cost_lv4;
			case 4:
				return data._cost_lv5;
			default:
				return data._cost_lv5;
			}
		}
	}
	
	public void SetNotificationIcon()
	{
		if (notificationSystem == null)
			notificationSystem = Services.Get<NotificationSystem>();		
		
		bool enable = notificationSystem.GetNotificationStatusForThisCell(NotificationType.Modifier, _data._id);
		notificationIcons.SetNotification(0, (enable) ? 0 : -1);
	}
	
	private void SetRankSprites()
	{
		// set up rank level & star sprites
		int level = GameProfile.SharedInstance.Player.GetArtifactLevel(_data._id);
		
		for (int i=0; i<=4; i++)
			rankSprites[i].enabled = (level >= i+1) ? true : false;
		
		bool turnStarsOn = (level == 5) ? true : false;
		for (int i=0; i<=4; i++)
			starSprites[i].enabled = turnStarsOn;	// turn on all 5 stars if maxed out at level 5
	}
	
	
	// wxj, enable buttons
	private void EnableButton( params int[] ids)
	{
		foreach (GameObject go in buttons)
			NGUITools.SetActive(go, false);
		
		foreach(int id in ids)
			NGUITools.SetActive(buttons[id], true);
	}	
}









	//void Update() {}	
								

					//SetButtonStatus(false);	// remove 'buy' button if maxed out...			//SetButtonStatus(true);//	private void SetButtonStatus(bool active)
//	{
//		gameObject.transform.Find("CellContents/GraphicsAnchor/ImageButton/Background").GetComponent<UISprite>().enabled = active;
//		gameObject.transform.Find("CellContents/GraphicsAnchor/ImageButton/Highlight").GetComponent<UISprite>().enabled = active;
//		gameObject.transform.Find("CellContents/GraphicsAnchor/Frame").GetComponent<UISprite>().enabled = active;
//		gameObject.transform.Find("CellContents/FontAnchor/LabelBuy").GetComponent<UILabel>().enabled = active;
//		gameObject.transform.Find("CellContents/GraphicsAnchor/ImageButton").GetComponent<BoxCollider>().enabled = active;
//	}	


		//int notificationValue = (enable) ? 0 : -1;
		
//		bool enable = GameProfile.SharedInstance.Player.CanAffordArtifact(data._id) &&	// we have enough currency to buy it, and...
//					!GameProfile.SharedInstance.Player.IsArtifactMaxedOut(data._id);	// it's not maxed out		
		//gameObject.transform.Find("CellContents/GraphicsAnchor/NotificationIconBg").GetComponent<UISprite>().enabled = enable;
		//gameObject.transform.Find("CellContents/IconAnchor/NotificationExclamation").GetComponent<UISprite>().enabled = enable;

	
//	private string GenerateGemmedKey(ArtifactProtoData data)
//	{	
//		return data._description.Substring(0, data._description.Length - 5) + "Gem";
//	}
	

			
			// set status and icon
//			if (GameProfile.SharedInstance.Player.IsArtifactPurchased(data._id) == false) 	//-- Check if purchased
//			{
//				gameObject.transform.Find("CellContents/Cost").GetComponent<UILabel>().text = data._cost.ToString();	// show price & coin icon if not
//				gameObject.transform.Find("CellContents/CoinDisplayIcon").GetComponent<UISprite>().spriteName = "currency_coin";
//				//gameObject.transform.Find("CellContents/Description").GetComponent<UILabel>().text = data._description;
//				gameObject.transform.Find("CellContents/SubPanel/Description").GetComponent<UILocalize>().SetKey(data._description);
//			}
//			else if (GameProfile.SharedInstance.Player.IsArtifactGemmed(data._id) == true)	//-- Check if gemmed also
//			{
//				gameObject.transform.Find("CellContents/Cost").GetComponent<UILabel>().text = " ";
//				gameObject.transform.Find("CellContents/CoinDisplayIcon").GetComponent<UISprite>().spriteName = "tools_1x1_empty_sprite"; // "empty1x1";
//				//gameObject.transform.Find("CellContents/Description").GetComponent<UILabel>().text = "GEMMED";
//				gameObject.transform.Find("CellContents/SubPanel/Description").GetComponent<UILocalize>().SetKey(data._buffDescription);
//			}	
//			else 																			// purchased, but not gemmed
//			{
//				gameObject.transform.Find("CellContents/Cost").GetComponent<UILabel>().text = "1";
//				gameObject.transform.Find("CellContents/CoinDisplayIcon").GetComponent<UISprite>().spriteName = "currency_gem";
//				gameObject.transform.Find("CellContents/SubPanel/Description").GetComponent<UILabel>().text = data._buffDescription;
//			}


			//gameObject.transform.Find("CellContents").GetComponent<CellData>().Data = data._id;
			//gameObject.transform.Find("CellContents").GetComponent<CellData>().cellParent = gameObject.transform;	

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