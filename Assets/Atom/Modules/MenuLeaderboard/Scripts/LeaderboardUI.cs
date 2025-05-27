using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Athena.Common.UI;
using System;
using TMPro;

namespace Atom
{
    public class LeaderboardUI : UIController
    {
        public System.Action OnCloseClicked;
        public System.Action OnInfoClicked;
        public System.Action OnPlayClicked;
        public System.Action OnRename;

        public GameObject itemPrefab;
        public GameObject promotionPrefab;
        public GameObject demotionPrefab;
        public Transform Content;

        public UserPanel UserPanel;

        public TextMeshProUGUI TimeRemain;

        public void onCloseClicked()
        {
            OnCloseClicked?.Invoke();
        }

        public void onInfoClicked()
        {
            OnInfoClicked?.Invoke();
        }

        public void onPlayClicked()
        {
            OnPlayClicked?.Invoke();
        }

        public void onRenameClicked()
        {
            OnRename?.Invoke();
        }

        public void Setup(string userName, int userRank, int userStatValue, TimeSpan remain, List<LeaderboardEntry> itemsData)
        {
            //TimeRemain.Setup(remain);
            SetupLeaderboardItems(itemsData);
            //PlayBtn.TextMesh.text = "ROUND " + (G.RoyalLeagueLogic.CurrentRound + 1).ToString();
            UserPanel.Setup(userRank, userStatValue, userName);
        }

        public void Setup(Leaderboard leaderboard, TimeSpan remain)
        {
            TimeRemain.text = remain.FormatTimeSpan();//.ToString("dd' D:'hh'H:'mm'M'");
            SetupLeaderboardItems(leaderboard);
            var playerEntry = leaderboard.GetPlayerEntry();
            UserPanel.Setup(playerEntry.Rank, playerEntry.StatValue, playerEntry.Name);
        }

        private void SetupLeaderboardItems(List<LeaderboardEntry> itemsData)
        {
            foreach (var itemData in itemsData)
            {
                GameObject itemGameObject = GameObject.Instantiate(itemPrefab, Content);
                itemGameObject.SetActive(true);
                itemGameObject.GetComponent<LeaderboardItemPanel>().Setup(itemData.Rank, itemData.StatValue, itemData.Name);
                /*
                var item = addItem();
                var rw = G.RoyalLeagueLogic.GetRewardAtRank(itemData.Rank);
                item.Setup(itemData, rw);
                if (itemData.Name == G.RoyalLeagueLogic.Name)
                {
                    _cacheUserPanel = item;
                }
                */
            }
        }

        private void SetupLeaderboardItems(Leaderboard leaderboard)
        {
            for (int i = 0; i < leaderboard.BoardData.Count; i++)
            {
                var itemData = leaderboard.BoardData[i];
                GameObject itemGameObject = GameObject.Instantiate(itemPrefab, Content);
                itemGameObject.SetActive(true);
                itemGameObject.GetComponent<LeaderboardItemPanel>().Setup(itemData.Rank, itemData.StatValue, itemData.Name);
                if (i == leaderboard.promoteEntries - 1)
                {
                    GameObject promotionGameObject = GameObject.Instantiate(promotionPrefab, Content);
                    promotionGameObject.SetActive(true);
                }
                else if (i == leaderboard.BoardData.Count - leaderboard.demoteEntries - 1)
                {
                    GameObject demotionGameObject = GameObject.Instantiate(demotionPrefab, Content);
                    demotionGameObject.SetActive(true);
                }
            }
        }
    }
}
