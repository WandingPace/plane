using UnityEngine;
using System.Collections;

public class Teammate : MonoBehaviour {

    public UISprite team1;
    public UISprite team2;
    public UISprite team3;

    private Vector3 pos1;
    private Vector3 pos2;
    private Vector3 pos3;
    private int sizeW1;
    private int sizeW2;
    private int sizeW3;
    private int sizeH1;
    private int sizeH2;
    private int sizeH3;
    
    private UISprite num1;
    public int playNums;

	void Awake() 
    {
        pos1 = team1.transform.localPosition;
        pos2 = team2.transform.localPosition;
        pos3 = team3.transform.localPosition;

        sizeW1 = team1.width;
        sizeW2 = team2.width;
        sizeW3 = team3.width;

        sizeH1 = team1.height;
        sizeH2 = team2.height;
        sizeH3 = team3.height;

        num1 = team1;
     
	}
	
	
    void SetTeammateActive(int count)
    {
        switch(count)
        {
            case 1:
                NGUIToolsExt.SetActive(team1.gameObject,true);
                team1.spriteName = GameProfile.SharedInstance.GetActiveCharacter().IconName;
                NGUIToolsExt.SetActive(team2.gameObject,false);
                NGUIToolsExt.SetActive(team3.gameObject,false);
            break;
            case 2:
                 team1.spriteName = GameProfile.SharedInstance.GetActiveCharacter().IconName;
                 team2.spriteName =UIManagerOz.SharedInstance.chaSelVC.GetCharacterByOrderIndex(
                    GameProfile.SharedInstance.Player.teamIndexsOrder[1]).IconName;
                NGUIToolsExt.SetActive(team1.gameObject,true);
                NGUIToolsExt.SetActive(team2.gameObject,true);
                NGUIToolsExt.SetActive(team3.gameObject,false);
                break;
            case 3:
            team1.spriteName = GameProfile.SharedInstance.GetActiveCharacter().IconName;
            team2.spriteName =UIManagerOz.SharedInstance.chaSelVC.GetCharacterByOrderIndex(
                GameProfile.SharedInstance.Player.teamIndexsOrder[1]).IconName;
            team3.spriteName =UIManagerOz.SharedInstance.chaSelVC.GetCharacterByOrderIndex(
                GameProfile.SharedInstance.Player.teamIndexsOrder[2]).IconName;
                NGUIToolsExt.SetActive(team1.gameObject,true);
                NGUIToolsExt.SetActive(team2.gameObject,true);
                NGUIToolsExt.SetActive(team3.gameObject,true);
                break;
        }
    }

    public void Show()
    {
        Reset();
        gameObject.SetActive(true);
        int count = GameProfile.SharedInstance.Player.teamIndexsOrder.Count;
        if(count ==1 ||( count == 2 && GameProfile.SharedInstance.Player.teamIndexsOrder[1]==-1)
           ||( count == 3 && GameProfile.SharedInstance.Player.teamIndexsOrder[1]==-1 && GameProfile.SharedInstance.Player.teamIndexsOrder[2]==-1))
        {
            SetTeammateActive(1);
            playNums = 1;
        }
        else if((count == 2 && GameProfile.SharedInstance.Player.teamIndexsOrder[1] != -1)
                ||( count == 3 && GameProfile.SharedInstance.Player.teamIndexsOrder[1] !=-1 && GameProfile.SharedInstance.Player.teamIndexsOrder[2]==-1))
        {
            SetTeammateActive(2);
            playNums = 2;
        }
        else if(count == 3 && GameProfile.SharedInstance.Player.teamIndexsOrder[2] != -1)
        {
            SetTeammateActive(3);
            playNums = 3;
        }
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }


    int relayCount =0;
    public void ChangeIconOrderPosition()
    {
        if(playNums ==1)
            return;
       
         ++relayCount;

        if(playNums == 2)
        {
            MoveToPosition(team1.gameObject,pos2,1f);
            MoveToPosition(team2.gameObject,pos1,1f);
            num1 = team2;
            team1.width = sizeW2;
            team1.height = sizeH2;
            team2.width = sizeW1;
            team2.height = sizeH1;
        }
        else if(playNums == 3)
        {
            if(relayCount ==1)
            {
                MoveToPosition(team1.gameObject,pos3,1f);
                MoveToPosition(team2.gameObject,pos1,1f);
                MoveToPosition(team3.gameObject,pos2,1f);
                num1 = team2;

                team1.width = sizeW3;
                team1.height = sizeH3;
                team2.width = sizeW1;
                team2.height = sizeH1;
                team3.width = sizeW2;
                team3.height = sizeH2;
            }
            else if(relayCount == 2)
            {
                MoveToPosition(team1.gameObject,pos2,1f);
                MoveToPosition(team2.gameObject,pos3,1f);
                MoveToPosition(team3.gameObject,pos1,1f);
                num1 = team3;

                team1.width = sizeW2;
                team1.height = sizeH2;
                team2.width = sizeW3;
                team2.height = sizeH3;
                team3.width = sizeW1;
                team3.height = sizeH1;
            }
        }
    }

    public void SetNum1IconLight()
    {
        num1.color = Color.white;
    }
    public void SetNum1IconGray()
    {
        num1.color = Color.gray;
    }

    private void Reset()
    {
        team1.color = Color.white;
        team2.color = Color.white;
        team3.color = Color.white;
        num1 = team1;
        team1.transform.localPosition = pos1;
        team2.transform.localPosition = pos2;
        team3.transform.localPosition = pos3;

        team1.width = sizeW1;
        team1.height = sizeH1;
        team2.width = sizeW2;
        team2.height = sizeH2;
        team3.width = sizeW3;
        team3.height = sizeH3;

        relayCount =0;
    }

    private void MoveToPosition(GameObject obj,Vector3 toPos,float durTime,float delayTime = 0f)
    {
        iTween.MoveTo(obj,iTween.Hash(
            "islocal",true,
            "position",toPos,
            "time",durTime,
            "delay",delayTime,
            "easytype",iTween.EaseType.easeInBounce
            
            ));

//        iTween.ValueTo(obj,iTween.Hash(
//            "from", 0,
//            "to", 1f,
//            "time",durTime,
//            "onupdate","ZoomBG",
//            "onupdatetarget", gameObject
//            ));
    }

//    void ZoomBG(float val)
//    {
//        int tempW = Mathf.FloorToInt(snapBGW*val);
//        //        int tempH = Mathf.FloorToInt(snapBGH*val);
//        BGTexture.width = tempW;
//        //        BGTexture.height = tempH;
//        
//        int atempW = Mathf.FloorToInt(asnapBGW*val);
//        int atempH = Mathf.FloorToInt(asnapBGH*val);
//
//        
//    }

  
}
