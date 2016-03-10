using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class UISignIn : MonoBehaviour
{
    public GameObject SureSignIn;
    public GameObject SignInroot;
    public GameObject Rewardroot;
    public GameObject RewardCellroot;
    public GameObject SignInClose;
    public UISprite RewardSprite;
    public UILabel RewardTitleLabel;
    public UILabel RewardNumLabel;
    private bool bFirstShown = false;
    public List<UISignInCellData> Days;

    private void Start()
    {
        RegisterEvent();
    }

    public void appear()
    {
        if (GameController.SharedInstance.levelTutorialPlayed && !bFirstShown)
        {
            bFirstShown = true;
            gameObject.SetActive(true);

            //====登陆数据======凌晨0点刷新

            DateTime myTime = DateTime.Now;
            TimeSpan diff1 = myTime.Subtract(GameProfile.SharedInstance.lastLoginTime.Date);
            TimeSpan diffReal =  myTime.Subtract(GameProfile.SharedInstance.lastLoginTime);
           
            UIManagerOz.SharedInstance.PaperVC.fuelSystem.ReFillFuelByTimeSpan(diffReal); //在重置登录时间之前计算要回复的燃料值

            if (diff1.Days == 1)
            {
                GameProfile.SharedInstance.onLoginDay += 1;
                //GameProfile.SharedInstance.lastLoginTime = DateTime.Now.Date;//当天0点0分0秒
                GameProfile.SharedInstance.isFirstLogin = true;
            }
            else if (diff1.Days > 1)
            {
                GameProfile.SharedInstance.onLoginDay = 1;
                //GameProfile.SharedInstance.lastLoginTime = DateTime.Now.Date; 
                GameProfile.SharedInstance.isFirstLogin = true;
            }


            //首次登陆记录登陆信息

//            if (GameProfile.SharedInstance.isFirstLogin)
//                GameProfile.SharedInstance.Serialize();

            GameProfile.SharedInstance.lastLoginTime = myTime;
            GameProfile.SharedInstance.Serialize();

            for (int i = 0; i < 7; i++)
            {
               
                //已领取else未领取特效
                if (i < GameProfile.SharedInstance.onLoginDay - 1)
                {
                    Days[i].iconRecived.SetActive(true);
                    Days[i].iconeffect.GetComponent<TweenRotation>().enabled = false;
                }
                else if (i > GameProfile.SharedInstance.onLoginDay - 1)
                {
                    Days[i].iconRecived.SetActive(false);
                    Days[i].iconeffect.SetActive(false);
                }

            }

            if (!GameProfile.SharedInstance.isFirstLogin)
            {
                Days[GameProfile.SharedInstance.onLoginDay - 1].iconRecived.SetActive(true); 
                SureSignIn.GetComponent<UISprite>().spriteName = "prompt_sure";

            }
            else
            {
                Days[GameProfile.SharedInstance.onLoginDay - 1].iconRecived.SetActive(false); 
                SureSignIn.GetComponent<UISprite>().spriteName = "prompt_receive";

                UIManagerOz.SharedInstance.PaperVC.fuelSystem.SetFuelFull();//第一次登录燃料填满


            }
        }

    }

    private void disappear(GameObject obj)
    {
        gameObject.SetActive(false);
    }

    private void RegisterEvent()
    {
        UIEventListener.Get(SureSignIn).onClick = OnSureSignIn;
    }

    private void OnSureSignIn(GameObject obj)
    {
        if (GameProfile.SharedInstance.isFirstLogin)
        {

            Days[GameProfile.SharedInstance.onLoginDay - 1].Reveive();
            SureSignIn.GetComponent<BoxCollider>().enabled = false;
            Invoke("ReceiveReward",1f);
            
        }
        else
            disappear(gameObject);

    }
    private void ReceiveReward()
    {
        //奖励
        switch (GameProfile.SharedInstance.onLoginDay)
        {
            case 1:
                GameProfile.SharedInstance.Player.coinCount += 500;
                RewardSprite.spriteName = "common_coin";
                RewardTitleLabel.text = "金币";
                RewardNumLabel.text = "500";
                break;
            case 2:
                GameProfile.SharedInstance.Player.numberChanceTokens += 3;
                RewardSprite.spriteName = "common_treasurebox";
                RewardTitleLabel.text = "宝箱";
                RewardNumLabel.text = "3";
                break;
            case 3:
                GameProfile.SharedInstance.Player.coinCount += 1000;
                RewardSprite.spriteName = "common_coin";
                RewardTitleLabel.text = "金币";
                RewardNumLabel.text = "1000";
                break;
            case 4:
                GameProfile.SharedInstance.Player.numberChanceTokens += 5;
                RewardSprite.spriteName = "common_treasurebox";
                RewardTitleLabel.text = "宝箱";
                RewardNumLabel.text = "5";
                break;
            case 5:
                GameProfile.SharedInstance.Player.medalGoldCount += 5;
                RewardSprite.spriteName = "madalgold";
                RewardTitleLabel.text = "金勋章";
                RewardNumLabel.text = "5";
                break;
            case 6:
                GameProfile.SharedInstance.Player.numberChanceTokens += 6;
                RewardSprite.spriteName = "common_treasurebox";
                RewardTitleLabel.text = "宝箱";
                RewardNumLabel.text = "6";
                break;
            case 7:
                GameProfile.SharedInstance.Player.specialCurrencyCount += 100;
                RewardSprite.spriteName = "common_gem";
                RewardTitleLabel.text = "钻石";
                RewardNumLabel.text = "100";
                break;

        }
        UIManagerOz.SharedInstance.PaperVC.UpdateCurrency();

        SignInroot.SetActive(false);
        Rewardroot.SetActive(true);
        UIDynamically.instance.ZoomZeroToOne(RewardCellroot, 0.5f);
        UIEventListener.Get(SignInClose).onClick = disappear;
    
    
    }

}
