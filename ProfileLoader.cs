using UnityEngine;
using System.Collections;

public class ProfileLoader : MonoBehaviour
{
	/*
	 * BWA Removed functionality of profile loader and placed it all into Profile Manager.
	 * 
	public static ProfileLoader SharedInstance;
	
	protected static Notify notify;
	

	private GameObject msgObject = null;
	
	void Awake ()
	{
		SharedInstance = this;
		notify = new Notify(this.GetType().Name);
		//ProfileManager.SharedInstance.RegisterForOnlineProfileLoaded(OnProfile);
	}
	
	// Use this for initialization
	void Start ()
	{
		notify.Debug("Profile Loader initialized");

		
	}
	
	/*
	// Update is called once per frame
	void Update ()
	{
	
	}
	
	public void CheckProfile(GameObject messageObject)
	{
		//Check profile
		ProfileManager.SharedInstance.RegisterForOnlineProfileLoaded(OnProfile);
		notify.Debug("Profile Loader - Check Profile");
		msgObject = messageObject;
		
		if (ProfileManager.SharedInstance != null)
		{
			notify.Debug("Profile Loader - Trigger Event");
			ProfileManager.SharedInstance.TriggerEvent();
		}
		else
		{
			notify.Debug("Profile Loader - No Profile Manager instatiated.");
			ProfileCheckComplete();
		}
	}
	
	public void ProfileCheckComplete()
	{
		if( msgObject )
		{
			msgObject.SendMessage("OnProfileLoadCheckDone");
		}
	}
	
	public void OnProfile()
	{
		ProfileManager.SharedInstance.UnregisterForOnlineProfileLoaded(OnProfile);
		
		UserProtoData userServerData = ProfileManager.SharedInstance.userServerData;
		PlayerStats inGameStats = GameProfile.SharedInstance.Player;
		
		notify.Debug("Delegate initiated");
		
		//If the server data is further along that in game, ask to replace it.
		if (userServerData != null && (userServerData._rank >= inGameStats.GetCurrentRank())
			&& (userServerData._totalMeters > inGameStats.lifetimeDistance))
		{
			notify.Debug("Profile Loader - Server data higher than local.");
			
			UIConfirmDialogOz.onNegativeResponse += onNotLoadProfile;
			UIConfirmDialogOz.onPositiveResponse += onLoadProfile;
		
			UIManagerOz.SharedInstance.confirmDialog.ShowConfirmDialog("Msg_ProfileStored_Title",
				"Msg_ProfileStored_Desc",
				"Btn_No", "Btn_Yes");
		}
		else 
		{
			ProfileCheckComplete();
		}
	}
	
	public void onLoadProfile()
	{
		UIConfirmDialogOz.onNegativeResponse -= onNotLoadProfile;
		UIConfirmDialogOz.onPositiveResponse -= onLoadProfile;
		
		//PlayerStats inGameStats = GameProfile.SharedInstance.Player;
		UserProtoData userServerData = ProfileManager.SharedInstance.userServerData;

		GameProfile.SharedInstance.Player.bestCoinScore = userServerData._bestCoins;
		GameProfile.SharedInstance.Player.bestDistanceScore = userServerData._bestMeters;
		GameProfile.SharedInstance.Player.bestSpecialCurrencyScore = userServerData._bestGems;
		GameProfile.SharedInstance.Player.bestScore = userServerData._bestScore;
					
		GameProfile.SharedInstance.Player.lifetimeCoins = userServerData._totalCoins;
		GameProfile.SharedInstance.Player.lifetimeDistance = userServerData._totalMeters;
		GameProfile.SharedInstance.Player.lifetimeSpecialCurrency = userServerData._totalGems;
		GameProfile.SharedInstance.Player.abilitiesUsed = userServerData._abilitiesUsed;
		
		GameProfile.SharedInstance.Player.coinCount = userServerData._currentCoins;
		GameProfile.SharedInstance.Player.specialCurrencyCount = userServerData._currentGems;
		
		GameProfile.SharedInstance.Player.numberResurectsUsed = userServerData._numberResurectsUsed;
		
		GameProfile.SharedInstance.Player.numberChanceTokens = userServerData._numberChanceTokens;
		GameProfile.SharedInstance.Player.lifetimePlays = userServerData._numberOfPlays;
		
		if (userServerData._artifactLevels != null && userServerData._artifactLevels.Count > 0) 
		{
			GameProfile.SharedInstance.Player.artifactLevels.Clear();
			foreach (int artifact in userServerData._artifactLevels)
			{
				GameProfile.SharedInstance.Player.artifactLevels.Add(artifact);
			}
		}
		
		if (userServerData._artifactsGemmed != null && userServerData._artifactsGemmed.Count > 0)
		{
			GameProfile.SharedInstance.Player.artifactsGemmed.Clear();
			foreach (int artifact in userServerData._artifactsDiscovered)
			{
				GameProfile.SharedInstance.Player.artifactsGemmed.Add(artifact);
			}
		}
		
		if (userServerData._artifactsDiscovered != null && userServerData._artifactsDiscovered.Count > 0)
		{
			GameProfile.SharedInstance.Player.artifactsDiscovered.Clear();
			foreach (int artifact in userServerData._artifactsDiscovered)
			{
				GameProfile.SharedInstance.Player.artifactsDiscovered.Add(artifact);
			}
		}
		
		
		if (userServerData._objectivesEarned != null && userServerData._objectivesEarned.Count > 0)
		{
			GameProfile.SharedInstance.Player.objectivesEarned.Clear();
			foreach (int objective in userServerData._objectivesEarned)
			{
				GameProfile.SharedInstance.Player.objectivesEarned.Add(objective);
			}
		}
		
		if (userServerData._legendaryObjectivesEarned != null 
			&& userServerData._legendaryObjectivesEarned.Count > 0
		) {
			GameProfile.SharedInstance.Player.legendaryObjectivesEarned.Clear();
			foreach (int legObjective in userServerData._legendaryObjectivesEarned)
			{
				GameProfile.SharedInstance.Player.legendaryObjectivesEarned.Add(legObjective);
			}
		}
		
		if (userServerData._powersPurchased != null && userServerData._powersPurchased.Count > 0)
		{
			GameProfile.SharedInstance.Player.powersPurchased.Clear();
			foreach (int power in userServerData._powersPurchased)
			{
				GameProfile.SharedInstance.Player.powersPurchased.Add(power);
			}
		}
		
		if (userServerData._powersGemmed != null && userServerData._powersGemmed.Count > 0)
		{
			GameProfile.SharedInstance.Player.powersGemmed.Clear();
			foreach (int power in userServerData._powersGemmed)
			{
				GameProfile.SharedInstance.Player.powersGemmed.Add(power);
			}
		}
		
		if (userServerData._consumablesQuantity != null && userServerData._consumablesQuantity.Count > 0)
		{
			GameProfile.SharedInstance.Player.consumablesPurchasedQuantity.Clear();
			foreach (int consumable in userServerData._consumablesQuantity)
			{
				GameProfile.SharedInstance.Player.consumablesPurchasedQuantity.Add(consumable);
			}
		}
		
		
		
		GameProfile.SharedInstance.Player.objectivesActive.Clear();
		GameProfile.SharedInstance.Player.RefillObjectives();
		
		UIManagerOz.SharedInstance.PaperVC.UpdateCurrency();

		notify.Debug("Loaded Profile with rank: " + userServerData._rank + " and total meters: " 
			+ GameProfile.SharedInstance.Player.lifetimeDistance);
		
		ProfileCheckComplete();
	}
	
	public void onNotLoadProfile()
	{
		UIConfirmDialogOz.onNegativeResponse -= onNotLoadProfile;
		UIConfirmDialogOz.onPositiveResponse -= onLoadProfile;
		
		ProfileCheckComplete();

	}
	*/
}

