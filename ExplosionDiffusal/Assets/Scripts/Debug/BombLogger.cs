using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombLogger : MonoBehaviour
{
    public static BombLogger INSTANCE;

    private string myLog = "START BOMB LOG:";
    private int kChars = 10500;

    public void InitBombLogger(bool isDeveloperMode)
    {
        if(isDeveloperMode) {
            if (INSTANCE == null)
            {
                INSTANCE = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void OnEnable() { Application.logMessageReceived += Log; }
    void OnDisable() { Application.logMessageReceived -= Log; }

    public void Log(string logString, string stackTrace, LogType type)
    {
        if (type == LogType.Warning) {
            return;
        }
        myLog = myLog + "\n" + logString;
        if (myLog.Length > kChars)
        {
            myLog = myLog.Substring(myLog.Length - kChars);
        }
    }

    public string GetLogString() { return myLog; }
}
