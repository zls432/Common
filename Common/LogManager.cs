using InternelSDK;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BaseSpace;
public class LogManager :BaseSpace.Singleton<LogManager>
{

    public override void Init()
    {
        HttpHelper.Instance.httpLog = Debug.Log;
        HttpHelper.Instance.httpLogWarning = Debug.LogWarning;
        HttpHelper.Instance.httpLogError = Debug.LogError;

        CustomLog.RegisterLogD(Debug.Log);
        CustomLog.RegisterLogWaring(Debug.LogWarning);
        CustomLog.RegisterLogError(Debug.LogError);
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
