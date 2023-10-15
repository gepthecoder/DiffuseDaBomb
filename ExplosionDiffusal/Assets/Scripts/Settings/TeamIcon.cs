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
}
