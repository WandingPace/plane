using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

/// <summary>
///     Implements oz specific console commands
/// </summary>
public class DebugConsoleOz : DebugConsole
{
    public new static DebugConsoleOz Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new DebugConsoleOz();
            }
            notify.Debug("returning " + _instance.GetType().Name);
            return (DebugConsoleOz) _instance;
        }
    }

    static DebugConsoleOz()
    {
        notify.Debug("start of debugconsoleoz");
        _instance = new DebugConsoleOz();

        notify.Debug("_instance = " + _instance);
    }

    public static void Init()
    {
        // empty but will fire off the static constructor
    }


    private static int page;
    private static int count;

    public new static void OnGUI()
    {
        DebugConsole.OnGUI();

        if (showTextEdit)
        {
            count = 0;
            GUILayout.Space(10f);
            GUILayout.Label("Shortcuts:");
            ShortcutButton("rich");
            ShortcutButton("poor");
            ShortcutButton("sbs", "invulnerable true");
            ShortcutButton("sbs", "invulnerable false");
            ShortcutButton("fillcoinmeter");
            ShortcutButton("ww");
            ShortcutButton("df");
            //	ShortcutButton("ybr");
            ShortcutButton("completeobjective", "0");
            ShortcutButton("completeobjective", "1");
            ShortcutButton("completeobjective", "2");
            ShortcutButton("completecurrentobjectives");
            ShortcutButton("qinfo");
            ShortcutButton("gatcha");
            ShortcutButton("unlockallobjectives");
            ShortcutButton("revertallobjectives");
            ShortcutButton("sethighdroprate");
            ShortcutButton("setexclusivedrop", "boost");
            ShortcutButton("setexclusivedrop", "poof");
            ShortcutButton("setexclusivedrop", "scorebonus");
            ShortcutButton("setexclusivedrop", "coinbonus");
            ShortcutButton("setexclusivedrop", "vacuum");
            ShortcutButton("completeobjdiff1to3");
            ShortcutButton("completeobjdiff4to5");
            ShortcutButton("completeobjdiff6to7");
            ShortcutButton("addmillionpoints");
            //ShortcutButton("addmillioncoins");
            ShortcutButton("popuptest");
            ShortcutButton("objpopuptest");
            ShortcutButton("distpopuptest");
            ShortcutButton("addcoins", "2350");
            ShortcutButton("addcoins", "22000");
            ShortcutButton("addcoins", "974500");
            ShortcutButton("addcoins", "8999500");
            ShortcutButton("addmeters", "42000");
            ShortcutButton("addmeters", "207000");
            ShortcutButton("addmeters", "749000");
            ShortcutButton("addmeters", "15933000");
            ShortcutButton("gamecenterresetachievements");
            ShortcutButton("fbscorereset");
            ShortcutButton("fbaccesstoken");
            ShortcutButton("callinit");


            GUILayout.BeginHorizontal();
            if (GUILayout.Button("<<", GUILayout.Width(75), GUILayout.Height(35f*Screen.height/1000f)))
            {
                page--;
            }
            if (GUILayout.Button(">>", GUILayout.Width(75), GUILayout.Height(35f*Screen.height/1000f)))
            {
                page++;
            }
            GUILayout.EndHorizontal();
            GUILayout.Label("Page: " + page);
        }
    }

    public static void ShortcutButton(string fn, string cmd = "")
    {
        if (count/10 == page)
        {
            if (GUILayout.Button(fn + " -" + cmd, GUILayout.Width(160), GUILayout.Height(35f*Screen.height/1000f)))
            {
                runCommand(fn + " " + cmd);
            }
        }
        count++;
    }


    /// <summary>
    ///     short for run info, displays the extra tracking info for the run
    /// </summary>
    /// <param name='cmd'>
    ///     unused
    /// </param>
    private static bool ri(string cmd)
    {
        typedString = "Run Info = " + StatTracker.GetStatsString();
        return true;
    }

    /// <summary>
    ///     short for tunnel transition info, displays the times we spent in the tunnel
    /// </summary>
    /// <param name='cmd'>
    ///     unused
    /// </param>
    private static bool tti(string cmd)
    {
        typedString = "Tunnel Transition Info = " + EnvironmentSetSwitcher.SharedInstance.GetTunnelTimes();
        return true;
    }

    /// <summary>
    ///     temp workaround as gui is not yet working
    /// </summary>
    /// <param name='cmd'>
    ///     unused
    /// </param>
    private static bool play(string cmd)
    {
        GameController.SharedInstance.OnRestartClickedUI();
        return true;
    }


    /// <summary>
    ///     Loads the balloon scene
    /// </summary>
    /// <param name='cmd'>
    ///     not used
    /// </param>
    private static bool bs(string cmd)
    {
        //Settings.SetBool("balloon-enabled", true);
        PoolManager.Pools.DestroyAll();
        DynamicElement.loaded_prefabs = new Dictionary<string, PrefabPool>();
        Application.LoadLevel("sandbox_Balloon");
        typedString = "loading sandbox_Balloon scene... ";
        return true;
    }

    /// <summary>
    ///     switch to dark forest pieces
    /// </summary>
    /// <param name='cmd'>
    ///     unused
    /// </param>
    private static bool df(string cmd)
    {
        Settings.SetInt("starting-envset", 2);
        typedString = "loading df pieces...";
        Application.LoadLevel("OzGame");
        PopupNotification.PopupList.Clear();
        //typedString = "dl on";
        return true;
    }

    /// <summary>
    ///     switch to whimsey woods pieces
    /// </summary>
    /// <param name='cmd'>
    ///     unused
    /// </param>
    private static bool ww(string cmd)
    {
        Settings.SetInt("starting-envset", 1);
        typedString = "loading ww pieces...";
        Application.LoadLevel("OzGame");
        return true;
    }

    /// <summary>
    ///     switch to yellow brick road pieces
    /// </summary>
    /// <param name='cmd'>
    ///     unused
    /// </param>
    private static bool ybr(string cmd)
    {
        Settings.SetInt("starting-envset", 3);
        typedString = "loading ybr pieces...";
        Application.LoadLevel("OzGame");
        return true;
    }

    private static bool ec(string cmd)
    {
        Settings.SetInt("starting-envset", 4);
        typedString = "loading ec pieces...";
        Application.LoadLevel("OzGame");
        return true;
    }

    /// <summary>
    ///     switch to machu pieces
    /// </summary>
    /// <param name='cmd'>
    ///     unused
    /// </param>
    private static bool machu(string cmd)
    {
        Settings.SetInt("starting-envset", 0);
        typedString = "loading machus pieces...";
        Application.LoadLevel("OzGame");
        return true;
    }

    /// <summary>
    ///     add one million coins and gems
    /// </summary>
    /// <param name='cmd'>
    ///     unused
    /// </param>
    private static bool rich(string cmd)
    {
        typedString = "You are now rich as hell!";
        GameProfile.SharedInstance.Player.coinCount = 1000000;
        GameProfile.SharedInstance.Player.specialCurrencyCount = 50000;
        UIManagerOz.SharedInstance.PaperVC.UpdateCurrency();
        return true;
    }


    /// <summary>
    ///     remove all coins and gems
    /// </summary>
    /// <param name='cmd'>
    ///     unused
    /// </param>
    private static bool poor(string cmd)
    {
        typedString = "You are now dirt poor!";
        GameProfile.SharedInstance.Player.coinCount = 0;
        GameProfile.SharedInstance.Player.specialCurrencyCount = 0;
        return true;
    }

    /// <summary>
    ///     reset and save game profile
    /// </summary>
    /// <param name='cmd'>
    ///     unused
    /// </param>
    private static bool reset(string cmd)
    {
        var gp = Object.FindObjectOfType(typeof (GameProfile)) as GameProfile;
        if (gp)
        {
            GameProfile.SharedInstance = gp;
        }
        else
        {
            typedString = "ERROR: Couldn't find the GameProfile Object in the Scene";
            return false;
        }

        if (GameProfile.SharedInstance.Characters != null)
        {
            GameProfile.SharedInstance.Characters.Clear();
            GameProfile.SharedInstance.SetupDefaultCharacters();
        }

        GameProfile.SharedInstance.Reset();
        GameProfile.SharedInstance.Serialize();
        if (Application.isPlaying)
        {
            //GameProfile.SharedInstance.Deserialize();
        }
        typedString = "The game profile has successfully been reset and saved! Close down the game NOW!!";
        return true;
    }

    /// <summary>
    ///     Fast travel  command
    /// </summary>
    /// <param name='cmd'>
    ///     expected parameters is the setcode, e.g ybr, df,ww
    /// </param>
    private static bool ft(string cmd)
    {
        var result = false;
        var splits = cmd.Split(' ');
        if (splits.Length < 2)
        {
            typedString = "Fast Travel usage : ft <abbreviated environment set code>";
        }
        else if (GameController.SharedInstance.IsGameStarted &&
                 !(GameController.SharedInstance.IsGameStarted && GameController.SharedInstance.IsGameOver))
        {
            typedString = "You can not fast travel while you are playing";
        }
        else
        {
            var destCode = splits[1];
            EnvironmentSetData envSetData = null;
            if (EnvironmentSetManager.SharedInstance.LocalCode2Dict.TryGetValue(destCode, out envSetData))
            {
                if (EnvironmentSetManager.SharedInstance.CurrentEnvironmentSet.SetCode != destCode)
                {
                    var distance = 1000f;
                    if (splits.Length >= 3)
                    {
                        float.TryParse(splits[2], out distance);
                    }
                    GamePlayer.SharedInstance.StartFastTravel(envSetData.SetId); //, distance);
                    typedString = "Success, press play now";
                    result = true;
                }
                else
                {
                    typedString = "You can't fast travel to the environment set you are in";
                }
            }
            else
            {
                typedString = "Environment Set " + destCode + " not found";
            }
        }
        return result;
    }

    /// <summary>
    ///     Change Language
    /// </summary>
    /// <param name='Lang'>
    ///     expected parameters is the setcode, e.g ybr, df,ww
    /// </param>
    private static bool lang(string cmd)
    {
        var result = false;
        var splits = cmd.Split(' ');
        if (splits.Length < 2)
        {
            typedString = "lang usage : lang <en,fr,nl,de,it,es,ru,ja,zh,zht,pt,ko>";
        }
        else
        {
            var lang = splits[1];
            switch (lang)
            {
                case "en":
                    Localization.SharedInstance.SetLanguage("English");
                    break;
                case "fr":
                    Localization.SharedInstance.SetLanguage("French");
                    break;
                case "nl":
                    Localization.SharedInstance.SetLanguage("Dutch");
                    break;
                case "de":
                    Localization.SharedInstance.SetLanguage("German");
                    break;
                case "it":
                    Localization.SharedInstance.SetLanguage("Italian");
                    break;
                case "es":
                    Localization.SharedInstance.SetLanguage("Spanish");
                    break;
                case "ru":
                    Localization.SharedInstance.SetLanguage("Russian");
                    break;
                case "ja":
                    Localization.SharedInstance.SetLanguage("Japanese");
                    break;
                case "zh":
                    Localization.SharedInstance.SetLanguage("Chinese");
                    break;
                case "zht":
                    Localization.SharedInstance.SetLanguage("Chinese_Traditional");
                    break;
                case "pt":
                    Localization.SharedInstance.SetLanguage("Portuguese");
                    break;
                case "ko":
                    Localization.SharedInstance.SetLanguage("Korean");
                    break;
            }
        }
        return result;
    }

    /// <summary>
    ///     Download Asset Bundle
    /// </summary>
    private static bool dab(string cmd)
    {
        var result = false;
        var splits = cmd.Split(' ');
        if (splits.Length < 2)
        {
            typedString = "download asset bundle usage : dab <df, ybr, >";
        }
        else
        {
            var code = splits[1];
            switch (code)
            {
                case "df":
                    ResourceManager.SharedInstance.LoadAssetBundle(
                        EnvironmentSetManager.SharedInstance.GetAssetBundleName(EnvironmentSetManager.DarkForestId)
                        , true, -1, false, false);
                    break;
                case "ybr":
                    ResourceManager.SharedInstance.LoadAssetBundle(
                        EnvironmentSetManager.SharedInstance.GetAssetBundleName(EnvironmentSetManager.YellowBrickRoadId),
                        true, -1, false, false);
                    break;
                case "ec":
                    ResourceManager.SharedInstance.LoadAssetBundle(
                        EnvironmentSetManager.SharedInstance.GetAssetBundleName(EnvironmentSetManager.EmeraldCityId),
                        true, -1, false, false);
                    break;
            }
        }
        return result;
    }

    private static bool kill(string cmd)
    {
        var consoleGUI = Services.Get<DebugConsoleOzGui>();
        if (consoleGUI != null)
        {
            consoleGUI.enabled = false;
        }
        var fpsGUI = Services.Get<HUDFPSUnityGui>();
        if (fpsGUI != null)
        {
            fpsGUI.enabled = false;
        }
        if (UIManagerOz.SharedInstance.inGameVC != null)
        {
            UIManagerOz.SharedInstance.inGameVC.StopShowingDebugDistance();
        }
        return true;
    }

    /// <summary>
    ///     Deliberately crash (to test Crittercism's crash handling)
    /// </summary>
    /// <param name='cmd'>
    ///     unused
    /// </param>
    private static bool crash(string cmd)
    {
        typedString = "Crashing...";

        string uninitializedString = null;
        uninitializedString = uninitializedString.ToLower();

        return true;
    }

    /// <summary>
    ///     Throw an exception (to test Crittercism's exception handling)
    /// </summary>
    /// <param name='cmd'>
    ///     unused
    /// </param>
    private static bool throwex(string cmd)
    {
        typedString = "Throwing an exception...";

        throw new Exception("Unhandled Exception");

#pragma warning disable 0162
        return true; // Unreachable code
#pragma warning restore 0162
    }

    /// <summary>
    ///     Log in to Facebook
    /// </summary>
    /// <param name='cmd'>
    ///     unused
    /// </param>
    private static bool fblogin(string cmd)
    {
        typedString = "Logging in to Facebook...";


        return true;
    }

    /// <summary>
    ///     Log out of Facebook
    /// </summary>
    /// <param name='cmd'>
    ///     unused
    /// </param>
    private static bool fblogout(string cmd)
    {
        typedString = "Logging out of Facebook...";


        return true;
    }

    private static bool fbscorereset(string cmd)
    {
        typedString = "Resetting Facebook top score entry...";


        return true;
    }

    /// <summary>
    ///     Displays the facebook access token in the cheat console
    ///     useful to get the info from the device, then enter it in
    ///     for the fb-access-token setting so we can see leaderboard
    ///     entries in unity editor
    /// </summary>
    /// <param name='cmd'>
    ///     unused
    /// </param>
    private static bool fbaccesstoken(string cmd)
    {
        if (Settings.GetString("fb-access-token", "") != "")
        {
            typedString = Settings.GetString("fb-access-token", "");
        }
        else
        {
            typedString = "Error IsFacebookSessionValid is false";
        }

        return true;
    }

    private static bool callinit(string cmd)
    {
        if (Application.internetReachability != NetworkReachability.NotReachable)
        {
            Initializer.SharedInstance.GetInitFromServer();

            typedString = "Retrieving server data";
        }
        else
        {
            typedString = "No network connectivity";
        }

        return true;
    }

    /// <summary>
    ///     Add a way for other scripts to put stuff into the consoles
    /// </summary>
    /// <param name='cmd'>
    ///     whatever
    /// </param>
    public static bool echo(string str)
    {
        typedString = str;
        return true;
    }

    private static bool unlockallobjectives(string cmd)
    {
        GameProfile.SharedInstance.Player.objectivesEarned.Clear();
        for (var i = 0; i < ObjectivesManager.MainObjectives.Count; i++)
            GameProfile.SharedInstance.Player.objectivesEarned.Add(ObjectivesManager.MainObjectives[i]._id);
        typedString = "All Objectives Complete!";
        return true;
    }

    private static bool revertallobjectives(string cmd)
    {
        GameProfile.SharedInstance.Player.objectivesEarned.Clear();
        typedString = "All Objectives Reverted...";
        return true;
    }

    private static bool completecurrentobjectives(string cmd)
    {
        foreach (var data in GameProfile.SharedInstance.Player.objectivesMain)
        {
            if (data != null && data._conditionList != null)
                data._conditionList[0]._earnedStatValue = data._conditionList[0]._statValue;
        }
        typedString = "Current Objectives Complete!";
        return true;
    }

    private static bool completeobjective(string cmd)
    {
        var ind = -1;
        var parts = cmd.Split(' ');

        if (parts.Length > 1)
            int.TryParse(parts[1], out ind);
        else
            return false;

        if (ind >= 0 && ind < GameProfile.SharedInstance.Player.objectivesMain.Count)
        {
            var data = GameProfile.SharedInstance.Player.objectivesMain[ind];
            if (data != null)
            {
                data._conditionList[0]._earnedStatValue = data._conditionList[0]._statValue;
                typedString = "Objective " + ind + " Complete!";
                return true;
            }
        }
        return false;
    }

    private static bool fillcoinmeter(string cmd)
    {
        GamePlayer.SharedInstance.AddPointsToPowerMeter(1000f);
        typedString = "Coin meter filled!";
        return true;
    }

    private static bool popuptest(string cmd)
    {
        PopupNotification.PopupList[PopupNotificationType.Generic].Show(cmd);
        return true;
    }

    private static bool distpopuptest(string cmd)
    {
        PopupNotification.PopupList[PopupNotificationType.Generic].Show(cmd, true);
        return true;
    }

    private static bool objpopuptest(string cmd)
    {
        PopupNotification.PopupList[PopupNotificationType.Objective].Show(cmd);
        return true;
    }

    private static bool qinfo(string cmd)
    {
        typedString = "UI [" + UIManagerOz.SharedInstance.InterfaceMaster.spriteMaterial.name +
                      " shader [" + UIManagerOz.SharedInstance.InterfaceMaster.spriteMaterial.shader.name + " ] " +
                      "] UI2 [" + UIManagerOz.SharedInstance.InterfaceMaster2.spriteMaterial.name +
                      " shader [" + UIManagerOz.SharedInstance.InterfaceMaster2.spriteMaterial.shader.name + " ] " +
                      "] \nEnvTex [" + GameController.SharedInstance.TrackMaterials.Opaque.name + "]" +
                      " Quality [" + QualitySettings.GetQualityLevel() + "]";
        return true;
    }

    private static bool gatcha(string cmd)
    {
        GameProfile.SharedInstance.Player.AddChanceToken();
        return true;
    }

    //private static bool setexclusivedrop(string cmd)
    //{
    //    BonusItemProtoData.SharedInstance.ProbabilityBoost = 0f;
    //    BonusItemProtoData.SharedInstance.ProbabilityCoinBonus = 0f;
    //    BonusItemProtoData.SharedInstance.ProbabilityPoof = 0f;
    //    BonusItemProtoData.SharedInstance.ProbabilityVacuum = 0f;
    //    BonusItemProtoData.SharedInstance.ProbabilityScoreBonus = 0f;

    //    string[] parts = cmd.Split(' ');
    //    if(parts.Length>1)
    //    {
    //        string val = parts[1];
    //        switch(val)
    //        {
    //        case "boost":		BonusItemProtoData.SharedInstance.ProbabilityBoost = 1f;		break;
    //        case "coinbonus":	BonusItemProtoData.SharedInstance.ProbabilityCoinBonus = 1f;	break;
    //        case "poof":		BonusItemProtoData.SharedInstance.ProbabilityPoof = 1f;			break;
    //        case "vacuum":		BonusItemProtoData.SharedInstance.ProbabilityVacuum = 1f;		break;
    //        case "scorebonus":	BonusItemProtoData.SharedInstance.ProbabilityScoreBonus = 1f;	break;
    //        default:			typedString = "Invalid parameter! No bonus items will show up.";break;
    //        }
    //        return true;
    //    }
    //    return false;
    //}

    private static bool sethighdroprate(string cmd)
    {
        BonusItemProtoData.SharedInstance.MinDistanceBetweenBonusItems = 20f;
        BonusItemProtoData.SharedInstance.MinDistanceBetweenGems = 20f;
        BonusItemProtoData.SharedInstance.MinDistanceBetweenTornadoTokens = 20f;
        return true;
    }

    private static bool completeobjdiff1to3(string cmd)
    {
        foreach (var opd in ObjectivesManager.MainObjectives)
        {
            if (opd._difficulty >= ObjectiveDifficulty.Difficulty1 && opd._difficulty <= ObjectiveDifficulty.Difficulty3)
            {
                if (!GameProfile.SharedInstance.Player.objectivesEarned.Contains(opd._id))
                    GameProfile.SharedInstance.Player.objectivesEarned.Add(opd._id);
            }
        }
        typedString =
            "Objectives of difficulty 1 through 3 have been marked complete! Complete current objectives to get new ones.";
        return true;
    }

    private static bool completeobjdiff4to5(string cmd)
    {
        foreach (var opd in ObjectivesManager.MainObjectives)
        {
            if (opd._difficulty >= ObjectiveDifficulty.Difficulty4 && opd._difficulty <= ObjectiveDifficulty.Difficulty5)
            {
                if (!GameProfile.SharedInstance.Player.objectivesEarned.Contains(opd._id))
                    GameProfile.SharedInstance.Player.objectivesEarned.Add(opd._id);
            }
        }
        typedString =
            "Objectives of difficulty 4 through 5 have been marked complete! Complete current objectives to get new ones.";
        return true;
    }

    private static bool completeobjdiff6to7(string cmd)
    {
        foreach (var opd in ObjectivesManager.MainObjectives)
        {
            if (opd._difficulty >= ObjectiveDifficulty.Difficulty6 && opd._difficulty <= ObjectiveDifficulty.Difficulty7)
            {
                if (!GameProfile.SharedInstance.Player.objectivesEarned.Contains(opd._id))
                    GameProfile.SharedInstance.Player.objectivesEarned.Add(opd._id);
            }
        }
        typedString =
            "Objectives of difficulty 6 through 7 have been marked complete! Complete current objectives to get new ones.";
        return true;
    }

//	private static bool addmillioncoins(string cmd)
//	{
//		GamePlayer.SharedInstance.AddCoinsToScore(1000000);
//		return true;
//	}

    private static bool addmillionpoints(string cmd)
    {
        GamePlayer.SharedInstance.AddScore(975000, true);
        return true;
    }

    private static bool addcoins(string cmd)
    {
        var num = 0;
        var parts = cmd.Split(' ');

        if (parts.Length > 1)
            int.TryParse(parts[1], out num);
        else
            return false;

        GamePlayer.SharedInstance.AddCoinsToScore(num);
        GameController.SharedInstance.collectedBonusItemPerRun[(int) BonusItem.BonusItemType.Coin] += num;

        return true;
    }

    private static bool addmeters(string cmd)
    {
        var num = 0;
        var parts = cmd.Split(' ');

        if (parts.Length > 1)
            int.TryParse(parts[1], out num);
        else
            return false;

        GameController.SharedInstance.ManipulateDistance(num);

        return true;
    }

    private static bool gamecenterresetachievements(string cmd)
    {
        typedString = "Resetting GameCenter achievements";

        return true;
    }

    private static bool di(string cmd)
    {
        typedString = "Unity Device Id = ";
        typedString += "\n\n";
        typedString += "Disney Mobile Id = " + ProfileManager.SharedInstance.userServerData._dbId;
        return true;
    }


#if !UNITY_EDITOR && UNITY_IPHONE
	[DllImport("__Internal")]
	public static extern void setUrbanAirshipAlias(string alias);
#else
    public static void setUrbanAirshipAlias(string alias)
    {
        // not yet implemented
        typedString = "setUrbanAirshipAlias not yet implemented for this platform";
    }
#endif

    /// <summary>
    ///     UAA stands for urban airship alias
    /// </summary>
    /// <param name='cmd'>
    ///     2nd parameter should be the name
    /// </param>
    private static bool uaa(string cmd)
    {
        var splits = cmd.Split(' ');
        if (splits.Length < 2)
        {
            typedString = "Urban Airship Alias usage : uaa <firstname_and_lastname_NO_spaces>";
        }
        else
        {
            var alias = splits[1].ToLower();
            typedString = "setting urban airship alias to " + alias;
            setUrbanAirshipAlias(alias);
        }
        return true;
    }


    /// <summary>
    ///     UAT stands for urban airship token, spits it out in the command console
    /// </summary>
    /// <param name='cmd'>
    ///     not used
    /// </param>
    private static bool uat(string cmd)
    {
        return true;
    }

    /// UAP stands for urban airship payload, spits it out in the command console
    /// </summary>
    /// <param name='cmd'>
    ///     not used
    /// </param>
    private static bool uap(string cmd)
    {
        return true;
    }

    /// <summary>
    ///     Set Facebook access token
    /// </summary>
    /// <param name='cmd'>
    ///     2nd parameter should be the Facebook access token
    /// </param>
    private static bool setfbtoken(string cmd)
    {
        var splits = cmd.Split(' ');
        if (splits.Length < 2)
        {
            typedString = "Set Facebook Token usage: setfbtoken <very_long_alphanumeric_string>";
        }
        else
        {
            //var token = splits[1].ToLower();

            typedString = "Facebook access token has been set";
        }
        return true;
    }

#if UNITY_EDITOR
    private static bool unittest(string cmd)
    {
        DownloadedAssetBundleTracker.UnitTest();
        return true;
    }
#endif
}