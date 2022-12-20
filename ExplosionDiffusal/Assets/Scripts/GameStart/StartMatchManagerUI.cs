using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using DG.Tweening;
using System;
using TMPro;

/// <summary>
/// INTRO -> GAME MODE -> GAME SETTINGS -> TEAM SELECTION -> START GAME LOOP
/// </summary>

public class StartMatchManagerUI : MonoBehaviour
{
    
    [Header("START MATCH")]
    [SerializeField] private Button m_StartMatchButton;
    [Header("PRE MATCH")]
    [SerializeField] private Transform m_GameTile;
    [SerializeField] private Transform m_GameTileEndPoint;
    [SerializeField] private Image m_BackgroundImage;
    [Space(5)]
    // START GAME
    [SerializeField] private GameObject m_StartGameOptions;
    [SerializeField] private List<Transform> m_StartGameOptionItems;
    [Space(5)]
    // ANIMATIONS
    [SerializeField] private Animation m_GameModeAnime;
    [SerializeField] private Animation m_MatchSettingsAnime;
    [SerializeField] private Animation m_SelectTeamsAnime;
    [SerializeField] private Animation m_SetupAnime;
    [Space(5)]
    // GAME MODE
    [SerializeField] private Transform m_GameMode;
    [SerializeField] private Transform m_MatchSettingsParent;
    [SerializeField] private Transform m_DuelParent;
    [Space(5)]
    // DUEL
    [SerializeField] private Transform m_Duel;
    [SerializeField] private Animator m_TeamSettingsAxis;
    [SerializeField] private Animator m_TeamSettingsAllies;
    private CanvasGroup m_TeamSettingsCanvasGroupRef = new CanvasGroup();
    [SerializeField] private Image m_TeamSettingsBackground;
    [Space(5)]
    // END
    [SerializeField] private Animator m_MidPanelCGroupAnime;
    [SerializeField] private Animator m_StartGameOptionsCGroupAnime;
    [SerializeField] private Animator m_VSCGroupAnime;
    [Header("END GOTO POSITONS")]
    [SerializeField] private Transform m_TeamNameAxisEndGoToPosition;
    [SerializeField] private Transform m_TeamNameAlliesEndGoToPosition;
    [Space(3)]
    [SerializeField] private Transform m_TeamCountAxisEndGoToPosition;
    [SerializeField] private Transform m_TeamCountAlliesEndGoToPosition;
    [Space(3)]
    [SerializeField] private Transform m_TeamEmblemAxisEndGoToPosition;
    [SerializeField] private Transform m_TeamEmblemAlliesEndGoToPosition;
    [Space(5)]
    //
    [SerializeField] private Button m_PlayButton;
    //
    [SerializeField] private Button m_ReadyButton;
    private TextMeshProUGUI m_ReadyText;
    //
    [Header("CONFIG - Game Mode")]
    [SerializeField] private GameModeController m_GameModeController;
    [Header("CONFIG - Match Settings")]
    [SerializeField] private MatchSettingsController m_MatchSettingsController;
    [Header("CONFIG - Duel")]
    [SerializeField] private DuelController m_DuelController;
    [Header("Main Canvas")]
    [SerializeField] private MainCanvas m_MainCanvas;
    [Header("Start Match Canvas")]
    [SerializeField] private Canvas m_StartMatchCanvas;

    [HideInInspector] public UnityEvent<GlobalConfig> OnStartMatchButtonClickedEvent = new UnityEvent<GlobalConfig>();
    [HideInInspector] public UnityEvent<Action> OnFadeOutEffectEvent = new UnityEvent<Action>();

    private RectTransform m_RectGameOptions;
    private StartMatchState m_CurrentState;

    private bool m_CanPlaySetupAnime = true;

    private bool m_CanShowDuelReadyButtonAnime = true;

    private GlobalConfig m_GlobalConfigData = new GlobalConfig();


    private void Awake()
    {
        m_MatchSettingsController.OnSetMatchSettingsDoneEvent.AddListener((DATA) => {
            m_GlobalConfigData.__MATCH_SETTINGS__ = DATA;
            TriggerBehaviour(StartMatchState.Duel);
        });

        m_GameModeController.OnGameModeSelectedFinalEvent.AddListener((MODE) => {
            m_GlobalConfigData.__GAME_MODE_TYPE__ = MODE;
            TriggerBehaviour(StartMatchState.MatchSettings);
        });


        m_DuelController.OnDuelConfigSetEvent.AddListener((DATA) => {

            if(m_CanShowDuelReadyButtonAnime)
            {
                m_ReadyButton.transform.DOScale(1f, 1f)
                 .SetEase(Ease.InQuart)
                 .OnComplete(() => {
                     m_ReadyText.DOFade(1f, .5f)
                     .OnComplete(() => {
                         m_ReadyButton.transform.DOScale(1.1f, .7f).SetEase(Ease.InQuart).OnComplete(() => {
                             m_ReadyButton.transform.DOScale(1f, .7f).SetEase(Ease.InOutBack).OnComplete(() => {
                                 m_ReadyButton.interactable = true;
                             });
                        });
                    });
                });

                m_CanShowDuelReadyButtonAnime = false;
            }

            m_GlobalConfigData.__DUEL_SETTINGS__ = DATA;
        });

        m_ReadyButton.onClick.AddListener(() => {
            m_ReadyButton.transform.DOScale(0f, .3f).SetEase(Ease.InQuart);

            m_DuelController.GetDuelObjByType(DuelObjectType.Attacker).OnDeSelected();
            m_DuelController.GetDuelObjByType(DuelObjectType.Defender).OnDeSelected();

            // Duel Object & Team Settings Are From Here On Not Interactable!
            m_DuelController.DeActivateDuelObjectInteractability();
            m_TeamSettingsAxis.GetComponent<CanvasGroup>().interactable = false;
            m_TeamSettingsAllies.GetComponent<CanvasGroup>().interactable = false;

            m_TeamSettingsAxis.Play("hide");
            m_TeamSettingsAllies.Play("hide");

            m_TeamSettingsBackground.DOColor(
                         new Color(m_TeamSettingsBackground.color.r,
                                     m_TeamSettingsBackground.color.g,
                                        m_TeamSettingsBackground.color.b, 0), .6f).OnComplete(() => {
                                            m_Duel.DOScale(Vector3.one, .5f);
                                            m_SetupAnime.Play("signal_HIDE");
                                            m_MidPanelCGroupAnime.Play("fadeOut");
                                            m_StartGameOptionsCGroupAnime.Play("fadeOut");
                                            m_Duel.DOLocalMoveY(0, .5f).SetEase(Ease.InOutBack).OnComplete(() => {
                                                m_VSCGroupAnime.Play("fadeOut");

                                                // Init Only Config Data
                                                m_MainCanvas.InitMainCanvas(m_GlobalConfigData.__DUEL_SETTINGS__, m_GlobalConfigData.__MATCH_SETTINGS__);
                                                // Get Seperate MOVABLE Components from each Duel Obj (Team Name, Team Emblem, Team Count) -> move with delays
                                                var axis = m_DuelController.GetDuelObjByType(DuelObjectType.Attacker);
                                                var allies = m_DuelController.GetDuelObjByType(DuelObjectType.Defender);

                                                var axisMovableComponents = axis.GetDuelObjectMovableComponents();
                                                var alliesMovableComponents = allies.GetDuelObjectMovableComponents();
                                                // Get STATIC Components (Emblem Shine, TeamName&TeamCount Placeholders) -> alpha to 0
                                                var axisStaticComponents = axis.GetDuelObjectStaticComponents();
                                                var alliesStaticComponents = allies.GetDuelObjectStaticComponents();
                                                // Main Canvas: Pop Up Seperate Static Elements on triggers
                                                var axisTeamHolder = m_MainCanvas.GetAxisTeamHolder();
                                                var alliesTeamHolder = m_MainCanvas.GetAlliesTeamHolder();

                                                // Da Sequence
                                                StartCoroutine(StartDuelTransitionSequence(axisMovableComponents, axisStaticComponents, axisTeamHolder, 
                                                    new List<Transform>() { m_TeamEmblemAxisEndGoToPosition, m_TeamNameAxisEndGoToPosition, m_TeamCountAxisEndGoToPosition }));
                                                StartCoroutine(StartDuelTransitionSequence(alliesMovableComponents, alliesStaticComponents, alliesTeamHolder,
                                                     new List<Transform>() { m_TeamEmblemAlliesEndGoToPosition, m_TeamNameAlliesEndGoToPosition, m_TeamCountAlliesEndGoToPosition }));

                                                OnStartMatchButtonClickedEvent?.Invoke(m_GlobalConfigData);
                                            });
                                        });
        });

        m_StartMatchButton.onClick.AddListener(() => {
            //OnStartMatchButtonClickedEvent?.Invoke();
        });

        m_PlayButton.onClick.AddListener(() => {
            if (m_CurrentState != StartMatchState.Initial)
                return;

            TriggerBehaviour(StartMatchState.ModeSelection);
        });
    }

    private IEnumerator StartDuelTransitionSequence(List<Transform> movableComponents, List<Image> staticComponents, MainTeamHolder teamHolder, List<Transform> endGoToPositionsOrganized)
    {
        for (int i = 0; i < staticComponents.Count; i++)
        {
            staticComponents[i].DOFade(0f, .2f);
        }

        for (int i = 0; i < movableComponents.Count; i++)
        {
            movableComponents[i].DOScale(1.1f, .2f).OnComplete(() => {
                movableComponents[i].DOScale(.5f, .5f);
                movableComponents[i].DOJump(endGoToPositionsOrganized[i].position, 1f, 1, .77f);
                if (i == 0) {
                    movableComponents[i].GetComponent<Image>()?.DOFade(0f, .65f);
                    teamHolder.DoDoScaleIn_Emblem(); 
                }
                if (i == 1)
                {
                    movableComponents[i].GetComponent<TextMeshProUGUI>().DOFade(0f, .65f);
                    teamHolder.DoDoScaleIn_TeamName();
                    teamHolder.DoDoScaleIn_ScoreText();
                }
                if (i == 2) {
                    movableComponents[i].GetComponent<TextMeshProUGUI>().DOFade(0f, .65f);
                    teamHolder.DoDoScaleIn_TeamCount(); 
                }
            });
            yield return new WaitForSeconds(.2f);
        }

    }

    private void Start()
    {
        SetupSceneDefault();
    }

    public void TriggerBehaviour(StartMatchState state)
    {
        m_CurrentState = state;

        switch (state)
        {
            case StartMatchState.Initial:
                Debug.Log($"<color=orange>StartMatchState</color><color=gold>Initial</color>");
                OnFadeOutEffectEvent?.Invoke(() => {
                    SpawnLayout();
                });
                break;
            case StartMatchState.ModeSelection:
                Debug.Log($"<color=orange>StartMatchState</color><color=gold>ModeSelection</color>");
                StartCoroutine(ShowModeSelectionInterfaceSequence());
                break;

            case StartMatchState.MatchSettings:
                Debug.Log($"<color=orange>StartMatchState</color><color=gold>MatchSettings</color>");
                StartCoroutine(ShowMatchSettingsInterfaceSequence());
                break;
            case StartMatchState.Duel:
                Debug.Log($"<color=orange>StartMatchState</color><color=gold>Duel</color>");
                StartCoroutine(ShowDuelInterfaceSequence());
                break;
            case StartMatchState.TeamAConfig:
            case StartMatchState.TeamBConfig:
                {
                    var log = state == StartMatchState.TeamAConfig ? "TeamAConfig" : "TeamBConfig";
                    Debug.Log($"<color=orange>StartMatchState</color><color=gold>{log}</color>");

                    if (m_CanPlaySetupAnime)
                    {
                        m_SelectTeamsAnime.Play("signal_HIDE");
                        m_SetupAnime.Play();

                        m_Duel.DOScale(new Vector3(.7f, .7f, .7f), .5f);
                        m_Duel.DOLocalMoveY(-200f, .5f).SetEase(Ease.InOutBack).OnComplete(() => {
                            m_TeamSettingsBackground.DOColor(
                                new Color(m_TeamSettingsBackground.color.r,
                                       m_TeamSettingsBackground.color.g,
                                           m_TeamSettingsBackground.color.b, 1f), .5f);

                            var settings = state == StartMatchState.TeamAConfig ? m_TeamSettingsAxis : m_TeamSettingsAllies;
                            settings?.Play("show");
                            m_TeamSettingsCanvasGroupRef = settings?.GetComponent<CanvasGroup>();
                            m_TeamSettingsCanvasGroupRef.blocksRaycasts = true;
                        });

                        m_ReadyButton.transform.DOScale(1f, .5f).SetEase(Ease.InOutExpo);

                        m_CanPlaySetupAnime = false;
                    } else
                    {
                        StartCoroutine(ShowTeamConfig(m_CurrentState));
                    }
                }
                break;
            default:
                break;
        }
    }

    private IEnumerator ShowDuelInterfaceSequence()
    {
        m_MatchSettingsParent.gameObject.SetActive(false);
        m_DuelParent.gameObject.SetActive(true);

        m_MatchSettingsAnime.Play("signal_HIDE");
        yield return new WaitForSeconds(.15f);
        m_SelectTeamsAnime.Play("signal_SHOW");
        yield return new WaitForSeconds(.3f);
        m_Duel.DOScale(1f, .3f);
    }

    private IEnumerator ShowMatchSettingsInterfaceSequence()
    {
        m_GameMode.gameObject.SetActive(false);
        m_MatchSettingsParent.gameObject.SetActive(true);

        m_MatchSettingsController.TriggerBehaviour(MatchSettingsStateType.GameTime);

        m_GameModeAnime.Play("signal_HIDE");
        yield return new WaitForSeconds(.15f);
        m_MatchSettingsAnime.Play("signal_SHOW");
        yield return new WaitForSeconds(.3f);
        m_MatchSettingsParent.DOScale(1f, .3f);
    }

    private IEnumerator ShowTeamConfig(StartMatchState teamConfig)
    {
        yield return new WaitForSeconds(.15f);

        var (settingShow, settingHide) = 
            teamConfig == StartMatchState.TeamAConfig ? (m_TeamSettingsAxis, m_TeamSettingsAllies) : 
            teamConfig == StartMatchState.TeamBConfig ? (m_TeamSettingsAllies, m_TeamSettingsAxis) : (null, null);

        settingHide?.Play("hide");
        m_TeamSettingsCanvasGroupRef = settingHide?.GetComponent<CanvasGroup>();
        m_TeamSettingsCanvasGroupRef.blocksRaycasts = false;
        settingShow?.Play("show");
        m_TeamSettingsCanvasGroupRef = settingShow?.GetComponent<CanvasGroup>();
        m_TeamSettingsCanvasGroupRef.blocksRaycasts = true;
    }


    private IEnumerator ShowModeSelectionInterfaceSequence()
    {
        foreach (var item in m_StartGameOptionItems)
        {
            item.DOScale(0f, .15f)
                .SetEase(Ease.OutSine);

            yield return new WaitForSeconds(.1f);
        }

        m_GameTile.DOScale(0, .4f).OnComplete(() =>
        {
            m_GameModeAnime.Play("signal_SHOW");
        });
        m_RectGameOptions.DOSizeDelta(new Vector2(m_RectGameOptions.sizeDelta.x, 787f), 1f).OnComplete(() =>
        {
            m_GameMode.DOScale(1f, .77f).SetEase(Ease.InOutQuart);
        });
    }


    private void SpawnLayout()
    {
        m_GameTile.DOScale(new Vector3(1.2f,1.2f,1.2f), .77f)
            .SetEase(Ease.InCubic)
            .OnComplete(() => {
                m_BackgroundImage.DOColor(new Color(
                    m_BackgroundImage.color.r, 
                    m_BackgroundImage.color.g,
                    m_BackgroundImage.color.b,
                    .77f), 1f);
                m_GameTile.DOScale(Vector3.one, 1f).SetEase(Ease.InCubic);
                m_GameTile.DOMoveY(m_GameTileEndPoint.position.y, 1f);

                StartCoroutine(StartGameSequenceEffect());
            });
    }

    private void SetupSceneDefault()
    {
        m_GameTile.localScale = Vector3.zero;
        m_GameTile.localPosition = Vector3.zero;

        m_BackgroundImage.color = new Color(1, 1, 1, 0);

        m_StartGameOptions.transform.localScale = Vector3.zero;
        m_GameMode.localScale = Vector3.zero;

        m_StartGameOptionItems.ForEach((item) => {
            item.localScale = Vector3.zero;
        });

        m_Duel.localScale = Vector3.zero;
        m_Duel.localPosition = Vector3.zero;

        m_TeamSettingsAxis.GetComponent<CanvasGroup>().alpha = 0;
        m_TeamSettingsAllies.GetComponent<CanvasGroup>().alpha = 0;
        m_TeamSettingsAxis.GetComponent<CanvasGroup>().interactable = true;
        m_TeamSettingsAllies.GetComponent<CanvasGroup>().interactable = true;
        m_TeamSettingsBackground.color = new Color(m_TeamSettingsBackground.color.r, m_TeamSettingsBackground.color.g, m_TeamSettingsBackground.color.b, 0f);
        m_CanPlaySetupAnime = true;

        m_ReadyText = m_ReadyButton.GetComponentInChildren<TextMeshProUGUI>();
        m_ReadyButton.interactable = false;
        m_ReadyText.color = new Color(m_ReadyText.color.r, m_ReadyText.color.g, m_ReadyText.color.b, .5f);
        m_ReadyButton.transform.localScale = Vector3.zero;


        m_RectGameOptions = m_StartGameOptions.GetComponent<RectTransform>();

        m_RectGameOptions.sizeDelta = new Vector2(m_RectGameOptions.sizeDelta.x, 531.2f);
    }

    private IEnumerator StartGameSequenceEffect()
    {
        m_StartGameOptions.SetActive(true);
        m_StartGameOptions.transform.DOScale(1f, .3f)
                .SetEase(Ease.InSine);

        yield return new WaitForSeconds(.35f);

        foreach (var item in m_StartGameOptionItems)
        {
            item.DOScale(1f, .3f)
                .SetEase(Ease.InSine);

            yield return new WaitForSeconds(.15f);
        }
    }

    public void DisableStartMatchCanvas()
    {
        m_StartMatchCanvas.enabled = false;
        m_StartMatchCanvas.gameObject.SetActive(false);
    }

}
