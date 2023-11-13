using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Advertisements;

[System.Serializable]
public class ADS
{
    public BannerAd BANNER;
    public InterstitialAd INTERSTITIAL;
    public RewardedVideoAd REWARDED;
}

public class AdManager : MonoBehaviour
{
    public static AdManager INSTANCE;

    [SerializeField] private ADS m_ADS;

    private void Awake()
    {
        INSTANCE = this;
    }

    // BANNER
    public void ShowBannerAd(BannerPosition bannerPosition) {
        if (IAPManager.INSTANCE.AdsEnabled()) {
            m_ADS.BANNER.ShowBannerAd(bannerPosition);
        }
    }
    public void HideBannerAd() {
        if (IAPManager.INSTANCE.AdsEnabled()) {
            m_ADS.BANNER.HideBannerAd();
        }
    }

    public void ForceHideBanner()
    {
        m_ADS.BANNER.HideBannerAd();
    }


    // INTERSTITIAL
    public void ShowInterstitalAd(Action callback) {
        if (IAPManager.INSTANCE.AdsEnabled()) {
            m_ADS.INTERSTITIAL.ShowAd(callback);
        }
        else { callback?.Invoke(); }
    }

    // REWARDED
    public void ShowRewardedVideoAd() {
        m_ADS.REWARDED.ShowAd();
    }
}
