using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class RankProtoData
{
    public int _nRank;

    public int _IconIndex;

    public int _nScore;

    public string _nameStr;

    public RankProtoData()
    {
        _nRank = -1;
        _IconIndex = 1;
        _nScore = 0;
        _nameStr = "无名氏";
    
    }

    public RankProtoData(Dictionary<string, object> dict) 
    {
        _IconIndex = (int)dict["IconIndex"];
        _nRank = (int)dict["nRank"];
        _nScore = (int)dict["nScore"];
        _nameStr = (string)dict["nameStr"];
    
    }



}
