using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DailyTaskCellData : MonoBehaviour {

    public UISprite costIcon,getIcon;
    public UISlider progressBar;
    public UILabel costCount, descTxt, getCount,Progresstxt;
    public GameObject btnReward;
    public GameObject btnRefresh;
    public GameObject getedMark;

    private ObjectiveProtoData _data;
    private static List<DailyTaskCellData> dailyTasks = new List<DailyTaskCellData>();
    private int []refreshCost =new int[]{1,1,1,2,2,2,3,3,3,4,4,4,5,5,5,6,6,6,7,7,7,8,8,8,9,9,9,10,10,10,11,11,11,12,12,12,14,14,14,15};

	void Start () 
    {
        RegisterEvent();
	}
	
    void RegisterEvent()
    {
        if (btnReward!=null)
         UIEventListener.Get(btnReward).onClick = OnGetRewardClicked;
        if (btnRefresh!=null)
         UIEventListener.Get(btnRefresh).onClick = OnGetRefreshClicked;
    }

    void OnGetRewardClicked(GameObject obj)
    {
        if(IsCompleted())
        {
            switch(_data._rewardType )
            {
                case RankRewardType.Coins: 
                    GameProfile.SharedInstance.Player.coinCount +=_data._rewardValue;
                    UIManagerOz.SharedInstance.PaperVC.UpdateCurrency();
                    break;
                case RankRewardType.Gems: 
                    GameProfile.SharedInstance.Player.specialCurrencyCount +=_data._rewardValue;
                    UIManagerOz.SharedInstance.PaperVC.UpdateCurrency();
                    break;

            }

            //领取 刷新按钮变灰
            btnRefresh.collider.enabled = false;
            costIcon.spriteName = "common_gem_grey";
            btnRefresh.GetComponent<UISprite>().spriteName = "task_refresh_grey";
            //标记已领取
            _data._hasDone = true;

            GameProfile.SharedInstance.Serialize();
            UpdateIcon();
            ////完成日常任务
            ObjectivesDataUpdater.AddToGenericStat(ObjectiveType.DailyTaskFinished, 1);
        }
        else
        {

        }

    }

    void OnGetRefreshClicked(GameObject obj)
    {
        //每天最多刷新40次任务
        if(GameProfile.SharedInstance.Player.todayRefreshTimes == refreshCost.Length-1)
        {
            UIManagerOz.SharedInstance.okayDialog.ShowOkayDialog("Msg_RefreshLimit","Btn_Ok");
            return ;
        }

        //钻石不足
        if(refreshCost[GameProfile.SharedInstance.Player.todayRefreshTimes]>GameProfile.SharedInstance.Player.specialCurrencyCount)
        {
            UIManagerOz.SharedInstance.StoreVC.BuyGems();
            return;
        }

        GameProfile.SharedInstance.Player.specialCurrencyCount -= refreshCost[GameProfile.SharedInstance.Player.todayRefreshTimes];

        UIDynamically.instance.Blink(descTxt.gameObject,0.5f);
       
        ++GameProfile.SharedInstance.Player.todayRefreshTimes;

        UIManagerOz.SharedInstance.PaperVC.UpdateCurrency();

        //刷新当前任务
        SetData(GameProfile.SharedInstance.Player.RefillObjectiveForDailyByIndex(int.Parse(gameObject.name)));

        //更新 刷新任务所需钻石数量
        foreach(DailyTaskCellData dt  in dailyTasks)
            dt.costCount.text = refreshCost[GameProfile.SharedInstance.Player.todayRefreshTimes].ToString();

        GameProfile.SharedInstance.Serialize();
    }



    //填充数据
    public void SetData(ObjectiveProtoData data)
    {
        _data = data;

        if(_data!=null)
        {
            Refresh();
        }
        // 添加日常任务项
        if(!dailyTasks.Contains(this))
            dailyTasks.Add(this);
    }

    //
    void Refresh()
    {
        if (_data._hasDone)
        {
            //领取 刷新按钮变灰
            btnRefresh.collider.enabled = false;
            costIcon.spriteName = "common_gem_grey";
            btnRefresh.GetComponent<UISprite>().spriteName = "task_refresh_grey";
        }
        if (costCount!=null)
        costCount.text = refreshCost[GameProfile.SharedInstance.Player.todayRefreshTimes].ToString();//_data._skipValue.ToString();
        if(descTxt!=null)
        descTxt.text = _data._title;
        if(getIcon!=null)
        getIcon.spriteName = ObjectivesManager.GetRewardIconSpriteName((int)_data._rewardType );
        if (getCount!=null)
        getCount.text = _data._rewardValue.ToString();

        UpdateIcon();

        UpdateProgressBar();
    }
    
    void UpdateIcon()
    {
        if (getedMark != null)
        {
            if (isRewardGeted())
            {
                
                getedMark.SetActive(true);
                if (btnReward != null)
                {
                    btnReward.SetActive(false);
                    getIcon.gameObject.SetActive(false);
                    getCount.gameObject.SetActive(false);
                }
            }
            else
            {
                
                getedMark.SetActive(false);
                if (btnReward != null)
                {
                    btnReward.SetActive(true);
                    getIcon.gameObject.SetActive(true);
                    getCount.gameObject.SetActive(true);
                }
            }
        }
    }

    private void UpdateProgressBar()
    {
        if(progressBar!=null)
        {
            if(_data!=null && _data._conditionList!=null)
            {
                progressBar.value = Mathf.Min(1.0f, (_data._conditionList[0]._earnedStatValue / (float)_data._conditionList[0]._statValue));
                //进度
                if (Progresstxt!=null)
                Progresstxt.text = _data._conditionList[0]._earnedStatValue+"/"+_data._conditionList[0]._statValue;
                if(isRewardGeted())
                    progressBar.value = 1f;
            }
        }
    }
    
    //任务是否完成
    bool IsCompleted()
    {
       if(_data._conditionList[0]._earnedStatValue >= _data._conditionList[0]._statValue)
            return true;
        return false;
    }
    //奖励是否已领取
    bool isRewardGeted()
    {
        if(_data._hasDone)
            return true;
        return false;
    }



}
