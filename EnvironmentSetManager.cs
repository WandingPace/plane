using UnityEngine;
using System;
using System.Collections.Generic;
using System.Collections;

/// <summary>
/// Responsible for keeping track of which environmen sets are available locally
/// further down the line would be responsible for getting data from the asset bundles
/// </summary>
public class EnvironmentSetManager : MonoBehaviour
{
    private List<int> EnvDict = new List<int>() {1, 2, 3};
	public const int WhimsyWoodsId = 1;
	public const int DarkForestId = 2;
	public const int YellowBrickRoadId = 3;
	public const int EmeraldCityId = 4;
	
	protected static Notify notify;
	public static EnvironmentSetManager SharedInstance = null;
	public EnvironmentSetData CurrentEnvironmentSet = null;
	
	public Dictionary <int, int> LatestVersionNumDict;
	
	/// <summary>
	/// The locally available environment sets
	/// </summary>
	public Dictionary<int, EnvironmentSetData> LocalDict;
	
	/// <summary>
	/// The locally available environment sets , where code is the key
	/// </summary>
	public Dictionary<string, EnvironmentSetData> LocalCode2Dict;
	
	/// <summary>
	/// All environment set data, where id is key.  Note that they may not be locally available.
	/// </summary>
	public Dictionary<int, EnvironmentSetData> AllDict;
	
	/// <summary>
	/// All environment set data, where code is key.  Note that they may not be locally available.
	/// </summary>
	public Dictionary<string, EnvironmentSetData> AllCode2Dict;
	
	/// <summary>
	/// All environment set bootstrap data , where env set id is key 
	/// </summary>
	private Dictionary<int, EnvironmentSetBootstrapData> bootstrapDict;
	
	/// <summary>
	/// Initialize ourself
	/// </summary>
	void Awake()
	{
		notify = new Notify(this.GetType().Name);
		SharedInstance = this;
	}
		
	/// <summary>
	/// Recalculates our dictionary data
	/// </summary>
	public void RecalculateData()
	{
		loadAllData(); 
		calcWhichAreLocal(); 

		notify.Debug( "[EnvironmentSetManager] Recalculate data.  Access GameProfile.SharedInstance.Player.savedEnvSet" );
		// we are making an assumption, that if this is not null, we should not override it
		// presumably he could download ybr envset while inside the df envset
		if (CurrentEnvironmentSet == null)
        {
            int savedEnvSet = GameProfile.SharedInstance.Player.savedEnvSet;
            int startingEnvSet = Settings.GetInt("starting-envset", savedEnvSet);

            SetCurrentEnviroment(startingEnvSet);
        }
	}

    public void SetCurrentEnviroment(int EnvId)
    {
        CurrentEnvironmentSet = LocalDict[EnvId];
    }

    public int GetRandomEnviroment(bool isFirst)
    {
        List<int> _EnvDict = new List<int>();
        if (isFirst)
            _EnvDict = EnvDict;
        else
        {
            for (int i = 0; i < EnvDict.Count; i++)
            {
                int _EnvId = EnvDict[i];
                if (CurrentEnvironmentSet.SetId.Equals(_EnvId))
                    continue;

                _EnvDict.Add(_EnvId);
            }
        }
        int index = UnityEngine.Random.Range(0, _EnvDict.Count);
        return _EnvDict[index];
    }

    /// <summary>
	/// Start this instance. Connect with other Game Objects we need
	/// </summary>
	void Start()
	{
		EnvironmentSetSwitcher.SharedInstance.RegisterForOnEnvironmentStateChange(EnvironmentStateChanged);
		ResourceManager.SharedInstance.RegisterForOnAssetBundleLoadSuccess(OnAssetBundleLoadedSuccess);
	}
	
	/// <summary>
	/// primarily used to recalculate which is local, now that we have a new asset bundle downloaded
	/// </summary>
	public  void OnAssetBundleLoadedSuccess(string assetBundleName, int version, bool downloadOnly)
	{	
		RecalculateData();
	}
	
	/// <summary>
	/// Getting informed the environment set has changed, update CurrentEnvironmentSet 
	/// </summary>
	/// <param name='newEnvSetId'>
	/// New env set identifier.
	/// </param>
	void EnvironmentStateChanged(EnvironmentSetSwitcher.SwitchState newState, int newEnvSetId)
	{
		if (newState == EnvironmentSetSwitcher.SwitchState.finished)
		{
			//Update objectives info...
			ObjectivesDataUpdater.AddToGenericStat(ObjectiveType.ExitLocation,1);
			int oldID = CurrentEnvironmentSet.SetId;
			
			if(ObjectivesDataUpdater.GetGenericStat(ObjectiveType.ReachLocation,1)<=0)
				ObjectivesDataUpdater.SetGenericStat(ObjectiveType.DistanceWithoutTransition,GameController.SharedInstance.DistanceTraveled);

		    SetCurrentEnviroment(newEnvSetId);

			//Note: These two are the same thing; don't want to mess with the enum
			ObjectivesDataUpdater.AddToGenericStat(ObjectiveType.EnvironmentSwitch,1);
			ObjectivesDataUpdater.AddToGenericStat(ObjectiveType.ReachLocation,1);
			//Track environment change based on where we came from
			switch(oldID) {
			case 1: ObjectivesDataUpdater.AddToGenericStat(ObjectiveType.TravelFromEnv1,1); break;
			case 2: ObjectivesDataUpdater.AddToGenericStat(ObjectiveType.TravelFromEnv2,1); break;
			case 3: ObjectivesDataUpdater.AddToGenericStat(ObjectiveType.TravelFromEnv3,1); break;
			case 4: ObjectivesDataUpdater.AddToGenericStat(ObjectiveType.TravelFromEnv4,1); break;
			case 5: ObjectivesDataUpdater.AddToGenericStat(ObjectiveType.TravelFromEnv5,1); break;
			}
			
			ObjectivesDataUpdater.LogEnvironmentVisited(newEnvSetId);
		}
	}
	
	/// <summary>
	/// Loads the all available environment set data.
	/// </summary>
	void loadAllData()
	{
		LatestVersionNumDict = new Dictionary<int, int>();
		LocalDict = new Dictionary<int, EnvironmentSetData>();
		LocalCode2Dict = new Dictionary<string, EnvironmentSetData>();
		AllDict = new Dictionary<int, EnvironmentSetData>();
		AllCode2Dict = new Dictionary<string, EnvironmentSetData>();
		bootstrapDict = new Dictionary<int, EnvironmentSetBootstrapData>();
		
		EnvironmentSetBootstrap.LoadFile();
		for ( int i=0; i< EnvironmentSetBootstrap.BootstrapList.Count; i++)
		{
			EnvironmentSetBootstrapData boot = EnvironmentSetBootstrap.BootstrapList[i];
			string setCode = boot.SetCode;
			EnvironmentSetData envsetData;
			bool loaded = EnvironmentSetData.LoadFile(setCode, out envsetData);
			if (loaded)
			{
				if (AllDict.ContainsKey(envsetData.SetId))
				{
					notify.Error("environment set " + setCode + " has the same id as " + AllDict[envsetData.SetId].SetCode);	
				}
				else
				{			
					AllDict[envsetData.SetId] = envsetData;	
					AllCode2Dict[envsetData.SetCode] = envsetData;
					bootstrapDict[envsetData.SetId] = boot;
				}
			}
			else
			{
				notify.Error("Could not load environment set data for " + boot.SetCode);	
			}
		}
	}
	
	/// <summary>
	/// Calculates the which ones are local
	/// </summary>
	private void calcWhichAreLocal()
	{
		notify.Debug("EnvironmentSetManager.calcWhichAreLocal");
		ResourceManager.SharedInstance.VerifyDownloadedFilesArePresent();
	    EnvironmentSetData envsetData = null;
		for ( int i=0; i< EnvironmentSetBootstrap.BootstrapList.Count; i++)
		{
			EnvironmentSetBootstrapData boot = EnvironmentSetBootstrap.BootstrapList[i];
			// we check if it's embedded or been downloaded to decide that it is local
			bool addToLocal = boot.Embedded;
			if ( ! addToLocal)
			{
				addToLocal = ResourceManager.SharedInstance.IsAssetBundleDownloaded( boot.AssetBundleName);
				if (addToLocal)
					notify.Debug("{0} has been downloaded and is locally available", boot.AssetBundleName);
				else
					notify.Debug("{0} has NOT been downloaded and is NOT locally available", boot.AssetBundleName);
			}
			if (Settings.GetBool("embed-all-envsets", false))
			{
				addToLocal = true;	
			}
			if ( addToLocal && AllCode2Dict.TryGetValue(boot.SetCode,out envsetData))
            {
                LocalDict.Add(envsetData.SetId, envsetData);
                LocalCode2Dict.Add(envsetData.SetCode, envsetData);
				notify.Debug("********** envset {0} is local", boot.SetCode);
			}
			else
			{
				notify.Debug("envset {0} is NOT local", boot.SetCode);
			}
		}
	}
	
	public void SetLatestVersion(int setId, int vid)
	{
		LatestVersionNumDict[setId] = vid;
	}
	
	public int GetAssetBundleLatestVersionNumber(int setId)
	{
		if( LatestVersionNumDict.ContainsKey(setId))
			return  LatestVersionNumDict[setId];
		notify.Warning("******* Get latest EnvBundle ver failed, id = {0}", setId);
		return -1;
	}

	public bool IsLocallyAvailableAndLatestVersion(int setId)
	{
		string bname = GetAssetBundleName(setId);// return null when not in list
		bool result = IsEmbedded(setId);
		if( !result && bname != null )
		{
			result = ResourceManager.SharedInstance.IsAssetBundleDownloadedLastestVersion( bname ) ; 
		}
//		Debug.LogError("---------Checking bundle " + bname + " result = " + result + " id = " + setId);
		return result;
	}
	
	/// <summary>
	/// Returns true if specified setId is locally available
	/// </summary>
	/// <returns>
	/// <c>true</c> if this environment set is locally available , otherwise, <c>false</c>.
	/// </returns>
	/// <param name='setId'>
	/// the EnviromentSet id
	/// </param>
	public bool IsLocallyAvailable(int setId)
	{
		bool result = LocalDict.ContainsKey(setId);
		return result;
	}
	
	/// <summary>
	/// Returns true if specified set is locally available
	/// </summary>
	/// <returns>
	/// <c>true</c> if this environment set is locally available , otherwise, <c>false</c>.
	/// </returns>
	/// <param name='setCode'>
	/// the string code for the environment set, e.g. ww, ybr, df
	/// </param>
	public bool IsLocallyAvailable(string setCode)
	{
		bool result = LocalCode2Dict.ContainsKey(setCode);
		return result;
	}
	
	/// <summary>
	/// Returns true if all the environment sets are locally available
	/// </summary>
	/// <returns>
	/// <c>true</c> if all the environment sets locally available; otherwise, <c>false</c>.
	/// </returns>
	public bool IsEverythingLocallyAvailable()
	{
		bool result = true;
		foreach ( int setId in AllDict.Keys)
		{
			if ( ! LocalDict.ContainsKey(setId))
			{
				result = false;
			}
		}
		return result;
	}	
	
	/// <summary>
	/// Returns the number of environment sets that are locally available
	/// </summary>
	/// <returns>
	/// Tthe number of environment sets that are locally available
	/// </returns>
	public int LocallyAvailableCount()
	{
		return LocalDict.Count;
	}
	
	/// <summary>
	/// Gets the asset bundle base name of the given environment set
	/// </summary>
	/// <returns>
	/// The asset bundle base name,  will return null if it's an embedded environment set
	/// </returns>
	/// <param name='envSetId'>
	/// Env set identifier.
	/// </param>
	public string GetAssetBundleName(int envSetId)
	{
		string result = null;
		EnvironmentSetBootstrapData bootData;
		if ( bootstrapDict.TryGetValue(envSetId, out bootData))
		{
			result = bootData.AssetBundleName;
		}
		return result;
	}
	
	/// <summary>
	/// Returns true if the given environment set is embedded in the app
	/// </summary>
	/// <returns>
	/// <c>true</c> if the environment set is embedded in the app
	/// </returns>
	/// <param name='envSetId'>
	/// the env set identifier.
	/// </param>
	public bool IsEmbedded(int envSetId)
	{
		bool result = false;
		EnvironmentSetBootstrapData bootData;
		if ( bootstrapDict.TryGetValue(envSetId, out bootData))
		{
			result = bootData.Embedded;
		}
		if (Settings.GetBool("embed-all-envsets", false))
		{
			// if we are forcing everything to be embedded, then make this return true
			result = true;
		}
		return result;
	}

	public delegate void OnEnvironmentSetReady();

    /// <summary>
    /// Ensures the current environment's asset bundle has been loaded
    /// </summary> 
    public void PrepareCurrentEnvironmentSet(OnEnvironmentSetReady delg)
    {
        StartCoroutine(loadCurrentBundle(delg));
    }

    /// <summary>
	/// Loads the asset bundle for the current environment setbundle.
	/// </summary>
	/// <returns>
	/// The current bundle.
	/// </returns>
	/// <param name='delg'>
	/// Delg.
	/// </param>
	IEnumerator loadCurrentBundle(OnEnvironmentSetReady delg)
	{
        yield return StartCoroutine(EnvironmentSetSwitcher.SharedInstance.LoadEnvironmentResources(CurrentEnvironmentSet.SetId));
		//yield return StartCoroutine ( EnvironmentSetSwitcher.SharedInstance.loadNewTrackMaterialCoroutine(CurrentEnvironmentSet.SetId));
		delg();
	}

    /// <summary>
    /// returns true if the current environment set is ready.
    /// </summary>
    public bool IsCurrentEnvironmentSetReady()
    {
        bool result = ResourceManager.SharedInstance.IsAssetBundleAvailable(GetAssetBundleName(CurrentEnvironmentSet.SetId));
        return result;
    }
}
