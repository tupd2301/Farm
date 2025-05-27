using System.Collections;
using System.Collections.Generic;
using Athena.Common.UI;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Atom
{
    public class PiggyBankRewardUI : UIController
    {
        public event System.Action OnTapToSkip;
        public event System.Action OnCoinAnimationCompleted;

        public FloatingTextUI FloatingTextUI;

        public Image Image;
        public Button TapToSkip;
        public TextMeshProUGUI PiggyCoin;

        private int _coin;
        private IList<int> _coinRewardPerStar;
        private PiggyBankLevelData _currentLevelData;

        [SerializeField]
        private Animator _starAnimation;
        [SerializeField]
        private List<Sprite> piggyCoinSpriteLevels;
        [SerializeField]
        private GameObject[] _stars;

        public void Setup(IList<int> coinRewardPerStar, int piggyCoin)
        {
            _coinRewardPerStar = coinRewardPerStar;
            _coin = piggyCoin;
            Image.sprite = piggyCoinSpriteLevels[PiggyBankManager.Instance.PiggyBankLogic.PiggyLevel - 1];
            PiggyCoin.text = piggyCoin.ToString();
        }

        public void StartStarAnimation(int starFrom, int starTo)
        {
            StartCoroutine(startStarAnimation(starFrom, starTo));
        }

        protected override void OnUIStart()
        {
            int level = PiggyBankManager.Instance.PiggyBankLogic.PiggyLevel - 1;
            _currentLevelData = PiggyBankManager.Instance.PiggyBankLogic.Config.Levels[level];
        }

        protected override void OnUIRemoved()
        {
            StopAllCoroutines();
        }

        private void OnDestroy()
        {
            StopAllCoroutines();
        }

        private IEnumerator startStarAnimation(int starFrom, int starTo)
        {
            if (starTo > _stars.Length)
            {
                throw new System.Exception($"Invalid starTo is > {_stars.Length}");
            }
            else
            {
                setActiveClearedStar(starFrom);

                //Star appear animation
                yield return startStarsPopAnimation(starFrom, starTo);

                //Coin popping animation    
                int bonusCoin = 0;
                for (int i = starFrom; i < starTo; i++)
                {
                    bonusCoin += _coinRewardPerStar[i];
                    yield return startPopAnimation(i + 1);
                    if (i == starTo - 1)
                    {
                        addPiggyBankCoin(bonusCoin);
                        onLastCoinAnimationReachThePig(bonusCoin);
                    }
                    yield return Yielders.Get(0.1f);
                }
            }
            yield return Yielders.Get(1f);
            OnCoinAnimationCompleted?.Invoke();
        }

        private void addPiggyBankCoin(int bonusCoin)
        {
            if (_coin >= _currentLevelData.MaxCoin)
            {
                return;
            }

            bonusCoin = calculateBonusCoin(bonusCoin);
            PiggyCoin.text = $"{_coin + bonusCoin}";
        }

        private void setActiveClearedStar(int starFrom)
        {
            for (int i = 0; i < starFrom; i++)
            {
                _stars[i].SetActive(true);
            }
        }

        private IEnumerator startPopAnimation(int star)
        {
            if(isPiggyBankFull() == false)
            {
                //AudioPlayer.sharedInstance.PlaySound(C.AudioIds.Sound.GetCoin);
            }
            else
            {
                //AudioPlayer.sharedInstance.PlaySound(C.AudioIds.Sound.SwapBubble);
            }
            yield return startCoinAnimationAnimation(star);
        }

        private IEnumerator startStarsPopAnimation(int starFrom, int starTo)
        {
            for (int i = starFrom; i < starTo; i++)
            {
                _stars[i].SetActive(true);
            }

            yield return startAnimation($"{starTo - starFrom}Stars_Pop", 0.4f);
        }

        private IEnumerator startCoinAnimationAnimation(int star)
        {
            if (isPiggyBankFull())
            {
                yield return startAnimation($"Star{star}_CoinFull", 0.35f);
            }
            else
            {
                yield return startAnimation($"Star{star}_Coin", 0.4f);
            }
        }

        private IEnumerator startAnimation(string animationName, float time)
        {
            _starAnimation.Play(animationName);
            yield return Yielders.Get(time);
        }

        public void onTapToSkip()
        {
            OnTapToSkip?.Invoke();
        }

        private void onLastCoinAnimationReachThePig(int bonusCoin)
        {
            if (_coin >= _currentLevelData.MaxCoin)
            {
                return;
            }

            bonusCoin = calculateBonusCoin(bonusCoin);

            FloatingTextUI.Value = $"+{bonusCoin}";
            FloatingTextUI.DoFxFloating();
        }

        private int calculateBonusCoin(int bonusCoin)
        {
            int remain = _currentLevelData.MaxCoin - _coin;
            if (remain < bonusCoin)
            {
                int minusCoin = bonusCoin - remain;
                bonusCoin -= minusCoin;
            }

            return bonusCoin;
        }

        private bool isPiggyBankFull()
        {
            return _coin >= _currentLevelData.MaxCoin;
        }
    }

}