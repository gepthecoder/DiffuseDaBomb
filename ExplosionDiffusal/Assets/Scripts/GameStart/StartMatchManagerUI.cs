using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using DG.Tweening;
using System;
using TMPro;

public class StartMatchManagerUI : MonoBehaviour
{
    
    [Header("START MATCH")]
    [SerializeField] private Button m_StartMatchButton;
    [Header("PRE MATCH")]
    [SerializeField] private Transform m_GameTile;
    [SerializeField] private Transform m_GameTileEndPoint;
    [SerializeField] private Image m_BackgroundImage;
    //
    [SerializeField] private GameObject m_StartGameOptions;
    [SerializeField] private List<Transform> m_StartGameOptionItems;
    //
    [SerializeField] private Button m_PlayButton;
    //
    [SerializeField] private Animation m_SelectTeamsAnime;
    [SerializeField] private Animation m_SetupAnime;
    //
    [SerializeField] private Transform m_Duel;
    [SerializeField] private Animator m_TeamSettingsAxis;
    [SerializeField] private Animator m_TeamSettingsAllies;
    private CanvasGroup m_TeamSettingsCanvasGroupRef = new CanvasGroup();
    [SerializeField] private Image m_TeamSettingsBackground;
    //
    [SerializeField] private Button m_ReadyButton;
    private TextMeshProUGUI m_ReadyText;
    //
    [Header("CONFIG - Duel")]
    [SerializeField] private DuelController m_DuelController;

    [HideInInspector] public UnityEvent OnStartMatchButtonClickedEvent = new UnityEvent();
    [HideInInspector] public UnityEvent<Action> OnFadeOutEffectEvent = new UnityEvent<Action>();

    private RectTransform m_RectGameOptions;
    private StartMatchState m_CurrentState;

    private bool m_CanPlaySetupAnime = true;

    private void Awake()
    {
        m_DuelController.OnDuelConfigSetEvent.AddListener(() => {

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

        });

        m_ReadyButton.onClick.AddListener(() => {
            Debug.Log("Ready!!");

            m_ReadyButton.transform.DOScale(0f, .3f).SetEase(Ease.InQuart);

            m_DuelController.GetDuelObjByType(DuelObjectType.Attacker).OnDeSelected();
            m_DuelController.GetDuelObjByType(DuelObjectType.Defender).OnDeSelected();

            m_TeamSettingsAxis.Play("hide");
            m_TeamSettingsAllies.Play("hide");

            m_TeamSettingsBackground.DOColor(
                         new Color(m_TeamSettingsBackground.color.r,
                                     m_TeamSettingsBackground.color.g,
                                         m_TeamSettingsBackground.color.b, 0), .6f).OnComplete(() => {
                                             m_Duel.DOScale(Vector3.one, .5f);
                                             m_Duel.DOLocalMoveY(0, .5f).SetEase(Ease.InOutBack);
                                         });

        });

        m_StartMatchButton.onClick.AddListener(() => {
            OnStartMatchButtonClickedEvent?.Invoke();
        });

        m_PlayButton.onClick.AddListener(() => {
            if (m_CurrentState != StartMatchState.Initial)
                return;

            TriggerBehaviour(StartMatchState.PlayMatchMain);
        });
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
                OnFadeOutEffectEvent?.Invoke(() => {
                    SpawnLayout();
                });
                break;
            case StartMatchState.PlayMatchMain:
                StartCoroutine(ShowPlayMatchMainScene());
                break;
            case StartMatchState.TeamAConfig:
            case StartMatchState.TeamBConfig:
                {
                    if(m_CanPlaySetupAnime)
                    {
                        m_SelectTeamsAnime.Play("select_teams_HIDE");
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
        yield break;
    }


    private IEnumerator ShowPlayMatchMainScene()
    {
        foreach (var item in m_StartGameOptionItems)
        {
            item.DOScale(0f, .15f)
                .SetEase(Ease.OutSine);

            yield return new WaitForSeconds(.1f);
        }

        m_GameTile.DOScale(0, .4f).OnComplete(() => {
            m_SelectTeamsAnime.Play("select_teams");
        }); ;
        m_RectGameOptions.DOSizeDelta(new Vector2(m_RectGameOptions.sizeDelta.x, 787f), 1f).OnComplete(() => {
            m_Duel.DOScale(1f, .3f);
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

        m_StartGameOptionItems.ForEach((item) => {
            item.localScale = Vector3.zero;
        });

        m_Duel.localScale = Vector3.zero;
        m_Duel.localPosition = Vector3.zero;

        m_TeamSettingsAxis.GetComponent<CanvasGroup>().alpha = 0;
        m_TeamSettingsAllies.GetComponent<CanvasGroup>().alpha = 0;
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

}
