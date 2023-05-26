using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Advertisements;

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

public enum GameState { PreMatch, Countdown, Initial, Planting, Defusing, Victory, Repair, EndMatch, }

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
    [SerializeField] private VictoryManager m_VictoryManager;
    [SerializeField] private ScoreManager m_ScoreManager;
    [SerializeField] private RepairBombManager m_RepairBombManager;
    [SerializeField] private EndMatchManager m_EndMatchManager;

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

    private void TriggerBehaviour(GameState state, VictoryEventData data = null)
    {
        m_CurrentState = state;

        switch (state)
        {
            case GameState.PreMatch:
                Debug.Log($"<color=red>GameState</color><color=gold>PreMatch</color>");
                {
                    AdManager.INSTANCE.ShowBannerAd(BannerPosition.BOTTOM_CENTER);
                    m_StartMatchManager.Init();
                } break;
            case GameState.Countdown:
                Debug.Log($"<color=red>GameState</color><color=gold>Countdown</color>");
                {
                    AdManager.INSTANCE.HideBannerAd();
                    m_ScoreManager.SetScoreLimit(___Global_Config___.__MATCH_SETTINGS__.ScoreLimit);
                    m_CountdownManager.InitCountdown(___Global_Config___.__MATCH_SETTINGS__);
                } break;
            case GameState.Initial:
                Debug.Log($"<color=red>GameState</color><color=gold>Initial</color>");
                {
                    AdManager.INSTANCE.ShowBannerAd(BannerPosition.BOTTOM_RIGHT);
                    m_BombManager.TriggerBombBehaviour(BombCaseState.Close);
                    m_SceneSetupManager.EnableBombCoverUps(true);
                } break;
            case GameState.Planting:
                Debug.Log($"<color=red>GameState</color><color=gold>Planting</color>");
                {
                    m_SceneSetupManager.SetupScene(SceneType.Planting);
                    m_PlantBombManager.TriggerPlantBehaviour(PlantBombState.Start);
                    m_UiManager.FadeOutScreen();
                    m_CameraManager.ZoomOutOfTarget();
                } break;
            case GameState.Defusing:
                Debug.Log($"<color=red>GameState</color><color=gold>Defusing</color>");
                { 
                    StartCoroutine(OnDefuseBombEvent());
                } break;
            case GameState.Victory:
                Debug.Log($"<color=red>GameState</color><color=gold>Victory</color>");
                if(data != null)
                {
                    // TODO: check for score limit reached - Bomb Defuse / Explosion!
                    Debug.Log($"{data._WinningTeam_} WON ROUND by {data._VictoryType_}");
                    m_VictoryManager.InitVictory(new VictoryEventData(data._WinningTeam_, data._VictoryType_, m_BombManager.GetCurrentBombCaseState(), data._WinningTeam_ == Team.Axis ? ___Global_Config___.__DUEL_SETTINGS__.AxisConfigData.TeamName : ___Global_Config___.__DUEL_SETTINGS__.AlliesConfigData.TeamName));
                } break;
            case GameState.Repair:
                Debug.Log($"<color=red>GameState</color><color=gold>Repair</color>");
                {
                    AdManager.INSTANCE.HideBannerAd();
                    m_CountdownManager.DeinitCountdownObjects();
                    m_RepairBombManager.Init(data._VictoryType_);
                } break;
            case GameState.EndMatch:
                Debug.Log($"<color=red>GameState</color><color=gold>EndMatch</color>");
                {
                    if(data != null)
                    {
                        EndMatchObjectData endMatchData = GetEndMatchData(data);
                        m_EndMatchManager.InitEndMatch(endMatchData);

                        // SaveData
                        HistoryItemData historyData = new HistoryItemData(
                              endMatchData.m_WinningTeamSprite.name,
                              endMatchData.m_LosingTeamSprite.name,
                              endMatchData.m_WinningTeamNameString,
                              endMatchData.m_LosingTeamNameString,
                              endMatchData.m_WinningTeamScore,
                              endMatchData.m_LosingTeamScore,
                              endMatchData.m_EndMatchDateString,
                              endMatchData.m_EndMatchTimeString
                        );
                        SaveLoadManager.INSTANCE?.SaveHistoryObject(historyData);
                    }
                } break;
            default:
                break;
        }
    }

    private EndMatchObjectData GetEndMatchData(VictoryEventData data)
    {
        EndMatchObjectData eMatchObjData = new EndMatchObjectData();

        // EMBLEM
        eMatchObjData.m_WinningTeamSprite = data._WinningTeam_ == Team.Axis ?
            ___Global_Config___.__DUEL_SETTINGS__.AxisConfigData.TeamEmblem : ___Global_Config___.__DUEL_SETTINGS__.AlliesConfigData.TeamEmblem;

        eMatchObjData.m_LosingTeamSprite = data._WinningTeam_ == Team.Allies ?
            ___Global_Config___.__DUEL_SETTINGS__.AxisConfigData.TeamEmblem : ___Global_Config___.__DUEL_SETTINGS__.AlliesConfigData.TeamEmblem;

        // TEAM NAME
        eMatchObjData.m_WinningTeamNameString = data._WinningTeam_ == Team.Axis ?
            ___Global_Config___.__DUEL_SETTINGS__.AxisConfigData.TeamName : ___Global_Config___.__DUEL_SETTINGS__.AlliesConfigData.TeamName;

        eMatchObjData.m_LosingTeamNameString = data._WinningTeam_ == Team.Allies ?
            ___Global_Config___.__DUEL_SETTINGS__.AxisConfigData.TeamName : ___Global_Config___.__DUEL_SETTINGS__.AlliesConfigData.TeamName;

        // SCORE
        int winningTeamScore;
        int losingTeamScore;
        m_ScoreManager.GetFinalScoreByTeamType(data._WinningTeam_, out winningTeamScore, out losingTeamScore);

        eMatchObjData.m_WinningTeamScore = winningTeamScore;
        eMatchObjData.m_LosingTeamScore = losingTeamScore;

        eMatchObjData.m_IsDraw = data._IsDraw_;

        // DATE & TIME
        DateTime currentDateTime = DateTime.Now;

        eMatchObjData.m_EndMatchDateString = $"{currentDateTime:dd/MM/yyyy}";
        eMatchObjData.m_EndMatchTimeString = $"{currentDateTime:HH:MM}";

        // SETTINGS ITEMS

        // WINNER
        var settingsItemDataWinner = new SettingsItemData();

        settingsItemDataWinner.Type = data._WinningTeam_ == Team.Axis ? SettingsItemType.Axis : SettingsItemType.Allies;
        settingsItemDataWinner.TeamName = eMatchObjData.m_WinningTeamNameString;
        settingsItemDataWinner.TeamCount = data._WinningTeam_ == Team.Axis ?
            ___Global_Config___.__DUEL_SETTINGS__.AxisConfigData.TeamCount : ___Global_Config___.__DUEL_SETTINGS__.AlliesConfigData.TeamCount;
        settingsItemDataWinner.TeamEmblem = eMatchObjData.m_WinningTeamSprite;

        eMatchObjData.m_SettingsItemDataWinner = settingsItemDataWinner;

        // LOSER
        var settingsItemDataLoser = new SettingsItemData();

        settingsItemDataLoser.Type = data._WinningTeam_ == Team.Allies ? SettingsItemType.Axis : SettingsItemType.Allies;
        settingsItemDataLoser.TeamName = eMatchObjData.m_LosingTeamNameString;
        settingsItemDataLoser.TeamCount = data._WinningTeam_ == Team.Allies ?
            ___Global_Config___.__DUEL_SETTINGS__.AxisConfigData.TeamCount : ___Global_Config___.__DUEL_SETTINGS__.AlliesConfigData.TeamCount;
        settingsItemDataLoser.TeamEmblem = eMatchObjData.m_LosingTeamSprite;

        eMatchObjData.m_SettingsItemDataLoser = settingsItemDataLoser;

        return eMatchObjData;
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

            RoundManager.instance.SetMaxRounds(___Global_Config___.__MATCH_SETTINGS__.ScoreLimit);

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
            m_CountdownManager.SetDefuseBombTimeText(-1, CountdownObjectType.BombCaseMagnetic3D, true);

            m_CountdownManager.DisableBombTimerOnDefuseEvent();

            Team team = RoundManager.instance.GetWinningTeamByVictoryType(VictoryType.BombDefused);
            TriggerBehaviour(GameState.Victory, new VictoryEventData(team, VictoryType.BombDefused));
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

        m_CountdownManager.OnVictoryEvent.AddListener((victoryDATA) => {
            TriggerBehaviour(GameState.Victory, new VictoryEventData(victoryDATA._WinningTeam_, victoryDATA._VictoryType_));
        });

        m_VictoryManager.OnVictoryShownEvent.AddListener((data) => {
            if(!data._ScoreLimitReached_)
            {
                TriggerBehaviour(GameState.Repair, data);
            } else
            {
                TriggerBehaviour(GameState.EndMatch, data);
            }
        }); 
        
        m_VictoryManager.OnZoomOutOfComplex.AddListener((callback) => {
            // Move To BombCase view
            m_UiManager.FadeInOutScreen(1f);
            m_CameraManager.ZoomInOutToBombCaseView(
                      () => {
                          m_SceneSetupManager.SetupScene(SceneType.Default);
                      },
                      () => {
                          callback?.Invoke();
                      }
                  );
        });

        m_RepairBombManager.OnBombRepairCompleted.AddListener(() => {

            // INTERSTITIAL AD POPUP
            AdManager.INSTANCE.ShowInterstitalAd(() => {

                // N E W  R O U N D

                // RESET
                m_PlantBombManager.Deinit();
                m_DefuseBombManager.ResetBombDefuseSettings();
                m_CodeManager.Deinit();
                m_CountdownManager.ResetCountDownObjects();
                m_VictoryManager.ResetBombAfterMathEffect();
                m_BombManager.ResetSparks();

                // START NEW ROUND
                RoundManager.instance.NewRound(() =>
                {
                    m_CountdownManager.InitRoundTimeCountdown();
                    TriggerBehaviour(GameState.Initial);
                });
            });
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
        m_CountdownManager.OnVictoryEvent.RemoveAllListeners();
        m_VictoryManager.OnVictoryShownEvent.RemoveAllListeners();
        m_VictoryManager.OnZoomOutOfComplex.RemoveAllListeners();
        m_RepairBombManager.OnBombRepairCompleted.RemoveAllListeners();
    }
}
