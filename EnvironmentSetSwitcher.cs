	using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// Main class responsible for switching the environment set
// interacts with TrackBuilder
public class EnvironmentSetSwitcher : MonoBehaviour 
{
	protected static Notify notify;
	public static EnvironmentSetSwitcher SharedInstance = null;
	
	private int destinationEnvironmentSetId; // which env set are we going to
	private int originEnvironmentSetId; // which env set are we coming from
	
	private float startDeleteTime;
	private float startWaitingToExitTunnelTime;

	private Dictionary<SwitchState, float> timeSpentPerState = new Dictionary<SwitchState, float>();
	
	// event for environment state change
	public delegate void OnEnvironmentStateChangeHandler(SwitchState newState, int destinationEnvironmentId);
	private static event OnEnvironmentStateChangeHandler onEnvironmentStateChangeEvent = null;
	public void RegisterForOnEnvironmentStateChange(OnEnvironmentStateChangeHandler delg) { 
		onEnvironmentStateChangeEvent += delg; }
	public void UnRegisterForOnEnvironmentSet(OnEnvironmentStateChangeHandler delg) { 
		onEnvironmentStateChangeEvent -= delg; }		
	
	private bool wantNewEnvSet;
	public AudioClip nextMusicClip;
	public bool WantNewEnvironmentSet
	{
		get
		{
			return wantNewEnvSet;	
		}
	}
	
	public int DestinationEnvironmentSet
	{
		get 
		{
			return destinationEnvironmentSetId;	
		}
		
		set
		{
			if (TransitionState != SwitchState.inactive)
			{
				notify.Error ("Destination Enviroment Set should not be called in the middle of a transition");
				return;
			}
			destinationEnvironmentSetId = value;
		}	
	}
	
	public enum SwitchState
	{
		inactive,
		waitingToBeAbleToDeletePools,
		deletingPools,
		loadingAssetBundle,
		addingNewPools,
		waitingToExitTunnel,
		finished,
	}
	
	/// <summary>
    /// 提示切换环境的路口到隧道的距离
	/// </summary>
	public float DistanceToTunnel { 
		get {
			if(GamePlayer.SharedInstance.HasFastTravel)
				return 0f;
            return EnvironmentSetManager.SharedInstance.CurrentEnvironmentSet.DistanceToNextEnv;
		}
	}
	
	private SwitchState _transitionState = SwitchState.inactive;
	public SwitchState TransitionState
	{
		get 
		{
			return _transitionState;	
		}
		
		private set
		{
			_transitionState = value;
			onEnvironmentStateChangeEvent(_transitionState, destinationEnvironmentSetId);
		}
	}
	
	/// <summary>
	/// Reset this instance when we want to run again.
	/// </summary> 
	public void Reset()
	{
		wantNewEnvSet = false;	
		TrackPiece.DistanceSubtracedPerPieceAdded = 2.0f;
		firstTransition = true;
	}
	
	void Awake()
	{
		SharedInstance = this;
		notify = new Notify(this.GetType().Name);
	}
	
	void Start()
	{
		GamePlayer.SharedInstance.RegisterForOnTrackPieceChange(OnTrackPieceChanged);	
		
		TrackPiece.onDoTurn += PlayerTurned;
	//	GameController.SharedInstance.RegisterForOnPlayerTurn(PlayerTurned);
	}
	
	public static bool IsInactive()
	{
		bool result = EnvironmentSetSwitcher.SharedInstance == null ||
			EnvironmentSetSwitcher.SharedInstance.TransitionState == EnvironmentSetSwitcher.SwitchState.inactive;
		return result;
	}

    private void OnTrackPieceChanged(TrackPiece oldTrackPiece, TrackPiece newTrackPiece)
    {
        if (newTrackPiece == null)
            return;
        notify.Debug("EnvironmentSetSwitcher.ChangedOnTrackPiece oldTrack" + oldTrackPiece + " newTrack" + newTrackPiece);
        if (newTrackPiece.TrackType == TrackPiece.PieceType.kTPTransitionTunnelEntrance)
        {
            startDeleteTime = Time.time;

            if (TransitionState == SwitchState.inactive)
                TransitionState = SwitchState.waitingToBeAbleToDeletePools;
        }
        if (TransitionState == SwitchState.waitingToBeAbleToDeletePools)
        {
            if (IsEverythingPreviousTunnelTransition(newTrackPiece))
                StartCoroutine(doEnvironmentSetTransition());
        }

        if (oldTrackPiece != null && oldTrackPiece.TrackType == TrackPiece.PieceType.kTPTransitionTunnelExit)
        {
            finishTransition();
            GameController.TextureReport("finishTransition");
            GameController.AudioReport("finishTransition");
            GameController.MeshReport("finishTransition");
        }
    }

    //Only allow difficulty reduction once per run
	private bool firstTransition = true;
	/// <summary>
	/// Finishes the transition. Makes sure the tunnel exit is fixed up
	/// </summary>
	void finishTransition()
	{
	    GameController.SharedInstance.lastEnvId = destinationEnvironmentSetId;

		TrackPieceTypeDefinition tunnelExit = TrackBuilder.SharedInstance.GetTypesFromTrackType( TrackPiece.PieceType.kTPTransitionTunnelExit);	
		tunnelExit.EnvironmentSet = -1;
		
		//Reduce the "DifficultyDistance" in GameController
		if(firstTransition)
		{
			GameController.SharedInstance.DifficultyDistanceTraveled -= GameProfile.SharedInstance.TransitionEndDifficultyReduction;
			firstTransition = false;
		}
		
		//dark forest temporarily needs a dynamic light
		//if (destinationEnvironmentSetId == 2)
		//{
		//	DebugConsole.DebugDirectionalLight(true);
		//}
		//else
		//{
		//	DebugConsole.DebugDirectionalLight(false);
		//}
		
		TrackPieceTypeDefinition tunnelEntrance = TrackBuilder.SharedInstance.GetTypesFromTrackType( TrackPiece.PieceType.kTPTransitionTunnelEntrance);	
		tunnelEntrance.EnvironmentSet = destinationEnvironmentSetId;
		TransitionState = SwitchState.inactive;
		
		timeSpentPerState[SwitchState.waitingToExitTunnel] = Time.time - startWaitingToExitTunnelTime;
		TrackPiece.DistanceSubtracedPerPieceAdded = 2.0f;
		
		TransitionState = SwitchState.finished;		// sends out 'finished' event to all listeners
		TransitionState = SwitchState.inactive;		
	}
	
	public string GetTunnelTimes()
	{
		string result = "";
		float oneTime;
		float totalTime = 0;
		if (timeSpentPerState.TryGetValue(SwitchState.deletingPools, out oneTime))
		{
			result += " deleting pools=" + oneTime;
			totalTime += oneTime;
		}
		if (timeSpentPerState.TryGetValue(SwitchState.addingNewPools, out oneTime))
		{
			result += " adding new pools=" + oneTime;
			totalTime += oneTime;
		}
		if (timeSpentPerState.TryGetValue(SwitchState.waitingToExitTunnel, out oneTime))
		{
			result += " waitingToExitTunnel=" + oneTime;
			totalTime += oneTime;
		}
		result += " total= " + totalTime;
		return result;
	}

    /// <summary>
    /// 检查当前所有加载的赛段都是关卡过渡段
    /// </summary>
    /// <param name="newTrackPiece"></param>
    /// <returns></returns>
    private bool IsEverythingPreviousTunnelTransition(TrackPiece newTrackPiece)
    {
        bool result = true;
        TrackPiece curTrackPiece = newTrackPiece;
        while (curTrackPiece != null)
        {
            TrackPieceTypeDefinition def = TrackBuilder.SharedInstance.GetTypesFromTrackType(curTrackPiece.TrackType);
            if (!def.IsTransitionTunnel)
            {
                result = false;
                break;
            }
            curTrackPiece = curTrackPiece.PreviousTrackPiece;
        }
        return result;
    }

    /// <summary>
    /// 清除旧环境的对象
    /// </summary>
    /// <returns></returns>
    private IEnumerator deleteOldObjects()
    {
        TransitionState = SwitchState.deletingPools;
        SpawnPool spawnPool;
        if (!PoolManager.Pools.TryGetValue("TrackMesh", out spawnPool))
        {
            notify.Debug("A 'TrackMesh' pool does NOT exist!");
            yield break;
        }

        notify.Debug("A 'TrackMesh' pool was found: " + spawnPool.group.name);

        // for memory reasons, we need to get rid of all the old pool objects
        List<TrackPiece.PieceType> pieceTypeKeys = TrackBuilder.SharedInstance.GetPieceTypesKeys();
        List<TrackPiece.PieceType> deletedKeys = new List<TrackPiece.PieceType>();
        int poolsDeleted = 0;

        foreach (TrackPiece.PieceType pieceType in pieceTypeKeys)
        {
            TrackPieceTypeDefinition def = TrackBuilder.SharedInstance.GetTypesFromTrackType(pieceType);
            if (!def.IsTransitionTunnel && !def.IsBalloon && !def.IsEvent)
            {
                foreach (string prefabName in def.Variations.Keys)
                {
                    string withSuffix = prefabName + "_prefab";
                    if (spawnPool.prefabs.ContainsKey(withSuffix))
                    {
                        Transform onePrefab = spawnPool.prefabs[withSuffix];
                        notify.Debug("onePrefab = " + onePrefab);
                        PrefabPool prefabPool = spawnPool.GetPrefabPool(onePrefab);
                        notify.Debug("prefabPool = " + prefabPool);
                        if (prefabPool != null)
                        {
                            prefabPool.SelfDestruct();
                            /*	
							prefabPool.cullAbove = 0;
							prefabPool.cullDelay = 1;
							prefabPool.cullDespawned = true;
							prefabPool.cullMaxPerPass = 1;
							prefabPool._logMessages = true;
							prefabPool.StartCulling();
							*/
                            notify.Debug("culling prefabPool = " + prefabPool);
                            //	string fullPath = TrackPiece.GetFullPathOfPrefab(prefabName);
                            spawnPool.DeletePrefabPool(prefabPool, withSuffix);
                            poolsDeleted += 1;

                        }

                        string fullPath = TrackPiece.GetFullPathOfPrefab(prefabName);

                        if (fullPath != null && DynamicElement.loaded_prefabs.ContainsKey(fullPath))
                            DynamicElement.loaded_prefabs.Remove(fullPath);
                    }
                    yield return null;
                    deletedKeys.Add(pieceType);
                }
            }
            yield return null;
        }
        notify.Debug("deleting piece types");

        TrackBuilder.SharedInstance.DeletePieceTypes(deletedKeys);

        SpawnEnemyFromPiece.DeSpawnPools();

        notify.Debug("deleting skybox");
        OzGameCamera.OzSharedInstance.Fade(3f);
        GameController.SharedInstance.DestroyOZSkyBox();

        float endDeleteTime = Time.time;
        timeSpentPerState[SwitchState.deletingPools] = endDeleteTime - startDeleteTime;
    }

    /// <summary>
	/// Add objects related to the new environment set 
	/// </summary>
	IEnumerator addNewEnvironmentSetObjectsCoroutine()
	{
		TransitionState = SwitchState.addingNewPools; 
		float startAddingTime = Time.time;
		
		float startTime = Time.realtimeSinceStartup;
		notify.Debug("warming pools start " + startTime);
		//TrackPiece.WarmResources();
		yield return StartCoroutine( TrackPiece.WarmPoolsCoroutine());
		
		notify.Debug ("yielded to WarmPoolsCoroutine for " + destinationEnvironmentSetId);
		
		float endTime = Time.realtimeSinceStartup;
		float totalTime = endTime - startTime;
		notify.Debug (string.Format ("warming resources took  {0:0.000000} seconds" , totalTime));
		TrackBuilder.SharedInstance.FixupForTransitionExit();
		
		timeSpentPerState[SwitchState.addingNewPools] = Time.time - startAddingTime ;
	}
	

	void addNewEnvironmentSetObjects()
	{
		TransitionState = SwitchState.addingNewPools; 
		float startAddingTime = Time.time;
		
		float startTime = Time.realtimeSinceStartup;
		notify.Debug("warming pools start " + startTime);
		//TrackPiece.WarmResources();
		TrackPiece.WarmPools();
		
		notify.Debug ("yielded to WarmPoolsCoroutine for " + destinationEnvironmentSetId);
		
		float endTime = Time.realtimeSinceStartup;
		float totalTime = endTime - startTime;
		notify.Debug (string.Format ("warming resources took  {0:0.000000} seconds" , totalTime));
		TrackBuilder.SharedInstance.FixupForTransitionExit();
		
		timeSpentPerState[SwitchState.addingNewPools] = Time.time - startAddingTime ;

        TransitionState = SwitchState.inactive;
	}	
	
    /// <summary>
    /// 执行关卡场景切换
    /// </summary>
    /// <returns></returns>
	IEnumerator doEnvironmentSetTransition()
	{
		TransitionState = SwitchState.deletingPools;
		wantNewEnvSet = false;

        yield return StartCoroutine(UnLoadEnvironmentResources(originEnvironmentSetId));

		yield return new WaitForEndOfFrame();

        TransitionState = SwitchState.loadingAssetBundle;
        yield return StartCoroutine(LoadEnvironmentResources(destinationEnvironmentSetId));

        //添加过渡出口段到末尾
		TrackPieceTypeDefinition tunnelExit = TrackBuilder.SharedInstance.GetTypesFromTrackType( TrackPiece.PieceType.kTPTransitionTunnelExit);
		if (tunnelExit == null)
		{
			notify.Error ("we are stuck forever in the tunnel as there is no tunnel exit");
			yield break;
		}
		
		startWaitingToExitTunnelTime = Time.time;
		tunnelExit.EnvironmentSet = destinationEnvironmentSetId;
		{
			TrackPiece piece = GamePlayer.SharedInstance.OnTrackPiece;
			while(piece.NextTrackPiece != null)
			{
				piece = piece.NextTrackPiece;
			}
			TrackBuilder.SharedInstance.QueuedPiecesToAdd.Add( piece.TrackType );
		}
		
		TrackBuilder.SharedInstance.QueuedPiecesToAdd.Add( TrackPiece.PieceType.kTPTransitionTunnelExit);
		
		TransitionState = SwitchState.waitingToExitTunnel;
		
		//等待离开过渡段后从雾化中淡出
		bool readyToExit = false;
		while(!readyToExit)
		{
			yield return null;
			TrackPiece piece = GamePlayer.SharedInstance.OnTrackPiece;
			if((piece.TrackType == TrackPiece.PieceType.kTPTransitionTunnelExit)
				||((piece.TrackType == piece.NextTrackPiece.TrackType)))
			{
				readyToExit = true;
				break;
			}
		}		
		// now once we get informed that the player is leaving the TunnelExit
		// bring us back to inactive
		//fade out fog
		GamePlayer.SharedInstance.StartFogFadeOut();
	}

    public IEnumerator LoadEnvironmentResources(int EnvId)
    {
        //加载新关卡到内存
        notify.Debug("{0}  loading asset bundle for the new environment set", Time.realtimeSinceStartup);
        string assetBundleName = EnvironmentSetManager.SharedInstance.GetAssetBundleName(EnvId);
        bool hasAssetBundle = ResourceManager.SharedInstance.IsAssetBundleAvailable(assetBundleName);
        bool isEmbedded = EnvironmentSetManager.SharedInstance.IsEmbedded(EnvId);

        if (!hasAssetBundle && !isEmbedded)
        {
            yield return StartCoroutine(ResourceManager.SharedInstance.LoadAssetBundleCoroutine(assetBundleName, false, -1,isEmbedded));
            notify.Debug("{0}  done loading the asset bundle", Time.realtimeSinceStartup);
        }

        ResourceManager.AllowLoad = true;

        //加载新关卡场景信息
        notify.Debug("adding new environment set pieces destinationEnvironmentSetId =" + EnvId);
        TrackBuilder.SharedInstance.PopulateEnvironmentSetPieces(EnvId);

        yield return null;//make sure we rendered the full whiteout frame

        //加载新关卡天空盒
        GameController.SharedInstance.NeededSkyBox = EnvId;
        notify.Debug("starting SpawnOZSkyBox for " + EnvId);
        StartCoroutine(GameController.SharedInstance.SpawnOZSkyBoxCoroutine(1f));

        //加载新关卡场景预制
        notify.Debug("starting WarmPrefabs for " + EnvId);
        SpawnEnemyFromPiece.SpawnPools(EnvId);
        TrackPiece.WarmPrefabs();
        //Debug.Log("UnloadUnusedAssets after WarmPrefabsCoroutine @ " + Time.realtimeSinceStartup);
        //yield return Resources.UnloadUnusedAssets();

        notify.Debug("starting loadNewTrackMaterial for " + destinationEnvironmentSetId);
        loadNewTrackMaterial(EnvId);
        notify.Debug("finished loadNewTrackMaterial for " + destinationEnvironmentSetId);
        ReloadEnvAudio(EnvId);

        ResourceManager.AllowLoad = false;

        addNewEnvironmentSetObjects();
    }

    public IEnumerator UnLoadEnvironmentResources(int EnvId)
    {
        //清理旧关卡场景对象
        yield return StartCoroutine(deleteOldObjects());
        releaseEnvMaterials();

        yield return null;//make sure we rendered the full whiteout frame

        //清除旧关卡内存
        notify.Debug("Unloading asset bundle");
        string assetBundleName = EnvironmentSetManager.SharedInstance.GetAssetBundleName(EnvId);
        if (assetBundleName != null)
            ResourceManager.SharedInstance.UnloadAssetBundle(assetBundleName);

        yield return Resources.UnloadUnusedAssets();
        System.GC.Collect();
    }

	public void ReloadEnvAudio(int envId)
	{// this is being called from GameController to load envAudio when we launch the game
		notify.Debug("ReloadEnvAudio " + envId);
		AudioManager.SharedInstance.SFXready = false;
		AddNewMusicResource(envId);
		//AddNewSfxResourceCoroutine(envId);
		AudioManager.SharedInstance.SFXready = true;
	}
	
	IEnumerator ReloadEnvAudioCoroutine(int envId){
		AudioManager.SharedInstance.SFXready = false;
		yield return StartCoroutine(AddNewMusicResourceCoroutine(envId));
		//yield return StartCoroutine(AddNewSfxResourceCoroutine(envId));
		AudioManager.SharedInstance.SFXready = true;
	}

	public void AddNewMusicResource(int envId )
	{
		notify.Debug("AddNewMusicResource----------------------- " + envId);
        if (GamePlayer.SharedInstance.isPlane)
            nextMusicClip = ResourceManager.Load("Music/IceSheet") as AudioClip;
        else
		    nextMusicClip = ResourceManager.Load(EnvironmentSetManager.SharedInstance.LocalDict[envId].MusicFile) as AudioClip;

		AudioManager.SharedInstance.GameMusic = nextMusicClip;
		/*
		if(nextMusicClip) 
		{
			notify.Debug("We loaded a new Music Clip " + nextMusicClip.name);
			if(autoSwitch) AudioManager.SharedInstance.SwitchMusic(nextMusicClip);
		}
		*/
	}
	
	public IEnumerator AddNewMusicResourceCoroutine(int envId ){
		notify.Debug("AddNewMusicResource----------------------- " + envId);

		AssetBundleRequest abr = ResourceManager.SharedInstance.LoadAsyncFromAssetBundle(EnvironmentSetManager.SharedInstance.LocalDict[envId].MusicFile, typeof(AudioClip)); 
				//AssetBundleRequest abr = ResourceManager.SharedInstance.LoadAsyncFromAssetBundle(materialNames[i], typeof(Material));
		if(abr != null)
		{
			yield return abr;
			nextMusicClip = abr.asset as AudioClip;//ResourceManager.Load(EnvironmentSetManager.SharedInstance.LocalDict[envId].MusicFile) as AudioClip;
		}
		else
		{
			nextMusicClip = ResourceManager.Load(EnvironmentSetManager.SharedInstance.LocalDict[envId].MusicFile) as AudioClip;
		}
		if(nextMusicClip) 
		{
			notify.Debug("We loaded a new Music Clip " + nextMusicClip.name);
			//if(autoSwitch) AudioManager.SharedInstance.SwitchMusic(nextMusicClip);
			AudioClip oldMusicClip = AudioManager.SharedInstance.GameMusic;
			AudioManager.SharedInstance.GameMusic = nextMusicClip;
			Resources.UnloadAsset(oldMusicClip);
		}
		yield return null;//new WaitForSeconds(0.05f);
	}

	
	/*public void AddNewSfxResource(int envId )
	{
		notify.Debug("AddNewSfxResource----------------------- " + envId);
		AudioClip footstepNew = null;
		AudioClip tempClip = null;
		footstepNew = ResourceManager.Load(EnvironmentSetManager.SharedInstance.LocalDict[envId].FootstepFile1) as AudioClip;
		
		if(footstepNew) 
		{
			notify.Debug ("Found footstep1 " + footstepNew.name);
			if(GamePlayer.SharedInstance.footsteps) 
			{
				tempClip = GamePlayer.SharedInstance.footsteps.footstepsSfx[0];
				GamePlayer.SharedInstance.footsteps.footstepsSfx[0] = footstepNew;
			}
			if(AudioManager.SharedInstance!=null)	AudioManager.SharedInstance.footstepsSfx[0] = footstepNew; // we also referencing the audioclip in AudioManager since at application launch we don't have a GamePlayer instanst
			if(tempClip != null)
			{
				Resources.UnloadAsset(tempClip);
				tempClip = null;
			}
			footstepNew = null;
		}
		
		footstepNew = ResourceManager.Load(EnvironmentSetManager.SharedInstance.LocalDict[envId].FootstepFile2) as AudioClip;

		if(footstepNew != null) 
		{
			notify.Debug ("Found footstep2 " + footstepNew.name);
			if(GamePlayer.SharedInstance.footsteps) 
			{
				tempClip = GamePlayer.SharedInstance.footsteps.footstepsSfx[1];
				GamePlayer.SharedInstance.footsteps.footstepsSfx[1] = footstepNew;
			}
			if(AudioManager.SharedInstance!=null)
			{
				AudioManager.SharedInstance.footstepsSfx[1] = footstepNew;
			}
			if(tempClip != null)
			{
				Resources.UnloadAsset(tempClip);
				tempClip = null;
			}
			footstepNew = null;
		}
	
		footstepNew = ResourceManager.Load(EnvironmentSetManager.SharedInstance.LocalDict[envId].FootstepFile3) as AudioClip;
		if(footstepNew != null)  
		{
			notify.Debug ("Found footstep3 " + footstepNew.name);
			if(GamePlayer.SharedInstance.footsteps) 
			{
				tempClip = GamePlayer.SharedInstance.footsteps.footstepsSfx[2];
				GamePlayer.SharedInstance.footsteps.footstepsSfx[2] = footstepNew;
			}
			if(AudioManager.SharedInstance!=null)	AudioManager.SharedInstance.footstepsSfx[2] = footstepNew;
			if(tempClip != null)
			{
				Resources.UnloadAsset(tempClip);
				tempClip = null;
			}
			footstepNew = null;
		}
		footstepNew = ResourceManager.Load(EnvironmentSetManager.SharedInstance.LocalDict[envId].FootstepFile4) as AudioClip;
		if(footstepNew != null)  
		{
			notify.Debug ("Found footstep4 " + footstepNew.name);
			if(GamePlayer.SharedInstance.footsteps) 
			{
				tempClip = GamePlayer.SharedInstance.footsteps.footstepsSfx[3];
				GamePlayer.SharedInstance.footsteps.footstepsSfx[3] = footstepNew;
			}
			if(AudioManager.SharedInstance!=null)	AudioManager.SharedInstance.footstepsSfx[3] = footstepNew;
			if(tempClip != null)
			{
				Resources.UnloadAsset(tempClip);
				tempClip = null;
			}
			footstepNew = null;
		}
		footstepNew = ResourceManager.Load(EnvironmentSetManager.SharedInstance.LocalDict[envId].FootstepFile5) as AudioClip;
		if(footstepNew)  
		{
			notify.Debug ("Found footstep5 " + footstepNew.name);
			if(GamePlayer.SharedInstance.footsteps) 
			{
				tempClip = GamePlayer.SharedInstance.footsteps.footstepsSfx[4];
				GamePlayer.SharedInstance.footsteps.footstepsSfx[4] = footstepNew;
			}
			if(AudioManager.SharedInstance!=null)	AudioManager.SharedInstance.footstepsSfx[4] = footstepNew;
			if(tempClip != null)
			{
				Resources.UnloadAsset(tempClip);
				tempClip = null;
			}
			footstepNew = null;
		}
		// jumping
		footstepNew = ResourceManager.Load(EnvironmentSetManager.SharedInstance.LocalDict[envId].JumpingSoundEffect) as AudioClip;
		if(footstepNew)  
		{
			notify.Debug ("Found LandingSoundEffect " + footstepNew.name);
			if(AudioManager.SharedInstance!=null)	
			{
				tempClip = AudioManager.SharedInstance.jumping;
				AudioManager.SharedInstance.jumping = footstepNew; 
			}
			if(tempClip != null)
			{
				Resources.UnloadAsset(tempClip);
				tempClip = null;
			}
			footstepNew = null;
		}
		// landing
		footstepNew = ResourceManager.Load(EnvironmentSetManager.SharedInstance.LocalDict[envId].LandingSoundEffect) as AudioClip;
		if(footstepNew)  
		{
			notify.Debug ("Found LandingSoundEffect " + footstepNew.name);
			if(AudioManager.SharedInstance!=null)	
			{
				tempClip = AudioManager.SharedInstance.landing;
				AudioManager.SharedInstance.landing = footstepNew; 
			}
			if(tempClip != null)
			{
				Resources.UnloadAsset(tempClip);
				tempClip = null;
			}
			footstepNew = null;
		}
		// sliding
		footstepNew = ResourceManager.Load(EnvironmentSetManager.SharedInstance.LocalDict[envId].SlidingSoundEffect) as AudioClip;
		if(footstepNew)  
		{
			notify.Debug ("Found LandingSoundEffect " + footstepNew.name);
			if(AudioManager.SharedInstance!=null)	
			{
				tempClip = AudioManager.SharedInstance.sliding;
				AudioManager.SharedInstance.sliding = footstepNew; 
			}
			if(tempClip != null)
			{
				Resources.UnloadAsset(tempClip);
				tempClip = null;
			}
			footstepNew = null;
		}
		// turnLeft
		footstepNew = ResourceManager.Load(EnvironmentSetManager.SharedInstance.LocalDict[envId].TurnLeftSoundEffect) as AudioClip;
		if(footstepNew)  
		{
			notify.Debug ("Found LandingSoundEffect " + footstepNew.name);
			if(AudioManager.SharedInstance!=null)	
			{
				tempClip = AudioManager.SharedInstance.turnLeft;
				AudioManager.SharedInstance.turnLeft = footstepNew; 
			}
			Resources.UnloadAsset(tempClip);
			tempClip = null;
			footstepNew = null;
		}
		// landing
		footstepNew = ResourceManager.Load(EnvironmentSetManager.SharedInstance.LocalDict[envId].TurnRightSoundEffect) as AudioClip;
		if(footstepNew)  
		{
			notify.Debug ("Found LandingSoundEffect " + footstepNew.name);
			if(AudioManager.SharedInstance!=null)	
			{
				tempClip = AudioManager.SharedInstance.turnRight;
				AudioManager.SharedInstance.turnRight = footstepNew; 
			}
			if(tempClip != null)
			{
				Resources.UnloadAsset(tempClip);
				tempClip = null;
			}
			footstepNew = null;
		}
	}
	
	public IEnumerator AddNewSfxResourceCoroutine(int envId )
	{
		notify.Debug("AddNewSfxResource----------------------- " + envId);
		EnvironmentSetData data = EnvironmentSetManager.SharedInstance.LocalDict[envId];
		AudioClip footstepNew = null;
		AudioClip tempClip = null;
		AssetBundleRequest abr = ResourceManager.SharedInstance.LoadAsyncFromAssetBundle(data.FootstepFile1, typeof(AudioClip)); 
		if(abr != null)
		{
			yield return abr;
			footstepNew = abr.asset as AudioClip;//ResourceManager.Load(EnvironmentSetManager.SharedInstance.LocalDict[envId].FootstepFile1) as AudioClip;
		}
		else
		{
			footstepNew = ResourceManager.Load(EnvironmentSetManager.SharedInstance.LocalDict[envId].FootstepFile1) as AudioClip;
		}
		
		if(footstepNew) 
		{
			notify.Debug ("Found footstep1 " + footstepNew.name);
			if(GamePlayer.SharedInstance.footsteps) 
			{
				tempClip = GamePlayer.SharedInstance.footsteps.footstepsSfx[0];
				GamePlayer.SharedInstance.footsteps.footstepsSfx[0] = footstepNew;
			}
			if(AudioManager.SharedInstance!=null)	AudioManager.SharedInstance.footstepsSfx[0] = footstepNew; // we also referencing the audioclip in AudioManager since at application launch we don't have a GamePlayer instanst
			if(tempClip != null)
			{
				Resources.UnloadAsset(tempClip);
				tempClip = null;
			}
			footstepNew = null;
		}
		
		abr = ResourceManager.SharedInstance.LoadAsyncFromAssetBundle(data.FootstepFile2, typeof(AudioClip)); 
		if(abr != null)
		{
			yield return abr;
			footstepNew = abr.asset as AudioClip;//ResourceManager.Load(EnvironmentSetManager.SharedInstance.LocalDict[envId].FootstepFile2) as AudioClip;
		}
		else
		{
			footstepNew = ResourceManager.Load(EnvironmentSetManager.SharedInstance.LocalDict[envId].FootstepFile2) as AudioClip;
		}

		if(footstepNew != null) 
		{
			notify.Debug ("Found footstep2 " + footstepNew.name);
			if(GamePlayer.SharedInstance.footsteps) 
			{
				tempClip = GamePlayer.SharedInstance.footsteps.footstepsSfx[1];
				GamePlayer.SharedInstance.footsteps.footstepsSfx[1] = footstepNew;
			}
			if(AudioManager.SharedInstance!=null)
			{
				AudioManager.SharedInstance.footstepsSfx[1] = footstepNew;
			}
			if(tempClip != null)
			{
				Resources.UnloadAsset(tempClip);
				tempClip = null;
			}
			footstepNew = null;
		}
	
		abr = ResourceManager.SharedInstance.LoadAsyncFromAssetBundle(data.FootstepFile3, typeof(AudioClip)); 
		if(abr != null)
		{
			yield return abr;
			footstepNew = abr.asset as AudioClip;//ResourceManager.Load(EnvironmentSetManager.SharedInstance.LocalDict[envId].FootstepFile3) as AudioClip;
		}
		else
		{
			footstepNew = ResourceManager.Load(EnvironmentSetManager.SharedInstance.LocalDict[envId].FootstepFile3) as AudioClip;
		}
		if(footstepNew != null)  
		{
			notify.Debug ("Found footstep3 " + footstepNew.name);
			if(GamePlayer.SharedInstance.footsteps) 
			{
				tempClip = GamePlayer.SharedInstance.footsteps.footstepsSfx[2];
				GamePlayer.SharedInstance.footsteps.footstepsSfx[2] = footstepNew;
			}
			if(AudioManager.SharedInstance!=null)	AudioManager.SharedInstance.footstepsSfx[2] = footstepNew;
			if(tempClip != null)
			{
				Resources.UnloadAsset(tempClip);
				tempClip = null;
			}
			footstepNew = null;
		}
		abr = ResourceManager.SharedInstance.LoadAsyncFromAssetBundle(data.FootstepFile4, typeof(AudioClip)); 
		if(abr != null)
		{
			yield return abr;
			footstepNew = abr.asset as AudioClip;//ResourceManager.Load(EnvironmentSetManager.SharedInstance.LocalDict[envId].FootstepFile4) as AudioClip;
		}
		else
		{
			footstepNew = ResourceManager.Load(EnvironmentSetManager.SharedInstance.LocalDict[envId].FootstepFile4) as AudioClip;
		}
		if(footstepNew != null)  
		{
			notify.Debug ("Found footstep4 " + footstepNew.name);
			if(GamePlayer.SharedInstance.footsteps) 
			{
				tempClip = GamePlayer.SharedInstance.footsteps.footstepsSfx[3];
				GamePlayer.SharedInstance.footsteps.footstepsSfx[3] = footstepNew;
			}
			if(AudioManager.SharedInstance!=null)	AudioManager.SharedInstance.footstepsSfx[3] = footstepNew;
			if(tempClip != null)
			{
				Resources.UnloadAsset(tempClip);
				tempClip = null;
			}
			footstepNew = null;
		}
		abr = ResourceManager.SharedInstance.LoadAsyncFromAssetBundle(data.FootstepFile5, typeof(AudioClip)); 
		if(abr != null)
		{
			yield return abr;
			footstepNew = abr.asset as AudioClip;//ResourceManager.Load(EnvironmentSetManager.SharedInstance.LocalDict[envId].FootstepFile5) as AudioClip;
		}
		else
		{
			footstepNew = ResourceManager.Load(EnvironmentSetManager.SharedInstance.LocalDict[envId].FootstepFile5) as AudioClip;
		}
		if(footstepNew)  
		{
			notify.Debug ("Found footstep5 " + footstepNew.name);
			if(GamePlayer.SharedInstance.footsteps) 
			{
				tempClip = GamePlayer.SharedInstance.footsteps.footstepsSfx[4];
				GamePlayer.SharedInstance.footsteps.footstepsSfx[4] = footstepNew;
			}
			if(AudioManager.SharedInstance!=null)	AudioManager.SharedInstance.footstepsSfx[4] = footstepNew;
			if(tempClip != null)
			{
				Resources.UnloadAsset(tempClip);
				tempClip = null;
			}
			footstepNew = null;
		}
		// jumping
		abr = ResourceManager.SharedInstance.LoadAsyncFromAssetBundle(data.JumpingSoundEffect, typeof(AudioClip)); 
		if(abr != null)
		{
			yield return abr;
			footstepNew = abr.asset as AudioClip;//ResourceManager.Load(EnvironmentSetManager.SharedInstance.LocalDict[envId].JumpingSoundEffect) as AudioClip;
		}
		else
		{
			footstepNew = ResourceManager.Load(EnvironmentSetManager.SharedInstance.LocalDict[envId].JumpingSoundEffect) as AudioClip;
		}
		if(footstepNew)  
		{
			notify.Debug ("Found LandingSoundEffect " + footstepNew.name);
			if(AudioManager.SharedInstance!=null)	
			{
				tempClip = AudioManager.SharedInstance.jumping;
				AudioManager.SharedInstance.jumping = footstepNew; 
			}
			if(tempClip != null)
			{
				Resources.UnloadAsset(tempClip);
				tempClip = null;
			}
			footstepNew = null;
		}
		// landing
		abr = ResourceManager.SharedInstance.LoadAsyncFromAssetBundle(data.LandingSoundEffect, typeof(AudioClip)); 
		if(abr != null)
		{
			yield return abr;
			footstepNew = abr.asset as AudioClip;//ResourceManager.Load(EnvironmentSetManager.SharedInstance.LocalDict[envId].LandingSoundEffect) as AudioClip;
		}
		else
		{
			footstepNew = ResourceManager.Load(EnvironmentSetManager.SharedInstance.LocalDict[envId].LandingSoundEffect) as AudioClip;
		}
		if(footstepNew)  
		{
			notify.Debug ("Found LandingSoundEffect " + footstepNew.name);
			if(AudioManager.SharedInstance!=null)	
			{
				tempClip = AudioManager.SharedInstance.landing;
				AudioManager.SharedInstance.landing = footstepNew; 
			}
			if(tempClip != null)
			{
				Resources.UnloadAsset(tempClip);
				tempClip = null;
			}
			footstepNew = null;
		}
		// sliding
		abr = ResourceManager.SharedInstance.LoadAsyncFromAssetBundle(data.SlidingSoundEffect, typeof(AudioClip)); 
		if(abr != null)
		{
			yield return abr;
			footstepNew = abr.asset as AudioClip;//ResourceManager.Load(EnvironmentSetManager.SharedInstance.LocalDict[envId].SlidingSoundEffect) as AudioClip;
		}
		else
		{
			footstepNew = ResourceManager.Load(EnvironmentSetManager.SharedInstance.LocalDict[envId].SlidingSoundEffect) as AudioClip;
		}
		if(footstepNew)  
		{
			notify.Debug ("Found LandingSoundEffect " + footstepNew.name);
			if(AudioManager.SharedInstance!=null)	
			{
				tempClip = AudioManager.SharedInstance.sliding;
				AudioManager.SharedInstance.sliding = footstepNew; 
			}
			if(tempClip != null)
			{
				Resources.UnloadAsset(tempClip);
				tempClip = null;
			}
			footstepNew = null;
		}
		// turnLeft
		abr = ResourceManager.SharedInstance.LoadAsyncFromAssetBundle(data.TurnLeftSoundEffect, typeof(AudioClip)); 
		if(abr != null)
		{
			yield return abr;
			footstepNew = abr.asset as AudioClip;//ResourceManager.Load(EnvironmentSetManager.SharedInstance.LocalDict[envId].TurnLeftSoundEffect) as AudioClip;
		}
		else
		{
			footstepNew = ResourceManager.Load(EnvironmentSetManager.SharedInstance.LocalDict[envId].TurnLeftSoundEffect) as AudioClip;
		}
		if(footstepNew)  
		{
			notify.Debug ("Found LandingSoundEffect " + footstepNew.name);
			if(AudioManager.SharedInstance!=null)	
			{
				tempClip = AudioManager.SharedInstance.turnLeft;
				AudioManager.SharedInstance.turnLeft = footstepNew; 
			}
			Resources.UnloadAsset(tempClip);
			tempClip = null;
			footstepNew = null;
		}
		// landing
		abr = ResourceManager.SharedInstance.LoadAsyncFromAssetBundle(data.TurnRightSoundEffect, typeof(AudioClip)); 
		if(abr != null)
		{
			yield return abr;
			footstepNew = abr.asset as AudioClip;//ResourceManager.Load(EnvironmentSetManager.SharedInstance.LocalDict[envId].TurnRightSoundEffect) as AudioClip;
		}
		else
		{
			footstepNew = ResourceManager.Load(EnvironmentSetManager.SharedInstance.LocalDict[envId].TurnRightSoundEffect) as AudioClip;
		}
		if(footstepNew)  
		{
			notify.Debug ("Found LandingSoundEffect " + footstepNew.name);
			if(AudioManager.SharedInstance!=null)	
			{
				tempClip = AudioManager.SharedInstance.turnRight;
				AudioManager.SharedInstance.turnRight = footstepNew; 
			}
			if(tempClip != null)
			{
				Resources.UnloadAsset(tempClip);
				tempClip = null;
			}
			footstepNew = null;
		}
		
		yield return null;//new WaitForSeconds(0.05f);
	}*/
	
	private string AlternateEnvironmentMaterialSuffix(bool allowAlt)
	{
		if(allowAlt)
		{
			switch(GameController.SharedInstance.GetDeviceGeneration())
			{
			case GameController.DeviceGeneration.Unsupported:
			case GameController.DeviceGeneration.iPhone3GS:
			case GameController.DeviceGeneration.iPodTouch3:
			case GameController.DeviceGeneration.MedEnd:
				return "_lo";
			case GameController.DeviceGeneration.LowEnd:
			case GameController.DeviceGeneration.iPhone4:
			case GameController.DeviceGeneration.iPodTouch4:
				return "_opt";
			default:
				return "_hi";
			}
		}
		else
		{
			return "";
		}
	}
	
	private IEnumerator loadEnvMaterialsCoroutine(string[] materialNames, GameController.EnvironmentMaterials materialsOut, bool allowAlt)
	{
		string materialSuffix = AlternateEnvironmentMaterialSuffix(allowAlt);
	
		Material[] materials = new Material[materialNames.Length];
		for(int i = 0; i < materials.Length; ++i)
		{
			if((materialNames[i] != null) && (materialNames[i] != ""))
			{
				AssetBundleRequest abr = ResourceManager.SharedInstance.LoadAsyncFromAssetBundle(materialNames[i], typeof(Material));
				if(abr != null)
				{
					yield return abr;
					materials[i] = abr.asset as Material;//ResourceManager.Load(materialNames[0] + materialSuffix, typeof(Material)) as Material;
				}
				else
				{
					materials[i] = ResourceManager.Load(materialNames[i] + materialSuffix, typeof(Material)) as Material;
				}
				if(materials[i] == null)
				{//fall back to low res
					abr = ResourceManager.SharedInstance.LoadAsyncFromAssetBundle(materialNames[i] + "_lo", typeof(Material));
					if(abr != null)
					{
						yield return abr;
						materials[i] = abr.asset as Material;
					}
					else
					{
						materials[i] = ResourceManager.Load(materialNames[i], typeof(Material)) as Material;
					}
				}
				if(materials[i] != null)
				{
					materials[i].mainTexture.mipMapBias = GameController.SharedInstance.EnvironmentMipMapBias;
				}
				else
				{
					notify.Warning("Failed to load " + materialNames[i]);
				}
			}
		}

		materialsOut.Opaque = materials[0];
		materialsOut.Decal = materials[1];
		for(int i = 0; i < materialsOut.Extra.Length; ++i)
		{
			if((i+2 < materials.Length)&&(materials[i+2] != null))
			{
				materialsOut.Extra[i] = materials[i+2];
			}
		}
	}

	private void releaseEnvMaterials(GameController.EnvironmentMaterials materials)
	{
		materials.Opaque = null;
		materials.Decal = null;
		for(int i =0; i < materials.Extra.Length; ++i)
		{
			materials.Extra[i] = null;
		}
	}	

	public void releaseEnvMaterials()
	{
		releaseEnvMaterials(GameController.SharedInstance.TrackMaterials);
		releaseEnvMaterials(GameController.SharedInstance.TrackFadeMaterials);
	}	
	
	private void loadEnvMaterials(string[] materialNames, GameController.EnvironmentMaterials materialsOut, bool allowAlt)
	{
		string materialSuffix = AlternateEnvironmentMaterialSuffix(allowAlt);
	
		Material[] materials = new Material[materialNames.Length];
		for(int i = 0; i < materials.Length; ++i)
		{
			if((materialNames[i] != null) && (materialNames[i] != ""))
			{
				materials[i] = ResourceManager.Load(materialNames[i] + materialSuffix, typeof(Material)) as Material;
				if(materials[i] == null)
				{//fall back to low res
					materials[i] = ResourceManager.Load(materialNames[i] + "_lo", typeof(Material)) as Material;
				}
				if(materials[i] != null)
                {
                    notify.Debug("load materials:"+materials[i].name);
					//materials[i].mainTexture.mipMapBias = GameController.SharedInstance.EnvironmentMipMapBias;
				}
				else
				{
					notify.Warning("Failed to load " + materialNames[i]);
				}
			}
		}

		materialsOut.Opaque = materials[0];
		materialsOut.Decal = materials[1];
		for(int i = 0; i < materialsOut.Extra.Length; ++i)
		{
			if((i+2 < materials.Length)&&(materials[i+2] != null))
			{
				materialsOut.Extra[i] = materials[i+2];
			}
		}
	}
	
	public IEnumerator loadNewTrackMaterialCoroutine(int newEnvironmentSet)
	{
		GameController.TextureReport("Before new track materials coroutine");
		if (EnvironmentSetManager.SharedInstance.IsLocallyAvailable(newEnvironmentSet))
		{
			if(GameController.SharedInstance.TunnelMaterials == null)
			{
				GameController.SharedInstance.TunnelMaterials = new GameController.EnvironmentMaterials();
				string[] tunnelMaterials = {
					"Oz/Materials/oz_tt_master_opaque",
					null,
					"Oz/Materials/oz_tt_master_fade_road_fog",
					"Oz/Materials/oz_tt_master_fade_bricks_fog",
 					"Oz/Materials/oz_tt_master_opaque_spinning_fog",
					"Oz/Materials/oz_tt_master_decals_spinning"
				};
				yield return StartCoroutine(loadEnvMaterialsCoroutine(tunnelMaterials, GameController.SharedInstance.TunnelMaterials,false));
			}
			if(GameController.SharedInstance.TunnelFadeMaterials == null)
			{
				GameController.SharedInstance.TunnelFadeMaterials = new GameController.EnvironmentMaterials();
				string[] tunnelFadeMaterials = {
					"Oz/Materials/oz_tt_master_alpha",
					null,
					"Oz/Materials/oz_tt_master_fade_road_fog",
					"Oz/Materials/oz_tt_master_fade_bricks_fog",
 					"Oz/Materials/oz_tt_master_opaquefade_spinning_fog",
					"Oz/Materials/oz_tt_master_decalsFade_spinning"
				};
				yield return StartCoroutine(loadEnvMaterialsCoroutine(tunnelFadeMaterials, GameController.SharedInstance.TunnelFadeMaterials,false));
			}

			if(GameController.SharedInstance.BalloonMaterials == null)
			{
				GameController.SharedInstance.BalloonMaterials = new GameController.EnvironmentMaterials();
				string[] balloonMaterials = {
					"Oz/Materials/oz_bc_master_opaque_fog",
					"Oz/Materials/oz_bc_master_decals_fog"
				};
				yield return StartCoroutine(loadEnvMaterialsCoroutine(balloonMaterials, GameController.SharedInstance.BalloonMaterials,false));
			}
			if(GameController.SharedInstance.BalloonFadeMaterials == null)
			{
				GameController.SharedInstance.BalloonFadeMaterials = new GameController.EnvironmentMaterials();
				string[] balloonFadeMaterials = {
					"Oz/Materials/oz_bc_master_alpha_fog",
					"Oz/Materials/oz_bc_master_alpha_decals_fog"
				};
				yield return StartCoroutine(loadEnvMaterialsCoroutine(balloonFadeMaterials, GameController.SharedInstance.BalloonFadeMaterials,false));
			}			
			
			EnvironmentSetData envSetData = EnvironmentSetManager.SharedInstance.LocalDict[newEnvironmentSet];
		
			string[] opaque = new string[4];
			string[] fade = new string[4];
			opaque[0] = envSetData.OpaqueMaterialPath;
			opaque[1] =	envSetData.DecalMaterialPath;
			opaque[2] = envSetData.Extra1MaterialPath;
			opaque[3] = envSetData.Extra2MaterialPath;
//			opaque[4] = envSetData.Extra3MaterialPath;
//			opaque[5] = envSetData.Extra4MaterialPath;

			//regular materials
			if(GameController.SharedInstance.TrackMaterials == null)
			{
				GameController.SharedInstance.TrackMaterials = new GameController.EnvironmentMaterials();
			}
			yield return StartCoroutine(loadEnvMaterialsCoroutine(opaque, GameController.SharedInstance.TrackMaterials,true));
			
			fade[0] = envSetData.FadeOutMaterialPath;
			fade[1] = envSetData.DecalFadeOutMaterialPath;
			fade[2] = envSetData.Extra1FadeMaterialPath;
			fade[3] = envSetData.Extra2FadeMaterialPath;
//			fade[4] = envSetData.Extra3FadeMaterialPath;
//			fade[5] = envSetData.Extra4FadeMaterialPath;

			if(GameController.SharedInstance.TrackFadeMaterials == null)
			{
				GameController.SharedInstance.TrackFadeMaterials = new GameController.EnvironmentMaterials();
			}
			yield return StartCoroutine(loadEnvMaterialsCoroutine(fade, GameController.SharedInstance.TrackFadeMaterials,true));
		}
		else
		{
			notify.Error("Don't know how to load the new track material for environment set " + newEnvironmentSet);
		}
		GameController.TextureReport("After new track materials coroutine");
		
	}
	
	/// <summary>
	/// Loads the new track material for the upcoming environment set
	/// </summary>
	/// <returns>
	/// True if the track material was loaded, false if it couldn't be found
	/// </returns>
	/// <param name='newEnvironmentSet'>
	/// 0 for machu, 1 for whimsey woods, 
	/// </param>
	public bool loadNewTrackMaterial(int newEnvironmentSet)
	{
		GameController.TextureReport("Before new track materials");
		bool result = true;
		
		if (EnvironmentSetManager.SharedInstance.IsLocallyAvailable(newEnvironmentSet))
		{
			if(GameController.SharedInstance.TunnelMaterials == null)
			{
				GameController.SharedInstance.TunnelMaterials = new GameController.EnvironmentMaterials();
				string[] tunnelMaterials = {
					"Oz/Materials/oz_tt_master_opaque",
					null,
					"Oz/Materials/oz_tt_master_fade_road_fog",
					"Oz/Materials/oz_tt_master_fade_bricks_fog",
 					"Oz/Materials/oz_tt_master_opaque_spinning_fog",
					"Oz/Materials/oz_tt_master_decals_spinning"
				};
				loadEnvMaterials(tunnelMaterials, GameController.SharedInstance.TunnelMaterials,false);
			}
			if(GameController.SharedInstance.TunnelFadeMaterials == null)
			{
				GameController.SharedInstance.TunnelFadeMaterials = new GameController.EnvironmentMaterials();
				string[] tunnelFadeMaterials = {
					"Oz/Materials/oz_tt_master_alpha",
					null,
					"Oz/Materials/oz_tt_master_fade_road_fog",
					"Oz/Materials/oz_tt_master_fade_bricks_fog",
 					"Oz/Materials/oz_tt_master_opaquefade_spinning_fog",
					"Oz/Materials/oz_tt_master_decalsFade_spinning"
				};
				loadEnvMaterials(tunnelFadeMaterials, GameController.SharedInstance.TunnelFadeMaterials,false);
			}

			if(GameController.SharedInstance.BalloonMaterials == null)
			{
				GameController.SharedInstance.BalloonMaterials = new GameController.EnvironmentMaterials();
				string[] balloonMaterials = {
					"Oz/Materials/oz_bc_master_opaque_fog",
					"Oz/Materials/oz_bc_master_decals_fog"
				};
				loadEnvMaterials(balloonMaterials, GameController.SharedInstance.BalloonMaterials,false);
			}
			if(GameController.SharedInstance.BalloonFadeMaterials == null)
			{
				GameController.SharedInstance.BalloonFadeMaterials = new GameController.EnvironmentMaterials();
				string[] balloonFadeMaterials = {
					"Oz/Materials/oz_bc_master_alpha_fog",
					"Oz/Materials/oz_bc_master_alpha_decals_fog"
				};
				loadEnvMaterials(balloonFadeMaterials, GameController.SharedInstance.BalloonFadeMaterials,false);
			}			
			
			EnvironmentSetData envSetData = EnvironmentSetManager.SharedInstance.LocalDict[newEnvironmentSet];
		
			string[] opaque = new string[4];
			string[] fade = new string[4];
			opaque[0] = envSetData.OpaqueMaterialPath;
			opaque[1] =	envSetData.DecalMaterialPath;
			opaque[2] = envSetData.Extra1MaterialPath;
			opaque[3] = envSetData.Extra2MaterialPath;
//			opaque[4] = envSetData.Extra3MaterialPath;
//			opaque[5] = envSetData.Extra4MaterialPath;

			//regular materials
			if(GameController.SharedInstance.TrackMaterials == null)
			{
				GameController.SharedInstance.TrackMaterials = new GameController.EnvironmentMaterials();
			}
			loadEnvMaterials(opaque, GameController.SharedInstance.TrackMaterials,true);
			
			fade[0] = envSetData.FadeOutMaterialPath;
			fade[1] = envSetData.DecalFadeOutMaterialPath;
			fade[2] = envSetData.Extra1FadeMaterialPath;
			fade[3] = envSetData.Extra2FadeMaterialPath;
//			fade[4] = envSetData.Extra3FadeMaterialPath;
//			fade[5] = envSetData.Extra4FadeMaterialPath;

			if(GameController.SharedInstance.TrackFadeMaterials == null)
			{
				GameController.SharedInstance.TrackFadeMaterials = new GameController.EnvironmentMaterials();
			}
			loadEnvMaterials(fade, GameController.SharedInstance.TrackFadeMaterials,true);
			

		}
		else
		{
			result = false;
			notify.Error("Don't know how to load the new track material for environment set " + newEnvironmentSet);
		}
		GameController.TextureReport("After new track materials");
		return result;
	}
	
	/// <summary>
	/// 请求切换环境
	/// </summary> 
	public void RequestEnvironmentSetChange( int newDestId)
	{
		notify.Debug("RequestEnvironmentSetChange");
		originEnvironmentSetId = TrackBuilder.SharedInstance.CurrentEnvironmentSetId;
		destinationEnvironmentSetId = newDestId;
		wantNewEnvSet = true;
	
		// we need to do this to avoid tunnel entrance being placed twice
		TrackPiece.DistanceSubtracedPerPieceAdded = 3000.0f;  // yes it's humongously large
		
		// show envProgressHud
        //UIManagerOz.SharedInstance.inGameVC.ShowEnvProgress(destinationEnvironmentSetId);
	}
	
	public void CancelEnvironmentSetChange()
	{
		wantNewEnvSet = false;
		if(UIManagerOz.SharedInstance.inGameVC!=null)
			UIManagerOz.SharedInstance.inGameVC.HideEnvProgress();
	}
	
	/// <summary>
	/// Gets the random destination environment set.
	/// </summary>
	/// <returns>
	/// 0 for machu, 1 for whimsy woods, others added as implemented
	/// </returns> 
	
	public void PlayerTurned(TrackPiece piece, int segment)
	{
		bool isLeft = !piece.UseAlternatePath;
		
		if (GamePlayer.SharedInstance.OnTrackPiece != null && GamePlayer.SharedInstance.OnTrackPiece.TrackType == TrackPiece.PieceType.kTPEnvSetJunction)
		{
			// okay so for now we say he's going to a different environment set if he turns towards the tunnel transitions
			if (!wantNewEnvSet)
			{
				TransitionSignDecider decider = GamePlayer.SharedInstance.OnTrackPiece.GetComponentInChildren<TransitionSignDecider>();
				if (decider == null)
				{
					notify.Error("couldn't find TransitionSignDecider in " + GamePlayer.SharedInstance.OnTrackPiece);
				}
				
				//Phil - Turn Towards Transition - Now how do we set the distance...
				if (decider.MainLeftGoesToTransitionTunnel)  
				{
					if (isLeft && (GamePlayer.SharedInstance.IsTurningLeft||GamePlayer.SharedInstance.JustTurned)) 
					{
						RequestEnvironmentSetChange(decider.DestinationId);	
						
						//Reduce the "DifficultyDistance" in GameController
						if(firstTransition)
							GameController.SharedInstance.DifficultyDistanceTraveled -= GameProfile.SharedInstance.EnvSignDifficultyReduction;
					}
				}
				else  {
					// so turning right goes to transition tunnel, and he did turn right
					if ( ! isLeft && (GamePlayer.SharedInstance.IsTurningRight||GamePlayer.SharedInstance.JustTurned)) 
					{
						RequestEnvironmentSetChange(decider.DestinationId);
						
						//Reduce the "DifficultyDistance" in GameController
						if(firstTransition)
							GameController.SharedInstance.DifficultyDistanceTraveled -= GameProfile.SharedInstance.EnvSignDifficultyReduction;
					}
				}
			}
		}
	}

}
