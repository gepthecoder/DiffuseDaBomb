using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum AudioEffect { Keypress, Success, Denial, Plant, BombsPlanted, Defuse, BombsDefused, }
public enum MenuAudioLoopType { Loop1, Loop2, Loop3, Loop4, None }


[System.Serializable]
public class MenuAudioLoopData
{
    public MenuAudioLoopType m_Type;
    public AudioSource m_MenuMusicLoopSource;
    public AudioClip m_MenuMusicLoopClip;

    public void Init()
    {
        m_MenuMusicLoopSource.clip = m_MenuMusicLoopClip;
        m_MenuMusicLoopSource.volume = 0f;
    }

    public MenuAudioLoopData() { }
}

public class AudioManager : MonoBehaviour
{
    public static AudioManager INSTANCE;

    [Header("Music Loops")]
    [SerializeField] private List<MenuAudioLoopData> m_MenuAudioLoops = new List<MenuAudioLoopData>();

    [Header("Sources")]
    [SerializeField] private AudioSource m_KeyPressAudio;
    [SerializeField] private AudioSource m_OtherAudio;
    [Space(5)]
    [SerializeField] private AudioSource m_StartMatchAudio;
    [SerializeField] private AudioSource m_StartMatchAudioTemp;
    [SerializeField] private AudioSource m_StartMatchAudioTemp1;
    [Space(5)]
    [SerializeField] private AudioSource m_ButtonPressDefaultAudio;

    [Header("SFX")]
    [SerializeField] private AudioClip m_KeyPressClip;
    [SerializeField] private AudioClip m_AccessDeniedClip;
    [SerializeField] private AudioClip m_BombPlantedClip;
    [SerializeField] private AudioClip m_DefusingClip;
    [SerializeField] private AudioClip m_AllBombsPlantedClip;
    [SerializeField] private AudioClip m_AllBombsDefusedClip;
    [SerializeField] private AudioClip m_PlantBombClip;
    [Space(5)]
    [SerializeField] private AudioClip m_SearchAndDestroyIntroBGClip;
    [SerializeField] private AudioClip m_StartMatchDrums;
    [Space(5)]
    [SerializeField] private AudioClip m_ButtonPressDefault;

    [Header("VO")]
    [SerializeField] private AudioClip m_SearchAndDestroyIntroClip;
    

    private void Awake()
    {
        InitMenuAudioLoops();

        INSTANCE = this;
    }

    #region SFX
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
            case AudioEffect.Defuse:
                m_OtherAudio.PlayOneShot(m_DefusingClip);
                break;
            case AudioEffect.BombsDefused:
                m_OtherAudio.PlayOneShot(m_AllBombsDefusedClip);
                break;
            default:
                break;
        }
    }

    public void DEFAULT_BUTTON_PRESS_SOUND()
    {
        m_ButtonPressDefaultAudio.PlayOneShot(m_ButtonPressDefault);
    }

    public void PlayGameIntroAudio()
    {
        StartCoroutine(PlayGameIntroAudioSequence());
    }

    private IEnumerator PlayGameIntroAudioSequence()
    {
        m_StartMatchAudio.PlayOneShot(m_StartMatchDrums);
        yield return new WaitForSeconds(m_StartMatchDrums.length / 3);

        m_StartMatchAudioTemp.PlayOneShot(m_SearchAndDestroyIntroBGClip);
        yield return new WaitForSeconds(m_SearchAndDestroyIntroBGClip.length / 6);

        m_StartMatchAudioTemp1.PlayOneShot(m_SearchAndDestroyIntroClip);

        StartCoroutine(FadeMusic(m_StartMatchAudio, 0f, 3f));
    }

    #endregion

    #region AUDIO LOOPS

    public void InitMenuAudioLoops()
    {
        foreach (var loopData in m_MenuAudioLoops)
        {
            loopData.Init();
        }
    }

    public void TriggerMenuLoopChanged(MenuAudioLoopType loopType)
    {
        foreach (var loopData in m_MenuAudioLoops)
        {
            bool fadeIn = loopData.m_Type == loopType;
            StartCoroutine(FadeMusic(loopData.m_MenuMusicLoopSource, fadeIn ? 1f : 0f, fadeIn ? 3f : 2f));
        }
    }

    public void FadeOutVolume(MenuAudioLoopType loopType, float time)
    {
        print("FadeOutVolume: " + time);

        foreach (var loopData in m_MenuAudioLoops)
        {
            if(loopData.m_Type == loopType)
            {
                StartCoroutine(FadeMusic(loopData.m_MenuMusicLoopSource, 0f, time));
                break;
            }
        }
    }

    private IEnumerator FadeMusic(AudioSource source, float targetVolume, float duration = 1.5f)
    {
        float time = 0f;
        float startVol = source.volume;

        if (targetVolume == 1) {
            source.Play(); 
        }

        while (time < duration)
        {
            time += Time.deltaTime;
            source.volume = Mathf.Lerp(startVol, targetVolume, time / duration);
            yield return null;
        }

        source.volume = targetVolume;
        if(targetVolume == 0)
        {
            source.Stop();
        }

        yield break;
    }

    #endregion

}
