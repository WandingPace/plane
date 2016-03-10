using UnityEngine;
using System;
using System.Collections;

public class TimeLeftCountdown : MonoBehaviour 
{

	private DateTime endDateTime = DateTime.MinValue;
    private DateTime allEndDateTime = DateTime.MinValue;
	private int previousSeconds = -1;

    public static TimeLeftCountdown instance;
    [HideInInspector]
    public string fuelCountDownStr;
    [HideInInspector]
    public string fuelAllCountDownStr;
    void Awake()
    {
        if(instance ==null)
            instance = this;
    }
	
	void OnDisable()
    {
        instance = null;
    }

    private bool countDownFirst = true;
	void Update()
	{
        if(UIManagerOz.SharedInstance.PaperVC == null)
            return;

        if(UIManagerOz.SharedInstance.PaperVC.fuelSystem.IsFuelFull())
        {
            if(!countDownFirst)
            {
                countDownFirst = true;
                fuelCountDownStr = "";
                fuelAllCountDownStr = "";
                UIManagerOz.SharedInstance.PaperVC.fuelCountDown.transform.parent.gameObject.SetActive(false);
            }
            if(UIManagerOz.SharedInstance.PaperVC.fuelCountDown.transform.parent.gameObject.activeSelf)
            {
                UIManagerOz.SharedInstance.PaperVC.fuelCountDown.transform.parent.gameObject.SetActive(false);
            }
            if(endDateTime != DateTime.MinValue)
            {
                endDateTime = DateTime.MinValue;
                allEndDateTime = DateTime.MinValue;
            }

            return;
        }
        else if(!UIManagerOz.SharedInstance.PaperVC.fuelCountDown.transform.parent.gameObject.activeSelf)
        {
            UIManagerOz.SharedInstance.PaperVC.fuelCountDown.transform.parent.gameObject.SetActive(true);
        }

		if(previousSeconds != DateTime.Now.Second)
		{
			TimeSpan span = endDateTime - DateTime.Now;
			if(span <= TimeSpan.Zero)
			{

                if(!countDownFirst)
                {
                    UIManagerOz.SharedInstance.PaperVC.fuelSystem.AddFuelCount(1);
                    UIManagerOz.SharedInstance.PaperVC.UpdateCurrency();
                }
                else
                {
                    countDownFirst = false;

                }

                endDateTime = DateTime.Now.AddSeconds(UIManagerOz.SharedInstance.PaperVC.fuelSystem.GetPerFuelRecoverTime());
                int val = (UIManagerOz.SharedInstance.PaperVC.fuelSystem.GetFuelUpLimit()-GameProfile.SharedInstance.Player.fuelCount)*
                    UIManagerOz.SharedInstance.PaperVC.fuelSystem.GetPerFuelRecoverTime();
                allEndDateTime = DateTime.Now.AddSeconds(val);
			}
          
            fuelCountDownStr = FormatMinTimeSpan(endDateTime - DateTime.Now);
			previousSeconds = DateTime.Now.Second;
            fuelAllCountDownStr = FormatHourTimeSpan(allEndDateTime - DateTime.Now);
			return;
		}
	
	}
	
    public void SetAllEndDateTime()
    {
        if(UIManagerOz.SharedInstance.PaperVC==null)
            return;
        int val = (UIManagerOz.SharedInstance.PaperVC.fuelSystem.GetFuelUpLimit()-GameProfile.SharedInstance.Player.fuelCount)*
            UIManagerOz.SharedInstance.PaperVC.fuelSystem.GetPerFuelRecoverTime();

        if(endDateTime>=DateTime.Now)
            allEndDateTime =DateTime.Now.AddSeconds( val + (endDateTime-DateTime.Now).TotalSeconds);
    }
    
    public void SetExpirationDateTime(DateTime endDT)
	{
		endDateTime = endDT;
	}
	
    private string FormatMinTimeSpan(TimeSpan span)
    {
        if (span <= TimeSpan.Zero)
            return "00:00";
        else
            return span.Minutes.ToString("00") + ":" + 
                span.Seconds.ToString("00");
    }    

    private string FormatHourTimeSpan(TimeSpan span)
    {
        if (span <= TimeSpan.Zero)
            return "00:00:00:00";
        else
            return 
                span.Hours.ToString("00") + "小时" + 
                span.Minutes.ToString("00") + "分钟" + 
                span.Seconds.ToString("00")+"秒";
    }    

   	private string FormatTimeSpan(TimeSpan span)
   	{
		if (span <= TimeSpan.Zero)
			return "00:00:00:00";
		else
		  	return span.Days.ToString("00") + ":" + 
				span.Hours.ToString("00") + ":" + 
				span.Minutes.ToString("00") + ":" + 
				span.Seconds.ToString("00");
   }	
}