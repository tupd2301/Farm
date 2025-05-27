using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Atom
{
    public class PiggyBankLogic
    {
        const string ProfileKey = "pf_piggy_bank_profile";
        public const string PiggyBankRemindStateKey = "pf_piggybank_remind";

        private Profile _profile;

        public int PiggyCoin { get { return _profile.PiggyCoin; } }
        public int PiggyLevel { get { return _profile.PiggyLevel > 0 ? _profile.PiggyLevel : 1; } }
        public int LastCoinAdd => _profile.LastAdd;
        public int LastStarAdd => _profile.LastStarAdd;

        PiggyBankConfig _config;
        public PiggyBankConfig Config { get => _config; }

        public void CheckMirgrateProfileIfAny(int piggyCoin, int piggyLevel)
        {
            if (!PlayerPrefs.HasKey(ProfileKey))
            {
                var profile = SaveDataHelper.GetJSONData(ProfileKey, new Profile()
                {
                    PiggyCoin = piggyCoin,
                    PiggyLevel = piggyLevel,
                    HasMigrated = true
                });
                SaveDataHelper.SetJSONData(ProfileKey, profile);
            }
        }

        public void Init(PiggyBankConfig piggyBankConfig)
        {
            _config = piggyBankConfig;
            loadProfile();
        }

        public void AddPiggyCoin(int amount)
        {
            _profile.PiggyCoin += amount;
            _profile.LastAdd = amount;
            save();
        }

        public void SetPiggyCoin(int amount)
        {
            _profile.PiggyCoin = amount;
            save();
        }

        public void SetPiggyLevel(int amount)
        {
            _profile.PiggyLevel = amount;
            save();
        }

        private void save()
        {
            SaveDataHelper.SetJSONData(ProfileKey, _profile);
        }

        private void loadProfile()
        {
            _profile = SaveDataHelper.GetJSONData(ProfileKey, new Profile()
            {
                PiggyCoin = 0,
                PiggyLevel = 0,
                HasMigrated = false
            });
            if (!PlayerPrefs.HasKey(ProfileKey))
            {
                save();
            }
        }

        #region Payout
        public void AddPiggyCoinIntoUser()
        {
            int coin = PiggyCoin;
            PiggyBankManager.Instance.ProfileService.AddCoin(coin);
            SetPiggyCoin(0);
        }

        public void UpdateProfilePiggyLevel()
        {
            int level = PiggyLevel + 1;
            if (level > _config.Levels.Count)
            {
                SetPiggyLevel(_config.Levels.Count);
                return;
            }
            SetPiggyLevel(level);
        }

        public bool IsContainCoinReward(IList<RewardData> rewardDatas)
        {
            foreach (RewardData rewardData in rewardDatas)
            {
                if (rewardData.Type == RewardData.RewardType.Currency)
                {
                    return true;
                }
            }
            return false;
        }
        #endregion


        public void ClearLastValue()
        {
            _profile.LastAdd = 0;
            _profile.LastStarAdd = 0;
        }

        #region Remind pop up
        //Check condition
        public bool ShouldShowPiggyBank()
        {
            bool hasUnlocked = PiggyBankManager.Instance.ProfileService.CurrentLevel >= _config.UnlockLevel;

            int piggyLevelIndex = PiggyLevel - 1;
            PiggyBankLevelData piggyData = _config.Levels[piggyLevelIndex];

            return hasUnlocked && (isPiggyBankReachPurchasablePopup(piggyData) || isPiggyBankReachMaxCoin(piggyData) || IsPiggyBankForceToHome(piggyData));
        }

        public bool IsPiggyBankForceToHome(PiggyBankLevelData piggyBankData)
        {
            bool isRemindPiggyBank = SaveDataHelper.Get(PiggyBankRemindStateKey, false);
            return PiggyCoin < piggyBankData.PurchasablePercent * piggyBankData.MaxCoin / 100 &&
                PiggyBankManager.Instance.ProfileService.CurrentLevel - 1 == _config.UnlockLevel &&
                isRemindPiggyBank;
        }

        private bool isPiggyBankReachPurchasablePopup(PiggyBankLevelData piggyData)
        {
            bool isRemindPiggyBank = SaveDataHelper.Get(PiggyBankRemindStateKey, false);
            return PiggyCoin >= piggyData.PurchasablePercent * piggyData.MaxCoin / 100 &&
                PiggyCoin < piggyData.MaxCoin &&
                isRemindPiggyBank;
        }

        private bool isPiggyBankReachMaxCoin(PiggyBankLevelData piggyData)
        {
            bool isRemindPiggyBank = SaveDataHelper.Get(PiggyBankRemindStateKey, false);
            return PiggyCoin >= piggyData.MaxCoin &&
                isRemindPiggyBank;
        }
        #endregion



        #region Reward
        public void SetupPiggyBankReward(int lastLevelResultStar, LevelResultData levelResultData, IList<int> coinRewardDatas)
        {
            int index = PiggyLevel - 1;

            int totalCoinRewardAmount = 0;
            foreach (int coinReward in coinRewardDatas)
            {
                totalCoinRewardAmount += coinReward;
            }

            PiggyBankLevelData piggyBankLevelData = _config.Levels[index];

            if (isPiggyBankReachPurchasable(piggyBankLevelData, totalCoinRewardAmount) ||
                isPiggyBankReachMaxCoin(piggyBankLevelData, totalCoinRewardAmount))
            {
                SaveDataHelper.Set(PiggyBankRemindStateKey, true);
            }

            addRewardToPiggyCoin(lastLevelResultStar, levelResultData, coinRewardDatas);
        }

        private bool isPiggyBankReachPurchasable(PiggyBankLevelData piggyBankLevelData, int totalCoinRewardAmount)
        {
            int purchasableAmount = piggyBankLevelData.PurchasablePercent * piggyBankLevelData.MaxCoin / 100;
            int piggyBankCoinRewardAmount = PiggyCoin + totalCoinRewardAmount;
            return PiggyCoin < purchasableAmount && piggyBankCoinRewardAmount >= purchasableAmount;
        }

        private bool isPiggyBankReachMaxCoin(PiggyBankLevelData piggyBankLevelData, int totalCoinRewardAmount)
        {
            int piggyBankCoinRewardAmount = PiggyCoin + totalCoinRewardAmount;
            return PiggyCoin < piggyBankLevelData.MaxCoin && piggyBankCoinRewardAmount >= piggyBankLevelData.MaxCoin;
        }

        public bool IsPiggyBankFull()
        {
            int currentLevelMaxCoin = getCurrentPiggyBankLevelCoin();
            return PiggyCoin >= currentLevelMaxCoin ? true : false;
        }

        private void addRewardToPiggyCoin(int lastLevelResultStar, LevelResultData levelResultData, IList<int> coinRewardDatas)
        {
            int reward = 0;
            _profile.LastStarAdd = 0;
            for (int i = lastLevelResultStar; i < levelResultData.Star; i++)
            {
                reward += coinRewardDatas[i];
                _profile.LastStarAdd++;
            }
            int currentLevelMaxCoin = getCurrentPiggyBankLevelCoin();
            int remain = currentLevelMaxCoin - PiggyCoin;
            if (reward > remain)
            {
                reward = remain;
            }
            AddPiggyCoin(reward);
        }

        public int GetCurrentPiggyBankPurchasableCoin()
        {
            int levelIndex = PiggyLevel - 1;
            int currentLevelMaxCoin = _config.Levels[levelIndex].MaxCoin;
            int purchasablePercent = _config.Levels[levelIndex].PurchasablePercent;
            return currentLevelMaxCoin * purchasablePercent / 100;
        }

        private int getCurrentPiggyBankLevelCoin()
        {
            int levelIndex = PiggyLevel - 1;
            int currentLevelMaxCoin = _config.Levels[levelIndex].MaxCoin;
            return currentLevelMaxCoin;
        }

        public PiggyBankModeRewardData GetModeRewardDataByPickLevel(int pickLevel, PiggyBankConfig config)
        {
            foreach (PiggyBankModeRewardData piggyBankModeRewardData in config.Modes)
            {
                switch (pickLevel)
                {
                    case 1:
                    default:
                        if (piggyBankModeRewardData.ModeId == "easy")
                        {
                            return piggyBankModeRewardData;
                        }
                        break;
                    case 2:
                        if (piggyBankModeRewardData.ModeId == "hard")
                        {
                            return piggyBankModeRewardData;
                        }
                        break;
                }
            }
            return null;
        }
        #endregion



        public class Profile
        {
            public int PiggyCoin;
            public int PiggyLevel;
            public int LastAdd = 0;
            public int LastStarAdd = 0;

            public bool HasMigrated;
        }
    }
}
