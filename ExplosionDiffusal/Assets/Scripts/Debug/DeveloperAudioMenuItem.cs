using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DeveloperAudioMenuItem : DeveloperItem
{
    [SerializeField] private Slider m_MasterVolumeSlider;
    [SerializeField] private TextMeshProUGUI m_MasterVolumeValueText;
    [Header("Audio Settings")]
    [SerializeField] private Image m_MuteSFX_Image;
    [SerializeField] private Image m_MuteMUSIC_Image;
    [SerializeField] private Sprite m_SFX_ON;
    [SerializeField] private Sprite m_SFX_OFF;
    [SerializeField] private TextMeshProUGUI m_SFX_Text;
    [SerializeField] private TextMeshProUGUI m_MUSIC_Text;

    #region Consts
    private const float m_MinInput = -80f;
    private const float m_MaxInput = 0f;

    private const float m_MinOutput = 0f;
    private const float m_MaxOutput = 100f;

    private const string m_SfxOnText = "MUTE <size=30><color=green>SFX</color>";
    private const string m_MusicOnText = "MUTE <size=30><color=yellow>MUSIC</color>";  
    
    private const string m_SfxOffText = "UNMUTE <size=30><color=green>SFX</color>";
    private const string m_MusicOffText = "UNMUTE <size=30><color=yellow>MUSIC</color>";

    #endregion

    private bool m_IsSFX_ON = true;
    private bool m_IsMUSIC_ON = true;

    public override void Init()
    {
        base.Init();

        m_MasterVolumeSlider.onValueChanged.AddListener((value) => {

            float VOL = Mathf.Clamp(value, -80, 0);
            AudioDirector.INSTANCE?.SetMasterVolume(VOL);

            float mappedValue = (value - m_MinInput) / (m_MaxInput - m_MinInput) * (m_MaxOutput - m_MinOutput) + m_MinOutput;

            m_MasterVolumeValueText.text = $"{(int)mappedValue}";
        });
    }

    public override void Deinit()
    {
        base.Deinit();

        m_MasterVolumeSlider.onValueChanged.RemoveAllListeners();
    }

    #region Button Methods

    public void SFX_BUTTON_PRESS()
    {
        if (m_IsSFX_ON)
        {
            m_SFX_Text.text = m_SfxOffText;
            m_MuteSFX_Image.sprite = m_SFX_ON;
            AudioDirector.INSTANCE?.MuteSFX();

            m_IsSFX_ON = false;
        } else
        {
            m_SFX_Text.text = m_SfxOnText;
            m_MuteSFX_Image.sprite = m_SFX_OFF;
            AudioDirector.INSTANCE?.AmplifySFX();

            m_IsSFX_ON = true;
        }
    }

    public void MUSIC_BUTTON_PRESS()
    {
        if (m_IsMUSIC_ON)
        {
            m_MUSIC_Text.text = m_MusicOffText;
            m_MuteMUSIC_Image.sprite = m_SFX_ON;
            AudioDirector.INSTANCE?.MuteMUSIC();

            m_IsMUSIC_ON = false;
        }
        else
        {
            m_MUSIC_Text.text = m_MusicOnText;
            m_MuteMUSIC_Image.sprite = m_SFX_OFF;
            AudioDirector.INSTANCE?.AmplifyMUSIC();

            m_IsMUSIC_ON = true;
        }
    }
    #endregion
}
