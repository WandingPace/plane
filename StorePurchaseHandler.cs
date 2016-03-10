using UnityEngine;
using System.Collections;

public class StorePurchaseHandler : MonoBehaviour 
{
	IAPWrapper iapWrapper;
	public static bool dontShowPurchaseResultDlg = false;
	
	protected static Notify notify;
	
	void Awake()
	{
		notify = new Notify(this.GetType().Name);
	}
	
	void Start() 
	{
		iapWrapper = GameObject.Find("NonVisibleObjects/IAPWrapper").GetComponent<IAPWrapper>();
		ConnectPurchaseHandlers();
	}
	
	private void ConnectPurchaseHandlers()
	{	
		iapWrapper.RegisterForPurchaseSuccessful(PurchaseSuccessful);
		iapWrapper.RegisterForPurchaseFailed(PurchaseFailed);	
		iapWrapper.RegisterForPurchaseCancelled(PurchaseCancelled);
 	
	}
	
	private void PurchaseSuccessful(string productIdentifier)
	{
		//System.Diagnostics.StackTrace t = new System.Diagnostics.StackTrace();
		//notify.Debug(t);
		
		IAP_DATA productData = IAPWrapper.iapTable[productIdentifier];
		
		// give player appropriate quantity of primary item purchased
		if (productData.costType == CostType.Coin) 
			GameProfile.SharedInstance.Player.coinCount += productData.costValueOrID;
		else if (productData.costType == CostType.Special) 
			GameProfile.SharedInstance.Player.specialCurrencyCount += productData.costValueOrID;			
		
		// give player appropriate quantity of secondary item purchased
		if (productData.costTypeSecondary == CostType.Coin) 
		{
			GameProfile.SharedInstance.Player.coinCount += productData.costValueSecondary;
		 
		}
		else if (productData.costTypeSecondary == CostType.Special) 
		{
			GameProfile.SharedInstance.Player.specialCurrencyCount += productData.costValueSecondary;		
		 
		}
		GameProfile.SharedInstance.Serialize();
		UIManagerOz.SharedInstance.PaperVC.UpdateCurrency();
		Services.Get<NotificationSystem>().SetNotificationIconsForThisPage(UiScreenName.UPGRADES);
	 
		
	//	SharingManagerBinding.HideBusyIndicator();
		
		//UIManagerOz.SharedInstance.okayDialog.ShowOkayDialog("Purchase successful", "for item with id: " + productIdentifier, "Btn_Ok");
		
		if( !dontShowPurchaseResultDlg ) 
		{
			bool isAmazonStore = (Settings.GetString("android-store","") == "amazon");
			if ( ! isAmazonStore)
			{
				// amazon's guidelines say don't show this in game thank you dialog
				if( !GameController.SharedInstance.IsInCountdown )
				{
					if(UIManagerOz.SharedInstance.inGameVC.pauseButton.activeSelf)
					{
						notify.Debug("pause button is active calling OnPauseClickedUI");
						//GameController.SharedInstance.OnPauseClickedUI();
						UIManagerOz.SharedInstance.inGameVC.OnPause();
					}
					else
					{
						notify.Debug("pause button is NOT active");
					}
					UIManagerOz.SharedInstance.okayDialog.ShowOkayDialog("Msg_ThankYouPurchase", "Btn_Ok");
				}
			}
		}
		
		//PauseGameIfActive();
	}
	
	private void PurchaseFailed(string error, bool isErrorLocalized)
	{
		//notify.Debug("Alex fixme: check isErrorLocalized, remove warning when done");
		
		//UIManagerOz.SharedInstance.okayDialog.ShowOkayDialog("Purchase failed", "for item with id: " + productIdentifier, "Btn_Ok");
//		UnityEngine.Debug.LogError ("Error = " + error + " is localized = " + isErrorLocalized);
		
	 
		
		if( dontShowPurchaseResultDlg ) 
			return;
		
		if (isErrorLocalized)
		{
#if UNITY_ANDROID
			//error = StripGoogleParens(error);
#endif			
//			UnityEngine.Debug.LogError ("localized Error = " + error );
			UIManagerOz.SharedInstance.okayDialog.ShowOkayDialog(error, "Btn_Ok", true);			//Product list request failed, show localized error
		}
		else
			UIManagerOz.SharedInstance.okayDialog.ShowOkayDialog("Msg_InternalError", "Btn_Ok");	//Lbl_Sto_NoStoreConn
		
		//PauseGameIfActive();
	}	
	
	private void PurchaseCancelled(string error, bool isErrorLocalized)
	{
		if (isErrorLocalized)
		{
#if UNITY_ANDROID
			//error = StripGoogleParens(error);
#endif
			UIManagerOz.SharedInstance.okayDialog.ShowOkayDialog(error, "Btn_Ok", true);			//Purchase was cancelled, show localized error
		}
		else
			UIManagerOz.SharedInstance.okayDialog.ShowOkayDialog("Msg_InternalError", "Btn_Ok");
		
		//PauseGameIfActive();
	}	
	
	private void PauseGameIfActive()
	{
		//if (GameController.SharedInstance.gameState == GameState.IN_RUN)
			// need a way to pause the game reliably in any condition without it breaking...
	}
	
	private string StripGoogleParens(string message)
	{
		// google store adds a message in parentheses at the end of every message, get rid of it
		string returnMessage = message;
		int startParen = message.IndexOf("(");
		int endParen = message.LastIndexOf(")");
		
		if (startParen != -1 && endParen != -1)
			returnMessage = message.Remove(startParen, endParen - startParen + 1);	
		
		return returnMessage;
	}
}







//	private void DisconnectPurchaseHandlers()
//	{
//		iapWrapper.UnregisterForPurchaseSuccessful(PurchaseSuccessful);
//		iapWrapper.UnregisterForPurchaseFailed(PurchaseFailed);		
//		iapWrapper.UnregisterForPurchaseCancelled(PurchaseCancelled);		
//	}
//	