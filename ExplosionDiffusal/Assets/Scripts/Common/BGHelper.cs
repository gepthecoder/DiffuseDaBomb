using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGHelper : MonoBehaviour
{
    [SerializeField] private Animator m_BGDarkenAnime;

    // change bg
    public void OnDarkenEvent()
    {
        BackgroundManager.INSTANCE.ChangeBackgroundToNextImage();
    }

    public void DarkenBackground()
    {
        m_BGDarkenAnime.SetTrigger("darken");
    }
}
