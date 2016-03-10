using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CharacterCardOz : MonoBehaviour 
{
	protected static Notify notify;
	public enum DisplayItem { Icon, Background, }
	
	public List<Transform> 	ArtifactSlotOne = new List<Transform>();
	public List<Transform> 	ArtifactSlotTwo = new List<Transform>();
	public List<Transform> 	ArtifactSlotThree = new List<Transform>();
	public List<Transform> 	PowerSlot = new List<Transform>();
	public UIGrid			AbilitiesGrid = null;
	public UILabel			DisplayName = null;
	public int				CharacterID = -1;
	
	void Awake() { 
		if (notify != null)
		{
			notify = new Notify(this.GetType().Name);	
		}
	}
	
	public void UpdateUI(CharacterStats characterStat) 
	{
		CharacterID = characterStat.characterId;
		
		//-- Update coins
		if(AbilitiesGrid != null) 
		{
			notify.Debug ("repositioning ablilities Grid for {0}", characterStat.displayName);
			AbilitiesGrid.repositionNow = true;
		}
		
		if(DisplayName != null) {
			DisplayName.text = characterStat.displayName;
		}
				
		//-- Update Wardrobe
		
		//-- Update SpecialPower
		UpdatePowerDisplay(characterStat);
	}
	
	private void UpdatePowerDisplay(CharacterStats characterStat) 
	{
		BasePower power = null;
		
		if (PowerStore.Powers != null && characterStat.powerID >=0 && characterStat.powerID < PowerStore.Powers.Count) 
		{
			power = PowerStore.PowerFromID(characterStat.powerID);	//.Powers[characterStat.powerID];	
			notify.Debug ("UpdatePowerDisplay {0},{1},{2}", characterStat.powerID, PowerStore.Powers.Count, power);
		}
		
		if (PowerSlot[(int)DisplayItem.Background] != null) 
		{
			NGUITools.SetActive(PowerSlot[(int)DisplayItem.Background].gameObject, characterStat.unlocked);
			if(characterStat.unlocked == true) 
			{
				UISprite icon = PowerSlot[(int)DisplayItem.Background].GetComponent<UISprite>() as UISprite;
				if (icon != null) { icon.color = new Color(250.0f/255.0f, 220.0f/255.0f, 125.0f/255.0f); }	
			}
		}
		
		if(PowerSlot[(int)DisplayItem.Icon] != null) 
		{
			NGUITools.SetActive(PowerSlot[(int)DisplayItem.Icon].gameObject, characterStat.unlocked);
			if(characterStat.unlocked == true) 
			{ 
				UISprite icon = PowerSlot[(int)DisplayItem.Icon].GetComponent<UISprite>() as UISprite;
				if (icon != null) 
				{
					if (power != null) { icon.spriteName = power.IconName; }
					else { icon.spriteName = "coin"; }
				}	
			}
		}
	}
}
