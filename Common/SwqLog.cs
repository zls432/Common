using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwqLog 
{

    public static Action<string> swqLog;
    public static Action<string> swqLogWaring;
    public static Action<string> swqLogError;
   
    public static void Log(string content)
    {
        swqLog?.Invoke("SwqLog:"+ content);
    }

    public static void LogWaring(string content)
    {
        swqLogWaring?.Invoke("SwqLogWaring:" + content);
    }

    public static void LogError(string content)
    {
        swqLogError?.Invoke("SwqLogError:" + content);
    }
}
