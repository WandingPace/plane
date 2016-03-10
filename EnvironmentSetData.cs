using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

/// <summary>
///     All the data needed for one environment set
///     all public members will be saved, key for json is the member name
/// </summary>
public class EnvironmentSetData
{
    protected static Notify notify = new Notify("EnvironmentSetData");

    /// <summary>
    ///     what distance do we fly in the clouds
    /// </summary>
    public float BalloonMinLength = 10000;

    /// <summary>
    ///     贴图淡出材质球路径
    /// </summary>
    public string DecalFadeOutMaterialPath = "";

    /// <summary>
    ///     贴图材质球路径
    /// </summary>
    public string DecalMaterialPath = "";

    /// <summary>
    ///     到下一个环境的距离(新增)
    /// </summary>
    public float DistanceToNextEnv = 1000f;

    /// <summary>
    ///     环境对应障碍孵化池
    /// </summary>
    public string EnemyPoolPath = "";

    public float Env0MaxLength = 350;
    //Env 0 distances
    public float Env0MinLength = 200;

    /// <summary>
    ///     hen we go over this distance, we'll force the env1 end piece
    /// </summary>
    public float Env1MaxLength = 350;

    /// <summary>
    ///     Used to be called ForestMinLength, minimum distance you'll run in Env 1
    /// </summary>
    public float Env1MinLength = 250;

    /// <summary>
    ///     When we go over this distance, we'll force the env2 end piece
    /// </summary>
    public float Env2MaxLength = 220;

    /// <summary>
    ///     Used to be called MineMinLength, minimum distance you'll run in Env 2
    /// </summary>
    public float Env2MinLength = 120;

    /// <summary>
    ///     extra1_fade材质球路径
    /// </summary>
    public string Extra1FadeMaterialPath = "";

    /// <summary>
    ///     extra1材质球路径
    /// </summary>
    public string Extra1MaterialPath = "";

    /// <summary>
    ///     extra2_fade材质球路径
    /// </summary>
    public string Extra2FadeMaterialPath = "";

    /// <summary>
    ///     extra2材质球路径
    /// </summary>
    public string Extra2MaterialPath = "";

    /// <summary>
    ///     远处场景应用淡出的材质球路径
    /// </summary>
    public string FadeOutMaterialPath = "";

    public string FailPostBalloonPiece = "";
    public int hardSurfaceEnv0;
    public int hardSurfaceEnv1;
    public int hardSurfaceEnv2;
    private string localizedTitle = "";

    /// <summary>
    ///     环境对应音乐文件
    /// </summary>
    public string MusicFile = "";

    /// <summary>
    ///     The number of post ballon fail pieces. really long fail pieces can bring this number down, ybr is at 2
    /// </summary>
    public int NumberOfPostBallonFailPieces = 4;

    /// <summary>
    ///     近处场景应用的材质球
    /// </summary>
    public string OpaqueMaterialPath = "";

    /// <summary>
    ///     环境对应开场预制路径
    /// </summary>
    public string OpeningTilePrefabPath = "";

    /// <summary>
    ///     环境名缩写, e.g. ww, ybr df
    /// </summary>
    public string SetCode = "";

    /// <summary>
    ///     环境设置标识符, e.g. 0 for machu, 1 for ww, 2 for df, 3 for ybr
    /// </summary>
    public int SetId = -1;

    /// <summary>
    ///     环境对应天空盒路径
    /// </summary>
    public string SkyboxPrefabPath = "";

    /// <summary>
    /// 雾化颜色
    /// </summary>
    public int FogColorR = 255;
    public int FogColorG = 255;
    public int FogColorB = 255;

    /// <summary>
    /// 雾化密度
    /// </summary>
    public float FogDensity = 0.02f;

    /// <summary>
    ///     环境名
    /// </summary>
    public string Title = "";

    public float Env9MinLength
    {
        get { return BalloonMinLength; }
    }

    /// <summary>
    ///     If we are this far from the tunnel entrance, don't allow new environment sets
    /// </summary>
    public float TunnelBufferDistance { get; private set; }

    // TODO move out PopuplateWWPieces, PopulateYBRPieces, PopulateDFPieces, and put the data to create them here

    /// <summary>
    ///     Sets the data from a dictionary. the dictionary is probably coming from a json object
    /// </summary>
    /// <param name='data'>
    ///     the dictionary.
    /// </param>
    public void SetDataFromDictionary(Dictionary<string, object> data)
    {
        if (data == null)
        {
            notify.Warning("EnvironmentSetData.SetDataFromDictionary dictionary is null");
            return;
        }

        SerializationUtils.SetDataFromDictionary(this, data);
        TunnelBufferDistance = ComputeTunnelBufferDistance();
    }

    /// <summary>
    ///     returns a dictionary representation of this object
    /// </summary>
    /// <returns>
    ///     The dict.
    /// </returns>
    public Dictionary<string, object> ToDict()
    {
        return SerializationUtils.ToDict(this);
    }

    /// <summary>
    ///     Stub for when localization actually works
    /// </summary>
    public string GetLocalizedTitle()
    {
        return localizedTitle; //Localization.SharedInstance.Get(Title);
    }

    private void SetLocalizedTitle()
    {
        localizedTitle = Localization.SharedInstance.Get(Title);
    }

    public void OnLanguageChanged(string language)
    {
        SetLocalizedTitle(); // = Localization.SharedInstance.Get(newData.Title);
    }

    /// <summary>
    ///     Validates the values, cross references with bootstrap info, checks resources are theres
    /// </summary>
    /// <returns>
    ///     true if all good, false otherwise, console log will have more info on the errors
    /// </returns>
    public bool ValidateValues()
    {
        // normally I hate returns in the middle of a function,  but this makes a little more
        // sense given the amount of checks we make
        if (SetId < 0)
        {
            notify.Warning("SetId must be 0 or greater");
            return false;
        }

        if (SetCode == "")
        {
            notify.Warning("SetCode must not be blank");
            return false;
        }

        /// try to load bootstrap info and verify the setcode is there as well
        var bootStrapLoaded = EnvironmentSetBootstrap.LoadFile();
        if (!bootStrapLoaded)
        {
            notify.Warning("Could not load the EnvironmentSet Bootstrap data file");
            return false;
        }

        // Todo there should be a way to do the for loop with .Contains()
        var found = false;
        foreach (var tempBootstrap in EnvironmentSetBootstrap.BootstrapList)
        {
            if (tempBootstrap.SetCode == SetCode)
            {
                found = true;
                break;
            }
        }
        if (!found)
        {
            notify.Warning("The Environment Set Bootstrap data file does not contain the SetCode " + SetCode);
            return false;
        }

        if (Title == "")
        {
            localizedTitle = "<not set>";
            notify.Warning("Title must not be blank");
            return false;
        }

        // because we added sufffixes at runtime to these paths, this check is no longer valid
        //string []  paths= { OpaqueMaterialPath, FadeOutMaterialPath, DecalMaterialPath};
        string[] paths = {};

        foreach (var path in paths)
        {
            if (path == "")
            {
                notify.Warning("path must not be blank");
                return false;
            }

            var testMat = Resources.Load(path) as Material;
            if (testMat == null)
            {
                notify.Warning("could not load the material at " + path);
                return false;
            }
        }

        var prefab = Resources.Load(SkyboxPrefabPath) as GameObject;
        if (prefab == null)
        {
            notify.Warning("could not load skybox prefab at " + SkyboxPrefabPath);
            return false;
        }

        prefab = Resources.Load(OpeningTilePrefabPath) as GameObject;
        if (prefab == null)
        {
            notify.Warning("could not opening tile prefab at " + OpeningTilePrefabPath);
            return false;
        }

        return true;
    }

    /// <summary>
    ///     Saves our info in the appropriate place, the filename is the SetCode followed by .txt
    /// </summary>
    /// <returns>
    ///     true if the file was saved successfully
    /// </returns>
    public bool SaveFile()
    {
        var result = true;
        var fullPath = Application.dataPath + Path.DirectorySeparatorChar + "Resources" + Path.DirectorySeparatorChar +
                       EnvironmentSetBootstrap.EnvironmentSetResourcesDirectory + Path.DirectorySeparatorChar + SetCode +
                       ".txt";
        notify.Info("saving to " + fullPath);
        var listString = SerializationUtils.ToJson(this);
        try
        {
            using (var fileWriter = File.CreateText(fullPath))
            {
                fileWriter.WriteLine(listString);
                fileWriter.Close();
            }
        }
        catch (Exception e)
        {
            result = false;
            notify.Warning("Save Exception: " + e);
        }
        return result;
    }

    /// <summary>
    ///     Tries to load the environment set data from a file.
    /// </summary>
    /// <returns>
    ///     true if we loaded successfully
    /// </returns>
    /// <param name='setCode'>
    ///     the set code which would correspond to the file name as well
    /// </param>
    /// <param name='loadedObject'>
    ///     will be null if there was an error,  otherwise should contain valid data
    /// </param>
    public static bool LoadFile(string setCode, out EnvironmentSetData loadedObject, bool validate = false)
    {
        var result = true;
        loadedObject = null;
        var newData = new EnvironmentSetData();
        try
        {
            var resourcePath = EnvironmentSetBootstrap.EnvironmentSetResourcesDirectory + "/" + setCode;
            notify.Debug("loading " + resourcePath);
            var jsonText = Resources.Load(resourcePath) as TextAsset;
            notify.Debug("EnvironmentSetData " + jsonText.text);

            var deserialized = SerializationUtils.FromJson(newData, jsonText.text);
            if (!deserialized)
            {
                notify.Warning("EnvironmentSetData.LoadFile, could not parse {0}", jsonText.text);
            }
            if (validate)
            {
                var valid = newData.ValidateValues();
                result = valid;
            }
            Localization.RegisterForOnLanguageChanged(newData.OnLanguageChanged);
            newData.SetLocalizedTitle();
            loadedObject = newData;
        }
        catch (Exception e)
        {
            result = false;
            notify.Error("Load Exception: " + e);
        }
        return result;
    }

    /// <summary>
    ///     If we are this far from the tunnel entrance, don't allow new environments to spawn
    /// </summary>
    /// <returns>
    ///     The tunnel buffer distance.
    /// </returns>
    public float ComputeTunnelBufferDistance()
    {
        var bufferDistance = 50f;

        bufferDistance = Mathf.Max(bufferDistance, Env0MaxLength);
        bufferDistance = Mathf.Max(bufferDistance, Env1MaxLength);
        bufferDistance = Mathf.Max(bufferDistance, Env2MaxLength);
        // then add just a little bit more to be safe
        bufferDistance += 50f;
        return bufferDistance;
    }

    /// <summary>
    ///     If true, the environment set is an assetbundle saved under Assets/StreamingAssets
    ///     if false, then the environment set could be under Resources/Prefabs/Temple/Environments
    ///     or a real asset bundle that we download
    /// </summary>
    public bool IsEmbeddedAssetBundle()
    {
        var result = false;
        foreach (var tempBootstrap in EnvironmentSetBootstrap.BootstrapList)
        {
            if (tempBootstrap.SetCode == SetCode)
            {
                result = tempBootstrap.EmbeddedAssetBundle;
                break;
            }
        }
        return result;
    }

    /// <summary>
    ///     If true, the environment set prefabs are saved under Resources/Prefabs/Temple/Environments
    ///     if false, then the environment set could be an assetbundle under Assets/StreamingAssets
    ///     or a real asset bundle that we download
    /// </summary>
    public bool IsEmbeddedAsResource()
    {
        var result = false;
        foreach (var tempBootstrap in EnvironmentSetBootstrap.BootstrapList)
        {
            if (tempBootstrap.SetCode == SetCode)
            {
                result = tempBootstrap.Embedded && !tempBootstrap.EmbeddedAssetBundle;
                break;
            }
        }
        return result;
    }
}