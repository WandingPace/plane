using UnityEngine;
using System;
using System.Collections;

public class AppCounters : MonoBehaviour
{
	public int countAppLaunch { get; private set; }
	public int countGameStart { get; private set; }
	public int countMapViews { get; private set; }
	
	private Int64 totalSecondsSpentInApp;
	//private DateTime appLaunchTime;
	private DateTime lastTotalSecondsUpdateTime;
	
	private NotificationSystem notificationSystem;
	
	protected static Notify notify;
	
	
	void Awake()
	{
		notify = new Notify(this.GetType().Name);
	}
	
	void Start()
	{
		notificationSystem = Services.Get<NotificationSystem>();
		
		countAppLaunch = PlayerPrefs.GetInt("countAppLaunch");
		countGameStart = PlayerPrefs.GetInt("countGameStart");
		countMapViews = PlayerPrefs.GetInt("countMapViews");
		totalSecondsSpentInApp = Int64.Parse(PlayerPrefs.GetString("totalSecondsSpentInApp", "0"));
		
		//appLaunchTime = lastTotalSecondsUpdateTime = DateTime.UtcNow;
		lastTotalSecondsUpdateTime = DateTime.UtcNow;
		AddAppLaunch();
		
		notify.Debug("App launched.  Total seconds spent in app: " + totalSecondsSpentInApp.ToString());
	}
	
	public static void ResetCountersInPlayerPrefs()
	{
		PlayerPrefs.SetInt("countAppLaunch",-1);
		PlayerPrefs.SetInt("countGameStart",-1);
		PlayerPrefs.SetInt("countMapViews",-1);
		PlayerPrefs.SetString("totalSecondsSpentInApp","0");
		PlayerPrefs.Save();
	}

	private void AddAppLaunch()
	{
		countAppLaunch++;
		PlayerPrefs.SetInt("countAppLaunch",countAppLaunch);
		PlayerPrefs.Save();
	}	
	
	public void AddGameStart()
	{
		countGameStart++;
		PlayerPrefs.SetInt("countGameStart",countGameStart);
		PlayerPrefs.Save();
	}		
	
	public void AddMapView()
	{
		countMapViews++;
		PlayerPrefs.SetInt("countMapViews",countMapViews);
		PlayerPrefs.Save();
	}
	
	public void UpdateSecondsSpentInApp()
	{
		Int64 seconds = (Int64)((DateTime.UtcNow - lastTotalSecondsUpdateTime).TotalSeconds);
		//notify.Warning("SecondsSpentInApp since lastcall to UpdateSecondsSpentInApp = " + seconds.ToString());
		
		lastTotalSecondsUpdateTime = DateTime.UtcNow;
		totalSecondsSpentInApp += seconds;
		PlayerPrefs.SetString("totalSecondsSpentInApp",totalSecondsSpentInApp.ToString());
		PlayerPrefs.Save();
	}	
	
	public Int64 GetSecondsSpentInApp()
	{
		UpdateSecondsSpentInApp();		// make sure it's up to date before sending
		return totalSecondsSpentInApp;
	}		
		
	void OnApplicationPause(bool pause)
	{
		bool t = (notificationSystem == null);
		notify.Debug("notificationSystem = null is " + t.ToString());
		
		if (notificationSystem == null)
			return;
		
		if (pause)
		{
			UpdateSecondsSpentInApp();	
			notificationSystem.SaveClearedNotificationsToPlayerPrefs();
			//notify.Debug("Paused.  Total seconds spent in app: " + totalSecondsSpentInApp.ToString());
		}
		else
		{
			lastTotalSecondsUpdateTime = DateTime.UtcNow;
			//notify.Debug("Unpaused.  Total seconds spent in app: " + totalSecondsSpentInApp.ToString());
			//notify.Debug("Dying " + GamePlayer.SharedInstance.Dying + " isDead " + GamePlayer.SharedInstance.IsDead + " timescale " + Time.timeScale);
			/* remove this fix temporarily, untill farther testing
			if(GamePlayer.SharedInstance.Dying || GamePlayer.SharedInstance.IsDead || GamePlayer.SharedInstance.IsFalling){
				GameController.SharedInstance.IsGameOver = true;
				Time.timeScale = 1f;
				if(GameController.SharedInstance.IsTutorialMode){
					GameController.SharedInstance.AutoRestart = true;
					GameController.SharedInstance.doHandleEndGame();
				}
				else{
					UIManagerOz.SharedInstance.EndGame();
				}
			}
			*/
			
		}
	}

	void OnApplicationQuit()
	{
		UpdateSecondsSpentInApp();
		notificationSystem.SaveClearedNotificationsToPlayerPrefs();
		notify.Debug("Quitting...");
	}	
}
