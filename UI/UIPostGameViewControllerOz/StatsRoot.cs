using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class StatsRoot : MonoBehaviour 
{	
	public UIPostGameViewControllerOz postGameVC = null;	
	
	public Transform camXform;
	
	public UILabel DistanceLabel;
	public UILabel CoinLabel;
	public UILabel GemLabel;
	public UILabel MultiplierLabel;	
	public UILabel ScoreLabel;	
	
	public UILabel newHighScore;
	public UILabel longestRun;
	
	public UISprite iconGem;
	public UISprite iconDist;
	public UISprite iconMult;
	public UISprite iconCoins;
	
	public UISlider scoreSlider;
	public UISlider distSlider;
	
	private bool bestRun = false;
	private float bestRunRatio = 0f;
	private bool bestScore = false;
	private float bestScoreRatio = 0f;
	
    //private Vector3 longestRunScale;
	private Vector3 distSliderScale;
	private Vector3 iconGemScale;
	private Vector3 iconDistScale;
	private Vector3 iconMultScale;
	private Vector3 iconCoinsScale;
	
	
    //private Vector3 newHighScoreScale;
	private Vector3 scoreSliderScale;
	
	private float scoreLastTime;
	
	private int id = 0;
	
	public GameObject AdditionalMultiplier;	
	
	//public UISprite deathPortraitSprite;
	//public UITexture deathPortraitTexture;
	public UILabel deathTipLabel;
	
	private int downloadPromptInterval = 2;
	
	private bool firstLoad = true;
	
	private string meterLoc;
	
	[HideInInspector]
	public static bool playAnimations = true;
	
	void Start() 
	{ 
	}

	public void EnterStatsPage()	
	{
		ParticleSystem[] fx = ScoreLabel.transform.GetComponentsInChildren<ParticleSystem>();
		foreach (ParticleSystem item in fx)
		{
			if(item && Time.timeScale < 1.2f){
				item.Stop(true);
				item.Clear(true);				
			}
		}			
		
		//DistanceLabel.text = ((int)GameController.SharedInstance.DistanceTraveled).ToString() + "m";
		DistanceLabel.text = ((int)GameController.SharedInstance.DistanceTraveled).ToString()+ Localization.SharedInstance.Get ("Lbl_MeterAb");
		ScoreLabel.text = GamePlayer.SharedInstance.Score.ToString();
		CoinLabel.text = GamePlayer.SharedInstance.CoinCountTotal.ToString();
		GemLabel.text = GamePlayer.SharedInstance.GemCountTotal.ToString();
		//Add in challenge multipler to the display.
		
        int temp = GameProfile.SharedInstance.GetTotalScoreMultiplier();
        MultiplierLabel.text = (GameProfile.SharedInstance.GetTotalScoreMultiplier() + temp).ToString() + "x";
		//if(temp>0)
		//	MultiplierLabel.text = GameProfile.SharedInstance.GetPermanentScoreMultiplier() + "x (+" + temp + "x)";	//GetScoreMultiplier
		//else
		//	MultiplierLabel.text = GameProfile.SharedInstance.GetPermanentScoreMultiplier() + "x";	//GetScoreMultiplier
		
		
		
	//	if (GameProfile.SharedInstance.AdditionalScoreMultiplier != 0)
	//		AdditionalMultiplier.active = true;
	//	else
	//		AdditionalMultiplier.active = false;
		AdditionalMultiplier.SetActive(false); 
	}
	
    public void Reset(){
        if(firstLoad){
			firstLoad = false;
            //longestRunScale = Vector3.one;//longestRun.transform.localScale;
            distSliderScale = Vector3.one;//distSlider.transform.localScale;
            //newHighScoreScale = Vector3.one;//newHighScore.transform.localScale;
            scoreSliderScale = Vector3.one;//scoreSlider.transform.localScale;
            iconGemScale = Vector3.one;//iconGem.transform.localScale;
            iconDistScale = Vector3.one;//iconDist.transform.localScale;
            iconMultScale = Vector3.one;//iconMult.transform.localScale;
            iconCoinsScale = Vector3.one;//iconCoins.transform.localScale;
		}
		EnterStatsPage();
		bestRun = false;
		bestScore = false;
		//DistanceLabel.enabled = false;
		CoinLabel.enabled = false;
		GemLabel.enabled = false;
		MultiplierLabel.enabled = false;
      
		longestRun.enabled = false;
		newHighScore.enabled = false;
      

		iconGem.transform.localScale = iconGemScale;
		iconDist.transform.localScale = iconDistScale;
		iconMult.transform.localScale = iconMultScale;
		iconCoins.transform.localScale = iconCoinsScale;
		
				
		distSlider.transform.localScale = distSliderScale;
        longestRun.transform.localScale = Vector3.one;//longestRunScale;
		distSlider.value = 0f;
		
        newHighScore.transform.localScale = Vector3.one;//newHighScoreScale;
		scoreSlider.transform.localScale = scoreSliderScale;
		scoreSlider.value = 0f;
		
		iconCoins.alpha = iconGem.alpha = iconMult.alpha = 0f;
		
		meterLoc = Localization.SharedInstance.Get ("Lbl_MeterAb");
		
		//ScoreLabel.MakePixelPerfect();
        ScoreLabel.transform.localScale = Vector3.one;
		ScoreLabel.text = "0";
		//DistanceLabel.MakePixelPerfect();
        DistanceLabel.transform.localScale  = Vector3.one;
		DistanceLabel.text = "0" + Localization.SharedInstance.Get ("Lbl_MeterAb");
		gameObject.SetActive(true);
		// disable this for now
		AdditionalMultiplier.SetActive(false);
		
	}
	
	private void KillAnimation(){
		iTween.Stop(gameObject, true);
	}
	
	
	public void StartAnimSeqWithDelay(float time){
		Invoke("StartAnimSeq", time);
	}
	
	public void StartAnimSeq(){
		Reset();
		// figuring out longest ratio ratio
		int bestRunDist = GameProfile.SharedInstance.Player.bestDistanceScore;
		if(bestRunDist == 0){
			bestRunDist = 10;
		}
		bestRunRatio = (float)GameController.SharedInstance.DistanceTraveled / (float)bestRunDist;
		if(bestRunRatio >= 1f){
			bestRun = true;
			bestRunRatio = 1f;
		}
		
		// figuring out best score ratio
		int bestScorePoints = GameProfile.SharedInstance.Player.bestScore;
		if(bestScorePoints == 0){
			bestScorePoints = 10;
		}
		bestScoreRatio = (float)GamePlayer.SharedInstance.Score / (float)bestScorePoints;
		if(bestScoreRatio >= 1f){
			bestScore = true;
			bestScoreRatio = 1f;
		}
		

		id = 1;
		SlideInScore(id); // start the animation with first item
		
		
	}
	
	public void SlideInScore(int id){
		if(id >= 4) {	
			scoreLastTime = Time.time;
			//AnimateScore();
			AnimateDist();
			return;
		}
		UISprite icon = null;
		UILabel label = null;
		switch(id){
			case 0: icon = iconDist;
					label = DistanceLabel;
					break;
			case 1: icon = iconCoins;
					label = CoinLabel;
					break;
			case 2: icon = iconGem;
					label = GemLabel;
					break;
			case 3: icon = iconMult;
					label = MultiplierLabel;
					break;
		}
		
		Hashtable param = new Hashtable();
		param.Add("icon", icon);
		param.Add("label", label);
		
		icon.transform.localScale *= 8f;
		iTween.ScaleTo(icon.gameObject, iTween.Hash(
			"scale", icon.transform.localScale * 0.125f,
			"time", 0.1f,
			"delay", 0.1f,
			"easetype", iTween.EaseType.easeOutExpo,
			"onstart", "OnPunchStart",
			"onstarttarget", gameObject,
			"oncomplete", "OnPunchComplete",
			"oncompletetarget", gameObject,
			"oncompleteparams", icon.gameObject
			));
		TweenAlpha ta = TweenAlpha.Begin(icon.gameObject, 0.2f, 1f);
		ta.delay = 0.2f;
		
		label.enabled = true;
		iTween.MoveFrom(label.gameObject, iTween.Hash(
			"position", new Vector3(600f, label.transform.localPosition.y, 0f),
			"isLocal", true,
			"time", 0.1f,
			"delay", 0.05f,
			"easetype", iTween.EaseType.easeOutExpo,
			"onstart", "OnSlideStart",
			"onstarttarget", gameObject,
			"oncomplete", "OnSlideInComplete",
			"oncompletetarget", gameObject,
			"oncompleteparams", param
			));
		
		
	}
	
	public void OnSlideStart(){
		AudioManager.SharedInstance.PlayFX(AudioManager.Effects.oz_UI_meterFull01);
		//Debug.Log ("Swish");
	}
	public void OnPunchStart(){
	}
	public void OnPunchComplete(GameObject icon){

		AudioManager.SharedInstance.PlayFX(AudioManager.Effects.oz_UI_scoreTally_fireworks_01);
		iTween.ShakePosition(gameObject, iTween.Hash(
			"amount", new Vector3(0.02f,0.02f,0f),
			"time", 0.1f,
			"isLocal", false
			));
		ParticleSystem fx = icon.transform.GetComponentInChildren<ParticleSystem>();
		if(fx && Time.timeScale < 1.2f){
			fx.Play(true);
		}
	}
	
	
	
	public void OnSlideInComplete(object vals){

		id++;
		SlideInScore(id);
		
	}
	

	public void AnimateDist(){
		iTween.ValueTo(gameObject, iTween.Hash(
			"time", 0.5f,
			"from", 0f,
			"to", 1f,
			"onupdate", "AnimateDistUpdate",
			"onupdatetarget", gameObject
			));
	}
	
	public void AnimateDistUpdate(float val){
		if(Time.time - scoreLastTime > 0.06f){
			AudioManager.SharedInstance.PlayCoin();
			//AudioManager.SharedInstance.PlayFX(AudioManager.Effects.oz_UI_scoreTallyBar);
			scoreLastTime = Time.time;
		}
		
		
		distSlider.value = bestRunRatio * val;
		DistanceLabel.text = ((int)(val * GameController.SharedInstance.DistanceTraveled)).ToString() + meterLoc;
		if(val >= 1f){
//			ParticleSystem fx = DistanceLabel.transform.GetComponentInChildren<ParticleSystem>();
//			if(fx && Time.timeScale < 1.2f){
//				fx.Play(true);
//			}
			
			if(bestRun){	
				ParticleSystem fx = DistanceLabel.transform.GetComponentInChildren<ParticleSystem>();
				if(fx && Time.timeScale < 1.2f){
					fx.Play(true);
				}	
			}
			
			AudioManager.SharedInstance.PlayFX(AudioManager.Effects.oz_UI_scoreTally_fireworks_02);
			iTween.ScaleTo(DistanceLabel.gameObject, iTween.Hash(
				"time", 0.3f,
				"scale",new Vector3(1.4f,1.4f,1.4f),
				"easetype", iTween.EaseType.easeOutBack,
				"oncomplete", "ScaleUpDistComplete",
				"oncompletetarget", gameObject
				));
           
			
		}
	}
	
	
	public void ScaleUpDistComplete(){

		if(bestRun){
			ShowLongestRun();
		}
		else{
			AnimateScore();
		}
	}
	
	public void ShowLongestRun(){		
		iTween.ScaleTo(distSlider.gameObject, iTween.Hash(
			"time", 0.3f,
			"scale", new Vector3(1f,0f,1f),
			"easetype", iTween.EaseType.easeInBack
			));
		longestRun.enabled = true;
		iTween.ScaleFrom(longestRun.gameObject, iTween.Hash(
			"time", 0.3f,
            "scale", new Vector3(2f,2f,2f),
			"oncomplete", "ShowLongestRunComplete",
			"oncompletetarget", gameObject
			));
		longestRun.alpha = 0f;
		TweenAlpha.Begin(longestRun.gameObject, 0.3f, 1f);
	}
	
	public void ShowLongestRunComplete(){
		AnimateScore();
	}
	
	public void AnimateScore(){
		iTween.ValueTo(gameObject, iTween.Hash(
			"time", 0.5f,
			"from", 0f,
			"to", 1f,
			"onupdate", "AnimateScoreUpdate",
			"onupdatetarget", gameObject
			));
	}
	
	
	public void AnimateScoreUpdate(float val){
		if(Time.time - scoreLastTime > 0.06f){
			AudioManager.SharedInstance.PlayCoin();
			scoreLastTime = Time.time;
		}
		int score = (int)(GamePlayer.SharedInstance.Score * val);
		ScoreLabel.text = score.ToString();
		scoreSlider.value = bestScoreRatio * val;
		if(val >= 1){
			if(bestScore){		
				ParticleSystem[] fx = ScoreLabel.transform.GetComponentsInChildren<ParticleSystem>();
				foreach (ParticleSystem item in fx)
				{
					if(item && Time.timeScale < 1.2f){
						item.Play(true);
					}
				}
			}
			
			AudioManager.SharedInstance.PlayFX(AudioManager.Effects.oz_UI_scoreTally_fireworks_03);
			iTween.ScaleTo(ScoreLabel.gameObject, iTween.Hash(
				"time", 0.3f,
                "scale", new Vector3(1.3f,1.3f,1.3f),
				"easetype", iTween.EaseType.easeOutBack,
				"oncomplete", "ScaleUpScoreComplete",
				"oncompletetarget", gameObject
				));	
				
			iTween.ShakePosition(gameObject, iTween.Hash(
				"amount", new Vector3(0.02f,0.02f,0f),
				"time", 0.2f,
				"isLocal", false
				));
		}
	}
	
	public void ScaleUpScoreComplete(){

		if(bestScore){
			ShowNewHighScore();
		}
		else{
			StatsAnimationComplete();
		}
	}
	
	public void ShowNewHighScore(){
		iTween.ScaleTo(scoreSlider.gameObject, iTween.Hash(
			"time", 0.3f,
			"scale", new Vector3(1f,0f,1f),
			"easetype", iTween.EaseType.easeInBack
			));
		newHighScore.enabled = true;
		iTween.ScaleFrom(newHighScore.gameObject, iTween.Hash(
			"time", 0.3f,
            "scale",new Vector3(1.4f,1.4f,1.4f),
			"oncomplete", "ShowNewHighScoreComplete",
			"oncompletetarget", gameObject
			));
		newHighScore.alpha = 0f;
		TweenAlpha.Begin(newHighScore.gameObject, 0.3f, 1f);
	}
	
	public void ShowNewHighScoreComplete()
	{
		StatsAnimationComplete();
	}
	
	private void StatsAnimationComplete()
	{
		//Invoke ("AllAnimEnded", 2.0f); // wait for partcle effects to end
		AllAnimEnded();
	}
	
	void AllAnimEnded()
	{
		Time.timeScale = 1f;
		UIManagerOz.SharedInstance.postGameVC.speedUpButton.collider.enabled = false;
		
		// Show Burstly post-game interstitial
		//		SharingManagerBinding.ShowBurstlyInterstitial( "post_game" );
				
		if(!AudioManager.SharedInstance.IsMenuMusicPlaying()) {
			AudioManager.SharedInstance.SwitchToMainMenuMusic(4f);
			AudioManager.SharedInstance.FadeMusicMultiplier(0f,0.6f);
		}
		
		// --- Analytics ------------------------------------------------------
        //bool isStartOfSession = false;
//		AnalyticsInterface.LogPlayerInfoEvent( isStartOfSession );
//		AnalyticsInterface.LogBalloonCollectiblesEvents();
//		AnalyticsInterface.LogConsumableEvents();
		// --------------------------------------------------------------------
		
		GameController.SharedInstance.UpdateAndSaveRecords();
		//Final place
		
		ProfileManager.SharedInstance.UpdateProfile();
		
		bool leaderboardTest = Settings.GetBool( "Leaderboard-Test", false );
		
		if ( leaderboardTest )
		{
			Initializer.SharedInstance.GetInitFromServer();
		}
				
		// refresh local leaderboard positions and notifications, in case player's run changed their ranks in score and/or distance
		LeaderboardManager leaderboardManager = Services.Get<LeaderboardManager>();
		leaderboardManager.RefreshLeaderboards();
		leaderboardManager.GetTopDistances(UIManagerOz.SharedInstance.leaderboardVC.GetkLeaderboardLabelCount(), true);
		leaderboardManager.GetTopScores(UIManagerOz.SharedInstance.leaderboardVC.GetkLeaderboardLabelCount(), true);
		
		//------- GameCenter -----------------------------

		#if UNITY_IPHONE
//			GameCenterBinding.reportScore((System.Int64)GamePlayer.SharedInstance.Score, GameController.Leaderboard_HighScores);
//			GameCenterBinding.reportScore((System.Int64)((int)GameController.SharedInstance.DistanceTraveled), GameController.Leaderboard_DistanceRun);
		
//			foreach(string obj in ObjectivesDataUpdater.objectivesQueue){
//				GameCenterBinding.reportAchievement(obj , 100.0f);
//			}
		#elif UNITY_ANDROID
				//TODO add GameCircle here
		#endif
		//------------------------------------------------------
		
		// DMOAnalytics
		_SendObjectivesToAnalytics();
		
		
		// Publish scores and achievements to Facebook
        //int facebookHighScore = 0;
        //if ( bestScore == true )
        //{
        //    facebookHighScore = GamePlayer.SharedInstance.Score;
        //}
		//		SharingManagerBinding.PublishFacebookScoresAndAchievements( facebookHighScore, ObjectivesDataUpdater.objectivesQueue );
		
		
		ObjectivesDataUpdater.objectivesQueue.Clear();
		
		
		UIManagerOz.SharedInstance.postGameVC.CanSwitchPages(); // we are done animating so let the player switch pages

		int currank = GameProfile.SharedInstance.Player.GetCurrentRank();
		int rankdiff =  currank - UIManagerOz.lastPromptedLevel;
//			Debug.LogError ("old Rank = " + UIManagerOz.lastPromptedLevel);
		if( rankdiff > 0 )
		{
//			Debug.LogError ("New Rank = " + currank);
			PlayerPrefs.SetInt("LastPromptedLevel", currank);
			UIManagerOz.lastPromptedLevel = currank;
		}

		if( GameController.SharedInstance.IsSafeToLaunchDownloadDialog() )
		{
			// Prompt sequence will also call the server after five minutes have passed
			if( rankdiff > 0 && (currank-2) % downloadPromptInterval == 0 )
			{
				UIManagerOz.SharedInstance.StartDownloadPrompts(true);
			}
			// We always want to check for server data after five minutes have passed.
			else if ( Initializer.SharedInstance.IsTimeToGetInit() )
			{
				Initializer.SharedInstance.GetInitFromServer();
			}
		}
		else
		{
//			Debug.LogError("Not Safe");
		}
		
		UIManagerOz.SharedInstance.postGameVC.ShowBottomPanel();		
	}
	
	private void _SendObjectivesToAnalytics()
	{
		// Send all the completed objectives to DMOAnalytics (not just the points-paying ones)
		List<string> objectivesEarnedDuringRun = GameProfile.SharedInstance.Player.objectivesEarnedDuringRun;

		for ( int objectiveIndex = 0; objectiveIndex < objectivesEarnedDuringRun.Count; objectiveIndex ++ )
		{
			//			AnalyticsInterface.LogGameAction( "objective", "achieved", objectivesEarnedDuringRun[objectiveIndex], "", 0 );
		}
		//	AnalyticsInterface.LogGameAction( "objective", "achieved", objectivesEarnedDuringRun.Count.ToString(), GameProfile.GetAreaCharacterString(), 0 );
	}
	
    /// <summary>
    /// …Ë÷√À¿ÕˆÕºœÒ
    /// </summary>
    /// <param name="dt"></param>
	public void SetDeathPortrait(DeathTypes dt)
	{		
		switch(dt)
		{
			case DeathTypes.Fall:
				//deathPortrait.spriteName = "Oz_DS_falling_color_r01.jpg";
				gameObject.GetComponent<CharacterDeathImageSwapper>().SetImage("falling");
				break;
			case DeathTypes.Baboon:
				//deathPortrait.spriteName = "Oz_DS_baboon_color_r01.jpg";
				gameObject.GetComponent<CharacterDeathImageSwapper>().SetImage("baboon");
				break;
			default:
				//deathPortrait.spriteName = "Oz_DS_stunned_color_r01.jpg";
				gameObject.GetComponent<CharacterDeathImageSwapper>().SetImage("stunned");
				break;
		}
		
        //int rand = 1 + (int)(Random.value * 38.9999f);
        //string msg = "Lbl_Tips_" + rand.ToString();
		//Debug.Log ("death msg " + rand + " local " + msg);
//		deathTipLabel.text = Localization.SharedInstance.Get(msg);
	}
	
	public void KillParticleEffect()
	{
		ParticleSystem[] fx = ScoreLabel.transform.GetComponentsInChildren<ParticleSystem>();
		
		foreach (ParticleSystem item in fx)
		{
			if (item)
			{
				item.Stop(true);
				item.Clear(true);				
			}
		}		
	}
}




//	
//	public void SetLabel(UILabel label, int val) 
//	{
//		label.text = val.ToString();
//	}	


//
//	public void EnterStatsPage()	
//	{
//		SetDistance((int)GameController.SharedInstance.DistanceTraveled);
//		SetScore(GamePlayer.SharedInstance.Score);
//		SetCoinScore(GamePlayer.SharedInstance.CoinCountTotal);
//		SetGemScore(GamePlayer.SharedInstance.GemCountTotal);			
//		SetScoreMultiplier(GameProfile.SharedInstance.GetScoreMultiplier());	
//	}
//	

//	
//	public void SetDistance(int distance) 
//	{
//		if (DistanceLabel == null) { return; }
//		DistanceLabel.text = distance.ToString();
//	}
//	
//	public void SetScore(int score) 
//	{
//		if (ScoreLabel == null) { return; }
//		ScoreLabel.text = score.ToString();
//	}
//	
//	public void SetCoinScore(int score) 
//	{
//		if (CoinLabel == null) { return; }
//		CoinLabel.text = score.ToString();
//	}
//	
//	public void SetGemScore(int score) 
//	{
//		if (GemLabel == null) { return; }
//		GemLabel.text = score.ToString();
//	}		
//	
//	public void SetScoreMultiplier(int score)
//	{
//		if (MultiplierLabel == null) { return; }
//		MultiplierLabel.text = score.ToString();
//	}	




	//public UILabel DeathMessageLabel = null;
	//public UISprite DeathPortrait = null;
		
	

//		
//			//-- pick a random death message
//			postGameVC.DeathMessageLabel.text = "When you don't succeed, try, try again!";
//			postGameVC.DeathPortrait.spriteName = "Eaten";
//	
//			ProtoDeathMessage dm = DeathMessage.GetRandomMessageForDeathType(GamePlayer.SharedInstance.DeathType);
//			if (dm != null && dm.messageChoices != null && dm.messageChoices.Count > 0) 
//			{
//				if(postGameVC.DeathPortrait.atlas.GetSprite(dm.spriteName) != null) 
//				{
//					postGameVC.DeathMessageLabel.text = dm.messageChoices[UnityEngine.Random.Range(0, dm.messageChoices.Count)];	
//					postGameVC.DeathPortrait.spriteName = dm.spriteName;
//				}
//				else 
//				{
//					string characterSpriteName = dm.getCharacterSpriteName(GameProfile.SharedInstance.GetActiveCharacter().characterId);
//					if(postGameVC.DeathPortrait.atlas.GetSprite(characterSpriteName) != null) 
//					{
//						postGameVC.DeathMessageLabel.text = dm.getRandomCharacterMessage(GameProfile.SharedInstance.GetActiveCharacter().characterId);	
//						postGameVC.DeathPortrait.spriteName = characterSpriteName;
//					}
//				}
//			}		
//		