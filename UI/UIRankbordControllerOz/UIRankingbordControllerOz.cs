using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum RankingScreenName { rankhistory = 0, rankfriend, rankScreenCount }

public enum RankingHistoryScreen { rankworld = 0, ranklocal, rankhistoryScreenCount }



public class UIRankingbordControllerOz : UIViewControllerOz
{
    // public UISprite iconHead, expProgress;

    //  public UILabel lvTxt, expTxt, rewardTxt, nextRewardTxt;

    public GameObject btnClose;
    public UISprite myheadicon,myrankicon;
    public UILabel myname, myscore, myrank;
    public List<UISprite> friendtabs = new List<UISprite>();  //历史和好友
    public List<UISprite> historytabs = new List<UISprite>(); //历史的世界和地区

    public List<GameObject> rankPanelGO = new List<GameObject>();
    public UIRankbordList friendPanelUIList;
    public List<UIRankbordList> historyPanelUILists = new List<UIRankbordList>();

    private NotificationIcons notificationIcons;

    public static  RankingScreenName pageToLoad = RankingScreenName.rankhistory;
    public static  RankingHistoryScreen historypageToLoad = RankingHistoryScreen.rankworld;

    protected override void Awake()
    {
        base.Awake();
     //   notificationIcons = gameObject.GetComponent<NotificationIcons>();
    }

    protected override void Start()
    {
        base.Start();
    }

    protected override void RegisterEvent()
    {
        UIEventListener.Get(btnClose).onClick = OnCloseBtnClicked;
        foreach (UISprite sp in friendtabs)
            UIEventListener.Get(sp.gameObject).onClick = OnButtonClick;
        foreach (UISprite sp in historytabs)
            UIEventListener.Get(sp.gameObject).onClick = OnButtonClick;
        
    }

    public override void appear()
    {
        UIDynamically.instance.ZoomZeroToOneWithMovePostion(gameObject,new Vector3(124f,-556f,0f),0.5f);

        base.appear();

        Services.Get<NotificationSystem>().SetNotificationIconsForThisPage(UiScreenName.RANDINGBOARDS);

        UIManagerOz.SharedInstance.PaperVC.SetCurrentPage(UIManagerOz.SharedInstance.RankingVC);



        SwitchToPanel(pageToLoad);
        SwitchhistoryToPanel(historypageToLoad);

        RefreshUserInfo();
    }

    void OnCloseBtnClicked(GameObject obj)
    {
        UIDynamically.instance.ZoomZeroToOneWithMovePostion(gameObject,new Vector3(124f,-556f,0f),0.5f,true);
        Invoke("disappear",0.4f);
    }

    //void disappear()
    //{
    //    gameObject.SetActive(false);
    //}

    public void RefreshUserInfo()
    {
        myheadicon.spriteName = GetPlayerIconSpriteName();
        myname.text= GameProfile.SharedInstance.Player.playerName;
        myscore.text = GameProfile.SharedInstance.Player.bestScore.ToString();
        if (pageToLoad == RankingScreenName.rankhistory)
        {
            myrank.text = historyPanelUILists[(int)historypageToLoad].playerdata._nRank.ToString();

        }
        else
        {
            myrank.text = friendPanelUIList.playerdata._nRank.ToString();
            if (friendPanelUIList.playerdata._nRank <= 3)
            {
                myrank.gameObject.SetActive(false);
                myrankicon.gameObject.SetActive(true);
                myrankicon.spriteName = "rank_NO" + friendPanelUIList.playerdata._nRank;
            }
        }
        
    }

    public void LoadThisPageNextTime(RankingScreenName page)
    {
        pageToLoad = page;
    }

    private void Refresh(RankingScreenName panelScreenName)   //历史和好友
    {

        if (panelScreenName == RankingScreenName.rankfriend)
        {
            friendPanelUIList.Refresh(); //获得注入信息
            RefreshUserInfo();
        }
        else
        {
            historyPanelUILists[(int)historypageToLoad].Refresh();
        }

        Services.Get<NotificationSystem>().SetNotificationIconsForThisPage(UiScreenName.RANDINGBOARDS);
    }
    private void Refresh(RankingHistoryScreen panelScreenName)  //世界和地区
    {
        historyPanelUILists[(int)panelScreenName].Refresh();
        Services.Get<NotificationSystem>().SetNotificationIconsForThisPage(UiScreenName.RANDINGBOARDS);
    }

    //=========================================================

    public void SwitchTab(RankingScreenName panelScreenName)
    {
        for (RankingScreenName objective = (RankingScreenName)0; objective < RankingScreenName.rankScreenCount; ++objective)
        {
            //当前页面
            if (objective == panelScreenName)
            {
                friendtabs[(int)panelScreenName].alpha = 1f;
                friendtabs[(int)panelScreenName].collider.enabled = false;
                rankPanelGO[(int)panelScreenName].gameObject.SetActive(true);
            }
            else
            {
                friendtabs[(int)objective].alpha = 0.03f;
                friendtabs[(int)objective].collider.enabled = true;
                rankPanelGO[(int)objective].gameObject.SetActive(false);
            }
        }

        Refresh(panelScreenName);
    }

    private void SwitchToPanel(RankingScreenName panelScreenName)	// activate panel upon button selection, passing in ObjectivesScreenName
    {

        pageToLoad = panelScreenName;

        SwitchTab(panelScreenName);

    }
    //---
    public void SwitchhistoryTab(RankingHistoryScreen panelScreenName)
    {
        for (RankingHistoryScreen objective = (RankingHistoryScreen)0; objective < RankingHistoryScreen.rankhistoryScreenCount; ++objective)
        {
            //当前页面
            if (objective == panelScreenName)
            {
                historytabs[(int)panelScreenName].alpha = 1f;
                historytabs[(int)panelScreenName].collider.enabled = false;
                historyPanelUILists[(int)panelScreenName].gameObject.SetActive(true);
            }
            else
            {
                historytabs[(int)objective].alpha = 0.03f;
                historytabs[(int)objective].collider.enabled = true;
                historyPanelUILists[(int)objective].gameObject.SetActive(false);
            }
        }

        Refresh(panelScreenName);
    }
     
    private void SwitchhistoryToPanel(RankingHistoryScreen panelScreenName)	// activate panel upon button selection, passing in ObjectivesScreenName
    {

        historypageToLoad = panelScreenName;

        SwitchhistoryTab(panelScreenName);

    }


    public void OnButtonClick(GameObject button)
    {
        switch (button.name)
        {

            case "icon_tab_history":
                SwitchToPanel(RankingScreenName.rankhistory);
                
                break;
            case "icon_tab_friend":
                SwitchToPanel(RankingScreenName.rankfriend);
                break;
            case "icon_tab_world":
                SwitchhistoryToPanel(RankingHistoryScreen.rankworld);
                break;
            case "icon_tab_local":
                SwitchhistoryToPanel(RankingHistoryScreen.ranklocal);
                break;
        }
    }

//==================================================
    public string GetPlayerIconSpriteName()  //获得当前头像名
    {
        return "player_head_" + GameProfile.SharedInstance.Player.playerIconIndex;
    }
}
