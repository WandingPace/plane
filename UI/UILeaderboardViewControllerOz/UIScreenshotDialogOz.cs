//using UnityEngine;
//using System;
//using System.Collections;
//
//public class UIScreenshotDialogOz : UIModalDialogOz	//MonoBehaviour
//{
//	public delegate void voidClickedHandler();
//	public static event voidClickedHandler	onNegativeResponse = null;
//	public static event voidClickedHandler	onPositiveResponse = null;
//	
//	public Transform LeftButton = null;
//	public Transform RightButton = null;
//	public Transform LeftButtonText = null;
//	public Transform RightButtonText = null;
//	public Transform DescriptionText = null;
//	public Transform TitleText = null;
//	public Transform CostText = null;	
//	public Transform BackgroundSprite = null;
//
//	public UILabel rewardLabel = null;
//	public UILabel levelLabel = null;
//	public UISprite itemIcon = null;
//	
//	public void ShowConfirmDialog(string title, string description, string negativeButtonText, string positiveButtonText) 
//	{
//		LeftButtonText.gameObject.GetComponent<UILocalize>().SetKey(negativeButtonText);	// localize the left button text
//		RightButtonText.gameObject.GetComponent<UILocalize>().SetKey(positiveButtonText);	// localize the right button text		
//		DescriptionText.gameObject.GetComponent<UILocalize>().SetKey(description);			// localize the description
//		TitleText.gameObject.GetComponent<UILocalize>().SetKey(title);						// localize the title		
//		NGUITools.SetActive(this.gameObject, true);
//	}
//	
//	public void ShowConfirmPurchaseDialog(string title, string description, string negativeButtonText, string positiveButtonText, Decimal cost) 
//	{
//		LeftButtonText.gameObject.GetComponent<UILocalize>().SetKey(negativeButtonText);	// localize the left button text
//		RightButtonText.gameObject.GetComponent<UILocalize>().SetKey(positiveButtonText);	// localize the right button text	
//		DescriptionText.gameObject.GetComponent<UILocalize>().SetKey(description);			// localize the purchase prompt
//		TitleText.gameObject.GetComponent<UILocalize>().SetKey(title);						// localize the title
//		CostText.gameObject.GetComponent<UILocalize>().SetMoney(cost);						// localize the price
//		NGUITools.SetActive(this.gameObject, true);
//	}	
//	
//	public void OnLeftButtonPress()
//	{
//		if (onNegativeResponse != null)
//			onNegativeResponse();
//		NGUITools.SetActive(this.gameObject, false);
//	}
//	
//	public void OnRightButtonPress() 
//	{
//		if (onPositiveResponse != null)
//			onPositiveResponse();
//		NGUITools.SetActive(this.gameObject, false);
//	}
//}
//
//
//
//
//	
////	public void ShowInfoDialog(string title, string description, string positiveButtonText) 
////	{
////		NGUITools.SetActive(this.gameObject, true);
////		NGUITools.SetActive(LeftButton.gameObject, false);
////		
////		if (RightButton != null)
////			RightButton.transform.position = new Vector3(0, RightButton.transform.position.y, RightButton.transform.position.z);
////		
////		if (RightButtonText) 
////		{
////			UILabel label = RightButtonText.GetComponent<UILabel>() as UILabel;
////			if (label)
////				label.text = positiveButtonText;
////		}
////		if(DescriptionText) 
////		{
////			UILabel label = DescriptionText.GetComponent<UILabel>() as UILabel;
////			if (label)
////				label.text = description;
////		}
////		if (TitleText) 
////		{
////			UILabel label = TitleText.GetComponent<UILabel>() as UILabel;
////			if (label)
////				label.text = title;
////		}
////	}
//
//	
////	public void ShowRewardDialog(string rewardText, string levelText, string itemIconName)
////	{
////		NGUITools.SetActive(this.gameObject, true);
////		
////		if (RightButton != null)
////			RightButton.transform.position = new Vector3(0, RightButton.transform.position.y, RightButton.transform.position.z);
////		
////		if (rewardLabel != null && rewardText!= null)
////			rewardLabel.text = rewardText;
////		
////		if (levelLabel != null && levelText!= null) 
////			levelLabel.text = levelText;
////		
////		if (itemIcon != null && itemIconName!= null) 
////		{
////			itemIcon.spriteName = itemIconName;
////			itemIcon.MakePixelPerfect();
////		}
////	}
//
//
//
////		if (LeftButtonText) 
////		{
////			UILabel label = LeftButtonText.GetComponent<UILabel>() as UILabel;
////			if (label)
////				label.text = negativeButtonText;
////		}
////				
////		if (RightButtonText)
////		{
////			UILabel label = RightButtonText.GetComponent<UILabel>() as UILabel;
////			if(label)
////				label.text = positiveButtonText;
////		}
//		
////		if (DescriptionText)
////		{
////			UILabel label = DescriptionText.GetComponent<UILabel>() as UILabel;
////			if (label)
////				label.text = description;
////		}		
//		
////		if (TitleText) 
////		{ 
////			UILabel label = TitleText.GetComponent<UILabel>() as UILabel;
////			if (label) { label.text = title; }
////		}
//
//	
//	//private Vector3 rightButtonCachedPosition = new Vector3(160, -76, 0);
//	
////	void Awake() 
////	{
////	//	if(RightButton != null) {
////	//		rightButtonCachedPosition = RightButton.transform.localPosition;
////	//	}
////	}
////	void Start() {
////		
////	}
//
//
////using UnityEngine;
////using System.Collections;
////
////public class UIConfirmDialogOz : MonoBehaviour
////{
////	public delegate void voidClickedHandler();
////	public static event voidClickedHandler	onNegativeResponse = null;
////	public static event voidClickedHandler	onPositiveResponse = null;
////	
////	
////	public Transform LeftButton = null;
////	public Transform RightButton = null;
////	public Transform LeftButtonText = null;
////	public Transform RightButtonText = null;
////	public Transform DescriptionText = null;
////	public Transform TitleText = null;
////	public Transform BackgroundSprite = null;
//;
////	
////	void Start() {
////		
////	}
////	
////	public void ShowConfirmDialog(string title, string description, string negativeButtonText, string positiveButtonText) {
////		if(LeftButtonText) {
////			UILabel label = LeftButtonText.GetComponent<UILabel>() as UILabel;
////			if(label) {
////				label.text = negativeButtonText;
////			}
////		}
////		if(RightButtonText) {
////			UILabel label = RightButtonText.GetComponent<UILabel>() as UILabel;
////			if(label) {
////				label.text = positiveButtonText;
////			}
////		}
////		if(DescriptionText) {
////			UILabel label = DescriptionText.GetComponent<UILabel>() as UILabel;
////			if(label) {
////				label.text = description;
////			}
////		}
////		if(TitleText) {
////			UILabel label = TitleText.GetComponent<UILabel>() as UILabel;
////			if(label) {
////				label.text = title;
////			}
////		}
////		NGUITools.SetActive(this.gameObject, true);
////	}
////	public void OnLeftButtonPress() {
////		if(onNegativeResponse != null)
////		{
////			onNegativeResponse();
////		}
////		NGUITools.SetActive(this.gameObject, false);
////	}
////	public void OnRightButtonPress() {
////		if(onPositiveResponse != null)
////		{
////			onPositiveResponse();
////		}
////		NGUITools.SetActive(this.gameObject, false);
////	}
////}
////
