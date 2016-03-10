using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class Rankdata : MonoBehaviour {


   

    public static List<RankProtoData> Getdata()
    {
           List<RankProtoData> dataList = new List<RankProtoData>();
        for (int i = 0; i < 50; i++)
        {
            Dictionary<string, object> dict = new Dictionary<string, object> { { "nRank", i + 1 }, { "IconIndex",i%4 +1 }, { "nScore", 1022 + i }, { "nameStr", "囧囧" } };
            RankProtoData pro = new RankProtoData(dict);
            dataList.Add(pro);
        }
            return dataList;
    }

    public static List<RankProtoData> GetHistoryworldData()
    {

        List<RankProtoData> dataList = new List<RankProtoData>();
        for (int i = 0; i < 50; i++)
        {
            Dictionary<string, object> dict = new Dictionary<string, object> { { "nRank", i + 1 }, { "IconIndex", i % 4 + 1 }, { "nScore", 36530 - i }, { "nameStr", "囧囧" } };
            RankProtoData pro = new RankProtoData(dict);
            dataList.Add(pro);
        }
        return dataList;
    
    }


}
