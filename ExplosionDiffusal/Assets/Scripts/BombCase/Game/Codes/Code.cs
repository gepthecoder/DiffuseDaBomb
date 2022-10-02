using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CodeEncryptionType { KeyboardEncryption, KeyPadEncryption }

public class Code : MonoBehaviour
{
    public CodeEncryptionType EncryptionType;

    protected string __BOMB__CODE__ = string.Empty;

    private bool __isAllowedSetter__ = true;
    private bool __isCodeSet__ = false;

    public void SetBombCode(string newCode)
    {
        if (!__isAllowedSetter__)
            return;

        __BOMB__CODE__ = newCode;
        __isCodeSet__ = true;
    }

    public bool DoCodeValidation(string pass)
    {
        if (!__isCodeSet__)
            return false;

        return __BOMB__CODE__ == pass;
    } 
}
