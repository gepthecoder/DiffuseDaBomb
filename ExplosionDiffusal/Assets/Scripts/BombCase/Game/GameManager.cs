using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Search and Destory Rules

// Attacker Defender

// Attacker Steals The Bomb -> Plant

// Defender Needs To Defuse it before Time Runs Out -> Defuse

// SCORE/TEAM system, history


// FLOW
// Bomb Suitcase Starts Wobbling -> Press To Open + Key To Open
// Bomb Opens -> Camera Zooms In -> Scene Transition
// Circuit Board With Key Locks and Riddle ??

public enum GameState { Initial, Planting, Defusing, Victory, }

public class GameManager : MonoBehaviour
{
    private GameState m_CurrentState = GameState.Initial;

    [SerializeField] private BombManager m_BombManager;
    [SerializeField] private CameraManager m_CameraManager;
    [SerializeField] private UiManager m_UiManager;
    [SerializeField] private PlantBombManager m_PlantBombManager;
    [SerializeField] private SceneSetupManager m_SceneSetupManager;

    private void Start()
    {
        AddListeners();

        TriggerBehaviour(m_CurrentState);
    }

    private void OnDestroy()
    {
        RemoveListeners();
    }

    private void TriggerBehaviour(GameState state)
    {
        m_CurrentState = state;

        switch (state)
        {
            case GameState.Initial:
                m_BombManager.TriggerBombBehaviour(BombCaseState.Close);
                break;
            case GameState.Planting:
                m_SceneSetupManager.SetupScene(SceneType.Planting);
                m_PlantBombManager.TriggerPlantBehaviour(PlantBombState.Start);
                m_UiManager.FadeOutScreen();
                m_CameraManager.ZoomOutOfTarget();
                break;
            case GameState.Defusing:
                break;
            case GameState.Victory:
                break;
            default:
                break;
        }
    }

    private void OnBombCaseOpeningEvent()
    {
        m_CameraManager.ZoomInToTarget();
        m_UiManager.FadeInScreen();
    }

    private void AddListeners()
    {
        m_BombManager.BombCaseOpeningEvent.AddListener(OnBombCaseOpeningEvent);
        m_UiManager.OnFadeInEvent.AddListener(() =>
        {
            TriggerBehaviour(GameState.Planting);
        });
    }

    private void RemoveListeners()
    {
        m_BombManager.BombCaseOpeningEvent.RemoveAllListeners();
    }
}
