using System.Collections.Generic;
using UnityEngine;

public class UILeaderboardList : MonoBehaviour
{
    //protected static Notify notify;
    protected static Notify notify;
    private List<LeaderboardCellData> childCells = new List<LeaderboardCellData>();
    private List<LeaderboardManager.LeaderboardEntry> dataList = new List<LeaderboardManager.LeaderboardEntry>();
    private GameObject grid;
    //public LeaderboardCellData playerCellData;		// shown when player is not in the top five
    //public string listName = "";
    public SocialScreenName listType;
    public PlayerLeaderboardCellRoot playerCellRoot;
    public UILeaderboardViewControllerOz viewController;
    //private NotificationSystem notificationSystem;

    private void Awake()
    {
        notify = new Notify(GetType().Name);
        notify.Debug("[UILeaderboardList] Awake");
        grid = gameObject.transform.Find("Grid").gameObject; // connect to this panel's grid automatically			
        //Initialize();
    }

    private void Start()
    {
        notify.Debug("[UILeaderboardList] Start");
        //notificationSystem = Services.Get<NotificationSystem>();
        //Refresh(dataList);
    }

    public void Refresh(List<LeaderboardManager.LeaderboardEntry> topValues)
    {
        notify.Debug("[UILeaderboardList] Refresh");

        LeaderboardManager.LeaderboardEntry playerData = null;

        DestroyCells();
        dataList = topValues;
        Initialize();

        var cellIndex = 0;

        foreach (var childCell in childCells)
        {
            if (dataList[cellIndex].isCurrentUser)
            {
                playerData = dataList[cellIndex]; // store reference to player data, for cell at bottom
            }

            childCell.SetData(dataList[cellIndex], this, true, false); // listName);
            childCell.gameObject.name = GenerateCellLabel(dataList[cellIndex]);
            cellIndex++;
        }

        // set status of player cell at bottom (show only if player is not in top 5)
        playerCellRoot.playerLeaderboardCell.SetPlayerCellStatus(gameObject.GetComponent<UIPanel>(),
            this, listType, playerCellRoot.playerLeaderboardCell, playerData);

        gameObject.GetComponent<UIScrollView>().ResetPosition();
    }

    public void Initialize()
    {
        notify.Debug("[UILeaderboardList] Initialize");

        childCells = CreateCells(); // create cell GameObject for each
//		grid.GetComponent<UIGrid>().sorted = false;	//true;
        grid.GetComponent<UIGrid>().Reposition(); // reset/correct positioning of all objects inside grid
    }

    public void Reposition()
    {
        notify.Debug("[UILeaderboardList] Reposition");

        grid.GetComponent<UIGrid>().Reposition();
    }

    public void TurnOffAllArrows()
    {
        notify.Debug("[UILeaderboardList] TurnOffAllArrows");

        foreach (var child in childCells) //foreach (GameObject child in childCells)
        {
            child.TurnOffBothArrows(); //child.GetComponent<LeaderboardCellData>().TurnOffBothArrows();
        }

        playerCellRoot.playerLeaderboardCell.GetComponent<LeaderboardCellData>().TurnOffBothArrows();
    }

    /// <summary>
    ///     Turns on  arrow in player's cell, and in bottom cell if rank > 5.
    /// </summary>
    /// <param name='up'>
    ///     true = up arrow, false = down arrow.
    /// </param>
    /// <param name='cellID'>
    ///     Cell ID, for player's cell in the scroll list.
    /// </param>
    public void TurnOnArrow(bool up, int cellID)
    {
        notify.Debug("[UILeaderboardList] TurnOnArrow");

        if (childCells.Count > cellID)
        {
            childCells[cellID].TurnOnArrow(up); //.GetComponent<LeaderboardCellData>().TurnOnArrow(up);

            var rank = Services.Get<LeaderboardManager>().GetUserRank(listType);

            if (rank > 5) // turn on bottom player cell arrow only if it's showing
            {
                playerCellRoot.playerLeaderboardCell.GetComponent<LeaderboardCellData>().TurnOnArrow(up);
            }
        }
    }

    public void CancelProfilePhotoDownloads()
    {
        SetupNotify();
        notify.Debug("[UILeaderboardList] CancelProfilePhotoDownloads");

        foreach (var child in childCells)
        {
            child.CancelDownload();
        }
    }

    private List<LeaderboardCellData> CreateCells()
    {
        notify.Debug("[UILeaderboardList] CreateCells");

        var newObjs = new List<LeaderboardCellData>();

        foreach (var data in dataList)
        {
            var panel = CreatePanel(data, grid); //GameObject panel = CreatePanel(data, grid);
            panel.name = GenerateCellLabel(data);
            newObjs.Add(panel);
        }

        return newObjs;
    }

    private void DestroyCells()
    {
        notify.Debug("[UILeaderboardList] Destroy Cells");

        foreach (var childCell in childCells) //foreach (GameObject childCell in childCells)
        {
            childCell.transform.parent = null;
            Destroy(childCell.gameObject);
        }
    }

    private string GenerateCellLabel(LeaderboardManager.LeaderboardEntry data)
    {
        notify.Debug("[UILeaderboardList] GenerateCellLabel");

        var isScores = (listType == SocialScreenName.TopScores); // true = scores, false = distances;
        var rank = isScores ? data.rankScore : data.rankDistance;

        return ("Cell_" + rank.ToString("D8"));
    }

    private LeaderboardCellData CreatePanel(LeaderboardManager.LeaderboardEntry _data, GameObject _grid)
        //, string cellName)
    {
        notify.Debug("[UILeaderboardList] CreatePanel");

        var obj = (GameObject) Instantiate(Resources.Load("LeaderboardsPlayerCellOz"));
            // instantiate objective cell from prefab
        obj.transform.parent = _grid.transform;
            // null passed in for parentCell, which means this is a regular size cell
        obj.transform.localScale = Vector3.one;
        obj.transform.rotation = grid.transform.rotation;
        obj.transform.localPosition = Vector3.zero;

        var cellData = obj.GetComponent<LeaderboardCellData>();
        //cellData._data = _data;						// store reference to data for this cell
        //obj.GetComponent<LeaderboardCellData>().viewController = viewController;	// pass on reference to view controller, for event response
        //cellData.scrollList = this;	//.gameObject;
        cellData.SetData(_data, this, false, false);
        return cellData; //obj;
    }

    private void SetupNotify()
    {
        if (notify == null)
        {
            notify = new Notify(GetType().Name);
        }
    }
}


//		int rank = 1;
//		
//		if (listType == SocialScreenName.TopScores)	//listName == "scores")
//		{
//			rank = data.rankScore;
//		}
//		else if (listType == SocialScreenName.TopDistances)	//listName == "distances")
//		{
//			rank = data.rankDistance;
//		}	


//		SocialScreenName listType = SocialScreenName.TopScores;
//		
//		if (listName == "distances")
//		{
//			listType = SocialScreenName.TopDistances;
//		}


//SetArrows();

//	private void SetArrows()
//	{
//		if (listName == "scores")
//		{
//			viewController.GetComponent<UILeaderboardViewControllerOz>().SetArrows(
//				Initializer.SharedInstance.GetDisplayLeaderboard(), SocialScreenName.TopScores);
//		}
//		else if (listName == "distances")
//		{
//			viewController.GetComponent<UILeaderboardViewControllerOz>().SetArrows(
//				Initializer.SharedInstance.GetDisplayLeaderboard(), SocialScreenName.TopDistances); 
//		}			
//	}