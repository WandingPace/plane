using System;
using System.Collections.Generic;
using System.IO;
using MiniJSON;
using UnityEngine;

public class AssetBundleCompatibilityManager : MonoBehaviour
{
    protected static Notify notify = new Notify("AssetBundleCompatibilityManager");
    public static List<AssetBundleCompatibilityEntry> Entries = new List<AssetBundleCompatibilityEntry>();

    public static bool LoadFile()
    {
        var fileName = "OZGameData/AssetBundleCompatibility";
        var entryText = Resources.Load(fileName) as TextAsset;

        if (entryText == null)
        {
            notify.Warning("No AssetBundleCompatibility.txt exists at: " + fileName);
            return false;
        }

        if (Entries == null)
        {
            Entries = new List<AssetBundleCompatibilityEntry>();
        }
        else
        {
            Entries.Clear();
        }

        var loadedData = Json.Deserialize(entryText.text) as Dictionary<string, object>;

        if (SaveLoad.Load(loadedData) == false)
        {
#if !UNITY_EDITOR
			return false;
#endif
        }

        var entries = loadedData["data"] as List<object>;
        if (entries == null)
        {
            return false;
        }

        foreach (var dict in entries)
        {
            var data = dict as Dictionary<string, object>;
            var entry = new AssetBundleCompatibilityEntry();
            entry.SetDataFromDictionary(data);
            Entries.Add(entry);
        }
        return false;
    }

    public static void SaveFile()
    {
        var fileName = Application.dataPath + Path.DirectorySeparatorChar
                       + "Resources" + Path.DirectorySeparatorChar + "OZGameData/AssetBundleCompatibility.txt";

        var list = new List<object>();

        foreach (var entry in Entries)
        {
            list.Add(entry.ToDict());
        }

        var secureData = SaveLoad.Save(list);
        var listString = Json.Serialize(secureData);

        try
        {
            using (var fileWriter = File.CreateText(fileName))
            {
                fileWriter.WriteLine(listString);
                fileWriter.Close();
            }
        }
        catch (Exception e)
        {
            notify.Warning("AssetBundleCompatibilityEntry Save Exception: " + e);
        }
    }

    public static AssetBundleCompatibilityEntry GetAssetBundleCompatibilityEntry(string clientVersion,
        string assetBundleName)
    {
        if (Entries == null)
        {
            return null;
        }

        foreach (var entry in Entries)
        {
            if (entry == null)
            {
                continue;
            }

            if (entry.clientVersion == clientVersion && entry.assetBundleName == assetBundleName)
            {
                return entry;
            }
        }
        return null;
    }
}


//#if UNITY_EDITOR
//	public static int GetNextAssetBundleCompatibilityEntryID()
//	{
//		int nextID = 0;
//		foreach (AssetBundleCompatibilityEntry entry in Entries)
//		{
//			if (entry == null) { continue; }
//			nextID = entry.id + 1;
//		}
//		return nextID;
//	}
//#endif


//	public static AssetBundleCompatibilityEntry AssetBundleCompatibilityEntryFromID(int gameTipID)
//	{
//		if (AssetBundleCompatibilityEntryManager.Entries == null) { return null; }
//		
//		foreach (AssetBundleCompatibilityEntry entry in AssetBundleCompatibilityEntryManager.Entries)
//		{
//			if (entry == null || entry.id != gameTipID) { continue; }
//			return entry;
//		}
//		return null;
//	}

//	public static AssetBundleCompatibilityEntry GetRandomAssetBundleCompatibilityEntry()
//	{
//		if (AssetBundleCompatibilityEntryManager.Entries == null || AssetBundleCompatibilityEntryManager.Entries.Count == 0 ) { 
//			return null; }
//		
//		int randomIndex = UnityEngine.Random.Range(0, AssetBundleCompatibilityEntryManager.Entries.Count);
//		return Entries[randomIndex];
//	}