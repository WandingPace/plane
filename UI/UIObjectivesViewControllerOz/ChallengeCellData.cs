//using UnityEngine;
//using System;
//using System.Collections;
//
//public class ChallengeCellData : MonoBehaviour 
//{
//	public UISprite progressBarFill;				// green
//	public UISprite progressBarNewEarnedFill;		// yellow
//	public UISprite icon, rewardIcon, checkedIcon;
//	public UILabel titleLabel, descLabel, statProgressLabel;
//	
//	private ObjectiveProtoData _data;	
//	public int oldEarnedStatValue = 0;
//	
//	private NotificationSystem notificationSystem;
//	private NotificationIcons notificationIcons;	
//		
//	void Awake()
//	{
//		notificationIcons = gameObject.GetComponent<NotificationIcons>();		
//	}
//	
//	void Start()
//	{
//		Destroy(gameObject.GetComponent<UIPanel>());	// kill auto-attached UIPanel component
//		
//		titleLabel = gameObject.transform.Find("CellContents/FontAnchor/LabelTitle01").GetComponent<UILabel>();		
//		descLabel = gameObject.transform.Find("CellContents/FontAnchor/LabelDescription01").GetComponent<UILabel>();
//		//statProgressLabel = gameObject.transform.Find("LabelStatProgress").GetComponent<UILabel>();	
//		
//		progressBarFill =  transform.Find("CellContents/GraphicsAnchor/SlicedSpriteFill01").GetComponent<UISprite>();
//		Transform fillNew = transform.Find("CellContents/GraphicsAnchor/SlicedSpriteFill02") ;
//		
//		if (fillNew)
//			progressBarNewEarnedFill = fillNew.GetComponent<UISprite>();	
//		
//		notificationSystem = Services.Get<NotificationSystem>();
//	}
//	
//	public void SetData(ObjectiveProtoData data)
//	{
//		_data = data;
//		
//		if (_data != null && _data._conditionList!=null)
//		{
//			oldEarnedStatValue = _data._conditionList[0]._earnedStatValue;		// back up old _earnedStatValue, if exists
//		
//			UpdateLabelText();			// populate text fields	
//			UpdateProgressBar();	
//			UpdateIcons();
//			SetNotificationIcon();		// show notification icon if appropriate
//		}
//	}
//	
//	private void UpdateIcons()		// set icon sprite
//	{
//		if(_data==null)
//		{
//			icon.spriteName = "";
//			return;
//		}
//		
//		string[] iconNames = new string[9] 
//		{ 
//			"icon_obj_class_coin", 			// Coin
//			"icon_obj_class_collection", 	// Collection
//			"icon_obj_class_discovery",		// Discovery
//			"icon_obj_class_distance",		// Distance
//			"icon_obj_class_legendary",		// Lifetime (Legendary?)
//			"icon_obj_class_obstacle",		// Obstacles
//			"icon_obj_class_legendary",		// Purchases (Weekly?)
//			"icon_obj_class_score",			// Score
//			"icon_obj_class_skill",			// Skill
//		};
//		
//		string[] rewardIconNames = new string[8]
//		{ 
//			"coinbundle_01", 				// Coins
//			"gembundle_01", 				// Gems
//			"icon_multiplier1",				// Multipliers
//			"icon_thirdeye",				// HeadStartConsumables
//			"multiplier3",					// ExtraMultiplierConsumables
//			"icon_stumbleproof",			// NoStumbleConsumables
//			"icon_headstart",				// ThirdEyeConsumables
//			"icon_megaheadstart",			// MegaHeadStartConsumables
//		};
//		
//		if (icon != null)
//			icon.spriteName = iconNames[(int)_data._category];
//		
//		if (rewardIcon != null)
//			rewardIcon.spriteName = rewardIconNames[(int)_data._rewardType];
//		
//		if (checkedIcon != null)
//			checkedIcon.enabled = IsObjectiveCompleted();		
//	}		
//	
//	private void UpdateLabelText()	// populate text fields
//	{
//		if (titleLabel == null)
//			Start();
//		
//		if(_data==null)	return;
//		
//		titleLabel.gameObject.GetComponent<UILocalize>().SetKey(_data._title);					//titleLabel.text = _data._title;	
//		descLabel.gameObject.GetComponent<UILocalize>().SetKey(_data._descriptionPreEarned);	//descLabel.text = _data._descriptionPreEarned;	// " (val=" + _objectiveData._statValue + ")";
//		//statProgressLabel.text = "(earned: " + _data._conditionList[0]._earnedStatValue + ", need: " + _data._conditionList[0]._statValue + ")"; 
//	}	
//	
//	/*
//	public TweenScale AnimateProgressBar(int newStatValue)
//	{
//		if (newStatValue != oldEarnedStatValue)		// play sound if animating
//		{
//			if(AudioManager.SharedInstance!=null)	AudioManager.Instance.SoundEffects.PlayOneShot(AudioManager.Instance.Recharged, 1.0f);
//		}
//		
//		//float tweenTime = 0.0001f + 2.0f * (float)(newStatValue - oldEarnedStatValue) / (float)_data._statValue;
//		float tweenTime = (newStatValue == oldEarnedStatValue) ? 0.001f : 1.0f;
//		float scaleX = 400.0f * Math.Min(1.0f, ((float)newStatValue / (float)_data._conditionList[0]._statValue));	// capped at 100%
//		TweenScale tween = TweenScale.Begin(progressBarNewEarnedFill, tweenTime, 
//			new Vector3(scaleX, progressBarNewEarnedFill.transform.localScale.y, 1.0f));
//		return tween;
//	}
//	*/
//	public bool IsCellAnimating { get; private set; }
//	public void AnimateProgressBar(int newStatValue)
//	{
//		if (newStatValue != oldEarnedStatValue)		// play sound if animating
//		{
//			if(AudioManager.SharedInstance!=null)	
//			{
//				//AudioManager.SharedInstance.PlayCoin();
//				AudioManager.Instance.GetSoundEffectsPlayer().PlayOneShot(AudioManager.Instance.Recharged, 1.0f);
//			}
//		}
//		
//		IsCellAnimating = true;
//		
//		float tweenTime = (newStatValue == oldEarnedStatValue) ? 0.001f : 0.5f;
//		float val =  Math.Min(1.0f, ((float)newStatValue / (float)_data._conditionList[0]._statValue));	
//		iTween.ValueTo( progressBarNewEarnedFill.gameObject, iTween.Hash(
//			"time", tweenTime,
//			"from", progressBarNewEarnedFill.fillAmount,
//			"to", val,
//			"onupdate", "OnProgressBarUpdate",
//			"onupdatetarget", gameObject,
//			"oncomplete", "OnProgressBarComplete",
//			"oncompletetarget", gameObject
//			));
//		
//	}
//	public void OnProgressBarComplete(){
//		IsCellAnimating = false;
//		//this is now controller in ObjectivesRoot
//	//	UIManagerOz.SharedInstance.postGameVC.objectivesRoot.GetComponent<ObjectivesRoot>().CheckIfObjectiveCompleted();
//	}
//	
//	public void OnProgressBarUpdate(float val){
//		progressBarNewEarnedFill.fillAmount = val;
//	}
//	
//	public void MakeTextCompleted()
//	{
//		//descLabel.text = "Completed!";
//		descLabel.gameObject.GetComponent<UILocalize>().SetKey("Ttl_Sub_Completed");
//	}	
//	
//	private void UpdateProgressBar()	// just set it, without animation.  Both yellow & green bars are set to same value.
//	{
//		
//		if (progressBarFill == null)
//			Start();
//		//else
//		//{
//	//		Debug.Log("UpdateProgressBar " + gameObject.name + " " + (float)_data._conditionList[0]._earnedStatValue + " " +  (float)_data._conditionList[0]._statValue);
//		if(progressBarFill!=null)
//		{
//			if(_data!=null && _data._conditionList!=null)
//			{
//				progressBarFill.fillAmount = Math.Min(1.0f, ((float)_data._conditionList[0]._earnedStatValue / (float)_data._conditionList[0]._statValue));
//				if(progressBarNewEarnedFill)
//					progressBarNewEarnedFill.fillAmount = progressBarFill.fillAmount;
//			}
//		}
//		//}
//		
//		/*
//		if (_data._conditionList[0]._statValue != 0)	// prevent divide by zero
//		{
//			float scaleX = 400.0f * Math.Min(1.0f, ((float)_data._conditionList[0]._earnedStatValue / (float)_data._conditionList[0]._statValue));	// capped at 100%
//			progressBarFill.transform.localScale = new Vector3(scaleX, progressBarFill.transform.localScale.y, 1.0f);
//			progressBarNewEarnedFill.transform.localScale = progressBarFill.transform.localScale;
//		}
//		*/
//	}	
//	
//	public bool IsObjectiveCompleted()
//	{
//		if(_data==null || _data._conditionList==null)	return false;
//		return (_data._conditionList[0]._earnedStatValue >= _data._conditionList[0]._statValue) ? true : false;
//	}
//	
//	public bool IsObjectiveNull()
//	{
//		return _data==null || _data._conditionList==null;
//	}
//	
//	private void SetNotificationIcon()
//	{
//		if (notificationIcons == null)		// must be a post-run objective, no notification icons on those, so just exit
//			return;
//		
//		if (notificationSystem == null)
//			notificationSystem = Services.Get<NotificationSystem>();
//		
//		bool enable = false;	//notificationSystem.GetNotificationStatusForThisCell(NotificationType.Powerup, _data._id);
//		notificationIcons.SetNotification(0, (enable) ? 0 : -1);
//	}		
//	
//	//void Update() { }
//}
//
//
//		// hack to work around NGUI scroller collider situation, which was preventing 'back' and 'play' buttons from working on objectives screen
//		//if (gameObject.collider != null)
//		//{
//		//	gameObject.collider.enabled = (gameObject.transform.position.y < -3.3f) ? false : true;
//		//}
//			
//			//progressBarFill.transform.localScale = new Vector3(400.0f * ((float)_data._earnedStatValue / (float)_data._statValue),
//			//	progressBarFill.transform.localScale.y, progressBarFill.transform.localScale.z);
//
//
//		//progressBarFill.GetComponent<UISprite>().color = new Color(1.0f, 1.0f, 0.0f, 1.0f);
//
//			//progressBarFill.GetComponent<UISprite>().color = new Color(23.0f/255.0f, 115.0f/255.0f, 98.0f/255.0f, 255.0f/255.0f);
//			
//	
////			if (gameObject.transform.position.y < -3.3f) 
////			{ 
////				gameObject.collider.enabled = false;
////			}
////			else { gameObject.collider.enabled = true; }