using UnityEngine;
using System.Collections.Generic;

public class SkillCountDown : MonoBehaviour {

    [HideInInspector]
    public bool isCountDown = false;
    private float tempTime = -1f;
    private float duration = 1f;

    public UISprite skillIcon;//initial in inspector
    public UISprite skillCountDownTime;//initial in inspector
 
    public static List<BonusItem.BonusItemType> mTypeList = new List<BonusItem.BonusItemType>();
    public static Dictionary<BonusItem.BonusItemType,SkillCountDown> mSkillList = new Dictionary<BonusItem.BonusItemType, SkillCountDown>();

   
    private BonusItem.BonusItemType mtype;

   
	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {

        if(!isCountDown)
            return;
        
        if(tempTime>0.04f)
        {
            skillCountDownTime.fillAmount =tempTime * (0.9f/duration);
            tempTime -=Time.deltaTime;
        }
        else
        {  
            disappear();
        }

	}

    public void ReStart()
    {
        skillCountDownTime.fillAmount =0.9f;
        tempTime = duration =GameProfile.SharedInstance.GetDurationByType(mtype);
        gameObject.SetActive(true);
        isCountDown = true;
       
    }


    public void appear(BonusItem.BonusItemType type)
    {
//        UIDynamically.instance.TopToScreen(this.gameObject,-200f,0f,0.5f);
        UIDynamically.instance.LeftToScreen(this.gameObject,500f,286f,0.5f);
        this.mtype =type;
        mTypeList.Add(type);
        mSkillList.Add(type,this);

        string spriteName = "common_takeoff";
        switch(type)
        {
            case BonusItem.BonusItemType.Vacuum:
                spriteName ="common_magnet";
                break;
            case BonusItem.BonusItemType.Shield:
                spriteName ="common_shield";
                break;
            case BonusItem.BonusItemType.Boost:
            case BonusItem.BonusItemType.Fengdao:
                spriteName ="common_speedpotion";
                break;
            case BonusItem.BonusItemType.ScoreBonus: 
                spriteName ="common_doublescore";
                break;
            case BonusItem.BonusItemType.MegaCoin:
                spriteName ="common_doublecoin";
                break;

        }
        skillIcon.spriteName =spriteName;
        ReStart();
       
    }
  
    public void disappear()
    {
        isCountDown =false;
        mTypeList.Remove(mtype);
        mSkillList.Remove(mtype);
//        UIDynamically.instance.TopToScreen(this.gameObject,0f,-200f,0.5f);
        UIDynamically.instance.LeftToScreen(this.gameObject,500f,286f,0.5f,true);
        Invoke("Hide",0.5f);
    }

    void Hide()
    {
        gameObject.SetActive(false);
    }

    public static void EndPoofDown()
    {
        if(SkillCountDown.mTypeList.Contains(BonusItem.BonusItemType.Shield))
        {
            SkillCountDown.mSkillList[BonusItem.BonusItemType.Shield].disappear();
        }
    }

}
