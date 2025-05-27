using Athena.Common.UI;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections;
using TMPro;

namespace Atom
{
    public class PiggyBankPayoutUI : UIController
    {
        public event System.Action OnBreakClicked;
        public System.Action OnFlyCompleted;
        public event System.Action<int> OnEachCoinReachTarget;

        public Image PiggyBank;

        public Button BreakBtn;

        public TextMeshProUGUI PiggyCoin;

        private int _coin;
        private int _piggyCoin;

        [SerializeField]
        private PayoutItemDirectorUI _payoutCoinDirectorUI;
        [SerializeField]
        private Animator _piggyBankBreakAnimator;
        [SerializeField]
        private CanvasGroup _piggyBankGroup;
        [SerializeField]
        private Sprite[] _piggySprites;

        public void SetupPayoutNormal(int coin)
        {
            _piggyCoin = coin;
            PiggyBank.transform.localPosition = new Vector3(1000, 0, 0);
            BreakBtn.transform.localPosition = new Vector3(1000, 0, 0);
            _payoutCoinDirectorUI.StartPayoutAnimation(_piggyCoin);
        }

        public void Setup(RectTransform coinTarget, PiggyBankLevelData levelData)
        {
            //_piggyCoin = G.PiggyBankLogic.PiggyCoin;
            _payoutCoinDirectorUI.Setup(coinTarget);

            //AudioPlayer.sharedInstance.PlaySound(C.AudioIds.Sound.BuySuccess);
            //_coin = G.ProfileService.Coin;
            int index = levelData.Level - 1;
            if (levelData.Level >= _piggySprites.Length)
            {
                index = _piggySprites.Length - 1;
            }
            PiggyBank.sprite = _piggySprites[index];
            //PiggyCoin.Value = G.PiggyBankLogic.PiggyCoin.ToString();
        }

        protected override void OnUIStart()
        {
            //BreakBtn.OnClicked += _ => onBreakClicked();
            _payoutCoinDirectorUI.OnFlyCompleted += onFlyCompleted;
            _payoutCoinDirectorUI.OnFlyCompletedInEachCoinItem += onCoinFlyCompletedInEachCoinItem;
        }

        protected override void OnUIRemoved()
        {
            //BreakBtn.OnClicked -= _ => onBreakClicked();
            _payoutCoinDirectorUI.OnFlyCompleted -= onFlyCompleted;
            _payoutCoinDirectorUI.OnFlyCompletedInEachCoinItem -= onCoinFlyCompletedInEachCoinItem;
        }

        public void ClickButtonBreak()
        {
            onBreakClicked();
        }

        private void onBreakClicked()
        {
            BreakBtn.gameObject.SetActive(false);
            StartCoroutine(startBreakAnimation());
            OnBreakClicked?.Invoke();
        }

        private IEnumerator startBreakAnimation()
        {
            yield return startAnimation("PiggyBankBreak", 0.9167f);
            _payoutCoinDirectorUI.StartPayoutAnimation(_piggyCoin);
            yield return startAnimation("PiggyBankBreakDone", 0.1f);
        }

        private IEnumerator startAnimation(string animationName, float time)
        {
            _piggyBankBreakAnimator.Play(animationName);
            yield return new WaitForSeconds(time);//yield return Yielders.Get(time);
        }

        private void onCoinFlyCompletedInEachCoinItem(int coin)
        {
            _coin += coin;
            int bonusCoin = _coin;
            OnEachCoinReachTarget?.Invoke(bonusCoin);
        }

        private void onFlyCompleted()
        {
            OnFlyCompleted?.Invoke();
        }
    }

}