using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using DG.Tweening;
using System;

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
    [SerializeField] private Transform m_Duel;
    [SerializeField] private Transform m_TeamSettings;
    [SerializeField] private Image m_TeamSettingsBackground;
    //


    [HideInInspector] public UnityEvent OnStartMatchButtonClickedEvent = new UnityEvent();
    [HideInInspector] public UnityEvent<Action> OnFadeOutEffectEvent = new UnityEvent<Action>();

    private RectTransform m_RectGameOptions;
    private StartMatchState m_CurrentState;

    private void Awake()
    {
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
                StartCoroutine(ShowTeamConfig(m_CurrentState));
                break;
            default:
                break;
        }
    }

    private IEnumerator ShowTeamConfig(StartMatchState teamConfig)
    {
        m_Duel.DOScale(new Vector3(.7f, .7f, .7f), .5f);
        m_Duel.DOLocalMoveY(-230f, .5f).SetEase(Ease.InOutBack);

        m_TeamSettingsBackground.DOColor(
            new Color(m_TeamSettingsBackground.color.r,
                        m_TeamSettingsBackground.color.g,
                            m_TeamSettingsBackground.color.b, 1f), .6f)
                                .OnComplete(() => {
                                    m_TeamSettings.DOScale(Vector3.one, .5f);
                                });
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
            m_SelectTeamsAnime.Play();
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

        m_TeamSettings.localScale = Vector3.zero;
        m_TeamSettingsBackground.color = new Color(m_TeamSettingsBackground.color.r, m_TeamSettingsBackground.color.g, m_TeamSettingsBackground.color.b, 0f);

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
