using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CustomUtils;
using System;
using Athena.Common.UI;

namespace Atom
{
    public class PiggyBankManager : SingletonMono<PiggyBankManager>
    {
        public PiggyBankLogic PiggyBankLogic;
        public ProfileService ProfileService;
        private PiggyBankLevelUpUI _piggyBankLevelUp;

        public event Action OnPiggyBankPayoutClose;

        PiggyBankUI _piggyBankUI;
        PiggyBankRewardUI _piggyBankRewardUI;
        //LevelTargetUI _levelTargetUI;



        #region Payout
        /*
        public PiggyBankPayoutUI ShowPiggyBankPayout(IAPReward.IAPRewardItem iapRewardItem)
        {
            _piggyBankPayout = showPiggyBankPopup(iapRewardItem);
            ShowCurrencyHUD(G.ProfileService.Coin, C.CanvasLayer.Mid);
            setupPiggyBankPayout();
            return _piggyBankPayout;
        }
        */
        public void ShowPiggyBankLevelUp(int fromLevel, int toLevel)
        {
            _piggyBankLevelUp = UIManager.Instance.ShowUIOnTop<PiggyBankLevelUpUI>(C.Layer.PiggyBankLevelUp, C.CanvasLayer.Mid);
            _piggyBankLevelUp.Setup(fromLevel, toLevel);
        }
        /*
        public void OnBuyPiggyBank(string productId, IAPProductMeta meta, Action onComplete = null)
        {
            BuyIAP(productId, (bool isSuccess) =>
            {
                if (isSuccess && _piggyBank != null)
                {
                    UIManager.Instance.ReleaseUI(_piggyBank, true);
                }
                onComplete?.Invoke();
            }, meta);
        }

        private PiggyBankPayoutUI showPiggyBankPopup(IAPReward.IAPRewardItem iapRewardItem)
        {
            if (G.PiggyBankLogic.IsContainCoinReward(iapRewardItem.Rewards) == false)
            {
                throw new Exception("Invalid Reward for PiggyBank!!, Reward not contain coin");
            }

            return UIManager.Instance.ShowUIOnTop<PiggyBankPayoutUI>(C.Layer.PiggyBankPayout, C.CanvasLayer.Mid);
        }

        private void setupPiggyBankPayout()
        {
            int piggyLevel = G.PiggyBankLogic.PiggyLevel;
            PiggyBankLevelData piggyBankLevelData = G.PiggyBankLogic.Config.Levels[piggyLevel - 1];

            _piggyBankPayout.OnFlyCompleted += updateCoinOnClosePiggyPayoutPopup;
            _piggyBankPayout.OnEachCoinReachTarget += updateCoinEachTimeReachTarget;
            _piggyBankPayout.Setup(_currencyHUD.CoinIcon.rectTransform, piggyBankLevelData);

            SaveDataHelper.Set(Managers.GameLogic.PiggyBankLogic.PiggyBankRemindStateKey, true);
            G.PiggyBankLogic.AddPiggyCoinIntoUser();
            G.PiggyBankLogic.UpdateProfilePiggyLevel();
        }

        private void updateCoinOnClosePiggyPayoutPopup()
        {
            UIManager.Instance.ReleaseUI(_piggyBankPayout, true);
            ReleaseCurrencyHUD();
            OnPiggyBankPayoutClose?.Invoke();
        }

        private void updateCoinEachTimeReachTarget(int coin)
        {
            _currencyHUD.SetCoinValue(coin);
        }
        */
        #endregion



        #region Remind UI
        //Show remind
        /*
        private void showPiggyBank(Action onClose)
        {
            int piggyLevelIndex = G.PiggyBankLogic.PiggyLevel - 1;
            PiggyBankLevelData piggyData = G.PiggyBankLogic.Config.Levels[piggyLevelIndex];

            bool needShowLevelStart = G.PiggyBankLogic.IsPiggyBankForceToHome(piggyData);
            SaveDataHelper.Set(GameLogic.PiggyBankLogic.PiggyBankRemindStateKey, false);
            G.IAPService.SetSourceScreen(C.SourceScreen.home_autooffer);

            _piggyBankUI = UIManager.Instance.ShowUIOnTop<PiggyBankUI>(C.Layer.PiggyBank);
            var currencyHUD = Instance.ShowCurrencyHUD(G.ProfileService.Coin);
            currencyHUD.OnBackClick += () => onClosePiggyBank(needShowLevelStart);

            _piggyBankUI.Setup(G.PiggyBankLogic.Config);

            _piggyBankUI.OnCloseClicked += () => onClosePiggyBank(needShowLevelStart);
            _piggyBankUI.OnCloseClicked += () => onClose?.Invoke();

            _piggyBankUI.OnBuyClicked += onBuyPiggyBank;
            _piggyBankUI.OnBuyClicked += (text) => onClose?.Invoke();
        }

        private void onClosePiggyBank(bool needShowLevelStart)
        {
            if (needShowLevelStart)
            {
                UIManager.Instance.ReleaseUI(_piggyBankUI, true);
                Instance.ReleaseCurrencyHUD();
                //ZTask.TaskManager.DoSecondsAfter(() =>
                //{
                //    onSelectLevel(G.ProfileService.CurrentLevel);
                //}, 0.2f);
            }
            else
            {
                UIManager.Instance.ReleaseUIWithAnimation(_piggyBankUI);
                Instance.ReleaseCurrencyHUD();
            }

        }

        private void onBuyPiggyBank(string productId)
        {
            Instance.OnBuyPiggyBank(productId, new IAPProductMeta { from = "piggy_bank", scenario_id = "home_autooffer" });
            UIManager.Instance.ReleaseUI(_piggyBankUI, true);
            Instance.ReleaseCurrencyHUD();
        }

        private void onSelectLevel(int level)
        {
            if (_levelTargetUI != null)
            {
                UIManager.Instance.ReleaseUI(_levelTargetUI, true);
            }
            _levelTargetUI = UIManager.Instance.ShowUIOnTop<LevelTargetUI>(C.Layer.LevelTarget, 1);
            _levelTargetUI.Setup(level);
            _levelTargetUI.OnStartGame += onLevelTargetStartBtn;
            _levelTargetUI.OnPopupClose += () =>
            {
                closeLevelTargetPopup();
            };
        }

        private void onLevelTargetStartBtn(int level)
        {
            Instance.OnTapStartBtnAtHome(level)
                .Then(() =>
                {
                    closeLevelTargetPopup();
                    Instance.Switch(new AppFlow.AppStateGameplay()
                    {
                        LevelId = level,
                        IsRetry = false
                    }, true);
                })
                .Done();
        }

        private void closeLevelTargetPopup()
        {
            UIManager.Instance.ReleaseUIWithAnimation(_levelTargetUI);
        }
        */
        #endregion



        #region Reward
        /*
        public void ShowPiggyBankReward(bool isRoyalMode, int lastLevelResultStar, LevelData levelData, LevelResultData levelResultData, Action onComplete)
        {
            // _piggyBankRewardUI = UIManager.Instance.ShowUIOnTop<PiggyBankRewardUI>(isRoyalMode ? C.Layer.RoyalLeaguePiggyBankCoinReward : C.Layer.PiggyBankCoinReward, C.CanvasLayer.Mid);

            // CurrencyHUDUI currencyHUD = Instance.ShowCurrencyHUD(G.ProfileService.Coin, C.CanvasLayer.Mid);
            // currencyHUD.OnBackClick += () =>
            // {
            //     onClosePiggyBankRewardPopup(_piggyBankRewardUI, onComplete);
            // };
            onShowPiggyBankRewardPopup(levelData, lastLevelResultStar, levelResultData);
            // setupPiggyBankEvents(onComplete);
            onComplete?.Invoke();
        }

        private void setupPiggyBankEvents(Action onComplete)
        {
            _piggyBankRewardUI.OnTapToSkip += () =>
            {
                onClosePiggyBankRewardPopup(_piggyBankRewardUI, onComplete);
            };

            _piggyBankRewardUI.OnCoinAnimationCompleted += () =>
            {
                onClosePiggyBankRewardPopup(_piggyBankRewardUI, onComplete);
            };
        }

        private void onClosePiggyBankRewardPopup(PiggyBankRewardUI piggyBankReward, Action onComplete)
        {
            UIManager.Instance.ReleaseUI(piggyBankReward, true);
            Instance.ReleaseCurrencyHUD();
            onComplete?.Invoke();
        }

        private void onShowPiggyBankRewardPopup(LevelData levelData, int lastLevelResultStar, LevelResultData levelResultData)
        {
            PiggyBankConfig config = G.PiggyBankLogic.Config;
            PiggyBankModeRewardData modeRewardData = G.PiggyBankLogic.GetModeRewardDataByPickLevel(levelData.PickLevel, config);
            setupPiggyBankReward(modeRewardData, lastLevelResultStar, levelResultData);
        }

        private void setupPiggyBankReward(PiggyBankModeRewardData modeRewardData, int lastLevelResultStar, LevelResultData levelResultData)
        {
            IList<int> coinRewardDatas = modeRewardData.CoinRewardPerStars;

            // setupPiggyCoinReward(lastLevelResultStar, levelResultData, coinRewardDatas);
            if (!G.PiggyBankLogic.IsPiggyBankFull())
            {
                checkPiggyBankReachPurchasableStatus(coinRewardDatas);
                G.PiggyBankLogic.SetupPiggyBankReward(lastLevelResultStar, levelResultData, coinRewardDatas);
            }
        }

        private void setupPiggyCoinReward(int lastLevelResultStar, LevelResultData levelResultData, IList<int> coinRewardDatas)
        {
            _piggyBankRewardUI.Setup(coinRewardDatas, G.PiggyBankLogic.PiggyCoin);
            _piggyBankRewardUI.StartStarAnimation(lastLevelResultStar, levelResultData.Star);
        }

        private void checkPiggyBankReachPurchasableStatus(IList<int> coinRewardDatas)
        {
            int currentPiggyCoin = G.PiggyBankLogic.PiggyCoin;
            int purchasableCoin = G.PiggyBankLogic.GetCurrentPiggyBankPurchasableCoin();

            //Already passed purchasable status.
            if (currentPiggyCoin >= purchasableCoin)
            {
                return;
            }

            trackingPiggyBankPurchasableEvent(coinRewardDatas, currentPiggyCoin, purchasableCoin);
        }

        private void trackingPiggyBankPurchasableEvent(IList<int> coinRewardDatas, int currentPiggyCoin, int purchasableCoin)
        {
            foreach (int coin in coinRewardDatas)
            {
                currentPiggyCoin += coin;
            }

            if (currentPiggyCoin >= purchasableCoin)
            {
                AthenaApp.Instance.AnalyticsManager.LogBIProgression("piggy_bank", G.ProfileService.CurrentLevel, $"piggy{G.PiggyBankLogic.PiggyLevel}", "purchasable");
            }
        }
        */
        #endregion
    }

    [System.Serializable]
    public class ProfileService
    {
        public int CurrentLevel;
        public void AddCoin(int coin)
        {

        }
    }
}