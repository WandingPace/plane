using System.Collections.Generic;
using UnityEngine;

public enum SocialScreenName
{
    TopDistances,
    TopScores
} //SocialConnections, TopScores, TopDistances, Challenges }
//public enum LeaderboardType { Distances, Scores, }			//Scores, Distances }

public class UILeaderboardViewControllerOz : UIViewControllerOz
{
    private const int kLeaderboardLabelCount = 99; //Int32.MaxValue;  //10; // Number of UILabels per leaderboard panel
    private readonly Dictionary<string, Texture2D> profilePhotos = new Dictionary<string, Texture2D>();
    private readonly List<Vector3> tabGOSpriteScales = new List<Vector3>();
    public GameObject buttonGameCenter;
    public UILabel comingSoonText;
    private TabSettings deSelectedTabSettings;
    public List<GameObject> graphicsGOs = new List<GameObject>();
    private NotificationIcons notificationIcons;
    public List<UILeaderboardList> pageLists = new List<UILeaderboardList>();
    private SocialScreenName pageToLoad = SocialScreenName.TopDistances; //.SocialConnections;		
    private TabSettings selectedTabSettings;
    public List<GameObject> tabGOs = new List<GameObject>();

    public int GetkLeaderboardLabelCount()
    {
        return kLeaderboardLabelCount;
    }

    protected override void Awake()
    {
        base.Awake();

        notify.Debug("UILeaderboardViewControllerOz Awake ");
        selectedTabSettings = new TabSettings(tabGOs[0]);
        deSelectedTabSettings = new TabSettings(tabGOs[1]);

        // store initial icon sprite scale values, to pass into scaling function
        tabGOSpriteScales.Add(tabGOs[0].transform.Find("Sprite").localScale);
        tabGOSpriteScales.Add(tabGOs[1].transform.Find("Sprite").localScale);

        tabGOs[0].transform.Find("Sprite").localScale *= TabSettings.tabScaleMultiplier;
            // hack to make icon of selected tab scaled up on launch

        notificationIcons = gameObject.GetComponent<NotificationIcons>();

        Initializer.RegisterForLeaderBoardLoaded(OnLeaderboardLoad);

#if !UNITY_IPHONE
        Destroy(buttonGameCenter); // remove 'GameCenter' button if not iOS
#endif
    }

    public void OnLeaderboardLoad()
    {
        PopulateLeaderboards();
        //SwitchToPanel(SocialScreenName.TopDistances);

        //Services.Get<NotificationSystem>().SetNotificationIconsForThisPage(UiScreenName.LEADERBOARDS);		

        if (Initializer.SharedInstance.GetDisplayLeaderboard()
            && UIManagerOz.SharedInstance.leaderboardVC.gameObject.activeSelf)
        {
            ShowLeaderboards(true, pageToLoad);
        }
    }

    protected override void Start()
    {
        base.Start();
        //notificationSystem = Services.Get<NotificationSystem>();
        //LeaderboardManager leaderboardManager = Services.Get<LeaderboardManager>();
    }

    public override void appear()
    {
        //SharingManagerBinding.ShowBurstlyBannerAd( "leaderboards", true );

        base.appear();

        //Services.Get<NotificationSystem>().SetNotificationIconsForThisPage(UiScreenName.LEADERBOARDS);					
        SwitchToPanel(SocialScreenName.TopDistances); //pageToLoad);

        Initializer.SharedInstance.RefreshLeaderboards();
        //PopulateLeaderboards();		

        //Services.Get<NotificationSystem>().SetNotificationIconsForThisPage(UiScreenName.LEADERBOARDS);		

        UIManagerOz.SharedInstance.PaperVC.SetPageName("Ttl_Leaderboards", "");
        UIManagerOz.SharedInstance.PaperVC.SetCurrentPage(UIManagerOz.SharedInstance.leaderboardVC);
        //ResetScrollListPanelAlphasToZero();
        SetTabColliders();

        // Show "Log into Facebook?" dialog box if the user is not already logged in

        //Services.Get<NotificationSystem>().SetNotificationIconsForThisPage(UiScreenName.LEADERBOARDS);	
        //SwitchToPanel(SocialScreenName.TopDistances);	//pageToLoad);		


        ResetScrollListsToTop();
    }

    private void OnDisable()
    {
        //SharingManagerBinding.ShowBurstlyBannerAd( "leaderboards", false );
    }

    public void SetNotificationIcon(int buttonID, int iconValue) // update actual icon onscreen
    {
        notificationIcons.SetNotification(buttonID, iconValue);
    }

    public void CancelProfilePhotoDownloads()
    {
        foreach (var list in pageLists)
        {
            list.CancelProfilePhotoDownloads();
        }
    }

    public void CacheProfilePhoto(string fbid, Texture2D tex)
    {
        if (profilePhotos.ContainsKey(fbid))
        {
            profilePhotos[fbid] = tex; // replace old one if it exists
        }
        else
        {
            profilePhotos.Add(fbid, tex); // add it if it doesn't exist
        }
    }

    public Texture2D GetCachedProfilePhoto(string fbid)
    {
        if (profilePhotos.ContainsKey(fbid))
        {
            return profilePhotos[fbid];
        }
        return null;
    }

    public void ClearProfilePhotoCache()
    {
        profilePhotos.Clear();
    }

    private void ResetScrollListsToTop()
    {
        foreach (var list in pageLists)
            list.gameObject.GetComponent<UIScrollView>().ResetPosition();
    }

    private void OnGameCenterClicked() // iOS only
    {
        notify.Debug("OnGameCenterClicked");

#if UNITY_IOS
    //if ( GameCenterBinding.isGameCenterAvailable() == true )
    //{
    //	GameCenterBinding.showLeaderboardWithTimeScope( GameCenterLeaderboardTimeScope.AllTime );
    //}
    /* jonoble: Removed because this doesn't appear to work if the user has never used GameCenter
		//if ( GameCenterBinding.isGameCenterAvailable() == true )
		//{
		//	if ( GameCenterBinding.isPlayerAuthenticated() == true )
		//	{
		//		GameCenterBinding.showLeaderboardWithTimeScope( GameCenterLeaderboardTimeScope.AllTime );
		//	}
		//	else
		//	{
		//		// Ask the player to log in (instead of showing a "Player is not signed in" error)
		//		GameCenterBinding.authenticateLocalPlayer();
		//	}
		//}
		*/
#endif
    }

    private void ShowLeaderboards(bool valid, SocialScreenName page)
    {
        NGUITools.SetActive(graphicsGOs[(int) SocialScreenName.TopScores], (valid && page == SocialScreenName.TopScores));
        NGUITools.SetActive(graphicsGOs[(int) SocialScreenName.TopDistances],
            (valid && page == SocialScreenName.TopDistances));
        comingSoonText.enabled = !valid;
        SetArrows(valid, page);
    }

    private void SetArrows(bool valid, SocialScreenName page)
    {
        var rank = 1;

        //arrowUp.enabled = false;
        //arrowDown.enabled = false;

        pageLists[(int) page].TurnOffAllArrows();
        //PositionArrow(true, 2, page);	//arrowDown, rank, page);

        var notSys = Services.Get<NotificationSystem>();
        var scoreWentUpCount = notSys.GetOneShotCount(OneShotNotificationType.TopScorePositionWentUp);
        var scoreWentDownCount = notSys.GetOneShotCount(OneShotNotificationType.TopScorePositionWentDown);
        var distanceWentUpCount = notSys.GetOneShotCount(OneShotNotificationType.TopDistancePositionWentUp);
        var distanceWentDownCount = notSys.GetOneShotCount(OneShotNotificationType.TopDistancePositionWentDown);

        if (valid)
        {
            rank = Services.Get<LeaderboardManager>().GetUserRank(page); // == SocialScreenName.TopScores);

            if (page == SocialScreenName.TopScores)
            {
                if (scoreWentUpCount > 0)
                {
                    SetArrow(page, rank, true, OneShotNotificationType.TopScorePositionWentUp,
                        OneShotNotificationType.TopScorePositionWentDown);
                }
                else if (scoreWentDownCount > 0)
                {
                    SetArrow(page, rank, false, OneShotNotificationType.TopScorePositionWentUp,
                        OneShotNotificationType.TopScorePositionWentDown);
                }
            }
            else if (page == SocialScreenName.TopDistances)
            {
                if (distanceWentUpCount > 0)
                {
                    SetArrow(page, rank, true, OneShotNotificationType.TopDistancePositionWentUp,
                        OneShotNotificationType.TopDistancePositionWentDown);
                }
                else if (distanceWentDownCount > 0)
                {
                    SetArrow(page, rank, false, OneShotNotificationType.TopDistancePositionWentUp,
                        OneShotNotificationType.TopDistancePositionWentDown);
                }
            }
        }
    }

    private void SetArrow(SocialScreenName page, int rank, bool status, OneShotNotificationType clear1,
        OneShotNotificationType clear2)
    {
        notify.Debug("showing: " + page + " with status: " + status);
        PositionArrow(status, rank - 1, page);
        Services.Get<NotificationSystem>().SetNotificationIconsForThisPage(UiScreenName.LEADERBOARDS);
        notify.Debug("clearing: " + clear1);
        notify.Debug("clearing: " + clear2);
        Services.Get<NotificationSystem>().ClearOneShotNotification(clear1);
        Services.Get<NotificationSystem>().ClearOneShotNotification(clear2);
    }

    private void PositionArrow(bool up, int cellID, SocialScreenName page) //UISprite arrow, int rank))
    {
        pageLists[(int) page].TurnOnArrow(up, cellID); //arrowUp, cellID);
    }

    private void SetTabColliders()
        // only use this on initial page entry, afterwards the scaling takes care of collider state
    {
        foreach (var tabGO in tabGOs)
            tabGO.GetComponent<BoxCollider>().enabled = true;

        tabGOs[(int) pageToLoad].GetComponent<BoxCollider>().enabled = false;
    }

    private void SwitchToPanel(SocialScreenName panelScreenName)
        //, bool isInitialClick = false )	// activate panel upon button selection, passing in SocialScreenName
    {
        //ID (in range of int 0-3)
        if (panelScreenName != pageToLoad)
        {
            ScaleTab((int) pageToLoad, false);
            ScaleTab((int) panelScreenName, true);
        }

        pageToLoad = panelScreenName;

        SetActivePage();
        //Refresh();

        notify.Debug("setting notification icons for page UiScreenName.LEADERBOARDS");
        Services.Get<NotificationSystem>().SetNotificationIconsForThisPage(UiScreenName.LEADERBOARDS);

        if (Initializer.SharedInstance.GetDisplayLeaderboard())
            ShowLeaderboards(true, pageToLoad);
        else
            ShowLeaderboards(false, pageToLoad); // show 'coming soon' text
    }

    private void SetActivePage()
    {
        // set only appropriate panel active, make others inactive
        foreach (var go in graphicsGOs)
            NGUITools.SetActive(go, false);

        NGUITools.SetActive(graphicsGOs[(int) pageToLoad], true);

        pageLists[(int) pageToLoad].GetComponent<UIScrollView>().ResetPosition();
    }

    private void ScaleTab(int index, bool bigger)
    {
        if (index >= tabGOs.Count) // prevent nulls
            return;

        var tweenTime = 0.1f;
        var targetTabSettings = (bigger) ? selectedTabSettings : deSelectedTabSettings;

        targetTabSettings.ScaleTab(tabGOs[index], targetTabSettings, tweenTime, bigger, tabGOSpriteScales[index]);
    }

    private void OnButtonClick(GameObject button)
    {
        switch (button.name)
        {
            case "Tab0": // top distances button
                //	AnalyticsInterface.LogNavigationActionEvent( "Distances", "Leaderboard", "Leaderboard-Distances" );

                SwitchToPanel(SocialScreenName.TopDistances);
                UIManagerOz.SharedInstance.PaperVC.SetPageName("Ttl_Leaderboards", "");
                break;

            case "Tab1": // top scores button
                //AnalyticsInterface.LogNavigationActionEvent( "Scores", "Leaderboard", "Leaderboard-Scores" );

                SwitchToPanel(SocialScreenName.TopScores);
                UIManagerOz.SharedInstance.PaperVC.SetPageName("Ttl_Leaderboards", "");
                break;
        }
    }

    private void PopulateLeaderboards()
    {
        notify.Debug("PopulateLeaderboards() called in UILeaderboardControllerOz");

        // --- Get the latest leaderboard data --------------------------------
        var leaderboardManager = Services.Get<LeaderboardManager>();
        leaderboardManager.RefreshLeaderboards();
        // --------------------------------------------------------------------

        RefreshLeaderboardData();
    }

    private void RefreshLeaderboardData()
    {
        var leaderboardManager = Services.Get<LeaderboardManager>();

        // --- Top Scores -----------------------------------------------------
        var topScores = leaderboardManager.GetTopScores(kLeaderboardLabelCount);

        notify.Debug("Top Scores being populated in PopulateLeaderboards() called in UILeaderboardControllerOz");

        if (topScores != null)
        {
            pageLists[(int) SocialScreenName.TopScores].Refresh(topScores); //    scoreList.Refresh(topScores);
        }
        // --------------------------------------------------------------------

        // --- Top Distances --------------------------------------------------
        var topDistances = leaderboardManager.GetTopDistances(kLeaderboardLabelCount);

        notify.Debug("Top Distances being populated in PopulateLeaderboards() called in UILeaderboardControllerOz");

        if (topDistances != null)
        {
            pageLists[(int) SocialScreenName.TopDistances].Refresh(topDistances); //distanceList.Refresh(topDistances);
        }
        // --------------------------------------------------------------------		
    }

    // "Log in to Facebook?" dialog box
    private void OnFacebookLoginNoButtonPressed()
    {
        UIFacebookDialogOz.onNegativeResponse -= OnFacebookLoginNoButtonPressed;
        UIFacebookDialogOz.onPositiveResponse -= OnFacebookLoginYesButtonPressed;
    }

    // "Log in to Facebook?" dialog box
    private void OnFacebookLoginYesButtonPressed()
    {
        UIFacebookDialogOz.onNegativeResponse -= OnFacebookLoginNoButtonPressed;
        UIFacebookDialogOz.onPositiveResponse -= OnFacebookLoginYesButtonPressed;

        // Allow the Initializer to download leaderboards and refresh the UILeaderboardLists
        Initializer.LoadInitialLeaderboard = true;
    }
}

//	private void SetProfilePhoto()
//	{
//		notify.Debug( "[LeaderboardCellData] SetProfilePhoto" );		
//		
//		string url = "";	// string url = FBPictureManager.GetImageByFacebookId(_data.fbId);
//		
//		if ( FBPictureManager.HasFriendImageUrls && FBPictureManager.HasPlayerImageUrl && _data.fbId != "" )
//		{
//			url = FBPictureManager.GetImageByFacebookId( _data.fbId );
//		}
//		
//		if (
//			url != ""
//			&& (
//				Regex.IsMatch( url, ".jpg", RegexOptions.IgnoreCase )
//				|| Regex.IsMatch( url, ".jpeg", RegexOptions.IgnoreCase )
//				|| Regex.IsMatch( url, ".png", RegexOptions.IgnoreCase )
//			)
//		) {
//			StartCoroutine(LoadFacebookPhoto(url));	//urls[_data.rankScore]));
//		}		
//	}
//	
//	private IEnumerator LoadFacebookPhoto(string url)
//	{		
//		notify.Debug( "[LeaderboardCellData] LoadFacebookPhoto" );		
//		
//    	Texture2D newTex = new Texture2D(4, 4, TextureFormat.DXT1, false);
//    
//        www = new WWW(url);
//        yield return www;
//		
//		if (www != null)
//		{
//			if (www.error == null)
//			{		
//       			www.LoadImageIntoTexture(newTex);
//				facebookPhoto.material = new Material(facebookPhoto.material);
//				facebookPhoto.material.renderQueue = 9000;
//				facebookPhoto.mainTexture = newTex;
//			}
//			else
//			{
//				notify.Warning("Error retrieving Facebook photo: " + www.error);
//			}
//			
//			notify.Warning("Download profile photo success, now nulling www, for " + gameObject.name);
//			www = null;
//			//ResetWWW("LoadFacebookPhoto");
//		}
//    }	
//}


//public UISprite arrowUp, arrowDown;

//arrowUp.enabled = false;
//arrowDown.enabled = false;

//			if (page == SocialScreenName.TopScores)
//			{
//				if (Services.Get<NotificationSystem>().GetOneShotCount(OneShotNotificationType.TopScorePositionWentUp) > 0)
//				{
//					notify.Debug("showing TopScorePositionWentUp arrow");
//					PositionArrow(true, rank-1, page);	//arrowUp, rank, page);
//					Services.Get<NotificationSystem>().SetNotificationIconsForThisPage(UiScreenName.LEADERBOARDS);					
//					notify.Debug("clearing TopScorePositionWentUp");
//					notify.Debug("clearing TopScorePositionWentDown");						
//					Services.Get<NotificationSystem>().ClearOneShotNotification(OneShotNotificationType.TopScorePositionWentUp);
//					Services.Get<NotificationSystem>().ClearOneShotNotification(OneShotNotificationType.TopScorePositionWentDown);						
//				}
//				else if (Services.Get<NotificationSystem>().GetOneShotCount(OneShotNotificationType.TopScorePositionWentDown) > 0)
//				{
//					notify.Debug("showing TopScorePositionWentDown arrow");
//					PositionArrow(false, rank-1, page);	//arrowDown, rank, page);
//					Services.Get<NotificationSystem>().SetNotificationIconsForThisPage(UiScreenName.LEADERBOARDS);					
//					notify.Debug("clearing TopScorePositionWentUp");
//					notify.Debug("clearing TopScorePositionWentDown");	
//					Services.Get<NotificationSystem>().ClearOneShotNotification(OneShotNotificationType.TopScorePositionWentUp);
//					Services.Get<NotificationSystem>().ClearOneShotNotification(OneShotNotificationType.TopScorePositionWentDown);						
//				}				
//			}
//			else if (page == SocialScreenName.TopDistances)
//			{
//				if (Services.Get<NotificationSystem>().GetOneShotCount(OneShotNotificationType.TopDistancePositionWentUp) > 0)
//				{
//					notify.Debug("showing TopDistancePositionWentUp arrow");
//					PositionArrow(true, rank-1, page);	//arrowUp, rank, page);
//					Services.Get<NotificationSystem>().SetNotificationIconsForThisPage(UiScreenName.LEADERBOARDS);					
//					notify.Debug("clearing TopDistancePositionWentUp");
//					notify.Debug("clearing TopDistancePositionWentDown");
//					Services.Get<NotificationSystem>().ClearOneShotNotification(OneShotNotificationType.TopDistancePositionWentUp);
//					Services.Get<NotificationSystem>().ClearOneShotNotification(OneShotNotificationType.TopDistancePositionWentDown);	
//				}
//				else if (Services.Get<NotificationSystem>().GetOneShotCount(OneShotNotificationType.TopDistancePositionWentDown) > 0)
//				{
//					notify.Debug("showing TopDistancePositionWentDown arrow");
//					PositionArrow(false, rank-1, page);	//arrowDown, rank, page);
//					Services.Get<NotificationSystem>().SetNotificationIconsForThisPage(UiScreenName.LEADERBOARDS);					
//					notify.Debug("clearing TopDistancePositionWentUp");
//					notify.Debug("clearing TopDistancePositionWentDown");	
//					Services.Get<NotificationSystem>().ClearOneShotNotification(OneShotNotificationType.TopDistancePositionWentUp);
//					Services.Get<NotificationSystem>().ClearOneShotNotification(OneShotNotificationType.TopDistancePositionWentDown);					
//				}	
//			}


//UIManagerOz.SharedInstance.PaperVC.OnButtonClick(button);	

//if (currentScreen == UiScreenName.SOCIAL)	// social screen buttons	
//{		

//	nextViewController = leaderboardVC;
//}		

//SetActivePage();
//Refresh();


//		for ( int i = 0; i < kLeaderboardLabelCount; i++ )
//		{
//			TopScore_PlayerNameLabels.Add(TopScore_GameObjects[i].transform.Find("CellContents/PlayerNameLabel").GetComponent<UISysFontLabel>());	//UILabel>());
//			TopScore_ScoreLabels.Add(TopScore_GameObjects[i].transform.Find("CellContents/ScoreLabel").GetComponent<UILabel>());
//			TopDistance_PlayerNameLabels.Add(TopDistance_GameObjects[i].transform.Find("CellContents/PlayerNameLabel").GetComponent<UISysFontLabel>());	//UILabel>());
//			TopDistance_DistanceLabels.Add(TopDistance_GameObjects[i].transform.Find("CellContents/DistanceLabel").GetComponent<UILabel>());
//		}


//public List<GameObject> socialPanelGOs = new List<GameObject>();
//public List<GameObject> labelGOs = new List<GameObject>();	

//private NotificationSystem notificationSystem;
//private LeaderboardManager leaderboardManager;


//	private float GetArrowPositionY(int rank)
//	{
//		return ((float)rank * -50f) + 182f;
//	}


//		if (pageToLoad == SocialScreenName.TopDistances)
//		{
//			Services.Get<NotificationSystem>().ClearOneShotNotification(OneShotNotificationType.TopDistancePositionWentUp);
//			Services.Get<NotificationSystem>().ClearOneShotNotification(OneShotNotificationType.TopDistancePositionWentDown);
//		}
//		else if (pageToLoad == SocialScreenName.TopScores)
//		{
//			Services.Get<NotificationSystem>().ClearOneShotNotification(OneShotNotificationType.TopScorePositionWentUp);		
//			Services.Get<NotificationSystem>().ClearOneShotNotification(OneShotNotificationType.TopScorePositionWentDown);		
//		}

//	public void TurnOffBothBottomPlayerCells()
//	{
//		foreach (UILeaderboardList list in pageLists)
//		{
//			list.playerCellRoot.gameObject.SetActive(false);
//		}
//	}


//	private void SetScrollList(int listID)
//	{
//		pageLists[listID].playerCellRoot.playerLeaderboardCell.scrollList = pageLists[listID];
//	}


//	public List<LeaderboardManager.LeaderboardEntry> GetDataList(string listType)
//	{
//		return new List<LeaderboardManager.LeaderboardEntry>();
//	}


//public GameObject TopScore_GameObjects;		//public List<GameObject> TopScore_GameObjects;
//public GameObject TopDistance_GameObjects;	//public List<GameObject> TopDistance_GameObjects;
//public UILeaderboardList scoreList;
//public UILeaderboardList distanceList;

//private List<UISysFontLabel> TopScore_PlayerNameLabels = new List<UISysFontLabel>();
//private List<UILabel> TopScore_ScoreLabels = new List<UILabel>();
//private List<UISysFontLabel> TopDistance_PlayerNameLabels = new List<UISysFontLabel>();
//private List<UILabel> TopDistance_DistanceLabels = new List<UILabel>();


//arrow.enabled = true;
//arrow.transform.localPosition = new Vector3(arrow.transform.localPosition.x, GetArrowPositionY(rank), arrow.transform.localPosition.z);	

//		if (page == SocialScreenName.TopScores)
//		{
//			scoreList.TurnOnArrow(arrowUp, rank);
//		}
//		else if (page == SocialScreenName.TopDistances)
//		{
//			distanceList.TurnOnArrow(arrowUp, rank);
//		}


//foreach (GameObject topScoreCell in TopScore_GameObjects)
//NGUITools.SetActive(topScoreCell, (valid && page == SocialScreenName.TopScores));
//NGUITools.SetActive(TopScore_GameObjects, (valid && page == SocialScreenName.TopScores));

//foreach (GameObject topDistanceCell in TopDistance_GameObjects)
//NGUITools.SetActive(topDistanceCell, (valid && page == SocialScreenName.TopDistances));	
//NGUITools.SetActive(TopDistance_GameObjects, (valid && page == SocialScreenName.TopDistances));	


// --- Hide all the labels --------------------------------------------
//		for ( i = 0; i < kLeaderboardLabelCount; i ++ )
//		{
//			TopScore_PlayerNameLabels[ i ].enabled = false;
//			TopScore_ScoreLabels[ i ].enabled = false;
//			TopDistance_PlayerNameLabels[ i ].enabled = false;
//			TopDistance_DistanceLabels[ i ].enabled = false;
//		}
// --------------------------------------------------------------------


//	private string TruncateStringToFitLabel( string theString, UILabel theLabel )
//	{
//		UIFont font = theLabel.font;
//		bool encoding = theLabel.supportEncoding;
//		UIFont.SymbolStyle symbolStyle = theLabel.symbolStyle;
//	
//		float scale = font.bmFont.charSize;
//		float labelWidth = ( kPlayerNameLabelWidth * scale );
//		
//		Vector2 stringPrintedSize = font.CalculatePrintedSize( theString, encoding, symbolStyle );
//		float stringPixelWidth = ( stringPrintedSize.x * scale );
//
//		if ( labelWidth > 0 && ( stringPixelWidth > labelWidth ) )
//		{
//			// The string is too long for the label
//			
//			// Get the pixel width of the ellipses
//			stringPrintedSize = font.CalculatePrintedSize( kEllipsesString, encoding, symbolStyle );
//			float ellipsesPixelWidth = ( stringPrintedSize.x * scale );
//			
//			// Calculate the ideal string width
//			float targetPixelWidth = ( labelWidth - ellipsesPixelWidth );
//
//			// Remove characters from the end of the string until it fits
//			while ( stringPixelWidth > targetPixelWidth )
//			{
//				// Remove one character from the end of the string
//				theString = theString.Remove( ( theString.Length - 1 ), 1 );
//		
//				stringPrintedSize = font.CalculatePrintedSize( theString, encoding, symbolStyle );
//				stringPixelWidth = ( stringPrintedSize.x * scale );
//			}
//			
//			// Add the ellipses
//			theString += kEllipsesString;
//		}
//		
//		return theString;
//	}


//	private void Refresh()	
//	{
// set only appropriate panel active, make others inactive
//		foreach (GameObject go in socialPanelGOs)
//			NGUITools.SetActive(go, false);
//		NGUITools.SetActive(socialPanelGOs[(int)pageToLoad], true);	

//paperViewController.ResetTabs((int)pageToLoad);		// reset highlighted tab to the one actually chosen	
//	}		


//foreach (GameObject go in graphicsGOs)
//	go.AddComponent<UIPanelAlpha>();

//foreach (GameObject go in labelGOs)
//	go.AddComponent<UIPanelAlpha>();		


//		if ( isInitialClick == true )
//		{
//			// Entering the Social Connections screen from another screen
//			// (as opposed to a panel change within the Social Connections screen)
//			PopulateLeaderboards();
//		}


//		foreach (GameObject go in graphicsGOs)
//		{
//			TweenAlpha ta = TweenAlpha.Begin(go, 0.1f, 0f);
//			
//			if (go == graphicsGOs[0])
//				ta.onFinished += FadeInNextPanelGraphics;	
//		}
//		
//		foreach (GameObject go in labelGOs)
//		{
//			TweenAlpha ta = TweenAlpha.Begin(go, 0.1f, 0f);
//			
//			if (go == labelGOs[0])
//				ta.onFinished += FadeInNextPanelLabels;	
//		}			

//	private void ResetScrollListPanelAlphasToZero()
//	{
//		foreach (GameObject go in graphicsGOs)
//			go.GetComponent<UIPanelAlpha>().alpha = 0f;
//		
//		foreach (GameObject go in labelGOs)
//			go.GetComponent<UIPanelAlpha>().alpha = 0f;		
//	}		

//	private void FadeInNextPanelGraphics(UITweener ta)
//	{
//		TweenAlpha.Begin(graphicsGOs[(int)pageToLoad], 0.2f, 1f);
//		ta.onFinished -= FadeInNextPanelGraphics;
//	}	
//	
//	private void FadeInNextPanelLabels(UITweener ta)
//	{
//		TweenAlpha.Begin(labelGOs[(int)pageToLoad], 0.2f, 1f);
//		ta.onFinished -= FadeInNextPanelLabels;
//	}


//	public void OnEnable()
//	{
//	//	notificationSystem.SetNotificationIconsToThisPage(UiScreenName.SOCIAL);		
//		UIManagerOz.SharedInstance.PaperVC.SetPageName("Ttl_Leaderboards", "");
//		UIManagerOz.SharedInstance.PaperVC.SetCurrentPage(UIManagerOz.SharedInstance.leaderboardVC);
//		ResetScrollListPanelAlphasToZero();
//		SwitchToPanel(pageToLoad);
//		SetTabColliders();
//		//SetActivePage();
//		//Refresh();
//	}	


//ActivatePanel(activePanel);
//	}


//
//	private void ActivatePanel(GameObject _panel)
//	{
//		if (_panel == socialConnectionsPanel)
//		{
//			NGUITools.SetActive(socialConnectionsPanel, true);
//			NGUITools.SetActive(topScoresPanel, false);
//			NGUITools.SetActive(topDistancesPanel, false);
//			NGUITools.SetActive(challengesPanel, false);			
//		}
//		else if (_panel == topScoresPanel)
//		{
//			NGUITools.SetActive(socialConnectionsPanel, false);
//			NGUITools.SetActive(topScoresPanel, true);
//			NGUITools.SetActive(topDistancesPanel, false);
//			NGUITools.SetActive(challengesPanel, false);	
//		}
//		else if (_panel == topDistancesPanel)
//		{
//			NGUITools.SetActive(socialConnectionsPanel, false);
//			NGUITools.SetActive(topScoresPanel, false);
//			NGUITools.SetActive(topDistancesPanel, true);
//			NGUITools.SetActive(challengesPanel, false);	
//		}
//		else if (_panel == challengesPanel)
//		{
//			NGUITools.SetActive(socialConnectionsPanel, false);
//			NGUITools.SetActive(topScoresPanel, false);
//			NGUITools.SetActive(topDistancesPanel, false);
//			NGUITools.SetActive(challengesPanel, true);	
//		}
//		
//		activePanel = _panel;
//	}		
//}


//	public GameObject socialConnectionsPanel;	
//	public GameObject topScoresPanel;	
//	public GameObject topDistancesPanel;
//	public GameObject challengesPanel;	
//	
//	public GameObject activePanel;


//	public void OnActivateSocialConnections(bool _value)
//	{
//		NGUITools.SetActive(socialConnectionsPanel, _value);
//		if (_value) 
//			activePanel = socialConnectionsPanel;
//	}	
//	
//	public void OnActivateTopScores(bool _value)
//	{
//		NGUITools.SetActive(topScoresPanel, _value);
//		if (_value) 
//			activePanel = topScoresPanel;
//	}	
//	
//	public void OnActivateTopDistances(bool _value)
//	{
//		NGUITools.SetActive(topDistancesPanel, _value);
//		if (_value) 
//			activePanel = topDistancesPanel;
//	}		
//	
//	public void OnActivateChallenges(bool _value)
//	{
//		NGUITools.SetActive(challengesPanel, _value);
//		if (_value)
//			activePanel = challengesPanel;
//	}			
//}


//	public UIInGameViewControllerOz inGameVC = null;	
//	public static event voidClickedHandler onPlayClickedHandler = null;
//	
//	public void OnPlayClicked() 
//	{
//		if (MainGameCamera != null) { MainGameCamera.enabled = true; }
//		disappear();
//		if(inGameVC != null) { inGameVC.appear(); } 
//		if (onPlayClickedHandler != null) { onPlayClickedHandler(); }	//-- Notify an object that is listening for this event.
//	}	