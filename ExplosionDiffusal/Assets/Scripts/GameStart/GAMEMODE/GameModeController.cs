using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GameModeController : MonoBehaviour
{
    [SerializeField] private List<GameModeObject> m_GameModes;

    private GameModeType m_CurrentSelectedGameMode = GameModeType.None;

    private void Awake()
    {
        m_GameModes.ForEach((item) =>
        {
            item.OnGameModeSelectedEvent.AddListener((type) => {
                OnGameModeSelected(type);
            });
        });
    }

    private void OnGameModeSelected(GameModeType type)
    {
        if (type == m_CurrentSelectedGameMode)
            return;

        m_CurrentSelectedGameMode = type;

        m_GameModes.ForEach((obj) => {
            if (obj.Type == m_CurrentSelectedGameMode) obj.OnSelected();
            else obj.OnDeSelected();
        });
    }


    private void OnDestroy()
    {
        m_GameModes.ForEach((item) =>
        {
            item.OnGameModeSelectedEvent.RemoveAllListeners();
        });
    }
}
