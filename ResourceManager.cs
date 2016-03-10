using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Replaces Resource.Load in the app, will first try to read from Resources, then from the asset bundles that we know of
/// </summary>
public class ResourceManager : MonoBehaviour
{
	protected static Notify notify;
	public static ResourceManager SharedInstance;
	
	public struct AssetBundleInfo
	{
		public string assetBundleBaseName;
		public bool isEmbedded;
		public int assetBundleVersion;
		public AssetBundle assetBundle;
		public AssetBundleLoader loader;
		
		public AssetBundleInfo( string baseName, int version, bool embedded)
		{
			assetBundleBaseName = baseName;
			assetBundleVersion = version;
			isEmbedded = embedded;
			assetBundle = null;
			loader = null;
		}
	}
	
	public void OnEnable()
	{
		// Do absolutely nothing.  Bug in Unity script execution order
	}
	
	public delegate void OnAssetBundleLoadSuccessHandler(string assetBundleName, int version, bool downloadOnly);
	private static event OnAssetBundleLoadSuccessHandler onAssetBundleSuccessEvent = null;
	public void RegisterForOnAssetBundleLoadSuccess(OnAssetBundleLoadSuccessHandler delg) { 
		onAssetBundleSuccessEvent += delg; }
	public void UnRegisterForOnAssetBundleLoadSuccess(OnAssetBundleLoadSuccessHandler delg) { 
		onAssetBundleSuccessEvent -= delg; }		
	
	// TODO Alex pls add  downloadonly flag parameter if the gui needs it for OnAssetBundleLoadFailureHandler
	public delegate void OnAssetBundleLoadFailureHandler(string assetBundleName, int version, string errMsg);
	private static event OnAssetBundleLoadFailureHandler onAssetBundleFailureEvent = null;
	public void RegisterForOnAssetBundleLoadFailure(OnAssetBundleLoadFailureHandler delg) { 
		onAssetBundleFailureEvent += delg; }
	public void UnRegisterForOnAssetBundleLoadFailure(OnAssetBundleLoadFailureHandler delg) { 
		onAssetBundleFailureEvent -= delg; }		
	
	public DownloadedAssetBundleTracker downloadedAssetBundles ;
	Dictionary<string, AssetBundleInfo> AssetBundles = new Dictionary<string, AssetBundleInfo>();
	Dictionary<string, AssetBundle> ResourceToAssetBundle = new Dictionary<string, AssetBundle>();
	/// <summary>
	/// Replaces Resource.Load, tries loading from Resources first then from asset bundles
	/// </summary>
	/// <param name='path'>
	/// Path.
	/// </param>
	public static Object Load (string path)
	{
		return ResourceManager.Load (path, typeof(Object));
	}
	
	public static bool AllowLoad = false; 
	/// <summary>
	/// Replaces Resource.Load, tries loading from Resources first then from asset bundles
	/// </summary>
	/// <param name='path'>
	/// Path.
	/// </param>
	/// <param name='type'>
	/// Type.
	/// </param>
	public static  Object Load (string path, System.Type type)
	{
		//notify.Debug("ResourceManager.Load path={0}", path);
		UnityEngine.Object result =  Resources.Load (path, type);		
		if (result == null)
		{
			// at this point try our asset bundles
			// for all of our asset bundles they have unique filenames
			string filename = System.IO.Path.GetFileName(path);
			if (SharedInstance.ResourceToAssetBundle.ContainsKey(filename))
			{
				result = SharedInstance.ResourceToAssetBundle[filename].Load(filename, type);
			}
		}
		return result;
	}
	
	/// <summary>
	/// Loads asynchronously from the assetbundle.
	/// </summary>
	/// <returns>
	/// Will return null if not found in a currently loaded asset bundle. You must yield on
	/// the returned AssetBundleRequest inside a coroutine
	/// </returns>
	/// <param name='path'>
	/// name of the asset bundle
	/// </param>
	public AssetBundleRequest LoadAsyncFromAssetBundle( string path, System.Type type)
	{
		if(!AllowLoad)
		{
			notify.Error("Load disabled!!"); 
		}
		
		// at this point try our asset bundles
		// for all of our asset bundles they have unique filenames
		string filename = System.IO.Path.GetFileName(path);
		if (SharedInstance.ResourceToAssetBundle.ContainsKey(filename))
		{
			AssetBundleRequest abr = SharedInstance.ResourceToAssetBundle[filename].LoadAsync(filename, type);
			return abr;			
		}		
		return null;
	}
	
	//static string cachedUndottedAssetBundleVersionString = null;	// no more caching now, since this can return different values for different bundles
	/// <summary>
	/// Sometimes we need the version string without a dot, instead of 1.10 we return 1_10
	/// </summary>
	/// <returns>
	/// The munged version string. the dot gets replaced with an underscore
	/// </returns>
	public static string GetUndottedStringForAssetBundleVersionToUse(string assetBundleName)	//GetUndottedVersionString()
	{
		//if (cachedUndottedAssetBundleVersionString != null)
		//{
		//	return cachedUndottedAssetBundleVersionString;
		//}
		
		//cachedUndottedAssetBundleVersionString = GetAssetBundleVersionToUse(assetBundleName).Replace(".","_");	// GetVersionString().Replace(".","_");
		//return cachedUndottedAssetBundleVersionString;
		
		return GetAssetBundleVersionToUse(assetBundleName).Replace(".","_");
	}
	
	/// <summary>
	/// Gets the asset bundle version number to use for this client version.
	/// </summary>
	/// <returns>
	/// The asset bundle version to use.
	/// </returns>
	private static string GetAssetBundleVersionToUse(string assetBundleName)
	{
	    string clientVersion = BundleInfo.GetBundleVersion();
		
		AssetBundleCompatibilityEntry entry = AssetBundleCompatibilityManager.GetAssetBundleCompatibilityEntry(
			clientVersion, assetBundleName);
		
		if (entry != null)
		{
			return entry.assetBundleVersionToUse;
		}
		else
		{
			return clientVersion;
		}
	}
	
	/// <summary>
	/// Gets the build number produced from jenkins, defaults to 2 if not found
	/// </summary>
	/// <returns>
	/// The build number.
	/// </returns>
	public static int GetBuildNumber() 
	{
		int result = 2;
		try
		{
			TextAsset versionAsset = (TextAsset) Resources.Load ("build_number", typeof(TextAsset));
			if (versionAsset != null)
			{
				result = int.Parse(versionAsset.text.Trim());
			}
			else
			{
				notify.Debug("no build_number.txt file found, defaulting to 2");	
			}
		}
		catch ( System.Exception e)
		{
			notify.Error("Error reading build_number.txt Exception =" + e.Message);
		}
		return result;
	}
	
	void Awake()
	{
		SharedInstance = this;
		notify = new Notify(this.GetType().Name);
	}

    public void Initializ()
    {
        AssetBundleCompatibilityManager.LoadFile();	// load asset bundle compatibility entries
        downloadedAssetBundles = new DownloadedAssetBundleTracker();
        downloadedAssetBundles.Load();
    }
	
	
	/// <summary>
	/// Loads the asset bundle.
	/// </summary>
	/// <param name='assetBundleName'>
	/// Asset bundle base name name.
	/// </param>
	/// <param name='assetBundleVersion'>
	/// Asset bundle version, typically received from login reply, if we get -1, we'll use the build number
	/// </param>
	/// <param name='downloadOnly'>
	/// if true, will unload the assetbundle from memory as soon as it's been loaded, used to keep the assetbundle in the disk cache
	/// </param>
	public AssetBundleLoader LoadAssetBundle(string assetBundleName, bool downloadOnly, int assetBundleVersion, bool isEmbedded, bool debugProgress)
	{
		if (assetBundleVersion == -1)
		{
			assetBundleVersion = downloadedAssetBundles.GetLatestVersionNumber( assetBundleName );
		}

		if( assetBundleVersion == -1)// still not avaiable, look for info in the AMP Asset List
		{
			//assetBundleVersion = UIManagerOz.SharedInstance.ampAssetBundleData.GetAssetVersion(assetBundleName);
			assetBundleVersion = Services.Get<AmpBundleManager>().ampAssetBundleData.GetAssetVersion(assetBundleName);
		}
		
		if( assetBundleVersion == -1)// not avaiable
		{
			assetBundleVersion = GetBuildNumber();// will download whatever is up there
			
			if ( Settings.GetBool("use-amps-server", true) )
			{
				// if we're using the alt-bundle-url, then we dont have a version as specified by amps
				notify.Error("AssetBundleVersion Number should not be -1 anymore");
				
				//UIManagerOz.SharedInstance.ShowAMPSDownloadError( "Msg_DownloadFailLong", "ResourceManager::LoadAssetBundle: couldn't get AssetBundleVersion for " + assetBundleName );
				Services.Get<AmpBundleManager>().ShowAMPSDownloadError( "Msg_DownloadFailLong", "ResourceManager::LoadAssetBundle: couldn't get AssetBundleVersion for " + assetBundleName );
	
				onAssetBundleFailureEvent(assetBundleName, assetBundleVersion, "");
				return null;
			}
		}
		
		
		notify.Debug("LoadAssetBundle {0} {1} {2}", assetBundleName, downloadOnly, assetBundleVersion);
		AssetBundleInfo newInfo = new AssetBundleInfo(assetBundleName, assetBundleVersion, isEmbedded);
		//string cdnUrl = UIManagerOz.SharedInstance.ampAssetBundleData.GetCdnUrl(assetBundleName);
		string cdnUrl = Services.Get<AmpBundleManager>().ampAssetBundleData.GetCdnUrl(assetBundleName);
		
		newInfo.loader = new AssetBundleLoader(assetBundleName, assetBundleVersion, isEmbedded, LoadSuccess, LoadFailure, cdnUrl);
		notify.Debug("Adding " + assetBundleName + "to AssetBundles, in LoadAssetBundle");
		AssetBundles[assetBundleName] = newInfo;
		StartCoroutine(newInfo.loader.LoadAssetBundle(downloadOnly));
		// temporarily since we only do this on the console:
		if( debugProgress )
			StartCoroutine(ShowProgress(newInfo));
		return newInfo.loader;
	}
	
	/// <summary>
	/// Updates downloadedAssetBundle if the expected files are NOT on disk
	/// needed when we go from 1.0 to 1.2
	/// </summary>
	public void VerifyDownloadedFilesArePresent()
	{
		downloadedAssetBundles.VerifyDownloadedFilesArePresent();
	}
	
	/// <summary>
	/// Loads the asset bundle, but does it in the coroutine way
	/// </summary>
	/// <returns>
	/// The asset bundle.
	/// </returns>
	/// <param name='assetBundleName'>
	/// Asset bundle name.
	/// </param>
	/// <param name='assetBundleVersion'>
	/// Asset bundle version, n, typically received from login reply, if we get -1, we'll use the build number
	/// </param>
	public IEnumerator LoadAssetBundleCoroutine(string assetBundleName,  bool downloadOnly, int assetBundleVersion, bool isEmbedded)
	{
	    if (assetBundleVersion == -1)
	        assetBundleVersion = downloadedAssetBundles.GetLatestVersionNumber(assetBundleName);

		if( assetBundleVersion == -1)// still not avaiable, look for info in the AMP Asset List
			assetBundleVersion = Services.Get<AmpBundleManager>().ampAssetBundleData.GetAssetVersion(assetBundleName);
		
		if( assetBundleVersion == -1)// not avaiable
		{
			assetBundleVersion = GetBuildNumber();// will download whatever is up there
			
			if ( Settings.GetBool("use-amps-server", true) )
			{
				// if we're using the alt-bundle-url, then we dont have a version as specified by amps
				notify.Error("AssetBundleVersion Number should not be -1 anymoe");
			}
		}
		
		notify.Debug("LoadAssetBundleCoroutine {0} {1} {2}", assetBundleName, downloadOnly, assetBundleVersion);
		AssetBundleInfo newInfo = new AssetBundleInfo(assetBundleName, assetBundleVersion, isEmbedded);
	
		string cdnUrl = Services.Get<AmpBundleManager>().ampAssetBundleData.GetCdnUrl(assetBundleName);
		notify.Debug("want to load assetBundleName:"+assetBundleName+";  cndUrl::"+cdnUrl);
		newInfo.loader = new AssetBundleLoader(assetBundleName, assetBundleVersion, isEmbedded, LoadSuccess, LoadFailure, cdnUrl);
		notify.Debug("Adding " + assetBundleName + " to AssetBundles, in LoadAssetBundleCoroutine");		
		AssetBundles[assetBundleName] = newInfo;
		yield return StartCoroutine(newInfo.loader.LoadAssetBundle(downloadOnly));		
	}
	
	/// <summary>
	/// utility function to remove entries in a dictionary 
	/// </summary>
	public static void RemoveByValue<TKey,TValue>(Dictionary<TKey, TValue> dictionary, TValue someValue)
	{
	    List<TKey> itemsToRemove = new List<TKey>();
	
	    foreach (var pair in dictionary)
	    {
	        if (pair.Value.Equals(someValue))
	            itemsToRemove.Add(pair.Key);
	    }
	
	    foreach (TKey item in itemsToRemove)
	    {
	        dictionary.Remove(item);
	    }
	}
	
	/// <summary>
	/// Unloads the asset bundle. frees up memory as well
	/// </summary>
	/// <param name='assetBundleName'>
	/// Asset bundle name.
	/// </param>
	/// <param name='assetBundleVersion'>
	/// Asset bundle version.
	/// </param>
	public void UnloadAssetBundle(string assetBundleName)
	{
		AssetBundleInfo toBeRemoved;
		if (AssetBundles.TryGetValue(assetBundleName, out toBeRemoved))
		{
			RemoveByValue<string,AssetBundle>( this.ResourceToAssetBundle, toBeRemoved.assetBundle);
			if (toBeRemoved.assetBundle != null)
			{
				toBeRemoved.assetBundle.Unload(true);
			}
			toBeRemoved.assetBundle = null;
			toBeRemoved.loader = null;
			notify.Debug("Removing " + assetBundleName + "from AssetBundles, in UnloadAssetBundle");		
			this._removeAssetBundle(assetBundleName);
			//TODO will this be enough, or do I need to rewrite dispose functions
		}
	}
	
	/// <summary>
	/// Shows the progress on the debug console,  should NOT be called when we have real GUI
	/// </summary>
	private IEnumerator ShowProgress(AssetBundleInfo newinfo)
	{
		while (newinfo.loader.GetProgress() < 1 && newinfo.assetBundle == null)
		{
			if ( Settings.GetBool("console-enabled", false))
			{
				DebugConsoleOz.echo( "progress = " +  (newinfo.loader.GetProgress() * 100f).ToString() + "%");
			}
			yield return new WaitForSeconds(0.1f);
		}
		//DebugConsoleOz.echo( "finished with " + newinfo.assetBundleBaseName);
	}
	
	/// <summary>
	/// Processes the catalog embedded in the asset bundle inside bundleInfo
	/// </summary>
	/// <returns>
	/// true if all went well, false if it couldn't read the catalog
	/// </returns>
	/// <param name='bundleInfo'>
	/// Bundle info.
	/// </param>
	bool processCatalog(AssetBundleInfo bundleInfo)
	{
		notify.Debug("processCatalog start {0}", bundleInfo.assetBundleBaseName);
		bool result = true;
		// read the catalog text file, then add it to our ResourceToAssetBundle dictionary
		string catalogName = AssetBundleLoader.GetFullAssetBundleName(bundleInfo.assetBundleBaseName);
		catalogName = System.IO.Path.GetFileNameWithoutExtension(catalogName);
		catalogName += "_catalog";
		TextAsset catalog = (TextAsset) bundleInfo.assetBundle.Load(catalogName);
		if (catalog == null)
		{
			notify.Error("Could not read {0} from assetbundle {1}", catalogName, bundleInfo.assetBundleBaseName);
			result = false;	
		}
		else
		{
			string[] lines =  catalog.text.Split('\n');
			foreach (string line in lines)
			{
				if (line.Trim().Length == 0)
				{
					// sometimes an endline at the very end gets through
					continue;	
				}
				notify.Debug("processing {0}",line);
				try 
				{
					ResourceToAssetBundle.Add( line, bundleInfo.assetBundle);
				}
				catch (System.Exception theException)
				{
					if (line != null && line.Length > 0)
					{
						notify.Warning("Error parsing catalog for: {0} exception = {1}", line, theException);	
					}
				}
			}
		}
		return result;
	}
	
	/// <summary>
	/// returns true if we have a valid catalog
	/// </summary>
	/// <returns>
	/// The catalog valid.
	/// </returns>
	bool isCatalogValid(AssetBundleInfo bundleInfo)
	{
		notify.Debug("isCatalogValid start {0} {1}", bundleInfo.assetBundleBaseName, bundleInfo.assetBundle);
		bool result = true;
		// read the catalog text file, then add it to our ResourceToAssetBundle dictionary
		string catalogName = AssetBundleLoader.GetFullAssetBundleName(bundleInfo.assetBundleBaseName);
		catalogName = System.IO.Path.GetFileNameWithoutExtension(catalogName);
		catalogName += "_catalog";
		TextAsset catalog = (TextAsset) bundleInfo.assetBundle.Load(catalogName);
		notify.Debug ("catalogName={0}", catalogName);
		if (catalog == null)
		{
			notify.Error("Could not read {0} from assetbundle {1}", catalogName, bundleInfo.assetBundleBaseName);
			result = false;	
		}
		else
		{
			string[] lines =  catalog.text.Split('\n');
			List<string> addedResources = new List<string>();
			foreach (string line in lines)
			{
				if (line.Trim().Length == 0)
				{
					// sometimes an endline at the very end gets through
					continue;	
				}
				notify.Debug("processing {0}",line);
				try 
				{
					//ResourceToAssetBundle.Add( line, bundleInfo.assetBundle);
					addedResources.Add(line);
				}
				catch (System.Exception theException)
				{
					if (line != null && line.Length > 0)
					{
						notify.Warning("Error parsing catalog for: {0} exception = {1}", line, theException);	
					}
				}
			}
			if (addedResources.Count == 0)
			{
				notify.Warning("got an empty catalog for {0}", bundleInfo.assetBundleBaseName);
				result = false;
			}
		}
		return result;		
	}
	
	/// <summary>
	/// Read each entry in the asset bundle, doing so will make unity decompress it and save it on disk
	/// assumes we have a catalog
	/// </summary>
	/// <param name='AssetBundleInfo'>
	/// should have a valid Asset bundle name and a valid AssetBundle
	/// </param>
	void DecompressAssetBundle( AssetBundleInfo bundleInfo)
	{
		string catalogName = AssetBundleLoader.GetFullAssetBundleName(bundleInfo.assetBundleBaseName);
		catalogName = System.IO.Path.GetFileNameWithoutExtension(catalogName);
		catalogName += "_catalog";
		TextAsset catalog = (TextAsset) bundleInfo.assetBundle.Load(catalogName);
		
		// this sucks and will take a while ~15 secs on 3gs, 2-3 secs on iPad3
		string[] lines =  catalog.text.Split('\n');
		foreach (string line in lines)
		{
			if (line.Trim().Length == 0)
			{
				// sometimes an endline at the very end gets through
				continue;	
			}
			notify.Debug("Decompressing {0}",line);
			bundleInfo.assetBundle.Load(line);
		}
	}
	
	/// <summary>
	/// Double check the bundle is valid, then unload it
	/// </summary>
	/// <param name='assetBundleName'>
	/// Asset bundle name.
	/// </param>
	/// <param name='assetBundleVersion'>
	/// Asset bundle version.
	/// </param>
	/// <param name='loadedBundle'>
	/// Loaded bundle.
	/// </param>
	bool LoadSuccessForDownloadOnly( string assetBundleName, int assetBundleVersion, AssetBundle loadedBundle)
	{
		bool result = true;
		notify.Debug("LoadSuccessForDownloadOnly assetBundleName = " + assetBundleName);
		notify.Debug ("LoadSuccessForDownloadOnly loadedBundle={0}", loadedBundle);
		notify.Debug("Modifying " + assetBundleName + "in AssetBundles, in LoadSuccessForDownloadOnly");		
		AssetBundleInfo temp;
		if ( AssetBundles.TryGetValue(assetBundleName, out temp))
		{	
			temp.assetBundle = loadedBundle;
			AssetBundles[assetBundleName] = temp;
			if ( isCatalogValid(temp))
			{
				//DecompressAssetBundle(temp);
				downloadedAssetBundles.UpdateAssetBundleDownload( assetBundleName, assetBundleVersion);
				notify.Debug("downloaded asset bunde {0} is good and has a valid catalog", assetBundleName);
				if ( Settings.GetBool("console-enabled", false))
				{
					DebugConsoleOz.echo( "success  downloading " + assetBundleName);
				}
				if (onAssetBundleSuccessEvent != null)
				{
					onAssetBundleSuccessEvent(assetBundleName, assetBundleVersion, true);	
				}
				result = true;
			}
			else
			{
				notify.Warning("downloaded asset bunde {0} but it has an error in the catalog", assetBundleName);
				DebugConsoleOz.echo(string.Format("downloaded asset bunde {0} but it has an error in the catalog", assetBundleName));
				if (onAssetBundleFailureEvent != null)
				{
					onAssetBundleFailureEvent(assetBundleName, assetBundleVersion, "catalog parse error");	
				}
				result = false;
			}
			loadedBundle.Unload(false);
			notify.Debug("Removing " + assetBundleName + "from AssetBundles, in LoadSuccessForDownloadOnly");		
			_removeAssetBundle(assetBundleName);
		}
		else
		{
			//Crittercism.LeaveBreadcrumb("avoiding DMTRO-2034");	
		}
		return result;
	}
	
	/// <summary>
	/// Callback when we successfully load the asset bundle
	/// </summary>
	/// <returns>
	/// true if assetbundle also had a valid catalog, false in all other cases
	/// </returns>
	/// <param name='assetBundleName'>
	/// Asset bundle name.
	/// </param>
	/// <param name='assetBundleVersion'>
	/// Asset bundle version.
	/// </param>
	/// <param name='loadedBundle'>
	/// Loaded bundle.
	/// </param>
	bool LoadSuccess(string assetBundleName, int assetBundleVersion, bool downloadOnly, AssetBundle loadedBundle)
	{
		if (downloadOnly)
		{
			return LoadSuccessForDownloadOnly (assetBundleName, assetBundleVersion, loadedBundle);
		}	
		notify.Debug("LoadSuccess");
		
		bool result = false;
		AssetBundleInfo temp; //= AssetBundles[assetBundleName];
		if ( AssetBundles.TryGetValue(assetBundleName, out temp))
		{
			temp.assetBundle = loadedBundle;
			notify.Debug("Modifying " + assetBundleName + "in AssetBundles, in LoadSuccess");				
			AssetBundles[assetBundleName] = temp;
			// note even though we got here, there's still a small chance the catalog is invalid
	
			result = processCatalog(AssetBundles[assetBundleName]);
			if (result)
			{
				downloadedAssetBundles.UpdateAssetBundleDownload( assetBundleName, assetBundleVersion);
				notify.Debug("loaded {0} and processed the catalog", assetBundleName);
				
	//			DebugConsoleOz.echo( "success loading " + assetBundleName);
				// send an event that we've succeeded to let GUI know	
				if (onAssetBundleSuccessEvent != null)
				{
					onAssetBundleSuccessEvent(assetBundleName, assetBundleVersion, downloadOnly);	
				}
			}
			else
			{
	//			DebugConsoleOz.echo( "error parsing catalog of " + assetBundleName);
				notify.Warning("We were able to load {0}, but there was an error in the catalog file", assetBundleName);	
					// send an event that we've succeeded to let GUI know	
				if (onAssetBundleFailureEvent != null)
				{
					onAssetBundleFailureEvent(assetBundleName, assetBundleVersion, "catalog parse error");	
				}
			}
		}
		else
		{
			//Crittercism.LeaveBreadcrumb("avoiding DMTRO-2033");	
		}
		return result;
	}
	
	/// <summary>
	/// Callback when there was a failure loading the aset bundle
	/// </summary>
	/// <param name='assetBundleName'>
	/// Asset bundle name.
	/// </param>
	/// <param name='asssetBundleVersion'>
	/// Assset bundle version.
	/// </param>
	/// <param name='errorMsg'>
	/// Error message. typically www.error
	/// </param>
	void LoadFailure( string assetBundleName, int asssetBundleVersion, string errorMsg)
	{
		notify.Warning("ResourceManager.LoadFailure {0}, {1}, {2}", assetBundleName, asssetBundleVersion, errorMsg);
		notify.Debug("Removing " + assetBundleName + " from AssetBundles, in LoadFailure");				
		_removeAssetBundle(assetBundleName);
		//DebugConsoleOz.echo( "error loading " + assetBundleName + " error=" + errorMsg);
		
		if (onAssetBundleFailureEvent != null)
		{
			onAssetBundleFailureEvent(assetBundleName, asssetBundleVersion, errorMsg);	
		}
		//TODO at this point send an event
	}
	
	/// <summary>
	/// Returns true if the assetBundle is loaded and available
	/// </summary>
	/// <returns>
	/// <c>true</c> if the assetBundle is loaded and available ; otherwise, <c>false</c>.
	/// </returns>
	/// <param name='assetBundleName'>
	/// The asset bundle name to check
	/// </param>
	public bool IsAssetBundleAvailable( string assetBundleName)
	{
		bool result = false;
		AssetBundleInfo  info;
		if ( AssetBundles.TryGetValue(assetBundleName, out info))
		{
			if (info.assetBundle != null)
			{
				result = true;	
			}
		}
		return result;
	}

	/// <summary>
	/// Returns true if the assetBundle has been downloaded. Note that just because it's been downloaded
	/// you still many not be able to use it. That's the IsAssetBundleAvailable check.  To use it you'll need to call
	/// LoadAssetBundle
	/// </summary>
	/// <returns>
	/// <c>true</c> if the assetBundle has been downloaded ; otherwise, <c>false</c>.
	/// </returns>
	/// <param name='assetBundleName'>
	/// The asset bundle name to check
	/// </param>	
	public bool IsAssetBundleDownloaded( string assetBundleName, int version = (int.MinValue +1))
	{
		bool result = downloadedAssetBundles.IsAssetBundleDownloaded( assetBundleName,version) ;
		return result;
	}
	
	public bool IsAssetBundleDownloadedLastestVersion( string assetBundleName)
	{
		bool result = downloadedAssetBundles.IsAssetBundleDownloadedLastestVersion( assetBundleName) ;
		return result;
	}
	
	/// <summary>
	/// removes the asset bundle associated with assetBundleName
	/// </summary>
	/// <param name='assetBundleName'>
	/// Asset bundle name.
	/// </param>
	private void _removeAssetBundle(string assetBundleName)
	{
		notify.Debug("Removing " + assetBundleName + " from AssetBundles, in RemoveAssetBundleLoader");
		try
		{
			AssetBundles.Remove(assetBundleName);
		}
		catch (System.Exception e)
		{
			notify.Warning("_removeAssetBundleLoader {0} could not remove {1}", e, assetBundleName);
		}
	}
	
	/// <summary>
	/// Properly cancel an asset bundle download
	/// </summary>
	/// <param name='assetBundleName'>
	/// Asset bundle name, basename e.g darkforest, yellowbrickroad
	/// </param>
	public void CancelDownload(string assetBundleName)
	{
		AssetBundleLoader loader = null;
		AssetBundleInfo info;
		if (AssetBundles.TryGetValue(assetBundleName, out info))
		{
			loader = info.loader;
		}
		// UnloadAssetBundle is safer than doing an AssetBundles.Remove(assetBundleName)
		this.UnloadAssetBundle(assetBundleName);
		// the old cancel logic had the cancelDownload being called AFTER it's been removed from AssetBundles,
		// since we know that worked, keeping the old behavior
		if (loader != null)
		{
			loader.CancelDownload();	
		}
	}
	
	/// <summary>
	/// Gets the version number component (major, minor or revision).
	/// </summary>
	/// <returns>
	/// The version number component.
	/// </returns>
	/// <param name='digit'>
	/// 0 to return major, 1 to return minor, 2 to return revision.
	/// </param>
	public static int GetVersionNumberComponent(int digit)	// minor)
	{
		//string[] currentVersion = GetVersionString().Split('.');	
		string[] currentVersion = BundleInfo.GetBundleVersion().Split('.');	
		return System.Int32.Parse(currentVersion[digit]);
	}
}






//		if (!minor)
//		{
//			return System.Int32.Parse(currentVersion[0]);
//		}
//		else
//		{
//			return System.Int32.Parse(currentVersion[1]);
//		}
