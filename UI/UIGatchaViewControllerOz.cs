using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIGatchaViewControllerOz : UIViewControllerOz
{
    public GatchaCard showBox;
    public GatchaCard hideBox;
   
//    public GameObject continueTip;

    [HideInInspector]
    public int availableCards;
	private GatchaData data;
	private int totalCardsFlipped = 0;
    private float screenOffset;
    private float originalY;
	protected override void Awake()
    {
		base.Awake();
		data = GetComponent<GatchaData>();
        screenOffset = 720f;
        originalY = -160f;
	}
	
    void OnDisable()
    {
        GameProfile.SharedInstance.Serialize();
    }

	public override void appear()
	{
		Reset();

        base.appear();

       
        //产生宝箱盒
		DealCards(gameObject);
       
	}
	
    //产生宝箱盒
	private void DealCards(GameObject obj)
    {
        //
        ShowCards(showBox);
        HideCards(hideBox);
      
	}

    void HideCards(GatchaCard card)
    {
        card.boxModel.SetSpeed(-1f);
        card.boxModel.Play();

        iTween.ScaleTo(card.awardDesc.transform.parent.gameObject, iTween.Hash(
            "scale",  Vector3.zero,
            "time", 0.1f,
            "easetype", iTween.EaseType.easeOutBack));
            
        StopEffect(card);
        if(card.gameObject.activeSelf)
        { 
            iTween.MoveTo(card.gameObject,iTween.Hash(
                "x",screenOffset,
                "islocal",true,
                "time",1.1f,
                "easytype",iTween.EaseType.easeInOutBack,
                "oncomplete","HideCardsFinished",
                "oncompletetarget",gameObject,
                "oncompleteparams",card
                ));
        }
        else
        {   //每次appear()进入此页面
            HideCardsFinished(card);
        }
    }


    void HideCardsFinished(GatchaCard card)
    {
        card.gameObject.SetActive(false); 
        card.gameObject.collider.enabled = false;
        card.transform.SetLocalPositionX(-screenOffset);
        card.BoxRoot.transform.localPosition = new Vector3(0, originalY, 0);
        showBox = card;
	}

    void StopEffect(GatchaCard card)
    {
        card.fx.Stop(true);
        card.fx.Clear();
        card.fxEmpty.Stop(true);
        card.fxEmpty.Clear();
        card.fxOpenBox.Stop(true);
        card.fxOpenBox.Clear();
    }

    void ShowCards(GatchaCard card)
    {
       
        card.transform.SetLocalPositionX(-screenOffset);
        card.gameObject.SetActive(true);
        card.gameObject.collider.enabled = false;
        card.BoxRoot.transform.localPosition = new Vector3(0, originalY, 0);
        //注册事件
        UIEventListener.Get(card.gameObject).onClick = OnCardPressed;
        //产生随机物品
        PopulateCards(card);

        iTween.MoveTo(card.gameObject,iTween.Hash(
            "x",0f,
            "islocal",true,
            "time",1f,
            "easytype",iTween.EaseType.easeInQuad,
            "oncomplete","ShowCardsFinished",
            "oncompletetarget",gameObject,
            "oncompleteparams",card
            ));

        //OnNextPressed();
    }
	
    void ShowCardsFinished(GatchaCard card)
    {
        card.gameObject.collider.enabled = true;
        hideBox = card;

    }

//    private GameObject CreateBaoxiang()
//    {
//        GameObject obj = GameObject.Instantiate(boxPrefab) as GameObject;
//        obj.transform.ResetTransformation();
//        return obj;
//    }

	private void Reset()
    {
        totalCardsFlipped = 0;

        showBox.transform.SetLocalPositionX(-screenOffset);
        hideBox.transform.SetLocalPositionX(-screenOffset);

        showBox.BoxRoot.transform.localPosition = new Vector3(0, originalY, 0);
        hideBox.BoxRoot.transform.localPosition = new Vector3(0, originalY, 0);
        showBox.awardDesc.transform.parent.localScale = Vector3.zero;
        hideBox.awardDesc.transform.parent.localScale = Vector3.zero;
        hideBox.gameObject.SetActive(false);
	}


	public void OnCardPressed(GameObject card)
    {
		GatchaCard gc = card.GetComponent<GatchaCard>();
		if(gc.isFlipped) return;
        FlipCard(gc);
	}
	
	private void FlipCard(GatchaCard gc)
    {
		GameProfile.SharedInstance.Player.PopChanceToken();
		
		ObjectivesDataUpdater.AddToGenericStat(ObjectiveType.UsedDestinyCard,1);
		
		gc.isFlipped = true;
		totalCardsFlipped++;

        //播放宝箱动画
        hideBox.boxModel.SetSpeed(1f);
        gc.boxModel.Play();

		if(gc.data.type == GatchaType.EMPTY)
        {
			AudioManager.SharedInstance.PlayFX(AudioManager.Effects.oz_Poof_activate);
			gc.fxEmpty.Play(true);
		}
		else
        {
			AudioManager.SharedInstance.PlayFX(AudioManager.Effects.oz_UI_Menu_levelUp);
            gc.fxOpenBox.gameObject.SetActive(true);
            gc.fx.gameObject.SetActive(true);
			gc.fxOpenBox.Play(true);
            gc.fx.Play();
		}

		FlipCardComplete(gc);
	}
	
	
	
	public void FlipCardComplete(GatchaCard gc)
    {
	
		if(gc.data != null)
        {
			switch(gc.data.type)
            {
				case GatchaType.EMPTY:
                    if(totalCardsFlipped == availableCards)
                    {
                        UIEventListener.Get(gc.gameObject).onClick = Finished;
                    
//						Invoke("RemoveGatchaIfDone", 1.5f);
					}
					break;
				case GatchaType.COINS:

                    //int currentCount =
                        GameProfile.SharedInstance.UpdateCoinsPostSession(gc.data.amount,true);
					AudioManager.SharedInstance.PlayFX(AudioManager.Effects.oz_Apprentice_01);

					break;

            case GatchaType.Gem:
                
                GameProfile.SharedInstance.Player.specialCurrencyCount+=gc.data.amount;
                AudioManager.SharedInstance.PlayFX(AudioManager.Effects.oz_Apprentice_01);
                
                break;

//				case GatchaType.SCORE_BONUS:
//					AudioManager.SharedInstance.PlayFX(AudioManager.Effects.oz_Apprentice_01);
//					GamePlayer.SharedInstance.AddScore(gc.data.amount, true);
//					break;

				case GatchaType.HeadBoost:
					GameProfile.SharedInstance.Player.EarnConsumable(UIManagerOz.SharedInstance.inGameVC.bonusButtons.HeadBoostID, gc.data.amount);	//.BigHeadStartID, gc.data.amount);
					AudioManager.SharedInstance.PlayFX(AudioManager.Effects.oz_Apprentice_01);
				
					break;
//				case GatchaType.DeadBoost:
//                    GameProfile.SharedInstance.Player.EarnConsumable(UIManagerOz.SharedInstance.inGameVC.bonusButtons.DeadBoostID, gc.data.amount);
//					AudioManager.SharedInstance.PlayFX(AudioManager.Effects.oz_Apprentice_01);
//
//					break;
				case GatchaType.Boost:
                    GameProfile.SharedInstance.Player.EarnConsumable(UIManagerOz.SharedInstance.inGameVC.bonusButtons.BoostID, gc.data.amount);
					AudioManager.SharedInstance.PlayFX(AudioManager.Effects.oz_Apprentice_01);

					break;
				case GatchaType.Shield:
                    GameProfile.SharedInstance.Player.EarnConsumable(UIManagerOz.SharedInstance.inGameVC.bonusButtons.ShieldID, gc.data.amount);
					AudioManager.SharedInstance.PlayFX(AudioManager.Effects.oz_Apprentice_01);

                    break;
                case GatchaType.Vacuum:
                    GameProfile.SharedInstance.Player.EarnConsumable(UIManagerOz.SharedInstance.inGameVC.bonusButtons.VacuumID, gc.data.amount);
                    AudioManager.SharedInstance.PlayFX(AudioManager.Effects.oz_Apprentice_01);

                     break; 
//                case GatchaType.Fragment:
//                    GameProfile.SharedInstance.Player.fragments+=gc.data.amount;
//                    AudioManager.SharedInstance.PlayFX(AudioManager.Effects.oz_Apprentice_01);
//                    
//                    break;
			}
            //宝箱动作
            StartCoroutine(AnimTreasureBox(gc));

		}

	
	}
    IEnumerator AnimTreasureBox(GatchaCard gc)
    {

        yield return new WaitForSeconds(1f);
        gc.fxOpenBox.gameObject.SetActive(false);
        gc.fx.gameObject.SetActive(false);
        //宝箱内容
        StartCoroutine(AnimateIcon(gc));
        yield return new WaitForSeconds(0.2f);
        iTween.MoveTo(gc.BoxRoot.gameObject, iTween.Hash(
        "y", -1100f,
         "islocal", true,
         "time", 2f,
         "easytype", iTween.EaseType.linear,
           "oncomplete", "AnimateIconComplete",
           "oncompletetarget", gameObject,
           "oncompleteparams", gc
         ));
       // yield return new WaitForSeconds(0.5f);
        
        yield break;
    }

	public void OnEscapeButtonClickedModel()
	{
		if( UIManagerOz.escapeHandled ) return;
		UIManagerOz.escapeHandled = true;

	}		
	
    //跳过（不花钱开宝箱）
    public void OnNextPressed()
    {
        StartCoroutine(OnNextPressedLoop());
    }
	
	public IEnumerator OnNextPressedLoop()
    {
		yield return StartCoroutine( FlipRemainingCards());
		OnRemoveGatcha();
	}
	
    //结束开箱
	public void OnRemoveGatcha()
    {
		
		GameProfile.SharedInstance.Serialize();

		NGUITools.SetActive(UIManagerOz.SharedInstance.inGameVC.gameObject, false);

       
         int starCount = UIManagerOz.SharedInstance.worldOfOzVC.GetCurLevelStarCount();
         if (!GameController.SharedInstance.EndlessMode && starCount == WorldOfOzCellData.PerfectClearStar)
        {
            UIManagerOz.SharedInstance.postGameVC.postAcount.PostAccountRoot.SetActive(true);
            UIManagerOz.SharedInstance.postGameVC.postAcount.next = true;

        }
         else
		    UIManagerOz.SharedInstance.postGameVC.appear();
		
		bool testWeeklyDisplay = Settings.GetBool( "always-display-weekly-postrun", false );
		
		if ( testWeeklyDisplay || ObjectivesDataUpdater.AreAnyWeeklyChallengesCompleted() )
		{
			UIManagerOz.SharedInstance.postGameVC.ShowWeeklyChallengesPage();	
		}
		else
		{
			UIManagerOz.SharedInstance.postGameVC.ShowObjectivesPage();	
		}
		
	
		UIManagerOz.SharedInstance.SetUICameraClearFlagToSolidColorBG(true);		
		
		disappear();
	}
	
    private void PopulateCards(GatchaCard card)
    {
            card.musicBox.spriteName = "common_treasurebox";
            card.awardDesc.transform.parent.localScale = Vector3.zero;

            //随机产生奖品
			GatchaDataSet gds = GetRandomGatcha();
            
			if(gds != null)
            {

				card.data = gds;
                card.isFlipped = false;

			    switch (gds.type)
			    {
			        case GatchaType.EMPTY:
                        card.awardIcon.spriteName ="";
                        card.awardDesc.text = "啊哦，空的";
			            break;

//                    case GatchaType.SCORE_BONUS:
//                        card.awardIcon.spriteName = "common_scoremutiplier";
//                        card.awardDesc.text = "获得双倍得分"+card.data.amount+"个";
//                        card.amount.text ="";//"x"+card.data.amount;
//                        break;

			        case GatchaType.COINS:
                        int amount = Random.Range(200,301);
                        card.data.amount = amount;
                        card.awardDesc.text = card.data.amount.ToString() + "个";
                        card.amount.text ="";
			            card.awardIcon.spriteName = "common_coin";
			            break;
			      
			        case GatchaType.HeadBoost:
                        card.awardDesc.text =
                            GameProfile.SharedInstance.Player.GetConsumableLocalizeString(
                        UIManagerOz.SharedInstance.inGameVC.bonusButtons.HeadBoostID) + card.data.amount.ToString() + "个";
                        card.amount.text ="";
                        card.awardIcon.spriteName ="common_takeoff";
			            break;

//			        case GatchaType.DeadBoost:
//                        card.awardDesc.text = "获得"+
//			                GameProfile.SharedInstance.Player.GetConsumableLocalizeString(
//                        UIManagerOz.SharedInstance.inGameVC.bonusButtons.DeadBoostID)+card.data.amount+"个";
//                        card.awardIcon.spriteName ="common_crash";
//                        card.amount.text ="";
//                
//			            break;

			        case GatchaType.Boost:
                         card.awardDesc.text = 
			                GameProfile.SharedInstance.Player.GetConsumableLocalizeString(
                        UIManagerOz.SharedInstance.inGameVC.bonusButtons.BoostID) + card.data.amount.ToString() + "个";
                        card.awardIcon.spriteName = "common_speedpotion";
                        card.amount.text ="";
                
			            break;

			        case GatchaType.Shield:
                        card.awardDesc.text = 
			                GameProfile.SharedInstance.Player.GetConsumableLocalizeString(
                        UIManagerOz.SharedInstance.inGameVC.bonusButtons.ShieldID) + card.data.amount + "个";
                        card.amount.text ="";
                         card.awardIcon.spriteName ="common_shield";
			            break;

			        case GatchaType.Vacuum:
                        card.awardDesc.text =
			                GameProfile.SharedInstance.Player.GetConsumableLocalizeString(
                        UIManagerOz.SharedInstance.inGameVC.bonusButtons.VacuumID) + card.data.amount + "个";
                        card.amount.text ="";
                        card.awardIcon.spriteName ="common_magnet";
			            break;
//                    case GatchaType.Fragment:
//                        card.awardDesc.text ="获得碎片"+card.data.amount+"个";
//                        card.awardIcon.spriteName ="role_frag";
//                        card.amount.text = "集齐一定数量可兑换特定飞机角色";
//                        break;
                    case GatchaType.Gem:
                        amount = Random.Range(1, 3);
                        card.data.amount = amount;
                        card.awardDesc.text = card.data.amount.ToString() + "个";
                        card.amount.text ="";
                        card.awardIcon.spriteName = "common_gem";
                        break;

                    default:
                        card.awardIcon.spriteName ="";
                        card.awardDesc.text = "";
                        card.amount.text ="";
                
                         break;
       			    }
             
			}
			else
            {
				notify.Debug("card is Empty");
			}
		
	}

	//随机产生奖品
	private GatchaDataSet GetRandomGatcha()
    {
		int totalWeight = 0;
		foreach(GatchaDataSet gds in data.gatchaList)
        {
			if(gds.active)
            {
				totalWeight += gds.randomWeight;
			}
		}

		int rand = (int)(Random.value * (float)totalWeight);
//		Debug.LogError(totalWeight + " rand " + rand);
		
		
		GatchaDataSet g = new GatchaDataSet();
		g.type = GatchaType.EMPTY;
		g.amount = 0;
		
		
		foreach(GatchaDataSet gds in data.gatchaList)
        {
			if(gds.active)
            {
				rand -= gds.randomWeight;
				if(rand < 0)
                {
//					Debug.LogError(" the winner is " + gds.type + " amount " + gds.amount);
					g.type = gds.type;
					g.amount = gds.amount;
					return g;
				}
			}
		}

		return g;
		
		
	}


    private IEnumerator AnimateIcon(GatchaCard card)
    {

        iTween.ScaleTo(card.awardDesc.transform.parent.gameObject, iTween.Hash(
			"scale",  Vector3.one,
			"time", 1.5f,
			"easetype", iTween.EaseType.easeOutBack
            //"oncomplete", "AnimateIconComplete",
            //"oncompletetarget", gameObject,
            //"oncompleteparams",card
			));
        yield break;
	}
	
    public void AnimateIconComplete(GatchaCard card)
    {
       
        if(totalCardsFlipped == availableCards){
//			Invoke("RemoveGatchaIfDone", 1f);
            UIEventListener.Get(card.gameObject).onClick = Finished;
            return;
		}
        
        //注册事件
        UIEventListener.Get(card.gameObject).onClick = DealCards ;
      
	}
	
    private void Finished(GameObject obj)
    {
        RemoveGatchaIfDone();
    }

    //打开剩余宝箱
	private IEnumerator FlipRemainingCards()
    {
		int i = 0;
		//AudioManager.SharedInstance.PlayFX(AudioManager.Effects.oz_Gatcha_PickupDeck);
		bool delayGame = false;
        GatchaCard gc = new GatchaCard();
        {
			if(!gc.isFlipped)
            {
				delayGame = true;
				yield return new WaitForSeconds(0.2f);
				
//				gc.musicBox.spriteName = "musicbox_opened";



				AudioManager.SharedInstance.PlayFX(AudioManager.Effects.oz_UI_Menu_back);

				if(gc.data.type != GatchaType.EMPTY)
                {
					gc.awardDesc.enabled = true;
					iTween.ScaleTo(gc.awardDesc.gameObject, iTween.Hash(
						"scale",  Vector3.one ,
						"time", 0.5f,
						"easetype", iTween.EaseType.easeOutBack
						));
						
					if(gc.data.type == GatchaType.COINS || gc.data.type == GatchaType.SCORE_BONUS)
                    {
                        gc.awardIcon.transform.localPosition = gc.awardDesc.transform.localPosition + new Vector3(-55f, 0f, -20f);
					}
                    gc.awardIcon.enabled = true;
                    iTween.ScaleTo(gc.awardIcon.gameObject, iTween.Hash(
                        "scale",  gc.awardIcon.transform.localScale ,
						"time", 0.4f,
						"delay", 0.3f,
						"easetype", iTween.EaseType.easeOutBack
						));	
				}
				i++;
			}
		}
		if(delayGame){
			yield return new WaitForSeconds(2f);
		}
	}
	


	private void RemoveGatchaIfDone()
    {
       
        if(GameController.SharedInstance.gameState == GameState.IN_MENUS)
        {
            disappear();
            UIManagerOz.SharedInstance.inventoryVC.clist.Refresh();
            return;
        }
	     OnRemoveGatcha();

	}

	
	IEnumerator EnableSwitchCardOnTimer(float time){
		yield return new WaitForSeconds(time);
		
	}
	
	
	public void OnNeedMoreCoinsNoInGame() 	// use in-game only, goes to mini store.  Used on gatcha screen.
	{
		//UIConfirmDialogOz.ClearEventHandlers();
		UIConfirmDialogOz.onNegativeResponse -= OnNeedMoreCoinsNoInGame;
		UIConfirmDialogOz.onPositiveResponse -= OnNeedMoreCoinsYesInGame;
	}
	
	public void OnNeedMoreCoinsYesInGame()  // use in-game only, goes to mini store.  Used on gatcha screen.
	{
		//UIConfirmDialogOz.ClearEventHandlers();
		UIConfirmDialogOz.onNegativeResponse -= OnNeedMoreCoinsNoInGame;
		UIConfirmDialogOz.onPositiveResponse -= OnNeedMoreCoinsYesInGame;
		
		UIManagerOz.SharedInstance.GoToMiniStore(ShopScreenName.Coins, false);	//"coins");	// send player to in-game mini store, coins page
	}		
	
}


