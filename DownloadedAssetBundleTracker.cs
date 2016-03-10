using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System;

/// <summary>
/// Basic unit of info we keep track of
/// </summary>
public class DownloadedAssetBundleInfo
{
	public string name = "";
	public int version = -1;
	public int latestVersionNumber = -1;
	
	public override string ToString()
	{
		string result = SerializationUtils.ToJson(this);	
		return result;
	}
}

/// <summary>
/// Keep track of which asset bundles have been downloaded to the device, and the version
/// </summary>
public class DownloadedAssetBundleTracker
{
	protected static Notify notify;
	
	static DownloadedAssetBundleTracker()
	{
		notify = new Notify("DownloadedAssetBundleTracker");
	}
	public List<DownloadedAssetBundleInfo> DownloadedList = new List<DownloadedAssetBundleInfo>();
	
	const string SaveFilePath = "DownloadedAssetBundles";
	private static string GetFullSaveFilePath() { return Application.persistentDataPath + Path.DirectorySeparatorChar + SaveFilePath; }
	
	/// <summary>
	/// Loads the list of downloaded asset bundles, if any
	/// </summary>
	public void Load()
	{
		string fileName = GetFullSaveFilePath();
		notify.Debug("DownloadedAssetBundleTracker.Load " + fileName);
		// if the file doesn't exist we still have an empty list which reflects nothing downloaded
		if (File.Exists(fileName))
		{
			using (StreamReader reader = File.OpenText(fileName))
			{
				string jsonString = reader.ReadToEnd();
				SerializationUtils.FromJson(this, jsonString);
			}
			
			// kill this loop in release
			foreach ( DownloadedAssetBundleInfo info in DownloadedList)
			{
				notify.Debug("DownloadedAssetBundleTracker.Load got {0}", info.name);
			}
			
		}
	}
	
	/// <summary>
	/// Save our list of downloaded asset bundles
	/// </summary>
	public void Save()
	{
		string fileName = GetFullSaveFilePath();
		string jsonString = SerializationUtils.ToJson( this);	
		try 
		{
			using (StreamWriter fileWriter = File.CreateText(fileName)) 
			{
				fileWriter.WriteLine(jsonString);
				fileWriter.Close(); 
			}
			notify.Debug("success saving to " + fileName);
		}
		catch (Exception e) 
		{
			notify.Warning("Save Exception: " + e);
		}		
	}
	
	/// <summary>
	/// clear out all the information of which ones we've downloaded
	/// </summary>
	public static void Reset()
	{
		string filePath = GetFullSaveFilePath();
		if (File.Exists(filePath))
		{	
			File.Delete(filePath);
		}
	}

	public DownloadedAssetBundleInfo FindInfo(string assetBundleName)
	{
		foreach (DownloadedAssetBundleInfo info in DownloadedList)
		{
			if (info.name == assetBundleName)
			{
				return info;
			}
		}
		return null;
	}
	
	/// <summary>
	/// Updates DownloadedList if the expected files are NOT on disk
	/// needed when we go from 1.0 to 1.2 and we are in airplane mode
	/// </summary>
	public void VerifyDownloadedFilesArePresent()
	{
		notify.Debug("VerifyDownloadedFilesArePresent");
		foreach (DownloadedAssetBundleInfo info in DownloadedList)
		{
			if ( ! AssetBundleLoader.IsCompressed(info.name))
			{
				// if it's not compressed we save this to disk	
				string diskPath = AssetBundleLoader.GetAssetBundleDiskPath(info.name, info.version);
				notify.Debug("diskPath="+diskPath);
				if ( ! File.Exists( diskPath))
				{
					// we must have upgraded, let's set version to int.MinValue so it gets flagged as NOT downloaded
					info.version = int.MinValue;
					notify.Debug( "{0} does NOT exist. setting info.version to {1}", diskPath, info.version);
				}
				else
				{
					notify.Debug(diskPath +   " file  exists");
				}
			}
		}
	}
	
	/// <summary>
	/// Forces the hi res asset bundle to be not present, for trying to fix black screen bug
	/// </summary>
	public void ForceHiResNotPresent()
	{
		bool found = false;
		foreach (DownloadedAssetBundleInfo info in DownloadedList)
		{
			if ( info.name == DownloadManagerUI.bundleName)
			{
				found = true;
				info.version = int.MinValue;
				break;
			}
		}	
		if (found)
		{
			// dave got a case where HD automatically updated when you force quit the app, save it to disk!
			Save();
		}
	}
	
	public bool SetNewVersionAvaiable(string assetBundleName, int newversion, bool save)
	{
		DownloadedAssetBundleInfo info = FindInfo(assetBundleName);
		if( info  != null)
		{
			if( info.latestVersionNumber < newversion )
			{
				info.latestVersionNumber = newversion;
				notify.Debug(" ENV:" + info.name + " Ver set to " + newversion);
				if( save )
					Save ();
				return true;
			}
		}
		return false;
	}
	
	public int GetLatestVersionNumber(string assetBundleName)
	{
		DownloadedAssetBundleInfo info = FindInfo(assetBundleName);
		if( info != null)
		{
			notify.Debug (assetBundleName + " ver = " + info.version + " latest " + info.latestVersionNumber);
			return info.latestVersionNumber;
		}
//		Debug.LogError("Asset not listed " + assetBundleName);
		return -1;
	}
	
	public bool IsAssetBundleDownloadedLastestVersion( string assetBundleName)
	{
		DownloadedAssetBundleInfo info = FindInfo(assetBundleName);
		if( info != null)
		{
			notify.Debug(assetBundleName + " ver = " + info.version + " need " + info.latestVersionNumber);
			return (info.version >= info.latestVersionNumber);
		}
		return false;
	}
	
		
	/// <summary>
	/// Returns true if the assetBundle has been downloaded. 
	/// </summary>
	public bool IsAssetBundleDownloaded( string assetBundleName, int version = (int.MinValue +1))
	{
		DownloadedAssetBundleInfo info = FindInfo(assetBundleName);
		if( info != null)
		{
//				Debug.LogError (assetBundleName + " ver = " + info.version + " need " + version);
			notify.Debug ("IsAssetBundleDownloaded {0} info.version={1}  version={2}", assetBundleName, info.version, version);
			return (info.version >= version);
		}
		return false;
	}
	
	/// <summary>
	/// Update our list that the assetBundleName has been downloaded and at the given version
	/// </summary>
	/// <param name='assetBundleName'>
	/// Asset bundle name.
	/// </param>
	/// <param name='version'>
	/// Version.
	/// </param>
	public void UpdateAssetBundleDownload( string assetBundleName, int version)
	{
		bool found = false;
		foreach (DownloadedAssetBundleInfo info in DownloadedList)
		{
			if (info.name == assetBundleName)
			{
				info.version = version;
				found = true;
				break;
			}
		}		
		
		if ( ! found)
		{
			notify.Debug ( assetBundleName + " First time downloaded ver = " + version);
			DownloadedAssetBundleInfo newInfo =  new DownloadedAssetBundleInfo();
			newInfo.name = assetBundleName;
			newInfo.version = version;
			newInfo.latestVersionNumber = version;
			DownloadedList.Add(newInfo);
		}
		Save ();
	}
	/// <summary>
	/// tests our serialization
	/// </summary>
	public static void UnitTest()
	{
		DownloadedAssetBundleTracker testTracker = new DownloadedAssetBundleTracker();
		
		DownloadedAssetBundleInfo testInfo = new DownloadedAssetBundleInfo();
		testInfo.name = "darkforest";
		testInfo.version = 3;
		
		testTracker.DownloadedList.Add(testInfo);
		
		DownloadedAssetBundleInfo testInfo2 = new DownloadedAssetBundleInfo();
		testInfo2.name = "yellowbrickroad";
		testInfo2.version = 6;
		
		testTracker.DownloadedList.Add(testInfo2);
		
		string json = SerializationUtils.ToJson(testTracker);
		notify.Debug("json text = " + json);
		
		DownloadedAssetBundleTracker destTracker = new DownloadedAssetBundleTracker();
		SerializationUtils.FromJson(destTracker, json);
		
		string json2 = SerializationUtils.ToJson(destTracker);
		notify.Debug ("json2 = " + json2);
		
		notify.Assert(json2 == json, "error in serialization");
	}
}
