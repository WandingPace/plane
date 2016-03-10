using UnityEngine;
using System.Collections;

public class UISignInCellData : MonoBehaviour {

    public GameObject iconRecived;
    public GameObject iconRewardItem;
    public UILabel txtRewardTxt;
    public GameObject iconeffect;
    void Start()
    {
      
    
    }
    public void Reveive()
    {

        //UIDynamically.instance.ZoomOutToOne(iconRecived, new Vector3(1.6f, 1.6f, 2f), 2f);//++

        iconRecived.transform.localScale = new Vector3(3f, 3f, 1f);
        iconRecived.SetActive(true);
        iTween.ScaleTo(iconRecived, iTween.Hash(
            "islocal", true,
            "scale", Vector3.one,
            "time", 1f,
            "easyType", iTween.EaseType.easeInQuart
            ));
    
    }
}
