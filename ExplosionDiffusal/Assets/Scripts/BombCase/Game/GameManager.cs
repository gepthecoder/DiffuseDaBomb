using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Search and Destory Rules

// Attacker Defender

// Attacker -> Plant The Bomb 

// Defender Needs To Defuse it before Time Runs Out -> Defuse -> Victory: Defender
// Defender Doesn't Defuse bomb in Time -> Victory: Attacker

// SCORE/TEAM system, history

// FLOW
// Bomb Suitcase Starts Wobbling -> OnDown timer || Click
// Bomb Opens -> Camera Zooms In -> Scene Transition
// Circuit Board With Key Locks and Riddle ??

// Keyboard || Keypad \\

// Enter Code On Both (TODO: settings to select either or)

// Bomb Is Planted

public enum GameState { Initial, Planting, Defusing, Victory, }

public class GameManager : MonoBehaviour
{
    private GameState m_CurrentState = GameState.Initial;

    [SerializeField] private BombManager m_BombManager;
    [SerializeField] private CameraManager m_CameraManager;
    [SerializeField] private UiManager m_UiManager;
    [SerializeField] private PlantBombManager m_PlantBombManager;
    [SerializeField] private SceneSetupManager m_SceneSetupManager;
    [SerializeField] private CodeManager m_CodeManager;

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
                Debug.Log($"<color=red>GameState</color><color=gold>Initial</color>");
                m_BombManager.TriggerBombBehaviour(BombCaseState.Close);
                break;
            case GameState.Planting:
                Debug.Log($"<color=red>GameState</color><color=gold>Planting</color>");
                m_SceneSetupManager.SetupScene(SceneType.Planting);
                m_PlantBombManager.TriggerPlantBehaviour(PlantBombState.Start);
                m_UiManager.FadeOutScreen();
                m_CameraManager.ZoomOutOfTarget();
                break;
            case GameState.Defusing:
                Debug.Log($"<color=red>GameState</color><color=gold>Defusing</color>");
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
        m_UiManager.FadeInScreen(1.5f, true);
    }

    private void OnSetBombCodeEvent(CodeEncryptionType encryption)
    {
        m_CameraManager.ZoomOutOfTarget();
        m_UiManager.FadeInOutScreen(.77f);

        m_PlantBombManager.TriggerPlantBehaviour(PlantBombState.Success, new HackingItemData(encryption));
    }

    private void AddListeners()
    {
        m_BombManager.BombCaseOpeningEvent.AddListener(OnBombCaseOpeningEvent);
        m_UiManager.OnFadeInEvent.AddListener(() =>
        {
            TriggerBehaviour(GameState.Planting);
        });
        m_CodeManager.OnSetCodeEvent.AddListener((codeEncryptionType) => {
            OnSetBombCodeEvent(codeEncryptionType);
        });
        m_PlantBombManager.OnPlantBombDoneEvent.AddListener(() =>
        {
            TriggerBehaviour(GameState.Defusing);
        });
    }

    private void RemoveListeners()
    {
        m_BombManager.BombCaseOpeningEvent.RemoveAllListeners();
        m_UiManager.OnFadeInEvent.RemoveAllListeners();
        m_CodeManager.OnSetCodeEvent.RemoveAllListeners();
    }
}
