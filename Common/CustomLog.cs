using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomLog 
{

    private static Action<string> customLog;
    private static Action<string> customLogWaring;
    private static Action<string> customLogError;
   
    public static void Log(string content)
    {
        customLog?.Invoke("CustomLog:"+ content);
    }

    public static void LogWaring(string content)
    {
        customLogWaring?.Invoke("customLogWaring:" + content);
    }

    public static void LogError(string content)
    {
        customLogError?.Invoke("customLogError:" + content);
    }

    public static void RegisterLogD(Action<string> log)
    {
        customLog = log;
    }
    public static void RegisterLogWaring(Action<string> log)
    {
        customLogWaring = log;
    }
    public static void RegisterLogError(Action<string> log)
    {
        customLogError = log;
    }
}
