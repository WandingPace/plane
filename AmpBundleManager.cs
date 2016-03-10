using System;
using System.Collections;
using System.Collections.Generic;
using MiniJSON;
using UnityEngine;

public class AmpBundleManager : MonoBehaviour
{
    public AMPVersionResultData ampAssetBundleData = new AMPVersionResultData();
    //private GameObject bundleVersioncheckMsgObbect = null;	
    private GameObject ampAssetListReuqestMsgObject;
    public bool bundleVersioncheck;
    private bool bundleVersioncheckInProgress;
    protected Notify notify;

    private void Awake()
    {
        notify = new Notify(GetType().Name);
    }

    public void DoBundleVersionCheck()
    {
        if (Settings.GetBool("embed-all-envsets", false) && Settings.GetBool("skip-bundle-check-if-embed-all", true))
            //false))
        {
            notify.Debug("Skipping bundle version check as embed-all-envsets and skip-bundle-check-if-embed-all is true");
        }
        else
        {
            StartCoroutine(StartBundleVersionCheck());
        }
    }

    /// <summary>
    ///     Runs the request. in a coroutine, will call the delegate when done and start the next one if needed
    /// </summary>
    /// <param name='netReq'>
    ///     Netrequest with needed info
    /// </param>
    private IEnumerator StartBundleVersionCheck()
    {
        if (bundleVersioncheck)
        {
            if (bundleVersioncheckInProgress)
            {
                notify.Error("MULTIPLE bundleVersioncheck called");
            }

            bundleVersioncheckInProgress = true;
            bundleVersioncheck = false; // only check once per prompt

            // do the async call and wait for call back

            var url = "http://oz.i.dol.cn/android/oz/0158-0006/assert";

            notify.Error("url:" + url);

            var www = new WWW(url);

            bool noErrors;

            notify.Debug("DoBundleVersionCheck starting yield return www, url=" + url);

            yield return www;


            notify.Debug("DoBundleVersionCheck done with yield return www, fullUrl = " + url);
            noErrors = (www.isDone && www.error == null && www.text != null);

            if (www.error != null)
            {
                notify.Warning("DoBundleVersionCheck  www.error=" + www.error);
                ShowAMPSDownloadError("Msg_DownloadFailLong", "UIManagerOz::StartBundleVersionCheck error=" + www.error);
                noErrors = false;
            }

            if (noErrors)
            {
                notify.Debug("www.text=" + www.text);
                try
                {
                    object result = null;
                    result = Json.Deserialize(www.text);
                    if (result == null)
                    {
                        notify.Warning("deserialization failed " + url);
                        noErrors = false;
                    }
                    else
                    {
                        // now set latest version numbers
                        GetVersionNumbers(www, noErrors, result);
                    }
                }
                catch (Exception e)
                {
                    notify.Warning("deserialization exception " + url + " exception=" + e);
                    ShowAMPSDownloadError("Msg_DownloadFailLong",
                        "UIManagerOz::StartBundleVersionCheck: deserialization exception. url=" + url + " exception=" +
                        e);
                    noErrors = false;
                }
            }

            if (noErrors)
            {
                if (ampAssetListReuqestMsgObject != null)
                {
                    ampAssetListReuqestMsgObject.SendMessage("OnAMPRequestAssetListDone");
                    ampAssetListReuqestMsgObject = null;
                }
            }
        }

        bundleVersioncheckInProgress = false;
    }

    private bool UpdateBundleVersion(string bundleName)
    {
        if (bundleName != "")
        {
            notify.Debug("checking bundle " + bundleName);
            //if( ResourceManager.SharedInstance.IsAssetBundleDownloadedLastestVersion(bundleName))
            //{
            var info = ResourceManager.SharedInstance.downloadedAssetBundles.FindInfo(bundleName);
            if (info != null)
            {
                var v = ampAssetBundleData.GetAssetVersion(bundleName);
                notify.Debug("ver = " + v);
                return ResourceManager.SharedInstance.downloadedAssetBundles.SetNewVersionAvaiable(bundleName, v, true);
            }
            //}
        }
        return false;
    }

    // important //
    private bool GetVersionNumbers(WWW www, bool noErrors, object results)
    {
        //return true; // temp getting a crash on ios
        notify.Debug("GetVersionNumbers noErrors=" + noErrors + " www.error=" + www.error);
        var result = false;
        if (noErrors)
        {
            if (results == null)
            {
                notify.Warning("No results! Must not be connected...");
//				return false;
            }

            try
            {
                var rootDict = results as Dictionary<string, object>;
//				object query = rootDict["querySuccess"];

                var resultData = rootDict["result"] as Dictionary<string, object>;

                if (resultData != null)
                {
                    var bItems = resultData["data"] as List<object>;
                    notify.Debug("Parsing list len = " + bItems.Count);
                    if (bItems.Count > 0)
                    {
                        var bundleList = new AMPBundleData[bItems.Count];
                        ampAssetBundleData.bundleList = bundleList;

                        for (var i = 0; i < bItems.Count; i++)
                        {
                            var bd = bItems[i] as Dictionary<string, object>;
                            result = true;

                            var adata = new AMPBundleData();
                            adata.fileName = bd["fileName"] as string;
                            adata.assetVersion = bd["assetVersion"] as string;
                            adata.status = bd["status"] as string;
                            adata.url = bd["url"] as string;
                            notify.Debug("----- " + adata.fileName + " v= " + adata.GetAssetVersion() + " state = " +
                                         adata.status + " url= " + adata.url);

                            bundleList[i] = adata;
                        }

                        // now go through all bundles and set their version number

                        // CHECK localization bundle version
                        UpdateBundleVersion(
                            Localization.SharedInstance.GetAssetBundleName(Localization.SharedInstance.GetLangBySystem()));
                        // check hires 
                        UpdateBundleVersion(DownloadManagerUI.bundleName);
                        // env
                        foreach (var kvp in EnvironmentSetManager.SharedInstance.AllDict)
                        {
                            if (!kvp.Value.IsEmbeddedAsResource())
                                // if not embedded resource / asset bundle, update version info
                            {
                                UpdateBundleVersion(EnvironmentSetManager.SharedInstance.GetAssetBundleName(kvp.Key));
                            }
                        }
                        //UpdateBundleVersion( EnvironmentSetManager.SharedInstance.GetAssetBundleName(EnvironmentSetManager.WhimsyWoodsId));
                        //UpdateBundleVersion( EnvironmentSetManager.SharedInstance.GetAssetBundleName(EnvironmentSetManager.DarkForestId ));
                        //UpdateBundleVersion( EnvironmentSetManager.SharedInstance.GetAssetBundleName(EnvironmentSetManager.YellowBrickRoadId ));
                    }
                    else
                    {
                        notify.Warning("empty Data!!!!!!!!!");
                    }
                }
                else
                {
                    notify.Warning("empty Data!!!!!!!!!");
                }
            }

            catch (Exception theException)
            {
                notify.Warning("AssetList www.text= " + www.text + " exception " + theException.Message);
            }
        }

        // No news were loaded for some reason.  Display a random GameTip.
        if (!result)
        {
            var tipString = "        ";
            tipString += GameTipManager.GetRandomGameTip().tip;
            tipString += "        ";

            if (UIManagerOz.SharedInstance.idolMenuVC != null)
            {
                //HorizontalScrollingLabel horizLabel = UIManagerOz.SharedInstance.idolMenuVC.gameObject.GetComponent<HorizontalScrollingLabel>();
                var horizLabel = UIManagerOz.SharedInstance.idolMenuVC.NewsFeed.GetComponent<HorizontalScrollingLabel>();
                if (horizLabel != null)
                {
                    horizLabel.FullString = tipString;
                }
                else
                {
                    notify.Warning("couldn't find horizontal scrolling label in idolMenuVC");
                }
            }
        }

        return result;
    }

    public void RequestAMPAssetList(GameObject obj)
    {
        ampAssetListReuqestMsgObject = obj;
            // this is a blocker path, only be called when the bundle is not loaded before
        if (bundleVersioncheckInProgress == false)
        {
            bundleVersioncheck = true;
            DoBundleVersionCheck();
        }
    }

    public void ShowAMPSDownloadError(string localizedMessageKey, string debugMessage)
    {
        // Display an error to the user...
        if (UIManagerOz.SharedInstance.okayDialog != null)
        {
            UIManagerOz.SharedInstance.okayDialog.ShowOkayDialog(localizedMessageKey, "Btn_Ok");
        }

        // ...and then send the debug message to Crittercism...
        var exception = new Exception(debugMessage);
        exception.Source = "(source)";


        // ...and finally, record the debug message in the log file.
        Debug.Log("*** ERROR DOWNLOADING ASSET FROM AMPS: " + exception.Message + "***");
    }
}

public class AMPVersionData
{
    public AMPVersionResponse response;
    public AMPVersionResultData result;
}

public class AMPVersionResponse
{
    public string message;
    public string status;
    public string timestamp;
    public string url;
}

public class AMPBundleData
{
    public string assetVersion = "";
    public string fileName = "";
    public string status = "";
    public string url = "";

    public int GetAssetVersion()
    {
        var vno = -1;

        try
        {
            vno = (int) (float.Parse(assetVersion));
        }

        catch (Exception theException)
        {
            UIManagerOz.notify.Warning("Asset version number parse error [" + assetVersion + "] exception " +
                                       theException.Message);
        }


        return vno;
    }

    public string GetCdnUrl()
    {
        return url;
    }
}

public class AMPVersionResultData
{
    public AMPBundleData[] bundleList;

    public bool IsReady()
    {
        return bundleList != null;
    }

    /// <summary>
    ///     Gets the most recent asset bundle version.
    /// </summary>
    /// <returns>
    ///     The asset version.
    /// </returns>
    /// <param name='bname'>
    ///     Bname.
    /// </param>
    public int GetAssetVersion(string bname)
    {
        if (bundleList == null)
            return -1;

        var latestVersion = -1;
        var fullname = AssetBundleLoader.GetFullAssetBundleName(bname);

        foreach (var ad in bundleList)
        {
            if (ad != null && ad.fileName == fullname)
            {
                var version = ad.GetAssetVersion();

                if (version > latestVersion)
                {
                    latestVersion = version;
                }

                //return ad.GetAssetVersion(); 	
            }
        }

        return latestVersion; //-1;
    }

    /// <summary>
    ///     Gets the cdn URL for most recent asset bundle version.
    /// </summary>
    /// <returns>
    ///     The cdn URL.
    /// </returns>
    /// <param name='basename'>
    ///     Basename.
    /// </param>
    public string GetCdnUrl(string basename)
    {
        if (bundleList == null)
            return "";

        var latestVersion = -1;
        AMPBundleData latestABD = null;
        var fullname = AssetBundleLoader.GetFullAssetBundleName(basename);

        foreach (var ad in bundleList)
        {
            if (ad != null && ad.fileName == fullname)
            {
                var version = ad.GetAssetVersion();

                if (version > latestVersion)
                {
                    latestABD = ad;
                    latestVersion = version;
                }

                //return ad.GetCdnUrl();
            }
        }

        if (latestABD != null)
        {
            return latestABD.GetCdnUrl();
        }
        return "";
    }
}


/*	
	bool GetVersionNumbers1( WWW www, bool noErrors, object results)
	{
		//return true; // temp getting a crash on ios
		notify.Debug("GetVersionNumbers noErrors=" + noErrors + " www.error=" + www.error);
		bool result = false;
		if (noErrors)
		{
			if(results==null)
			{
				notify.Warning("No results! Must not be connected...");
//				return false;
			}
			
			try
			{	AMPVersionData vData = new AMPVersionData();
				
				string tt = "{ \"response\": { \"status\": \"OK\", \"message\": \"\", \"timestamp\": \"Wed Feb 13 15:43:41 PST 2013\", \"url\": \"/amps/api/listassets/troz_test?deviceModel=iphone\" } }";
				SerializationUtils.FromJson(vData, tt);
				foreach(AMPBundleData adata in vData.result.bundleList)
				{
					notify.Debug("----- " + adata.fileName + " v= " + adata.GetAssetVersion() + " state = " + adata.status + " url= " + adata.url);
				}

			}
			catch (System.Exception theException)
			{
				notify.Warning("GotTheNews www.text= " + www.text + " exception " + theException.Message);
			}
		}
		
		// No news were loaded for some reason.  Display a random GameTip.
		if (!result)
		{			
			string tipString = "        ";
			tipString += GameTipManager.GetRandomGameTip().tip;
			tipString += "        ";
			
			if (UIManagerOz.SharedInstance.idolMenuVC != null)
			{
				//HorizontalScrollingLabel horizLabel = UIManagerOz.SharedInstance.idolMenuVC.gameObject.GetComponent<HorizontalScrollingLabel>();
				HorizontalScrollingLabel horizLabel = UIManagerOz.SharedInstance.idolMenuVC.NewsFeed.GetComponent<HorizontalScrollingLabel>();
				if (horizLabel != null)
				{
					horizLabel.FullString = tipString;
				}
				else
				{
					notify.Warning("couldn't find horizontal scrolling label in idolMenuVC");	
				}
			}
		}
		
		return result;	
	}	
*/