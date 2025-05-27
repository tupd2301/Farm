#if USE_ADJUST
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using com.adjust.sdk;

namespace Athena.GameOps
{
    public class AdjustAnalyticsAgent : UAAnalyticsAgent
    {
        Dictionary<string, string> _eventTokens = new Dictionary<string, string>();
        List<AdjustEvent> _pendingEvents = new List<AdjustEvent>();
        IMainAppService _appService;

        public AdjustAnalyticsAgent(string adjustAppToken, string[] eventNames, string[] eventTokens, IMainAppService appService)
        {
            _appService = appService;

            for (int i = 0; i < eventNames.Length; i++)
                _eventTokens.Add(eventNames[i], eventTokens[i]);

            _appService.StartCoroutine(WaitForAdjustReady());
        }

        public override void LogRevenue(string eventName, double value, string currency, string transactionId, string productId, string googlePublickKey = "", string unityReceiptPayload = "")
        {
            string eventToken = null;
            if (_eventTokens.TryGetValue(eventName, out eventToken))
            {
                var adjustEvent = new AdjustEvent(eventToken);
                adjustEvent.setRevenue(value, currency);
                adjustEvent.setTransactionId(transactionId);
                adjustEvent.addCallbackParameter("transaction_id", transactionId);
                adjustEvent.addCallbackParameter("product_id", productId);

                if (_appService.IsAdjustReady)
                {
                    Adjust.trackEvent(adjustEvent);
                }
                else
                {
                    _pendingEvents.Add(adjustEvent);
                }
            }
        }
        public override void LogAdRevenue(double revenue, string networkName, string adUnitId, string adPlacement)
        {
            string source = "unknow";
#if USE_MAX_MEDIATION
            source = AdjustConfig.AdjustAdRevenueSourceAppLovinMAX;
#elif USE_LEVELPLAY_MEDIATION
            source = AdjustConfig.AdjustAdRevenueSourceIronSource;
#endif
            AdjustAdRevenue adjustAdRevenue = new AdjustAdRevenue(source);

            adjustAdRevenue.setRevenue(revenue, "USD");
            adjustAdRevenue.setAdRevenueNetwork(networkName);
            adjustAdRevenue.setAdRevenueUnit(adUnitId);
            adjustAdRevenue.setAdRevenuePlacement(adPlacement);

            Adjust.trackAdRevenue(adjustAdRevenue);
        }
        public override void LogSceneName(string sceneName, string sceneClass)
        {

        }

        public override void SetUserProperty(string name, string value)
        {

        }

        public override void LogEvent(string eventName)
        {
            string eventToken = null;
            if (_eventTokens.TryGetValue(eventName, out eventToken))
            {
                var adjustEvent = new AdjustEvent(eventToken);
                if (_appService.IsAdjustReady)
                {
                    Adjust.trackEvent(adjustEvent);
                }
                else
                {
                    _pendingEvents.Add(adjustEvent);
                }
            }
        }

        public override void LogEventWithParameters(string eventName, Dictionary<string, object> parameters)
        {
            string eventToken = null;
            if (_eventTokens.TryGetValue(eventName, out eventToken))
            {
                var adjustEvent = new AdjustEvent(eventToken);
                if (_appService.IsAdjustReady)
                {
                    foreach (var keyPair in parameters)
                    {
                        adjustEvent.addCallbackParameter(keyPair.Key, keyPair.Value == null ? string.Empty : keyPair.Value.ToString());
                    }
                    Adjust.trackEvent(adjustEvent);
                }
                else
                {
                    _pendingEvents.Add(adjustEvent);
                }
            }
        }

        IEnumerator WaitForAdjustReady()
        {
            while (!_appService.IsAdjustReady)
                yield return null;

            if (_pendingEvents.Count > 0)
            {
                foreach (var adjustEvent in _pendingEvents)
                {
                    Adjust.trackEvent(adjustEvent);
                }

                _pendingEvents.Clear();
            }
        }
    }
}

#endif