using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public enum UpgradesScreenName
{
    PowerUps,
    Artifacts,
    Consumables,
    MoreCoins
} //Stats, }

public class UIInventoryViewControllerOz : UIViewControllerOz
{
    private readonly List<Vector3> tabGOSpriteScales = new List<Vector3>();
    public AirPort airPortScript;
    public UIArtifactsList alist;
    public GameObject btnBack;
    public GameObject btnStart;
    public GameObject tutorialCharacter;
    public GameObject centerPowerRoot;
    public UIConsumablesList clist;
    private TabSettings deSelectedTabSettings;
    public List<GameObject> inventoryPanelGOs = new List<GameObject>();
    private NotificationIcons notificationIcons;
    private UpgradesScreenName pageToLoad = UpgradesScreenName.Consumables;
    public UIPowersList plist;
    private TabSettings selectedTabSettings;
    public UIStoreList slist;
    // wxj
    public GameObject storeScroll;
    public List<GameObject> tabGOs = new List<GameObject>();

    protected override void Awake()
    {
        base.Awake();
        notificationIcons = gameObject.GetComponent<NotificationIcons>();

        selectedTabSettings = new TabSettings(tabGOs[2]);
        deSelectedTabSettings = new TabSettings(tabGOs[1]);

        // store initial icon sprite scale values, to pass into scaling function
        tabGOSpriteScales.Add(tabGOs[0].transform.Find("Sprite").localScale);
        tabGOSpriteScales.Add(tabGOs[1].transform.Find("Sprite").localScale);
        tabGOSpriteScales.Add(tabGOs[2].transform.Find("Sprite").localScale);
        tabGOSpriteScales.Add(tabGOs[3].transform.Find("Sprite").localScale);

        tabGOs[0].transform.Find("Sprite").localScale *= TabSettings.tabScaleMultiplier;
            // hack to make icon of selected tab scaled up on launch
    }

    protected override void Start()
    {
        base.Start();
    }

    protected override void RegisterEvent()
    {
        UIEventListener.Get(btnBack).onClick = UIManagerOz.SharedInstance.PaperVC.OnHomeClicked;
        UIEventListener.Get(btnStart).onClick = OnPlayClicked;
    }

    private void OnPlayClicked(GameObject obj)
    {
        //调整关卡引导结束
        //if (GameController.SharedInstance.unlockChallengeTutorialPlayed
        //   && !GameController.SharedInstance.challengeTutorialPlayed)
       if (GameController.SharedInstance.GetTutorialIDforSys() == 5)
        {
            //tutorialCharacter.SetActive(false);
            //GameController.SharedInstance.challengeTutorialPlayed = true;
            //PlayerPrefs.SetInt("challengeTutorialPlayedInt", 1);
            //PlayerPrefs.Save();
            //StartCoroutine(WaitOilEffect());
            //return;
        }

        if (!UIManagerOz.SharedInstance.PaperVC.fuelSystem.IsFuelEnough())
        {
            UIManagerOz.SharedInstance.PaperVC.fuelSystem.CannotIntoGame();
            return;
        }
        else
        {
            UIManagerOz.SharedInstance.PaperVC.fuelSystem.AddFuelCount(-2);//消耗燃料
        }
    
        StartCoroutine(WaitOilEffect());
           
    }

    private IEnumerator WaitOilEffect()
    {
        Vector3 fromPos = UIManagerOz.SharedInstance.PaperVC.oilPos.position;

        OilEffect.instance.PlayEffect(btnStart.transform.position,fromPos);

        while(!OilEffect.instance.isPlayFinished)
        {
            yield return null;
        }
      
        
        GameStart();
        yield break;
    }

    public void GameStart()
    {
       
        if (GameController.SharedInstance.EndlessMode)
        {
            UIDynamically.instance.LeftToScreen(centerPowerRoot, 0f, -1200f, 1f);

            UIDynamically.instance.TopToScreen(btnBack, 590f, 720f, 0.5f);
        }

        StartCoroutine(WaitForLoadingHide());
    
    }

    IEnumerator WaitForLoadingHide()
    {
        while(UIManagerOz.SharedInstance.loadingVC.gameObject.activeSelf)
        {
            yield return 0;
        }

        airPortScript.StartClick();
        
        AudioManager.SharedInstance.PlayPreflight();
    }

    public override void appear()
    {
        base.appear();

        GameController.SharedInstance.MenusEntered();

        UIManagerOz.SharedInstance.PaperVC.SetCurrentPage(UIManagerOz.SharedInstance.inventoryVC);

        UIManagerOz.SharedInstance.PaperVC.appear();

        UIManagerOz.SharedInstance.PaperVC.UpdateCurrency();

        ResetScrollListsToTop();

        SwitchToPanelOnEnter(pageToLoad);

        SetTabColliders();

        AudioManager.SharedInstance.SwitchMusic(AudioManager.SharedInstance.EmptyMusic);


        if (GameController.SharedInstance.EndlessMode)
        {
            UIDynamically.instance.LeftToScreen(centerPowerRoot, 1200f, 0f, 1f, false, 0.3f);
            Invoke("ChallengeTutorial",1f);
            UIDynamically.instance.TopToScreen(btnBack, 720f, 590f, 0.5f);


        }
        else
        {
            centerPowerRoot.transform.SetLocalPositionX(1200f);
            btnBack.transform.SetLocalPositionY(720f);
            GameStart();
        }
    }

    //开始挑战教程
    public void ChallengeTutorial()
    {
        //if (GameController.SharedInstance.unlockChallengeTutorialPlayed && !GameController.SharedInstance.challengeTutorialPlayed) //挑战关
        if (GameController.SharedInstance.GetTutorialIDforSys() == 5 )
        {
            btnStart.GetComponent<UISprite>().depth = 101;
            tutorialCharacter.transform.SetParent(btnStart.transform);
            tutorialCharacter.transform.ResetTransformation();
            tutorialCharacter.SetActive(true);
            var tutorialTS = TweenScale.Begin(tutorialCharacter, 1.0f, Vector3.one);
            tutorialTS.to = Vector3.one * 0.8f;
            tutorialTS.style = UITweener.Style.Loop;
        }
    }
    public void OnEnable()
    {
        btnBack.SetActive(true);
    }

    public void OnDisable()
    {
    }

    private void SetTabColliders()
        // only use this on initial page entry, afterwards the scaling takes care of collider state
    {
        foreach (var tabGO in tabGOs)
            tabGO.GetComponent<BoxCollider>().enabled = true;

        tabGOs[(int) pageToLoad].GetComponent<BoxCollider>().enabled = false;
    }

    public void LoadThisPageNextTime(UpgradesScreenName page)
    {
        pageToLoad = page;
    }

    public void Refresh()
    {
        // refresh appropriate panel prior to showing
        Services.Get<Store>().GetWeeklyDiscountManagerClass().ExpireDiscounts();

        switch (pageToLoad)
        {
            case UpgradesScreenName.PowerUps:
                plist.Refresh();
                break;
            case UpgradesScreenName.Artifacts:
                alist.Refresh();
                break;
            case UpgradesScreenName.Consumables:
                clist.Refresh();
                break;
            case UpgradesScreenName.MoreCoins:
                if (UIStoreList.storeLoaded == false) // request product list from store
                    slist.RequestStoreList(); //Refresh();
                else if (UIStoreList.fullStoreScrollListGenerated == false) // generate scroll list
                    slist.GenerateScrollList();
                else
                    slist.Refresh();

                break;
        }

        Services.Get<NotificationSystem>().SetNotificationIconsForThisPage(UiScreenName.UPGRADES);
    }

    private void SetActivePage()
    {
        foreach (var go in inventoryPanelGOs)
            NGUITools.SetActive(go, false);

        NGUITools.SetActive(inventoryPanelGOs[(int) pageToLoad], true);

        inventoryPanelGOs[(int) pageToLoad].GetComponent<UIScrollView>().ResetPosition();

//		if ((int)pageToLoad <= 2)
//			Services.Get<MenuTutorials>().SendEvent((int)pageToLoad);	// for menu tutorials


        // wxj
        if (storeScroll != null)
        {
            if (pageToLoad == UpgradesScreenName.MoreCoins)
                NGUITools.SetActive(storeScroll, true);
            else
                NGUITools.SetActive(storeScroll, false);
        }
    }

    public void ResetScrollListsToTop()
    {
        foreach (var go in inventoryPanelGOs)
            go.GetComponent<UIScrollView>().ResetPosition();
    }

    public void SwitchToPanelOnEnter(UpgradesScreenName panelScreenName)
        // activate panel upon button selection, passing in UpgradesScreenName
    {
        var tweenTime = 0.001f;

        selectedTabSettings.ScaleTab(tabGOs[(int) panelScreenName], selectedTabSettings, tweenTime, true,
            tabGOSpriteScales[(int) panelScreenName]); // scale selected tab up


        for (var i = 0; i <= 3; i++)
        {
            if (i != (int) panelScreenName)
                deSelectedTabSettings.ScaleTab(tabGOs[i], deSelectedTabSettings, tweenTime, false,
                    tabGOSpriteScales[i]); // scale other three tabs down
        }

        SetActivePage();
        Refresh();
    }

    private void SwitchToPanel(UpgradesScreenName panelScreenName)
        // activate panel upon button selection, passing in UpgradesScreenName
    {
        if (panelScreenName != pageToLoad)
        {
            ScaleTab((int) pageToLoad, false);
            ScaleTab((int) panelScreenName, true);
        }

        pageToLoad = panelScreenName;

        SetActivePage();
        Refresh();
    }

    private void ScaleTab(int index, bool bigger)
    {
        if (index >= tabGOs.Count) // prevent nulls
            return;

        //var tweenTime = 0.1f;
        //var targetTabSettings = (bigger) ? selectedTabSettings : deSelectedTabSettings;

//		targetTabSettings.ScaleTab(tabGOs[index], targetTabSettings, tweenTime, bigger, tabGOSpriteScales[index]);				
    }

    public void OnButtonClick(GameObject button)
    {
        switch (button.name)
        {
            case "Tab0": // powerups button
                //				AnalyticsInterface.LogNavigationActionEvent( "Powerups", "Store", "Store-Powerups" );

                SwitchToPanel(UpgradesScreenName.PowerUps);
                UIManagerOz.SharedInstance.PaperVC.SetPageName("Ttl_Store", "Ttl_Sub_Powerups");
                break;

            case "Tab1": // modifiers button
                //	AnalyticsInterface.LogNavigationActionEvent( "Abilities", "Store", "Store-Abilities" );

                SwitchToPanel(UpgradesScreenName.Artifacts);
                UIManagerOz.SharedInstance.PaperVC.SetPageName("Ttl_Store", "Ttl_Sub_Modifiers");
                break;
            case "Tab2": // consumables button
                //AnalyticsInterface.LogNavigationActionEvent( "Utilities", "Store", "Store-Utilities" );

                SwitchToPanel(UpgradesScreenName.Consumables);
                UIManagerOz.SharedInstance.PaperVC.SetPageName("Ttl_Store", "Ttl_Sub_Consumables");
                break;

            case "Tab3": // more coins button

                //AnalyticsInterface.LogNavigationActionEvent( "More Coins", "Store", "Store-More Coins" );

                SwitchToPanel(UpgradesScreenName.MoreCoins);
                UIManagerOz.SharedInstance.PaperVC.SetPageName("Ttl_Store", "Lbl_MoreCoins");
                break;
        }
    }

    public void SetNotificationIcon(int buttonID, int iconValue) // update actual icon onscreen
    {
        if (notificationIcons != null)
            notificationIcons.SetNotification(buttonID, iconValue);
    }

    public string GetCostIconNameByType(CostType type)
    {
        var name = string.Empty;
        switch (type)
        {
            case CostType.Coin:
                name = "common_coin";
                break;
            case CostType.Special:
                name = "common_gem";
                break;
            case CostType.Fragment:
                name = "medalgold";
                break;
            default:
                name = "common_coin";
                break;
        }

        return name;
    }

    public void OpenLuckyBox(int pid)
    {
        //需要增加一个ui来显示开启过程
        GameProfile.SharedInstance.Player.AddChanceToken(1);
        UIManagerOz.SharedInstance.gatchVC.availableCards = 1;
        UIManagerOz.SharedInstance.gatchVC.appear();


//        var i = Random.Range(0, ConsumableStore.consumablesList.Count);
//
//        var consumable = ConsumableStore.consumablesList[i];
//        PlayerStats playerStats = GameProfile.SharedInstance.Player;
//        playerStats.consumablesPurchasedQuantity[consumable.PID]++;
//        playerStats.consumablesPurchasedQuantity[pid]++;
//
//        UIManagerOz.SharedInstance.confirmDialog.ShowConfirmDialog(string.Format("获得{0}",Localization.SharedInstance.Get(consumable.Title)), "Btn_No", "Btn_Yes");
    }
}