using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UISettingsViewControllerOz : UIViewControllerOz
{

    public UIToggle music_onoff;
    public GameObject monSprite;
    public GameObject moffSprite;
    public UIToggle sound_onoff;
    public GameObject sonSprite;
    public GameObject soffSprite;
    public GameObject btnReset;
    public GameObject btnClose;
    //private bool firstTimeOnTutorial = true; // is this 1st time we turn on checkbox, always happens 1st time we go to settings so we need to ignore first callback
    //private bool firstTimeOnAutoFBpost = true;	
    //private bool firstTimeOnGraphicsQuality = true;	
	
	protected override void Awake()
	{
		base.Awake();
        RegisterEvent();
	}

    protected override void RegisterEvent()
    {
        EventDelegate.Add(music_onoff.onChange, OnMusicClick);
        EventDelegate.Add(sound_onoff.onChange, OnSoundClick);
        UIEventListener.Get(btnReset).onClick = OnReset;
        UIEventListener.Get(btnClose).onClick = OnClose;
    }

    void OnMusicClick()
    {
        AudioManager.SharedInstance.MusicVolume =  UIToggle.current.value? 1f : 0f;
       
        monSprite.SetActive(UIToggle.current.value);

        moffSprite.SetActive(!UIToggle.current.value);

    }

    void OnSoundClick()
    {
        AudioManager.SharedInstance.SoundVolume =  UIToggle.current.value? 1f : 0f;

        sonSprite.SetActive(UIToggle.current.value);
        
        soffSprite.SetActive(!UIToggle.current.value);
 
    }

    void ResetTutorial()
    {
        UIConfirmDialogOz.onPositiveResponse -= ResetTutorial; 
        PlayerPrefs.SetInt("ShowTutorial",1);
        PlayerPrefs.Save();
    }

    void OnReset(GameObject obj)
    {
        UIConfirmDialogOz.onPositiveResponse += ResetTutorial;
        UIManagerOz.SharedInstance.confirmDialog.ShowConfirmDialog("Msg_ResetTutorial", "Btn_No", "Btn_Yes");

      
//        AudioManager.SharedInstance.MusicVolume = 1f;
//        AudioManager.SharedInstance.SoundVolume = 1f;
//        music_onoff.Set(true);
//        sound_onoff.Set(true);
    }

	public override void appear()
    {	
        UIDynamically.instance.ZoomZeroToOneWithMovePostion(gameObject,new Vector3(244f,-556f,0f),0.5f);
		base.appear();	
//		UIManagerOz.SharedInstance.PaperVC.SetPageName("Ttl_Settings", "Ttl_Sub_General");
//		UIManagerOz.SharedInstance.PaperVC.SetCurrentPage(UIManagerOz.SharedInstance.settingsVC);
	}

    public override void disappear()
    {
        base.disappear();
    }

    public void OnClose(GameObject obj)
    {
        UIDynamically.instance.ZoomZeroToOneWithMovePostion(gameObject,new Vector3(244f,-556f,0f),0.5f,true);
        Invoke("disappear",0.4f);
    }



	public void OnLanguageClick()
	{
		Localization.SharedInstance.CycleLanguages();
	}
}

	
