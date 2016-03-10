using System.Reflection;
using UnityEngine;

public class UIPaperViewControllerOz : UIViewControllerOz //MonoBehaviour
{
    private string analyticsCurrentPageName;
    private string analyticsPreviousPageName;
    private AppCounters appCounters;
    public GameObject btnAddCoin;
    public GameObject btnAddGem;
    public GameObject btnAddOil;
    public Transform oilPos;
    //newnew
    private UICharacterSelect charaSelVC;
    public UILabel CoinLabel;
    public UIViewControllerOz currentViewController;
    public UILabel GemLabel;
    public UILabel OilLabel;
    
    public UILabel fuelCountDown;

//	private NotificationSystem notificationSystem;	
    public FuelSystem fuelSystem;
    public bool goBackToIdolMenu = true;
    private UIIdolMenuViewControllerOz idolVC = null;
    private UIInventoryViewControllerOz inventoryVC = null;
    //private UILeaderboardViewControllerOz leaderboardVC;
    private UIMainMenuViewControllerOz mainVC = null;
    private UIObjectivesViewControllerOz ObjectivesVC = null;

    private UIPostGameViewControllerOz postVC = null;
    public UIViewControllerOz previousViewController;
    //private UIIAPViewControllerOz IAPStoreVC;
    //private UIIAPMiniViewControllerOz IAPMiniStoreVC;	
    //private UIMoreGamesViewControllerOz moreGamesVC;
    //private UISettingsViewControllerOz settingsVC;
    //private UIStatViewControllerOz statsVC;
    public Transform top;
    public GameObject topbg;
    private UIWorldOfOzViewControllerOz worldOfOzVC = null;

    private void OnEnable()
    {
        fuelSystem.gameObject.SetActive(false);
    }

    private void OnDisable()
    {
    }

    protected override void RegisterEvent()
    {
        UIEventListener.Get(btnAddCoin).onClick = OnCoinClicked;
        UIEventListener.Get(btnAddGem).onClick = OnGemClicked;
        UIEventListener.Get(btnAddOil).onClick = OnOilClicked;
    }

    private void OnCoinClicked(GameObject obj)
    {
        //   GameProfile.SharedInstance.Player.coinCount+=1000;
        //  UpdateCurrency();
        UIManagerOz.SharedInstance.StoreVC.appear();
        UIManagerOz.SharedInstance.StoreVC.SwitchtoTab(StoreScreenName.coin);
    }

    private void OnGemClicked(GameObject obj)
    {
        UIManagerOz.SharedInstance.StoreVC.appear();
        UIManagerOz.SharedInstance.StoreVC.SwitchtoTab(StoreScreenName.gem);
        // GameProfile.SharedInstance.Player.specialCurrencyCount+=1000;
        //UpdateCurrency();
    }

    private void OnOilClicked(GameObject obj)
    {
//        UIManagerOz.SharedInstance.okayDialog.ShowOkayDialog("Msg_Developing", "Btn_Ok");
        if(GameProfile.SharedInstance.Player.fuelCount >=fuelSystem.GetFuelUpLimit())
        {
            UIManagerOz.SharedInstance.okayDialog.ShowOkayDialog("燃料已满无需购买", "Btn_Ok");
            return;
        }
        fuelSystem.BuyFuel();
    }

    // wxj, call when exchange onekey artifact purchase succeed
    public void artifactPurchaseSuccess(string pId)
    {
        UIManagerOz.SharedInstance.okayDialog.ShowOkayDialog("Msg_ThankYouPurchase", "Btn_Ok");
        for (var i = 0; i < IAPWrapper.IAPS_ONEKEY_UPG.Length; i++)
        {
            if (IAPWrapper.IAPS_ONEKEY_UPG[i].Equals(pId))
            {
                GameProfile.SharedInstance.Player.PurchaseArtifact(i, true);
                inventoryVC.inventoryPanelGOs[1].GetComponent<UIArtifactsList>().Refresh();
            }
        }
    }

    public void purchaseChinaGirlSuccess()
    {
        UIManagerOz.SharedInstance.okayDialog.ShowOkayDialog("Msg_ThankYouPurchase", "Btn_Ok");
        var character = GameProfile.SharedInstance.Characters[3];
        character.unlocked = true;
        GameProfile.SharedInstance.Serialize();
        UIManagerOz.SharedInstance.PaperVC.UpdateCurrency();
//		worldOfOzVC.worldOfOzPanelGOs[1].GetComponent<UICharacterList>().Refresh();
    }

    public void purchaseSuperConsumableSuccess()
    {
        Debug.LogError("purchaseSuperConsumableSuccess call");
//		PurchaseUtil.bIAnalysisWithParam("Purchase_Consumables","ConsumablesName|Super_Consumable,amount|1");
        var playerStats = GameProfile.SharedInstance.Player;
        playerStats.EarnConsumable(9, 6);
        playerStats.EarnConsumable(4, 6);
        playerStats.EarnConsumable(7, 6);
        playerStats.EarnConsumable(8, 6);
        UIManagerOz.SharedInstance.PaperVC.UpdateCurrency();
        UIManagerOz.SharedInstance.okayDialog.ShowOkayDialog("Msg_ThankYouPurchase", "Btn_Ok");
        inventoryVC.inventoryPanelGOs[2].GetComponent<UIConsumablesList>().Refresh();
        //consumableToPurchase.Refresh();
    }

    protected override void Start()
    {
        base.Start();
        linkupOtherViewControllers();
        appCounters = Services.Get<AppCounters>();
        RegisterEvent();
    }

    public void Update()
    {
        if(!string.IsNullOrEmpty(TimeLeftCountdown.instance.fuelCountDownStr))
        {
            fuelCountDown.text = TimeLeftCountdown.instance.fuelCountDownStr;
        }
    }

    public void PlayDynamicUI(bool isIn)
    {
        UIDynamically.instance.TopToScreen(top.gameObject, 150f, 38.8f, 0.5f, !isIn);
    }

    public override void appear()
    {
        PlayDynamicUI(true);

        base.appear();

        if (currentViewController == UIManagerOz.SharedInstance.idolMenuVC)
        {
            UIManagerOz.SharedInstance.idolMenuVC.playerHead.SetActive(true);

            topbg.SetActive(false);


//            top.localPosition = new Vector3(100f,top.localPosition.y,0f);
        }
        else
        {
            UIManagerOz.SharedInstance.idolMenuVC.playerHead.SetActive(false);

            topbg.SetActive(true);
//            top.localPosition = new Vector3(154f,top.localPosition.y,0f);
        }

        UpdateCurrency();
    }

    public void UpdateCurrency()
    {
        var player = GameProfile.SharedInstance.Player;
        CoinLabel.text = player.coinCount.ToString();
        GemLabel.text = player.specialCurrencyCount.ToString();
        OilLabel.text = player.fuelCount+ "/"+ fuelSystem.GetFuelUpLimit();
        ObjectivesDataUpdater.SetGenericStat(ObjectiveType.CollectCoins, player.coinCount);//账户拥有金币成就统计
    }

    public void SetPageName(string title, string subTitle)
    {
        // Store the name of the previous page so we can include the "from" location
        // in the analytics when the "Back" button is pressed
        analyticsPreviousPageName = analyticsCurrentPageName;

        analyticsCurrentPageName = title;
        if (subTitle != "")
        {
            // Append the subtitle, if any
            analyticsCurrentPageName += "/" + subTitle;
        }

        if (analyticsPreviousPageName == null)
        {
            // "analyticsPreviousPageName" hasn't been set yet, so just set it to the current page name
            analyticsPreviousPageName = analyticsCurrentPageName;
        }
    }

    private void OnPlayClicked()
    {
//		if( DownloadManager.IsDownloadInProgress() )
//		{
//			UIManagerOz.SharedInstance.StartDownloadPrompts( true, false, true, gameObject);
//			return;
//		}

        UIManagerOz.SharedInstance.OnPlayClicked();
    }

    public void SetCurrentPage(UIViewControllerOz page)
    {
        previousViewController = currentViewController;
        currentViewController = page;
    }

    public void ShowExitPrompt()
    {
        UIConfirmDialogOz.onNegativeResponse += CancelExit;
        UIConfirmDialogOz.onPositiveResponse += ExitGame;
        UIManagerOz.SharedInstance.confirmDialog.ShowConfirmDialog("Msg_LeaveGame", "Btn_No", "Btn_Yes");
    }

    public void CancelExit()
    {
        UIConfirmDialogOz.onNegativeResponse -= CancelExit;
        UIConfirmDialogOz.onPositiveResponse -= ExitGame;
    }

    public void ExitGame()
    {
        Invoke("Quit", 0.2f); // wait to prevent screen glitch
    }

    private void Quit()
    {
        Application.Quit();
    }

    public void OnEscapeButtonClicked() // Android hardware back button is mapped to this
    {
        if (UIManagerOz.escapeHandled) return;
        UIManagerOz.escapeHandled = true;

        if (currentViewController == mainVC) // if on main menu screen			
            ShowExitPrompt();
        else
            OnHomeClicked(null);
    }

    public void OnHomeClicked(GameObject button)
    {
        UIDynamically.instance.TopToScreen(top.gameObject, 38.8f, 150f, 0.2f);

        var hidePaperVC = false;

        appCounters.UpdateSecondsSpentInApp();

        UIViewControllerOz nextViewController = mainVC; //idolVC;

        if (currentViewController == mainVC) // if on main menu screen			
        {
            if (goBackToIdolMenu)
                nextViewController = idolVC; // go back to idol menu	
            else
                nextViewController = postVC; // go back to post-run		

            hidePaperVC = true;
        }
        else if (currentViewController == inventoryVC) // if on inventory screen			
        {
            AudioManager.SharedInstance.SwitchMusic(AudioManager.SharedInstance.GameMusic);
            if (previousViewController == worldOfOzVC)
            {
                nextViewController = worldOfOzVC; // go back to post-run	
                hidePaperVC = false;
            }
            else
            {
                nextViewController = idolVC; // go back to main menu		
            }
        }
        else
        {
            nextViewController = idolVC;
        }

        currentViewController.disappear();
        nextViewController.appear();

        // --- Analytics ------------------------------------------------------
        //var previousPageName = analyticsPreviousPageName;

        //var nextViewControllerAnalyticsName = "Main Menu";

        //if (nextViewController == idolVC)
        //{
        //    nextViewControllerAnalyticsName = "Title Screen";
        //    previousPageName = analyticsCurrentPageName;
        //}
        //else if (nextViewController == postVC)
        //{
        //    nextViewControllerAnalyticsName = "Post Run";
        //}

        //		AnalyticsInterface.LogNavigationActionEvent( "Back", previousPageName, nextViewControllerAnalyticsName );
        // ------------------------------------------------------------------

//		previousViewController = currentViewController;		// 'SetCurrentPage' now does this, from other pages
//		currentViewController = nextViewController;

        // fix for situation where main menu doesn't disappear when going into Challenges screen
        if (ObjectivesVC.gameObject.activeSelf && mainVC.gameObject.activeSelf)
            mainVC.disappear();

        if (hidePaperVC)
        {
            Invoke("disappear", 0.2f);
        }

        // fix for extremely sporadic issue when 'Challenges' screen lingers for some reason
        //if (ObjectivesVC.gameObject.active == true && mainVC.gameObject.active == true)
        //	ObjectivesVC.disappear();
    }

    public bool IsOneLevelDownFromMainMenu() // Would hitting 'BACK' button bring us back to main menu?
    {
        return (currentViewController != idolVC && currentViewController != mainVC && previousViewController == mainVC);
    }

    public bool IsOnPostRunScreen()
    {
        return (currentViewController == postVC);
    }

    private void linkupOtherViewControllers()
        // Links up other View Controllers, anything that inherits from UIViewControllerOz, but not exactly UIViewControllerOz
    {
        var myType = GetType(); // Get the type handle of a specified class.
        FieldInfo[] myFieldInfo;
        myFieldInfo = myType.GetFields(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);

        for (var i = 0; i < myFieldInfo.Length; i++) // Display the field information of FieldInfoClass. 
        {
            var fieldType = myFieldInfo[i].FieldType;
            var descendant = typeof (UIViewControllerOz).IsAssignableFrom(fieldType);
            if (fieldType != typeof (UIViewControllerOz) && descendant)
            {
                var method = typeof (UIManagerOz).GetMethod("GetInstantiatedObject",
                    BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public);
                var generic = method.MakeGenericMethod(fieldType);
                var result = generic.Invoke(UIManagerOz.SharedInstance, null);

                myFieldInfo[i].SetValue(this, result);
            }
        }
    }
}