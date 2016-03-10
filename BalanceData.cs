using System;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[Serializable]
public class BalanceParameter
{
    public string Identifier = "";
    public float Value;
    public bool BoolValue = true;
    public bool IsBool;
    public bool IsEnabled = true;

    public BalanceParameter(string id, float val, bool isbool)
    {
        Identifier = id;
        if (isbool) BoolValue = val != 0f;
        else Value = val;
        IsBool = isbool;
    }
}

[Serializable]
public class BalanceState
{
    public float Distance = 0f;

    public List<string> Commands = new List<string>();

    //TODO: Make a serializeable string-dictionary!
    [SerializeField] 
    public List<string> StateParamKeys = new List<string>();
    [SerializeField] 
    public List<BalanceParameter> StateParamValues = new List<BalanceParameter>();

    public BalanceParameter this[string key]
    {
        get
        {
            var ind = StateParamKeys.IndexOf(key);
            if (ind != -1)
                return StateParamValues[ind];
            Debug.LogWarning(key + " not found!!");
            return null;
        }
        set
        {
            if (StateParamKeys.Contains(key))
                StateParamValues[StateParamKeys.IndexOf(key)] = value;
        }
    }

    public bool ContainsKey(string s)
    {
        return StateParamKeys.Contains(s);
    }

    public int Count
    {
        get { return StateParamKeys.Count; }
    }

    public void Add(string key, BalanceParameter bp)
    {
        if (StateParamKeys.Contains(key)) return;

        StateParamKeys.Add(key);
        StateParamValues.Add(bp);
    }

    public void Insert(string key, BalanceParameter bp, int index)
    {
        if (StateParamKeys.Contains(key)) return;

        StateParamKeys.Insert(index, key);
        StateParamValues.Insert(index, bp);
    }

    public void Remove(string key)
    {
        if (!StateParamKeys.Contains(key)) return;

        var index = StateParamKeys.IndexOf(key);
        StateParamKeys.RemoveAt(index);
        StateParamValues.RemoveAt(index);
    }
}


public class BalanceData : MonoBehaviour
{
    private static BalanceData _main;

    private static BalanceData main
    {
        get
        {
            if (_main == null)
            {
                var prf = (GameObject) Resources.Load("OzGameData/BalanceData");
                _main = prf.GetComponent<BalanceData>();
            }
            return _main;
        }
    }

    private static BalanceData _current;
    private static string _currentCode;

    public static BalanceData Current
    {
        get { return GetDataForEnvSet(EnvironmentSetManager.SharedInstance.CurrentEnvironmentSet.SetCode); }
    }

    public static BalanceData _GetMainUnsafe
    {
        get { return main; }
    }

    public static BalanceData GetDataForEnvSet(string envSet)
    {
        if (_current == null || _currentCode != envSet)
        {
            _currentCode = envSet;
            var prf = (GameObject) Resources.Load("OzGameData/BalanceData_" + envSet);

#if UNITY_EDITOR
            if (prf == null && !Application.isPlaying && !string.IsNullOrEmpty(envSet))
            {
                prf = new GameObject("BalanceData_" + envSet);
                var oldPrf = prf;
                /*BalanceData dt = */
                prf.AddComponent<BalanceData>();

                prf.GetComponent<BalanceData>().BalanceSheetPrototype = main.BalanceSheetPrototype;
                prf.GetComponent<BalanceData>().BalanceInfo = new List<BalanceState>(main.BalanceInfo);

                EditorUtility.SetDirty(prf);

                PrefabUtility.CreatePrefab("Assets/Resources/OzGameData/BalanceData_" + envSet + ".prefab", prf);

                AssetDatabase.Refresh();

                DestroyImmediate(oldPrf);
            }
#endif

            if (prf != null) 
                _current = prf.GetComponent<BalanceData>();
            else 
                _current = main;
        }
        return _current;
    }

    // -- Balancing info - Use the "BalanceSheet" window to tweak this
    [HideInInspector] 
    public List<BalanceState> BalanceInfo = new List<BalanceState>();

    [HideInInspector] 
    public BalanceState BalanceSheetPrototype = new BalanceState();
}