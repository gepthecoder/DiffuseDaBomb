using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class GameModeController : MonoBehaviour
{
    [SerializeField] private List<GameModeObject> m_GameModes;
    [SerializeField] private Button m_NextButton;
    [SerializeField] private Button m_PrevButton;

    [HideInInspector] public UnityEvent<GameModeType> OnGameModeSelectedFinalEvent = new UnityEvent<GameModeType>();
    [HideInInspector] public UnityEvent OnPreviousButtonClickedEvent = new UnityEvent();

    private GameModeType m_CurrentSelectedGameMode = GameModeType.None;

    private bool m_IsNextButtonShown = false;

    private void Awake()
    {
        m_GameModes.ForEach((item) =>
        {
            item.OnGameModeSelectedEvent.AddListener((type) => {
                OnGameModeSelected(type);
            });
        });

        m_NextButton.onClick.AddListener(() =>
        {
            m_NextButton.interactable = false;

            m_NextButton.transform.DOScale(1.15f, .25f).OnComplete(() => {
                m_NextButton.transform.DOScale(0f, .5f);
            });

            m_PrevButton.transform.DOScale(1.15f, .25f).OnComplete(() => {
                m_PrevButton.transform.DOScale(0f, .5f);
            });

            StartCoroutine(HideGameModeSequence());
        });

        m_PrevButton.onClick.AddListener(() => {
            m_PrevButton.interactable = false;

            m_NextButton.transform.DOScale(1.15f, .25f).OnComplete(() => {
                m_NextButton.transform.DOScale(0f, .5f);
            });

            m_PrevButton.transform.DOScale(1.15f, .25f).OnComplete(() => {
                m_PrevButton.transform.DOScale(0f, .5f);
            });

            StartCoroutine(HideGameModeSequence_TransitionBack());

            OnPreviousButtonClickedEvent?.Invoke();
        });
    }

    private void Start()
    {
        m_NextButton.transform.localScale = Vector3.zero;
    }

    public void Init()
    {
        StartCoroutine(WaitAndShowModule());
    }

    private IEnumerator WaitAndShowModule()
    {
        yield return new WaitForSeconds(2f);

        m_PrevButton.transform.DOScale(1.15f, .77f).OnComplete(() => {
            m_PrevButton.transform.DOScale(1f, .25f);
            m_PrevButton.interactable = true;
        });

        foreach (var item in m_GameModes)
        {
            item.transform.DOScale(.9f, .77f).SetEase(Ease.InSine).OnComplete(() => {
                item.transform.DOScale(.77f, .5f).SetEase(Ease.InExpo);
            });
            yield return new WaitForSeconds(.7f);
        }
    }

    private IEnumerator HideGameModeSequence_TransitionBack()
    {
        foreach (var item in m_GameModes)
        {
            item.transform.DOScale(0f, .7f).SetEase(Ease.InExpo);
            yield return new WaitForSeconds(.7f);
        }
    }

    private IEnumerator HideGameModeSequence()
    {
        Transform selected = null;

        foreach (var item in m_GameModes)
        {
            if (item.Type == m_CurrentSelectedGameMode) { 
                selected = item.transform;
                continue;
            }
            item.transform.DOScale(0f, .7f).SetEase(Ease.InExpo);
        }

        yield return new WaitForSeconds(.7f);

        selected?.DOScale(0f, .7f).SetEase(Ease.InExpo).OnComplete(() => {
            OnGameModeSelectedFinalEvent?.Invoke(m_CurrentSelectedGameMode);
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

        HandleNextButton(type);
    }

    private void HandleNextButton(GameModeType type)
    {
        if(!m_IsNextButtonShown)
        {
            m_NextButton.transform.DOScale(1.15f, .77f).OnComplete(() => {
                m_NextButton.transform.DOScale(1f, .25f);
            });

            m_IsNextButtonShown = true;
        }

        switch (type)
        {
            case GameModeType.OriginalEdition:
                m_NextButton.interactable = true;
                break;
            case GameModeType.SpecialEdition:
                m_NextButton.interactable = false;
                break;
            case GameModeType.None:
            default:
                m_NextButton.interactable = false;
                break;
        }
    }

    private void OnDestroy()
    {
        m_GameModes.ForEach((item) =>
        {
            item.OnGameModeSelectedEvent.RemoveAllListeners();
        });

        m_NextButton.onClick.RemoveAllListeners();
        m_PrevButton.onClick.RemoveAllListeners();
    }
}
