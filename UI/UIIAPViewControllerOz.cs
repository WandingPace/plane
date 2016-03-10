using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIIAPViewControllerOz : UIViewControllerOz
{
	public List<GameObject> storePanelGOs = new List<GameObject>();
	
	private ShopScreenName pageToLoad = ShopScreenName.Gems;		
	
	public enum AndroidIAPMechanism
	{
		None,
		GooglePlay,
		Amazon
	};	
	
	public override void appear() 
	{
		base.appear();
		
		// set only appropriate panel active, make others inactive
		foreach (GameObject go in storePanelGOs)
			NGUITools.SetActive(go, false);
		NGUITools.SetActive(storePanelGOs[(int)pageToLoad], true);			
		
		// refresh panels prior to showing
		//foreach (GameObject go in storePanelGOs)
		//	go.GetComponent<UIStoreList>().Refresh();
		
		//paperViewController.ResetTabs((int)pageToLoad);		// reset highlighted tab to the one actually chosen		
	}	
	
	public void SwitchToPanel(ShopScreenName panelScreenName)	// activate panel upon button selection, passing in ShopScreenName
	{
		pageToLoad = panelScreenName;
	}
}





//	public void UpdateCurrency()
//	{
//		this.updateCurrency();
//	}	
//}
//	
//	public void ShowScreen(ShopScreenName _name)
//	{
//		pageToLoad = _name;
//	}	

		//gemsPanel.GetComponent<UIArtifactsList>().Refresh();
		//coinsPanel.GetComponent<UIPowersList>().Refresh();		
		//miscPanel.GetComponent<UIStatsList>().Refresh();
		//coinOffersPanel.GetComponent<UIConsumablesList>().Refresh();
		
//		NGUITools.SetActive(gemsPanel, (screenToLoad == ShopScreenName.Gems) ? true : false);	
//		NGUITools.SetActive(coinsPanel, (screenToLoad == ShopScreenName.Coins) ? true : false);
//		NGUITools.SetActive(miscPanel, (screenToLoad == ShopScreenName.Misc) ? true : false);
//		NGUITools.SetActive(coinOffersPanel, (screenToLoad == ShopScreenName.CoinOffers) ? true : false);	

	
//	public GameObject gemsPanel = null;
//	public GameObject coinsPanel = null;
//	public GameObject miscPanel = null;
//	public GameObject coinOffersPanel = null;	