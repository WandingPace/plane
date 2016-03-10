using UnityEngine;
using System.Collections;

public class RankCellData : MonoBehaviour
{
    public UILabel nameTxt, scoreTxt, rankTxt;
    public UISprite headIcon,ranknumIcon;


    private RankProtoData _data;
	// Use this for initialization
	void Start () {
	
	}

    void Refresh()
    {
        //        titleTxt.GetComponent<UILocalize>().SetKey(_data._title);
        //        descTxt.GetComponent<UILocalize>().SetKey(_data._descriptionEarned);
        nameTxt.text = _data._nameStr;
        scoreTxt.text = _data._nScore.ToString();
        rankTxt.text = gameObject.name;
        headIcon.spriteName = "player_head_" + _data._IconIndex;
       // costIcon.spriteName = playerInfo.GetMenuIconSpriteName();
        if (_data._nRank <= 3)
        {
            rankTxt.gameObject.SetActive(false);
            ranknumIcon.gameObject.SetActive(true);
            ranknumIcon.spriteName = "rank_NO" + _data._nRank;
        }

    }
    public void SetData(RankProtoData data)
    {
       data._nRank= int.Parse(gameObject.name);
       _data = data;
        if (_data != null)
        {
            Refresh();
        }

    }


}
