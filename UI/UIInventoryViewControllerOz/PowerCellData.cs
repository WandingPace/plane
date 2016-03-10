using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PowerCellData : MonoBehaviour 
{
	public BasePower _data;											// reference to powerup data	
    public UISprite iconItem;

    public UILabel Counts;
    public UILabel desc;

    public UISprite iconCost;
    public UILabel cost;
    public UISprite btnBuy;

	private NotificationSystem notificationSystem;
	private NotificationIcons notificationIcons;
	protected static Notify notify;

  
	
	void Awake()
	{
		if (notify != null)
			notify = new Notify("PowerCellData");
		
		notificationIcons = gameObject.GetComponent<NotificationIcons>();		
	}
	
	void Start()
	{
	
		notificationSystem = Services.Get<NotificationSystem>();
		
        ToggleBuyDisplay( true );
		
		if (_data != null)
		{
			Refresh();												// populate fields
		}
	}

	public void SetData(BasePower data)
	{
		_data = data;
		Refresh();													// populate fields	
	}
	
	private void ToggleBuyDisplay( bool active )
	{
		
		// disable sale related elements if artifact is already maxed out
		if (GameProfile.SharedInstance.Player.IsPowerPurchased(_data.PowerID))
		{
			active = false;	
		}
		
         btnBuy.spriteName = "store_buy";
	}
	


	public void Refresh()
	{
		if (_data != null )
		{	
			
			SetNotificationIcon();	// show notification icon if can afford to purchase this item
			
            iconItem.spriteName = _data.IconName;
            Counts.text = _data.amounts.ToString();
            desc.text = _data.Description;
     
            iconCost.spriteName = UIManagerOz.SharedInstance.inventoryVC.GetCostIconNameByType(_data.CostType);
            cost.text = _data.Cost.ToString();
            ToggleBuyDisplay( true ); 

			// set status and icon
			if (GameProfile.SharedInstance.Player.IsPowerPurchased(_data.PowerID) == false) 	//未购买
			{

				EnableButton(true);
			
			}
			else if (GameProfile.SharedInstance.IsPowerEquipped(_data.PowerID, GameProfile.SharedInstance.GetActiveCharacter().characterId) == true)	//-- Check if equipped also
			{
				ToggleBuyDisplay( false );
          
				EnableButton(false);
				
			}
			else 																		// purchased, but not equipped
			{
				ToggleBuyDisplay( false );
				
			}			
		}
		else
			notify.Warning("BasePower (data) or UIInventoryViewControllerOz (viewController) is null in PowerCellData!");
	}
	
	public void SetNotificationIcon()
	{
		if (notificationSystem == null)
			notificationSystem = Services.Get<NotificationSystem>();
		
		bool enable = notificationSystem.GetNotificationStatusForThisCell(NotificationType.Powerup, _data.PowerID);
		notificationIcons.SetNotification(0, (enable) ? 0 : -1);
	}	
	
	private void EnableButton(bool canclick)
	{
        btnBuy.transform.collider.enabled = canclick;
	}
	

}
