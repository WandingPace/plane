using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerLeaderboardCellRoot : MonoBehaviour 
{
	protected static Notify notify;
	
	public PlayerLeaderboardCell playerLeaderboardCell;
	public UISprite background;
	public List<UISprite> playerCellAssets = new List<UISprite>();
	
	void Awake()
	{
		notify = new Notify("PlayerLeaderboardCellRoot");
		notify.Debug( "[PlayerLeaderboardCellRoot] Awake" );		
		
		GameObject playerCell = (GameObject)Instantiate(Resources.Load("LeaderboardsPlayerCellOz"));	
		playerCell.transform.parent = transform;
		playerCell.transform.localPosition = Vector3.zero;
		playerCell.transform.localScale = Vector3.one;
		playerLeaderboardCell = playerCell.AddComponent<PlayerLeaderboardCell>();
	}
	
	public void SetCellVisible(bool status)
	{
		notify.Debug( "[PlayerLeaderboardCellRoot] SetCellVisible" );		
		
		background.enabled = status;
		foreach(UISprite sprite in playerCellAssets)
		{
			sprite.enabled = status;
		}
			
		playerLeaderboardCell.GetComponent<LeaderboardCellData>().SetCellVisible(status);
	}
}
