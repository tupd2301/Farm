using System;
using System.Collections.Generic;
using Athena.Common.UI;
using CustomUtils;
using Newtonsoft.Json;
using UnityEngine;

namespace Atom
{
    public class FakeLeaderboard : MonoBehaviour
    {
        public FakeLeaderboardData config;
        protected LeaderboardUI _leaderboardUI;

        public LeaderboardUI LeaderboardUI
        {
            get
            {
                return _leaderboardUI;
            }
        }

        public string playerName;


        protected Leaderboard leaderboard;
        protected DateTime _lastUpdateTime;

        TimeSpan timeRemain;

        [Header("Debug")]
        public int hourElapsed;

        private void Start()
        {
            Initialize();
        }

        public void SetPlayerName(string name)
        {
            playerName = name;
        }

        public void Initialize()
        {
            LoadLeaderboard();
            if (leaderboard == null || leaderboard.IsEmpty || leaderboard.IsEmpty)
            {
                CreateLeaderboard();
                SaveLeaderboard();
            }
            else
            {
                UpdateLeaderboard();
            }
            timeRemain = leaderboard.timeExpire - DateTime.Now;
            Show();
        }

        private void CreateLeaderboard()
        {
            leaderboard = GenerateFakeData(config.entriesToShow);
            AddPlayerToLeaderboard();
        }

        public void UpdatePlayerScore(int newScore)
        {
            var playerEntry = leaderboard.BoardData.Find(x => x.Type == LeaderboardEntryType.Player);
            if (playerEntry != null)
            {
                playerEntry.StatValue = newScore;
            }
            leaderboard.Sort();
            leaderboard.AssignRanks();
            //SaveLeaderboard();
        }

        private void Show()
        {
            _leaderboardUI = AppManager.Instance.ShowSafeTopUI<LeaderboardUI>("Atom/LeaderboardUI", false);
            _leaderboardUI.Setup(leaderboard, timeRemain);

            _leaderboardUI.OnCloseClicked += Hide;
            _leaderboardUI.OnInfoClicked += ShowInfo;
            _leaderboardUI.OnPlayClicked += Play;
            _leaderboardUI.OnRename += ShowRenamePopup;
        }

        private void Hide()
        {
            UIManager.Instance.ReleaseUI(_leaderboardUI, true);

            _leaderboardUI.OnCloseClicked -= Hide;
            _leaderboardUI.OnInfoClicked -= ShowInfo;
            _leaderboardUI.OnPlayClicked -= Play;
            _leaderboardUI.OnRename -= ShowRenamePopup;
        }

        private Leaderboard GenerateFakeData(int count)
        {
            var leaderboard = new Leaderboard(config.duration)
            {
                remainEntries = config.remainEntries,
                promoteEntries = config.promoteEntries,
            };
            string[] fakeNames = config.fakeNames.Split("\n");
            System.Random random = new System.Random();

            for (int i = 1; i <= count; i++)
            {
                string name = fakeNames[random.Next(fakeNames.Length)] + random.Next(1, 1000);
                leaderboard.AddBotEntry(name, 0, GenerateBotLevel());

            }

            leaderboard.Sort();
            leaderboard.AssignRanks();

            return leaderboard;
        }

        private float GenerateBotLevel()
        {
            var random = new System.Random();
            var cumulatedRate = 0;
            int index = 0;
            if (config.botLevelRates.Count != config.botLevelRanges.Count)
            {
                Debug.LogError("botLevelRates and botLevelRanges must have the same length");
                return -1;
            }
            foreach (var rate in config.botLevelRates)
            {
                cumulatedRate += rate;
                if (random.Next(0, 100) <= cumulatedRate)
                {
                    index = config.botLevelRates.IndexOf(rate);
                    break;
                } 
            }

            return (float)random.NextDouble() * (config.botLevelRanges[index].y - config.botLevelRanges[index].x) + config.botLevelRanges[index].x;
        }

        private void AddPlayerToLeaderboard()
        {
            leaderboard.AddEntry(0, 0, playerName, LeaderBoardEntryStatus.None, true);
        }

        private void UpdateLeaderboard()
        {
            DateTime currentTime = DateTime.Now;
            
            foreach (var entry in leaderboard.BoardData)
            {
                if (entry.Type == LeaderboardEntryType.Bot)
                {
                    var score = ((DateTime.Now - leaderboard.timeCreated) / config.botUpdateInterval).Hours * config.baseScore * entry.BotLevel * config.difficulty;
                    entry.StatValue = Math.Clamp((int) score, 0, config.highestScore);
                }
            }
            

            _lastUpdateTime = currentTime;

            leaderboard.Sort();
            leaderboard.AssignRanks();
            SaveLeaderboard();
        }

        private void SaveLeaderboard()
        {
            // Save the leaderboard to player prefs
            PlayerPrefs.SetString("Leaderboard", JsonConvert.SerializeObject(leaderboard));
            PlayerPrefs.Save();
        }

        private void LoadLeaderboard()
        {
            Debug.Log(PlayerPrefs.GetString("Leaderboard"));
            leaderboard = JsonConvert.DeserializeObject<Leaderboard>(PlayerPrefs.GetString("Leaderboard"));
        }

        [ContextMenu("Clear Save Data")]
        public void ClearSaveData()
        {
            PlayerPrefs.DeleteKey("Leaderboard");
            PlayerPrefs.Save();
        }

        [ContextMenu("Increase Player Score By One")]
        public void IncreasePlayerScoreByOne()
        {
            var playerEntry = leaderboard.BoardData.Find(x => x.Type == LeaderboardEntryType.Player);
            if (playerEntry != null)
            {
                playerEntry.StatValue += 1;
            }
            leaderboard.Sort();
            leaderboard.AssignRanks();

            SaveLeaderboard();
        }

        [ContextMenu("Update bot based on hourElapsed")]
        public void UpdateLeaderboardTest()
        {   
            foreach (var entry in leaderboard.BoardData)
            {
                if (entry.Type == LeaderboardEntryType.Bot)
                {
                    var score = hourElapsed * config.baseScore * entry.BotLevel * config.difficulty;
                    entry.StatValue = (int) score;
                }
            }
            

            leaderboard.Sort();
            leaderboard.AssignRanks();
            SaveLeaderboard();
        }

        public void ShowInfo()
        {

        }

        public void ShowRenamePopup()
        {

        }

        public void Play()
        {

        }
    }
}
