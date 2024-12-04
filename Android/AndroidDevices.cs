using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AndroidDevices :Singleton<AndroidDevices>
{
    public bool TryGetMac(out string message)
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            using (AndroidJavaClass wifiManagerClass = new AndroidJavaClass("android/net/wifi/WifiInfo"))
            {
                using (AndroidJavaObject wifiManager = new AndroidJavaObject("android.net.wifi.WifiManager", GetActivity()))
                {
                    using (AndroidJavaObject connectionInfo = wifiManager.Call<AndroidJavaObject>("getConnectionInfo"))
                    {
                        string macAddress = connectionInfo.Call<string>("getMacAddress");
                        message = macAddress;
                        return true;
                    }
                }
            }
        }
        message = "无法获取 MAC 地址";
        return false;
    }
    private AndroidJavaObject GetActivity()
    {
        using (AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
        {
            return unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
        }
    }

    
}
