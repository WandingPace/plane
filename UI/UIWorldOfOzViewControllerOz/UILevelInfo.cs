using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class UILevelInfo : MonoBehaviour {

    public GameObject btnClose,btnInto,tutorialFinger;
    public UILabel titleTxt, envName, star1Num, star2Num, star3Num, descTxt,descCondition, rewardnum1, rewardnum2, rewardnum3;
    public UISprite star1,star2,star3,progressBar;
    public List<UISprite> level_rewardSlot,level_rewardStaricon;
    private ObjectiveProtoData _data;
    private int starCount;

    void Awake()
    {
      
    }

	void Start () 
    {
        if (btnClose!=null)
            UIEventListener.Get(btnClose).onClick = disappear;
        if (btnInto != null)
            UIEventListener.Get(btnInto).onClick = OnPlayClicked;
	}
	
    public void OnPlayClicked(GameObject obj)
    {

        if(_data._conditionList[0]._isBigLevel&&!_data._conditionList[0]._isBigLevelUnlock)
        {

            UIConfirmDialogOz.onPositiveResponse += CancelUnLock;
            UIConfirmDialogOz.onNegativeResponse += SureUnLock;
            UIManagerOz.SharedInstance.confirmDialog.ShowConfirmDialog("花费200钻石解锁改关卡", "解锁", "确定", 200);
            return;
        }
        else
        {
            if (GameController.SharedInstance.gameState != GameState.IN_RUN)    // only trigger this once per run
            {
                

                //if (GameController.SharedInstance.levelTutorialPlayed)
                if (GameController.SharedInstance.GetTutorialIDforSys() > 1)
                {
                    if (!UIManagerOz.SharedInstance.PaperVC.fuelSystem.IsFuelEnough())
                    {
                        UIManagerOz.SharedInstance.PaperVC.fuelSystem.CannotIntoGame();
                        return;
                    }
                    else
                    {
                        UIManagerOz.SharedInstance.PaperVC.fuelSystem.AddFuelCount(-2);//消耗燃料
                    }
                }
             
                //关卡引导结束
                //if ( !GameController.SharedInstance.levelTutorialPlayed)
                if (GameController.SharedInstance.GetTutorialIDforSys() == 1)
                {
                    //tutorialFinger.SetActive(false);
                    //GameController.SharedInstance.levelTutorialPlayed = true;
                    //PlayerPrefs.SetInt("levelTutorialPlayedInt", 1);
                    //PlayerPrefs.Save();
                }

                GameProfile.SharedInstance.Player.activeLevel = _data._id;

                GameController.SharedInstance.EndlessMode = false;

                GameController.SharedInstance.SwitchEnviroment(_data._environmentID);

                StartCoroutine(WaitOilEffect());
            }
        }

    }

    private IEnumerator WaitOilEffect()
    {
        Vector3 fromPos = UIManagerOz.SharedInstance.PaperVC.oilPos.position;
        
        OilEffect.instance.PlayEffect(btnInto.transform.position,fromPos);
        
        while(!OilEffect.instance.isPlayFinished)
        {
            yield return null;
        }
        
        UIManagerOz.SharedInstance.inventoryVC.appear();
        //关卡3d飞机改变层次
        UIManagerOz.SharedInstance.worldOfOzVC.worldList.disappear();
        CancelInvoke("StartMainMenuMusicOnStart");
        
        disappear(gameObject);
        
        UIManagerOz.SharedInstance.worldOfOzVC.disappear();
      
        yield break;
    }

    private void CancelUnLock()
    {
        UIConfirmDialogOz.onPositiveResponse -= CancelUnLock;
        UIConfirmDialogOz.onNegativeResponse -= SureUnLock;
    }

    private void SureUnLock()
    {
        if (GameProfile.SharedInstance.Player.specialCurrencyCount <= 200)
        {
            UIManagerOz.SharedInstance.StoreVC.BuyGems();
            Time.timeScale = 0f;
            return;
        }
        GameProfile.SharedInstance.Player.specialCurrencyCount -= 200;
        UIManagerOz.SharedInstance.PaperVC.UpdateCurrency();

        btnInto.GetComponent<UISprite>().spriteName = "level_into";
        _data._conditionList[0]._isBigLevelUnlock = true;
        ObjectivesManager.SaveLevelProgress();

        UIConfirmDialogOz.onPositiveResponse -= CancelUnLock;
        UIConfirmDialogOz.onNegativeResponse -= SureUnLock;
    }
	public void appear(ObjectiveProtoData data,int starCount)
    {
        _data = data;
        this.starCount = starCount;

        UIManagerOz.SharedInstance.worldOfOzVC.SetStarPosition(progressBar,star1.gameObject,star2.gameObject,star3.gameObject,true);

        Refresh();

        //关卡引导
        //if(GameController.SharedInstance.upgradeTutorialPlayed
        //   && !GameController.SharedInstance.levelTutorialPlayed)
        if (GameController.SharedInstance.GetTutorialIDforSys() == 1)
        {
            UIManagerOz.SharedInstance.worldOfOzVC.worldList.tutorialFinger.SetActive(false);
            tutorialFinger.SetActive(true);
            var tutorialTS = TweenScale.Begin(tutorialFinger, 1.0f, Vector3.one);
            tutorialTS.to = Vector3.one * 0.8f;
            tutorialTS.style = UITweener.Style.Loop;
            btnClose.collider.enabled = false;
        }
        else if (tutorialFinger)
        {
            tutorialFinger.SetActive(false);
            btnClose.collider.enabled = true;
        }
    }

    private void  Refresh()
    {
        if(_data != null)
        {
            
            if (btnInto != null)
            {
                if (_data._conditionList[0]._isBigLevel)
                {   
                    //当前大关已解锁
                    if (_data._conditionList[0]._isBigLevelUnlock)
                        btnInto.GetComponent<UISprite>().spriteName = "level_into";
                     else 
                        btnInto.GetComponent<UISprite>().spriteName = "level_unlock";
                }
            }

            titleTxt.text = _data._title;
            envName.text = GetEnvNameByID(_data._environmentID);
            star1Num.text = _data._conditionList[0]._statValue1ForLevel.ToString();
            star2Num.text = _data._conditionList[0]._statValue2ForLevel.ToString();
            star3Num.text = _data._conditionList[0]._statValue3ForLevel.ToString();
            if (level_rewardSlot != null && level_rewardSlot.Count>0)
            {
                for (int i = 0; i < 3; i++)
                {
                    if (i < starCount)
                    {
                        level_rewardSlot[i].spriteName = "level_reward_slot_grey";
                        level_rewardStaricon[i].color = new Color(0.3f,0.3f,0.3f);
                    }
                    else
                    {
                        level_rewardSlot[i].spriteName = "level_reward_slot";
                        level_rewardStaricon[i].color = new Color(1f, 1f, 1f);
                    }
                }
            }
             int val;
            if(starCount < 1)
            {
                val = _data._conditionList[0]._statValue1ForLevel;
            }
            else if(starCount>=1 && starCount<2)
            {
                val = _data._conditionList[0]._statValue2ForLevel;                
            }
            else
            {
                val = _data._conditionList[0]._statValue3ForLevel;              
            }

            descTxt.text = string.Format(_data._descriptionPreEarned, val);

            if (descCondition != null)
            {
                descCondition.text = string.Format("关卡模式获得{0}个钻石", _data._conditionList[0]._BigLevelunLockStarNeed);
                rewardnum1.text = string.Format("钻石*{0}", _data._conditionList[0]._BigLevelReward1);
                rewardnum2.text = string.Format("钻石*{0}", _data._conditionList[0]._BigLevelReward2);
                rewardnum3.text = string.Format("钻石*{0}", _data._conditionList[0]._BigLevelReward3);
            }
            UIManagerOz.SharedInstance.worldOfOzVC.UpdateStarSprite(star1,star2,star3,starCount);
            UIManagerOz.SharedInstance.worldOfOzVC.UpdateStarProgressBar(progressBar,_data);

            gameObject.SetActive(true);
        }
    }


    private string GetEnvNameByID(int id)
    {
        string name="";
        switch(id)
        {
            case 1: name = "冰雪关";break;
            case 2: name = "墨西哥关"; break;
            case 3: name = "印度关"; break;
        }
        return name + GetLevelID();
    }
    private string GetLevelID()
    {
        string bigLevelID = "";
        string smallLevelID = "";
        int biglevelcount = 1;
        int latestbiglevel = -1;
       foreach(ObjectiveProtoData ob in ObjectivesManager.LevelObjectives)
       {
           if (ob._id<_data._id && ob._conditionList[0]._isBigLevel)
           {
              biglevelcount++;
              latestbiglevel = ob._id;
           }
       
       }
       bigLevelID = biglevelcount.ToString();
       smallLevelID = (_data._id - latestbiglevel).ToString();

       return bigLevelID + "-" + smallLevelID;
    }

    public void disappear(GameObject obj)
    {
        gameObject.SetActive(false);
       // UIManagerOz.SharedInstance.worldOfOzVC.worldList.plane.SetActive(true);
      //  UIManagerOz.SharedInstance.worldOfOzVC.worldList.PlayAnim(UIManagerOz.SharedInstance.worldOfOzVC.worldList.PlaneModel, "Idle");
    }

}
