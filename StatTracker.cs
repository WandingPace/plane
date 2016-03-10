using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public enum ObstacleType { kJump, kDuck, kStumble, kLedge}

public class StatTracker
{
	public struct StatSet
	{
		public float fTimeRunning;
		
		public int turnsSpawned;
		public List<float> timesBetweenTurns;
		public float averageTimeBetweenTurns {
			get {
				float average = 0f;
				foreach(float time in timesBetweenTurns)	average += time;
				average /= (float)timesBetweenTurns.Count;
				return average;
			}
		}
		
		public int coinsLv1Spawned;
		public int coinsLv2Spawned;
		public int coinsLv3Spawned;
		public int coinsSpawned { get { return coinsLv1Spawned + coinsLv2Spawned + coinsLv3Spawned; } }
		
		public int gemsSpawned;
		
		public int bonusItemsSpawned;
		public Dictionary<BonusItem.BonusItemType,int> bonusItemRecord;
		
		public int obstaclesSpawned;
		public Dictionary<ObstacleType,int> obstacleRecord;
		public List<float> timesBetweenObstacles;
	}
	
	
	private static StatSet Stats;
	
	
	
	private static bool isRunning = false;
	
	private static float startTime = 0f;
	
	private static float lastTurnTime = 0f;
//	private static float lastObstacleTime = 0f;
	
	
	
	//Start/Stop functions
	
	public static void Begin()
	{
		if(!isRunning)
		{
			//Initialize the Stats struct if we havent yet
			if(Stats.timesBetweenTurns==null)	Reset();
			
			isRunning = true;
			lastTurnTime = startTime = Time.time;
		}
	}
	
	public static void Stop()
	{
		if(isRunning)
		{
			isRunning = false;
			Stats.fTimeRunning += Time.time-startTime;
		}
		/*
		Debug.Log("STATS STOPPED:");
		Debug.Log("Running Time: "+Stats.fTimeRunning);
		Debug.Log("Turns Spawned: "+Stats.turnsSpawned);
		Debug.Log("Average Time Between Turns:"+Stats.averageTimeBetweenTurns);
		Debug.Log("Coins Spawned: "+Stats.coinsSpawned);
		Debug.Log("--Lv1 Coins: "+Stats.coinsLv1Spawned);
		Debug.Log("--Lv2 Coins: "+Stats.coinsLv2Spawned);
		Debug.Log("--Lv3 Coins: "+Stats.coinsLv3Spawned);
		Debug.Log("Gems Spawned: "+Stats.gemsSpawned);
		Debug.Log("Bonus Items Spawned: "+Stats.bonusItemsSpawned);
		Debug.Log("Obstacles Spawned: "+Stats.obstaclesSpawned);
		*/
	}
	public static void Reset()
	{
		Stats.fTimeRunning = 0f;
		
		Stats.turnsSpawned = 0;
		Stats.timesBetweenTurns = new List<float>();
		
		Stats.coinsLv1Spawned = 0;
		Stats.coinsLv2Spawned = 0;
		Stats.coinsLv3Spawned = 0;
		
		Stats.gemsSpawned = 0;
		
		Stats.bonusItemsSpawned = 0;
		Stats.bonusItemRecord = new Dictionary<BonusItem.BonusItemType, int>();
		
		Stats.obstaclesSpawned = 0;
		Stats.obstacleRecord = new Dictionary<ObstacleType, int>();
		Stats.timesBetweenObstacles = new List<float>();
	}
	
	public static StatSet GetStats()
	{
		return Stats;
	}
	
	public static string GetStatsString()
	{
		string result = "";
		result += "Running Time: "+Stats.fTimeRunning+ " ";
		result += "Turns Spawned: "+Stats.turnsSpawned+ " ";
		result += "Average Time Between Turns:"+Stats.averageTimeBetweenTurns+ " ";
		result += "Coins Spawned: "+Stats.coinsSpawned+ " ";
		result += "--Lv1 Coins: "+Stats.coinsLv1Spawned+ " ";
		result += "--Lv2 Coins: "+Stats.coinsLv2Spawned+ " ";
		result += "--Lv3 Coins: "+Stats.coinsLv3Spawned+ " ";
		result += "Gems Spawned: "+Stats.gemsSpawned+ " ";
		result += "Bonus Items Spawned: "+Stats.bonusItemsSpawned+ " ";
		result += "Obstacles Spawned: "+Stats.obstaclesSpawned+ " ";		
		return result;
	}
	
	public static string GetStatsCommaDelimitedString()
	{
		string result = "";
		result += Stats.fTimeRunning + "," + 
			Stats.turnsSpawned + "," +
			Stats.averageTimeBetweenTurns + "," +
			Stats.coinsSpawned + "," +
			Stats.coinsLv1Spawned + "," +
			Stats.coinsLv2Spawned + "," +
			Stats.coinsLv3Spawned + "," +
			Stats.gemsSpawned + "," +
			Stats.bonusItemsSpawned + "," +
			Stats.obstaclesSpawned;
		
		return result;
	}
	
	//Update Functions
	
	public static void TurnSpawned()
	{
		if(isRunning)
		{
			Stats.turnsSpawned++;
			
			Stats.timesBetweenTurns.Add(Time.time-lastTurnTime);
			lastTurnTime = Time.time;
		}
	}
	
	public static void CoinLv1Spawned()
	{
		if(isRunning)	Stats.coinsLv1Spawned++;
	}
	public static void CoinLv2Spawned()
	{
		if(isRunning)	Stats.coinsLv2Spawned++;
	}
	public static void CoinLv3Spawned()
	{
		if(isRunning)	Stats.coinsLv3Spawned++;
	}
	
	public static void GemSpawned()
	{
		if(isRunning)	Stats.gemsSpawned++;
	}
	
	
	public static void BonusItemSpawned(BonusItem.BonusItemType type)
	{
		if(isRunning)	
		{
			if(!Stats.bonusItemRecord.ContainsKey(type))	
                Stats.bonusItemRecord.Add(type,0);
			Stats.bonusItemRecord[type]++;
			Stats.bonusItemsSpawned++;
		}
	}
	
	
	public static void ObstacleSpawned(ObstacleType type)
	{
		if(isRunning)
		{
			if(!Stats.obstacleRecord.ContainsKey(type))		Stats.obstacleRecord.Add(type,0);
			Stats.obstacleRecord[type]++;
			
			Stats.obstaclesSpawned++;
			
			Stats.timesBetweenObstacles.Add(Time.time-lastTurnTime);
			lastTurnTime = Time.time;
		}
	}
	
	
}



