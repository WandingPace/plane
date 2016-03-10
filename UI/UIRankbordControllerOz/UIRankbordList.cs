using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class UIRankbordList : MonoBehaviour
{
    //public RankingScreenName pageToLoad = RankingScreenName.rankhistory;
    //public RankingHistoryScreen historypageToLoad = RankingHistoryScreen.rankworld;
    public bool IsInitialized { get; private set; }

    public GameObject grid;

    public GameObject prefab;

    private List<GameObject> childObjectiveCells = new List<GameObject>();

     List<RankProtoData> dataList = new List<RankProtoData>();

    public RankProtoData playerdata = new RankProtoData();

    protected static Notify notify;

    void Awake()
    {
        notify = new Notify(this.GetType().Name);

        IsInitialized = false;
    }



    public void PopulateTaskData()
    {

        if (UIRankingbordControllerOz.pageToLoad == RankingScreenName.rankhistory)
        {
            if (UIRankingbordControllerOz.historypageToLoad == RankingHistoryScreen.rankworld)

                dataList = Rankdata.GetHistoryworldData();
            else if (UIRankingbordControllerOz.historypageToLoad == RankingHistoryScreen.ranklocal)
                dataList = Rankdata.Getdata();
        }
        else if (UIRankingbordControllerOz.pageToLoad == RankingScreenName.rankfriend)
        {
              dataList = Rankdata.Getdata();

        }

        

        if (dataList.Count > 0)
        {
            dataList.Add(playerInfodata());

            SortGridItemsByPriority(dataList);
      
          //  dataList = Services.Get<ObjectivesManager>().SortGridItemsByPriority(dataList);


            if (!IsInitialized)
            {
                Initialize();
            }

            RefreshCells();

        }

    }

    public void Refresh()
    {
        if (!IsInitialized) //&& Initializer.IsBuildVersionPassThreshold())
        {
            PopulateTaskData();
        }
        else
        {
            RefreshCells();
        }
    }

    public void RefreshCells()
    {
        int i = 0;
        foreach (GameObject childCell in childObjectiveCells)
        {
            if (UIRankingbordControllerOz.pageToLoad == RankingScreenName.rankhistory)
            {
                if (UIRankingbordControllerOz.historypageToLoad == RankingHistoryScreen.rankworld)
                    childCell.GetComponent<RankCellData>().SetData(dataList[i]);
                else if (UIRankingbordControllerOz.historypageToLoad == RankingHistoryScreen.ranklocal)
                    childCell.GetComponent<RankCellData>().SetData(dataList[i]);
            
            }
            else if (UIRankingbordControllerOz.pageToLoad == RankingScreenName.rankfriend)
            {
                childCell.GetComponent<RankCellData>().SetData(dataList[i]);
            }

            i++;
        }
        grid.GetComponent<UIGrid>().Reposition();
        grid.transform.parent.GetComponent<UIScrollView>().ResetPosition();
    }

    public void Initialize()
    {
        ClearGrid(grid);										// kill all old objects under grid, prior to initialization
        childObjectiveCells = CreateCells();					// create cell GameObjects for all objectives
        grid.GetComponent<UIGrid>().Reposition();				// reset/correct positioning of all objects inside grid
        grid.transform.parent.GetComponent<UIScrollView>().ResetPosition();
        IsInitialized = true;
    }

    private void ClearGrid(GameObject _grid)
    {
        UIDragScrollView[] contentArray = _grid.GetComponentsInChildren<UIDragScrollView>();
        foreach (UIDragScrollView contents in contentArray)
        {
            contents.transform.parent = null;	// unparent first to remove bug when calling NGUI's UIGrid.Reposition(), because Destroy() is not immediate!
            Destroy(contents.gameObject);
        }
    }

    private List<GameObject> CreateCells()
    {
        List<GameObject> newObjs = new List<GameObject>();
        int index = 0;
        foreach (RankProtoData objectiveData in dataList)
        {
            GameObject obj = CreateObjectivePanel(objectiveData);
            obj.name = (index+1).ToString();
            newObjs.Add(obj);
            ++index;
        }

        return newObjs;
    }

    private GameObject CreateObjectivePanel(RankProtoData _objectiveData)	//string _title, string _description)
    {
        GameObject obj = (GameObject)Instantiate(prefab);
        obj.transform.parent = grid.transform;
        obj.transform.localPosition = Vector3.zero;
        obj.transform.localScale = Vector3.one;
        obj.transform.rotation = grid.transform.rotation;
        return obj;
    }

    public List<RankProtoData> SortGridItemsByPriority(List<RankProtoData> list)
    {
        notify.Debug("SortGridItemsByPriority called in UIObjectivesList");
        list.Sort((a2, a1) => a1._nScore.CompareTo(a2._nScore));
        return list;	//listToSort;
    }

    public RankProtoData playerInfodata() //
    {
       
        playerdata._nRank = 0;
        playerdata._IconIndex = GameProfile.SharedInstance.Player.playerIconIndex;
        playerdata._nScore = GameProfile.SharedInstance.Player.bestScore;
        playerdata._nameStr = GameProfile.SharedInstance.Player.playerName;
        return playerdata;
    }
}
