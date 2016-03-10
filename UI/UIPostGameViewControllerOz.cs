using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIPostGameViewControllerOz : UIViewControllerOz
{
    public PostAccount postAcount;
	public GameObject weeklyRoot;
	public GameObject objectivesRoot;
	public GameObject statsRoot;
    private StatsRoot statsRootCmpt = null;
	public GameObject statsButton;

	public GameObject rewardPanel;
	public UILabel	  rewardLabel;
	public UISprite	  rewardIcon;
	public GameObject speedUpButton;
	
	public List<GameObject> bottomPanelGOs = new List<GameObject>();
	
    public GameObject btnHome;
    public GameObject btnRunAgain;
    public GameObject btnShare;
    public GameObject btnSpeedUp;
    public GameObject tutorialHome;
    public GameObject tutorialUpgrade;
	private bool statsShowing = false;
	
	private bool canSwitchPages = false; // we can't switch pages until all animation is over.
	
	//private NotificationSystem notificationSystem;
	private NotificationIcons notificationIcons;	
	
	public ParticleSystem rewardIconGlow;
	public ParticleSystem rewardIconStars;	
	
	protected override void Awake() 
	{ 
		base.Awake();
		notificationIcons = gameObject.GetComponent<NotificationIcons>();
	}
	
	protected override void Start()
	{
       
		base.Start();
        RegisterEvent();
		
		//weeklyRoot.GetComponent<WeeklyRoot>().FillInObjectiveData();	// store data snapshot in each objective, for progress comparison
//		objectivesRoot.GetComponent<ObjectivesRoot>().FillInObjectiveData();	// store data snapshot in each objective, for progress comparison
//		statsRootCmpt = statsRoot.GetComponent<StatsRoot>();
		
	
	}

    protected override void RegisterEvent()
    {
        UIEventListener.Get(btnHome).onClick = OnMenuClicked;
        UIEventListener.Get(btnRunAgain).onClick = OnPlayClicked;
        UIEventListener.Get(btnShare).onClick = OnShareClicked;
        UIEventListener.Get (btnSpeedUp).onClick = OnSpeedUpButton;
    }
	

    void SetPosition()
    {
        //是否显示关卡结算//
        if(!GameController.SharedInstance.EndlessMode)
        {
            btnHome.GetComponent<UISprite>().spriteName = "post_home";
            btnRunAgain.transform.parent.SetLocalPositionY(-649f);
            postAcount.transform.SetLocalPositionY(99.0f);
            postAcount.level.SetActive(true);
        }
        else
        {
            btnHome.GetComponent<UISprite>().spriteName = "post_home";
            btnRunAgain.transform.parent.SetLocalPositionY(-440f);
            postAcount.transform.SetLocalPositionY(15.0f);
            postAcount.level.SetActive(false);
        }
        
    }

	public override void appear()
	{
        tutorialUpgrade.SetActive(false);
        UIDynamically.instance.ZoomZeroToOne(gameObject,0.3f);
        SetPosition();
		UIManagerOz.SharedInstance.PaperVC.SetCurrentPage(UIManagerOz.SharedInstance.postGameVC);
		base.appear();
		//rewardPanel.SetActive(false);
        //rewardPanel.transform.localPosition = new Vector3(0f, 800f, -100);
        //Services.Get<NotificationSystem>().SetNotificationIconsForThisPage(UiScreenName.POSTRUN);		
		//speedUpButton.collider.enabled = true;

	    //if (!GameController.SharedInstance.upgradeTutorialPlayed)
        if (GameController.SharedInstance.GetTutorialIDforSys() == 0)
	    {
	        btnRunAgain.SetActive(false);
            btnShare.SetActive(false);
            tutorialHome.SetActive(true);
	    }
	    else
        {
            btnRunAgain.SetActive(true);
            btnShare.SetActive(true);
            tutorialHome.SetActive(false);
	    }

        postAcount.SetAccount();
//		objectivesRoot.GetComponent<ObjectivesRoot>().SetRankProgressIcons(GameProfile.SharedInstance.Player.GetCurrentRankProgress());
//		UIManagerOz.SharedInstance.MainGameCamera.enabled = false;
	}
	
   
	public void ShowWeeklyChallengesPage()
	{
		canSwitchPages = false;
		NGUITools.SetActive(weeklyRoot, true);
		NGUITools.SetActive(objectivesRoot, false);
		NGUITools.SetActive(statsButton, false);
		SetWeeklyChallengesPagePosition();
		weeklyRoot.GetComponent<WeeklyRoot>().EnterWeeklyChallengesPage();
	}

	public void ShowObjectivesPage()
	{
        //canSwitchPages = false;
        //NGUITools.SetActive(weeklyRoot, false);
        //NGUITools.SetActive(objectivesRoot, true);
        //NGUITools.SetActive(statsButton, true);
        //SetObjectivesPagePosition(); // make sure it defaults to objectives page in case the last time we left it on stats page. no animation here
        //objectivesRoot.GetComponent<ObjectivesRoot>().EnterObjectivesPage();
	}
	

	
	public void ShowStatsPage() // just show stats page, with no animation .
	{
		notify.Debug("ShowStatsPage");
		float tweenTime = 0.2f;
		statsRoot.GetComponent<StatsRoot>().EnterStatsPage();
		MoveTweener(statsRoot, new Vector3(0.0f, statsRoot.transform.localPosition.y, 0.0f), tweenTime);
		MoveTweener(objectivesRoot, new Vector3(-1000.0f, objectivesRoot.transform.localPosition.y, 0.0f), tweenTime);
		//MoveTweener(statsButton, new Vector3(290.5f, statsButton.transform.localPosition.y, 0.0f), tweenTime);
		MoveTweener(statsButton, new Vector3(-290.5f, statsButton.transform.localPosition.y, statsButton.transform.localPosition.z), tweenTime);
		statsButton.transform.Find("Sprite").localRotation = Quaternion.Euler(0f, 0f, 180f);

//		iTween.RotateTo(statsButton,iTween.Hash(
//			"y",  0f,
//			"time", tweenTime,
//			"islocal", true));
		//paperViewController.SetToPostRunStats();
		statsShowing = true;
	}
	
	public void CanSwitchPages()
	{
		canSwitchPages = true;
		Services.Get<MenuTutorials>().SendEvent(4);	// pop up dialog if 1st time player has earned enough coins to purchase something
		GameController.SharedInstance.MenusEntered();
	}
	
	private void MoveTweener(GameObject obj, Vector3 pos, float time)
	{
		iTween.MoveTo(obj, iTween.Hash("isLocal", true, "position", pos, "time", time, "easetype", iTween.EaseType.easeOutSine));
	}
	
	private void SetWeeklyChallengesPagePosition()
	{
		notify.Debug("SetWeeklyChallengesPagePosition");
		weeklyRoot.transform.localPosition = new Vector3(0.0f, weeklyRoot.transform.localPosition.y, 0.0f);
		statsRoot.transform.localPosition  =  new Vector3(1000.0f, statsRoot.transform.localPosition.y, 0.0f);
		objectivesRoot.transform.localPosition = new Vector3(1000.0f, objectivesRoot.transform.localPosition.y, 0.0f);
		statsButton.transform.localPosition = new Vector3(295f, statsButton.transform.localPosition.y, statsButton.transform.localPosition.z);
		statsButton.transform.Find("Sprite").localRotation = Quaternion.Euler(0f, 0f, 0f);	//180f,0f);
		//paperViewController.SetToPostRun();
		statsShowing = false;
		
		// make sure bottomPanel is down
		//bottomPanel.transform.localPosition = -Vector3.up * 350f; - commented out for now because of nulls N.N.
		foreach (GameObject go in bottomPanelGOs)
			go.transform.localPosition = -Vector3.up * 350f;
	}	
	
	private void SetObjectivesPagePosition()
	{
		notify.Debug("SetObjectivesPagePosition");
		weeklyRoot.transform.localPosition = new Vector3(1000.0f, weeklyRoot.transform.localPosition.y, 0.0f);
		statsRoot.transform.localPosition  =  new Vector3(1000.0f, statsRoot.transform.localPosition.y, 0.0f);
		objectivesRoot.transform.localPosition = new Vector3(0.0f, objectivesRoot.transform.localPosition.y, 0.0f);
		statsButton.transform.localPosition = new Vector3(295f, statsButton.transform.localPosition.y, statsButton.transform.localPosition.z);
		statsButton.transform.Find("Sprite").localRotation = Quaternion.Euler(0f, 0f, 0f);	//180f,0f);
		//paperViewController.SetToPostRun();
		statsShowing = false;
		
		// make sure bottomPanel is down
		//bottomPanel.transform.localPosition = -Vector3.up * 350f; - commented out for now because of nulls N.N.
		foreach (GameObject go in bottomPanelGOs)
			go.transform.localPosition = -Vector3.up * 350f;
	}
	
	public void ShowBottomPanel()
	{
		/*
		foreach (GameObject go in bottomPanelGOs)
			iTween.MoveTo(go, iTween.Hash(
				"y", 0f,
				"islocal", true,
				"time", 0.5f,
				"easetype", iTween.EaseType.easeOutCubic));
				*/
		iTween.ValueTo(gameObject, iTween.Hash(
				"from", 0f,
				"to", 1f,
				"time", 0.3f,
				"easetype", iTween.EaseType.easeOutCubic,
				"onupdate", "ShowBottomPanelUpdate",
				"onupdatetarget", gameObject,
				"oncomplete", "CleanUpMemory",
				"oncompletetarget", gameObject		
				));
	}
	
	private void CleanUpMemory()
	{
		Resources.UnloadUnusedAssets();
		System.GC.Collect();
	}
	
	public void ShowBottomPanelUpdate(float val)
	{
		foreach (GameObject go in bottomPanelGOs)
			go.transform.localPosition = new Vector3(go.transform.localPosition.x, (1 - val) * -350f, go.transform.localPosition.z);
		
	
	}

#if UNITY_ANDROID	
	public void OnEscapeButtonClicked()
	{
		if (GameController.SharedInstance.IsSafeToLaunchDownloadDialog())	// only go back to menu if animations are completed
		{
			OnMenuClicked(gameObject);	// mapped to Android hardware 'back' button, in this case take us back to main menu
		}
		else
		{
			OnSpeedUpButton(gameObject);	// speed up animations
		}
	}		
#endif	
	
	public void OnEscapeButtonClickedModel()
	{
		if( rewardPanel.gameObject.activeSelf == false ) return ;
		
		if( UIManagerOz.escapeHandled ) return;
		UIManagerOz.escapeHandled = true;
		
		OnRewardButtonClicked();
	}		

	public void OnRewardButtonClicked()
	{
		notify.Debug ("OnRewardButtonClicked");
		CancelInvoke("HideRewardPanel");
		HideRewardPanel();
		//AnimateStatsIn();
		//objectivesRoot.GetComponent<ObjectivesRoot>().UpdateProgress();
		
	}
	
	public void AnimateStatsIn()
	{
		notify.Debug("AnimateStatsIn");
		statsRootCmpt.Reset();
		ShowStatsPage();
		statsRootCmpt.StartAnimSeqWithDelay(0.8f);
	}
	
	public bool ShowingRewardPanel { get; private set; }
	
	public void ShowRewardPanelDelay(float time)
	{
		ShowingRewardPanel = true;
		Invoke("ShowRewardPanel", time);
	}
	
	public void ShowRewardPanel()
	{
//		speedUpButton.collider.enabled = false;
		rewardPanel.transform.localPosition = new Vector3(0f, 800f, -100f);
		rewardPanel.SetActive(true);
		rewardIcon.alpha = 0f;
		rewardLabel.alpha = 0f;
		
		iTween.MoveTo(rewardPanel, iTween.Hash(
			"y", 0f,
			"islocal", true,
			"time", 0.3f,
			"easetype", iTween.EaseType.easeOutCubic,
			"oncomplete", "OnRewardPanelArrived",
			"oncompletetarget", gameObject
			));
		AudioManager.SharedInstance.PlayFX(AudioManager.Effects.oz_UI_Menu_back);
	}
	
	public void OnRewardPanelArrived()
	{
		TweenAlpha.Begin(rewardLabel.gameObject, 0.2f, 1f);
		iTween.ScaleFrom(rewardLabel.gameObject, iTween.Hash(
			"scale", rewardLabel.transform.localScale * 4f,
			"time", 0.3f,
			"oncomplete", "OnRewardLabelComplete",
			"ignoretimescale", true, 			
			"oncompletetarget", gameObject
			));
		
		TweenAlpha ta = TweenAlpha.Begin(rewardIcon.gameObject, 0.2f, 1f);
		ta.delay = 0.3f;
		
		iTween.ScaleFrom(rewardIcon.gameObject, iTween.Hash(
			"scale", rewardIcon.transform.localScale * 4f,
			"time", 0.3f,
			"delay", 0.3f,
			"oncomplete", "OnRewardIconComplete", 
			"ignoretimescale", true, 
			"oncompletetarget", gameObject
			));
		
		Invoke("HideRewardPanel", 8f);
	}
	
	public void OnRewardLabelComplete()
	{
		AudioManager.SharedInstance.PlayFX(AudioManager.Effects.MusicBox);
		ShakeRewardScreen();
		objectivesRoot.GetComponent<ObjectivesRoot>().ResetRank();
		//AudioManager.SharedInstance.PlayFX(AudioManager.Effects.oz_MagicWand_01);
	}
	
	//Balloon Icons?
	public void OnRewardIconComplete()
	{
		if(rewardIconGlow) 
		{
			rewardIconGlow.playbackSpeed = 1.0f/Time.timeScale;
			rewardIconGlow.Play();
		}
		
		if(rewardIconStars)
		{ 
			rewardIconStars.playbackSpeed = 1.0f/Time.timeScale;
			rewardIconStars.Play();			
		}
		
		//AudioManager.SharedInstance.PlayFX(AudioManager.Effects.oz_UI_Menu_levelMax);
		//AudioManager.SharedInstance.PlayFX(AudioManager.Effects.oz_MagicWand_01);
		//AudioManager.SharedInstance.PlayFX(AudioManager.Effects.oz_CrystalHit_03);
		AudioManager.SharedInstance.PlayFX(AudioManager.Effects.oz_firework_big);
		ShakeRewardScreen();
	}
	
	public void HideRewardPanel()
	{
		iTween.MoveTo(rewardPanel, iTween.Hash(
			"y", 800f,
			"islocal", true,
			"time", 0.3f,
			"easetype", iTween.EaseType.easeInCubic,
			"oncomplete", "HideRewardPanelComplete",
			"oncompletetarget", gameObject));
		
		AudioManager.SharedInstance.PlayFX(AudioManager.Effects.oz_UI_Menu_back);
	}
	
	public void HideRewardPanelComplete()
	{
		ShowingRewardPanel = false;
		rewardPanel.SetActive(false);
//		speedUpButton.collider.enabled = true;
		
		if(rewardIconGlow) 
		{
			rewardIconGlow.Stop();
			rewardIconGlow.Clear ();
		}
		
		if(rewardIconStars)
		{ 
			rewardIconStars.Stop();	
			rewardIconStars.Clear();
		}		
	}
	
	public void ShakeRewardScreen()
	{
		iTween.ShakePosition(rewardPanel, iTween.Hash(
			"amount", new Vector3(0.02f,0.02f,0f),
			"time", 0.2f,
			"isLocal", false));
	}
	
	public void OnStatsButtonClicked()
	{
		if (!canSwitchPages)
			return;
		
		float tweenTime = 0.2f;
		
		if (!statsShowing)	// slide in stats, slide out objectives	
		{
			//			AnalyticsInterface.LogNavigationActionEvent( "Stats", "Post Run", "Post Run-Stats" );
			
			statsRoot.GetComponent<StatsRoot>().EnterStatsPage();
			MoveTweener(statsRoot, new Vector3(0.0f, statsRoot.transform.localPosition.y, 0.0f), tweenTime);
			MoveTweener(objectivesRoot, new Vector3(-1000.0f, objectivesRoot.transform.localPosition.y, 0.0f), tweenTime);
			MoveTweener(statsButton, new Vector3(-295f, statsButton.transform.localPosition.y, statsButton.transform.localPosition.z), tweenTime);
			statsButton.transform.Find("Sprite").localRotation = Quaternion.Euler(0f, 0f, 180f);	//180f,0f);

//			iTween.RotateTo(statsButton,iTween.Hash(
//				"y",  0f,
//				"time", tweenTime,
//				"islocal", true));
			//paperViewController.SetToPostRunStats();
			statsShowing = true;
		}
		else				// slide in objectives, slide out stats	
		{
			//			AnalyticsInterface.LogNavigationActionEvent( "Objectives", "Post Run", "Post Run-Objectives" );
			
			MoveTweener(statsRoot, new Vector3(1000.0f, statsRoot.transform.localPosition.y, 0.0f), tweenTime);
			MoveTweener(objectivesRoot, new Vector3(0.0f, objectivesRoot.transform.localPosition.y, 0.0f), tweenTime);
			MoveTweener(statsButton, new Vector3(295f, statsButton.transform.localPosition.y, statsButton.transform.localPosition.z), tweenTime);
			statsButton.transform.Find("Sprite").localRotation = Quaternion.Euler(0f, 0f, 0f);	//180f,0f);

//			iTween.RotateTo(statsButton,iTween.Hash(
//				"y",  180f,
//				"time", tweenTime,
//				"islocal", true));
			//paperViewController.SetToPostRun();
			statsShowing = false;
		}
	}

    public void OnMenuClicked(GameObject obj) //OnHomeButtonClicked()
    {
        GamePlayer.SharedInstance.DespawnModel();
        UIManagerOz.SharedInstance.loadingVC.SwitchToMainUI();

#if UNITY_ANDROID
        // fix for issue on very slow android devices, when switching between post-run and main menu the particles can linger
        if (GameController.SharedInstance.GetDeviceGeneration() == GameController.DeviceGeneration.LowEnd)
            statsRoot.GetComponent<StatsRoot>().KillParticleEffect();
#endif
        //		UIManagerOz.SharedInstance.mainVC.appear();
        GameController.SharedInstance.gameState = GameState.PRE_RUN;

        if(GameController.SharedInstance.EndlessMode)
        {
            UIManagerOz.SharedInstance.idolMenuVC.appear(); //newnew
        }
        else
        {
           // if (GameController.SharedInstance.unlockrole2PosTutorialPlayed && !GameController.SharedInstance.unlockChallengeTutorialPlayed && GameProfile.SharedInstance.Player.GetIsChallengeUnlock()) //挑战教学
           if( GameController.SharedInstance.GetTutorialIDforSys() == 5 && GameProfile.SharedInstance.Player.GetIsChallengeUnlock())
            {
                UIManagerOz.SharedInstance.idolMenuVC.appear(); //newnew
            }
            else
            {
                UIManagerOz.SharedInstance.idolMenuVC.appear(); //newnew
                UIManagerOz.SharedInstance.idolMenuVC.disappear();
                UIManagerOz.SharedInstance.worldOfOzVC.appear();
            }
        }

        disappear();
    }


    public void OnUpgradesClicked()
	{
		//		AnalyticsInterface.LogNavigationActionEvent( "Store", "Post Run", "Store" );
		
	
//		UIManagerOz.SharedInstance.inventoryVC.appear();
//		UIManagerOz.SharedInstance.PaperVC.appear();
        UIManagerOz.SharedInstance.chaSelVC.appear();
       
		disappear();
	}	
	
    public override void disappear ()
    {
        base.disappear ();

        GamePlayer.SharedInstance.ClearOldPieces();
        GamePlayer.SharedInstance.transform.localPosition = GamePlayer.SharedInstance.StartingPosition;
        GamePlayer.SharedInstance.transform.forward = new Vector3(0,0,-1);
        
        Time.timeScale = 1.0f;      // so menus can animate correctly
        GameController.SharedInstance.DeSpawnTrack();
        TrackBuilder.SharedInstance.QueuedPiecesToAdd.Clear();
    }

	public void OnShareClicked(GameObject obj) //- commented out for now because of nulls N.N.
	{
        GamePlayer.SharedInstance.DespawnModel();
//        UIManagerOz.SharedInstance.loadingVC.SwitchToMainUI();
        
        #if UNITY_ANDROID
        // fix for issue on very slow android devices, when switching between post-run and main menu the particles can linger
        if (GameController.SharedInstance.GetDeviceGeneration() == GameController.DeviceGeneration.LowEnd)
            statsRoot.GetComponent<StatsRoot>().KillParticleEffect();
        #endif
        //      UIManagerOz.SharedInstance.mainVC.appear();
        GameController.SharedInstance.gameState = GameState.PRE_RUN;

        Invoke("ShowCharacterUpgrade",0.05f);

        disappear();

//        UIManagerOz.SharedInstance.okayDialog.ShowOkayDialog("Msg_Developing", "Btn_Ok");
//        #if UNITY_EDITOR
//        //        Application.CaptureScreenshot("Assets/screenshot"+Services.Get<AppCounters>().GetSecondsSpentInApp()+".png");
//        StartCoroutine(UIManagerOz.SharedInstance.inGameVC.CaptureScreenshot2(new Rect(0,0,Screen.width,Screen.height)));
//
//        #endif
	}

    void ShowCharacterUpgrade()
    {
        UIManagerOz.SharedInstance.chaSelVC.appear();
    }

	private void OnPlayClicked(GameObject obj) 
	{
        GamePlayer.SharedInstance.DespawnModel();
		if( DownloadManager.IsDownloadInProgress() )
		{// don't allow playing if a download is in progress
			// just bring up downloadng UI
			UIManagerOz.SharedInstance.StartDownloadPrompts( true, false, true, gameObject);
			return;
		}

	    StartCoroutine(ReStart());
//		UIManagerOz.SharedInstance.OnPlayClicked();
	}


    IEnumerator ReStart()
    {
        if(!UIManagerOz.SharedInstance.PaperVC.fuelSystem.IsFuelEnough())
        {
            UIManagerOz.SharedInstance.okayDialog.ShowOkayDialog("燃料不足","Btn_Ok");
            yield break;
        }
     
        yield return new WaitForFixedUpdate();

        if (!GameController.SharedInstance.EndlessMode)
        {
            yield return StartCoroutine(WaitOilEffect());
        }

       

        UIManagerOz.SharedInstance.idolMenuVC.appear(); //newnew
        UIManagerOz.SharedInstance.loadingVC.SwitchToMainUI();
        yield return new WaitForSeconds(0.2f);
        if (GameController.SharedInstance.EndlessMode)
        {
            UIManagerOz.SharedInstance.idolMenuVC.OnPlayClicked(gameObject);
            this.disappear();
        }
        else
        {
            UIManagerOz.SharedInstance.PaperVC.fuelSystem.AddFuelCount(-2);//消耗燃料
            UIManagerOz.SharedInstance.inventoryVC.appear();
            UIManagerOz.SharedInstance.idolMenuVC.disappear();
            this.disappear();
        }

    }
	
    private IEnumerator WaitOilEffect()
    {
        Vector3 fromPos = UIManagerOz.SharedInstance.PaperVC.oilPos.position;
        
        OilEffect.instance.PlayEffect(btnRunAgain.transform.position,fromPos);
        
        while(!OilEffect.instance.isPlayFinished)
        {
            yield return null;
        }

        yield break;
    }

	public void OnSpeedUpButton(GameObject obj)
	{
		Time.timeScale = 10f;
		speedUpButton.collider.enabled = false;	
	}
	
	public void SetNotificationIcon(int buttonID, int iconValue)		// update actual icon onscreen
	{
		notificationIcons.SetNotification(buttonID, iconValue);
	}
}
