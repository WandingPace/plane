using UnityEngine;
using System.Collections;

public class CellBoundsChecker : MonoBehaviour 
{
	private UISprite background;
	//private UIScrollView dragPanel;
	private UIPanel panel;	
	private Vector3 yOffsetCellBounds;	// yOffsetNotifications
	
	private NotificationType cellNotificationType;
	private NotificationType saleNotificationType = NotificationType.None;
	private int id;
	
	private UIAtlas interfaceMaster, interfaceMasterOpaque;
	
	void Start()
	{
		background = transform.Find("CellContents/GraphicsAnchor/SlicedSprite (bg_storecell)").GetComponent<UISprite>();
		//yOffsetNotifications = new Vector3(0.0f, background.transform.lossyScale.y * 0.75f / 2.0f, 0.0f);	// for notification clearing	
		yOffsetCellBounds = new Vector3(0.0f, background.transform.lossyScale.y / 2.0f, 0.0f);		// for background opaque/alpha switching
		
		interfaceMaster = UIManagerOz.SharedInstance.InterfaceMaster;
		interfaceMasterOpaque = UIManagerOz.SharedInstance.InterfaceMasterOpaque;
		
		panel = transform.parent.parent.GetComponent<UIPanel>();
		//dragPanel = transform.parent.parent.GetComponent<UIScrollView>();
		//dragPanel.onDragFinished += CheckCellVisibility;

		SetNotificationTypeAndID();
	}
	public bool test=true;
	void LateUpdate()						// for background opaque/alpha switching
	{
		if(test)
		if (panel.IsVisible(background.transform.position - yOffsetCellBounds) && 
			(panel.IsVisible(background.transform.position + yOffsetCellBounds)))
		{
			background.atlas = interfaceMasterOpaque;	//SetWidgetAtlasToOpaque(true);
			Services.Get<NotificationSystem>().ClearNotification(cellNotificationType, id);
			
			if (saleNotificationType != NotificationType.None)
			{
				Services.Get<NotificationSystem>().ClearNotification(saleNotificationType, id);
			}
		}
		else
			background.atlas = interfaceMaster;			//SetWidgetAtlasToOpaque(false);
	}

	private void SetNotificationTypeAndID()
	{
		switch (gameObject.transform.parent.parent.gameObject.name)
		{
			case "PowersPanel":
				cellNotificationType = NotificationType.Powerup;
				saleNotificationType = NotificationType.PowerupSale;
				id = gameObject.GetComponent<PowerCellData>()._data.PowerID;			
				break;			
			case "ArtifactPanel":
				cellNotificationType = NotificationType.Modifier;
				saleNotificationType = NotificationType.ArtifactSale;
				id = gameObject.GetComponent<ArtifactCellData>()._data._id;
				break;
			case "ConsumablePanel":
				cellNotificationType = NotificationType.Consumable;
				saleNotificationType = NotificationType.ConsumableSale;
				id = gameObject.GetComponent<ConsumableCellData>()._data.PID;
				break;			
			case "MoreCoinsPanel":
				cellNotificationType = NotificationType.MoreCoins;
				id = 0;	//gameObject.GetComponent<StoreCellData>()._data.productID;
				break;
			case "WorldOfOzPanel":
				cellNotificationType = NotificationType.Land;
//				id = gameObject.GetComponent<WorldOfOzCellData>()._data.ID;
				break;				
			case "CharacterPanel":
				cellNotificationType = NotificationType.Character;
				saleNotificationType = NotificationType.CharacterSale;
				id = gameObject.GetComponent<CharacterCellData>()._data.characterId;
				break;				
//			case "DraggablePanel":
//				cellNotificationType = NotificationType.Challenge;
//				id = 0;	//gameObject.GetComponent<StoreCellData>()._data.productID;
//				break;				
		}
	}
	
	private void SetWidgetAtlasToOpaque(bool makeOpaque)
	{
		background.atlas = (makeOpaque) ? interfaceMasterOpaque : interfaceMaster;
	}
}





	
//	public void CheckCellVisibility()	// for notification clearing - called 'on drag released'
//	{
//		if (panel.IsVisible(background.transform.position - yOffsetNotifications) && 
//			(panel.IsVisible(background.transform.position + yOffsetNotifications)))
//		{
//			Services.Get<NotificationSystem>().ClearNotification(cellNotificationType, id);
//		}
//	}




		//CheckCellVisibility();



				//Services.Get<NotificationSystem>().SetNotificationIconsForThisPage(UiScreenName.UPGRADES);

			//SetNotificationIcon();
	
//	private void SetNotificationIcon()
//	{
//		switch (cellNotificationType)
//		{
//			case NotificationType.Powerup:
//				gameObject.GetComponent<PowerCellData>().SetNotificationIcon();	
//				break;			
//			case NotificationType.Modifier:
//				gameObject.GetComponent<ArtifactCellData>().SetNotificationIcon();	
//				break;
//			case NotificationType.Consumable:
//				gameObject.GetComponent<ConsumableCellData>().SetNotificationIcon();	
//				break;			
//			case NotificationType.MoreCoins:
//				gameObject.GetComponent<StoreCellData>().SetNotificationIcon();	
//				break;	
//		}
//	}	
	


			
			//if (cellNotificationType == NotificationType.Powerup || cellNotificationType == NotificationType.Modifier ||
			//	cellNotificationType == NotificationType.Consumable || cellNotificationType == NotificationType.MoreCoins)
			//{	
			//}
		
			//if (cellNotificationType == NotificationType.Challenge)
			//	Services.Get<NotificationSystem>().SetNotificationIconsForThisPage(UiScreenName.OBJECTIVES);


//	void OnEnable()
//	{
//		if (panel != null)
//			CheckCellVisibility();
//	}


			//private UISprite icon;

		//icon = transform.Find("CellContents/IconAnchor/SpriteIcon").GetComponent<UISprite>();//	void Update() 
//	{
//		//icon.enabled = CheckCellVisibility();
//	}


//			icon.enabled = false;	//false;
//			//return false;	//true;
//		else
//			icon.enabled = true;
//			//return true;

		//panel = transform.parent.parent.GetComponent<UIPanel>();

	
	//private Bounds cellBounds;
	//private BoxCollider coll;
	//private Bounds panelBounds;	
	

		//coll = transform.Find("CellContents").GetComponent<BoxCollider>();
		//dragPanel = transform.parent.parent.GetComponent<UIScrollView>();

		//cellBounds = coll.bounds;
		//panelBounds = dragPanel.bounds;
	