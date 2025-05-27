using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if USE_AMAZON
using AmazonAds;
#endif

namespace Athena.GameOps
{
#if USE_LEVELPLAY_MEDIATION
    public enum PrecisionLevelPlay
    {
        NONE = 0,
        BID,
        RATE,
        CPM
    }
    public class LevelPlayAdManager : RewardedAdListener, InterstitialAdListener, IAdManager
    {
        bool _isIPad;
        const int DURATION_FULLSCREEN_AD_PAID_EVENT_EXPIRED = 5;

        public bool IsLevelPlayInitialized { get { return _initializedLevelPlay; } }

        public bool IsPlayingFullscreenlAds { get { return _isPlayingFullscreenAd; } }
        public bool IsInterstitialAdFailedToLoad { get { return _interstitialAdInfo.AdState == FullscreenAdState.LoadFailed; } }
        public bool IsInterstitialAdLoaded { get { return _interstitialAdInfo.AdState == FullscreenAdState.Loaded; } }

        public bool CheckInterstitialAdLoaded(string adUnitId)
        {
            FullscreenAdInfo adInfo;
            if (!_allFullscreenAds.TryGetValue(adUnitId, out adInfo))
                return false;

            return adInfo.AdState == FullscreenAdState.Loaded;
        }

        public bool CheckInterstitialAdFailedToLoad(string adUnitId)
        {
            FullscreenAdInfo adInfo;
            if (!_allFullscreenAds.TryGetValue(adUnitId, out adInfo))
                return false;

            return adInfo.AdState == FullscreenAdState.LoadFailed;
        }

        public Vector2 BannerPositionInPixels { get; private set; }
        public bool IsBannerAdCreated { get; private set; }
        public bool IsBannerAdActive { get { return IsBannerAdCreated && _bannerAdLoaded; } }
        public bool IsBannerDeactivated { get { return _bannerIsAskedToBeHidden; } }
        public bool IsPaddingBannerSupported { get { return true; } }

        public string BannerAdId { get { return _bannerAdId; } }
        public string BannerPlacementId
        {
            get { return _bannerAdPlacement; }
            set { _bannerAdPlacement = value; }
        }
        public string InterstitialAdId { get { return _interstitialAdInfo.AdUnitId; } }
        public string ColdsStartAdId { private set; get; }

        public bool IsPaddingBannerAd { get { return _isPaddingBannerAd; } }
        public int PaddingBannerX { get { return _bannerPaddingX; } }
        public int PaddingBannerY { get { return _bannerPaddingY; } }
        public bool IsUsingAdaptiveBanner { get { return _isUsingAdaptiveBanner; } }

        public event System.Action OnBannerDidUpdateConfigs;
        public IAdEventsListener AdEventsListener { get; set; }

        string _bannerAdId = "banner";
        string _bannerAdPlacement = string.Empty;
        int _safeBannerTopY;

        Coroutine _checkingSoundSwitch;
        bool _isPlayingFullscreenAd;
        string _fullscreenAdPlacementId;

        InterstitialAdInfo _interstitialAdInfo;
        RewardedAdInfo _rewardedAdInfo;
        Dictionary<string, FullscreenAdInfo> _allFullscreenAds = new Dictionary<string, FullscreenAdInfo>();

        bool _initializedLevelPlay;
        IMainAppService _appService;

        bool _bannerAdLoaded;
        bool _shouldUpdateBannerAdNetworkName;
        Coroutine _requestBannerCoroutine;
        bool _paidValueIsReady;
        bool _bannerIsHiddenByAppPerf;
        bool _bannerIsAskedToBeHidden;

        bool _isUsingAdaptiveBanner;
        bool _isPaddingBannerAd;
        int _bannerPaddingX = 0;
        int _bannerPaddingY = 0;
        string _interstitialAdId = "interstitial";
        string _rewardedAdId = "rewarded_video";
        System.Action _onLevelPlayInitialized;

#if USE_AMAZON
        private APSBannerAdRequest bannerAdRequest;
        private string amazonBannerSlotId = string.Empty;
#endif
        public LevelPlayAdManager(System.Action onSDKInitialized, bool enablePaddingBanner, bool usingAdaptiveBanner, string appKey, string amazonAppId, string amazonBannerId, string amazonInterId, string amazonRwdId, IMainAppService appService,
            bool hideBannerAdWhenAppPaused = false, bool hideBannerAdWhenAppLowPerf = false)
        {
#if USE_AMAZON && !UNITY_EDITOR
            amazonBannerSlotId = amazonBannerId;
            Amazon.Initialize(amazonAppId);
            IronSource.Agent.setManualLoadRewardedVideo(isOn: true);
#endif
            _onLevelPlayInitialized = onSDKInitialized;
            _isPaddingBannerAd = enablePaddingBanner;
            _isUsingAdaptiveBanner = usingAdaptiveBanner;
            _appService = appService;

            NativeHelper.InitAdClosedObserver();

            // if (hideBannerAdWhenAppPaused)
            //     _appService.SubscribeAppPause(OnAppPaused);

            // if (hideBannerAdWhenAppLowPerf)
            //     _appService.SubscribeAppPerfChanged(OnAppPerfChanged);

            BannerPlacementId = string.Empty;
            _interstitialAdInfo = new InterstitialAdInfo()
            {
                AdUnitId = _interstitialAdId,
                adType = VideoAdType.InterNormal,
                Listener = this
            };
#if USE_AMAZON
            _interstitialAdInfo.AmzonInterstitialSlotId = amazonInterId;
#endif
            _allFullscreenAds.Add(_interstitialAdId, _interstitialAdInfo);

            _rewardedAdInfo = new RewardedAdInfo()
            {
                AdUnitId = _rewardedAdId,
                adType = VideoAdType.RewardedAd,
                Listener = this
            };
#if USE_AMAZON
            _rewardedAdInfo.amazonRewardedVideoSlotId = amazonRwdId;
            _rewardedAdInfo.AutoRetry = true;
#endif
            _allFullscreenAds.Add(_rewardedAdId, _rewardedAdInfo);
            // _rewardedAdInfo.Load(true);
            // LoadRewardedAd(_rewardedAdId, autoRetry: true);

            if (string.IsNullOrEmpty(appKey))
            {
                Debug.LogError("appkey null or empty!");
            }
#if CHEAT
            IronSource.Agent.setMetaData("is_test_suite", "enable");
            IronSource.Agent.setAdaptersDebug(true);
            IronSource.Agent.validateIntegration();
#endif
            IronSourceEvents.onImpressionDataReadyEvent += ImpressionDataReadyEvent;
            IronSourceEvents.onSdkInitializationCompletedEvent += SdkInitializationCompletedEvent;
            //Banner event
            IronSourceBannerEvents.onAdLoadedEvent += OnBannerAdLoaded;
            IronSourceBannerEvents.onAdLoadFailedEvent += OnBannerAdFailedToLoad;
            IronSourceBannerEvents.onAdClickedEvent += OnBannerAdClicked;
            //Debug.LogError("[LEVEL]appkey " + appKey);
            // Initialize
            IronSourceAdQuality.Initialize(appKey);
            IronSource.Agent.init(appKey, IronSourceAdUnits.REWARDED_VIDEO, IronSourceAdUnits.INTERSTITIAL, IronSourceAdUnits.BANNER);
            //Debug.LogError("[LEVEL]appkey " + appKey);

#if UNITY_IOS
                        _isIPad = SystemInfo.deviceModel.Contains("iPad");

                        if (hideBannerAdWhenAppPaused)
                            _appService.SubscribeAppPause(OnAppPaused);

                        if (hideBannerAdWhenAppLowPerf)
                            _appService.SubscribeAppPerfChanged(OnAppPerfChanged);
#endif
        }
        private int GetPrecision(string precision)
        {
            if (precision.Equals(PrecisionLevelPlay.BID.ToString()))
            {
                return (int)PrecisionLevelPlay.BID;
            }
            else if (precision.Equals(PrecisionLevelPlay.RATE.ToString()))
            {
                return (int)PrecisionLevelPlay.RATE;
            }
            else if (precision.Equals(PrecisionLevelPlay.CPM.ToString()))
            {
                return (int)PrecisionLevelPlay.CPM;
            }
            return (int)PrecisionLevelPlay.NONE;
        }
        private void ImpressionDataReadyEvent(IronSourceImpressionData impressionData)
        {
            Debug.Log("[Levelplay] ImpressionDataReadyEvent: " + impressionData.ToString());
            // log ad paid only applicable for Admob Mediation with whitelisted account
            // if (impressionData.adUnit.Equals("rewarded_video"))
            // {
            //     AdEventsListener?.OnRewardedAdPaid(_bannerAdPlacement, impressionData.auctionId, impressionData.adNetwork, (long)impressionData.revenue, "USD", GetPrecision(impressionData.precision));
            // }
            // else if (impressionData.adUnit.Equals("interstitial"))
            // {
            //     AdEventsListener?.OnInterstitialAdPaid(_bannerAdPlacement, impressionData.auctionId, impressionData.adNetwork, (long)impressionData.revenue, "USD", GetPrecision(impressionData.precision));
            // }
            // else if (impressionData.adUnit.Equals("banner"))
            // {
            //     AdEventsListener?.OnBannerAdPaid(_bannerAdPlacement, impressionData.auctionId, impressionData.adNetwork, (long)impressionData.revenue, "USD", GetPrecision(impressionData.precision));
            // }
            string adPlacement = AnalyticsManager.AD_PLACEMENT_EMPTY;
           
            if (impressionData.adUnit.Contains("interstitial") || impressionData.adUnit.Contains("rewarded_video"))
            {
                adPlacement = _fullscreenAdPlacementId;
            }
            
            if (impressionData.adUnit.Equals("banner"))
            {
                adPlacement = _bannerAdPlacement;
                //call load ads
                IronSourceBannerSize smartBannerSize = IronSourceBannerSize.SMART;
#if !UNITY_IOS
                if (_isUsingAdaptiveBanner)
                {
                    float Width = IronSource.Agent.getDeviceScreenWidth();
                    float Height = IronSource.Agent.getMaximalAdaptiveHeight(Width);
                    ISContainerParams isContainerParams = new ISContainerParams { Width = Width, Height = Height };
                    smartBannerSize.setBannerContainerParams(isContainerParams);
                    smartBannerSize.SetAdaptive(_isUsingAdaptiveBanner);
                }
#endif
#if USE_AMAZON
                LoadBannerAmazone(smartBannerSize);
#endif
            }
            AdEventsListener?.OnLogAdImpression("ironSource", impressionData.adNetwork, impressionData.instanceName, impressionData.adUnit, "USD", impressionData.revenue.Value);
           
            AdEventsListener?.OnLogAdRevenue(impressionData.revenue.Value, impressionData.adNetwork, impressionData.adUnit, adPlacement);
            AdEventsListener?.OnLogBIAdValueLevelPlay(adPlacement, impressionData.adUnit, impressionData.adNetwork, impressionData.revenue.Value, "USD", GetPrecision(impressionData.precision));
        }
        private void LoadBannerAmazone(IronSourceBannerSize smartBannerSize)
        {
#if USE_AMAZON
            Vector2Int size = new Vector2Int(320, 50);
#if UNITY_IOS
            if (SystemInfo.deviceModel.Contains("iPad"))
            {
                size = new Vector2Int(728, 90);
            }
#else
            if (Screen.width > 720)
            {
                size = new Vector2Int(728, 90);
            }
#endif
            Debug.Log("[LevelPlay] smartBannerSize: " + size.x + " height: " + size.y + " Screen.width: " + Screen.width);
            bannerAdRequest = new APSBannerAdRequest(size.x, size.y, amazonBannerSlotId);
            bannerAdRequest.onFailedWithError += (adError) =>
            {
                Debug.Log("[LevelPlay] bannerAdRequest: onFailedWithError " + adError.GetMessage());
                IronSource.Agent.loadBanner(smartBannerSize, IronSourceBannerPosition.BOTTOM);
            };
            bannerAdRequest.onSuccess += (adResponse) =>
                {
                    Debug.Log("[LevelPlay] bannerAdRequest: onSuccess ");
                    IronSource.Agent.setNetworkData(APSMediationUtils.APS_IRON_SOURCE_NETWORK_KEY,
                                                APSMediationUtils.GetBannerNetworkData(amazonBannerSlotId, adResponse));
                    IronSource.Agent.loadBanner(smartBannerSize, IronSourceBannerPosition.BOTTOM);
                };
            bannerAdRequest.LoadAd();
            Debug.Log("[Levelplay]Fresh load banner APS ");
#endif
        }
        private void SdkInitializationCompletedEvent()
        {
            _initializedLevelPlay = true;
            // IronSource.Agent.validateIntegration();
            _onLevelPlayInitialized?.Invoke();
#if USE_AMAZON
            RequestRewardedAd(autoRetry: true);
#endif
        }
        public void ShowDebuggerPanel()
        {
#if CHEAT
            if (_initializedLevelPlay)
            {
                IronSource.Agent.launchTestSuite();
            }
#endif
        }
        public void CleanUp()
        {

        }
        public void SetTestDevices(string[] testDevices)
        {

        }

        #region Banner Ad
        public void RequestBanner()
        {
            if (_requestBannerCoroutine != null)
            {
                return;
            }

            if (IsBannerAdCreated)
            {
                return;
            }

            if (_bannerAdLoaded)
            {
                return;
            }

            _requestBannerCoroutine = _appService.StartCoroutine(CreateBanner());
        }
        public void DestroyBannerAd()
        {
            _bannerIsAskedToBeHidden = true;
            IronSource.Agent.destroyBanner();
        }
        public void DeactiveBannerAd()
        {
            _bannerIsAskedToBeHidden = true;

            if (IsBannerAdCreated)
            {
                // _bannerView.Hide();
                IronSource.Agent.hideBanner();
            }
        }

        public void ActiveBannerAd()
        {
            _bannerIsAskedToBeHidden = false;

            if (IsBannerAdCreated)
            {
                if (!_bannerIsHiddenByAppPerf)
                {
                    // _bannerView.Show();
                    IronSource.Agent.displayBanner();
                }
            }
        }

        public void SetSafeTopAdaptiveBanner(float screenPosY)
        {
            var value = (int)screenPosY;
            if (_safeBannerTopY != value)
            {
                _safeBannerTopY = value;

                if (IsBannerAdCreated)
                {
                    _bannerAdLoaded = false;
                    IsBannerAdCreated = false;
                    RequestBanner();

                    OnBannerDidUpdateConfigs?.Invoke();
                }
            }
        }

        public void RefreshBannerConfigs(string bannerAdId, int paddingX, int paddingY, bool useAdaptiveBanner)
        {
            bool isUpdated = useAdaptiveBanner != _isUsingAdaptiveBanner || paddingX != _bannerPaddingX || paddingY != _bannerPaddingY;//|| (!_bannerAdId.Equals(bannerAdId) && !string.IsNullOrEmpty(bannerAdId)
            if (!isUpdated)
                return;

            _bannerPaddingX = paddingX;
            _bannerPaddingY = paddingY;
            _isUsingAdaptiveBanner = useAdaptiveBanner;
            // if (!string.IsNullOrEmpty(bannerAdId))
            //     _bannerAdId = bannerAdId;

            if (_bannerAdLoaded)
            {
                Debug.Log("[AdManager]RefreshBannerConfigs Banner ad configs are updated!");

                _bannerAdLoaded = false;
                IsBannerAdCreated = false;
                // IronSourceBannerEvents.onAdLoadedEvent -= OnBannerAdLoaded;
                // IronSourceBannerEvents.onAdLoadFailedEvent -= OnBannerAdFailedToLoad;
                // IronSourceBannerEvents.onAdClickedEvent -= OnBannerAdClicked;
                DestroyBannerAd();
                RequestBanner();

                OnBannerDidUpdateConfigs?.Invoke();
            }
        }
        #endregion

        #region Interstitial Ad
        public bool IsInterstitialReady()
        {
            return _interstitialAdInfo.IsAvailable();
        }
        public bool IsSoftLaunchAdReady()
        {
            return _interstitialAdInfo.IsAvailable();
        }
        public bool IsColdStartAdReady()
        {
            return _interstitialAdInfo.IsAvailable();
        }
        public bool IsAdBreakReady()
        {
            return _interstitialAdInfo.IsAvailable();
        }
        public void RequestColdStartAd(bool autoRetry = true)
        {
            // LoadInterstitialAd(_interstitialAdInfo.AdUnitId, autoRetry);
        }
        public void RequestSoftLaunchAd(bool autoRetry = true)
        {
            // LoadInterstitialAd(_interstitialAdInfo.AdUnitId, autoRetry);
        }
        public void RequestAdBreak(bool autoRetry = true)
        {
            // LoadInterstitialAd(_interstitialAdInfo.AdUnitId, autoRetry);
        }
        public void ShowAdBreak(string placemendId, System.Action<bool> cb, bool autoPreLoad = true)
        {
            ShowInterstitial(placemendId, cb, autoPreLoad);
        }
        public void ShowAdSoftLaunch(string placemendId, System.Action<bool> cb, bool autoPreLoad = true)
        {
            ShowInterstitial(placemendId, cb, autoPreLoad);
        }
        public void ShowAdColdStart(string placemendId, System.Action<bool> cb, bool autoPreLoad = true)
        {
            ShowInterstitial(placemendId, cb, autoPreLoad);
        }
        public void RefreshInterstitialConfigs(string interstitialAdId)
        {
            // if (_interstitialAdInfo.AdUnitId != interstitialAdId && !string.IsNullOrEmpty(interstitialAdId))
            // {
            //     _allFullscreenAds.Remove(_interstitialAdInfo.AdUnitId);
            //     _interstitialAdInfo.AdUnitId = interstitialAdId;
            //     _allFullscreenAds.Add(interstitialAdId, _interstitialAdInfo);
            // }
        }

        public void RequestInterstitial(bool autoRetry = true)
        {
            LoadInterstitialAd(_interstitialAdInfo.AdUnitId, autoRetry);
        }

        public void ShowInterstitial(string placemendId, System.Action<bool> cb, bool autoPreload = true)
        {
            ShowFullscreenAd(_interstitialAdInfo, placemendId, (success) =>
            {
                if (success)
                    AdEventsListener?.OnInterstitialAdClosed(placemendId, _interstitialAdInfo.AdUnitId, VideoAdType.InterAdBreak);

                cb(success);

                if (success && autoPreload)
                    RequestInterstitial();
            });
        }

        public IEnumerator WaitForInterstitialAdLoadedAndCallback(System.Action<bool, PollFullscreenAdErrorCode> cb, float timeout)
        {
            yield return WaitForFullscreenAdLoadedAndCallback(_interstitialAdInfo, cb, timeout);
        }

        public void AskToReloadMutedInterstitialAd() { }
        #endregion

        #region Other interstitial
        public void RequestInterstitial(string adUnitId, bool autoRetry = true)
        {
            LoadInterstitialAd(adUnitId, autoRetry);
        }

        public void ShowInterstitialAd(string adUnitId, string placemendId, System.Action<bool> cb, bool autoPreload = true)
        {
            FullscreenAdInfo adInfo;
            if (!_allFullscreenAds.TryGetValue(adUnitId, out adInfo))
            {
                cb(false);
                return;
            }

            ShowFullscreenAd(adInfo, placemendId, (success) =>
            {
                if (success)
                    AdEventsListener?.OnInterstitialAdClosed(placemendId, adUnitId, VideoAdType.InterNormal);

                cb(success);

                if (success && autoPreload)
                    RequestInterstitial(adUnitId);
            });
        }

        public IEnumerator WaitForInterstitialAdLoadedAndCallback(string adUnitId, System.Action<bool, PollFullscreenAdErrorCode> cb, float timeout)
        {
            FullscreenAdInfo adInfo;
            if (!_allFullscreenAds.TryGetValue(adUnitId, out adInfo))
            {
                cb(false, PollFullscreenAdErrorCode.NotRequested);
                yield break;
            }

            yield return WaitForFullscreenAdLoadedAndCallback(adInfo, cb, timeout);
        }

        public void AskToReloadMutedInterstitialAd(string adUnitId) { }
        #endregion

        #region Reward Ad
        public void RequestRewardedAd(bool autoRetry = true)
        {
#if USE_AMAZON
            LoadRewardedAd(_rewardedAdInfo.AdUnitId, autoRetry);
#endif
        }
        public void ShowRewardedAd(string placemendId, System.Action<string, double> cbRewardEarned, System.Action<bool> cbClosed, bool autoPreLoad = true)//update config to file
        {
            Debug.Log("[AdManager]ShowRewardedAd");
            ShowRewardedAd(placemendId, _rewardedAdInfo.AdUnitId, cbRewardEarned, cbClosed, autoPreLoad);
            // _rewardedAdInfo.Show(placemendId);
        }
        public bool IsRewardedAdLoaded()
        {
            return _rewardedAdInfo.IsAvailable();
        }
        public void RequestRewardedAd(string adUnitId, bool autoRetry = true)
        {
            // LoadRewardedAd(adUnitId, autoRetry);
        }

        public IEnumerator WaitForRewardedAdLoadedAndCallback(string adUnitId, System.Action<bool, PollFullscreenAdErrorCode> cb, float timeout)
        {
            FullscreenAdInfo adInfo;
            if (!_allFullscreenAds.TryGetValue(adUnitId, out adInfo))
            {
                cb(false, PollFullscreenAdErrorCode.NotRequested);
                yield break;
            }

            yield return WaitForFullscreenAdLoadedAndCallback(adInfo, cb, timeout);
        }

        public bool IsRewardedAdLoaded(string adUnitId)
        {
            FullscreenAdInfo adInfo;
            if (!_allFullscreenAds.TryGetValue(adUnitId, out adInfo))
                return false;

            return adInfo.AdState == FullscreenAdState.Loaded;
        }

        public void ShowRewardedAd(string placemendId, string adUnitId, System.Action<string, double> cbRewardEarned, System.Action<bool> cbClosed, bool autoPreload = true)
        {
            FullscreenAdInfo adInfo;
            if (!_allFullscreenAds.TryGetValue(adUnitId, out adInfo))
            {
                cbClosed(false);
                return;
            }

            (adInfo as RewardedAdInfo).OnRewardEarned = (rewardedAdUnitId, adNetwork, type, amount) =>
            {
                AdEventsListener?.OnRewardedAdUserEarned(placemendId, rewardedAdUnitId, adNetwork, -1);
                cbRewardEarned?.Invoke(type, amount);
            };

            ShowFullscreenAd(adInfo, placemendId, (success) =>
            {
                if (success)
                    AdEventsListener?.OnRewardedAdClosed(placemendId, adUnitId);

                cbClosed(success);
#if USE_AMAZON
                if (success && autoPreload)
                    LoadRewardedAd(adInfo.AdUnitId, adInfo.AutoRetry);
#endif
            });
        }
#if USE_AMAZON
        void LoadRewardedAd(string adUnitId, bool autoRetry, float delay = 0f)
        {
            Debug.Log("[AdManager] LoadRewardedAd...");
            FullscreenAdInfo adInfo;
            if (!_allFullscreenAds.TryGetValue(adUnitId, out adInfo))
            {
                adInfo = new RewardedAdInfo()
                {
                    AdUnitId = adUnitId,
                    Listener = this
                };
                _allFullscreenAds.Add(adUnitId, adInfo);
            }
            adInfo.AutoRetry = autoRetry;

            if (adInfo.AdState == FullscreenAdState.Loading)
            {
                Debug.Log("[AdManager] Waiting for last rewarded ad response!");
                return;
            }

            if (adInfo.AdState == FullscreenAdState.Loaded)
            {
                Debug.Log("[AdManager] Use last rewarded ad response!");
                return;
            }

            if (adInfo.RequestAdCoroutine != null)
                return;

            adInfo.RequestAdCoroutine = _appService.StartCoroutine(CreateRewardedAd(adInfo, delay));
        }

        IEnumerator CreateRewardedAd(FullscreenAdInfo adInfo, float delay)
        {
            while (!_initializedLevelPlay)
                yield return null;

            if (adInfo.WaitForAdPaidEventExpiredAt > System.DateTimeOffset.UtcNow.ToUnixTimeSeconds())
                Debug.Log("[AdManager] Wait for rewarded ad paid event!");

            while (adInfo.WaitForAdPaidEventExpiredAt > System.DateTimeOffset.UtcNow.ToUnixTimeSeconds())
                yield return null;

            if (delay > 0)
                yield return new WaitForSeconds(delay);

            Debug.Log("[AdManager] Requesting rewarded ad...");

#if UNITY_IOS || UNITY_ANDROID
            AdEventsListener?.OnRewardedAdStartLoading(adInfo.AdUnitId);

            bool isMuted = _appService.ShouldMuteVideoAd();
            if (!isMuted)
            {
                NativeHelper.CheckSoundSwitchStatus();
                while (NativeHelper.IsCheckingSoundSwitch())
                    yield return null;

                isMuted = NativeHelper.IsMuted();
            }
            adInfo.Load(isMuted);
#endif
            adInfo.RequestAdCoroutine = null;
        }
#endif
        #endregion

        #region Banner Ad Internal
        void OnAppPaused(bool pausedStatus)
        {
#if UNITY_IOS
            if (pausedStatus && IsBannerAdActive)
            {
                IronSource.Agent.hideBanner();
            }
            else if (!pausedStatus && IsBannerAdActive)
            {
                _appService.StartCoroutine(LazyResumeBanner());
            }
#endif
            IronSource.Agent.onApplicationPause(pausedStatus);
        }

        void OnAppPerfChanged(bool lowPerf)
        {
            if (lowPerf)
            {
                Debug.LogWarningFormat("[AdManager] OnAppPerfChanged({0}) - Hide banner ad!", lowPerf);
                _bannerIsHiddenByAppPerf = true;

                if (IsBannerAdCreated)
                {
                    IronSource.Agent.hideBanner();
                }
            }
            else if (!lowPerf && _bannerIsHiddenByAppPerf)
            {
                Debug.LogWarningFormat("[AdManager] OnAppPerfChanged({0}) - Show banner ad!", lowPerf);
                _bannerIsHiddenByAppPerf = false;

                if (IsBannerAdCreated && !_bannerIsAskedToBeHidden)
                {
                    IronSource.Agent.displayBanner();
                }
            }
        }

        IEnumerator LazyResumeBanner()
        {
            yield return new WaitForSeconds(0.5f);

            if (!_appService.IsAppPaused && !_bannerIsHiddenByAppPerf && !_bannerIsAskedToBeHidden)
            {
                IronSource.Agent.displayBanner();
            }
        }
        // IronSourceBannerSize admobAdaptiveSize;
        IEnumerator CreateBanner()
        {
            while (!_initializedLevelPlay)
                yield return null;

            _bannerAdLoaded = false;
            // Vector2 bannerSize = CalculateBannerSize();
            IsBannerAdCreated = true;
            IronSourceBannerSize smartBannerSize = IronSourceBannerSize.SMART;
#if !UNITY_IOS
            if (_isUsingAdaptiveBanner)
            {
                smartBannerSize.SetAdaptive(_isUsingAdaptiveBanner);
            }
#endif
#if USE_AMAZON
            LoadBannerAmazone(smartBannerSize);
#else
            // admobAdaptiveSize = new IronSourceBannerSize((int)bannerSize.x, (int)bannerSize.y);

            // if (_isUsingAdaptiveBanner)
            // {
            //     admobAdaptiveSize.SetAdaptive(_isUsingAdaptiveBanner);
            // }

            // IronSource.Agent.loadBanner(ironSourceBannerSize, IronSourceBannerPosition.BOTTOM, _bannerAdPlacement);
            IronSource.Agent.loadBanner(smartBannerSize, IronSourceBannerPosition.BOTTOM);//, _bannerAdPlacement);
#endif
        }
        Vector2 CalculateBannerSize()
        {
            if (_isPaddingBannerAd && (_bannerPaddingX != 0 || _bannerPaddingY != 0))
            {
                var scale = NativeHelper.GetDeviceNativeScale();
                var bannerW = (int)(Screen.width / scale - 2 * _bannerPaddingX);

                var bannerH = AthenaGameOpsUtils.DefaultBannerHeight(_isIPad);
                return new Vector2((int)bannerW, bannerH);
            }

            return BannerSize();
        }

        Vector2 BannerSize()
        {
            var width = Screen.width / NativeHelper.GetDeviceNativeScale();
            return new Vector2(width, AthenaGameOpsUtils.DefaultBannerHeight(_isIPad));
        }

        void OnBannerAdLoaded(IronSourceAdInfo adInfo)
        {
            Debug.Log("[Levelplay]OnBannerAdLoaded");
            _bannerAdLoaded = true;
            _shouldUpdateBannerAdNetworkName = true;
            _requestBannerCoroutine = null;
            if (_bannerIsAskedToBeHidden || _bannerIsHiddenByAppPerf)
            {
                Debug.Log("[Levelplay]hideBanner");
                IronSource.Agent.hideBanner();
            }
            AdEventsListener?.OnBannerAdRefresh(_bannerAdPlacement, adInfo.adUnit, adInfo.adNetwork, (float)adInfo.revenue);
        }
        void OnBannerAdClicked(IronSourceAdInfo adInfo)
        {
            AdEventsListener?.OnBannerAdClicked(_bannerAdPlacement, adInfo.adUnit, adInfo.adNetwork);
        }

        void OnBannerAdFailedToLoad(IronSourceError ironSourceError)
        {
            Debug.Log("[Levelplay]OnBannerAdFailedToLoad: " + ironSourceError.getCode() + " des: " + ironSourceError.getDescription());
            _bannerAdLoaded = false;
            _requestBannerCoroutine = null;

            string message = ironSourceError.getDescription();
            AdEventsListener?.OnBannerAdFailedToLoad(_bannerAdId, message);
        }
        #endregion

        #region FullscreenAd Internal
        void ShowFullscreenAd(FullscreenAdInfo adInfo, string placemendId, System.Action<bool> cb)
        {
            if (_checkingSoundSwitch != null)
            {
                cb(false);
                return;
            }

            _appService.SetAppInputActive(false);
            if (_appService.ShouldMuteVideoAd())
            {
                PlayFullscreenAd(adInfo, placemendId, true, (success) =>
                {
                    _appService.SetAppInputActive(true);
                    cb(success);
                });
            }
            else
            {
                _checkingSoundSwitch = _appService.StartCoroutine(CheckSoundSwitch((isMuted) =>
                {
                    _checkingSoundSwitch = null;
                    PlayFullscreenAd(adInfo, placemendId, isMuted, (success) =>
                    {
                        _appService.SetAppInputActive(true);
                        cb(success);
                    });
                }));
            }
        }

        void PlayFullscreenAd(FullscreenAdInfo adInfo, string placementId, bool isMuted, System.Action<bool> cb)
        {
#if UNITY_IOS || UNITY_ANDROID
            if (adInfo.AdState == FullscreenAdState.Loaded)
            {
                // Device is muted and our loaded interstitial has sound on, we should ignore this interstitial
                // if (isMuted && !adInfo.LoadedAdWithMutedSound
                // // Google Ad is controlled by SetApplicationVolume
                // && adInfo.AdNetwork != null && !adInfo.AdNetwork.Contains("oogle")
                // // Facebook Ad is always muted
                // && !adInfo.AdNetwork.Contains("acebook"))
                // {
                //     Debug.LogWarning("[AdManager] Device is muted and interstitial ad has sound on. This interstitial is ignored!");
                //     adInfo.AdState = FullscreenAdState.Used;
                //     cb(false);
                //     return;
                // }

                _isPlayingFullscreenAd = true;
                adInfo.AdState = FullscreenAdState.Used;
                _fullscreenAdPlacementId = placementId;

                adInfo.WaitForAdPaidEventExpiredAt = System.DateTimeOffset.UtcNow.ToUnixTimeSeconds() + DURATION_FULLSCREEN_AD_PAID_EVENT_EXPIRED;
                adInfo.Show(placementId);

                NativeHelper.StartPlayingFullscreenAd();
                _appService.StartCoroutine(WaitForFullscreenAdClosedAndCallback(() =>
                {
                    cb(true);
                }));
            }
            else
            {
                Debug.LogWarning("[AdManager] FullscreenAd is not loaded yet!");
                cb(false);
            }
#endif
        }

        IEnumerator WaitForFullscreenAdLoadedAndCallback(FullscreenAdInfo adInfo, System.Action<bool, PollFullscreenAdErrorCode> cb, float timeout)
        {
#if UNITY_EDITOR
            cb(false, PollFullscreenAdErrorCode.Failed);
            yield break;
#else
            var t = 0f;
            while (true)
            {
                if (adInfo.AdState == FullscreenAdState.LoadFailed
                || adInfo.AdState == FullscreenAdState.Loaded)
                    break;

                yield return null;

                if (timeout > 0)
                {
                    t += Time.deltaTime;
                    if (t >= timeout)
                        break;
                }
            }

            var success = adInfo.AdState == FullscreenAdState.Loaded;
            cb(success, success ? PollFullscreenAdErrorCode.Success : (adInfo.AdState == FullscreenAdState.LoadFailed ? PollFullscreenAdErrorCode.Failed : PollFullscreenAdErrorCode.Timeout));
#endif
        }

        IEnumerator WaitForFullscreenAdClosedAndCallback(System.Action cb)
        {
            while (_isPlayingFullscreenAd && !NativeHelper.IsFullScreenAdNativeClosed())
                yield return null;

            _isPlayingFullscreenAd = false;

            cb();
        }

        IEnumerator CheckSoundSwitch(System.Action<bool> cb)
        {
            NativeHelper.CheckSoundSwitchStatus();
            while (NativeHelper.IsCheckingSoundSwitch())
                yield return null;

            bool isMuted = NativeHelper.IsMuted();
            cb(isMuted);
        }
        #endregion

        #region Interstitial Ad Internal
        void LoadInterstitialAd(string adUnitId, bool autoRetry, float delay = 0f)
        {
            FullscreenAdInfo adInfo;
            if (!_allFullscreenAds.TryGetValue(adUnitId, out adInfo))
            {
                if (_interstitialAdInfo != null)
                {
                    adInfo = _interstitialAdInfo;
                }
                else
                {
                    Debug.LogError("LoadInterstitialAd null");
                    return;
                }
                // adInfo = new InterstitialAdInfo()
                // {
                //     AdUnitId = adUnitId,
                //     Listener = this
                // };
                // _allFullscreenAds.Add(adUnitId, adInfo);
            }
            adInfo.AutoRetry = autoRetry;

            if (adInfo.AdState == FullscreenAdState.Loading)
            {
                Debug.Log("[AdManager] Waiting for last interstitial ad response!");
                return;
            }

            if (adInfo.AdState == FullscreenAdState.Loaded)
            {
                Debug.Log("[AdManager] Use last interstitial ad response!");
                return;
            }

            if (adInfo.RequestAdCoroutine != null)
                return;

            adInfo.RequestAdCoroutine = _appService.StartCoroutine(CreateInterstitial(adInfo, delay));
        }

        IEnumerator CreateInterstitial(FullscreenAdInfo adInfo, float delay)
        {
            while (!_initializedLevelPlay)
                yield return null;

            if (adInfo.WaitForAdPaidEventExpiredAt > System.DateTimeOffset.UtcNow.ToUnixTimeSeconds())
                Debug.Log("[AdManager] Wait for interstitial ad paid event!");

            while (adInfo.WaitForAdPaidEventExpiredAt > System.DateTimeOffset.UtcNow.ToUnixTimeSeconds())
                yield return null;

            if (delay > 0f)
                yield return new WaitForSeconds(delay);

            Debug.Log("[AdManager] Requesting interstitial ad...");

#if UNITY_IOS || UNITY_ANDROID

            AdEventsListener?.OnInterstitialAdStartLoading(adInfo.AdUnitId);
            bool isMuted = _appService.ShouldMuteVideoAd();
            if (!isMuted)
            {
                NativeHelper.CheckSoundSwitchStatus();
                while (NativeHelper.IsCheckingSoundSwitch())
                    yield return null;

                isMuted = NativeHelper.IsMuted();
            }

            adInfo.Load(isMuted);
#endif
            adInfo.RequestAdCoroutine = null;
        }

        public void OnInterstitialAdOpening(InterstitialAdInfo adInfo)
        {
            AdEventsListener?.OnInterstitialAdShow(_fullscreenAdPlacementId, adInfo.AdUnitId, adInfo.AdNetwork, -1, adInfo.adType);
        }

        public void OnInterstitialAdClicked(InterstitialAdInfo adInfo)
        {
            AdEventsListener?.OnInterstitialAdClicked(_fullscreenAdPlacementId, adInfo.AdUnitId, adInfo.AdNetwork, adInfo.adType);
        }
        public void OnInterstitialAdFailedToLoad(InterstitialAdInfo adInfo, IronSourceError loadAdError)
        {
            string message = loadAdError.getDescription();
            AdEventsListener?.OnInterstitialAdFailedToLoad(adInfo.AdUnitId, message, adInfo.adType);

            if (adInfo.AutoRetry)
            {
                _appService.RunOnMainThread(() =>
                {
                    // We recommend retrying with exponentially higher delays up to a maximum delay (in this case 64 seconds)
                    var retryDelay = Mathf.Pow(2, Mathf.Min(6, adInfo.RetryAttempt));
                    LoadInterstitialAd(adInfo.AdUnitId, adInfo.AutoRetry, retryDelay);
                });
            }
        }

        public void OnInterstitialAdLoaded(InterstitialAdInfo adInfo)
        {
            AdEventsListener?.OnInterstitialAdLoaded(adInfo.AdUnitId, adInfo.AdNetwork);
        }

        public void OnInterstitialAdClosed()
        {
            _isPlayingFullscreenAd = false;
            NativeHelper.FullScreenAdClosed();
        }
        #endregion

        #region Rewarded Ad Internal

        public void OnRewardedAdLoaded(RewardedAdInfo adInfo)
        {
            AdEventsListener?.OnRewardedAdLoaded(adInfo.AdUnitId, adInfo.AdNetwork);
        }
        public void OnRewardedAdFailedToLoad(RewardedAdInfo adInfo, IronSourceError error)
        {
            string message = "levelplay-unknown";
            message = error.getDescription();
            AdEventsListener?.OnRewardedAdFailedToLoad(adInfo.AdUnitId, message);
#if USE_AMAZON
            if (adInfo.AutoRetry)
            {
                _appService.RunOnMainThread(() =>
                {
                    // We recommend retrying with exponentially higher delays up to a maximum delay (in this case 64 seconds)
                    var retryDelay = Mathf.Pow(2, Mathf.Min(6, adInfo.RetryAttempt));
                    LoadRewardedAd(adInfo.AdUnitId, adInfo.AutoRetry, retryDelay);
                });
            }
#endif
        }

        public void OnRewardedAdOpening(RewardedAdInfo adInfo)
        {
            AdEventsListener?.OnRewardedAdShow(_fullscreenAdPlacementId, adInfo.AdUnitId, adInfo.AdNetwork);
        }

        public void OnRewardedAdFailedToShow(RewardedAdInfo adInfo, IronSourceError error)
        {
            AdEventsListener?.OnRewardedAdFailedToShow(_fullscreenAdPlacementId, adInfo.AdUnitId, adInfo.AdNetwork);

            _isPlayingFullscreenAd = false;
            NativeHelper.FullScreenAdClosed();
        }

        public void OnRewardedAdClosed(RewardedAdInfo adInfo)
        {
            _isPlayingFullscreenAd = false;
            NativeHelper.FullScreenAdClosed();
        }
        #endregion
    }
#endif
}