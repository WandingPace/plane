using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ObjectivesRoot : MonoBehaviour 
{
	protected static Notify notify;
	public UIPostGameViewControllerOz postGameVC;
	
 	//private ObjectivesDataUpdater objectivesDataUpdater;
	
	public UILabel RankLabel;
	//public UILabel RankLabelNumber;
	//public UISlider RankProgress;
	public GameObject objectivesTitle;
	
	public UISprite nextRankRewardIcon;
	//public UILabel nextRankRewardQuantity;	
	
	public UILabel completedAllObjectives;
	
	public GameObject[] objectiveRootGOs = new GameObject[3];	
	public GameObject[] checkboxGOs = new GameObject[3];	// old, needed to maintain animation timing
	public UISprite[] newCheckmarkSprites = new UISprite[3];
	public GameObject[] levelboxGOs = new GameObject[3];
	public GameObject[] levelboxGOsOn = new GameObject[3];
	public ParticleSystem fx_trail;
	public UISprite rewardIcon;
	private Vector3 rewardIconScale;
	public GameObject rewardBg;
	private Vector3 rewardBgScale;
	
	public UISprite seperator1;
	public UISprite seperator2;
	public ParticleSystem completePrt;
	public UILabel completeLabel;
	
	private Vector3 checkboxStartPosition = Vector3.zero;
	
//	private int objectiveCounter = 0;
	private float posTweenTime = 0.1f;
	private float posTweenDistance = 800.0f;	
	private float posTweenCenter = 10.0f;	
	
	private int oldRank = 0;
	private int newRank = 0;

	
	public static bool playAnimations = true;
	
	void Awake()
	{
		notify = new Notify(this.GetType().Name);	
		rewardIconScale = rewardIcon.transform.localScale;
		rewardBgScale = rewardBg.transform.localScale;
	}

	public void EnterObjectivesPage()
	{
		notify.Debug("EnterObjectivesPage " + playAnimations);
		checkboxStartPosition = checkboxGOs[0].transform.localPosition;
		rewardIcon.transform.localScale = rewardIconScale;
		rewardBg.transform.localScale = rewardBgScale;
		oldRank = GameProfile.SharedInstance.Player.GetCurrentRank();		//-- Capture the current rank
		newRank = oldRank;
		SetRank(oldRank);
		SetRankProgressIcons(GameProfile.SharedInstance.Player.GetCurrentRankProgress());
		//RankLabelNumber.text = oldRank.ToString();
		//postGameVC.paperViewController.ShowBackButton(false);				// hide back button
		
		if (playAnimations)
		{
			for (int i=0; i<3; i++)
			{ 
				MoveObjectiveOffscreen(objectiveRootGOs[i]);				// move objectives offscreen
				//SetCheckbox(checkboxGOs[i], false);						// turn off checkboxes	//NGUITools.SetActive(checkboxGOs[i], false);
				//SetCheckbox(levelboxGOs[i], false);	
				checkboxGOs[i].GetComponent<UISprite>().alpha = 0f;
				checkboxGOs[i].transform.localScale = Vector3.zero;
				newCheckmarkSprites[i].enabled = false;
				//notify.Debug("checkbox off " + checkboxGOs[i].name + " " + checkboxGOs[i].GetComponent<UISprite>().alpha);
			}	
			
	//		objectiveCounter = -1;	//0;
			//objectivesDataUpdater = gameObject.AddComponent<ObjectivesDataUpdater>();	// update objective data
			//GameProfile.SharedInstance.Player.UpdateObjectiveStats();	//update objective data - NEW
			UIManagerOz.SharedInstance.PaperVC.UpdateCurrency();
			
			UIManagerOz.SharedInstance.postGameVC.speedUpButton.collider.enabled = true;
			StartCoroutine(DoObjectivesPresentation());
		}
	}
	
	private void SetCheckmarkForObjectiveCompletion(int index)
	{
		ObjectiveCellData cellData = objectiveRootGOs[index].GetComponent<ObjectiveCellData>();
		newCheckmarkSprites[index].spriteName = "icon_objective_status_checked";			
		newCheckmarkSprites[index].enabled = cellData.IsObjectiveCompleted();
	}

	public IEnumerator DoObjectivesPresentation()
	{
		seperator1.enabled = true;
		seperator2.enabled = true;
		completeLabel.enabled = false;
		
		yield return StartCoroutine(BringInAllObjectives());
		
		for(int i=0;i<3;i++)
		{
			UpdateProgress(i);
			
			SetCheckmarkForObjectiveCompletion(i);
		
			while(objectiveRootGOs[i].GetComponent<ObjectiveCellData>().IsCellAnimating)
				yield return null;
			
			TweenColor tween = CheckIfObjectiveCompleted(i);
			
			
			if(tween!=null)
			{
				while(tween!=null && tween.enabled)	yield return null;
				
				yield return StartCoroutine(SwapInNewObjective(i));
				yield return StartCoroutine(GetNewObjective(i));
				CheckIfLevelCompleted();
				
				while(UIManagerOz.SharedInstance.postGameVC.ShowingRewardPanel)
					yield return null;
			}
		}
		
		GameProfile.SharedInstance.Serialize();
		playAnimations = false;
		
		List<ObjectiveProtoData> objs = GameProfile.SharedInstance.Player.objectivesMain;
		
		//Show "Complete!" effect
		if( (objs.Count<1 || objs[0]==null) &&
			(objs.Count<2 || objs[1]==null) &&
			(objs.Count<3 || objs[2]==null) )
		{
			yield return new WaitForSeconds(0.5f);
			completePrt.Play();
			seperator1.enabled = false;
			seperator2.enabled = false;
			completeLabel.enabled = true;
			completeLabel.GetComponent<UILocalize>().SetKey("Ttl_Sub_Completed");
			ShakePanel();
			AudioManager.SharedInstance.PlayFX(AudioManager.Effects.oz_UI_Menu_levelMax);
			yield return new WaitForSeconds(1f);
		}
		
		//NOTE: The above code does the same thing. Do we need this? -bc
		// show "Completed' text if no more uncompleted objectives left
		/*if (GameProfile.SharedInstance.Player.objectivesEarned.Count >= ObjectivesManager.Objectives.Count)
		{
			completedAllObjectives.enabled = true;
			completedAllObjectives.GetComponent<UILocalize>().SetKey("Ttl_Sub_Completed");
		}
		else
			completedAllObjectives.enabled = false;*/
		
		//End the objectives screen
		yield return new WaitForSeconds(1f);
		completePrt.Stop();
		
		ObjectiveCompletedSwitchToStats();
		
		
	}
	
	
	
	

	public IEnumerator BringInAllObjectives() 		
	{
		Invoke("PlaySwoosh", 0.1f);	// play sound effects
		
		//Cache these for later
		List<ObjectiveProtoData> activeObs = GameProfile.SharedInstance.Player.objectivesMain;
		
		for (int i=0; i<3; i++)
		{
			
			//Look for duplicates, and do the same thing with those as we do with null objectives
			bool isDuplicate = false;
			for(int j=i-1;j>=0;j--)
			{
				if( i<activeObs.Count && j<activeObs.Count && 
					activeObs[i]!=null && activeObs[j]!=null && 
					activeObs[i]._id==activeObs[j]._id )
				{
					isDuplicate = true;
					break;
				}
			}
			
			
			if(GameProfile.SharedInstance.Player.objectivesMain[i] != null && !isDuplicate)
			{
				/*TweenPosition tween = */BringInObjective(objectiveRootGOs[i]);
				//tween.delay = tweenDelay;
				//tweenDelay += posTweenTime;
				
				
				//PlaySwoosh();
				
			//	if (i == 2) 	// for final objective, when tween is done move to CheckObjective()
		//		{ 
			//		tween.eventReceiver = this.gameObject;
			//		tween.callWhenFinished = "UpdateProgress";
			//	}
			
				if(i!=2)
				{
					yield return new WaitForSeconds(posTweenTime);
				}
			}
			else
			{
				//? not sure about this one yet...
				yield return StartCoroutine(GetNewObjective(i));
			}
		}
	}
	
	private void PlaySwoosh()
	{
		//if(AudioManager.SharedInstance!=null)	AudioManager.Instance.GetSoundEffectsPlayer().PlayOneShot(AudioManager.Instance.Swish, 1.0f);
		AudioManager.SharedInstance.PlayFX(AudioManager.Effects.oz_UI_Menu_play);
	}
	
	public void UpdateProgress(int index) 		
	{	
		
	//	objectiveCounter++;
		notify.Debug("UpdateProgress called with index = " + index);	
		
		if (index < 3 && index >= 0)
		{
			ObjectiveProtoData data = GameProfile.SharedInstance.Player.objectivesMain[index];	
			ObjectiveCellData cellData = objectiveRootGOs[index].GetComponent<ObjectiveCellData>();
			cellData.SetData(data,"current");	
			//TweenScale progressScale = objectiveRootGOs[objectiveCounter].GetComponent<ObjectiveCellData>().AnimateProgressBar(data._conditionList[0]._earnedStatValue);
			//progressScale.eventReceiver = this.gameObject;
			//progressScale.callWhenFinished = "CheckIfObjectiveCompleted";
			
			GameProfile.SharedInstance.Player.UpdateObjectiveStat(index);
			
			if(data!=null)
			{
				objectiveRootGOs[index].GetComponent<ObjectiveCellData>().AnimateProgressBar(data._conditionList[0]._earnedStatValue);
			}
		}
		else	// done updating progress of all 3 objectives
		{
		//	GameProfile.SharedInstance.Serialize();				// save progress
			//postGameVC.ActivateButtons();	//ShowButtons();	// activate 'next' and 'menu' buttons
		//	playAnimations = false;
		//	Invoke("ObjectiveCompletedSwitchToStats", 1f);
			// wait for button press...
		}
	
		if (newRank != oldRank)		
		{
			//UIOkayDialogOz.onPositiveResponse -= UpdateProgress;	// clean up dialog box callback if necessary
			SetRank(newRank);
			SetRankProgressIcons(0);	//AnimateRankProgressBar(0.0f, 0.2f);		// clear rank progress
			oldRank = newRank;
			//RankLabelNumber.text = newRank.ToString();
			//objectiveCounter = -1; only enable that if you want to complete objective check after levelUp
			notify.Debug("we got a new rank " + newRank);
		}
	}
	
	
	
	private void ObjectiveCompletedSwitchToStats(){
		notify.Debug("ObjectiveCompletedSwitchToStats");
		postGameVC.ShowStatsPage();
		StatsRoot sr = postGameVC.statsRoot.GetComponent<StatsRoot>();
		sr.StartAnimSeq();
	}
	
	public TweenColor CheckIfObjectiveCompleted(int slot) 	
	{
		notify.Debug("CheckIfObjectiveCompleted called with objectiveCounter = " + slot);			
		
		if ((slot < 0)||(slot >= GameProfile.SharedInstance.Player.objectivesMain.Count))
		{
			notify.Warning("objectiveCounter = " + slot + ", which is out of range, in CheckIfObjectiveCompleted!");
	//		UpdateProgress();	// this should never happen, so just kick it back and exit
		}
		else
		{	
			ObjectiveProtoData data = GameProfile.SharedInstance.Player.objectivesMain[slot];	
			ObjectiveCellData cellData = objectiveRootGOs[slot].GetComponent<ObjectiveCellData>();
			cellData.SetData(data,"current");														// update data, now that animation has been invoked		
			
			//cellData.UpdateProgressBar();		//SetData calls this instead			// get rid of yellow bar
			
			if (cellData.IsObjectiveCompleted())
			{
				ObjectiveProtoData ob = GameProfile.SharedInstance.Player.objectivesMain[slot];
				GameProfile.SharedInstance.Player.objectivesEarned.Add(ob._id);			// save objective ID in 'earned' list
				GameProfile.SharedInstance.Player.objectivesEarnedDuringRun.Add(ob._title); // Used for publishing Facebook achievements
				if(ob._pointValue > 0){
					string achievementId = BundleInfo.GetBundleId() + "." + data._title;
					ObjectivesDataUpdater.objectivesQueue.Add(achievementId);
				}
				cellData.MakeTextCompleted();											// make description text say 'Completed!"
				TweenColor tween = SetCheckbox(checkboxGOs[slot], true);	// activate checkbox
			//	tween.eventReceiver = this.gameObject;
			//	tween.callWhenFinished = "SwapInNewObjective";	//CheckIfLevelCompleted


                if (AudioManager.SharedInstance != null)
                    AudioManager.SharedInstance.GetSoundEffectsPlayer().PlayOneShot(AudioManager.SharedInstance.ScoreBlast, 1.0f);	// play sound effect				
				
				
				float progress = GameProfile.SharedInstance.Player.GetCurrentRankProgress();
				notify.Debug("ObjectiveIsCompleted " + slot + " progress " + progress);
				AnimateRankProgress(checkboxGOs[slot], 0.3f, progress);
				
				return tween;
				
//				float progress = GameProfile.SharedInstance.Player.GetCurrentRankProgress();
//				if (progress == 0.0f)
//					//AnimateRankProgressBar(1.0f, 1.0f);										// animate to full bar
//				else
//					//AnimateRankProgressBar(GameProfile.SharedInstance.Player.GetCurrentRankProgress(), 1.0f);		// animate to reflect progress
			}
			//Not controlled here anymore
		//	else
		//		UpdateProgress();														// update progress of next objective
		}
		return null;
	}
	
	
	public void AnimateRankProgress(GameObject checkbox, float time, float progress)
	{
		
		//float progress = GameProfile.SharedInstance.Player.GetCurrentRankProgress();
		int progressInt = -1;
		if (progress > 0.1f)
			progressInt = 0;
		if (progress > 0.5f)
			progressInt = 1;
		if (progress == 0)
			progressInt = 2;
		
		
		notify.Debug("AnimateRankProgress " + checkbox + " progressInt " + progressInt);
		
		if(progressInt >= 0){		
			//levelboxGOsOn[progressInt].transform.position = checkbox.transform.position;
			//levelboxGOsOn[progressInt].active =  true;
			//levelboxGOsOn[progressInt].GetComponent<UISprite>().alpha = 1f;
			fx_trail.transform.position = checkbox.transform.position - Vector3.forward * 0.180f;
			if( Time.timeScale < 1.2f){
				fx_trail.enableEmission = true;
				fx_trail.Play(true);
			}
			//iTween.MoveTo(levelboxGOsOn[progressInt], iTween.Hash(
			iTween.MoveTo(fx_trail.gameObject, iTween.Hash(
				"isLocal", false, 
				"position", levelboxGOs[progressInt].transform.position - Vector3.forward * 0.180f,
				"oncompletetarget", gameObject,
				"oncomplete", "AnimateRankProgressComplete",	
				"oncompleteparams", levelboxGOsOn[progressInt],
				"time", time, 
				"easetype", iTween.EaseType.easeOutSine)
				);
		}
	}
	
	public void AnimateRankProgressComplete(GameObject checkbox){
		fx_trail.Stop(true);
		fx_trail.enableEmission = false;
		fx_trail.Clear();
		checkbox.SetActive(true);
		checkbox.GetComponent<UISprite>().alpha = 1f;
		if( Time.timeScale < 1.2f){
			checkbox.transform.GetComponentInChildren<ParticleSystem>().Play(true);
		}
		//AudioManager.SharedInstance.PlayFX(AudioManager.Effects.oz_UI_meterFull03);
		ShakePanel();
	}
	
	private void ShakePanel(){
		iTween.ShakePosition(gameObject, iTween.Hash(
			"amount", new Vector3(0.02f,0.02f,0f),
			"time", 0.2f,
			"isLocal", false
			));
	}
	
	public void SetRankProgressIcons(float progress)	{
		int progressInt = -1;
		
		// convert float back to count of how many objectives completed towards the 3 for completing the level
		if (progress > 0.1f)
			progressInt = 0;
		if (progress > 0.5f)
			progressInt = 1;
		if (progress > 0.8f)
			progressInt = 2;
		
		notify.Debug("SetRankProgressIcons " + progress  + " progressInt " + progressInt);
		
		for (int i=0; i<=2; i++){
			if (i <= progressInt){
				//levelboxGOs[i].GetComponent<UISprite>().spriteName = "button_radio_on";
				levelboxGOs[i].SetActive(false);
				levelboxGOsOn[i].SetActive(true); 
				levelboxGOsOn[i].GetComponent<UISprite>().alpha = 1f;
			}	
			else{
				//levelboxGOs[i].GetComponent<UISprite>().spriteName = "button_radio_off";
				levelboxGOs[i].SetActive(true); 
				levelboxGOsOn[i].SetActive(false);
			}
		}
	}
	

	[System.Obsolete("等级不再给予奖励")]
	public void CheckIfLevelCompleted()		
	{
		notify.Debug("CheckIfLevelCompleted");
		newRank = GameProfile.SharedInstance.Player.GetCurrentRank();
				
		if (newRank != oldRank)																// new rank level earned, so show popup and give rewards
			GetRewards();
	
	}
	
	
	private void AnimateStatsIn()
	{
		notify.Debug("AnimateStatsIn");
		//UIRewardDialogOz.onPositiveResponse -= AnimateStatsIn;
		UIManagerOz.SharedInstance.postGameVC.AnimateStatsIn();
	}
	
	private void GetRewards()
	{
		notify.Debug("GetRewards " + oldRank);
		ShakePanel();
		//AudioManager.SharedInstance.PlayFX(AudioManager.Effects.oz_UI_Menu_levelUp);
		rewardIcon.transform.GetComponentInChildren<ParticleSystem>().Play();
		RankRewardType currentRewardType = GameProfile.SharedInstance.Player.GetRankRewardTypeForLevel(oldRank);
		int currentRewardItemID = GameProfile.SharedInstance.Player.GetRankRewardQuanityOrItemForLevel(oldRank);	//, currentRewardType);
		string rewardText = GameProfile.SharedInstance.Player.GetRewardTextFor(oldRank);	//currentRewardType, currentRewardItemID);
		string iconName = GameProfile.SharedInstance.Player.GetRewardIconFor(oldRank);	//currentRewardType, currentRewardItemID);

		
		iTween.ScaleTo(rewardBg, iTween.Hash(
			"scale", rewardBgScale * 1.5f,
			"time", 0.4f,
			"easetype", iTween.EaseType.easeOutBack
			));
		
		UIManagerOz.SharedInstance.postGameVC.rewardIcon.spriteName = iconName;
		UIManagerOz.SharedInstance.postGameVC.rewardLabel.text = rewardText;
		UIManagerOz.SharedInstance.postGameVC.ShowRewardPanelDelay(0.6f);

        if (AudioManager.SharedInstance != null) AudioManager.SharedInstance.GetSoundEffectsPlayer().PlayOneShot(AudioManager.SharedInstance.AngelWings, 1.0f);	// play sound effect
		
		if (currentRewardType == RankRewardType.Coins) 
			GameProfile.SharedInstance.Player.coinCount += currentRewardItemID;
		else if (currentRewardType == RankRewardType.Gems) 
			GameProfile.SharedInstance.Player.specialCurrencyCount += currentRewardItemID;
		
		// consumables
		else if (currentRewardType == RankRewardType.HeadBoostConsumables) 
			GameProfile.SharedInstance.Player.EarnConsumable(UIManagerOz.SharedInstance.inGameVC.bonusButtons.HeadBoostID, currentRewardItemID);
		else if (currentRewardType == RankRewardType.DeadBoostConsumables) 
			GameProfile.SharedInstance.Player.EarnConsumable(UIManagerOz.SharedInstance.inGameVC.bonusButtons.DeadBoostID, currentRewardItemID);					
		else if (currentRewardType == RankRewardType.BoostConsumables) 
			GameProfile.SharedInstance.Player.EarnConsumable(UIManagerOz.SharedInstance.inGameVC.bonusButtons.BoostID, currentRewardItemID);
		else if (currentRewardType == RankRewardType.ShieldConsumables) 		
			GameProfile.SharedInstance.Player.EarnConsumable(UIManagerOz.SharedInstance.inGameVC.bonusButtons.ShieldID, currentRewardItemID);
		else if (currentRewardType == RankRewardType.VacuumtConsumables) 	
			GameProfile.SharedInstance.Player.EarnConsumable(UIManagerOz.SharedInstance.inGameVC.bonusButtons.VacuumID, currentRewardItemID);
		

		string earnedObjectiveString = "";
		
		foreach (string objTitle in GameProfile.SharedInstance.Player.objectivesEarnedDuringRun)
		{
			earnedObjectiveString += objTitle + "-";
		}
		
		/*
		AnalyticsInterface.LogGameAction(
			"multiplier",
			"achieved", 
			currentRewardType.ToString(),
			earnedObjectiveString,
			0
		);*/

		
		UIManagerOz.SharedInstance.PaperVC.UpdateCurrency();
	}
	
	public IEnumerator SwapInNewObjective(int slot)	
	{
//		if (newRank != oldRank)		// clean up dialog box callback if necessary
//		{
//			UIOkayDialogOz.onPositiveResponse -= SwapInNewObjective;			
//			SetRank(newRank);
//			AnimateRankProgressBar(0.0f, 0.2f);
//			oldRank = newRank;
//		}
		
		notify.Debug("SwapInNewObjective");
		// start the objective swap
		if(slot > 2)	yield break;
		
		TweenPosition clearOutTween = ClearOutObjective(objectiveRootGOs[slot]);
		while(clearOutTween.enabled)
			yield return null;
		//clearOutTween.eventReceiver = this.gameObject;
		//clearOutTween.callWhenFinished = "GetNewObjective";
	}

    [System.Obsolete("主线任务完成后不再填充新任务")]
	public IEnumerator GetNewObjective(int slot)
	{
		//ResetCheckboxPositionAndScale(checkboxGOs[objectiveCounter]);		// put checkbox back to start position, since offscreen now
		notify.Debug("GetNewObjective() objectiveCounter " + slot);
	//	ObjectiveProtoData ob = GameProfile.SharedInstance.Player.objectivesActive[objectiveCounter];
		ObjectiveProtoData objData = GameProfile.SharedInstance.Player.RefillObjectiveForIndex(slot);	//, ob._conditionList[0]._statValue);	// get new current objective
				
		if (objData != null)
		{
			ObjectiveProtoData ob = GameProfile.SharedInstance.Player.objectivesMain[slot];						// get new data
			
			MoveObjectiveOffscreen(objectiveRootGOs[slot]);	
			objectiveRootGOs[slot].GetComponent<ObjectiveCellData>().SetData(ob,"current");				// set new data to the cell
			//SetCheckbox(checkboxGOs[objectiveCounter], false);												// turn off checkbox
			checkboxGOs[slot].GetComponent<UISprite>().alpha = 0f;
			newCheckmarkSprites[slot].enabled = false;
		//	MoveObjectiveOffscreen(objectiveRootGOs[objectiveCounter]);										// move it back to start position, ready for entrance animation
		
			Invoke("PlaySwoosh",0.1f);
			TweenPosition bringInTween = BringInObjective(objectiveRootGOs[slot]);		
			//bringInTween.eventReceiver = this.gameObject;
			//bringInTween.callWhenFinished = "CheckIfLevelCompleted";	//"UpdateProgress";
			while(bringInTween!=null && bringInTween.enabled)
				yield return null;
		}
		else
		{
		//	ob = GameProfile.SharedInstance.Player.objectivesActive[objectiveCounter];						// get new data
			
			MoveObjectiveOffscreen(objectiveRootGOs[slot]);	
			objectiveRootGOs[slot].GetComponent<ObjectiveCellData>().SetData(null,"current");				// set new data to the cell
			
		//	CheckIfLevelCompleted();
		}
	}
	
	private void ResetCheckboxPositionAndScale(GameObject checkbox)
	{
		checkbox.transform.localPosition = checkboxStartPosition;
		checkbox.transform.localScale = Vector3.one;
	}
	
	private void MoveObjectiveOffscreen(GameObject objectiveRoot)
	{
		//objectiveRoot.transform.localPosition = new Vector3(-1 * posTweenDistance, objectiveRoot.transform.localPosition.y, 0.0f);	
		objectiveRoot.transform.localPosition = new Vector3(posTweenCenter - posTweenDistance, objectiveRoot.transform.localPosition.y, 0.0f);	
	}

	public TweenPosition BringInObjective(GameObject objectiveRoot) 		
	{
		//return TweenPosition.Begin(objectiveRoot, posTweenTime, new Vector3(0.0f, objectiveRoot.transform.localPosition.y, 0.0f));
		//Invoke("PlaySwoosh",0.15f);
		return TweenPosition.Begin(objectiveRoot, posTweenTime, new Vector3(posTweenCenter, objectiveRoot.transform.localPosition.y, 0.0f));
	}
	
	public TweenPosition ClearOutObjective(GameObject objectiveRoot) 		
	{
		return TweenPosition.Begin(objectiveRoot, posTweenTime, new Vector3(posTweenDistance, objectiveRoot.transform.localPosition.y, 0.0f));
	}	
	
	public TweenColor SetCheckbox(GameObject checkboxGO, bool on) 		
	{
		TweenColor tween;
		
		if (on)
			tween = TweenColor.Begin(checkboxGO, 1.0f, Color.white);							// Activate checkbox
		else
			tween = TweenColor.Begin(checkboxGO, 0.001f, new Color(1.0f, 1.0f, 1.0f, 0.0f));	// turn it off
		
		return tween;
	}
	
	public void FillInObjectiveData() 
	{
		int index = 0;
		
		foreach(ObjectiveProtoData data in GameProfile.SharedInstance.Player.objectivesMain)
		{
			objectiveRootGOs[index].GetComponent<ObjectiveCellData>().SetData(data,"current");
			index++;
		}
	}
	
	public void ResetRank(){
		//this is called from UIPost after reward has been rewarded
		notify.Debug("ResetRank " + (newRank));
		SetRank(newRank);
	}
	
	private void SetRank(int rank) 
	{
		notify.Debug("SetRank = " + rank.ToString());
		
		//RankLabel.text = "Level " + rank.ToString();
		RankLabel.text = Localization.SharedInstance.Get("Lbl_Level") + " " + rank.ToString();
		rewardBg.transform.localScale = rewardBgScale;
		nextRankRewardIcon.spriteName = GameProfile.SharedInstance.Player.GetRewardIconFor(rank);
		//nextRankRewardQuantity.text = GameProfile.SharedInstance.Player.GetRankRewardQuanityOrItemForLevel(rank+1).ToString();
		
		//if (nextRankRewardQuantity.text == "0")
		//	nextRankRewardQuantity.text = "";	// hide the label, if it's 0
	}
	

}



