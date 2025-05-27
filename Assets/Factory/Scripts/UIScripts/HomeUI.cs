using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Athena.Common.UI;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Factory
{
    public class HomeUI : UIController
    {
        [SerializeField]
        private GridLayoutGroup _gearItemContainer;

        [SerializeField]
        private GameObject _shopPopup;

        [SerializeField]
        private Button _startButton;

        [SerializeField]
        private Button _rerollButton;

        [SerializeField]
        private TMP_Text _goldText;

        [SerializeField]
        private RectTransform _goldContainer;

        [SerializeField]
        private List<ShopItem> _shopItems = new List<ShopItem>();

        public GridLayoutGroup GearItemContainer => _gearItemContainer;

        public Button StartButton => _startButton;

        public List<ShopItem> ShopItems => _shopItems;

        public GameObject TempContainer;

        public RectTransform GameStartPanel;
        public TMP_Text GameStartLevelText;
        public TMP_Text GameStartDayText;

        private bool _isLockFirstOpenShop = false;

        void Start()
        {
            _rerollButton.onClick.AddListener(OnRerollButtonClick);
        }

        public void LockStartButton()
        {
            _isLockFirstOpenShop = true;
            _startButton.interactable = false;
            _rerollButton.interactable = false;
        }

        public void UnlockStartButton()
        {
            _isLockFirstOpenShop = false;
            _startButton.interactable = true;
            _rerollButton.interactable = true;
            SetLockRerollButton(GameManager.Instance.CheckGold(5));
        }

        public void SetLockRerollButton(bool state)
        {
            if (_isLockFirstOpenShop)
            {
                return;
            }
            _rerollButton.interactable = state;
        }

        public void ShowShopPopup()
        {
            _shopPopup.SetActive(true);
        }

        public void HideShopPopup()
        {
            _shopPopup.SetActive(false);
        }

        public async Task ShowGameStartPanel()
        {
            GameStartPanel.gameObject.SetActive(true);
            GameStartPanel.DOComplete();
            GameStartPanel.anchoredPosition = new Vector2(0, 0);
            GameStartLevelText.text = "";
            GameStartDayText.text = "";
            await GameStartPanel
                .GetComponent<CanvasGroup>()
                .DOFade(1, 0.1f)
                .SetEase(Ease.InSine)
                .AsyncWaitForCompletion();
            await ShowGameStartText();
            await Task.Delay(2000);
            await HideGameStartPanel();
        }

        public async Task ShowGameStartText()
        {
            string levelText = "";
            if (GameManager.Instance.currentLevel == 0)
            {
                levelText = "tutorial";
            }
            else
            {
                levelText = (GameManager.Instance.currentLevel).ToString();
            }
            string textLevel = "Level " + levelText;
            string textDay = "Day " + (GameManager.Instance.currentDay + 1);
            for (int i = 0; i < textLevel.Length; i++)
            {
                await Task.Delay(100);
                GameStartLevelText.text = textLevel.Substring(0, i + 1);
            }
            await Task.Delay(1000);
            GameStartLevelText.text = textLevel;
            for (int i = 0; i < textDay.Length; i++)
            {
                await Task.Delay(100);
                GameStartDayText.text = textDay.Substring(0, i + 1);
            }
            await Task.Delay(1000);
            GameStartDayText.text = textDay;
        }

        public async Task ShowWinPanel()
        {
            GameStartPanel.gameObject.SetActive(true);
            GameStartPanel.DOComplete();
            GameStartPanel.anchoredPosition = new Vector2(0, 0);
            GameStartLevelText.text = "";
            GameStartDayText.text = "";
            string levelText = "";
            if (GameManager.Instance.currentLevel == 0)
            {
                levelText = "tutorial";
            }
            else
            {
                levelText = (GameManager.Instance.currentLevel).ToString();
            }
            await GameStartPanel
                .GetComponent<CanvasGroup>()
                .DOFade(1, 0.5f)
                .SetEase(Ease.InSine)
                .AsyncWaitForCompletion();
            string text = "Congratulations! You have completed Level " + levelText + "!";
            for (int i = 0; i < text.Length; i++)
            {
                await Task.Delay(100);
                GameStartLevelText.text = text.Substring(0, i + 1);
            }
            await Task.Delay(1000);
            GameStartLevelText.text = text;
            await Task.Delay(1000);
            await Task.Delay(2000);
            await HideGameStartPanel();
        }

        public async Task ShowLosePanel()
        {
            GameStartPanel.gameObject.SetActive(true);
            GameStartPanel.DOComplete();
            GameStartPanel.anchoredPosition = new Vector2(0, 0);
            GameStartLevelText.text = "";
            GameStartDayText.text = "";
            await GameStartPanel
                .GetComponent<CanvasGroup>()
                .DOFade(1, 0.5f)
                .SetEase(Ease.InSine)
                .AsyncWaitForCompletion();
            string text = "You have lost the game!";
            for (int i = 0; i < text.Length; i++)
            {
                await Task.Delay(100);
                GameStartLevelText.text = text.Substring(0, i + 1);
            }
            await Task.Delay(1000);
            GameStartLevelText.text = text;
            await Task.Delay(1000);
            await Task.Delay(2000);
            await HideGameStartPanel();
        }

        public async Task HideGameStartPanel()
        {
            GameStartPanel.DOComplete();
            await GameStartPanel
                .GetComponent<CanvasGroup>()
                .DOFade(0, 0.5f)
                .SetEase(Ease.InSine)
                .AsyncWaitForCompletion();
            GameStartPanel.gameObject.SetActive(false);
        }

        public void OnStartButtonClick()
        {
            GameManager.Instance.StartGame();
        }

        public void UpdateCostText(ShopItem shopItem, int cost)
        {
            shopItem.price.text = cost.ToString();
        }

        public void UpdatePanelImage(ShopItem shopItem, bool isPurchased = false)
        {
            shopItem.panelImage.color = isPurchased
                ? new Color(0.15f, 0.15f, 0.15f, 1)
                : new Color(0.4438857f, 0.8962264f, 0.4496849f, 1);
        }

        public void StrikethroughCostText(ShopItem shopItem, bool isStrikethrough = false)
        {
            if (shopItem == null)
            {
                return;
            }
            shopItem.price.fontStyle = isStrikethrough
                ? FontStyles.Strikethrough
                : FontStyles.Normal;
            shopItem.priceStrikethrough = isStrikethrough;
        }

        public void UpdateGoldText(int gold)
        {
            _goldText.text = gold.ToString();
            _goldContainer.DOComplete();
            _goldContainer.DOScale(1.05f, 0.1f).SetEase(Ease.InSine).SetLoops(2, LoopType.Yoyo);
            _goldContainer
                .GetComponent<Image>()
                .DOColor(new Color(1, 1, 1, 0.5f), 0.1f)
                .SetEase(Ease.InSine)
                .SetLoops(2, LoopType.Yoyo);
        }

        public void WarningGoldPanel()
        {
            _goldContainer.DOComplete();
            _goldContainer.DOScale(1.05f, 0.1f).SetEase(Ease.InSine).SetLoops(2, LoopType.Yoyo);
            _goldContainer
                .GetComponent<Image>()
                .DOColor(new Color(1, 0.7f, 0.7f, 1), 0.1f)
                .SetEase(Ease.InSine)
                .SetLoops(2, LoopType.Yoyo);
        }

        public void OnRerollButtonClick()
        {
            if (!GameManager.Instance.CheckGold(5))
            {
                WarningGoldPanel();
                return;
            }
            GameManager.Instance.RandomGearsInShop();
        }
    }

    [System.Serializable]
    public class ShopItem
    {
        public GearController gear;
        public TMP_Text price;
        public bool purchased = false;
        public bool priceStrikethrough = false;
        public Image panelImage;
    }
}
