using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class LeaderboardManager : MonoBehaviour
{
	public class LeaderboardEntry
	{
		public string name;
		public int score;
		public int meters;
		//public int rank;
		public int rankScore = 1;
		public int rankDistance = 1;
		public bool isCurrentUser;
		public string fbPhotoUrl = "";
		public string fbId = "";
	}

	private class LeaderboardEntryScoreComparer : IComparer<LeaderboardEntry>
	{
		public int Compare( LeaderboardEntry obj1, LeaderboardEntry obj2 )
   	 	{
			int result = 0;
			if ( obj1.score > obj2.score )
			{
				result = -1;
			}
			else if ( obj1.score < obj2.score )
			{
				result = 1;
			}
			
			return result;
		}
	}
	
	private class LeaderboardEntryDistanceComparer : IComparer<LeaderboardEntry>
	{
    	public int Compare( LeaderboardEntry obj1, LeaderboardEntry obj2 )
   	 	{
			int result = 0;
			if ( obj1.meters > obj2.meters )
			{
				result = -1;
			}
			else if ( obj1.meters < obj2.meters )
			{
				result = 1;
			}
			
			return result;
		}
	}

	private List<LeaderboardEntry> mLeaderboardEntries = null;
	protected static Notify notify;
	//private int mUserScoresLeaderboardPreviousRank = -1;	//0;
	//private int mUserDistancesLeaderboardPreviousRank = -1;	//0;
	//private int mUserScoresRank = 1;
	//private int mUserDistancesRank = 1;
	
	void Awake()
	{
		notify = new Notify( this.GetType().Name );
	}

	public void RefreshLeaderboards()
	{
		notify.Debug( "LeaderboardManager.RefreshLeaderboards() entered");		
		
		//UserProtoData userServerData = ProfileManager.SharedInstance.userServerData;
		UserProtoData lastServerData = ProfileManager.SharedInstance.lastServerData;
		
		if ( lastServerData != null ) 	//userServerData != null )
		{
			mLeaderboardEntries = new List<LeaderboardEntry>();
	
			// Add the user's top score/top distance
			PlayerStats playerStats = GameProfile.SharedInstance.Player;
			LeaderboardEntry leaderboardEntry = new LeaderboardEntry();
			leaderboardEntry.isCurrentUser = true;

			leaderboardEntry.fbId = lastServerData._fbId;
			
			notify.Debug( "[LeaderboardManager] - User's facebook id " + lastServerData._fbId );
			
			leaderboardEntry.name = lastServerData._name;
			
			//If the server's data is higher than the local profile, use the server.
 			
			if (lastServerData._bestScore > playerStats.bestScore)
				leaderboardEntry.score = lastServerData._bestScore;
			else 
				leaderboardEntry.score = playerStats.bestScore; // Use local score, since the server data may be a few minutes old

			if (lastServerData._bestMeters > playerStats.bestDistanceScore)
				leaderboardEntry.meters = lastServerData._bestMeters;
			else
				leaderboardEntry.meters = playerStats.bestDistanceScore; // Use local distance, since the server data may be a few minutes old
			 
			mLeaderboardEntries.Add( leaderboardEntry );

			/*
			if ( SharingManagerBinding.IsFacebookSessionValid() == true || SharingManagerBinding.IsGameCenterPlayerAuthenticated() == true ||
				Settings.GetString("fb-access-token","") != "")
			{
				// Add each neighbor's top score/top distance
				if ( lastServerData._neighborList != null )
				{
					foreach ( NeighborProtoData neighbor in lastServerData._neighborList )
					{
						leaderboardEntry = new LeaderboardEntry();
						leaderboardEntry.name = neighbor._name;
						leaderboardEntry.score = neighbor._bestScore;
						leaderboardEntry.meters = neighbor._bestMeters;
						leaderboardEntry.fbId = neighbor._fbId;
						
						mLeaderboardEntries.Add( leaderboardEntry );

					}
				}
			}*/
		}
	}
	
	public List<LeaderboardEntry> GetTopScores(int count, bool dontOverwritePrevious = false)
	{
		notify.Debug( "LeaderboardManager.GetTopScores() entered");		
		
		List<LeaderboardEntry> entries = null;
		
		// Sort mLeaderboardEntries by score (descending)
		if ( mLeaderboardEntries != null )
		{
			LeaderboardEntryScoreComparer leaderboardEntryScoreComparer = new LeaderboardEntryScoreComparer();
			mLeaderboardEntries.Sort( leaderboardEntryScoreComparer );
			
			entries = GetTopEntries( mLeaderboardEntries, count , true);
			
			PlayerStats playerStats = GameProfile.SharedInstance.Player;
			
			// Send out a notification if the user's rank has changed
			LeaderboardEntry userEntry = GetUserEntry( entries );
			if ( userEntry != null )
			{
				notify.Debug("userEntry.rankScore = " + userEntry.rankScore + " in GetTopScores");
				
				/*
				if (mUserScoresLeaderboardPreviousRank == -1)
				{
					mUserScoresLeaderboardPreviousRank = userEntry.rankScore;
				}
				*/

				notify.Debug( string.Format( "[LeaderboardManager] - GetTopScores: previousScoreRank: {0} -- currentRank: {1}", playerStats.previousScoreRank, userEntry.rankScore ) );
				
				if ( playerStats.previousScoreRank == -1 )
				{
					playerStats.previousScoreRank = userEntry.rankScore;
				}
							
				//else if ( userEntry.rankScore != mUserScoresLeaderboardPreviousRank )	//if ( userEntry.rank != mUserScoresLeaderboardRank )
				else if ( userEntry.rankScore != playerStats.previousScoreRank )
				{
					notify.Debug("clearing TopScorePositionWentUp");
					notify.Debug("clearing TopScorePositionWentDown");
					Services.Get<NotificationSystem>().ClearOneShotNotification(OneShotNotificationType.TopScorePositionWentUp);							
					Services.Get<NotificationSystem>().ClearOneShotNotification(OneShotNotificationType.TopScorePositionWentDown);							
					
					//if (userEntry.rank > mUserScoresLeaderboardRank)
					//if (userEntry.rankScore > mUserScoresLeaderboardPreviousRank)
					if ( userEntry.rankScore > playerStats.previousScoreRank )
					{
						notify.Debug("logging TopScorePositionWentDown");
						Services.Get<NotificationSystem>().SendOneShotNotificationEvent(-1, (int)OneShotNotificationType.TopScorePositionWentDown);	// userEntry.rank );
						//mUserScoresRank = userEntry.rank;
					}
					else
					{
						notify.Debug("logging TopScorePositionWentDown");
						Services.Get<NotificationSystem>().SendOneShotNotificationEvent(-1, (int)OneShotNotificationType.TopScorePositionWentUp);	// userEntry.rank );
						//mUserScoresRank = userEntry.rank;
					}
					
					if (!dontOverwritePrevious)		// this is so the main menu notification can use this function, then the page itself also can the second time
					{
						//mUserScoresLeaderboardPreviousRank = userEntry.rankScore;	//userEntry.rank;
						//mUserScoresRank = userEntry.rank;
						playerStats.previousScoreRank = userEntry.rankScore;
					}
				}
			}
			else
			{
				notify.Debug("userEntry == null in GetTopScores!");
			}
			
			GameProfile.SharedInstance.Serialize();
		}
		
		return entries;
	}
	
	public List<LeaderboardEntry> GetTopDistances(int count, bool dontOverwritePrevious = false)
	{
		notify.Debug( "LeaderboardManager.GetTopDistances() entered");		
		
		List<LeaderboardEntry> entries = null;
		
		// Sort mLeaderboardEntries by distance (descending)
		if ( mLeaderboardEntries != null )
		{
			LeaderboardEntryDistanceComparer leaderboardEntryDistanceComparer = new LeaderboardEntryDistanceComparer();
			mLeaderboardEntries.Sort( leaderboardEntryDistanceComparer );
		
			entries = GetTopEntries( mLeaderboardEntries, count , false);
			
			// Send out a notification if the user's rank has changed
			LeaderboardEntry userEntry = GetUserEntry( entries );
			
			PlayerStats playerStats = GameProfile.SharedInstance.Player;
			
			if ( userEntry != null )
			{
				notify.Debug( string.Format( "[LeaderboardManager] - GetTopScores: previousScoreRank: {0} -- currentRank: {1}", playerStats.previousScoreRank, userEntry.rankScore ) );
				
				/*
				if (mUserDistancesLeaderboardPreviousRank == -1)
				{
					mUserDistancesLeaderboardPreviousRank = userEntry.rankDistance;
				}
				*/
				if ( playerStats.previousDistanceRank == -1 )
				{
					playerStats.previousDistanceRank = userEntry.rankDistance; 
				}
				
				//else if ( userEntry.rankDistance != mUserDistancesLeaderboardPreviousRank )	//if ( userEntry.rank != mUserDistancesLeaderboardRank )
				else if ( userEntry.rankDistance != playerStats.previousDistanceRank )
				{
					notify.Debug("clearing TopDistancePositionWentUp");
					notify.Debug("clearing TopDistancePositionWentDown");					
					Services.Get<NotificationSystem>().ClearOneShotNotification(OneShotNotificationType.TopDistancePositionWentUp);
					Services.Get<NotificationSystem>().ClearOneShotNotification(OneShotNotificationType.TopDistancePositionWentDown);
					
					//if (userEntry.rank > mUserDistancesLeaderboardRank)
					//if (userEntry.rankDistance > mUserDistancesLeaderboardPreviousRank)
					if ( userEntry.rankDistance > playerStats.previousDistanceRank )
					{
						notify.Debug("logging TopDistancePositionWentDown");					
						Services.Get<NotificationSystem>().SendOneShotNotificationEvent(-2, (int)OneShotNotificationType.TopDistancePositionWentDown);	// userEntry.rank );
						//mUserDistancesRank = userEntry.rank;
					}
					else
					{
						notify.Debug("logging TopDistancePositionWentUp");					
						Services.Get<NotificationSystem>().SendOneShotNotificationEvent(-2, (int)OneShotNotificationType.TopDistancePositionWentUp);	// userEntry.rank );
						//mUserDistancesRank = userEntry.rank;
					}			
					
					if (!dontOverwritePrevious)		// this is so the main menu notification can use this function, then the page itself also can the second time
					{
						playerStats.previousDistanceRank = userEntry.rankDistance;
						//mUserDistancesLeaderboardPreviousRank = userEntry.rankDistance;	//userEntry.rank;
						//mUserDistancesRank = userEntry.rank;
					}
				}
			}
			else
			{
				notify.Debug("userEntry == null in GetTopDistances!");
			}
			
			GameProfile.SharedInstance.Serialize();
		}
		
		return entries;
	}
	
	// --- EMPLOYEES-ONLY BEYOND THIS POINT -----------------------------------

	private List<LeaderboardEntry> GetTopEntries( List<LeaderboardEntry> sourceList, int count, bool isScore)
	{
		notify.Debug( "LeaderboardManager.GetTopEntries() entered");		
		
		List<LeaderboardEntry> entries = null;
		
		if ( sourceList != null )
		{
			// --- Rank all the entries ---------------------------------------
			//LeaderboardEntry userEntry = null;
	
			for ( int i = 0; i < sourceList.Count; i ++ )
			{
				LeaderboardEntry entry = sourceList[ i ];
				//entry.rank = ( i + 1 );
				if (isScore)
				{
					entry.rankScore = ( i + 1 );
				}
				else
				{
					entry.rankDistance = ( i + 1 );	
				}
				
				//if ( entry.isCurrentUser == true )
				//{
				//	userEntry = entry;
				//}
			}
			// ----------------------------------------------------------------
			
			// --- Get the top "count" entries --------------------------------
			if ( count > sourceList.Count )
			{
				count = sourceList.Count;
			}
			entries = sourceList.GetRange( 0, count );
			// ----------------------------------------------------------------
		}
		
		return entries;
	}
	
	private LeaderboardEntry GetUserEntry( List<LeaderboardEntry> sourceList )
	{
		notify.Debug( "LeaderboardManager.GetUserEntry() entered");		
		
		LeaderboardEntry userEntry = null;
		
		for ( int i = 0; i < sourceList.Count; i ++ )
		{
			LeaderboardEntry entry = sourceList[ i ];
					
			if ( entry.isCurrentUser == true )
			{
				userEntry = entry;
				break;
			}
		}
		
		return userEntry;
	}
	
	public int GetUserRank(SocialScreenName page)	// true = scores, false = distances
	{		
		notify.Debug( "LeaderboardManager.GetUserRank() entered");		
		
		PlayerStats player = GameProfile.SharedInstance.Player;
		bool isScoresPage = (page == SocialScreenName.TopScores);
		int rank = isScoresPage ? player.previousScoreRank : player.previousDistanceRank;
		
		return rank;
	}
}




			
			// this section is now obsolete, as we show all friends in the scroll list and cut off at 99			
//			if ( userEntry != null )
//			{
//				// The user has an entry in the lederboard
//				// If the user's rank is not in the top "count" then replace the last entry with the user's entry
//				// (so the user's entry is always visible)
//				
//				int rank = (isScore) ? userEntry.rankScore : userEntry.rankDistance;
//				
//				//if ( userEntry.rank > count )
//				if ( rank > count )
//				{
//					// The user's entry is not in the top "count"
//					// Remove the last entry from the list
//					entries.RemoveAt( ( entries.Count - 1 ) );
//					
//					// Append the user's entry to the list
//					entries.Add( userEntry );
//				}
//			}


		//notify.Debug( "LeaderboardManager.GetUserRank() called with parameter scores = " + scores.ToString() + ", returning rank = " + rank.ToString());		

		
//		if (page == SocialScreenName.TopScores)
//		{
//			//rank = mUserScoresLeaderboardPreviousRank;	//mUserScoresRank;
//			rank = GameProfile.SharedInstance.Player.previousScoreRank;
//		}
//		else
//		{
//			//rank = mUserDistancesLeaderboardPreviousRank;	//mUserDistancesRank;
//			rank = GameProfile.SharedInstance.Player.previousDistanceRank;
//		}


	// if scores == true return player's scores table rank, otherwise return player's distances table rank

		/*
		// FOR TESTING
		mLeaderboardEntries = new List<LeaderboardEntry>();
	
		LeaderboardEntry leaderboardEntry = new LeaderboardEntry();
		leaderboardEntry.name = "Long player name";
		leaderboardEntry.isCurrentUser = true;
		leaderboardEntry.score = 123;
		leaderboardEntry.meters = 89101;
		mLeaderboardEntries.Add( leaderboardEntry );
	
		leaderboardEntry = new LeaderboardEntry();
		leaderboardEntry.name = "Friend A";
		leaderboardEntry.score = 56789;
		leaderboardEntry.meters = 4567;
		mLeaderboardEntries.Add( leaderboardEntry );
	
		leaderboardEntry = new LeaderboardEntry();
		leaderboardEntry.name = "Friend B";
		leaderboardEntry.score = 123456789;
		leaderboardEntry.meters = 123;
		mLeaderboardEntries.Add( leaderboardEntry );
		
		leaderboardEntry = new LeaderboardEntry();
		leaderboardEntry.name = "Friend C";
		leaderboardEntry.score = 234;
		leaderboardEntry.meters = 234567;
		mLeaderboardEntries.Add( leaderboardEntry );
		
		leaderboardEntry = new LeaderboardEntry();
		leaderboardEntry.name = "Friend D";
		leaderboardEntry.score = 233;
		leaderboardEntry.meters = 234568;
		mLeaderboardEntries.Add( leaderboardEntry );
		
		leaderboardEntry = new LeaderboardEntry();
		leaderboardEntry.name = "Friend E";
		leaderboardEntry.score = 232;
		leaderboardEntry.meters = 234569;
		mLeaderboardEntries.Add( leaderboardEntry );
		
		leaderboardEntry = new LeaderboardEntry();
		leaderboardEntry.name = "Friend F";
		leaderboardEntry.score = 231;
		leaderboardEntry.meters = 234570;
		mLeaderboardEntries.Add( leaderboardEntry );
		
		leaderboardEntry = new LeaderboardEntry();
		leaderboardEntry.name = "Friend G";
		leaderboardEntry.score = 230;
		leaderboardEntry.meters = 234571;
		mLeaderboardEntries.Add( leaderboardEntry );
		
		leaderboardEntry = new LeaderboardEntry();
		leaderboardEntry.name = "Friend H";
		leaderboardEntry.score = 229;
		leaderboardEntry.meters = 234572;
		mLeaderboardEntries.Add( leaderboardEntry );
		
		leaderboardEntry = new LeaderboardEntry();
		leaderboardEntry.name = "Friend I";
		leaderboardEntry.score = 228;
		leaderboardEntry.meters = 234573;
		mLeaderboardEntries.Add( leaderboardEntry );
		
		leaderboardEntry = new LeaderboardEntry();
		leaderboardEntry.name = "Friend J";
		leaderboardEntry.score = 227;
		leaderboardEntry.meters = 234574;
		mLeaderboardEntries.Add( leaderboardEntry );
		*/