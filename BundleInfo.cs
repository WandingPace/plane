/// <summary>
///     Retrieves information specific to the bundle.
/// </summary>
public static class BundleInfo
{
//#if !UNITY_EDITOR && UNITY_IPHONE
//	[DllImport("__Internal")]
//	public static extern string GetBundleId();
//#else
    public static string GetBundleId()
    {
        return CurrentBundleVersion.bundleId;
    }

//#endif

    public static string GetBundleVersion()
    {
        var bundleVersion = "";

        bundleVersion = CurrentBundleVersion.version + ".0";
        return bundleVersion;
    }

    public static string GetBundleShortVersion()
    {
        string bundleVersion = CurrentBundleVersion.version;
        string[] str = bundleVersion.Split('.');
        bundleVersion = string.Format("{0}.{1}", str[0], str[1]);
        return bundleVersion;
    }
}