using System;
using System.Collections.Generic;
using UnityEngine;

public class UIConsumablesList : MonoBehaviour
{
    protected static Notify notify;
    private List<GameObject> childCells = new List<GameObject>();
    private GameObject grid;
    private List<BaseConsumable> sortedDataList = new List<BaseConsumable>();

    private void Awake()
    {
        notify = new Notify(GetType().Name);
        grid = gameObject;
        sortedDataList = SortGridItemsByPriority(ConsumableStore.consumablesList);
        //
        Initialize();
    }

    private void Start()
    {
        //Refresh();
    }

    public void Refresh()
    {
        // Ensure that data list has been populated
        if (sortedDataList != null && sortedDataList.Count > 0)
        {
            sortedDataList = SortGridItemsByPriority(sortedDataList);

            var cellIndex = 0;

            foreach (var childCell in childCells)
            {
                var data = sortedDataList[cellIndex];

                if (data.Type.Equals("DeadBoostConsumables"))
                { 
                    data = sortedDataList[++cellIndex];
                }

                childCell.GetComponent<ConsumableCellData>().SetData(data);
                childCell.name = GenerateCellLabel(data);
                cellIndex++;
            }

            transform.parent.GetComponent<UIScrollView>().ResetPosition(); //
        }
    }

    public void Initialize()
    {
        childCells = CreateCells();

        grid.GetComponent<UIGrid>().Reposition();
    }

    private List<GameObject> CreateCells()
    {
        var newObjs = new List<GameObject>();

        foreach (var consumableData in sortedDataList)
        {
            var type = (ConsumableType) Enum.Parse(typeof (ConsumableType), consumableData.Type);
            if (type >= ConsumableType.LevelItem || type == ConsumableType.DeadBoostConsumables)
                continue;

            var panel = CreatePanel(consumableData, grid);
            panel.name = GenerateCellLabel(consumableData);
            newObjs.Add(panel);
        }

        return newObjs;
    }

    private string GenerateCellLabel(BaseConsumable consumableData)
    {
        return ("Cell_" + consumableData.SortPriority.ToString("D8") + "_" + consumableData.PID.ToString("D8"));
    }

    public void OnConsumableCellPressed(GameObject cell)
    {
        //Services.Get<NotificationSystem>().ClearNotification(NotificationType.Consumable, cell.transform.parent.GetComponent<ConsumableCellData>()._data.PID);
    }

    public void Reposition()
    {
        grid.GetComponent<UIGrid>().Reposition();
    }

    private List<BaseConsumable> SortGridItemsByPriority(List<BaseConsumable> list) //unsortedList)
    {
        list.Sort((a1, a2) => a1.SortPriority.CompareTo(a2.SortPriority));
        return list;
    }

    private GameObject CreatePanel(BaseConsumable _data, GameObject _grid)
    {
        var obj = (GameObject) Instantiate(Resources.Load("ConsumableStoreCellOz"));
        obj.transform.parent = _grid.transform;
        obj.transform.localScale = Vector3.one;
        obj.transform.rotation = grid.transform.rotation;
        obj.transform.localPosition = Vector3.zero;
        obj.GetComponent<ConsumableCellData>()._data = _data;
        return obj;
    }
}