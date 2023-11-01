using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlantBombActionHandler : MonoBehaviour
{
    [Header("3D")]
    [SerializeField] private GameObject m_3dKeyboard;
    [SerializeField] private GameObject m_3dKeypad;
    [Space(5)]
    [SerializeField] private List<GameObject> m_PlasticBombCoverObjects;
    [Header("UI")]
    [SerializeField] private Keyboard m_2dKeyboard;
    [SerializeField] private Keypad m_2dKeypad;
    [Header("Controllers")]
    [SerializeField] private PlantBombHackingController m_HackingController;
    [SerializeField] private CameraManager m_CameraManager;
    [SerializeField] private UiManager m_UiManager;

    [HideInInspector] public UnityEvent<HackingItemData> OnEncryptorCloseEvent = new UnityEvent<HackingItemData>();

    private void Start()
    {
        m_HackingController.OnHackingItemSelectedEvent.AddListener(OnHackingItemSelected);
        m_2dKeyboard.OnEncryptorClose.AddListener((data) => { OnEncryptorClose(data); });
        m_2dKeypad.OnEncryptorClose.AddListener((data) => { OnEncryptorClose(data); });
    }

    private void OnDestroy()
    {
        m_HackingController.OnHackingItemSelectedEvent.RemoveListener(OnHackingItemSelected);
        m_2dKeyboard.OnEncryptorClose.RemoveAllListeners();
        m_2dKeypad.OnEncryptorClose.RemoveAllListeners();
    }

    private void OnHackingItemSelected(HackingItemData data)
    {
        AudioManager.INSTANCE.PlayAudioEffectByType(AudioEffect.Plant);

        SuitcaseHelper.INSTANCE?.ShowCloseSuitcaseButton(false);

        m_CameraManager.ZoomInOutOfTarget(data.Position, () => {
            m_UiManager.FadeInOutScreen(.77f);
        }, () => 
            {
                if(data.SelectedType == ClickableType.Keyboard) { InitKeyboardView(); }
                if(data.SelectedType == ClickableType.Keypad) { InitKeyPadView(); }
            });
    }

    private void OnEncryptorClose(HackingItemData data)
    {
        SuitcaseHelper.INSTANCE?.ShowCloseSuitcaseButton(true);

        m_CameraManager.ZoomOutOfTarget();

        DeinitKeyboardView();
        DeinitKeypadView();

        m_UiManager.DisableKeyBoardUI();
        m_UiManager.DisableKeyPadUI();

        OnEncryptorCloseEvent?.Invoke(data);
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

    public void ActivateBombEffect(bool activate, CodeEncryptionType type)
    {
        switch (type)
        {
            case CodeEncryptionType.KeyboardEncryption:

                foreach (var item in m_PlasticBombCoverObjects)
                {
                    item.SetActive(activate);
                }
                break;
            case CodeEncryptionType.KeyPadEncryption:
            default:
                break;
        }

    }

    public void SetMainStateForEncryptors(GameState state)
    {
        m_2dKeypad.currentGameState = state;
        m_2dKeyboard.currentGameState = state;
    }

    public Encryptor GetKeyboardEncryptor() { return m_2dKeyboard; }
    public Encryptor GetKeypadEncryptor() { return m_2dKeypad; }
}
