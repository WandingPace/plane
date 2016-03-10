using UnityEngine;
using System.Collections;

public class UIDownloadDialogOz : UIModalDialogOz	//MonoBehaviour
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
		if (progressBar && progressBar.value < p)
		{
			progressBar.value = p;
		}
	}
	
	public void StartPrompt(GameObject messageobj, bool forcedownload, bool noDownloadOKPrompt)
	{
		msgObject = messageobj;
		
		progressBar.value = 0f;		// reset progress bar value
/*
		if( DownloadManager.IsDownloading() )// currently downloading
		{
			msgObject.SendMessage("OnEnvDownloadCheckDone", false);
			return ;
		}
*/

		if( MaxCancelReached() && !forcedownload )
		{
			if (msgObject)
				msgObject.SendMessage("OnEnvDownloadCheckDone", false);
			return;
		}
 
		NGUITools.SetActive(gameObject, true);	//downloadDialogVC.appear();
		
		string title = EnvironmentSetManager.SharedInstance.AllDict[EnvironmentSetManager.DarkForestId].GetLocalizedTitle();
		UILabel label = transform.Find("Camera/CenterAnchor/Download").GetComponent<UILabel>();
		UILocalize ul = label.gameObject.GetComponent<UILocalize>();
		ul.enabled = false;
		label.text = string.Format(Localization.SharedInstance.Get ("Msg_DownloadNow"), title);		
		
		if( DownloadManager.IsDownloading() )// currently downloading
		{
			DownloadManager.SharedInstance.ResetUpdateTimer();
			UIManagerOz.HideUIItem(closeButton, false);
			UIManagerOz.HideUIItem(okButton, true);
			UIManagerOz.HideUIItem(failedButton, true);
			UIManagerOz.HideUIItem(progressBar.gameObject, false); // continue!!??
			
		 
			
		}
		else
		if( forcedownload && noDownloadOKPrompt )// go straight to download
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
			UIManagerOz.HideUIItem(closeButton, false);
			UIManagerOz.HideUIItem(okButton, false);
			UIManagerOz.HideUIItem(failedButton, true);
			UIManagerOz.HideUIItem(progressBar.gameObject, true);
			
			// stop the burstly badge showing up over the download dialogs
		 	
		}
	}
	
	void OnEnvDownloadCheckDone(bool success)
	{
		if( success )
		{
			NGUITools.SetActive(this.gameObject, false);	//disappear();
			msgObject.SendMessage("OnEnvDownloadCheckDone", success);
		}
		else
		{
//			UIManagerOz.HideUIItem(closeButton, false);
//			UIManagerOz.HideUIItem(okButton, false);
			UIManagerOz.HideUIItem(failedButton, false);
			UIManagerOz.HideUIItem(progressBar.gameObject, true);
		}
	}
	
	protected override void Awake()
	{
		base.Awake();
		numCanceled = PlayerPrefs.GetInt("DownloadDFCancelCtr", 0);
	}

	public void OnEscapeButtonClickedModel()
	{
		if( UIManagerOz.escapeHandled ) return;
		UIManagerOz.escapeHandled = true;

		
		CloseDialog();
	}
	
	public void CloseDialog()// cancel download
	{
		numCanceled++;
		PlayerPrefs.SetInt("DownloadDFCancelCtr", numCanceled);
		PlayerPrefs.Save();

		if (onNegativeResponse != null)
			onNegativeResponse();

		DownloadManager.SharedInstance.StopTimer();
		
		OnEnvDownloadCheckDone(true);
		
		DownloadManager.CancelDownload();	// cancel download, if one is in progress		
	}
	

	public void OnFailedPressed()
	{
		OnEnvDownloadCheckDone(true);
	}
	
	public void OnAMPRequestAssetListDone()
	{
//		Debug.LogError ("Actual loading starts");
		DownloadManager.DownloadAllLocations( gameObject );
	}
	
	public void OnOkPressed()
	{ 
		if (onPositiveResponse != null)
			onPositiveResponse();

//		closeButton.SetActive(false);
	//	okButton.SetActive(false);
//		progressBar.gameObject.SetActive(true);

//		UIManagerOz.HideUIItem(closeButton, true);
		UIManagerOz.HideUIItem(okButton, true);
//		UIManagerOz.HideUIItem(failedButton, false);
		UIManagerOz.HideUIItem(progressBar.gameObject, false);
		
		
		/*
		if (Application.internetReachability != NetworkReachability.ReachableViaLocalAreaNetwork )
		{
			notify.Error("Need internet");
			UIManagerOz.SharedInstance.okayDialog.ShowOkayDialog("Msg_InternetRequired", "", "Btn_Ok", gameObject);	
			return ;
		}
*/
		if( !EnvironmentSetManager.SharedInstance.IsLocallyAvailable(EnvironmentSetManager.DarkForestId) ||
			!EnvironmentSetManager.SharedInstance.IsLocallyAvailable(EnvironmentSetManager.YellowBrickRoadId) )
		{// need the version number to continue
			//if( UIManagerOz.SharedInstance.ampAssetBundleData.IsReady() == false )
			if( Services.Get<AmpBundleManager>().ampAssetBundleData.IsReady() == false )
			{
//				Debug.LogError ("waiting for amp asset list...");
				//UIManagerOz.SharedInstance.RequestAMPAssetList(gameObject);
				Services.Get<AmpBundleManager>().RequestAMPAssetList(gameObject);
				return;// not real call yet
			}
		}
		DownloadManager.DownloadAllLocations( gameObject );
	}
	
}


	
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