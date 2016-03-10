using UnityEngine;
using System;
using System.Collections;

/// <summary>
/// Scrolls the letters in a UILabel over time, similar to a news ticker
/// </summary> 
public class HorizontalScrollingLabel : MonoBehaviour
{
	// hook this up in the scenes
	//public UILabel Label;
	public UILabel LabelSysFont;
	int realMaxCharsToShow = 40;
	int MaxCharsToShow = 40;
	public float scrollWaitTime; // how may seconds to wait before scrolling one characters
	public float initialPause;
	
	public float startPosition = 800.0f;
	public float endPosition = -800.0f;
	
	float curTime = -1.0f;
	
	
	public string _Test = "";
	
	private int curIndex;
	const int stopIndex = 0; // if curIndex reaches this, reset to 0
	private string shownString; // the only visible part of the string being shown
	
	float oldy;
	float oldz;
	
	float TotalTime = 0.0f;
	float speed = 150.0f;
	
	float realaverageCharWidth = 24.0f;
	float averageCharWidth = 24.0f;
	
	void Awake()
	{
		oldy = transform.localPosition.y;
		oldz = transform.localPosition.z;
		
	}
	
	void SyncFontSize()
	{
		realMaxCharsToShow = MaxCharsToShow;
		realaverageCharWidth = averageCharWidth;
		string lang = Localization.SharedInstance.GetLangBySystem();
//			Debug.LogError("Scroll language = " + lang);
		
		if( lang == "Chinese_Traditional" ||
			lang == "Chinese_Simplified" ||
			lang == "Korean"  )
		{
			realMaxCharsToShow = (int)(realMaxCharsToShow * 0.55f);
			realaverageCharWidth /= 0.5f;
		}
		
	}
	
	
	private string fullString = "";	
	public string FullString 
	{ 
		get { return fullString; }
		set
		{
//			if( UIManagerOz.SharedInstance._Test == "" )
				fullString = value.Trim();
//			else
//				fullString = UIManagerOz.SharedInstance._Test;
			StartScrollIfNeeded();
		}
	}

	void OnEnable()
	{
		StartScrollIfNeeded();
	}

	public void StartScrollIfNeeded()
	{
		SyncFontSize();
		
		LabelSysFont.text = fullString;
//		Debug.LogError("News [" + fullString + "] len = " + fullString.Length);
		startPosition = fullString.Length * realaverageCharWidth / 2.0f + 300.0f;
		endPosition = -startPosition;
		
		TotalTime = (startPosition - endPosition )/ speed;
		
//		Debug.LogError("WIDTH = " + LabelSysFont.relativeSize.x );
		
		if (fullString.Length > realMaxCharsToShow)
		{
			curTime = 0.0f;
		}
		else
		{
			transform.localPosition = new Vector3( 0.0f, oldy, oldz);
		}
	}
	
	void Update()
	{
		if( curTime >= 0.0f )
		{
			curTime += Time.deltaTime;
			if( curTime >= TotalTime )
			{
				curTime = 0.0f;
			}
			transform.localPosition = new Vector3( Mathf.Lerp(startPosition, endPosition, curTime/TotalTime), oldy, oldz);
		}
	}
	
/*	
	/// <summary>
	/// starts the text scrolling if needed
	/// </summary> 
	public void StartScrollIfNeeded()
	{
		CancelInvoke("StartScrollIfNeeded");
		
		if (fullString != null)
		{
			if (fullString.Length > MaxCharsToShow)
			{
				stopIndex = fullString.Length - 1;
				curIndex = 0;	//-MaxCharsToShow;
				shownString = fullString.Substring(0, MaxCharsToShow);
				LabelSysFont.Text = shownString;	//Label.text = shownString;
				Invoke("ScrollOneChar", initialPause);
			}
			else
				LabelSysFont.Text = fullString;	//Label.text = fullString; 	// fix for news text string being too short for scroller, just show the whole thing.
		}
	}
*/	
	/// <summary>
	/// Continuously scrolls one char,
	/// </summary> 
	public void ScrollOneChar()
	{
		CancelInvoke("ScrollOneChar");
		
		curIndex++;
		
		if (curIndex == stopIndex)
			curIndex = 0;	
		
//		if (curIndex >= 0)
//		{
			int subLength = Math.Min(MaxCharsToShow, stopIndex - curIndex);
			shownString = fullString.Substring(curIndex, subLength);
//		}
//		else
//		{
//			shownString = "";
//			
//			for (int i=0; i < -curIndex; i++)
//				shownString += " ";	
//			
//			int subLength = (int)Mathf.Min(MaxCharsToShow + curIndex, stopIndex);
//			shownString += fullString.Substring(0, subLength);
//		}
		
		LabelSysFont.text = shownString;	//Label.text = shownString;
		
		if (curIndex == 0)
			Invoke("ScrollOneChar", initialPause);	
		else
			Invoke("ScrollOneChar", scrollWaitTime);	// Settings.GetFloat("scroll-wait-time",0.5f));
	}
}



	
//	public void SetScrollText()
//	{
//		shownString = fullString.Substring(0, MaxCharsToShow);
//		Label.text = shownString;
//	}	

		//get { return fullString; }
		//set { fullString = value; }
	
