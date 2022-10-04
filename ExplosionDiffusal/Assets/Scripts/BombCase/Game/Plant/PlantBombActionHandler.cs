using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlantBombActionHandler : MonoBehaviour
{
    [SerializeField] private GameObject m_3dKeyboard;
    [SerializeField] private GameObject m_3dKeypad;

    [SerializeField] private PlantBombHackingController m_HackingController;
    [SerializeField] private CameraManager m_CameraManager;
    [SerializeField] private UiManager m_UiManager;

    private void Start()
    {
        m_HackingController.OnHackingItemSelectedEvent.AddListener(OnHackingItemSelected);
    }

    private void OnDestroy()
    {
        m_HackingController.OnHackingItemSelectedEvent.RemoveListener(OnHackingItemSelected);
    }

    private void OnHackingItemSelected(HackingItemData data)
    {
        AudioManager.INSTANCE.PlayButtonPressedSFX(AudioEffect.Plant);

        m_CameraManager.ZoomInOutOfTarget(data.Position, () => {
            m_UiManager.FadeInOutScreen(.77f);
        }, () => 
            {
                if(data.SelectedType == ClickableType.Keyboard) { InitKeyboardView(); }
                if(data.SelectedType == ClickableType.Keypad) { InitKeyPadView(); }
            });
    }

  
    private void InitKeyboardView()
    {
        m_UiManager.EnableKeyBoardUI();
        m_3dKeyboard.SetActive(false);
    }

    public void DeinitKeyboardView()
    {
        m_3dKeyboard.SetActive(true);
    }

    private void InitKeyPadView()
    {
        m_UiManager.EnableKeyPadUI();
        m_3dKeypad.SetActive(false);
    }

    public void DeinitKeypadView()
    {
        m_3dKeypad.SetActive(true);
    }
}
