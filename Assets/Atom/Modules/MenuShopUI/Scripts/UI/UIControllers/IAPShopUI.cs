using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Athena.Common;
using Athena.Common.UI;
using DG.Tweening;
using UnityEngine.UI;
using TMPro;
using System.Linq;

namespace Atom
{
    public class IAPShopUI : FlexibleUIController
    {
        public System.Action<IAPBundle> onBuyPressed;
        public System.Action onClosePressed, onRestorePressed, onMoreOfferPressed;

        [SerializeField]
        private TextMeshProUGUI _textCoinAmount;

        [SerializeField]
        private ScrollRect _scroll;
        [SerializeField]
        private RectTransform _scrollContent, _restorePanel, _moreOffer;
        [SerializeField]
        private IAPItemCoinUI _coinItemPrefab, _removeAdItemPrefab;
        [SerializeField]
        private IAPItemBundleUI _bundleItemPrefab, _specialBundleItemPrefab;

        private IAPItemCoinUI _removeAdItem;
        private List<IAPItemCoinUI> _itemUIs;
        public List<IAPItemCoinUI> ItemUIs { get { return _itemUIs; } }

        private List<IAPItemBundleUI> _bundleUIs;
        public List<IAPItemBundleUI> BundleUIs { get { return _bundleUIs; } }

        private List<IAPItemBundleUI> _specialBundleUIs;
        public List<IAPItemBundleUI> SpecialBundleUIs { get { return _specialBundleUIs; } }

        protected override void OnBack()
        {
            if (UIManager.Instance.IsActiveOnTop(this))
            {
                onClosePressed();
            }
        }

        public void ResetPackItems()
        {
            if (_itemUIs == null)
            {
                _itemUIs = new List<IAPItemCoinUI>();
            }
            else if (_itemUIs.Count > 0)
            {
                for (int i = 0; i < _itemUIs.Count; i++)
                {
                    _itemUIs[i].gameObject.SetActive(false);
                }
            }
        }

        public void ResetBundleItems()
        {
            if (_bundleUIs == null)
            {
                _bundleUIs = new List<IAPItemBundleUI>();
            }
            else if (_bundleUIs.Count > 0)
            {
                for (int i = 0; i < _bundleUIs.Count; i++)
                {
                    _bundleUIs[i].gameObject.SetActive(false);
                }
            }
        }

        public void ResetSpecialBundleItems()
        {
            if (_specialBundleUIs == null)
            {
                _specialBundleUIs = new List<IAPItemBundleUI>();
            }
            else if (_bundleUIs.Count > 0)
            {
                for (int i = 0; i < _specialBundleUIs.Count; i++)
                {
                    _specialBundleUIs[i].gameObject.SetActive(false);
                    _specialBundleUIs[i].EnableBundleCountDown(false);
                }
            }
        }

        public void SetupScrollItems(List<IAPBundle> bundles, bool showMoreOffer)
        {
            float scrollItemPos = 0.0f;
            float height = 0;
            float deltaY = 20;
            int packIndex = 0;
            int bundleIndex = 0;
            bool isFirstItem = true;
            float bottomY = 0;
            for (int i = 0; i < bundles.Count; i++)
            {
                IAPBundle bundle = bundles[i];
                if (bundle.BundleType == IAPBundleType.NoAds)
                {
                    IAPItemCoinUI removeAdItem = GetRemoveAdItem();
                    removeAdItem.Setup(bundle);
                    removeAdItem.gameObject.SetActive(true);
                    removeAdItem.onBuyPressed = OnBuyPressed;
                    RectTransform rt = removeAdItem.GetComponent<RectTransform>();
                    rt.anchoredPosition = new Vector2(rt.anchoredPosition.x, -height - 30);
                    bottomY = rt.anchoredPosition.y - rt.sizeDelta.y;
                    height += rt.sizeDelta.y + deltaY;
                    Debug.Log("SetupScrollItems NoAds height = " + height.ToString());
                    continue;
                }
                else if (bundle.BundleType == IAPBundleType.Pack)
                {
                    IAPItemCoinUI iapCoinItem = GetIAPItem(packIndex);
                    if (iapCoinItem != null)
                    {
                        iapCoinItem.Setup(bundle);
                        iapCoinItem.onBuyPressed = OnBuyPressed;
                        iapCoinItem.gameObject.SetActive(true);

                        RectTransform rt = iapCoinItem.GetComponent<RectTransform>();
                        rt.anchoredPosition = new Vector2(rt.anchoredPosition.x, -height - 30);

                        bottomY = rt.anchoredPosition.y - rt.sizeDelta.y;
                        height += rt.sizeDelta.y + deltaY;
                    }
                    packIndex++;
                    continue;
                }
                else if (bundle.BundleType == IAPBundleType.Bundle)
                {
                    IAPItemBundleUI iapBundleItem = GetIAPBundleItem(bundleIndex);
                    if (iapBundleItem != null)
                    {
                        iapBundleItem.Setup(bundle);
                        iapBundleItem.onBuyPressed = OnBuyPressed;
                        iapBundleItem.gameObject.SetActive(true);

                        RectTransform rt = iapBundleItem.GetComponent<RectTransform>();
                        rt.anchoredPosition = new Vector2(rt.anchoredPosition.x, -height - 30);

                        bottomY = rt.anchoredPosition.y - rt.sizeDelta.y;
                        height += rt.sizeDelta.y + deltaY;
                    }
                    bundleIndex++;
                    continue;
                }
                else if (bundle.BundleType == IAPBundleType.SBundle)
                {
                    IAPItemBundleUI iapBundleItem = GetIAPSpecialBundleItem(bundleIndex);
                    if (iapBundleItem != null)
                    {
                        iapBundleItem.Setup(bundle);
                        iapBundleItem.onBuyPressed = OnBuyPressed;
                        iapBundleItem.gameObject.SetActive(true);
                        iapBundleItem.EnableBundleCountDown(true, IAPManager.Instance.GetSpecialBundleEndTime(bundle.BundleId));

                        RectTransform rt = iapBundleItem.GetComponent<RectTransform>();
                        rt.anchoredPosition = new Vector2(rt.anchoredPosition.x, -height - 30);

                        bottomY = rt.anchoredPosition.y - rt.sizeDelta.y;
                        height += rt.sizeDelta.y + deltaY;
                    }
                    bundleIndex++;
                    continue;
                }

                if (isFirstItem)
                {
                    isFirstItem = false;
                    scrollItemPos = -bottomY;
                }
            }
            _moreOffer.gameObject.SetActive(showMoreOffer);
            if (showMoreOffer)
            {
                _moreOffer.anchoredPosition = new Vector2(_moreOffer.anchoredPosition.x, -height - 30);
                height += _moreOffer.sizeDelta.y + deltaY;
            }
#if UNITY_ANDROID
            _restorePanel.gameObject.SetActive(false);
#elif UNITY_IOS
            _restorePanel.gameObject.SetActive(!showMoreOffer);
            if (!showMoreOffer)
            {
                _restorePanel.anchoredPosition = new Vector2(_restorePanel.anchoredPosition.x, -height - 30);
                height += _restorePanel.sizeDelta.y + deltaY;
            }
#endif
            height += deltaY;
            _scrollContent.sizeDelta = new Vector2(_scrollContent.sizeDelta.x, height);

            float contentHeight = _scroll.GetComponent<RectTransform>().rect.height;
            _scrollContent.anchoredPosition = new Vector2(_scrollContent.anchoredPosition.x, Mathf.Max(scrollItemPos - contentHeight, 0));
        }

        public void Setup(List<IAPBundle> bundles, bool isFull = false)
        {
            //_textCoinAmount.text = AppManager.Instance.UserStorage.GetQuantity(ItemId.Coin).ToString("N0");
            ResetPackItems();
            ResetBundleItems();
            ResetSpecialBundleItems();
            if (_removeAdItem != null)
            {
                _removeAdItem.gameObject.SetActive(false);
            }
            List<IAPBundle> bundleList = new List<IAPBundle>();

            int maxPack = 2;
            int maxBundle = 2;
            int maxSBundle = 1;
            int countPack = 0;
            int countBundle = 0;
            int countSBundle = 0;
            List<SBundleOfferData> specialOffers = IAPManager.Instance.SpecialBundleOffers.Where(o =>
            {
                long now = System.DateTimeOffset.Now.ToUnixTimeSeconds();
                return now >= o.Start && now < o.End;
            }).OrderBy(o => o.End).ToList();
            foreach (SBundleOfferData offerData in specialOffers)
            {
                if (!isFull && countSBundle >= maxSBundle)
                {
                    break;
                }
                IAPBundle bundle = bundles.Find(o => o.BundleId.Equals(offerData.BundleId));
                if (bundle != null && bundle.IsSelling)
                {
                    bundleList.Add(bundle);
                    countSBundle++;
                    maxBundle--;
                }
            }
            foreach (IAPBundle bundle in bundles)
            {
                /*if (bundle.BundleType == IAPBundleType.NoAds)
                {
                    if (!isFull || AppManager.Instance.UserStorage.GetQuantity(ItemId.RemoveAds) > 0)
                    {
                        continue;
                    }
                    bundleList.Add(bundle);
                }
                else */if (bundle.BundleType == IAPBundleType.Pack)
                {
                    if (!isFull && countPack >= maxPack)
                    {
                        continue;
                    }
                    bundleList.Add(bundle);
                    countPack++;
                }
                else if (bundle.BundleType == IAPBundleType.Bundle)
                {
                    if (!isFull && countBundle >= maxBundle)
                    {
                        continue;
                    }
                    bundleList.Add(bundle);
                    countBundle++;
                }
            }

            SetupScrollItems(bundleList, !isFull);

            UIAnimUtils.AssignAnimForChildButtons(this.transform);
        }

        public override void Show()
        {
            base.Show();

            onShowStarted?.Invoke();
            UIAnimUtils.PlayShowPopUpAnim(this.transform, onShowFinished);
        }

        public override void Hide()
        {
            base.Hide();

            onHideStarted?.Invoke();
            UIAnimUtils.PlayHidePopUpAnim(this.transform, onHideFinished);
        }

        public override void FadeIn()
        {
            onFadeOutStarted?.Invoke();
            CanvasGroup canvasGroup = GetComponent<CanvasGroup>();
            canvasGroup.DOKill();
            canvasGroup.DOFade(1f, Global.UIConfig.FADING_TIME).SetEase(Ease.OutSine).OnComplete(() => {
                onFadeInFinished?.Invoke();
            });
        }

        public override void FadeOut()
        {
            onFadeOutStarted?.Invoke();
            CanvasGroup canvasGroup = GetComponent<CanvasGroup>();
            canvasGroup.DOKill();
            canvasGroup.DOFade(0f, Global.UIConfig.FADING_TIME).SetEase(Ease.OutSine).OnComplete(() => {
                onFadeOutFinished?.Invoke();
            });
        }

        public void OnBuyPressed(IAPBundle bundle)
        {
            onBuyPressed?.Invoke(bundle);
        }

        public void OnClosePressed()
        {
            AudioManager.Instance.PlaySfxTapButton();
            onClosePressed?.Invoke();
        }

        public void OnRestorePressed()
        {
            AudioManager.Instance.PlaySfxTapButton();
            onRestorePressed?.Invoke();
        }

        public void OnMoreOfferPressed()
        {
            AudioManager.Instance.PlaySfxTapButton();
            onMoreOfferPressed?.Invoke();
        }

        private IAPItemCoinUI GetRemoveAdItem()
        {
            if (_removeAdItem == null)
            {
                _removeAdItem = Instantiate(_removeAdItemPrefab, _scrollContent.transform);
            }
            return _removeAdItem;
        }

        private IAPItemCoinUI GetIAPItem(int index)
        {
            IAPItemCoinUI iapCoinItem;
            if (index >= _itemUIs.Count)
            {
                iapCoinItem = Instantiate(_coinItemPrefab, _scrollContent.transform);
                _itemUIs.Add(iapCoinItem);
            }
            else if (_itemUIs[index] == null)
            {
                iapCoinItem = Instantiate(_coinItemPrefab, _scrollContent.transform);
                _itemUIs[index] = iapCoinItem;
            }
            else
            {
                iapCoinItem = _itemUIs[index];
            }
            return iapCoinItem;
        }

        private IAPItemBundleUI GetIAPBundleItem(int index)
        {
            IAPItemBundleUI iapBundleItem;
            if (index >= _bundleUIs.Count)
            {
                iapBundleItem = Instantiate(_bundleItemPrefab, _scrollContent.transform);
                _bundleUIs.Add(iapBundleItem);
            }
            else if (_bundleUIs[index] == null)
            {
                iapBundleItem = Instantiate(_bundleItemPrefab, _scrollContent.transform);
                _bundleUIs[index] = iapBundleItem;
            }
            else
            {
                iapBundleItem = _bundleUIs[index];
            }
            return iapBundleItem;
        }

        private IAPItemBundleUI GetIAPSpecialBundleItem(int index)
        {
            IAPItemBundleUI iapBundleItem;
            if (index >= _specialBundleUIs.Count)
            {
                iapBundleItem = Instantiate(_specialBundleItemPrefab, _scrollContent.transform);
                _specialBundleUIs.Add(iapBundleItem);
            }
            else if (_specialBundleUIs[index] == null)
            {
                iapBundleItem = Instantiate(_specialBundleItemPrefab, _scrollContent.transform);
                _specialBundleUIs[index] = iapBundleItem;
            }
            else
            {
                iapBundleItem = _specialBundleUIs[index];
            }
            return iapBundleItem;
        }

        protected override void OnUIRefresh()
        {
            if (_specialBundleUIs != null)
            {
                for (int i = 0; i < _specialBundleUIs.Count; i++)
                {
                    _specialBundleUIs[i].UpdateCountDownTime();
                }
            }
        }
    }
}