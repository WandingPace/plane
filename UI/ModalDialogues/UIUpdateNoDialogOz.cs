using UnityEngine;
using System.Collections;

public class UIUpdateNoDialogOz : UIModalDialogOz	//MonoBehaviour
{
	public GameObject closeButton;
	GameObject msgObject;

	public void StartPrompt(GameObject messageobj)
	{
		msgObject = messageobj;
		NGUITools.SetActive(gameObject, true);	//downloadDialogVC.appear();
	}
	
	public void CloseDialog()
	{ 
		NGUITools.SetActive(this.gameObject, false);	//disappear();
		if (msgObject)
			msgObject.SendMessage("OnEnvDownloadCheckDone", true);
	}	
}

	



	//public UISlider progressBar;
	
//	public delegate void voidClickedHandler();
	
//	public GameObject okButton;	
	
	
//	public static event voidClickedHandler onNegativeResponse = null;
//	public static event voidClickedHandler onPositiveResponse = null;
	
/*	
	
//	public static bool alreadyChecked = false;
	public void OnProgressUpdate(float p)
	{
		if( progressBar )
			progressBar.sliderValue = p;
	}
	
	public void StartPrompt(GameObject messageobj, string bundlename)
	{
		NGUITools.SetActive(gameObject, true);	//downloadDialogVC.appear();
		OnProgressUpdate(0.0f);
		
		Localization.SharedInstance.LoadDictionarOnly();		
	
		if( ResourceManager.SharedInstance.downloadedAssetBundles.IsAssetBundleDownloaded(bundlename) == false ) // new download, need ver info
		{// need the version number to continue
			if( UIManagerOz.SharedInstance.ampAssetBundleData.IsReady() == false )
			{
//				Debug.LogError ("Local Font UI waiting for amp asset list...");
				UIManagerOz.SharedInstance.RequestAMPAssetList(gameObject);
				return;// not real call yet
			}
		}
		
		DownloadManagerLocalization.SharedInstance.CheckLoad( gameObject );
	}

	public void OnAMPRequestAssetListDone()
	{
//		Debug.LogError ("Actual local font loading starts");
		DownloadManagerLocalization.SharedInstance.CheckLoad( gameObject );
	}		
	
	
	void OnLocalizationDownloadCheckDone(bool success)// message from the downlad manager
	{
		NGUITools.SetActive(this.gameObject, false);	//disappear();
		msgObject.SendMessage("OnLocalizationDownloadCheckDone", success);
	}
	
	protected override void Awake()
	{
		base.Awake ();
		numCanceled = PlayerPrefs.GetInt("DownloadLocalCancelCtr", 0);
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
		PlayerPrefs.SetInt("DownloadLocalCancelCtr", numCanceled);
		PlayerPrefs.Save();

		//		if (onNegativeResponse != null)
//			onNegativeResponse();
		DownloadManagerLocalization.ResetStatus();
		OnLocalizationDownloadCheckDone(false);
	}
	*/
//}




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