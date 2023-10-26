using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AudioButtonSFXHelper : MonoBehaviour
{
    [SerializeField] private List<Button> m_CommonButtons = new List<Button>();
    [SerializeField] private List<Button> m_ArrowButtons = new List<Button>();

    private void Start()
    {
        // sub
        m_CommonButtons.ForEach((btn) => {
            btn.onClick.AddListener(() => {
                AudioManager.INSTANCE.DEFAULT_BUTTON_PRESS_SOUND();
            });
        });

        m_ArrowButtons.ForEach((btn) => {
            btn.onClick.AddListener(() => {
                AudioManager.INSTANCE.PlayButtonPressedSFX(AudioEffect.Keypress);
            });
        });
    }

    private void OnDestroy()
    {
        // unsub
        m_CommonButtons.ForEach((btn) => {
            btn.onClick.RemoveAllListeners();
        });

        m_ArrowButtons.ForEach((btn) => {
            btn.onClick.RemoveAllListeners();
        });
    }
}
