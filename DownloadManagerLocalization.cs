using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DownloadManagerLocalization : MonoBehaviour
{
	public static DownloadManagerLocalization SharedInstance;

	private static AssetBundleLoader loader;				// reference to loader for download currently in progress
//	private static int downloadsTriggered;					// number of downloads requested at once and thrown into the queue
	private static float progress;							// actual total progress of all downloads in progress
	private	string bundleName = "";
	
	protected static Notify notify;

	public float UdateInterval = 0.3f;
	private static float mTimer = 0.0f;
	
	private GameObject msgObject = null;
	
	void Awake()
	{
		ResetStatus();
		SharedInstance = this;
		notify= new Notify(this.GetType().Name);
	}
	
	void ResetUpdateTimer()
	{
		mTimer = UdateInterval;
	}
	
	void Update()
	{
		if( mTimer > 0.0f )
		{
			mTimer -= Time.deltaTime;
			if( mTimer <= 0.0f )
			{
				SendOutUpdateMessages();
				SharedInstance.ResetUpdateTimer();
			}
		}
	}	
	
	public static void ResetStatus()
	{
		loader = null;
		progress = 1.0f;
		mTimer = 0.0f;
		if( SharedInstance ) 
			SharedInstance.bundleName = "";
	}
	
	void CheckCompleted(bool t)// called when the download is successful or failed
	{
		if( msgObject )
		{
			msgObject.SendMessage("OnLocalizationDownloadCheckDone", t);
		}
	}
	
	public void CheckLoad( GameObject messageObject)
	{
		msgObject = messageObject;
		
		// first decide which localization bundle to download
		
		string  sysLanguage = Localization.SharedInstance.GetLangBySystem();
		notify.Debug("Sysfont = " + sysLanguage + "  loaded = " +  Localization.SharedInstance.GetLoadedLanguage());
//		
		bundleName = Localization.SharedInstance.GetAssetBundleName(sysLanguage); //"local_" + sysLanguage.ToLower();
		if( ResourceManager.SharedInstance.IsAssetBundleDownloadedLastestVersion( bundleName ) )// latest downloaded
		{
			if( Localization.SharedInstance.GetLoadedLanguage() == sysLanguage)// already loaded 
			{
				CheckCompleted(true); // no need,
				return;
			}
		}
		
		if ( bundleName != "" )// need to download localization resources
		{
			notify.Debug("Load localization bunlde = " + bundleName);

			ResourceManager.SharedInstance.RegisterForOnAssetBundleLoadSuccess(OnAssetBundleLoadedSuccess);
			ResourceManager.SharedInstance.RegisterForOnAssetBundleLoadFailure(OnAssetBundleLoadedFailure);	
			
			if( ResourceManager.SharedInstance.IsAssetBundleDownloadedLastestVersion( bundleName ) ||
				Application.internetReachability != NetworkReachability.NotReachable )
			{
				// just go ahead and load it
				DownloadAsset();
			}
			else
			{
//				Debug.LogError ("cannot load now");
				CheckCompleted(false); // no download,
			}
		}
		else
		{
//			Debug.LogError ("No need to load");
			CheckCompleted(true); // no need,
		}
	}
	
	
	public static bool IsDownloadInProgress()				// true if download is currently in progress
	{
		bool dlStatus = (loader != null) ? true : false;
		return dlStatus;
	}
	
	
	public void DownloadAsset()				// trigger the download of all un-downloaded locations
	{	
		SendOutUpdateMessages();							// ask map & play button to refresh itself
		ResetUpdateTimer(); // stop update
//		MyUIProgressBar.ShowdProgresBar(true);
		
//		downloadsTriggered = 1;				// store total number of downloads to do, for progress bar calculation
		loader = ResourceManager.SharedInstance.LoadAssetBundle(bundleName, false, -1,false, false);	
		
	}
	
	public static float GetTotalDownloadProgress()		// get progress total of all items being downloaded, in range 0.0f - 1.0f
	{
		if (loader != null )
			progress = loader.GetProgress();
			//progress = loader.GetProgress();
		else
		{
//			Debug.Log("No Loader");
			progress = 1.0f;
		}
		
		return progress;
	}
	
	private static void SendOutUpdateMessages()
	{
		SharedInstance.msgObject.SendMessage ("OnProgressUpdate", GetTotalDownloadProgress());
//		UIProgressBar.SetProgress(GetTotalDownloadProgress());
	}	

	private void OnAssetBundleLoadedSuccess(string assetBundleName, int version, bool downloadOnly)
	{	
		if( bundleName != assetBundleName )
		{
			notify.Debug("Localization download ignored success callback for " + assetBundleName+ ". Expecting bundle name = " + bundleName);
			return ;
		}
		
		notify.Debug("Downloaded Localization assetBundleName = " + assetBundleName);
		ResourceManager.SharedInstance.UnRegisterForOnAssetBundleLoadSuccess(OnAssetBundleLoadedSuccess);	// stop listening for this event
		ResourceManager.SharedInstance.UnRegisterForOnAssetBundleLoadFailure(OnAssetBundleLoadedFailure);	// stop listening for this event
			
		ResetStatus();

		mTimer = 0.0f; // stop update
//		MyUIProgressBar.ShowdProgresBar(false);
		CheckCompleted(true);
		
	}		
	
	private void OnAssetBundleLoadedFailure(string assetBundleName, int version, string errMsg)
	{
		if( bundleName != assetBundleName )
		{
			notify.Debug("Localization download ignored fail callback for " + assetBundleName+ ". Expecting bundle name = " + bundleName);
			return ;
		}
		
		ResourceManager.SharedInstance.UnRegisterForOnAssetBundleLoadSuccess(OnAssetBundleLoadedSuccess);	// stop listening for this event
		ResourceManager.SharedInstance.UnRegisterForOnAssetBundleLoadFailure(OnAssetBundleLoadedFailure);	// stop listening for this event
		
		notify.Error("Downloaded localization failed assetBundleName = " + assetBundleName);
		ResetStatus();
		mTimer = 0.0f; // stop update
		CheckCompleted(false);
		
//		UIManagerOz.SharedInstance.okayDialog.ShowOkayDialog("Msg_DownloadFail", "", "Btn_Ok", gameObject);		

	}
	
	void OnOkayDialogClosed()
	{
		CheckCompleted(false);
	}
	
}
