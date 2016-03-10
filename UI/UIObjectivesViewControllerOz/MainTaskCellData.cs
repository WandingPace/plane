using System.Linq;
using UnityEngine;

public class MainTaskCellData : MonoBehaviour
{
    private ObjectiveProtoData _data;
    public GameObject btnFinish;
    public GameObject completed;
    public UISprite costIcon;
    public GameObject down;
    private int oldLv;
    //进度条
    public UISlider progressBar;
    public UILabel titleTxt, descTxt, costTxt, Progresstxt;

    private void Start()
    {
        RegisterEvent();
    }

    private void RegisterEvent()
    {
        if (btnFinish != null)
            UIEventListener.Get(btnFinish).onClick = OnFinishClicked;
    }

    //立即完成
    private void OnFinishClicked(GameObject ob)
    {
        oldLv = GameProfile.SharedInstance.Player.playerLv;

        if (!GameProfile.SharedInstance.Player.objectivesEarned.Contains(_data._id))
        {
            GameProfile.SharedInstance.Player.objectivesEarned.Add(_data._id);
            _data._hasDone = true;
        }

        if (IsCompleted())
        {
            UIDynamically.instance.ZoomOutToOne(completed, new Vector3(2f, 2f, 2f), 0.5f);
        }

        UpdateIcon();

        var canLevelUP = true;
        var dataList = GameProfile.SharedInstance.Player.objectivesMain;
        dataList = dataList.GroupBy(x => x._id).Select(y => y.First()).ToList();
        foreach (var pd in dataList)
        {
            if (!GameProfile.SharedInstance.Player.objectivesEarned.Contains(pd._id))
                //pd._conditionList[0]._earnedStatValue < pd._conditionList[0]._statValue) //未完成
            {
                canLevelUP = false;
            }
        }
        var isLvUp = false;
        //获得经验（此等级经验满了则升一级）
        if (canLevelUP)
            isLvUp = GameProfile.SharedInstance.Player.AddPlayerExp(_data._rewardValue);
        else
            isLvUp = GameProfile.SharedInstance.Player.AddPlayerExpButNoLevelup(_data._rewardValue);


        //刷新用户数据界面
        UIManagerOz.SharedInstance.ObjectivesVC.RefreshUserInfo(isLvUp, canLevelUP); //isLvUp  true

        UIManagerOz.SharedInstance.idolMenuVC.RefreshPlayer();

        //升级
        if (oldLv < 30 && oldLv < GameProfile.SharedInstance.Player.playerLv)
        {
            Invoke("WaitCompleteAnimate", 1.2f);
        }
        else
        {
            //保存数据
            GameProfile.SharedInstance.Serialize();
        }
    }

    private void WaitCompleteAnimate()
    {
        //刷新显示下一等级的主线任务
        UIManagerOz.SharedInstance.ObjectivesVC.objectivesPanelUILists[(int) ObjectivesScreenName.MainTask].Refresh();

        //保存数据
        GameProfile.SharedInstance.Serialize();
    }

    //填充数据
    public void SetData(ObjectiveProtoData data, bool updateProgress = true)
    {
        _data = data;

        if (_data != null)
        {
            Refresh(updateProgress);
        }
    }

    private void Refresh(bool updateProgress = true)
    {
        if (titleTxt != null)
            titleTxt.text = _data._title;
        if (descTxt != null)
            descTxt.text = _data._descriptionPreEarned;
        if (costIcon != null)
            costIcon.spriteName = "common_gem";
        if (costTxt != null)
            costTxt.text = _data._skipValue.ToString();


        //Debug.Log(string.Format(_data._title + "进度:{0}/{1}", _data._conditionList[0]._earnedStatValue , _data._conditionList[0]._statValue));
        if (updateProgress)
            UpdateProgressBar();

        UpdateIcon();
    }

    private void UpdateIcon()
    {
        if (btnFinish != null)
        {
            if (IsCompleted())
            {
                progressBar.value = 1f;
                btnFinish.transform.parent.gameObject.SetActive(false);
                completed.SetActive(true);
                down.SetActive(true);
            }
            else
            {
                btnFinish.transform.parent.gameObject.SetActive(true);
                completed.SetActive(false);
                down.SetActive(false);
            }
        }
        //完成标志
        if (IsCompleted())
        {
            completed.SetActive(true);
            progressBar.value = 1f;
        }
        else
        {
            completed.SetActive(false);
        }
    }

    private void UpdateProgressBar()
    {
        if (progressBar != null)
        {
            if (_data != null && _data._conditionList != null)
            {
                progressBar.value = Mathf.Min(1.0f,
                    (_data._conditionList[0]._earnedStatValue/(float) _data._conditionList[0]._statValue));
                //进度
                if (Progresstxt != null)
                    Progresstxt.text = _data._conditionList[0]._earnedStatValue + "/" +
                                       _data._conditionList[0]._statValue;

                if (_data._hasDone)
                    progressBar.value = 1f;
            }
        }
    }

    //任务是否完成
    private bool IsCompleted()
    {
        if (GameProfile.SharedInstance.Player.objectivesEarned.Contains(_data._id))
            return true;
        return false;
    }
}