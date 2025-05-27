using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using CustomUtils;
using Athena.Common.UI;

namespace Atom
{
    public class MainMenuManager : SingletonMono<MainMenuManager>
    {
        private MainMenuUI _mainMenuUI;
        private TopUI _topUI;
        private DownContainerUI _downContainerUI;

        public void ShowMainMenu()
        {
            _mainMenuUI = AppManager.Instance.ShowSafeTopUI<MainMenuUI>("Atom/MainMenuUI", false);
            _mainMenuUI.OnOpenSetting = () =>
            {
                AudioManager.Instance.PlaySfxTapButton();
                SettingManager.Instance.ShowUI();
            };
            _mainMenuUI.OnOpenRating = () =>
            {
                AudioManager.Instance.PlaySfxTapButton();
                RatingManager.Instance.ShowUI();
            };
            _mainMenuUI.OnOpenMessages = () =>
            {
                AudioManager.Instance.PlaySfxTapButton();
                DailyFreeCoinManager.Instance.ShowUI(); //PiggyBankManager.Instance.ShowPiggyBankLevelUp(1, 2);
            };
            //_topUI = AppManager.Instance.ShowSafeTopUI<TopUI>("Atom/TopUI", false);
            _downContainerUI = AppManager.Instance.ShowSafeTopUI<DownContainerUI>("Atom/DownContainerUI", false);
            _downContainerUI.InitFloatingUI(new List<UIController>() { SelectLevelManager.Instance.StoryLevelUI, LeaderboardManager.Instance.LeaderboardUI,  _mainMenuUI, DailyChallengeManager.Instance.DailyChallengeUI, ShopManager.Instance.ShopUI });
        }

        private void UpdateCoin()
        {
            
        }

        private void PlayLevel()
        {
            Athena.Common.UI.UIManager.Instance.ReleaseUI(_mainMenuUI, true);
            Athena.Common.UI.UIManager.Instance.ReleaseUI(_topUI, true);
            Athena.Common.UI.UIManager.Instance.ReleaseUI(_topUI, true);
            Athena.Common.UI.UIManager.Instance.ReleaseUI(_downContainerUI, true);
        }
    }
}