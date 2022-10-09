using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class BombOpeningUiManager : MonoBehaviour
{
    [SerializeField] private GameObject m_BombOpeningParent;
    private Slider m_BombOpeningSlider;

    private bool m_CanShow = true;
    private float m_ShakeIntensity = 0;

    private void Awake()
    {
        if(m_CanShow)
        {
            m_BombOpeningSlider = m_BombOpeningParent.GetComponentInChildren<Slider>();
            ShowSlider(false);
        }
        else
        {
            m_BombOpeningParent.SetActive(false);
        }
    }

    public void ShowSlider(bool show)
    {
        if (m_BombOpeningSlider.gameObject.activeSelf == show)
            return;

        m_BombOpeningSlider.gameObject.SetActive(show);
    }

    private bool bitVal = false;
    public void SetSliderValue(float value)
    {
        m_BombOpeningSlider.value = value;
        bitVal = !bitVal;
        m_BombOpeningSlider.transform.DOBlendablePunchRotation(new Vector3(0, 0, bitVal ? -3 : 3), value / m_BombOpeningSlider.maxValue);
    }

    public void SetupSlider(float onDownTreshold)
    {
        m_BombOpeningSlider.maxValue = Mathf.Round(onDownTreshold);
    }
}
