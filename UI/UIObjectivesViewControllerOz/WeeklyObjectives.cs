using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using MiniJSON;
using UnityEngine;

public class WeeklyObjectives : MonoBehaviour // load 'weekly' objectives from the web (called 'challenges')
{
    protected static Notify notify;
    private readonly List<int> _earnedTeamObjectiveList = new List<int>();
    private readonly List<ObjectiveProtoData> _teamObjectiveList = new List<ObjectiveProtoData>();
    private readonly List<int> earnedWeeklyObjectiveList = new List<int>();
    private readonly List<ObjectiveProtoData> weeklyObjectivesList = new List<ObjectiveProtoData>();

    static WeeklyObjectives()
    {
        AreChallengesExpired = false;
        TeamObjectiveCount = 0;
        WeeklyObjectiveCount = 0;
    }

    /*
	private object challengeQueryResults = null;
	private bool pendingQuery = false;
	private DateTime lastQueryTime = new DateTime(0);
	private TimeSpan queryDelay = new TimeSpan(0,5,0);
	private TimeSpan maxQueryDelay = new TimeSpan(2,0,0);
	private bool wantQuery = false;
	*/

    //private string minAppVersion = "0.0.0";

    public static int WeeklyObjectiveCount { get; private set; }
    public static int TeamObjectiveCount { get; private set; }
    public static bool AreChallengesExpired { get; private set; }

    public List<ObjectiveProtoData> TeamObjectiveList
    {
        get { return _teamObjectiveList; }
    }

    public List<int> EarnedTeamObjectiveList
    {
        get { return _earnedTeamObjectiveList; }
    }

    public List<ObjectiveProtoData> GetWeeklyObjectives()
    {
        //UpdateAndCheckTheChallenges();
        if (Initializer.SharedInstance != null)
        {
            Initializer.SharedInstance.CheckForChallenges();
        }

        return weeklyObjectivesList;
    }

    public List<int> GetEarnedWeeklyObjectiveList()
    {
        //UpdateAndCheckTheChallenges();
        if (Initializer.SharedInstance != null)
        {
            Initializer.SharedInstance.CheckForChallenges();
        }

        return earnedWeeklyObjectiveList;
    }

    private void Awake()
    {
        notify = new Notify(GetType().Name);
    }

    private void Start()
    {
        /*
		if ( ! Settings.GetBool("use-init-msg", true))
		{
			GetChallenges(); 
		}
		*/
    }

//	void Update () { }


    //private List<object> challengesItems = new List<object>();

    /// <summary>
    ///     Gets the latest in game challenges
    /// </summary>
    [Obsolete("GetChallenges is deprecated, Challenges are set by Initializer through ApplyFromInit", true)]
    public void GetChallenges()
    {
        //lastQueryTime = DateTime.UtcNow;

        var languageCode = Localization.SharedInstance.GetISO1LanguageCode();

        var jsonStr = "{\"lang\":\"" + languageCode + "\"}";

        notify.Debug("GetChallenges: " + jsonStr);

        var hashSalt = Settings.GetString("hash-salt", "");

        if (hashSalt != "")
        {
            var hashMessage = SaveLoad.CheckState(jsonStr + hashSalt);
            var hashHeader = new Hashtable();
            hashHeader.Add("X_AUTHENTICATION", hashMessage);

            var postData = new WWWForm();

            postData.AddField("data", jsonStr);

            NetAgent.Submit(new NetRequest("/init", postData, GotTheChallenges, hashHeader));
        }
    }

    public void SaveChallenges()
    {
        using (var stream = new MemoryStream())
        {
            notify.Debug("SaveChallenges saveFileToPersistents-- weeklyobject.txt");
            var fileName = Application.persistentDataPath + Path.DirectorySeparatorChar +
                           "weeklyobject.txt";
            var challengeList = new List<object>();
            foreach (var data in weeklyObjectivesList)
            {
                challengeList.Add(data.ToWebObjectiveDict());
            }

            var teamChallengeList = new List<object>();
            foreach (var data in _teamObjectiveList)
            {
                teamChallengeList.Add(data.ToWebObjectiveDict(true));
            }

            var saveDict = new Dictionary<string, object>();

            saveDict.Add("challengesActiveData", challengeList);
            saveDict.Add("challengesEarned", earnedWeeklyObjectiveList);
            saveDict.Add("teamObjective", teamChallengeList);
            saveDict.Add("earnedTeamObjective", _earnedTeamObjectiveList);

            var secureData = SaveLoad.Save(saveDict);
            var dictString = Json.Serialize(secureData);

            try
            {
                using (var fileWriter = File.CreateText(fileName))
                {
                    fileWriter.WriteLine(dictString);
                    fileWriter.Close();
                }
            }
            catch (Exception ex)
            {
                notify.Warning("Weekly Challenge Save Exception: " + ex);
            }
        }
    }

    public static void ResetChallengesFile()
    {
        using (var stream = new MemoryStream())
        {
            notify.Debug("ResetChallengesFile saveFileToPersistents-- weeklyobject.txt");
            var fileName = Application.persistentDataPath + Path.DirectorySeparatorChar +
                           "weeklyobject.txt";

            var saveDict = new Dictionary<string, object>();

            saveDict.Add("challengesActiveData", new List<object>());
            saveDict.Add("challengesEarned", new List<int>());
            saveDict.Add("teamObjective", new List<object>());
            saveDict.Add("earnedTeamObjective", new List<int>());

            var secureData = SaveLoad.Save(saveDict);
            var dictString = Json.Serialize(secureData);

            try
            {
                using (var fileWriter = File.CreateText(fileName))
                {
                    fileWriter.WriteLine(dictString);
                    fileWriter.Close();
                }
            }
            catch (Exception ex)
            {
                notify.Warning("Weekly Challenge Reset Exception: " + ex);
            }
        }
    }

    public bool IsWeeklyObjectiveIDLoaded(int id)
    {
        foreach (var data in weeklyObjectivesList)
        {
            if (id == data._id)
                return true;
        }
        return false;
    }

    public void LoadChallenges()
    {
        var fileName = Application.persistentDataPath + Path.DirectorySeparatorChar +
                       "weeklyobject.txt";
        if (File.Exists(fileName) == false)
        {
            notify.Warning("No weeklyobjective.txt exists.");
        }
        else
        {
            // fix iOS crash bug
            var f = new FileInfo(fileName);
            if (f.Length > 2*1024*1024)
            {
                // if we're over 2MB we are screwed
                notify.Warning("weekly objective is too big at " + f.Length);
                return;
            }

            var reader = File.OpenText(fileName);
            var jsonString = reader.ReadToEnd();
            reader.Close();

            Dictionary<string, object> loadedData = null;


            try
            {
//				Dictionary<string, object> loadedData = 
                loadedData = Json.Deserialize(jsonString) as Dictionary<string, object>;
            }
            catch (Exception ex)
            {
                notify.Warning("Error Deserializing Locally stored Weekly Challenges: " + ex.Message);
                return;
            }

            if (loadedData != null)
            {
                if (SaveLoad.Load(loadedData) == false)
                {
                    return;
                }

                var dataDict = loadedData["data"]
                    as Dictionary<string, object>;

                if (dataDict == null)
                {
                    return;
                }

                var loadObjList
                    = dataDict["challengesActiveData"] as List<object>;

                foreach (var loadObj in loadObjList)
                {
                    var weekObj = DecodeChallengeJsonObject(loadObj);

                    //Remove challenges that do not have a conditionIndex.
                    if (weekObj != null
                        && weekObj._conditionList != null
                        && weekObj._conditionList.Count > 0
                        && weekObj._conditionList[0]._conditionIndex != 0)
                    {
                        if (!IsWeeklyObjectiveIDLoaded(weekObj._id))
                        {
                            weeklyObjectivesList.Add(weekObj);
                        }
                    }
                }

                var earnedList = dataDict["challengesEarned"] as List<object>;

                var count = 0;
                foreach (var earnedObj in earnedList)
                {
                    int earnedIndex;

                    if (int.TryParse(earnedObj.ToString(), out earnedIndex))
                    {
                        if (!earnedWeeklyObjectiveList.Contains(earnedIndex))
                            earnedWeeklyObjectiveList.Add(earnedIndex);
                    }
                    count ++;

                    //This is to make sure that a previous bug doesnt make the user wait a really, really long time
                    if (count > 10000)
                        break;
                }

                notify.Debug("Read file in Weekly Objectives");

                _teamObjectiveList.Clear();

                if (dataDict.ContainsKey("teamObjective"))
                {
                    var teamChallengeObjectList = dataDict["teamObjective"] as List<object>;

                    if (teamChallengeObjectList != null)
                    {
                        foreach (var teamChallengeObject in teamChallengeObjectList)
                        {
                            var teamChallenge = DecodeChallengeJsonObject(teamChallengeObject);
                            _teamObjectiveList.Add(teamChallenge);
                        }
                    }
                }

                // Add earned team indices to memory if not currently present

                if (dataDict.ContainsKey("earnedTeamObjective"))
                {
                    var earnedTeamChallengeList = dataDict["earnedTeamObjective"] as List<object>;

                    if (earnedTeamChallengeList != null)
                    {
                        foreach (var earnedTeamIndexObject in earnedTeamChallengeList)
                        {
                            var earnedTeamIndex = 0;

                            if (int.TryParse(earnedTeamIndexObject.ToString(), out earnedTeamIndex)
                                && !_earnedTeamObjectiveList.Contains(earnedTeamIndex)
                                )
                            {
                                _earnedTeamObjectiveList.Add(earnedTeamIndex);
                            }
                        }
                    }
                }
            }
        }
    }

    public void RemoveExpiredChallenges()
    {
        //	List<ObjectiveProtoData> objList = new List<ObjectiveProtoData>();
        //	List<int> objIdList = new List<int>();

        var toRemove = new List<ObjectiveProtoData>();

        foreach (var weeklyObj in weeklyObjectivesList)
        {
            if (weeklyObj._endDate <= DateTime.UtcNow
                || weeklyObj._startDate > DateTime.UtcNow
                )
            {
                notify.Debug("Removed Expired Challenge");
                //	objList.Add(weeklyObj);
                //	objIdList.Add(weeklyObj._id);
                toRemove.Add(weeklyObj);
                //NOTE!!! We don't want to remove these anymore, because people can change their clock to re-complete them otherwise
                //	earnedWeeklyObjectiveList.Remove(weeklyObj._id);
            }
        }

        foreach (var data in toRemove)
        {
            weeklyObjectivesList.Remove(data);
        }

        toRemove.Clear();

        foreach (var teamObjective in _teamObjectiveList)
        {
            if (teamObjective._endDate <= DateTime.UtcNow
                || teamObjective._startDate > DateTime.UtcNow
                )
            {
                notify.Debug("[WeeklyObjectives] -- RemoveExpiredChallenges : Team Objective removed");

                toRemove.Add(teamObjective);
            }
        }

        foreach (var data in toRemove)
        {
            _teamObjectiveList.Remove(data);
        }

        /*
		_weeklyObjectiveCount = weeklyObjectivesList.Count;
		
		if ( _weeklyObjectiveCount == 0 )
		{
			_areChallengesExpired = true;
		}
		else
		{
			_areChallengesExpired = false;
		}
		*/

        //	weeklyObjectivesList.Clear();

        //	foreach(ObjectiveProtoData obj in objList) { weeklyObjectivesList.Add(obj); }
    }

    public bool HaveExpiredChallenges()
    {
        if (weeklyObjectivesList.Count == 0)
        {
            Debug.Log("No objectives");
            return true;
        }
        foreach (var weeklyObj in weeklyObjectivesList)
        {
            if (weeklyObj._endDate < DateTime.UtcNow)
            {
                return true;
            }
        }
        return false;
    }

    [Obsolete("GotTheChallenges is deprecated, please use ApplyFromInit", true)]
    private bool GotTheChallenges(WWW www, bool noErrors, object results)
    {
        //Retrieve the challenges stored in pesistent data
/*
		notify.Debug("GotTheChallenges noErrors=" + noErrors + " www.error=" + www.error);
		bool result = false;
		pendingQuery = true;
		challengeQueryResults = null;
*/
        // Restored the original functionality of GotTheChallenges, however this code
        // path is deprecated and will likely never be called any longer as we are
        // using Initializer to perform all of the calls.

        var result = false;

        LoadChallenges();

        if (noErrors)
        {
            if (results == null)
            {
                notify.Error("No results! Must not be connected...");
                return false;
            }

            var dataDict = Json.Deserialize(www.text) as Dictionary<string, object>;

            notify.Debug("GotTheChallenges " + www.text + " " + www.error);

            var responseCode = 400;

            if (dataDict.ContainsKey("responseCode"))
            {
                responseCode = JSONTools.ReadInt(dataDict["responseCode"]);
            }

            //challengeQueryResults = results;
            if (responseCode == 200)
            {
                var webChallengeList = dataDict["challenges"] as List<object>;

                var webChallengeObjList =
                    new List<ObjectiveProtoData>();

                //Process the incoming objects
                foreach (var oneObject in webChallengeList)
                {
                    var oneItem = DecodeChallengeJsonObject(oneObject);
                    webChallengeObjList.Add(oneItem);
                }
                notify.Debug("WebChallenge Length " + webChallengeObjList.Count);

                _reconcileChallenges(weeklyObjectivesList, webChallengeObjList, earnedWeeklyObjectiveList, false);

                /*
				//int webChallengeObjListCount = webChallengeObjList.Count;
				
				
				//If there are multiple instances of a challenge with a same index, quash all but one of them
				List<int> indexExistList = new List<int>();
				
				List<int> reconcileEarnedList = new List<int>();
				
				GameProfile.SharedInstance.ChallengeScoreMultiplier = 0;
				
				//Apply the earned value from the stored conditions.
				foreach(ObjectiveProtoData webChallengeObj in webChallengeObjList)
				{
					if (weeklyObjectivesList != null && weeklyObjectivesList.Count > 0) {
						foreach(ObjectiveProtoData weeklyObj in weeklyObjectivesList)
						{
							if ((weeklyObj._id == webChallengeObj._id)
								&& !indexExistList.Contains(weeklyObj._id)
							) {
								if (earnedWeeklyObjectiveList.Contains(weeklyObj._id)) {
									reconcileEarnedList.Add(weeklyObj._id);
									
									//Add Multiplier for rank rewards.
									if (weeklyObj._rewardType == RankRewardType.Multipliers && !GameProfile.SharedInstance.Player.objectivesUnclaimed.Contains(weeklyObj._id))
									{
										GameProfile.SharedInstance.ChallengeScoreMultiplier +=
											weeklyObj._rewardValue;
									}
								}
								
								indexExistList.Add(weeklyObj._id);
								
								foreach(ConditionProtoData condProtoData in webChallengeObj._conditionList)
								{
									foreach (ConditionProtoData storedCondProtoData in weeklyObj._conditionList)
									{
										if (condProtoData._conditionIndex == storedCondProtoData._conditionIndex)
										{
											condProtoData._earnedStatValue = storedCondProtoData._earnedStatValue;
										}
									}
								}
							}
						}
					}
				}
				
				weeklyObjectivesList.Clear();
				
				foreach(ObjectiveProtoData objProta in webChallengeObjList)
				{
					if(!weeklyObjectivesList.Contains(objProta))
						weeklyObjectivesList.Add(objProta);
				}
				earnedWeeklyObjectiveList.Clear();
				
				foreach (int earnedIndex in reconcileEarnedList)
				{
					if(!earnedWeeklyObjectiveList.Contains(earnedIndex))
						earnedWeeklyObjectiveList.Add(earnedIndex);
				}
				
				foreach (ObjectiveProtoData objProta in weeklyObjectivesList)
				{
					if (!earnedWeeklyObjectiveList.Contains(objProta._id))
					{
						AnalyticsInterface.LogGameAction("challenges","participating",objProta._title, GameProfile.GetAreaCharacterString(), 0);
					}
				}
				*/
                RemoveExpiredChallenges();
                SaveChallenges();
            }


            // If the response from the server was a code of 204, the query was
            // successful but no data was present
            else if (responseCode == 204)
            {
                Reset();
                result = true;
                SaveChallenges();
            }
            // If error responses were received, use the stored challenge list.
            //    If the stored challenges' end date is less than the device's UTC
            //    date time, remove the challenge.
            else
            {
                RemoveExpiredChallenges();
                SaveChallenges();
            }

            //Set the static variable for the number of weekly objectives.
            WeeklyObjectiveCount = weeklyObjectivesList.Count;
        }
        // No results, probably not connected.  Use the stored challenges
        else
        {
            notify.Debug("No connection.  Load stored challenges if they exist");
        }

        //Assign Weekly Challenges to Player Profile.
        //GameProfile.SharedInstance.Player.FillChallenges();
        //TODO: check if the UI is up and callback...

        //ProcessTheChallenges();
        //if(GameController.SharedInstance.IsSafeToUpdateWeeklyObjectives())
        //	UIManagerOz.SharedInstance.ObjectivesVC.RefreshWeeklyObjectivesList();

        return result;
    }

    private void _reconcileChallenges(
        List<ObjectiveProtoData> localObjectiveList,
        List<ObjectiveProtoData> remoteObjectiveList,
        List<int> earnedLocalObjectiveList,
        bool isTeamChallenge
        )
    {
        // Reset the challenge multiplier
        var gameProfile = GameProfile.SharedInstance;


        // Prevent multiple instances of the same objective from being added to the reconcile list
        //   This should never happen
        var indexExistList = new List<int>();

        // Apply the earned value from the locally stored objective
        foreach (var remoteObjective in remoteObjectiveList)
        {
            if (localObjectiveList != null && localObjectiveList.Count > 0)
            {
                foreach (var localObjective in localObjectiveList)
                {
                    // Compare objective ids
                    if (localObjective._id == remoteObjective._id
                        && !indexExistList.Contains(localObjective._id)
                        )
                    {
                        if (earnedLocalObjectiveList.Contains(localObjective._id))
                        {
                            // Add multiplier for earned objectives with reward type of multipler
                            if (localObjective._rewardType == RankRewardType.Multipliers
                                && !gameProfile.Player.objectivesUnclaimed.Contains(localObjective._id)
                                )
                            {
                                gameProfile.ChallengeScoreMultiplier += localObjective._rewardValue;
                            }
                        }

                        indexExistList.Add(localObjective._id);

                        foreach (var remoteCondition in remoteObjective._conditionList)
                        {
                            foreach (var localCondition in localObjective._conditionList)
                            {
                                if (remoteCondition._conditionIndex == localCondition._conditionIndex)
                                {
                                    remoteCondition._earnedStatValue = localCondition._earnedStatValue;
                                }
                            }
                        }
                    }
                }
            }
        }

        // Store the ids for the local objectives.
        var localObjectiveIndexList = new List<int>();

        foreach (var localObjective in localObjectiveList)
        {
            localObjectiveIndexList.Add(localObjective._id);
        }

        if (isTeamChallenge)
        {
            var tempEarnIndexList = new List<int>();

            foreach (var earnedIndex in earnedLocalObjectiveList)
            {
                if (localObjectiveIndexList.Contains(earnedIndex))
                {
                    tempEarnIndexList.Add(earnedIndex);
                }
            }

            // Flush the current earned index list.
            earnedLocalObjectiveList.Clear();

            // And add the reconciled list
            foreach (var tempEarnIndex in tempEarnIndexList)
            {
                earnedLocalObjectiveList.Add(tempEarnIndex);
            }
        }
        localObjectiveList.Clear();

        foreach (var remoteObjective in remoteObjectiveList)
        {
            localObjectiveList.Add(remoteObjective);

            // Set one shot notifications for challenges
            if (!localObjectiveIndexList.Contains(remoteObjective._id))
            {
                notify.Debug("[WeeklyObjectives] Sending One shot for new challenge challenge id");

                if (isTeamChallenge)
                {
                    Services.Get<NotificationSystem>()
                        .SendOneShotNotificationEvent(remoteObjective._id,
                            (int) OneShotNotificationType.NewTeamChallenge);
                }
                else
                {
                    Services.Get<NotificationSystem>()
                        .SendOneShotNotificationEvent(remoteObjective._id,
                            (int) OneShotNotificationType.NewWeeklyChallenge);
                }
            }
        }

        foreach (var localObjective in localObjectiveList)
        {
            if (!earnedLocalObjectiveList.Contains(localObjective._id))
            {
                // TO DO, analytic call.
                //				AnalyticsInterface.LogGameAction( "challenges", "participating", localObjective._title, GameProfile.GetAreaCharacterString(), 0 );
            }
        }
    }

    public bool ApplyChallengesFromInit(List<object> webChallengeList, int responseCode)
    {
        var result = false;
        LoadChallenges();

        if (responseCode == 200)
        {
            var webChallengeObjList = new List<ObjectiveProtoData>();

            var teamWebChallengeObjList = new List<ObjectiveProtoData>();

            //Process the incoming objects
            foreach (var oneObject in webChallengeList)
            {
                var oneItem = DecodeChallengeJsonObject(oneObject);

                if (oneItem._guildChallenge)
                {
                    teamWebChallengeObjList.Add(oneItem);
                }
                else
                {
                    webChallengeObjList.Add(oneItem);
                }
            }
            notify.Debug("WebChallenge Length " + webChallengeObjList.Count);
            //int webChallengeObjListCount = webChallengeObjList.Count;


            GameProfile.SharedInstance.ChallengeScoreMultiplier = 0;

            _reconcileChallenges(weeklyObjectivesList, webChallengeObjList, earnedWeeklyObjectiveList, false);
            _reconcileChallenges(_teamObjectiveList, teamWebChallengeObjList, _earnedTeamObjectiveList, true);

            // Update team challenges with neighbors progress.

            // Only apply challenge progress if the player is logged into facebook


            RemoveExpiredChallenges();
            SaveChallenges();
        }

        // If the response from the server was a code of 204, the query was
        // successful but no data was present
        else if (responseCode == 204)
        {
            Reset();
            result = true;
        }
        // If error responses were received, use the stored challenge list.
        //    If the stored challenges' end date is less than the device's UTC
        //    date time, remove the challenge.
        else
        {
            RemoveExpiredChallenges();
            SaveChallenges();
        }

        //Set the static variable for the number of weekly objectives.
        WeeklyObjectiveCount = weeklyObjectivesList.Count;

        //Set the static variable for the number of team objective
        TeamObjectiveCount = _teamObjectiveList.Count;

        return result;
    }

    public void CheckExpired()
    {
        if (weeklyObjectivesList != null && weeklyObjectivesList.Count != 0)
        {
            foreach (var weeklyObjective in weeklyObjectivesList)
            {
                if (weeklyObjective._endDate < DateTime.UtcNow)
                {
                    AreChallengesExpired = true;
                    return;
                }
            }
            AreChallengesExpired = false;
        }

        if (_teamObjectiveList != null && _teamObjectiveList.Count != 0)
        {
            foreach (var teamObjective in _teamObjectiveList)
            {
                if (teamObjective._endDate < DateTime.UtcNow)
                {
                    AreChallengesExpired = true;
                }
                else // if at least one of the team challenges is active set the field to false, then break the loop.
                {
                    AreChallengesExpired = false;
                    break;
                }
            }
        }
    }

    public void Reset()
    {
        earnedWeeklyObjectiveList.Clear();
        weeklyObjectivesList.Clear();
        SaveChallenges();
    }

    [Obsolete("Update Guild Challenges is deprecated.", true)]
    public void UpdateGuildChallenges()
    {
        //ProfileManager profileMngr = Services.Get<ProfileManager>();

        //UserProtoData userServerData = profileMngr.userServerData;

        var userServerData = ProfileManager.SharedInstance.userServerData;

        if ((userServerData != null) &&
            (userServerData._guildChallengeList != null) &&
            (userServerData._guildChallengeList.Count > 0)
            )
        {
            foreach (var guildChallenge in userServerData._guildChallengeList)
            {
                foreach (var weeklyObj in weeklyObjectivesList)
                {
                    if ((weeklyObj._id == guildChallenge._challengeIndex))
                    {
                        weeklyObj._conditionList[0]._earnedNeighborValue = guildChallenge._neighborValueTotal;
                    }
                }
            }
        }
    }

    public void CompleteChallenges()
    {
        notify.Debug("Weekly Challenges Completion Check");

        foreach (var weeklyChallenge in weeklyObjectivesList)
        {
            if (!earnedWeeklyObjectiveList.Contains(weeklyChallenge._id))
            {
                var totalEarnedSv = 0;
                var totalSv = 0;
                var totalNeighborSv = 0;

                notify.Debug("weeklyChallenge: " + weeklyChallenge.ToJson());

                foreach (var cond in weeklyChallenge._conditionList)
                {
                    totalEarnedSv += cond._earnedStatValue;
                    totalSv += cond._statValue;
                    totalNeighborSv += cond._earnedNeighborValue;
                }

                notify.Debug("Total Earned: " + totalEarnedSv + " Total Neigh: " + totalNeighborSv +
                             " Total SV: " + totalSv);
                //Earned Value is higher that required, complete the challenge.
                //And Reward the player.
                if ((totalEarnedSv + totalNeighborSv) >= totalSv)
                {
                    if (!earnedWeeklyObjectiveList.Contains(weeklyChallenge._id))
                        earnedWeeklyObjectiveList.Add(weeklyChallenge._id);
                    RewardPlayer(weeklyChallenge._rewardType,
                        weeklyChallenge._rewardValue);
                }
            }
        }
    }

    public void RewardObjective(int objIndex)
    {
        notify.Warning("Weekly Objective reward during gameplay");

        //	Debug.Log("Index: "+objIndex);

        foreach (var weeklyObj in weeklyObjectivesList)
        {
            if (objIndex == weeklyObj._id)
            {
                //		Debug.Log("Reward: "+weeklyObj._rewardType + " " +weeklyObj._rewardValue);
                switch (weeklyObj._rewardType)
                {
                    case RankRewardType.Multipliers:
                        GameProfile.SharedInstance.ChallengeScoreMultiplier += weeklyObj._rewardValue;
                        break;
                    case RankRewardType.Coins:
                        GameProfile.SharedInstance.Player.coinCount += weeklyObj._rewardValue;
                        break;
                    case RankRewardType.Gems:
                        GameProfile.SharedInstance.Player.specialCurrencyCount += weeklyObj._rewardValue;
                        break;
                }
                UIManagerOz.SharedInstance.PaperVC.UpdateCurrency();
            }
        }

        foreach (var teamObjective in _teamObjectiveList)
        {
            if (objIndex == teamObjective._id)
            {
                switch (teamObjective._rewardType)
                {
                    case RankRewardType.Multipliers:
                        GameProfile.SharedInstance.ChallengeScoreMultiplier += teamObjective._rewardValue;
                        break;
                    case RankRewardType.Coins:
                        GameProfile.SharedInstance.Player.coinCount += teamObjective._rewardValue;
                        break;
                    case RankRewardType.Gems:
                        GameProfile.SharedInstance.Player.specialCurrencyCount += teamObjective._rewardValue;
                        break;
                }
                UIManagerOz.SharedInstance.PaperVC.UpdateCurrency();
            }
        }
    }

    //public void RewardPlayer(int rewardType, int rewardValue)
    public void RewardPlayer(RankRewardType rewardType, int rewardValue)
    {
        notify.Warning("Weekly Objective: Rewarding Player. Type: " +
                       rewardType + " Value: " + rewardValue
            );
        switch (rewardType)
        {
            case RankRewardType.Multipliers:
                GameProfile.SharedInstance.ChallengeScoreMultiplier += rewardValue;
                break;
            case RankRewardType.Coins:
                GameProfile.SharedInstance.Player.coinCount += rewardValue;
                break;
            case RankRewardType.Gems:
                GameProfile.SharedInstance.Player.specialCurrencyCount += rewardValue;
                break;
            default:
                GameProfile.SharedInstance.Player.coinCount += rewardValue;
                break;
        }
        /*
		switch (rewardType)
		{
		case 1:
			//TO DO -- Add Multiplier for reward.
			TR.LOG ("Score Multiplier: " + GameProfile.SharedInstance.GetAdditionalScoreMultiplier());
			GameProfile.SharedInstance.ChallengeScoreMultiplier += rewardValue;
			
			TR.LOG ("Post Reward: " + GameProfile.SharedInstance.GetAdditionalScoreMultiplier());
			break;
		case 2:
			GameProfile.SharedInstance.Player.coinCount += rewardValue;			
			break;
			
		case 3:
			GameProfile.SharedInstance.Player.specialCurrencyCount += rewardValue;
			break;
			
		default:
			GameProfile.SharedInstance.Player.coinCount += rewardValue;	
			break;
		}
		*/
    }

    private ObjectiveProtoData DecodeChallengeJsonObject(object _challengeItem)
    {
        var dict = _challengeItem as Dictionary<string, object>;
        var challenge = new ObjectiveProtoData(dict);
        return challenge;
    }

    public void AddChallengeStat(ObjectiveType objType, ObjectiveTimeType timeType,
        ObjectiveFilterType filterType, int incrementValue)
    {
        if (timeType == ObjectiveTimeType.LifeTime)
        {
            //-- TODO Funnel this into something else
        }
        else
        {
            foreach (var challengeOb in weeklyObjectivesList)
            {
                if (challengeOb == null)
                    continue;

                ConditionProtoData foundCondition = null;

                //Cycle through the conditions to see if there is a match for the challenge.
                var condMax = challengeOb._conditionList.Count;

                for (var j = 0; j < condMax; j++)
                {
                    var conditionOb = challengeOb._conditionList[j];

                    if (conditionOb._type != objType
                        || conditionOb._timeType != timeType
                        || conditionOb._filterType != filterType
                        )
                    {
                        continue;
                    }
                    foundCondition = conditionOb;

                    //-- Breaking here mean we will only find the first one to match, but the design complies with this because we
                    //-- should only have one of each type in the list.
                    if (foundCondition != null)
                    {
                        if (timeType == ObjectiveTimeType.OverTime)
                        {
                            foundCondition._earnedStatValue += incrementValue;
                        }
                        else if (timeType == ObjectiveTimeType.PerRun)
                        {
                            if (incrementValue >= foundCondition._earnedStatValue)
                            {
                                foundCondition._earnedStatValue = incrementValue;
                            }
                        }
                    }

                    if (foundCondition._earnedStatValue
                        > foundCondition._statValue
                        )
                    {
                        foundCondition._earnedStatValue =
                            foundCondition._statValue;
                    }
                }
            }
        }
    }
}

/*			
			//If there are multiple instances of a challenge with a same index, quash all but one of them
			List<int> indexExistList = new List<int>();
			
			List<int> reconcileEarnedList = new List<int>();
			
			GameProfile.SharedInstance.ChallengeScoreMultiplier = 0;
			
			//Apply the earned value from the stored conditions.
			foreach(ObjectiveProtoData webChallengeObj in webChallengeObjList)
			{
				if (weeklyObjectivesList != null && weeklyObjectivesList.Count > 0) {
					foreach(ObjectiveProtoData weeklyObj in weeklyObjectivesList)
					{
						if ((weeklyObj._id == webChallengeObj._id)
							&& !indexExistList.Contains(weeklyObj._id)
						) {
							if (earnedWeeklyObjectiveList.Contains(weeklyObj._id)) {
								reconcileEarnedList.Add(weeklyObj._id);
								
								//Add Multiplier for rank rewards.
								if (weeklyObj._rewardType == RankRewardType.Multipliers && !GameProfile.SharedInstance.Player.objectivesUnclaimed.Contains(weeklyObj._id))
								{
									GameProfile.SharedInstance.ChallengeScoreMultiplier +=
										weeklyObj._rewardValue;
								}
							}
							
							indexExistList.Add(weeklyObj._id);
							
							foreach(ConditionProtoData condProtoData in webChallengeObj._conditionList)
							{
								foreach (ConditionProtoData storedCondProtoData in weeklyObj._conditionList)
								{
									if (condProtoData._conditionIndex == storedCondProtoData._conditionIndex)
									{
										condProtoData._earnedStatValue = storedCondProtoData._earnedStatValue;
									}
								}
							}
						}
					}
				}
			}
			
			weeklyObjectivesList.Clear();
			
			foreach(ObjectiveProtoData objProta in webChallengeObjList)
			{
				if(!weeklyObjectivesList.Contains(objProta))
					weeklyObjectivesList.Add(objProta);
			}
			earnedWeeklyObjectiveList.Clear();
			
			foreach (int earnedIndex in reconcileEarnedList)
			{
				if(!earnedWeeklyObjectiveList.Contains(earnedIndex))
					earnedWeeklyObjectiveList.Add(earnedIndex);
			}
			
			foreach (ObjectiveProtoData objProta in weeklyObjectivesList)
			{
				if (!earnedWeeklyObjectiveList.Contains(objProta._id))
				{
					AnalyticsInterface.LogGameAction("challenges","participating",objProta._title, GameProfile.GetAreaCharacterString(), 0);
				}
			}
			*/

/*	
	public bool IsBuildVersionPassThreshold()
	{
		bool result = true;
		
		string buildVersion = BundleInfo.GetBundleVersion().ToString();
		
		notify.Debug("IsBuildVersionPassThreshold: buildVersion - " + buildVersion);
		
		notify.Debug("IsBuildVersionPassThreshold: minAppVersion - " + minAppVersion);
		
		string[] buildVersionArray = buildVersion.Split('.');
		
		string[] minAppVersionArray = minAppVersion.Split('.');
		
		try {
			for (int i = 0; i < buildVersionArray.Length; i++)
			{
				if (int.Parse(buildVersionArray[i]) < int.Parse(minAppVersionArray[i])) {
					result = false;
				}
				
				notify.Debug("Loop for build version compare. i = " + i);
			}
			
		} catch (Exception ex) {
			notify.Debug("Error comparing build versions.  Err Msg: " + ex.Message);
			
			result = false;
		}
		
		notify.Debug ("Result of IsBuildPassThreshold: " + result.ToString());
		
		return result;
	}
*/


/*	
	private bool UpdateAndCheckTheChallenges()
	{
		if(GameController.SharedInstance.IsSafeToUpdateWeeklyObjectives())
		{
			notify.Debug ("Could UpdateAndCheckTheChallenges @ " + Time.frameCount);
			if((!ProcessTheChallenges())||HaveExpiredChallenges())
			{
				if(!wantQuery)
				{
					notify.Debug ("Queueing challenge update @ " + Time.frameCount);
				}
				wantQuery = true;
			}
			else
			if(queryDelay < maxQueryDelay)
			{//we have good data so reset the query delay
				queryDelay = new TimeSpan(0,5,0);
			}
			if(wantQuery)
			{
				if(DateTime.UtcNow - lastQueryTime >= queryDelay)
				{
					notify.Debug ("Requesting challenge update after " + queryDelay.ToString());
					wantQuery = false;
					queryDelay.Add (queryDelay);
					GetChallenges();
				}
			}
			notify.Debug ("Done UpdateAndCheckTheChallenges @ " + Time.frameCount);
			return true;
		}			
		return false;
	}
	
	private bool ProcessTheChallenges( )
	{
		bool result = false;
		if(pendingQuery)
		{
//			Debug.Log("Processing new challenge data");
			pendingQuery = false;
			//Retrieve the challenges stored in pesistent data
			LoadChallenges();

			if(challengeQueryResults == null)
			{
				// No results, probably not connected.  Use the stored challenges
				RemoveExpiredChallenges();
				SaveChallenges();
			}
			else
			try
			{	
				Dictionary<string, object> rootDict = challengeQueryResults as Dictionary<string, object>;
	
					
	//Replaced checking querySucces with checking if responseCode is '200' - Response is Okay, and data is present
	//				bool querySuccess = (bool) rootDict["querySuccess"] ;
	//				if (querySuccess)
	
				int responseCode = int.Parse(rootDict["responseCode"].ToString());
					
				
				if (responseCode == 200)
				{ 
					if (rootDict.ContainsKey("minAppVersion")) 
					{
						minAppVersion = rootDict["minAppVersion"].ToString();
					}
					
					if (Initializer.IsBuildVersionPassThreshold())
					{
						
						List<object> webChallengeList = rootDict["challenges"] as List<object>;
										
						List<ObjectiveProtoData> webChallengeObjList = 
							new List<ObjectiveProtoData>();
						
						//Process the incoming objects
						foreach (object oneObject in webChallengeList)
						{
							ObjectiveProtoData oneItem = DecodeChallengeJsonObject(oneObject);
							webChallengeObjList.Add(oneItem);
						}
						notify.Debug("WebChallenge Length " + webChallengeObjList.Count);
						//int webChallengeObjListCount = webChallengeObjList.Count;
						
						
						List<int> reconcileEarnedList = new List<int>();
						
						List<int> indexExistList = new List<int>();
						
						//Apply the earned value from the stored conditions.
						foreach(ObjectiveProtoData webChallengeObj in webChallengeObjList)
						{
							if (weeklyObjectivesList != null && weeklyObjectivesList.Count > 0) {
								foreach(ObjectiveProtoData weeklyObj in weeklyObjectivesList)
								{
									if ((weeklyObj._id == webChallengeObj._id)
										&& (!indexExistList.Contains(weeklyObj._id))
									) {
										indexExistList.Add(weeklyObj._id);
										
										if (earnedWeeklyObjectiveList.Contains(weeklyObj._id)) {
											reconcileEarnedList.Add(weeklyObj._id);
										}
										foreach(ConditionProtoData condProtoData in webChallengeObj._conditionList)
										{
											foreach (ConditionProtoData storedCondProtoData in weeklyObj._conditionList)
											{
												if (condProtoData._conditionIndex == storedCondProtoData._conditionIndex)
												{
													condProtoData._earnedStatValue = storedCondProtoData._earnedStatValue;
												}
											}
										}
									}
								}
							}
						}
						
						weeklyObjectivesList.Clear();
						
						foreach(ObjectiveProtoData objProta in webChallengeObjList)
						{
							weeklyObjectivesList.Add(objProta);
						}
						//Dont clear these anymore... we need to save them to prevent cheating
					//	earnedWeeklyObjectiveList.Clear();
						
					//	foreach (int earnedIndex in reconcileEarnedList)
					//	{
					//		if(!earnedWeeklyObjectiveList.Contains(earnedIndex))
					//			earnedWeeklyObjectiveList.Add(earnedIndex);
					//	}
						
						//foreach (ObjectiveProtoData objProta in weeklyObjectivesList)
						//	Services.Get<NotificationSystem>().SendOneShotNotificationEvent(objProta._id, (int)OneShotNotificationType.NewWeeklyChallenge);
						
						RemoveExpiredChallenges();
						SaveChallenges();
						result = true;
					}
				}
				// If the response from the server was a code of 204, the query was
				// successful but no data was present
				else if (responseCode == 204)
				{
					Reset();
					SaveChallenges();
				}
				// If error responses were received, use the stored challenge list.
				//    If the stored challenges' end date is less than the device's UTC
				//    date time, remove the challenge.
				else 
				{
					RemoveExpiredChallenges();
					SaveChallenges();	
				}
			}
			catch (System.Exception theException)
			{
				notify.Warning("ProcessTheChallenges exception " + theException.Message);
			}
		}

		
		//Assign Weekly Challenges to Player Profile.
		//GameProfile.SharedInstance.Player.FillChallenges();
		return result;
	}
*/

//Services.Get<NetAgent>().Submit(new NetRequest("/api/challenges", GotTheChallenges));

/*
	/// <summary>
	/// Loads the challenge file.
	/// </summary>
	/// <returns>
	/// A List of objects for the challenges.
	/// </returns>
	public List<object> LoadChallengeFile()
	{
		string fileName = Application.persistentDataPath + Path.DirectorySeparatorChar + 
			"weeklyobject.txt";
		
		List<object> challengeList = null;
		
		if (File.Exists(fileName) == false)
		{
			TR.LOG("No Weekly Objectives exists.");	
		}
		else 
		{
			StreamReader reader = File.OpenText(fileName);
			string jsonString = reader.ReadToEnd();
			reader.Close();
			
			Dictionary<string,object> loadedData = MiniJSON.Json.Deserialize(jsonString) 
				as Dictionary<string, object>;
			
			TR.LOG("Read file in Weekly Objectives");
			
			if (SaveLoad.Load(loadedData) == false)
			{
				return null;	
			}
			
			Dictionary<string,object> dataDict = loadedData["data"] 
				as Dictionary<string, object>;
			
			if (dataDict == null) { return null; }
			
			challengeList = dataDict["challenges"] as List<object>;
			
		}
		return challengeList;	
	}
	
		/// <summary>
	/// Saves the file.
	/// </summary>
	/// <returns>
	/// True if successful.
	/// </returns>
	/// <param name='saveData'>
	/// List of objects to serialize and save.
	/// </param>
	public bool SaveChallengeFile(List<object> saveData)
	{
			string fileName = Application.persistentDataPath + Path.DirectorySeparatorChar + 
				"weeklyobject.txt";
			List<object> list = new List<object>();
			
			foreach (object saveItem in saveData) {
				Dictionary<string, object> challengeItem = 	saveItem as Dictionary<string, object>;
				list.Add(challengeItem);
			}
			
			Dictionary<string,object> saveDict = new Dictionary<string, object>();
			
			saveDict.Add("challenges", list);
			
			Dictionary<string, object> secureData = SaveLoad.Save(saveDict);
			string listString = MiniJSON.Json.Serialize(secureData);
		
			try
			{		
				using(StreamWriter fileWriter =  File.CreateText(fileName))
				{
					fileWriter.WriteLine(listString);
					fileWriter.Close();
				}
			}
			catch (Exception ex)
			{
				Dictionary<string,string> d = new Dictionary<string, string>();
				
				d.Add("Exception", ex.ToString());
				TR.LOG("Save Exception: " + ex);
			
				return false;
			}
		return true;
	}
	
	/// <summary>
	/// Reconciles the stored challenges with the web challenges
	/// </summary>
	/// <returns>
	/// A list of reconciled challenges
	/// </returns>
	/// <param name='webChallengeList'>
	/// Web challenge list.
	/// </param>
	/// <param name='storeChallengeList'>
	/// Store challenge list.
	/// </param>
	private List<object> ReconcileChallenges(List<object> webChallengeList, List<object> storeChallengeList)
	{
		List<object> reconciledChallengeList = new List<object>();
	
		foreach (object webChallengeObj in webChallengeList)
		{
			bool webChallengeInStore = false;

			//If items exist in the store, cycle through the store and compare them
			// to the challenges from the web
			if (storeChallengeList != null)
			{
				Dictionary<string, object> webChallengeDict =
					webChallengeObj as Dictionary<string, object>;
			
				//Cycle through the challenge items currently stored.
				foreach (object storeChallengeObj in storeChallengeList)
				{
					Dictionary<string,object> storeChallengeDict =
						storeChallengeObj as Dictionary<string,object>;
					
					// If the web challenge does exist in the stored list, 
					// use the stored values of the challenge.
					if (webChallengeDict["challengeIndex"] 
						== storeChallengeDict ["challengeIndex"])
					{
						webChallengeInStore = true;
						
						reconciledChallengeList.Add(storeChallengeObj);
					}
					//If the stored challenge does not match anything from the web,
					// drop the stored challenge
				}
			}
			
			// If the web challenge does not exist in the store add it.
			if (!webChallengeInStore)
			{
				reconciledChallengeList.Add(webChallengeObj);
			}
		}	
		return reconciledChallengeList;
	}
	*/

/*				
		
				//-- Security check
		Dictionary<string, object> loadedData = MiniJSON.Json.Deserialize(text.text) as Dictionary<string, object>;
		if(SaveLoad.Load(loadedData) == false)
		{
#if !UNITY_EDITOR			
			return false;
#endif
		}
		
		List<object> store = loadedData["data"] as List<object>;
		
		weeklyObjectivesList = DecodeChallengeJSON(challengesItems);
		
		return result;	
	}	

	private List<ObjectiveProtoData> DecodeChallengeJSON(List<object> _data)
	{
		List<ObjectiveProtoData> decodedObjectives = new List<ObjectiveProtoData>();
		//List<object> store = _data["data"] as List<object>;
		
		foreach(object dict in _data) 
		{
			Dictionary<string, object> data = dict as Dictionary<string, object>;
			ObjectiveProtoData ob = new ObjectiveProtoData(data);
			decodedObjectives.Add(ob);
		}
		
		return decodedObjectives;
	}
}
	
	*/