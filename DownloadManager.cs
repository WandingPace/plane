using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DownloadManager : MonoBehaviour
{
	private const string kBundleDescription = "Lands of Oz";
	private static List<int> loaderQueue = new List<int>();	// stores ID's of locations to be downloaded
	private static AssetBundleLoader loader;				// reference to loader for download currently in progress
	private static int downloadsTriggered;					// number of downloads requested at once and thrown into the queue
	private static float progress;							// actual total progress of all downloads in progress
	
	public float UdateInterval = 0.3f;
	private float mTimer = 0.0f;
	
	protected static Notify notify;
	
	public static DownloadManager SharedInstance = null;
	
	private static string loadingBundleName = "";			// the name of the bundle to be downloaded, used for checking callback from AssetLoader
	private static GameObject msgObject = null;					// callback/message object when this operation is done
	public static int latestVersionNumber = -1;


	void Awake()
	{
		SharedInstance = this;
		notify = new Notify(this.GetType().Name);
	}
	
	public static void ResetStatus()
	{
		loaderQueue.Clear();		
		loader = null;
		downloadsTriggered = 0;
		progress = 1.0f;
		loadingBundleName = "";
		
		if( SharedInstance )
			SharedInstance.mTimer = 0.0f;
	}
	
	void Start()
	{
		ResetStatus();
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
	
	public static bool IsDownloadInProgress()				// true if download is currently in progress
	{
		bool dlStatus = (loader != null) ? true : false;
		return dlStatus;
	}
	
	public static bool HaveAllLocationsBeenDownloaded()		// returns false if there exists an un-downloaded location
	{
		notify.Debug("HaveAllLocationsBeenDownloaded in DownloadManager entered");
		
		bool response = true;
		
		foreach ( EnvironmentSetBootstrapData bootData in  EnvironmentSetBootstrap.BootstrapList)
		{
			notify.Debug("foreach loop in HaveAllLocationsBeenDownloaded processing " + bootData.AssetBundleName);
			
			if ( ! bootData.Embedded)
			{
				notify.Debug("! bootData.Embedded = true for " + bootData.AssetBundleName);
				EnvironmentSetData envSetData =  EnvironmentSetManager.SharedInstance.AllCode2Dict[bootData.SetCode];
				if (!EnvironmentSetManager.SharedInstance.IsLocallyAvailableAndLatestVersion(envSetData.SetId))
				{
					notify.Debug("!EnvironmentSetManager.SharedInstance.IsLocallyAvailableAndLatestVersion(" + envSetData.SetId.ToString() + ") = true");
					response = false;
					break;
				}
				else
				{
					notify.Debug("!EnvironmentSetManager.SharedInstance.IsLocallyAvailableAndLatestVersion(" + envSetData.SetId.ToString() + ") = false");
				}
			}	
			else
			{
				notify.Debug("! bootData.Embedded = false for " + bootData.AssetBundleName);
			}			
		}
		
		notify.Debug("response = " + response + " in HaveAllLocationsBeenDownloaded");
		return response;
	}

	public void ResetUpdateTimer()
	{
		mTimer = UdateInterval;
	}
	
	
	public void StopTimer()
	{
		mTimer = 0.0f;
	}
	
	public static void CancelDownload()
	{
		notify.Debug("DownloadManager.CancelDownload called");
		
		//ResourceManager.SharedInstance.RemoveAssetBundleLoader(loadingBundleName);
		//loadingBundleName = "";
		//ResetStatus();
		
		if (IsDownloadInProgress())
		{
			notify.Debug("CancelDownload in DownloadManager");
			ResourceManager.SharedInstance.CancelDownload(loadingBundleName);
			loader = null;
		}		
		
		loadingBundleName = "";
		ResetStatus();		
	}
	
	private static void DepopulateMeshPool()
	{
		SpawnPool spawnPool;
		if (!PoolManager.Pools.TryGetValue("TrackMesh", out spawnPool))
		{
			notify.Debug("A 'TrackMesh' pool does NOT exist!");
			return;
		}
		List<TrackPiece.PieceType> pieceTypeKeys = TrackBuilder.SharedInstance.GetPieceTypesKeys();
		foreach( TrackPiece.PieceType pieceType in pieceTypeKeys)
		{
			TrackPieceTypeDefinition def = TrackBuilder.SharedInstance.GetTypesFromTrackType(pieceType);
            if (!def.IsTransitionTunnel && !def.IsBalloon && !def.IsEvent)
			{
				foreach ( string prefabName in def.Variations.Keys)
				{
					string withSuffix = prefabName + "_prefab";
					if (spawnPool.prefabs.ContainsKey(withSuffix))
					{
						Transform onePrefab = spawnPool.prefabs[withSuffix];
						PrefabPool prefabPool = spawnPool.GetPrefabPool(onePrefab);
						if (prefabPool != null)
						{
							prefabPool.ReleaseUnspawned();
						}
					}
				}
			}
		}
		System.GC.Collect();
	}
	
	public static void DownloadAllLocations(GameObject msgobject = null)				// trigger the download of all un-downloaded locations
	{
		if(loadingBundleName != "")
		{
			notify.Error("StartDownload when already loading bundle name = " + loadingBundleName);
			//UIManagerOz.SharedInstance.ShowAMPSDownloadError( "Msg_DownloadFailLong", "DownloadManager::DownloadAllLocations: already downloading '" + loadingBundleName + "'" );
			Services.Get<AmpBundleManager>().ShowAMPSDownloadError( "Msg_DownloadFailLong", "DownloadManager::DownloadAllLocations: already downloading '" + loadingBundleName + "'" );
			return;// skip this call!?
		}
		
		msgObject = msgobject; // ediaglog done message will be send to this object
		
		foreach ( EnvironmentSetBootstrapData bootData in  EnvironmentSetBootstrap.BootstrapList)
		{
			if (! bootData.Embedded)
			{
				EnvironmentSetData envSetData =  EnvironmentSetManager.SharedInstance.AllCode2Dict[bootData.SetCode];
				if (!EnvironmentSetManager.SharedInstance.IsLocallyAvailableAndLatestVersion(envSetData.SetId))
					loaderQueue.Add(envSetData.SetId);				
			}	
		}

		downloadsTriggered = loaderQueue.Count;				// store total number of downloads to do, for progress bar calculation
		
		if( downloadsTriggered == 0 )
		{
			SharedInstance.CheckCompleted(true);
			return;
		}
		
		//depopulate the track mesh pool to save memory...
		DepopulateMeshPool();
		
		ResourceManager.SharedInstance.RegisterForOnAssetBundleLoadSuccess(SharedInstance.OnAssetBundleLoadedSuccess);
		ResourceManager.SharedInstance.RegisterForOnAssetBundleLoadFailure(SharedInstance.OnAssetBundleLoadedFailure);	
		
		DownloadNextLocationInQueue();
		SendOutUpdateMessages();							// ask map & play button to refresh itself
		
		SharedInstance.ResetUpdateTimer();

//		SharedInstance.Invoke( "TestUpdate", 0.2f);
	}

	private static void DownloadNextLocationInQueue()
	{
		if (loaderQueue.Count > 0)
		{
			StartDownload(loaderQueue[loaderQueue.Count-1], true);		// download last one in list
			// if there is an error loaderQueue can get cleared out, make sure we don't access -1
			if (loaderQueue.Count >0)
			{
				loaderQueue.RemoveAt(loaderQueue.Count-1);					// clear it out from the queue
			}
		}
	}
	
	private static void StartDownload(int id, bool downloadOnly)
	{
		loadingBundleName = EnvironmentSetManager.SharedInstance.GetAssetBundleName(id);
		bool isEmbedded = EnvironmentSetManager.SharedInstance.IsEmbedded(id);
		loader = ResourceManager.SharedInstance.LoadAssetBundle(loadingBundleName, downloadOnly, latestVersionNumber, isEmbedded, false);	
	}		
	
	public static float GetTotalDownloadProgress()		// get progress total of all items being downloaded, in range 0.0f - 1.0f
	{
		if (loader != null && downloadsTriggered != 0)
			progress = ((float)(downloadsTriggered - (loaderQueue.Count+1)) + loader.GetProgress()) / (float)downloadsTriggered;
			//progress = loader.GetProgress();
		else
			progress = 1.0f;
		
		return progress;
	}
	
//	public static float ppp = 0.0f;
	
	private static void SendOutUpdateMessages()
	{
		msgObject.SendMessage ("OnProgressUpdate", GetTotalDownloadProgress(), SendMessageOptions.DontRequireReceiver);
	}	
	
	public void FinishingUp()
	{
		ResourceManager.SharedInstance.UnRegisterForOnAssetBundleLoadSuccess(OnAssetBundleLoadedSuccess);	// stop listening for this event
		ResourceManager.SharedInstance.UnRegisterForOnAssetBundleLoadFailure(OnAssetBundleLoadedFailure);	// stop listening for this event
		ResetStatus();
	}	

	private void OnAssetBundleLoadedSuccess(string assetBundleName, int version, bool downloadOnly)
	{	
		notify.Debug("DownloadManager.OnAssetBundleLoadedSuccess {0} {1} {2}", assetBundleName, version, downloadOnly);
		
		// veridy if the asset bundle is one of ours
		if( assetBundleName != loadingBundleName )
		{
			notify.Debug("Environment download ignored success callback for " + assetBundleName+ ". Expecting bundle name = " + loadingBundleName);
			return ;
		}
	
		loadingBundleName = ""; // clear
		
		if (loaderQueue.Count > 0 && loader != null)
			DownloadNextLocationInQueue();
		else								// last download completed
		{
			//repopulate track mesh pool...
			
			notify.Debug("Downloaded DFassetBundleName = " + assetBundleName);
			
			SendOutUpdateMessages();		// ask map & play button to refresh itself
			FinishingUp();

			
		 	
			if( !ProfileManager.GotGemReward )
			{
				GameProfile.SharedInstance.Player.specialCurrencyCount += 3;		// get 3 gems for downloading
				GameProfile.SharedInstance.Serialize();// save to disk
				UIManagerOz.SharedInstance.PaperVC.UpdateCurrency();
				ProfileManager.GotGemReward = true;
			}
			
			//UIManagerOz.SharedInstance.rewardDialog.ShowRewardDialog("Msg_LocsReward", "", 5, "currency_gem", gameObject);
//			OnRewardDialogClosed();		// reward dialog was removed, so just go to the next step
			CheckCompleted(true);
			
			System.GC.Collect();
		}
		
//		if( UIManagerOz.SharedInstance.worldOfOzVC.gameObject.active )
//			UIManagerOz.SharedInstance.worldOfOzVC.Refresh();
		//notify.Warning("Downloaded assetBundleName = " + assetBundleName);
	}		
	
	private void OnAssetBundleLoadedFailure(string assetBundleName, int version, string errMsg)
	{
		notify.Debug("DownloadManager.OnAssetBundleLoadedFailure {0} {1} {2}", assetBundleName, version, errMsg);
		// veridy if the asset bundle is one of ours
		if( assetBundleName != loadingBundleName )
		{
			notify.Debug("Environment download ignored fail callback for " + assetBundleName+ ". Expecting bundle name = " + loadingBundleName);
			//UIManagerOz.SharedInstance.ShowAMPSDownloadError( "Msg_DownloadFailLong", "DownloadManager::OnAssetBundleLoadedFailure: expecting bundle name '" + loadingBundleName + "' but got '" + assetBundleName + "'" );
			Services.Get<AmpBundleManager>().ShowAMPSDownloadError( "Msg_DownloadFailLong", "DownloadManager::OnAssetBundleLoadedFailure: expecting bundle name '" + loadingBundleName + "' but got '" + assetBundleName + "'" );
			return;
		}

		//repopulate track mesh pool...
		notify.Error("Downloaded level resource failed assetBundleName = " + assetBundleName); //no internet connection N.N.
		//UIManagerOz.SharedInstance.ShowAMPSDownloadError( "Msg_DownloadFailLong", "DownloadManager::OnAssetBundleLoadedFailure: assetBundleName='" + assetBundleName + "'" );
		Services.Get<AmpBundleManager>().ShowAMPSDownloadError( "Msg_DownloadFailLong", "DownloadManager::OnAssetBundleLoadedFailure: assetBundleName='" + assetBundleName + "'" );
		
		SendOutUpdateMessages();	// ask play button and map to refresh themselves
		FinishingUp();
		
//		UIManagerOz.SharedInstance.PaperVC.PlayButtonRoot.GetComponent<PlayButton>().ResetPlayButton();	// reset play button back to normal
//		UIManagerOz.SharedInstance.okayDialog.ShowOkayDialog("Msg_DownloadFail", "", "Btn_Ok", gameObject);
		CheckCompleted(false);
		System.GC.Collect();
	}
	
	public static bool IsDownloading()
	{
		return loadingBundleName != "";
	}
			
	void CheckCompleted(bool success)
	{
		if (msgObject)
			msgObject.SendMessage("OnEnvDownloadCheckDone", success, SendMessageOptions.DontRequireReceiver);
	}
}




/*	
	void TestUpdate()
	{
		ppp += 0.1f;
		SendOutUpdateMessages();
		Invoke( "TestUpdate", 0.2f);
	}
*/
	

		
		//TODO Alex please read in the data from EnvironmentSetBootstrap to see which environment sets are even defined in this build
		
		//if (!EnvironmentSetManager.SharedInstance.IsLocallyAvailable(EnvironmentSetManager.YellowBrickRoadId))		// uncomment this when Winkie Country is added
		//	loaderQueue.Add(EnvironmentSetManager.YellowBrickRoadId);
		


/*	
	void OnOkayDialogClosed()
	{
		if( msgObject ) // UIDownloadDialogOZ
		{
			msgObject.SendMessage("OnEnvDownloadCheckDone");
		}
	}

		void OnRewardDialogClosed()
	{
		if( msgObject )// UIDownloadDialogOZ
		{
			msgObject.SendMessage("OnEnvDownloadCheckDone");
		}
	}
		*/


//progressAdditive = 0.0f;

	//private static float progressAdditive;					// temp value, for totaling progress of all downloads together prior to dividing by # downloads
	

		//if (loader != null)
		//	progress = loader.GetProgress();
		

		//progressAdditive = 0.0f;
		
		//float completedDLprogress = (float)(loaderQueue.Count + 1) / (float)downloadsTriggered;
		//float thisDLprogress = loader.GetProgress() / (float)downloadsTriggered;
		
//
//		foreach (KeyValuePair<int, AssetBundleLoader> kvp in loaders)				//foreach (AssetBundleLoader loader in loaders)
//		{
//			float prog = kvp.Value.GetProgress();							// loader.GetProgress();
//			progressAdditive += prog;
//		}
//			
//		if (downloadsTriggered > 0)	// prevent divide by 0
//			progress = progressAdditive / (float)downloadsTriggered;	//loader.GetProgress();
//		else 
//			progress = 1.0f;		// if no downloads in progress, they must all be done
//		
		


		//loaders.Clear();
		//downloadsTriggered = 0;

		//return downloadsTriggered;	//loaders.Count;

	//private static Dictionary<int,AssetBundleLoader> loaders = new Dictionary<int,AssetBundleLoader>();
		//loaders.Add(id, loader);


//		if (assetBundleName == "darkforest")
//			loaders.Remove(EnvironmentSetManager.DarkForestId);
//		else if (assetBundleName == "yellowbrickroad")
//			loaders.Remove(EnvironmentSetManager.YellowBrickRoadId);



//public interface IDownloadManager
//{
//    int HowManyDownloadsInProgress();			
//    bool HaveAllLocationsBeenDownloaded();		
//	void DownloadAllLocations();				
//	float GetTotalDownloadProgress();			
//}


		//downloadsInProgress = 0;
		

		//downloadsInProgress--;

		//downloadsInProgress++;
		
	//public static int downloadsInProgress = 0;

		//UIManagerOz.SharedInstance.mapVC.downloadingLocationID = id;	// set value in UIMapViewControllerOz

		//playButton.SetAssetBundleLoaderReference(id, loader);
