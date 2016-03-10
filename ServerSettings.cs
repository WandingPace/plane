using System;
using System.Collections.Generic;
using System.IO;
using MiniJSON;
using UnityEngine;

public class ServerSettings : MonoBehaviour
{
    private static readonly Dictionary<string, bool> _boolDictionary = new Dictionary<string, bool>();
    private static readonly Dictionary<string, int> _intDictionary = new Dictionary<string, int>();
    private static readonly Dictionary<string, float> _floatDictionary = new Dictionary<string, float>();
    private static readonly Dictionary<string, string> _stringDictionary = new Dictionary<string, string>();
    protected static Notify notify;

    /// <summary>
    ///     Apply the server settings to what the rest of the game normally accesses, e.g. Settings.GetInt("key", 1);
    /// </summary>
    private void _applyToSettings()
    {
        /// note that some of these may not take if we have keys that trump it in local-settings.txt
        foreach (var pair in _boolDictionary)
        {
            Settings.SetBool(pair.Key, pair.Value, Settings.SourcePriority.GameServer);
        }
        foreach (var pair in _intDictionary)
        {
            Settings.SetInt(pair.Key, pair.Value, Settings.SourcePriority.GameServer);
        }
        foreach (var pair in _floatDictionary)
        {
            Settings.SetFloat(pair.Key, pair.Value, Settings.SourcePriority.GameServer);
        }
        foreach (var pair in _stringDictionary)
        {
            Settings.SetString(pair.Key, pair.Value, Settings.SourcePriority.GameServer);
        }
    }

    private void _loadLocalCopyServerSettings()
    {
        var fileName = Application.persistentDataPath + Path.DirectorySeparatorChar + "server-settings.txt";

        if (File.Exists(fileName) == false)
        {
            notify.Debug("[ServerSettings] - _loadLocalCopyServerSettings: 'server-settings.txt' not found.");
            return;
        }

        var fileReader = File.OpenText(fileName);
        var settingsJsonString = fileReader.ReadToEnd();
        fileReader.Close();

        notify.Debug("[ServerSettings] - _loadLocalCopyServerSettings: attempting to deserialize.");

        try
        {
            var loadedDataDictionary
                = Json.Deserialize(settingsJsonString) as Dictionary<string, object>;

            if (loadedDataDictionary != null)
            {
                if (SaveLoad.Load(loadedDataDictionary) == false)
                {
                    return;
                }

                var dataDictionary = loadedDataDictionary["data"]
                    as Dictionary<string, object>;

                if (dataDictionary == null)
                {
                    return;
                }

                var boolSettingDictionary = dataDictionary["boolSettings"]
                    as Dictionary<string, object>;

                var intSettingsDictionary = dataDictionary["intSettings"]
                    as Dictionary<string, object>;

                var floatSettingsDictionary = dataDictionary["floatSettings"]
                    as Dictionary<string, object>;

                var stringSettingsDictionary = dataDictionary["stringSettings"]
                    as Dictionary<string, object>;

                if (boolSettingDictionary != null)
                {
                    _boolDictionary.Clear();

                    foreach (var boolKVP in boolSettingDictionary)
                    {
                        _boolDictionary.Add(boolKVP.Key, bool.Parse(boolKVP.Value.ToString()));
                    }
                }

                if (intSettingsDictionary != null)
                {
                    _intDictionary.Clear();

                    foreach (var intKVP in intSettingsDictionary)
                    {
                        _intDictionary.Add(intKVP.Key, int.Parse(intKVP.Value.ToString()));
                    }
                }

                if (floatSettingsDictionary != null)
                {
                    _floatDictionary.Clear();

                    foreach (var floatKVP in floatSettingsDictionary)
                    {
                        _floatDictionary.Add(floatKVP.Key, float.Parse(floatKVP.Value.ToString()));
                    }
                }

                if (stringSettingsDictionary != null)
                {
                    _stringDictionary.Clear();

                    foreach (var stringKVP in stringSettingsDictionary)
                    {
                        _stringDictionary.Add(stringKVP.Key, stringKVP.ToString());
                    }
                }
            }
        }
        catch (Exception ex)
        {
            notify.Warning("[ServerSettings] - _loadLocalCopyServerSettings: Error Deserializing - " + ex.Message);
        }
    }

    public void ApplyServerSettingsLocalCopy()
    {
        _loadLocalCopyServerSettings();
        _applyToSettings();
    }

    private void _saveLocalCopyServerSettings()
    {
        using (var stream = new MemoryStream())
        {
            var fileName = Application.persistentDataPath + Path.DirectorySeparatorChar + "server-settings.txt";

            var settingsDataDictionary = new Dictionary<string, object>();

            settingsDataDictionary.Add("boolSettings", _boolDictionary);
            settingsDataDictionary.Add("intSettings", _intDictionary);
            settingsDataDictionary.Add("floatSettings", _floatDictionary);
            settingsDataDictionary.Add("stringSettings", _stringDictionary);

            var secureDataDictionary = SaveLoad.Save(settingsDataDictionary);

            var settingsJsonString = Json.Serialize(secureDataDictionary);

            try
            {
                using (var fileWriter = File.CreateText(fileName))
                {
                    fileWriter.WriteLine(settingsJsonString);
                    fileWriter.Close();
                }
            }
            catch (Exception ex)
            {
                notify.Error("[ServerSettings] - _saveLocalCopyServerSettings: Error Saving " + ex.Message);
            }
        }
    }

    // Use this for initialization
    private void Start()
    {
        notify = new Notify(GetType().Name);
    }

    /// <summary>
    ///     Apply server settings parameters to internal dictionaries.
    /// </summary>
    /// <param name='serverSettingsList'>
    ///     Server settings list.
    /// </param>
    /// <param name='responseCode'>
    ///     Web response code.
    /// </param>
    public void ApplyServerSettings(List<object> serverSettingsList, int responseCode)
    {
        if (responseCode == 200)
        {
            notify.Debug("[ServerSettings] - ApplyServerSettings: Got Response");

            // Clear Dictionaries in memory.
            _boolDictionary.Clear();
            _intDictionary.Clear();
            _floatDictionary.Clear();
            _stringDictionary.Clear();

            if (serverSettingsList != null && serverSettingsList.Count > 0)
            {
                notify.Debug("[ServerSettings] - ApplyServerSettings: Settings List not null");

                foreach (var settingsObject in serverSettingsList)
                {
                    notify.Debug("[ServerSettings] - ApplyServerSettings: Looping through objects");

                    try
                    {
                        var settingsDictionary = settingsObject as Dictionary<string, object>;

                        notify.Debug("[ServerSettings] - ApplyServerSettings: Attempting to Deserialize");

                        if (settingsDictionary != null
                            && settingsDictionary.ContainsKey("type")
                            && settingsDictionary.ContainsKey("name")
                            && settingsDictionary.ContainsKey("value")
                            )
                        {
                            var settingType = settingsDictionary["type"].ToString();
                            var settingName = settingsDictionary["name"].ToString();

                            switch (settingType)
                            {
                                case "Boolean":
                                    var boolValue = bool.Parse(settingsDictionary["value"].ToString());
                                    _boolDictionary.Add(settingName, boolValue);
                                    break;

                                case "Int":
                                    var intValue = int.Parse(settingsDictionary["value"].ToString());
                                    _intDictionary.Add(settingName, intValue);
                                    break;

                                case "Float":
                                    var floatValue = float.Parse(settingsDictionary["value"].ToString());
                                    _floatDictionary.Add(settingName, floatValue);
                                    break;

                                case "String":
                                    _stringDictionary.Add(settingName, settingsDictionary["value"].ToString());
                                    break;
                            }

                            notify.Debug("[ServerSettings] ApplyServerSettings - Setting '" + settingName
                                         + "' with value: " + settingsDictionary["value"]);
                        }
                    }
                    catch (Exception ex)
                    {
                        notify.Error("[ServerSettings] ApplyServerSettings - Deserializing Error: " + ex.Message);
                    }
                }

                _saveLocalCopyServerSettings();
                _applyToSettings();
            }
        }
    }
}