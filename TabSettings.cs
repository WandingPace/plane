using UnityEngine;
using System.Collections;

public class TabSettings
{
	public Vector3 backgroundScale;
	public Vector3 frameScale;	
	public Vector3 highlightPosition;	
	public Vector3 spriteScale;
	public Vector3 notificationIconPosition;	
	public Color textColor;
	
	public static float tabScaleMultiplier = 1.4f;
	
	public TabSettings(GameObject tabGO)
   	{
		backgroundScale = new Vector3(tabGO.transform.Find("Background").localScale.x, tabGO.transform.Find("Background").localScale.y, 1.0f);
		frameScale = new Vector3(tabGO.transform.Find("Frame").localScale.x, tabGO.transform.Find("Frame").localScale.y, 1.0f);
		highlightPosition = new Vector3(0.0f, tabGO.transform.Find("Highlight").localPosition.y, tabGO.transform.Find("Highlight").localPosition.z);	
		spriteScale = tabGO.transform.Find("Sprite").localScale;
		notificationIconPosition = tabGO.transform.Find("NotificationIcon").transform.localPosition;		
		textColor = tabGO.transform.Find("LabelTab").GetComponent<UILabel>().color;
   	}

	public void ScaleTab(GameObject tabGO, TabSettings targetTabSettings, float tweenTime, bool bigger, Vector3 actualSpriteScale)
    {  
		iTween.ScaleTo(tabGO.transform.Find("Checkmark").gameObject, iTween.Hash(
			"scale", targetTabSettings.backgroundScale,
			"islocal", true,
			"time", tweenTime,
			"easetype", iTween.EaseType.easeInOutSine));		
		
		tabGO.transform.Find("Checkmark").gameObject.GetComponent<UISprite>().enabled = bigger;		
      
		iTween.ScaleTo(tabGO.transform.Find("Background").gameObject, iTween.Hash(
			"scale", targetTabSettings.backgroundScale,
			"islocal", true,
			"time", tweenTime,
			"easetype", iTween.EaseType.easeInOutSine));				

		iTween.ScaleTo(tabGO.transform.Find("Frame").gameObject, iTween.Hash(
			"scale", targetTabSettings.frameScale,
			"islocal", true,
			"time", tweenTime,
			"easetype", iTween.EaseType.easeInOutSine));	
		
		iTween.MoveTo(tabGO.transform.Find("Highlight").gameObject, iTween.Hash(
			"position", targetTabSettings.highlightPosition,
			"islocal", true,
			"time", tweenTime,
			"easetype", iTween.EaseType.easeInOutSine));		
		
		//Vector3 scale = (bigger) ? 1.4f * targetTabSettings.spriteScale : targetTabSettings.spriteScale;
		Vector3 scale = (bigger) ? tabScaleMultiplier * actualSpriteScale : actualSpriteScale;
		
		iTween.ScaleTo(tabGO.transform.Find("Sprite").gameObject, iTween.Hash(
			"scale", scale,	//targetTabSettings.spriteScale,
			"islocal", true,
			"time", tweenTime,
			"easetype", iTween.EaseType.easeInOutSine));
		
		tabGO.GetComponent<BoxCollider>().enabled = !bigger;	// disable box collider if tab is big (currently selected one), enabled otherwise
		
		iTween.MoveTo(tabGO.transform.Find("NotificationIcon").gameObject, iTween.Hash(
			"position", targetTabSettings.notificationIconPosition,
			"islocal", true,
			"time", tweenTime,
			"easetype", iTween.EaseType.easeInOutSine));	
		
		tabGO.transform.Find("LabelTab").GetComponent<UILabel>().color = targetTabSettings.textColor;
	}
}