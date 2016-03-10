using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PauseObjectives : MonoBehaviour 
{
	public GameObject[] objectiveRootGOs = new GameObject[3];
	public UISprite[] newCheckmarkSprites = new UISprite[3];
	//public UILabel rankLabel; - don't need in pause menu N.N.
	
	public UILabel completedLabel;
	public UISprite separator1;
	public UISprite separator2;
	
	public void FillInObjectiveData() 
	{
		completedLabel.enabled = false;
		separator1.enabled = true;
		separator2.enabled = true;
		
		int index = 0;
		
		foreach (ObjectiveProtoData data in GameProfile.SharedInstance.Player.objectivesMain)
		{
			Vector3 pos = objectiveRootGOs[index].transform.localPosition;
			
			objectiveRootGOs[index].GetComponent<ObjectiveCellData>().SetData(data, "current");
			
			int earnedProgress = GameProfile.SharedInstance.Player.GetObjectiveProgressDuringRun(index);
			ObjectiveCellData cellData = objectiveRootGOs[index].GetComponent<ObjectiveCellData>();
			cellData.SetProgressBarValueOverride(earnedProgress);					// set progress earned so far this run
			newCheckmarkSprites[index].spriteName = "icon_objective_status_checked";			
			newCheckmarkSprites[index].enabled = cellData.IsObjectiveCompleted();	// turn on checkmark if objective completed

			
			//Move the objective offscreen if the data is null (there are no objectives left!)
			if (data != null)
				pos.x = 0f;
			else
				pos.x = 1000f;
			
			objectiveRootGOs[index].transform.localPosition = pos;
			
			index++;
		}
		if (GameProfile.SharedInstance.Player.objectivesMain.Count>=3 &&
			GameProfile.SharedInstance.Player.objectivesMain[0]==null &&
			GameProfile.SharedInstance.Player.objectivesMain[1]==null &&
			GameProfile.SharedInstance.Player.objectivesMain[2]==null)
		{
			completedLabel.enabled = true;
			separator1.enabled = false;
			separator2.enabled = false;
		}
	}
}





//	public void FillInObjectiveData() 
//	{
//		objectiveRootGOs[0].GetComponent<ObjectiveCellData>().SetData(GameProfile.SharedInstance.Player.objectivesActive[0],"current");
//		objectiveRootGOs[1].GetComponent<ObjectiveCellData>().SetData(GameProfile.SharedInstance.Player.objectivesActive[1],"current");
//		objectiveRootGOs[2].GetComponent<ObjectiveCellData>().SetData(GameProfile.SharedInstance.Player.objectivesActive[2],"current");
//	}	


//
//		SetRank(GameProfile.SharedInstance.Player.GetCurrentRank());	//-- Capture the current rank
//		//SetRankProgress(GameProfile.SharedInstance.Player.GetCurrentRankProgress());
//	}	
//	
//	private void SetRank(int rank) 
//	{
//		//rankLabel.text = "Level " + rank.ToString();
//		//rankLabel.text = Localization.SharedInstance.Get("Lbl_Level") + " " + rank.ToString(); - don't need in pause menu N.N.
//	}
//}


//	private void SetRankProgress(float progress)
//	{
//		//rankProgress.sliderValue = Mathf.Clamp01(progress);	//-- Just to be safe, clamp from 0 to 1.
//	}	
	

	
//	public void UpdateProgress() 		
//	{
//		Debug.LogWarning("UpdateProgress called with objectiveCounter = " + objectiveCounter);		
//	
//		objectiveCounter++;
//		
//		for (int objectiveCounter = 0; objectiveCounter < 3; objectiveCounter++)
//		{
//			ObjectiveProtoData data = GameProfile.SharedInstance.Player.objectivesActive[objectiveCounter];
//			TweenScale progressScale = objectiveRootGOs[objectiveCounter].GetComponent<ObjectiveCellData>().SetData(data);	//.AnimateProgressBar(data._earnedStatValue);  UpdateProgressBar
//		}
//	}


			//progressScale.eventReceiver = this.gameObject;
			//progressScale.callWhenFinished = "CheckIfObjectiveCompleted";
		
		//else	// done updating progress of all 3 objectives
		//{
			//GameProfile.SharedInstance.Serialize();				// save progress
			//postGameVC.ActivateButtons();	//ShowButtons();	// activate 'next' and 'menu' buttons
			//playAnimations = false;
			// wait for button press...
		//}


	//public UIPostGameViewControllerOz postGameVC;
	
 	//private ObjectivesDataUpdater objectivesDataUpdater;
	
	//public UILabel RankLabel;
	//public UISlider RankProgress;
	//public GameObject objectivesTitle;

	
	//public GameObject[] checkboxGOs = new GameObject[3];
	
	//private int objectiveCounter = 0;
	//private float posTweenTime = 0.2f;
	//private float posTweenDistance = 800.0f;	
	
	//private int oldRank = 0;
	//private int newRank = 0;
		
	//public static bool playAnimations = true;
	
//	void Update() { }
//
//	public void EnterObjectivesPage()
//	{
//		oldRank = GameProfile.SharedInstance.Player.GetCurrentRank();		//-- Capture the current rank
//		SetRank(oldRank);
//		SetRankProgress(GameProfile.SharedInstance.Player.GetCurrentRankProgress());
//		
//		postGameVC.paperViewController.ShowBackButton(false);				// hide back button
//		
//		if (playAnimations)
//		{
//			for (int i=0; i<3; i++)
//			{ 
//				MoveObjectiveOffscreen(objectiveRootGOs[i]);				// move objectives offscreen
//				SetCheckbox(checkboxGOs[i], false);							// turn off checkboxes	//NGUITools.SetActive(checkboxGOs[i], false);
//			}	
//			
//			objectiveCounter = -1;	//0;
//			objectivesDataUpdater = gameObject.AddComponent<ObjectivesDataUpdater>();	// update objective data
//			postGameVC.UpdateCurrency();
//			BringInAllObjectives();
//		}
//		else 
//		{
//			postGameVC.ActivateButtons();	//ShowButtons();				// activate 'next' and 'menu' buttons
//			// wait for button press...
//		}
//	}
//
//	public void BringInAllObjectives() 		
//	{
//		float tweenDelay = 0.0f;
//		
//		for (int i=0; i<3; i++)
//		{
//			TweenPosition tween = BringInObjective(objectiveRootGOs[i]);
//			tween.delay = tweenDelay;
//			tweenDelay += posTweenTime;
//			
//			Invoke("PlaySwoosh", tweenDelay);	// play sound effects
//			
//			if (i == 2) 	// for final objective, when tween is done move to CheckObjective()
//			{ 
//				tween.eventReceiver = this.gameObject;
//				tween.callWhenFinished = "UpdateProgress";
//			}
//		}
//	}
//	
//	private void PlaySwoosh()
//	{
//		//AudioManager.Instance.PlayFX(AudioManager.Effects.oz_UI_Menu_back);	
//		//AudioManager.Instance.PlayCoin();	//(AudioManager.Effects.oz_UI_Menu_back);	
//		//AudioManager.Instance.PlayFX(AudioManager.Effects.oz_UI_Menu_back, 1.0f, 1.0f);
//		AudioManager.Instance.SoundEffects.PlayOneShot(AudioManager.Instance.oz_UI_Menu_back, 1.0f);
//	}
//

//
//	public void CheckIfObjectiveCompleted() 	
//	{
//		Debug.LogWarning("CheckIfObjectiveCompleted called with objectiveCounter = " + objectiveCounter);			
//		
//		if (objectiveCounter >= GameProfile.SharedInstance.Player.objectivesActive.Count) 
//		{
//			Debug.LogWarning("objectiveCounter = " + objectiveCounter + ", which is out of range, in CheckIfObjectiveCompleted!");
//			UpdateProgress();
//		}
//		else
//		{
//			ObjectiveProtoData data = GameProfile.SharedInstance.Player.objectivesActive[objectiveCounter];	
//			ObjectiveCellData cellData = objectiveRootGOs[objectiveCounter].GetComponent<ObjectiveCellData>();
//			cellData.SetData(data);														// update data, now that animation has been invoked		
//			cellData.UpdateProgressBar();												// get rid of yellow bar
//			
//			if (cellData.IsObjectiveCompleted())
//			{
//				ObjectiveProtoData ob = GameProfile.SharedInstance.Player.objectivesActive[objectiveCounter];
//				GameProfile.SharedInstance.Player.objectivesEarned.Add(ob._id);			// save objective ID in 'earned' list
//				cellData.MakeTextCompleted();											// make description text say 'Completed!"
//				AudioManager.Instance.SoundEffects.PlayOneShot(AudioManager.Instance.ScoreBlast, 1.0f);	// play sound effect
//				TweenColor tween = SetCheckbox(checkboxGOs[objectiveCounter], true);	// activate checkbox
//				tween.eventReceiver = this.gameObject;
//				tween.callWhenFinished = "CheckIfLevelCompleted";	//SwapInNewObjective";
//				
//				float progress = GameProfile.SharedInstance.Player.GetCurrentRankProgress();
//				if (progress == 0.0f)
//				{
//					AnimateRankProgressBar(1.0f, 1.0f);										// animate to full bar
//				}
//				else
//				{
//					AnimateRankProgressBar(GameProfile.SharedInstance.Player.GetCurrentRankProgress(), 1.0f);		// animate to reflect progress
//				}
//			}
//			else { UpdateProgress(); }													// update progress of next objective
//		}
//	}
//
//	public void AnimateRankProgressBar(float progress, float time)
//	{
//		iTween.ValueTo(gameObject, iTween.Hash(
//			"from", RankProgress.sliderValue,
//			"to", progress,
//			"onupdatetarget", gameObject,
//			"onupdate", "SetRankProgress",
//			"time", time, 
//			"easetype", iTween.EaseType.easeOutExpo));
//	}
//	
//	public void CheckIfLevelCompleted()		
//	{
//		newRank = GameProfile.SharedInstance.Player.GetCurrentRank();
//				
//		if (newRank != oldRank)																// new rank level earned, so show popup and give rewards
//		{
//			GetRewards();
//		}
//		else { SwapInNewObjective(); }
//	}
//	
//	private void GetRewards()
//	{
//		RankRewardType currentRewardType = GameProfile.SharedInstance.Player.GetRankRewardTypeForLevel(oldRank);
//		int currentRewardItemID = GameProfile.SharedInstance.Player.GetRankRewardQuanityOrItemForLevel(oldRank, currentRewardType);
//		string rewardText = GameProfile.SharedInstance.Player.GetRewardTextFor(currentRewardType, currentRewardItemID);
//		string iconName = GameProfile.SharedInstance.Player.GetRewardIconFor(currentRewardType, currentRewardItemID);
//				
//		UIOkayDialogOz.onPositiveResponse += SwapInNewObjective;
//		UIManagerOz.SharedInstance.okayDialog.ShowRewardDialog("You earned " + rewardText, "for reaching level " + newRank + "!", "mainicons_coins");	//iconName);		
//		
//		//AudioManager.SharedInstance.PlayFX(AudioManager.Effects.angelWings);		// play reward sound
//		AudioManager.Instance.SoundEffects.PlayOneShot(AudioManager.Instance.AngelWings, 1.0f);	// play sound effect
//		
//		if (currentRewardType == RankRewardType.Coins) 
//		{
//			GameProfile.SharedInstance.Player.coinCount += currentRewardItemID;
//		}
//		else if (currentRewardType == RankRewardType.Gems) 
//		{
//			GameProfile.SharedInstance.Player.specialCurrencyCount += currentRewardItemID;
//		}
//	}
//	
//	public void SwapInNewObjective()	
//	{
//		if (newRank != oldRank)		// clean up dialog box callback if necessary
//		{
//			UIOkayDialogOz.onPositiveResponse -= SwapInNewObjective;			
//			SetRank(newRank);
//			AnimateRankProgressBar(0.0f, 0.2f);
//			oldRank = newRank;
//		}
//		
//		// start the objective swap
//		TweenPosition clearOutTween = ClearOutObjective(objectiveRootGOs[objectiveCounter]);
//		clearOutTween.eventReceiver = this.gameObject;
//		clearOutTween.callWhenFinished = "GetNewObjective";
//	}
//	
//	public void GetNewObjective()
//	{
//		ObjectiveProtoData ob = GameProfile.SharedInstance.Player.objectivesActive[objectiveCounter];
//		GameProfile.SharedInstance.Player.RefillObjectiveForIndex(objectiveCounter, ob._statValue);		// get new current objective
//		
//		ob = GameProfile.SharedInstance.Player.objectivesActive[objectiveCounter];						// get new data
//		objectiveRootGOs[objectiveCounter].GetComponent<ObjectiveCellData>().SetData(ob);				// set new data to the cell
//		SetCheckbox(checkboxGOs[objectiveCounter], false);												// turn off checkbox
//		MoveObjectiveOffscreen(objectiveRootGOs[objectiveCounter]);										// move it back to start position, ready for entrance animation
//		
//		TweenPosition bringInTween = BringInObjective(objectiveRootGOs[objectiveCounter]);		
//		bringInTween.eventReceiver = this.gameObject;
//		bringInTween.callWhenFinished = "UpdateProgress";
//	}
//	
//	private void MoveObjectiveOffscreen(GameObject objectiveRoot)
//	{
//		objectiveRoot.transform.localPosition = new Vector3(-1 * posTweenDistance, objectiveRoot.transform.localPosition.y, 0.0f);	
//	}
//
//	public TweenPosition BringInObjective(GameObject objectiveRoot) 		
//	{
//		return TweenPosition.Begin(objectiveRoot, posTweenTime, new Vector3(0.0f, objectiveRoot.transform.localPosition.y, 0.0f));
//	}
//	
//	public TweenPosition ClearOutObjective(GameObject objectiveRoot) 		
//	{
//		return TweenPosition.Begin(objectiveRoot, posTweenTime, new Vector3(posTweenDistance, objectiveRoot.transform.localPosition.y, 0.0f));
//	}	
//	
//	public TweenColor SetCheckbox(GameObject checkboxGO, bool on) 		
//	{
//		TweenColor tween;
//		
//		if (on) { tween = TweenColor.Begin(checkboxGO, 1.0f, Color.white); }						// Activate checkbox
//		else { tween = TweenColor.Begin(checkboxGO, 0.001f, new Color(1.0f, 1.0f, 1.0f, 0.0f)); }	// turn it off
//		return tween;
//	}
//
//
//	private void SetRank(int rank) 
//	{
//		RankLabel.text = "Level " + rank.ToString();
//	}
//	
//	private void SetRankProgress(float progress)
//	{
//		progress = Mathf.Clamp01(progress);					//-- Just to be safe, clamp from 0 to 1.
//		RankProgress.sliderValue = progress;
//	}
//}


	
			//SetRank(newRank);
			//SetRankProgress(GameProfile.SharedInstance.Player.GetCurrentRankProgress(), 0.1f);
			//AnimateRankProgressBar(0.0f, 0.01f);
			//Invoke("GetRewards", 0.1f);	//GetRewards();
	

//	currentRewardType = GameProfile.SharedInstance.Player.GetRankRewardTypeForLevel(oldrank);
//	currentRewardItemID = GameProfile.SharedInstance.Player.GetRankRewardQuanityOrItemForLevel(oldrank, currentRewardType);
//	string rewardText = GameProfile.SharedInstance.Player.GetRewardTextFor(currentRewardType, currentRewardItemID);
//	string iconName = GameProfile.SharedInstance.Player.GetRewardIconFor(currentRewardType, currentRewardItemID);
	
	


				//SetRankProgress(GameProfile.SharedInstance.Player.GetCurrentRankProgress());	// need to make this animate

//		
//		if (GameProfile.SharedInstance.Player.objectivesEarned.Contains(ob._id) == false)
//		{
//			GameProfile.SharedInstance.Player.objectivesEarned.Add(ob._id);	
//		}

		
		//objectiveCounter++;

//			TweenColor checkboxAlpha = TweenColor.Begin(checkboxGOs[objectiveCounter], 1.0f, Color.white);			// Activate checkbox

			
//			TweenColor.Begin(checkboxGOs[i], 0.001f, new Color(1.0f, 1.0f, 1.0f, 0.0f));	

	
		//FillInObjectiveData();
	


			//NGUITools.SetActive(objectiveRootGOs[i], true);		// activate all objectives and move them offscreen

//		
//	private void ActivateCheckbox(int index, ObjectiveProtoData data)	
//	{
//		if (true)	//data._earnedStatValue >= data._statValue)
//		{
//			//NGUITools.SetActive(checkboxGOs[index], true); 
//			TweenColor.Begin(checkboxGOs[index], posTweenTime, Color.white);
//		}
//	}	
//		
		
		
//		
//		int index = 0;
//		foreach(ObjectiveProtoData data in GameProfile.SharedInstance.Player.objectivesActive)
//		{
//			objectiveRootGOs[index].GetComponent<ObjectiveCellData>().AnimateProgressBar(data._earnedStatValue);
//			index++;
//		}		
		
				
//		progressBarFill = gameObject.transform.Find("SlicedSpriteFill01").gameObject;
		
//		if (data._statValue != 0)	// prevent divide by zero
//		{
//			progressBarFill.transform.localScale = new Vector3(400.0f * ((float)data._earnedStatValue / (float)data._statValue),
//				progressBarFill.transform.localScale.y, progressBarFill.transform.localScale.z);
//		}
		
//		ActivateCheckbox(objectiveCounter, objectiveRootGOs[objectiveCounter].GetComponent<ObjectiveCellData>().data);


			// added by Alex, to turn on checkbox if objective completed:
//			float xPos = (data._earnedStatValue >= data._statValue) ? -211.5f : -22222211.5f;
//			checkboxGOs[index].transform.localPosition = new Vector3(xPos, 0.0f, 0.0f);

			
//			if (data._earnedStatValue >= data._statValue) 
//			{
//				checkboxGOs[index].transform.localPosition = new Vector3(-211.5f, 0.0f, 0.0f); 
//			}
//			else 
//			{ 
//				checkboxGOs[index].transform.localPosition = new Vector3(-22222211.5f, 0.0f, 0.0f); 
//			}

//	
//	public GameObject objectiveOne = null;
//	public GameObject objectiveTwo = null;
//	public GameObject objectiveThree = null;	
//	
//	public UISprite objectiveOneCheckBox = null;
//	public UISprite objectiveTwoCheckBox = null;
//	public UISprite objectiveThreeCheckBox = null;	
//	

	
	//public const float cTransitionTime = 0.5f;


		//objectivesChecker = gameObject.GetComponent<ObjectivesChecker>();
		
		// set up references to objective root GameObjects
		//objectiveRootGOs.Add(objectiveOne);
		//objectiveRootGOs.Add(objectiveTwo);		
		//objectiveRootGOs.Add(objectiveThree);		
		
		// set up checkbox references and turn them all off
		//checkboxGOs.Add(objectiveOneCheckBox.gameObject);
		//checkboxGOs.Add(objectiveTwoCheckBox.gameObject);		
		//checkboxGOs.Add(objectiveThreeCheckBox.gameObject);		

//	private GameObject GetObjectiveRootFromIndex(int index) 
//	{
//		switch(index)
//		{
//			case 0:
//				return objectiveOne;
//			case 1:
//				return objectiveTwo;
//			case 2: 
//				return objectiveThree;
//			default: 
//				return null;			
//		}
//	}
//	
//	private UISprite GetCheckBoxForObjectiveRootFromIndex(int index)
//	{
//		switch(index)
//		{
//			case 0: 
//				return objectiveOneCheckBox;
//			case 1: 
//				return objectiveTwoCheckBox;
//			case 2:
//				return objectiveThreeCheckBox;
//			default: 
//				return null;			
//		}	
//	}


		
//		if (index < 0 || index >=3) { return null; }
//		if (index == 0) { return objectiveOneCheckBox; }
//		else if (index == 1) { return objectiveTwoCheckBox; }
//		else if (index == 2) { return objectiveThreeCheckBox; }
//		return null;
		
		
		//if (index < 0 || index >=3) { return null; }
//		if (index == 0) { return objectiveOne; }
//		else if (index == 1) { return objectiveTwo; }
//		else if (index == 2) { return objectiveThree; }
//		return null;


		//objectivesChecker.SetObjectiveData();
		

		//DidComputeObjectives = false;
		//ComputeCompletedObjectives();	//-- This will save the player state to disk.


//
//	//-- We have moved the completed objective to the left.  now see if we Leveled, pop the level reward if so. other move onto pulling in the new objective.
//	private RankRewardType currentRewardType = RankRewardType.Coins;
//	private int currentRewardItemID = -1;
//	
//	public void OnDoneAwardObjectiveProgress(UITweener tween)
//	{
//		//TR.LOG ("OnDoneAwardObjectiveProgress");
//		if (tween) { tween.onFinished -= OnDoneAwardObjectiveProgress; }
//		
//		if (currentAwardIndex < 3) 
//		{
//			//Transform currentObectiveRoot = getObjectiveRootFromIndex(currentAwardIndex);
//			
//			int oldrank = animatedRanks[currentAwardIndex];
//			int newrank = animatedRanks[currentAwardIndex+1];
//			if (oldrank != newrank) 
//			{
//				//-- LEVELED!
//				//-- WE set this data so that if the user hits "next" to speed through the animations.
//				//-- we don't double reward because when they hit next, we will walk this list again,
//				//-- looking for a change in rank so that we can award the rank change.
//				animatedRanks[currentAwardIndex] = newrank;
//				
//				currentRewardType = GameProfile.SharedInstance.Player.GetRankRewardTypeForLevel(oldrank);
//				currentRewardItemID = GameProfile.SharedInstance.Player.GetRankRewardQuanityOrItemForLevel(oldrank, currentRewardType);
//				string rewardText = GameProfile.SharedInstance.Player.GetRewardTextFor(currentRewardType, currentRewardItemID);
//				string iconName = GameProfile.SharedInstance.Player.GetRewardIconFor(currentRewardType, currentRewardItemID);
//				//TR.LOG ("LevelReward {0} = {1}", currentRewardType, currentRewardItemID);
//				
//				UIConfirmDialogOz.onPositiveResponse += OnGetRankReward;	
//				UIManager.SharedInstance.rewardDialog.ShowRewardDialog(rewardText, "On Reaching Level " + newrank, iconName);
//				SetRank(newrank);
//				AudioManager.SharedInstance.PlayFX(AudioManager.Effects.angelWings);
//				return;
//			}
//			else 
//			{
//				currentAwardIndex++;
//				if (currentAwardIndex < 3) 
//				{
//					RankProgress.sliderValue = animatedRankProgress[(currentAwardIndex*2)];
//					TweenSlider ts = null;
//					float duration = 0.5f;
//					float end = animatedRankProgress[(currentAwardIndex*2)+1];
//					if(Mathf.Abs(end-RankProgress.sliderValue) < Mathf.Epsilon) { duration = 0.01f; }
//					ts = TweenSlider.Begin(RankProgress.gameObject, duration, end);
//					ts.onFinished += OnDoneAwardObjectiveProgress;
//					ts.method = UITweener.Method.Linear;
//					return;
//				}
//			}	
//		}
//		
//		int count = 1;
//		foreach (int index in completedIndices)
//		{
//			GameObject currentObectiveRoot = GetObjectiveRootFromIndex(index);
//			if (currentObectiveRoot)
//			{
//				//-- Animate offscreen, start the reward loop again.
//				Vector3 offscreenLeft = new Vector3(-Screen.width*3.0f, currentObectiveRoot.transform.localPosition.y, currentObectiveRoot.transform.localPosition.z);
//				TweenPosition tp = TweenPosition.Begin(currentObectiveRoot, cTransitionTime*(1.25f*count), offscreenLeft);
//				if (tp) 
//				{
//					tp.method = UITweener.Method.EaseInOut;
//					tp.onFinished += OnObjectiveAwardedFinished;
//				}
//				count++;
//			}
//		}
//	}
//	


//	
//	public void MoveToStatPage() 
//	{
//		if (HaveGivenLevelRewards == false) 	//-- Give rewards if we haven't done so yet.
//		{
//			for (int i=0; i<(animatedRanks.Count-1); i++) 
//			{
//				int oldrank = animatedRanks[i];
//				int newrank = animatedRanks[i+1];	
//				
//				if (oldrank != newrank) 
//				{
//					currentRewardType = GameProfile.SharedInstance.Player.GetRankRewardTypeForLevel(oldrank);
//					currentRewardItemID = GameProfile.SharedInstance.Player.GetRankRewardQuanityOrItemForLevel(oldrank, currentRewardType);
//					GiveLevelRewards(currentRewardType, currentRewardItemID);	
//				}
//			}	
//		}
//		
//		postGameVC.ShowStatsPage();	//false);	//true);
//	}
	

//	private bool FillInObjectiveDataForIndex(int index, ObjectiveProtoData data) 
//	{
//		GameObject currentObjectiveRoot = getObjectiveRootFromIndex(index);
//		NGUITools.SetActive(currentObjectiveRoot, true);
//		
//		currentObjectiveRoot.gameObject.GetComponent<ObjectiveCellData>().data = data;	
//		
//		// added by Alex, to turn on checkbox if objective completed:
//		if (data._earnedStatValue >= data._statValue) { checkboxGOs[index].transform.localPosition = new Vector3(-211.5f, 0.0f, 0.0f); }
//		else { checkboxGOs[index].transform.localPosition = new Vector3(-22222211.5f, 0.0f, 0.0f); }
//		return true;
//	}	

		
			//if (currentObjectiveRoot == null) { TR.ERROR("We should never have a null here."); return false; }
		//if (data._title == null || data._title.Length == 0) { return false; }
			
		
		//label.text = data._descriptionPreEarned + " (earned: " + data._earnedStatValue + "), (need:" + data._statValue + ")";



//	
//	// We have shown objectives one first appearance OR have completed cycle of CheckBox, move left, move in from right. walk down list of objectives until have animated them all.
//	public void OnObjectiveFlyinFinished (UITweener tween) 
//	{
//		TR.LOG ("OnObjectiveFlyinFinished");
//		if (tween != null) { tween.onFinished -= OnObjectiveFlyinFinished;}
//		//CurrentState = State.ObjectiveReward;
//		
//		//-- Play back the changes rank.progress.objectives changes.
//	 	for (int i=1; i<animatedRanks.Count; i++)
//		{
//			if (currentAwardIndex >= GameProfile.SharedInstance.Player.objectivesActive.Count) { break; }
//			
//			ObjectiveProtoData currentOb = GameProfile.SharedInstance.Player.objectivesActive[currentAwardIndex];
//			int currentObID = -1;
//			if (currentOb != null) { currentObID = currentOb._id; }
//			
//			//-- Move to the next objective if they haven't changes OR the current one is nil or empty.
//			if (currentObID == oldObjectiveIds[currentAwardIndex] || oldObjectiveIds[currentAwardIndex] == -1)
//			{
//				currentAwardIndex++;
//				continue;
//			}
//				
//			//-- Animate the checkbox.
//			UISprite checkBox = getCheckBoxForObjectiveRootFromIndex(currentAwardIndex);
//			if (checkBox) 
//			{
//				NGUITools.SetActive(checkBox.gameObject, true);
//				TweenColor tc = TweenColor.Begin(checkBox.gameObject, 0.33f, Color.clear);
//				tc.style = UITweener.Style.PingPong;
//			}
//			
//			if (currentAwardIndex > 0) 
//			{
//				checkBox = getCheckBoxForObjectiveRootFromIndex(currentAwardIndex-1);
//				if (checkBox)
//				{
//					TweenColor tc = checkBox.GetComponent<TweenColor>() as TweenColor;
//					if (tc) { tc.enabled = false; }
//					checkBox.color = Color.white;
//				}
//			}
//			
//			AudioManager.SharedInstance.PlayFX(AudioManager.Effects.bonusMeterFull);
//			
//			//RankProgress.sliderValue = animatedRankProgress[(currentAwardIndex*2)];
//			TweenSlider ts = null;
//			//ts = TweenSlider.Begin(RankProgress.gameObject, 1.0f, animatedRankProgress[(currentAwardIndex*2)+1]);
//			ts = TweenSlider.Begin(RankProgress.gameObject, 0.66f, RankProgress.sliderValue);
//			ts.onFinished += OnObjectiveFlyinFinished;
//			currentAwardIndex++;
//			return;
//		}
//		
//		//-- If we get here, time to animate the progressbar because we have checked off the completed objectives.
//		TR.LOG ("READY TO PROGRESS");
//		if (currentAwardIndex > 0) 
//		{
//			UISprite checkBox = getCheckBoxForObjectiveRootFromIndex(currentAwardIndex-1);
//			if (checkBox) 
//			{
//				TweenColor tc = checkBox.GetComponent<TweenColor>() as TweenColor;
//				if (tc) { tc.enabled = false; }
//				checkBox.color = Color.white;
//			}
//		}
//		
//		//-- Now start ticking up the progressbar.
//		currentAwardIndex = 0;
//		RankProgress.sliderValue = animatedRankProgress[(currentAwardIndex*2)];
//		TweenSlider tweenSlider = null;
//		tweenSlider = TweenSlider.Begin(RankProgress.gameObject, 0.5f, animatedRankProgress[(currentAwardIndex*2)+1]);
//		tweenSlider.onFinished += OnDoneAwardObjectiveProgress;
//		tweenSlider.method = UITweener.Method.Linear;
//	}
//
//	

	
	//-- Repop the data for the new objectives and move in from the right.
//	public void OnObjectiveAwardedFinished(UITweener tween) 
//	{
//		//TR.LOG ("OnObjectiveAwardedFinished");
//		Transform currentObjectiveRoot = tween.gameObject.transform;//getObjectiveRootFromIndex(currentAwardIndex);
//		int currentIndex = 0;
//		if (currentObjectiveRoot == objectiveTwo) { currentIndex = 1; }
//		else if (currentObjectiveRoot == objectiveThree) { currentIndex = 2; }
//		
//		Vector3 offscreenRight = new Vector3(Screen.width*3.0f, currentObjectiveRoot.localPosition.y, currentObjectiveRoot.localPosition.z);
//		currentObjectiveRoot.localPosition = offscreenRight;
//		
//		if (FillInObjectiveDataForIndex(currentIndex, GameProfile.SharedInstance.Player.objectivesActive[currentIndex]) == false) 
//		{
//			NGUITools.SetActive(currentObjectiveRoot.gameObject, false);
//			return;
//		}
//		
//		//-- Start fly in from right.
//		//Vector3 onscreen = getStartingPositionForObjectiveRootFromIndex(currentIndex);
//		//TweenPosition tp = TweenPosition.Begin(currentObectiveRoot.gameObject, cTransitionTime*1.25f, onscreen);
//		//if(tp) { tp.method = UITweener.Method.EaseInOut; }
//	}


//		if (GameProfile.SharedInstance == null ||
//			GameProfile.SharedInstance.Player == null ||
//			GameProfile.SharedInstance.Player.objectivesActive == null)
//			return;
		


			//if (ob == null) { TR.ERROR("We should never have a null here."); continue; }
						
			
//			if (FillInObjectiveDataForIndex(index, ob) == false) 
//			{
//				GameObject currentObjectiveRoot = getObjectiveRootFromIndex(index);
//				if (currentObectiveRoot != null) { NGUITools.SetActive(getObjectiveRootFromIndex(index), false); }
//			}
//			index++;

	
	//-- We have clicked "get reward", now move the completed objectives off to the left. then move to repopulated the objectives data.
//	public void OnGetRankReward() 
//	{
//		//TR.LOG ("OnGetRankReward");
//		UIConfirmDialogOz.onPositiveResponse -= OnGetRankReward;	
//		GameObject currentObectiveRoot = getObjectiveRootFromIndex(currentAwardIndex);
//		//-- Animate offscreen, start the reward loop again.
//		Vector3 offscreenLeft = new Vector3(-Screen.width*3.0f, currentObectiveRoot.transform.localPosition.y, currentObectiveRoot.transform.localPosition.z);
//		TweenPosition tp = TweenPosition.Begin(currentObectiveRoot, cTransitionTime*1.25f, offscreenLeft);
//		if(tp)
//		{
//			tp.method = UITweener.Method.EaseInOut;
//			tp.onFinished += OnDoneAwardObjectiveProgress;
//		}
//		SetRankProgress(0);
//		AudioManager.SharedInstance.PlayFX(AudioManager.Effects.cashRegister);
//		currentAwardIndex++;
//		
//		GiveLevelRewards(currentRewardType, currentRewardItemID);
//	}

//		//-- Walk in reverse order, since the 3rd objectives animates in last.
//		//-- We need to set at least one FLYIN IS OVER callback or else we need to call the callback directly.
//		bool hasActive = false;
//		//ObjectiveProtoData ob = null;			
//		
//		if (oldObjectiveIds.Count > 2) 
//		{
//			if (oldObjectiveIds[2] != -1) { hasActive = true; }
//			else { NGUITools.SetActive(objectiveThree, false); }	
//		}
//		else { NGUITools.SetActive(objectiveThree, false); }
//		
//		if (oldObjectiveIds.Count > 1) 
//		{
//			if (oldObjectiveIds[1] != -1) 
//			{
//				if (hasActive == false) { hasActive = true; }
//			}
//			else { NGUITools.SetActive(objectiveTwo, false); }	
//		}
//		else { NGUITools.SetActive(objectiveTwo, false); }
//		
//		if (oldObjectiveIds.Count > 0) 
//		{
//			if (oldObjectiveIds[0] != -1) 
//			{
//				if (hasActive == false) { hasActive = true; }
//			}
//			else { NGUITools.SetActive(objectiveOne, false); }
//		}
//		else { NGUITools.SetActive(objectiveOne, false); }
//		
//		//-- NO objectives left to earn, skip the the flyin.
//		if (hasActive == false) { postGameVC.ShowStatsPage(); }	//false); }	//true); }

		
		//NGUITools.SetActive(objecttiveOneCheckBox.gameObject, false);
		//NGUITools.SetActive(objecttiveTwoCheckBox.gameObject, false);
		//NGUITools.SetActive(objecttiveThreeCheckBox.gameObject, false);	
	

	//public enum State { ObjectiveFlyin, ObjectiveReward, Stats, }
	//public State CurrentState = State.ObjectiveFlyin;

	
//	private Vector3 getStartingPositionForObjectiveRootFromIndex(int index) 
//	{
//		if (index < 0 || index >=3) { return Vector3.zero; }
//		if (index == 0) { return objectiveOneStartPosition; }
//		else if (index == 1) { return objectiveTwoStartPosition; }
//		else if (index == 2) { return objectiveThreeStartPosition; }
//		return Vector3.zero;
//	}

