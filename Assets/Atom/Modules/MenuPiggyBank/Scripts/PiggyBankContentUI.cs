using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Atom
{
    public class PiggyBankContentUI : MonoBehaviour
    {
        public TextMeshProUGUI Coin;
        public Image PiggyImage;
        public ProcessBarUI OptProcessBar;

        public void Setup(PiggyBankLevelData levelData, PiggyBankSprite[] piggyBankSprites, int purchasableCoin)
        {
            int piggyCoin = PiggyBankManager.Instance.PiggyBankLogic.PiggyCoin;
            Coin.text = piggyCoin.ToString();

            setImageByPiggyBankLevel(levelData, piggyBankSprites);
            if (OptProcessBar != null)
            {
                OptProcessBar.Setup(levelData.MaxCoin, piggyCoin, purchasableCoin);
            }
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        public void Show()
        {
            gameObject.SetActive(true);
        }

        private void setImageByPiggyBankLevel(PiggyBankLevelData piggyBankLevelData, PiggyBankSprite[] piggyBankSprites)
        {
            int index = getIndexByLevel(piggyBankLevelData.Level);
            if (piggyBankLevelData.Level >= piggyBankSprites.Length)
            {
                index = piggyBankSprites.Length - 1;
            }
            PiggyBankSprite piggyBankSprite = piggyBankSprites[index];
            setPiggyBankSprite(piggyBankLevelData, piggyBankSprite);
        }

        private int getIndexByLevel(int level)
        {
            return level - 1;
        }

        private void setPiggyBankSprite(PiggyBankLevelData piggyBankLevelData, PiggyBankSprite piggyBankSprite)
        {
            Sprite sprite = piggyBankSprite.PiggySprites[0];
            if (PiggyBankManager.Instance.PiggyBankLogic.PiggyCoin == piggyBankLevelData.MaxCoin)
            {
                //Piggy smile sprite
                sprite = piggyBankSprite.PiggySprites[1];
            }
            PiggyImage.sprite = sprite;
        }
    }
}