using UnityEngine;
using System.Collections;

public class HeadStart : MonoBehaviour 
{
	//protected static Notify notify;
	public bool isMegaHeadStart = false;
	
	//private UIWidget[] headStartWidgets;
	public UILabel headStartCostLabel;
	public Transform headStartRoot;

	//private Vector3 startingHeadStartPosition = Vector3.zero;	
	
	//private bool firstUpdate = true;
	//private bool blinking = false;
	
	/*void Awake()
	{
		if ( notify != null) 
		{
			notify = new Notify(this.GetType().Name);	
		}
	}
	
	void Start() 
	{
		NGUITools.SetActive(headStartRoot.gameObject, false);	// hide icon
		//startingHeadStartPosition = headStartRoot.localPosition;
	}
			
	public void appear()
	{
	//	firstUpdate = true;
	//	blinking = false;	
		
		if (isMegaHeadStart) { headStartCostLabel.text = GameProfile.SharedInstance.Player.GetHeadStartCost().ToString(); }
		else { headStartCostLabel.text = GameProfile.SharedInstance.Player.GetMegaHeadStartCost().ToString(); }
		
		NGUITools.SetActive(headStartRoot.gameObject, false);		// disable for now
	}
	
	public void OnMegaHeadStart() 
	{ 
		notify.Debug ("MEGAHEADSTART");
		//-- Charge the player
		GameProfile.SharedInstance.Player.coinCount -= GameProfile.SharedInstance.Player.GetMegaHeadStartCost();
		
		GamePlayer.SharedInstance.StartBoost();
		GamePlayer.SharedInstance.BoostDistanceLeft = 2500.0f;
		GameController.SharedInstance.HeadStartsThisRun++;
		
		NGUITools.SetActive(headStartRoot.gameObject, false);
	}
	
	public void OnHeadStart() 
	{
		notify.Debug ("HEADSTART");
		
		//-- Charge the player
		GameProfile.SharedInstance.Player.coinCount -= GameProfile.SharedInstance.Player.GetHeadStartCost();
		GamePlayer.SharedInstance.StartBoost();
		GamePlayer.SharedInstance.BoostDistanceLeft = 1000.0f;
		GameController.SharedInstance.HeadStartsThisRun++;
		
		NGUITools.SetActive(headStartRoot.gameObject, false);
	}
	
	private void TurnOffHeadStartBlink() 
	{	
//		int max = headStartWidgets.Length;
//		
//		for (int i = 0; i < max; i++) 
//		{
//			UIWidget widget = headStartWidgets[i];
//			if (widget == null) { continue; }
//			
//			TweenColor tc = widget.GetComponent<TweenColor>();
//			if (tc != null) { tc.enabled = false; }
//			widget.color = Color.white;
//		}
	}*/
}



//		int max = megaHeadStartWidgets.Length;
//		for (int i = 0; i < max; i++) 
//		{
//			UIWidget widget = megaHeadStartWidgets[i];
//			if (widget == null) { continue; }
//			
//			TweenColor tc = widget.GetComponent<TweenColor>();
//			if (tc != null) { tc.enabled = false; }
//			widget.color = Color.white;
//		}

		//NGUITools.SetActive(megaHeadStartRoot.gameObject, false);		
		//NGUITools.SetActive(megaHeadStartRoot.gameObject, false);


		//NGUITools.SetActive(megaHeadStartRoot.gameObject, false);
		
		
		//NGUITools.SetActive(headStartRoot.gameObject, false);
		//NGUITools.SetActive(megaHeadStartRoot.gameObject, false);

		
		//NGUITools.SetActive(headStartRoot.gameObject, false);
		//NGUITools.SetActive(megaHeadStartRoot.gameObject, false);
		//TurnOffHeadStartBlink();
		

// 	private UIWidget[] megaHeadStartWidgets;
// 	public UILabel megaheadStartCostLabel;
// 	public Transform megaHeadStartRoot;	







		//-adding a comment so we can re push this script and make sure it is up to date
		//-- Headstart show/hide
//		if (GameController.SharedInstance.IsIntroScene == true || GameController.SharedInstance.TimeSinceGameStart > 10.0f || GameController.SharedInstance.IsInCountdown == true)
//		{
//			NGUITools.SetActive(headStartRoot.gameObject, false);
//			NGUITools.SetActive(megaHeadStartRoot.gameObject, false);
//			return;
//		}
//			
//		if (firstUpdate == true)
//		{
//			//-- If we have enough money for headstart and megaheadstart, show and blink them.
//			firstUpdate = false;
//			blinking = true;
//			if (GamePlayer.SharedInstance.HasBoost == false) 
//			{
//				blinking = false;
//				bool canAffordHeadStart = GameProfile.SharedInstance.Player.CanAffordHeadStart(false);
//				bool canAffordMegaHeadStart = GameProfile.SharedInstance.Player.CanAffordHeadStart(true);
//				
//				if (headStartRoot != null) 
//				{
//					NGUITools.SetActive(headStartRoot.gameObject, canAffordHeadStart);
//					if (canAffordMegaHeadStart) { headStartRoot.localPosition = startingHeadStartPosition; }
//					else { headStartRoot.localPosition = new Vector3(0, startingHeadStartPosition.y, startingHeadStartPosition.z); }
//				}
//				if (megaHeadStartRoot != null) { NGUITools.SetActive(megaHeadStartRoot.gameObject, canAffordMegaHeadStart); }
//				TurnOffHeadStartBlink();	
//			}
//			
//		}
//		else if (GameController.SharedInstance.TimeSinceGameStart > 6.0f && blinking == false) 
//		{
//			blinking = true;
//			if (NGUITools.GetActive(headStartRoot.gameObject)) 
//			{
//				if (headStartWidgets == null) { headStartWidgets = headStartRoot.GetComponentsInChildren<UIWidget>(); }
//				if (headStartWidgets != null) 
//				{
//					int max = headStartWidgets.Length;
//					for (int i = 0; i < max; i++) 
//					{
//						UIWidget widget = headStartWidgets[i];
//						if (widget == null) { continue; }
//						TweenColor tc = TweenColor.Begin(widget.gameObject, 0.25f, new Color(255,255,255,0));
//						if (tc != null) { tc.style = UITweener.Style.PingPong; }
//					}
//				}
//			}
//			
//			if (NGUITools.GetActive(megaHeadStartRoot.gameObject)) 
//			{
//				if (megaHeadStartWidgets == null) { megaHeadStartWidgets = megaHeadStartRoot.GetComponentsInChildren<UIWidget>(); }
//				if (megaHeadStartWidgets != null) 
//				{
//					int max = megaHeadStartWidgets.Length;
//					for (int i = 0; i < max; i++) 
//					{
//						UIWidget widget = megaHeadStartWidgets[i];
//						if (widget == null) { continue; }
//						TweenColor tc = TweenColor.Begin(widget.gameObject, 0.25f, new Color(255,255,255,0));
//						if (tc != null) { tc.style = UITweener.Style.PingPong; }
//					}
//				}
//			}
//		}
//	}
//}
