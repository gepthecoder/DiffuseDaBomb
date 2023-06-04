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
    [Header("IAP")]
    [SerializeField] private Button m_IAPButtonShow;
    [SerializeField] private Button m_IAPButtonHide;
    [Header("START MATCH")]
    [SerializeField] private Button m_StartMatchButton;
    [Header("PRE MATCH")]
    [SerializeField] private Transform m_GameTile;
    [SerializeField] private Transform m_GameTileEndPoint;
    [SerializeField] private Image m_BackgroundImage;
    [Header("BG SMOKE")]
    [SerializeField] private Camera m_ParticleSystemSmokeCamera;
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

        m_MatchSettingsController.OnBackToGameModeEvent.AddListener(() => {
            StartCoroutine(TransitionBackToGameModeSequence());
        });

        m_GameModeController.OnGameModeSelectedFinalEvent.AddListener((MODE) => {
            m_GlobalConfigData.__GAME_MODE_TYPE__ = MODE;
            TriggerBehaviour(StartMatchState.MatchSettings);
        });

        m_GameModeController.OnPreviousButtonClickedEvent.AddListener(() => {
            StartCoroutine(TransitionBackToMainMenu());
        });

        m_DuelController.OnPreviousButtonClickedEvent.AddListener(() => {
            StartCoroutine(TransitionToMatchSettings());
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

            NavigationManager.instance.ShowMenuNavigation(false);

            StartCoroutine(TurnOffSmoke());

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
                                                
                                                // Get Objects
                                                var axis = m_DuelController.GetDuelObjByType(DuelObjectType.Attacker);
                                                var allies = m_DuelController.GetDuelObjByType(DuelObjectType.Defender);

                                                var axisMovableComponents = axis.GetDuelObjectMovableComponents();
                                                var alliesMovableComponents = allies.GetDuelObjectMovableComponents();

                                                var axisStaticComponents = axis.GetDuelObjectStaticComponents();
                                                var alliesStaticComponents = allies.GetDuelObjectStaticComponents();

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

        m_IAPButtonShow.onClick.AddListener(() => {
            if (m_CurrentState != StartMatchState.Initial)
                return;
            OnIAPButtonClicked();
        });

        m_IAPButtonHide.onClick.AddListener(() => {
            if (m_CurrentState != StartMatchState.Initial)
                return;
            OnIAPButtonCloseClicked();
        });
    }

    private void Start()
    {
        SetupSceneDefault();
    }

    public void TriggerBehaviour(StartMatchState state, bool goToPrevious = false)
    {
        m_CurrentState = state;

        switch (state)
        {
            case StartMatchState.Initial:
                Debug.Log($"<color=orange>StartMatchState</color><color=gold>Initial</color>");
                if(goToPrevious)
                {
                    BackgroundManager.INSTANCE.TriggerBackgroundChanged(true);
                    SpawnLayout();
                }
                else
                {
                    OnFadeOutEffectEvent?.Invoke(() => {
                        SpawnLayout();
                    });
                }
                break;
            case StartMatchState.ModeSelection:
                Debug.Log($"<color=orange>StartMatchState</color><color=gold>ModeSelection</color>");
                BackgroundManager.INSTANCE.TriggerBackgroundChanged();
                StartCoroutine(ShowModeSelectionInterfaceSequence());
                break;
            case StartMatchState.MatchSettings:
                Debug.Log($"<color=orange>StartMatchState</color><color=gold>MatchSettings</color>");
                BackgroundManager.INSTANCE.TriggerBackgroundChanged();
                if(goToPrevious)
                {
                    ShowMatchSettingsInterfaceSequence_v2();
                }else
                {
                    StartCoroutine(ShowMatchSettingsInterfaceSequence());
                }
                break;
            case StartMatchState.Duel:
                Debug.Log($"<color=orange>StartMatchState</color><color=gold>Duel</color>");
                BackgroundManager.INSTANCE.TriggerBackgroundChanged();
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
                                           m_TeamSettingsBackground.color.b, .6f), .5f);

                            var settings = state == StartMatchState.TeamAConfig ? m_TeamSettingsAxis : m_TeamSettingsAllies;
                            settings?.Play("show");
                            m_TeamSettingsCanvasGroupRef = settings?.GetComponent<CanvasGroup>();
                            m_TeamSettingsCanvasGroupRef.blocksRaycasts = true;
                        });

                        m_ReadyButton.transform.DOScale(1f, .5f).SetEase(Ease.InOutExpo);

                        m_CanPlaySetupAnime = false;

                        m_DuelController.Init();
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

    private IEnumerator TransitionToMatchSettings()
    {
        // HIDE DUEL
        m_DuelController.Deinit();

        m_SetupAnime.Play("signal_HIDE");
        yield return new WaitForSeconds(.15f);
        m_MatchSettingsAnime.Play("signal_SHOW");

        m_TeamSettingsBackground.DOColor(
                               new Color(m_TeamSettingsBackground.color.r,
                                      m_TeamSettingsBackground.color.g,
                                          m_TeamSettingsBackground.color.b, 0f), .5f).OnComplete(() => {
                                              m_Duel.DOScale(Vector3.zero, .5f);
                                              m_Duel.DOLocalMoveY(0f, .5f).SetEase(Ease.InOutBack).OnComplete(() => {
                                                  m_DuelParent.gameObject.SetActive(false);
                                              });
                                          });

        m_ReadyButton.transform.DOScale(0f, .5f).SetEase(Ease.InOutExpo);
        m_TeamSettingsCanvasGroupRef.blocksRaycasts = false;

        m_CanPlaySetupAnime = true;

        m_TeamSettingsAxis?.Play("hide");
        m_TeamSettingsCanvasGroupRef = m_TeamSettingsAxis?.GetComponent<CanvasGroup>();
        m_TeamSettingsCanvasGroupRef.blocksRaycasts = false;

        m_TeamSettingsAllies?.Play("hide");
        m_TeamSettingsCanvasGroupRef = m_TeamSettingsAllies?.GetComponent<CanvasGroup>();
        m_TeamSettingsCanvasGroupRef.blocksRaycasts = false;

        yield return new WaitForSeconds(1f);

        // SHOW MATCH SETTINGS
        TriggerBehaviour(StartMatchState.MatchSettings, true);
    }

    private IEnumerator TransitionBackToMainMenu()
    {
        m_GameModeAnime.Play("signal_HIDE");

        NavigationManager.instance.ShowMenuNavigation(false);
        m_GameModeController.Deinit();

        yield return new WaitForSeconds(.5f);

        m_GameMode.DOScale(0f, .77f).SetEase(Ease.InOutQuart).OnComplete(() => {
            TriggerBehaviour(StartMatchState.Initial, true);
            m_RectGameOptions.DOSizeDelta(new Vector2(m_RectGameOptions.sizeDelta.x, 531.2f), .77f).OnComplete(() => {
            });
        });
    }

    private IEnumerator TurnOffSmoke()
    {
        float waitTime = 1f;
        float elapsedTime = 0;

        var currentColor = m_ParticleSystemSmokeCamera.backgroundColor;
        var alphaZeroColor = new Color(
            m_ParticleSystemSmokeCamera.backgroundColor.r,
            m_ParticleSystemSmokeCamera.backgroundColor.g,
            m_ParticleSystemSmokeCamera.backgroundColor.b,
            0f
        );

        while (elapsedTime < waitTime)
        {
            m_ParticleSystemSmokeCamera.backgroundColor = Color.Lerp(currentColor, alphaZeroColor, elapsedTime / waitTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Make sure we got there
        m_ParticleSystemSmokeCamera.backgroundColor = alphaZeroColor;
        yield return null;
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
                if (i == 0)
                {
                    movableComponents[i].GetComponent<Image>()?.DOFade(0f, .65f);
                    teamHolder.DoDoScaleIn_Emblem();
                }
                if (i == 1)
                {
                    movableComponents[i].GetComponent<TextMeshProUGUI>().DOFade(0f, .65f);
                    teamHolder.DoDoScaleIn_TeamName();
                    teamHolder.DoDoScaleIn_ScoreText();
                }
                if (i == 2)
                {
                    movableComponents[i].GetComponent<TextMeshProUGUI>().DOFade(0f, .65f);
                    teamHolder.DoDoScaleIn_TeamCount();
                    teamHolder.DoDoScaleIn_AttckDefObjects();
                }
            });
            yield return new WaitForSeconds(.2f);
        }

    }

    private IEnumerator ShowDuelInterfaceSequence()
    {
        NavigationManager.instance.SetNavigationPointerByState(StartMatchState.Duel);

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

    private void ShowMatchSettingsInterfaceSequence_v2()
    {
        m_MatchSettingsParent.gameObject.SetActive(true);

        m_MatchSettingsController.TriggerBehaviour(MatchSettingsStateType.ScoreLimit);
        m_MatchSettingsParent.DOScale(1f, .3f);
    }

    private IEnumerator TransitionBackToGameModeSequence()
    {
        m_MatchSettingsParent.DOScale(0f, .5f).OnComplete(() => {
            m_MatchSettingsParent.gameObject.SetActive(false);
        });

        m_GameMode.gameObject.SetActive(true);

        m_MatchSettingsAnime.Play("signal_HIDE");
        yield return new WaitForSeconds(.15f);
        m_GameModeAnime.Play("signal_SHOW");
        yield return new WaitForSeconds(.3f);

        TriggerBehaviour(StartMatchState.ModeSelection);
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
        // TODO: check for history
        if(m_IsIAPOpened) {
            OnIAPButtonCloseClicked();
        }

        NavigationManager.instance.SetNavigationPointerByState(StartMatchState.ModeSelection);

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

        m_GameMode.DOScale(1f, .77f).SetEase(Ease.InOutQuart).OnComplete(() => {
            m_GameModeController.Init();
        });

        m_RectGameOptions.DOSizeDelta(new Vector2(m_RectGameOptions.sizeDelta.x, 787f), 1f);
    }


    private void SpawnLayout()
    {
        m_GameTile.DOScale(new Vector3(1.2f,1.2f,1.2f), .65f)
            .SetEase(Ease.InCubic)
            .OnComplete(() => {
                m_BackgroundImage.DOColor(new Color(
                    m_BackgroundImage.color.r, 
                    m_BackgroundImage.color.g,
                    m_BackgroundImage.color.b,
                    .77f), 1f);
                m_GameTile.DOScale(Vector3.one, 1f).SetEase(Ease.InCubic);
                m_GameTile.DOMoveY(m_GameTileEndPoint.position.y, .65f);

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
        yield return new WaitForEndOfFrame();

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


    // IAP
    private bool m_IsIAPOpened = false;
    private void OnIAPButtonClicked()
    {
        if (m_IsIAPOpened)
            return;

        // HIDE BUTTON
        m_IAPButtonShow.transform.parent.transform.DOScale(0f, .2f).SetEase(Ease.InExpo);
        // show iap
        IAPManager.INSTANCE.ShowIAP();

        m_IsIAPOpened = true;
    }

    public void OnIAPButtonCloseClicked()
    {
        if (!m_IsIAPOpened)
            return;

        // SHOW BUTTON
        m_IAPButtonShow.transform.parent.transform.DOScale(1f, .5f).SetEase(Ease.InExpo);
        // hide iap
        IAPManager.INSTANCE.HideIAP();

        m_IsIAPOpened = false;
    }
    //
}
