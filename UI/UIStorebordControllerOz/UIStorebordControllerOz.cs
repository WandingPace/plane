using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public enum StoreScreenName { gem = 0, coin, other,storescreenCount }
public class UIStorebordControllerOz : UIViewControllerOz 
{
    public List<UISprite> btnAddGemlist = new List<UISprite>();
    public List<UISprite> btnAddCoinlist = new List<UISprite>();
    public List<UISprite> tabstoreCreenList = new List<UISprite>();
    public List<GameObject> StorePanelUILists = new List<GameObject>();
    public GameObject modelChug;
    public GameObject btnClose;
    public Animation chugAnimation;
    public GameObject top;
    public UILabel CoinLabel;   
    public UILabel GemLabel; 
    public UILabel OilLabel;


    private void UpdateCurrency()
    {
        CoinLabel.text = GameProfile.SharedInstance.Player.coinCount.ToString();
        GemLabel.text = GameProfile.SharedInstance.Player.specialCurrencyCount.ToString();
        OilLabel.text =  GameProfile.SharedInstance.Player.fuelCount+ "/"+UIManagerOz.SharedInstance.PaperVC.fuelSystem.GetFuelUpLimit();
    }

    protected override void Awake()
    {
//        CoinLabel.text = "0";
//        GemLabel.text = "0";
       
        base.Awake();
        //   notificationIcons = gameObject.GetComponent<NotificationIcons>();
    }

    protected override void Start()
    {
        base.Start();
    }

    protected override void RegisterEvent()
    {
        foreach (UISprite btnaddgem in btnAddGemlist)
            UIEventListener.Get(btnaddgem.gameObject).onClick = OnGemaddClicked;

        foreach (UISprite btnaddcoin in btnAddCoinlist)
            UIEventListener.Get(btnaddcoin.gameObject).onClick = OnCoinClicked;

        foreach (UISprite btnstoretype in tabstoreCreenList)
            UIEventListener.Get(btnstoretype.gameObject).onClick = OnStoreTabClicked;

        UIEventListener.Get(btnClose).onClick = OnCloseBtnClicked;
    }

    public override void appear()
    {
        UpdateCurrency();

        modelChug.SetActive(true);

        UIDynamically.instance.LeftToScreen(gameObject,800f,0f,0.5f,false,0f,true);
//        UIDynamically.instance.TopToScreen(top,200f,37f,0.5f,false,0.3f,true);
        base.appear();
        StartCoroutine(PlayAinm(chugAnimation, "Idle"));
    }

    private IEnumerator PlayAinm(Animation animation, string clipName)
    {
        AnimationState _currState = animation[clipName];
        _currState.wrapMode = WrapMode.Loop;
        bool isPlaying = true;
        //float _startTime = 0F;
        float _progressTime = 0F;
        float _timeAtLastFrame = 0F;
        float _timeAtCurrentFrame = 0F;
        float deltaTime = 0F;
        animation.Play(clipName);
        _timeAtLastFrame = Time.realtimeSinceStartup;
        while (isPlaying)
        {
            _timeAtCurrentFrame = Time.realtimeSinceStartup;
            deltaTime = _timeAtCurrentFrame - _timeAtLastFrame;
            _timeAtLastFrame = _timeAtCurrentFrame; 
            
            _progressTime += deltaTime;
            _currState.normalizedTime = _progressTime / _currState.length; 
            animation.Sample ();
            if (_progressTime >= _currState.length)
            {
                if(_currState.wrapMode != WrapMode.Loop)
                {
                    isPlaying = false;
                }
                else
                {
                    _progressTime = 0.0f;
                }
            }
            yield return new WaitForEndOfFrame();
        }
        yield return null;
    }

    public void BuyCoins()
    {
        UIConfirmDialogOz.onNegativeResponse += UIManagerOz.SharedInstance.StoreVC.OnNeedMoreCoinsNo;
        UIConfirmDialogOz.onPositiveResponse += UIManagerOz.SharedInstance.StoreVC.OnNeedMoreCoinsYes;
        UIManagerOz.SharedInstance.confirmDialog.ShowConfirmDialog("Lbl_Dialogue_MoreCoins_Prompt", "Btn_No", "Btn_Yes");
    }

    public void BuyGems()
    {
        UIConfirmDialogOz.onNegativeResponse += UIManagerOz.SharedInstance.StoreVC.OnNeedMoreCoinsNo;
        UIConfirmDialogOz.onPositiveResponse += UIManagerOz.SharedInstance.StoreVC.OnNeedMoreGemsYes;
        UIManagerOz.SharedInstance.confirmDialog.ShowConfirmDialog("Lbl_Dialogue_MoreGems_Prompt", "Btn_No", "Btn_Yes");
    }

    private void OnNeedMoreCoinsNo()
    {
        Time.timeScale = 1f;
        DisconnectHandlers();
    }
    
    private void OnNeedMoreCoinsYes()
    {
        DisconnectHandlers();
        appear();
        SwitchtoTab(StoreScreenName.coin);
    }   

    private void OnNeedMoreGemsYes()
    {
        DisconnectHandlers();
        appear();
        SwitchtoTab(StoreScreenName.gem);
    }

    private void DisconnectHandlers()
    {
        UIConfirmDialogOz.onNegativeResponse -= OnNeedMoreCoinsNo;
        UIConfirmDialogOz.onPositiveResponse -= OnNeedMoreCoinsYes;
        UIConfirmDialogOz.onPositiveResponse -= OnNeedMoreGemsYes;
    }

    void OnCloseBtnClicked(GameObject obj)
    {
        modelChug.SetActive(false);
        StopCoroutine("PlayAinm");
        UIDynamically.instance.LeftToScreen(gameObject,0f,-800f,0.5f,false,0f,true);
//        UIDynamically.instance.TopToScreen(top,200f,37f,0.5f,true,0.3f,true);
        Invoke("disappear",0.5f);
        Time.timeScale = 1f;
    }

    //void disappear()
    //{
    //    gameObject.SetActive(false);
    //}

    void OnGemaddClicked(GameObject obj)  //购买钻石
    {
         switch (obj.name)
        {
            case "icon_buy1":
                GameProfile.SharedInstance.Player.specialCurrencyCount += 100;
                break;
            case "icon_buy2":
                GameProfile.SharedInstance.Player.specialCurrencyCount += 500;
                break;
            case "icon_buy3":
                GameProfile.SharedInstance.Player.specialCurrencyCount += 1000;
                break;
            case "icon_buy4":
                GameProfile.SharedInstance.Player.specialCurrencyCount += 3000;
                break;
            case "icon_buy5":
                GameProfile.SharedInstance.Player.specialCurrencyCount += 10000;
                break;

        }
        UpdateCurrency();
         UIManagerOz.SharedInstance.PaperVC.UpdateCurrency();

    }
    void OnCoinClicked(GameObject obj)  //购买金币
    {
        switch (obj.name)
        {
            case "icon_coinbuy1":
                GameProfile.SharedInstance.Player.coinCount += 1000;
                break;
            case "icon_coinbuy2":
                GameProfile.SharedInstance.Player.coinCount += 2000;
                break;
            case "icon_coinbuy3":
                GameProfile.SharedInstance.Player.coinCount += 3000;
                break;
            case "icon_coinbuy4":
                GameProfile.SharedInstance.Player.coinCount += 4000;
                break;
            case "icon_coinbuy5":
                GameProfile.SharedInstance.Player.coinCount += 5000;
                break;


        }
        UIManagerOz.SharedInstance.PaperVC.UpdateCurrency();
        UpdateCurrency();
        
    }
    void OnStoreTabClicked(GameObject obj)
    {
        switch (obj.name)
        {
            case "icon_tab_coin":
                SwitchtoTab(StoreScreenName.coin);
                break;
            case "icon_tab_gem":
                SwitchtoTab(StoreScreenName.gem);
                break;
            case "icon_tab_other":
                SwitchtoTab(StoreScreenName.other);
                break;
        
        }
    
    }
   public void SwitchtoTab(StoreScreenName storescreen)
    {
        for (StoreScreenName objective = (StoreScreenName)0; objective < StoreScreenName.storescreenCount; ++objective)
        {
            //当前页面
            if (objective == storescreen)
            {
                tabstoreCreenList[(int)storescreen].alpha = 1f;
                tabstoreCreenList[(int)storescreen].collider.enabled = false;
                StorePanelUILists[(int)storescreen].gameObject.SetActive(true);
            }
            else
            {
                tabstoreCreenList[(int)objective].alpha = 0.03f;
                tabstoreCreenList[(int)objective].collider.enabled = true;
                StorePanelUILists[(int)objective].gameObject.SetActive(false);
            }
        }
       
    
    }
    void OnGemClicked(GameObject obj)
    {
        GameProfile.SharedInstance.Player.specialCurrencyCount += 1000;
        UIManagerOz.SharedInstance.PaperVC.UpdateCurrency();
        UpdateCurrency();
    }
    void OnOilClicked(GameObject obj)
    {
        UIManagerOz.SharedInstance.okayDialog.ShowOkayDialog("Msg_Developing", "Btn_Ok");
    }
}
