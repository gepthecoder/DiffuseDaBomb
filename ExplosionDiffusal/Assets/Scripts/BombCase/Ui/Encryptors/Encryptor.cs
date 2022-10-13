using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public abstract class Encryptor : MonoBehaviour {

    public GameState currentGameState;

    abstract public void OnKeyButtonPress(string key);
    virtual public void CloseEncryptor() { }

    public UnityEvent<HackingItemData> OnEncryptorClose = new UnityEvent<HackingItemData>();

    public GameObject BombDefusalCodesUi;
}

public static class Extensions
{
    public static void clearText(this InputField inputField)
    {
        inputField.Select();
        inputField.text = "";
    }
}
