using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System;
using UnityEngine.Events;

public class RepairBombManager : MonoBehaviour
{
    [HideInInspector] public UnityEvent OnBombRepairCompleted = new UnityEvent();

    [SerializeField] private RepairHelper m_RepairHelper;

    [SerializeField] private Transform m_RepairBombParent;
    [SerializeField] private Button m_RepairBombButton;
    [SerializeField] private Image m_RepairBombFillImage;

    private bool m_OnRepairBtnPointerDownEvent; // set through event trigger component
    private bool m_IsInitialized = false;

    private float m_OnDownTimer;
    private float m_OnDownThreshold = 5f;

    private void Awake()
    {
        m_RepairBombButton.interactable = false;
    }

    private void Update()
    {
        if(m_IsInitialized)
        {
            if (m_OnRepairBtnPointerDownEvent)
            {
                m_OnDownTimer += Time.deltaTime;
            }
            else
            {
                if (m_OnDownTimer > 0)
                {
                    m_OnDownTimer -= Time.deltaTime * 2;
                }

                if (m_OnDownTimer <= 0f)
                {
                    m_OnDownTimer = 0f;
                }
            }

            // Update Fill
            UpdateRepairBombBtnFill(m_OnDownTimer);

            // Update Repair State
            UpdateRepairState(m_OnDownTimer, m_OnDownThreshold);

            if(m_OnDownTimer >= m_OnDownThreshold)
            {
                // Suitcase is Repaired
                m_OnDownTimer = m_OnDownThreshold;

                // TRIGGER ON REPAIR COMPLETED
                OnBombRepairCompleted?.Invoke();
                
                Deinit();
            }
        }
    }

    private void UpdateRepairState(float onDownTimer, float onDownThreshold)
    {
        m_RepairHelper.RepairBomb(onDownTimer / onDownThreshold);
        m_RepairHelper.SetSmokeAlpha(onDownTimer / (onDownThreshold / 2f));
    }

    private void UpdateRepairBombBtnFill(float onDownTimer)
    {
        float norValue =  onDownTimer / m_OnDownThreshold;
        float currentFillAmount = m_RepairBombFillImage.fillAmount;

        m_RepairBombFillImage.fillAmount = Mathf.Lerp(currentFillAmount, norValue, .2f);
    }

    internal void Init()
    {
        // Show Button
        m_RepairBombParent.DOScale(1.1f, .5f).SetEase(Ease.InExpo)
        .OnComplete(() => {
            m_IsInitialized = true;
            m_RepairBombButton.interactable = true;
            m_RepairBombParent.DOScale(1f, .25f).SetEase(Ease.OutExpo); 
        });
    }

    internal void Deinit()
    {
        // Hide Button
        m_IsInitialized = false;
        m_RepairBombButton.interactable = false;

        m_RepairBombParent.DOScale(1.1f, .25f).SetEase(Ease.InOutBack)
            .OnComplete(() => {
                m_RepairBombParent.DOScale(0, .5f);
            });
    }

    public void SetPointerDownInfo(bool down) // Event Trigger On Object
    {
        m_OnRepairBtnPointerDownEvent = down;
    }
}
