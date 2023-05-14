using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public enum DeveloperItemType { TimeMenu, LogMenu, TriggerMenu, }

// BASE
public class DeveloperItem : MonoBehaviour
{
    public DeveloperItemType ID;

    [SerializeField] private Image m_ButtonImage;

    public void InitItem() {
        Init();
    }

    public void DeinitItem() {
        Deinit();
    }

    public virtual void Init() {
        EnableItem(true);
    }
    public virtual void Deinit() {
        EnableItem(false);
    }

    private void EnableItem(bool enable) {
        gameObject.SetActive(enable);
        m_ButtonImage.DOColor(new Color(m_ButtonImage.color.r, m_ButtonImage.color.g, m_ButtonImage.color.b,
            enable ? 1f : .5f), .5f);
    }
}
