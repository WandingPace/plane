using UnityEngine;
using System.Collections;
using System.Collections.Generic;



public enum AbilityUsed
{
	None = 0,
	Mod1 = 1,
	Mod2 = 2,
	Mod3 = 4,
	Mod4 = 8,
	Mod5 = 16,
	Cons1 = 32,
	Cons2 = 64,
	Cons3 = 128,
	Cons4 = 256,
	Cons5 = 512,
	AllInGame = 1023,
	Pow1 = 1024,
	Pow2 = 2048,
	Pow3 = 4096,
	Pow4 = 8192,
	Pow5 = 16384,
	Pow6 = 32768,
	All = 65535,
//	All = 131071,
}




public enum BonusButtonType
{
	Consumable,
	Modifier,
	Pickup,
	Power,
}


public class BonusButtons : MonoBehaviour
{
	protected static Notify notify;
	
	//Modifier ID's
	public int DoubleCoinsID = 0;
	public int DiscountID = 0;
	public int PowerDurationID = 0;
	public int CoinMeterSpeedID = 0;
	public int LuckID = 0;
	
	//Modifier Button Objects
	public GameObject DoubleCoinsButton;
	public GameObject DiscountButton;
	public GameObject PowerDurationButton;
	public GameObject CoinMeterSpeedButton;
	public GameObject LuckButton;
	
	//消耗道具ID
    public int HeadBoostID = 1;
	public int DeadBoostID = 2;
    public int BoostID = 3;
    public int ShieldID = 4;
    public int VacuumID = 5;
	
	//消耗道具按钮
    public GameObject HeadBoostButton;
    public GameObject DeadBoostButton;
    public GameObject BoostButton;
    public GameObject ShieldButton;
    public GameObject VacuumButton;

	public GameObject GemCountLabel;
	public Transform gemTransform;
    public GameObject DoubleCoin;
	//Pickup Image Objects
	public GameObject BoostImage;
	public GameObject PoofImage;
	public GameObject MagnetImage;
	public GameObject MegaCoinImage;
	public GameObject TornadoTokenImage;
	public GameObject GemImage;
	public GameObject ScoreBonusImage;
	
	
	public Transform displayLocator;
	
	public ParticleSystemRenderer glow1;
	public ParticleSystemRenderer glow2;
	
	//public UILocalize DescriptionLabelLocalization;
	public UILabel DescriptionLabel;

    public GameObject Tip;

    public UISprite skillIcon;

    public UILabel skillDesc;
	
    public List<SkillCountDown> SkillCountDowns;//initial in inspector
  
    public GameObject bottom;
	
    public GameObject speedLine;


	private List<GameObject> purchasedModifiers = new List<GameObject>();
	private List<GameObject> purchasedConsumables = new List<GameObject>();
	
	
	
	private const float PIXELS_TO_OFFSCREEN = 150f;
	private const float ANIMATION_BOUNCINESS = 1.5f;
	private const float ANIMATION_SPEED = 3f;
	
	[HideInInspector]
	public GameObject tutorialButtonAbility;
	[HideInInspector]
	public GameObject tutorialButtonUtility;
	
	private int UsedAbilityFlags = 0;
	
	public static BonusButtons main
	{
		get; private set;
	}
	
	void Awake()
	{
		notify = new Notify(this.GetType().Name);	
		main = this;

        UIManagerOz.onPauseClicked += OnPause;

        RegisterEvent();
	}
	
	
	void OnPause()
	{
		//HideConsumableAndModifierButtons();
	}

    private void Start()
    {
        HeadBoostButton.SetActive(false);
        DeadBoostButton.SetActive(false);

        if (GameProfile.SharedInstance == null)
        {
            ShowAllButtons();
        }

        DeactivatePickupButtons();
        //DescriptionLabelLocalization.GetComponent<UILabel>().text = "";
        DescriptionLabel.text = "";
        if (gemTransform != null)
            gemTransform.renderer.enabled = false;
    }

    void RegisterEvent()
    {
        UIEventListener.Get(HeadBoostButton).onClick = UseHeadBoost;
        UIEventListener.Get(DeadBoostButton).onClick = UseDeadBoost;
        UIEventListener.Get(BoostButton).onClick = UseBoost;
        UIEventListener.Get(ShieldButton).onClick = UseShield;
        UIEventListener.Get(VacuumButton).onClick = UseVacuum;
    }
	
	void OnEnable()
	{
		//DescriptionLabelLocalization.GetComponent<UILabel>().text = "";
		DescriptionLabel.text = "";
		DeactivatePickupButtons();
		
	//	TweenAlpha.Begin(DescriptionLabel.gameObject,0f,0f);
		DescriptionLabel.transform.localPosition = new Vector3(0,400,-5);	//The -5 is so that it shows up in front of any other In-Game UI (overlap problems in Russian)
	}
	
	public void EnableAllButtons(bool on)
	{
        //notify.Debug ("EnableAllButtons " + on);
        //DoubleCoinsButton.collider.enabled = on;
        //DiscountButton.collider.enabled = on;
        //PowerDurationButton.collider.enabled = on;
        //CoinMeterSpeedButton.collider.enabled = on;
        //LuckButton.collider.enabled = on;
		
        //HeadBoostButton .collider.enabled = on;
        //DeadBoostButton.collider.enabled = on;
        //BoostButton.collider.enabled = on;
        //ShieldButton.collider.enabled = on;
        //VacuumButton .collider.enabled = on;
	}
	
	public void EnableAllButtonsAbility(bool on)
    {
        //notify.Debug ("EnableAllButtonsAbility " + on);
        //DoubleCoinsButton.collider.enabled = on;
        //DiscountButton.collider.enabled = on;
        //PowerDurationButton.collider.enabled = on;
        //CoinMeterSpeedButton.collider.enabled = on;
        //LuckButton.collider.enabled = on;
	}
	public void EnableAllButtonsUtility(bool on)
    {
        //notify.Debug ("EnableAllButtonsUtility " + on);

        //HeadBoostButton.collider.enabled = on;
        //DeadBoostButton.collider.enabled = on;
        //BoostButton.collider.enabled = on;
        //ShieldButton.collider.enabled = on;
        //VacuumButton.collider.enabled = on;
	}

	
	public void DeactivatePickupButtons()
	{
		BoostImage.SetActive(false);
		PoofImage.SetActive(false);
		MagnetImage.SetActive(false);
		MegaCoinImage.SetActive(false);
		TornadoTokenImage.SetActive(false);
		GemImage.SetActive(false);
		ScoreBonusImage.SetActive(false);
	}
	
	public void ShowAllButtons()
	{
        HeadBoostButton.SetActive(false);
        DeadBoostButton.SetActive(false);

		EmergePurchasedModifierButtons(false);
		EmergePurchasedConsumableButtons(false);
		
		currentSprite = null;
			
		//if(purchasedModifiers.Count>0)
		if(GameProfile.SharedInstance.Player.artifactsPurchased.Count > 0)
		{
			ShowButton(GemCountLabel,true,false,GameProfile.SharedInstance.Player.GetGemCount());
			UpdateGemCount();
		}
		else
		{
			GemCountLabel.SetActive(false);
		}
		
		EmergePurchasedModifierButtons(true);
		EmergePurchasedConsumableButtons(true);
		DeactivatePickupButtons();
		
        //StopCoroutine("BlinkIcons");
		
        //StartCoroutine("BlinkIcons");
		
        ////Hide buttons in 7 seconds
        //HideConsumableAndModifierButtons();
	}


	public void HideConsumableAndModifierButtons()
	{
		CancelHideConsumableAndModifierButtons();	
		
		StartCoroutine("HideConsumableAndModifierButtons_internal");
	}
	
	private IEnumerator HideConsumableAndModifierButtons_internal()
	{
		yield return new WaitForSeconds(7f);
		
		HideConsumableAndModifierButtonsNow();
	}
	
	public void HideConsumableAndModifierButtonsNow()
	{
		if(purchasedModifiers.Contains(GemCountLabel))
			ShowButton(GemCountLabel,false,false,GameProfile.SharedInstance.Player.GetGemCount());
		
		EmergePurchasedModifierButtons(false);
		EmergePurchasedConsumableButtons(false);
		
		CancelHideConsumableAndModifierButtons();
	}
	
	
	public void CancelHideConsumableAndModifierButtons(){
		StopCoroutine("HideConsumableAndModifierButtons_internal");
	}
	
	//
	public bool CanShowModifiers()
	{
		if(GameProfile.SharedInstance!=null)
		{
			if(GameProfile.SharedInstance.Player.IsArtifactPurchased(DoubleCoinsID))
				return true;
			//This one doesnt count; it's an addition to the boost. If someone only buys this, it's just going to have to be an exception.
			//if(GameProfile.SharedInstance.Player.IsArtifactPurchased(DiscountID))
			//	return true;
			if(GameProfile.SharedInstance.Player.IsArtifactPurchased(PowerDurationID))
				return true;
			if(GameProfile.SharedInstance.Player.IsArtifactPurchased(CoinMeterSpeedID))
				return true;
			if(GameProfile.SharedInstance.Player.IsArtifactPurchased(LuckID))
				return true;
		}
		return false;
	}
	
	//
	public bool CanShowConsumables()
	{
		if(GameProfile.SharedInstance!=null)
		{
			if(GameProfile.SharedInstance.Player.GetConsumableCount(HeadBoostID) > 0)
				return true;
			if(GameProfile.SharedInstance.Player.GetConsumableCount(DeadBoostID) > 0)
				return true;
			if(GameProfile.SharedInstance.Player.GetConsumableCount(BoostID) > 0)
				return true;
			if(GameProfile.SharedInstance.Player.GetConsumableCount(VacuumID) > 0)
				return true;
			if(GameProfile.SharedInstance.Player.GetConsumableCount(ShieldID) > 0)
				return true;
		}
		return false;
	}
	
    //显示隐藏已购买属性剂//
	private void EmergePurchasedModifierButtons(bool show)
	{
		if(!GameController.SharedInstance.abilityTutorialOn)
			tutorialButtonAbility = null;
		
		if(show)
			StopCoroutine("BlinkIcons");
		if(GameProfile.SharedInstance!=null)
		{
			if(GameProfile.SharedInstance.Player.IsArtifactPurchased(DoubleCoinsID))
				ShowButton(DoubleCoinsButton,show,false);
			else
				DoubleCoinsButton.SetActive(false);
			
			if(GameProfile.SharedInstance.Player.IsArtifactPurchased(DiscountID) && !show)
				ShowButton(DiscountButton,show,false);
			else
				DiscountButton.SetActive(false);
			
			if(GameProfile.SharedInstance.Player.IsArtifactPurchased(PowerDurationID))
				ShowButton(PowerDurationButton,show,false);
			else
				PowerDurationButton.SetActive(false);
			
			if(GameProfile.SharedInstance.Player.IsArtifactPurchased(CoinMeterSpeedID))
				ShowButton(CoinMeterSpeedButton,show,false);
			else
				CoinMeterSpeedButton.SetActive(false);
			
			if(GameProfile.SharedInstance.Player.IsArtifactPurchased(LuckID))
				ShowButton(LuckButton,show,false);
			else
				LuckButton.SetActive(false);

           
		}
		if(!show)
			purchasedModifiers.Clear();
	}
	
    //显示隐藏已购买消耗品//
	public void EmergePurchasedConsumableButtons(bool show)
	{
		if(!GameController.SharedInstance.utilityTutorialOn)
			tutorialButtonUtility = null;
		
		if(show)
			StopCoroutine("BlinkIcons");
		if(GameProfile.SharedInstance!=null)
		{
			//NOTE: These calls will automatically short-circuit if we pass in '0' for the consumable count.
            //ShowButton(HeadBoostButton,show,true,GameProfile.SharedInstance.Player.GetConsumableCount(HeadBoostID));
            //ShowButton(DeadBoostButton,show,true,GameProfile.SharedInstance.Player.GetConsumableCount(DeadBoostID));
            if(GameController.SharedInstance.EndlessMode)
            {   //挑战模式显示
                ShowButton(BoostButton,show,true,GameProfile.SharedInstance.Player.GetConsumableCount(BoostID));
    			ShowButton(ShieldButton,show,true,GameProfile.SharedInstance.Player.GetConsumableCount(ShieldID));
    			ShowButton(VacuumButton,show,true,GameProfile.SharedInstance.Player.GetConsumableCount(VacuumID));
                DoubleCoin.transform.SetLocalPositionX(-800f);
            }
            else
            {
                int level = GameProfile.SharedInstance.Player.activeLevel;
                ObjectiveProtoData mdata = ObjectivesManager.LevelObjectives[level];
                int LevelItemId = mdata._conditionList[0]._itemForLevel;
                //关卡模式是否显示双倍金币
                if (LevelItemId > 0 && LevelItemId < ConsumableStore.consumablesList.Count && GamePlayer.SharedInstance.LevelItem!=null)
                {
                  Transform bonussprite =  DoubleCoin.transform.FindChild("bonusSprite");
                  if (bonussprite != null)
                  {
                      bonussprite.gameObject.GetComponent<UISprite>().spriteName = ConsumableStore.consumablesList[LevelItemId - 1].IconName;
                  }
                  UIDynamically.instance.LeftToScreen(DoubleCoin, -800f, -583f, 0.5f);
                }
                else
                {
                    DoubleCoin.transform.SetLocalPositionX(-800f);
                }
                //if(GamePlayer.SharedInstance.GetCoinMultiplier() == 2)
                //    UIDynamically.instance.LeftToScreen(DoubleCoin,150f,13f,0.5f);
                //else
                //    DoubleCoin.transform.SetLocalPositionX(150f);
            }
         }
		if(!show)
			purchasedConsumables.Clear();
	}
	
	float Bump(float total, float t, float bumpiness)
	{
		return t*total + t*total*bumpiness*(1f-t);
	}
	
	//显示隐藏消耗品图标、个数//
	public void EnableAllCountLabels(bool enable)
	{
        //EnableCount(HeadBoostButton, enable);
        //EnableCount(DeadBoostButton, enable);
        EnableCount(BoostButton, enable);
        EnableCount(ShieldButton, enable);
        EnableCount(VacuumButton, enable);
	}
	
	public void EnableCount(GameObject go, bool enable)
	{
		BonusButton bb = go.GetComponent<BonusButton>();
		if(bb!=null)
		{
			UISprite spr = bb.amtSprite;
			UILabel lbl = bb.amtLabel;
			if(spr!=null)	spr.enabled = enable;
			if(lbl!=null)	lbl.enabled = enable;
		}
	}
	
    //显示、隐藏按钮//
	public void ShowButton(GameObject go, bool show, bool isConsumable, int count = 1)
	{
		if(!show && (go == tutorialButtonAbility || go == tutorialButtonUtility))
			return; // don't hide the button if it's a tutorial button, since we need to display it
		
		EnableCount(go,true);
		
		if(show && !go.activeSelf)
		{
			go.SetActive(true);
		}
		if(go.activeSelf)
		{
			StartCoroutine(ShowButton_internal(go,show,isConsumable,count));
		}
		
		// reset back z since we move it on tutorial to be infront of fade
		go.transform.localPosition = new Vector3(go.transform.localPosition.x, go.transform.localPosition.y, 0f);
		
		if(show && isConsumable && count > 0 && !GameController.SharedInstance.utilityTutorialPlayed){
			tutorialButtonUtility = go; // this is the last utility button activated
            notify.Debug("tutorialButtonUtility:"+tutorialButtonUtility.name);
		}
		if(show && !isConsumable && go != GemCountLabel && !GameController.SharedInstance.abilityTutorialPlayed){
			tutorialButtonAbility = go; // this is the last ability button activated
            notify.Debug("tutorialButtonAbility:"+tutorialButtonAbility.name);
            
		}
	}
	
    //显示、隐藏按钮//
	private IEnumerator ShowButton_internal(GameObject go, bool show, bool isConsumable, int count = 1)
	{
		//没有可用消耗品
		if(show && count<=0 && isConsumable)
		{
			go.SetActive(false);
			yield break;
		}

		UIPanelAlpha panel = GetComponent<UIPanelAlpha>();
		panel.alpha = 1f;
		
		List<GameObject> buttonList = isConsumable ? purchasedConsumables : purchasedModifiers;
        //已显示
		if(buttonList.Contains(go)==show && go!=GemCountLabel)
		{
			yield break;
		}
		
		//显示拥有数量
		if(show)
        {   
			UILabel label = go.GetComponentInChildren<UILabel>();
			if(label!=null)	label.text = count.ToString();

            buttonList.Add(go);
        }
        else       
            buttonList.Remove(go);
		
		//按个数设置y坐标的位置显示
		float yPos = show ? -748f + buttonList.Count*123f : go.transform.localPosition.y;
		if(!isConsumable && show)	yPos -= 123;
		
		//显示隐藏按钮
		float curT = show ? 0f : 1f;
		float target = show ? 1f : 0f;
		float offscreenOffset = isConsumable ? PIXELS_TO_OFFSCREEN : -PIXELS_TO_OFFSCREEN;
		Vector3 tempVec3 = Vector3.zero;
		while(curT!=target && buttonList.Contains(go)==show)
        {  
			curT = Mathf.MoveTowards(curT,target,Realtime.deltaTime*ANIMATION_SPEED);
			tempVec3 = go.transform.localPosition;
			tempVec3.x = Bump(-offscreenOffset,curT,ANIMATION_BOUNCINESS)+offscreenOffset;
			tempVec3.y = yPos;
			go.transform.localPosition = tempVec3;
			yield return null;
		}
		if(!show)
		{
			yield return new WaitForSeconds(6f);	//Wait, so that its not deactivated while its being shown
			go.SetActive(false);
		}
		yield break;
	}
	
	private int cur_glow_id = 0;
	IEnumerator ShowGlow(bool show, Color baseColor, float delay = 0f)
	{
#if !UNITY_EDITOR
		if(GameController.SharedInstance.GetDeviceTier() < 3)
		{
			yield break;
		}
#endif
		
//		Debug.Log("Glow! "+show+" "+Realtime.time);
		
		int myId = ++cur_glow_id;
		
		yield return StartCoroutine(Realtime.WaitForSeconds(delay));
		
		if(myId!=cur_glow_id)	yield break;
		
		if(show)
		{
			glow1.particleSystem.Play();
			glow2.particleSystem.Play();
		}
		else
		{
			glow1.particleSystem.Stop();
			glow2.particleSystem.Stop();
		}
		
		float target = show ? 1f : 0f;
		float start = show ? 0f : 1f;
		float speed = show ? 2f : 4f;
		
		Color col = baseColor;
		
		float t = start;
		while(t!=target && myId==cur_glow_id)
		{
			t = Mathf.MoveTowards(t,target,Realtime.deltaTime*speed);
			
			col.a = Mathf.Sqrt(t)/4f;
			
			glow1.material.SetColor("_TintColor",col);
			glow2.material.SetColor("_TintColor",col);
			
			yield return null;
		}
	}
	
	public IEnumerator ShiftButton(GameObject go)
	{
		float t = 0f;
		
		while (t < 123f)
		{
			float moveDist = ((123f-t)+20f)*Realtime.deltaTime*5f;

			go.transform.localPosition -= Vector3.up*moveDist;
			t+=moveDist;
			
			yield return null;
		}
	}
	
	
	static int move_id = 0;
	private IEnumerator ShowText(bool on)
	{
		int id = ++move_id;
		Vector3 target = on? new Vector3(0,100,-5) : new Vector3(0,300,-5);	//-5 is for overlap issues, so it looks right in Russian
		while(id==move_id && DescriptionLabel.transform.localPosition!=target)
		{
			DescriptionLabel.transform.localPosition = Vector3.Lerp(DescriptionLabel.transform.localPosition,target,Realtime.deltaTime*10f);
			yield return null;
		}
	}
	
	
	public void DisplayButton(GameObject go, BonusButtonType type, string translateKey = "")
	{
		//DescriptionLabel.alpha = 1f;
		if(!go.activeSelf)
		{
			go.SetActive(true);
		}
		// set button back on z , since we may have moved it forward in tutorial
		go.transform.localPosition = new Vector3(go.transform.localPosition.x, go.transform.localPosition.y, 0f);
		//go.GetComponent<UIPanel>().widgetsAreStatic = false;
		
		EnableCount(go,false);
		
		StartCoroutine(DisplayButton_internal(go,type,translateKey));
	}
   
	void LateUpdate()
	{
       
		if(currentSprite!=null)
			currentSprite.alpha = 1f;
	}
	
	
	public static bool isDisplaying = false;
	public static int cur_show_id = 0;
	private UISprite currentSprite;
	public IEnumerator DisplayButton_internal(GameObject go, BonusButtonType type, string translateKey = "")
	{
		int my_id = ++cur_show_id;
		
		currentSprite = go.GetComponentInChildren<UISprite>();
		
		isDisplaying = true;
		PopupNotification.PopupList[PopupNotificationType.Generic].FadeOut();
		PopupNotification.PopupList[PopupNotificationType.Objective].FadeOut();
		FastTravelButton.FadeOut();
		UIManagerOz.SharedInstance.inGameVC.FadeOutEnvProgress();
		
//		TweenAlpha.Begin(DescriptionLabel.gameObject, 0.3f, 1f);
//		StartCoroutine(ShowText(true));
        //int id = move_id;
		
		Vector3 origPos = go.transform.localPosition;
		
		List<GameObject> buttonList = null;
		if(type==BonusButtonType.Consumable)
			buttonList = purchasedConsumables;
		else if(type==BonusButtonType.Modifier)
			buttonList = purchasedModifiers;
		
		if(buttonList!=null)
		{
			for(int i=buttonList.Count-1;i>=0 && go!=buttonList[i];i--)
			{
				StartCoroutine(ShiftButton(buttonList[i]));//调整剩余按钮位置
			}
		}
		
		if(buttonList!=null)	
            buttonList.Remove(go);

	    TweenScale ts = TweenScale.Begin(go, 1f, Vector3.one);
	    ts.to = Vector3.one*4.5f;
        TweenPosition.Begin(go, 1f, Vector3.up*1000f);
        TweenAlpha.Begin(go, 1f, 0f);

	    yield return new WaitForSeconds(1f);

        go.transform.localPosition = origPos;
        TweenAlpha.Begin(go, 0f, 1f);
        go.SetActive(false);

    //    int glowId = cur_glow_id + 1;
    //    StartCoroutine(ShowGlow(true, type == BonusButtonType.Modifier ? Color.green : Color.white, 0.25f));


    //    //TODO: Clean this up.
    //    float t = 0f;
    //    Vector3 defScale = go.transform.localScale;
    //    float scalespeed = 3f;
    //    float speed = type == BonusButtonType.Pickup ? 7f : 3.75f;
    //    //float parabolicMax = -0.25f;
    //    float maxScale = 4.5f;
    //    float scaleFactor = 0f;
    //    Transform tr = go.transform;

    //    //		string int_key = translateKey;
    //    //		DescriptionLabelLocalization.GetComponent<UILabel>().enabled = true;
    //    //		DescriptionLabelLocalization.key = translateKey;
    //    //		DescriptionLabelLocalization.Localize();
    //    DescriptionLabel.text = Localization.SharedInstance.Get(translateKey);

    //    if (type == BonusButtonType.Modifier)
    //    {
    //        gemTransform.position = GemCountLabel.transform.position;
    //    }

    //    Vector3 offset = new Vector3(-.1f, -.1f);

    //    while (t < maxScale && !GameController.SharedInstance.IsPaused && !GamePlayer.SharedInstance.IsDead && !GamePlayer.SharedInstance.Dying && (cur_show_id == my_id || tr.position != displayLocator.position))
    //    {
    //        t = Mathf.MoveTowards(t, maxScale, scalespeed * Realtime.deltaTime);
    //        if (t < 1)
    //            scaleFactor = 1 + (t * t - t);
    //        else
    //            scaleFactor = 1 + Mathf.Sqrt(t) / 4f - 0.25f;//+ parabolicMax*4f*(t-t*t);
    //        tr.localScale = defScale * scaleFactor;
    //        Vector3 targ = displayLocator.position;
    //        tr.position = Vector3.MoveTowards(tr.position, targ, Realtime.deltaTime * speed);
    //        if (type == BonusButtonType.Modifier && t > 1f)
    //        {
    //            gemTransform.renderer.enabled = true;
    //            gemTransform.position = Vector3.MoveTowards(gemTransform.position, displayLocator.position + offset, Realtime.deltaTime * speed);
    //            gemTransform.RotateAround(Vector3.up, Realtime.deltaTime * 5f);
    //        }

    //        if (t > 1.5f && glowId == cur_glow_id)
    //            StartCoroutine(ShowGlow(false, type == BonusButtonType.Modifier ? Color.green : Color.white));


    //        yield return null;
    //    }
    //    //If we didn't already, turn off the glow
    //    if (t <= 1.5f && glowId == cur_glow_id)
    //        StartCoroutine(ShowGlow(false, type == BonusButtonType.Modifier ? Color.green : Color.white));

    //    //		if(glowId==cur_glow_id)
    //    //			StartCoroutine(ShowGlow(false,type==BonusButtonType.Modifier ? Color.green : Color.white));

    //    t = 1f;
    //    speed = 0f;
    //    Vector3 defPos = tr.position;
    //    while (t > 0f)
    //    {
    //        float amt = t + (t - t * t) * 2f;
    //        tr.position = defPos + Vector3.up * (1f - amt * 1f);
    //        if (type == BonusButtonType.Modifier)
    //        {
    //            gemTransform.renderer.enabled = true;
    //            gemTransform.position = tr.position + offset;
    //            gemTransform.RotateAround(Vector3.up, Realtime.deltaTime * 5f);
    //        }

    //        t -= Realtime.deltaTime * 5f;

    //        yield return null;
    //    }

    //    gemTransform.renderer.enabled = false;

    //    /* comment out for now
    //    if(int_key == DescriptionLabelLocalization.key)
    //    {
    //        DescriptionLabelLocalization.GetComponent<UILabel>().enabled = false;
    //    }
    //    */
    //    if (id == move_id)
    //        StartCoroutine(ShowText(false));

    //    tr.localScale = defScale;

    //    if (type == BonusButtonType.Consumable) origPos.x += PIXELS_TO_OFFSCREEN;
    //    else if (type == BonusButtonType.Modifier) origPos.x -= PIXELS_TO_OFFSCREEN;
    //    go.transform.localPosition = origPos;
		
    ////	TweenAlpha.Begin(DescriptionLabel.gameObject, 0.3f, 0f);
    //    //DescriptionLabel.alpha = 0f;
    //    go.SetActive(false);
		
		PopupNotification.PopupList[PopupNotificationType.Generic].FadeIn();
		PopupNotification.PopupList[PopupNotificationType.Objective].FadeIn();
		FastTravelButton.FadeIn();
		UIManagerOz.SharedInstance.inGameVC.FadeInEnvProgress();
		
		isDisplaying = false;
		
		if(my_id==cur_show_id)
			DeactivatePickupButtons();
		
	//	Debug.Log("fin");
		
		yield break;
	}

    void DisplayButtonOver()
    {
        
    }
	
	void UpdateGemCount()
	{
		UILabel label = GemCountLabel.GetComponentInChildren<UILabel>();
		int gemCount = GameProfile.SharedInstance.Player.GetGemCount();
		if(label!=null)	label.text = gemCount.ToString();
		
		if(gemCount<=0)
		{
			//EmergePurchasedModifierButtons(false);	// don't hide anymore if no gems, go to mini store instead if clicked
			StartCoroutine(BlinkGemCountLabel());
		}
	}
	
	IEnumerator BlinkGemCountLabel()
	{
		float t=0f;
		Color col = Color.gray;
		
		UISprite gemsprite = GemCountLabel.GetComponentInChildren<UISprite>();
		
		//Blink for ten seconds (so that it blinks until it's hidden)
		while(t<10f)
		{
			col.a = Mathf.PingPong(t,0.5f)+0.5f;
			gemsprite.color = col;
			t+=Realtime.deltaTime;
			yield return null;
		}
	}
	
	
	IEnumerator BlinkIcons()
	{
		float t=0f;
		float alpha = 1f;
		
		UIPanelAlpha panel = GetComponent<UIPanelAlpha>();

		panel.alpha = 1f;
	
		yield return new WaitForSeconds(5f);
		
		//Blink for ten seconds (so that it blinks until it's hidden)
		while(t<2f)
		{
			alpha = Mathf.PingPong(t*5f + 0.85f,0.85f)+0.15f;
			panel.alpha = alpha;
			t+=Realtime.deltaTime;
			yield return null;
		}
	}
	
	
	public void MakeButtonsStatic(bool doStatic){
		StartCoroutine(MakeButtonsStaticLoop(doStatic));
	}
	
	IEnumerator MakeButtonsStaticLoop(bool doStatic){
		yield return new WaitForSeconds( 0.1f);
		DoubleCoinsButton.GetComponent<UIPanel>().widgetsAreStatic = doStatic;
		yield return new WaitForSeconds( 0.1f);
		DiscountButton.GetComponent<UIPanel>().widgetsAreStatic = doStatic;
		yield return new WaitForSeconds( 0.1f);
		PowerDurationButton.GetComponent<UIPanel>().widgetsAreStatic = doStatic;
		yield return new WaitForSeconds( 0.1f);
		CoinMeterSpeedButton.GetComponent<UIPanel>().widgetsAreStatic = doStatic;
		yield return new WaitForSeconds( 0.1f);
		LuckButton.GetComponent<UIPanel>().widgetsAreStatic = doStatic;
		
		
		yield return new WaitForSeconds( 0.1f);
		HeadBoostButton.GetComponent<UIPanel>().widgetsAreStatic = doStatic;
		yield return new WaitForSeconds( 0.1f);
		DeadBoostButton.GetComponent<UIPanel>().widgetsAreStatic = doStatic;
		yield return new WaitForSeconds( 0.1f);
		BoostButton.GetComponent<UIPanel>().widgetsAreStatic = doStatic;
		yield return new WaitForSeconds( 0.1f);
		ShieldButton.GetComponent<UIPanel>().widgetsAreStatic = doStatic;
		yield return new WaitForSeconds( 0.1f);
		VacuumButton.GetComponent<UIPanel>().widgetsAreStatic = doStatic;
      
		
		yield return new WaitForSeconds( 0.1f);
		BoostImage.GetComponent<UIPanel>().widgetsAreStatic = doStatic;
		yield return new WaitForSeconds( 0.1f);
		PoofImage.GetComponent<UIPanel>().widgetsAreStatic = doStatic;
		yield return new WaitForSeconds( 0.1f);
		MagnetImage.GetComponent<UIPanel>().widgetsAreStatic = doStatic;
		yield return new WaitForSeconds( 0.1f);
		MegaCoinImage.GetComponent<UIPanel>().widgetsAreStatic = doStatic;
		yield return new WaitForSeconds( 0.1f);
		TornadoTokenImage.GetComponent<UIPanel>().widgetsAreStatic = doStatic;
		yield return new WaitForSeconds( 0.1f);
		GemImage.GetComponent<UIPanel>().widgetsAreStatic = doStatic;
		yield return new WaitForSeconds( 0.1f);
		ScoreBonusImage.GetComponent<UIPanel>().widgetsAreStatic = doStatic;
		
		
		doStatic = !doStatic;
		
		yield return new WaitForSeconds( 0.1f);
		DoubleCoinsButton.GetComponent<UIPanel>().widgetsAreStatic = doStatic;
		yield return new WaitForSeconds( 0.1f);
		DiscountButton.GetComponent<UIPanel>().widgetsAreStatic = doStatic;
		yield return new WaitForSeconds( 0.1f);
		PowerDurationButton.GetComponent<UIPanel>().widgetsAreStatic = doStatic;
		yield return new WaitForSeconds( 0.1f);
		CoinMeterSpeedButton.GetComponent<UIPanel>().widgetsAreStatic = doStatic;
		yield return new WaitForSeconds( 0.1f);
		LuckButton.GetComponent<UIPanel>().widgetsAreStatic = doStatic;
		
		
		yield return new WaitForSeconds( 0.1f);
        HeadBoostButton.GetComponent<UIPanel>().widgetsAreStatic = doStatic;
		yield return new WaitForSeconds( 0.1f);
        DeadBoostButton.GetComponent<UIPanel>().widgetsAreStatic = doStatic;
		yield return new WaitForSeconds( 0.1f);
        BoostButton.GetComponent<UIPanel>().widgetsAreStatic = doStatic;
		yield return new WaitForSeconds( 0.1f);
        ShieldButton.GetComponent<UIPanel>().widgetsAreStatic = doStatic;
		yield return new WaitForSeconds( 0.1f);
        VacuumButton.GetComponent<UIPanel>().widgetsAreStatic = doStatic;
		
		
		yield return new WaitForSeconds( 0.1f);
		BoostImage.GetComponent<UIPanel>().widgetsAreStatic = doStatic;
		yield return new WaitForSeconds( 0.1f);
		PoofImage.GetComponent<UIPanel>().widgetsAreStatic = doStatic;
		yield return new WaitForSeconds( 0.1f);
		MagnetImage.GetComponent<UIPanel>().widgetsAreStatic = doStatic;
		yield return new WaitForSeconds( 0.1f);
		MegaCoinImage.GetComponent<UIPanel>().widgetsAreStatic = doStatic;
		yield return new WaitForSeconds( 0.1f);
		TornadoTokenImage.GetComponent<UIPanel>().widgetsAreStatic = doStatic;
		yield return new WaitForSeconds( 0.1f);
		GemImage.GetComponent<UIPanel>().widgetsAreStatic = doStatic;
		yield return new WaitForSeconds( 0.1f);
		ScoreBonusImage.GetComponent<UIPanel>().widgetsAreStatic = doStatic;
	}

	//TODO: Consolidate most of this functionality into a smaller function
	// Modifier Gemming events
	void TriggerCoinMeterSpeed()
	{
		if (!GameController.SharedInstance.IsPaused)
		{
			if (GameProfile.SharedInstance.Player.CanAffordArtifactGem(CoinMeterSpeedID))
			{
				if(GameProfile.SharedInstance.Player.artifactsGemmed.Count==0)
					ObjectivesDataUpdater.SetGenericStat(ObjectiveType.DistanceWithoutPowerups,GameController.SharedInstance.DistanceTraveled);
				AudioManager.SharedInstance.PlayFX(AudioManager.Effects.MusicBox);
				GameProfile.SharedInstance.Player.GemArtifact(CoinMeterSpeedID);
				DisplayButton(CoinMeterSpeedButton,BonusButtonType.Modifier,ArtifactStore.GetArtifactProtoData(CoinMeterSpeedID)._gemmedTitle);
				UpdateGemCount();
				//Bounce(CoinMeterSpeedButton.transform);
				AddAbilityUsedFlag(AbilityUsed.Mod1);
				UIManagerOz.SharedInstance.inGameVC.HideAbilityTutorial();
			}
			else
				UIManagerOz.SharedInstance.inGameVC.OnOutOfGemsPause(TriggerCoinMeterSpeed);
		}
	}
	
	void TriggerTripleCoins()
	{
		if (!GameController.SharedInstance.IsPaused)
		{
			if (GameProfile.SharedInstance.Player.CanAffordArtifactGem(DoubleCoinsID))
			{
				if(GameProfile.SharedInstance.Player.artifactsGemmed.Count==0)
					ObjectivesDataUpdater.SetGenericStat(ObjectiveType.DistanceWithoutPowerups,GameController.SharedInstance.DistanceTraveled);
				AudioManager.SharedInstance.PlayFX(AudioManager.Effects.MusicBox);
				GameProfile.SharedInstance.Player.GemArtifact(DoubleCoinsID);
				GameController.SharedInstance.SetUpUpgradeData();
				DisplayButton(DoubleCoinsButton,BonusButtonType.Modifier,ArtifactStore.GetArtifactProtoData(DoubleCoinsID)._gemmedTitle);
				UpdateGemCount();
				//Bounce(DoubleCoinsButton.transform);
				AddAbilityUsedFlag(AbilityUsed.Mod2);
				UIManagerOz.SharedInstance.inGameVC.HideAbilityTutorial();
			}
			else
				UIManagerOz.SharedInstance.inGameVC.OnOutOfGemsPause(TriggerTripleCoins);
		}		
	}
	
	void TriggerLuck()
	{
		if (!GameController.SharedInstance.IsPaused)
		{
			if (GameProfile.SharedInstance.Player.CanAffordArtifactGem(LuckID))
			{
				if(GameProfile.SharedInstance.Player.artifactsGemmed.Count==0)
					ObjectivesDataUpdater.SetGenericStat(ObjectiveType.DistanceWithoutPowerups,GameController.SharedInstance.DistanceTraveled);
				UIManagerOz.SharedInstance.inGameVC.HideAbilityTutorial();
				AudioManager.SharedInstance.PlayFX(AudioManager.Effects.MusicBox);
				GameProfile.SharedInstance.Player.GemArtifact(LuckID);
				DisplayButton(LuckButton,BonusButtonType.Modifier,ArtifactStore.GetArtifactProtoData(LuckID)._gemmedTitle);
				UpdateGemCount();
				//Bounce(LuckButton.transform);
				AddAbilityUsedFlag(AbilityUsed.Mod3);
			}
			else
				UIManagerOz.SharedInstance.inGameVC.OnOutOfGemsPause(TriggerLuck);
		}			
	}
	
	void TriggerDiscount()
	{
		if (!GameController.SharedInstance.IsPaused)
		{
			if (GameProfile.SharedInstance.Player.CanAffordArtifactGem(DiscountID))
			{
				if(GameProfile.SharedInstance.Player.artifactsGemmed.Count==0)
					ObjectivesDataUpdater.SetGenericStat(ObjectiveType.DistanceWithoutPowerups,GameController.SharedInstance.DistanceTraveled);
				UIManagerOz.SharedInstance.inGameVC.HideAbilityTutorial();
				AudioManager.SharedInstance.PlayFX(AudioManager.Effects.MusicBox);
				GameProfile.SharedInstance.Player.GemArtifact(DiscountID);
				DisplayButton(DiscountButton,BonusButtonType.Modifier,ArtifactStore.GetArtifactProtoData(DiscountID)._gemmedTitle);
				UpdateGemCount();
				//Bounce(DiscountButton.transform);
				AddAbilityUsedFlag(AbilityUsed.Mod4);
			}
			else
				UIManagerOz.SharedInstance.inGameVC.OnOutOfGemsPause(TriggerDiscount);
		}			
	}
	
	void TriggerPowerDuration()
	{
		if (!GameController.SharedInstance.IsPaused)
		{
			if (GameProfile.SharedInstance.Player.CanAffordArtifactGem(PowerDurationID))
			{
				if(GameProfile.SharedInstance.Player.artifactsGemmed.Count==0)
					ObjectivesDataUpdater.SetGenericStat(ObjectiveType.DistanceWithoutPowerups,GameController.SharedInstance.DistanceTraveled);
				UIManagerOz.SharedInstance.inGameVC.HideAbilityTutorial();
				GameProfile.SharedInstance.Player.GemArtifact(PowerDurationID);
				DisplayButton(PowerDurationButton,BonusButtonType.Modifier,ArtifactStore.GetArtifactProtoData(PowerDurationID)._gemmedTitle);
				UpdateGemCount();
				//Bounce(PowerDurationButton.transform);
				AddAbilityUsedFlag(AbilityUsed.Mod5);
			}
			else
				UIManagerOz.SharedInstance.inGameVC.OnOutOfGemsPause(TriggerPowerDuration);
		}			
	}
	
	public static void HideHeadStarts()
	{
		main.ShowButton(main.HeadBoostButton,false,true);
	}
	
	void HideDiscountButton()
	{
		ShowButton(DiscountButton,false,false);
	}
	
   

	//Consumable events
    void UseHeadBoost(GameObject obj)
	{
		if(!GameController.SharedInstance.IsPaused && GameProfile.SharedInstance.Player.PopConsumable(HeadBoostID))
		{
			UIManagerOz.SharedInstance.inGameVC.HideAbilityTutorial();
            AudioManager.SharedInstance.PlayFX(AudioManager.Effects.MusicBox);

            ObjectivesDataUpdater.AddToGenericStat(ObjectiveType.UseConsumable, 1);
            GamePlayer.SharedInstance.StartHeadBoost(ConsumableStore.ConsumableFromID(HeadBoostID).Value);

            //ShowButton(BigHeadStartButton,false,true);
            DisplayButton(HeadBoostButton, BonusButtonType.Consumable, GameProfile.SharedInstance.Player.GetConsumableLocalizeString(HeadBoostID));
			//Bounce(HeadStartButton.transform);
			AddAbilityUsedFlag(AbilityUsed.Cons1);	//For both head starts
			AddAbilityUsedFlag(AbilityUsed.Cons2);
			
            skillDesc.text = "起飞冲刺";
            skillIcon.spriteName ="common_takeoff";
            ShowTip();

			/* jonoble: Commented out to see if it improves performance
			BaseConsumable consumable = ConsumableStore.ConsumableFromID(HeadStartID);
			
			AnalyticsInterface.LogGameAction(
				"run",
				"consumable_used", 
				consumable.Title,
				EnvironmentSetManager.SharedInstance.CurrentEnvironmentSet.SetCode,
				0
			);
			*/
			//			AnalyticsInterface.UseConsumable( AnalyticsInterface.Consumable.HeadStart, 
			//EnvironmentSetManager.SharedInstance.CurrentEnvironmentSet.SetCode );
			
			if(GameProfile.SharedInstance.Player.IsArtifactPurchased(DiscountID)) {
				ShowButton(DiscountButton,true,false);
			//	Invoke("HideDiscountButton",5f);
			}
			
			FastTravelButton.HideAll();
		}
	}

    private void HideTipPrompt()
    {
        Tip.SetActive(false);
    }

    public void ShowTip(bool inverse = false)
    {
        iv = inverse;
        UIDynamically.instance.LeftToScreen(Tip,-800f,-232f,0.5f,inverse);
        if(!inverse)
        {

            StartCoroutine(BlinkTip());
        }
        else
        {
            StopCoroutine(BlinkTip());
            Invoke("HideTipPrompt",0.5f);
        }

    }
    private bool iv;
    IEnumerator BlinkTip()
    {
        Tip.SetActive(true);
        float t=0f;
        float alpha = 1f;
        UIPanelAlpha panel =Tip.GetComponent<UIPanelAlpha>();
        panel.alpha = 1f;
        while(!iv)
        {
            alpha = Mathf.PingPong(t*5f + 0.85f,0.85f)+0.15f;
            panel.alpha = alpha;
            t+=Realtime.deltaTime;
            yield return null;
        }
    }


    void UseDeadBoost(GameObject obj)
	{
		if(GameProfile.SharedInstance.Player.PopConsumable(DeadBoostID))
		{
            if (GameController.SharedInstance.CurrentMode == GameMode.Boss)
                GameController.SharedInstance.WaitToExitEventMode();

			UIManagerOz.SharedInstance.inGameVC.HideAbilityTutorial();
            UIManagerOz.SharedInstance.inGameVC.resurrectMenu.hideResurrectMenu();

            ObjectivesDataUpdater.AddToGenericStat(ObjectiveType.UseConsumable, 1);
            GamePlayer.SharedInstance.StartDeadBoost(ConsumableStore.ConsumableFromID(DeadBoostID).Value);

            GamePlayer.SharedInstance.isUsedDeadBoostThisRun = true;
            //ShowButton(HeadStartButton,false,true);
            DisplayButton(DeadBoostButton, BonusButtonType.Consumable, GameProfile.SharedInstance.Player.GetConsumableLocalizeString(DeadBoostID));
			//Bounce(BigHeadStartButton.transform);
			AddAbilityUsedFlag(AbilityUsed.Cons2);	//For both head starts
			AddAbilityUsedFlag(AbilityUsed.Cons1);
			
            skillDesc.text = "终极冲刺";
            skillIcon.spriteName ="common_crash";
            ShowTip();


			/* jonoble: Commented out to see if it improves performance
			BaseConsumable consumable = ConsumableStore.ConsumableFromID(BigHeadStartID);
			
			AnalyticsInterface.LogGameAction(
				"run",
				"consumable_used",
				consumable.Title,
				EnvironmentSetManager.SharedInstance.CurrentEnvironmentSet.SetCode,
				0
			);		
			*/
			//			AnalyticsInterface.UseConsumable( AnalyticsInterface.Consumable.BigHeadStart, EnvironmentSetManager.SharedInstance.CurrentEnvironmentSet.SetCode );
			
			
			if(GameProfile.SharedInstance.Player.IsArtifactPurchased(DiscountID)) {
				ShowButton(DiscountButton,true,false);
		//		Invoke("HideDiscountButton",5f);
			}
			
			FastTravelButton.HideAll();
		}
	}

    void UseBoost(GameObject obj)
	{
		if(!GameController.SharedInstance.IsPaused && GameProfile.SharedInstance.Player.PopConsumable(BoostID))
		{
			UIManagerOz.SharedInstance.inGameVC.HideAbilityTutorial();
			AudioManager.SharedInstance.PlayFX(AudioManager.Effects.MusicBox);

            //CharacterStats roleData = GameProfile.SharedInstance.GetActiveCharacter();
            GamePlayer.SharedInstance.StartBoostByTime(GameProfile.SharedInstance.GetDurationByType(BonusItem.BonusItemType.Boost), BonusItem.BonusItemType.Boost);
            ObjectivesDataUpdater.AddToGenericStat(ObjectiveType.UseConsumable, 1);
            //隐藏按钮
            DisplayButton(BoostButton, BonusButtonType.Consumable, GameProfile.SharedInstance.Player.GetConsumableLocalizeString(BoostID));
			//Bounce(BonusMultiplierButton.transform);
			AddAbilityUsedFlag(AbilityUsed.Cons3);
            skillDesc.text = "速度提升";
            skillIcon.spriteName ="common_speedpotion";
            ShowTip();
			/* jonoble: Commented out to see if it improves performance
			BaseConsumable consumable = ConsumableStore.ConsumableFromID(BonusMultiplierID);
			
			AnalyticsInterface.LogGameAction(
				"run",
				"consumable_used",
				consumable.Title,
				EnvironmentSetManager.SharedInstance.CurrentEnvironmentSet.SetCode,
				0
			);
			*/
			//			AnalyticsInterface.UseConsumable( AnalyticsInterface.Consumable.Multiplier, EnvironmentSetManager.SharedInstance.CurrentEnvironmentSet.SetCode );
		}
	}

    void UseShield(GameObject obj)
	{
		if(!GameController.SharedInstance.IsPaused && GameProfile.SharedInstance.Player.PopConsumable(ShieldID))
		{
			UIManagerOz.SharedInstance.inGameVC.HideAbilityTutorial();
            AudioManager.SharedInstance.PlayFX(AudioManager.Effects.MusicBox);

            GamePlayer.SharedInstance.StartPoof(GameProfile.SharedInstance.GetDurationByType(BonusItem.BonusItemType.Shield));
            ObjectivesDataUpdater.AddToGenericStat(ObjectiveType.UseConsumable, 1);

            DisplayButton(ShieldButton, BonusButtonType.Consumable, GameProfile.SharedInstance.Player.GetConsumableLocalizeString(ShieldID));
			//Bounce(StumbleProofButton.transform);
			AddAbilityUsedFlag(AbilityUsed.Cons4);
			
			/* jonoble: Commented out to see if it improves performance
			BaseConsumable consumable = ConsumableStore.ConsumableFromID(StumbleProofID);
			
			AnalyticsInterface.LogGameAction(
				"run",
				"consumable_used",
				consumable.Title,
				EnvironmentSetManager.SharedInstance.CurrentEnvironmentSet.SetCode,
				0
			);
			*/
			//			AnalyticsInterface.UseConsumable( AnalyticsInterface.Consumable.StumbleProof, EnvironmentSetManager.SharedInstance.CurrentEnvironmentSet.SetCode );
		}
	}

    void UseVacuum(GameObject obj)
	{
		if(!GameController.SharedInstance.IsPaused && GameProfile.SharedInstance.Player.PopConsumable(VacuumID))
		{
			UIManagerOz.SharedInstance.inGameVC.HideAbilityTutorial();
            AudioManager.SharedInstance.PlayFX(AudioManager.Effects.MusicBox);

            GamePlayer.SharedInstance.StartVacuum(GameProfile.SharedInstance.GetDurationByType(BonusItem.BonusItemType.Vacuum));
            ObjectivesDataUpdater.AddToGenericStat(ObjectiveType.UseConsumable, 1);

            DisplayButton(VacuumButton, BonusButtonType.Consumable, GameProfile.SharedInstance.Player.GetConsumableLocalizeString(VacuumID));
			//Bounce(ThirdEyeButton.transform);
			AddAbilityUsedFlag(AbilityUsed.Cons5);
			
			/* jonoble: Commented out to see if it improves performance
			BaseConsumable consumable = ConsumableStore.ConsumableFromID(ThirdEyeID);
			
			AnalyticsInterface.LogGameAction(
				"run",
				"consumable_used",
				consumable.Title,
				EnvironmentSetManager.SharedInstance.CurrentEnvironmentSet.SetCode,
				0
			);
			*/
			//			AnalyticsInterface.UseConsumable( AnalyticsInterface.Consumable.ThirdEye, EnvironmentSetManager.SharedInstance.CurrentEnvironmentSet.SetCode );
		}
	}
	
	public void ResetAbilityUsedFlags()
	{
		UsedAbilityFlags = 0;
	}
	
	public void AddAbilityUsedFlag(AbilityUsed abilityFlag)
	{
		//If we haven't used this ability this run, add to our stat
		if(((int)abilityFlag & UsedAbilityFlags) == 0 && abilityFlag < AbilityUsed.AllInGame)
			ObjectivesDataUpdater.AddToGenericStat(ObjectiveType.UseAllInGamePowerups,1);
		//If we've never used this ability before, add this to the "All" stat
		if(((int)abilityFlag & GameProfile.SharedInstance.Player.abilitiesUsed) == 0)
			ObjectivesDataUpdater.AddToGenericStat(ObjectiveType.UseEverything,1);
	
		UsedAbilityFlags |= (int)abilityFlag;
		
		GameProfile.SharedInstance.Player.abilitiesUsed |= (int)abilityFlag;
	}
	
	
	//Pickup events
	public void OnBoostPickedUp(Vector3 worldPos)
	{
		DisplayButton(BoostImage,BonusButtonType.Pickup,"Upg_Powerup_3_Title");
		Vector3 screenPos = GameCamera.SharedInstance.camera.WorldToScreenPoint(worldPos);
		BoostImage.transform.position = UICamera.mainCamera.camera.ScreenToWorldPoint(new Vector3(screenPos.x,screenPos.y,0f));
	}
	public void OnPoofPickedUp(Vector3 worldPos)
	{
		DisplayButton(PoofImage,BonusButtonType.Pickup,"Upg_Powerup_2_Title");
		Vector3 screenPos = GameCamera.SharedInstance.camera.WorldToScreenPoint(worldPos);
		PoofImage.transform.position = UICamera.mainCamera.camera.ScreenToWorldPoint(new Vector3(screenPos.x,screenPos.y,0f));
	}
	public void OnMagnetPickedUp(Vector3 worldPos)
	{
		DisplayButton(MagnetImage,BonusButtonType.Pickup,"Upg_Powerup_4_Title");
		Vector3 screenPos = GameCamera.SharedInstance.camera.WorldToScreenPoint(worldPos);
		MagnetImage.transform.position = UICamera.mainCamera.camera.ScreenToWorldPoint(new Vector3(screenPos.x,screenPos.y,0f));
	}
	public void OnMegaCoinPickedUp(Vector3 worldPos)
	{
		DisplayButton(MegaCoinImage,BonusButtonType.Pickup,"Pkp_MegaCoin");
		Vector3 screenPos = GameCamera.SharedInstance.camera.WorldToScreenPoint(worldPos);
		MegaCoinImage.transform.position = UICamera.mainCamera.camera.ScreenToWorldPoint(new Vector3(screenPos.x,screenPos.y,0f));
	}
	public void OnTornadoTokenPickedUp(Vector3 worldPos)
	{
		DisplayButton(TornadoTokenImage,BonusButtonType.Pickup,"Pkp_DestinyCard");
		Vector3 screenPos = GameCamera.SharedInstance.camera.WorldToScreenPoint(worldPos);
		TornadoTokenImage.transform.position = UICamera.mainCamera.camera.ScreenToWorldPoint(new Vector3(screenPos.x,screenPos.y,0f));
	}
	public void OnGemPickedUp(Vector3 worldPos)
	{
		DisplayButton(GemImage,BonusButtonType.Pickup,"Pkp_Gem");
		Vector3 screenPos = GameCamera.SharedInstance.camera.WorldToScreenPoint(worldPos);
		GemImage.transform.position = UICamera.mainCamera.camera.ScreenToWorldPoint(new Vector3(screenPos.x,screenPos.y,0f));
	}
	public void OnScoreBonusPickedUp(Vector3 worldPos)
	{
		DisplayButton(ScoreBonusImage,BonusButtonType.Pickup,"Pkp_ScoreBonus");
		Vector3 screenPos = GameCamera.SharedInstance.camera.WorldToScreenPoint(worldPos);
		ScoreBonusImage.transform.position = UICamera.mainCamera.camera.ScreenToWorldPoint(new Vector3(screenPos.x,screenPos.y,0f));
	}

    public void OnSkillCountDown(BonusItem.BonusItemType type)
    {  
        if(!bottom.activeSelf)
            bottom.SetActive(true);
     
        if(SkillCountDown.mTypeList.Contains(type))
        {
            SkillCountDown.mSkillList[type].ReStart();
            return;
        }
      
        if(!SkillCountDowns[0].isCountDown)
        {   
            SkillCountDowns[0].appear(type);
        }
        else if(!SkillCountDowns[1].isCountDown)
        {
            SkillCountDowns[1].appear(type);

        }
        else if(!SkillCountDowns[2].isCountDown)
        {
           
            SkillCountDowns[2].appear(type);
        }
        else if(!SkillCountDowns[3].isCountDown)
        {
            SkillCountDowns[3].appear(type);
        }
       
    }


    /// <summary>
    /// 显示隐藏速度线
    /// </summary>
    public void ShowSpeedLine(bool show)
    {
        //NGUIToolsExt.SetActive(speedLine,show);
    }

    public IEnumerator ShowStartBoost(float delayHide = 3f)
    {
        int count = GameProfile.SharedInstance.Player.GetConsumableCount(HeadBoostID);

        //没有可用消耗品
        if ( count <= 0 )
        {
            HeadBoostButton.SetActive(false);
            yield break;
        }

        //已显示
        if (HeadBoostButton.activeSelf)
        {
            yield break;
        }

        HeadBoostButton.SetActive(true);
        
        TweenAlpha.Begin(HeadBoostButton, 0f, 1f);

        //显示拥有数量
        UILabel label = HeadBoostButton.GetComponentInChildren<UILabel>();
        if (label != null)
            label.text = count.ToString();

        yield return new WaitForSeconds(delayHide);

        TweenAlpha.Begin(HeadBoostButton, 1f, 0f);
    }

    public void ShowDeadBoost()
    {
        //int count = GameProfile.SharedInstance.Player.GetConsumableCount(DeadBoostID);

        ////没有可用消耗品
        //if (count <= 0)
        //{
        //    DeadBoostButton.SetActive(false);
        //    return;
        //}
     
        //DeadBoostButton.SetActive(true);
        
        ////显示拥有数量
        //UILabel label = DeadBoostButton.GetComponentInChildren<UILabel>();
        //if (label != null)
        //    label.text = count.ToString();
    }
}
