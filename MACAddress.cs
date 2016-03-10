using UnityEngine;

public class MACAddress : MonoBehaviour
{
    public static string ShowPrimaryNetworkInterface()
    {
/*
#if UNITY_EDITOR
		string macAddress = "";
		NetworkInterface[] nics = NetworkInterface.GetAllNetworkInterfaces();
		
		foreach (NetworkInterface adapter in nics)
		{
			PhysicalAddress address = adapter.GetPhysicalAddress();
			if (address.ToString() != "")
			{
				return address.ToString();	
			}
		}
#endif
*/
        return "";
    }

    public static string ShowNetworkInterfaces()
    {
/*#if UNITY_EDITOR
		IPGlobalProperties deviceProperties = 
			IPGlobalProperties.GetIPGlobalProperties();	
		
		NetworkInterface[] nics = 
			NetworkInterface.GetAllNetworkInterfaces();
		
		string macAddresses = "";

		PhysicalAddress address = nics[0].GetPhysicalAddress();
		byte[] bytes = address.GetAddressBytes();
		
		string mac = null;
		for (int i = 0; i < bytes.Length; i++)
		{
			mac = string.Concat
				(mac + (string.Format("{0}", bytes[i].ToString("X2"))));
			
			if (i != bytes.Length - 1) 
			{
				mac = string.Concat(mac + "-");
			}
		}
		
		if (mac != "00-00-00-00-00-00-00-E0" && mac != "")
		{
			macAddresses += mac + ",";
		}
		
		return macAddresses;
#else
		return "";
#endif*/
        return "";
    }
}