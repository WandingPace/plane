using UnityEngine;
using System.Collections;

public class UIRewardDialogOz : UIModalDialogOz	//MonoBehaviour
{
	public delegate void voidClickedHandler();
	//public static event voidClickedHandler onNegativeResponse = null;
	public static event voidClickedHandler onPositiveResponse = null;
	
	public Transform CenterButton = null;
	public Transform CenterButtonText = null;
	public Transform DescriptionText = null;
	public Transform TitleText = null;
	public Transform BackgroundSprite = null;
	public Transform Quantity = null;
	
	private GameObject messageObject = null;

	public UISprite itemIcon = null;
	
	

	public void ShowRewardDialog(string title, string itemIconName, string positiveButtonText, GameObject msgObject = null) 
	{
		messageObject = msgObject;
		NGUITools.SetActive(this.gameObject, true);
	
		CenterButtonText.gameObject.GetComponent<UILocalize>().SetKey(positiveButtonText);			// center button text
		//DescriptionText.gameObject.GetComponent<UILocalize>().SetKey(description);			// localize the description
		TitleText.gameObject.GetComponent<UILocalize>().SetKey(title);						// localize the title		
		//Quantity.gameObject.GetComponent<UILabel>().text = quantity.ToString();				// show the quantity
	
		if (itemIcon != null && itemIconName!= null) 
		{
			itemIcon.spriteName = itemIconName;
			//itemIcon.MakePixelPerfect();
		}	
	
		Invoke("OnCenterButtonPress", 8f);
	}
	
	public void OnCenterButtonPress()
	{
		CancelInvoke("OnCenterButtonPress");
		
		AudioManager.SharedInstance.PlayFX(AudioManager.Effects.oz_UI_Menu_back);
		
		if (onPositiveResponse != null)
			onPositiveResponse();
				
		if (messageObject)
			messageObject.SendMessage("OnRewardDialogClosed");
		
		NGUITools.SetActive(this.gameObject, false);
	}
	
	public void OnEscapeButtonClickedModel()
	{
		if( UIManagerOz.escapeHandled ) return;
		UIManagerOz.escapeHandled = true;
		
		OnCenterButtonPress();
	}	
}
