using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class PostAccount : MonoBehaviour {


    //结算主界面
    public GameObject PostAccountRoot, PostSuccessbgTop, PostFailbgTop;
    //normal
    public UILabel coin,score,txtExp,highest,medal,medalsilver,medalgold;
    public UISprite expProgressBar;
    public GameObject txtExpTip,txtEarnExpTip;
    //level
    public GameObject level,topstar1,topstar2,topstar3;//关卡
    public GameObject Buttonshieldmask; //按钮屏蔽遮罩
    public UISprite star1,star2,star3,starProgressBar;
    public UILabel bestRecord, currRecord, title, leveldescribe;
    //模型摄像机
    public GameObject modelCamera;
    //三星通关界面
    public GameObject perpectClroot, perpectClstarroot;
    public GameObject perpectClClose;
    public GameObject perpectClpanel;
    public GameObject perpectClstar1, perpectClstar2, perpectClstar3;
    public Transform modelrolatemid,modelPos;
    private GameObject ModelteamCenter;
    //庆祝界面
    public GameObject newRecordroot;
    public GameObject btnRecordClose;
    public GameObject newRecordpanel;
   // public GameObject newRecordPlane;
    private bool bNewRecordend=false;
    private bool bperpectClend = false;
    private bool bMainTaskend = false;
    public UILabel RecordScore;
    //任务结算界面
    public UIObjectivesList MainTasklist;
    public GameObject MainTasktip;
    public GameObject MainTaskPanel;
    public GameObject btnMainTaskClose;
    public List<GameObject> MaintaskObjectiveCells = new List<GameObject>();
    private GameObject[] MainTaskCompleted =new GameObject[3];
    //升级界面
    public Levelup levelup;
    //tasktip
    public GameObject taskTip;
    public UILabel txtTask;
    public UILabel txtReward;
    public GameObject btnContinue;
    //小队飞机图标
    public List<GameObject> teamPlaneicon;
    [HideInInspector]
    public bool next = false;

    //public bool bShownGatcha=false; //经过显示宝箱
    private int exp;
    private bool isLvUp;
    private int oldLv;
    private int newLv;

    void Start()
    {

        UIEventListener.Get(btnContinue).onClick = OnContinueClicked;
        UIEventListener.Get(btnRecordClose).onClick = OnRecordClose;
        UIEventListener.Get(newRecordpanel).onClick = OnRecordClose;
        UIEventListener.Get(perpectClClose).onClick = OnperfectClose;
        UIEventListener.Get(btnMainTaskClose).onClick = OnMainTaskClose;


    }
    void OnEnable()
    {
        modelrolatemid.transform.localEulerAngles = new Vector3(modelrolatemid.transform.localEulerAngles.x, 9f, modelrolatemid.transform.localEulerAngles.z); //初始角度

        for (int i = 0; i < MainTaskCompleted.Length; i++)
        {
            MainTaskCompleted[i] = null;
        }
        for (int i = 0; i < teamPlaneicon.Count; i++)
        {
            if (i < GameProfile.SharedInstance.Player.teamIndexsOrder.Count)
            {
                if (GameProfile.SharedInstance.Player.teamIndexsOrder[i] != -1)
                {
                    teamPlaneicon[i].transform.FindChild("icon").GetComponent<UISprite>().spriteName = GameProfile.SharedInstance.Characters[GameProfile.SharedInstance.Player.teamIndexsOrder[i]].IconName.Replace("game", "role");
                    teamPlaneicon[i].transform.FindChild("title").GetComponent<UILabel>().text = "+" + string.Format("{0:P}", (float)(GameProfile.SharedInstance.currCharactersPropertyDict[GameProfile.SharedInstance.Player.teamIndexsOrder[i]].scoreMultiplier - 10) / 100);
                }
                else
                    teamPlaneicon[i].SetActive(false);
            }
            teamPlaneicon[i].transform.ResetTransformation();
            teamPlaneicon[i].SetActive(false);

        }
    
    }
    //=====================关闭界面============================
    void OnContinueClicked(GameObject obj)
    {
        next = true;
        taskTip.SetActive(false);
    }
    void OnRecordClose(GameObject obj) //关闭刷新纪录界面
    {
        if (bNewRecordend)
        {
           // iTween.Stop();
            modelCamera.SetActive(false); 
            DespawnModel(ModelteamCenter);
            newRecordroot.SetActive(false);



            AudioManager.SharedInstance.StopFX();
            //显示主线任务
            ShowMainTask();
        }

    }
    void OnperfectClose(GameObject obj) //关闭三星庆祝界面
    {
        if (bperpectClend)
        {
            modelCamera.SetActive(false); 
            DespawnModel(ModelteamCenter);
            perpectClroot.SetActive(false);
            if (GameProfile.SharedInstance.Player.GetNumberChanceTokens() <= 0)
            {
                PostAccountRoot.SetActive(true);
                next = true; //继续postaccount结算
            }
            else
                ShowGatch(); //开宝箱
            //显示主线任务
            
        }
    
    }
    void OnMainTaskClose(GameObject obj) //关闭任务界面
    {
        if (bMainTaskend)
        {
            Buttonshieldmask.SetActive(true);
            //显示任务信息
            MainTasktip.SetActive(false);
            isMaintaskclose = false;
        }

    }
    //==================================================================
    //开宝箱
    private void ShowGatch()
    {
        if (GameProfile.SharedInstance.Player.GetNumberChanceTokens() > 0) // if we get chance tokens lets show the gatcha first
        {
          //  NGUIToolsExt.SetActive(this.gameObject, false);
            UIManagerOz.SharedInstance.gatchVC.availableCards = GameProfile.SharedInstance.Player.GetNumberChanceTokens();
            UIManagerOz.SharedInstance.gatchVC.appear();
        }
    }
    //关卡信息
    private void ShowLevelInfo()
    {
        int level = GameProfile.SharedInstance.Player.activeLevel;
        ObjectiveProtoData mdata = ObjectivesManager.LevelObjectives[level];

        UIManagerOz.SharedInstance.worldOfOzVC.SetStarPosition(starProgressBar,star1.gameObject,star2.gameObject,star3.gameObject);
        

        title.text = GetEnvNameByID(mdata._environmentID);
      
       //更新星星
        int starCount =UIManagerOz.SharedInstance.worldOfOzVC.GetStarRank(mdata,true);
        UIManagerOz.SharedInstance.worldOfOzVC.UpdateStarActive(topstar1,topstar2,topstar3,starCount);
        UIManagerOz.SharedInstance.worldOfOzVC.UpdateStarSprite(star1,star2,star3,starCount);
        UIManagerOz.SharedInstance.worldOfOzVC.UpdateStarProgressBar(starProgressBar,mdata,true);
        if (starCount < 1)
        {         
            leveldescribe.text = string.Format("还差" + mdata._descriptionPreEarned + "就能获得下一个星星", (mdata._conditionList[0]._statValue1ForLevel - mdata._conditionList[0]._statValue));
        }
        else if (starCount < 2)
            leveldescribe.text = string.Format("还差" + mdata._descriptionPreEarned + "就能获得下一个星星", (mdata._conditionList[0]._statValue2ForLevel - mdata._conditionList[0]._statValue));
        else if (starCount < 3)
            leveldescribe.text = string.Format("还差" + mdata._descriptionPreEarned + "就能获得下一个星星", (mdata._conditionList[0]._statValue3ForLevel - mdata._conditionList[0]._statValue));
        else
            leveldescribe.text = "您已三星通关，请挑战其他关卡。";

        currRecord.text = mdata._conditionList[0]._earnedStatValue.ToString();

        //返还体力
        if (starCount > UIManagerOz.SharedInstance.worldOfOzVC.oldStarCountBest)
        {
            if (starCount == 1)
            {
                UIManagerOz.SharedInstance.PaperVC.fuelSystem.AddFuelCount(2);//返还燃料
            }
            else if (starCount == 2)
            {
                UIManagerOz.SharedInstance.PaperVC.fuelSystem.AddFuelCount(2);//返还燃料
            }
            else if (starCount == 3)
            {
                UIManagerOz.SharedInstance.PaperVC.fuelSystem.AddFuelCount(2);//返还燃料
            }
        }
        //大关更新奖励
        if (mdata._conditionList[0]._isBigLevel)
        {
           //获得星星数大于历史获得最大星星数
            if (starCount > UIManagerOz.SharedInstance.worldOfOzVC.oldStarCountBest)
            {
                if (starCount == 1)
                {
                    UpdateBigLevelReward(mdata,mdata._conditionList[0]._BigLevelReward1);
                }
                else if (starCount == 2)
                {
                    UpdateBigLevelReward(mdata, mdata._conditionList[0]._BigLevelReward2);
                }
                else
                {
                    UpdateBigLevelReward(mdata, mdata._conditionList[0]._BigLevelReward3);
                }
            }
        
        }
        
        if ( ObjectivesManager.LevelObjectives.Count.Equals(level + 1))
        {
            if (starCount >= 1)
            {
                PostSuccessbgTop.SetActive(true);
                PostFailbgTop.SetActive(false);
                ObjectivesManager.UnlockNewLevel();
                //解锁关卡为大关卡
                if (ObjectivesManager.LevelObjectives[ObjectivesManager.LevelObjectives.Count - 1]._conditionList[0]._isBigLevel)
                {
                    //当前星星数大于需求星星数
                    if (UIManagerOz.SharedInstance.worldOfOzVC.GetStarTotal() >= ObjectivesManager.QuickAccessLevelObjectives[ObjectivesManager.LevelObjectives.Count-1]._conditionList[0]._BigLevelunLockStarNeed)
                    {
                        ObjectivesManager.LevelObjectives[ObjectivesManager.LevelObjectives.Count - 1]._conditionList[0]._isBigLevelUnlock = true;
                    }
                }
                UIManagerOz.SharedInstance.worldOfOzVC.worldList.isUnlockNewLevel = true;
            }
            else if (starCount == 0)
            {
                UIManagerOz.SharedInstance.postGameVC.tutorialUpgrade.SetActive(true);
                PostSuccessbgTop.SetActive(false);
                PostFailbgTop.SetActive(true);
            }
        }
        ObjectivesManager.SaveLevelProgress();
    }

    //更新奖励
    private void UpdateBigLevelReward(ObjectiveProtoData objdata,int num)
    {
        if (objdata._conditionList[0]._BigLevelRewardType == CostType.Special)
        {
            GameProfile.SharedInstance.Player.specialCurrencyCount += num;
            UIManagerOz.SharedInstance.PaperVC.UpdateCurrency();
        }
    
    }
    private string GetEnvNameByID(int id)
    {
        string name = "";
        switch (id)
        {
            case 1: name = "冰雪关"; break;
            case 2: name = "墨西哥关"; break;
            case 3: name = "印度关"; break;
        }
        return name;
    }


    public void SetAccount()
    {
       // bShownGatcha = false;

        oldLv = GameProfile.SharedInstance.Player.playerLv;
        exp = Mathf.RoundToInt(GamePlayer.SharedInstance.Score /200f);
        newLv = GameProfile.SharedInstance.Player.playerLv;
       // isLvUp = GameProfile.SharedInstance.Player.AddPlayerExp(exp); 

        if(!GameController.SharedInstance.EndlessMode)
        {
            next = true;
            ShowLevelInfo();
        }
        else
        {
           next = true;
         
        }
        SetpostAccountData();
        StartCoroutine(coStartAnimSeq());//策划需求，新手教程中,正常显示在游戏中的收益
        //if (GameController.SharedInstance.upgradeTutorialPlayed)
        if (GameController.SharedInstance.GetTutorialIDforSys() > 0)
        {//非新手教程
            Buttonshieldmask.SetActive(true);
          
        }
        else
        {
            //新手教程
            UIManagerOz.SharedInstance.PaperVC.fuelSystem.SetFuelFull();
        }
    }

    void SetpostAccountData() //结算主界面的数据
    {
        txtExpTip.SetActive(false);
        txtEarnExpTip.SetActive(false);
        expProgressBar.transform.GetComponent<UISlider>().value = UIManagerOz.SharedInstance.idolMenuVC.playerInfo.GetExpBar();
        if (GamePlayer.SharedInstance.Score > GameProfile.SharedInstance.Player.bestScore)
            highest.text = GamePlayer.SharedInstance.Score.ToString();
        else
            highest.text = GameProfile.SharedInstance.Player.bestScore.ToString();
    }
    //============================显示三星通关=========================
    private void ShowPerfectClearance()
    {
        bperpectClend = false;
        perpectClstarroot.SetActive(false);
        perpectClroot.SetActive(true);
        perpectClpanel.transform.localScale = Vector3.zero;
        iTween.ScaleTo(perpectClpanel, iTween.Hash(
        "islocal", true,
        "scale", Vector3.one,
         "time", 0.6f,
         "easyType", iTween.EaseType.easeOutCubic
         ));
        Invoke("ShowPerfectClend",0.55f);
    }
    private void ShowPerfectClend()
    {
        AudioManager.SharedInstance.PlayFX(AudioManager.Effects.ui_opening_cheer);
        flyPlane();
        perpectClstarroot.SetActive(true);

        perpectClstar1.GetComponent<UISprite>().alpha = 0f;
        perpectClstar2.GetComponent<UISprite>().alpha = 0f;
        perpectClstar3.GetComponent<UISprite>().alpha = 0f;

        UIDynamically.instance.ZoomOutToOne(perpectClstar1, new Vector3(2f, 2f, 1f), 0.72f);
        UIDynamically.instance.ZoomOutToOne(perpectClstar2, new Vector3(2f, 2f, 1f), 0.72f,0.3f);
        UIDynamically.instance.ZoomOutToOne(perpectClstar3, new Vector3(2f, 2f, 1f), 0.72f,0.6f);


        iTween.ValueTo(gameObject, iTween.Hash(
            "time", 0.72f,
            "from", 0.2f,
             "to", 1f,
            "onupdate", "alphaUpdate",
            "onupdatetarget", gameObject
           ));

        Invoke("ZoomInOut", 1.32f);
    }

    void alphaUpdate(float val)  //星星alpha
    {
        perpectClstar1.GetComponent<UISprite>().alpha = val;
        perpectClstar2.GetComponent<UISprite>().alpha = val;
        perpectClstar3.GetComponent<UISprite>().alpha = val;
    
    }
    void ZoomInOut()// //从1放大到x再从x到1
    {
      UIDynamically.instance.ZoomInOut(perpectClstar1,new Vector3(1.2f,1.2f,1.2f),0.4f);
      UIDynamically.instance.ZoomInOut(perpectClstar2, new Vector3(1.2f, 1.2f, 1.2f), 0.4f);
      UIDynamically.instance.ZoomInOut(perpectClstar3, new Vector3(1.2f, 1.2f, 1.2f), 0.4f);
      Invoke("PerfectClAnimend",0.4f);
    }
    void PerfectClAnimend()
    {
        bperpectClend = true;
        

    }
    //结算飞机飞入
    void flyPlane()
    {
        modelCamera.SetActive(true);
        int orderIndex = GameProfile.SharedInstance.Player.teamIndexsOrder[0];
        ModelteamCenter = SpawnModelByOrderIndex(orderIndex);
        Playanim(ModelteamCenter, "Flight");//播放动作
        ResetModel(ModelteamCenter, modelPos);
          iTween.ValueTo(gameObject, iTween.Hash(
         "time", 1.5f,
         "from", 9f,
         "to", -41.5f,
         "onupdate", "modelflyUpdate",
         "onupdatetarget", gameObject,
         "oncomplete", "modelflyEnd",
         "oncompletetarget", gameObject,
          "easyType", iTween.EaseType.easeOutCubic
         ));
          
    }
    void modelflyUpdate(float val)
    {
        //modelrolatemid.transform.localPosition = modelrolatemid.transform.localPosition;
        modelrolatemid.transform.localEulerAngles = new Vector3(modelrolatemid.transform.localEulerAngles.x, val, modelrolatemid.transform.localEulerAngles.z);
     }
    void modelflyEnd()
    {
        Playanim(ModelteamCenter, "3star");//播放动作
        bNewRecordend = true;
    
    }
     GameObject SpawnModelByOrderIndex(int index) //生成模型
    {
        CharacterStats activeCharacter = UIManagerOz.SharedInstance.chaSelVC.GetCharacterByOrderIndex(index);
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
     private void DespawnModel(GameObject CharacterModel)
     {
         if (CharacterModel != null && CharacterModel.activeSelf
             && PoolManager.Pools.ContainsKey("characters"))
             PoolManager.Pools["characters"].Despawn(CharacterModel.transform, null);
     }
     private void Playanim(GameObject model,string anim)
     {
         Animation animsInGO = model.GetComponentsInChildren<Animation>()[0];
         if (animsInGO != null)
         {
             AnimationClip AnimClip = animsInGO.GetClip(anim);
             if (AnimClip != null)
                 animsInGO.CrossFade(AnimClip.name,0.5f, PlayMode.StopAll);
             else
                 animsInGO.CrossFade("Idle",0.5f, PlayMode.StopAll);
         }
     }

     void ResetModel(GameObject model,Transform planepos)
     {
         if (model != null)
         {
             model.transform.parent = planepos.transform;
             model.transform.ResetTransformation();
             model.SetActive(true);


         }
     
     }
    //========================================================

     //==================显示新纪录庆祝的页面==================
    public void ShowNewRecord()  
    {
        bNewRecordend = false;
     
        newRecordroot.SetActive(true);

        newRecordpanel.transform.localScale = Vector3.zero;


        iTween.ScaleTo(newRecordpanel, iTween.Hash(
          "islocal", true,
          "scale", Vector3.one,
          "oncomplete", "ShowNewRecordend",
          "oncompletetarget", gameObject,
          "time", 1,
          "delay", 0.1f,
         "easyType", iTween.EaseType.easeInOutBounce
        ));




        iTween.ValueTo(gameObject, iTween.Hash(
            "delay",1.5f,
        "time", 0.5f,
        "from", 0f,
         "to", 1f,
        "onupdate", "RecordScoreUpdate",
         "onupdatetarget", gameObject,

        "oncomplete", "ShowRecordScoredend",
          "oncompletetarget", gameObject
       
        ));
    
    }
    public void RecordScoreUpdate(float val) //数字动态显示
    {
        int temp = (int)(GamePlayer.SharedInstance.Score * val);
        RecordScore.text = temp.ToString();
    }
   //public void ShowRecordScoredend()  //庆祝结束
   // {
      

   // }

   public void ShowNewRecordend()
   {
       flyPlane();

   }
   //===============================================
    public void NewHighScoreEffect(){
        iTween.ScaleFrom(highest.gameObject, iTween.Hash(
            "time", 1f,
            "scale",new Vector3(1.7f,1.7f,1f),
            "oncomplete", "AllAnimEnded",
            "oncompletetarget", gameObject,
            "easyType",iTween.EaseType.easeInOutBounce
            ));
        highest.alpha = 0f;
        TweenAlpha.Begin(highest.gameObject, 1f, 1f);
    }

    //===================弹出等级任务流程===============
    //已触发 显示目前主线任务
    public void ShowMainTask()
    {
        bool showMainTaskTip = false;
        int index = 0; //任务索引
        foreach (ObjectiveProtoData pd in GameProfile.SharedInstance.Player.objectivesMain)
        {
            MaintaskObjectiveCells[index].GetComponent<MainTaskCellData>().SetData(pd, false);
            index++;

            //if (GameProfile.SharedInstance.Player.MainTaskearnedStartValue[index-1] > pd._conditionList[0]._earnedStatValue)  //本局积累了任务数据亦显示
            //    showMainTaskTip = true;

            if (pd._conditionList[0]._earnedStatValue >= pd._conditionList[0]._statValue
               && !pd._hasDone)
            {
                MainTaskCompleted[index - 1] = null;
                if (!GameProfile.SharedInstance.Player.objectivesEarned.Contains(pd._id))
                {
                    showMainTaskTip = true;
                    MainTaskCompleted[index - 1] = MaintaskObjectiveCells[index - 1].GetComponent<MainTaskCellData>().completed;
                    MainTaskCompleted[index - 1].SetActive(false);
                }


                // break;
            }
        }

        if (showMainTaskTip)
        {
            //MainTasktip.SetActive(true);
            PopupMainTaskUI();
           
            //MainTasklist.Refresh();
            isMaintaskclose = true; //停止在增加经验前
            StartCoroutine(ShowMainTaskTip());

        }
        else
        {
            isMaintaskclose = false;
            StartCoroutine(ShowMainTaskTip());


            // MainTasktip.SetActive(false);
            next = true;
        }

    }
    void PopupMainTaskUI()
    {
        //游戏开始时任务进度条信息
        for (int i = 0; i < 3; i++)
        {
            if (GameProfile.SharedInstance.Player.objectivesEarned.Contains(GameProfile.SharedInstance.Player.objectivesMain[i]._id))
            {
                //进度已满
                MaintaskObjectiveCells[i].GetComponent<MainTaskCellData>().progressBar.value = 1.0f;
                GameProfile.SharedInstance.Player.objectivesMain[i]._conditionList[0]._earnedStatValue = GameProfile.SharedInstance.Player.objectivesMain[i]._conditionList[0]._statValue;
                MaintaskObjectiveCells[i].GetComponent<MainTaskCellData>().Progresstxt.text = GameProfile.SharedInstance.Player.objectivesMain[i]._conditionList[0]._statValue + "/" + GameProfile.SharedInstance.Player.objectivesMain[i]._conditionList[0]._statValue;
                GameProfile.SharedInstance.Player.MainTaskearnedStartValue[i] = GameProfile.SharedInstance.Player.objectivesMain[i]._conditionList[0]._statValue;
            }
            else
            {
                MaintaskObjectiveCells[i].GetComponent<MainTaskCellData>().progressBar.value =
                    Mathf.Min(1.0f, (GameProfile.SharedInstance.Player.MainTaskearnedStartValue[i] / (float)GameProfile.SharedInstance.Player.objectivesMain[i]._conditionList[0]._statValue));
                //Debug.Log("fillamount" + MaintaskObjectiveCells[i].GetComponent<MainTaskCellData>().progressBar.fillAmount);
                MaintaskObjectiveCells[i].GetComponent<MainTaskCellData>().Progresstxt.text = GameProfile.SharedInstance.Player.MainTaskearnedStartValue[i] + "/" + GameProfile.SharedInstance.Player.objectivesMain[i]._conditionList[0]._statValue;
                //Debug.Log(MaintaskObjectiveCells[i].GetComponent<MainTaskCellData>().Progresstxt.text);
            }
        }

        bMainTaskend = false;

        MainTasktip.SetActive(true);

        MainTaskPanel.transform.localScale = Vector3.zero;
        iTween.ScaleTo(MainTaskPanel, iTween.Hash(
         "islocal", true,
         "scale", Vector3.one,
         "oncomplete", "MainTaskPopupEnd",
         "oncompletetarget", gameObject,
         "time", 1,
         "delay", 0.3f,
        "easyType", iTween.EaseType.easeInOutBounce
        ));
    }
    void MainTaskPopupEnd()
    {
        StartCoroutine(MainTaskProgressAnim());

    }
    //进度条动画
    public IEnumerator MainTaskProgressAnim()
    {

           for (int i = 0; i < 3; i++)
           {
               if (GameProfile.SharedInstance.Player.objectivesMain[i]._conditionList[0]._earnedStatValue != 0)
               {
                   MaintaskObjectiveCells[i].GetComponent<MainTaskCellData>().progressBar.transform.GetComponent<MyUIProgressBar>().StartAnimation(0, GameProfile.SharedInstance.Player.objectivesMain[i]._conditionList[0]._earnedStatValue, GameProfile.SharedInstance.Player.objectivesMain[i]._conditionList[0]._statValue);
                   yield return new WaitForSeconds(0.55f);
                   MainTaskCompleteTipAnim(i);
               }
           }

           yield return new WaitForSeconds(0.4f);
           bMainTaskend = true;
    
    }

    //升级完成动画 
    void MainTaskCompleteTipAnim(int i)
    {

        MaintaskObjectiveCells[i].GetComponent<MainTaskCellData>().Progresstxt.text = (GameProfile.SharedInstance.Player.objectivesMain[i]._conditionList[0]._earnedStatValue).ToString() + "/" + GameProfile.SharedInstance.Player.objectivesMain[i]._conditionList[0]._statValue;
        if (MainTaskCompleted[i] != null)
        {
            UIDynamically.instance.Blink(MaintaskObjectiveCells[i].GetComponent<MainTaskCellData>().progressBar.gameObject, 0.2f);
            UIDynamically.instance.ZoomOutToOne(MainTaskCompleted[i], new Vector3(2f, 2f, 2f), 0.4f);
        }
    }
  

    
 
    //======================主界面流程====================
    private float bestScoreRatio;
    private bool bestScore =false;

   public IEnumerator coStartAnimSeq(){
        Buttonshieldmask.SetActive(true);

        bestScore = false; //是否显示庆祝 防止再次游戏 仍然显示
        score.text = "0";
        // figuring out best score ratio
        int bestScorePoints = GameProfile.SharedInstance.Player.bestScore;
        if(bestScorePoints == 0){
            bestScorePoints = 10;
        }
        if (GameController.SharedInstance.EndlessMode) //无尽模式-刷新纪录页面
        {
            bestScoreRatio = (float)GamePlayer.SharedInstance.Score / (float)bestScorePoints;
            if (bestScoreRatio >= 1f)
            {
                bestScore = true;
                bestScoreRatio = 1f;
            }
        }
        else
        {

             int starCount = UIManagerOz.SharedInstance.worldOfOzVC.GetCurLevelStarCount();
             if (starCount >= WorldOfOzCellData.PerfectClearStar)
             {
                     PostAccountRoot.SetActive(false);          
                     next = false;
                     ShowPerfectClearance();

             }
        
        }

       
       //三星通关阻塞,小队加成阻塞
        while (!next)   
            yield return null;


        AnimateCoin();//金币-勋章-分数

        yield break;
    }

    //飞机得分加成
   IEnumerator PlaneScoreBonus()
   {
       //IsteamiconNext = false;
       Buttonshieldmask.SetActive(true);
       yield return new WaitForSeconds(0.5f);
       //移动
       for (int i = 0; i < teamPlaneicon.Count; i++)
       {
          // teamPlaneicon[i].transform.localScale = new Vector3(1,1,1);
           if (i < GameProfile.SharedInstance.Player.teamIndexsOrder.Count)
           {
               if (GameProfile.SharedInstance.Player.teamIndexsOrder[i] != -1)
               {
                   teamPlaneicon[i].transform.localScale = new Vector3(1, 1, 1);
                   UIDynamically.instance.LeftToScreen(teamPlaneicon[i].gameObject, -150f, -0f, 1f);
                   yield return new WaitForSeconds(1f);
               }

           }

                   
       }
       //缩小
       for (int i = 0; i < teamPlaneicon.Count; i++)
       {
           if (i < GameProfile.SharedInstance.Player.teamIndexsOrder.Count)
           {
               if (GameProfile.SharedInstance.Player.teamIndexsOrder[i] != -1)
               {
                   iTween.MoveTo(teamPlaneicon[i], iTween.Hash(
                        "islocal", false,
                        "position", score.transform.position,
                         "time", 1f,
                          "easyType", iTween.EaseType.easeInCubic));
                   iTween.ScaleTo(teamPlaneicon[i], new Vector3(0f, 0f, 0f), 2);
               }
           }
       }
       yield return new WaitForSeconds(1f);
     
       //IsteamiconNext = true;
   
   }
    public void AnimateExpBar()
    {
        int newProg= GameProfile.SharedInstance.Player.GetPlayerExp();
        int newMaxProg= GameProfile.SharedInstance.Player.GetPlayerLvMaxExpByLv(GameProfile.SharedInstance.Player.playerLv);
        expProgressBar.transform.GetComponent<MyUIProgressBar>().StartAnimation(newLv-oldLv,newProg,newMaxProg);

    }

    public void AnimateCoin()
    {
        iTween.ValueTo(gameObject, iTween.Hash(
            "time", 0.5f,
            "from", 0f,
            "to", 1f,
            "onupdate", "AnimateCoinUpdate",
            "onupdatetarget", gameObject,
            "oncomplete", "ScoreBonus",                 
            "oncompletetarget", gameObject

            ));

    }
    
    
    public void AnimateCoinUpdate(float val){
        
        int temp = (int)(GamePlayer.SharedInstance.CoinCountTotal * val);
        coin.text = temp.ToString();

        AudioManager.SharedInstance.PlayFX(AudioManager.Effects.ui_postgame_coin);

    }


    IEnumerator  ScoreBonus()
    {
        //飞机头像动作
        if (!GameController.SharedInstance.EndlessMode)
        {
            yield return StartCoroutine(PlaneScoreBonus());
        }
        //while (!IsteamiconNext)
        //    yield return null;
        AnimateScore();
        yield break;
    }

    public void AnimateScore(){
        //更新勋章数目
        medal.text = GamePlayer.SharedInstance.MedalCountTotal.ToString();//GameController.SharedInstance.collectedBonusItemPerRun[(int)BonusItem.BonusItemType.Medal].ToString();
        medalsilver.text = GamePlayer.SharedInstance.MedalSilverCountTotal.ToString();// GameController.SharedInstance.collectedBonusItemPerRun[(int)BonusItem.BonusItemType.MedalSilver].ToString();
        medalgold.text = GamePlayer.SharedInstance.MedalGoldCountTotal.ToString();//GameController.SharedInstance.collectedBonusItemPerRun[(int)BonusItem.BonusItemType.MedalGold].ToString();
        iTween.ValueTo(gameObject, iTween.Hash(
            "time", 0.5f,
            "from", 0f,
            "to", 1f,
            "onupdate", "AnimateScoreUpdate",
            "onupdatetarget", gameObject,
            "oncomplete", "ScoreUpdateEnd",
            "oncompletetarget", gameObject
            ));
    }

    void ScoreUpdateEnd()
    {
        if (bestScore)
        {
            if (GameController.SharedInstance.EndlessMode)
            {
                NewHighScoreEffect();  //刷新纪录
                ShowNewRecord(); //显示庆祝界面
            }

        }
        else
        {
            //任务结算
            //显示任务信息
            ShowMainTask();


            // AllAnimEnded();
        }
        
    }
    
    public void AnimateScoreUpdate(float val)
    {

        int temp = (int)(GamePlayer.SharedInstance.Score * val);
        score.text = temp.ToString();

        //AudioManager.SharedInstance.PlayFX(AudioManager.Effects.ui_postgame_score);

    }
    
  
    void AllAnimEnded()
    {
        Time.timeScale = 1f;
        UIManagerOz.SharedInstance.postGameVC.speedUpButton.collider.enabled = false;

        GameController.SharedInstance.UpdateAndSaveRecords();
        //Final place
        
        ProfileManager.SharedInstance.UpdateProfile();
        
        bool leaderboardTest = Settings.GetBool( "Leaderboard-Test", false );
        
        if ( leaderboardTest )
        {
            Initializer.SharedInstance.GetInitFromServer();
        }
        
        // refresh local leaderboard positions and notifications, in case player's run changed their ranks in score and/or distance
        LeaderboardManager leaderboardManager = Services.Get<LeaderboardManager>();
        leaderboardManager.RefreshLeaderboards();
        leaderboardManager.GetTopDistances(UIManagerOz.SharedInstance.leaderboardVC.GetkLeaderboardLabelCount(), true);
        leaderboardManager.GetTopScores(UIManagerOz.SharedInstance.leaderboardVC.GetkLeaderboardLabelCount(), true);

    }

    void OnDisable()
    {
        iTween.Stop(gameObject);
        GameProfile.SharedInstance.Serialize();
    }
   
    int showCount;

    //int CompleteMainTaskCount;
    int exptotal = 0;
    //===========主线任务完成提示及增加经验==============
    IEnumerator ShowMainTaskTip()
    {
        while (isMaintaskclose)
            yield return null;

        //CompleteMainTaskCount = 0;
        //if (MainTasktip.activeSelf)
        //{
            //foreach (ObjectiveProtoData pd in GameProfile.SharedInstance.Player.objectivesMain)
            //{
            //    //if (pd._conditionList[0]._earnedStatValue >= pd._conditionList[0]._statValue
            //    //&& !pd._hasDone)
            //    {
            //        ++CompleteMainTaskCount;
            //    }
            //}

            List<ObjectiveProtoData> dataList = GameProfile.SharedInstance.Player.objectivesMain;
            dataList = dataList.GroupBy(x => x._id).Select(y => y.First()).ToList();
            int rewardValuetotal = 0;

            bool canLevelUP = true;
            foreach (ObjectiveProtoData pd in dataList)
            {
                if (pd._conditionList[0]._earnedStatValue >= pd._conditionList[0]._statValue
                   && !pd._hasDone)
                {
                    if (!GameProfile.SharedInstance.Player.objectivesEarned.Contains(pd._id))
                    {
                        GameProfile.SharedInstance.Player.objectivesEarned.Add(pd._id);
                        pd._hasDone = true;

                        rewardValuetotal += pd._rewardValue;
                        exptotal += pd._rewardValue;  //本局所获得经验

                    }


                }
                if (!GameProfile.SharedInstance.Player.objectivesEarned.Contains(pd._id))
                {
                    canLevelUP = false;
                }
            }



            AnimtaskDegreeOfcompletion();



            if (canLevelUP)
            {
                //获得经验（此等级经验满了则升一级）
                if (GameProfile.SharedInstance.Player.AddPlayerExp(rewardValuetotal + exp))   //升级
                {
                    //升级屏蔽按钮，等待升级庆祝界面弹出
                    Buttonshieldmask.SetActive(true);

                    iTween.ValueTo(gameObject, iTween.Hash(
                     "time", 0.5f,
                     "from", 0f,
                     "to", 1f,
                     "onupdate", "AnimateAfterShowntipUpdate",
                     "oncomplete", "PopupLevelUpUI",
                     "onupdatetarget", gameObject
                     ));

                }
                else
                {
                    iTween.ValueTo(gameObject, iTween.Hash(
                            "time", 0.5f,
                             "from", 0f,
                             "to", 1f,
                             "onupdate", "AnimateAfterShowntipUpdate",
                        // "oncomplete", "PopupLevelUpUI",
                             "onupdatetarget", gameObject
                     ));
                }
            }
            else
            {
                Buttonshieldmask.SetActive(false);


                if (GameProfile.SharedInstance.Player.AddPlayerExpButNoLevelup(rewardValuetotal + exp))  //升级rewardValuetotal+exp
                {
                    //当前等级最大经验 当前角色经验和当局经验
                    int LevelUpexptotal = GameProfile.SharedInstance.Player.GetPlayerLvMaxExpByLv(GameProfile.SharedInstance.Player.playerLv) - GameProfile.SharedInstance.Player.GetPlayerExp() - exp;
                    //debug
                    Debug.Log("LevelUpexptotal" + LevelUpexptotal);
                    exptotal = LevelUpexptotal;
                    iTween.ValueTo(gameObject, iTween.Hash(
                     "time", 0.5f,
                     "from", 0f,
                     "to", 1f,
                     "onupdate", "AnimateAfterShowntipUpdate",
                      "oncomplete", "showExpfullTip",
                     "onupdatetarget", gameObject
                    ));
                }
                else
                {
                    //test--
                    //---
                    iTween.ValueTo(gameObject, iTween.Hash(
                        "time", 0.5f,
                        "from", 0f,
                        "to", 1f,
                        "onupdate", "AnimateAfterShowntipUpdate",
                   //      "oncomplete", "PopupLevelUpUI",
                        "onupdatetarget", gameObject
                        ));

                }



            }
            newLv = GameProfile.SharedInstance.Player.playerLv;
            AnimateExpBar();  //刷新任务条
        

        yield break;
    
    }
    private bool isMaintaskclose = false; //等级任务栏关闭再增加经验
    void AnimateAfterShowntipUpdate(float val)   //任务窗口显示完成经验值增加
    {
        int texp = (int)(exptotal * val);
        txtExp.text = (exp + texp).ToString();
    }

    //经验条增加完升级时弹出升级啦界面
    void PopupLevelUpUI()  
    {
        levelup.PopupLevelUpUI();
        
    }
    void showExpfullTip()
    { 

        
       txtExpTip.SetActive(true);
       txtEarnExpTip.SetActive(true);
       UIDynamically.instance.Blink(txtExpTip, 0.2f);
       UIDynamically.instance.Blink(txtEarnExpTip, 0.2f);
    }

    //任务条完成度动画
    void AnimtaskDegreeOfcompletion()
    {
        AllAnimEnded();
     
    }

    ////完成任务提示//
    //IEnumerator ShowTaskTip()
    //{
    //    showCount = 0;
    //    if(MainTasktip.activeSelf)
    //    {
    //        foreach(ObjectiveProtoData pd in GameProfile.SharedInstance.Player.objectivesMain)
    //        {
    //            if(pd._conditionList[0]._earnedStatValue>=pd._conditionList[0]._statValue
    //              && !pd._hasDone)
    //            {
    //                ++showCount;
    //            }
    //        }
    //        foreach(ObjectiveProtoData pd in GameProfile.SharedInstance.Player.objectivesDaily)
    //        {
    //            if(pd._conditionList[0]._earnedStatValue>=pd._conditionList[0]._statValue
    //               &&!pd._hasDone)
    //            {
    //                ++showCount;
    //            }
    //        }

    //        List<ObjectiveProtoData> dataList = GameProfile.SharedInstance.Player.objectivesMain;
    //        dataList = dataList.GroupBy( x => x._id ).Select( y => y.First() ).ToList();
    //        foreach(ObjectiveProtoData pd in dataList)
    //        {   
    //            if(pd._conditionList[0]._earnedStatValue>=pd._conditionList[0]._statValue
    //               && !pd._hasDone)
    //            {
    //                if(!GameProfile.SharedInstance.Player.objectivesEarned.Contains(pd._id))
    //                {
    //                    GameProfile.SharedInstance.Player.objectivesEarned.Add(pd._id);
    //                    pd._hasDone = true;
    //                }
    //                //获得经验（此等级经验满了则升一级）
    //                GameProfile.SharedInstance.Player.AddPlayerExp(pd._rewardValue);
    //                exp += pd._rewardValue;
    //                SetTaskContent(pd);
    //                FadeInTask();
    //                while(isTaskAinm)
    //                    yield return null;
    //            }
    //        }

    //        foreach(ObjectiveProtoData pd in GameProfile.SharedInstance.Player.objectivesDaily)
    //        {
    //            if(pd._conditionList[0]._earnedStatValue>=pd._conditionList[0]._statValue
    //               && !pd._hasDone)
    //            {
    //                switch(pd._rewardType )
    //                {
    //                case RankRewardType.Coins: 
    //                    GameProfile.SharedInstance.Player.coinCount +=pd._rewardValue;
    //                    UIManagerOz.SharedInstance.PaperVC.UpdateCurrency();
    //                    break;
    //                case RankRewardType.Gems: 
    //                    GameProfile.SharedInstance.Player.specialCurrencyCount +=pd._rewardValue;
    //                    UIManagerOz.SharedInstance.PaperVC.UpdateCurrency();
    //                    break;
                        
    //                }
    //                //标记已领取
    //                pd._hasDone = true;
    //                ////完成日常任务
    //                ObjectivesDataUpdater.AddToGenericStat(ObjectiveType.DailyTaskFinished, 1);

    //                SetTaskContent(pd);
    //                FadeInTask();
    //                while(isTaskAinm)
    //                    yield return null;
    //            }
    //        }
    //    }

    //    yield break;
    //}
    //int count=0;
    //bool isTaskAinm = false;
    //void FadeInTask()
    //{  
    //    ++count;
    //    isTaskAinm = true;
    //    iTween.ValueTo(gameObject, iTween.Hash(
    //        "time", 1f,
    //        "from", 0f,
    //        "to", 1f,
    //        "onupdate", "AnimateTaskUpdate",
    //        "onupdatetarget", gameObject,
    //        "oncomplete","FadeOutTask",
    //        "oncompletetarget",gameObject
    //        ));

    //}

    //void FadeOutTask()
    //{
    //    if(showCount==count)
    //    {
    //        isTaskAinm =false;
    //        return;
    //    }

    //    iTween.ValueTo(gameObject, iTween.Hash(
    //        "time", 1f,
    //        "from", 1f,
    //        "to", 0f,
    //        "onupdate", "AnimateTaskUpdate",
    //        "onupdatetarget", gameObject,
    //        "oncomplete","FadeOutTaskFinished",
    //        "oncompletetarget",gameObject
    //        ));

    //}

    //void FadeOutTaskFinished()
    //{
    //    isTaskAinm = false;
    //}

    //void SetTaskContent(ObjectiveProtoData pd)
    //{
    //    txtTask.text = pd._title;
    //    txtReward.text = pd._rewardValue +GetTypeDesc(pd._rewardType);
    //}

    //string GetTypeDesc(RankRewardType type)
    //{
    //    string desc = "";
    //    switch(type)
    //    {
    //        case RankRewardType.Exp: desc = "经验";break;
    //        case RankRewardType.Coins: desc ="金币";break;
    //        case RankRewardType.Gems: desc = "钻石";break;
    //        default: desc="";break;
    //    }
    //    return desc;
    //}
    
    //void AnimateTaskUpdate(float val)
    //{
    //    txtTask.alpha = val;
    //    txtReward.alpha = val;
    //}


 }

