using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerLeaderboardCell : MonoBehaviour 
{
	protected static Notify notify;
	
	private float fourCellYposition = 56f;//just moves the top position, increasing the number moves it higher, was 48
	private float fiveCellYposition = 0f;
	
	private float fourCellClipYScale = 409f;//expands-decreases the size in both directions, was 422
	private float fiveCellClipYScale = 520f;	
	
	void Awake()
	{
		notify = new Notify("PlayerLeaderboardCell");
		notify.Debug( "[PlayerLeaderboardCell] Awake" );		
		
		//notificationIcons = gameObject.GetComponent<NotificationIcons>();
	}	
	
	/// <summary>
	/// Sets the bottom player cell status.
	/// </summary>
	public void SetPlayerCellStatus(UIPanel clippedPanel, UILeaderboardList scrollList, SocialScreenName listType, 
		PlayerLeaderboardCell playerCell, LeaderboardManager.LeaderboardEntry playerData)
	{
		notify.Debug( "[PlayerLeaderboardCell] SetPlayerCellStatus" );		
		
		Transform scrollListRoot = scrollList.transform.parent;
		PlayerLeaderboardCellRoot playerCellRoot = playerCell.transform.parent.GetComponent<PlayerLeaderboardCellRoot>();
		int rank = Services.Get<LeaderboardManager>().GetUserRank(listType);
		
		if (rank > 5)
		{
			scrollListRoot.localPosition = new Vector3(scrollListRoot.localPosition.x, fourCellYposition, scrollListRoot.localPosition.z);
            clippedPanel.baseClipRegion = new Vector4(clippedPanel.baseClipRegion.x, clippedPanel.baseClipRegion.y, clippedPanel.baseClipRegion.z, fourCellClipYScale);
			
			playerCellRoot.SetCellVisible(true);	//playerCell.gameObject.SetActive(true);
			playerCell.GetComponent<LeaderboardCellData>().SetData(playerData, scrollList, true, true);
		}
		else
		{
			scrollListRoot.localPosition = new Vector3(scrollListRoot.localPosition.x, fiveCellYposition, scrollListRoot.localPosition.z);
            clippedPanel.baseClipRegion = new Vector4(clippedPanel.baseClipRegion.x, clippedPanel.baseClipRegion.y, clippedPanel.baseClipRegion.z, fiveCellClipYScale);
			playerCellRoot.SetCellVisible(false);	// playerCell.gameObject.SetActive(false);
		}
	}
}




		//int rank = Services.Get<LeaderboardManager>().GetUserRank(listType == SocialScreenName.TopScores);

			//playerCell.transform.parent.GetComponent<PlayerLeaderboardCellRoot>().divider.enabled = true;

			//playerCell.GetComponent<LeaderboardCellData>().SetData(playerData, scrollList, true);

			//playerCell.GetComponent<LeaderboardCellData>().scrollList = scrollList;

