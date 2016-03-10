using UnityEngine;

public class UIWorldOfOzViewControllerOz : UIViewControllerOz
{
    public UILevelInfo biglevelInfo;
    public GameObject btnBack;
    public UILevelInfo levelInfo;
    private NotificationIcons notificationIcons;
    public UILabel StarGet_txt;
    public UIWorldOfOzList worldList;
    public LevelDialogData levelDialogsData;
    public int oldStarCountBest=0;
    public GameObject tutorialFinger;
    protected override void Awake()
    {
        base.Awake();
        notificationIcons = transform.GetComponent<NotificationIcons>();
    }

    protected override void RegisterEvent()
    {
        UIEventListener.Get(btnBack).onClick = OnHomeClicked;
    }

    private void OnHomeClicked(GameObject obj)
    {
        worldList.disappear();
        UIManagerOz.SharedInstance.idolMenuVC.isFromWorldLevel = true;
        UIManagerOz.SharedInstance.PaperVC.OnHomeClicked(gameObject);
    }

    private void OnGUI()
    {
        if (GUI.Button(new Rect(20, 250, 100, 30), "unlock next"))
        {


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
            ObjectivesManager.SaveLevelProgress();
            UIManagerOz.SharedInstance.worldOfOzVC.worldList.isUnlockNewLevel = true;
            worldList.Refresh();
        }
        if (GUI.Button(new Rect(20, 300, 100, 30), "unlock all"))
        {
            ObjectivesManager.UnlockAllBigLevel();
            ObjectivesManager.SaveLevelProgress();
            worldList.Refresh();
            worldList.RefreshMapCellDate();
        }
    }

    public override void appear()
    {
        UIManagerOz.SharedInstance.PaperVC.topbg.SetActive(true);
        StarGet_txt.text = UIManagerOz.SharedInstance.worldOfOzVC.GetStarTotal().ToString();
        UIDynamically.instance.TopToScreen(btnBack, 720f, 590f, 0.5f);
        base.appear();
        GameController.SharedInstance.MenusEntered();
        UIManagerOz.SharedInstance.PaperVC.SetCurrentPage(UIManagerOz.SharedInstance.worldOfOzVC);
        worldList.Refresh();
        levelInfo.disappear(gameObject);


    }

    public void SetNotificationIcon(int buttonID, int iconValue) // update actual icon onscreen
    {
        if (notificationIcons != null)
            notificationIcons.SetNotification(buttonID, iconValue);
    }

    public int GetStarRank(ObjectiveProtoData mdta, bool isPerRun = false)
    {
        var count = 0;
        var con = mdta._conditionList[0];
        var getVal = isPerRun ? con._statValue : con._earnedStatValue;

        // Debug.Log("star " + getVal);
        if (getVal < con._statValue1ForLevel)
        {
            count = 0;
        }
        else if (getVal >= con._statValue1ForLevel
                 && getVal < con._statValue2ForLevel)
        {
            count = 1;
        }
        else if (getVal >= con._statValue2ForLevel
                 && getVal < con._statValue3ForLevel)
        {
            count = 2;
        }
        else
        {
            count = 3;
        }
        return count;
    }

    //获得当前关卡的星星数
    public int GetCurLevelStarCount(bool isPerRun = true)
    {
        var level = GameProfile.SharedInstance.Player.activeLevel;
        var mdata = ObjectivesManager.LevelObjectives[level];
        var starCount = UIManagerOz.SharedInstance.worldOfOzVC.GetStarRank(mdata, isPerRun);

        return starCount;
    }

    //获得总星星数
    public int GetStarTotal()
    {
        var total = 0;
        foreach (var op in ObjectivesManager.LevelObjectives)
        {
            total += UIManagerOz.SharedInstance.worldOfOzVC.GetStarRank(op);
        }
        return total;
    }

    public void UpdateStarActive(GameObject sr1, GameObject sr2, GameObject sr3, int starC)
    {
        if (starC == 0)
        {
            sr1.SetActive(false);
            sr2.SetActive(false);
            sr3.SetActive(false);
        }
        else if (starC == 1)
        {
            sr1.SetActive(true);
            sr2.SetActive(false);
            sr3.SetActive(false);
        }
        else if (starC == 2)
        {
            sr1.SetActive(true);
            sr2.SetActive(true);
            sr3.SetActive(false);
        }
        else if (starC == 3)
        {
            sr1.SetActive(true);
            sr2.SetActive(true);
            sr3.SetActive(true);
        }
    }

    public void SetStarPosition(UIWidget tprogressBar,GameObject tstar1,GameObject tstar2,GameObject tstar3,bool withParent = false)
    {
        float minBarPos = -(tprogressBar.width/2);
        float maxBarPos = (tprogressBar.width/2);
        //        float starR = star1.width/2;
        
//                float t1 = _data._conditionList[0]._statValue1ForLevel*1.0f/_data._conditionList[0]._statValue3ForLevel;
        float star1Pos = Mathf.Lerp(minBarPos,maxBarPos,1/3f);
        //        star1Pos +=starR;
        
        //        float t2 = _data._conditionList[0]._statValue2ForLevel*1.0f/_data._conditionList[0]._statValue3ForLevel;
        float star2Pos = Mathf.Lerp(minBarPos,maxBarPos,2/3f);
        //        star2Pos +=starR;
        if(withParent)
        {
            tstar1.transform.parent.SetLocalPositionX(star1Pos);
            tstar2.transform.parent.SetLocalPositionX(star2Pos);
            tstar3.transform.parent.SetLocalPositionX(maxBarPos);
        }
        else
        {
            tstar1.transform.SetLocalPositionX(star1Pos);
            tstar2.transform.SetLocalPositionX(star2Pos);
            tstar3.transform.SetLocalPositionX(maxBarPos);
        }
       
    }


    public float UpdateStarProgressBar(UISprite starProgressBar, ObjectiveProtoData mdata, bool isPerRun = false)
    {
//        var starCount = UIManagerOz.SharedInstance.worldOfOzVC.GetStarRank(mdata, isPerRun);
        float val = isPerRun ? mdata._conditionList[0]._statValue : mdata._conditionList[0]._earnedStatValue;
        //float total =(float) mdata._conditionList[0]._statValue3ForLevel;
        //float third = total/3;

        float interval= 0 ;

        float ratio = 0;
        if(val < mdata._conditionList[0]._statValue1ForLevel)
        {
            ratio = 1/3f*val/(float) mdata._conditionList[0]._statValue1ForLevel;
        }
        else if(val == mdata._conditionList[0]._statValue1ForLevel)
        {
            ratio =1/3f;
        }
        else if(val < mdata._conditionList[0]._statValue2ForLevel)
        {
            interval = (float)(mdata._conditionList[0]._statValue2ForLevel-mdata._conditionList[0]._statValue1ForLevel);
            ratio = 1/3f+1/3f*(val-mdata._conditionList[0]._statValue1ForLevel)/interval;
        }
        else if(val == mdata._conditionList[0]._statValue2ForLevel)
        {
            ratio = 2/3f;
        }
        else if(val < mdata._conditionList[0]._statValue3ForLevel)
        {
            interval = (float)(mdata._conditionList[0]._statValue3ForLevel-mdata._conditionList[0]._statValue2ForLevel);
            ratio = 2/3f+1/3f* (val-mdata._conditionList[0]._statValue2ForLevel)/interval;
        }
        else
        {
            ratio = 1f;
        }

        if(starProgressBar !=null)
            starProgressBar.fillAmount  = ratio;

        return ratio;
            //normal
//        float val = isPerRun ? mdata._conditionList[0]._statValue : mdata._conditionList[0]._earnedStatValue;
//        starProgressBar.fillAmount = val*1.0f/mdata._conditionList[0]._statValue3ForLevel;
    }

    public void UpdateStarSprite(UISprite sr1, UISprite sr2, UISprite sr3, int starC)
    {
        var starTxt = "level_rate_star";
        var star1Txt = "level_rate_star1";

        if (starC == 0)
        {
            sr1.spriteName = star1Txt;
            sr2.spriteName = star1Txt;
            sr3.spriteName = star1Txt;
        }
        else if (starC == 1)
        {
            sr1.spriteName = starTxt;
            sr2.spriteName = star1Txt;
            sr3.spriteName = star1Txt;
        }
        else if (starC == 2)
        {
            sr1.spriteName = starTxt;
            sr2.spriteName = starTxt;
            sr3.spriteName = star1Txt;
        }
        else if (starC == 3)
        {
            sr1.spriteName = starTxt;
            sr2.spriteName = starTxt;
            sr3.spriteName = starTxt;
        }
    }
}