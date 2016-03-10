using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class UIObjectivesList : MonoBehaviour 
{
    public ObjectivesScreenName pageToLoad = ObjectivesScreenName.MainTask;

    private bool IsInitialized = false;
	
	public GameObject grid;

    public GameObject prefab;

	private List<GameObject> childObjectiveCells = new List<GameObject>();

	public List<ObjectiveProtoData> dataList = new List<ObjectiveProtoData>();
	
	protected static Notify notify;
	
	void Awake() 
	{ 
		notify = new Notify(this.GetType().Name);
//        IsInitialized = false;
	}
	
	public void PopulateTaskData()
	{
		
        if (pageToLoad == ObjectivesScreenName.MainTask)
		{	
            dataList = GameProfile.SharedInstance.Player.objectivesMain;//ObjectivesManager.MainObjectives;
        }
        else if (pageToLoad == ObjectivesScreenName.DailyTask)
		{
            dataList = GameProfile.SharedInstance.Player.objectivesDaily;// ObjectivesManager.AllDailyObjectives;

        }
        else if(pageToLoad == ObjectivesScreenName.Achievement)
        {
            dataList = ObjectivesManager.LegendaryObjectives;

        }

		if ( dataList.Count > 0 )
		{
//			dataList = dataList.GroupBy( x => x._id ).Select( y => y.First() ).ToList(); //创建了另个列表，会出现升级后任务列表不刷新问题

            if (pageToLoad == ObjectivesScreenName.Achievement)
            {
                //dataList = Services.Get<ObjectivesManager>().FlitrateLegendaryObjective();
                dataList = Services.Get<ObjectivesManager>().SortGridItemsById(dataList);
            }
            else
            {
                dataList = Services.Get<ObjectivesManager>().SortGridItemsByPriority(dataList);
            }
			
			if ( !IsInitialized )
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
        ////将已完成的成就任务置顶,将已领取奖励的成就置底
        //if (pageToLoad == ObjectivesScreenName.Achievement)
        //{
        //    dataList = Services.Get<ObjectivesManager>().SortlegendaryObjective(dataList);
        //}

		int i=0;
		foreach (GameObject childCell in childObjectiveCells)
		{
            if(pageToLoad == ObjectivesScreenName.DailyTask)
            {
                childCell.GetComponent<DailyTaskCellData>().SetData(dataList[i]);
            }
            else if(pageToLoad == ObjectivesScreenName.MainTask)
            {
                childCell.GetComponent<MainTaskCellData>().SetData(dataList[i]);
            }
            else if(pageToLoad == ObjectivesScreenName.Achievement)
            {
                childCell.GetComponent<AchieveCellData>().SetData(dataList[i]);
            }
			i++;
		}	
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
        int index =0;
		foreach (ObjectiveProtoData objectiveData in dataList)
        {
            GameObject obj = CreateObjectivePanel(objectiveData);
            obj.name = index.ToString();
            newObjs.Add(obj);
            ++index;
        }
		
		return newObjs;
	}	

	private GameObject CreateObjectivePanel(ObjectiveProtoData _objectiveData)	//string _title, string _description)
	{
        GameObject obj = (GameObject)Instantiate(prefab);
		obj.transform.parent = grid.transform;
		obj.transform.localPosition = Vector3.zero;		
		obj.transform.localScale = Vector3.one;
		obj.transform.rotation = grid.transform.rotation;
		return obj;
	}	
}
