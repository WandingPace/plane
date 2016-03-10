using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//public class WrappedList	// necessary for JSON serialization & deserialization of Menu Tutorial popupss
//{
//	public List<int> list = new List<int>();
//	
//	public WrappedList() { }
//	
//	public WrappedList(List<int> sourceList)
//	{
//		list = sourceList;
//	}
//}

public class MenuTutorials : MonoBehaviour 
{
	public List<int> menuTutorialsActivated = new List<int>();	
	
	protected static Notify notify; 
	
	void Awake()
	{
		notify = new Notify(this.GetType().Name);
	}
	
	void Start()
	{
		menuTutorialsActivated = RestoreMenuTutorialStatusFromPlayerPrefs();	// load list of previously shown 'menu tutorial popup' ID strings
	}
	
	public void SendEvent(int eventID)	
	{	
		//return;		// bypass all this until loading & saving works
		
		if (menuTutorialsActivated.Contains(eventID))	// do nothing if this 'menu tutorial popup' has been already shown
			return;
		
		bool saveNeeded = true;
		
		switch (eventID)
		{
			case 0:		// 1st time powerups tab entered
				//UIManagerOz.SharedInstance.okayDialog.ShowOkayDialog("Tut_Menu_Powerup", "", "Btn_Ok");
				UIManagerOz.SharedInstance.okayDialog.ShowOkayDialog("Tut_Menu_Powerup", "Btn_Ok");
				break;
			case 1:		// 1st time artifacts tab entered
				//UIManagerOz.SharedInstance.okayDialog.ShowOkayDialog("Tut_Menu_ModifierPrompt_Title", "Tut_Menu_ModifierPrompt_Desc", "Btn_Ok");
				UIManagerOz.SharedInstance.okayDialog.ShowOkayDialog("Tut_Menu_ModifierPrompt_Desc", "Btn_Ok");
				break;
			case 2:		// 1st time consumables tab entered
				//UIManagerOz.SharedInstance.okayDialog.ShowOkayDialog("Tut_Menu_UtilitiesPrompt_Title", "Tut_Menu_UtilitiesPrompt_Desc", "Btn_Ok");
				UIManagerOz.SharedInstance.okayDialog.ShowOkayDialog("Tut_Menu_UtilitiesPrompt_Desc", "Btn_Ok");
				break;
			case 3:		// 1st time	artifact is purchased/unlocked
				//UIManagerOz.SharedInstance.okayDialog.ShowOkayDialog("Tut_Menu_ModifierPurchase", "", "Btn_Ok");
				UIManagerOz.SharedInstance.okayDialog.ShowOkayDialog("Tut_Menu_ModifierPurchase", "Btn_Ok");
				break;
			case 4:		// 1st time having enough coins to purchase something
				if (GameProfile.SharedInstance.Player.coinCount >= 2500)
				{
					UIConfirmDialogOz.onNegativeResponse += DisconnectHandlers;
					UIConfirmDialogOz.onPositiveResponse += GoToAbilitiesPage;
					//UIManagerOz.SharedInstance.confirmDialog.ShowConfirmDialog("Tut_Menu_PostRun", "", "Btn_No", "Btn_Yes");
					UIManagerOz.SharedInstance.confirmDialog.ShowConfirmDialog("Tut_Menu_PostRun", "Btn_No", "Btn_Yes");
				}
				else
					saveNeeded = false;		// didn't get the required number of coins yet to purchase anything
				break;
			default:
				saveNeeded = false;			// ID not found, so skip saving
				break;
		}
			
		if (saveNeeded)
		{
			menuTutorialsActivated.Add(eventID);
			SaveMenuTutorialsDict();
		}
	}
	
	private List<int> RestoreMenuTutorialStatusFromPlayerPrefs()
	{
//		string pref = PlayerPrefs.GetString("menuTutorials", "{}");	// get string from player prefs
//		//pref = (pref == "{}") ? pref : StringCompressor.DecompressString(pref);	// decompress, if applicable
//		notify.Debug("Loading menuTutorials status: " + pref);		
//		WrappedList wrappedList1 = new WrappedList();
//		SerializationUtils.FromJson(wrappedList1, pref);
//		return wrappedList1.list;
		string pref = PlayerPrefs.GetString("menuTutorials", "");	// get string from player prefs
		return ListToStringConverter.GetListFromString(pref);
	}

	private void SaveMenuTutorialsDict()
	{
		//string compressed = StringCompressor.CompressString(MiniJSON.Json.Serialize(dict));
		//string compressed = MiniJSON.Json.Serialize(dict);
		//string compressed = SerializationUtils.ToJson(new WrappedList(menuTutorialsActivated));
		string compressed = ListToStringConverter.MakeStringFromList<int>(menuTutorialsActivated);
		PlayerPrefs.SetString("menuTutorials", compressed);	//compressed);
		notify.Debug("Saving menuTutorials status (menuTutorials): " + compressed + " / List.Count = " + menuTutorialsActivated.Count);
	}	
	
	public static void ResetMenuTutorialsInPlayerPrefs()
	{
		PlayerPrefs.SetString("menuTutorials", "");
		PlayerPrefs.Save();
	}		

	private void GoToAbilitiesPage()
	{
		DisconnectHandlers();
		
		UIManagerOz.SharedInstance.inventoryVC.LoadThisPageNextTime(UpgradesScreenName.Artifacts);
		UIManagerOz.SharedInstance.inventoryVC.appear();
		UIManagerOz.SharedInstance.PaperVC.appear();	
		UIManagerOz.SharedInstance.postGameVC.disappear();		
	}
	
	private void DisconnectHandlers()	
	{
		UIConfirmDialogOz.onNegativeResponse -= DisconnectHandlers;
		UIConfirmDialogOz.onPositiveResponse -= GoToAbilitiesPage;		
	}
}
