using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class ScaleModalDialog : MonoBehaviour
{
	public List<GameObject> buttons = new List<GameObject>();
	public List<GameObject> topCorners = new List<GameObject>();
	public List<GameObject> bottomCorners = new List<GameObject>();
	
	public UISprite background;
	public UILabel description;
	
	
    //private float textSize = 0f;		// equals number of lines of text
	
	public void SetScale(float scale = -1f)
	{
	
//		textSize = description.height;
//        Debug.LogError(textSize);
//		if (scale != -1f)	// manual override for textSize parameter, corresponds to number of lines
//		{
//			textSize = scale;
//		}
//		
////		background.transform.localScale = new Vector3(500.0f, ((textSize - 1.0f) * 50.0f) + 200.0f, 1.0f);
//		
//		description.transform.localPosition = new Vector3(description.transform.localPosition.x, 
//			((textSize - 1.0f) * 25.0f) + 50.0f, description.transform.localPosition.z);
//		
//	
//		
//		foreach (GameObject button in buttons)
//			button.transform.localPosition = new Vector3(button.transform.localPosition.x,
//				((textSize - 1.0f) * -25.0f) - 35.0f, button.transform.localPosition.z);		
	}
}


