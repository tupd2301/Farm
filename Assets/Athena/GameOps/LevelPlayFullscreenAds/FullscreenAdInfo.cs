using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if USE_LEVELPLAY_MEDIATION
namespace Athena.GameOps
{
    public abstract class FullscreenAdInfo
    {
        public string AdUnitId;
        public FullscreenAdState AdState;
        public VideoAdType adType;

        public string AdNetwork;

        public Coroutine RequestAdCoroutine;
        public long WaitForAdPaidEventExpiredAt;
        public bool LoadedAdWithMutedSound;
        public bool AutoRetry;
        public int RetryAttempt;

        public void Load(bool isMuted)
        {
            AdState = FullscreenAdState.Loading;
            WaitForAdPaidEventExpiredAt = 0;
            LoadedAdWithMutedSound = isMuted;

            OnLoad(isMuted);
        }

        protected abstract void OnLoad(bool isMuted);
        public abstract void Show(string adPlacement);
        public abstract bool IsAvailable();
        // public abstract string MediationAdapterClassName();

        protected virtual void OnFullscreenAdLoaded(IronSourceAdInfo adInfo)
        {
            RetryAttempt = 0;
            AdState = FullscreenAdState.Loaded;
        }

        protected virtual void OnFullscreenAdFailedToLoad(IronSourceError ironSourceError)
        {
            AdState = FullscreenAdState.LoadFailed;
            RetryAttempt++;
        }

        protected virtual void OnFullscreenAdOpening(IronSourceAdInfo adInfo)
        {
            AdState = FullscreenAdState.Used;
        }

        // protected virtual void OnFullscreenAdPaid(float adValue)
        // {
        //     WaitForAdPaidEventExpiredAt = 0;
        // }

        protected virtual void OnFullscreenAdClosed(IronSourceAdInfo adInfo)
        {

        }
    }
}

#endif
