using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum AdState { Disabled, Enabled }


[System.Serializable]
public class NoADS
{
    [SerializeField] private GameObject AdsNotBoughtUI;
    [SerializeField] private GameObject AdsBoughtUI;

    public void EnableAdsByState(AdState state) {
        switch (state)
        {
            case AdState.Disabled:
                AdsNotBoughtUI.SetActive(false);
                AdsBoughtUI.SetActive(true);
                break;
            case AdState.Enabled:
                AdsBoughtUI.SetActive(false);
                AdsNotBoughtUI.SetActive(true);
                break;
            default:
                break;
        }
    }

}

public class IAPManager : MonoBehaviour
{
    public static IAPManager INSTANCE;

    #region MONO
    private void Awake()
    {
        if(INSTANCE == null)
        {
            INSTANCE = this;
        } else { Destroy(this); }


        if(PlayerPrefs.HasKey(_AdsPrefsID_))
        {
            // WE HAD A PREVIOUS SESSION
            _AdsEnabled_ = PlayerPrefs.GetInt(_AdsPrefsID_, (int)AdState.Enabled);
        } else
        {
            // SAVE
            PlayerPrefs.SetInt(_AdsPrefsID_, _AdsEnabled_);
            PlayerPrefs.Save();
        }
    }

    private void Start()
    {
        m_NoADS?.EnableAdsByState((AdState)_AdsEnabled_);
    }
    #endregion

    #region IAP
    [Header("IAP")]
    [SerializeField] private Animator m_IAPAnimator;
    
    public void ShowIAP() {
        m_IAPAnimator.SetTrigger("show");
    }

    public void HideIAP() {
        m_IAPAnimator.SetTrigger("hide");
    }
    #endregion

    #region ADS
    [SerializeField] private NoADS m_NoADS;

    // 1 - enabled
    // 0 - disabled
    protected int _AdsEnabled_ = (int)AdState.Enabled;
    protected readonly string _AdsPrefsID_ = "ADS";

    public bool AdsEnabled()
    {
        return (AdState)_AdsEnabled_ == AdState.Enabled;
    }

    // IAP Button Actions
    public void OnNoADSPurchaseSuccessfulAction()
    {
        _AdsEnabled_ = (int)AdState.Disabled;

        // SAVE
        PlayerPrefs.SetInt(_AdsPrefsID_, _AdsEnabled_);
        PlayerPrefs.Save();

        // UPDATE VIEW
        m_NoADS.EnableAdsByState((AdState)_AdsEnabled_);

        // EMIT NO ADS!
        if(AdManager.INSTANCE != null)
        {
            AdManager.INSTANCE.ForceHideBanner();
        }

        print("OnNoADSPurchaseSuccessfulAction");
    }

    public void OnNoADSPurchaseFailedAction()
    {
        _AdsEnabled_ = (int)AdState.Enabled;

        // SAVE
        PlayerPrefs.SetInt(_AdsPrefsID_, _AdsEnabled_);
        PlayerPrefs.Save();

        // UPDATE VIEW
        m_NoADS.EnableAdsByState((AdState)_AdsEnabled_);

        print("OnNoADSPurchaseFailedAction");
    }

    #endregion
}
