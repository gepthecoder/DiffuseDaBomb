using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalConfig
{
    public GameModeType __GAME_MODE_TYPE__;
    public MatchSettingsConfigData __MATCH_SETTINGS__;
    public DuelConfigData __DUEL_SETTINGS__;

    public GlobalConfig(GameModeType type)
    {
        this.__GAME_MODE_TYPE__ = type;
    }

    public GlobalConfig(DuelConfigData data)
    {
        this.__DUEL_SETTINGS__ = data;
    }

    public GlobalConfig(MatchSettingsConfigData data)
    {
        this.__MATCH_SETTINGS__ = data;
    }

    public GlobalConfig() { }
}

public enum GameState { PreMatch, Countdown, Initial, Planting, Defusing, Victory, }

public class GameManager : MonoBehaviour
{
    private GameState m_CurrentState = GameState.Initial;

    [SerializeField] private StartMatchManager m_StartMatchManager;
    [SerializeField] private CountdownManager m_CountdownManager;
    [SerializeField] private BombManager m_BombManager;
    [SerializeField] private CameraManager m_CameraManager;
    [SerializeField] private UiManager m_UiManager;
    [SerializeField] private PlantBombManager m_PlantBombManager;
    [SerializeField] private DefuseBombManager m_DefuseBombManager;
    [SerializeField] private SceneSetupManager m_SceneSetupManager;
    [SerializeField] private CodeManager m_CodeManager;

    protected GlobalConfig ___Global_Config___;

    private void Start()
    {
        AddListeners();
        TriggerBehaviour(GameState.PreMatch);
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
            case GameState.PreMatch:
                Debug.Log($"<color=red>GameState</color><color=gold>PreMatch</color>");
                m_StartMatchManager.Init();
                break;
            case GameState.Countdown:
                Debug.Log($"<color=red>GameState</color><color=gold>Countdown</color>");
                m_CountdownManager.InitCountdown(___Global_Config___.__MATCH_SETTINGS__);
                break;
            case GameState.Initial:
                Debug.Log($"<color=red>GameState</color><color=gold>Initial</color>");
                m_BombManager.TriggerBombBehaviour(BombCaseState.Close);
                m_SceneSetupManager.EnableBombCoverUps(true);
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
                StartCoroutine(OnDefuseBombEvent());
                break;
            case GameState.Victory:
                Debug.Log($"<color=red>GameState</color><color=gold>Victory</color>");

                break;
            default:
                break;
        }
    }

    private IEnumerator OnDefuseBombEvent()
    {
        m_SceneSetupManager.EnableBombCoverUps(false);

        yield return new WaitForSeconds(1.5f);

        m_UiManager.FadeInOutScreen(1f);
        m_CameraManager.ZoomInOutToBombCaseView(
                  () => {
                      m_PlantBombManager.InitClockMotion(false);
                      m_SceneSetupManager.SetupScene(SceneType.Default);
                  },
                  () => {
                      m_BombManager.TriggerBombBehaviour(BombCaseState.Close);
                      m_CountdownManager.SyncDefuseBombTime(CountdownObjectType.CircuitTimer3D, CountdownObjectType.BombCaseTimer3D);
                      m_BombManager.InitClockMotion(true);
                      m_PlantBombManager.InitLights(LightType.PlasticBomb, LightAction.Trip);
                  }
              );
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

    private void OnValidateBombCodeEvent(CodeEncryptionType encryption)
    {
        m_CameraManager.ZoomOutOfTarget();
        m_UiManager.FadeInOutScreen(.77f);

        m_DefuseBombManager.TriggerDefuseBehaviour(DefuseBombState.Success, new HackingItemData(encryption));
    }

    private void AddListeners()
    {
        m_StartMatchManager.OnStartMatchEvent.AddListener((gameState, config) => {

            ___Global_Config___ = config;

            TriggerBehaviour(gameState);
        });

        m_BombManager.BombCaseOpeningEvent.AddListener(OnBombCaseOpeningEvent);
        m_UiManager.OnFadeInEvent.AddListener(() =>
        {
            if(m_CurrentState == GameState.Initial)
            {
                TriggerBehaviour(GameState.Planting);
                return;
            }

            if (m_CurrentState == GameState.Defusing)
            {
                m_SceneSetupManager.SetupScene(SceneType.Defusing);
                m_UiManager.FadeOutScreen();
                m_CameraManager.ZoomOutOfTarget();

                m_CountdownManager.SyncDefuseBombTime(CountdownObjectType.BombCaseTimer3D, CountdownObjectType.CircuitTimer3D);
                m_CountdownManager.SyncDefuseBombTime(CountdownObjectType.BombCaseTimer3D, CountdownObjectType.MagneticBombTimer3D);
                m_PlantBombManager.InitClockMotion(true);
                m_PlantBombManager.InitLights(LightType.PlasticBomb, LightAction.Trip);

                m_DefuseBombManager.TriggerDefuseBehaviour(DefuseBombState.Start);
            }
        });

        m_CodeManager.OnSetCodeEvent.AddListener((codeEncryptionType) => {
            OnSetBombCodeEvent(codeEncryptionType);
        });

        m_CodeManager.OnValidateCodeEvent.AddListener((codeEncryptionType) =>
        {
            OnValidateBombCodeEvent(codeEncryptionType);
        });

        m_PlantBombManager.OnPlantBombDoneEvent.AddListener(() =>
        {
            m_CountdownManager.InitDefuseBombTime(___Global_Config___.__MATCH_SETTINGS__.BombTimeInMinutes, new List<CountdownObjectType>() { CountdownObjectType.CircuitTimer3D, CountdownObjectType.MagneticBombTimer3D });
            m_PlantBombManager.InitClockMotion(true);

            TriggerBehaviour(GameState.Defusing);
        });

        m_DefuseBombManager.OnDefuseBombDoneEvent.AddListener(() =>
        {
            m_DefuseBombManager.InitClockMotion(false);

            m_CountdownManager.SetDefuseBombTimeText(-1, CountdownObjectType.MagneticBombTimer3D, true);
            m_CountdownManager.SetDefuseBombTimeText(-1, CountdownObjectType.CircuitTimer3D, true);

            TriggerBehaviour(GameState.Victory);
        });

        m_CountdownManager.OnCountdownCompletedEvent.AddListener(() => {
            TriggerBehaviour(GameState.Initial);
        });

        m_PlantBombManager.OnPlantBombEvent.AddListener((type) => {
            m_CountdownManager.SetDefuseBombTimeText(___Global_Config___.__MATCH_SETTINGS__.BombTimeInMinutes,
                type == CodeEncryptionType.KeyboardEncryption ? CountdownObjectType.MagneticBombTimer3D : CountdownObjectType.CircuitTimer3D);
        });

        m_DefuseBombManager.OnDefuseBombEvent.AddListener((type) => {

            if(type == CodeEncryptionType.KeyboardEncryption)
            {
                m_DefuseBombManager.InitClockMotion(false);
            }
            m_CountdownManager.SetDefuseBombTimeText(-1,
               type == CodeEncryptionType.KeyboardEncryption ? CountdownObjectType.MagneticBombTimer3D : CountdownObjectType.CircuitTimer3D, true);
        });
    }

    private void RemoveListeners()
    {
        m_BombManager.BombCaseOpeningEvent.RemoveAllListeners();
        m_UiManager.OnFadeInEvent.RemoveAllListeners();
        m_CodeManager.OnSetCodeEvent.RemoveAllListeners();
        m_StartMatchManager.OnStartMatchEvent.RemoveAllListeners();
        m_CodeManager.OnValidateCodeEvent.RemoveAllListeners();
        m_PlantBombManager.OnPlantBombDoneEvent.RemoveAllListeners();
        m_DefuseBombManager.OnDefuseBombDoneEvent.RemoveAllListeners();
        m_PlantBombManager.OnPlantBombEvent.RemoveAllListeners();
        m_DefuseBombManager.OnDefuseBombEvent.RemoveAllListeners();
    }
}
