using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIIdolMenuViewControllerOz : UIViewControllerOz
{
    public GameObject playerHead;
    public UISprite iconSprite;
    public UILabel lvTxt;
    public UISprite expProgress;

    public UIPlayerInfo playerInfo;

    public GameObject StartingSetPiece ;

    public List<GameObject> modelContainer;//mid lef right

    public GameObject left_lock,left_lockFX;
    public GameObject left_enter;
    public GameObject right_lock,right_lockFX;
    public GameObject right_enter;

    public UITeamSelect teamSelect;

  

    [HideInInspector]
    public List<GameObject> models;//mid lef right


    public GameObject btn_level;
    public GameObject btn_challenge;
    public GameObject btn_charsel;
    public GameObject btn_setting;
    public GameObject btn_task;
    public GameObject btn_achivement;
    public GameObject btn_leaderboard;
    public GameObject tutorialCharacter;

    public GameObject NewsFeed;
    public static bool isQuitting = false;
    
    public float mQuitTimer = 0.0f;

    public UISignIn SignInRoot;
    //private NotificationSystem notificationSystem;
    private NotificationIcons notificationIcons;
    
    bool ItemClicked = false;
    
    public bool DidLaunchViaPushNotification { get; set; } // For push notifications
    public string MenuButtonToPressAfterGameIntro { get; set; } // For push notifications and Burstly Notification Ads


    //Response can break into while running.  Bad.
    //On Bring in Bottom complete.

    public delegate void OnMainMenuAnimationCompleteHandler();
    protected static event OnMainMenuAnimationCompleteHandler onMainMenuAnimationCompleteEvent = null;

    public static void RegisterForOnIdolBottomPanelComplete( OnMainMenuAnimationCompleteHandler delg) {
        onMainMenuAnimationCompleteEvent += delg;
    }
    public static void UnregisterForOnMainMenuAnimationCompleteHandler( OnMainMenuAnimationCompleteHandler delg) {
        onMainMenuAnimationCompleteEvent -= delg;
    }
    
    protected override void Awake() 
    { 
        base.Awake();
        notificationIcons = gameObject.GetComponent<NotificationIcons>();
    }       
   
    private void UnlockSlot()
    {
        if(GameProfile.SharedInstance.Player.teamIndexsOrder.Count == 1 &&
           GameProfile.SharedInstance.Player.playerLv>=5)
            GameProfile.SharedInstance.Player.teamIndexsOrder.Add(-1);
        if(GameProfile.SharedInstance.Player.teamIndexsOrder.Count == 2 &&
            GameProfile.SharedInstance.Player.playerLv>=10)
                GameProfile.SharedInstance.Player.teamIndexsOrder.Add(-1);
    }

    protected override void Start() 
    {
        base.Start();
        playerInfo.gameObject.SetActive(false);
        if (!GameProfile.SharedInstance.Player.GetIsChallengeUnlock())
            switchtoUnlockChallenge(false);
        else
            switchtoUnlockChallenge(true);
    }

    protected override void RegisterEvent()
    {
        UIEventListener.Get(btn_charsel).onClick= OnRoleClicked;
        UIEventListener.Get(btn_level).onClick = OnLevelClicked;
        UIEventListener.Get(btn_challenge).onClick = OnPlayClicked;
        UIEventListener.Get(btn_setting).onClick = OnSettingClicked;
        UIEventListener.Get(btn_task).onClick = OnTaskClicked;
        UIEventListener.Get(btn_achivement).onClick = OnAchieveClicked;
        UIEventListener.Get(btn_leaderboard).onClick = OnLeaderBoardClicked;
        UIEventListener.Get(left_lock).onClick = OnLeftSlotClicked;
        UIEventListener.Get(right_lock).onClick = OnRightSlotClicked;
        UIEventListener.Get(left_enter).onClick = OnLeftEnterClicked;
        UIEventListener.Get(right_enter).onClick = OnRightEnterClicked;
        UIEventListener.Get(playerHead).onClick = OnHeadClicked;
        UIEventListener.Get(modelContainer[0]).onClick = OnCharacterClicked0;
        UIEventListener.Get(modelContainer[1]).onClick = OnCharacterClicked1;
        UIEventListener.Get(modelContainer[2]).onClick = OnCharacterClicked2;

    }

    void OnHeadClicked(GameObject obj)
    {
        playerInfo.appear(gameObject);
    }

    void OnLevelClicked(GameObject obj)
    {
        UIManagerOz.SharedInstance.worldOfOzVC.appear();
        UIManagerOz.SharedInstance.PaperVC.appear();
        //
        disappear();
    }

    void OnTaskClicked(GameObject obj)
    {
        UIManagerOz.SharedInstance.ObjectivesVC.appear();
    }

    void OnAchieveClicked(GameObject obj)
    {
        UIManagerOz.SharedInstance.statsVC.appear();
    }

    void OnLeaderBoardClicked(GameObject obj)
    {
        UIManagerOz.SharedInstance.RankingVC.appear();
        
       // UIManagerOz.SharedInstance.okayDialog.ShowOkayDialog("Msg_Developing", "Btn_Ok");
    }

    //二号机位
    void OnLeftSlotClicked(GameObject obj)
    {
        UIConfirmDialogOz.onPositiveResponse +=CancelBuySlot;
        UIConfirmDialogOz.onNegativeResponse +=SureBuyLeftSlot;
        UIManagerOz.SharedInstance.confirmDialog.ShowConfirmDialog("用户等级达到5级时解锁","立即解锁","确定",50);


    }
    //三号机位
    void OnRightSlotClicked(GameObject obj)
    {
       
        if(GameProfile.SharedInstance.Player.teamIndexsOrder.Count==1)
        {   
            UIManagerOz.SharedInstance.okayDialog.ShowOkayDialog("请先解锁二号机位", "Btn_Ok");
            return;
        }

        UIConfirmDialogOz.onPositiveResponse +=CancelBuySlot;
        UIConfirmDialogOz.onNegativeResponse +=SureBuyRightSlot;
        UIManagerOz.SharedInstance.confirmDialog.ShowConfirmDialog("用户等级达到10级时解锁","立即解锁","确定",100);

    }

    void CancelBuySlot()
    {
        UIConfirmDialogOz.onPositiveResponse -=CancelBuySlot;
        UIConfirmDialogOz.onNegativeResponse -=SureBuyLeftSlot;
        UIConfirmDialogOz.onNegativeResponse -=SureBuyRightSlot;
    }

    void SureBuyLeftSlot()
    {
        CancelBuySlot();
        if(GameProfile.SharedInstance.Player.specialCurrencyCount<50)
        {
            UIManagerOz.SharedInstance.StoreVC.BuyGems();
            return;
            
        }
       
        GameProfile.SharedInstance.Player.specialCurrencyCount -=50;
        GameProfile.SharedInstance.Player.teamIndexsOrder.Add(-1);
        left_lock.SetActive(false);
        left_lockFX.SetActive(false);
        left_enter.SetActive(true);
        GameProfile.SharedInstance.Serialize();
        modelContainer[1].collider.enabled = true;
        UIManagerOz.SharedInstance.okayDialog.ShowOkayDialog("您已经解锁2号小队位置", "Btn_Ok");
        UIManagerOz.SharedInstance.PaperVC.UpdateCurrency();
    }
    void SureBuyRightSlot()
    {
        CancelBuySlot();
        if(GameProfile.SharedInstance.Player.specialCurrencyCount<100)
        {
            UIManagerOz.SharedInstance.StoreVC.BuyGems();
            return;
        }
        GameProfile.SharedInstance.Player.specialCurrencyCount -=100;
        GameProfile.SharedInstance.Player.teamIndexsOrder.Add(-1);
        right_lock.SetActive(false);
        right_lockFX.SetActive(false);
        right_enter.SetActive(true);
        GameProfile.SharedInstance.Serialize();
        modelContainer[2].collider.enabled = true;
        UIManagerOz.SharedInstance.okayDialog.ShowOkayDialog("您已经解锁3号小队位置", "Btn_Ok");
        UIManagerOz.SharedInstance.PaperVC.UpdateCurrency();
        
    }

    void OnLeftEnterClicked(GameObject obj)
    {
        if(isInTeamSelect)
        {
            teamSelect.disapear();
            HideBottomButton(false);
            return;
        }
        HideBottomButton(true);
        teamSelect.appear(1);
    }

    void OnRightEnterClicked(GameObject obj)
    {
        if(GameProfile.SharedInstance.Player.teamIndexsOrder.Count>1&&
           GameProfile.SharedInstance.Player.teamIndexsOrder[1]==-1)
        {
            UIManagerOz.SharedInstance.okayDialog.ShowOkayDialog("请先上二号机位", "Btn_Ok");
            return;
        }

        if(isInTeamSelect)
        {
            teamSelect.disapear();
            HideBottomButton(false);
            return;
        }
        HideBottomButton(true);
        teamSelect.appear(2);
        
    }

    private bool isInTeamSelect = false; //是否已经在小队选择界面
    int index = 0;//选中小队机位下标
    public void OnCharacterClicked0(GameObject obj)
    {
        if(!isShowFinished)
            return;
        if(isInTeamSelect)
        {
//            teamSelect.disapear();
//            UpdownStage(true);
//            HideBottomButton(false);
            UpdownStage(index,true);//上一个台子复位
           
           if(index == 0)
            {
                teamSelect.disapear();
                HideBottomButton(false);
                return;
            }
           
        }
        else
        {
            HideBottomButton(true);
        }
        index = 0;
        UpdownStage(index);
        Invoke("ShowTeamSelect",0.2f);
        isShowFinished = false;
    }
    private bool isShowFinished = false;
    void ShowTeamSelect()
    {
        isShowFinished = true;
        teamSelect.appear(index);
    }
    public void OnCharacterClicked1(GameObject obj)
    {
        if(!isShowFinished)
            return;
        if(isInTeamSelect)
        {
            UpdownStage(index,true);//上一个台子复位
            if(index == 1)
            {
                teamSelect.disapear();
                HideBottomButton(false);
                return;
            }

        }
        else
        {
            HideBottomButton(true);
        }
        index = 1;
        UpdownStage(index);
        Invoke("ShowTeamSelect",0.2f);
        isShowFinished = false;

    }
    public void OnCharacterClicked2(GameObject obj)
    {
        if(!isShowFinished)
            return;
        if(isInTeamSelect)
        {
            UpdownStage(index,true);//上一个台子复位
            if(index == 2)
            {
                teamSelect.disapear();
                HideBottomButton(false);
                return;
            }
        }
        else
        {
            HideBottomButton(true);
        }
        index = 2;
        UpdownStage(index);

        Invoke("ShowTeamSelect",0.2f);
        isShowFinished = false;

    }


    public void PlayCheerAnim(GameObject obj)
    {
        CharacterSounds sounds = obj.GetComponent<CharacterSounds>();
        if (sounds)
            AudioManager.SharedInstance.PlayCharacterSound(sounds.Cheer);
        StartCoroutine(coCheerAnim(obj,"Cheer"));
    }

    public float speakSpeed=1f;
    IEnumerator coCheerAnim(GameObject obj,string clipName)
    {
        Animation anim = obj.GetComponentInChildren<Animation>();
        if(anim != null)
        {
            AnimationClip CheereClip = anim.GetClip(clipName);
            if(CheereClip != null)
            {   
                if(anim.IsPlaying(CheereClip.name))
                    yield break;
                anim[clipName].speed = speakSpeed;
                anim.CrossFade(CheereClip.name);
                yield return new WaitForSeconds(CheereClip.length);
                
//                obj.collider.enabled = true;
                
                anim.CrossFade("Idle", CheereClip.length);//播放空闲动画
                
            }
        }
    }

    private void OnRoleClicked(GameObject obj)
    {   
        
        if(ItemClicked) return;
        
        ItemClicked = true;
        // clear background with color similar to menu UI background, eliminate flashing between screens        
        UIManagerOz.SharedInstance.SetUICameraClearFlagToSolidColorBG(true);        
       
        //
        UIManagerOz.SharedInstance.chaSelVC.appear();
        UIManagerOz.SharedInstance.PaperVC.appear();
        //
        disappear();
        
    }

    void OnSettingClicked(GameObject obj)
    {
        UIManagerOz.SharedInstance.settingsVC.appear();
    }

   

#if UNITY_ANDROID
    void Update()
    {
        if( mQuitTimer > 0.0f)
        {
            mQuitTimer -= Time.deltaTime;
            if( mQuitTimer <= 0.0f )
            //PurchaseUtil.exitGame();
                Application.Quit();
        }
    }
#endif

    public void PlayDynamicUI(bool hide = false)
    {
        HideBottomButton();
        UIDynamically.instance.TopToScreen(playerHead,110f,-3f,uiDurTime);
        if (UIManagerOz.SharedInstance.loadingVC.OnFinished==null)
         UIManagerOz.SharedInstance.idolMenuVC.SignInRoot.appear(); 
    }

    private float uiDurTime = 0.3f;
    public void HideBottomButton(bool hide = false)
    {
//        if(!hide)
//        {
//            UIDynamically.instance.TopToScreen(btn_charsel,-150f,0f,0.5f,hide,0.08f);
//            UIDynamically.instance.TopToScreen(btn_task,-150f,0f,0.5f,hide,0.16f);
//            UIDynamically.instance.TopToScreen(btn_achivement,-150f,0f,0.5f,hide,0.24f);
//            UIDynamically.instance.TopToScreen(btn_leaderboard,-150f,0f,0.5f,hide,0.32f);
//            UIDynamically.instance.TopToScreen(btn_setting,-150f,0f,0.5f,hide,0.4f);
//            UIDynamically.instance.LeftToScreen(btn_level,-500f,-205f,0.5f,hide,0.5f);
//            UIDynamically.instance.LeftToScreen(btn_challenge,500f,206f,0.5f,hide,0.5f);
//        }
//        else
        {
            UIDynamically.instance.TopToScreen(btn_charsel,-150f,0f,uiDurTime,hide);
            UIDynamically.instance.TopToScreen(btn_task,-150f,0f,uiDurTime,hide);
            UIDynamically.instance.TopToScreen(btn_achivement,-150f,0f,uiDurTime,hide);
            UIDynamically.instance.TopToScreen(btn_leaderboard,-150f,0f,uiDurTime,hide);
            UIDynamically.instance.TopToScreen(btn_setting,-150f,0f,uiDurTime,hide);
            UIDynamically.instance.LeftToScreen(btn_level,-500f,-205f,uiDurTime,hide);
            UIDynamically.instance.LeftToScreen(btn_challenge,500f,206f,uiDurTime,hide);
        }
        isInTeamSelect = hide;

    }

    public override void appear()
    {

        //----  教学debug
        //PlayerPrefs.SetInt("upgradeTutorialPlayedInt", 0);
        //PlayerPrefs.SetInt("levelTutorialPlayedInt", 0);
        //PlayerPrefs.SetInt("unlockChallengeTutorialPlayedInt", 0);
        //PlayerPrefs.SetInt("challengeTutorialPlayedInt", 0);
        //PlayerPrefs.Save();
        //----


        isShowFinished = true;
        if (models == null)
        {
            models = new List<GameObject>(3);
        }
        if (models.Count == 0)
        {
            models.Add(null);
            models.Add(null);
            models.Add(null);
        }

        teamSelect.disapear();
        PlayDynamicUI();
        UnlockSlot(); //是否要解锁机位

        //if (!GameController.SharedInstance.levelTutorialPlayed)
        if (GameController.SharedInstance.GetTutorialIDforSys()== 1f)
        {
            btn_charsel.GetComponent<UISprite>().depth = 3;
            btn_level.GetComponent<UISprite>().depth = 99;
            tutorialCharacter.transform.SetParent(btn_level.transform);
            tutorialCharacter.transform.ResetTransformation();
            tutorialCharacter.SetActive(true);
            var tutorialTS = TweenScale.Begin(tutorialCharacter, 1.0f, Vector3.one);
            tutorialTS.to = Vector3.one * 0.8f;
            tutorialTS.style = UITweener.Style.Loop;
        }
        //else if (GameController.SharedInstance.levelTutorialPlayed
        //         && !GameController.SharedInstance.upgradeTutorialPlayed)
        else if (GameController.SharedInstance.GetTutorialIDforSys() == 2f || GameController.SharedInstance.GetTutorialIDforSys() == 3f)
        {
            btn_charsel.GetComponent<UISprite>().depth = 99;
            btn_level.GetComponent<UISprite>().depth = 3;
            tutorialCharacter.transform.SetParent(btn_charsel.transform);
            tutorialCharacter.transform.ResetTransformation();
            tutorialCharacter.SetActive(true);
            var tutorialTS = TweenScale.Begin(tutorialCharacter, 1.0f, Vector3.one);
            tutorialTS.to = Vector3.one * 0.8f;
            tutorialTS.style = UITweener.Style.Loop;
        }
       // else if (GameController.SharedInstance.unlockrole2PosTutorialPlayed && !GameController.SharedInstance.unlockChallengeTutorialPlayed && GameProfile.SharedInstance.Player.GetIsChallengeUnlock()) //挑战关
        else if (GameController.SharedInstance.GetTutorialIDforSys() == 5f && GameProfile.SharedInstance.Player.GetIsChallengeUnlock())
        {
            switchtoUnlockChallenge(true);
            btn_charsel.GetComponent<UISprite>().depth = 3;
            btn_level.GetComponent<UISprite>().depth = 3;
            btn_challenge.GetComponent<UISprite>().depth = 99;
            tutorialCharacter.transform.SetParent(btn_challenge.transform);
            tutorialCharacter.transform.ResetTransformation();
            tutorialCharacter.SetActive(true);
            var tutorialTS = TweenScale.Begin(tutorialCharacter, 1.0f, Vector3.one);
            tutorialTS.to = Vector3.one * 0.8f;
            tutorialTS.style = UITweener.Style.Loop;
        
        }
        else
        {
            tutorialCharacter.SetActive(false);
        }


        RefreshPlayer();
        GameController.SharedInstance.MenusEntered();

        UIManagerOz.SharedInstance.SetUICameraClearFlagToSolidColorBG(false);
        UIManagerOz.SharedInstance.PaperVC.SetCurrentPage(UIManagerOz.SharedInstance.idolMenuVC);

        base.appear();
        StartingSetPiece.SetActive(true);

        ItemClicked = false;

        Services.Get<NotificationSystem>().SetNotificationIconsForThisPage(UiScreenName.IDOL);

        AudioManager.SharedInstance.SwitchToMainMenuMusic();

        //更新角色
        Invoke("InitialTeamModel", 0.1f);
        UIManagerOz.SharedInstance.PaperVC.appear();


    }

    public override void disappear ()
    {
        base.disappear ();
        StartingSetPiece.SetActive(false);
    }
    public void switchtoUnlockChallenge(bool isunlock = true)
    {
        if (isunlock)
        {
            btn_challenge.collider.enabled = true;
            btn_challenge.GetComponent<UISprite>().spriteName = "menu_challenge";  

        }
        else
        {
            btn_challenge.collider.enabled = false;
            btn_challenge.GetComponent<UISprite>().spriteName = "menu_lockchallenge";
        }
    }
    public void ResetCharacter(GameObject CharacterModel,GameObject parent,int characterIndex)
    {
        iTween.Stop(CharacterModel);//改bug

        CharacterModel.transform.parent = parent.transform;
        CharacterModel.transform.localRotation = Quaternion.Euler(0f,30f,0f);
        CharacterModel.transform.localScale = Vector3.one;
//        CharacterModel.transform.ResetTransformation();
        CharacterModel.SetActive(true);

//        //添加点击欢呼事件//
//        if(CharacterModel.GetComponent<SphereCollider>()== null)
//        {
//            CharacterStats  activeCharacter = GameProfile.SharedInstance.Characters[characterIndex];
//            SphereCollider sc =CharacterModel.AddComponent<SphereCollider>();
//            sc.radius = activeCharacter.colliderRadius;
//            sc.center = new Vector3(0f, activeCharacter.colliderRadius + 0.16f,0f);
//        }
//        else
        if(CharacterModel.GetComponent<SphereCollider>()!= null)
        CharacterModel.GetComponent<SphereCollider>().enabled = false;//true;

        //添加点击事件
//        UIEventListener.Get(CharacterModel).onClick = PlayCheerAnim;

        //删除长按事件
        UIEventListener.Get(CharacterModel).onPress = null;

        //删除旋转事件
        SpinWithMouse sp =CharacterModel.GetComponent<SpinWithMouse>();
        if(sp!=null)
            DestroyObject(sp);

    }

    public void RefreshPlayer()
    {
        iconSprite.spriteName = playerInfo.GetMenuIconSpriteName();
        lvTxt.text = "Lv "+GameProfile.SharedInstance.Player.playerLv;
        expProgress.fillAmount = playerInfo.GetExpBar();
        ObjectivesDataUpdater.SetGenericStat(ObjectiveType.PlayerLv, GameProfile.SharedInstance.Player.playerLv);//用户等级成就统计
    }

    public void PlayIdle(GameObject model)
    {
        if(model == null)
            return;
        Animation animsInGO = model.GetComponentsInChildren<Animation>(true)[0];
        if (animsInGO != null)
        {
            AnimationClip idle = animsInGO.GetClip("Idle");
            if (idle != null)
                animsInGO.Play(idle.name, PlayMode.StopAll);
        }
    }

    public bool isFromWorldLevel = false;//修改进入关卡界面返回后重复创建模型bug
    public void InitialTeamModel()
    {
        if(isFromWorldLevel)
        {
           foreach(GameObject obj in models)
                PlayIdle(obj);
            isFromWorldLevel = false;
            return;
        }

        int orderIndex = GameProfile.SharedInstance.Player.teamIndexsOrder[0];
        if (orderIndex>-1)
        {
            models[0] = SpawnModelByOrderIndex(orderIndex);
            ResetCharacter(models[0], modelContainer[0], GameProfile.SharedInstance.CharacterOrder[orderIndex]);

            SetModelLocalPosition(models[0].transform,0);
//            teamCenterModel.transform.
//            UIEventListener.Get(teamCenterModel).onClick = OnCharacterClicked0;
        }
        PlayIdle(models[0]);

        //2、3号位未解锁
        if (GameProfile.SharedInstance.Player.teamIndexsOrder.Count == 1)
        {
            left_lock.SetActive(true);
            left_lockFX.SetActive(true);
            left_enter.SetActive(false);
            right_lock.SetActive(true);
            right_lockFX.SetActive(true);
            right_enter.SetActive(false);
            modelContainer[1].collider.enabled = false;
            modelContainer[2].collider.enabled = false;
            models[1] = null;
            models[2] = null;
            return;
        }
        else if (GameProfile.SharedInstance.Player.teamIndexsOrder.Count == 2)
        {
            left_lock.SetActive(false);
            left_lockFX.SetActive(false);
            left_enter.SetActive(true);
            right_lock.SetActive(true);
            right_lockFX.SetActive(true);
            right_enter.SetActive(false);
            modelContainer[1].collider.enabled = true;
            modelContainer[2].collider.enabled = false;
        }
        else
        {
            right_lock.SetActive(false);
            right_lockFX.SetActive(false);
            left_lock.SetActive(false);
            left_lockFX.SetActive(false);
            left_enter.SetActive(true);
            right_enter.SetActive(true);
            modelContainer[1].collider.enabled = true;
            modelContainer[2].collider.enabled = true;
        }

        int orderIndex1 = GameProfile.SharedInstance.Player.teamIndexsOrder[1];
        if (orderIndex1 > -1 )
        {
            left_enter.SetActive(false);
            models[1] = SpawnModelByOrderIndex(orderIndex1);
            ResetCharacter( models[1], modelContainer[1], GameProfile.SharedInstance.CharacterOrder[orderIndex1]);

            SetModelLocalPosition(  models[1].transform,1);
//            UIEventListener.Get(teamLeftModel).onClick = OnCharacterClicked1;
            
            PlayIdle( models[1]);
        }
        if (GameProfile.SharedInstance.Player.teamIndexsOrder.Count == 2)
        { 
            models[2] = null;
            return;
        }

        int orderIndex2 = GameProfile.SharedInstance.Player.teamIndexsOrder[2];
        if (orderIndex2 > -1)
        {
            right_enter.SetActive(false);;
            models[2] = SpawnModelByOrderIndex(orderIndex2);
            ResetCharacter(models[2], modelContainer[2], GameProfile.SharedInstance.CharacterOrder[orderIndex2]);
            SetModelLocalPosition(models[2].transform,2);

//            UIEventListener.Get(teamRightModel).onClick = OnCharacterClicked2;
            
            PlayIdle(models[2]);
           
        }
        else
        {
            models[2] = null;
        }
    }

    public void SetModelLocalPosition(Transform ts,int _index)
    {
        if(_index == 0)
          ts.localPosition = new Vector3(0f,-0.16f,0f);
        else if(_index == 1)
            ts.localPosition =new Vector3(0f,-0.31f,0f);
        else if(_index == 2)
            ts.localPosition =new Vector3(0f,-0.32f,0f);
}
   
    public GameObject SpawnModelByOrderIndex(int index)
    {
        CharacterStats  activeCharacter = UIManagerOz.SharedInstance.chaSelVC.GetCharacterByOrderIndex(index);
        ProtoCharacterVisual protoVisual =
            GameProfile.SharedInstance.ProtoCharacterVisuals[activeCharacter.protoVisualIndex];
        GameObject prefab = protoVisual.prefab;
        if (prefab != null)
        {
            GameObject model = PoolManager.Pools["characters"].Spawn(prefab.transform).gameObject;
            return model;
        }

        return null;
    }

    public void UpdownStage(int _index,bool isInverse = false)
    {
        if(_index == 0)
        {
            UIDynamically.instance.TopToScreen(modelContainer[0],0f,0.1f,0.2f,isInverse);
        }
        else if(_index == 1)
        {
            UIDynamically.instance.TopToScreen(modelContainer[1],0.092f,0.2f,0.2f,isInverse);
        }
        else if(_index == 2)
        {
            UIDynamically.instance.TopToScreen(modelContainer[2],0.092f,0.2f,0.2f,isInverse);
        }
    }
    //有上升的台子恢复原位
    public void UpdownStage(bool isInverse = true)
    {
        if(modelContainer[0].transform.localPosition.y > 0)
        {
            UIDynamically.instance.TopToScreen(modelContainer[0],0f,0.15f,0.2f,isInverse);
        }
        if(modelContainer[1].transform.localPosition.y > 0.1f)
        {
            UIDynamically.instance.TopToScreen(modelContainer[1],0.092f,0.23f,0.2f,isInverse);
        }
        if(modelContainer[2].transform.localPosition.y > 0.1f)
       
        {
            UIDynamically.instance.TopToScreen(modelContainer[2],0.092f,0.23f,0.2f,isInverse);
        }
    }



    void OnDisable()
    {
//        DespawnTeamModel();
    }

    public void DespawnTeamModel()
    {
        foreach(GameObject obj in models)
           DespawnModel(obj);
      
    }

    private void DespawnModel(GameObject CharacterModel)
    {
        if (CharacterModel != null && CharacterModel.activeSelf
            && PoolManager.Pools.ContainsKey("characters"))
            PoolManager.Pools["characters"].Despawn(CharacterModel.transform, null);
    }

    public void ReadyForIntroAnimation()
    {  

        NGUITools.SetActive(gameObject, true);
      
    }
    
    public void BringInIdolMenu()
    {
        float ratio = (float)Screen.width / (float)Screen.height;
        float ratioMin = 640f / 1136f; 
        float ratioMax = 768f / 1024f; 
        float current = (ratio - ratioMin) / (ratioMax - ratioMin);
        current = Mathf.Clamp01(current);
            
        Invoke("BringInBottomPanel", 2.0f);
        
        iTween.ValueTo(GameController.SharedInstance.gameObject, iTween.Hash(
            "time", 1f,
            "from", 4f,
            "to", 2f,
            "onupdate", "OnUpdateFade",
            "onupdatetarget", GameController.SharedInstance.gameObject
            ));
        //paperViewController.DoIntroAnimation();
        
        if (onMainMenuAnimationCompleteEvent != null)
        {
            onMainMenuAnimationCompleteEvent();
            
        }
    }

    public void BringInBottomPanel()
    {
        notify.Debug("----BringInBottomPanel---");
        //UIManagerOz.SharedInstance.okayDialog.ShowOkayDialog("Msg_Reward", "Btn_Ok");
        PlayerPrefs.SetInt("IS_FIRST_OPEN_REWARD",0);
        GameProfile.SharedInstance.Player.specialCurrencyCount += 3;    
        UIManagerOz.SharedInstance.PaperVC.UpdateCurrency();
        GameProfile.SharedInstance.Serialize();
        ObjectivesDataUpdater.AddToGenericStat(ObjectiveType.CollectSpecialCurrency, 3);
            
        //}

        int isNeedReward = PlayerPrefs.GetInt("IS_REWARD",0);
        if(isNeedReward == 1)
        {
            string text = Localization.SharedInstance.Get ("Toast_Reward0");
            int coin = PlayerPrefs.GetInt("coin",100);
            int gem = PlayerPrefs.GetInt("gem",1);
            if(coin!=0 && gem!=0)
            {
                text=string.Format(Localization.SharedInstance.Get ("Toast_Reward3"), coin,gem);
                
            }else if(coin==0 && gem != 0)
            {
                
                text=string.Format(Localization.SharedInstance.Get ("Toast_Reward2"),gem);
                
            }else if(coin != 0 && gem == 0)
            {
                text=string.Format(Localization.SharedInstance.Get ("Toast_Reward1"),coin);
                
            }
            
            UIManagerOz.SharedInstance.okayDialog.ShowOkayDialog(text, "Btn_Ok");
            GameProfile.SharedInstance.Player.coinCount += coin;
            GameProfile.SharedInstance.Player.specialCurrencyCount += gem;  
            UIManagerOz.SharedInstance.PaperVC.UpdateCurrency();
            GameProfile.SharedInstance.Serialize();
      
        }

//        NewsFeed.GetComponent<HorizontalScrollingLabel>().StartScrollIfNeeded();    // reset news ticket just prior to showing it   
    }

    public void OnMenuClicked()
    {   
       
        if(ItemClicked) return;
        
        ItemClicked = true;

    
        // clear background with color similar to menu UI background, eliminate flashing between screens        
        UIManagerOz.SharedInstance.SetUICameraClearFlagToSolidColorBG(true);        
        
        UIManagerOz.SharedInstance.mainVC.MenuButtonToPress = MenuButtonToPressAfterGameIntro;
        MenuButtonToPressAfterGameIntro = null; // Clear this for next time
        UIManagerOz.SharedInstance.mainVC.appear();
        UIManagerOz.SharedInstance.PaperVC.appear();        
        disappear();

    }


    public void OnPlayClicked(GameObject obj)
    {
        if (ItemClicked || !GameProfile.SharedInstance.Player.GetIsChallengeUnlock()) return;
        
        ItemClicked = true;
        //解锁挑战引导结束
       // if (GameController.SharedInstance.unlockrole2PosTutorialPlayed && !GameController.SharedInstance.unlockChallengeTutorialPlayed && GameProfile.SharedInstance.Player.GetIsChallengeUnlock()) //挑战关
        if (GameController.SharedInstance.GetTutorialIDforSys() == 5 && GameProfile.SharedInstance.Player.GetIsChallengeUnlock())
        {
            //tutorialCharacter.SetActive(false);
            //GameController.SharedInstance.unlockChallengeTutorialPlayed = true;
            //PlayerPrefs.SetInt("unlockChallengeTutorialPlayedInt", 1);
            //PlayerPrefs.Save();
        }
        if (GameController.SharedInstance.gameState != GameState.IN_RUN) // only trigger this once per run
        {
            GameController.SharedInstance.EndlessMode = true;

            int envId = EnvironmentSetManager.SharedInstance.GetRandomEnviroment(true);
            GameController.SharedInstance.SwitchEnviroment(envId);

            UIManagerOz.SharedInstance.inventoryVC.appear();
//            UIManagerOz.SharedInstance.OnPlayClicked();

            disappear(); //newnew
            //FadeOutIdolMenu();
        }
    }   

    float fadeOutTime = 1.5f;
    
    private void FadeOutIdolMenu()
    {
        iTween.ValueTo(gameObject, iTween.Hash(
                "from", 1f,
                "to", 0f,
                "time", fadeOutTime,
                "easetype", iTween.EaseType.easeOutCubic,   //easeInOutSine
                "onupdate", "FadeIdolMenuAlpha",
                "onupdatetarget", gameObject,
                "oncomplete", "HideIdolMenu",
                "oncompletetarget", gameObject,
                "ignoretimescale", true));
        

    }
    
    private void FadeIdolMenuAlpha(float val)
    {
//        foreach (UIPanelAlpha pa in panelAlphas)
//            pa.alpha = val;
       
    }
    
   
    
    private void HideIdolMenu()
    {
        disappear();
        
//        foreach (UIPanelAlpha pa in panelAlphas)
//            pa.alpha = 1f;      // reset panel alphas   
     
     
    }

    
    public void SetNotificationIcon(int buttonID, int iconValue)        // update actual icon onscreen
    {
        notificationIcons.SetNotification(buttonID, iconValue);
    }
    
    public void OnEscapeButtonClicked()
    {
        if( UIManagerOz.escapeHandled ) return;
        UIManagerOz.escapeHandled = true;
        
        UIConfirmDialogOz.onNegativeResponse += CancelExit;
        UIConfirmDialogOz.onPositiveResponse += ExitGame;
        //UIManagerOz.SharedInstance.confirmDialog.ShowConfirmDialog("Msg_LeaveGame","", "Btn_No", "Btn_Yes");
        //      PurchaseUtil.exitGame();
        UIManagerOz.SharedInstance.confirmDialog.ShowConfirmDialog("Msg_LeaveGame", "Btn_No", "Btn_Yes");
    }
    
    public void CancelExit()
    {
        UIConfirmDialogOz.onNegativeResponse -= CancelExit;
        UIConfirmDialogOz.onPositiveResponse -= ExitGame;   
    }
    
    public void ExitGame()  
    {
        mQuitTimer = 0.2f;// wait o prevent screen glitch
        isQuitting = true;
    }   
    
}
