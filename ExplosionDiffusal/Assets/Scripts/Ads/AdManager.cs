using System;
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
        m_ADS.BANNER.ShowBannerAd(bannerPosition);
    }
    public void HideBannerAd() {
        m_ADS.BANNER.HideBannerAd();
    }

    // INTERSTITIAL
    public void ShowInterstitalAd(Action callback) {
        m_ADS.INTERSTITIAL.ShowAd(callback);
    }

    // REWARDED
    public void ShoRewardedVideoAd() {
        m_ADS.REWARDED.ShowAd();
    }
}
