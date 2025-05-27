using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Athena.Common;
using TMPro;

namespace Atom
{
    public class IAPItemRotateUI : MonoBehaviour
    {
        public const string ENABLED_TAG_KEY = "EnabledTag";

        public System.Action<IAPBundle> onWatchAdsPressed, onBuyPressed, onFreePressed;

        [SerializeField]
        private TextMeshProUGUI _countText, _priceText;
        [SerializeField]
        private Button _freeBtn, _watchAdsBtn, _buyBtn;
        [SerializeField]
        private GameObject _greyBtn;
        [SerializeField]
        private Image _tag;

        private IAPBundle _bundle;
        public IAPBundle Bundle { get { return _bundle; } }

        public void Setup(IAPBundle bundle)
        {
            _bundle = bundle;
            _priceText.text = _bundle.Price;
            if (_bundle.Payout != null && _bundle.Payout.Count > 0)
            {
                _countText.text = bundle.Payout[0].Quantity.ToString();
            }
            if (_bundle.Layout.ContainsKey(ENABLED_TAG_KEY))
            {
                _tag.gameObject.SetActive(bool.Parse(_bundle.Layout[ENABLED_TAG_KEY]));
            }

            UIAnimUtils.AssignAnimForChildButtons(this.transform);
        }

        public void SetActiveFreeButton(bool isActive)
        {
            _freeBtn.gameObject.SetActive(isActive);
        }

        public void SetActiveWatchAdsButton(bool isActive)
        {
            _watchAdsBtn.gameObject.SetActive(isActive);
        }

        public void SetActiveGreyButton(bool isActive)
        {
            _greyBtn.SetActive(isActive);
        }

        public void SetActiveBuyButton(bool isActive)
        {
            _buyBtn.gameObject.SetActive(isActive);
        }

        public void SetActiveTag(bool isActive)
        {
            _tag.gameObject.SetActive(isActive);
        }

        public void OnFreePressed()
        {
            AudioManager.Instance.PlaySfxTapButton();
            onFreePressed?.Invoke(_bundle);
        }

        public void OnWatchAdsPressed()
        {
            AudioManager.Instance.PlaySfxTapButton();
            onWatchAdsPressed?.Invoke(_bundle);
        }

        public void OnBuyPressed()
        {
            AudioManager.Instance.PlaySfxTapButton();
            onBuyPressed?.Invoke(_bundle);
        }
    }
}