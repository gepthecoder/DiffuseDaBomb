using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Config : MonoBehaviour
{
    public static Config INSTANCE;

    protected GlobalConfig __globalConfig__;

    private void Awake()
    {
        if (INSTANCE == null)
        {
            INSTANCE = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void UpdateGlobalConfig(GlobalConfig cfg)
    {
        __globalConfig__ = cfg;
    }

    public GlobalConfig GetGlobalConfig()
    {
        return __globalConfig__;
    }
}