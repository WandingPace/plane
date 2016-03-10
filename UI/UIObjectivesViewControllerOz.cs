using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// wxj, add activity
public enum ObjectivesScreenName { MainTask=0, DailyTask, ScreenCount,Achievement}

public class UIObjectivesViewControllerOz : UIViewControllerOz
{
    public UISprite iconHead,newDailytasktip;

    public UISlider expProgress;

    public UILabel lvTxt, expTxt, rewardTxt, nextRewardTxt;
    
    public GameObject  btnClose;

    public Levelup levelup;

    public GameObject RefreshTaskuigrid;

    public List<UISprite> tabs = new List<UISprite>();

	public List<UIObjectivesList> objectivesPanelUILists = new List<UIObjectivesList>();	
	
	private NotificationIcons notificationIcons;

    private ObjectivesScreenName pageToLoad = ObjectivesScreenName.MainTask;

    public GameObject tutorialCharacter;

    
	protected override void Awake()
	{
		base.Awake();
		notificationIcons = gameObject.GetComponent<NotificationIcons>();
    }
	
	protected override void Start() 
	{ 
		base.Start();
	}

    protected override void RegisterEvent ()
    {
        UIEventListener.Get(btnClose).onClick = OnCloseBtnClicked;
        foreach(UISprite sp in tabs)
            UIEventListener.Get(sp.gameObject).onClick = OnButtonClick;
    }
	
	public override void appear() 
	{	
        UIDynamically.instance.ZoomZeroToOneWithMovePostion(gameObject,new Vector3(124f,-556f,0f),0.5f);

		base.appear();		
		
		Services.Get<NotificationSystem>().SetNotificationIconsForThisPage(UiScreenName.OBJECTIVES);

		UIManagerOz.SharedInstance.PaperVC.SetCurrentPage(UIManagerOz.SharedInstance.ObjectivesVC);
		
        RefreshUserInfo();

        SwitchToPanel(pageToLoad);

	}

    void OnCloseBtnClicked(GameObject obj)
    {
        //if (GameController.SharedInstance.unlockrole2PosTutorialPlayed && !GameController.SharedInstance.unlockChallengeTutorialPlayed && GameProfile.SharedInstance.Player.GetIsChallengeUnlock()) //挑战关
        if (GameController.SharedInstance.GetTutorialIDforSys() == 5 && GameProfile.SharedInstance.Player.GetIsChallengeUnlock())
        {
            UIManagerOz.SharedInstance.idolMenuVC.switchtoUnlockChallenge(true);
            UIManagerOz.SharedInstance.idolMenuVC.btn_charsel.GetComponent<UISprite>().depth = 3;
            UIManagerOz.SharedInstance.idolMenuVC.btn_level.GetComponent<UISprite>().depth = 3;
            UIManagerOz.SharedInstance.idolMenuVC.btn_challenge.GetComponent<UISprite>().depth = 99;
            UIManagerOz.SharedInstance.idolMenuVC.tutorialCharacter.transform.SetParent(UIManagerOz.SharedInstance.idolMenuVC.btn_challenge.transform);
            UIManagerOz.SharedInstance.idolMenuVC.tutorialCharacter.transform.ResetTransformation();
            UIManagerOz.SharedInstance.idolMenuVC.tutorialCharacter.SetActive(true);
            var tutorialTS = TweenScale.Begin(UIManagerOz.SharedInstance.idolMenuVC.tutorialCharacter, 1.0f, Vector3.one);
            tutorialTS.to = Vector3.one * 0.8f;
            tutorialTS.style = UITweener.Style.Loop;

        }
        UIDynamically.instance.ZoomZeroToOneWithMovePostion(gameObject,new Vector3(-124f,-556f,0f),0.5f,true);

        Invoke("disappear",0.4f);
    }

    //void disappear()
    //{
    //    gameObject.SetActive(false);
    //}
 
    public void RefreshUserInfo(bool isLvUp = false,bool addExp = false)
    { 
        iconHead.spriteName =UIManagerOz.SharedInstance.idolMenuVC.playerInfo.GetPlayerIconSpriteName();
        lvTxt.text = GameProfile.SharedInstance.Player.playerLv.ToString();
        rewardTxt.text = UIManagerOz.SharedInstance.idolMenuVC.playerInfo.GetLvEffectDesc(GameProfile.SharedInstance.Player.playerLv);
        nextRewardTxt.text = UIManagerOz.SharedInstance.idolMenuVC.playerInfo.GetLvEffectDesc(GameProfile.SharedInstance.Player.playerLv+1);

        if(addExp)
        {
            int newProg= GameProfile.SharedInstance.Player.GetPlayerExp();
            int newMaxProg= GameProfile.SharedInstance.Player.GetPlayerLvMaxExpByLv(GameProfile.SharedInstance.Player.playerLv);
            int count = isLvUp ? 1:0;
            UIDynamically.instance.AnimateProgressBar(expProgress.gameObject,count,(float)newProg,(float)newMaxProg);
            UIDynamically.instance.AnimateNumsToTarget(expTxt,newProg,newMaxProg,1.5f);
        }
        else
        {
            expTxt.text =UIManagerOz.SharedInstance.idolMenuVC.playerInfo.GetExpTxt();
            expProgress.value = UIManagerOz.SharedInstance.idolMenuVC.playerInfo.GetExpBar();

            if (isLvUp || expProgress.value==1) //判断当前经验是否已满
                expTxt.text = "经验已满";
        }

        if(isLvUp)
        {
            if (addExp)
            {
                UIDynamically.instance.LeftToScreen(RefreshTaskuigrid, 0f, 650f,0.5f, false);
                UIDynamically.instance.LeftToScreen(RefreshTaskuigrid, -650f, 0f, 0.5f, false, 0.7f);
                Invoke("PopupLevelUp", 1f);
                
            }
            UIDynamically.instance.Blink(lvTxt.gameObject,0.5f);
            UIDynamically.instance.Blink(rewardTxt.gameObject,0.5f);
            UIDynamically.instance.Blink(nextRewardTxt.gameObject,0.5f);
            
        }
    }
   void PopupLevelUp()
   {
        levelup.PopupLevelUpUI();
    
    }



	public void LoadThisPageNextTime(ObjectivesScreenName page)
	{
		pageToLoad = page;
	}
	
    private void Refresh(ObjectivesScreenName panelScreenName) 	
	{
        objectivesPanelUILists[(int)panelScreenName].Refresh();
		Services.Get<NotificationSystem>().SetNotificationIconsForThisPage(UiScreenName.OBJECTIVES);
	}	
	
    public void SwitchTab(ObjectivesScreenName panelScreenName)
    {
        for(ObjectivesScreenName objective = (ObjectivesScreenName)0; objective< ObjectivesScreenName.ScreenCount; ++objective)
        {
            //当前页面
            if(objective == panelScreenName)
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

	private void SwitchToPanel(ObjectivesScreenName panelScreenName)	// activate panel upon button selection, passing in ObjectivesScreenName
	{
		
		pageToLoad = panelScreenName;

        SwitchTab(panelScreenName);	

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
                if (newDailytasktip != null)
                    newDailytasktip.gameObject.SetActive(false);
				break;
		}
	}	
	
	public void SetNotificationIcon(int buttonID, int iconValue)		// update actual icon onscreen
	{
		notificationIcons.SetNotification(buttonID, iconValue);
	}

}
