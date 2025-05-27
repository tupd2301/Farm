using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Atom
{
    public class LeaderboardItemPanel : MonoBehaviour
    {
        public TextMeshProUGUI Rank;
        public TextMeshProUGUI StatValue;
        public TextMeshProUGUI Name;

        [Header("Rewards")]
        public GameObject reward;
        public TextMeshProUGUI rewardText;

        public void Setup(int rank, int crowns, string name)
        {
            Rank.text = rank.ToString();
            StatValue.text = crowns.ToString();
            Name.text = name;

            if (rank > 3)
            {
                reward.SetActive(false);
            }
        }
    }
}