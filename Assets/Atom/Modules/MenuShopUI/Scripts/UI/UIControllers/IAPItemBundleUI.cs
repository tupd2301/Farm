using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Athena.Common;
using TMPro;

namespace Atom
{
    public class IAPItemBundleUI : MonoBehaviour
    {
        public const string TAG_KEY = "Tag";

        public System.Action<IAPBundle> onBuyPressed;

        [SerializeField]
        private TextMeshProUGUI _coinText, _priceText, _titleText;
        [SerializeField]
        private GameObject _buyBtn;
        [SerializeField]
        private Image _coinIcon;
        [SerializeField]
        private GameObject _tagBest, _tagPopular, _tagOnce;
        [SerializeField]
        private RectTransform _boosterContent;
        [SerializeField]
        private ItemUI[] _boosterItems;
        [SerializeField]
        private GameObject _countDownPanel;
        [SerializeField]
        private TextMeshProUGUI _countDownText;

        private IAPBundle _bundle;
        public IAPBundle Bundle { get { return _bundle; } }

        public void Setup(IAPBundle bundle)
        {
            _bundle = bundle;
            _priceText.text = _bundle.Price;
            if (_titleText != null)
            {
                _titleText.text = bundle.BundleName;
            }
            if (_bundle.Payout != null && _bundle.Payout.Count > 0)
            {
                List<Item> boosterList = new List<Item>();
                foreach (Item item in _bundle.Payout)
                {
                    if (item.Id == ItemId.Coin)
                    {
                        _coinText.text = item.Quantity.ToString("N0");
                    }
                    else if (item.Category == ItemCategory.Booster)
                    {
                        boosterList.Add(item);
                    }
                }

                float width = _boosterContent.sizeDelta.x;
                float boosterCount = Mathf.Min(boosterList.Count, _boosterItems.Length);
                float deltaX = width / boosterCount;
                for (int i = 0; i< _boosterItems.Length; i++)
                {
                    if (i >= boosterList.Count)
                    {
                        _boosterItems[i].gameObject.SetActive(false);
                        continue;
                    }
                    _boosterItems[i].gameObject.SetActive(true);
                    _boosterItems[i].SetupSmallIcon(boosterList[i]);
                    RectTransform rt = _boosterItems[i].GetComponent<RectTransform>();
                    rt.anchoredPosition = new Vector2((i + 0.5f) * deltaX, rt.anchoredPosition.y);
                }
            }

            bool enableTagBest = false;
            bool enableTagPopular = false;
            bool enableTagOnce = false;
            if (_bundle.Layout.ContainsKey(TAG_KEY))
            {
                string tag = _bundle.Layout[TAG_KEY];
                enableTagBest = tag == "best";
                enableTagPopular = tag == "popular";
                enableTagOnce = tag == "once";
            }
            if (_tagBest != null)
            {
                _tagBest.SetActive(enableTagBest);
            }
            if (_tagPopular != null)
            {
                _tagPopular.SetActive(enableTagPopular);
            }
            if (_tagOnce != null)
            {
                _tagOnce.SetActive(enableTagOnce);
            }

            string iconKey = Global.GameConfig.IAP_LAYOUT_KEY_PACK_ICON;
            if (_coinIcon!= null && _bundle.Layout.ContainsKey(iconKey))
            {
                string coinIcon = _bundle.Layout[iconKey];
                int index = coinIcon switch
                {
                    "s" => 0,
                    "m" => 1,
                    "l" => 2,
                    _ => 0
                };
                //_coinIcon.sprite = ResourceManager.Instance.GetCoinPackSpriteByIndex(index);
                _coinIcon.SetNativeSize();
            }

            UIAnimUtils.AssignAnimForChildButtons(this.transform);
        }

        private bool _enableCountDown = false;
        private long _endtime = 0;
        public void EnableBundleCountDown(bool enable, long endTime = 0)
        {
            _enableCountDown = enable;
            if (_countDownPanel != null)
            {
                _countDownPanel.SetActive(_enableCountDown);
            }
            if (_enableCountDown)
            {
                _endtime = endTime;
                UpdateCountDownTime();
            }
        }

        public void UpdateCountDownTime()
        {
            if (_enableCountDown)
            {
                long remainTime = _endtime - System.DateTimeOffset.UtcNow.ToUnixTimeSeconds();
                if (remainTime < 0)
                {
                    _enableCountDown = false;
                    _countDownText.text = "Last call";
                }
                else
                {
                    if (_countDownText != null)
                    {
                        System.TimeSpan ts = System.TimeSpan.FromSeconds(remainTime);
                        _countDownText.text = UIAnimUtils.TimeSpanToString(ts);
                    }
                }
            }
        }

        public void OnBuyPressed()
        {
            AudioManager.Instance.PlaySfxTapButton();
            onBuyPressed?.Invoke(_bundle);
        }
    }
}