using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIStatViewControllerOz : UIViewControllerOz
{
    public GameObject btnClose;

    public UISprite icon,progressBar;

    public UILabel txtDesc,txtProgress;

    public UIObjectivesList list;

    protected override void Start() 
    { 
        base.Start();
    }
    
    protected override void RegisterEvent ()
    {
        UIEventListener.Get(btnClose).onClick = OnCloseBtnClicked;
    }

    void OnCloseBtnClicked(GameObject obj)
    {
        UIDynamically.instance.ZoomZeroToOneWithMovePostion(gameObject,new Vector3(0f,-556f,0f),0.5f,true);
        Invoke("disappear",0.5f);
    }

    //void disappear()
    //{
    //    gameObject.SetActive(false);
    //}

	public override void appear()
	{	
        UIDynamically.instance.ZoomZeroToOneWithMovePostion(gameObject,new Vector3(0f,-556f,0f),0.5f);

        base.appear();

		UIManagerOz.SharedInstance.PaperVC.SetCurrentPage(UIManagerOz.SharedInstance.statsVC);	
		
        list.Refresh();
	}

    void Refresh()
    {
//        progressBar.fillAmount = 20f;
    }

}
