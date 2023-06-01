using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class BackgroundManager : MonoBehaviour
{
    public static BackgroundManager INSTANCE;

    [SerializeField] private BGHelper m_BGHelper;
    [SerializeField] private Image m_BackgroundImage;
    [SerializeField] private List<Sprite> m_BGSprites;

    private int m_CurrentBGSpriteIndex = 0;

    private void Awake() {
        INSTANCE = this;
    }

    public void TriggerBackgroundChanged()
    {
        m_BGHelper.DarkenBackground();
    }


    public void ChangeBackgroundToNextImage()
    {
        m_CurrentBGSpriteIndex++;

        if(m_CurrentBGSpriteIndex > m_BGSprites.Count) {
            m_CurrentBGSpriteIndex = 0;
        }

        m_BackgroundImage.sprite = m_BGSprites[m_CurrentBGSpriteIndex];
    }
}
