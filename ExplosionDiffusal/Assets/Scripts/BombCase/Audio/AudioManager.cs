using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
public enum AudioEffect { Keypress, Success, Denial, Plant, BombsPlanted, Defuse, BombsDefused, OpenBomb, }
public enum MenuAudioLoopType { Loop1, Loop2, Loop3, Loop4, None }
public enum BombCountdownLoopType { Default, Last48, Last16 }


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


[System.Serializable]
public class BombCountdownLoopData
{
    public BombCountdownLoopType m_Type;
    public AudioSource m_BombCountdownLoopSource;
    public AudioClip m_BombCountdownLoopClip;

    public void Init()
    {
        m_BombCountdownLoopSource.clip = m_BombCountdownLoopClip;
        m_BombCountdownLoopSource.volume = 0f;
    }

    public BombCountdownLoopData() { }
}

public class AudioManager : MonoBehaviour
{
    public static AudioManager INSTANCE;

    [Header("Loops")]
    [SerializeField] private List<MenuAudioLoopData> m_MenuAudioLoops = new List<MenuAudioLoopData>();
    [Space(10)]
    [SerializeField] private List<BombCountdownLoopData> m_BombCountdownLoops = new List<BombCountdownLoopData>();

    [Header("Sources")]
    [SerializeField] private AudioSource m_KeyPressAudio;
    [SerializeField] private AudioSource m_OtherAudio;
    [Space(5)]
    [SerializeField] private AudioSource m_StartMatchAudio;
    [SerializeField] private AudioSource m_StartMatchAudioTemp;
    [SerializeField] private AudioSource m_StartMatchAudioTemp1;
    [Space(5)]
    [SerializeField] private AudioSource m_ButtonPressDefaultAudio;
    [Space(5)]
    [SerializeField] private AudioSource m_ExplosionAudio;
    [SerializeField] private AudioSource m_ExplosionTemp01Audio;
    [SerializeField] private AudioSource m_ExplosionTemp02Audio;

    [Header("SFX")]
    [SerializeField] private AudioClip m_KeyPressClip;
    [SerializeField] private AudioClip m_AccessDeniedClip;
    [SerializeField] private AudioClip m_BombPlantedClip;
    [SerializeField] private AudioClip m_DefusingClip;
    [SerializeField] private AudioClip m_AllBombsPlantedClip;
    [SerializeField] private AudioClip m_AllBombsDefusedClip;
    [SerializeField] private AudioClip m_PlantBombClip;
    [Space(5)]
    [SerializeField] private AudioClip m_BeforeVOClip;
    [SerializeField] private AudioClip m_StartMatchDrums;
    [Space(5)]
    [SerializeField] private AudioClip m_ButtonPressDefault;
    [Space(5)]
    [SerializeField] private AudioClip m_OpenBombClip;
    [Space(5)]
    [SerializeField] private AudioClip m_BombExplosionAnticipationClip;
    [SerializeField] private AudioClip m_BombExplosion00Clip;
    [SerializeField] private AudioClip m_BombExplosion01Clip;
    [SerializeField] private AudioClip m_BombExplosion02Clip;

    [Header("VO")]
    [SerializeField] private AudioClip m_SearchAndDestroyIntroClipVO;
    [Space(5)]
    [SerializeField] private AudioClip m_BombHasBeenPlantedClipVO;
    [Space(5)]
    [SerializeField] private AudioClip m_Last60SecClipVO;
    [Space(5)]
    [SerializeField] private AudioClip m_ObjectiveDestroyedClipVO;
    [SerializeField] private AudioClip m_AlliesWinClipVO;
    [SerializeField] private AudioClip m_AxisWinClipVO;
    

    private void Awake()
    {
        InitMenuAudioLoops();
        InitBombCountdownLoops();

        INSTANCE = this;
    }

    #region SFX

    public void OnPlantBombVFX()
    {
        StartCoroutine(OnPlantBombVFXSeq());
    }

    public void Last60Sec()
    {
        m_StartMatchAudioTemp.PlayOneShot(m_BeforeVOClip);
        m_StartMatchAudioTemp1.PlayOneShot(m_Last60SecClipVO);
    }

    public void Last48Sec()
    {
        StartCoroutine(FadeMusic(GetDataByBombCountdownLoopType(BombCountdownLoopType.Default).m_BombCountdownLoopSource, 0f, 1));
        StartCoroutine(FadeMusic(GetDataByBombCountdownLoopType(BombCountdownLoopType.Last48).m_BombCountdownLoopSource, 1f, 1));
    }

    public void Last16Sec()
    {
        StartCoroutine(FadeMusic(GetDataByBombCountdownLoopType(BombCountdownLoopType.Last48).m_BombCountdownLoopSource, 0f, 1));
        StartCoroutine(FadeMusic(GetDataByBombCountdownLoopType(BombCountdownLoopType.Last16).m_BombCountdownLoopSource, 1f, 1));
    }

    // On Explode, Defuse
    public void MuteAllBombCountdownLoops()
    {
        StartCoroutine(FadeMusic(GetDataByBombCountdownLoopType(BombCountdownLoopType.Default).m_BombCountdownLoopSource, 0f, .5f));
        StartCoroutine(FadeMusic(GetDataByBombCountdownLoopType(BombCountdownLoopType.Last48).m_BombCountdownLoopSource, 0f, .5f));
        StartCoroutine(FadeMusic(GetDataByBombCountdownLoopType(BombCountdownLoopType.Last16).m_BombCountdownLoopSource, 0f, .5f));
    }

    public void TriggerExplosionAudio(Team winTeam)
    {
        StartCoroutine(TriggerExplosionAudioSeq(winTeam));
    }    
    
    private IEnumerator TriggerExplosionAudioSeq(Team winTeam)
    {
        // anticipation
        m_ExplosionAudio.PlayOneShot(m_BombExplosionAnticipationClip);

        yield return new WaitForSeconds(1f);

        m_ExplosionAudio.PlayOneShot(m_BombExplosion00Clip);

        yield return new WaitForSeconds(.5f);

        m_ExplosionTemp01Audio.PlayOneShot(m_BombExplosion01Clip);

        yield return new WaitForSeconds(.5f);

        m_ExplosionTemp01Audio.PlayOneShot(m_BombExplosion02Clip);

        yield return new WaitForSeconds(1f);

        // "OBJECTIVE DESTROYED"
        m_StartMatchAudioTemp.PlayOneShot(m_BeforeVOClip);
        m_StartMatchAudioTemp1.PlayOneShot(m_ObjectiveDestroyedClipVO);
       
        yield return new WaitForSeconds(m_ObjectiveDestroyedClipVO.length);

        // "AXIS / ALLIES WIN"
        var winWO = winTeam == Team.Allies ? m_AlliesWinClipVO : m_AxisWinClipVO;

        m_StartMatchAudioTemp.PlayOneShot(m_BeforeVOClip);
        m_StartMatchAudioTemp1.PlayOneShot(winWO);

        yield break;
    }

    public void PlayAudioEffectByType(AudioEffect effectType)
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
            case AudioEffect.OpenBomb:
                if(!m_OtherAudio.isPlaying)
                    m_OtherAudio.PlayOneShot(m_OpenBombClip);
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

        m_StartMatchAudioTemp.PlayOneShot(m_BeforeVOClip);
        m_StartMatchAudioTemp1.PlayOneShot(m_SearchAndDestroyIntroClipVO);

        StartCoroutine(FadeMusic(m_StartMatchAudio, 0f, 3f));
    }

    private IEnumerator OnPlantBombVFXSeq()
    {
        yield return new WaitForSeconds(1f);

        // Tick Loop Sound 0 - 1 volume t-LENGHT OF VO: "TheBombHasBeenPlanted.."
        StartCoroutine(FadeMusic(GetDataByBombCountdownLoopType(BombCountdownLoopType.Default).m_BombCountdownLoopSource, 1, m_BombHasBeenPlantedClipVO.length + 2f));

        m_StartMatchAudioTemp.PlayOneShot(m_BeforeVOClip);
        m_StartMatchAudioTemp1.PlayOneShot(m_BombHasBeenPlantedClipVO);

        yield break;
    }

    private BombCountdownLoopData GetDataByBombCountdownLoopType(BombCountdownLoopType type)
    {
        return m_BombCountdownLoops.FirstOrDefault((item => item.m_Type == type));
    }

    private void InitBombCountdownLoops()
    {
        foreach (var loopData in m_BombCountdownLoops)
        {
            loopData.Init();
        }
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

            StartCoroutine(FadeMusic(loopData.m_MenuMusicLoopSource, fadeIn ? 1f : 0f, 
                fadeIn ? loopType == MenuAudioLoopType.Loop1 ? 5f: 3f : 2f
            ));
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
