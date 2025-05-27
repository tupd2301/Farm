using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;
using Athena.Common;
using Athena.Common.UI;
using UnityEngine.Purchasing.Extension;
using CustomUtils;
using Newtonsoft.Json;

namespace Atom
{
    class UnityPurchaseReceipt
    {
        public string Store;
        public string TransactionID;
        public string Payload;
    }
    class UnityGooglePlayReceipt
    {
        public string json;
        public string signature;
    }
    class GooglePurchaseData
    {
        public string purchaseToken;
        public string developerPayload;
        public string orderId;
    }

    public class IAPManager : SingletonMono<IAPManager>, IStoreListener
    {
        public System.Action<IStoreController, IExtensionProvider> onInitialized;
        public System.Action<InitializationFailureReason> onInitializeFailed;
        public System.Action<Product, PurchaseFailureReason> onPurchaseFailed;
        public System.Action<PurchaseEventArgs> onItemPurchased, onItemRestored;
        public System.Func<PurchaseEventArgs, bool> checkSkipTrackingNonConsumableBundle;

        private bool _isInitialized = false;
        public bool IsInitialized { get { return _isInitialized; } }

        List<IAPBundle> _iapBundles;
        public List<IAPBundle> IAPBundles { get { return _iapBundles; } }

        List<SBundleOfferData> _specialBundleOffers;
        public List<SBundleOfferData> SpecialBundleOffers { get { return _specialBundleOffers; } }

        List<string> _oneTimePurchaseBundleIds;
        public List<string> OneTimePurchaseBundleIds
        {
            get { return _oneTimePurchaseBundleIds; }
            set { _oneTimePurchaseBundleIds = value; }
        }
        List<string> _oneTimePurchaseHistory;
        public List<string> OneTimePurchaseHistory
        {
            get { return _oneTimePurchaseHistory; }
        }

        private IStoreController _controller;
        private IExtensionProvider _extensions;

        public void Initialize()
        {
            _iapBundles = JsonConvert.DeserializeObject<List<IAPBundle>>(Resources.Load<TextAsset>("IAP/iap_bundles_android").text);//Global.RemoteConfigGetter.IAP_BUNDLE_CONFIGS;
            LoadSpecialOffers();
            LoadPurchaseHistory();

            UpdateSpecialEventStatus();

            var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());
            foreach (IAPBundle bundle in _iapBundles)
            {
                if (!string.IsNullOrEmpty(bundle.StoreBundleId))
                {
                    builder.AddProduct(bundle.StoreBundleId, GetProductType(bundle.Type), new IDs
                    {
                        { bundle.StoreBundleId, GooglePlay.Name },
                        { bundle.StoreBundleId, AppleAppStore.Name }
                    });
                }
            }
            UnityPurchasing.Initialize(this, builder);
        }

        private ProductType GetProductType(IAPType type)
        {
            switch (type)
            {
                case IAPType.NonConsumable:
                    return ProductType.NonConsumable;
                case IAPType.AutoRenewableSubscription:
                case IAPType.NonRenewingSubscription:
                    return ProductType.Subscription;
                default:
                    return ProductType.Consumable;
            }
        }

        public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
        {
            _isInitialized = true;
            _controller = controller;
            _extensions = extensions;
            onInitialized?.Invoke(_controller, _extensions);
        }

        public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
        {
            UIManager.Instance.SetUIInteractable(true);
            onPurchaseFailed?.Invoke(product, failureReason);
        }

        public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs purchaseEvent)
        {
            UIManager.Instance.SetUIInteractable(true);
            string bundleId = purchaseEvent.purchasedProduct.definition.storeSpecificId;
            if (_oneTimePurchaseBundleIds.Find(o => o.Equals(bundleId)) != null)
            {
                AddPurchaseHistory(bundleId);
            }
            UpdateSpecialEventStatus();
            onItemRestored?.Invoke(purchaseEvent);
            if (onItemPurchased != null)
            {
                onItemPurchased.Invoke(purchaseEvent);
                onItemPurchased = null;
                //AppManager.Instance.TrackingBusinessEvent(purchaseEvent.purchasedProduct);
                return PurchaseProcessingResult.Complete;
            }
            else
            {
                bool skipTrackingBundle = false;
                if (checkSkipTrackingNonConsumableBundle != null)
                {
                    skipTrackingBundle = checkSkipTrackingNonConsumableBundle(purchaseEvent);
                }
                IAPBundle bundle = _iapBundles.Find(o => o.BundleId == purchaseEvent.purchasedProduct.definition.storeSpecificId);
                if (bundle != null)
                {
                    //AppManager.Instance.ConsumeIAPBundle(bundle);
                }
                if (!skipTrackingBundle)
                {
                    //AppManager.Instance.TrackingBusinessEvent(purchaseEvent.purchasedProduct);
                }
                return PurchaseProcessingResult.Complete;
            }
        }

        public void PurchaseItem(IAPBundle bundle)
        {
            if (Application.internetReachability != NetworkReachability.NotReachable)
            {
                UIManager.Instance.SetUIInteractable(false);
                Debug.Log("Purchasing item: " + bundle.StoreBundleId);
                _controller.InitiatePurchase(bundle.StoreBundleId);
            }
            else
            {
                ConfirmUI connectionErrorUI = UIManager.Instance.ShowUIOnTop<ConfirmUI>("ConnectionErrorUI");
                connectionErrorUI.Show();
                connectionErrorUI.onHideFinished = () => {
                    UIManager.Instance.ReleaseUI(connectionErrorUI, true);
                };
                connectionErrorUI.onOKPressed = () => {
                    connectionErrorUI.Hide();
                };
            }
        }

        public void OnInitializeFailed(InitializationFailureReason error, string message)
        {
            onInitializeFailed?.Invoke(error);
        }

        public void OnInitializeFailed(InitializationFailureReason error)
        {
            onInitializeFailed?.Invoke(error);
        }

        public void Restore(System.Action onSuccess, System.Action<string> onFailed)
        {
#if UNITY_IOS
            _extensions.GetExtension<IAppleExtensions>().RestoreTransactions((result, reason) => {
                if (result)
                {
                    onSuccess?.Invoke();
                }
                else
                {
                    onFailed?.Invoke(reason);
                }
                onItemRestored = null;
            });
#elif UNITY_ANDROID
            _extensions.GetExtension<IGooglePlayStoreExtensions>().RestoreTransactions((result, reason) =>
            {
                if (result)
                {
                    onSuccess?.Invoke();
                }
                else
                {
                    onFailed?.Invoke(reason);
                }
                onItemRestored = null;
            });
#endif
        }

        public IAPBundle GetIAPBundle(string bundleId)
        {
            return _iapBundles.Find(o => o.BundleId.Equals(bundleId));
        }

        public void SaveSpecialOffers()
        {
            if (_specialBundleOffers == null)
            {
                return;
            }
            LocalDatabase.SetJSONData(Global.LocalDatabaseKey.IAP_SBUNDLE_EVENT_DATA, _specialBundleOffers);
        }

        public void LoadSpecialOffers()
        {
            _specialBundleOffers = LocalDatabase.GetJSONData(Global.LocalDatabaseKey.IAP_SBUNDLE_EVENT_DATA, new List<SBundleOfferData>());
        }

        public void AddSpecialOffer(SBundleOfferData offerData)
        {
            if (_specialBundleOffers.Find(o => o.BundleId.Equals(offerData.BundleId)) == null)
            {
                _specialBundleOffers.Add(offerData);
                SaveSpecialOffers();
            }
        }

        public bool HasSpecialOffer(string bundleId)
        {
            if (_specialBundleOffers.Find(o => o.BundleId.Equals(bundleId)) != null)
            {
                return true;
            }
            return false;
        }

        public void UpdateSpecialEventStatus()
        {
            bool hasChanged = false;
            for (int i = 0; i < _specialBundleOffers.Count; i++)
            {
                SBundleOfferData offerData = _specialBundleOffers[i];
                if (offerData.End - System.DateTimeOffset.UtcNow.ToUnixTimeSeconds() <= 0 || DidPurchase(offerData.BundleId))
                {
                    _specialBundleOffers.Remove(offerData);
                    i--;
                    hasChanged = true;
                }
            }
            if (hasChanged)
            {
                SaveSpecialOffers();
            }
        }

        public long GetSpecialBundleEndTime(string bundleId)
        {
            SBundleOfferData offerData = _specialBundleOffers.Find(o => o.BundleId.Equals(bundleId));
            if (offerData != null)
            {
                return offerData.End;
            }
            return 0;
        }

        public void SetSpecialBundleEndTime(string bundleId, long endTime)
        {
            SBundleOfferData offerData = _specialBundleOffers.Find(o => o.BundleId.Equals(bundleId));
            if (offerData != null)
            {
                offerData.End = endTime;
                SaveSpecialOffers();
            }
        }

        public void SavePurchaseHistory()
        {
            if (_specialBundleOffers == null)
            {
                return;
            }
            //LocalDatabase.SetJSONData(Global.LocalDatabaseKey.IAP_PURCHASE_HISTORY, _oneTimePurchaseHistory);
        }

        public void LoadPurchaseHistory()
        {
            //_oneTimePurchaseHistory = LocalDatabase.GetJSONData(Global.LocalDatabaseKey.IAP_PURCHASE_HISTORY, new List<string>());
        }

        public void AddPurchaseHistory(string bundleId)
        {
            if (_oneTimePurchaseHistory.Find(o => o.Equals(bundleId)) == null)
            {
                _oneTimePurchaseHistory.Add(bundleId);
                SavePurchaseHistory();
            }
        }

        public bool DidPurchase(string bundleId)
        {
            if (_oneTimePurchaseHistory.Find(o => o.Equals(bundleId)) != null)
            {
                return true;
            }
            return false;
        }
    }
}