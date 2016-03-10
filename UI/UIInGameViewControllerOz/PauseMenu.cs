using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public enum GamePauseMode
{
   EndlessModePause,
   LevelModePause

}
public class PauseMenu : MonoBehaviour 
{
   // public GamePauseMode pausemode;
	//public UIInGameViewControllerOz inGameVC;	
    public GameObject LevelcontinueButton, EndlesscontinueButton;
    public GameObject LevelhomeButton, EndlesshomeButton;
    public GameObject EndlessModecenter;
    public GameObject LevelModecenter;
    //无尽模式暂停界面
    public List<UISprite> tabs = new List<UISprite>();
    public List<GameObject> objectivesPanelUILists = new List<GameObject>();

    public List<GameObject> MaintaskObjectiveCells = new List<GameObject>();
    public List<GameObject> DailytaskObjectiveCells = new List<GameObject>();


    public UILabel Levelcoin, Levelscore, Levelmedal, Levelmedalsilver, Levelmedalgold;
    public UILabel Endlesscoin, Endlessscore, Endlessmedal, Endlessmedalsilver, Endlessmedalgold;

    //关卡模式
    public UILevelInfo levelinfo;
    void Start()
    {
        RegisterEvent();
    }


    private void RegisterEvent()
    {
        if (LevelcontinueButton != null)
            UIEventListener.Get(LevelcontinueButton).onClick = UIManagerOz.SharedInstance.inGameVC.OnUnPaused;
        if (LevelhomeButton != null)
            UIEventListener.Get(LevelhomeButton).onClick = UIManagerOz.SharedInstance.inGameVC.OnHomeButtonClicked;

        if (EndlesscontinueButton != null)
            UIEventListener.Get(EndlesscontinueButton).onClick = UIManagerOz.SharedInstance.inGameVC.OnUnPaused;
        if (EndlesshomeButton != null)
            UIEventListener.Get(EndlesshomeButton).onClick = UIManagerOz.SharedInstance.inGameVC.OnHomeButtonClicked;

        if (tabs!=null&&tabs.Count!=0)
            foreach (UISprite sp in tabs)
             UIEventListener.Get(sp.gameObject).onClick = OnButtonClick;
    }
    public void OnButtonClick(GameObject button)
    {
        switch (button.name)
        {

            case "icon_tab_main":
                SwitchToPanel(ObjectivesScreenName.MainTask);
                break;
            case "icon_tab_daily":
                SwitchToPanel(ObjectivesScreenName.DailyTask);
                //if (newDailytasktip != null)
                //    newDailytasktip.gameObject.SetActive(false);
                break;
        }
    }
    private void SwitchToPanel(ObjectivesScreenName panelScreenName)	// activate panel upon button selection, passing in ObjectivesScreenName
    {
        if (tabs == null || objectivesPanelUILists == null)
            return;


        SwitchTab(panelScreenName);

    }
    public void SwitchTab(ObjectivesScreenName panelScreenName)
    {
        for (ObjectivesScreenName objective = (ObjectivesScreenName)0; objective < ObjectivesScreenName.ScreenCount; ++objective)
        {
            //当前页面
            if (objective == panelScreenName)
            {
                tabs[(int)panelScreenName].alpha = 1f;
                tabs[(int)panelScreenName].collider.enabled = false;
                objectivesPanelUILists[(int)panelScreenName].gameObject.SetActive(true);
            }
            else
            {
                tabs[(int)objective].alpha = 0.03f;
                tabs[(int)objective].collider.enabled = true;
                objectivesPanelUILists[(int)objective].gameObject.SetActive(false);
            }
        }

        Refresh(panelScreenName);
    }

    void Refresh(ObjectivesScreenName panelScreenName)
    {
        if (panelScreenName == ObjectivesScreenName.MainTask)
        {
            int index = 0; //任务索引
            foreach (ObjectiveProtoData pd in GameProfile.SharedInstance.Player.objectivesMain)
            {
                MaintaskObjectiveCells[index].GetComponent<MainTaskCellData>().SetData(pd);
                index++;

                if (pd._conditionList[0]._earnedStatValue >= pd._conditionList[0]._statValue
                   && !pd._hasDone)
                {

                    MaintaskObjectiveCells[index - 1].GetComponent<MainTaskCellData>().completed.SetActive(true);

                }
            }
        }
        else if (panelScreenName == ObjectivesScreenName.DailyTask)
        {
            int index = 0; //任务索引
            foreach (ObjectiveProtoData pd in GameProfile.SharedInstance.Player.objectivesDaily)
            {
                DailytaskObjectiveCells[index].GetComponent<DailyTaskCellData>().SetData(pd);
                index++;

                if (pd._conditionList[0]._earnedStatValue >= pd._conditionList[0]._statValue
                   && !pd._hasDone)
                {

                  //  DailytaskObjectiveCells[index - 1].GetComponent<DailyTaskCellData>().completed.SetActive(true);

                }
            }

        }
    }
    public void ShowPauseMenu()		// show whole pause menu
    {
        if (GamePlayer.SharedInstance.Dying)
            return;

        if (GameController.SharedInstance.EndlessMode)
        {
            EndlessModecenter.SetActive(true);
            LevelModecenter.SetActive(false);
            EndlessModeData();

        }
        else
        {
            LevelModecenter.SetActive(true);
            EndlessModecenter.SetActive(false);
            LevelModeData();
        }


       // UpdateData();//更新数据


    }

    public void HidePauseMenu()		// hide whole pause menu
    {
            EndlessModecenter.SetActive(false);
            LevelModecenter.SetActive(false);
    }
    private void UpdateData()
    {

    
    }

    //关卡模式数据
    private void LevelModeData()
    {
        int level = GameProfile.SharedInstance.Player.activeLevel;
        ObjectiveProtoData mdata = ObjectivesManager.LevelObjectives[level]; 
        int starCount = UIManagerOz.SharedInstance.worldOfOzVC.GetStarRank(mdata);
        levelinfo.appear(mdata, starCount);
        UIManagerOz.SharedInstance.worldOfOzVC.UpdateStarProgressBar(levelinfo.progressBar, mdata,true);

        Levelscore.text = GamePlayer.SharedInstance.Score.ToString();
        Levelcoin.text = GamePlayer.SharedInstance.CoinCountTotal.ToString();
        Levelmedal.text = GamePlayer.SharedInstance.MedalCountTotal.ToString();
        Levelmedalsilver.text = GamePlayer.SharedInstance.MedalSilverCountTotal.ToString();
        Levelmedalgold.text = GamePlayer.SharedInstance.MedalGoldCountTotal.ToString();
    }
    private void EndlessModeData()
    {
        SwitchToPanel(ObjectivesScreenName.MainTask);
        Endlessscore.text = GamePlayer.SharedInstance.Score.ToString();
        Endlesscoin.text = GamePlayer.SharedInstance.CoinCountTotal.ToString();
        Endlessmedal.text = GamePlayer.SharedInstance.MedalCountTotal.ToString();
        Endlessmedalsilver.text = GamePlayer.SharedInstance.MedalSilverCountTotal.ToString();
        Endlessmedalgold.text = GamePlayer.SharedInstance.MedalGoldCountTotal.ToString();
    
    }
}
