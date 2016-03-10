using System.Collections.Generic;
using System;

/// <summary>
/// Represents the data the environment set bootstrapper has to know, but not the internal data of each environment set
/// </summary> 
public class EnvironmentSetBootstrapData {
	
	public string SetCode;
	public const string CodeKey = "SetCode";
	
	/// <summary>
	/// The name of the asset bundle. e.g darkforest, yellowbrickroad, 
	/// </summary>
	public string AssetBundleName;
	public const string AssetBundleNameKey = "AssetBundleName";
	
	/// <summary>
	/// if true this environment set is part of the game and is not downloaded
	/// </summary>
	public bool Embedded;
	public const string EmbeddedKey = "Embedded";
	
	/// <summary>
	/// if true this environment set is part of the game and is not downloaded and is an assetbundle saved in StreamingAssets
	/// </summary>
	public bool EmbeddedAssetBundle;
	public const string EmbeddedAssetBundleKey = "EmbeddedAssetBundle";
	
	/// <summary>
	/// Initializes a new instance of the <see cref="EnvironmentSetBootstrapData"/> class.
	/// </summary>
	/// <param name='envsetCode'>
	/// the short abbreviation for the envset, e.g. ybr for yellow brick road, df for dark forest, etc
	/// indicates a text file with that envsetCode exists in Resources/OzGameData
	/// </param>
	public EnvironmentSetBootstrapData( string envsetCode, string bundleName, bool embedded, bool embeddedAssetBundle)
	{
		SetCode = envsetCode;
		AssetBundleName = bundleName;
		Embedded = embedded;
		EmbeddedAssetBundle = embeddedAssetBundle;
	}
	
	/// <summary>
	/// Initializes a new instance of the <see cref="EnvironmentSetBootstrapData"/> class.
	/// </summary>
	/// a dictionary representation of this object, probably coming from a json deserialization
	/// Dict.
	/// </param>
	public EnvironmentSetBootstrapData(Dictionary<string, object> dict) 
	{
		SetCode = "None";
		AssetBundleName = "None";
		Embedded = false;
		
		if(dict.ContainsKey(CodeKey)) {
			SetCode = (string)dict[CodeKey];	
		}
		
		if (dict.ContainsKey(AssetBundleNameKey)) {
			AssetBundleName = (string)dict[AssetBundleNameKey];	
		}
		
		if (dict.ContainsKey(EmbeddedKey)) {
			Embedded = (bool)dict[EmbeddedKey];	
		}
		
		if (dict.ContainsKey(EmbeddedAssetBundleKey)) {
			EmbeddedAssetBundle = (bool)dict[EmbeddedAssetBundleKey];	
		}
	}
	
	/// <summary>
	/// Change this to a dictionary
	/// </summary>
	/// <returns>
	/// The dictionary version of the data
	/// </returns>
	public Dictionary<string, object> ToDict() 
	{
		Dictionary<string, object> d = new Dictionary<string, object>();
		d.Add (CodeKey, SetCode);
		d.Add (AssetBundleNameKey, AssetBundleName);
		d.Add (EmbeddedKey, Embedded);
		d.Add (EmbeddedAssetBundleKey, EmbeddedAssetBundle);
		return d;
	}
	
	/// <summary>
	/// Convert this data to json
	/// </summary>
	/// <returns>
	/// The json string
	/// </returns>
	public string ToJson() {
		Dictionary<string, object> d = ToDict();
		return MiniJSON.Json.Serialize(d);
	}
}

// Custom comparer for the EnvironmentSetBootstrapData class 
class EnvironmentSetBootstrapDataComparer : IEqualityComparer<EnvironmentSetBootstrapData>
{
    // Products are equal if their names and product numbers are equal. 
    public bool Equals(EnvironmentSetBootstrapData x, EnvironmentSetBootstrapData y)
    {

        //Check whether the compared objects reference the same data. 
        if (Object.ReferenceEquals(x, y)) return true;

        //Check whether any of the compared objects is null. 
        if (Object.ReferenceEquals(x, null) || Object.ReferenceEquals(y, null))
            return false;

        //Check whether the products' properties are equal. 
        return ( x.SetCode == y.SetCode &&
			x.AssetBundleName == y.AssetBundleName &&
			x.Embedded == y.Embedded &&
			x.EmbeddedAssetBundle == y.EmbeddedAssetBundle);
			
    }
	
    // If Equals() returns true for a pair of objects  
    // then GetHashCode() must return the same value for these objects. 
    public int GetHashCode(EnvironmentSetBootstrapData product)
    {
        //Check whether the object is null 
        if (Object.ReferenceEquals(product, null)) 
			return 0;

        //Get hash code for the Code field if it is not null. 
        int hash = product.SetCode == null ? 0 :
			product.SetCode.GetHashCode() + product.AssetBundleName.GetHashCode() + product.Embedded.GetHashCode() ;

 		return hash;
    }
}