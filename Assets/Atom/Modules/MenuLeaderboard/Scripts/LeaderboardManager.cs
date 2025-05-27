using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CustomUtils;
using Athena.Common.UI;
using PlayFab;
using PlayFab.ClientModels;
using System;

namespace Atom
{
    public class LeaderboardManager : SingletonMono<LeaderboardManager>
    {
        protected LeaderboardUI _royalLeagueLeaderboardUI;

        public LeaderboardUI LeaderboardUI
        {
            get
            {
                return _royalLeagueLeaderboardUI;
            }
        }

        public void Setup()
        {
            _royalLeagueLeaderboardUI = AppManager.Instance.ShowSafeTopUI<LeaderboardUI>("Atom/LeaderboardUI", false);

            _royalLeagueLeaderboardUI.OnCloseClicked += () =>
            {
                UIManager.Instance.ReleaseUI(_royalLeagueLeaderboardUI, true);
            };
            _royalLeagueLeaderboardUI.OnInfoClicked += () =>
            {
                //StartCoroutine(ShowRoyalLeagueTutorialPopupCor());
            };
            _royalLeagueLeaderboardUI.OnPlayClicked += () =>
            {
                /*
                if (needLogTutorialEnd)
                {
                    AppManager.Instance.AthenaApp.AnalyticsManager.LogBIProgression("tutorial_end", G.ProfileService.CurrentLevel, "tut_league", "tap_Round1");
                }

                AppManager.Instance.Switch(new AppStateGameplay()
                {
                    LevelId = G.RoyalLeagueLogic.CurrentRoyalLevel,
                    IsRetry = false,
                    IsRoyalMode = true
                }, true);
                */
            };
            _royalLeagueLeaderboardUI.OnRename += () =>
            {
                /*
                ShowRoyalLeagueRenamePopup(false, (success) =>
                {
                    if (success)
                    {
                        _royalLeagueLeaderboardUI.Refresh();
                    }
                });
                */
            };
        }

        public void GetLeaderboard()
        {
            GetListLeaderboardItem((user, list) =>
            {
                _royalLeagueLeaderboardUI.Setup(user.Name, user.Rank, user.StatValue, new System.TimeSpan(1, 1, 1), list);
            });
        }

        public void UpdateLeaderboard(int statValue)
        {
            PlayFabClientAPI.UpdatePlayerStatistics(new UpdatePlayerStatisticsRequest()
            {
                Statistics = new List<StatisticUpdate> {
                    new StatisticUpdate {
                        StatisticName = "Points",
                        Value = statValue
                    }
                }
            }, OnStatisticsUpdated, FailureCallback);
        }

        private void GetListLeaderboardItem(System.Action<LeaderboardEntry, List<LeaderboardEntry>> action)
        {
            List<LeaderboardEntry> leaderboardItemDatas = new List<LeaderboardEntry>();
            LeaderboardEntry user = null;
            PlayFabClientAPI.GetLeaderboardAroundPlayer(new GetLeaderboardAroundPlayerRequest
            {
                StatisticName = "Points",
                MaxResultsCount = 1
            }, (result) => {
                if (result.Leaderboard.Count > 0)
                {
                    PlayerLeaderboardEntry playerData = result.Leaderboard[0];
                    user = new LeaderboardEntry() { Rank = playerData.Position, StatValue = playerData.StatValue, Name = playerData.DisplayName };
                }
                //-----------------------
                PlayFabClientAPI.GetLeaderboard(new GetLeaderboardRequest
                {
                    StatisticName = "Points",
                    MaxResultsCount = 10,
                }, (result) => {
                    for (int i = 0; i < result.Leaderboard.Count; i++)
                    {
                        PlayerLeaderboardEntry playerData = result.Leaderboard[i];
                        leaderboardItemDatas.Add(new LeaderboardEntry() { Rank = playerData.Position, StatValue = playerData.StatValue, Name = playerData.DisplayName });
                    }
                    action.Invoke(user, leaderboardItemDatas);
                }, FailureCallback);
            }, FailureCallback);
        }

        private void OnStatisticsUpdated(UpdatePlayerStatisticsResult updateResult)
        {
            Debug.Log("Successfully submitted high score");
        }

        private void FailureCallback(PlayFabError error)
        {
            Debug.LogWarning("Something went wrong with your API call. Here's some debug information:");
            Debug.LogError(error.GenerateErrorReport());
        }
    }

    [Serializable]
    public class LeaderboardEntry
    {
        public int Rank;
        public int StatValue;
        public string Name;
        public float BotLevel;
        public LeaderboardEntryType Type = LeaderboardEntryType.None;
        public LeaderBoardEntryStatus Status = LeaderBoardEntryStatus.None;
    }

    public enum LeaderboardEntryType
    {
        Player,
        Bot,
        None
    }

    public enum LeaderBoardEntryStatus
    {
        None,
        Promote,
        Remain,
        Demote
    }
}