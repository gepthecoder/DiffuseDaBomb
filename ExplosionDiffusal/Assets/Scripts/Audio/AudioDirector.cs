using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioDirector : MonoBehaviour
{
    public static AudioDirector INSTANCE;

    [SerializeField] private AudioMixer m_RootAudio;

    private void Awake()
    {
        if (INSTANCE == null)
        {
            INSTANCE = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    #region public-methods

    public void SetMasterVolume(float volume)
    {
        m_RootAudio?.SetFloat("Master", volume);
    }

    public void SetMusicVolume(float volume)
    {
        m_RootAudio?.SetFloat("MUSIC", volume);
    }

    public void SetSFXVolume(float volume)
    {
        m_RootAudio?.SetFloat("SFX", volume);
    }

    public void MuteSFX()
    {
        m_RootAudio.SetFloat("SFX", -80f);
    } 
    
    public void AmplifySFX()
    {
        m_RootAudio.SetFloat("SFX", 0f);
    }

    public void MuteMUSIC()
    {
        m_RootAudio.SetFloat("MUSIC", -80f);
    }

    public void AmplifyMUSIC()
    {
        m_RootAudio.SetFloat("MUSIC", 0f);
    }
    #endregion
}
