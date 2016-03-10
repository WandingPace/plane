using UnityEngine;
using System.Collections;

public class UIDownloadDialogUI : UIModalDialogOz	//MonoBehaviour
{
	public GameObject okButton = null;
	public GameObject failedButton = null;
	public GameObject closeButton;
	public UISlider progressBar;
	
	public delegate void voidClickedHandler();
	public static event voidClickedHandler onNegativeResponse = null;
	public static event voidClickedHandler onPositiveResponse = null;
	
	private GameObject msgObject = null;// notified by send message when the operation is done, so the UI manager can start the next prompt
	
	private int numCanceled = 0;
	private int maxNumCancel = 5;

	public bool MaxCancelReached()
	{
		return ( numCanceled >= maxNumCancel );
	}	
	
	public void OnProgressUpdate(float p)
	{
		if( progressBar )
			progressBar.value = p;
	}
	
	public void StartPrompt(GameObject messageobj, bool forcedownload, bool noDownloadOKPrompt)
	{
//		Debug.LogError("UI download requested");
		msgObject = messageobj;
		
		if( MaxCancelReached() && !forcedownload)
		{
			msgObject.SendMessage("OnUIDownloadCheckDone", false);
			return ;
		}

		if( DownloadManagerUI.IsDownloading() )// currently downloading
		{// continue
			NGUITools.SetActive(gameObject, true);	//downloadDialogVC.appear();
			UIManagerOz.HideUIItem(closeButton, false);
			UIManagerOz.HideUIItem(okButton, true);
			UIManagerOz.HideUIItem(failedButton, true);
			UIManagerOz.HideUIItem(progressBar.gameObject, false); // continue!!??
			DownloadManagerUI.SharedInstance.ResetUpdateTimer();
			
			// stop the burstly badge showing up over the download dialogs
			//SharingManagerBinding.ShowBurstlyBannerAd( "idol_badge", false );
		}
		else
//		failedButton.SetActive(false);
 		if (ResourceManager.SharedInstance.IsAssetBundleDownloadedLastestVersion( DownloadManagerUI.bundleName) )
		{// go ahead and download it
//				Debug.LogError("UI:already local, just go ahead");
//			okButton.SetActive(false);
//			progressBar.gameObject.SetActive(true);

			/*
			UIManagerOz.HideUIItem(closeButton, true);
			UIManagerOz.HideUIItem(okButton, true);
			UIManagerOz.HideUIItem(progressBar.gameObject, false);
			
			*/			
			DownloadManagerUI.SharedInstance.CheckLoad( msgObject ); // just load without showing any UI
		}
		else
		{
			NGUITools.SetActive(gameObject, true);	//downloadDialogVC.appear();
			if( forcedownload && noDownloadOKPrompt)
			{
				UIManagerOz.HideUIItem(closeButton, false);
				UIManagerOz.HideUIItem(okButton, true);
				UIManagerOz.HideUIItem(failedButton, true);
				UIManagerOz.HideUIItem(progressBar.gameObject, false);
			// 
				OnOkPressed();
			}
			else
			{
				UIManagerOz.HideUIItem(failedButton, true);
				UIManagerOz.HideUIItem(closeButton, false);
				UIManagerOz.HideUIItem(okButton, false);
				UIManagerOz.HideUIItem(progressBar.gameObject, true);
				
			}
			// stop the burstly badge showing up over the download dialogs
				//	SharingManagerBinding.ShowBurstlyBannerAd( "idol_badge", false );
		}
	}
	
	void OnUIDownloadCheckDone(bool success)// message from the downlad manager
	{
		if( success )
		{
			NGUITools.SetActive(this.gameObject, false);	//disappear();
			msgObject.SendMessage("OnUIDownloadCheckDone", success);
		}
		else
		{
//		UIManagerOz.HideUIItem(closeButton, true);
//		UIManagerOz.HideUIItem(okButton, true);
			UIManagerOz.HideUIItem(failedButton, false);
			UIManagerOz.HideUIItem(progressBar.gameObject, true);
			
//			progressBar.gameObject.SetActive(false);
//			okButton.SetActive(true);
		}
	}
	
	protected override void Awake()
	{
		base.Awake();
		numCanceled = PlayerPrefs.GetInt("DownloadUICancelCtr", 0);
//		Debug.Log ("UI canceled time = " + numCanceled);
	}

	public void OnEscapeButtonClickedModel()
	{
		if( UIManagerOz.escapeHandled ) return;
		UIManagerOz.escapeHandled = true;

		
		CloseDialog();
	}
	

	public void CloseDialog()
	{
		numCanceled++;
		PlayerPrefs.SetInt("DownloadUICancelCtr", numCanceled);
		PlayerPrefs.Save();
		
		if (onNegativeResponse != null)
			onNegativeResponse();
		
		DownloadManagerUI.SharedInstance.StopTimer();

		OnUIDownloadCheckDone(true);
		
		//***** the following line was commented out because it was causing a crash  *****
		//***** when attempting to cancel HD asset download, couldn't figure out why *****
		//DownloadManagerUI.CancelDownload();	// cancel download, if one is in progress
	}

	public void OnFailedPressed()
	{
		OnUIDownloadCheckDone(true);
	}
	
	
	public void OnOkPressed()
	{ 
		if (onPositiveResponse != null)
			onPositiveResponse();
		
//		UIManagerOz.HideUIItem(closeButton, true);
		UIManagerOz.HideUIItem(okButton, true);
//		UIManagerOz.HideUIItem(failedButton, true);
		UIManagerOz.HideUIItem(progressBar.gameObject, false);
		
//		closeButton.SetActive(false);
//		okButton.SetActive(false);	public void OnAMPRequestAssetListDone()
//		progressBar.gameObject.SetActive(true);
/*		
		if (Application.internetReachability != NetworkReachability.ReachableViaLocalAreaNetwork )
		{
			notify.Error("Need internet");
			UIManagerOz.SharedInstance.okayDialog.ShowOkayDialog("Msg_InternetRequired", "", "Btn_Ok", gameObject);	
			return ;
		}
*/
		
		if( ResourceManager.SharedInstance.downloadedAssetBundles.IsAssetBundleDownloaded(DownloadManagerUI.bundleName) == false ) // new download, need ver info
		{// need the version number to continue
			//if( UIManagerOz.SharedInstance.ampAssetBundleData.IsReady() == false )
			if( Services.Get<AmpBundleManager>().ampAssetBundleData.IsReady() == false )
			{
//				Debug.LogError ("HIREs UI waiting for amp asset list...");
				//UIManagerOz.SharedInstance.RequestAMPAssetList(gameObject);
				Services.Get<AmpBundleManager>().RequestAMPAssetList(gameObject);
				return;// not real call yet
			}
		}
		
		DownloadManagerUI.SharedInstance.CheckLoad( gameObject );		
	}

	public void OnAMPRequestAssetListDone()
	{
//		Debug.LogError ("Actual UI loading starts");
		DownloadManagerUI.SharedInstance.CheckLoad( gameObject );		
	}	

	void OnOkayDialogClosed()
	{
		okButton.SetActive(true);
	}
}



//	public void DownloadAllLocations()
//	{
//		RemoveCallbacks();
//		DownloadManager.DownloadAllLocations( msgObject );
//		//notify.Warning("DOWNLOAD BEGINS NOW!!");
//	}	


//	public void ShowConfirmDialog()
//	{
//		//notify.Warning("ShowConfirmDialog");
//		//UIDownloadDialogOz.onPositiveResponse -= ShowConfirmDialog;
//		
//		UIConfirmDialogOz.onNegativeResponse += DownloadNone;
//		UIConfirmDialogOz.onPositiveResponse += DownloadAllLocations;
//		UIManagerOz.SharedInstance.confirmDialog.ShowConfirmDialog("Lbl_BeginDownloading","", "Btn_No", "Btn_Yes");
//	}
//	
//	public void RemoveCallbacks()
//	{
//		UIConfirmDialogOz.onNegativeResponse -= RemoveCallbacks;
//		UIConfirmDialogOz.onPositiveResponse -= DownloadAllLocations;
//	}	
//
//	public void DownloadNone()// negative
//	{
//		RemoveCallbacks();
//		OnEnvDownloadCheckDone();
//	}	


	
//	public void OnLeftButtonPress()
//	{
//		if (onNegativeResponse != null)
//			onNegativeResponse();
//		NGUITools.SetActive(this.gameObject, false);
//		OnEnvDownloadCheckDone();
//	}
//	
//	public void OnRightButtonPress() 
//	{
//		if (onPositiveResponse != null)
//			onPositiveResponse();
//		NGUITools.SetActive(this.gameObject, false);
//		OnEnvDownloadCheckDone();
//	}	

	
//	public override void disappear(bool hidePaper = true)
//	{
//		notify.Debug("disappear");
//		gameObject.SetActive(false);
//
//	}
	

//: UIViewControllerOz 