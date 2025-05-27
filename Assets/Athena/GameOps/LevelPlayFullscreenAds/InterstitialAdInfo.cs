#if USE_LEVELPLAY_MEDIATION
using System;
using System.Collections;
using System.Collections.Generic;
#if USE_AMAZON
using AmazonAds;
#endif
namespace Athena.GameOps
{
    public interface InterstitialAdListener
    {
        void OnInterstitialAdLoaded(InterstitialAdInfo adInfo);
        void OnInterstitialAdFailedToLoad(InterstitialAdInfo adInfo, IronSourceError error);
        void OnInterstitialAdOpening(InterstitialAdInfo adInfo);
        void OnInterstitialAdClicked(InterstitialAdInfo adInfo);
        // void OnInterstitialAdPaid(InterstitialAdInfo adInfo, float value);
        void OnInterstitialAdClosed();
    }

    public class InterstitialAdInfo : FullscreenAdInfo
    {
        public InterstitialAdListener Listener;
#if USE_AMAZON
        public string AmzonInterstitialSlotId = string.Empty;
        private APSVideoAdRequest interstitialVideoAdRequest;

#endif

        public InterstitialAdInfo()
        {
            IronSourceInterstitialEvents.onAdClosedEvent += OnFullscreenAdClosed;
            IronSourceInterstitialEvents.onAdReadyEvent += OnFullscreenAdLoaded;
            IronSourceInterstitialEvents.onAdLoadFailedEvent += OnFullscreenAdFailedToLoad;
            IronSourceInterstitialEvents.onAdOpenedEvent += OnFullscreenAdOpening;
            IronSourceInterstitialEvents.onAdClickedEvent += OnInterstitialAdClick;
        }
        protected override void OnLoad(bool isMuted)
        {
#if USE_AMAZON
            UnityEngine.Debug.Log("[Levelplay] interstitial video ID:" + AmzonInterstitialSlotId);
            interstitialVideoAdRequest = new APSVideoAdRequest(320, 480, AmzonInterstitialSlotId);
            interstitialVideoAdRequest.onSuccess += (adResponse) =>
            {
                UnityEngine.Debug.Log("[Levelplay] Amazon: interstitialVideoAdRequest success");

                IronSource.Agent.setNetworkData(APSMediationUtils.APS_IRON_SOURCE_NETWORK_KEY,
                                                    APSMediationUtils.GetInterstitialNetworkData(AmzonInterstitialSlotId, adResponse));
                IronSource.Agent.loadInterstitial();
            };
            interstitialVideoAdRequest.onFailedWithError += (adError) =>
            {
                UnityEngine.Debug.Log("[Levelplay] Amazon: interstitialVideoAdRequest onFailedWithError " + adError.GetMessage());
                IronSource.Agent.loadInterstitial();
            };
            interstitialVideoAdRequest.LoadAd();
#else
            IronSource.Agent.loadInterstitial();
#endif
        }

        public override void Show(string adPlacement)
        {
            IronSource.Agent.showInterstitial(adPlacement);
        }
        public override bool IsAvailable()
        {
            return IronSource.Agent.isInterstitialReady();
        }
        /************* Interstitial AdInfo Delegates *************/
        // Invoked when the interstitial ad was loaded succesfully.
        protected override void OnFullscreenAdLoaded(IronSourceAdInfo adInfo)
        {
            AdNetwork = adInfo.adNetwork;
            base.OnFullscreenAdLoaded(adInfo);
            Listener.OnInterstitialAdLoaded(this);
        }

        protected override void OnFullscreenAdFailedToLoad(IronSourceError ironSourceError)
        {
            base.OnFullscreenAdFailedToLoad(ironSourceError);
            Listener.OnInterstitialAdFailedToLoad(this, ironSourceError);
        }

        protected override void OnFullscreenAdOpening(IronSourceAdInfo adInfo)
        {
            base.OnFullscreenAdOpening(adInfo);
            Listener.OnInterstitialAdOpening(this);
        }
        protected void OnInterstitialAdClick(IronSourceAdInfo adInfo)
        {
            Listener.OnInterstitialAdClicked(this);
        }

        // Invoked when the interstitial ad closed and the user went back to the application screen.
        protected override void OnFullscreenAdClosed(IronSourceAdInfo adInfo)
        {
            base.OnFullscreenAdClosed(adInfo);
            Listener.OnInterstitialAdClosed();
        }
    }

}
#endif