using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public enum tutorialMessageType
{
    SUCESS,
    FAIL,
    MSG
}

public class UIInGameViewControllerOz : UIViewControllerOz
{
    public delegate void RetriggeringGemmingOfArtifact();

    public BonusButtons bonusButtons;
    public GameObject camerafade;
    public GameObject PerfectClearFX;
    public CoinMeter coinMeter;
    public UILabel countDownLabel;
    public UISpriteAnimationExt countDownSpriteAnimation;
    private float curDistanceRatio;
    private ObjectiveProtoData currLevelData;
    public UILabel debugDistanceLabel;
    public UIPanelAlpha envChangeHud = null;
    public GameObject envChangeHudArrow = null;
    public UILabel envChangeHudLabel = null;
    private int envprogfadecount;
    public UILabel envProgressLabel;
    public Transform envProgressMove = null;
    public UISlider envProgressSlider = null;
    //private float envProgressDistance;
    public UISprite envProgressSprite;
    private float estimatedDistLeft;
    //-- Tutorial
    //-- Fast Travel
    public FastTravelButton[] fastTravel = null;
    public UISlider FeverProgressBar;
    public ParticleSystem fx_coin;
    //private PopupNotification popupNotification;
    public HeadStart headStart, megaHeadStart;
    public bool isAbilityTutorialOn;
    private bool isPowerMeterFinger;
    private bool isUtilityTutorial;
    private float lastDistTravelled;
    public UILabel leveldesc, progressTxt;
    public UIShadow mUIShadowInstance;
    private int oldEarnedSateValue;
    public Transform passiveCountDown;
    public GameObject pauseButton;
    public PauseMenu pauseMenu;
    private bool PerfectClearTrigger = true;
    private Vector3 playerSpeedAbilityTutorial;
    public UISlider progressBar;
    public ResurrectMenu resurrectMenu;
    //public Transform activePowerupButton;
    public ScoreUI scoreUI;
    public bool ShowDebugDistance = true;
    public bool showingEnvProgress;
    public RetriggeringGemmingOfArtifact sourceArtifactMethod;
    public UISprite star1, star2, star3, bestProgressBar;
    private int starCountBest;
    public Teammate teamIcons;
    public UIPanelAlpha tutorialAbility;
    public Transform tutorialAbilityButton;
    private Vector3 tutorialAbilityButtonPos;
    private int tutorialAbilityCounter;
    public UISprite tutorialAbilityGem;
    public UILabel tutorialAbilityLabel;
    public GameObject tutorialAbilityRing;
    //public Material		tutorialRingMaterial;
    private int tutorialAbilityRingCount;
    public UISprite tutorialAbilityRingSprite;
    public Transform tutorialArrow = null;
    //-- Tutorial
    public GameObject tutorialArrowLabelPanel = null;
    public Transform tutorialAvoid = null;
    public UILabel tutorialAvoidLabel = null;
    public Transform tutorialBalloon = null;
    public UILabel tutorialBalloonLabel = null;
    public UIPanelAlpha tutorialCollectCoins = null;
    public Transform tutorialEnv = null;
    public UILabel tutorialEnvLabel = null;
    public UISprite tutorialFinger;
    private Vector3 tutorialFingerScale;
    public Transform tutorialFinley = null;
    public Transform tutorialHand = null;
    public Transform tutorialJump = null;
    public UILabel tutorialLabel = null;
    public Transform tutorialMeter = null;
    public UILabel tutorialMeterLabel = null;
    public Transform tutorialRoot = null;
    public Transform tutorialSlide = null;
    public UILabel tutorialSlideLabel = null;
    private TweenColor tutorialTC;
    public Transform tutorialTileIcon;
    public Transform tutorialTilt = null;
    public UILabel tutorialTiltLabel = null;
    private TweenPosition tutorialTP;
    private TweenScale tutorialTS;
    public Transform tutorialTurn = null;
    public UISprite wandVignette = null;

    protected override void Start()
    {
        base.Start();
        //popupNotification = Services.Get<PopupNotification>();

        tutorialAbilityButtonPos = tutorialAbilityButton.transform.localPosition;
        tutorialFingerScale = tutorialFinger.transform.localScale;
        //bonusButtons = GetComponent<BonusButtons>();
        //foreach(FastTravelButton ftb in fastTravel){
        //	ftb.ingameVc = this;
        //}
        //resurrectMenu.statsRoot = statsRoot;

//		Material mat = tutorialFinger.material;
//		Shader shader = mat.shader;
//		tutorialFinger.material = new Material(shader);
//		tutorialFinger.material.renderQueue += 30;
        debugDistanceLabel.text = "";
    }

    /*	public void OnGUI()
	{
		if (GUI.Button(new Rect(0,50,100,50), "Kill GUI"))
			NGUIToolsExt.SetActive(this.gameObject, false);
			//ShowInGameGUI(false);
	}*/

//	public void ShowInGameGUI(bool state)
//	{
//		NGUIToolsExt.SetActive(this.gameObject, false);
//	}

    public override void appear()
    {
        GameProfile.SharedInstance.Player.numberChanceTokensThisRun = 0; //
        scoreUI.SetTreasureBoxCount(0);
        UIDynamically.instance.LeftToScreen(scoreUI.gameObject, -100f, 225f, 0.5f);
        UIDynamically.instance.LeftToScreen(FeverProgressBar.transform.parent.gameObject, -450f, -307f, 0.5f);
        UIDynamically.instance.LeftToScreen(coinMeter.gameObject, -100f, 162f, 0.5f);
        UIDynamically.instance.LeftToScreen(pauseButton, -450f, -286f, 0.5f);
        UIDynamically.instance.LeftToScreen(teamIcons.gameObject, 450f, 205f, 0.5f);

        iTween.ColorTo(camerafade, Color.black, 0.5f);

        //可进入三星通关触发器
        PerfectClearTrigger = true;

        progressBar.value = 0f;
        if (ObjectivesManager.LevelObjectives.Count > GameProfile.SharedInstance.Player.activeLevel)
            currLevelData = ObjectivesManager.LevelObjectives[GameProfile.SharedInstance.Player.activeLevel];
        if (currLevelData != null)
        {
            currLevelData._conditionList[0]._statValue = 0;
            oldEarnedSateValue = currLevelData._conditionList[0]._earnedStatValue;
            starCountBest = UIManagerOz.SharedInstance.worldOfOzVC.GetStarRank(currLevelData);
            UIManagerOz.SharedInstance.worldOfOzVC.oldStarCountBest = starCountBest;
            UIManagerOz.SharedInstance.worldOfOzVC.SetStarPosition(progressBar.foregroundWidget,star1.gameObject,star2.gameObject,star3.gameObject);
            
        }

        debugDistanceLabel.text = "";
        coinMeter.appear();
        NGUIToolsExt.SetActive(gameObject, true);
        envChangeHud.gameObject.SetActive(false);
        ResetInGameGUI();
        UIManagerOz.SharedInstance.PaperVC.SetCurrentPage(UIManagerOz.SharedInstance.inGameVC);
        coinMeter.ActivePowerFX.SetActive(false);
        PerfectClearFX.SetActive(false);
        if (!GameController.SharedInstance.EndlessMode)
        {
            ShowLevelTips();
            
        }
        else
        {
            leveldesc.transform.parent.gameObject.SetActive(false);
            star1.transform.parent.gameObject.SetActive(false);
            leveldesc.transform.parent.localPosition = new Vector3(0f, 800f, 0f);
            star1.transform.parent.localPosition = new Vector3(0f, -800f, 0f);
        }
    }

    public override void disappear() //bool hidePaper = false) 
    {
        pauseMenu.HidePauseMenu(); //NGUIToolsExt.SetActive(UIPauseMenu, false);
        scoreUI.ResetCurrencyBars(); //lastScoreXOffset = 0.0f;
        NGUIToolsExt.SetActive(gameObject, false);
        //base.disappear();				// don't fade out alpha
    }

    public void ResetInGameGUI()
    {
        countDownLabel.gameObject.SetActive(false);

        resurrectMenu.hideResurrectMenu(); //.disableResurrectMenu();
        scoreUI.ResetCurrencyBars(); //lastScoreXOffset = 0.0f;

        pauseMenu.HidePauseMenu();
        HideTutorial();
        ResetEnvProgress();
        bonusButtons.bottom.SetActive(false);
        ExpandPeripheralUIelementsBackToNormalAfterPause();

        foreach (var ftb in fastTravel)
        {
            ftb.Hide();
        }

        teamIcons.Show();
        bonusButtons.ShowSpeedLine(false);

        NGUITools.SetActiveChildren(mUIShadowInstance.gameObject, false);
    }

    public void ShowLevelTips()
    {
//        leveldesc.transform.parent.gameObject.SetActive(true);
        star1.transform.parent.gameObject.SetActive(true);
        var mdata = ObjectivesManager.LevelObjectives[GameProfile.SharedInstance.Player.activeLevel];

        var starCount = UIManagerOz.SharedInstance.worldOfOzVC.GetStarRank(mdata);
//        int val;
//        if (starCount < 1)
//        {
//            val = mdata._conditionList[0]._statValue1ForLevel;
//        }
//        else if (starCount >= 1 && starCount < 2)
//        {
//            val = mdata._conditionList[0]._statValue2ForLevel;
//        }
//        else
//        {
//            val = mdata._conditionList[0]._statValue3ForLevel;
//        }
//
//        leveldesc.text = string.Format(mdata._descriptionPreEarned, val);

        UIManagerOz.SharedInstance.worldOfOzVC.UpdateStarSprite(star1, star2, star3, starCount);
        UIManagerOz.SharedInstance.worldOfOzVC.UpdateStarProgressBar(bestProgressBar, mdata);

        //var showPos = new Vector3(0f, 255f, 0f);

        var barShowPos = new Vector3(0f, -605f, 0f);

//        //显示提示
//        iTween.MoveTo(leveldesc.transform.parent.gameObject, iTween.Hash(
//            "position", showPos,
//            "islocal", true,
//            "time", 0.5f,
//            "ignoretimescale", true
//            ));
        //显示进度
        iTween.MoveTo(star1.transform.parent.gameObject, iTween.Hash(
            "position", barShowPos,
            "islocal", true,
            "time", 0.5f,
            "ignoretimescale", true
            ));
        Invoke("HideLevelTips", 2f);
    }

    private void HideLevelTips()
    {
        var hidePos = new Vector3(0f, 800f, 0f);
//        Vector3 barHidePos = new Vector3(0f,-800f,0f);
        iTween.MoveTo(leveldesc.transform.parent.gameObject, iTween.Hash(
            "position", hidePos,
            "islocal", true,
            "time", 0.5f,
            "ignoretimescale", true
            ));
    }

   
    public void SetDistanceTotal(int distanceTotal)
    {
//expensive on 3GS
        if (ShowDebugDistance && Settings.GetBool("console-enabled", false))
        {
            debugDistanceLabel.text = "Dist: " + distanceTotal + "  Spd: " +
                                      GameController.SharedInstance.velocityMagnitude;
        }
    }

    public void StopShowingDebugDistance()
    {
        ShowDebugDistance = false;
        if (Settings.GetBool("console-enabled", false))
        {
            debugDistanceLabel.text = "";
        }
    }

    public void SetCountDownNumber(int number)
    {
        if (number < 0 || number >= 4)
        {
            countDownSpriteAnimation.ResetToBeginning();
            countDownLabel.gameObject.SetActive(false);
            coinMeter.UnPause();

            return;
        }
        //countDownSprite.enabled = true;
        //countDownSprite.spriteName = number.ToString();	
        countDownLabel.gameObject.SetActive(true);
        countDownLabel.text = number.ToString();
        coinMeter.Pause();
    }

    public void HidePauseButton()
    {
        NGUIToolsExt.SetActive(pauseButton, false);
    }

    public void ShowPauseButton()
    {
        NGUIToolsExt.SetActive(pauseButton, true);
    }

    public void OnUnPaused(GameObject obj)
    {
        notify.Debug("OnUnPaused");

        coinMeter.UnPause();
        pauseMenu.HidePauseMenu();
        //	NGUIToolsExt.SetActive(pauseButton, true);	

        UIManagerOz.SharedInstance.Unpause();
            //.onUnPauseClicked();									//-- Notify an object that is listening for this event.
    }

    public void OnEscapeButtonClicked()
    {
        if (UIManagerOz.escapeHandled)
            return;

        UIManagerOz.escapeHandled = true;

        if (pauseButton.activeSelf && Time.timeScale != 0f) //!isAbilityTutorialOn)
            OnPause();
        else if (pauseMenu.LevelhomeButton.activeSelf && pauseMenu.EndlesshomeButton.activeSelf)
            OnUnPaused(gameObject);
//			OnHomeButtonClicked();
    }

    public void OnPause()
    {
        notify.Debug("OnPause");

        if (GamePlayer.SharedInstance.Dying || GamePlayer.SharedInstance.IsDead ||
            GameController.SharedInstance.IsPaused)
        {
            return;
        }

        NGUIToolsExt.SetActive(pauseButton, false); // disable in-game UI elements
        coinMeter.Pause();
        pauseMenu.ShowPauseMenu();

        ShrinkPeripheralUIelementsDuringPause(); // shrink tutorial to get it out of the way, if tutorial is active

        UIManagerOz.SharedInstance.Pause();
            //.onPauseClicked();									//-- Notify an object that is listening for this event.
        AudioManager.SharedInstance.StopFX();
        AudioManager.SharedInstance.StopStumbleProof();
        if (gameObject.activeSelf)
            StartCoroutine("CleanUpMemory");
                // take this fine opportunity to clean up memory in-game, since we've just paused
    }

    private IEnumerator CleanUpMemory()
    {
        yield return null; // wait until next frame, so pause menu gets drawn first
        Resources.UnloadUnusedAssets();
        GC.Collect();
    }

    public void OnOutOfGemsPause(RetriggeringGemmingOfArtifact source)
        // triggered when trying to gem a utility without enough gems
    {
        notify.Debug("OnOutOfGemsPause");

        sourceArtifactMethod = source;

        NGUIToolsExt.SetActive(pauseButton, false); // disable in-game UI elements
        coinMeter.Pause();
        //pauseMenu.ShowPauseMenu();	

        ShrinkPeripheralUIelementsDuringPause(); // shrink tutorial to get it out of the way, if tutorial is active

        UIManagerOz.SharedInstance.Pause();
            //.onPauseClicked();									//-- Notify an object that is listening for this event.

        UIConfirmDialogOz.onNegativeResponse += OnNeedMoreGemsNoInGame;
        UIConfirmDialogOz.onPositiveResponse += OnNeedMoreGemsYesInGame;

        UIManagerOz.SharedInstance.confirmDialog.ShowConfirmDialog("Lbl_Dialogue_MoreGems_Prompt", "Btn_No", "Btn_Yes");
    }

    public void OnNeedMoreGemsNoInGame()
        // use in-game only, when choosing to not go to mini store.  Used on continue/resurrect screen.
    {
        UIConfirmDialogOz.onNegativeResponse -= OnNeedMoreGemsNoInGame;
        UIConfirmDialogOz.onPositiveResponse -= OnNeedMoreGemsYesInGame;

        if (GameProfile.SharedInstance.Player.GetGemCount() <= 0)
            UIManagerOz.SharedInstance.inGameVC.sourceArtifactMethod = null;
                // kill attempt to gem the ability, since didn't buy any gems

        //if you don't want any gems, just continue with run.
        OnUnPaused(gameObject);
    }

    public void OnNeedMoreGemsYesInGame() // use in-game only, goes to mini store.  Used on continue/resurrect screen.
    {
        UIConfirmDialogOz.onNegativeResponse -= OnNeedMoreGemsNoInGame;
        UIConfirmDialogOz.onPositiveResponse -= OnNeedMoreGemsYesInGame;

        UIManagerOz.SharedInstance.GoToMiniStore(ShopScreenName.Gems, false);
            // "gems");	// send player to in-game mini store, gems page
    }

    public void OnHomeButtonClicked(GameObject obj)
    {
        if (Settings.GetBool("activate-quit-game-button", true))
        {
            UIConfirmDialogOz.onNegativeResponse += DisconnectHandlers;
            UIConfirmDialogOz.onPositiveResponse += ExitGame;
            //UIManagerOz.SharedInstance.confirmDialog.ShowConfirmDialog("Msg_LeaveGame","", "Btn_No", "Btn_Yes");
            UIManagerOz.SharedInstance.confirmDialog.ShowConfirmDialog("Msg_QuitGame", "Btn_No", "Btn_Yes");
        }
    }

    // --- jonoble: Temporary fix ---------------------------------------------
    public void OnButtonClick()
    {
        // TODO: Edit "InGameEdit.unity" and change the UIButton's function name
        // from "OnButtonClick()" to "OnShareButtonClicked()"
        OnShareButtonClicked();
    }

    // ------------------------------------------------------------------------

    public void OnShareButtonClicked()
    {
#if UNITY_EDITOR
//        Application.CaptureScreenshot("Assets/screenshot.png");
        StartCoroutine(CaptureScreenshot2(new Rect(0, 0, Screen.width, Screen.height)));
        Debug.LogError("screenshot");
#endif
    }

    public IEnumerator CaptureScreenshot2(Rect rect)
    {
        yield return new WaitForEndOfFrame();
        // 先创建一个的空纹理，大小可根据实现需要来设置  
        var screenShot = new Texture2D((int) rect.width, (int) rect.height, TextureFormat.RGB24, false);

        // 读取屏幕像素信息并存储为纹理数据，  
        screenShot.ReadPixels(rect, 0, 0);
        screenShot.Apply();

        // 然后将这些纹理数据，成一个png图片文件  
        var bytes = screenShot.EncodeToPNG();
        var filename = Application.dataPath + "/Screenshot.png";
        File.WriteAllBytes(filename, bytes);
        Debug.Log(string.Format("截屏了一张图片: {0}", filename));

        // 最后，我返回这个Texture2d对象，这样我们直接，所这个截图图示在游戏中，当然这个根据自己的需求的。  
//        return screenShot;  
    }

    public void DisconnectHandlers()
    {
        UIConfirmDialogOz.onNegativeResponse -= DisconnectHandlers;
        UIConfirmDialogOz.onPositiveResponse -= ExitGame;
    }

    public void ExitGame()
    {
        //AnalyticsInterface.LogNavigationActionEvent( "Menu", "Pause Menu", "Main Menu" );

        DisconnectHandlers();

        // clear background with color similar to menu UI background, eliminate flashing between screens		
        UIManagerOz.SharedInstance.SetUICameraClearFlagToSolidColorBG(true);
        OzGameCamera.OzSharedInstance.SwitchView(GameMode.Normal);
        GameController.SharedInstance.Player.DespawnModel();
        GameController.SharedInstance.Player.Reset();
        GameController.SharedInstance.DeSpawnTrack();
        //GameController.SharedInstance.ResetLevelInformation();
        //GameController.SharedInstance.ShowStartingTemple();	

        Time.timeScale = 1.0f; // so menus can animate correctly

        UIManagerOz.SharedInstance.PaperVC.goBackToIdolMenu = true;
//		UIManagerOz.SharedInstance.mainVC.appear();
        GameController.SharedInstance.gameState = GameState.PRE_RUN;
        UIManagerOz.SharedInstance.idolMenuVC.appear(); //newnew
        UIManagerOz.SharedInstance.MainGameCamera.enabled = false;
        UIManagerOz.SharedInstance.PaperVC.appear();
        disappear();
    }

    public void OnDiePostGame()
    {
        notify.Debug("OnDiePostGame1");
        //		PurchaseUtil.bIAnalysisWithParam("Game_Distance","Best_Distance|"+GameProfile.SharedInstance.Player.bestDistanceScore);
        //PurchaseUtil.bIAnalysisWithParam("Player_Coins","Total_Coins|"+GameProfile.SharedInstance.Player.coinCount);
        //PurchaseUtil.bIAnalysisWithParam("Player_Gems","Total_Gems|"+GameProfile.SharedInstance.Player.specialCurrencyCount);

        //if (resurrectMenu.chooseToResurrect == true) { return; }

        //Moving this to the BEGINNING of the game, rather than the end (has to happen after the DistanceTraveled is reset)
        //popupNotification.ResetDistance();					//MessageBoardLastDistance = 0;

        ObjectivesRoot.playAnimations = true; // animate in objectives in post-run screen

        AudioManager.SharedInstance.StopFX(true);

        AudioManager.SharedInstance.FadeMusicMultiplier(0.0f, 0.0f);
        //AudioManager.SharedInstance.StartMainMenuMusic(0.2f);

        //Update challenges and user profile to reflect full run.
        var challengeUpdater = Services.Get<ChallengeDataUpdater>();
        challengeUpdater.SetChallengeData();

        /*
		ProfileManager userProfile = Services.Get<ProfileManager>();
		userProfile.UpdateProfile();
		*/

        coinMeter.AnimateCoinMeter(0.01f, 0f);

        //ProfileManager.SharedInstance.UpdateProfile();

        /* eyal moving this to statsRoot so it is called after Gatcha score is added
#if UNITY_IPHONE
		GameCenterBinding.reportScore((System.Int64)GamePlayer.SharedInstance.Score, GameController.Leaderboard_HighScores);
		GameCenterBinding.reportScore((System.Int64)((int)GameController.SharedInstance.DistanceTraveled), GameController.Leaderboard_DistanceRun);
#elif UNITY_ANDROID
		//TODO add GameCircle here
#endif
		
		*/
        GameProfile.SharedInstance.UpdateCoinsPostSession(GamePlayer.SharedInstance.CoinCountTotal, false);
        GameProfile.SharedInstance.UpdateMedalPostSession(GamePlayer.SharedInstance.MedalCountTotal);
        GameProfile.SharedInstance.UpdateMedalSilverPostSession(GamePlayer.SharedInstance.MedalSilverCountTotal);
        GameProfile.SharedInstance.UpdateMedalGoldPostSession(GamePlayer.SharedInstance.MedalGoldCountTotal);

        var starCount = UIManagerOz.SharedInstance.worldOfOzVC.GetCurLevelStarCount();
        if (GameProfile.SharedInstance.Player.GetNumberChanceTokens() > 0 &&
            starCount < WorldOfOzCellData.PerfectClearStar) // if we get chance tokens lets show the gatcha first
        {
            NGUIToolsExt.SetActive(gameObject, false);
            UIManagerOz.SharedInstance.gatchVC.availableCards =
                GameProfile.SharedInstance.Player.GetNumberChanceTokens();
            UIManagerOz.SharedInstance.gatchVC.appear();
        }
        else
        {
            UIManagerOz.SharedInstance.postGameVC.appear();

            notify.Debug(string.Format("[UIInGameViewControllerOz] - OnDiePostGame.  Weekly Objective count: {0}",
                WeeklyObjectives.WeeklyObjectiveCount));

            var testWeeklyDisplay = Settings.GetBool("always-display-weekly-postrun", false);

            //if (true && WeeklyObjectives.WeeklyObjectiveCount > 0 ) // && ObjectivesDataUpdater.AreAnyWeeklyChallengesCompleted()) // TODO Remove the true, and the WeeklyObjectiveCount.  AreAnyWeeklyChallengesCompleted will do the check for if any challenges exist.
            if (testWeeklyDisplay || ObjectivesDataUpdater.AreAnyWeeklyChallengesCompleted())
            {
                UIManagerOz.SharedInstance.postGameVC.ShowWeeklyChallengesPage();
            }
            else
            {
                UIManagerOz.SharedInstance.postGameVC.ShowObjectivesPage();
            }

            // clear background with color similar to menu UI background, eliminate flashing between screens			
            UIManagerOz.SharedInstance.SetUICameraClearFlagToSolidColorBG(true);

            disappear(); //true);
        }
    }

    public void ShowMagicWandVignette()
    {
        notify.Warning("Calling show magic wand");
        notify.Warning(wandVignette.gameObject.name);

        wandVignette.enabled = true;
        //wandVignette.localPosition = new Vector3(0, 0, 0);
        //wandVignette.localRotation = Quaternion.Euler(new Vector3(0,0,90));
        //TweenPosition.Begin(wandVignette.gameObject, 1.0f, new Vector3(-Screen.width, wandVignette.localPosition.y, wandVignette.localPosition.z));
        //if(tutorialTC != null) {
        //tutorialTC.color = Color.white;	
        //}
        //tutorialTC = TweenColor.Begin (tutorialArrow.gameObject, 0.5f, new Color(1,1,1,0));
        //tutorialTC.delay = 0.5f;

        //AudioManager.SharedInstance.PlayFX(AudioManager.Effects.oz_UI_Menu_back);
    }

    public void ExpandPeripheralUIelementsBackToNormalAfterPause() // make them reappear
    {
        tutorialRoot.localScale = Vector3.one;
        bonusButtons.gameObject.transform.localScale = Vector3.one;

        foreach (var button in fastTravel)
            button.gameObject.transform.localScale = Vector3.one;

        //UIManagerOz.SharedInstance.popupNotification.transform.localScale = Vector3.one;
        //UIManagerOz.SharedInstance.popupNotificationObjectives.transform.localScale = Vector3.one;
    }

    public void ShrinkPeripheralUIelementsDuringPause() // make them disappear
    {
        tutorialRoot.localScale = Vector3.zero;
        bonusButtons.gameObject.transform.localScale = Vector3.zero;

        foreach (var button in fastTravel)
            button.gameObject.transform.localScale = Vector3.zero;

        //UIManagerOz.SharedInstance.popupNotification.transform.localScale = Vector3.zero;
        //UIManagerOz.SharedInstance.popupNotificationObjectives.transform.localScale = Vector3.zero;		
    }

    public void HideTutorial()
    {
        NGUIToolsExt.SetActive(tutorialArrow.gameObject, false);
        NGUIToolsExt.SetActive(tutorialRoot.gameObject, false);
    }

    public UIPanelAlpha ShowFinleyInstruction()
    {
        NGUIToolsExt.SetActive(tutorialRoot.gameObject, false);
        tutorialRoot.gameObject.SetActive(true);
        NGUIToolsExt.SetActive(tutorialFinley.gameObject, true);
        return tutorialFinley.GetComponent<UIPanelAlpha>();
    }

    public UIPanelAlpha ShowJumpInstruction()
    {
        //Debug.Log("ShowJumpInstruction");
        NGUIToolsExt.SetActive(tutorialRoot.gameObject, false);
        tutorialRoot.gameObject.SetActive(true);
        NGUIToolsExt.SetActive(tutorialJump.gameObject, true);
        return tutorialJump.GetComponent<UIPanelAlpha>();
    }

    public UIPanelAlpha ShowTurnInstruction()
    {
        NGUIToolsExt.SetActive(tutorialRoot.gameObject, false);
        tutorialRoot.gameObject.SetActive(true);
        NGUIToolsExt.SetActive(tutorialTurn.gameObject, true);
        return tutorialTurn.GetComponent<UIPanelAlpha>();
    }

    public UIPanelAlpha ShowSlideInstruction()
    {
        NGUIToolsExt.SetActive(tutorialRoot.gameObject, false);
        tutorialRoot.gameObject.SetActive(true);
        NGUIToolsExt.SetActive(tutorialSlide.gameObject, true);
        return tutorialSlide.GetComponent<UIPanelAlpha>();
    }

    public UIPanelAlpha ShowAvoidInstruction()
    {
        NGUIToolsExt.SetActive(tutorialRoot.gameObject, false);
        tutorialRoot.gameObject.SetActive(true);
        NGUIToolsExt.SetActive(tutorialAvoid.gameObject, true);
        return tutorialAvoid.GetComponent<UIPanelAlpha>();
    }

    public UIPanelAlpha ShowTiltInstruction(bool balloon = false)
    {
        NGUIToolsExt.SetActive(tutorialRoot.gameObject, false);
        tutorialRoot.gameObject.SetActive(true);
        NGUIToolsExt.SetActive(tutorialTilt.gameObject, true);
        if (balloon)
        {
            tutorialTiltLabel.text = Localization.SharedInstance.Get("Tut_AvoidCrystals");
        }
        else
        {
            tutorialTiltLabel.text = Localization.SharedInstance.Get("Tut_TiltToCollect");
        }
        tutorialTileIcon.localRotation = Quaternion.Euler(0f, 0f, -20f);
        iTween.RotateTo(tutorialTileIcon.gameObject, iTween.Hash(
            "z", 20f,
            "islocal", true,
            "time", 0.5f,
            "easetype", iTween.EaseType.easeInOutCirc,
            "looptype", iTween.LoopType.pingPong
            ));
        return tutorialTilt.GetComponent<UIPanelAlpha>();
    }

    public UIPanelAlpha ShowEnvInstruction()
    {
        notify.Debug("ShowEnvInstruction");
        NGUIToolsExt.SetActive(tutorialRoot.gameObject, false);
        tutorialRoot.gameObject.SetActive(true);
        NGUIToolsExt.SetActive(tutorialEnv.gameObject, true);
        return tutorialEnv.GetComponent<UIPanelAlpha>();
    }

    public UIPanelAlpha ShowMeterInstruction()
    {
        NGUIToolsExt.SetActive(tutorialRoot.gameObject, false);
        tutorialRoot.gameObject.SetActive(true);
        NGUIToolsExt.SetActive(tutorialMeter.gameObject, true);
        SetPowerMeterMessage();
        return tutorialMeter.GetComponent<UIPanelAlpha>();
    }

    public UIPanel ShowSwipeInstruction()
    {
        tutorialRoot.gameObject.SetActive(true);
        tutorialHand.gameObject.SetActive(true);

        return tutorialRoot.GetComponent<UIPanel>();
    }

    public void SetPowerMeterMessage()
    {
        if (tutorialMeterLabel == null)
            return;

        NGUIToolsExt.SetActive(tutorialMeterLabel.gameObject, true);
        var ts = TweenScale.Begin(tutorialMeter.gameObject, 0.25f, tutorialMeter.transform.localScale);
        if (ts)
        {
//			ts.onFinished += OnPart1PowerMeterLabel;
//			ts.onFinished.Add(new EventDelegate(OnPart1PowerMeterLabel));
            EventDelegate.Add(ts.onFinished, OnPart1PowerMeterLabel);
        }

        StartPowerMeterFinger();
    }

    public void StartPowerMeterFinger()
    {
        //Debug.Log ("StartPowerMeterFinger");
        // this bit will be the finger tapping
        if (isPowerMeterFinger)
            return;
        isPowerMeterFinger = true;

        NGUITools.SetActive(tutorialRoot.gameObject, false);
        tutorialRoot.gameObject.SetActive(true);
        tutorialAbility.transform.parent.gameObject.SetActive(true);

        tutorialAbility.gameObject.SetActive(true);
        tutorialAbility.transform.localPosition = Vector3.right*2000f;

        tutorialFinger.gameObject.SetActive(true);
        tutorialFinger.transform.localRotation = Quaternion.Euler(0f, 0f, 50f);
        tutorialFinger.transform.position = new Vector3(0.1f, -0.1f, 0.1f);
        //tutorialFinger.transform.localPosition = new Vector3(0f,-150f,0f);
        //tutorialAbilityRing.renderer.material = tutorialRingMaterial;
        //tutorialAbilityRing.renderer.material.SetColor("_Color", new Color(1f,0f,0f,0f));
        //tutorialAbilityRing.renderer.enabled = false;
        tutorialAbilityRing.gameObject.SetActive(true);
        tutorialAbilityRingSprite.alpha = 0f;
        tutorialAbilityRing.transform.position = tutorialFinger.transform.position + Vector3.forward*0.1f;
        tutorialAbility.gameObject.SetActive(true);
        var ta = TweenAlpha.Begin(tutorialAbility.gameObject, 0.6f, 1f);
        EventDelegate.Add(ta.onFinished, ShowFingerFx);
    }

    private void OnPart1PowerMeterLabel()
    {
        GamePlayer.SharedInstance.AddPointsToPowerMeter(1000);
//		tween.onFinished -= OnPart1PowerMeterLabel;
        EventDelegate.Remove(UITweener.current.onFinished, OnPart1PowerMeterLabel);
    }

    public void ShowCollectCoinsTutorial()
    {
        notify.Debug("ShowCollectCoinsTutorial");
        tutorialCollectCoins.alpha = 0f;
        tutorialCollectCoins.gameObject.SetActive(true);
        var ta = TweenAlpha.Begin(tutorialCollectCoins.gameObject, 0.5f, 1f);
        EventDelegate.Add(ta.onFinished, OnShowCollectCoinsTutorial);
    }

    public void OnShowCollectCoinsTutorial()
    {
//		tween.onFinished -= OnShowCollectCoinsTutorial;
        EventDelegate.Remove(UITweener.current.onFinished, OnShowCollectCoinsTutorial);
        StartCoroutine(HideCollectCoinsTutorial());
    }

    private IEnumerator HideCollectCoinsTutorial()
    {
        yield return new WaitForSeconds(2f);
        TweenAlpha.Begin(tutorialCollectCoins.gameObject, 0.5f, 0f);
    }

    public UIPanelAlpha ShowBalloonInstruction()
    {
        notify.Debug("ShowBalloonInstruction");
        NGUIToolsExt.SetActive(tutorialRoot.gameObject, false);
        tutorialRoot.gameObject.SetActive(true);
        NGUIToolsExt.SetActive(tutorialBalloon.gameObject, true);
        return tutorialBalloon.GetComponent<UIPanelAlpha>();
    }

    public void ShowTutorialMessage(string message, tutorialMessageType type = tutorialMessageType.SUCESS)
    {
        if (tutorialLabel == null)
            return;

        tutorialLabel.text = message;
        tutorialLabel.MakePixelPerfect();
        tutorialLabel.gameObject.SetActive(true);

        var normalScale = new Vector3(1f, 1f, 1.0f);
        //tutorialLabel.transform.localScale = new Vector3(0.5f, 0.5f, 1.0f);
        tutorialLabel.transform.localScale = normalScale*0.5f;
        normalScale *= 2f;
        //normalScale = new Vector3(2f, 2f, 1.0f);
        var time = 1f;

        //TweenScale.Begin(tutorialLabel.gameObject, time, normalScale);
        iTween.ScaleTo(tutorialLabel.gameObject, iTween.Hash(
            "scale", normalScale,
            "time", time
            ));

        if (type == tutorialMessageType.FAIL)
        {
            tutorialLabel.transform.localPosition = new Vector3(0, 0, 0);
            //TweenPosition.Begin(tutorialLabel.gameObject, time, new Vector3(0, 0, 0));
            iTween.MoveTo(tutorialLabel.gameObject, iTween.Hash(
                "position", new Vector3(0, 0, 0),
                "time", time,
                "isLocal", true
                ));
        }
        else if (type == tutorialMessageType.SUCESS)
        {
            /*
			float y = 960f;
			if(Screen.height <= 480){
				y = 480;
			}
			*/
            tutorialLabel.transform.localPosition = new Vector3(0, 400, 0);
            iTween.MoveTo(tutorialLabel.gameObject, iTween.Hash(
                "position", new Vector3(0, 250, 0),
                "time", time,
                "isLocal", true
                ));
        }
        else if (type == tutorialMessageType.MSG)
        {
            time = 2f;
            tutorialLabel.transform.localPosition = new Vector3(0, 400, 0);
            iTween.MoveTo(tutorialLabel.gameObject, iTween.Hash(
                "position", new Vector3(0, 250f, 0),
                "time", time,
                "isLocal", true
                ));
        }

        //tutorialLabel.color = Color.white;
        tutorialLabel.alpha = 1f;
//		TweenColor tc =  TweenColor.Begin(tutorialLabel.gameObject, time, new Color(1f,1f,1f,0f));
//		tc.delay = time;
//		tc.onFinished += OnHideTutorialMessage;
        iTween.ValueTo(tutorialLabel.gameObject, iTween.Hash(
            "from", 1f,
            "to", 0f,
            "time", time,
            "delay", time,
            "onupdate", "OnTutorialMessageUpdate",
            "onupdatetarget", gameObject
            ));

        AudioManager.SharedInstance.PlayFX(AudioManager.Effects.oz_Apprentice_01);
    }

    public void OnTutorialMessageUpdate(float val)
    {
        //tutorialLabel.alpha = val;
        if (val <= 0f)
        {
            NGUIToolsExt.SetActive(tutorialLabel.gameObject, false);
            GameController.SharedInstance.canShowNextTutorialStep = true;
            if (!GameController.SharedInstance.IsTutorialMode)
            {
                UIManagerOz.SharedInstance.inGameVC.tutorialRoot.gameObject.SetActive(false);
            }
        }
    }

    /*
	void OnHideTutorialMessage(UITweener tween) {
		tween.onFinished -= OnHideTutorialMessage;
		NGUIToolsExt.SetActive(tutorialLabel.gameObject, false);
		GameController.SharedInstance.canShowNextTutorialStep = true;
	}
	*/

    public void ShowSwipeLeft()
    {
        if (tutorialTP != null)
        {
            if (tutorialTP.enabled)
                return;
        }

        tutorialArrowLabelPanel.SetActive(true);
        NGUIToolsExt.SetActive(tutorialArrow.gameObject, true);
        tutorialArrow.localPosition = new Vector3(0, 0, 0);
        tutorialArrow.localRotation = Quaternion.Euler(new Vector3(0, 0, 90));
        //tutorialTP = TweenPosition.Begin(tutorialArrow.gameObject, 1.0f, new Vector3(-Screen.width, tutorialArrow.localPosition.y, tutorialArrow.localPosition.z));
        tutorialTP = TweenPosition.Begin(tutorialArrow.gameObject, 1.0f,
            new Vector3(-900, tutorialArrow.localPosition.y, tutorialArrow.localPosition.z));
        if (tutorialTC != null)
        {
            tutorialTC.value = Color.white;
        }
        tutorialTC = TweenColor.Begin(tutorialArrow.gameObject, 0.5f, new Color(1, 1, 1, 0));
        tutorialTC.delay = 0.5f;

        //AudioManager.SharedInstance.PlayFX(AudioManager.Effects.oz_turnleft_ww01);
        AudioManager.SharedInstance.PlayClip(GamePlayer.SharedInstance.characterSounds.turn_left);
    }

    public void ShowSwipeRight()
    {
        if (tutorialTP != null)
        {
            if (tutorialTP.enabled)
                return;
        }

        tutorialArrowLabelPanel.SetActive(true);
        NGUIToolsExt.SetActive(tutorialArrow.gameObject, true);
        tutorialArrow.localPosition = new Vector3(0, 0, 0);
        tutorialArrow.localRotation = Quaternion.Euler(new Vector3(0, 0, -90));
        //tutorialTP = TweenPosition.Begin(tutorialArrow.gameObject, 1.0f, new Vector3(Screen.width, tutorialArrow.localPosition.y, tutorialArrow.localPosition.z));
        tutorialTP = TweenPosition.Begin(tutorialArrow.gameObject, 1.0f,
            new Vector3(900, tutorialArrow.localPosition.y, tutorialArrow.localPosition.z));
        if (tutorialTC != null)
        {
            tutorialTC.value = Color.white;
        }
        tutorialTC = TweenColor.Begin(tutorialArrow.gameObject, 0.5f, new Color(1, 1, 1, 0));
        tutorialTC.delay = 0.5f;

        //AudioManager.SharedInstance.PlayFX(AudioManager.Effects.oz_turnright_ww01);
        AudioManager.SharedInstance.PlayClip(GamePlayer.SharedInstance.characterSounds.turn_right);
    }

    public void ShowSwipeUp()
    {
        if (tutorialTP != null)
        {
            if (tutorialTP.enabled)
                return;
        }

        tutorialArrowLabelPanel.SetActive(true);
        NGUIToolsExt.SetActive(tutorialArrow.gameObject, true);
        tutorialArrow.localPosition = new Vector3(0, 0, 0);
        tutorialArrow.localRotation = Quaternion.Euler(new Vector3(0, 0, 0));
        //tutorialTP = TweenPosition.Begin(tutorialArrow.gameObject, 1.0f, new Vector3(tutorialArrow.localPosition.x, Screen.height, tutorialArrow.localPosition.z));
        tutorialTP = TweenPosition.Begin(tutorialArrow.gameObject, 1.0f,
            new Vector3(tutorialArrow.localPosition.x, 900, tutorialArrow.localPosition.z));
        if (tutorialTC != null)
        {
            tutorialTC.value = Color.white;
        }
        tutorialTC = TweenColor.Begin(tutorialArrow.gameObject, 0.5f, new Color(1, 1, 1, 0));
        tutorialTC.delay = 0.5f;

        //AudioManager.SharedInstance.PlayFX(AudioManager.Effects.oz_turnleft_ww01);
        AudioManager.SharedInstance.PlayClip(GamePlayer.SharedInstance.characterSounds.turn_right);
    }

    public void ShowSwipeDown()
    {
        if (tutorialTP != null)
        {
            if (tutorialTP.enabled)
                return;
        }

        tutorialArrowLabelPanel.SetActive(true);
        NGUIToolsExt.SetActive(tutorialArrow.gameObject, true);
        tutorialArrow.localPosition = new Vector3(0, 100, 0);
        tutorialArrow.localRotation = Quaternion.Euler(new Vector3(0, 0, 180));
        //tutorialTP = TweenPosition.Begin(tutorialArrow.gameObject, 1.0f, new Vector3(tutorialArrow.localPosition.x, -Screen.height, tutorialArrow.localPosition.z));
        tutorialTP = TweenPosition.Begin(tutorialArrow.gameObject, 1.0f,
            new Vector3(tutorialArrow.localPosition.x, -900, tutorialArrow.localPosition.z));
        if (tutorialTC != null)
        {
            tutorialTC.value = Color.white;
        }
        tutorialTC = TweenColor.Begin(tutorialArrow.gameObject, 0.5f, new Color(1, 1, 1, 0));
        tutorialTC.delay = 0.5f;

        //AudioManager.SharedInstance.PlayFX(AudioManager.Effects.oz_turnright_ww01);
        AudioManager.SharedInstance.PlayClip(GamePlayer.SharedInstance.characterSounds.turn_right);
    }

    public void ShowSwipeTutorial(int TutorialID)
    {
        if (tutorialTP != null && tutorialTP.enabled)
        {
            return;
        }

        if (tutorialTS != null && tutorialTS.enabled)
        {
            return;
        }

        Time.timeScale = 0f;
        tutorialRoot.gameObject.SetActive(true);
        tutorialHand.gameObject.SetActive(true);

        switch (TutorialID)
        {
            case 0: //左划
                tutorialHand.localPosition = new Vector3(200f, 0f, 0f);
                tutorialTP = TweenPosition.Begin(tutorialHand.gameObject, 1.0f, new Vector3(-200f, 0f, 0f));
                break;
            case 1: //右划
            case 3:
                tutorialHand.localPosition = new Vector3(-200f, 0f, 0f);
                tutorialTP = TweenPosition.Begin(tutorialHand.gameObject, 1.0f, new Vector3(200f, 0f, 0f));
                break;
            case 2: //跳跃
                tutorialHand.localPosition = new Vector3(0f, 0f, 0f);
                tutorialTP = TweenPosition.Begin(tutorialHand.gameObject, 1.0f, new Vector3(0f, 200f, 0f));
                break;
            case 4: //点击技能
                tutorialHand.localPosition = new Vector3(-200f, -480f, 0f);
                tutorialTS = TweenScale.Begin(tutorialHand.gameObject, 1.0f, Vector3.one);
                tutorialTS.to = Vector3.one*0.8f;
                //tutorialTS.style = UITweener.Style.Loop;
                //tutorialTS.duration = 3f;
                break;
        }

        if (tutorialTC != null)
            tutorialTC.value = Color.white;

        tutorialTC = TweenColor.Begin(tutorialHand.gameObject, 0.5f, new Color(1, 1, 1, 0));
        tutorialTC.delay = 0.5f;

        //AudioManager.SharedInstance.PlayClip(GamePlayer.SharedInstance.characterSounds.turn_right);
    }

    public void ShowEnvChangeHud(string message)
    {
        if (envChangeHudLabel == null)
            return;

        NGUIToolsExt.SetActive(envChangeHud.gameObject, true);
        NGUIToolsExt.SetActive(envChangeHudLabel.gameObject, true);
        NGUIToolsExt.SetActive(envChangeHudArrow, true);

        envChangeHudLabel.text = message;
        //envChangeHudLabel.MakePixelPerfect();

        envChangeHud.alpha = 0f;
        var ta = envChangeHud.GetComponent<TweenAlpha>();
        if (ta)
        {
            ta.delay = 0f;
        }
        ta = TweenAlpha.Begin(envChangeHud.gameObject, 1f, 1f);
//		ta.onFinished += OnCompleteEnvChangeHud;
        EventDelegate.Add(ta.onFinished, OnCompleteEnvChangeHud);
        AudioManager.SharedInstance.PlayFX(AudioManager.Effects.oz_UI_scoreTally_fireworks_01, 0.3f);
    }

    private void OnCompleteEnvChangeHud()
    {
//		tween.onFinished -= OnCompleteEnvChangeHud;
        EventDelegate.Remove(UITweener.current.onFinished, OnCompleteEnvChangeHud);

        var ta = TweenAlpha.Begin(envChangeHud.gameObject, 2f, 0.98f);
//		ta.onFinished += OnHideEnvAllChangeHud;
        EventDelegate.Add(ta.onFinished, OnHideEnvAllChangeHud);
    }

    private void OnHideEnvAllChangeHud()
    {
//		tween.onFinished -= OnCompleteEnvChangeHud;
        EventDelegate.Remove(UITweener.current.onFinished, OnHideEnvAllChangeHud);

        var ta = envChangeHud.GetComponent<TweenAlpha>();
        if (ta)
        {
            ta.delay = 3f;
        }
        ta = TweenAlpha.Begin(envChangeHud.gameObject, 1f, 0f);
//		ta.onFinished += OnHideEnvChangeHud;
        EventDelegate.Add(ta.onFinished, OnHideEnvChangeHud);
    }

    private void OnHideEnvChangeHud()
    {
//		tween.onFinished -= OnHideEnvChangeHud;
        EventDelegate.Remove(UITweener.current.onFinished, OnHideEnvChangeHud);

        //envChangeHudLabel.MakePixelPerfect();
        NGUIToolsExt.SetActive(envChangeHud.gameObject, false);
        NGUIToolsExt.SetActive(envChangeHudLabel.gameObject, false);
        NGUIToolsExt.SetActive(envChangeHudArrow, false);
    }

    private void OnClickButton()
    {
        OnShareButtonClicked();
    }

    public void EmitCoin()
    {
        fx_coin.Emit(1);
    }

    public void ShowFastTravel()
    {
        ShowFastTravelNow(); //Invoke("ShowFastTravelNow", 3f);
    }

    private void ShowFastTravelNow()
    {
        if (!GamePlayer.SharedInstance.IsDead && GamePlayer.SharedInstance.HasFastTravel || true)
        {
            var buttonsToShow = new List<FastTravelButton>();

            foreach (var ftb in fastTravel)
            {
                if (ftb.IsSetDownloaded() &&
                    EnvironmentSetManager.SharedInstance.CurrentEnvironmentSet.SetId != ftb.environmentSetId &&
                    !GamePlayer.SharedInstance.HasBoost && !GamePlayer.SharedInstance.HasFastTravel &&
                    GameProfile.SharedInstance.Player.GetConsumableCount(
                        ftb.GetFastTravelConsumableID(ftb.environmentSetId)) > 0)
                    buttonsToShow.Add(ftb); //ftb.Show();
            }

            if (buttonsToShow.Count == 1) // if one ftb to show, center it
                buttonsToShow[0].Show(0);
            else if (buttonsToShow.Count == 2) // if two ftb's to show, show them side by side
            {
                buttonsToShow[0].Show(-70);
                buttonsToShow[1].Show(70);
            }
            else if (buttonsToShow.Count == 3) // if three ftb's to show, show them side by side
            {
                buttonsToShow[0].Show(-140);
                buttonsToShow[1].Show(0);
                buttonsToShow[2].Show(140);
            }
        }
    }

    public void HideFastTravel()
    {
        if (!GamePlayer.SharedInstance.IsDead && GamePlayer.SharedInstance.HasFastTravel || true)
        {
            foreach (var ftb in fastTravel)
            {
                ftb.Hide();
            }
        }
    }

    private void ResetEnvProgress()
    {
        envProgressSlider.value = 0;
        envProgressMove.transform.localPosition = new Vector3(0f, 120f, -200f);
        envProgressMove.gameObject.SetActive(false);
    }

    public void FadeInEnvProgress()
    {
        envprogfadecount--;
        if (envprogfadecount <= 0)
            TweenAlpha.Begin(envProgressMove.gameObject, 0.25f, 1f);
    }

    public void FadeOutEnvProgress()
    {
        envprogfadecount++;
        TweenAlpha.Begin(envProgressMove.gameObject, 0.25f, 0f);
    }

    public void ShowEnvProgress(int setId)
    {
        showingEnvProgress = true;
        envProgressMove.gameObject.SetActive(true);
        switch (setId)
        {
            case 1:
                envProgressSprite.spriteName = "icon_map_whimsiewoods";
                break;
            case 2:
                envProgressSprite.spriteName = "icon_map_darkforest";
                break;
            case 3:
                envProgressSprite.spriteName = "icon_map_winkiecountry";
                break;
            case 4:
                envProgressSprite.spriteName = "icon_location_emeraldcity";
                break;
        }
        envProgressLabel.text =
            Localization.SharedInstance.Get(EnvironmentSetManager.SharedInstance.LocalDict[setId].Title);
        iTween.MoveTo(envProgressMove.gameObject, iTween.Hash(
            "y", 0f,
            "islocal", true,
            "time", 0.5f
            ));
        //envProgressDistance = GameController.SharedInstance.RealDistanceTraveled;
        lastDistTravelled = GameController.SharedInstance.RealDistanceTraveled;
        curDistanceRatio = 0f;
        estimatedDistLeft = 1000f;
    }

    public void HideEnvProgress()
    {
        showingEnvProgress = false;
        iTween.MoveTo(envProgressMove.gameObject, iTween.Hash(
            "y", 120f,
            "islocal", true,
            "time", 0.5f,
            "oncomplete", "HideEnvProgressComplete",
            "oncompletetarget", gameObject
            ));
    }

    public void HideEnvProgressComplete()
    {
        envProgressMove.gameObject.SetActive(false);
    }

    //道具教程
    public void ShowAbilityTutorial(bool isUtility = false)
    {
        playerSpeedAbilityTutorial = GamePlayer.SharedInstance.GetPlayerVelocity();
        UIManagerOz.SharedInstance.inGameVC.bonusButtons.CancelHideConsumableAndModifierButtons();
        UIManagerOz.SharedInstance.inGameVC.bonusButtons.EnableAllButtons(false);
        if (isUtility)
        {
            isUtilityTutorial = true;
        }
        else
        {
            isUtilityTutorial = false;
        }

        notify.Debug("ShowAbilityTutorial " + isUtility);

        isAbilityTutorialOn = true;

        iTween.ValueTo(gameObject, iTween.Hash(
            "from", 1f,
            "to", 0f,
            "time", 0.5f,
            "onupdate", "OnUpdateTimeScale",
            "onupdatetarget", gameObject,
            "ignoretimescale", true
            ));
        notify.Debug("UpdateTimeScale to 0");

        bonusButtons.StopCoroutine("BlinkIcons");

        NGUITools.SetActive(tutorialRoot.gameObject, false);
        tutorialRoot.gameObject.SetActive(true);
        tutorialAbility.transform.parent.gameObject.SetActive(true);

        tutorialAbilityCounter = 0;
        tutorialAbility.transform.localPosition = Vector3.zero;
        tutorialAbility.alpha = 0f;
        tutorialAbility.gameObject.SetActive(true);
        tutorialAbilityButton.transform.localPosition = tutorialAbilityButtonPos;
        tutorialAbilityButton.collider.enabled = true;
        //tutorialAbilityRing.renderer.enabled = false;
        //tutorialAbilityRing.renderer.material = tutorialRingMaterial;
        tutorialAbilityRingSprite.alpha = 0f;
        tutorialAbilityGem.enabled = false;
        Transform ab;
        if (isUtilityTutorial)
        {
            // Debug.LogError("fuckfuck");
            tutorialAbilityLabel.text = Localization.SharedInstance.Get("Tut_HUD_Cons");
            UIManagerOz.SharedInstance.inGameVC.bonusButtons.tutorialButtonAbility = null;
            ab = UIManagerOz.SharedInstance.inGameVC.bonusButtons.tutorialButtonUtility.transform;
            ab.localPosition = new Vector3(ab.localPosition.x, ab.localPosition.y, -150f);
            tutorialFinger.transform.localPosition = new Vector3(-300f, -1000f, -200f);
            tutorialFinger.transform.localRotation = Quaternion.Euler(0f, 0f, -130f);
            var pos = ab.position - Vector3.forward*1.2f;
            tutorialAbilityButton.transform.localPosition = new Vector3(1000f, 0f, -200f);
            tutorialAbilityRing.transform.position = ab.position - Vector3.forward*0.2f;
            iTween.MoveTo(tutorialFinger.gameObject, iTween.Hash(
                "position", pos,
                "time", 1.6f,
                "oncomplete", "ShowFingerFx",
                "oncompletetarget", gameObject,
                "ignoretimescale", true
                ));
        }
        else
        {
            var loc = Localization.SharedInstance.Get("Tut_InGame_ModifierPrompt_1");
            var powerupName =
                Localization.SharedInstance.Get(
                    UIManagerOz.SharedInstance.inGameVC.bonusButtons.tutorialButtonAbility.GetComponent<BonusButton>()
                        .bonusName);
            var text = string.Format(loc, powerupName);

            tutorialAbilityLabel.text = text;
            tutorialFinger.transform.localPosition = new Vector3(300f, -1000f, -200f);
            tutorialFinger.transform.localRotation = Quaternion.Euler(0f, 0f, 130f);
            UIManagerOz.SharedInstance.inGameVC.bonusButtons.tutorialButtonUtility = null;
            ab = UIManagerOz.SharedInstance.inGameVC.bonusButtons.tutorialButtonAbility.transform;
            ab.localPosition = new Vector3(ab.localPosition.x, ab.localPosition.y, -150f);
            tutorialAbilityButton.transform.localPosition = new Vector3(
                tutorialAbilityButton.transform.localPosition.x, tutorialAbilityButton.transform.localPosition.y, -200f);
            ab.collider.enabled = true;
        }
        tutorialAbility.GetComponent<ScaleModalDialog>().SetScale();
        TweenAlpha.Begin(tutorialAbility.gameObject, 0.5f, 1f);
    }

    public void HideAbilityTutorial()
    {
        if (!isAbilityTutorialOn)
            return;
        isAbilityTutorialOn = false;
        notify.Debug("HideAbilityTutorial " + isUtilityTutorial);
        GamePlayer.SharedInstance.SetPlayerVelocity(playerSpeedAbilityTutorial.magnitude);
        var bb = UIManagerOz.SharedInstance.inGameVC.bonusButtons;

        if (isUtilityTutorial)
        {
            PlayerPrefs.SetInt("utilityTutorialPlayedInt", 1);
            GameController.SharedInstance.utilityTutorialPlayed = true;
            GameProfile.SharedInstance.Player.EarnConsumable(
                bb.tutorialButtonUtility.GetComponent<BonusButton>().artifactId, 1);
            //UIManagerOz.SharedInstance.inGameVC.bonusButtons.EnableAllButtonsUtility(true);
        }
        else
        {
            PlayerPrefs.SetInt("abilityTutorialPlayedInt", 1);
            GameController.SharedInstance.abilityTutorialPlayed = true;
            //UIManagerOz.SharedInstance.inGameVC.bonusButtons.EnableAllButtonsAbility(true);
        }

        //bb.EnableAllButtons(true);

        iTween.ValueTo(gameObject, iTween.Hash(
            "from", 0f,
            "to", 1f,
            "time", 0.5f,
            "onupdate", "OnUpdateTimeScale",
            "onupdatetarget", gameObject,
            "ignoretimescale", true
            ));
        notify.Debug("UpdateTimeScale to 1");

        var ta = TweenAlpha.Begin(tutorialAbility.gameObject, 0.4f, 0f);
//		ta.onFinished += OnHideAbilityTutorial;
        EventDelegate.Add(ta.onFinished, OnHideAbilityTutorial);


        bb.HideConsumableAndModifierButtonsNow();
        if (isUtilityTutorial)
        {
            //Transform t =  bb.tutorialButtonUtility.transform;
            //t.localPosition = new Vector3(t.localPosition.x, t.localPosition.y, 0f);
            bb.tutorialButtonUtility = null;
            // this is if we use both tutorials together
            //if(GameController.SharedInstance.abilityTutorialPlayed || !bb.CanShowModifiers()){
            //	bb.HideConsumableAndModifierButtons();
            //}
        }
        else
        {
            //Transform t =  UIManagerOz.SharedInstance.inGameVC.bonusButtons.tutorialButtonAbility.transform;
            //t.localPosition = new Vector3(t.localPosition.x, t.localPosition.y, 0f);
            bb.tutorialButtonUtility = null;

            //if(GameController.SharedInstance.utilityTutorialPlayed || !bb.CanShowConsumables()){
            //	bb.HideConsumableAndModifierButtons();
            //}
        }
    }

    public void OnHideAbilityTutorial()
    {
        notify.Debug("OnHideAbilityTutorial " + GameController.SharedInstance.abilityTutorialPlayed + " " +
                     GameController.SharedInstance.utilityTutorialPlayed);
//		tweener.onFinished -= OnHideAbilityTutorial;
        EventDelegate.Remove(UITweener.current.onFinished, OnHideAbilityTutorial);

        tutorialAbility.gameObject.SetActive(false);
        //UIManagerOz.SharedInstance.inGameVC.bonusButtons.HideConsumableAndModifierButtons();
    }

    public void OnAbilityOkClicked()
    {
        if (tutorialAbilityCounter == 0)
        {
            tutorialAbilityLabel.text = Localization.SharedInstance.Get("Tut_InGame_ModifierPrompt_2");
            tutorialAbility.GetComponent<ScaleModalDialog>().SetScale();
        }
        if (tutorialAbilityCounter == 1)
        {
            var bb = UIManagerOz.SharedInstance.inGameVC.bonusButtons.tutorialButtonAbility.GetComponent<BonusButton>();
            var powerupName = Localization.SharedInstance.Get(bb.bonusName);
            var powerupDesc = Localization.SharedInstance.Get(bb.desc);
            var loc3 = Localization.SharedInstance.Get("Tut_InGame_ModifierPrompt_3");
            var loc4 = Localization.SharedInstance.Get("Tut_InGame_ModifierPrompt_4");
            var text = loc3 + "\n" + string.Format(loc4, powerupName, powerupDesc);
            tutorialAbilityLabel.text = text;
            tutorialAbility.GetComponent<ScaleModalDialog>().SetScale();
            tutorialAbilityButton.gameObject.SetActive(false);
            Vector3 pos;

            UIManagerOz.SharedInstance.inGameVC.bonusButtons.tutorialButtonAbility.transform.localPosition -=
                Vector3.forward*90f;
            tutorialAbilityRing.transform.position =
                UIManagerOz.SharedInstance.inGameVC.bonusButtons.tutorialButtonAbility.transform.position -
                Vector3.forward*0.2f;
            pos = UIManagerOz.SharedInstance.inGameVC.bonusButtons.tutorialButtonAbility.transform.position -
                  Vector3.forward*1.2f;

            iTween.MoveTo(tutorialFinger.gameObject, iTween.Hash(
                "position", pos,
                "time", 1.6f,
                "oncomplete", "ShowFingerFx",
                "oncompletetarget", gameObject,
                "ignoretimescale", true
                ));
            tutorialAbilityGem.enabled = true;
            //give the player a gem
            //GamePlayer.SharedInstance.AddGemsToScore(1);
            GameProfile.SharedInstance.Player.specialCurrencyCount ++;
        }

        tutorialAbilityCounter++;
    }

    public void ShowFingerFx()
    {
        if (isAbilityTutorialOn)
        {
            if (isUtilityTutorial)
            {
                notify.Debug("ShowFingerFx " +
                             UIManagerOz.SharedInstance.inGameVC.bonusButtons.tutorialButtonUtility.name);
                UIManagerOz.SharedInstance.inGameVC.bonusButtons.tutorialButtonUtility.collider.enabled = true;
            }
            else
            {
                notify.Debug("ShowFingerFx " +
                             UIManagerOz.SharedInstance.inGameVC.bonusButtons.tutorialButtonAbility.name);
                UIManagerOz.SharedInstance.inGameVC.bonusButtons.tutorialButtonAbility.collider.enabled = true;
            }
        }

        tutorialAbilityRing.transform.localScale = Vector3.one;
        //tutorialAbilityRing.renderer.enabled = true;
        //tutorialAbilityRing.renderer.enabled = false;
        tutorialAbilityRingCount = 0;
        EmitRing();
    }

    private void EmitRing()
    {
        tutorialAbilityRingCount++;
        if (tutorialAbilityRingCount > 2 && !isAbilityTutorialOn)
        {
            OnRingComplete();
            return;
        }

        iTween.ScaleTo(tutorialFinger.gameObject, iTween.Hash(
            "scale", tutorialFingerScale*1.4f,
            "time", 0.05f,
            "ignoretimescale", true,
            "easetype", iTween.EaseType.easeOutCirc,
            "oncomplete", "OnFingerScaleComplete",
            "oncompletetarget", gameObject
            ));
    }

    public void OnFingerScaleComplete()
    {
        iTween.ScaleTo(tutorialFinger.gameObject, iTween.Hash(
            "scale", tutorialFingerScale/1.4f,
            "time", 0.05f,
            "ignoretimescale", true,
            "easetype", iTween.EaseType.easeInCirc,
            "oncomplete", "StartRingAnim",
            "oncompletetarget", gameObject
            ));
    }

    public void StartRingAnim()
    {
        iTween.ValueTo(tutorialAbilityRing, iTween.Hash(
            "from", 0f,
            "to", 1f,
            "time", 0.3f,
            "ignoretimescale", true,
            "onupdate", "OnUpdateRing",
            "onupdatetarget", gameObject
            ));
    }

    public void OnUpdateRing(float val)
    {
        tutorialAbilityRing.transform.localScale = Vector3.one*(10f + 150f*val);
        //tutorialAbilityRing.renderer.material.SetColor("_Color",new Color(1f,0f,0f,0.8f - val * 0.8f));
        tutorialAbilityRingSprite.alpha = 1f - val;
        if (val >= 1)
            EmitRing();
    }

    private void OnRingComplete()
    {
        if (GameController.SharedInstance.IsTutorialMode)
        {
            isPowerMeterFinger = false;
            tutorialAbilityRing.SetActive(false);
            TweenAlpha.Begin(tutorialAbility.gameObject, 0.6f, 0f);
        }
    }

    public void OnUpdateTimeScale(float val)
    {
        Time.timeScale = val;
        if (isAbilityTutorialOn)
        {
            UIManagerOz.SharedInstance.inGameVC.bonusButtons.GetComponent<UIPanelAlpha>().alpha = 1 - val;
        }
        if (Time.timeScale == 0f)
        {
            UIManagerOz.SharedInstance.inGameVC.bonusButtons.MakeButtonsStatic(false);
        }
    }

    public void SetEstimatedDistanceLeft(float dist)
    {
        estimatedDistLeft = dist;
    }

    private void Update()
    {
        //实时更新关卡任务进度
        if (!GameController.SharedInstance.EndlessMode)
        {
//            progressBar.value = currLevelData._conditionList[0]._statValue*1.0f/
//                                currLevelData._conditionList[0]._statValue3ForLevel;
            progressBar.value = UIManagerOz.SharedInstance.worldOfOzVC.UpdateStarProgressBar(
               null, currLevelData,true);
            
            progressTxt.text = currLevelData._conditionList[0]._statValue.ToString();
            var curstarCount = UIManagerOz.SharedInstance.worldOfOzVC.GetStarRank(currLevelData, true);
            if (curstarCount == WorldOfOzCellData.PerfectClearStar) //三星强制结算
            {
                if (PerfectClearTrigger)
                {
                    PerfectClearTrigger = false;
                    forcefinish();
                }
            }
            //超越记录点亮星星
            if (currLevelData._conditionList[0]._statValue > oldEarnedSateValue)
            {
                var starCountPerRun = UIManagerOz.SharedInstance.worldOfOzVC.GetStarRank(currLevelData, true);
                if (starCountPerRun > starCountBest)
                {
                    UIManagerOz.SharedInstance.worldOfOzVC.UpdateStarSprite(star1, star2, star3, starCountPerRun);
                    starCountBest = starCountPerRun;
                    
                    //
                    AudioManager.SharedInstance.PlayFX(AudioManager.Effects.ui_postgame_achievement);
                    GameObject temp = null;
                    if (starCountPerRun == 1)
                        temp = star1.gameObject;
                    else if (starCountPerRun == 2)
                        temp = star2.gameObject;
                    else if (starCountPerRun == 3)
                        temp = star3.gameObject;

                    if (temp != null)
                        UIDynamically.instance.ZoomOutToOne(temp, new Vector3(1.7f, 1.7f, 1f), 1f);
                }
            }
        }
        //
        if (envProgressSlider.gameObject.activeSelf)
        {
            var delta = GameController.SharedInstance.RealDistanceTraveled - lastDistTravelled;
            var ratioToAdd = delta/estimatedDistLeft;

            curDistanceRatio += (1 - curDistanceRatio)*ratioToAdd;

            //	float ratio = (GameController.SharedInstance.DistanceTraveled - envProgressDistance) / 1100f;
            envProgressSlider.value = curDistanceRatio;

            estimatedDistLeft -= delta;
            lastDistTravelled = GameController.SharedInstance.RealDistanceTraveled;
        }
    }

    private void forcefinish()
    {
        GameController.SharedInstance.Hold = true;
        GamePlayer.SharedInstance.NoStumble = true;
        PerfectClearFX.SetActive(true);
        GamePlayer.SharedInstance.StartSlowingByTime(3f); //减速*
        StartCoroutine(camerafadeEnd());

    }

   IEnumerator camerafadeEnd()
    {


        yield return new WaitForSeconds(3f);
        
        PerfectClearFX.SetActive(false);
        //camerafade.SetActive(true);
        //iTween.ColorTo(camerafade, iTween.Hash(
        //    "color", Color.white,
        //    "time", 1f
        //    ));
        yield return new WaitForSeconds(0.5f);
        GameController.SharedInstance.Hold = false;
        GamePlayer.SharedInstance.NoStumble = false;

        GameController.SharedInstance.Player.IsDead = true;
        GameController.SharedInstance.Player.DespawnModel();
        GameController.SharedInstance.IsGameOver = true;
        yield break;
    }

    public void SetFeverProgress(float progress)
    {
        if (FeverProgressBar)
            FeverProgressBar.value = progress;
    }

    public void BlinkFeverProgress()
    {
        if (!FeverProgressBar)
            return;

        var slider = FeverProgressBar.GetComponent<UISlider>();
        var ta = slider.foregroundWidget.GetComponent<TweenAlpha>();
        if (!ta)
        {
            ta = slider.foregroundWidget.gameObject.AddComponent<TweenAlpha>();
            ta.style = UITweener.Style.PingPong;
            ta.method = UITweener.Method.Linear;
            ta.duration = 0.5f;
            ta.from = 1;
            ta.to = 0f;
        }
        ta.enabled = true;
        ta.PlayForward();
    }

    public void StopBlinkFeverProgress()
    {
        var slider = FeverProgressBar.GetComponent<UISlider>();
        var ta = slider.foregroundWidget.GetComponent<TweenAlpha>();
        ta.enabled = false;
        slider.alpha = 1;
    }

    public void ShowSKillAndConsumable(bool isFever = false)
    {
        bonusButtons.EmergePurchasedConsumableButtons(true);
        UIDynamically.instance.LeftToScreen(coinMeter.gameObject, -100f, 190f, 0.5f);
        if (!isFever)
            UIDynamically.instance.LeftToScreen(FeverProgressBar.transform.parent.gameObject, -450f, -307f, 0.5f);
    }

    public void HideSKillAndConsumable(bool isFever = false)
    {
        bonusButtons.EmergePurchasedConsumableButtons(false);
        UIDynamically.instance.LeftToScreen(coinMeter.gameObject, 190f, -100f, 0.5f);
        if (!isFever)
            UIDynamically.instance.LeftToScreen(FeverProgressBar.transform.parent.gameObject, -307f, -450f, 0.5f);
    }
}