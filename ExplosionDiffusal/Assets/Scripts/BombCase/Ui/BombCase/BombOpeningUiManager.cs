using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BombOpeningUiManager : MonoBehaviour
{
    [SerializeField] private GameObject m_BombOpeningParent;
    private Slider m_BombOpeningSlider;

    private bool m_CanShow = true;

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

    public void SetSliderValue(float value)
    {
        m_BombOpeningSlider.value = value;
    }
}
