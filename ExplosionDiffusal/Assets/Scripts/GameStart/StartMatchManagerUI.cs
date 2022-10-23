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
            default:
                break;
        }
    }


    private IEnumerator ShowPlayMatchMainScene()
    {
        foreach (var item in m_StartGameOptionItems)
        {
            item.DOScale(0f, .15f)
                .SetEase(Ease.OutSine);

            yield return new WaitForSeconds(.1f);
        }


        m_GameTile.DOScale(0, .4f);
        m_RectGameOptions.DOSizeDelta(new Vector2(m_RectGameOptions.sizeDelta.x, 787f), 1f);
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

        m_RectGameOptions = m_StartGameOptions.GetComponent<RectTransform>();
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
