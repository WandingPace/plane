using UnityEngine;
using System.Collections;

public class AchieveCellData : MonoBehaviour {

    public UILabel titleTxt,descTxt,rewardTxt,progressTxt;
    public UISprite icon,rewardIcon,headboxicon;
    public UISlider progressBar;
    public GameObject btnReward;
    public GameObject getedMark;
    public GameObject doingMark;

    private ObjectiveProtoData _data;

    void Start () 
    {
        RegisterEvent();
    }
    
    void RegisterEvent()
    {
        UIEventListener.Get(btnReward).onClick = OnGetRewardClicked;
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

//            if(GameProfile.SharedInstance.Player.objectivesUnclaimed.Contains(_data._id))
//                    GameProfile.SharedInstance.Player.objectivesUnclaimed.Remove(_data._id);

            Services.Get<NotificationSystem>().ClearOneShotNotification(OneShotNotificationType.LegendaryChallengeCompleted);
            Services.Get<NotificationSystem>().SetNotificationIconsForThisPage(UiScreenName.IDOL);

            if(!GameProfile.SharedInstance.Player.legendaryObjectivesEarned.Contains(_data._id))
            {
                GameProfile.SharedInstance.Player.legendaryObjectivesEarned.Add(_data._id);
            }
            GameProfile.SharedInstance.Serialize();
            UpdateIcon();

            //StartCoroutine(PlayAnim());

            UIManagerOz.SharedInstance.statsVC.list.Refresh();
        }

        
    }

    IEnumerator PlayAnim()
    {
        UIDynamically.instance.LeftToScreen(gameObject,800,0,0.5f,true);
        yield return new WaitForSeconds(0.5f);
        UIManagerOz.SharedInstance.statsVC.list.dataList= Services.Get<ObjectivesManager>().FlitrateLegendaryObjective();
        UIDynamically.instance.LeftToScreen(gameObject,-800,0,0.5f);
        UIManagerOz.SharedInstance.statsVC.list.Refresh();
    }


    //填充数据
    //
    public void SetData(ObjectiveProtoData data)
    {
        _data = data;
        
        if(_data!=null)
        {
            Refresh();
        }
        
    }
    
    //
    void Refresh()
    {
//        titleTxt.GetComponent<UILocalize>().SetKey(_data._title);
//        descTxt.GetComponent<UILocalize>().SetKey(_data._descriptionEarned);
        switch ((int)_data._difficulty)
        {
            case 1:
                headboxicon.gameObject.SetActive(false);
                break;
            case 2:
                headboxicon.spriteName = "achieve_silverbox";
                headboxicon.gameObject.SetActive(true);
                break;
            case 3:
                headboxicon.spriteName = "achieve_goldbox";
                headboxicon.gameObject.SetActive(true);
                break;

        }
        icon.spriteName = _data._iconNameEarned;
        titleTxt.text = _data._title;
        descTxt.text = _data._descriptionPreEarned;
        rewardIcon.spriteName = ObjectivesManager.GetRewardIconSpriteName((int)_data._rewardType );
        rewardTxt.text = _data._rewardValue.ToString();
        progressTxt.text = _data._conditionList[0]._earnedStatValue+"/"+_data._conditionList[0]._statValue;

        UpdateIcon();

        UpdateProgressBar();
    }
    
    void UpdateIcon()
    {
        if(isRewardGeted())
        {
            btnReward.SetActive(false);
            doingMark.SetActive(false);
            getedMark.SetActive(true);
        }
        else
        {
            if(IsCompleted())
            {
                btnReward.SetActive(true);
                getedMark.SetActive(false);
                doingMark.SetActive(false);
            }
            else
            {
                btnReward.SetActive(false);
                getedMark.SetActive(false);
                doingMark.SetActive(true);
            }
        }
    }
    
    private void UpdateProgressBar()
    {
        if(progressBar!=null)
        {
            if(_data!=null && _data._conditionList!=null)
            {
                int overrideVal = GameProfile.SharedInstance.Player.legendaryProgress[_data._id];
                progressBar.value = Mathf.Min(1.0f, (overrideVal / (float)_data._conditionList[0]._statValue));
                
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
        if(GameProfile.SharedInstance.Player.legendaryObjectivesEarned.Contains(_data._id))
            return true;
        return false;
    }
	
}
