using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MiniJSON;
using UnityEngine;

/// <summary>
///     Read in the environment set index file to determine what environment sets are available
/// </summary>
public class EnvironmentSetBootstrap
{
    public const string EnvironmentSetResourcesDirectory = "OZGameData/EnvironmentSet";
    protected static Notify notify = new Notify("EnvironmentSetBootstrap");
    private static List<EnvironmentSetBootstrapData> bootstrapList = new List<EnvironmentSetBootstrapData>();

    public static List<EnvironmentSetBootstrapData> BootstrapList
    {
        get { return bootstrapList; }

        set { bootstrapList = value; }
    }

    /// <summary>
    ///     Gets the name of the bootstrap data by the bundle name, may return null
    /// </summary>
    public static EnvironmentSetBootstrapData GetBootstrapDataByBundleName(string bundleName)
    {
        foreach (var data in bootstrapList)
        {
            if (data.AssetBundleName == bundleName)
            {
                return data;
            }
        }
        return null;
    }

    /// <summary>
    ///     Validates our bootstrap list
    /// </summary>
    /// <returns>
    ///     true if all data is good, false otherwise, say two of them have the same code
    /// </returns>
    public static bool ValidateValues()
    {
        var result = true;

        // currently our bootstrap data is just all string codes, so this simple check will work.
        if (bootstrapList.Distinct(new EnvironmentSetBootstrapDataComparer()).Count() != bootstrapList.Count)
        {
            result = false;
        }
        return result;
    }

    /// <summary>
    ///     Loads the bootstrap list from the file.
    /// </summary>
    /// <returns>
    ///     true if we loaded successfully
    /// </returns>
    public static bool LoadFile()
    {
        var result = true;
        try
        {
            bootstrapList.Clear();

            var jsonText = Resources.Load(EnvironmentSetResourcesDirectory + "/EnvironmentSetIndex") as TextAsset;
            if (Application.isPlaying)
            {
                notify.Debug("EnvironmentSetBootstrap " + jsonText.text);
            }
            var loadedData = Json.Deserialize(jsonText.text) as Dictionary<string, object>; //-- Security check
            var myList = loadedData["data"] as List<object>;

            foreach (var dict in myList)
            {
                var data = dict as Dictionary<string, object>;
                var newBootStrapData = new EnvironmentSetBootstrapData(data);
                if (Application.isPlaying)
                {
                    notify.Debug("EnvironmentSetBootstrap adding <" + newBootStrapData.AssetBundleName +
                                 "> data to bootstrapList");
                }
                bootstrapList.Add(newBootStrapData);
            }
        }
        catch (Exception e)
        {
            result = false;
            if (Application.isPlaying)
            {
                notify.Error("Load Exception: " + e);
            }
            else
            {
                // rare case we use Debug.LogError, being accessed by the build script
                Debug.LogError("Load Exception: " + e);
            }
        }
        return result;
    }

    /// <summary>
    ///     Saves our bootstrap list to the files
    /// </summary>
    /// <returns>
    ///     true if we saved successfully, false otherwise
    /// </returns>
    public static bool SaveFile()
    {
        var result = true;
        var fileName = Application.dataPath + Path.DirectorySeparatorChar + "Resources" + Path.DirectorySeparatorChar +
                       EnvironmentSetResourcesDirectory + Path.DirectorySeparatorChar + "EnvironmentSetIndex.txt";
        var list = new List<object>();
        foreach (var  data in bootstrapList)
        {
            list.Add(data.ToDict());
        }

        //-- Hash before we save.
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
            result = false;
            notify.Error("Save Exception: " + e);
        }
        return result;
    }
}