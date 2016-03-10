using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;

public class LeaderboardCellData : MonoBehaviour 
{
	protected static Notify notify;
	
	//public GameObject viewController;
	public UILeaderboardList scrollList;				// reference to the scroll list that this cell is parented under the grid/table of
	private LeaderboardManager.LeaderboardEntry _data;	// reference to leaderboard cell data	
	
	public UILabel rankLabel, nameLabel, scoreOrDistanceLabel;
	//public UILabel scoreOrDistanceLabel;
	public UITexture facebookPhoto;
	public UISprite arrowUp, arrowDown;
	public List<UISprite> background = new List<UISprite>();
	public Color[] colors = new Color[3];	// { Color.black, Color.white, Color.blue, };
	
	private WWW www;	// for downloading Facebook photos
	
	void Awake()
	{
		notify = new Notify("LeaderboardCellData");
		notify.Debug( "[LeaderboardCellData] Awake" );		
		
		//notificationIcons = gameObject.GetComponent<NotificationIcons>();
	}
	
	void Start()
	{
		notify.Debug( "[LeaderboardCellData] Start" );		
		
		Destroy(gameObject.GetComponent<UIPanel>());				// kill auto-attached UIPanel component
	}
	
	void OnEnable()
	{
		if (_data != null && scrollList != null)
		{
			SetProfilePhoto();
		}
	}
	
	public void SetData(LeaderboardManager.LeaderboardEntry data, UILeaderboardList list, bool refreshCell, bool forceBackgroundOff)
	{
		notify.Debug( "[LeaderboardCellData] SetData" );		
		
		_data = data;
		scrollList = list;

		if (_data != null && scrollList != null && refreshCell)	// && viewController != null)
		{
			// populate fields from data
			bool isScorePage = (scrollList.listType == SocialScreenName.TopScores);	// true = score, false = distance
			int scoreOrDistanceValue = isScorePage ? _data.score : _data.meters;
			int scoreOrDistanceRank = isScorePage ? _data.rankScore : _data.rankDistance;
			int textColorIndex = scoreOrDistanceRank % 2;

			nameLabel.text = _data.name;
			rankLabel.text = scoreOrDistanceRank.ToString() + ".";	//rankStr;
			scoreOrDistanceLabel.text = scoreOrDistanceValue.ToString();		//scoreOrDistanceLabel.text = scoreOrDistanceStr;
			
			SetCellBackground(textColorIndex == 0 && !forceBackgroundOff);
			SetTextColorByIndex(_data.isCurrentUser ? 2 : textColorIndex);			
			SetProfilePhoto();
		}
		//else
		//	notify.Warning("LeaderboardEntryProtoData (data) or UILeaderboardViewControllerOz (viewController) is null in LeaderboardCellData!");
	}
	
	public void TurnOffBothArrows()
	{
		notify.Debug( "[LeaderboardCellData] TurnOffBothArrows" );		
		
		arrowUp.enabled	= false;
		arrowDown.enabled = false;
	}
	
	public void TurnOnArrow(bool up)
	{
		notify.Debug( "[LeaderboardCellData] TurnOnArrow" );		
		
		arrowUp.enabled = up;
		arrowDown.enabled = !up;
	}

	public void CancelDownload()
	{
		SetupNotify();
		notify.Debug( "[LeaderboardCellData] CancelDownload" );		
		
		if (www != null)		// check that a download request is still outstanding
		{		
			if (!www.isDone)	// only do this if the download request has not already returned
			{
				ResetWWW("CancelDownload");
			}
			else
			{
				notify.Debug("www.isDone = true in CancelDownload in LeaderboardCellData");
			}
		}
		else
		{
			notify.Debug("www = null in CancelDownload in LeaderboardCellData");
		}
	}	
	
	public void SetCellVisible(bool status)
	{	
		notify.Debug( "[LeaderboardCellData] SetCellVisible" );		
		
		rankLabel.enabled = status;
		nameLabel.enabled = status;
		scoreOrDistanceLabel.enabled = status;
		facebookPhoto.enabled = status;
		SetCellBackground(status);
		
		if (!status)
		{
			TurnOffBothArrows();			// turn off both arrows also if cell is not visible
		}		
	}
	
	private void SetProfilePhoto()
	{
		notify.Debug( "[LeaderboardCellData] SetProfilePhoto" );		
		
		string url = "";	// string url = FBPictureManager.GetImageByFacebookId(_data.fbId);
		
		if ( FBPictureManager.HasFriendImageUrls && FBPictureManager.HasPlayerImageUrl && _data.fbId != "" )
		{
			url = FBPictureManager.GetImageByFacebookId( _data.fbId );
		}
		
		if (
			url != ""
			&& (
				Regex.IsMatch( url, ".jpg", RegexOptions.IgnoreCase )
				|| Regex.IsMatch( url, ".jpeg", RegexOptions.IgnoreCase )
				|| Regex.IsMatch( url, ".png", RegexOptions.IgnoreCase )
			)
		) {
			// first check cache for profile photo for this fbid
			Texture2D tex = UIManagerOz.SharedInstance.leaderboardVC.GetCachedProfilePhoto(_data.fbId);
			
			if (tex != null) 	// if profile photo was found, use it
			{
				SetActualPhotoTexture(tex);	
			}
			else 				// if we don't have one yet, fetch it
			{
				StartCoroutine(LoadFacebookPhoto(url));	//urls[_data.rankScore]));
			}
		}		
	}
	
	private void SetActualPhotoTexture(Texture2D newTex)
	{
		facebookPhoto.material = new Material(facebookPhoto.material);
		facebookPhoto.material.renderQueue = 9000;
		facebookPhoto.mainTexture = newTex;		
	}
	
	private IEnumerator LoadFacebookPhoto(string url)
	{		
		notify.Debug( "[LeaderboardCellData] LoadFacebookPhoto" );		
		
    	Texture2D tex = new Texture2D(4, 4, TextureFormat.DXT1, false);
    
        www = new WWW(url);
        yield return www;
		
		if (www != null)
		{
			if (www.error == null)
			{		
       			www.LoadImageIntoTexture(tex);
				UIManagerOz.SharedInstance.leaderboardVC.CacheProfilePhoto(_data.fbId, tex);
				SetActualPhotoTexture(tex);			
				//facebookPhoto.material = new Material(facebookPhoto.material);
				//facebookPhoto.material.renderQueue = 9000;
				//facebookPhoto.mainTexture = newTex;
			}
			else
			{
				notify.Warning("Error retrieving Facebook photo: " + www.error);
			}
			
			notify.Warning("Download profile photo success, now nulling www, for " + gameObject.name);
			www = null;
			//ResetWWW("LoadFacebookPhoto");
		}
    }

	private void ResetWWW(string source)
	{
		notify.Debug( "[LeaderboardCellData] ResetWWW, for " + gameObject.name);		
		
		if (www != null)		// check that a download request is still outstanding
		{		
			notify.Debug(source + " disposing www in LeaderboardCellData, for " + gameObject.name);
			www.Dispose();
			notify.Debug("nulling www");
			www = null;		
		}
	}

	private void SetCellBackground(bool status)
	{
		notify.Debug( "[LeaderboardCellData] SetCellBackground" );		
		
		foreach (UISprite sprite in background)
		{
			sprite.enabled = status;
		}
	}
	
	private void SetTextColorByIndex(int index)
	{
		notify.Debug( "[LeaderboardCellData] SetTextColorByIndex" );		
		
		rankLabel.color = colors[index];
		nameLabel.color = colors[index];
		scoreOrDistanceLabel.color = colors[index];	
	}
	
	private void SetupNotify()
	{
		if (notify == null)
		{
			notify = new Notify(this.GetType().Name);
		}
	}		
}

		


			
//			int scoreOrDistanceValue = 0;
//			int scoreOrDistanceRank = 0;
//			
//			if (scrollList.listType == SocialScreenName.TopScores)	//list.name == "scores")
//			{
//				scoreOrDistanceValue = _data.score;
//				scoreOrDistanceRank = _data.rankScore;
//			}
//			else if (scrollList.listType == SocialScreenName.TopDistances)	//list.name == "distances")
//			{
//				scoreOrDistanceValue = _data.meters;
//				scoreOrDistanceRank = _data.rankDistance;			
//			}


//			if (_data.isCurrentUser)
//			{
//				SetTextColorByIndex(2);				
//			}
//			else
//			{
//				SetTextColorByIndex(textColorIndex);
//			}	


//				rankLabel.color = colors[2];
//				nameLabel.color = colors[2];
//				scoreOrDistanceLabel.color = colors[2];	
//
//				rankLabel.color = colors[textColorIndex];
//				nameLabel.color = colors[textColorIndex];
//				scoreOrDistanceLabel.color = colors[textColorIndex];

		
//			backgroundtop.enabled = bgShowing;
//			backgroundbottom.enabled = bgShowing;
//			backgroundleft.enabled = bgShowing;
//			backgroundright.enabled = bgShowing;


	// backgroundtop;
	//public UISprite backgroundbottom;
	//public UISprite backgroundright;
	//public UISprite backgroundleft;

			
		//notificationSystem = Services.Get<NotificationSystem>();		
		
		//if (_data != null)	// && viewController != null)
		//	Refresh();												// populate fields
		
	//	Refresh();													// populate fields	
	//}

	//private void Refresh()
	//{		
		
			
			//if (_data.rankScore <= 5)
			
			
			//gameObject.transform.Find("CellContents/Sprite (leaderboards_portrait)").GetComponent<UISprite>().spriteName = _data._iconName;			
			
			//SetNotificationIcon();	// show notification icon if can afford to purchase this item and it hasn't been cleared	

	
			//string nameStr = "";

			//gameObject.transform.Find("CellContents").GetComponent<UIButtonMessage>().target = viewController;			
			//gameObject.transform.Find("CellContents/PlayerNameLabel").GetComponent<UISysFontLabel>().Text = _data.name;
			
			//arrowUp.enabled = false;
			//arrowDown.enabled = false;
			
			//string listName = scrollList.listName;	//GetComponent<UILeaderboardList>().listName;
			
			
//			if (listName == "scores")
//			{		
//				rankStr = _data.rankScore.ToString() + ".";	//kRankSeparatorCharacter;
//				nameStr = _data.name;
//				scoreOrDistanceStr = _data.score.ToString();
//				textColorIndex = _data.rankScore % 2;
//				backgroundtop.enabled = (_data.rankScore % 2 == 1);
//				backgroundbottom.enabled = (_data.rankScore % 2 == 1);
//				backgroundleft.enabled = (_data.rankScore % 2 == 1);
//				backgroundright.enabled = (_data.rankScore % 2 == 1);
//			}
//			else if (listName == "distances")
//			{
//				rankStr = _data.rankDistance.ToString() + ".";	//kRankSeparatorCharacter;
//				nameStr = _data.name;
//				scoreOrDistanceStr = _data.meters.ToString();
//				textColorIndex = _data.rankDistance % 2;
//				backgroundtop.enabled = (_data.rankDistance % 2 == 1);
//				backgroundbottom.enabled = (_data.rankDistance % 2 == 1);
//				backgroundleft.enabled = (_data.rankDistance % 2 == 1);
//				backgroundright.enabled = (_data.rankDistance % 2 == 1);
//			}


	
	//private NotificationSystem notificationSystem;
	//private NotificationIcons notificationIcons;	

	//private const string kEllipsesString = "...";
	//private const string kRankSeparatorCharacter = ".";
	//private const float kPlayerNameLabelWidth = 6.5f; // Ideally we'd get this from the label itself, but label.lineWidth is returning "0"
	

	//private List<string> urls = new List<string>();
		
		
		//urls.Add("");
		//urls.Add("https://fbcdn-profile-a.akamaihd.net/hprofile-ak-prn1/c41.18.221.221/s148x148/249214_100652496695620_8016435_n.jpg");
		//urls.Add("http://www.google.com/images/srpr/logo4w.png");
		//urls.Add("http://www.tasharen.com/ngui/docs/ngui.png");
		//urls.Add("http://portablegamingregion.com/wp-content/uploads/2013/03/Unity-Technologies-Logo-Thumbnail-300x300.png");
		//urls.Add("http://jenkins-ci.org/sites/default/files/images/headshot.png");


//	public void TurnOnArrow(bool up)
//	{
//		if (up)
//		{
//			arrowUp.enabled = true;
//		}
//		else
//		{
//			arrowDown.enabled = true;
//		}
//	}


//	public void SetNotificationIcon()
//	{
//		if (notificationSystem == null)
//			notificationSystem = Services.Get<NotificationSystem>();		
//		
//		string listName = scrollList.GetComponent<UILeaderboardList>().listName;
//		int rank = 1;	
//	
//		if (listName == "score")
//		{
//			rank = _data.rankScore;
//		}
//		else if (listName == "distance")
//		{
//			rank = _data.rankDistance;
//		}		
//		
//		bool enable = notificationSystem.GetNotificationStatusForThisCell(NotificationType.Modifier, rank);
//		notificationIcons.SetNotification(0, (enable) ? 0 : -1);
//	}
							
	
//	private Color currentUserColor = Color.white;
//	private Color currentDataColor = Color.white;
//	private Color otherUsersColor = Color.gray;
//	private Color otherDataColor = Color.gray;
				
			
//			if (_data.isCurrentUser == true)
//			{
//				nameLabel.color = currentUserColor;
//				scoreOrDistanceLabel.color = currentDataColor;
//				//Debug.Log( "(that was the user's score) " +  TopScore_PlayerNameLabels[ i ].Text + " color " + TopScore_PlayerNameLabels[ i ].color);
//			}
//			else
//			{
//				nameLabel.color = otherUsersColor;
//				scoreOrDistanceLabel.color = otherDataColor;
//			}			
	
			
			
			
			/*
			int i = 0;
			LeaderboardManager.LeaderboardEntry leaderboardEntry = null; 
			
			for ( i = 0; i < topScores.Count; i++ )
			{
				leaderboardEntry = topScores[ i ];
				
				//string nameStr = leaderboardEntry.rank.ToString() + kRankSeparatorCharacter + " " + leaderboardEntry.name;
				string nameStr = leaderboardEntry.rankScore.ToString() + kRankSeparatorCharacter + " " + leaderboardEntry.name;
				//TopScore_PlayerNameLabels[ i ].Text = TruncateStringToFitLabel( nameStr, TopScore_PlayerNameLabels[ i ] );
				if ( nameStr.Length > 18)
				{
					nameStr = nameStr.Substring(0, 15);
					nameStr += "...";
				}
				TopScore_PlayerNameLabels[ i ].Text = nameStr;


				string scoreStr = string.Format( "{0:n0}", leaderboardEntry.score );
				TopScore_ScoreLabels[ i ].text = scoreStr;
				
				notify.Debug( leaderboardEntry.name + " " + scoreStr );
				if ( leaderboardEntry.isCurrentUser == true )
				{
					TopScore_PlayerNameLabels[ i ].color = currentUserColor;
					TopScore_ScoreLabels[ i ].color = currentDataColor;
					//Debug.Log( "(that was the user's score) " +  TopScore_PlayerNameLabels[ i ].Text + " color " + TopScore_PlayerNameLabels[ i ].color);
				}
				else{
					TopScore_PlayerNameLabels[ i ].color = otherUsersColor;
					TopScore_ScoreLabels[ i ].color = otherDataColor;
				}
				
				// Make this row visible
				TopScore_PlayerNameLabels[ i ].enabled = true;
				TopScore_ScoreLabels[ i ].enabled = true;
			}
		}			
			
			
		for ( i = 0; i < topDistances.Count; i++ )
			{
				leaderboardEntry = topDistances[ i ];
				
				//string nameStr = leaderboardEntry.rank.ToString() + kRankSeparatorCharacter + " " + leaderboardEntry.name;
				string nameStr = leaderboardEntry.rankDistance.ToString() + kRankSeparatorCharacter + " " + leaderboardEntry.name;
				
				if ( nameStr.Length > 18)
				{
					nameStr = nameStr.Substring(0, 15);
					nameStr += "...";
				}
				//TopDistance_PlayerNameLabels[ i ].Text = TruncateStringToFitLabel( nameStr, TopDistance_PlayerNameLabels[ i ] );
				TopDistance_PlayerNameLabels[ i ].Text = nameStr;

				string distanceStr = string.Format( "{0:n0}m", leaderboardEntry.meters );
				TopDistance_DistanceLabels[ i ].text = distanceStr;
				
				notify.Debug( leaderboardEntry.name + " " + distanceStr );
				if ( leaderboardEntry.isCurrentUser == true )
				{
					TopDistance_PlayerNameLabels[ i ].color = currentUserColor;
					TopDistance_DistanceLabels[ i ].color = currentDataColor;
					notify.Debug( "(that was the user's score)" );
				}
				else{
					TopDistance_PlayerNameLabels[ i ].color = otherUsersColor;
					TopDistance_DistanceLabels[ i ].color = otherDataColor;
				}
				
				// Make this row visible
				TopDistance_PlayerNameLabels[ i ].enabled = true;
				TopDistance_DistanceLabels[ i ].enabled = true;
			}			
			*/
