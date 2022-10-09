using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum AudioEffect { Keypress, Success, Denial, Plant, BombsPlanted}

public class AudioManager : MonoBehaviour
{
    public static AudioManager INSTANCE;

    [Header("Audio")]
    [SerializeField] private AudioSource m_KeyPressAudio;
    [SerializeField] private AudioSource m_OtherAudio;

    [SerializeField] private AudioClip m_KeyPressClip;
    [SerializeField] private AudioClip m_AccessDeniedClip;
    [SerializeField] private AudioClip m_BombPlantedClip;
    [SerializeField] private AudioClip m_AllBombsPlantedClip;
    [SerializeField] private AudioClip m_PlantBombClip;

    private void Awake()
    {
        INSTANCE = this;
    }

    public void PlayButtonPressedSFX(AudioEffect effectType)
    {
        switch (effectType)
        {
            case AudioEffect.Keypress:
                m_KeyPressAudio.PlayOneShot(m_KeyPressClip);
                break;
            case AudioEffect.Success:
                m_OtherAudio.PlayOneShot(m_BombPlantedClip);
                break;
            case AudioEffect.Denial:
                m_OtherAudio.PlayOneShot(m_AccessDeniedClip);
                break;
            case AudioEffect.Plant:
                m_OtherAudio.PlayOneShot(m_PlantBombClip);
                break;
            case AudioEffect.BombsPlanted:
                m_OtherAudio.PlayOneShot(m_AllBombsPlantedClip);
                break;
            default:
                break;
        }
    }

}
