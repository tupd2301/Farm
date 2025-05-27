using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Athena.Common;
using TMPro;

namespace Atom
{
    public class IAPItemCoinUI : MonoBehaviour
    {
        public const string TAG_KEY = "Tag";

        public System.Action<IAPBundle> onBuyPressed;

        [SerializeField]
        private TextMeshProUGUI _countText, _priceText;
        [SerializeField]
        private GameObject _buyBtn;
        [SerializeField]
        private Image _packIcon;
        [SerializeField]
        private GameObject _tagBest, _tagPopular;

        private IAPBundle _bundle;
        public IAPBundle Bundle { get { return _bundle; } }

        public void Setup(IAPBundle bundle)
        {
            _bundle = bundle;
            _priceText.text = _bundle.Price;
            if (_countText != null &&  _bundle.Payout != null && _bundle.Payout.Count > 0)
            {
                _countText.text = bundle.Payout[0].Quantity.ToString("N0");
            }

            bool enableTagBest = false;
            bool enableTagPopular = false;
            if (_bundle.Layout.ContainsKey(TAG_KEY))
            {
                string tag = _bundle.Layout[TAG_KEY];
                enableTagBest = tag == "best";
                enableTagPopular = tag == "popular";
            }
            if (_tagBest != null)
            {
                _tagBest.SetActive(enableTagBest);
            }
            if (_tagPopular != null)
            {
                _tagPopular.SetActive(enableTagPopular);
            }

            string iconKey = Global.GameConfig.IAP_LAYOUT_KEY_PACK_ICON;
            if (_bundle.Layout.ContainsKey(iconKey))
            {
                string coinIcon = _bundle.Layout[iconKey];
                int index = coinIcon switch
                {
                    "s" => 0,
                    "m" => 1,
                    "l" => 2,
                    _ => 0
                };
                //_packIcon.sprite = ResourceManager.Instance.GetCoinPackSpriteByIndex(index);
                _packIcon.SetNativeSize();
            }

            UIAnimUtils.AssignAnimForChildButtons(this.transform);
        }

        public void OnBuyPressed()
        {
            AudioManager.Instance.PlaySfxTapButton();
            onBuyPressed?.Invoke(_bundle);
        }
    }
}