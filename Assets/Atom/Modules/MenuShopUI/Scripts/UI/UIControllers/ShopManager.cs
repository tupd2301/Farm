using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CustomUtils;
using Athena.Common.UI;

namespace Atom
{
    public class ShopManager : SingletonMono<ShopManager>
    {
        protected IAPShopUI _iapShopUI;
        private IAPLoadingUI _iapLoadingUI;
        private float IAP_PURCHASE_TIMEOUT = 10.0f;

        public IAPShopUI ShopUI
        {
            get
            {
                return _iapShopUI;
            }
        }

        public void Setup()
        {
            _iapShopUI = AppManager.Instance.ShowSafeTopUI<IAPShopUI>("Atom/IAPShopUI", false);
        }

        public void ShowMoreCoinUI()
        {
            //AppManager.Instance.DeactivateBanner();
            //_queueController.IsPlaying = false;
            //_gameBoardController.IsPlaying = false;
            List<IAPBundle> bundles = IAPManager.Instance.IAPBundles;
            _iapShopUI = UIManager.Instance.ShowUIOnTop<IAPShopUI>("Atom/IAPShopUI");
            _iapShopUI.Setup(bundles);
            _iapShopUI.onMoreOfferPressed = () =>
            {
                _iapShopUI.Setup(bundles, true);
            };
            _iapShopUI.onClosePressed = () => {
                _iapShopUI.Hide();
            };
            _iapShopUI.onHideFinished = () => {
                UIManager.Instance.ReleaseUI(_iapShopUI, true);
                //AppManager.Instance.ActivateBanner();
            };
            _iapShopUI.onBuyPressed = OnBuyPressed;
            //_iapShopUI.onRestorePressed = OnRestorePressed;
        }

        public void InitializeIAP()
        {
            if (!IAPManager.Instance.IsInitialized)
            {
                IAPManager.Instance.checkSkipTrackingNonConsumableBundle = (purchaseEvent) =>
                {
                    bool willSkip = false;
                    if (!string.IsNullOrEmpty(purchaseEvent.purchasedProduct.appleOriginalTransactionID)
                    && !purchaseEvent.purchasedProduct.transactionID.Equals(purchaseEvent.purchasedProduct.appleOriginalTransactionID))
                    {
                        willSkip = true;
                    }
                    if (purchaseEvent.purchasedProduct.definition.storeSpecificId.Equals(Global.GameConfig.IAP_REMOVE_AD_PACK_ID)
                    )//&& AppManager.Instance.UserStorage.GetQuantity(ItemId.RemoveAds) > 0)
                    {
                        willSkip = true;
                    }
                    return willSkip;
                };
                IAPManager.Instance.onInitialized = (controller, provider) => {
                    Debug.LogError("Store initialized!");
                    //IAPManager.Instance.onItemRestored = OnItemRestored;
                    IAPManager.Instance.Restore(() => {

                    }, (error) => {

                    });
                };
                IAPManager.Instance.onInitializeFailed = (error) => {
                    Debug.LogError("Store initialization is failed: " + error.ToString());
                };
                IAPManager.Instance.Initialize();
                IAPManager.Instance.OneTimePurchaseBundleIds = new List<string>
                {
                    Global.GameConfig.IAP_REMOVE_AD_PACK_ID,
                    Global.GameConfig.IAP_BEGINNER_BUNDLE_ID,
                    Global.GameConfig.IAP_DASH_DEAL_BUNDLE_ID,
                    Global.GameConfig.IAP_MASTER_BUNDLE_ID,
                };
            }
        }

        private void OnBuyPressed(IAPBundle bundle)
        {
            if (Application.internetReachability == NetworkReachability.NotReachable)
            {
                ConfirmUI connectionErrorUI = UIManager.Instance.ShowUIOnTop<ConfirmUI>("Atom/ConnectionErrorUI");
                connectionErrorUI.Show();
                connectionErrorUI.onHideFinished = () => {
                    UIManager.Instance.ReleaseUI(connectionErrorUI, true);
                };
                connectionErrorUI.onOKPressed = () => {
                    connectionErrorUI.Hide();
                };
                return;
            }
            IAPManager.Instance.onItemPurchased = (eventArgs) =>
            {
                if (bundle.Payout[0].Id == ItemId.RemoveAds)
                {
                    //AppManager.Instance.UserStorage.SetQuantity(ItemId.RemoveAds, bundle.Payout[0].Quantity, StorageAction.Add);
                    //AppManager.Instance.SaveUserStorage();
                    //AppManager.Instance.UpdateNoAdState();
                    //AppManager.Instance.TrackingResourceEvent("source", "Main_Shop", "", "Remove_Ads", 1, _levelId.ToString());
                    ConfirmUI removeAdCompletedUI = UIManager.Instance.ShowUIOnTop<ConfirmUI>("Atom/IAPResmoveAdCompleteUI");
                    removeAdCompletedUI.Show();
                    removeAdCompletedUI.onHideFinished = () => {
                        UIManager.Instance.ReleaseUI(removeAdCompletedUI, true);
                    };
                    removeAdCompletedUI.onOKPressed = () => {
                        removeAdCompletedUI.Hide();
                    };
                    //ReleaseMoreCoinUI();
                    return;
                }
                foreach (Item item in bundle.Payout)
                {
                    //AppManager.Instance.UserStorage.SetQuantity(item.Id, item.Quantity, StorageAction.Add);
                    //AppManager.Instance.TrackingResourcePurchase(item, "Coin_Popup", _levelId.ToString());
                }
                //AppManager.Instance.SaveUserStorage();
                //ReleaseMoreCoinUI();
                IAPCompletedUI purchaseCompletedUI = UIManager.Instance.ShowUIOnTop<IAPCompletedUI>("Atom/IAPCompletedUI");
                purchaseCompletedUI.SetupItems(bundle);
                //purchaseCompletedUI.SetLayer(Global.GameLayer.ScreenOverlay);
                purchaseCompletedUI.Show();
                purchaseCompletedUI.onHideFinished = () => {
                    UIManager.Instance.ReleaseUI(purchaseCompletedUI, true);
                    //if (_moreBoosterUI != null)
                    //{
                    //    _moreBoosterUI.UpdateCoinAmount();
                    //}
                };
                purchaseCompletedUI.onOKPressed = () => {
                    purchaseCompletedUI.Hide();
                };
            };
            IAPManager.Instance.onPurchaseFailed = (product, reason) => {
                //ReleaseMoreCoinUI();
                ConfirmUI purchaseFailedUI = UIManager.Instance.ShowUIOnTop<ConfirmUI>("Atom/IAPFailedUI");
                //purchaseFailedUI.SetLayer(Global.GameLayer.ScreenOverlay);
                purchaseFailedUI.Show();
                purchaseFailedUI.onHideFinished = () => {
                    UIManager.Instance.ReleaseUI(purchaseFailedUI, true);
                };
                purchaseFailedUI.onOKPressed = () => {
                    purchaseFailedUI.Hide();
                };
            };
            
            _iapLoadingUI = UIManager.Instance.ShowUIOnTop<IAPLoadingUI>("Atom/IAPLoadingUI");
            _iapLoadingUI.onTimeOut = () =>
            {
                UIManager.Instance.SetUIInteractable(true);
                _iapLoadingUI.Hide();
            };
            _iapLoadingUI.onLoadingShowed = () =>
            {
                IAPManager.Instance.PurchaseItem(bundle);
            };
            _iapLoadingUI.onHideFinished = () =>
            {
                UIManager.Instance.ReleaseUI(_iapLoadingUI, true);
            };
            _iapLoadingUI.Setup(true, IAP_PURCHASE_TIMEOUT);
            _iapLoadingUI.SetLayer(Global.GameLayer.ScreenOverlay);
            _iapLoadingUI.Show();
            
        }
    }
}