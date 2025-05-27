using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CustomUtils;
using Athena.Common.UI;

namespace Atom
{
    public class DailyFreeCoinManager : SingletonMono<DailyFreeCoinManager>
    {
        private DailyFreeCoinsUI _dailyFreeCoinUI;
        public void ShowUI()
        {
            showDailyFreeCoins(0);
        }
        /*
        public void ShowDailyFreeCoinsOnFreeUserCloseShop(int layer = 0, System.Action onClose = null)
        {
            bool isRemainDailyFreeCoin = G.ProfileService.DailyFreeCoinRollCount < G.DataService.AdConfig.RwCappingFreeCoin;
            if (G.ProfileService.IsFreeUser() && isRemainDailyFreeCoin)
            {
                showDailyFreeCoins(layer, onClose);
                _dailyFreeCoinsPlacementAds = C.RewardedAdsPlacement.DailyFreeCoinsShop;
            }
            else
            {
                onClose?.Invoke();
            }
        }
        */
        /*
        public void ShowDailyFreeCoinsByIcon(int layer = 0)
        {
            showDailyFreeCoins(layer);
            _dailyFreeCoinsPlacementAds = C.RewardedAdsPlacement.DailyFreeCoinsOpenIcon;
        }

        public void ShowAutoDailyFreeCoins(int layer = 0)
        {
            showDailyFreeCoins(layer);
            _dailyFreeCoinsPlacementAds = C.RewardedAdsPlacement.DailyFreeCoinsAutoPopup;
        }
        */
        private void showDailyFreeCoins(int layer, System.Action onClose = null)
        {
            if (_dailyFreeCoinUI != null)
            {
                UIManager.Instance.ReleaseUI(_dailyFreeCoinUI, true);
            }
            _dailyFreeCoinUI = AppManager.Instance.ShowSafeTopUI<DailyFreeCoinsUI>("Atom/DailyFreeCoinsUI", false);

            _dailyFreeCoinUI.OnClosePopup += onDailyFreeCoinCloseBtn;
            _dailyFreeCoinUI.OnClosePopup += onClose;

            _dailyFreeCoinUI.OnWatchAdsClicked += watchAdsEarnToEarnDailyFreeCoin;

            _dailyFreeCoinUI.Setup(JsonUtility.FromJson<DailyFreeCoinsConfig>(Resources.Load<TextAsset>(string.Format("Atom/{0}", "daily_free_coins_config")).text));
        }

        private void onDailyFreeCoinCloseBtn()
        {
            UIManager.Instance.ReleaseUI(_dailyFreeCoinUI, true);//UIManager.Instance.ReleaseUIWithAnimation(_dailyFreeCoinUI);
        }

        private void watchAdsEarnToEarnDailyFreeCoin(int coinEarned)
        {
            onDailyFreeCoinWatchAdsSuccess(10);//ZTask.TaskRunner.Instance.Run(showRewardedAdsAfterTime(0.5f, coinEarned));
        }
        /*
        private IEnumerator showRewardedAdsAfterTime(float time, int coinEarned)
        {
            yield return Yielders.Get(time);
            Instance.ShowRewardedAds(_dailyFreeCoinsPlacementAds, (success) =>
            {
                if (success)
                {
                    onDailyFreeCoinWatchAdsSuccess(coinEarned);
                }
                else
                {
                    ZTask.TaskRunner.Instance.Run(continuePatrolAftertime(0.5f));
                }
            });
        }
        */
        private void onDailyFreeCoinWatchAdsSuccess(int coinEarned)
        {
            //_athenaApp.AnalyticsManager.LogBIResource_Source("free_coins", new Data.RewardData { Type = Data.RewardData.RewardType.Currency, Amount = coinEarned }, "home");
            addCoin(coinEarned);
            //minusRemainingTimes();
            //ZTask.TaskRunner.Instance.Run(continuePatrolAftertime(0.5f));
        }
        /*
        private IEnumerator continuePatrolAftertime(float time)
        {
            yield return Yielders.Get(time);
            _dailyFreeCoinUI.ContinuePatrol();
        }
        */
        
        private void addCoin(int coin)
        {
            //G.ProfileService.AddCoin(coin);
            //showPayoutPopup(coin);
            PiggyBankPayoutUI _piggyBankPayoutUI = AppManager.Instance.ShowSafeTopUI<PiggyBankPayoutUI>("Atom/PiggyBankPayoutUI", false);
            _piggyBankPayoutUI.SetupPayoutNormal(coin);
            _piggyBankPayoutUI.OnFlyCompleted = () =>
            {
                UIManager.Instance.ReleaseUI(_piggyBankPayoutUI, true);
            };
        }
        /*
        private void showPayoutPopup(int bonus)
        {
            List<Data.RewardData> reward = new List<Data.RewardData>
            {
                createCoinReward(bonus)
            };
            ShowPayoutPopup(reward, null);
        }

        private Data.RewardData createCoinReward(int bonus)
        {
            Data.RewardData rewardData = new Data.RewardData();
            rewardData.Type = Data.RewardData.RewardType.Currency;
            rewardData.Id = "coin";
            rewardData.Amount = bonus;
            return rewardData;
        }

        private void minusRemainingTimes()
        {
            int dailyFreeCoinRollCount = G.ProfileService.DailyFreeCoinRollCount + 1;
            G.ProfileService.SetRemainingTimes(dailyFreeCoinRollCount);
            _dailyFreeCoinUI.SetRemainingTimesText();
        }
        */
    }
}