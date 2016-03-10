using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class Services : MonoBehaviour
{
	private static Dictionary<string, MonoBehaviour> serviceDict = new Dictionary<string, MonoBehaviour>();		// single class-level lookup table for all services
	
	private static Services main;
	
	private static bool _servicesActive = false;
	
	// banderson -- Added a static check to see if the class had been loaded.
	public static bool ServicesActive
	{
		get
		{
			return _servicesActive;
		}
	}
	
	void Awake()
	{
		if (main!=null) 
			Destroy(gameObject);
		else 
			main = this;
		
		_servicesActive = true;

		// Add service MonoBehaviours. Eventually, any script derived from 'Service' should get automatically registered here in a different way using reflection.
		// 							   Might want to also spawn a separate GameObject parented under the 'Services' GO, to attach each service's MonoBehaviour to. 

        Add<Rand>();
        Add<ObjectivesManager>();
        Add<Store>();
        Add<NewsManager>();
        Add<TuningAnalyticsManager>();
		if ( Settings.GetBool("console-enabled", false))
		{
            Add<DebugConsoleOzGui>();
            Add<HUDFPSUnityGui>();
		}
        Add<ProfileManager>();
		//gameObject.AddComponent<ProfileLoader>();
        Add<DownloadManager>();
        Add<DownloadManagerUI>();
        Add<DownloadManagerLocalization>();
        Add<ChallengeDataUpdater>();
        Add<LeaderboardManager>();
        Add<AppCounters>();
        Add<NotificationSystem>();
        Add<MenuTutorials>();
        Add<StorePurchaseHandler>();
        Add<ServerSettings>();
        Add<AmpBundleManager>();
		
        //foreach(MonoBehaviour service in gameObject.transform.GetComponentsInChildren<MonoBehaviour>())
        //{
        //    string name = service.ToString();
        //    serviceDict.Add(name, service);
        //}
	}
	
	void Start() 
	{		
		DontDestroyOnLoad(gameObject);	// keeps services around when changing scenes
	}

    public void Add<T>()
    {
        Type t = typeof (T);
        serviceDict.Add(t.ToString(), (MonoBehaviour) gameObject.AddComponent(t));
    }

    /// <summary>
    /// register whatever monobehaviour as a service
    /// </summary>
    /// <param name="name"></param>
    /// <param name="service"></param>
    /// <returns></returns>
    public static bool Register(string name, MonoBehaviour service)
	{
        //serviceDict["Services(Clone) (" + name + ")"] = service;
        serviceDict[name] = service;
		return true;	
	}

    /// <summary>
    ///  return service MonoBehaviour script instance (component) based on type
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
	public static T Get<T>()			
	{
        //return (T)(object)serviceDict["Services(Clone) (" + typeof(T).ToString() + ")"];
        return (T)(object)serviceDict[typeof(T).ToString()];
	}
}