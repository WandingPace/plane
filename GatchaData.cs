using System;
using System.Collections.Generic;
using UnityEngine;

public enum GatchaType
{
    EMPTY,
    COINS, //金币
    SCORE_BONUS, //
    //SCORE_MULTIPLIER,
    HeadBoost, //起飞加速
    DeadBoost, //死亡加速
    Boost, //加速
    Shield, //护盾
    Vacuum, //磁铁
    Fragment, //零件
    Gem //钻石
}

[Serializable]
public class GatchaDataSet
{
    public bool active = true;
    public int amount;
    public int randomWeight = 1;
    public GatchaType type;
}

public class GatchaData : MonoBehaviour
{
    public List<GatchaDataSet> gatchaList;
}