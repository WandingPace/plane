//using UnityEngine;
//using System;
//using System.Collections;
//using System.Collections.Generic;
//using System.Globalization;
//
//public class MapCellData : MonoBehaviour 
//{
//	public GameObject viewController;								// make link to UIInventoryViewControllerOz in prefab, in Unity	
//	//public UIMapList mapList;
//	
//	public UILabel titleLabel;
//	public UILabel descLabel;
//	public UILabel costLabel;
//	public UISprite iconSprite;
//	public UISprite buttonIcon;	
//	public UISprite coinIcon;
//	public BoxCollider buttonCollider;		
//	
//	public MapLocation _data { get; private set; }	
//	
//	Color grey = new Color(0.5f, 0.5f, 0.5f, 1.0f);
//	
//	public List<UISprite> arrows;
//	
//	void Start()
//	{
//		gameObject.transform.Find("CellContents/Image Button").GetComponent<UIButtonMessage>().target = mapList.gameObject;	//viewController;	
//		
//		//buttonCollider = transform.Find("CellContents/Image Button").GetComponent<BoxCollider>(); 
//		//buttonIconBG = gameObject.transform.Find("CellContents/Image Button").GetComponent<UISprite>();
//		
//		Destroy(gameObject.GetComponent<UIPanel>());	// kill auto-attached UIPanel component
//				
//		if (_data != null && viewController != null)
//			Refresh();									// populate fields
//	}
//
//	public void SetData(MapLocation data)
//	{
//		//if (_data != null) { oldEarnedStatValue = _data._conditionList[0]._earnedStatValue; }		// back up old _earnedStatValue, if exists
//		_data = data;
//		Refresh();										// populate fields	
//		EnableArrow(_data.id);
//	}
//	
//	public void Refresh()								// populate fields
//	{
//		if (_data != null && viewController != null)
//		{		
//			//gameObject.transform.Find("CellContents").GetComponent<UIButtonMessage>().target = viewController;		
//			
//			// set button icon
//			string iconName = (DownloadManager.HaveAllLocationsBeenDownloaded()) ? "icon_map_fasttravel" : "icon_map_download";	//(_data.isLocallyAvailable) 
//			iconName = (_data.id == EnvironmentSetManager.SharedInstance.CurrentEnvironmentSet.SetId) ? "icon_main_upgrades" : iconName;	// current location
//			buttonIcon.spriteName = iconName;
//			
//			//gameObject.transform.Find("CellContents/Image Button/ButtonIcon").GetComponent<UISprite>().spriteName = iconName;
//				
//			// populate fields from data		
//			//titleLabel.gameObject.GetComponent<UILocalize>().SetKey(_data.title);
//			
//			//Decimal cost = _data.cost / 100.00M;
//			//costLabel.text = string.Format(Localization.instance.GetCultureInfo(), "{0:C}", cost);		//costLabel.text = String.Format("{0:C}", cost);			
//			//costLabel.gameObject.GetComponent<UILocalize>().SetMoney(cost);
//			//costLabel.text = _data.cost.ToString();
//			
//		 	//quantityLabel.text = string.Format("{0:n0}", _data.itemQuantity);
//			
//			//gameObject.transform.Find("CellContents/SpriteIcon").GetComponent<UISprite>().spriteName = _data.icon;	
//			iconSprite.spriteName = _data.icon;		//"currency_coin";
//			
//			SetButtonEnabled();
//			//string desc = SetButtonEnabledAndGetDescriptionTextKey();
//			//descLabel.gameObject.GetComponent<UILocalize>().SetKey(desc);	// set description
//		}
//	}
//	
//	private void EnableArrow(int arrowID)
//	{
//		foreach (UISprite arrow in arrows)
//			arrow.enabled = false;
//		arrows[arrowID].enabled = true;
//	}
//	
//	private void SetButtonEnabled()	//AndGetDescriptionTextKey()
//	{
//		//string descriptionKey = "";
//			
//		if (_data.id == EnvironmentSetManager.SharedInstance.CurrentEnvironmentSet.SetId)														// current location
//		{
//			titleLabel.text = Localization.SharedInstance.Get(_data.title);
//			buttonIcon.color = Color.white; 			// show 'current location' icon
//			buttonCollider.enabled = false;				// disable the button
//			//descriptionKey = "Ttl_Sub_CurLoc";			// current location
//			coinIcon.enabled = false;
//			costLabel.enabled = true;
//			costLabel.text = Localization.SharedInstance.Get("Ttl_Sub_CurLoc");
//		}
//		else if (GamePlayer.SharedInstance.HasFastTravel && buttonIcon.spriteName == "icon_map_fasttravel")										// has fast travel
//		{
//			titleLabel.text = string.Format(Localization.SharedInstance.Get("Lbl_FastTravel"), Localization.SharedInstance.Get(_data.title));
//			buttonIcon.color = grey;					// grey it out if a fast travel is in effect
//			buttonCollider.enabled = false;				// disable the button
//			//descriptionKey = "Upg_Consumables_1_Title";	// head start to {location}
//			coinIcon.enabled = false;
//			costLabel.enabled = false;
//		}
//		else if (!GamePlayer.SharedInstance.HasFastTravel && buttonIcon.spriteName == "icon_map_fasttravel")									// before fast travel
//		{
//			titleLabel.text = string.Format(Localization.SharedInstance.Get("Lbl_FastTravel"), Localization.SharedInstance.Get(_data.title));
//			buttonIcon.color = Color.white;				// show regular icon
//			buttonCollider.enabled = true;				// enable the button
//			//descriptionKey = "Upg_Consumables_1_Title";	// head start to {location}
//			coinIcon.enabled = true;
//			coinIcon.spriteName = "currency_coin";
//			costLabel.enabled = true;
//			costLabel.text = _data.cost.ToString();
//		}		
//		//else if (UIManagerOz.SharedInstance.mapVC.downloadingLocationID == EnvironmentSetManager.SharedInstance.CurrentEnvironmentSet.SetId)	// downloading
//		else if (DownloadManager.IsDownloadInProgress())	// downloading
//		{
//			titleLabel.text = string.Format(Localization.SharedInstance.Get("Loc_Downloading"), Localization.SharedInstance.Get(_data.title));
//			buttonIcon.color = grey;					// grey it out if something is being downloaded
//			buttonCollider.enabled = false;				// disable the button
//			//descriptionKey = "Loc_Downloading";			// downloading
//			coinIcon.enabled = false;
//			costLabel.enabled = false;
//		}			
//		//else if (_data.isLocallyAvailable == false)
//		else if (!DownloadManager.HaveAllLocationsBeenDownloaded())		// some or all locations haven't been downloaded yet, so give option to download
//		{
//			titleLabel.text = string.Format(Localization.SharedInstance.Get("Lbl_Download"), Localization.SharedInstance.Get(_data.title));
//			buttonIcon.color = Color.white;				// show regular icon
//			buttonCollider.enabled = true;				// enable the button
//			//descriptionKey = "Ttl_Sub_DownloadFree";	// download location for free
//			coinIcon.enabled = false;
//			//coinIcon.spriteName = "icon_notifications_exclamation";
//			costLabel.enabled = true;
//			costLabel.text = Localization.SharedInstance.Get("Lbl_Free");
//		}				
//		
//		//return descriptionKey;
//	}
//}
//	
//
//
//// DownloadManager.HowManyDownloadsInProgress() > 0
//	
//			//if (UIManagerOz.SharedInstance.mapVC.fastTravelID == _data.id)
//			//	buttonIcon.color = green;	// fast travel is in effect, to this location
//			//else 	
//	
//	//grey; // grey it out if it represents a fast travel to current location
////		else if (_data.id == EnvironmentSetManager.SharedInstance.CurrentEnvironmentSet.SetId && buttonIcon.spriteName == "icon_map_fasttravel")
////		{
////			buttonIcon.color = grey;		// grey it out if it represents a fast travel to current location
////			buttonCollider.enabled = false;	// disable the button" +
////			descriptionKey = "Ttl_Sub_CurLoc";
////		}		
//		//else if (UIManagerOz.SharedInstance.mapVC.downloadingLocationID != -1 && buttonIcon.spriteName == "icon_map_download")
//
//		//string descriptionKey = "";	
//		//string icon = "";	
//
//
////		if (iconSprite.spriteName == "icon_map_fasttravel")	// if this is a fast travel button
////		{	
////			if (GamePlayer.SharedInstance.HasFastTravel)
////			{
////				iconSprite.color = grey;	// disable it if a fast travel is in effect
////				collider.enabled = false;
////			}
////			else
////			{
////				iconSprite.color = Color.white;
////				collider.enabled = true;	
////			}
////		}
////		else if (iconSprite.spriteName == "icon_map_download")	// if this is a download button
////		{
////			if (UIManagerOz.SharedInstance.mapVC.downloadingLocationID != -1)
////			{
////				iconSprite.color = grey;	// disable it if a download is in progress
////				collider.enabled = false;
////			}
////			else
////			{
////				iconSprite.color = Color.white;
////				collider.enabled = true;	
////			}				
////		}
//	
//	//void Update() { }	
//
//
//
//			//gameObject.transform.Find("CellContents/Title").GetComponent<UILabel>().text = _data.Title;					
//			//gameObject.transform.Find("CellContents/Description").GetComponent<UILabel>().text = _data.Description;
//			//gameObject.transform.Find("CellContents/Icon").GetComponent<UISprite>().spriteName = _data.IconName;
//			//gameObject.transform.Find("CellContents/Quantity").GetComponent<UILabel>().text = GameProfile.SharedInstance.Player.consumablesPurchasedQuantity[_data.PID].ToString();			
//			
//			//titleLabel.text = _data.title;	
//			//descLabel.text = _data.description;	
//
//
//			
//			//string cultureString = Localization.instance.GetCultureInfo();	//"en-GB";	// make this string pull from some global setting (haha, global...)
//			//CultureInfo culture = new CultureInfo(cultureString);
//
//		// hack to work around NGUI scroller collider situation, which was preventing 'back' and 'play' buttons from working on objectives screen
//		//if (gameObject.collider != null)
//		//{
//		//	gameObject.collider.enabled = (gameObject.transform.position.y < -3.3f) ? false : true;
//		//}
//			
//			//progressBarFill.transform.localScale = new Vector3(400.0f * ((float)_data._earnedStatValue / (float)_data._statValue),
//			//	progressBarFill.transform.localScale.y, progressBarFill.transform.localScale.z);
//
//
//		//progressBarFill.GetComponent<UISprite>().color = new Color(1.0f, 1.0f, 0.0f, 1.0f);
//
//			//progressBarFill.GetComponent<UISprite>().color = new Color(23.0f/255.0f, 115.0f/255.0f, 98.0f/255.0f, 255.0f/255.0f);
//			
//	
////			if (gameObject.transform.position.y < -3.3f) 
////			{ 
////				gameObject.collider.enabled = false;
////			}
////			else { gameObject.collider.enabled = true; }