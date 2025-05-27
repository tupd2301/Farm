#if USE_LEVELPLAY_MEDIATION
using System.Collections;
using System.Collections.Generic;
// using GoogleMobileAds.Api;
#if USE_AMAZON
using AmazonAds;
#endif

namespace Athena.GameOps
{
    public interface RewardedAdListener
    {
        void OnRewardedAdLoaded(RewardedAdInfo adInfo);
        void OnRewardedAdFailedToLoad(RewardedAdInfo adInfo, IronSourceError error);
        void OnRewardedAdOpening(RewardedAdInfo adInfo);
        void OnRewardedAdFailedToShow(RewardedAdInfo adInfo, IronSourceError error);
        // void OnRewardedAdPaid(RewardedAdInfo adInfo, AdValue args);
        void OnRewardedAdClosed(RewardedAdInfo adInfo);
    }

    public class RewardedAdInfo : FullscreenAdInfo
    {
        public RewardedAdListener Listener;
        public IronSourceRewardedVideoEvents RewardedAd;

        public System.Action<string, string, string, double> OnRewardEarned;
#if USE_AMAZON
        public string amazonRewardedVideoSlotId = string.Empty;
        private APSVideoAdRequest rewardedVideoAdRequest;
#endif
        public RewardedAdInfo()
        {
#if USE_AMAZON
            //Add AdInfo Rewarded Video Events
            IronSourceRewardedVideoEvents.onAdReadyEvent += OnFullscreenAdLoaded;
            IronSourceRewardedVideoEvents.onAdLoadFailedEvent += OnFullscreenAdFailedToLoad;
            IronSourceRewardedVideoEvents.onAdOpenedEvent += OnFullscreenAdOpening;
            IronSourceRewardedVideoEvents.onAdClosedEvent += OnFullscreenAdClosed;
            IronSourceRewardedVideoEvents.onAdShowFailedEvent += OnFullscreenAdFailedToShow;
            IronSourceRewardedVideoEvents.onAdRewardedEvent += RewardedVideoOnAdRewardedEvent;
            // IronSourceRewardedVideoEvents.onAdClickedEvent += RewardedVideoOnAdClickedEvent;
#else
            IronSourceRewardedVideoEvents.onAdAvailableEvent += OnFullscreenAdLoaded;
            IronSourceRewardedVideoEvents.onAdOpenedEvent += OnFullscreenAdOpening;
            IronSourceRewardedVideoEvents.onAdShowFailedEvent += OnFullscreenAdFailedToShow;
            IronSourceRewardedVideoEvents.onAdClosedEvent += OnFullscreenAdClosed;
            IronSourceRewardedVideoEvents.onAdLoadFailedEvent += OnFullscreenAdFailedToLoad;
            IronSourceRewardedVideoEvents.onAdRewardedEvent += RewardedVideoOnAdRewardedEvent;
#endif
        }
        protected override void OnLoad(bool isMuted)
        {
#if USE_AMAZON
            rewardedVideoAdRequest = new APSVideoAdRequest(320, 480, amazonRewardedVideoSlotId);
            rewardedVideoAdRequest.onSuccess += (adResponse) =>
            {
                UnityEngine.Debug.Log("[Levelplay] Amazon: rewardedVideoAdRequest success");
                IronSource.Agent.setNetworkData(APSMediationUtils.APS_IRON_SOURCE_NETWORK_KEY,
                                                    APSMediationUtils.GetRewardedNetworkData(amazonRewardedVideoSlotId, adResponse));
                IronSource.Agent.loadRewardedVideo(); // If manual rewarded mode
            };
            rewardedVideoAdRequest.onFailedWithError += (adError) =>
            {
                UnityEngine.Debug.Log("[Levelplay] Amazon: onFailedWithError " + adError.GetMessage());
                IronSource.Agent.loadRewardedVideo(); // If manual rewarded mode
            };
            rewardedVideoAdRequest.LoadAd();
#endif
        }

        public override void Show(string adPlacement)
        {
            IronSource.Agent.showRewardedVideo(adPlacement);
        }
        public override bool IsAvailable()
        {
            return IronSource.Agent.isRewardedVideoAvailable();
        }
        protected override void OnFullscreenAdLoaded(IronSourceAdInfo adInfo)
        {
            UnityEngine.Debug.Log("[Levelplay] OnFullscreenAdLoaded");
            base.OnFullscreenAdLoaded(adInfo);
            AdNetwork = adInfo.adNetwork;
            Listener.OnRewardedAdLoaded(this);
        }

        protected override void OnFullscreenAdFailedToLoad(IronSourceError adInfo)
        {
            UnityEngine.Debug.Log("[Levelplay] OnFullscreenAdFailedToLoad " + adInfo.getDescription());
            base.OnFullscreenAdFailedToLoad(adInfo);
            Listener.OnRewardedAdFailedToLoad(this, adInfo);
        }

        protected override void OnFullscreenAdOpening(IronSourceAdInfo adInfo)
        {
            base.OnFullscreenAdOpening(adInfo);
            Listener.OnRewardedAdOpening(this);
        }

        protected void OnFullscreenAdFailedToShow(IronSourceError error, IronSourceAdInfo adInfo)
        {
            Listener.OnRewardedAdFailedToShow(this, error);
        }

        void RewardedVideoOnAdRewardedEvent(IronSourcePlacement placement, IronSourceAdInfo adInfo)
        {
            OnRewardEarned?.Invoke(AdUnitId, AdNetwork, "reward", 0);
        }

        protected override void OnFullscreenAdClosed(IronSourceAdInfo adInfo)
        {
            base.OnFullscreenAdClosed(adInfo);
            Listener.OnRewardedAdClosed(this);
        }
    }
}
#endif