using UnityEngine;
using System.Collections;

public class CoinMeter : MonoBehaviour
{

	public GameObject activePowerButton;
	public UISprite spritePowerIcon;

	public UISlider progressBar;
    public GameObject ActivePowerFX;
  
	private bool activePower = false;

	
	
	void Start(){
        RegisterEvent();
        
	}
	
    void RegisterEvent()
    {
        UIEventListener.Get(activePowerButton).onClick = OnMainSkillClicked;
    }
    private bool isClicked = false;
    void OnMainSkillClicked(GameObject obj)
    {  
        if(isClicked)
            return;
        isClicked = true;
        GameController.SharedInstance.UsePower();

//        CharacterStats roleData=GameProfile.SharedInstance.GetActiveCharacter();
//        int id=roleData.characterId;
//        float duration = (id==1||id==2||id==4)?0.5f:roleData.mainSkillDose;
//        UIManagerOz.SharedInstance.inGameVC.coinMeter.AnimateCoinMeter(duration);
//        UIManagerOz.SharedInstance.chaSelVC.roleUpgrade.UseRoleMainSkill(roleData);

        if(GameController.SharedInstance.IsTutorialMode){
            UIManagerOz.SharedInstance.inGameVC.tutorialMeter.GetComponent<UIPanelAlpha>().alpha = 0f;
            GameController.SharedInstance.TimeSinceTutorialEnded = 0.0f;
            GameController.SharedInstance.canShowNextTutorialStep = true;
        }
        isClicked = false;
    }


	public void appear()
	{
        //ActivePowerFX.gameObject.SetActive(false);
	    // set powerup icon
//        int roleId= GameProfile.SharedInstance.GetActiveCharacter().characterId;
        RefreshSuperSkillIcon();
        spritePowerIcon.alpha =0.5f;

    }
    
    public void RefreshSuperSkillIcon()
    {
        CharacterStats activeCharacter = GameProfile.SharedInstance.Characters[GamePlayer.SharedInstance.ActiveCharacterId];

        spritePowerIcon.spriteName = UIManagerOz.SharedInstance.chaSelVC.roleUpgrade.GetMainSkillIconByRoleId(activeCharacter.characterId);
    }
	
	public void SetPowerProgress(float progress) 
	{
		iTween.Stop(progressBar.gameObject);
         progressBar.value = progress;
        spritePowerIcon.alpha = 0.5f;
	}	

	public void FadePowerGlow() 						// remove glow when powerup is used
	{
		activePower = false;
		RemoveBlink(gameObject);
        ActivePowerFX.gameObject.SetActive(false);
	}
	
	public void ActivePowerIcon()						// make glow when power meter is full
    {	
		activePower = true;
        activePowerButton.collider.enabled = true;
		Blink(gameObject, 0.25f);
        ActivePowerFX.gameObject.SetActive(true);
		AudioManager.SharedInstance.PlayFX(AudioManager.Effects.oz_ScoreMultiplier_01);
	}
	
	public void Pause()
	{
		if (activePower) { RemoveBlink(gameObject); }
	}

	public void UnPause()
	{
		if (activePower) { ActivePowerIcon(); }	
	}
	
	
	private void Blink(GameObject root, float duration)
	{

		UIWidget[] widgets = root.GetComponentsInChildren<UIWidget>();
		TweenColor tc = null;
		
		for (int i = 0, imax = widgets.Length; i < imax; ++i)
		{
			UIWidget w = widgets[i];
			if (w == null) { continue; }
			w.color = new Color(w.color.r, w.color.g, w.color.b, 1f);//Color.clear;
			tc = root.GetComponentInChildren<TweenColor>();
			tc.method = UITweener.Method.EaseInOut;
			tc.style = UITweener.Style.PingPong;
			tc.Play (true);

		}
		
		
	}
	
	private void RemoveBlink(GameObject root) 
	{
		UITweener[] tweens = root.GetComponentsInChildren<UITweener>();
		for (int i=0, imax = tweens.Length; i < imax; ++i)
		{
			UITweener tween = tweens[i];
			if (tween == null) { continue; }
			tween.enabled = false;
		}
		spritePowerIcon.color = new Color(spritePowerIcon.color.r, spritePowerIcon.color.g, spritePowerIcon.color.b, 0.5f);
		
	}	
	
    //能量槽递减
	public void AnimateCoinMeter(float time = 0.5f, float endVal = 0f){

		iTween.Stop(progressBar.gameObject);
        activePowerButton.collider.enabled = false;
	
		float startVal = progressBar.value;
 		iTween.ValueTo(progressBar.gameObject, iTween.Hash(
		"from", startVal,
		"to", endVal,
		"onupdatetarget", gameObject,
		"onupdate", "EmptyCoinMeterUpdate",
		"oncompletetarget", gameObject,
		"oncomplete", "EmptyCoinMeterComplete",
		"time", time,
		"easetype", iTween.EaseType.linear));
	}
	public void EmptyCoinMeterUpdate(float val){
         progressBar.value = val;
	}
	public void EmptyCoinMeterComplete(){

        spritePowerIcon.alpha =0.5f;
        GameController.SharedInstance.LosePower();

	}
	
	
}

