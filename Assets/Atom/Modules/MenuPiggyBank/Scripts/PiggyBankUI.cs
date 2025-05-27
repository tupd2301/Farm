using System;
using UnityEngine;
using Athena.Common.UI;
using UnityEngine.UI;
using TMPro;

namespace Atom
{
    public class PiggyBankUI : UIController
    {
        public event Action OnCloseClicked;
        public event Action<string> OnBuyClicked;

        public Button CloseBtn;
        public Button BuyBtn;
        public Button OkBtn;

        public TextMeshProUGUI Title;
        public TextMeshProUGUI Description;

        public Price Price;

        private PiggyBankConfig _data;
        private int _purchasableCoin;

        [SerializeField]
        private PiggyBankSprite[] _piggyLevelSprites;
        [SerializeField]
        private PiggyBankContentUI _piggyBankNotFullContent;
        [SerializeField]
        private PiggyBankContentUI _piggyBankFullContent;
        [SerializeField]
        private GameObject _activableIcon;

        public void Setup(PiggyBankConfig data)
        {
            _data = data;

            int level = PiggyBankManager.Instance.PiggyBankLogic.PiggyLevel;

            int piggyCoin = PiggyBankManager.Instance.PiggyBankLogic.PiggyCoin;
            setupLayout(level, piggyCoin);
        }

        public void ClickButtonBuy()
        {
            int level = PiggyBankManager.Instance.PiggyBankLogic.PiggyLevel;
            OnBuyClicked.Invoke(getPiggyBankLevel(level).ProductId);
        }

        public void ClickButtonOK()
        {
            OnCloseClicked?.Invoke();
        }

        public void ClickButtonClose()
        {
            OnCloseClicked?.Invoke();
        }

        private void setupLayout(int level, int piggyCoin)
        {
            if (level > _data.Levels.Count)
            {
                throw new Exception("level have invalid value");
            }
            PiggyBankLevelData levelData = getPiggyBankLevel(level);
            _purchasableCoin = levelData.MaxCoin * levelData.PurchasablePercent / 100;
            Title.text = levelData.Name;
            setupContentLayout(levelData, piggyCoin);
        }

        private PiggyBankLevelData getPiggyBankLevel(int level)
        {
            int index = level - 1;
            return _data.Levels[index];
        }

        private void setupContentLayout(PiggyBankLevelData level, int piggyCoin)
        {
            if (_data.StageDescriptions == null || _data.StageDescriptions.Count <= 0)
            {
                throw new Exception("Data is null or empty!");
            }

            _piggyBankNotFullContent.Setup(level, _piggyLevelSprites, _purchasableCoin);

            if (piggyCoin >= level.MaxCoin)
            {
                onPiggyBankReachMaxCoin(level);
                return;
            }

            if (piggyCoin >= _purchasableCoin)
            {
                onPiggyBankActive(level);
            }
            else
            {
                onPiggyBankInactive();
            }
        }

        private void onPiggyBankActive(PiggyBankLevelData level)
        {
            Description.text = _data.StageDescriptions[1];
            _activableIcon.SetActive(true);
            _piggyBankNotFullContent.Show();
            _piggyBankFullContent.gameObject.SetActive(false);
            setupPrice(level);
            OkBtn.gameObject.SetActive(false);
        }

        private void onPiggyBankReachMaxCoin(PiggyBankLevelData level)
        {
            Description.text = _data.StageDescriptions[2];
            _piggyBankFullContent.Setup(level, _piggyLevelSprites, _purchasableCoin);
            _piggyBankFullContent.Show();
            _piggyBankNotFullContent.gameObject.SetActive(false);
            setupPrice(level);
            OkBtn.gameObject.SetActive(false);
        }

        private void onPiggyBankInactive()
        {
            Description.text = string.Format(_data.StageDescriptions[0], _purchasableCoin);
            _piggyBankNotFullContent.Show();
            _piggyBankFullContent.gameObject.SetActive(false);
            BuyBtn.gameObject.SetActive(false);
        }

        private void setupPrice(PiggyBankLevelData levelData)
        {
            /*
            var iapConfig = G.DataService.GetIAPProduct(levelData.ProductId);
            Price.Setup(new PriceData()
            {
                Type = PriceData.CurrencyType.RM,
                Amount = iapConfig.Price
            });
            */
        }

        private void onCloseClicked()
        {
            OnCloseClicked?.Invoke();
        }

        private void onBuyClicked(string productId)
        {
            if (PiggyBankManager.Instance.PiggyBankLogic.PiggyCoin >= _purchasableCoin)
            {
                OnBuyClicked?.Invoke(productId);
            }
        }
    }

}

[Serializable]
public class PiggyBankSprite
{
    public Sprite[] PiggySprites;
}
