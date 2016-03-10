using UnityEngine;
using System.Collections;

public class DistanceMeter : MonoBehaviour 
{
	bool isDistanceMeterOnScreen = false;
	private Vector3 UpPosition = new Vector3(0, 0, 0);
	private Vector3 DownPosition = new Vector3(0, 0, 0);
	
	private Transform distanceMeter;	//public Transform distanceMeter = null;
	private UILabel DistanceLabel;		//public UILabel DistanceLabel = null;
	
	void Start() 
	{ 
		distanceMeter = gameObject.transform;
		DistanceLabel = gameObject.transform.Find("DistanceLabel").GetComponent<UILabel>();
	}
	
	public void ShowDistanceMeterWithDistance(int distance) 
	{
		if (isDistanceMeterOnScreen == true) { return; }	// || DistanceLabel == null || distanceMeter == null)

		UpPosition.y = Screen.height;
		DownPosition.y = 375;//Screen.height * 0.5f;
		
		NGUITools.SetActive(distanceMeter.gameObject, true);
		distanceMeter.localPosition = UpPosition;
		DistanceLabel.text = string.Format("{0}m", distance);
		StartCoroutine(AnimateMessageBoard());
	}
	
	public IEnumerator AnimateMessageBoard()
	{
		AudioManager.SharedInstance.PlayFX(AudioManager.Effects.oz_UI_Menu_back);
		distanceMeter.localPosition = UpPosition;
		isDistanceMeterOnScreen = true;
		TweenPosition.Begin(distanceMeter.gameObject, 0.25f, DownPosition);
		yield return new WaitForSeconds(2);
		
		TweenPosition outTp = TweenPosition.Begin(distanceMeter.gameObject, 0.25f, UpPosition);
		outTp.eventReceiver = this.gameObject;
		outTp.callWhenFinished = "DistanceMeterFinishHideAnimation";
	}
	
	public void DistanceMeterFinishHideAnimation() 
	{
		isDistanceMeterOnScreen = false;
		NGUITools.SetActive(distanceMeter.gameObject, false);
	}		
	
	void Update () { }
}

		//if (distanceMeter == null) { return; }