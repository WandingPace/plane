using UnityEngine;
using System.Collections;

public class UIIngameOz : MonoBehaviour {
	
	public GameObject ButtonNotReady;
	public GameObject ButtonReady;
	public GameObject LabelActivate;
	
	public static UIIngameOz SharedInstance;
	// Use this for initialization
	void Start () 
	{
		SharedInstance = this;
		DisableActivateButton();
	}
	
	
	// Update is called once per frame
	// DAVID please take away this constant polling once we have time
	void Update () {
		//if(GamePlayer.SharedInstance.CoinCountForBonus >= GamePlayer.SharedInstance.CoinCountForBonusThreshold)
		//{
		//	EnableActivateButton();
		//}
		//else
		//{
		//	DisableActivateButton();
		//}
	}
	
	public void DisableActivateButton()
	{
		ButtonNotReady.SetActive(true);
		ButtonReady.SetActive(false);
		LabelActivate.SetActive(false);
	}
	
	public void EnableActivateButton()
	{
		ButtonNotReady.SetActive(false);
		ButtonReady.SetActive(true);
		LabelActivate.SetActive(true);
	}
}
