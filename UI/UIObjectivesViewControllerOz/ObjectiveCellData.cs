using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class ObjectiveCellData : MonoBehaviour 
{
	public UISprite progressBarFill;				// green
	public UISprite progressBarNewEarnedFill;		// yellow
	public UISprite icon, rewardIcon, checkedIcon;
	// wxj
	public UILabel titleLabel, descLabel, actiDescLabel, actiCollectLabel;	//, statProgressLabel;
	// wxj
	public UILabel titleLabelSysFont, descLabelSysFont, actiTitleLabel;
	// wxj
	public UISprite actiRewardIcon;
	// wxj
	public UISprite[] envCompletedSprites;
	public UILabel[] envNumLabels;
	public GameObject envPanel;
	
	public GameObject buttonCollect;
	
	private ObjectiveProtoData _data;	
	public int oldEarnedStatValue = 0;
	
	//private NotificationSystem notificationSystem;
	private NotificationIcons notificationIcons;
	
	private string objectiveList = "current";
	
	protected static Notify notify;
		
	void Awake()
	{
		notify = new Notify( this.GetType().Name );
		
		notificationIcons = gameObject.GetComponent<NotificationIcons>();		

		Destroy(gameObject.GetComponent<UIPanel>());	// kill auto-attached UIPanel component
		
		//titleLabel = gameObject.transform.Find("LabelTitle01").GetComponent<UILabel>();		
		//descLabel = gameObject.transform.Find("LabelDescription01").GetComponent<UILabel>();
		//statProgressLabel = gameObject.transform.Find("LabelStatProgress").GetComponent<UILabel>();	
		
		progressBarFill = transform.Find("SlicedSpriteFill01").GetComponent<UISprite>();
		Transform fillNew = transform.Find("SlicedSpriteFill02");
		
		if (fillNew)
			progressBarNewEarnedFill = fillNew.GetComponent<UISprite>();
		
		NGUITools.SetActive(buttonCollect, false);
	}
	
	public void SetData(ObjectiveProtoData data, string objectiveListType)
	{
		_data = data;
		
		objectiveList = objectiveListType;
		
		if (_data != null && _data._conditionList!=null && _data._conditionList.Count!=0)
		{
			if ( objectiveListType == "team" )
			{
				oldEarnedStatValue = _data._conditionList[0]._earnedStatValue + _data._conditionList[0]._earnedNeighborValue;
			}
			else
			{
				oldEarnedStatValue = _data._conditionList[0]._earnedStatValue;		// back up old _earnedStatValue, if exists
			}
			
			UpdateLabelText();			// populate text fields	
			UpdateProgressBar( objectiveListType );	
			UpdateIcons();
			SetNotificationIcon();		// show notification icon if appropriate
		}
		
		SetCollectButton();				// show collect button if unclaimed objective is present, otherwise hide button
	}
	
	public void SetProgressBarValueOverride(int earnedProgress)
	{
		if (_data!=null)
		{
			UpdateProgressBar(earnedProgress);	
		}
	}
	
	private void SetCollectButton()
	{
		if(_data!=null)
		{	
			bool active = GameProfile.SharedInstance.Player.objectivesUnclaimed.Contains(_data._id);	
			NGUITools.SetActive(buttonCollect, active);	// set 'collect' button active if this cell has ID of unclaimed completed challenge
					
		}
	}
	
	private void UpdateIcons()		// set icon sprite
	{
		if(_data==null)
		{
			icon.spriteName = "";
			return;
		}
		
		if ( icon != null )
		{
			icon.spriteName = ObjectivesManager.GetIconName( (int) _data._category );
		}
		
		
		if ( rewardIcon != null )
		{
			// the new objective cell data's reward icon requires a different atlas than the original.
			rewardIcon.spriteName = ObjectivesManager.GetRewardIconSpriteName( (int)_data._rewardType );
		}
		
		
		
//		notify.Debug( string.Format("[ObjectiveCellData] : UpdateIcons for: {0} Reward Icon Now: {1}", objectiveList, rewardIcon.spriteName ) );
		
		if ( objectiveList == "weeklyPostRun" )
		{
			//The weekly post run's objectiveCell's reward icon uses the old altas.
            gameObject.transform.Find("RewardIcon").GetComponent<UISprite>().spriteName = ObjectivesManager.GetRewardIconSpriteName( (int) _data._rewardType );
		}
		
		if (checkedIcon != null)
			checkedIcon.enabled = IsObjectiveMarkedComplete();	
		
		if (rewardIcon != null)
			rewardIcon.enabled = ! IsObjectiveMarkedComplete() || GameProfile.SharedInstance.Player.objectivesUnclaimed.Contains( _data._id );
	
		// wxj
		if(rewardIcon == null)
		{
			return;
		}
		// wxj, MegaHeadStart activity
		if(objectiveList == "activity" && _data._rewardType == RankRewardType.MegaHeadStartConsumables)
		{
			actiRewardIcon.enabled = true;
			rewardIcon.enabled = false;
		}
		else
		{
			if(rewardIcon != null)
				rewardIcon.enabled = true;
			
			if(actiRewardIcon != null)
				actiRewardIcon.enabled = false;
		}
		
	}		
	
	private void UpdateLabelText()	// populate text fields
	{
		//if (titleLabel == null)
		//	Start();
		
		if (_data==null)
			return;
		
		//titleLabel.gameObject.GetComponent<UILocalize>().SetKey(_data._title);					//titleLabel.text = _data._title;	
		//descLabel.gameObject.GetComponent<UILocalize>().SetKey(_data._descriptionPreEarned);	//descLabel.text = _data._descriptionPreEarned;	// " (val=" + _objectiveData._statValue + ")";
		//statProgressLabel.text = "(earned: " + _data._conditionList[0]._earnedStatValue + ", need: " + _data._conditionList[0]._statValue + ")"; 
		
		if  (objectiveList == "weekly" || objectiveList == "team" )
		{
			descLabelSysFont.enabled = true;
			
			// Turn off reward value text if objective is marked as complete.
			titleLabelSysFont.enabled = !IsObjectiveMarkedComplete() || GameProfile.SharedInstance.Player.objectivesUnclaimed.Contains( _data._id );
			
			titleLabelSysFont.text = _data._rewardValue.ToString();
			descLabelSysFont.text = _data._descriptionPreEarned;
			titleLabel.enabled = descLabel.enabled = false;
			
			// wxj
			actiTitleLabel.enabled = actiDescLabel.enabled = false;
			NGUITools.SetActive(envPanel, false);
			
		}
		else if (objectiveList == "legendary")
		{
			titleLabel.enabled = descLabel.enabled = true;
			titleLabel.text = Localization.SharedInstance.Get(_data._title);
			descLabel.text = Localization.SharedInstance.Get(_data._descriptionPreEarned);	
			titleLabelSysFont.enabled = descLabelSysFont.enabled = false;	
			
			// wxj
			actiTitleLabel.enabled = actiDescLabel.enabled = false;
			NGUITools.SetActive(envPanel, false);
		}
		// wxj
		else if(objectiveList == "activity")
		{
			if(_data._conditionList[0]._type == ObjectiveType.Activity4)
			actiCollectLabel.text = Localization.SharedInstance.Get("Btn_Collect") + "  ("+ (_data._conditionList[0]._actiRewardedCount+1) +"/5)";
			
			titleLabelSysFont.enabled = true;
			actiTitleLabel.enabled = actiDescLabel.enabled = true;
			
			// Turn off reward value text if objective is marked as complete.
			titleLabelSysFont.enabled = !IsObjectiveMarkedComplete() || GameProfile.SharedInstance.Player.objectivesUnclaimed.Contains( _data._id );
			
			titleLabelSysFont.text = _data._rewardValue.ToString();
			actiTitleLabel.text = Localization.SharedInstance.Get(_data._title);
			actiDescLabel.GetComponent<UILocalize>().SetKey(_data._descriptionPreEarned);
			titleLabel.enabled = descLabel.enabled = false;
			descLabelSysFont.enabled = false;	
			
			
			// set envs activity
			if(_data._conditionList[0]._type == ObjectiveType.Activity4 || _data._conditionList[0]._type == ObjectiveType.Activity3)
			{
				NGUITools.SetActive(envPanel, false);
				Vector3 temp = actiDescLabel.transform.localPosition;
				temp.y = -66F;
				actiDescLabel.transform.localPosition = temp;
			}
			else
			{
				NGUITools.SetActive(envPanel, true);
				Vector3 temp = actiDescLabel.transform.localPosition;
				temp.y = -181F;
				actiDescLabel.transform.localPosition = temp;
				ConditionProtoData cd = _data._conditionList[0];
				
				int maxStat = 0;
				switch(cd._type)
				{
				case ObjectiveType.Activity1:
					maxStat = 1500;
					break;
				case ObjectiveType.Activity2:
					maxStat = 15;
					break;
				}
				
				for(int i = 0; i < envNumLabels.Length; i++)
				{
					if(!cd._actiEarnedStatForEnvs.ContainsKey(i + 1))
					{
						cd._actiEarnedStatForEnvs.Add(i + 1, 0);
					}
					envNumLabels[i].text = (int)cd._actiEarnedStatForEnvs[i + 1] + "/" + maxStat;
					if((int)cd._actiEarnedStatForEnvs[i + 1] >= maxStat)
					{
						envCompletedSprites[i].enabled = true;
					}
					else
					{
						envCompletedSprites[i].enabled = false;
					}
					
				}
				
				
			}
			
			
		}
		else
		{
			titleLabel.text = Localization.SharedInstance.Get(_data._title);
			descLabel.text = Localization.SharedInstance.Get(_data._descriptionPreEarned);	
		}		
	}	

	public bool IsCellAnimating { get; private set; }
	public void AnimateProgressBar(int newStatValue)
	{
		if (newStatValue != oldEarnedStatValue)		// play sound if animating
		{
			if(AudioManager.SharedInstance!=null)
			{
				//AudioManager.SharedInstance.PlayCoin();
				//AudioManager.Instance.SoundEffects.PlayOneShot(AudioManager.Instance.Recharged, 1.0f);
			}
		}
		
		IsCellAnimating = true;
		
		float tweenTime = (newStatValue == oldEarnedStatValue) ? 0.001f : 0.5f;
		float val =  Math.Min(1.0f, ((float)newStatValue / (float)_data._conditionList[0]._statValue));	
		iTween.ValueTo( progressBarNewEarnedFill.gameObject, iTween.Hash(
			"time", tweenTime,
			"from", progressBarNewEarnedFill.fillAmount,
			"to", val,
			"onupdate", "OnProgressBarUpdate",
			"onupdatetarget", gameObject,
			"oncomplete", "OnProgressBarComplete",
			"oncompletetarget", gameObject
			));
		
	}
	public void OnProgressBarComplete(){
		IsCellAnimating = false;
		//this is now controller in ObjectivesRoot
	//	UIManagerOz.SharedInstance.postGameVC.objectivesRoot.GetComponent<ObjectivesRoot>().CheckIfObjectiveCompleted();
	}
	
	public void OnProgressBarUpdate(float val){
		progressBarNewEarnedFill.fillAmount = val;
	}
	
	public void MakeTextCompleted()
	{
		//descLabel.text = "Completed!";
		descLabel.gameObject.GetComponent<UILocalize>().SetKey("Ttl_Sub_Completed");
	}	
	
	private void UpdateProgressBar( string objectiveType )
	{
		
		notify.Debug(  string.Format( "[ObjectiveCellData] UpdateProgressBar - for objective type: {0} with value: {1} and friend value: {2}",
				objectiveType,
				_data._conditionList[0]._earnedStatValue,
				_data._conditionList[0]._earnedNeighborValue
			)
		);
		
		if ( objectiveType == "team" )
		{
			UpdateProgressBar( (float) ( _data._conditionList[0]._earnedStatValue + _data._conditionList[0]._earnedNeighborValue ) );
		}
		else
		{
			UpdateProgressBar((float)_data._conditionList[0]._earnedStatValue);
		}
	}
	
	private void UpdateProgressBar(float overrideVal)	// just set it, without animation.  Both yellow & green bars are set to same value.
	{
		
		//if (progressBarFill == null)
		//	Start();
		//else
		//{
	//		Debug.Log("UpdateProgressBar " + gameObject.name + " " + (float)_data._conditionList[0]._earnedStatValue + " " +  (float)_data._conditionList[0]._statValue);
		if(progressBarFill!=null)
		{
			if(_data!=null && _data._conditionList!=null)
			{
				progressBarFill.fillAmount = Math.Min(1.0f, (overrideVal / (float)_data._conditionList[0]._statValue));
				
				if(IsObjectiveMarkedComplete())
					progressBarFill.fillAmount = 1f;
				
				if(progressBarNewEarnedFill)
					progressBarNewEarnedFill.fillAmount = progressBarFill.fillAmount;
			}
		}
		//}
		
		/*
		if (_data._conditionList[0]._statValue != 0)	// prevent divide by zero
		{
			float scaleX = 400.0f * Math.Min(1.0f, ((float)_data._conditionList[0]._earnedStatValue / (float)_data._conditionList[0]._statValue));	// capped at 100%
			progressBarFill.transform.localScale = new Vector3(scaleX, progressBarFill.transform.localScale.y, 1.0f);
			progressBarNewEarnedFill.transform.localScale = progressBarFill.transform.localScale;
		}
		*/
	}	
	
	public bool IsObjectiveCompleted()
	{
		if(_data==null || _data._conditionList==null)	return false;
		return (_data._conditionList[0]._earnedStatValue >= _data._conditionList[0]._statValue) ? true : false;
	}
	
	public bool IsObjectiveMarkedComplete()
	{
		if(_data==null || _data._conditionList==null)	return false;
		if(objectiveList=="legendary")	return GameProfile.SharedInstance.Player.legendaryObjectivesEarned.Contains(_data._id);
		if(objectiveList=="weekly")		return Services.Get<ObjectivesManager>().GetWeeklyObjectivesClass().GetEarnedWeeklyObjectiveList().Contains(_data._id);
		if (objectiveList == "team" ) return Services.Get<ObjectivesManager>().GetWeeklyObjectivesClass().EarnedTeamObjectiveList.Contains( _data._id );
		// wxj
		if (objectiveList == "activity" ) return _data._conditionList[0]._earnedStatValue >= _data._conditionList[0]._statValue;
		return GameProfile.SharedInstance.Player.objectivesEarned.Contains(_data._id);
	}
	
	public bool IsObjectiveNull()
	{
		return _data==null || _data._conditionList==null;
	}
	
	private void SetNotificationIcon()
	{
		if (notificationIcons == null)					// must be a post-run objective, no notification icons on those, so just exit
			return;
		
		bool enable = Services.Get<NotificationSystem>().GetNotificationStatusForThisCell(NotificationType.Challenge, _data._id);
		bool active = GameProfile.SharedInstance.Player.objectivesUnclaimed.Contains(_data._id);	// only show icon when 'collect' button is active	
		notificationIcons.SetNotification(0, (enable && active) ? 0 : -1);	
	}

	private void OnCollectButtonPressed()
	{
		// clear notification icon
		Services.Get<NotificationSystem>().ClearOneShotNotification(_data._id);
		
		// give reward here
		if (GameProfile.SharedInstance.Player.objectivesUnclaimed.Contains(_data._id))
		{
			GameProfile.SharedInstance.Player.objectivesUnclaimed.Remove(_data._id);
				
			//Try rewarding for weekly and legendary (there should be no ID overlap)
			Services.Get<ObjectivesManager>().GetWeeklyObjectivesClass().RewardObjective(_data._id);
			ObjectivesManager.RewardLegendaryObjective(_data._id);
			
			// wxj, raward activity objectives
			ObjectivesManager.RewardDailyObjective(_data._id);
			
			GameProfile.SharedInstance.Serialize();	
			
			SetData(_data, objectiveList);	// refresh cell
			
			ShowRewardDialog();
			
			// wxj, update UIPagerView currency
			UIManagerOz.SharedInstance.PaperVC.UpdateCurrency();
		}
	}
	
	private void ShowRewardDialog()
	{
		string rewardText = Localization.SharedInstance.Get("Lbl_LevelReward");

        string rewardIconSpriteName = ObjectivesManager.GetRewardIconSpriteName( (int) _data._rewardType );
		
		// wxj
		if (objectiveList == "weekly" || objectiveList == "team" || objectiveList == "activity")
		{
			//UIManagerOz.SharedInstance.rewardDialog.ShowRewardDialog(rewardText + "\n" + _data._title, rewardIcon.spriteName, "Btn_Ok");
			
			UIManagerOz.SharedInstance.rewardDialog.ShowRewardDialog( rewardText + "\n" + Localization.SharedInstance.Get(_data._title), rewardIconSpriteName, "Btn_Ok" );
			
			notify.Debug( "[ObjectiveCellData] ShowRewardDialog: " + rewardIcon.spriteName );
			
			//UIManagerOz.SharedInstance.okayDialog.ShowOkayDialog(rewardText + "\n" + _data._title, "Btn_Ok");
		}
		else if (objectiveList == "legendary")		
		{
			string multiplierText = Localization.SharedInstance.Get("Lbl_Mulitplier");
			//UIManagerOz.SharedInstance.rewardDialog.ShowRewardDialog(rewardText + "\n" + "1 " + multiplierText, rewardIcon.spriteName, "Btn_Ok");
			UIManagerOz.SharedInstance.rewardDialog.ShowRewardDialog( rewardText + "\n" + "1 " + multiplierText, rewardIconSpriteName, "Btn_Ok" );
			//UIManagerOz.SharedInstance.okayDialog.ShowOkayDialog(rewardText + "\n" + "1 " + multiplierText, "Btn_Ok");			
		}
		
		AudioManager.SharedInstance.PlayFX(AudioManager.Effects.MusicBox);
		Services.Get<NotificationSystem>().SetNotificationIconsForThisPage(UiScreenName.OBJECTIVES);
	}
}
