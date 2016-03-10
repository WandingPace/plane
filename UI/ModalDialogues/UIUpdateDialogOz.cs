using UnityEngine;
using System;
using System.Collections;
using System.IO;

public class UIUpdateDialogOz : UIModalDialogOz	//MonoBehaviour
{
	public UISlider progressBar;

	private GameObject msgObject = null;	// notified by send message when operation is done, so UI manager can start next prompt
	
	private TimeSpan fifteenSeconds = new TimeSpan(0,0,0,15);
	private DateTime lastTimeStuck;
	private float lastSliderValue = 0f;

	public void OnProgressUpdate(float p)
	{
//		if (progressBar)
//			progressBar.sliderValue = p;
		
		if (progressBar && progressBar.value < p)
		{
			progressBar.value = p;						// update progress bar, but only if it's moving to the right
		}	
	}
	
	void Update()
	{
		if (lastSliderValue != progressBar.value)			// check if it's stuck
		{
			lastTimeStuck = DateTime.UtcNow;					// if not, store the new time
		}
		
		if ((DateTime.UtcNow - lastTimeStuck) > fifteenSeconds)	// if it's been stuck for more than 15 seconds, give up
		{
			CloseDialog();
		}
		
		lastSliderValue = progressBar.value;				// store the previous value
	}
	
	public void StartPrompt(GameObject messageobj, bool forcedownload, bool noDownloadOKPrompt)
	{
		lastTimeStuck = DateTime.UtcNow;
		
		msgObject = messageobj;
		NGUITools.SetActive(gameObject, true);			//downloadDialogVC.appear();
		
		progressBar.value = 0f;		// reset progress bar value
		
		//CleanUpOldBundleFiles("darkforest");
		//CleanUpOldBundleFiles("yellowbrickroad");
		
		//string title = EnvironmentSetManager.SharedInstance.AllDict[EnvironmentSetManager.DarkForestId].GetLocalizedTitle();
		//UILabel label = transform.Find("Camera/CenterAnchor/Description").GetComponent<UILabel>();
		//UILocalize ul = label.gameObject.GetComponent<UILocalize>();
		//ul.enabled = false;
		//label.text = string.Format(Localization.SharedInstance.Get("Msg_VisitWCandDF"), title);		
		
		//if (DownloadManager.IsDownloading())			// currently downloading
		//	DownloadManager.SharedInstance.ResetUpdateTimer();
		//else 
		if (forcedownload && noDownloadOKPrompt)	// go straight to download
		{
			UIManagerOz.HideUIItem(progressBar.gameObject, false);
			OnOkPressed();
		}
		else
			UIManagerOz.HideUIItem(progressBar.gameObject, true);
	}
	
	void OnEnvDownloadCheckDone(bool success)
	{
		if (success)
		{
			NGUITools.SetActive(this.gameObject, false);	//disappear();
			UIManagerOz.SharedInstance.newLocDialog.StartPrompt(msgObject);
			//msgObject.SendMessage("OnEnvDownloadCheckDone", success);
		}
		else
		{
			//UIManagerOz.HideUIItem(progressBar.gameObject, true);
			NGUITools.SetActive(this.gameObject, false);	//disappear();
			UIManagerOz.SharedInstance.updateNoDialog.StartPrompt(msgObject);
		}
	}

	public void OnAMPRequestAssetListDone()
	{
//		Debug.LogError ("Actual loading starts");
		DownloadManager.DownloadAllLocations(gameObject);
	}
	
	private void CloseDialog()		// cancel download, when timed out
	{
		DownloadManager.CancelDownload();
		OnEnvDownloadCheckDone(false);
	}	

	public void OnOkPressed()
	{ 
		UIManagerOz.HideUIItem(progressBar.gameObject, false);

		if (!EnvironmentSetManager.SharedInstance.IsEverythingLocallyAvailable())
//			// need the version number to continue
			//if (UIManagerOz.SharedInstance.ampAssetBundleData.IsReady() == false)
			if (Services.Get<AmpBundleManager>().ampAssetBundleData.IsReady() == false)				
			{
//				Debug.LogError ("waiting for amp asset list...");
				//UIManagerOz.SharedInstance.RequestAMPAssetList(gameObject);
				Services.Get<AmpBundleManager>().RequestAMPAssetList(gameObject);
				return;// not real call yet
			}
//		
		DownloadManager.DownloadAllLocations( gameObject );
	}
	
	private void CleanUpOldBundleFiles(string filename)	//, string fullname)
	{
		//notify.Debug( "----- Cleaning up old bundle files, new bundle file =  " + fullname );
		notify.Debug( "----- filter : " + filename );
		
		string path = Application.persistentDataPath;
		string [] flist = Directory.GetFiles(path);
		if( flist == null )
		{
			notify.Debug( "Bad Folder!!!!");
			return ;
		}
		
		foreach( string f in flist )
		{
			if( f != null && f.Contains(filename))	// &&  f != fullname )
			{
				notify.Debug( "!!!!!!!! old bundle file deleted : " + f );
				File.Delete(f);
			}
		}
	}	
}





//	public void CloseDialog()	// cancel download
//	{
//		DownloadManager.SharedInstance.StopTimer();		
//		OnEnvDownloadCheckDone(true);
//	}
//
//	public void OnFailedPressed()
//	{
//		OnEnvDownloadCheckDone(true);
//	}
	

		
/*
		if( DownloadManager.IsDownloading() )// currently downloading
		{
			msgObject.SendMessage("OnEnvDownloadCheckDone", false);
			return ;
		}
*/


			//UIManagerOz.HideUIItem(closeButton, false);
			//UIManagerOz.HideUIItem(okButton, true);
			//UIManagerOz.HideUIItem(failedButton, true);
			
				//UIManagerOz.HideUIItem(closeButton, false);
			//UIManagerOz.HideUIItem(okButton, false);
			//UIManagerOz.HideUIItem(failedButton, true);		
			//UIManagerOz.HideUIItem(closeButton, false);
			//UIManagerOz.HideUIItem(okButton, true);
			//UIManagerOz.HideUIItem(failedButton, true);
			//UIManagerOz.HideUIItem(progressBar.gameObject, false); // continue!!??
						

//			UIManagerOz.HideUIItem(closeButton, false);
//			UIManagerOz.HideUIItem(okButton, false);
//			UIManagerOz.HideUIItem(failedButton, false);

//		numCanceled++;
//		PlayerPrefs.SetInt("DownloadDFCancelCtr", numCanceled);
//		PlayerPrefs.Save();

//		if (onNegativeResponse != null)
//			onNegativeResponse();

			
//		if (onPositiveResponse != null)
//			onPositiveResponse();

//		closeButton.SetActive(false);
	//	okButton.SetActive(false);
//		progressBar.gameObject.SetActive(true);

//		UIManagerOz.HideUIItem(closeButton, true);
//		UIManagerOz.HideUIItem(okButton, true);
//		UIManagerOz.HideUIItem(failedButton, false);
		
		/*
		if (Application.internetReachability != NetworkReachability.ReachableViaLocalAreaNetwork )
		{
			notify.Error("Need internet");
			UIManagerOz.SharedInstance.okayDialog.ShowOkayDialog("Msg_InternetRequired", "", "Btn_Ok", gameObject);	
			return ;
		}
*/
	
	//public GameObject closeButton;
	
//	public delegate void voidClickedHandler();
//	public static event voidClickedHandler onNegativeResponse = null;
//	public static event voidClickedHandler onPositiveResponse = null;
