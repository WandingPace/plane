using UnityEngine;
using System.Collections;

public class ResurrectMenu : MonoBehaviour 
{
	protected static Notify notify;
	public UIInGameViewControllerOz inGameVC;
    public GameObject btnClose;
    public UISprite btnContinue;
    public UISprite btnReviveTeam;
    public Transform btnContinueTxt;
    public Transform btnReviveTeamTxt;
    

	public Transform resurrectMenu;
    public UILabel continueCostLabel;  
	public UILabel reviveCostLabel;	

	public UILabel MessageLabel;
    public UISpriteAnimationExt countDown;

    //继续次数
    private int [] continueCost = new int[]{10,20,50};
    private Vector3 initContinueTxtPos;

    //复活小队次数
    private int [] reviveCost = new int[]{2000,4000};
    private Vector3 initReviveTxtPos;

    [HideInInspector]
	public bool chooseToResurrect = false;
		
	//private bool continueButtonIsNowCancelButton = false;	// for canceling purchase of gems in mini store during resurrect menu
	
	void OnEnable()
	{
	
	}
	
	void OnDisable()
	{

	}
	
	void notifyToResurrectByRMBEvent()
    {
		if (GamePlayer.SharedInstance.MovePlayerToNextSafeSpot() == true) 
		{
			//-- Slow down current speed
			//TR.LOG("DeathRunVelocity " + GamePlayer.SharedInstance.DeathRunVelocity + " getRunVelocity() " + GamePlayer.SharedInstance.getRunVelocity());
			//GamePlayer.SharedInstance.SetPlayerVelocity(GamePlayer.SharedInstance.DeathRunVelocity*0.5f);
			//GamePlayer.SharedInstance.SetPlayerVelocity( GamePlayer.SharedInstance.getRunVelocity() );
			//TR.LOG ("Resurrect speed set to {0}, Previous speed was {1}", GamePlayer.SharedInstance.PlayerMagnitude, GamePlayer.SharedInstance.DeathRunVelocity);
			
			if (GamePlayer.SharedInstance.PlayerMagnitude > GamePlayer.SharedInstance.getModfiedMaxRunVelocity()) 
				Debug.Break(); 
			
			GamePlayer.SharedInstance.Resurrect();	//-- give invulnerable for a few seconds.
			
			GameController.SharedInstance.IsHandlingEndGame = false;
			GameController.SharedInstance.IsGameOver = false;

			// reset coin meter
			inGameVC.coinMeter.FadePowerGlow();
			GamePlayer.SharedInstance.ResetCoinCountForBonus();
			
			// go back to game
			hideResurrectMenu();	//disableResurrectMenu();
			inGameVC.OnPause();
			inGameVC.OnUnPaused(gameObject);
		}
		else
		{
			
			notify.Warning ("OnResurrect: Could not find safe spot, going to die");
			inGameVC.OnDiePostGame();
		}
		
	}
	
	void notifyToResurrectByGEMEvent()
    {
		OnResurrect();
		
	}
	
	void notifyToReSumeGameEvent()
    {
		notify.Debug("unity notifyToReSumeGameEvent call!!!");
		chooseToResurrect = false;
		inGameVC.OnDiePostGame();
		
	}
	
	
	void Awake() 
	{
		notify = new Notify(this.GetType().Name);
		notify.Debug("unity Awake call!!!");
		gameObject.AddComponent<UIPanelAlpha>();

        initContinueTxtPos = btnContinueTxt.localPosition;
        initReviveTxtPos = btnReviveTeamTxt.localPosition;
	}
	
    void Start()
    {
        RegisterEvent();
    }

    private void RegisterEvent()
    {
        UIEventListener.Get(btnClose).onClick = OnDie; 
        //UIEventListener.Get(btnClose).onClick += GamePlayer.SharedInstance.ResurrectMenuOnClose;
        UIEventListener.Get(btnContinue.gameObject).onClick =OnContinueButtonClick;
        UIEventListener.Get (btnReviveTeam.gameObject).onClick = OnReviveTeamClick;

    }

    public void OnDie(GameObject obj)
    {
        //if (GamePlayer.SharedInstance.IsDead)
        //    inGameVC.OnDiePostGame();
        //else
        //GameController.SharedInstance.doHandleEndGame();

        UIDynamically.instance.ZoomZeroToOne(gameObject,0.5f,true);

        Invoke("Hide",0.5f);

        StopCoroutine("CountDown");

        GameController.SharedInstance.IsGameOver = true;
       
        UIManagerOz.SharedInstance.inGameVC.bonusButtons.DeadBoostButton.SetActive(false);
    }

    void Hide()
    {
        NGUITools.SetActive(resurrectMenu.gameObject, false);
    }

    public void hideResurrectMenu()
	{
        Hide ();
	}	

	public void enableResurrectMenu() 
	{
		
		chooseToResurrect = false;
		
		NGUITools.SetActive(resurrectMenu.gameObject, true);
        //继续游戏
        if(GameController.SharedInstance.continueTimes<3)
        {
            btnContinue.spriteName="revive_btn";
            continueCostLabel.text = continueCost[GameController.SharedInstance.continueTimes].ToString();//GameProfile.SharedInstance.GetResurrectionCost().ToString();
            continueCostLabel.gameObject.SetActive(true);
            btnContinueTxt.localPosition = initContinueTxtPos; //
            GameController.SharedInstance.ResurrectionCost = continueCost[GameController.SharedInstance.continueTimes];
            btnContinue.collider.enabled = true;
        }
        else
        {
            btnContinue.spriteName="revive_btn1"; //灰色按钮
            continueCostLabel.gameObject.SetActive(false); //隐藏价格
            btnContinueTxt.localPosition = btnContinue.transform.localPosition; //字位置居中
            btnContinue.collider.enabled = false;
            
        }
        //复活小队
        if (GameController.SharedInstance.reviveTimes >= 2 
            || UIManagerOz.SharedInstance.inGameVC.teamIcons.playNums==1 
            || (UIManagerOz.SharedInstance.inGameVC.teamIcons.playNums==2 && GameController.SharedInstance.reviveTimes >= 1))
        {
            btnReviveTeam.spriteName="revive_btn1"; //灰色按钮
            reviveCostLabel.gameObject.SetActive(false); //隐藏价格
            btnReviveTeamTxt.localPosition = btnReviveTeam.transform.localPosition; //字位置居中
            btnReviveTeam.collider.enabled = false;
          
        }
        else
        {
            btnReviveTeam.spriteName="revive_btn";
            reviveCostLabel.text = reviveCost[GameController.SharedInstance.reviveTimes].ToString();
            reviveCostLabel.gameObject.SetActive(true);
            btnReviveTeamTxt.localPosition = initReviveTxtPos;
            btnReviveTeam.collider.enabled = true;
        }


		inGameVC.HidePauseButton();			// hide everything else
		inGameVC.coinMeter.FadePowerGlow();
		
		SetContinueButtonStatus(true);	//ResetContinueButton();			
	}

    //复活飞机
	public void OnContinueButtonClick(GameObject obj)	
	{
        if (GameProfile.SharedInstance.Player.CanAffordResurrect() == false)
        {
//            UIManagerOz.SharedInstance.okayDialog.ShowOkayDialog("钻石不足","Btn_Ok");
            Time.timeScale = 0f;
            UIManagerOz.SharedInstance.StoreVC.BuyGems();

            return;
        }

        ++GameController.SharedInstance.continueTimes;
        //复活次数达到上限
        if (GameController.SharedInstance.continueTimes > 3)
        {
            Time.timeScale = 0f;
            UIManagerOz.SharedInstance.okayDialog.ShowOkayDialog("Msg_ReviveLimit","Btn_Ok");
            UIOkayDialogOz.onPositiveResponse += RecoverTime;
            return;
        }

        //飞机图标变亮
        UIManagerOz.SharedInstance.inGameVC.teamIcons.SetNum1IconLight();

        AudioManager.SharedInstance.SwitchToGameMusic();
        //--------//

		if (!chooseToResurrect)
        {
			OnResurrect();						// go ahead with resurrection process
        }

        inGameVC.ShowPauseButton();	
	}

    //小队接力
    public void OnReviveTeamClick(GameObject obj)   
    {
       
        int curcost = reviveCost[GameController.SharedInstance.reviveTimes];

        if (curcost> GameProfile.SharedInstance.Player.coinCount)
        {
            //            UIManagerOz.SharedInstance.okayDialog.ShowOkayDialog("钻石不足","Btn_Ok");
            Time.timeScale = 0f;
            UIManagerOz.SharedInstance.StoreVC.BuyCoins();
            return;
        }

        GameProfile.SharedInstance.Player.coinCount -= curcost;
        GamePlayer.SharedInstance.AddCoinsToScore(-curcost);
        GameProfile.SharedInstance.Serialize();

        ++GameController.SharedInstance.reviveTimes;

        //改变小队图标顺序
        UIManagerOz.SharedInstance.inGameVC.teamIcons.ChangeIconOrderPosition();

        GamePlayer.SharedInstance.ResetScoreMultiplier();

        hideResurrectMenu();

        inGameVC.ShowPauseButton();	

        GamePlayer.SharedInstance.StartRelays();

        AudioManager.SharedInstance.SwitchToGameMusic();

    }
	
	public void SetContinueButtonStatus(bool active)
	{
		
	}		
	
	public void OnBackButtonClick()		
	{
		SetContinueButtonStatus(true);
		StartResurrectTimer();	//sliderValue);
	}

    void RecoverTime()
    {
        Time.timeScale =1f;
        UIOkayDialogOz.onPositiveResponse -= RecoverTime;
    }

    public void OnResurrect()
    {
        chooseToResurrect = true;

        AudioManager.SharedInstance.StopFX(true);

        // hide store, if open
        if (UIManagerOz.SharedInstance.IAPMiniStoreVC.gameObject.activeSelf)	// != null)
            UIManagerOz.SharedInstance.IAPMiniStoreVC.disappear();


        //If we don't have enough coins in the pool, make sure we take from the current-run gem count
        int curcost = GameProfile.SharedInstance.GetResurrectionCost();
        if (curcost > GameProfile.SharedInstance.Player.specialCurrencyCount)
        {
            curcost -= GameProfile.SharedInstance.Player.specialCurrencyCount;
            GameProfile.SharedInstance.Player.specialCurrencyCount = 0;
            GamePlayer.SharedInstance.AddGemsToScore(-curcost);
        }
        else
        {
            GameProfile.SharedInstance.Player.specialCurrencyCount -= curcost;
        }
       
        // --- Analytics ------------------------------------------------------

        GameProfile.SharedInstance.Serialize();

        GameController.SharedInstance.IsResurrecting = true;

        //-- Slow down current speed
        //GamePlayer.SharedInstance.SetPlayerVelocity(GamePlayer.SharedInstance.DeathRunVelocity*0.5f);
        //GamePlayer.SharedInstance.SetPlayerVelocity( GamePlayer.SharedInstance.getRunVelocity() );

        if (GamePlayer.SharedInstance.PlayerMagnitude > GamePlayer.SharedInstance.getModfiedMaxRunVelocity())
            Debug.Break();

        Resurrect();

        ObjectivesDataUpdater.AddToGenericStat(ObjectiveType.Resurrects,1);
    }

    public void Resurrect(bool isRelay =true)
    {
        GamePlayer.SharedInstance.Resurrect();  //-- give invulnerable for a few seconds.
        
        GameController.SharedInstance.IsHandlingEndGame = false;
        GameController.SharedInstance.IsGameOver = false;
        
        // reset coin meter
        inGameVC.coinMeter.FadePowerGlow();
        //GamePlayer.SharedInstance.ResetCoinCountForBonus();
        
        // go back to game
        hideResurrectMenu();    //disableResurrectMenu();

        if (!isRelay)
        {
            inGameVC.OnPause();
            inGameVC.OnUnPaused(gameObject);
        }
    }

    public void StartResurrectTimer(int startTimerValue = 3)
    {
        iTween.Stop(gameObject);

        //不能复活不能接力（使用完后）
        if (GameController.SharedInstance.continueTimes>=3 &&(GameController.SharedInstance.reviveTimes >= 2 
            || UIManagerOz.SharedInstance.inGameVC.teamIcons.playNums==1 
            || (UIManagerOz.SharedInstance.inGameVC.teamIcons.playNums==2 
            && GameController.SharedInstance.reviveTimes >= 1)))
        {
            GameController.SharedInstance.IsGameOver = true;
            return;
        }

        UIDynamically.instance.ZoomZeroToOne(gameObject,0.5f);

        chooseToResurrect = false;
      
        enableResurrectMenu();

        //int curcost = GameProfile.SharedInstance.GetResurrectionCost();
        //if (GameProfile.SharedInstance.Player.CanAffordResurrect() == false)
        //{
        //    curcost = 0;
        //}
        //float currentDistance = GameController.SharedInstance.DifficultyDistanceTraveled;
        //PlayerStats playerStats = GameProfile.SharedInstance.Player;
        //float bestDistance = playerStats.bestDistanceScore;
        //int difference = 0;
        //int _difference = 0;
        //if ((bestDistance - currentDistance) < 500)
        //{
        //    difference = (int)(bestDistance - currentDistance);

        //}
        //if (bestDistance < currentDistance)
        //{
        //    _difference = (int)(currentDistance - bestDistance);
        //}
        //		PurchaseUtil.notifyToShowResurrectDialog(difference,_difference,curcost);
            
        StartCoroutine("CountDown");
    }

    IEnumerator CountDown()
    {
        countDown.ResetToBeginning();
        int time = countDown.useNames.Count;

        while(true)
        {
            UIDynamically.instance.ZoomOutToOne(countDown.gameObject,new Vector3(2f,2f,1f),0.92f);
            yield return new WaitForSeconds(1f);
            --time;
            if(time<1)
                break;
        }

        NGUITools.SetActive( UIManagerOz.SharedInstance.okayDialog.gameObject, false);

        GameController.SharedInstance.IsGameOver = true;

        UIDynamically.instance.ZoomZeroToOne(gameObject,0.5f,true);
        
        Invoke("Hide",0.5f);

        UIManagerOz.SharedInstance.inGameVC.bonusButtons.DeadBoostButton.SetActive(false);
    }

}
