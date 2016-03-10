using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class MapLocation 
{
	public string title = "";	
	public string description = "";
	public string icon = "";
	public int cost = 1;			// coins required for Fast Travel
	public int sortPriority = 0;	
	public int id = 0;				// corresponds to EnvironmentSetManager.SharedInstance.CurrentEnvironmentSet.SetId
	
	public bool isLocallyAvailable = false;		// not really used anymore
	
	public bool _showFoldOut = false;
	
	public void SetDataFromDictionary(Dictionary<string, object> data)
	{
		if (data.ContainsKey("Title"))
			title = (string)data["Title"];
		
		if (data.ContainsKey("Description"))
			description = (string)data["Description"];
		
		if (data.ContainsKey("Icon"))
			icon = (string)data["Icon"];

		if (data.ContainsKey("Cost"))
			cost = int.Parse((string)data["Cost"]);
		
		if (data.ContainsKey("SortPriority"))
			sortPriority = int.Parse((string)data["SortPriority"]);
		
		if (data.ContainsKey("ID"))
			id = int.Parse((string)(data["ID"]));
	}
	
	public string ToJson() 
	{
		Dictionary<string, object> d = this.ToDict();
		return MiniJSON.Json.Serialize(d);
	}
	
	public Dictionary<string, object> ToDict() 
	{
		Dictionary<string, object> d = new Dictionary<string, object>();
		d.Add ("Title", title);
		d.Add ("Description", description);	
		d.Add ("Icon", icon);
		d.Add ("Cost", cost.ToString());
		d.Add ("SortPriority", sortPriority.ToString());
		d.Add ("ID", id.ToString());
		return d;
	}
}




	//public int markDownCost = 1;	
	//public StoreItemType itemType = StoreItemType.CoinBundle;
	//public int itemQuantity = 1;	


		//d.Add ("InternalTitle", internalTitle);		
		//d.Add ("CostType", costType.ToString());				
		//d.Add ("MarkDownCost", markDownCost.ToString());
		//d.Add ("ItemType", itemType.ToString());
		//d.Add ("ItemQuantity", itemQuantity.ToString());	
		
	
		//if (data.ContainsKey("InternalTitle"))
		//	internalTitle = (string)data["InternalTitle"];
				
		
		//if (data.ContainsKey("CostType")) 
		//	costType = (CostType)Enum.Parse(typeof(CostType), (string)data["CostType"]);
				
		
		//if (data.ContainsKey("MarkDownCost"))
		//		markDownCost = int.Parse((string)data["MarkDownCost"]);
		
		//if (data.ContainsKey("ItemType"))
		//	itemType = (StoreItemType)Enum.Parse(typeof(StoreItemType), (string)data["ItemType"]);
		
		//if (data.ContainsKey("ItemQuantity"))
		//	itemQuantity = int.Parse((string)data["ItemQuantity"]);		
		