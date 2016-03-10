using UnityEngine;
using System.Collections;

public class UIPostGame : MonoBehaviour {
	
	public GamePlayer Player;
	public UILabel ScoreLabel;
	public UILabel DistanceLabel;
	public UILabel CoinCountLabel;
	public UILabel MultiplierLabel;

	// Use this for initialization
	void Start () {
		Player = GamePlayer.SharedInstance;
		ScoreLabel.text = Player.Score.ToString();
		CoinCountLabel.text = Player.CoinCountTotal.ToString();
		//MultiplierLabel.text = GameProfile.SharedInstance.Player.scoreMultiplier.ToString();
		DistanceLabel.text = GameController.SharedInstance.DistanceTraveled.ToString();
	}
	
	// Update is called once per frame
	void Update () {
		ScoreLabel.text = Player.Score.ToString();
		CoinCountLabel.text = Player.CoinCountTotal.ToString();
		//MultiplierLabel.text = GameProfile.SharedInstance.Player.scoreMultiplier.ToString();
		DistanceLabel.text = GameController.SharedInstance.DistanceTraveled.ToString();
	}
}
