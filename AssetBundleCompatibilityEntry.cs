using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AssetBundleCompatibilityEntry
{
	public string clientVersion = "";
	public string assetBundleName = "";
	public string assetBundleVersionToUse = "";
	
	public bool _showFoldOut = false;
	
	public void SetDataFromDictionary(Dictionary<string, object> data)
	{
		if (data.ContainsKey("ClientVersion"))
		{
			clientVersion = (string)data["ClientVersion"];
		}
		
		if (data.ContainsKey("AssetBundleName"))
		{
			assetBundleName = (string)data["AssetBundleName"];
		}
		
		if (data.ContainsKey("VersionToUse"))
		{
			assetBundleVersionToUse = (string)data["VersionToUse"];
		}
	}
	
	public string ToJson()
	{
		Dictionary<string, object> d = this.ToDict();
		return MiniJSON.Json.Serialize(d);
	}
	
	public Dictionary<string, object> ToDict()
	{
		Dictionary<string,object> d = new Dictionary<string, object>();
		d.Add("ClientVersion", clientVersion);
		d.Add("AssetBundleName", assetBundleName);
		d.Add("VersionToUse", assetBundleVersionToUse);
		return d;
	}
}
