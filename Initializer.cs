using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using MiniJSON;
using UnityEngine;

/// <summary>
///     Initializer. In execution order will execute before other scripts
///     sends the initial login message and will parse out weekly challenge, news, and store discounts to appropriate
///     objects
/// </summary>
public class Initializer : MonoBehaviour
{
    public delegate void LeaderBoardRetrievedHandler();

    public static Initializer SharedInstance;
    private static bool initIsDownloading;
    private static bool initDownloadSuccess;
    private static bool initDownloadFailure;
    private static bool _isBuildVersion = true;
    private readonly TimeSpan _maxQueryDelay = new TimeSpan(2, 0, 0);
    //private readonly DateTime lastInitCall = DateTime.UtcNow;
    private readonly DateTime lastLeaderBoardCall = DateTime.UtcNow;
    private string _DMOId = "";
    private DateTime _lastQueryTime = new DateTime(0);
    private TimeSpan _queryDelay = new TimeSpan(0, 5, 0);
    // Pulling in Gavin's Challenge Updating Code into Initializer from Weekly Objectives
    private bool _wantQuery;
    private bool displayLeaderboard;
    private DateTime lastFriendCheck = DateTime.UtcNow;
    private string mDeviceId;
    private string mFacebookAccessToken;
    private string mGameCenterId = "";
    private string mGameCenterName;
    protected string minAppVersion = "0.0.0";
    protected Notify notify;
    //private string tempFBToken = "";
    //private string tempGCId = "";

    static Initializer()
    {
        LoadInitialLeaderboard = false;
    }

    public static bool LoadInitialLeaderboard { get; set; }

    public void SetDMOId(string DMOId)
    {
        if (string.IsNullOrEmpty(DMOId))
        {
            return;
        }

        _DMOId = DMOId;
        SaveLastFriendCheck();
    }

    public static bool GetInitIsDownloading()
    {
        return initIsDownloading;
    }

    public static bool GetInitDownloadSuccess()
    {
        return initDownloadSuccess;
    }

    public static bool GetInitDownloadFailure()
    {
        return initDownloadFailure;
    }

    public bool GetDisplayLeaderboard()
    {
        return displayLeaderboard;
    }

    protected static event LeaderBoardRetrievedHandler onLeaderBoardRetrieved;

    public static void RegisterForLeaderBoardLoaded(LeaderBoardRetrievedHandler delg)
    {
        onLeaderBoardRetrieved += delg;
    }

    public static void UnregisterForLeaderBoardLoaded(LeaderBoardRetrievedHandler delg)
    {
        onLeaderBoardRetrieved -= delg;
    }

    // Use this for initialization
    private void Awake()
    {
        notify = new Notify(GetType().Name);
        //ProfileManager.RegisterForOnlineProfileLoaded(WaitForProfile);
    }

    /// <summary>
    ///     Logs the important info that all game play starts must see.  Try to keep this small
    /// </summary>
    private void LogImportantInfo()
    {
        notify.Info("client version = {0} platform={1} bundleId={2}",
            BundleInfo.GetBundleVersion(),
            Application.platform,
            BundleInfo.GetBundleId());
        notify.Info("server ={0} amps={1}",
            Settings.GetString("server-url", ""),
            Settings.GetString("amps-server", AssetBundleLoader.DefaultAmpsServer));
    }

    private void OnEnable()
    {
#if UNITY_IOS
    //GameCenterManager.playerAuthenticated += GameCenterPlayerAuthenticated;
    //GameCenterManager.playerFailedToAuthenticate += GameCenterPlayerFailedToAuthenticate;
    //GameCenterManager.playerDataLoaded += GameCenterFriendsLoaded;
    //GameCenterManager.loadPlayerDataFailed += GameCenterFriendsFailedToLoad;
#endif
    }

    private void OnDisable()
    {
#if UNITY_IOS
    //GameCenterManager.playerAuthenticated -= GameCenterPlayerAuthenticated;
    //GameCenterManager.playerFailedToAuthenticate -= GameCenterPlayerFailedToAuthenticate;
    //GameCenterManager.playerDataLoaded -= GameCenterFriendsLoaded;
    //GameCenterManager.loadPlayerDataFailed -= GameCenterFriendsFailedToLoad;
#endif
    }

    private void Start()
    {
        if (SharedInstance == null)
        {
            SharedInstance = this;
        }
        else
            notify.Warning("Multiple instances of Initializer running");

        GetInitFromServer();
    }

    public void GetInitFromServer()
    {
    }

    /*
	public void WaitForProfile ()
	{

	}
	*/

    private void SaveLastFriendCheck()
    {
        using (var stream = new MemoryStream())
        {
            var fileName = Application.persistentDataPath + Path.DirectorySeparatorChar
                           + "lastFriendCheck.txt";

            var saveDict = new Dictionary<string, object>();

            saveDict.Add("lastFriendCheck", lastFriendCheck);

            saveDict.Add("gamecenterId", mGameCenterId);
            saveDict.Add("fbAuthToken", mFacebookAccessToken);

            saveDict.Add("DMOId", _DMOId);

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
                notify.Warning("Error saving Last Friend Check {0}", ex);
            }
        }
    }

    public bool IsTimeToGetInit()
    {
        return true;
        //var tempDate = lastInitCall;

        //var delayMilliseconds = Settings.GetFloat("init-delay-time", 300000);

        //tempDate = tempDate.AddMilliseconds(delayMilliseconds);

        //notify.Debug("Initializer - IsTimeToGetInit Current: " + DateTime.UtcNow
        //             + " Last was: " + tempDate + " LastInit: " + lastInitCall);

        //if (DateTime.UtcNow > tempDate)
        //    return true;
        //return false;
    }

    private void LoadLastFriendCheck()
    {
        var fileName = Application.persistentDataPath + Path.DirectorySeparatorChar
                       + "lastFriendCheck.txt";

        if (File.Exists(fileName) == false)
        {
            notify.Debug("lastFriendCheck.txt not found");
            lastFriendCheck = DateTime.MinValue;
        }
        else
        {
            var reader = File.OpenText(fileName);
            var jsonString = reader.ReadToEnd();
            reader.Close();

            notify.Debug("Friend Check: " + jsonString);

            var loadedData =
                Json.Deserialize(jsonString) as Dictionary<string, object>;

            if (SaveLoad.Load(loadedData) == false)
            {
                return;
            }

            var dataDict = loadedData["data"]
                as Dictionary<string, object>;

            if (dataDict.ContainsKey("lastFriendCheck"))
            {
                lastFriendCheck = DateTime.Parse((string) dataDict["lastFriendCheck"]);
            }

            //if (dataDict.ContainsKey("gamecenterId") && dataDict["gamecenterId"] != null)
            //{
            //    tempGCId = dataDict["gamecenterId"].ToString();
            //}

            //if (dataDict.ContainsKey("fbAuthToken") && dataDict["fbAuthToken"] != null)
            //{
            //    tempFBToken = dataDict["fbAuthToken"].ToString();
            //}

            if (dataDict.ContainsKey("DMOId") && dataDict["DMOId"] != null)
            {
                _DMOId = dataDict["DMOId"].ToString();
            }

            notify.Debug("Last Friend check loaded");
        }
    }

    private bool IsTimeToRefreshLeaderBoard()
    {
        var tempDate = lastLeaderBoardCall;

        tempDate = tempDate.AddMinutes(Settings.GetInt("leaderboard-refresh-minutes", 2));

        notify.Debug("Initializer - Leaderboard Is Time? stored: " + tempDate + " Now: " + DateTime.UtcNow);

        if (DateTime.UtcNow > tempDate)
            return true;
        return false;
    }

    public void UpdateFacebookAccessToken(string accessToken)
    {
        mFacebookAccessToken = accessToken;
        notify.Debug("Added facebook access token: " + accessToken);

        FBPictureManager.FetchImageUrls();

        // GetLeaderBoards will refresh everything on UILeaderboardViewControllerOz, which causes a hitch
        // and artifacts if not on the leaderboard screen.
        if (LoadInitialLeaderboard)
        {
            GetLeaderBoards();
        }
        // GetInitFromServer will only update the data on the leoderboard.
        else
        {
            GetInitFromServer();
        }
    }

    private void SendDataToServer()
    {
        /*
		string jsonStr = ComposeJSONString( mDeviceId, mFacebookAccessToken, mGameCenterId, mGameCenterName, mGameCenterFriends );
		notify.Debug( "SendDataToServer: " + jsonStr );

		string hashSalt = Settings.GetString("hash-salt", "");

		if (hashSalt != "")
		{
			string hashMessage = SaveLoad.CheckState(jsonStr + hashSalt);
			Hashtable hashHeader = new Hashtable();
			hashHeader.Add("X_AUTHENTICATION", hashMessage);
			
			WWWForm postData = new WWWForm();

			postData.AddField("data", jsonStr);
			
			initIsDownloading = true;
	
			NetAgent.Submit(new NetRequest("/init", postData, GotInitReply, hashHeader));
		}
		
		lastInitCall = DateTime.UtcNow;
		lastLeaderBoardCall = DateTime.UtcNow;
		*/
    }

    private void GetGameCenterInfo()
    {
        /*
		// Get ID, name, and list of friends
		mGameCenterId = GameCenterBinding.playerIdentifier();
		mGameCenterName = GameCenterBinding.playerAlias();

		GameCenterBinding.retrieveFriends( false ); // Load profile pics = false
		*/
        // Wait for a "success" or "failure" response, and then call "SendDataToServer()"
    }

    private void GetLeaderBoards()
    {
        /*
		string jsonStr = ComposeJSONString( mDeviceId, mFacebookAccessToken, mGameCenterId, mGameCenterName, mGameCenterFriends );
		notify.Debug( "Initializer - GetLeaderBoards: " + jsonStr );

		string hashSalt = Settings.GetString("hash-salt", "");

		if (hashSalt != "")
		{
			string hashMessage = SaveLoad.CheckState(jsonStr + hashSalt);
			Hashtable hashHeader = new Hashtable();
			hashHeader.Add("X_AUTHENTICATION", hashMessage);
			
			WWWForm postData = new WWWForm();

			postData.AddField("data", jsonStr);
			
			initIsDownloading = true;
	
			NetAgent.Submit(new NetRequest("/init", postData, GotLeaderBoard, hashHeader));
		}
		
		lastInitCall = DateTime.UtcNow;
		lastLeaderBoardCall = DateTime.UtcNow;
		*/
    }

    public void RefreshLeaderboards()
    {
        notify.Debug("Initializer Refresh Leaderboards");

        if (IsTimeToRefreshLeaderBoard())
        {
            GetLeaderBoards();
        }
        else
        {
            notify.Debug("Initializer It isn't time to reload");
            if (onLeaderBoardRetrieved != null)
                onLeaderBoardRetrieved();
        }
    }

    private void _compareBuildVersion()
    {
        var buildVersion = BundleInfo.GetBundleVersion();

        var buildVersionArray = buildVersion.Split('.');
        var minAppVersionArray = minAppVersion.Split('.');

        try
        {
            for (var i = 0; i < buildVersionArray.Length; i++)
            {
                if (int.Parse(buildVersionArray[i]) < int.Parse(minAppVersionArray[i]))
                {
                    //BuildVersion is Less than the min app version; break and false
                    _isBuildVersion = false;
                    return;
                }
            }

            //If we did not return from the for loop, the build version is greater or equal to the min app
            _isBuildVersion = true;
        }
        catch (Exception ex)
        {
            notify.Error("[Initializer] _compareBuildVersion error comparing version numbers. " + ex.Message);
        }
    }

    public static bool IsBuildVersionPassThreshold()
    {
        return _isBuildVersion;
    }

    private void GameCenterPlayerAuthenticated()
    {
        notify.Debug("GameCenterPlayerAuthenticated");

        GetGameCenterInfo();
    }

    private void GameCenterPlayerFailedToAuthenticate(string error)
    {
        notify.Debug("GameCenterPlayerFailedToAuthenticate " + error);

        SendDataToServer();
    }

    private void GameCenterFriendsFailedToLoad(string error)
    {
        notify.Debug("GameCenterPlayerFailedToAuthenticate " + error);

        SendDataToServer();
    }

    public bool GotLeaderBoard(WWW www, bool noErrors, object results)
    {
        notify.Debug("GotLeaderBoard");
        if (noErrors)
        {
            notify.Debug("Got Leaderboard reply: " + www.text);

            var dataDict = Json.Deserialize(www.text) as Dictionary<string, object>;

            var responseCode = 400;

            if (dataDict != null)
            {
                if (dataDict.ContainsKey("responseCode"))
                {
                    responseCode = JSONTools.ReadInt(dataDict["responseCode"]);
                }
                var restoreProfile = false;

                /*
				if (dataDict.ContainsKey("profileRestore"))
				{
					notify.Debug("Initializer restoreProfile: " + dataDict["profileRestore"].ToString());
					
					restoreProfile = bool.Parse(dataDict["profileRestore"].ToString());
				}
				*/

                if (dataDict.ContainsKey("leaderboard"))
                {
                    notify.Debug("GotLeaderBoard Initializer leaderboard display: " + displayLeaderboard);

                    displayLeaderboard = bool.Parse(dataDict["leaderboard"].ToString());
                }
                else
                {
                    notify.Debug("GotLeaderBoard Initializer could not find leaderboard.");
                }

                notify.Debug("GotLeaderBoard Good Leaderboard: " + displayLeaderboard);


                if (dataDict.ContainsKey("account"))
                {
                    var profileList = dataDict["account"] as List<object>;

                    notify.Debug("ProfileList Count: " + profileList.Count);

                    //If we did get a profile, then apply it.
                    if (profileList != null && profileList.Count == 1 && profileList[0] != null)
                    {
                        //Services.Get<ProfileManager>().ApplyProfileFromInit(profileList, responseCode, restoreProfile);

                        ProfileManager.SharedInstance.ApplyProfileFromInit(profileList, responseCode, restoreProfile);

                        //After applying the user profile, cycle through all of the guildChallenges
                        // in the weekly objectives.
                        //Services.Get<ObjectivesManager>().AddNeighborStats();
                    }
                }
            }

            initDownloadSuccess = true;
        }
        /*
		else 
		{
			//Apply local information as we did not have a connection.
			Services.Get<NewsManager>().ApplyGameTipForEmptyNews();
			Services.Get<ObjectivesManager>().LoadLocalWeeklyChallenge();
			Services.Get<ObjectivesManager>().RemoveExpiredWeeklyChallenges();			
			initDownloadFailure = true;
		}
		*/

        Debug.Log("[Initializer] got initreply");

        if (onLeaderBoardRetrieved != null)
        {
            notify.Debug("Initializer - On Load Leaderboard delegate called");

            onLeaderBoardRetrieved();
        }


        initIsDownloading = false;

        return true;
    }

    /// <summary>
    ///     Process the init reply from the servers
    /// </summary>
    public bool GotInitReply(WWW www, bool noErrors, object results)
    {
        if (noErrors)
        {
            notify.Debug("Got Init reply: " + www.text);

            var dataDict = Json.Deserialize(www.text) as Dictionary<string, object>;

            var responseCode = 400;

            var objManager = Services.Get<ObjectivesManager>();

            if (dataDict != null)
            {
                if (dataDict.ContainsKey("responseCode"))
                {
                    responseCode = JSONTools.ReadInt(dataDict["responseCode"]);
                }


                // Moved settings to be processed second, after deserialization of responseCode
                if (dataDict.ContainsKey("settings"))
                {
                    var settingsList = dataDict["settings"] as List<object>;

                    notify.Debug("[Initializer] GotInitReply - Got settings");

                    Services.Get<ServerSettings>().ApplyServerSettings(settingsList, responseCode);
                }

                var restoreProfile = false;

                if (dataDict.ContainsKey("profileRestore"))
                {
                    notify.Debug("Initializer restoreProfile: " + dataDict["profileRestore"]);

                    restoreProfile = bool.Parse(dataDict["profileRestore"].ToString());
                }

                if (dataDict.ContainsKey("leaderboard"))
                {
                    notify.Debug("GotInitReply Initializer leaderboard display: " + displayLeaderboard);

                    displayLeaderboard = bool.Parse(dataDict["leaderboard"].ToString());
                }
                else
                {
                    notify.Debug("GotInitReply Initializer could not find leaderboard.");
                }


                if (dataDict.ContainsKey("minAppVersion"))
                {
                    notify.Debug("Initializer: Deserialize minAppVersion");
                    minAppVersion = dataDict["minAppVersion"].ToString();

                    notify.Info("Initializer: minAppVersion: " + dataDict["minAppVersion"]);

                    //Compare the build version number against the minAppVersion
                    _compareBuildVersion();
                }

                notify.Debug("GotInitReply Good Leaderboard: " + displayLeaderboard);

                if (dataDict.ContainsKey("sales"))
                {
                    // pass the contents of that substring  to WeeklyDiscountManager
                    var sales = dataDict["sales"] as List<object>;

                    Services.Get<Store>().ApplyDiscountFromInit(sales, responseCode);
                }

                if (IsBuildVersionPassThreshold())
                {
                    if (dataDict.ContainsKey("challenges"))
                    {
                        // pass the contents of that substring  to WeeklyObjectives
                        var challengeList = dataDict["challenges"] as List<object>;

                        notify.Debug("Challenges: " + Json.Serialize(dataDict["challenges"]));

                        objManager.ApplyWeeklyChallenge(challengeList, responseCode);
                    }
                }

                if (dataDict.ContainsKey("news"))
                {
                    // pass the contents of that substring  to NewsManager.cs
                    var news = dataDict["news"] as List<object>;
                    Services.Get<NewsManager>().ApplyNewsFromInit(news, responseCode);
                }


                if (dataDict.ContainsKey("account"))
                {
                    var profileList = dataDict["account"] as List<object>;

                    notify.Debug("ProfileList Count: " + profileList.Count);

                    //If we did get a profile, then apply it.
                    if (profileList != null && profileList.Count == 1 && profileList[0] != null)
                    {
                        //Services.Get<ProfileManager>().ApplyProfileFromInit(profileList, responseCode, restoreProfile);

                        ProfileManager.SharedInstance.ApplyProfileFromInit(profileList, responseCode, restoreProfile);

                        //After applying the user profile, cycle through all of the guildChallenges
                        // in the weekly objectives.
                        //Services.Get<ObjectivesManager>().AddNeighborStats();

                        var fbAccessToken = Settings.GetString("fb-access-token", "");

                        if (fbAccessToken != "")
                        {
                            FBPictureManager.FetchImageUrls();
                        }
                    }
                }
            }

            initDownloadSuccess = true;
        }
        else
        {
            //Apply local information as we did not have a connection.
            Services.Get<ServerSettings>().ApplyServerSettingsLocalCopy();
            Services.Get<NewsManager>().ApplyGameTipForEmptyNews();
            Services.Get<ObjectivesManager>().LoadLocalWeeklyChallenge();
            Services.Get<ObjectivesManager>().RemoveExpiredWeeklyChallenges();
            initDownloadFailure = true;
        }

        // if we see minAppVersion in the console, then we know we got the initReply		
        notify.Debug("[Initializer] got initreply");
        //var isStartOfSession = true;

        initIsDownloading = false;


        /*
		if (
			onLeaderBoardRetrieved != null
			&& GameController.SharedInstance != null
			&& UIManagerOz.SharedInstance != null
			&& GameController.SharedInstance.IsSafeToLaunchDownloadDialog()
		) {
			//onLeaderBoardRetrieved();
			
			Services.Get<LeaderboardManager>().RefreshLeaderboards();
		}
		*/
        Services.Get<LeaderboardManager>().RefreshLeaderboards();

        return true;
    }

    public void CheckForChallenges()
    {
        if (GameController.SharedInstance.IsSafeToUpdateWeeklyObjectives())
        {
            if (Services.ServicesActive)
            {
                Services.Get<ObjectivesManager>().GetWeeklyObjectivesClass().CheckExpired();
            }

            notify.Debug("[Initializer] CheckForChallenges Could Check @ " + Time.frameCount);

            // Challenges need to be updated because we have either none, or they are expired
            if (WeeklyObjectives.WeeklyObjectiveCount == 0
                || WeeklyObjectives.AreChallengesExpired
                )
            {
                if (!_wantQuery)
                {
                    notify.Debug("[Initializer] CheckForChallenges queuing update @ " + Time.frameCount);
                }
                _wantQuery = true;
            }
        }
        //We Have good data, so reset the query delay
        else
        {
            if (_queryDelay < _maxQueryDelay)
            {
                _queryDelay = new TimeSpan(0, 5, 0);
            }
        }

        // Check if challenges are now available.
        if (_wantQuery)
        {
            if (DateTime.UtcNow - _lastQueryTime >= _queryDelay)
            {
                // Allow the request to go through if we are not already downloading
                if (!GetInitIsDownloading())
                {
                    notify.Debug("[Initializer] CheckForChallenges Requesting challenge update after " + _queryDelay);
                    _wantQuery = false;
                    _queryDelay.Add(_queryDelay);
                    _getChallenges();
                }
            }
        }
    }

    public void OnAppForegroundRefreshChallenges()
    {
        _getChallenges();
    }

    private void _getChallenges()
    {
        _lastQueryTime = DateTime.UtcNow;

        var languageCode = Localization.SharedInstance.GetISO1LanguageCode();

        // If we only pass the language short code to the server, the server will not attempt to retrieve account
        //  information, returning only challenges, sales, and news from memcached. The reduces load and provides
        //  a faster response.
        var jsonStr = "{\"lang\":\"" + languageCode + "\"}";

        notify.Debug("[Initializer] - _getChallenges - JSON String: " + jsonStr);

        var hashSalt = Settings.GetString("hash-salt", "");

        if (hashSalt != "")
        {
            var hashMessage = SaveLoad.CheckState(jsonStr + hashSalt);
            var hashHeader = new Hashtable();
            hashHeader.Add("X_AUTHENTICATION", hashMessage);

            var postData = new WWWForm();

            postData.AddField("data", jsonStr);

            initIsDownloading = true;

            NetAgent.Submit(new NetRequest("/init", postData, _gotTheChallenges, hashHeader));
        }
    }

    private bool _gotTheChallenges(WWW www, bool noErrors, object results)
    {
        if (noErrors)
        {
            notify.Debug("Got Init reply: " + www.text);

            var dataDict = Json.Deserialize(www.text) as Dictionary<string, object>;

            var responseCode = 400;

            var objManager = Services.Get<ObjectivesManager>();

            if (dataDict != null)
            {
                if (dataDict.ContainsKey("responseCode"))
                {
                    responseCode = JSONTools.ReadInt(dataDict["responseCode"]);
                }


                if (dataDict.ContainsKey("minAppVersion"))
                {
                    notify.Debug("[Initializer] Deserialize minAppVersion");
                    minAppVersion = dataDict["minAppVersion"].ToString();

                    notify.Debug("Initializer: minAppVersion: " + dataDict["minAppVersion"]);

                    _compareBuildVersion();
                }

                if (IsBuildVersionPassThreshold())
                {
                    if (dataDict.ContainsKey("challenges"))
                    {
                        // pass the contents of that substring  to WeeklyObjectives
                        var challengeList = dataDict["challenges"] as List<object>;

                        notify.Debug("Challenges: " + Json.Serialize(dataDict["challenges"]));

                        objManager.ApplyWeeklyChallenge(challengeList, responseCode);
                    }
                }
            }

            initDownloadSuccess = true;
        }
        else
        {
            //Apply local information as we did not have a connection.
            Services.Get<NewsManager>().ApplyGameTipForEmptyNews();
            Services.Get<ObjectivesManager>().LoadLocalWeeklyChallenge();
            Services.Get<ObjectivesManager>().RemoveExpiredWeeklyChallenges();
            initDownloadFailure = true;
        }


        Debug.Log("[Initializer] got the challenges");

        initIsDownloading = false;

        return true;
    }

    // Update is called once per frame
    private void Update()
    {
    }
}