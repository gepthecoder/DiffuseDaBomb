using UnityEngine;
using UnityEngine.Events;
public abstract class Encryptor : MonoBehaviour {
    abstract public void OnKeyButtonPress(string key);

    virtual public void CloseEncryptor() { }

    public UnityEvent<HackingItemData> OnEncryptorClose = new UnityEvent<HackingItemData>();
}
