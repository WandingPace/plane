using UnityEngine;
using System.Collections;
using System.IO;
using System.Collections.Generic;

/// <summary>
/// Loads an asset bundle, either cached locally or from the AMPS server
/// </summary>
public class AssetBundleLoader 
{
	protected static Notify notify;
	private static string platformLowerCase;
	private static string deviceModel;
	private static string ampsServer;
	private static string ampsAppId;
	private static string altBundleUrl;
	
	public const string DefaultAmpsServer = "http://amps.tapulous.com";

    /// <summary>
    /// 字典标识assetbundle是否被压缩
    /// </summary>
    private static Dictionary<string, bool> compressedInfo = new Dictionary<string, bool>()
    {
        {"icesheet", false},
        {"desert", false},
        {"yellowbrickroad", false},
        {"emeraldcity", false},
        {"hiresresources", true},
        {"localchinesetraditional", true},
        {"localchinesesimplified", true},
        {"localkorean", true},
        {"localrussian", true},
    };
	
	static AssetBundleLoader()
	{
		notify = new Notify("AssetBundleLoader");
#if UNITY_IPHONE
		platformLowerCase = "iphone";
		deviceModel = "iPhone";
#elif UNITY_ANDROID
		platformLowerCase = "android";
		deviceModel = "android";
#elif !UNITY_EDITOR
		notify.Error("Unsupported platform");
#endif
		ampsServer = Settings.GetString("amps-server", DefaultAmpsServer );
		ampsAppId = Settings.GetString("amps-app-id", "troz");
		altBundleUrl = Settings.GetString("alt-bundle-url", "https://www.dropbox.com/sh/ssg5rh1odg61hpx/n63gmzLbDK");
	}
	
	/// <summary>
	/// the asset bundle name without decorations, e.g. darkforest, yellowbrickroad
	/// </summary>
	private string assetBundleBaseName;
	
	/// <summary>
	/// is the asset on a server or in the local StreamingAssets folder
	/// </summary>
	private bool isEmbedded;
	
	/// <summary>
	/// If the cache is lower than this number, download it again
	/// </summary>
	private int assetBundleVersion;
	
	/// <summary>
	/// An override to so that we don't get old files from the amps server
	/// </summary>
	private string cdnUrl;
	
	/// <summary>
	/// Our www object, useful for returning progress and 
	/// </summary>
	private WWW www;
	
	/// <summary>
	/// This will be true when we have either called our success or failure callback
	/// </summary>
	private bool finished;
	
	/// <summary>
	/// This will be each set to true when we have either called our success or failure callback
	/// </summary>	
	private bool successCallbackCalled = false;
	private bool failureCallbackCalled = false;
	
	public delegate bool LoadSuccess(string assetBundleName, int assetBundleVersion, bool downloadOnly, AssetBundle loadedBundle);
	public delegate void LoadFailure( string assetBundleName, int asssetBundleVersion, string errorMsg);
	
	LoadSuccess successCallback;
	LoadFailure failureCallback;
	
	/// <summary>
	/// Returns true if we know what to do with this assetbundle, meaning there's an entry in the compressedInfo dictonary
	/// </summary>
	/// <returns>
	/// <c>true</c> if we know what to do with this assetbundle, meaning there's an entry in the compressedInfo dictonary; otherwise, <c>false</c>.
	/// </returns>
	/// <param name='assetBundleBaseName'>
	/// the asset bundle base name,  e.g. darkforest, yellowbrickroad, hiresresoures
	/// </param>
	public static bool IsRegistered(string assetBundleBaseName)
	{
		bool result = compressedInfo.ContainsKey(assetBundleBaseName);
		return result;
	}
	
	/// <summary>
	/// Determines whether the asset bundle is compressed, you should have run IsRegistered first
	/// </summary>
	/// <returns>
	/// <c>true</c> if this instance is compressed the specified assetBundleBaseName; otherwise, <c>false</c>.
	/// </returns>
	/// <param name='assetBundleBaseName'>
	/// If set to <c>true</c> asset bundle base name.
	/// </param>
	public static bool IsCompressed( string assetBundleBaseName)
	{
		// I am deliberately accessing the dictionary directly. 
		// and fail badly as that means we didn't do the IsRegistered check
		return compressedInfo[assetBundleBaseName];
	}
	
	/// <summary>
	/// returns the fully qualified name of the assetbundle. e.g passing darkforest returns darkforest_iphone_0_7.assetbundle
	/// </summary>
	/// <param name='assetBundleName'>
	/// Asset bundle name, e.g. whimsywoods, darkforest, yellowbrickroad
	/// </param>
	public static string GetFullAssetBundleName(string assetBundleName)
	{
		//string result = assetBundleName + "_" + platformLowerCase + "_" + ResourceManager.GetUndottedVersionString() + ".assetbundle";
		string result = assetBundleName + "_" + platformLowerCase + "_" + 
			ResourceManager.GetUndottedStringForAssetBundleVersionToUse(assetBundleName) + ".assetbundle";
		return result;
	}
	
	/// <summary>
	/// Returns the location of where we save the asset bundle
	/// </summary>
	public static string GetAssetBundleDiskPath(string assetBundleName, int assetBundleVersion)
	{
		string result = string.Format("{0}/{1}_{2}_{3}_{4}.assetbundle",
			Application.persistentDataPath,
			assetBundleName,
			platformLowerCase,
			ResourceManager.GetUndottedStringForAssetBundleVersionToUse(assetBundleName),	//GetUndottedVersionString(),
			assetBundleVersion.ToString());
		notify.Debug ("GetAssetBundleDiskPath result=" + result);
		return result;		
	}

	public static string GetAssetBundleStreamingPath(string assetBundleName, int assetBundleVersion)
	{
		string result = string.Format("{0}/{1}_{2}_{3}.assetbundle",
			Application.streamingAssetsPath,
			assetBundleName,
			platformLowerCase,
			ResourceManager.GetUndottedStringForAssetBundleVersionToUse(assetBundleName));	//GetUndottedVersionString());
		notify.Debug ("GetAssetBundleStreamingPath result=" + result);
		return result;		
	}
	
	public static string GetAssetBundleInfoCallURL()
	{//https://amps.tapulous.com/amps/api/listassets/troz_test?deviceModel=iphone
		string result = "";
		if (Settings.GetBool("use-amps-server", true))// amp only
		{
			result = string.Format( "{0}/amps/api/listassets/{1}?deviceModel={2}&pageSize=100000",//&latestVersionOnly=true" , 
				ampsServer,
				ampsAppId,
				deviceModel
				);
		}
		return result;
	}
	
	/// <summary>
	/// Gets the full asset bundle URL, considers version, platform, server url
	/// </summary>
	/// <param name='assetBundleName'>
	/// Asset bundle name. e.g. darkforest, whimsywoods
	/// </param>
	public string GetFullAssetBundleURL(string assetBundleName)
	{
		string result;
		if (Settings.GetBool("use-amps-server", true))
		{
			if (cdnUrl != "" && cdnUrl != null)
			{
				result = cdnUrl;
			}
			else 
			{
				result = string.Format( "{0}/amps/api/downloadasset/{1}/{2}/{3}?deviceModel={4}", 
					ampsServer,
					ampsAppId,
					assetBundleName,
					GetFullAssetBundleName(assetBundleName),
					deviceModel
					);
				notify.Debug("use-amps-server url = " + result);
			}
		}
		else
		{
			result = string.Format( "{0}/{1}", altBundleUrl, GetFullAssetBundleName(assetBundleName));
			notify.Debug("full url = " + result);
		}
		return result;
	}
	
	/// <summary>
	/// Initializes a new instance of the <see cref="AssetBundleLoader"/> class.
	/// </summary>
	/// <param name='assetBundleName'>
	/// Asset bundle name. e.g whimsywoods darkforest
	/// </param>
	/// <param name='assetBundleVersion'>
	/// this should come from the login reply message
	/// </param>
	/// <param name='successCallback'>
	/// Success callback.
	/// </param>
	/// <param name='failureCallback'>
	/// Failure callback.
	/// </param>
	/// <param name='cdnUrl'>
	/// We can get an old file if we request it according to AMPS documentation, this is the direct cdn url
	/// </param>
	public AssetBundleLoader( string assetBundleName, int assetBundleVersion, bool embedded, LoadSuccess successCallback, LoadFailure failureCallback, string cdnUrl)
	{
		this.assetBundleBaseName = assetBundleName;
		this.isEmbedded = embedded;
		this.assetBundleVersion = assetBundleVersion;
		this.successCallback = successCallback;
		this.failureCallback = failureCallback;
		this.cdnUrl = cdnUrl;
		this.finished = false;
		www = null;
	}
	
	/// <summary>
	/// Loads the asset bundle via the WWW, note that this can be pointing to a local file or on a server somewheres
	/// </summary>
	/// <returns>
	/// The asset bundle url
	/// </returns>
	private IEnumerator LoadAssetBundleViaWww( string url, bool downloadOnly)
	{
		notify.Debug("downloading {0}", url);
		www = WWW.LoadFromCacheOrDownload( url, assetBundleVersion);
		www.threadPriority = ThreadPriority.High;	// it takes too  long (10 minutes) without this

		yield return www;
		notify.Debug("done with loadFromCacheOrDownload");
		
		if (www != null)
		{
			if (www.error != null)
			{
				notify.Warning("LoadAssetBundle failed with error: " + www.error);	
				//failureCallback( assetBundleBaseName, assetBundleVersion, www.error.ToString());
				TriggerFailureCallback( assetBundleBaseName, assetBundleVersion, www.error.ToString());
			}
			else if ( www.assetBundle == null)
			{
				notify.Warning("LoadAssetBundle asset bundle is null");	
				//failureCallback( assetBundleBaseName, assetBundleVersion, "asset bundle is null");
				TriggerFailureCallback( assetBundleBaseName, assetBundleVersion, "asset bundle is null");
			}
			else
			{
				notify.Debug("success loading assetbundle {0} assetBundle={1}", assetBundleBaseName, www.assetBundle);
				//successCallback( assetBundleBaseName, assetBundleVersion, downloadOnly, www.assetBundle);
				TriggerSuccessCallback( assetBundleBaseName, assetBundleVersion, downloadOnly, www.assetBundle);
			}
			notify.Debug ("setting finished tot true");
			finished = true;
			notify.Debug ("LoadAssetBundleViaWww disposing www in AssetBundleLoader");
			www.Dispose();
			notify.Debug ("nulling www");
			www = null;		
		}
		else
		{
			notify.Debug ("www was null in LoadAssetBundleViaWww");
		}
	}
	
	public void CancelDownload()
	{
		if (www != null)		// check that a download request is still outstanding
		{		
			if (!www.isDone)	// only do this if the download request has not already returned
			{
				notify.Warning("LoadAssetBundle failed with error: user cancelled");	
				notify.Debug ("setting finished tot true");
				notify.Debug ("CancelDownload disposing www in AssetBundleLoader");
				www.Dispose();
				notify.Debug ("nulling www");
				www = null;
			}
			else
			{
				notify.Debug ("www.isDone = true in CancelDownload in AssetBundleLoader");
			}
		}
		else
		{
			notify.Debug ("www = null in CancelDownload in AssetBundleLoader");
		}
		
		//failureCallback( assetBundleBaseName, assetBundleVersion, "CancelDownload called");
		TriggerFailureCallback( assetBundleBaseName, assetBundleVersion, "CancelDownload called");
		finished = true;		
	}	
	
	/// <summary>
	/// Loads an compressed asset bundle .
	/// </summary>
	private IEnumerator LoadAssetBundleCompressed(bool downloadOnly)
	{
		yield return null;
		
		notify.Debug("LoadAssetBundleCompressed start");

		if(isEmbedded)
		{
			string bundleFilePath = GetAssetBundleStreamingPath(assetBundleBaseName, assetBundleVersion);
			if (bundleFilePath.Contains("://"))
			{
				yield return ResourceManager.SharedInstance.StartCoroutine(LoadAssetBundleViaWww(bundleFilePath, downloadOnly));
//				LoadAssetBundleViaWww(bundleFilePath, downloadOnly);
			}
			else if (File.Exists(bundleFilePath))
			{
				AssetBundle assetBundleCreated = AssetBundle.CreateFromFile(bundleFilePath);
				if (assetBundleCreated == null)
				{
					//failureCallback( assetBundleBaseName, assetBundleVersion, "Compressed AssetBundle.CreateFromFile failed for" + bundleFilePath);
					TriggerFailureCallback( assetBundleBaseName, assetBundleVersion, "Compressed AssetBundle.CreateFromFile failed for" + bundleFilePath);
				}
				else
				{
					//successCallback( assetBundleBaseName, assetBundleVersion, downloadOnly, assetBundleCreated);
					TriggerSuccessCallback( assetBundleBaseName, assetBundleVersion, downloadOnly, assetBundleCreated);
				}
			}
			else
			{
				//failureCallback( assetBundleBaseName, assetBundleVersion, "CompressedAssetBundle.File not exist CreateFromFile failed for" + bundleFilePath);
				TriggerFailureCallback( assetBundleBaseName, assetBundleVersion, "CompressedAssetBundle.File not exist CreateFromFile failed for" + bundleFilePath);
			}
		}
		else
		{
			string url = GetFullAssetBundleURL(assetBundleBaseName);
			notify.Debug("Before StartCoroutine(LoadAssetBundleViaWww(url, downloadOnly)) in LoadAssetBundleCompressed");
			yield return ResourceManager.SharedInstance.StartCoroutine(LoadAssetBundleViaWww(url, downloadOnly));
			notify.Debug("After StartCoroutine(LoadAssetBundleViaWww(url, downloadOnly)) in LoadAssetBundleCompressed");
//			ResourceManager.SharedInstance.StartCoroutine(LoadAssetBundleViaWww(url, downloadOnly));
		}
	}
	
	/// <summary>
	/// Loads a uncompressed asset bundle
	/// </summary>
	private IEnumerator LoadAssetBundleUncompressed(bool downloadOnly)
	{
		yield return null;
		notify.Debug("{0} LoadAssetBundle",  Time.realtimeSinceStartup);
		float startTime = Time.realtimeSinceStartup;
		
		string bundleFilePath = GetAssetBundleDiskPath(assetBundleBaseName, assetBundleVersion);
		bool isDone = false;
		if (File.Exists(bundleFilePath))
		{
			AssetBundle assetBundleCreated = AssetBundle.CreateFromFile(bundleFilePath);
			if (assetBundleCreated == null)
			{
				File.Delete(bundleFilePath);
//				failureCallback( assetBundleBaseName, assetBundleVersion, "Uncompressed AssetBundle.CreateFromFile failed for" + bundleFilePath);
			}
			else
			{
				float endTime = Time.realtimeSinceStartup;
				notify.Debug("******** {0} seconds for LoadAssetBundle existing file {1}", endTime - startTime, bundleFilePath );				
				//successCallback( assetBundleBaseName, assetBundleVersion, downloadOnly, assetBundleCreated);
				TriggerSuccessCallback( assetBundleBaseName, assetBundleVersion, downloadOnly, assetBundleCreated);
				isDone = true;
			}
		}
		
		if( isDone == false )
		{
			notify.Debug("disk:"+Application.persistentDataPath+" not exist the assetbundle ");
			if(isEmbedded)
			{
				bundleFilePath = GetAssetBundleStreamingPath(assetBundleBaseName, assetBundleVersion);
				bool exists = File.Exists(bundleFilePath);
				notify.Debug ("bundleFilePath={0} exists={1}", bundleFilePath, exists);
				if (bundleFilePath.Contains("://"))
				{
					yield return ResourceManager.SharedInstance.StartCoroutine(LoadAssetBundleViaWww(bundleFilePath, downloadOnly));
				}
				else if (File.Exists(bundleFilePath))
				{
					notify.Debug ("the bundleFilePath exists");
					AssetBundle assetBundleCreated = AssetBundle.CreateFromFile(bundleFilePath);
					if (assetBundleCreated == null)
					{
						//failureCallback( assetBundleBaseName, assetBundleVersion, "Embedded UncompressedAssetBundle.CreateFromFile failed for" + bundleFilePath);
						TriggerFailureCallback( assetBundleBaseName, assetBundleVersion, "Embedded UncompressedAssetBundle.CreateFromFile failed for" + bundleFilePath);
					}
					else
					{
						float endTime = Time.realtimeSinceStartup;
						notify.Debug("******** {0} seconds for LoadAssetBundle existing file {1}", endTime - startTime, bundleFilePath );				
						//successCallback( assetBundleBaseName, assetBundleVersion, downloadOnly, assetBundleCreated);
						TriggerSuccessCallback( assetBundleBaseName, assetBundleVersion, downloadOnly, assetBundleCreated);
					}
				}
				else
				{
					//failureCallback( assetBundleBaseName, assetBundleVersion, "Embedded Uncompressed AssetBundle.File not exist CreateFromFile failed for" + bundleFilePath);
					TriggerFailureCallback( assetBundleBaseName, assetBundleVersion, "Embedded Uncompressed AssetBundle.File not exist CreateFromFile failed for" + bundleFilePath);
				}
			}
			else
			{
				yield return Resources.UnloadUnusedAssets();
				System.GC.Collect();
				// no file, download it firsts
				string url = GetFullAssetBundleURL(assetBundleBaseName);
				www = new WWW(url);
				yield return www;
				
				notify.Debug("url=" + url);	
				
				if (www != null)
				{
					notify.Debug("url=" + url+ " www.isDone = " + www.isDone);	
					
					if (www.error != null)
					{
						notify.Error("LoadAssetBundle failed with error: " + www.error);	
						//failureCallback( assetBundleBaseName, assetBundleVersion, www.error.ToString());
						TriggerFailureCallback( assetBundleBaseName, assetBundleVersion, www.error.ToString());
						
						notify.Debug ("LoadAssetBundleUncompressed disposing www in AssetBundleLoader");
						www.Dispose();
						notify.Debug ("nulling www");
						www = null;
					}
					else
					{
						System.GC.Collect();
						notify.Debug("www.isDone = " + www.isDone + " numbytes = " + www.bytes.Length);
						/// save the bundle data to disk
						bool fileCreated = false;
						try 
						{
							//FileStream fs = ;
							using (BinaryWriter fileWriter = new BinaryWriter(File.Create(bundleFilePath)))
							{
								fileWriter.Write(www.bytes);
								fileWriter.Flush();
								fileWriter.Close();
							}
							notify.Debug("success saving to " + bundleFilePath);
							fileCreated = true;
						}
						catch (System.Exception e) 
						{
							notify.Warning("Save Exception: " + e);
							//failureCallback( assetBundleBaseName, assetBundleVersion, www.error.ToString());
							notify.Debug("About to call TriggerFailureCallback");
							string errorString = "www.error is null";
							if (www.error != null)
							{
								errorString = www.error.ToString();
							}
							TriggerFailureCallback( assetBundleBaseName, assetBundleVersion, errorString);
							notify.Debug("Done with TriggerFailureCallback");
						}
						
						notify.Debug ("LoadAssetBundleUncompressed disposing www in AssetBundleLoader");// do this as soon as possible to free up memory
						www.Dispose();
						notify.Debug ("nulling www");
						www = null;
						yield return Resources.UnloadUnusedAssets();
						System.GC.Collect();
						if (fileCreated)
						{
							/// now that it's saved, read it
							AssetBundle assetBundleCreated = AssetBundle.CreateFromFile(bundleFilePath);
							if (assetBundleCreated == null)
							{
								//failureCallback( assetBundleBaseName, assetBundleVersion, "WWW AssetBundle.CreateFromFile failed for " + bundleFilePath);
								TriggerFailureCallback( assetBundleBaseName, assetBundleVersion, "WWW AssetBundle.CreateFromFile failed for " + bundleFilePath);
							}
							else
							{
								float endTime = Time.realtimeSinceStartup;
								notify.Debug("******** {0} seconds for LoadAssetBundle downloaded and saved file {1}", endTime - startTime, bundleFilePath );	
								//bool success = successCallback( assetBundleBaseName, assetBundleVersion, downloadOnly, assetBundleCreated);
								bool success = TriggerSuccessCallback( assetBundleBaseName, assetBundleVersion, downloadOnly, assetBundleCreated);
			
								if( success )
								{
									CleanUpOldBundleFiles(assetBundleBaseName, bundleFilePath);
								}
								else
								{
									File.Delete(bundleFilePath);// catalog check failed!!!
									notify.Error("!!!  Bad catalog detected, bundle deleted : " + bundleFilePath );	
									assetBundleCreated.Unload(true);
								}
									
							}					
							yield return Resources.UnloadUnusedAssets();
							System.GC.Collect();
						}
					}
				}
				else
				{
					notify.Debug ("www was null, probably due to user cancellation");
				}
			}
		}
		
		finished = true;		
	}
	
	private bool TriggerSuccessCallback(string assetBundleBaseName, int assetBundleVersion, bool downloadOnly, AssetBundle assetBundleCreated)
	{
		bool result = false;
		notify.Debug("TriggerSuccessCallback start");
		System.Diagnostics.StackTrace t = new System.Diagnostics.StackTrace();
		notify.Debug(t);
		if (!successCallbackCalled && !failureCallbackCalled)
		{
			result = successCallback(assetBundleBaseName, assetBundleVersion, downloadOnly, assetBundleCreated);
			notify.Debug("successCallback triggered in AssetBundleLoader");
		}
		
		successCallbackCalled = true;
		return result;
	}	

	private void TriggerFailureCallback(string assetBundleBaseName, int assetBundleVersion, string errorMsg)
	{
		notify.Debug("TriggerFailureCallback start");
		notify.Debug("failureCallbackCalled ={0} successCallbackCalled={1}", failureCallbackCalled, successCallbackCalled);
		if (!failureCallbackCalled && !successCallbackCalled)
		{		
			failureCallback(assetBundleBaseName, assetBundleVersion, errorMsg);
			notify.Debug("failureCallback triggered in AssetBundleLoader");
		}
		
		failureCallbackCalled = true;
	}

	private void CleanUpOldBundleFiles(string filename, string fullname)
	{
		notify.Debug( "----- Cleaning up old bundle files, new bundle file =  " + fullname );
		notify.Debug( "----- filter : " + filename );
		
		string path = Application.persistentDataPath;
		string [] flist = Directory.GetFiles(path);
		if( flist == null )
		{
			notify.Debug( "Bad Folder!!!!");
			return ;
		}
		
		foreach( string f in flist )
		{
			if( f != null && f.Contains(filename) &&  f != fullname )
			{
				notify.Debug( "!!!!!!!! old bundle file deleted : " + f );
				File.Delete(f);
			}
		}
	}

    /// <summary>
    /// Coroutine for loading the asset bundle, downloadOnly is meant for use by ResouceManager
    /// </summary>
    /// <returns>
    /// The asset bundle.
    /// </returns>
    /// <param name='downloadOnly'>
    /// Use by resource manager, if false, then it makes the contents of the asset bundle available for immediate use
    /// </param>
    public IEnumerator LoadAssetBundle(bool downloadOnly)
    {
        if (AssetBundleLoader.IsCompressed(this.assetBundleBaseName))
        {
            notify.Debug("Before call to LoadAssetBundleCompressed(downloadOnly) in LoadAssetBundle");
            return LoadAssetBundleCompressed(downloadOnly);
        }
        else
        {
            return LoadAssetBundleUncompressed(downloadOnly);
        }
    }

    /// <summary>
	/// Gets the assetbunlde loading progress, returns a number between 0 and 1
	/// </summary>
	/// <returns>
	/// The progress.
	/// </returns> 
	public float GetProgress()
	{
		notify.Debug ("GetProgress start finished={0}", finished);
		float result =0;
		// return 95% when www.progress is at 100%
		// return 98% when www.progress is at 100% and www.done is true
		// return 100% when www.progress is at 100%, www.done is true and www.assetbundle is not null
		notify.Debug ("GetProgress assetBundleBaseName=" + assetBundleBaseName + "(www == null) = " + (www == null));
		try
		{
			if (finished)
			{
				result = 1;	
			}
			else if (www !=null &&  www.isDone && IsRegistered(this.assetBundleBaseName) && ! IsCompressed(this.assetBundleBaseName))
			{
				result = 0.99f;
			}
			else if (www !=null &&  www.isDone && www.assetBundle != null)
			{
				result = 1.0f;	
			}
			else if ( www!= null && www.isDone)
			{
				result = 0.98f;	
			}
			else if (www != null)
			{
				result = www.progress * 0.95f;	
			}
		}
		catch (System.Exception exception)
		{
			notify.Warning("GetProgress exception" + exception);
		}
		notify.Debug ("returning " + result);
		return result;
	}
}
