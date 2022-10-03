using System;
using UnityEngine;
using UnityEngine.UI;

public class Keypad : Encryptor
{
    [Header("Elements")]
    [SerializeField] private InputField m_InputField;
    [SerializeField] private GameObject m_EnterCodeText;

    private string m_CurrentString = "";
    private const int m_MaxCharacters = 7;
    private int m_CharacterCounter = 0;

    private bool m_CanShowEnterCode = true;

    private void Awake()
    {
        m_InputField.characterLimit = m_MaxCharacters;
        m_EnterCodeText.SetActive(m_CanShowEnterCode);
    }

    public override void OnKeyButtonPress(string key)
    {
        bool isEnter = key == "Enter";
        bool isClear = key == "Clear";

        if (m_CanShowEnterCode) { m_CanShowEnterCode = false; m_EnterCodeText.SetActive(m_CanShowEnterCode); }

        if (m_CharacterCounter >= m_MaxCharacters && !isEnter && !isClear)
        {
            PlayButtonPressedSFX(AudioEffect.Denial);
            return;
        }

        UpdateInputField(key);
    }

    private void UpdateInputField(string key)
    {
        string fakeEvent = "";
        switch (key)
        {
            case "Clear":
                ClearCode();
                return;
            case "Enter":
                SubmitCode();
                return;
            case "Close":
                CloseKeypad();
                return;
            default:
                if (key.Length != 1)
                {
                    Debug.LogError("Ignoring spurious multi-character key value: " + key);
                    return;
                }
                char keyChar = key[0];
                if(Char.IsDigit(keyChar))
                {
                    fakeEvent = $"{keyChar}";
                }
                break;
        }

        m_CurrentString += fakeEvent;
        m_InputField.text += fakeEvent;
        m_CharacterCounter++;

        PlayButtonPressedSFX(AudioEffect.Keypress);
    }

    private void CloseKeypad()
    {
        EnableObject(false);
    }

    private void SubmitCode()
    {
        Debug.Log("<color=gold>SubmitCode</color>: " + m_CurrentString);
        if (m_CurrentString.Length != m_MaxCharacters)
        {
            Debug.Log("SubmitCode Denial ");

            PlayButtonPressedSFX(AudioEffect.Denial);
            return;
        }

        CodeManager.instance.SetCode(CodeEncryptionType.KeyPadEncryption, m_CurrentString);

        PlayButtonPressedSFX(AudioEffect.Success);
        CloseKeypad();
    }

    private void ClearCode()
    {
        if(m_CharacterCounter == 0)
        {
            m_CanShowEnterCode = true; m_EnterCodeText.SetActive(m_CanShowEnterCode);
            PlayButtonPressedSFX(AudioEffect.Denial);
        } else
        {
            m_CharacterCounter = 0;
            m_CurrentString = "";
            m_InputField.text = "";
        } 
    }

    private void PlayButtonPressedSFX(AudioEffect fx)
    {
        AudioManager.INSTANCE.PlayButtonPressedSFX(fx);
    }

    public void EnableObject(bool enable)
    {
        gameObject.SetActive(enable);
    }
}
