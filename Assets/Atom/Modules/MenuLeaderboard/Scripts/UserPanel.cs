using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Atom
{
    public class UserPanel : MonoBehaviour
    {
        public event Action OnRename;

        public TextMeshProUGUI Rank;
        public TextMeshProUGUI StatValue;
        public TextMeshProUGUI Name;

        [SerializeField] Image _bgRank;
        [SerializeField] Sprite[] _bgRankSprites;
        [SerializeField] GameObject _rankBgObj;
        [SerializeField] TMP_FontAsset _fontAsset;

        public void Setup(int rank, int crowns, string name)
        {
            Rank.text = rank.ToString();
            StatValue.text = crowns.ToString();
            Name.text = name;
        }

        private void onRenameClicked()
        {
            OnRename?.Invoke();
        }
    }
}