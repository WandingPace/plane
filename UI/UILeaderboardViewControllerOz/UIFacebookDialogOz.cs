using UnityEngine;
using System;
using System.Collections;

public class UIFacebookDialogOz : UIModalDialogOz	//MonoBehaviour
{
	public delegate void voidClickedHandler();
	public static event voidClickedHandler onNegativeResponse = null;
	public static event voidClickedHandler onPositiveResponse = null;
	
	public void ShowFacebookLoginDialog() 
	{
		NGUITools.SetActive(this.gameObject, true);
	}
	
	public void OnCloseButtonPress()
	{
		if (onNegativeResponse != null)
			onNegativeResponse();
		NGUITools.SetActive(this.gameObject, false);
	}
	
	public void OnLoginButtonPress() 
	{
		if (onPositiveResponse != null)
			onPositiveResponse();
		NGUITools.SetActive(this.gameObject, false);
	}
	
	public void OnEscapeButtonClickedModel()
	{
		if( UIManagerOz.escapeHandled ) return;
		UIManagerOz.escapeHandled = true;
		
		OnCloseButtonPress();// left always??!!
	}		
}
