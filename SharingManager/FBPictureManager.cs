using UnityEngine;
using System;
using System.Collections;
using System.Collections. Generic;

public class FBPictureManager : MonoBehaviour
{
	protected static Notify notify = new Notify( "FBPictureManager" );
	
	public delegate void PhotoReceivedHandler(string fbID);
	protected static event PhotoReceivedHandler OnPhotoReceivedEvent = null;	
	public void RegisterForPhotoReceived( PhotoReceivedHandler delg) {
		OnPhotoReceivedEvent += delg; }	
	public void UnregisterForPhotoReceived( PhotoReceivedHandler delg) {
		OnPhotoReceivedEvent -= delg; }		
	
	private static Dictionary< string, string > _fbImages = new Dictionary<string, string>();
	
	public static string GetImageByFacebookId( string fbId )
	{
		notify.Debug( "[FBPictureManager] GetImageByFacebookId - with: " + fbId );
		
		if ( fbId != "" &&  _fbImages != null && _fbImages.ContainsKey( fbId ) )
		{
			notify.Debug( "[FBPictureManager] GetImageByFacebookId - returning: " + _fbImages[fbId].ToString() );
			
			return _fbImages[fbId];
		}
		
		notify.Debug( "[FBPictureManager] GetImageByFacebookId - returning nothing.  Dictionary not set, or no fbId was passed" );
		
		return "";
	}
	
//	private static string  _currentPlayerImage = "";
//	
//	public static string CurrentPlayerImage
//	{
//		get
//		{
//			return _currentPlayerImage;
//		}
//	}
	
	private static bool _hasPlayerImageUrl = false;
	public static bool HasPlayerImageUrl
	{
		get
		{
			return _hasPlayerImageUrl;
		}
	}
	
	private static bool _hasFriendImageUrls = false;
	public static bool HasFriendImageUrls
	{
		get
		{
			return _hasFriendImageUrls;
		}
	}
	
	
	public static void FetchImageUrls()
	{
		 
	}
		
	private static void _getMyImage()
	{
		/*
		string accessToken = SharingManagerBinding.GetFacebookAccessToken();
		
		string fqlQueryString = "https://graph.facebook.com/fql?q=SELECT%20uid,%20pic_square%20FROM%20user%20WHERE%20uid%20=%20me()"
			+ "&access_token=" + accessToken;
		
		NetAgent.Submit( new NetRequest( fqlQueryString, true, _gotMyImage ) );
		*/
	}
	
	private static bool _gotMyImage( WWW www, bool noErrors, object results )
	{
		if ( noErrors )
		{
			//Attempt to deserialize the response.
			Dictionary<string,object> dict = results as Dictionary<string, object>;
			
			if ( dict != null )
			{
				if ( dict.ContainsKey( "data" ) )
				{
					List<object> friendList = dict[ "data" ] as List<object>;
					
					if ( friendList != null )
					{
						foreach ( object friendObj in friendList ) 
						{
							Dictionary<string, object> friend = friendObj as Dictionary<string, object>;
							
							if ( friend != null )
							{
								if ( friend.ContainsKey( "uid") && friend.ContainsKey( "pic_square" ) )
								{
									string fbId = friend[ "uid" ].ToString();
									
									string _currentPlayerImage = friend[ "pic_square" ].ToString();
									
									notify.Debug( "[FBPictureManager] Player image: " + _currentPlayerImage );
									
									_hasPlayerImageUrl = true;
									
									if (_fbImages.ContainsKey(fbId))
									{
										_fbImages[fbId] = _currentPlayerImage;		// swap in new image
									}
									else
									{
										_fbImages.Add(fbId, _currentPlayerImage);	// add image
									}
								}
							}
						}
					}
				}
			}
		}
		return true;
	}
	
	private static void _getFriendPictures( )
	{
		/*
		string accessToken = SharingManagerBinding.GetFacebookAccessToken();
		
		string fqlQueryString = "https://graph.facebook.com/fql?q=SELECT%20uid,%20pic_square%20FROM%20user%20WHERE%20uid%20IN%20"
			+ "(%20SELECT%20uid2%20FROM%20friend%20WHERE%20uid1%20=%20me())&access_token=" + accessToken;
				
		NetAgent.Submit( new NetRequest( fqlQueryString, true, _gotFriendPictures ) );
		*/
	}
	
	private static bool _gotFriendPictures( WWW www, bool noErrors, object results )
	{
		if ( noErrors )
		{
			// _fbImages.Clear();
			
			//Attempt to deserialize the response.
			Dictionary<string,object> dict = results as Dictionary<string, object>;
			
			if ( dict != null )
			{
				if ( dict.ContainsKey( "data" ) )
				{
					List<object> friendList = dict[ "data" ] as List<object>;
					
					if ( friendList != null )
					{
						foreach ( object friendObj in friendList ) 
						{
							Dictionary<string, object> friend = friendObj as Dictionary<string, object>;
							
							if ( friend != null )
							{
								if ( friend.ContainsKey( "uid") && friend.ContainsKey( "pic_square" ) )
								{
								 	//int fbId = (Int32)JSONTools.ReadInt( friend[ "uid" ] );
									string fbId = friend[ "uid" ].ToString();
									string imageUrl = friend[ "pic_square" ].ToString();
									
									notify.Debug( 
										string.Format( 
											"[FBPictureManager] GotFriendImages fbId: {0} url: {1} ",
											fbId,
											imageUrl
										)
									);
									
									_fbImages.Add( fbId, imageUrl );
									
									if ( !_hasFriendImageUrls )
									{
										_hasFriendImageUrls = true;
									}
									
									if ( OnPhotoReceivedEvent != null )
									{
										OnPhotoReceivedEvent(fbId);
									}
								}
							}
						}
					}
				}
			}
		}
		
		return true;
	}
}
