using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

// TODO: option for SENDING LOGS to DEV / SAVE LOGS TO DEVICE

public class DeveloperLogMenuItem : DeveloperItem
{
    [SerializeField] private TextMeshProUGUI m_LogText;

    public override void Init()
    {
        base.Init();

        m_LogText.text = string.Empty;
        m_LogText.text = BombLogger.INSTANCE.GetLogString();
    }

    public override void Deinit()
    {
        base.Deinit();
    }
}
