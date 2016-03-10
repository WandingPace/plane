//using UnityEngine;
//using System.Collections;
//using System.Collections.Generic;
//
//public class PlayButton : MonoBehaviour 
//{
//	public UISprite bgSprite;
//	//public UISprite progressBG, progressFG; -N.N.
//	public BoxCollider boxCollider;
//	Color green = new Color(0.0f, 0.75f, 0.0f, 1.0f);
//	Color grey = new Color(0.5f, 0.5f, 0.5f, 1.0f);
//
//	DLProgressBar bar;
//	
//	public void ResetPlayButton()
//	{
//		//progressBG.enabled = false; -N.N.
//		//progressFG.enabled = false; -N.N.
//		boxCollider.enabled = true;
//		bgSprite.color = Color.white;
//	}
//	
//	void OnEnable()
//	{
//		if (UIManagerOz.SharedInstance != null)
//		{
//			if (UIManagerOz.SharedInstance.mapVC != null)
//				UpdateStatus();	// update itself before appearing
//		}
//	}	
//
//	void Start() 
//	{
//		//GamePlayer.OnFastTravel += UpdateStatus;
//		UpdateStatus();
//	}
//	
//	public void UpdateStatus()
//	{
//		SetButtonColor();
//		SetButtonEnabled();
//		//SetProgressBarEnabled(); -N.N.
//	}
//
////	private void SetProgressBarEnabled() - N.N.
////	{
////		if (!DownloadManager.IsDownloadInProgress())	// downloads are all done, hide the progress bar
////		{
////			Destroy(bar);
////			progressBG.enabled = false;
////			progressFG.enabled = false;
////		}	
////		else 											// show progress bar when something is being downloaded
////		{
////			progressBG.enabled = true;
////			progressFG.enabled = true;
////			progressFG.transform.localScale = new Vector3(0.0f, progressFG.transform.localScale.y, 1.0f);	// start at completely empty
////			
////			if (bar == null)							// hook up progress bar
////			{
////				bar = gameObject.AddComponent<DLProgressBar>();
////				bar.progressFG = progressFG;
////			}
////		}
////	}
//	
//	private void SetButtonEnabled()
//	{
//		if (DownloadManager.IsDownloadInProgress())
//			boxCollider.enabled = false;
//		else
//			boxCollider.enabled = true;
//	}	
//	
//	private void SetButtonColor()
//	{
//		if (GamePlayer.SharedInstance.HasFastTravel)
//			bgSprite.color = green;
//		else if (DownloadManager.IsDownloadInProgress())
//			bgSprite.color = grey;
//		else
//			bgSprite.color = Color.white;
//	}
//}
//
//
//
//
////	private void SetPlayButtonIcon()
////	{
////		if (GamePlayer.SharedInstance != null)	// so it works in 'UI Edit' scenes also, when game is not running
////		{
////			if (DownloadManager.IsDownloadInProgress())	// something is being downloaded
////				iconSprite.spriteName = "icon_map_download";
////			else if (GamePlayer.SharedInstance.HasFastTravel)
////				iconSprite.spriteName = "icon_map_fasttravel";
////			else
////				iconSprite.spriteName = "icon_navigation_play";
////		}
////	}
//
////
////using UnityEngine;
////using System.Collections;
////using System.Collections.Generic;
////
////public class PlayButton : MonoBehaviour 
////{
////	public UISprite bgSpriteL, bgSpriteR;
////	public UISprite iconSprite;
////	public UISprite progressBG, progressFG;
////	Color green = new Color(0.0f, 0.75f, 0.0f, 1.0f);
////	Color grey = new Color(0.5f, 0.5f, 0.5f, 1.0f);
////
////	DLProgressBar bar;
////	
////	public void ResetPlayButton()
////	{
////		progressBG.enabled = false;
////		progressFG.enabled = false;
////		collider.enabled = true;
////		bgSpriteL.color = Color.white;
////		bgSpriteR.color = Color.white;
////		iconSprite.spriteName = "icon_navigation_play";
////	}
////	
////	void OnEnable()
////	{
////		if (UIManagerOz.SharedInstance != null)
////		{
////			if (UIManagerOz.SharedInstance.mapVC != null)
////				UpdateStatus();	// update itself before appearing
////		}
////	}	
////
////	void Start() 
////	{
////		//GamePlayer.OnFastTravel += UpdateStatus;
////		UpdateStatus();
////	}
////	
////	public void UpdateStatus()
////	{
////		SetButtonColor();
////		SetPlayButtonIcon();
////		SetButtonEnabled();
////		SetProgressBarEnabled();
////	}
////
////	private void SetProgressBarEnabled()
////	{
////		if (!DownloadManager.IsDownloadInProgress())	// downloads are all done, hide the progress bar
////		{
////			Destroy(bar);
////			progressBG.enabled = false;
////			progressFG.enabled = false;
////		}	
////		else 											// show progress bar when something is being downloaded
////		{
////			progressBG.enabled = true;
////			progressFG.enabled = true;
////			progressFG.transform.localScale = new Vector3(0.0f, progressFG.transform.localScale.y, 1.0f);	// start at completely empty
////			
////			if (bar == null)							// hook up progress bar
////			{
////				bar = gameObject.AddComponent<DLProgressBar>();
////				bar.progressFG = progressFG;
////			}
////		}
////	}
////	
////	private void SetButtonEnabled()
////	{
////		if (DownloadManager.IsDownloadInProgress())
////			collider.enabled = false;
////		else
////			collider.enabled = true;
////	}	
////	
////	private void SetButtonColor()
////	{
////		if (GamePlayer.SharedInstance.HasFastTravel)
////		{
////			bgSpriteL.color = green;
////			bgSpriteR.color = green;
////		}			
////		else if (DownloadManager.IsDownloadInProgress())
////		{
////			bgSpriteL.color = grey;
////			bgSpriteR.color = grey;
////		}			
////		else
////		{
////			bgSpriteL.color = Color.white;
////			bgSpriteR.color = Color.white;
////		}
////	}
////	
////	private void SetPlayButtonIcon()
////	{
////		if (GamePlayer.SharedInstance != null)	// so it works in 'UI Edit' scenes also, when game is not running
////		{
////			if (DownloadManager.IsDownloadInProgress())	// something is being downloaded
////				iconSprite.spriteName = "icon_map_download";
////			else if (GamePlayer.SharedInstance.HasFastTravel)
////				iconSprite.spriteName = "icon_map_fasttravel";
////			else
////				iconSprite.spriteName = "icon_navigation_play";
////		}
////	}
////}
////
//
//
//
//
//		//ResourceManager.SharedInstance.RegisterForOnAssetBundleLoadFailure(OnAssetBundleLoadedFailure);		
//
//
////	private void OnAssetBundleLoadedFailure(string assetBundleName, int version, string errMsg)
////	{
////		UpdateStatus();
////	}	
//
//
//		//SetPlayButtonColorIconEnabled()
//
//
////	
////	void Update()
////	{
////		if (isDownloadInProgress)
////			UpdateProgressBar();
////	}
//	
//			//loader = null;
//
////	private void SetDownloadProgress()
//
//		//SetDownloadProgress();	// do this first, as a lot of the states depend on this being correct
//
//	//bool isDownloadInProgress = false;
//	
//
//		//isDownloadInProgress = (DownloadManager.HowManyDownloadsInProgress() > 0) ? true : false;	
//
//	
//		//TurnButtonColorGreen;
//		//SetPlayButtonColorIconEnabled();
//		//ResourceManager.SharedInstance.RegisterForOnAssetBundleLoadSuccess(OnAssetBundleLoadedSuccess);			
////		progressBG.enabled = false;
////		progressFG.enabled = false;
//
//
////		if (progress >= 1.0f)	// downloads are all done, hide the progress bar
////		{
////			//loader = null;
////			progressBG.enabled = false;
////			progressFG.enabled = false;
////		}		
//
//	//UIManagerOz.SharedInstance.mapVC.downloadingLocationID != -1)
//
//		//if (GamePlayer.SharedInstance != null)	// so it works in 'UI Edit' scenes also, when game is not running
//		//{
//		
//		//if (GamePlayer.SharedInstance != null)	// so it works in 'UI Edit' scenes also, when game is not running
//		//{		
//		//}
//
//
////	public void OnAssetBundleLoadedSuccess(string assetBundleName, int version, bool downloadOnly)
////	{	
////		//UIManagerOz.SharedInstance.mapVC.downloadingLocationID = -1;	// reset location being downloaded		
////		SetPlayButtonColorIconEnabled();
////	}	
//
//
//	
////		
////		foreach (KeyValuePair<int, AssetBundleLoader> kvp in DownloadManager.loaders)	//foreach (AssetBundleLoader loader in loaders)
////			progressTotal += kvp.Value.GetProgress();									// loader.GetProgress();
////		
////		if (DownloadManager.downloadsInProgress != 0)	// prevent divide by 0
////			progress = progressTotal / (float)(DownloadManager.downloadsInProgress);	//loader.GetProgress();
////		else 
////			progress = 1.0f;	// if no downloads in progress, they must all be done
////			
//
//	
////	public void SetAssetBundleLoaderReference(int id, AssetBundleLoader bundleLoader)
////	{
////		loaders.Add(id, bundleLoader);	//loader = bundleLoader;
////	}
//	
//
//	//DownloadManager.HowManyDownloadsInProgress() > 0)	//if (loader != null)
//
//	//Color greyGreen = new Color(0.5f, 0.75f, 0.5f, 1.0f);		
//		
////		bgSpriteL = transform.Find("BackgroundL").GetComponent<UISprite>();
////		bgSpriteR = transform.Find("BackgroundR").GetComponent<UISprite>();
////		iconSprite = transform.Find("Sprite").GetComponent<UISprite>();
////		progressBG = transform.Find("ProgressBG").GetComponent<UISprite>();
////		progressFG = transform.Find("ProgressFG").GetComponent<UISprite>();
////		collider = GetComponent<BoxCollider>();
//		
//
//
////			if (UIManagerOz.SharedInstance.mapVC.downloadingLocationID != -1 &&
////				 GamePlayer.SharedInstance.HasFastTravel)
////			{
////				bgSpriteL.color = green;
////				bgSpriteR.color = green;
////			}			
////			else if (UIManagerOz.SharedInstance.mapVC.downloadingLocationID != -1)
////			{
////				bgSpriteL.color = grey;
////				bgSpriteR.color = grey;
////			}			
////			else if (GamePlayer.SharedInstance.HasFastTravel)
////			{
////				bgSpriteL.color = green;
////				bgSpriteR.color = green;
////			}
////			else
////			{
////				bgSpriteL.color = Color.white;
////				bgSpriteR.color = Color.white;
//
//
////	private void TurnButtonColorGreen(bool enabled)
////	{
////		if (enabled)
////		{
////			bgSpriteL.color = green;
////			bgSpriteR.color = green;
////			//iconSprite.spriteName = "icon_map_fasttravel";
////		}
////		else
////		{
////			bgSpriteL.color = Color.white;
////			bgSpriteR.color = Color.white;
////			//iconSprite.spriteName = "icon_navigation_play";
////		}
////	}
////	
////	public void ShowDownloading(bool enabled)
////	{
////		if (enabled)
////		{
////			if (GamePlayer.SharedInstance.HasFastTravel)
////				TurnButtonColorGreen(true);
////			else
////				TurnButtonColorGreen(false);
////			
////			
////			iconSprite.spriteName = "icon_map_download";
////		}
////		else
////		{
////			SetFastTravel();
////		}		
////	}
//
//
//
//
//
//		//ResourceManager.SharedInstance.RegisterForOnAssetBundleLoadFailure(OnAssetBundleLoadedFailure);		
//
//
////	private void OnAssetBundleLoadedFailure(string assetBundleName, int version, string errMsg)
////	{
////		UpdateStatus();
////	}	
//
//
//		//SetPlayButtonColorIconEnabled()
//
//
////	
////	void Update()
////	{
////		if (isDownloadInProgress)
////			UpdateProgressBar();
////	}
//	
//			//loader = null;
//
////	private void SetDownloadProgress()
//
//		//SetDownloadProgress();	// do this first, as a lot of the states depend on this being correct
//
//	//bool isDownloadInProgress = false;
//	
//
//		//isDownloadInProgress = (DownloadManager.HowManyDownloadsInProgress() > 0) ? true : false;	
//
//	
//		//TurnButtonColorGreen;
//		//SetPlayButtonColorIconEnabled();
//		//ResourceManager.SharedInstance.RegisterForOnAssetBundleLoadSuccess(OnAssetBundleLoadedSuccess);			
////		progressBG.enabled = false;
////		progressFG.enabled = false;
//
//
////		if (progress >= 1.0f)	// downloads are all done, hide the progress bar
////		{
////			//loader = null;
////			progressBG.enabled = false;
////			progressFG.enabled = false;
////		}		
//
//	//UIManagerOz.SharedInstance.mapVC.downloadingLocationID != -1)
//
//		//if (GamePlayer.SharedInstance != null)	// so it works in 'UI Edit' scenes also, when game is not running
//		//{
//		
//		//if (GamePlayer.SharedInstance != null)	// so it works in 'UI Edit' scenes also, when game is not running
//		//{		
//		//}
//
//
////	public void OnAssetBundleLoadedSuccess(string assetBundleName, int version, bool downloadOnly)
////	{	
////		//UIManagerOz.SharedInstance.mapVC.downloadingLocationID = -1;	// reset location being downloaded		
////		SetPlayButtonColorIconEnabled();
////	}	
//
//
//	
////		
////		foreach (KeyValuePair<int, AssetBundleLoader> kvp in DownloadManager.loaders)	//foreach (AssetBundleLoader loader in loaders)
////			progressTotal += kvp.Value.GetProgress();									// loader.GetProgress();
////		
////		if (DownloadManager.downloadsInProgress != 0)	// prevent divide by 0
////			progress = progressTotal / (float)(DownloadManager.downloadsInProgress);	//loader.GetProgress();
////		else 
////			progress = 1.0f;	// if no downloads in progress, they must all be done
////			
//
//	
////	public void SetAssetBundleLoaderReference(int id, AssetBundleLoader bundleLoader)
////	{
////		loaders.Add(id, bundleLoader);	//loader = bundleLoader;
////	}
//	
//
//	//DownloadManager.HowManyDownloadsInProgress() > 0)	//if (loader != null)
//
//	//Color greyGreen = new Color(0.5f, 0.75f, 0.5f, 1.0f);		
//		
////		bgSpriteL = transform.Find("BackgroundL").GetComponent<UISprite>();
////		bgSpriteR = transform.Find("BackgroundR").GetComponent<UISprite>();
////		iconSprite = transform.Find("Sprite").GetComponent<UISprite>();
////		progressBG = transform.Find("ProgressBG").GetComponent<UISprite>();
////		progressFG = transform.Find("ProgressFG").GetComponent<UISprite>();
////		collider = GetComponent<BoxCollider>();
//		
//
//
////			if (UIManagerOz.SharedInstance.mapVC.downloadingLocationID != -1 &&
////				 GamePlayer.SharedInstance.HasFastTravel)
////			{
////				bgSpriteL.color = green;
////				bgSpriteR.color = green;
////			}			
////			else if (UIManagerOz.SharedInstance.mapVC.downloadingLocationID != -1)
////			{
////				bgSpriteL.color = grey;
////				bgSpriteR.color = grey;
////			}			
////			else if (GamePlayer.SharedInstance.HasFastTravel)
////			{
////				bgSpriteL.color = green;
////				bgSpriteR.color = green;
////			}
////			else
////			{
////				bgSpriteL.color = Color.white;
////				bgSpriteR.color = Color.white;
//
//
////	private void TurnButtonColorGreen(bool enabled)
////	{
////		if (enabled)
////		{
////			bgSpriteL.color = green;
////			bgSpriteR.color = green;
////			//iconSprite.spriteName = "icon_map_fasttravel";
////		}
////		else
////		{
////			bgSpriteL.color = Color.white;
////			bgSpriteR.color = Color.white;
////			//iconSprite.spriteName = "icon_navigation_play";
////		}
////	}
////	
////	public void ShowDownloading(bool enabled)
////	{
////		if (enabled)
////		{
////			if (GamePlayer.SharedInstance.HasFastTravel)
////				TurnButtonColorGreen(true);
////			else
////				TurnButtonColorGreen(false);
////			
////			
////			iconSprite.spriteName = "icon_map_download";
////		}
////		else
////		{
////			SetFastTravel();
////		}		
////	}
