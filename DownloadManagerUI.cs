using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DownloadManagerUI : MonoBehaviour
{
	public static DownloadManagerUI SharedInstance;

	private static AssetBundleLoader loader;				// reference to loader for download currently in progress
//	private static int downloadsTriggered;					// number of downloads requested at once and thrown into the queue
	private static float progress;							// actual total progress of all downloads in progress
	
	protected static Notify notify;

	public float UdateInterval = 0.3f;
	public float mTimer = 0.0f;
	
	
	public static bool needUIBundle = false;
	public static string loadUIName = "";
	public static string loadUINameOpaque = "";
	public static string loadUIName2 = "";
	public static string bundleName = "hiresresources";
	public static string loadingBundleName = "";
	
	public static int latestVersionNumber = -1;
	
	private GameObject msgObject = null;
	
	void Awake()
	{
		ResetStatus();
		SharedInstance = this;
		notify= new Notify(this.GetType().Name);
	}
	
	public void ResetUpdateTimer()
	{
		mTimer = UdateInterval;
	}
	
	public void StopTimer()
	{
		mTimer = 0.0f;
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
//		downloadsTriggered = 0;
		progress = 1.0f;
		loadingBundleName = "";
		if( SharedInstance )
			SharedInstance.mTimer = 0.0f;
	}
	
	public static bool IsDownloading()
	{
		return loadingBundleName != "";
	}	
	
	void CheckCompleted(bool success)
	{
		if( msgObject )
		{
			msgObject.SendMessage("OnUIDownloadCheckDone", success, SendMessageOptions.DontRequireReceiver);
		}
	}
	

	public void CheckLoad( GameObject messageObject)
	{
		msgObject = messageObject;
		ResourceManager.SharedInstance.RegisterForOnAssetBundleLoadSuccess(OnAssetBundleLoadedSuccess);
		ResourceManager.SharedInstance.RegisterForOnAssetBundleLoadFailure(OnAssetBundleLoadedFailure);	

		DownloadUI();
	}
	
	
	public static bool IsDownloadInProgress()				// true if download is currently in progress
	{
		bool dlStatus = (loader != null) ? true : false;
		return dlStatus;
	}
	
	
	public static bool NeedHiresUI()		// returns false if there exists an un-downloaded location
	{
//		Debug.LogError("NeedHD = " + needUIBundle);
		return false;
	}
	
	
	public static void CancelDownload()
	{
		if (IsDownloadInProgress())
		{
			notify.Debug("CancelDownload in DownloadManagerUI");
			ResourceManager.SharedInstance.CancelDownload(loadingBundleName);
			loader = null;
		}
	}
	
	
	public static void DownloadUI()				// trigger the download of all un-downloaded locations
	{	
		if(loadingBundleName != "")
		{
			notify.Error("DownloadUI when already loading " + bundleName + " bundle name = " + loadingBundleName);
			return ;
		}
		SharedInstance.ResetUpdateTimer(); // stop update
		
//		downloadsTriggered = 1;				// store total number of downloads to do, for progress bar calculation
		loadingBundleName = bundleName;
		loader = ResourceManager.SharedInstance.LoadAssetBundle(bundleName, false, latestVersionNumber, false, false);
		
		SendOutUpdateMessages();							// ask map & play button to refresh itself
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
		SharedInstance.msgObject.SendMessage ("OnProgressUpdate", GetTotalDownloadProgress(), SendMessageOptions.DontRequireReceiver);
	}
	
	public void FinishingUp()
	{
		ResourceManager.SharedInstance.UnRegisterForOnAssetBundleLoadSuccess(OnAssetBundleLoadedSuccess);	// stop listening for this event
		ResourceManager.SharedInstance.UnRegisterForOnAssetBundleLoadFailure(OnAssetBundleLoadedFailure);	// stop listening for this event
			
		ResetStatus();
	}

	private void OnAssetBundleLoadedSuccess(string assetBundleName, int version, bool downloadOnly)
	{	
//		Debug.LogError("Dloaded " + assetBundleName + " waiting for " + bundleName );
		
		if( loadingBundleName != assetBundleName )
		{
			notify.Debug("UI Download ignored success callback for " + assetBundleName+ ". Expecting bundle name = " + loadingBundleName);
			return ;
		}

		//		Debug.LogError("Downloaded HIRES UI assetBundleName = ");
		notify.Debug("Downloaded HIRES UI assetBundleName = " + assetBundleName);
		
		FinishingUp();
		
		GameObject go = ResourceManager.Load(loadUIName, typeof(GameObject)) as GameObject;
		GameObject goOpaque = ResourceManager.Load(loadUINameOpaque, typeof(GameObject)) as GameObject;
		GameObject go2 = ResourceManager.Load(loadUIName2, typeof(GameObject)) as GameObject;
		
		if( go != null && go2 != null && goOpaque != null )
		{
			UIManagerOz.UIResolutionType chosenType = UIManagerOz.UIResolutionType.kResolution1136;
			
			if (Screen.height > 2048) 
			{ 
				chosenType = UIManagerOz.UIResolutionType.kResolution2048;
			} 
			
			UIManagerOz.SharedInstance.ChangeUISet(go, goOpaque, go2, (int)chosenType);
		}
		else
		{
			notify.Error("Downloaded HIRES UI Cannot find  = " + loadUIName);
		}

		needUIBundle = false;// no more prompting
		CheckCompleted(true);
//		if( UIManagerOz.SharedInstance.worldOfOzVC.gameObject.active )
//			UIManagerOz.SharedInstance.worldOfOzVC.Refresh();
	}		
	
	private void OnAssetBundleLoadedFailure(string assetBundleName, int version, string errMsg)
	{
		if( loadingBundleName != assetBundleName )
		{
			notify.Debug("UI Download ignored fail callback for " + assetBundleName+ ". Expecting bundle name = " + loadingBundleName);
			return ;
		}

		notify.Error("Downloaded HIRES UI failed assetBundleName = " + assetBundleName);

		FinishingUp();
		
		SharedInstance.mTimer = 0.0f; // stop update
		CheckCompleted(false);

	}
	
}
