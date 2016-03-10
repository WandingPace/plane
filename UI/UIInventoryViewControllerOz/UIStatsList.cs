using UnityEngine;
using System.Collections;

public class UIStatsList : MonoBehaviour
{
	public GameObject viewController;
	
	public GameObject RecentGroup;
	public GameObject SingleRunGroup;
	public GameObject LifetimeGroup;
	
	void Start() 
	{
		Refresh();
	}
	
	//void Update() { }
	
	public void Refresh()
	{
        RecentGroup.transform.Find("LabelMultiplier#").GetComponent<UILabel>().text = GameProfile.SharedInstance.GetTotalScoreMultiplier() + "x";	//GetScoreMultiplier
		
		RecentGroup.transform.Find("LabelRecentDistance#").GetComponent<UILabel>().text = ((int)GameController.SharedInstance.DistanceTraveled).ToString();	
		RecentGroup.transform.Find("LabelRecentScore#").GetComponent<UILabel>().text = GamePlayer.SharedInstance.Score.ToString();
		
		SingleRunGroup.transform.Find("LabelHighScore#").GetComponent<UILabel>().text = GameProfile.SharedInstance.Player.bestScore.ToString();	
		SingleRunGroup.transform.Find("LabelRun#").GetComponent<UILabel>().text = GameProfile.SharedInstance.Player.bestDistanceScore.ToString();		
		SingleRunGroup.transform.Find("LabelMostCoins#").GetComponent<UILabel>().text = GameProfile.SharedInstance.Player.bestCoinScore.ToString();	
		SingleRunGroup.transform.Find("LabelMostGems#").GetComponent<UILabel>().text = GameProfile.SharedInstance.Player.bestSpecialCurrencyScore.ToString();	

		LifetimeGroup.transform.Find("LabelGames#").GetComponent<UILabel>().text = GameProfile.SharedInstance.Player.lifetimePlays.ToString();	
		LifetimeGroup.transform.Find("LabelDistance#").GetComponent<UILabel>().text = GameProfile.SharedInstance.Player.lifetimeDistance.ToString();	
		LifetimeGroup.transform.Find("LabelTotalCoins#").GetComponent<UILabel>().text = ObjectivesDataUpdater.GetLifetimeStat(ObjectiveType.CollectCoins,-1).ToString();	
		LifetimeGroup.transform.Find("LabelTotalGems#").GetComponent<UILabel>().text = GameProfile.SharedInstance.Player.lifetimeSpecialCurrency.ToString();		
	}
}
