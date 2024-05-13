using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UnityEngine;
using BaseSpace;
public class NetWorkTool : Singleton<NetWorkTool>
{
    public static async Task<string> GetExtranetIp()
    {
        string IP = string.Empty;
        try
        {
            //从网址中获取本机ip数据  
            System.Net.WebClient client = new System.Net.WebClient();
            client.Encoding = System.Text.Encoding.Default;
            IP = await client.DownloadStringTaskAsync("http://checkip.amazonaws.com/");
            client.Dispose();
            IP = Regex.Replace(IP, @"[\r\n]", "");
        }
        catch (Exception) { }

        return IP;
    }
}
