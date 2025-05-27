using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CustomUtils;
using Athena.GameOps;

namespace Atom
{
    public class TestMainManager : SingletonMono<TestMainManager>
    {
        // Start is called before the first frame update
        void Start()
        {
            Application.targetFrameRate = 60;
            DontDestroyOnLoad(gameObject);

            //GameObject audioManagerObject = GameObject.Instantiate(Resources.Load<GameObject>("AudioManager"));
            //audioManagerObject.name = "AudioManager";

            GameModeContainer.Instance.InitGame();

            CreateMenuAtom();
        }

        public void CreateMainMenu()
        {
            GameObject mainUIObject = GameObject.Instantiate(Resources.Load<GameObject>("MainUI"), GameModeContainer.Instance.Manager.transform);
            mainUIObject.name = "UI";
            //MainUIManager.Instance.ShowMainMenu();
            //SplashManager.Instance.Show();
        }

        public void CreateMenuAtom()
        {
            CreateModule("Atom/AudioManager", "AudioManager");
            CreateModule("PortraitLoginManager", "LoginManager");
            CreateModule("Atom/MainMenuManager", "MainMenuManager");
            CreateModule("Atom/DailyChallengeManager", "DailyChallengeManager");
            CreateModule("Atom/SelectLevelManager", "SelectLevelManager");
            CreateModule("Atom/RatingManager", "RatingManager");
            CreateModule("Atom/SettingManager", "SettingManager");
            CreateModule("Atom/LeaderboardManager", "LeaderboardManager");
            CreateModule("Atom/ShopManager", "ShopManager");
            CreateModule("Atom/DailyFreeCoinManager", "DailyFreeCoinManager");
            CreateModule("Atom/PiggyBankManager", "PiggyBankManager");
            //Show Menu
            SelectLevelManager.Instance.Setup("Atom/SelectLevelUI", "QuestStory", (levels) => {
                UnityEngine.Debug.Log("level : " + levels[0]);
                RatingManager.Instance.ShowFeedbackThankUI("Level", "Play Level " + levels[0].ToString(), "OK");
            });

            DailyChallengeManager.Instance.Setup((level, showVideoAds) => {
                UnityEngine.Debug.Log("level : " + level + " : " + showVideoAds);
                RatingManager.Instance.ShowFeedbackThankUI("Level", "Play Level " + level, "OK");
            });
            var now = System.DateTime.Today;
            DailyChallengeManager.Instance.DailyChallengeUI.Setup(new System.DateTime(now.Year, now.Month, now.Day), null);
            DailyChallengeManager.Instance.DailyChallengeUI.ProgressBarUI.UpdateBadgeIcon(5, new System.DateTime(now.Year, now.Month, 1));

            LeaderboardManager.Instance.Setup();

            ShopManager.Instance.InitializeIAP();
            ShopManager.Instance.ShowMoreCoinUI();

            MainMenuManager.Instance.ShowMainMenu();

            AudioManager.Instance.PlayBackgroundMusic("MainBG");

            StartCoroutine(loopGetLeaderboard());
        }

        private void CreateModule(string module, string nameModule)
        {
            GameObject loginObject = GameObject.Instantiate(Resources.Load<GameObject>(module), GameModeContainer.Instance.Manager.transform);
            loginObject.name = nameModule;
        }

        private IEnumerator loopGetLeaderboard()
        {
            yield return new WaitForSeconds(2);
            LeaderboardManager.Instance.UpdateLeaderboard(100);
            LeaderboardManager.Instance.GetLeaderboard();
            ShowAds();
        }

        private void ShowAds()
        {
            AthenaApp.Instance.AdManager.RequestBanner();
        }
    }
}