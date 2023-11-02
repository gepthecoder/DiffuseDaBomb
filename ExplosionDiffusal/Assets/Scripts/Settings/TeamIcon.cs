using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TeamIcon : MonoBehaviour
{
    [SerializeField] private Image m_Icon = null;
    
    public void SetTeamIconSprite(Sprite sprite)
    {
        m_Icon.sprite = sprite;
    }

    public void SetTeamIconMaterial(Material mat)
    {
        m_Icon.material = mat;
    }

    public void SetTeamIconImageAlpha(float alpha)
    {
        m_Icon.color = new Color(
            m_Icon.color.r,
            m_Icon.color.g,
            m_Icon.color.b,
            alpha
        );
    }

    public void SetTeamIconImageViaMapper(TeamIconImageMapper mapper)
    {
        if(mapper.hasMaterial) {
            m_Icon.sprite = null;
            SetTeamIconMaterial(mapper.MATERIAL);
        } else
        {
            m_Icon.material = null;
            SetTeamIconSprite(mapper.SPRITE);
        }
    }

    public void FadeTeamIcon(float endVal, float duration)
    {
        m_Icon.DOFade(endVal, duration);
    }

    public string GetAvatarSourceID()
    {
        if(m_Icon.material != null)
        {
            return m_Icon.material.mainTexture.name;
        }

        return m_Icon.sprite.name;
    }

    public (Sprite, Material) GetSpriteMaterial()
    {
        return (m_Icon.sprite, m_Icon.material);
    }
}
