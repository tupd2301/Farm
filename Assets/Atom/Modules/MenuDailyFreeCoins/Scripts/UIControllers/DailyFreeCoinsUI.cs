using System;
using System.Collections.Generic;
using Athena.Common.UI;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Atom
{
    public class DailyFreeCoinsUI : UIController
    {
        private const string COIN_ICON = "<sprite=0>";
        private const int DEFAULT_MULTIPLIER = 2;

        public event System.Action<int> OnWatchAdsClicked;
        public event System.Action OnRemainingTimeLast;
        public event System.Action OnClosePopup;

        public Button WatchAdsBtn;
        public Button ConfirmBtn;
        public Button CloseBtn;
        public TextMeshProUGUI DescriptionTxt;
        public TextMeshProUGUI RemainingTimesTxt;
        //public UI.TimeCountDownUI TimeCountDown;

        private DailyFreeCoinsConfig _data;

        [SerializeField]
        private MultiplierIndicator _multiplierIndicator;
        [SerializeField] GameObject _x2Reward;
        
        public void Setup(DailyFreeCoinsConfig layoutData)
        {
            if (layoutData == null)
            {
                Debug.LogError("Data is null!");
                return;
            }
            _data = layoutData;
            UnityEngine.Debug.Log("========== : " + _data.RemainDescription);
            DescriptionTxt.text = layoutData.RemainDescription;
            setButtonText(_data.CoinCount);
            SetRemainingTimesText();
            setupTimer();
            _multiplierIndicator.Setup((List<int>)layoutData.LuckyMultipliers);
            //_x2Reward.gameObject.SetActive(G.FantasyPassOffer.X2RewardIsAvailable);
        }
        
        public void SetRemainingTimesText()
        {
            //int maximumRemainingTime = G.DataService.AdConfig.RwCappingFreeCoin;
            //int remainingTimes = G.DataService.AdConfig.RwCappingFreeCoin - G.ProfileService.DailyFreeCoinRollCount;
            //if (remainingTimes == 0)
            {
            //    onRemainingTimeLasted();
            }

            //string remainingTimesInfo = $"[Remaining times: {remainingTimes}/{maximumRemainingTime}]";
            //RemainingTimesTxt.Value = remainingTimesInfo;
        }

        public void ContinuePatrol()
        {
            _multiplierIndicator.ContinuePatrol();
        }

        protected override void OnUIStart()
        {
            //CloseBtn.onClick += _ => closePopup();
            //ConfirmBtn.OnClicked += _ => closePopup();
            //WatchAdsBtn.OnClicked += _ => watchAds();
            //TimeCountDown.OnTimeCountOver += resetupUI;
            //_multiplierIndicator.OnPatrollingAtMultiplier += onPatrollingUpdated;
        }

        protected override void OnUIRemoved()
        {
            //CloseBtn.OnClicked -= _ => closePopup();
            //ConfirmBtn.OnClicked -= _ => closePopup();
            //WatchAdsBtn.OnClicked -= _ => watchAds();
            //TimeCountDown.OnTimeCountOver -= resetupUI;
            //_multiplierIndicator.OnPatrollingAtMultiplier -= onPatrollingUpdated;
        }

#if UNITY_ANDROID
        protected override void OnBack()
        {
            closePopup();
        }
#endif

        private void onPatrollingUpdated()
        {
            setButtonText(_data.CoinCount, _multiplierIndicator.PatrolValue);
        }

        private void setButtonText(int cointCount, int luckyMultiplier = DEFAULT_MULTIPLIER)
        {
            string buttonText = $"{cointCount} {COIN_ICON} x{luckyMultiplier}";
            //WatchAdsBtn.Text = buttonText;
        }

        public void closePopup()
        {
            OnClosePopup?.Invoke();
        }

        public void watchAds()
        {
            setButtonText(_data.CoinCount, _multiplierIndicator.PatrolValue);
            _multiplierIndicator.Stop();
            int x2 = 1;
            //if (G.FantasyPassOffer.X2RewardIsAvailable) x2 = 2;
            OnWatchAdsClicked?.Invoke(_data.CoinCount * _multiplierIndicator.PatrolValue * x2);
        }

        private void onRemainingTimeLasted()
        {
            WatchAdsBtn.gameObject.SetActive(false);
            ConfirmBtn.gameObject.SetActive(true);
            DescriptionTxt.text = _data.RemainLastDescription;
            OnRemainingTimeLast?.Invoke();
        }

        private void setupTimer()
        {
            DateTime now = DateTime.UtcNow;// Services.PlayfabService.GetUTCTime();
            var nextDay = now.AddDays(1);
            nextDay = new DateTime(nextDay.Year, nextDay.Month, nextDay.Day, 0, 0, 0);
            TimeSpan remainTime = nextDay - now;
            //TimeCountDown.Setup(remainTime);
        }

        private void resetupUI()
        {
            setButtonText(_data.CoinCount);
            SetRemainingTimesText();
            setupTimer();
        }
    }
}