using System;
using UnityEngine;
using System.Collections;

public class ConsumableCellData : MonoBehaviour 
{
								
	public BaseConsumable _data;									// reference to consumable data
                                        
    public UISprite iconItem;
    
    public UILabel Counts;
    public UILabel desc;
    
    public UISprite iconCost;
    public UILabel cost;
    public GameObject btnBuy;
	
	private NotificationSystem notificationSystem;	
	private NotificationIcons notificationIcons;		
	
	protected static Notify notify;
	
	void Awake()
	{
		notify = new Notify("ConsumableCellData");
		notificationIcons = gameObject.GetComponent<NotificationIcons>();
	}
	
	void Start()
	{
	
		notificationSystem = Services.Get<NotificationSystem>();
		
		if (_data != null) 
		{
			Refresh();												// populate fields
		}

        RegisterEvent();
	}

    void RegisterEvent()
    {
        UIEventListener.Get(btnBuy).onClick = CellBuyButtonPressed;
    }
	
    public void CellBuyButtonPressed(GameObject cell)   //public void OnConsumableCellPressed(GameObject cell) 
    {
        int consumableID = _data.PID;
        PlayerStats playerStats = GameProfile.SharedInstance.Player;
        Services.Get<NotificationSystem>().ClearNotification(NotificationType.Consumable, consumableID);        
        Services.Get<NotificationSystem>().SetNotificationIconsForThisPage(UiScreenName.UPGRADES);  
        
        if (playerStats.IsConsumableMaxedOut(consumableID) == false)   
        {
           
            if (playerStats.CanAffordConsumable(consumableID) == true)
            {
                OnPurchaseYes();   
            }
            else
            {

                if(_data.CostType == CostType.Special)
                {
                    UIManagerOz.SharedInstance.StoreVC.BuyGems();
                }
                else
                {
                    UIManagerOz.SharedInstance.StoreVC.BuyCoins();
                }
             }
        }

        UIManagerOz.SharedInstance.PaperVC.UpdateCurrency();
        Refresh();                                   // ask cell to update its GUI rendering to match data, in case it was updated in transaction
    }

    IEnumerator PlayEffect()
    {
        UIDynamically.instance.ZoomInOut(Counts.gameObject,new Vector3(1.2f,1.2f,1f),0.2f,0.2f);
        iconItem.depth = 7;
        UIDynamically.instance.ZoomOutToOne(iconItem.gameObject,new Vector3(1.5f,1.5f,1f),0.5f);
        yield return new WaitForSeconds(0.5f);
        iconItem.depth = 4;
    }

    private void OnPurchaseYes()
    {
        AudioManager.SharedInstance.PlayFX(AudioManager.Effects.ui_inventory_buy);

        PlayerStats playerStats = GameProfile.SharedInstance.Player;

        StartCoroutine(PlayEffect());

        playerStats.PurchaseConsumable(_data.PID);

        if (_data.Type == ConsumableType.LuckyBox.ToString())
            UIManagerOz.SharedInstance.inventoryVC.OpenLuckyBox(_data.PID);

        UIManagerOz.SharedInstance.PaperVC.UpdateCurrency();
        Refresh();                               
    }       

    private void ShowDownloadPrompt()
    {
        UIManagerOz.SharedInstance.StartDownloadPrompts(true, false, false);
    }

	public void SetData(BaseConsumable data)
	{
		_data = data;

		Refresh();													// populate fields	
	}
	
	public void Refresh()
	{
		if (_data != null)
		{
            SetNotificationIcon();   // show notification icon if can afford to purchase this item
            
            iconItem.spriteName = _data.IconName;

            //购买宝箱不显示个数，即买即用
            if (_data.Type != ConsumableType.LuckyBox.ToString())
            {
                Counts.text = GameProfile.SharedInstance.Player.consumablesPurchasedQuantity[_data.PID].ToString();
            }

		    desc.text = Localization.SharedInstance.Get(_data.Description);
            
            iconCost.spriteName = UIManagerOz.SharedInstance.inventoryVC.GetCostIconNameByType(_data.CostType);
            cost.text = _data.Cost.ToString();
          
			// set status and icon
			if (GameProfile.SharedInstance.Player.IsConsumableMaxedOut(_data.PID) == false) 	
			{
				

			}
			else 																			
			{
				
				
			}
		}
		else
			notify.Warning("BaseConsumable (data) or UIInventoryViewControllerOz (viewController) is null in ConsumableCellData!");
	}

	public void SetNotificationIcon()
	{
		if (notificationSystem == null)
			notificationSystem = Services.Get<NotificationSystem>();		
		
		bool enable = notificationSystem.GetNotificationStatusForThisCell(NotificationType.Consumable, _data.PID);		
		notificationIcons.SetNotification(0, (enable) ? 0 : -1);
	}
}

