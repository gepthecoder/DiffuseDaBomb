using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class MatchSettingsItem : MonoBehaviour
{
    public MatchSettingsStateType Type = MatchSettingsStateType.None;

    [SerializeField] private Animator m_MatchSettingsItemAnimator;
    [SerializeField] private CanvasGroup m_MatchSettingsItemCGroup;
    [Space(5)]
    [SerializeField] public Button m_LeftButton;
    [SerializeField] public Button m_RightButton;
    [Space(5)]
    [SerializeField] public TextMeshProUGUI m_ValueText;

    [HideInInspector] public int m_Value;
    public int m_DefaultValue = 5;
    public int m_DefaultMinValue = 2;
    public int m_DefaultMaxValue = 500;
    public bool m_EvenNumbers = false;

    private bool m_CanInteractWithArrows = true;

    [HideInInspector] public UnityEvent<int> OnMatchSettingsItemValueChangedEvent = new UnityEvent<int>();

    private void Awake()
    {
        m_Value = m_DefaultValue;
        m_ValueText.text = m_Value.ToString();

        Sub();
    }

    public void InitSettingsItemValue(float val, bool enableArrow = true)
    {
        m_Value = (int)val;
        m_ValueText.text = $"{m_Value}";

        EnableArrowInteraction(enableArrow);
    }

    public void OnShowItem()
    {
        EnableArrowInteraction(true);
        m_MatchSettingsItemAnimator.Play("show");
    }

    public void OnHideItem()
    {
        m_MatchSettingsItemAnimator.Play("hide");
    }

    public CanvasGroup GetCanvasGroup()
    {
        return m_MatchSettingsItemCGroup;
    }

    public void EnableArrowInteraction(bool enable)
    {
        m_CanInteractWithArrows = enable;
    }

    public void Sub()
    {
        EventTrigger triggerLeftArrow = m_LeftButton.GetComponent<EventTrigger>();
        var pointerDownLeftArrow = new EventTrigger.Entry();
        pointerDownLeftArrow.eventID = EventTriggerType.PointerDown;

        pointerDownLeftArrow.callback.AddListener((e) => {
            if (!m_CanInteractWithArrows)
                return;

            if (m_EvenNumbers)
                m_Value -= 2;
            else
                m_Value--;

            if (m_Value < m_DefaultMinValue) { m_Value = m_DefaultMinValue; }

            m_ValueText.transform.DOLocalMoveY(-80f, .15f).SetEase(Ease.InFlash)
            .OnComplete(() => {
                m_ValueText.text = m_Value.ToString();
                m_ValueText.transform.DOLocalMoveY(-98f, .15f).SetEase(Ease.InFlash);

            });

            OnMatchSettingsItemValueChangedEvent?.Invoke(m_Value);
        });
        triggerLeftArrow.triggers.Add(pointerDownLeftArrow);

        EventTrigger triggerRightArrow = m_RightButton.GetComponent<EventTrigger>();
        var pointerDownRightArrow = new EventTrigger.Entry();
        pointerDownRightArrow.eventID = EventTriggerType.PointerDown;

        pointerDownRightArrow.callback.AddListener((e) => {
            if (!m_CanInteractWithArrows)
                return;

            if (m_EvenNumbers)
                m_Value += 2;
            else
                m_Value++;

            if (m_Value > m_DefaultMaxValue) { m_Value = m_DefaultMaxValue; }

            m_ValueText.transform.DOLocalMoveY(-80, .15f).SetEase(Ease.InFlash)
            .OnComplete(() => {
                m_ValueText.text = m_Value.ToString();
                m_ValueText.transform.DOLocalMoveY(-98f, .15f).SetEase(Ease.InFlash);

            });

            OnMatchSettingsItemValueChangedEvent?.Invoke(m_Value);
        });
        triggerRightArrow.triggers.Add(pointerDownRightArrow);
    }

}
