using GoogleMobileAds;
using GoogleMobileAds.Api;
using System;
using System.Collections.Generic;
using UnityEngine;

public class AdManager
{
    private RewardedAd _rewardedAd;

    private Action _rewardedCallback;
    private DateTime _lastAdWatchedTime;
    private const string LAST_AD_TIME_KEY = "LastAdWatchedTime";

    private float adDelay = 3f;


    private readonly float Sec = 60.0f;


    List<string> testDeviceIds = new List<string>();
    BannerView _bannerView;
    InterstitialAd _interstitialAd;
    RewardedAd rewardedAd;



#if UNITY_ANDROID
    string appId = "ca-app-pub-1382577074205698~5560472574";
    string bannerId = "ca-app-pub-1382577074205698/4051664370";
    string interId = "ca-app-pub-1382577074205698/5033573553";
    string rewardId = "ca-app-pub-1382577074205698/8046749702";
#elif UNITY_IPHONE
    string appId = "ca-app-pub-1382577074205698~7898484008";
    string bannerId = "ca-app-pub-3940256099942544/6300978111";
    string interId = "ca-app-pub-3940256099942544/1033173712";
    string rewardId = "ca-app-pub-3940256099942544/5224354917";
    //string adUnitId = "ca-app-pub-3940256099942544/1712485313";

#else
    string appId = "ca-app-pub-1382577074205698~5560472574";
    string bannerId = "ca-app-pub-3940256099942544/6300978111";
    string interId = "ca-app-pub-3940256099942544/1033173712";
    string rewardId = "ca-app-pub-3940256099942544/5224354917";
#endif





    public void Init()
    {



#if UNITY_EDITOR
        // Test ID
        bannerId = "ca-app-pub-3940256099942544/6300978111";
        interId = "ca-app-pub-3940256099942544/1033173712";
        rewardId = "ca-app-pub-3940256099942544/5224354917";
#endif

        _lastAdWatchedTime = LoadLastAdTime();

        testDeviceIds.Add("d98259fd-80b9-429c-ad8d-5af514ebed3c");
        testDeviceIds.Add("a8f37508-30d4-4e32-88f7-6a46079a32c8");
        testDeviceIds.Add("a4c5f293-463f-47a3-afb4-540626da7039");
        testDeviceIds.Add("0921bb97-a781-4bf2-9c06-d6bf190c6fad");
        testDeviceIds.Add("e26ca221-7478-4a0f-a9af-6a67dc563a49");
        testDeviceIds.Add("7a7678d6-dbfa-41d4-8d32-e8ab246163a5");
        testDeviceIds.Add("e2408ab4-f6d9-4c17-88ed-0be55b85b2ac");
        testDeviceIds.Add("c80ca528-95a4-4907-8c4e-90105ef01c75");

        testDeviceIds.Add(AdRequest.TestDeviceSimulator);
        RequestConfiguration config = new RequestConfiguration.Builder().SetTestDeviceIds(testDeviceIds).build();

        // Initialize the Google Mobile Ads SDK.
        MobileAds.RaiseAdEventsOnUnityMainThread = true;
        MobileAds.Initialize(initStatus =>
        {
#if UNITY_EDITOR
            Debug.Log("Ads Initialised !!");
#endif
            LoadBannerAd();
            LoadIntertitialAd();
            LoadRewawrdedAd();
        });
    }

    #region banner
    public void LoadBannerAd()
    {
        CreateBannerView();
        ListenToBannerEvents();

        if (_bannerView == null)
        {
            CreateBannerView();
        }

        var adRequest = new AdRequest();
        adRequest.Keywords.Add("unity-admob-sample");
#if UNITY_EDITOR
        Debug.Log("Loading banner Ad !!");
#endif
        _bannerView.LoadAd(adRequest);


    }
    void CreateBannerView()
    {
        if (_bannerView != null)
        {
            DestoryBannerAd();
        }
        _bannerView = new BannerView(bannerId, AdSize.Banner, AdPosition.Bottom);
    }

    /// <summary>
    /// listen to events the banner view may raise.
    /// </summary>
    private void ListenToBannerEvents()
    {
        // Raised when an ad is loaded into the banner view.
        _bannerView.OnBannerAdLoaded += () =>
        {
#if UNITY_EDITOR
            Debug.Log("Banner view loaded an ad with response : "
                + _bannerView.GetResponseInfo());
#endif
        };
        // Raised when an ad fails to load into the banner view.
        _bannerView.OnBannerAdLoadFailed += (LoadAdError error) =>
        {
            Debug.LogError("Banner view failed to load an ad with error : "
                + error);
        };
        // Raised when the ad is estimated to have earned money.
        _bannerView.OnAdPaid += (AdValue adValue) =>
        {
            Debug.Log(String.Format("Banner view paid {0} {1}.",
                adValue.Value,
                adValue.CurrencyCode));
        };
        // Raised when an impression is recorded for an ad.
        _bannerView.OnAdImpressionRecorded += () =>
        {
            Debug.Log("Banner view recorded an impression.");
        };
        // Raised when a click is recorded for an ad.
        _bannerView.OnAdClicked += () =>
        {
            Debug.Log("Banner view was clicked.");
        };
        // Raised when an ad opened full screen content.
        _bannerView.OnAdFullScreenContentOpened += () =>
        {
            Debug.Log("Banner view full screen content opened.");
        };
        // Raised when the ad closed full screen content.
        _bannerView.OnAdFullScreenContentClosed += () =>
        {
            Debug.Log("Banner view full screen content closed.");
        };
    }

    public void DestoryBannerAd()
    {
        if (_bannerView != null)
        {
            Debug.Log("Destorying banner Ad");
            _bannerView.Destroy();
            _bannerView = null;
        }
    }
    #endregion

    #region Interstitial 전면광고

    Action _inteAction;

    public void LoadIntertitialAd()
    {
        if (_interstitialAd != null)
        {
            _interstitialAd.Destroy();
            _interstitialAd = null;
        }

        var adRequest = new AdRequest();
        adRequest.Keywords.Add("unity-admob-sample");

        InterstitialAd.Load(interId, adRequest, (InterstitialAd ad, LoadAdError error) =>
         {
             if (error != null || ad == null)
             {
#if UNITY_EDITOR
                 Debug.Log("Interstitail ad failed to load" + error);
#endif
             }

             Debug.Log("Interstitail ad loaded" + ad.GetResponseInfo());
             _interstitialAd = ad;
             RegisterEventHandlers(_interstitialAd);
         });
    }

    /// <summary>
    /// Shows the interstitial ad.
    /// </summary>
    public void ShowInterstitialAd(Action action)
    {

        _inteAction = null;
        _inteAction = action;

        if (_interstitialAd != null && _interstitialAd.CanShowAd())
        {
            Debug.Log("Showing interstitial ad.");
            _interstitialAd.Show();
        }
        else
        {
            Debug.LogError("Interstitial ad is not ready yet.");
        }

    }

    private void RegisterEventHandlers(InterstitialAd interstitialAd)
    {
        // Raised when the ad is estimated to have earned money.
        interstitialAd.OnAdPaid += (AdValue adValue) =>
        {
            Debug.Log(String.Format("Interstitial ad paid {0} {1}.",
                adValue.Value,
                adValue.CurrencyCode));
        };
        // Raised when an impression is recorded for an ad.
        interstitialAd.OnAdImpressionRecorded += () =>
        {
            Debug.Log("Interstitial ad recorded an impression.");
        };
        // Raised when a click is recorded for an ad.
        interstitialAd.OnAdClicked += () =>
        {
            Debug.Log("Interstitial ad was clicked.");
        };
        // Raised when an ad opened full screen content.
        interstitialAd.OnAdFullScreenContentOpened += () =>
        {
            Debug.Log("Interstitial ad full screen content opened.");
        };
        // Raised when the ad closed full screen content.
        interstitialAd.OnAdFullScreenContentClosed += () =>
        {
            Debug.Log("Interstitial ad full screen content closed.");
            _inteAction?.Invoke();
            Managers.Game.ADCountReset();
            LoadIntertitialAd();
            Time.timeScale = 1f;
        };
        // Raised when the ad failed to open full screen content.
        interstitialAd.OnAdFullScreenContentFailed += (AdError error) =>
        {
            Debug.LogError("Interstitial ad failed to open full screen content " +
                           "with error : " + error);
        };
    }

    public bool CanShowIntAd()
    {
        return _interstitialAd != null && _interstitialAd.CanShowAd();
    }

    #endregion


    #region reward

    public void LoadRewawrdedAd()
    {
        if (rewardedAd != null)
        {
            rewardedAd.Destroy();
            rewardedAd = null;
        }

        var adRequest = new AdRequest();
        adRequest.Keywords.Add("unity-admob-sample");

        RewardedAd.Load(rewardId, adRequest, (RewardedAd ad, LoadAdError error) =>
         {
             if (error != null || ad == null)
             {
#if UNITY_EDITOR
                 Debug.Log("Reward Failed to load" + error);
#endif
             }
#if UNITY_EDITOR
             Debug.Log("Reward AD Loaded !!");
#endif
             rewardedAd = ad;
             RewardedAdEvents(rewardedAd);
         });
    }

    public bool CanShowRewardAd()
    {
        return rewardedAd != null && rewardedAd.CanShowAd();
    }
    public void ShowRewardedAd(Action action)
    {
        _rewardAction = null;
        _rewardAction = action;

        if (rewardedAd != null && rewardedAd.CanShowAd())
        {


            rewardedAd.Show((Reward reward) =>
            {
                Debug.Log("Give Reward To Player !!");
                _rewardAction?.Invoke();

            });
        }
        else
        {
            Debug.Log("Rewarded Ad not Ready");
        }
    }


    Action _rewardAction;
    public void RewardedAdEvents(RewardedAd ad)
    {
        // Raised when the ad is estimated to have earned money.
        ad.OnAdPaid += (AdValue adValue) =>
        {
#if UNITY_EDITOR
            Debug.Log(String.Format("Rewarded ad paid {0} {1}.",
                adValue.Value,
                adValue.CurrencyCode));
#endif
        };
        // Raised when an impression is recorded for an ad.
        ad.OnAdImpressionRecorded += () =>
        {
#if UNITY_EDITOR
            Debug.Log("Rewarded ad recorded an impression.");
#endif
        };
        // Raised when a click is recorded for an ad.
        ad.OnAdClicked += () =>
        {
            Debug.Log("Rewarded ad was clicked.");
        };
        // Raised when an ad opened full screen content.
        ad.OnAdFullScreenContentOpened += () =>
        {
            Debug.Log("Rewarded ad full screen content opened.");
        };
        // Raised when the ad closed full screen content.
        ad.OnAdFullScreenContentClosed += () =>
        {
            Debug.Log("Rewarded ad full screen content closed.");
            LoadRewawrdedAd();
        };
        // Raised when the ad failed to open full screen content.
        ad.OnAdFullScreenContentFailed += (AdError error) =>
        {
            Debug.LogError("Rewarded ad failed to open full screen content " +
                           "with error : " + error);
        };
    }

    #region 타이머

    private void SaveLastAdTime()
    {
        PlayerPrefs.SetString(LAST_AD_TIME_KEY, DateTime.UtcNow.ToString("o"));
        PlayerPrefs.Save();
    }

    private DateTime LoadLastAdTime()
    {
        if (PlayerPrefs.HasKey(LAST_AD_TIME_KEY))
        {
            string timeString = PlayerPrefs.GetString(LAST_AD_TIME_KEY);
            return DateTime.Parse(timeString, null, System.Globalization.DateTimeStyles.RoundtripKind);
        }

        return DateTime.UtcNow.AddMinutes(-adDelay);  // Default to 5 minutes ago if no time saved.
    }

    public string GetRemainingAdTime()
    {
        TimeSpan timeSinceLastAd = DateTime.UtcNow - _lastAdWatchedTime;
        double remainingSeconds = adDelay * Sec - timeSinceLastAd.TotalSeconds;  // 300 seconds is 5 minutes

        if (remainingSeconds <= 0)
        {
            return "";
        }

        int minutes = (int)(remainingSeconds / 60);
        int seconds = (int)(remainingSeconds % 60);

        return $"{minutes}:{seconds:D2}";  // D2 ensures the seconds are displayed as two digits.
    }

    public string GetRemainingMarktAdTime()
    {
        TimeSpan timeSinceLastAd = DateTime.UtcNow - _lastAdWatchedTime;
        double remainingSeconds = adDelay * Sec - timeSinceLastAd.TotalSeconds;  // 300 seconds is 5 minutes

        if (remainingSeconds <= 0)
        {
            return "Reset";
        }

        int minutes = (int)(remainingSeconds / 60);
        int seconds = (int)(remainingSeconds % 60);

        return $"{minutes}:{seconds:D2}";  // D2 ensures the seconds are displayed as two digits.
    }

    #endregion

    #endregion


    public void Clear()
    {
        DestoryBannerAd();
    }
}
