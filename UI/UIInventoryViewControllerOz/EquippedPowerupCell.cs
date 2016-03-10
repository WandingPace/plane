using UnityEngine;
using System.Collections;

public class EquippedPowerupCell : MonoBehaviour 
{	
	//private UISprite icon;
	
	void Start() 
	{
		//icon = gameObject.transform.Find("Icon").GetComponent<UISprite>();
	}
	
	void Update()
	{
		//icon.spriteName = PowerStore.Powers[GameProfile.SharedInstance.GetActiveCharacter().powerID].IconName;
	}
	
	public void UpdateEquippedCell(BasePower data) 
	{
		// populate fields from data		
		gameObject.transform.Find("Icon").GetComponent<UISprite>().spriteName = data.IconName;			
	}
}


//		}
//		else 
//		{
//			//-- Nothing equipped.
//			if (equippedCellTitle) { equippedCellTitle.text = ""; }
//			if (equippedCellDesc) { equippedCellDesc.text = ""; }
//			if (equippedCellBuff) { NGUITools.SetActive(equippedCellBuff.gameObject, false); }
//			if (equippedCellIcon) { equippedCellIcon.spriteName = "unknown"; }	//"empty1x1";
//			//if (equippedCellEquipButton) { NGUITools.SetActive(equippedCellEquipButton.gameObject, false); }
//			//if (equippedCellProgressbar) { NGUITools.SetActive(equippedCellProgressbar.gameObject, false); }
//		}

		
	//public BasePower data;		// reference to equipped powerup data
	
	//public UILabel equippedCellTitle = null;		// Title
	//public UILabel equippedCellDesc = null;			// Description
	//public UILabel equippedCellBuff = null;			// GemDescription
	//public UILabel equippedCellBuffCost = null;		// Cost
	//public UISprite equippedCellIcon = null;		// Icon
	//public Transform equippedCellEquipButton = null;// EquipButton
	//public UISlider equippedCellProgressbar = null;	// None
	//public UILabel equippedCellGroupTitle = null;	// InvTitle

		//BasePower power = PowerStore.Powers[equippedPower];
		//equippedCellTitle.text = data.Title;	//power.Title;
	

//		
//		//if (power == null) { return; }
//		if (equippedCellTitle) { equippedCellTitle.text = power.Title; }
//		if (equippedCellDesc) { equippedCellDesc.text = power.Description; }
//		
//		if (equippedCellBuff) 
//		{
//			NGUITools.SetActive(equippedCellBuff.gameObject, true);
//			equippedCellBuff.text = power.BuffDescription;
//		}
//		
//		if(equippedCellBuffCost) 
//		{
//			equippedCellBuffCost.text = GameProfile.SharedInstance.Player.GetBuffCost(BuffType.Powerup, equippedPower, PowerStore.Powers[equippedPower].ProtoBuff).ToString();
//		}
//		
//		//if(equippedCellProgressbar) 
//		//{
//		//	NGUITools.SetActive(equippedCellProgressbar.gameObject, true);
//		//	equippedCellProgressbar.sliderValue = GameProfile.SharedInstance.Player.GetBuffProgress(BuffType.Powerup, equippedPower, PowerStore.Powers[equippedPower].ProtoBuff);
//		//}
//		
//		if (equippedCellIcon) { equippedCellIcon.spriteName = power.IconName; }
		//if (equippedCellEquipButton) { NGUITools.SetActive(equippedCellEquipButton.gameObject, true); }
		//UpdatePowerups();

	//private int equippedPower = -1;
	//if (equippedPower >= PowerStore.Powers.Count) { return; }

//	private int equippedArtifact = -1;
//
//		if(equippedArtifact != -1) 
//		{
//			if (equippedArtifact >= ArtifactStore.Artifacts.Count) { return; }
//			ArtifactProtoData theArtifact = ArtifactStore.Artifacts[equippedArtifact];
//			if (theArtifact == null) { return; }
//			if (equippedCellTitle) { equippedCellTitle.text = theArtifact._title; }
//			if (equippedCellDesc) { equippedCellDesc.text = theArtifact._description; }
//			if (equippedCellBuff) { NGUITools.SetActive(equippedCellBuff.gameObject, false); } //equippedCellBuff.text = theArtifact._buffDescription;
////			if(equippedCellBuffCost) { equippedCellBuffCost.text = GameProfile.SharedInstance.Player.GetBuffCost(BuffType.Artifact, equippedArtifact).ToString(); }
//			if (equippedCellProgressbar) 
//			{ 
//				NGUITools.SetActive(equippedCellProgressbar.gameObject, false);
//				//equippedCellProgressbar.sliderValue = GameProfile.SharedInstance.Player.GetBuffProgress(BuffType.Artifact, equippedArtifact);
//			}
//			if (equippedCellIcon) { equippedCellIcon.spriteName = theArtifact._iconName; }
//			if (equippedCellEquipButton) { NGUITools.SetActive(equippedCellEquipButton.gameObject, false); }
//			//UpdateArtifacts();
//		}