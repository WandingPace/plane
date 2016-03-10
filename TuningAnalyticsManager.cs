using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TuningAnalyticsManager : MonoBehaviour
{
	protected static Notify notify;
	
	void Awake ()
	{
		notify = new Notify(this.GetType().Name);
	}
	
	/// <summary>
	/// Sends the tuning analytics.
	/// </summary>
	/// <param name='statAnalyticString'>
	/// Stat analytic string.
	/// </param>
	public void SendTuningAnalytics(string statAnalyticString)
	{
#if !UNITY_EDITOR
		WWWForm analyticForm = new WWWForm();
		analyticForm.AddField("data",statAnalyticString);
		
		NetRequest analyticReq = new NetRequest("/analytics",analyticForm, SentTuningAnalytics);
		NetAgent.Submit(analyticReq);
#endif
	}
	
	public bool SentTuningAnalytics(WWW www, bool noErrors, object results)
	{
		return true;	
	}

}

