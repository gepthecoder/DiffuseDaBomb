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
    [Space(5)]
    [SerializeField] private AudioSource m_RepairBombAudio;
    [Space(5)]
    [SerializeField] private AudioSource m_VictoryAudio;
    [SerializeField] private AudioSource m_VictoryAudio1;

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
    [Space(5)]
    [SerializeField] private AudioClip m_SwooshClip;
    [Space(5)]
    [SerializeField] private AudioClip m_StaticClip;
    [Space(5)]
    [SerializeField] private AudioClip m_VictoryClip;
    [SerializeField] private AudioClip m_VictoryClip1;

    [Header("VO")]
    [SerializeField] private AudioClip m_SearchAndDestroyIntroClipVO;
    [Space(5)]
    [SerializeField] private AudioClip m_BombHasBeenPlantedClipVO;
    [Space(5)]
    [SerializeField] private AudioClip m_BombHasBeenDefusedClipVO;
    [Space(5)]
    [SerializeField] private AudioClip m_Last60SecClipVO;
    [Space(5)]
    [SerializeField] private AudioClip m_ObjectiveDestroyedClipVO;
    [Space(5)]
    [SerializeField] private AudioClip m_AlliesWinClipVO;
    [SerializeField] private AudioClip m_AxisWinClipVO;
    [Space(5)]
    [SerializeField] private AudioClip m_SwitchingSidesClipVO;

    // TODO: 

    // SWOOSH SOUND when goin in/out of circuitboard - DONE
    // DEFUSE CASE - DONE
    // SWITCHING SIDES - DONE

    // END MATCH


    private void Awake()
    {
        InitMenuAudioLoops();
        InitBombCountdownLoops();
        InitRepairBombAudio();

        INSTANCE = this;
    }

    #region SFX

        #region public-methods

        public void PlayVictorySFX(Team wTeam)
        {
            StartCoroutine(VictorySeq(wTeam));
        }

        private IEnumerator VictorySeq(Team wTeam)
        {
            m_VictoryAudio.volume = 0;

            StartCoroutine(FadeMusic(m_RepairBombAudio, 0f, 1f));
            StartCoroutine(FadeMusic(m_VictoryAudio, 1f, 2f));

            m_VictoryAudio.PlayOneShot(m_VictoryClip);

            yield return new WaitForSeconds(m_VictoryClip.length / 2.8f);

            if(wTeam != Team.None)
            {
                var wClip = wTeam == Team.Allies ? m_AlliesWinClipVO : m_AxisWinClipVO;
                m_StartMatchAudioTemp.PlayOneShot(m_BeforeVOClip);
                m_StartMatchAudioTemp1.PlayOneShot(wClip);
            }

            m_VictoryAudio1.volume = 0;
            m_VictoryAudio1.loop = true;
            m_VictoryAudio1.clip = m_VictoryClip1;

            StartCoroutine(FadeMusic(m_VictoryAudio1, .65f, 10f));

            m_VictoryAudio1.Play();

            yield break;
        }

        public void MuteVictoryAudio()
        {
            StartCoroutine(FadeMusic(m_VictoryAudio1, 0f, 2f));
        }

    /// <summary>
    /// called every frame in repair state (while holding down repair btn)
    /// </summary>
    /// <param name="volume"></param>
    public void PlayStaticSparkSFXUpdate(float volume)
        {
            m_RepairBombAudio.volume = 1 - volume;

            if (Mathf.Approximately(volume, 1))
            {
                // reset
                m_RepairBombAudio.Stop();
                m_RepairBombAudio.volume = 1;
            }
        }

        public void PlayStaticSparkSFX()
        {
            m_RepairBombAudio.volume = 1;
            m_RepairBombAudio.Play();
        }

        public void PlaySwooshSound()
        {
            m_OtherAudio.PlayOneShot(m_SwooshClip);
        }

        public void OnPlantBombSFX()
        {
            StartCoroutine(OnPlantBombSFXSeq());
        }

        public void Last60Sec()
        {
            StartCoroutine(Last60SecSeq());
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
            StartCoroutine(FadeMusic(GetDataByBombCountdownLoopType(BombCountdownLoopType.Default).m_BombCountdownLoopSource, 0f, .8f));
            StartCoroutine(FadeMusic(GetDataByBombCountdownLoopType(BombCountdownLoopType.Last48).m_BombCountdownLoopSource, 0f, .8f));
            StartCoroutine(FadeMusic(GetDataByBombCountdownLoopType(BombCountdownLoopType.Last16).m_BombCountdownLoopSource, 0f, .8f));
        }

        public void ExplodeOnly()
        {
            StartCoroutine(ExplosionOnlySeq());
        }

        public void SwitchingSides()
        {
            StartCoroutine(SwitchingSidesSeq());   
        }

        public void TriggerDefuseBombAudio(Team winTeam)
        {
            StartCoroutine(TriggerDefuseBombAudioSeq(winTeam));
        }   

        public void TriggerExplosionAudio(Team winTeam)
        {
            StartCoroutine(TriggerExplosionAudioSeq(winTeam));
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

        public void PlayGameIntroAudio(bool playIntro = true)
        {
            StartCoroutine(PlayGameIntroAudioSequence(playIntro));
        }

        public void PlayWinningTeamVO(Team wTeam)
        {
            StartCoroutine(PlayWinningTeamVOSeq(wTeam));
        }
        #endregion

        #region private-methods

        private IEnumerator Last60SecSeq()
        {
            if (m_StartMatchAudioTemp1.isPlaying)
            {
                while (m_StartMatchAudioTemp1.isPlaying)
                {
                    yield return null;
                }
            }

            m_StartMatchAudioTemp.PlayOneShot(m_BeforeVOClip);
            m_StartMatchAudioTemp1.PlayOneShot(m_Last60SecClipVO);
        }

        private IEnumerator PlayWinningTeamVOSeq(Team wTeam)
        {
            yield return new WaitForSeconds(1f);

            var wClip = wTeam == Team.Allies ? m_AlliesWinClipVO : m_AxisWinClipVO;

            m_StartMatchAudioTemp.PlayOneShot(m_BeforeVOClip);
            m_StartMatchAudioTemp1.PlayOneShot(wClip);
        }

        private IEnumerator PlayGameIntroAudioSequence(bool playIntro = true)
        {
            if(playIntro)
            {
                m_StartMatchAudio.PlayOneShot(m_StartMatchDrums);
                StartCoroutine(FadeMusic(m_StartMatchAudio, 1f, 2f));

                yield return new WaitForSeconds(m_StartMatchDrums.length / 3);

                StartCoroutine(FadeMusic(m_StartMatchAudio, 0f, 3f));

                if (m_StartMatchAudioTemp1.isPlaying)
                {
                    while(m_StartMatchAudioTemp1.isPlaying)
                    {
                        yield return null;
                    }
                }

                m_StartMatchAudioTemp.PlayOneShot(m_BeforeVOClip);
                m_StartMatchAudioTemp1.PlayOneShot(m_SearchAndDestroyIntroClipVO);

            } else
            {
                yield return new WaitForSeconds(1.5f);

                m_StartMatchAudioTemp.PlayOneShot(m_BeforeVOClip);
                m_StartMatchAudioTemp1.PlayOneShot(m_SearchAndDestroyIntroClipVO);
            }      
        }

        private IEnumerator OnPlantBombSFXSeq()
        {
            yield return new WaitForSeconds(1f);

            // Tick Loop Sound 0 - 1 volume t-LENGHT OF VO: "TheBombHasBeenPlanted.."
            StartCoroutine(FadeMusic(GetDataByBombCountdownLoopType(BombCountdownLoopType.Default).m_BombCountdownLoopSource, 1, m_BombHasBeenPlantedClipVO.length + 2f));

            if(m_StartMatchAudioTemp1.isPlaying)
            {
                while (m_StartMatchAudioTemp1.isPlaying)
                {
                    yield return null;
                }
            }

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

        private void InitRepairBombAudio()
        {
            m_RepairBombAudio.clip = m_StaticClip;
            m_RepairBombAudio.loop = true;
        }

        private IEnumerator ExplosionOnlySeq()
        {
            m_ExplosionAudio.PlayOneShot(m_BombExplosion00Clip);

            yield return new WaitForSeconds(.25f);

            m_ExplosionTemp01Audio.PlayOneShot(m_BombExplosion01Clip);

            yield return new WaitForSeconds(.5f);

            m_ExplosionTemp01Audio.PlayOneShot(m_BombExplosion02Clip);
        }

        private IEnumerator TriggerExplosionAudioSeq(Team winTeam)
        {
            // anticipation
            m_ExplosionAudio.PlayOneShot(m_BombExplosionAnticipationClip);

            yield return new WaitForSeconds(3.75f);

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

        private IEnumerator TriggerDefuseBombAudioSeq(Team winTeam)
        {
            yield return new WaitForSeconds(1.5f);

            m_StartMatchAudioTemp.PlayOneShot(m_BeforeVOClip);
            m_StartMatchAudioTemp1.PlayOneShot(m_BombHasBeenDefusedClipVO);

            yield return new WaitForSeconds(m_BombHasBeenDefusedClipVO.length + 2f);

            var winClip = winTeam == Team.Allies ? m_AlliesWinClipVO : m_AxisWinClipVO;

            m_StartMatchAudioTemp.PlayOneShot(m_BeforeVOClip);
            m_StartMatchAudioTemp1.PlayOneShot(winClip);

            yield break;
        }

        private IEnumerator SwitchingSidesSeq()
        {
            m_StartMatchAudio.PlayOneShot(m_StartMatchDrums);

            StartCoroutine(FadeMusic(m_StartMatchAudio, 1f, 1f));

            yield return new WaitForSeconds(m_StartMatchDrums.length / 4.5f);

            StartCoroutine(FadeMusic(m_StartMatchAudio, 0f, 2f));

            yield return new WaitForSeconds(1f);

            m_StartMatchAudioTemp.PlayOneShot(m_BeforeVOClip);
            m_StartMatchAudioTemp1.PlayOneShot(m_SwitchingSidesClipVO);
        }
        #endregion

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
                fadeIn ? loopType == MenuAudioLoopType.Loop1 ? 5f : 1f : 1f
            ));
        }
    }

    public void FadeOutVolume(MenuAudioLoopType loopType, float time)
    {
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
