using UnityEngine;
using System.Collections;

public class SubPanel : MonoBehaviour 
{
	public GameObject scrollList;								// reference to the scroll list that this cell is parented under the grid/table of
		
	void Start()
	{
		// hide dotted divider
		UISprite dotted_divider = transform.Find("CellContents/Sprite (divider_store)").GetComponent<UISprite>();
		iTween.ScaleTo(dotted_divider.gameObject, new Vector3(dotted_divider.transform.localScale.x, 0.0f, 1.0f), 0.01f);
	}
	
	public GameObject OnCellPressed(GameObject cell, GameObject selectedCell) 	
	{
		GameObject newSelectedCell;
		
		if (cell == selectedCell)		// just close it
		{
			ResizeCell(selectedCell, false);
			newSelectedCell = null;
		}
		else
		{
			if (selectedCell != null)	// is there a selected cell? If so, close it.
				ResizeCell(selectedCell, false);
			
			ResizeCell(cell, true);		// open the clicked cell
			newSelectedCell = cell;
		}
		
		return newSelectedCell;
	}		
	
	public void ResizeCell(GameObject cell, bool open)
	{
		Vector3 newSubPanelPos = (open) ? new Vector3(0.0f, -165.0f, 0.0f) : new Vector3(0.0f, -35.0f, 0.0f);
		GameObject subPanel = cell.transform.Find("SubPanel").gameObject;
		float time = 0.3f;		
		
		if (!open)		// closing subpanel
			MoveSubPanelItems(cell, false);		// move sub panel items out of the way		
		else 			// opening subpanel
		{
			iTween.MoveTo(subPanel, iTween.Hash(	//iTween.MoveTo(divider, newDividerPos, 0.5f);
			"isLocal", true,
			"position", newSubPanelPos,
			"onupdatetarget", scrollList,
			"onupdate", "Reposition",
			"oncompletetarget", gameObject,
			"oncomplete", "BringInSubPanelItems",
			"oncompleteparams", cell,
			"time", time,
			"easetype", iTween.EaseType.easeOutSine));
			
			SetArrowDivider(cell, true, time);
			SetBackgroundScale(cell, true, time);
		}
	}
	
	private void CloseSubPanel(GameObject cell)
	{
		Vector3 newSubPanelPos = new Vector3(0.0f, -35.0f, 0.0f);
		GameObject subPanel = cell.transform.Find("SubPanel").gameObject;		
		float time = 0.3f;
		
		TurnSubPanelOff(cell);					// kill sub panel items before repositioning
		
		iTween.MoveTo(subPanel, iTween.Hash(	//iTween.MoveTo(divider, newDividerPos, 0.5f);
			"isLocal", true,
			"position", newSubPanelPos,
			"onupdatetarget", scrollList,
			"onupdate", "Reposition",
			"oncompletetarget", scrollList,	//gameObject,
			"oncomplete", "AnimationDone",
			//"oncompleteparams", cell,			
			"time", time,
			"easetype", iTween.EaseType.easeOutSine));	
		
		SetArrowDivider(cell, false, time);	
		SetBackgroundScale(cell, false, time);
	}
	
	private void SetBackgroundScale(GameObject cell, bool state, float time)
	{
		UISprite background = cell.transform.Find("Sprite (bg_storecell_opened)").GetComponent<UISprite>();
		float scale = state ? 260.0f : 130.0f;
		
		iTween.ScaleTo(background.gameObject, iTween.Hash(
			"isLocal", true,
			"scale", new Vector3(background.transform.localScale.x, scale, 1.0f),
			"time", time,
			"easetype", iTween.EaseType.easeOutSine));		
	}
	
	private void SetArrowDivider(GameObject cell, bool state, float time)
	{
		UISprite arrow = cell.transform.Find("SubPanel").Find("Sprite (button_slider_content)").GetComponent<UISprite>();
		UISprite divider = cell.transform.Find("SubPanel").Find("Sprite (button_expandstore)").GetComponent<UISprite>();
		UISprite dotted_divider = cell.transform.Find("Sprite (divider_store)").GetComponent<UISprite>();
		
		float mult1 = state ? 0.0f : 1.0f;
		float mult2 = state ? 1.0f : 0.0f;
		
		iTween.ScaleTo(arrow.gameObject, new Vector3(arrow.transform.localScale.x, 39.0f * mult1, 1.0f), time);	
		iTween.ScaleTo(divider.gameObject, new Vector3(divider.transform.localScale.x, 59.0f * mult1, 1.0f), time);
		iTween.ScaleTo(dotted_divider.gameObject, new Vector3(dotted_divider.transform.localScale.x, 6.0f * mult2, 1.0f), time * 2.0f);
	}	

	private void BringInSubPanelItems(GameObject cell)
	{
		MoveSubPanelItems(cell, true);
	}
	
	private void MoveSubPanelItems(GameObject cell, bool open)
	{
		Vector3 newBuyButtonPos = (open) ? new Vector3(210.0f, 35.0f, 0.0f) : new Vector3(710.0f, 35.0f, 0.0f);
		Vector3 newDescTextPos = (open) ? new Vector3(-300.0f, 98.0f, -2.0f) : new Vector3(-800.0f, 98.0f, -2.0f);

		GameObject buyButton = cell.transform.Find("SubPanel").Find("BuyButton").gameObject;
		GameObject descriptionText = cell.transform.Find("SubPanel").Find("Description").gameObject;
		
		//string onComplete = "";		
		
		float time = 0.15f;
		
		if (!open)	// closing subpanel
			CloseSubPanel(cell);
		else 		// opening subpanel
		{
			//onComplete = "";
			TurnSubPanelOn(cell);							// turn on sub panel immediately if opening
				
			iTween.MoveTo(buyButton, iTween.Hash(			// move 'buy' button
				"isLocal", true,
				"position", newBuyButtonPos,
				"time", time,
				"easetype", iTween.EaseType.easeOutExpo));
			
			iTween.MoveTo(descriptionText, iTween.Hash(		// move description text
				"isLocal", true,
				"position", newDescTextPos,
				"oncompletetarget", scrollList,	//gameObject,
				"oncomplete", "AnimationDone",
				//"oncompleteparams", cell,
				"time", time, 
				"easetype", iTween.EaseType.easeOutExpo));
		}
	}
		
	private void TurnSubPanelOn(GameObject cell)
	{
		NGUITools.SetActive(cell.transform.Find("SubPanel").Find("BuyButton").gameObject, true);
		NGUITools.SetActive(cell.transform.Find("SubPanel").Find("Description").gameObject, true);
		
		BoxCollider boxCollider = cell.GetComponent<BoxCollider>();
		boxCollider.center = new Vector3(-25.0f, -70.0f, 0.0f);
		boxCollider.size = new Vector3(650.0f, 275.0f, 0.0f);		
	}
	
	public void TurnSubPanelOff(GameObject cell)
	{
		// reset position of sub panel items to offscreen before turning them off
		cell.transform.Find("SubPanel").Find("BuyButton").localPosition = new Vector3(710.0f, 35.0f, 0.0f);
		cell.transform.Find("SubPanel").Find("Description").localPosition = new Vector3(-800.0f, 98.0f, -2.0f);
		
		NGUITools.SetActive(cell.transform.Find("SubPanel").Find("BuyButton").gameObject, false);
		NGUITools.SetActive(cell.transform.Find("SubPanel").Find("Description").gameObject, false);

		BoxCollider boxCollider = cell.GetComponent<BoxCollider>();
		boxCollider.center = new Vector3(-25.0f, -5.0f, 0.0f);
		boxCollider.size = new Vector3(650.0f, 150.0f, 0.0f);			
	}		
}


	
//	private void FlipArrow(GameObject cell, bool state, float time)
//	{
//		UISprite arrow = cell.transform.Find("Sprite (button_slider_content)").GetComponent<UISprite>();
//		float mult = state ? 0.0f : 1.0f;
//		
//		//arrow.transform.localScale = new Vector3(arrow.transform.localScale.x, 38.6f * mult, 1.0f);
//		iTween.ScaleTo(arrow.gameObject, new Vector3(arrow.transform.localScale.x, 38.6f * mult, 1.0f), time);	
//	}
