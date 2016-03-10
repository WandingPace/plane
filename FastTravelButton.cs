using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FastTravelButton : MonoBehaviour 
{
	public int environmentSetId;
	public UILabel label;
	//public int cost = 10000;
	public UILabel costLabel;
	public UILabel quantityLabel;	
	
	private Hashtable on;
	private Hashtable off;
//	private static Hashtable fadeInHash, fadeOutHash;
	
	private static List<FastTravelButton> All = new List<FastTravelButton>();
	
	//private static UIPanel mainPanel;
	
	//TODO: cache tweenAlpha rather than using GetComponent
	private TweenAlpha tweenAlpha;
	//TODO: make sure button is not active when offscreen
//	private bool turnedOff = false;
//	[HideInInspector]
	
	protected static Notify notify;
	
	public void Awake()
	{
		if (notify == null)
		{
			notify = new Notify(this.GetType().Name);	
		}
		//mainPanel = gameObject.GetComponent<UIPanel>();
		//gameObject.SetActive(false);
	//	fadeInHash = iTween.Hash("alpha",1f,"time",0.25f,"includechildren",true);
	//	fadeOutHash = iTween.Hash("alpha",0f,"time",0.25f,"includechildren",true);
		All.Add(this);
	}
	
	void OnDestroy()
	{
		All.Remove(this);
	}

	public bool IsSetDownloaded()
	{
		return EnvironmentSetManager.SharedInstance.LocalDict.ContainsKey(environmentSetId);
	}
	
	private static int fadeCount = 0;
	
	public static void FadeOut()
	{
		fadeCount++;
		
		foreach(FastTravelButton ftb in All)
			TweenAlpha.Begin(ftb.gameObject, 0.15f, 0f);
	}
	
	public static void FadeIn()
	{
		fadeCount--;
		
		if (fadeCount<=0)
		{
			foreach(FastTravelButton ftb in All)
				TweenAlpha.Begin(ftb.gameObject, 0.15f, 1f);
		}
	}

	private static int buttoncount = 0;
	
	public void Show(int x)
	{
		if (buttoncount<0)
			buttoncount = 0;
		
		//costLabel.text = RealCost.ToString();
		int quantity = GameProfile.SharedInstance.Player.GetConsumableCount(GetFastTravelConsumableID(environmentSetId));
		quantityLabel.text = quantity.ToString();

		gameObject.SetActive(true);
		collider.enabled = true;
		
		// position the button horizontally based on received X parameter
		gameObject.transform.localPosition = new Vector3(x, gameObject.transform.localPosition.y, gameObject.transform.localPosition.z);
			
		iTween.MoveTo(gameObject, iTween.Hash(
			"y", -180f,	//-1400f,	//-160f - buttoncount*120f,
			"islocal", true,
			"time", 0.5f,
			"oncomplete", "ShowComplete",
			"oncompletetarget", gameObject,
			"oncompleteparams", 6.5f));	//3.5f));	//4f));
		
		buttoncount++;
		
		PopupNotification.PopupList[PopupNotificationType.Generic].FadeOut();
		PopupNotification.PopupList[PopupNotificationType.Objective].FadeOut();
		
		// todo full localization
		string title = EnvironmentSetManager.SharedInstance.LocalDict[environmentSetId].GetLocalizedTitle();
		label.text = string.Format(Localization.SharedInstance.Get ("Lbl_FastTravel"), title);
		
		//TODO: make sure button is not active when offscreen
		
//		Vector3 position = gameObject.transform.position;
//		gameObject.transform.position = new Vector3(position.x, 50f, position.z);
//		turnedOff = true;
//		gameObject.SetActive(false);
	
		Invoke ("BlinkIcons", 3f);
	}
	
	private void BlinkIcons()
	{
		if (gameObject.activeSelf)
		{
			StopCoroutine("BlinkIcon");
			StartCoroutine("BlinkIcon");	
		}
	}
	
	public static void HideAll()
	{
		foreach (FastTravelButton ftb in All)
			ftb.ShowComplete(0f);
		buttoncount = 0;
	}
	
	public void ShowComplete(float delay)
	{
		if (delay==0f)
			collider.enabled = false;
		
		if (collider.enabled)
			buttoncount--;
		
		iTween.MoveTo(gameObject, iTween.Hash(
			"y", 50f,
			"islocal", true,
			"time", 0.3f,
			"delay", delay,
			"oncomplete", "Hide",
			"oncompletetarget", gameObject
			));
	}
	
	public void Hide()
	{
		if (collider.enabled)
			buttoncount--;
		
		gameObject.SetActive(false);
		collider.enabled = false;

		PopupNotification.PopupList[PopupNotificationType.Generic].FadeIn();
		PopupNotification.PopupList[PopupNotificationType.Objective].FadeIn();


	}
	
	public void FastTravelTo(GameObject button)
	{
		if (GameController.SharedInstance.IsPaused || GamePlayer.SharedInstance.Dying || GamePlayer.SharedInstance.IsDead)
			return;
		
		collider.enabled = false;
		BonusButtons.HideHeadStarts();
		ShowComplete(0f);
		GamePlayer.SharedInstance.StartFastTravel(environmentSetId); //, 1000f);
		//GameProfile.SharedInstance.Player.coinCount -= RealCost;
		GameProfile.SharedInstance.Player.consumablesPurchasedQuantity[GetFastTravelConsumableID(environmentSetId)]--;
		
		//string environmentTitle = EnvironmentSetManager.SharedInstance.LocalDict[environmentSetId].GetLocalizedTitle();
		//AnalyticsInterface.LogInAppCurrencyActionEvent( CostType.Coin, cost, "fast_travel", environmentTitle, 0, "store" );
		
		/* jonoble: Commented out to see if it improves performance
		BaseConsumable consumable = ConsumableStore.ConsumableFromID(GetFastTravelConsumableID(environmentSetId));
		
		AnalyticsInterface.LogGameAction(
			"run",
			"consumable_used",
			consumable.Title,
//			GameProfile.SharedInstance.Player.GetConsumableLocalizeString(GetFastTravelConsumableID(environmentSetId)), 
			EnvironmentSetManager.SharedInstance.CurrentEnvironmentSet.SetCode,
			0
		);
		*/	
		
		StartCoroutine(DelayedSetEstimatedDistance());
		
		HideAll();
	}
	
	private IEnumerator DelayedSetEstimatedDistance()
	{
		yield return new WaitForSeconds(0.25f);
		UIManagerOz.SharedInstance.inGameVC.SetEstimatedDistanceLeft(250f);
	}
	
	public int GetFastTravelConsumableID(int environmentSetID)
	{
		switch (environmentSetID)
		{
			case EnvironmentSetManager.WhimsyWoodsId:
				return 7;
			case EnvironmentSetManager.DarkForestId:
				return 8;
			case EnvironmentSetManager.YellowBrickRoadId:
				return 9;	
			case EnvironmentSetManager.EmeraldCityId:
				return 4;	
			default:
				notify.Error("Unhandled environmentSetId {0}", environmentSetID);
				break;
		}
		return 7;	// return Whimsie Woods by default, should never get here
	}
	
	IEnumerator BlinkIcon()
	{
		float t=0f;
		float alpha = 1f;
		
		UIPanelAlpha panel = GetComponent<UIPanelAlpha>();
		
		panel.alpha = 1f;
	
		yield return new WaitForSeconds(2f);	//5f);
		
		//Blink for ten seconds (so that it blinks until it's hidden)
		while(t<5f)	//2f)
		{
			alpha = Mathf.PingPong(t*5f + 0.85f,0.85f)+0.15f;
			panel.alpha = alpha;
			collider.enabled = true;	// keep collider on, even when it's flashing
			t+=Realtime.deltaTime;
			yield return null;
		}
	}		
}




	
//	private int RealCost
//	{
//		get { return (int)(cost * GameProfile.SharedInstance.GetHeadStartDiscount()); }
//	}


		//if (GamePlayer.SharedInstance.HasBoost || GamePlayer.SharedInstance.HasFastTravel)
		//	return;
		//if (environmentSetId == EnvironmentSetManager.SharedInstance.CurrentEnvironmentSet.SetId)
		//	return;
		//if (GameProfile.SharedInstance.Player.coinCount < cost)
		//if (GameProfile.SharedInstance.Player.GetConsumableCount(GetFastTravelConsumableID(environmentSetId)) <= 0)
		//	return;
		