using System.Collections.Generic;
using UnityEngine;

public class UIPowersList : MonoBehaviour
{
    protected static Notify notify;
    private List<GameObject> childCells = new List<GameObject>();
    private GameObject grid;
    private PowerCellData powerupToPurchase; // temp reference, for use when purchasing a specific item
    private List<BasePower> sortedDataList = new List<BasePower>();
    public GameObject viewController;

    private void Awake()
    {
        notify = new Notify(GetType().Name);
        grid = gameObject; // connect to this panel's grid automatically			
        sortedDataList = SortGridItemsByPriority(PowerStore.Powers);
        Initialize();
    }

    private void Start()
    {
        //notificationSystem = Services.Get<NotificationSystem>();	
        Refresh();
    }

    public void Refresh()
    {
        // ensure that the sortedDataList has been initialized
        if (sortedDataList != null && sortedDataList.Count > 0)
        {
            sortedDataList = SortGridItemsByPriority(sortedDataList);

            var cellIndex = 0;

            foreach (var childCell in childCells)
            {
                childCell.GetComponent<PowerCellData>().SetData(sortedDataList[cellIndex]);

                notify.Debug("[UIPowersList] - childCell name before: " + childCell.name);

                childCell.name = GenerateCellLabel(sortedDataList[cellIndex]);

                notify.Debug("[UIPowerList] - childCell name after: " + childCell.name);

                cellIndex++;
            }

            transform.parent.GetComponent<UIScrollView>().ResetPosition();
        }
    }

    public void Initialize()
    {
        childCells = CreateCells(); // create cell GameObject for each

        grid.GetComponent<UIGrid>().Reposition(); // reset/correct positioning of all objects inside grid
    }

    private List<GameObject> CreateCells()
    {
        var newObjs = new List<GameObject>();

        foreach (var powerData in sortedDataList)
        {
            var panel = CreatePanel(powerData, grid);
            panel.name = GenerateCellLabel(powerData);
            newObjs.Add(panel);
        }

        return newObjs;
    }

    private string GenerateCellLabel(BasePower powerData)
    {
        return ("Cell_" + powerData.SortPriority.ToString("D8") + "_" + powerData.PowerID.ToString("D8"));
    }

    public void OnPowerupCellPressed(GameObject cell)
    {
        //Services.Get<NotificationSystem>().ClearNotification(NotificationType.Powerup, cell.transform.parent.GetComponent<PowerCellData>()._data.PowerID);		
    }

    public void Reposition()
    {
        grid.GetComponent<UIGrid>().Reposition();
    }

    private List<BasePower> SortGridItemsByPriority(List<BasePower> list) //unsortedList)
    {
        //List<BasePower> listToSort = unsortedList.ToList();
        //listToSort = listToSort.OrderBy(x => x.SortPriority).ToList(); 
        list.Sort((a1, a2) => a1.SortPriority.CompareTo(a2.SortPriority));
        return list; //listToSort;
    }

    private GameObject CreatePanel(BasePower data, GameObject _grid)
    {
        var obj = (GameObject) Instantiate(Resources.Load("PowerStoreCellOz")); // instantiate objective from prefab	
        obj.transform.parent = _grid.transform;
        obj.transform.localScale = Vector3.one;
        obj.transform.rotation = grid.transform.rotation;
        obj.transform.localPosition = Vector3.zero;
        obj.GetComponent<PowerCellData>()._data = data; // store reference to data for this objective

        // move subpanel offscreen and turn it off
        //obj.GetComponent<SubPanel>().TurnSubPanelOff(obj.transform.Find("CellContents").gameObject);			
        return obj;
    }

    private void OnPurchaseYes()
    {
        // set up shorter local identifiers, to keep code easy to read
        //UIInventoryViewControllerOz invViewCont = viewController.GetComponent<UIInventoryViewControllerOz>();		
        var playerStats = GameProfile.SharedInstance.Player;

        playerStats.PurchasePower(powerupToPurchase._data.PowerID); // buy it if we can afford it
        UIManagerOz.SharedInstance.PaperVC.UpdateCurrency();
        //invViewCont.UpdateCurrency();								// will update coin and gem counts in UI				
        powerupToPurchase.Refresh();
            // ask cell to update its GUI rendering to match data, in case it was updated in the transaction		
        //UIConfirmDialogOz.onNegativeResponse -= OnPurchaseNo;
        //UIConfirmDialogOz.onPositiveResponse -= OnPurchaseYes;
    }

    public void CellBuyButtonPressed(GameObject cell) // public void OnPowerItemPressed(GameObject cell) 
    {
        //notify.Debug("CellBuyButtonPressed called at: " + Time.realtimeSinceStartup.ToString());

        // set up shorter local identifiers, to keep code easy to read
        //var invViewCont = viewController.GetComponent<UIInventoryViewControllerOz>();
        var powerCellData = cell.transform.parent.parent.parent.GetComponent<PowerCellData>();
        //PowerCellData powerCellData = cell.transform.parent.GetComponent<PowerCellData>();
        var powerID = powerCellData._data.PowerID;
        var activeCharacter = GameProfile.SharedInstance.GetActiveCharacter();
        var playerStats = GameProfile.SharedInstance.Player;

        Services.Get<NotificationSystem>().ClearNotification(NotificationType.Powerup, powerID);
        Services.Get<NotificationSystem>().SetNotificationIconsForThisPage(UiScreenName.UPGRADES);

        if (playerStats.IsPowerPurchased(powerID) == false) // check if already purchased
        {
            if (playerStats.CanAffordPower(powerID))
            {
                powerupToPurchase = powerCellData;
                //UIConfirmDialogOz.onNegativeResponse += OnPurchaseNo;
                //UIConfirmDialogOz.onPositiveResponse += OnPurchaseYes;
                //UIManagerOz.SharedInstance.confirmDialog.ShowConfirmDialog(powerCellData.data.Title, "Purchase this powerup?", "Btn_No", "Btn_Yes");
                //playerStats.PurchasePower(powerID);					
                OnPurchaseYes(); // buy it if we can afford it
            }
            else
            {
                UIManagerOz.SharedInstance.StoreVC.BuyCoins();
            }
        }
        else if (GameProfile.SharedInstance.IsPowerEquipped(powerID, activeCharacter.characterId) == false)
            // check if already equipped
        {
            //Change the power id for each character
            foreach (var character in GameProfile.SharedInstance.Characters)
            {
                character.powerID = powerID;
            }

            //activeCharacter.powerID = powerID;

//			equippedPowerupCell.GetComponent<EquippedPowerupCell>().UpdateEquippedCell(PowerStore.Powers[powerID]);	// change icon next to character
            //invViewCont.characterSelectVC.UpdateCharacterCard(activeCharacter);
            //UIManagerOz.SharedInstance.characterSelectVC.UpdateCharacterCard(activeCharacter);

            //AnalyticsInterface.LogGameAction("powerup", "equipped", ((PowerType)powerID).ToString(),GameProfile.GetAreaCharacterString(),0);
            GameProfile.SharedInstance.Serialize();

            // ask all power cells to update their GUI renderings, to match updated data
            var allPowerCells = grid.GetComponentsInChildren<PowerCellData>();
            foreach (var powerCell in allPowerCells)
            {
                powerCell.Refresh();
            }
        }
//		else 	//-- Can't equip this because its already equipped.
//		{
//			UIOkayDialogOz.onPositiveResponse += OnAlreadyEquipped;
//			UIManagerOz.SharedInstance.okayDialog.ShowOkayDialog("Oops!", "That PowerUp is already in use.", "Btn_Ok");	//-- Show error dialog
//		}

        UIManagerOz.SharedInstance.PaperVC.UpdateCurrency();
        //invViewCont.UpdateCurrency();								// will update coin and gem counts in UI				
        powerCellData.Refresh();
            // ask cell to update its GUI rendering to match data, in case it was updated in the transaction
    }
}